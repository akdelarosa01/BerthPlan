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
''' 水先人マスター
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者-----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　 デザインを作った。
'''    00.02      2020/03/13      KD.Ga            コーディングを開始しました。
'''    00.02      2020/03/17      AK.Dela Rosa     データテーブルを修正した。
''' </history>
Public Class PilotMaster
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
    ''' <summary>EXCEL</summary>
    Const C_Excel_File = "水先人マスター.xlsx"
    ''' <summary>Excel Name</summary>
    Const C_Excel_Name = "水先人マスター"
#End Region

#Region "## クラス内変数 ## "
    ''' <summary>Berth Plan Database</summary>
    Private Shared _db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities
#End Region

#Region "## コントロールイベント定義 ##"
    ''' <summary>
    ''' PageLoad
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("UserID")) Then
            Response.Redirect("~/Login.aspx?SessionExpire")
            Exit Sub
        End If
        Call fgCheckSession()
    End Sub

#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' GetPilot List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetPilotList() As Object
        flGetPilotList = Nothing

        Try
            flGetPilotList = (From lPilot In _db.mPilot.AsNoTracking
                                Order By lPilot.UpdTime Descending
                                Where lPilot.Flag = False
                                Select lPilot).ToList()

        Catch ex As Exception
            Throw
        End Try

        Return flGetPilotList
    End Function

    ''' <summary>
    ''' データテーブルのソートおよび順序付け関数
    ''' </summary>
    ''' <param name="order"></param>
    ''' <param name="orderDir"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function flSortByColumnWithOrder(ByVal order As String, ByVal orderDir As String, data As List(Of BerthPlan.mPilot)) As List(Of BerthPlan.mPilot)
        flSortByColumnWithOrder = Nothing

        Try
            If IsNothing(orderDir) Then
                orderDir = String.Empty
            End If

            Select Case order
                Case "0"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(b) b.ID).ToList(),
                                               data.OrderBy(Function(b) b.ID).ToList())
                Case "1"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(b) b.PilotCD).ToList(),
                                               data.OrderBy(Function(b) b.PilotCD).ToList())
                Case "2"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(b) b.PilotName).ToList(),
                                               data.OrderBy(Function(b) b.PilotName).ToList())
                Case "3"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(b) b.UpdTime).ToList(),
                                               data.OrderBy(Function(b) b.UpdTime).ToList())
            End Select

        Catch ex As Exception
            Throw
        End Try

        Return flSortByColumnWithOrder
    End Function

    ''' <summary>
    ''' Delete Pilot
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flDelPilot(ByVal lPilot As List(Of BerthPlan.mPilot)) As MyResult
        Dim mPilot As IQueryable(Of BerthPlan.mPilot) = Nothing

        Try
            flDelPilot = New MyResult
            flDelPilot.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flDelPilot.Status = C_Flag.CodeO
                Exit Function
            End If

            'Check UpdTime if the same
            For Each sLine As BerthPlan.mPilot In lPilot
                If flCheckUpdDate(sLine.UpdTime, (From x In _db.mPilot.AsNoTracking
                                                  Where x.ID = sLine.ID
                                                  Select x.UpdTime).FirstOrDefault) = False Then
                    flDelPilot.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If
            Next

            'Delete Pilot
            For Each sRow As BerthPlan.mPilot In lPilot
                Dim sData = _db.mPilot.Where(Function(x) x.ID = sRow.ID).FirstOrDefault
                sData.UpdTime = DateTime.Now
                sData.Flag = True

                _db.mPilot.Attach(sData)
                _db.Entry(sData).State = EntityState.Modified
            Next
            If _db.SaveChanges() < 1 Then
                flDelPilot.Msg = fgMsgOut("EBP004", "")
                Exit Function
            End If

            flDelPilot.Status = C_Flag.CodeS
            flDelPilot.Msg = fgMsgOut("IBP003", "")

        Catch ex As Exception
            flDelPilot = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    '''  Save/Update Pilot
    ''' </summary>
    ''' <param name="pPilotInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flUpdData(ByVal pPilotInfo As BerthPlan.mPilot) As MyResult
        Dim oPilot As BerthPlan.mPilot = New BerthPlan.mPilot
        Dim sPilotCD As String = String.Empty
        Dim iID As Integer = 0

        Try
            flUpdData = New MyResult
            flUpdData.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flUpdData.Status = C_Flag.CodeO
                Exit Function
            End If

            'メールをチェックする
            If fgCheckEmail(pPilotInfo.Email) = False Then
                flUpdData.Msg = fgMsgOut("EBP008", "")
                Exit Function
            End If

            'Get Code and Check If already exist
            sPilotCD = fgNullToStr(pPilotInfo.PilotCD.ToUpper)
            iID = fgNullToZero(pPilotInfo.ID)
            If flCheckCode(sPilotCD, iID) = False Then
                flUpdData.Msg = fgMsgOut("EBP001", "", "パイロットCD")
                Exit Function
            End If

            If iID = 0 Then
                If flInsert(pPilotInfo) = False Then
                    Exit Function
                End If
                flUpdData.Msg = fgMsgOut("IBP001", "")
            Else
                If flUpdate(pPilotInfo) = False Then
                    flUpdData.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If
                flUpdData.Msg = fgMsgOut("IBP002", "", "パイロット")
            End If

            flUpdData.Status = C_Flag.CodeS

        Catch ex As Exception
            flUpdData = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Print Pilot Master
    ''' </summary>
    ''' <param name="lPilot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flPrint(ByVal lPilot As List(Of BerthPlan.mPilot)) As MyResult
        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing
        Dim ReturnData = New String() {"", ""}

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

            With objWorkSheet.Range("A4", "D" & (lPilot.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each row In lPilot
                    .Cell(iRow, 1).Value = row.PilotCD
                    .Cell(iRow, 2).Value = row.PilotName
                    .Cell(iRow, 3).Value = row.Email
                    .Cell(iRow, 4).Value = row.Tel
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
            ReturnData = {sSaveFilePath, sSaveFileName}

            flPrint.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            flPrint.Data = ReturnData
            flPrint.Status = C_Flag.CodeS

        Catch ex As Exception
            flPrint = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Insert Pilot
    ''' </summary>
    ''' <param name="pPilot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flInsert(ByVal pPilot As BerthPlan.mPilot) As Boolean
        Dim Auth As Authentication = New Authentication()
        Dim oPilot As BerthPlan.mPilot = New BerthPlan.mPilot

        Try
            flInsert = False

            oPilot.PilotCD = fgNullToStr(pPilot.PilotCD.ToUpper)
            oPilot.PilotName = fgNullToStr(pPilot.PilotName)
            oPilot.Email = fgNullToStr(pPilot.Email)
            oPilot.Tel = fgNullToStr(pPilot.Tel)

            oPilot.UpdTime = DateTime.Now
            oPilot.UpdUserID = Auth.userID
            oPilot.UpdPGID = C_PGID.PilotMaster
            oPilot.Flag = False

            _db.mPilot.Add(oPilot)
            _db.SaveChanges()

            flInsert = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Update Pilot
    ''' </summary>
    ''' <param name="pPilot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flUpdate(ByVal pPilot As BerthPlan.mPilot) As Boolean
        Dim Auth As Authentication = New Authentication()

        Try
            flUpdate = False

            Dim getRow = _db.mPilot.Where(Function(x) x.ID = pPilot.ID).FirstOrDefault()

            'Check UpdTime if the same
            If flCheckUpdDate(pPilot.UpdTime, (From x In _db.mPilot.AsNoTracking
                                               Where x.ID = pPilot.ID
                                               Select x.UpdTime).FirstOrDefault) = False Then
                Exit Function
            End If

            getRow.PilotCD = fgNullToStr(pPilot.PilotCD.ToUpper)
            getRow.PilotName = fgNullToStr(pPilot.PilotName)
            getRow.Email = fgNullToStr(pPilot.Email)
            getRow.Tel = fgNullToStr(pPilot.Tel)

            getRow.UpdTime = DateTime.Now
            getRow.UpdUserID = Auth.userID
            getRow.UpdPGID = C_PGID.PilotMaster
            getRow.Flag = False

            _db.mPilot.Attach(getRow)
            _db.Entry(getRow).State = EntityState.Modified
            _db.SaveChanges()

            flUpdate = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Check PilotCD
    ''' </summary>
    ''' <param name="sPilotCD"></param>
    ''' <param name="iID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flCheckCode(ByVal sPilotCD As String, ByVal iID As Integer) As Boolean
        Dim lPilot As List(Of BerthPlan.mPilot) = New List(Of BerthPlan.mPilot)

        Try
            flCheckCode = False

            lPilot = (From rData In _db.mPilot
                      Where rData.PilotCD.ToUpper = sPilotCD _
                        And rData.Flag = False _
                        And rData.ID <> iID
                      Select rData).ToList()
            If lPilot.Count > 0 Then
                flCheckCode = False
                Exit Function
            End If

            flCheckCode = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete PilotName (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flPilotName(ByVal PilotCD As String) As String

        flPilotName = String.Empty

        Try
            PilotCD = fgNullToStr(PilotCD)
            flPilotName = _db.mPilot.AsNoTracking.Where(Function(b) b.PilotCD = PilotCD And b.Flag = False).Select( _
                                            Function(b) b.PilotName).FirstOrDefault()

        Catch ex As Exception
            flPilotName = ex.Message
        End Try
    End Function

#End Region

End Class