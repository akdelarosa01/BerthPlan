Imports System.Web.Services
Imports BerthPlan.GlobalFunction

Public Class ChangePassword
    Inherits System.Web.UI.Page

    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities

#Region "Event"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("UserID")) Then
            Response.Redirect("~/Login.aspx?SessionExpire")
            Exit Sub
        End If
        Call fgCheckSession()
    End Sub

    <WebMethod()>
    Public Shared Function ChangePassword(CurrentPassword As String, Password As String) As MyResult
        Dim Auth As Authentication = New Authentication()
        ChangePassword = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                ChangePassword.Status = C_Flag.CodeO
                Exit Function
            End If

            If (From u In db.mUser.AsNoTracking.ToList Where u.UserID = Auth.userID And u.Password = Auth.Hash(CurrentPassword)).Count <> 0 Then
                Dim mUser = (From u In db.mUser.ToList()
                        Where u.UserID = GlobalProperties.gsUserID
                        Select u).FirstOrDefault()
                mUser.Password = Auth.Hash(Password)
                mUser.UpdTime = DateTime.Now
                mUser.UpdPGID = C_PGID.ChangePassword
                mUser.UpdUserID = Auth.userID
                db.Entry(mUser).State = EntityState.Modified
                db.SaveChanges()
                ChangePassword.Msg = fgMsgOut("IBP002", "", "パスワード")
                ChangePassword.Status = C_Flag.CodeS
            Else
                ChangePassword.Msg = fgMsgOut("EXX003", "")
                ChangePassword.Status = "danger"
            End If

            With HttpContext.Current
                .Session.Clear()
                .Session.Abandon()
                .Session.RemoveAll()
            End With

        Catch ex As Exception
            Exit Function
        End Try
    End Function

#End Region

End Class