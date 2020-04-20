Imports System.Web
Imports System.Text
Imports System.Linq

Public Class GlobalProperties
    Shared _userID As String
    Shared _userName As String
    Shared _emailAddress As String
    Shared _isAdmin As Boolean
    Shared _ID As Integer

    ''' <summary>
    ''' Authenticated User's ID
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Property gsID As Integer
        Get
            Return _ID
        End Get
        Set(ByVal id As Integer)
            _ID = id
        End Set
    End Property
    ''' <summary>
    ''' Authenticated User's ID
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Property gsUserID As String
        Get
            Return _userID
        End Get
        Set(ByVal user_id As String)
            _userID = user_id
        End Set
    End Property

    ''' <summary>
    ''' Authenticated User's Name
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Property gsUserName As String
        Get
            Return _userName
        End Get
        Set(ByVal user_name As String)
            _userName = user_name
        End Set
    End Property

    ''' <summary>
    ''' Authenticated User's Email
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Property gsEmailAddress As String
        Get
            Return _emailAddress
        End Get
        Set(ByVal email As String)
            _emailAddress = email
        End Set
    End Property

    ''' <summary>
    ''' Authenticated User if Admin
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Property gbIsAdmin As Boolean
        Get
            Return _isAdmin
        End Get
        Set(ByVal is_admin As Boolean)
            _isAdmin = is_admin
        End Set
    End Property

End Class

''' <summary>
''' Base
''' </summary>
''' <remarks></remarks>
Partial Public Class MyResult
    Public Property Status As String
    Public Property Msg As String
    Public Property Data As Object
End Class

''' <summary>
''' Message Code
''' </summary>
''' <remarks></remarks>
Public Structure C_Flag
    Const CodeE = "error"
    Const CodeS = "success"
    Const CodeF = "failed"
    Const CodeO = "expire"
End Structure

''' <summary>
''' Message Code
''' </summary>
''' <remarks></remarks>
Public Structure C_PGID
    'Master
    Const UserMaster = "BP-08-00"
    Const VesselMaster = "BP-09-00"
    Const BerthMaster = "BP-10-00"
    Const CompanyMaster = "BP-11-00"
    Const PilotMaster = "BP-12-00"
    Const WharfMaster = "BP-13-00"
    'Transaction
    Const Login = "BP-01-00"
    Const ChangePassword = "BP-02-00"
    Const SystemMenu = "BP-03-00"
    Const VesselScheduleList = "BP-04-00"
    Const VesselScheduleRegistration = "BP-05-00"
    Const VesselScheduleVisual = "BP-06-00"
    Const BoardRegistration = "BP-07-00"

End Structure

''' <summary>
''' Login Result
''' </summary>
''' <remarks></remarks>
Public Structure sLoginResult
    Public LoggedIn As Boolean
    Public ErrMsg As String
End Structure

''' <summary>
''' DataTable Structure
''' </summary>
''' <remarks></remarks>
Public Structure sDataTables
    Public draw As Integer
    Public recordsTotal As Integer
    Public recordsFiltered As Integer
    Public data As Object
End Structure

