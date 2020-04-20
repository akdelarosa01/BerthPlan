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
Imports System.Configuration
Imports System.Reflection
Imports System.Net
Imports BerthPlan.GlobalFunction

#End Region

Public Class SystemMenu
    Inherits System.Web.UI.Page

#Region "## クラス内変数 ## "
    Public Shared db As BerthPlanEntities = New BerthPlanEntities()
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

        If Not Auth.isAdmin Then
            menuMasters.Visible = False
        End If
        Call fgCheckSession()
    End Sub

#End Region

#Region "## WebMethod ##"

    ''' <summary>
    ''' データ取得機能(Board Title and Content)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flGetBoardList() As Object
        flGetBoardList = Nothing
        Try
            Dim ID As Integer = Auth.ID
            flGetBoardList = (From b In db.tBoard.AsNoTracking.ToList
                         Where b.Flag = False And (b.PostingStartDate <= Date.Today And (b.PostingEndDate) >= Date.Today)
                         Select New With {
                            .BoardID = b.BoardID,
                            .Title = b.Title,
                            .Contents = b.Contents,
                            .HyperLink = b.HyperLink,
                            .PostingStartDate = b.PostingStartDate,
                            .PostingEndDate = b.PostingEndDate,
                            .Seen = If((From bv In db.tBoardView.AsNoTracking Where b.BoardID = bv.BoardID And bv.UserID = ID Select bv).Count = 0, 0, 1),
                            .UpdUserID = b.UpdUserID,
                            .CreatedTime = b.CreatedTime,
                            .CreateUserID = (From u In db.mUser.AsNoTracking Where u.UserID = b.CreateUserID And u.Flag = False Select u.UserName).FirstOrDefault,
            .UpdTime = b.UpdTime
                        }).OrderByDescending(Function(x) x.PostingStartDate).OrderByDescending(Function(x) x.CreatedTime).ToList
        Catch ex As Exception
            Console.Write(ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' データ取得機能(Board File)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flFileDisplay(ByVal BoardID As Integer) As MyResult
        flFileDisplay = New MyResult
        Dim data As List(Of tBoardFile) = Nothing

        Try
            'Check Session
            If fgCheckSession() = False Then
                flFileDisplay.Status = C_Flag.CodeO
                Exit Function
            End If

            data = db.tBoardFile.AsNoTracking.Where(Function(bf) bf.Flag = False And bf.BoardID = BoardID).ToList

            flFileDisplay.Data = data
            flFileDisplay.Msg = "OK"
            flFileDisplay.Status = C_Flag.CodeS
            Exit Function
        Catch ex As Exception
            flFileDisplay.Msg = ex.Message
            flFileDisplay.Status = C_Flag.CodeE
        End Try

        Return flFileDisplay
    End Function

    ''' <summary>
    ''' 掲示板が見られました
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flSeenUserViewBoard(ByVal BoardID As String) As MyResult
        flSeenUserViewBoard = New MyResult
        flSeenUserViewBoard.Status = C_Flag.CodeS
        Dim tBoardView As New tBoardView
        Try
            'Check Session
            If fgCheckSession() = False Then
                flSeenUserViewBoard.Status = C_Flag.CodeO
                Exit Function
            End If

            If flIsAlreadySeen(BoardID, Auth.ID) = False Then
                Exit Function
            End If

            tBoardView.BoardID = BoardID
            tBoardView.UserID = Auth.ID
            tBoardView.IsChecked = True
            tBoardView.UpdTime = DateTime.Now
            tBoardView.UpdPGID = C_PGID.SystemMenu
            tBoardView.UpdUserID = Auth.userID
            tBoardView.Flag = False
            db.tBoardView.Add(tBoardView)
            db.SaveChanges()
        Catch ex As Exception
            flSeenUserViewBoard.Msg = ex.Message
            flSeenUserViewBoard.Status = C_Flag.CodeE
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="BoardID"></param>
    ''' <param name="UserID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flIsAlreadySeen(ByVal BoardID As String, ByVal UserID As String) As Boolean
        Try
            flIsAlreadySeen = False

            BoardID = Integer.Parse(BoardID)
            Dim obj = (From tBV In _db.tBoardView.AsNoTracking
                        Where tBV.UserID = UserID And tBV.BoardID = BoardID _
                       Select tBV).ToList()
            If obj.Count > 0 Then
                Exit Function
            End If

            flIsAlreadySeen = True

        Catch ex As Exception
            Throw
        End Try
    End Function

#End Region

End Class