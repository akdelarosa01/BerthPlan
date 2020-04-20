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
''' 会社マスター
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者------------対応内容----
'''    00.01      2020/03/12      AK. Dela Rosa　　デザインを作った。
'''    00.03      2020/03/17      AV. Dela Rosa    データテーブルを修正した。 
''' </history>

Public Class CompanyMaster
    Inherits System.Web.UI.Page


#Region "## クラス内変数 ## "
    Public Shared db As BerthPlanEntities = New BerthPlanEntities()
    Public Shared Auth As Authentication = New Authentication()
#End Region

#Region "## クラス内定数 ## "
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
    Public Shared Function flGetCompanyList() As Object
        flGetCompanyList = Nothing

        Try
            flGetCompanyList = _db.mCompany.AsNoTracking.Where(Function(x) x.Flag = False).OrderByDescending(Function(x) x.UpdTime).ToList()
        Catch ex As Exception
            Throw
        End Try
        Return flGetCompanyList
    End Function

    ''' <summary>
    ''' 保存機能
    ''' </summary>
    ''' <param name="mCompany"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function SaveCompany(ByVal mCompany As mCompany) As MyResult
        SaveCompany = New MyResult
        SaveCompany.Status = C_Flag.CodeF
        Try
            'Check Session
            If fgCheckSession() = False Then
                SaveCompany.Status = C_Flag.CodeO
                Exit Function
            End If

            If fgNullToStr(mCompany.Email).Length <> 0 Then
                If fgCheckEmail(mCompany.Email) = False Then
                    SaveCompany.Msg = fgMsgOut("EBP008", "")
                    Exit Function
                End If
            End If

            If mCompany.ID = 0 Then

                If (From c In _db.mCompany.AsNoTracking.ToList Where c.ApplicantCD = mCompany.ApplicantCD And c.Flag = False).Count <> 0 Then
                    SaveCompany.Msg = fgMsgOut("EBP001", "", "会社コード")
                    Exit Function
                End If

                mCompany.UpdTime = DateTime.Now
                mCompany.UpdPGID = C_PGID.CompanyMaster
                mCompany.UpdUserID = Auth.userID
                mCompany.Flag = False
                _db.mCompany.Add(mCompany)
                _db.SaveChanges()
                SaveCompany.Msg = fgMsgOut("IBP001", "")
            Else
                If (From c In _db.mCompany.AsNoTracking.ToList Where c.ApplicantCD = mCompany.ApplicantCD And c.Flag = False And c.ID <> mCompany.ID).Count <> 0 Then
                    SaveCompany.Msg = fgMsgOut("EBP001", "", "会社コード")
                    Exit Function
                End If

                If flCheckUpdDate(mCompany.UpdTime, (From c In _db.mCompany.AsNoTracking.ToList() Where c.ID = mCompany.ID Select c.UpdTime).FirstOrDefault()) = False Then
                    SaveCompany.Status = C_Flag.CodeF
                    SaveCompany.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If

                Dim UpdateCompany As mCompany = (From c In _db.mCompany.ToList() Where c.ID = mCompany.ID Select c).FirstOrDefault()

                UpdateCompany.ApplicantCD = fgNullToStr(mCompany.ApplicantCD)
                UpdateCompany.ApplicantName = fgNullToStr(mCompany.ApplicantName)
                UpdateCompany.Address = fgNullToStr(mCompany.Address)
                UpdateCompany.PostCode = fgNullToStr(mCompany.PostCode)
                UpdateCompany.Tel = fgNullToStr(mCompany.Tel)
                UpdateCompany.Fax = fgNullToStr(mCompany.Fax)
                UpdateCompany.Email = fgNullToStr(mCompany.Email)
                UpdateCompany.Color = fgNullToStr(mCompany.Color)
                UpdateCompany.UpdTime = DateTime.Now
                UpdateCompany.UpdPGID = C_PGID.CompanyMaster
                UpdateCompany.UpdUserID = Auth.userID
                UpdateCompany.Flag = False

                _db.Entry(UpdateCompany).State = EntityState.Modified
                _db.SaveChanges()
                SaveCompany.Msg = fgMsgOut("IBP002", "", "会社")
            End If
            SaveCompany.Status = C_Flag.CodeS
        Catch ex As Exception
            SaveCompany = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 掲示板を削除する機能
    ''' </summary>
    ''' <param name="mCompany"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function DeleteCompany(ByVal mCompany As List(Of mCompany)) As MyResult
        DeleteCompany = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                DeleteCompany.Status = C_Flag.CodeO
                Exit Function
            End If

            For Each c In mCompany
                If flCheckUpdDate(c.UpdTime, (From x In _db.mCompany.AsNoTracking.ToList() Where x.ID = c.ID Select x.UpdTime).FirstOrDefault()) = False Then
                    DeleteCompany.Msg = fgMsgOut("EXX004", "")
                    DeleteCompany.Status = C_Flag.CodeF
                    Exit Function
                End If
            Next

            For Each c In mCompany
                Dim DCompany As mCompany = (From x In _db.mCompany.ToList() Where x.ID = c.ID And c.Flag = False Select x).FirstOrDefault()
                DCompany.UpdTime = DateTime.Now
                DCompany.UpdPGID = C_PGID.CompanyMaster
                DCompany.UpdUserID = Auth.userID
                DCompany.Flag = True
                _db.Entry(DCompany).State = EntityState.Modified
                _db.SaveChanges()
                DeleteCompany.Msg = fgMsgOut("IBP003", "")
                DeleteCompany.Status = C_Flag.CodeS
            Next
        Catch ex As Exception
            DeleteCompany = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 印刷
    ''' </summary>
    ''' <param name="mCompany"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function PrintCompany(mCompany As List(Of mCompany)) As MyResult
        PrintCompany = New MyResult
        PrintCompany.Status = C_Flag.CodeF
        Dim iCount As Integer = 0
        Dim iRow As Integer = 4
        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing

        Dim sOpenFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/Templates/")
        Dim sOpenFileName As String = "会社マスター.xlsx"

        ' ダウンロードファイル
        Dim sSaveFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/SaveFile/")
        Dim sSaveFileName As String = String.Empty
        Try
            objWorkBook = New XLWorkbook(sOpenFilePath & sOpenFileName)

            sSaveFileName = "会社マスター" & Date.Now.ToString("yyyyMMddHHmmssfff") & ".xlsx"
            objWorkSheet = objWorkBook.Worksheet(1)


            With objWorkSheet.Range("A4", "G" & (mCompany.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each s In mCompany
                    .Cell(iRow, 1).Value = s.ApplicantCD
                    .Cell(iRow, 2).Value = s.ApplicantName
                    .Cell(iRow, 3).Value = s.PostCode
                    .Cell(iRow, 4).Value = s.Address
                    .Cell(iRow, 5).Value = s.Tel
                    .Cell(iRow, 6).Value = s.Fax
                    .Cell(iRow, 7).Value = s.Email
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
            PrintCompany.Data = ReturnData
            PrintCompany.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            PrintCompany.Status = C_Flag.CodeS

            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If
        Catch ex As Exception
            PrintCompany = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete ApplicantName (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fgApplicantName(ApplicantCD As String) As String
        fgApplicantName = ""
        Try
            fgApplicantName = _db.mCompany.AsNoTracking.Where(Function(b) b.ApplicantCD = ApplicantCD And b.Flag = False).Select(Function(b) b.ApplicantName).FirstOrDefault()
        Catch ex As Exception

        End Try
    End Function

#End Region

End Class