''' <summary>
''' Common Function
''' </summary>
''' <remarks></remarks>
Public Class GlobalFunction
    ''' <summary>メッセージ保持DataView</summary>
    Private Shared V_dtMessage As List(Of mMessage) = New List(Of mMessage)
    ''' <summary>Berth Plan Database</summary>
    Public Shared _db As BerthPlanEntities = New BerthPlanEntities()

    ''' <summary>
    ''' メッセージマスタ取得
    ''' </summary>
    ''' <returns>True:正常 False：異常</returns>
    ''' <remarks>メッセージマスタ取得してDataViewへ格納</remarks>
    Public Shared Function fgGetMsgmst() As Boolean
        Try
            fgGetMsgmst = False

            'メッセージマスタの読込。
            V_dtMessage = (From sMsg In _db.mMessage
                            Order By sMsg.MessageID
                           Select sMsg).ToList()

            fgGetMsgmst = True

        Catch ex As Exception
            '親にエラーを返す
            Throw
        End Try
    End Function

    ''' <summary>
    ''' メッセージ表示
    ''' </summary>
    ''' <param name="strMsgType"></param>
    ''' <param name="strMsg"></param>
    ''' <param name="sParmStr1"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function fgMsgOut(ByVal strMsgType As String, _
                                    ByVal strMsg As String, _
                                    Optional ByVal sParmStr1 As String = "", _
                                    Optional ByVal sParmStr2 As String = "") As String

        Dim strSubMsg As String = String.Empty 'サブメッセージ
        Dim sbMsg As New StringBuilder  'メッセージ

        Dim dtMessage As mMessage = New mMessage

        Try
            fgMsgOut = String.Empty

            If IsNothing(V_dtMessage) Then
                Exit Function
            End If

            'メッセージが取得されている場合
            If IsNothing(V_dtMessage) = False Then
                'メッセージIDでメッセージを取得
                dtMessage = V_dtMessage.Where(Function(x) x.MessageID = strMsgType).FirstOrDefault
                If dtMessage.ID <> 0 Then
                    strSubMsg = strMsg
                    strMsg = dtMessage.MessageContent

                    'パラメータが設定されている場合パラメータ文字位置を取得し置換する
                    If sParmStr1 <> String.Empty Then
                        strMsg = strMsg.Replace("%1", sParmStr1)
                    End If
                    If sParmStr2 <> String.Empty Then
                        strMsg = strMsg.Replace("%2", sParmStr2)
                    End If
                End If

            End If

            'メッセージを付加する
            sbMsg.Append(vbCrLf).Append(strMsg)

            If strSubMsg.Trim <> String.Empty Then
                sbMsg.Append(vbCrLf).Append(strSubMsg)
            End If

            fgMsgOut = sbMsg.ToString

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' 共通エラー処理
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function sgErrProc(ByVal errMsg As Object) As MyResult

        sgErrProc = New MyResult

        Try
            sgErrProc.Status = C_Flag.CodeE
            sgErrProc.Msg = "予期しないエラーが発生しました。"
            sgErrProc.Data = errMsg.Message

        Catch ex As Exception
            sgErrProc.Status = C_Flag.CodeE
            sgErrProc.Msg = ex.ToString
        End Try
    End Function

    ''' <summary>
    '''  Null文字列を""にして返す
    ''' </summary>
    ''' <param name="objString">変換対象オブジェクト</param>
    ''' <param name="TrimOption">Trimオプション 0:Trim 1:TrimEnd(RTrim) 2:Trimなし</param>
    ''' <returns>変換後文字列</returns>
    ''' <remarks> Null文字列を""にして返す</remarks>
    Public Shared Function fgNullToStr(ByVal objString As Object, Optional TrimOption As Integer = 0) As String
        Try
            'オブジェクトをチェック
            If IsDBNull(objString) = True Then
                'NULLの場合、空文字を返す
                Return ""
            ElseIf IsNothing(objString) = True Then
                'Nothingの場合、空文字を返す
                Return ""
            Else
                'その他の場合、Stringに型変換して返す
                Select Case TrimOption
                    Case 0  'Trim
                        Return Convert.ToString(objString).Trim
                    Case 1  'TrimEnd
                        Return Convert.ToString(objString).TrimEnd
                    Case 2  'Trimなし
                        Return Convert.ToString(objString)
                    Case Else
                        Return Convert.ToString(objString).Trim
                End Select
            End If
        Catch ex As Exception
            '親にエラーを返す
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Null文字列を0にして返す
    ''' </summary>
    ''' <param name="objNumber">変換対象オブジェクト</param>
    ''' <returns>変換後数字</returns>
    ''' <remarks> Null文字列を0にして返す</remarks>
    Public Shared Function fgNullToZero(ByVal objNumber As Object) As Double
        Try
            'オブジェクトをチェック
            If IsDBNull(objNumber) = True Then
                'NULLの場合、0を返す
                Return 0
            ElseIf IsNothing(objNumber) = True Then
                'Nothingの場合、0を返す
                Return 0
            ElseIf IsNumeric(objNumber) = False Then
                '数値として扱えない場合、0を返す
                Return 0
            Else
                'その他の場合、Decimal型に型変換して返す
                Return Convert.ToDecimal(objNumber)
            End If
        Catch ex As Exception
            '親にエラーを返す
            Throw
        End Try
    End Function

    ''' <summary>
    ''' メールをチェックするl
    ''' </summary>
    ''' <param name="sEmail"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function fgCheckEmail(ByVal sEmail As String) As Boolean
        Try
            fgCheckEmail = False

            If sEmail.Length = 0 Then
                Exit Function
            End If

            If sEmail.IndexOf("@") > -1 Then
                If (sEmail.IndexOf(".", sEmail.IndexOf("@")) > sEmail.IndexOf("@")) AndAlso sEmail.Split(".").Length > 0 _
                    AndAlso sEmail.Split(".")(1) <> String.Empty Then
                    fgCheckEmail = True
                    Exit Function
                End If
            End If

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Check Session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function fgCheckSession() As Boolean
        Dim UserID As String = HttpContext.Current.Session("UserID")
        Try
            fgCheckSession = False

            If _db.mUser.AsNoTracking.Where(Function(x) x.UserID = UserID And x.Flag = True).Count = 1 Then
                With HttpContext.Current
                    .Session.Clear()
                    .Session.Abandon()
                    .Session.RemoveAll()
                End With
            End If

            If IsNothing(UserID) Then
                Exit Function
            End If



            fgCheckSession = True

        Catch ex As Exception
            fgCheckSession = False
        End Try
    End Function

    ''' <summary>
    ''' TimeStamp型変換
    ''' </summary>
    ''' <param name="objTimeStamp">変換するTimeStamp</param>
    ''' <returns>変換後の文字列</returns>
    ''' <remarks></remarks>
    Public Shared Function fgConvertTimeStamp(ByVal objTimeStamp As Object) As String

        Try
            If IsDBNull(objTimeStamp) = True Then
                Return String.Empty
            ElseIf IsNothing(objTimeStamp) = True Then
                Return String.Empty
            Else
                Return Convert.ToBase64String(CType(objTimeStamp, Byte()))
            End If
        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Check Time Stamp
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function flCheckUpdDate(ByVal OldUpdDate As DateTime, ByVal NewUpdDate As DateTime) As Boolean
        Dim sMsg As String = String.Empty

        Try
            flCheckUpdDate = False

            If OldUpdDate.ToString("yyyyMMddHHmmss") <> NewUpdDate.ToString("yyyyMMddHHmmss") Then
                Exit Function
            End If

            flCheckUpdDate = True

        Catch ex As Exception
            Throw
        End Try
    End Function

End Class