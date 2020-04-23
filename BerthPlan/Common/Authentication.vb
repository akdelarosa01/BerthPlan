Imports System
Imports System.Text
Imports System.Security.Cryptography
Imports System.Web
Imports System.Data.Entity.Validation
Imports BerthPlan.GlobalFunction

Public Class Authentication

    Private _db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities


    Public Function Hash(ByVal password As String) As String
        Dim hashBytes As Byte() = ComputeHash(password)

        Dim hashWithSaltBytes As Byte()

        Hash = ""

        Try
            ReDim hashWithSaltBytes(hashBytes.Length)

            For i = 0 To hashBytes.Length - 1
                hashWithSaltBytes(i) = hashBytes(i)
            Next i

            Hash = Convert.ToBase64String(hashWithSaltBytes)
        Catch ex As Exception
            Throw
        End Try

        Return Hash
    End Function

    Private Function ComputeHash(ByVal password As String) As Byte()
        ComputeHash = Nothing

        Try
            Dim passwordTextBytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim Hashing As HashAlgorithm = New SHA256Managed()
            ComputeHash = Hashing.ComputeHash(passwordTextBytes)

        Catch ex As Exception
            Throw
        End Try

        Return ComputeHash
    End Function

    Public Function Login(ByVal txtUserID As String, ByVal txtPassword As String) As sLoginResult
        Login.LoggedIn = False
        Login.ErrMsg = ""
        Dim mUsertbl As BerthPlan.mUser = Nothing

        Try

            'If Not HttpContext.Current.Session("UserID") = String.Empty Then
            '    Login.LoggedIn = False
            '    Login.ErrMsg = "ユーザーはすでにログインしています。"
            'End If

            Dim pword = Hash(txtPassword)

            mUsertbl = (From u In _db.mUser
                        Where (u.UserID = txtUserID And u.Password = pword) And u.Flag = False
                        Select u).FirstOrDefault

            If Not IsNothing(mUsertbl) Then
                With HttpContext.Current
                    .Session("ID") = mUsertbl.ID
                    .Session("UserID") = mUsertbl.UserID
                    .Session("UserName") = mUsertbl.UserName
                    .Session("EmailAddress") = mUsertbl.EmailAddress
                    .Session("IsAdmin") = mUsertbl.IsAdmin

                    Call slUpdateLastLogin(mUsertbl.UserID)
                    GlobalProperties.gsID = .Session("ID")
                    GlobalProperties.gsUserID = .Session("UserID")
                    GlobalProperties.gsUserName = .Session("UserName")
                    GlobalProperties.gsEmailAddress = .Session("EmailAddress")
                    GlobalProperties.gbIsAdmin = .Session("IsAdmin")
                End With
                Login.LoggedIn = True
            Else
                mUsertbl = (From u In _db.mUser
                            Where (u.UserID = txtUserID And u.Password = pword)
                            Select u).FirstOrDefault
                If Not IsNothing(mUsertbl) Then
                    Login.LoggedIn = False
                    Login.ErrMsg = "ユーザーアクセスが拒否されました。"
                Else
                    Login.ErrMsg = "ユーザーIDとパスワードが一致しませんでした。"
                End If
            End If
        Catch ex As Exception
            Throw
        End Try

        Return Login
    End Function

    Public Function Logout() As Boolean
        Logout = False
        Try
            With HttpContext.Current
                .Session.Clear()
                .Session.Abandon()
                .Session.RemoveAll()

                If IsNothing(.Session("UserID")) Then
                    Logout = True
                End If
            End With
        Catch ex As Exception
            Throw
        End Try

        Return Logout
    End Function

    Public Function isLoggedIn() As Boolean
        isLoggedIn = False

        Try
            If Not IsNothing(HttpContext.Current.Session("UserID")) Then
                isLoggedIn = True
            End If
        Catch ex As Exception
            Throw
        End Try

        Return isLoggedIn
    End Function

    Public Function userID() As String
        Return HttpContext.Current.Session("UserID")
    End Function

    Public Function userName() As String
        Return HttpContext.Current.Session("UserName")
    End Function

    Public Function emailAddress() As String
        Return HttpContext.Current.Session("EmailAddress")
    End Function

    Public Function isAdmin() As Boolean
        Return HttpContext.Current.Session("IsAdmin")
    End Function

    Public Function ID() As Integer
        Return HttpContext.Current.Session("ID")
    End Function

    Private Sub slUpdateLastLogin(ByVal UserID As String)
        Dim mUser As BerthPlan.mUser = New BerthPlan.mUser
        Try
            mUser = _db.mUser.Single(Function(u) u.UserID = UserID)

            'mUser = (From u In _db.mUser
            '        Where u.UserID = UserID
            '        Select u).FirstOrDefault

            mUser.LastLogin = DateTime.Today

            _db.Entry(mUser).State = EntityState.Modified
            _db.SaveChanges()
        Catch ex As DbEntityValidationException
            Throw New ApplicationException(ex.EntityValidationErrors.ToString, ex)
        End Try
    End Sub
End Class
