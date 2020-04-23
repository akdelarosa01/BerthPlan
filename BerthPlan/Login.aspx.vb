#Region "## インポート ##"

Imports System.Text
Imports System.Linq
Imports System.Web
Imports BerthPlan.GlobalFunction

#End Region

''' <summary>
''' ログイン画面
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　デザインを作った。
'''    00.02      2020/03/16      AK.Dela Rosa    コーディングを開始しました。
''' </history>
Public Class Login

    Inherits System.Web.UI.Page
#Region "## クラス内変数 ## "
    Dim _db As BerthPlan.BerthPlanEntities
    Dim Auth As Authentication = New Authentication
    Private LoginResult As sLoginResult
#End Region

#Region "## コントロールイベントの定義 ## "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack Then
            failed_alert.Visible = False
            failed_msg.InnerText = ""

            txtUserID_grp.Attributes.Remove("class")
            txtUserID_grp.Attributes.Add("class", "form-group has-feedback")
            txtUserIDmsg.InnerText = ""

            txtPassword_grp.Attributes.Remove("class")
            txtPassword_grp.Attributes.Add("class", "form-group has-feedback")
            txtPasswordmsg.InnerText = ""
        Else
        If Request.RawUrl.Split("?")(Request.RawUrl.Split("?").Length - 1).ToString = "SessionExpire" Then
            failed_alert.Visible = True
            failed_msg.InnerText = "セッションの有効期限が切れ。アカウントに再ログインします"
        Else
            failed_alert.Visible = False
            failed_msg.InnerText = ""
        End If
        End If

        'If Not Session("UserID") = String.Empty Then
        '    Response.Redirect("~/Pages/SystemMenu.aspx")
        'End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        If Not flCheckCredentials(txtUserID.Text, txtPassword.Text) Then
            Exit Sub
        End If

        LoginResult = Auth.Login(txtUserID.Text, txtPassword.Text)

        If LoginResult.LoggedIn Then
            failed_alert.Visible = False
            failed_msg.InnerText = ""

            ClientScript.RegisterStartupScript(Me.GetType(),
                                                       "OpenNewWindow",
                                                       "<script language=""javascript"">window.open('Pages/SystemMenu.aspx'," &
                                                       "'_blank'," &
                                                       "'status=1, scrollbars=1, resizable=1, fullscreen=yes'); window.location.href = '.../../../Login.aspx'; </script>")

            '自画面を閉じる
            ClientScript.RegisterStartupScript(Me.GetType(), "OpenClose", "<script language=""javascript"">window.open('', '_self').close();</script>")

            'ClientScript.RegisterStartupScript(Me.GetType(),
            '                                           "OpenNewWindow",
            '                                           "<script language=""javascript"">window.open('Pages/SystemMenu.aspx'," &
            '                                           "'_blank'," &
            '                                           "'status=1, scrollbars=1, resizable=1, fullscreen=yes'); window.open('', '_self').close() </script>")

            'メッセージマスタ取得
            Call fgGetMsgmst()

            Exit Sub
        Else
            failed_alert.Visible = True
            failed_msg.InnerText = LoginResult.ErrMsg
            Exit Sub
        End If
    End Sub
#End Region

#Region "## 内部メソッド ##"
    Private Function flCheckCredentials(ByVal txtUserID As String, ByVal txtPassword As String) As Boolean
        flCheckCredentials = False

        Try
            If txtUserID = String.Empty And txtPassword = String.Empty Then
                txtUserID_grp.Attributes.Add("class", "has-error")
                txtUserIDmsg.InnerText = "ユーザーID入力フィールドに入力してください。"

                txtPassword_grp.Attributes.Add("class", "has-error")
                txtPasswordmsg.InnerText = "パスワード入力フィールドに入力してください。"
                Exit Function
            End If

            If txtUserID = String.Empty Then
                txtUserID_grp.Attributes.Add("class", "has-error")
                txtUserIDmsg.InnerText = "ユーザーID入力フィールドに入力してください。"
                Exit Function
            End If

            If txtPassword = String.Empty Then
                txtPassword_grp.Attributes.Add("class", "has-error")
                txtPasswordmsg.InnerText = "パスワード入力フィールドに入力してください。"
                Exit Function
            End If

            flCheckCredentials = True
            Exit Function
        Catch ex As Exception

        End Try
        Return flCheckCredentials
    End Function

    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        ClientScript.RegisterStartupScript(Me.GetType(), "closePage", "window.close();", True)
    End Sub

#End Region

End Class