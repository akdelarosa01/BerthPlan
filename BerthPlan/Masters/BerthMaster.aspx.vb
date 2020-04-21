#Region "## インポート ##"
Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.IO
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.HiddenField
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports System.Reflection
Imports BerthPlan.GlobalFunction
Imports ClosedXML.Excel

#End Region

''' <summary>
''' バースマスター
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者-----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　 デザインを作った。
'''    00.02      2020/03/13      KD.Ga            コーディングを開始しました。
'''    00.02      2020/03/17      AK.Dela Rosa     データテーブルを修正した。
''' </history>
Public Class BerthMaster
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
    ''' <summary>EXCEL</summary>
    Const C_Excel_File = "バースマスター.xlsx"
    ''' <summary>Excel Name</summary>
    Const C_Excel_Name = "バースマスター"
#End Region

#Region "## クラス内変数 ## "
    ''' <summary>Berth Plan Database</summary>
    Private Shared _db As BerthPlanEntities = New BerthPlanEntities

    Private Shared aTimeStamp As Object()
#End Region

#Region "## コントロールイベント定義 ##"
    ''' <summary>
    ''' ページ読み込み機能
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Call fgCheckSession()

        If IsNothing(Session("UserID")) Then
            Response.Redirect("~/Login.aspx?SessionExpire")
            Exit Sub
        End If
    End Sub

