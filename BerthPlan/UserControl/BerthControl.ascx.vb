Public Class BerthControl
    Inherits System.Web.UI.UserControl
    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

#Region "Property"

    ''' <summary>
    ''' TextID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TextID() As String
        Get
            Return BerthID.Text
        End Get
        Set(ByVal value As String)
            BerthID.Text = value
        End Set
    End Property

    ''' <summary>
    ''' TextCode
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TextCode() As String
        Get
            Return BerthCD.Text
        End Get
        Set(ByVal value As String)
            BerthCD.Text = value
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
            Return BerthName.Text
        End Get
        Set(ByVal value As String)
            BerthName.Text = value
        End Set
    End Property

#End Region

End Class