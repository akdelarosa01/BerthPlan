Public Class WharfControl
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

#Region "Property"
    ''' <summary>
    ''' TextCode
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TextCode() As String
        Get
            Return WharfCD.Text
        End Get
        Set(ByVal value As String)
            WharfCD.Text = value
        End Set
    End Property

    ''' <summary>
    ''' TextName
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TextName() As String
        Get
            Return WharfName.Text
        End Get
        Set(ByVal value As String)
            WharfName.Text = value
        End Set
    End Property
#End Region

End Class