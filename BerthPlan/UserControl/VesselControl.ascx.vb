Public Class VesselControl
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
            Return VesselCD.Text
        End Get
        Set(ByVal value As String)
            VesselCD.Text = value
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
            Return VesselName.Text
        End Get
        Set(ByVal value As String)
            VesselName.Text = value
        End Set
    End Property
#End Region
End Class