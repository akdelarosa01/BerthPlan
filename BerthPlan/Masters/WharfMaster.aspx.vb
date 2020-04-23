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
''' 岸壁マスター
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　 デザインを作った。
'''    00.02      2020/03/17      AK.Dela Rosa     データテーブルを修正した。
''' </history>
''' 

Public Class WharfMaster
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
#End Region

#Region "## クラス内変数 ## "
    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities
    Public Shared Auth As Authentication = New Authentication()
#End Region

#Region "## コントロールイベントの定義 ## "

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
    ''' データ取得機能
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetWharfList() As Object
        flGetWharfList = Nothing

        Try
            flGetWharfList = _db.mWharf.AsNoTracking.Where(Function(x) x.Flag = False).OrderByDescending(Function(x) x.UpdTime).ToList()
        Catch ex As Exception
            Throw
        End Try
        Return flGetWharfList
    End Function

    ''' <summary>
    ''' 保存機能
    ''' </summary>
    ''' <param name="mWharf"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function SaveWharf(ByVal mWharf As BerthPlan.mWharf) As MyResult
        SaveWharf = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                SaveWharf.Status = C_Flag.CodeO
                Exit Function
            End If

            If mWharf.ID = 0 Then

                If (From c In _db.mWharf.AsNoTracking.ToList Where c.WharfCD = mWharf.WharfCD And c.Flag = False).Count <> 0 Then
                    SaveWharf.Msg = fgMsgOut("EBP001", "", "岸壁コード")
                    SaveWharf.Status = C_Flag.CodeF
                    Exit Function
                End If

                mWharf.UpdTime = DateTime.Now
                mWharf.UpdPGID = C_PGID.VesselMaster
                mWharf.UpdUserID = Auth.userID
                mWharf.Flag = False
                _db.mWharf.Add(mWharf)
                _db.SaveChanges()
                SaveWharf.Msg = fgMsgOut("IBP001", "")
            Else
                If (From c In _db.mWharf.AsNoTracking.ToList Where c.WharfCD = mWharf.WharfCD And c.Flag = False And c.ID <> mWharf.ID).Count <> 0 Then
                    SaveWharf.Msg = fgMsgOut("EBP001", "", "岸壁コード")
                    SaveWharf.Status = C_Flag.CodeF
                    Exit Function
                End If

                If flCheckUpdDate(mWharf.UpdTime, (From c In _db.mWharf.AsNoTracking.ToList() Where c.ID = mWharf.ID Select c.UpdTime).FirstOrDefault()) = False Then
                    SaveWharf.Msg = fgMsgOut("EXX004", "")
                    SaveWharf.Status = C_Flag.CodeF
                    Exit Function
                End If

                Dim UpdateWharf As BerthPlan.mWharf = (From c In _db.mWharf.ToList() Where c.ID = mWharf.ID Select c).FirstOrDefault()

                UpdateWharf.WharfCD = fgNullToStr(mWharf.WharfCD)
                UpdateWharf.WharfName = fgNullToStr(mWharf.WharfName)
                UpdateWharf.UpdTime = DateTime.Now
                UpdateWharf.UpdPGID = C_PGID.VesselMaster
                UpdateWharf.UpdUserID = Auth.userID
                UpdateWharf.Flag = False

                _db.Entry(UpdateWharf).State = EntityState.Modified
                _db.SaveChanges()
                SaveWharf.Msg = fgMsgOut("IBP002", "", "岸壁")
            End If
            SaveWharf.Status = C_Flag.CodeS
        Catch ex As Exception
            SaveWharf = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 掲示板を削除する機能
    ''' </summary>
    ''' <param name="mWharf"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function DeleteWharf(ByVal mWharf As List(Of BerthPlan.mWharf)) As MyResult
        DeleteWharf = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                DeleteWharf.Status = C_Flag.CodeO
                Exit Function
            End If

            For Each w In mWharf
                If flCheckUpdDate(w.UpdTime, (From x In _db.mWharf.AsNoTracking.ToList() Where x.ID = w.ID Select x.UpdTime).FirstOrDefault()) = False Then
                    DeleteWharf.Msg = fgMsgOut("EXX004", "")
                    DeleteWharf.Status = C_Flag.CodeF
                    Exit Function
                End If
            Next

            For Each w In mWharf
                Dim DWharf As BerthPlan.mWharf = (From x In _db.mWharf.ToList() Where x.ID = w.ID And x.Flag = False Select x).FirstOrDefault()
                DWharf.UpdTime = DateTime.Now
                DWharf.UpdPGID = C_PGID.VesselMaster
                DWharf.UpdUserID = Auth.userID
                DWharf.Flag = True
                _db.Entry(DWharf).State = EntityState.Modified
                _db.SaveChanges()
            Next

            DeleteWharf.Msg = fgMsgOut("IBP003", "")
            DeleteWharf.Status = C_Flag.CodeS
        Catch ex As Exception
            DeleteWharf = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 印刷
    ''' </summary>
    ''' <param name="mWharf"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function PrintWharf(mWharf As List(Of BerthPlan.mWharf)) As MyResult
        PrintWharf = New MyResult
        PrintWharf.Status = C_Flag.CodeF
        Dim iCount As Integer = 0
        Dim iRow As Integer = 4
        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing

        Dim sOpenFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/Templates/")
        Dim sOpenFileName As String = "ワーフマスター.xlsx"

        ' ダウンロードファイル
        Dim sSaveFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/SaveFile/")
        Dim sSaveFileName As String = String.Empty
        Try
            objWorkBook = New XLWorkbook(sOpenFilePath & sOpenFileName)

            sSaveFileName = "ワーフマスター" & Date.Now.ToString("yyyyMMddHHmmssfff") & ".xlsx"
            objWorkSheet = objWorkBook.Worksheet(1)


            With objWorkSheet.Range("A4", "B" & (mWharf.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each s In mWharf
                    .Cell(iRow, 1).Value = s.WharfCD
                    .Cell(iRow, 2).Value = s.WharfName
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

            'ファイルダウンロード
            Dim ReturnData = New String() {sSaveFilePath, sSaveFileName}
            PrintWharf.Data = ReturnData
            PrintWharf.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            PrintWharf.Status = C_Flag.CodeS

            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If
        Catch ex As Exception
            PrintWharf = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete WharfName (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fgWharfName(WharfCD As String) As String
        fgWharfName = ""
        Try
            fgWharfName = _db.mWharf.AsNoTracking.Where(Function(b) b.WharfCD = WharfCD And b.Flag = False).Select(Function(b) b.WharfName).FirstOrDefault()
        Catch ex As Exception

        End Try
    End Function

#End Region

End Class