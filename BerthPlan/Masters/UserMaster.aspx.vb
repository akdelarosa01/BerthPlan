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
''' ユーザーマスタ
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　デザインを作った。
'''    00.02      2020/03/15      KD.Ga            コーディングを開始しました。
'''    00.02      2020/03/18      AK.Dela Rosa     データテーブルを修正した。
''' </history>

Public Class UserMaster
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
    ''' <summary>EXCEL</summary>
    Const C_Excel_File = "ユーザーマスター.xlsx"
    ''' <summary>Excel Name</summary>
    Const C_Excel_Name = "ユーザーマスター"
#End Region

#Region "## クラス内変数 ## "
    ''' <summary>DBBerth</summary>
    Public Shared _db As BerthPlan.BerthPlanEntities = New BerthPlanEntities
#End Region

#Region "## コントロールイベント定義 ##"
    ''' <summary>
    ''' PageLoad
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsNothing(Session("UserID")) Then
                Response.Redirect("~/Login.aspx")
                Exit Sub
            End If

            If fgCheckSession() Then
                Exit Sub
            End If

        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' Get User List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json, UseHttpGet:=True)>
    Public Shared Function flGetUserList() As sDataTables
        flGetUserList = Nothing
        Dim data As List(Of mUser) = Nothing   'Data
        Dim search As String = String.Empty     'DataTable Search Value
        Dim draw As String = String.Empty       'Value when drawing DataTable
        Dim order As String = String.Empty      'Defined Column to Order
        Dim orderDir As String = String.Empty   'Ordering Direction {ASC/DESC}
        Dim startRec As Integer = 0
        Dim pageSize As Integer = 0
        Dim totalRec As Integer = 0
        Dim recFilter As Integer = 0

        Try
            With HttpContext.Current.Request
                search = .Params("search[value]")
                draw = .Params("draw")
                order = .Params("order[0][column]")
                orderDir = .Params("order[0][dir]")
                startRec = Convert.ToInt32(.Params("start"))
                pageSize = Convert.ToInt32(.Params("length"))
            End With

            data = _db.mUser.AsNoTracking.ToList()

            totalRec = data.Count

            If Not String.IsNullOrEmpty(search) And Not String.IsNullOrWhiteSpace(search) Then
                data = data.Where(Function(u) u.ID.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.UserID.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.UserName.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.EmailAddress.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.IsAdmin.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.Flag.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.LastLogin.ToString().ToLower().Contains(search.ToLower()) _
                                      Or u.UpdTime.ToString().ToLower().Contains(search.ToLower())
                                ).ToList()
            End If

            data = flSortByColumnWithOrder(order, orderDir, data)

            recFilter = data.Count

            data = data.Skip(startRec).Take(pageSize).ToList()

            flGetUserList.draw = Convert.ToInt32(draw)
            flGetUserList.recordsTotal = totalRec
            flGetUserList.recordsFiltered = recFilter
            flGetUserList.data = data
        Catch ex As Exception
            Throw
        End Try

        Return flGetUserList
    End Function

    ''' <summary>
    ''' Get User List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fGetData() As Object
        Try
            fGetData = (From x In _db.mUser.AsNoTracking
                        Order By x.UpdTime Descending
                        Select New With {
                            .ID = If(x.ID = Nothing, Nothing, x.ID),
                            .UserID = If(x.UserID = Nothing, Nothing, x.UserID),
                            .UserName = If(x.UserName = Nothing, Nothing, x.UserName),
                            .Password = If(x.Password = Nothing, Nothing, x.Password),
                            .EmailAddress = If(x.EmailAddress = Nothing, Nothing, x.EmailAddress),
                            .ApplicantCD = If(x.ApplicantCD = Nothing, Nothing, x.ApplicantCD),
                            .ApplicantName = If((From c In _db.mCompany
                                                 Where c.ApplicantCD = x.ApplicantCD _
                                                  And c.Flag = False
                                                 Select c.ApplicantName).FirstOrDefault() = Nothing, Nothing, (From c In _db.mCompany
                                                                                                               Where c.ApplicantCD = x.ApplicantCD _
                                                                                                                    And c.Flag = False
                                                                                                               Select c.ApplicantName).FirstOrDefault()),
                            .LastLogin = x.LastLogin,
                            .Flag = If(x.Flag = Nothing, Nothing, x.Flag),
                            .IsAdmin = If(x.IsAdmin = Nothing, Nothing, x.IsAdmin),
                            .UpdTime = If(x.UpdTime = Nothing, Nothing, x.UpdTime)
                            }).ToList

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' データテーブルのソートおよび順序付け関数
    ''' </summary>
    ''' <param name="order"></param>
    ''' <param name="orderDir"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function flSortByColumnWithOrder(ByVal order As String, ByVal orderDir As String, data As List(Of mUser)) As List(Of mUser)
        flSortByColumnWithOrder = New List(Of mUser)

        Try
            If IsNothing(orderDir) Then
                orderDir = String.Empty
            End If
            Select Case order
                Case "0"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.ID).ToList(),
                                               data.OrderBy(Function(u) u.ID).ToList())
                    Exit Function
                Case "2"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.UserID).ToList(),
                                               data.OrderBy(Function(u) u.UserID).ToList())
                    Exit Function
                Case "3"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.UserName).ToList(),
                                               data.OrderBy(Function(u) u.UserName).ToList())
                    Exit Function
                Case "4"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.EmailAddress).ToList(),
                                               data.OrderBy(Function(u) u.EmailAddress).ToList())
                    Exit Function
                Case "5"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.IsAdmin).ToList(),
                                               data.OrderBy(Function(u) u.IsAdmin).ToList())
                    Exit Function
                Case "6"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.Flag).ToList(),
                                               data.OrderBy(Function(u) u.Flag).ToList())
                    Exit Function
                Case "7"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.LastLogin).ToList(),
                                               data.OrderBy(Function(u) u.LastLogin).ToList())
                    Exit Function
                Case "8"
                    flSortByColumnWithOrder = If(orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase),
                                               data.OrderByDescending(Function(u) u.UpdTime).ToList(),
                                               data.OrderBy(Function(u) u.UpdTime).ToList())
                    Exit Function
            End Select

        Catch ex As Exception
            Throw
        End Try

        Return flSortByColumnWithOrder
    End Function

    ''' <summary>
    ''' Delete User
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flDelUser(ByVal lUser As List(Of mUser)) As MyResult
        Dim mUser As IQueryable(Of mUser) = Nothing
        Dim bIsMe As Boolean = False

        Try
            flDelUser = New MyResult
            flDelUser.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flDelUser.Status = C_Flag.CodeO
                Exit Function
            End If

            'Check UpdTime if the same
            For Each sLine As mUser In lUser
                If flCheckUpdDate(sLine.UpdTime, (From x In _db.mUser.AsNoTracking
                                                  Where x.ID = sLine.ID
                                                  Select x.UpdTime).FirstOrDefault) = False Then
                    flDelUser.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If
            Next

            'Delete User
            For Each sRow As mUser In lUser
                Dim sData = _db.mUser.Where(Function(x) x.ID = sRow.ID).FirstOrDefault

                If fgNullToStr(sData.ID) = fgNullToStr(GlobalProperties.gsID) Then
                    bIsMe = True
                    'Delete Session
                    Call flDeleteSession()
                End If

                sData.UpdTime = DateTime.Now
                sData.Flag = True

                _db.mUser.Attach(sData)
                _db.Entry(sData).State = EntityState.Modified
            Next
            If _db.SaveChanges() < 1 Then
                flDelUser.Msg = fgMsgOut("EBP004", "すでに削除されています。")
                Exit Function
            End If

            flDelUser.Status = C_Flag.CodeS
            flDelUser.Msg = fgMsgOut("IBP003", "")
            flDelUser.Data = bIsMe

        Catch ex As Exception
            flDelUser = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Delete Session
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub flDeleteSession()
        Try
            With HttpContext.Current
                .Session.Clear()
                .Session.Abandon()
                .Session.RemoveAll()
            End With
        Catch ex As Exception
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="lUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flPrint(ByVal lUser As List(Of mUser)) As MyResult
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

            With objWorkSheet.Range("A4", "E" & (lUser.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each row In lUser
                    .Cell(iRow, 1).Value = row.UserID
                    .Cell(iRow, 2).Value = row.UserName
                    .Cell(iRow, 3).Value = row.EmailAddress
                    .Cell(iRow, 4).Value = row.ApplicantCD
                    .Cell(iRow, 5).Value = If(row.Flag = False, "", "〇")
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
    ''' Save/Update User
    ''' </summary>
    ''' <param name="pUserInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flUpdData(ByVal pUserInfo As mUser) As MyResult
        Dim lUser As List(Of mUser) = New List(Of mUser)
        Dim oUser As mUser = New mUser
        Dim sUserID As String = String.Empty
        Dim iID As Integer = 0

        Try
            flUpdData = New MyResult
            flUpdData.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flUpdData.Status = C_Flag.CodeO
                Exit Function
            End If

            'Get Code and Check If already exist
            iID = fgNullToZero(pUserInfo.ID)
            sUserID = fgNullToStr(pUserInfo.UserID.ToUpper)
            If flCheckCode(iID, sUserID) = False Then
                flUpdData.Msg = fgMsgOut("EBP001", "", "ユーザーID")
                Exit Function
            End If

            'Check EmailAddress
            If fgCheckEmail(pUserInfo.EmailAddress) = False Then
                flUpdData.Msg = fgMsgOut("EBP008", "")
                Exit Function
            End If
            If flIsEmailExist(pUserInfo.EmailAddress, iID) = False Then
                flUpdData.Msg = fgMsgOut("EBP001", "", "メール")
                Exit Function
            End If

            'Save/Update Data
            If iID = 0 Then
                If flInsert(pUserInfo) = False Then
                    Exit Function
                End If
                flUpdData.Msg = fgMsgOut("IBP001", "")
            Else
                If flUpdate(pUserInfo) = False Then
                    flUpdData.Msg = fgMsgOut("EXX004", "")
                    Exit Function
                End If
                flUpdData.Msg = fgMsgOut("IBP002", "", "ユーザー")
            End If

            flUpdData.Status = C_Flag.CodeS

        Catch ex As Exception
            flUpdData = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Check UserCode
    ''' </summary>
    ''' <param name="iID"></param>
    ''' <param name="sUserID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flCheckCode(ByVal iID As Integer, ByVal sUserID As String) As Boolean
        Dim lUser As List(Of mUser) = New List(Of mUser)

        Try
            flCheckCode = False

            lUser = (From rData In _db.mUser
                     Where rData.UserID.ToUpper = sUserID _
                        And rData.Flag = False _
                        And rData.ID <> iID
                     Select rData).ToList()

            If lUser.Count > 0 Then
                flCheckCode = False
                Exit Function
            End If

            flCheckCode = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Insert
    ''' </summary>
    ''' <param name="pUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flInsert(ByVal pUser As mUser) As Boolean
        Dim Auth As Authentication = New Authentication()
        Dim oUser As mUser = New mUser

        Try
            flInsert = False

            oUser.UserID = fgNullToStr(pUser.UserID)
            oUser.UserName = fgNullToStr(pUser.UserName)
            oUser.Password = Auth.Hash(Trim(pUser.Password))
            oUser.EmailAddress = fgNullToStr(pUser.EmailAddress)
            oUser.ApplicantCD = fgNullToStr(pUser.ApplicantCD)
            oUser.IsAdmin = pUser.IsAdmin

            oUser.UpdTime = DateTime.Now
            oUser.UpdUserID = Auth.userID
            oUser.UpdPGID = C_PGID.UserMaster
            oUser.Flag = False

            _db.mUser.Add(oUser)
            _db.SaveChanges()

            flInsert = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Update
    ''' </summary>
    ''' <param name="pUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flUpdate(ByVal pUser As mUser) As Boolean
        Dim Auth As Authentication = New Authentication()
        Dim sPassWord As String = String.Empty

        Try
            flUpdate = False

            Dim sGetRow = _db.mUser.Where(Function(x) x.ID = pUser.ID).FirstOrDefault()
            If sGetRow.Password.ToUpper = pUser.Password.ToUpper Then
                sPassWord = sGetRow.Password
            Else
                sPassWord = Auth.Hash(pUser.Password)
            End If

            'Check UpdTime
            If flCheckUpdDate(pUser.UpdTime, (From x In _db.mUser.AsNoTracking
                                              Where x.UserID = pUser.UserID
                                              Select x.UpdTime).FirstOrDefault) = False Then
                Exit Function
            End If

            sGetRow.UserName = fgNullToStr(pUser.UserName)
            sGetRow.Password = sPassWord
            sGetRow.EmailAddress = fgNullToStr(pUser.EmailAddress)
            sGetRow.ApplicantCD = fgNullToStr(pUser.ApplicantCD)
            sGetRow.IsAdmin = pUser.IsAdmin

            sGetRow.UpdTime = DateTime.Now
            sGetRow.UpdUserID = Auth.userID
            sGetRow.UpdPGID = C_PGID.UserMaster
            sGetRow.Flag = False

            _db.mUser.Attach(sGetRow)
            _db.Entry(sGetRow).State = EntityState.Modified
            _db.SaveChanges()

            'Change Current UserName Session
            If HttpContext.Current.Session("ID") = sGetRow.ID Then
                HttpContext.Current.Session("UserName") = sGetRow.UserName
            End If

            flUpdate = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Check If Email is Already been in the Database
    ''' </summary>
    ''' <param name="pEmail"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flIsEmailExist(ByVal pEmail As String, ByVal pID As Integer) As Boolean
        Try
            flIsEmailExist = False

            Dim bIsExist = _db.mUser.Where(Function(x) x.EmailAddress.ToUpper = pEmail.ToUpper And
                                               x.Flag = False And x.ID <> pID).Count
            If bIsExist > 0 Then
                Exit Function
            End If

            flIsEmailExist = True

        Catch ex As Exception
            Throw
        End Try
    End Function

#End Region

End Class