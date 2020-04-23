Imports System.Web.Services

Public Class CompanyControl
    Inherits System.Web.UI.UserControl

    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities

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
            Return ApplicantCD.Text
        End Get
        Set(ByVal value As String)
            ApplicantCD.Text = value
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
            Return ApplicantName.Text
        End Get
        Set(ByVal value As String)
            ApplicantName.Text = value
        End Set
    End Property
#End Region

End Class