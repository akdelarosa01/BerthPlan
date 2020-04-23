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

#End Region

''' <summary>
''' 掲示板登録
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　デザインを作った。
'''    00.02      2020/03/13      AK.Dela Rosa    コーディングを開始しました。
''' </history>

Public Class BoardRegistration
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
#End Region

#Region "## クラス内変数 ## "

    Public Shared _db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities()
    Public Shared _BoardID As Integer = 0
    Public Shared ServerPath As String = HttpContext.Current.Server.MapPath("~/Assets/bulletin_files/")
#End Region

#Region "## コントロールイベントの定義 ## "
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

        If Not IsPostBack Then

        End If

    End Sub
#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' データ取得機能
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetBulletin() As Object
        flGetBulletin = Nothing

        Try
            flGetBulletin = _db.tBoard.AsNoTracking.Where(Function(x) x.Flag = False).ToList()
        Catch ex As Exception
            Throw
        End Try

        Return flGetBulletin
    End Function

    ''' <summary>
    ''' ファイルのデータテーブル
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetFiles(BoardID As Integer) As Object
        flGetFiles = Nothing
        Try
            flGetFiles = (From b In _db.tBoardFile.AsNoTracking
                          Where b.BoardID = BoardID And b.Flag = False
                          Select New With {
                              .BoardFileID = b.BoardFileID,
                              .FileName = b.FileName,
                              .FileType = b.FileType,
                              .FileSize = b.FileSize,
                              .UploadDate = b.UploadDate,
                              .BoardID = b.BoardID
                              })

        Catch ex As Exception
        End Try
        Return flGetFiles
    End Function

    ''' <summary>
    ''' ファイルのデータテーブル
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetViewedUsers(BoardID As Integer) As Object
        flGetViewedUsers = Nothing
        Try

            flGetViewedUsers = (From u In _db.mUser.AsNoTracking.Where(Function(x) x.Flag = False).ToList
                         Group Join bv In _db.tBoardView.AsNoTracking.Where(Function(x) x.BoardID = BoardID And x.Flag = False).ToList
                         On u.ID Equals bv.UserID Into gr = Group
                         From x In gr.DefaultIfEmpty()
                         Select New With
                            {
                                .UserID = u.UserID,
                                .UserName = u.UserName,
                                .IsChecked = If(x Is Nothing, "", "○"),
                                .UpdTime = If(x Is Nothing, Nothing, x.UpdTime)
                        })

        Catch ex As Exception
            Console.Write(ex.Message)
        End Try
        Return flGetViewedUsers
    End Function

    ''' <summary>
    ''' 掲示板を削除する機能
    ''' </summary>
    ''' <param name="ids"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flDeleteBoard(ByVal ids As Object) As MyResult
        Dim tBoards As IQueryable(Of BerthPlan.tBoard) = Nothing
        Dim id As String() = Nothing
        Dim files As Object = Nothing
        flDeleteBoard = New MyResult

        Try
            'Check Session
            If fgCheckSession() = False Then
                flDeleteBoard.Status = C_Flag.CodeO
                Exit Function
            End If

            id = ids.Split(",")
            tBoards = _db.tBoard.Where(
                Function(b) id.Contains(b.BoardID)
                )

            For Each board As BerthPlan.tBoard In tBoards
                board.Flag = True
                files = _db.tBoardFile.Where(Function(f) f.BoardID = board.BoardID).FirstOrDefault
                If Directory.Exists(ServerPath & board.BoardID) Then
                    Directory.Delete(ServerPath & board.BoardID, True)
                End If

            Next

            If _db.SaveChanges() > 0 Then
                flDeleteBoard.Msg = fgMsgOut("IBP003", "")
                flDeleteBoard.Status = C_Flag.CodeS
            Else
                flDeleteBoard.Msg = fgMsgOut("EBP004", "")
                flDeleteBoard.Status = C_Flag.CodeF
            End If

        Catch ex As Exception
            flDeleteBoard = sgErrProc(ex)
            Return flDeleteBoard
        End Try
        Return flDeleteBoard
    End Function

    ''' <summary>
    ''' 登録
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flRegisterKeijiban(ByVal id As Integer, ByVal title As String, ByVal contents As String, ByVal link As String, _
                                               ByVal postSdate As String, ByVal postEdate As String, ByVal hdStatus As String, ByVal hdUserID As String) As MyResult
        Dim Auth As Authentication = New Authentication()
        Dim tBoard As BerthPlan.tBoard
        flRegisterKeijiban = New MyResult

        Try
            'Check Session
            'If fgCheckSession() = False Then
            '    flSaveBulletinBoard.Status = "expire"
            '    Exit Function
            'End If

            Dim validation = flValidateBoard(title, contents, postSdate, postEdate)
            If validation.Data = False Then
                flRegisterKeijiban.Status = validation.Status
                flRegisterKeijiban.Msg = validation.Msg
                Exit Function
            End If

            tBoard = New BerthPlan.tBoard
            Select Case hdStatus
                Case "EDIT"
                    tBoard = (From c In _db.tBoard.ToList()
                        Where c.BoardID = id
                        Select c).FirstOrDefault()

                    tBoard.Title = title
                    tBoard.Contents = contents
                    tBoard.HyperLink = link
                    tBoard.PostingStartDate = postSdate
                    tBoard.PostingEndDate = postEdate
                    tBoard.UpdTime = DateTime.Now
                    tBoard.UpdPGID = C_PGID.BoardRegistration
                    tBoard.UpdUserID = hdUserID
                    tBoard.Flag = False

                    _db.Entry(tBoard).State = EntityState.Modified
                Case Else
                    tBoard.Title = title
                    tBoard.Contents = contents
                    tBoard.HyperLink = link
                    tBoard.PostingStartDate = postSdate
                    tBoard.PostingEndDate = postEdate
                    tBoard.CreateUserID = hdUserID
                    tBoard.CreatedTime = DateTime.Now
                    tBoard.UpdTime = DateTime.Now
                    tBoard.UpdPGID = C_PGID.BoardRegistration
                    tBoard.UpdUserID = hdUserID
                    tBoard.Flag = False

                    _db.tBoard.Add(tBoard)
            End Select

            If _db.SaveChanges() > 0 Then
                Dim tBoardView As New BerthPlan.tBoardView

                tBoardView.BoardID = tBoard.BoardID
                tBoardView.UserID = Auth.ID
                tBoardView.IsChecked = True
                tBoardView.UpdTime = DateTime.Now
                tBoardView.UpdPGID = "SystemMenu"
                tBoardView.UpdUserID = Auth.userID
                tBoardView.Flag = False
                _db.tBoardView.Add(tBoardView)
                _db.SaveChanges()

                flRegisterKeijiban.Msg = fgMsgOut("IBP001", "")
                flRegisterKeijiban.Status = C_Flag.CodeS
                flRegisterKeijiban.Data = tBoard.BoardID
            Else
                flRegisterKeijiban.Status = C_Flag.CodeF
                flRegisterKeijiban.Msg = fgMsgOut("EBP005", "")
            End If

        Catch ex As Exception
            flRegisterKeijiban = sgErrProc(ex)
        End Try
        Return flRegisterKeijiban
    End Function

    Public Shared Function flValidateBoard(ByVal title As String, ByVal contents As String, _
                                        ByVal postSdate As String, ByVal postEdate As String) As MyResult
        flValidateBoard = New MyResult

        Try
            flValidateBoard.Status = C_Flag.CodeF
            flValidateBoard.Data = False

            If String.IsNullOrEmpty(title) Or String.IsNullOrEmpty(contents) _
                Or String.IsNullOrEmpty(postSdate) Or String.IsNullOrEmpty(postEdate) Then
                flValidateBoard.Msg = fgMsgOut("EBP006", "")
                flValidateBoard.Status = C_Flag.CodeF
                Exit Function
            End If

            If postSdate > postEdate Then
                flValidateBoard.Msg = fgMsgOut("EBP007", "")
                flValidateBoard.Status = C_Flag.CodeF
                Exit Function
            End If

            flValidateBoard.Data = True

            Exit Function
        Catch ex As Exception
            flValidateBoard = sgErrProc(ex)
            Exit Function
        End Try
        Return flValidateBoard

    End Function

    ''' <summary>
    ''' 掲示板のファイルを削除する機能
    ''' </summary>
    ''' <param name="ids"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flDeleteBoardFiles(ByVal ids As Object) As MyResult
        Dim tBoardFiles As IQueryable(Of BerthPlan.tBoardFile) = Nothing
        Dim id As String() = Nothing
        flDeleteBoardFiles = New MyResult
        Dim BoardID As Integer

        Try
            'Check Session
            'If fgCheckSession() = False Then
            '    flDeleteBoardFiles.Status = "expire"
            '    Exit Function
            'End If

            id = ids.Split(",")
            tBoardFiles = _db.tBoardFile.Where(
                Function(b) id.Contains(b.BoardFileID)
                )

            For Each board As BerthPlan.tBoardFile In tBoardFiles
                board.Flag = True
                BoardID = board.BoardID

                'If File.Exists(ServerPath & board.BoardID & "/" & board.FileName) Then
                '    File.Delete(ServerPath & board.BoardID & "/" & board.FileName)
                'End If
            Next

            If _db.SaveChanges() > 0 Then
                flDeleteBoardFiles.Msg = fgMsgOut("IBP003", "")
                flDeleteBoardFiles.Status = C_Flag.CodeS
                flDeleteBoardFiles.Data = BoardID
            Else
                flDeleteBoardFiles.Msg = fgMsgOut("EBP004", "")
                flDeleteBoardFiles.Status = C_Flag.CodeF
            End If

        Catch ex As Exception
            flDeleteBoardFiles = sgErrProc(ex)
            Return flDeleteBoardFiles
        End Try
        Return flDeleteBoardFiles
    End Function

    ''' <summary>
    ''' データ取得機能
    ''' </summary>
    ''' <param name="BoardID"></param>
    ''' <returns></returns>
    <WebMethod()>
    Public Shared Function flGetCompanyList(ByVal BoardID As Integer) As Object
        flGetCompanyList = Nothing

        Try
            If BoardID = 0 Then
                flGetCompanyList = (From b In _db.mCompany.AsNoTracking
                                    Where b.Flag = False
                                    Select New With {
                                      .ID = b.ID,
                                      .ApplicantCD = b.ApplicantCD,
                                      .ApplicantName = b.ApplicantName,
                                      .UpdTime = b.UpdTime
                                      })
            Else

                flGetCompanyList = (From c In _db.mCompany.AsNoTracking.ToList
                                    Group Join b In _db.tBoardCompany.AsNoTracking.ToList On c.ID Equals b.CompanyID Into Group
                                    From x In Group.DefaultIfEmpty()
                                    Select New With {
                                        .ID = c.ID,
                                        .ApplicantCD = c.ApplicantCD,
                                        .ApplicantName = c.ApplicantName,
                                        .UpdTime = If(IsNothing(x), String.Empty, x.UpdTime),
                                        .BoardID = If(IsNothing(x), String.Empty, x.BoardID)
                                    })

            End If

        Catch ex As Exception
            Throw
        End Try
        Return flGetCompanyList
    End Function
#End Region

End Class