#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' GetBerth List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetBerthList() As Object
        flGetBerthList = Nothing

        Try
            flGetBerthList = (From x In _db.mBerth.AsNoTracking.ToList()
                              Where x.Flag = False
                                Order By x.UpdTime Descending
                                        Select New With {
                                        .BerthID = x.BerthID,
                                        .BerthCD = x.BerthCD,
                                        .BerthName = x.BerthName,
                                        .WharfCD = x.WharfCD,
                                        .WharfName = fgNullToStr((From w In _db.mWharf.AsNoTracking.Where(Function(c) c.WharfCD = x.WharfCD _
                                                                                                              And c.Flag = False) Select w.WharfName).FirstOrDefault),
                                        .UpdTime = x.UpdTime
                                    }).ToList()

        Catch ex As Exception
            Throw
        End Try

        Return flGetBerthList
    End Function

    ''' <summary>
    ''' 保存機能
    ''' </summary>
    ''' <param name="pBerthInfo"></param>
    ''' <param name="pIsChanged"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flUpdData(ByVal pBerthInfo As mBerth, ByVal pIsChanged As Boolean) As MyResult
        Dim oBerth As mBerth = New mBerth
        Dim sWharfCD As String = String.Empty
        Dim sBerthCD As String = String.Empty

        Try
            flUpdData = New MyResult
            flUpdData.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flUpdData.Status = C_Flag.CodeO
                Exit Function
            End If

            'Initialize Berth and Wharf Code
            sWharfCD = fgNullToStr(pBerthInfo.WharfCD.ToUpper)
            sBerthCD = fgNullToStr(pBerthInfo.BerthCD.ToUpper)

            'Check WharfCode
            Dim iWharf = (From w In _db.mWharf.AsNoTracking()
                          Where w.WharfCD.ToUpper = sWharfCD _
                                  And w.Flag = False
                          Select w)
            If iWharf.Count < 1 Then
                flUpdData.Msg = fgMsgOut("EBP002", "", "ワーフコード")
                Exit Function
            End If

            'Check Wharf With Berth Code
            If pIsChanged = True Then
                Dim iBerth = From sBerth In _db.mBerth.ToList()
                             Where Trim(sBerth.WharfCD.ToUpper()) = sWharfCD _
                                 And Trim(sBerth.BerthCD.ToUpper()) = sBerthCD _
                                 And sBerth.Flag = False
                             Select sBerth
                If iBerth.Count >= 1 Then
                    flUpdData.Msg = fgMsgOut("EBP001", "", "岸壁コードとバース")
                    Exit Function
                End If
            End If

            'Save/Update Data 
            If pBerthInfo.BerthID = 0 Then
                If flInsert(pBerthInfo) = False Then
                    Exit Function
                End If
                flUpdData.Msg = fgMsgOut("IBP001", "")

            Else
                If flUpdate(pBerthInfo) = False Then
                    flUpdData.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If
                flUpdData.Msg = fgMsgOut("IBP002", "", "バース")

            End If

            flUpdData.Status = C_Flag.CodeS

        Catch ex As Exception
            flUpdData = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flDelBerth(ByVal lBerth As List(Of mBerth)) As MyResult
        Dim mBerth As IQueryable(Of mBerth) = Nothing

        Try
            flDelBerth = New MyResult
            flDelBerth.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flDelBerth.Status = C_Flag.CodeO
                Exit Function
            End If

            'Check UpdTime if the same
            For Each sLine As mBerth In lBerth
                If flCheckUpdDate(sLine.UpdTime, (From x In _db.mBerth.AsNoTracking _
                                                    Where x.BerthID = sLine.BerthID _
                                                   Select x.UpdTime).FirstOrDefault) = False Then
                    flDelBerth.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If
            Next

            'Delete Berth
            For Each sBerth As mBerth In lBerth
                Dim sData = _db.mBerth.Where(Function(x) x.BerthID = sBerth.BerthID).FirstOrDefault
                sData.UpdTime = DateTime.Now
                sData.Flag = True

                _db.mBerth.Attach(sData)
                _db.Entry(sData).State = EntityState.Modified
            Next
            If _db.SaveChanges() < 1 Then
                flDelBerth.Msg = fgMsgOut("EBP004", "")
                Exit Function
            End If

            flDelBerth.Status = C_Flag.CodeS
            flDelBerth.Msg = fgMsgOut("IBP003", "")

        Catch ex As Exception
            flDelBerth = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Print Berth Master
    ''' </summary>
    ''' <param name="lBerth"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flPrint(ByVal lBerth As List(Of mBerth)) As MyResult
        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing
        Dim sReturnData = New String() {"", ""}

        Dim iCount As Integer = 0
        Dim iRow As Integer = 4

        Dim sOpenFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/Templates/")
        Dim sOpenFileName As String = C_Excel_File

        ' ダウンロードファイル
        Dim sSaveFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/SaveFile/")
        Dim sSaveFileName As String = String.Empty

        Try
            flPrint = New MyResult
            flPrint.Status = C_Flag.CodeF

            objWorkBook = New XLWorkbook(sOpenFilePath & sOpenFileName)

            sSaveFileName = C_Excel_Name & Date.Now.ToString("yyyyMMddHHmmssfff") & ".xlsx"
            objWorkSheet = objWorkBook.Worksheet(1)

            With objWorkSheet.Range("A4", "D" & (lBerth.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each row In lBerth
                    .Cell(iRow, 1).Value = row.WharfCD
                    .Cell(iRow, 2).Value = (From x In _db.mWharf _
                                                Where x.WharfCD = row.WharfCD _
                                                And x.Flag = False
                                            Select x.WharfName).FirstOrDefault
                    .Cell(iRow, 3).Value = row.BerthCD
                    .Cell(iRow, 4).Value = row.BerthName
                    iRow += 1
                Next
            End With

            'ワークブックを保存する
            objWorkBook.SaveAs(sSaveFilePath & sSaveFileName)
            '後処理
            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If

            ' ダウンロードしたファイルをサーバーから削除する
            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If

            'ファイルダウンロード
            sReturnData = {sSaveFilePath, sSaveFileName}

            flPrint.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            flPrint.Data = sReturnData
            flPrint.Status = C_Flag.CodeS

        Catch ex As Exception
            flPrint = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Insert New Berth
    ''' </summary>
    ''' <param name="pBerth"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flInsert(ByVal pBerth As mBerth) As Boolean
        Dim Auth As Authentication = New Authentication()
        Dim oBerth As mBerth = New mBerth

        Try
            flInsert = False

            oBerth.WharfCD = fgNullToStr(pBerth.WharfCD)
            oBerth.BerthCD = fgNullToStr(pBerth.BerthCD)
            oBerth.BerthName = fgNullToStr(pBerth.BerthName)

            oBerth.UpdTime = DateTime.Now
            oBerth.UpdUserID = Auth.userID
            oBerth.UpdPGID = C_PGID.BerthMaster
            oBerth.Flag = False

            _db.mBerth.Add(oBerth)
            _db.SaveChanges()

            flInsert = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Update Existing Berth
    ''' </summary>
    ''' <param name="pBerth"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flUpdate(ByVal pBerth As mBerth) As Boolean
        Dim Auth As Authentication = New Authentication()

        Try
            flUpdate = False

            Dim getRow = _db.mBerth.Where(Function(x) x.BerthID = pBerth.BerthID).FirstOrDefault()

            'Check UpdTime if the same
            If flCheckUpdDate(pBerth.UpdTime, (From x In _db.mBerth.AsNoTracking _
                                                Where x.BerthID = pBerth.BerthID _
                                               Select x.UpdTime).FirstOrDefault) = False Then
                Exit Function
            End If

            getRow.WharfCD = fgNullToStr(pBerth.WharfCD)
            getRow.BerthCD = fgNullToStr(pBerth.BerthCD)
            getRow.BerthName = fgNullToStr(pBerth.BerthName)

            getRow.UpdTime = DateTime.Now
            getRow.UpdUserID = Auth.userID
            getRow.UpdPGID = C_PGID.BerthMaster
            getRow.Flag = False

            _db.mBerth.Attach(getRow)
            _db.Entry(getRow).State = EntityState.Modified
            _db.SaveChanges()

            flUpdate = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete BerthName (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fgBerthName(BerthCD As String) As mBerth
        fgBerthName = Nothing

        Try
            fgBerthName = _db.mBerth.AsNoTracking.Where(Function(b) b.BerthCD = BerthCD And b.Flag = False).FirstOrDefault()

        Catch ex As Exception
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete Berth (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fgBerth(BerthID As Integer) As mBerth
        fgBerth = Nothing

        Try
            fgBerth = _db.mBerth.Where(Function(b) b.BerthID = BerthID And b.Flag = False).FirstOrDefault()

        Catch ex As Exception
        End Try
    End Function


#End Region

End Class