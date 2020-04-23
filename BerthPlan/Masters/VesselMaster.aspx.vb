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
''' 本船マスター
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　 デザインを作った。
'''    00.02      2020/03/17      AK.Dela Rosa     データテーブルを修正した。
''' </history>
''' 

Public Class VesselMaster
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
#End Region

#Region "## クラス内変数 ## "
    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities
    Public Shared Auth As Authentication = New Authentication()
#End Region

#Region "## コントロールイベントの定義 ## "
    ''' <summary>
    ''' ページ読み込み機能
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
    ''' データ取得機能
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetVesselList() As Object
        flGetVesselList = Nothing

        Try
            flGetVesselList = (From v In _db.mVessel.AsNoTracking.ToList
                               Where v.Flag = False
                               Select New With {
                                    .ID = v.ID,
                                    .VesselCD = v.VesselCD,
                                    .IMO = v.IMO,
                                    .VesselName = v.VesselName,
                                    .GrossTon = v.GrossTon,
                                    .Nationality = v.Nationality,
                                    .ApplicantCD = v.ApplicantCD,
                                    .ApplicantName = fgNullToStr((From c In _db.mCompany Where c.ApplicantCD = v.ApplicantCD And c.Flag = False Select c.ApplicantName).FirstOrDefault),
                                    .LOA = v.LOA,
                                    .IO = v.IO,
                                    .UpdTime = v.UpdTime,
                                    .UpdUserID = v.UpdUserID,
                                    .UpdPGID = v.UpdPGID,
                                    .TimeStamp = v.TimeStamp,
                                    .Flag = v.Flag
                                }).OrderByDescending(Function(x) x.UpdTime).ToList()

        Catch ex As Exception
            Throw
        End Try
        Return flGetVesselList
    End Function

    ''' <summary>
    ''' 保存機能
    ''' </summary>
    ''' <param name="mVessel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function SaveVessel(ByVal mVessel As BerthPlan.mVessel) As MyResult
        SaveVessel = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                SaveVessel.Status = C_Flag.CodeO
                Exit Function
            End If

            If (From c In db.mCompany.AsNoTracking.ToList Where c.ApplicantCD = mVessel.ApplicantCD And c.Flag = False).Count = 0 And mVessel.ApplicantCD <> "" Then
                SaveVessel.Msg = fgMsgOut("EBP002", "", "申請者")
                SaveVessel.Status = "failed"
                SaveVessel.Data = "MainContent_ApplicantCD_ApplicantCD"
                Exit Function
            End If

            If mVessel.ID = 0 Then

                If (From c In _db.mVessel.AsNoTracking.ToList Where c.VesselCD = mVessel.VesselCD And c.Flag = False).Count <> 0 Then
                    SaveVessel.Msg = fgMsgOut("EBP001", "", "本船コード")
                    SaveVessel.Status = C_Flag.CodeF
                    Exit Function
                End If

                mVessel.UpdTime = DateTime.Now
                mVessel.UpdPGID = C_PGID.VesselMaster
                mVessel.UpdUserID = Auth.userID
                mVessel.Flag = False
                _db.mVessel.Add(mVessel)
                _db.SaveChanges()
                SaveVessel.Msg = fgMsgOut("IBP001", "")
            Else
                If (From c In _db.mVessel.AsNoTracking.ToList Where c.VesselCD = mVessel.VesselCD And c.Flag = False And c.ID <> mVessel.ID).Count <> 0 Then
                    SaveVessel.Msg = fgMsgOut("EBP001", "", "本船コード")
                    SaveVessel.Status = C_Flag.CodeF
                    Exit Function
                End If

                If flCheckUpdDate(mVessel.UpdTime, (From c In _db.mVessel.AsNoTracking.ToList() Where c.ID = mVessel.ID Select c.UpdTime).FirstOrDefault()) = False Then
                    SaveVessel.Msg = fgMsgOut("EXX004", "")
                    SaveVessel.Status = C_Flag.CodeF
                    Exit Function
                End If

                Dim UpdateVessel As BerthPlan.mVessel = (From c In _db.mVessel.ToList() Where c.ID = mVessel.ID Select c).FirstOrDefault()

                UpdateVessel.VesselCD = fgNullToStr(mVessel.VesselCD)
                UpdateVessel.IMO = fgNullToStr(mVessel.IMO)
                UpdateVessel.VesselName = fgNullToStr(mVessel.VesselName)
                UpdateVessel.GrossTon = fgNullToZero(mVessel.GrossTon)
                UpdateVessel.Nationality = fgNullToStr(mVessel.Nationality)
                UpdateVessel.ApplicantCD = fgNullToStr(mVessel.ApplicantCD)
                UpdateVessel.LOA = fgNullToZero(mVessel.LOA)
                UpdateVessel.IO = mVessel.IO
                UpdateVessel.UpdTime = DateTime.Now
                UpdateVessel.UpdPGID = C_PGID.VesselMaster
                UpdateVessel.UpdUserID = Auth.userID
                UpdateVessel.Flag = False
                _db.Entry(UpdateVessel).State = EntityState.Modified
                _db.SaveChanges()
                SaveVessel.Msg = fgMsgOut("IBP002", "", "本船")
            End If
            SaveVessel.Status = C_Flag.CodeS
        Catch ex As Exception
            SaveVessel = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 掲示板を削除する機能
    ''' </summary>
    ''' <param name="mVessel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function DeleteVessel(ByVal mVessel As List(Of BerthPlan.mVessel)) As MyResult
        DeleteVessel = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                DeleteVessel.Status = C_Flag.CodeO
                Exit Function
            End If

            For Each v In mVessel
                If flCheckUpdDate(v.UpdTime, (From x In _db.mVessel.AsNoTracking.ToList() Where x.ID = v.ID Select x.UpdTime).FirstOrDefault()) = False Then
                    DeleteVessel.Msg = fgMsgOut("EXX004", "")
                    DeleteVessel.Status = C_Flag.CodeF
                    Exit Function
                End If
            Next

            For Each v In mVessel
                Dim DVessel As BerthPlan.mVessel = (From x In _db.mVessel.ToList() Where x.ID = v.ID And x.Flag = False Select x).FirstOrDefault()
                DVessel.UpdTime = DateTime.Now
                DVessel.UpdPGID = C_PGID.VesselMaster
                DVessel.UpdUserID = Auth.userID
                DVessel.Flag = True
                _db.Entry(DVessel).State = EntityState.Modified
                _db.SaveChanges()
            Next
            DeleteVessel.Msg = fgMsgOut("IBP003", "")
            DeleteVessel.Status = C_Flag.CodeS
        Catch ex As Exception
            Exit Function
        End Try
    End Function

    ''' <summary>
    ''' 印刷
    ''' </summary>
    ''' <param name="mVessel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function PrintVessel(mVessel As List(Of BerthPlan.mVessel)) As MyResult
        PrintVessel = New MyResult
        PrintVessel.Status = C_Flag.CodeF
        Dim iCount As Integer = 0
        Dim iRow As Integer = 4
        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing

        Dim sOpenFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/Templates/")
        Dim sOpenFileName As String = "本船マスター.xlsx"

        ' ダウンロードファイル
        Dim sSaveFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/SaveFile/")
        Dim sSaveFileName As String = String.Empty
        Try
            objWorkBook = New XLWorkbook(sOpenFilePath & sOpenFileName)

            sSaveFileName = "本船マスター" & Date.Now.ToString("yyyyMMddHHmmssfff") & ".xlsx"
            objWorkSheet = objWorkBook.Worksheet(1)


            With objWorkSheet.Range("A4", "I" & (mVessel.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each v In mVessel
                    .Cell(iRow, 1).Value = v.VesselCD
                    .Cell(iRow, 2).Value = v.IMO
                    .Cell(iRow, 3).Value = v.VesselName
                    .Cell(iRow, 4).Value = Format(v.GrossTon, "###,###,###,###,##0.0###")
                    .Cell(iRow, 5).Value = Format(v.LOA, "###,###,###,###,##0.0###")
                    .Cell(iRow, 6).Value = v.Nationality
                    .Cell(iRow, 7).Value = If(v.IO = True, "外", "内")
                    .Cell(iRow, 8).Value = v.ApplicantCD
                    .Cell(iRow, 9).Value = fgNullToStr((From c In _db.mCompany Where c.ApplicantCD = v.ApplicantCD And c.Flag = False Select c.ApplicantCD).FirstOrDefault)
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
            PrintVessel.Data = ReturnData
            PrintVessel.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            PrintVessel.Status = C_Flag.CodeS

            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If
        Catch ex As Exception
            PrintVessel = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete VesselName (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fgVesselName(VesselCD As String) As String
        fgVesselName = ""
        Try
            fgVesselName = _db.mVessel.AsNoTracking.Where(Function(b) b.VesselCD = VesselCD And b.Flag = False).Select(Function(b) b.VesselName).FirstOrDefault()
        Catch ex As Exception

        End Try
    End Function

#End Region

End Class