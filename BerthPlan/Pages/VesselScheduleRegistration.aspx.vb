#Region "## インポート ##"

Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.Services
Imports BerthPlan.GlobalFunction

#End Region

Public Class VesselScheduleRegistration
    Inherits System.Web.UI.Page

#Region "## クラス内定数 ## "
#End Region

#Region "## クラス内変数 ## "
    ''' <summary>_db</summary>
    Private Shared _db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities
#End Region

#Region "## コントロールイベント定義 ##"
    ''' <summary>
    ''' Page Load
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsNothing(Session("UserID")) Then
                Response.Redirect("~/Login.aspx?SessionExpire")
                Exit Sub
            End If
            Call fgCheckSession()
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' Search
    ''' </summary>
    ''' <param name="pVesselCD"></param>
    ''' <param name="pVoyageNo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flSearch(ByVal pVesselCD As String, ByVal pVoyageNo As String) As MyResult
        Dim oObj As Object = Nothing

        Try
            flSearch = New MyResult
            flSearch.Status = C_Flag.CodeF

            pVesselCD = fgNullToStr(pVesselCD)
            pVoyageNo = fgNullToStr(pVoyageNo)

            'Check Parameters
            If pVesselCD = String.Empty Or pVoyageNo = String.Empty Then
                flSearch.Msg = fgMsgOut("EBP006", "")
                Exit Function
            End If

            'Get Vessel
            If flGetVessel(pVesselCD, pVoyageNo, oObj) = False Then
                flSearch.Msg = fgMsgOut("EBP002", "", "本船")
                Exit Function
            End If

            flSearch.Data = oObj
            flSearch.Status = C_Flag.CodeS

        Catch ex As Exception
            flSearch = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Search Voyage
    ''' </summary>
    ''' <param name="pVoyageNo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flSearchVoyage(ByVal pVoyageNo As String, ByVal pVesselCD As String) As MyResult
        Dim rMsg As String = String.Empty
        Dim rFlag As String = String.Empty

        Try
            flSearchVoyage = New MyResult
            flSearchVoyage.Status = C_Flag.CodeF

            'Initialize Data
            pVoyageNo = fgNullToStr(pVoyageNo)
            pVesselCD = fgNullToStr(pVesselCD)

            'Check Session
            If fgCheckSession() = False Then
                flSearchVoyage.Status = C_Flag.CodeO
                Exit Function
            End If

            'Check Vessel in master
            Dim iVessel = _db.mVessel.Where(Function(x) x.VesselCD = pVesselCD _
                                                And x.Flag = False).ToList
            If iVessel.Count < 1 Then
                flSearchVoyage.Data = "Err"
                Exit Function
            End If

            'Check Voyage
            If flCheckVoyage(pVoyageNo, pVesselCD, rMsg, rFlag) = False Then
                flSearchVoyage.Status = rFlag
                flSearchVoyage.Msg = rMsg
                Exit Function
            End If

            flSearchVoyage.Status = C_Flag.CodeS

        Catch ex As Exception
            flSearchVoyage = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' GetVessel
    ''' </summary>
    ''' <param name="pVesselCD"></param>
    ''' <param name="pVoyageNo"></param>
    ''' <param name="rObj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flGetVessel(ByVal pVesselCD As String,
                                        ByVal pVoyageNo As String,
                                        ByRef rObj As Object) As Boolean
        Try
            flGetVessel = False

            rObj = (From tS In _db.tSchedule
                    Join mV In _db.mVessel
                            On tS.VesselCD Equals mV.VesselCD
                    Join mB In _db.mBerth
                            On tS.BerthID Equals mB.BerthID
                    Join mC In _db.mCompany
                            On tS.ApplicantCD Equals mC.ApplicantCD
                    Join mP In _db.mPilot
                            On tS.PilotCD Equals mP.PilotCD
                    Where tS.VesselCD = pVesselCD And tS.VoyageNo = pVoyageNo And tS.Flag = False
                    Select New With {
                            .VesselName = mV.VesselName,
                            .GrossTon = mV.GrossTon,
                            .LOA = mV.LOA,
                            .ScheduleID = tS.ScheduleID,
                            .VoyageNo = tS.VoyageNo,
                            .BerthID = tS.BerthID,
                            .BerthCD = mB.BerthCD,
                            .BerthName = mB.BerthName,
                            .PilotCD = tS.PilotCD,
                            .PilotName = mP.PilotName,
                            .ApplicantCD = tS.ApplicantCD,
                            .ApplicantName = mC.ApplicantName,
                            .PilotGuide = tS.PilotGuide,
                            .Tag = tS.Tag,
                            .LineBoat = tS.LineBoat,
                            .ETA = tS.ETA,
                            .ETB = tS.ETB,
                            .ETD = tS.ETD,
                            .ShipFace = tS.ShipFace,
                            .UpdTime = tS.UpdTime
                        }).FirstOrDefault()
            'If Nothing, Get VesselInfo for New Registration
            If IsNothing(rObj) Then
                rObj = (From x In _db.mVessel.Where(Function(x) x.VesselCD = pVesselCD _
                                                And x.Flag = False).ToList
                        Select New With {
                             .ScheduleID = 0,
                             .VesselName = x.VesselName,
                             .GrossTon = x.GrossTon,
                             .LOA = x.LOA
                            }).FirstOrDefault()
            End If

            flGetVessel = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Register To Schedule
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function flRegister(ByVal pSched As BerthPlan.tSchedule, ByVal berthCD As String) As MyResult
        Dim sMsg As String = String.Empty
        Dim aName As String() = {"", "", ""}
        Dim sErr As String = "Error"
        Dim iBerth As BerthPlan.mBerth = New BerthPlan.mBerth
        Dim iBerthID As Integer = 0

        Try
            flRegister = New MyResult
            flRegister.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flRegister.Status = C_Flag.CodeO
                Exit Function
            End If

            'Check User Controls
            aName = flCheckUserControl(pSched.BerthID, berthCD, pSched.ApplicantCD, pSched.PilotCD, iBerthID)
            If aName(0) <> String.Empty Or aName(1) <> String.Empty Or aName(2) <> String.Empty Then
                flRegister.Data = aName
                Exit Function
            End If

            'Pass the updated current BerthID
            pSched.BerthID = iBerthID

            'Check Date
            If flCheckDate(pSched.ETA, pSched.ETB, pSched.ETD) = False Then
                flRegister.Msg = fgMsgOut("EBP007", "")
                Exit Function
            End If

            'Check Schedule
            If flCheckSched(pSched.BerthID, pSched.ETB, pSched.ETD, pSched.ScheduleID) = False Then
                flRegister.Msg = fgMsgOut("EBP003", "")
                Exit Function
            End If

            If pSched.ScheduleID = 0 Then
                'Insert Schedule Data
                If flInsData(pSched, sMsg) = False Then
                    flRegister.Msg = sMsg
                    Exit Function
                End If
            Else
                'Update Schedule Data
                If flUpdData(pSched, sMsg) = False Then
                    flRegister.Msg = sMsg
                    Exit Function
                End If
            End If

            flRegister.Msg = sMsg
            flRegister.Status = C_Flag.CodeS

        Catch ex As Exception
            flRegister = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' Check User Controls
    ''' </summary>
    ''' <param name="iBerthID"></param>
    ''' <param name="sBerthCD"></param>
    ''' <param name="sApplicantCD"></param>
    ''' <param name="sPilotCD"></param>
    ''' <param name="rBerthID"></param>
    ''' <returns></returns>
    Private Shared Function flCheckUserControl(ByVal iBerthID As Integer, ByVal sBerthCD As String,
                                               ByVal sApplicantCD As String, ByVal sPilotCD As String,
                                               ByRef rBerthID As Integer) As String()
        Dim sErr As String = "Error"
        Dim iBerth As BerthPlan.mBerth = New BerthPlan.mBerth
        Dim aName As String() = {"", "", ""}

        Try
            aName = {"", "", ""}

            'Check Berth
            If iBerthID = 0 Then
                iBerth = (From b In _db.mBerth.AsNoTracking
                          Where b.BerthCD.ToUpper = sBerthCD.ToUpper _
                                 And b.Flag = False
                          Order By b.UpdTime Descending
                          Select b).FirstOrDefault
                If iBerth Is Nothing Then
                    aName(0) = sErr
                End If
            Else
                iBerth = (From b In _db.mBerth.AsNoTracking
                          Where b.BerthCD.ToUpper = sBerthCD.ToUpper _
                                And b.BerthID = iBerthID And b.Flag = False
                          Select b).FirstOrDefault
                If iBerth Is Nothing Then
                    aName(0) = sErr
                End If
            End If

            'Check Applicant
            Dim iApplicant = From c In _db.mCompany.AsNoTracking
                             Where c.ApplicantCD.ToUpper = sApplicantCD.ToUpper _
                                    And c.Flag = False
                             Select c
            If iApplicant.Count < 1 Then
                aName(1) = sErr
            End If

            'Check Pilot
            Dim iPilot = From p In _db.mPilot.AsNoTracking()
                         Where p.PilotCD.ToUpper = sPilotCD.ToUpper _
                            And p.Flag = False
                         Select p
            If iPilot.Count < 1 Then
                aName(2) = sErr
            End If

            'Return Got BerthID 
            If Not iBerth Is Nothing Then
                rBerthID = iBerth.BerthID
            End If

            flCheckUserControl = aName

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Check Date
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flCheckDate(ByVal pETA As DateTime,
                                    ByVal pETB As DateTime,
                                    ByVal pETD As DateTime) As Boolean
        Dim dtETA As DateTime
        Dim dtETB As DateTime
        Dim dtETD As DateTime

        Dim iResult As Integer

        Try
            flCheckDate = False

            dtETA = pETA             'Arrival Time
            dtETB = pETB             'Berth Time
            dtETD = pETD             'Departure Time
            iResult = 0

            'Check Arrival
            iResult = DateTime.Compare(dtETA, dtETB)
            If iResult >= 0 Then
                Exit Function 'Arrival is greater than Berth
            End If
            'Check Berth
            iResult = DateTime.Compare(dtETB, dtETD)
            If iResult >= 0 Then
                Exit Function 'Berth is greater than Depart
            End If
            'Check Departure
            iResult = DateTime.Compare(dtETA, dtETB)
            If iResult >= 0 Then
                Exit Function 'Departure is less than Depart
            End If

            flCheckDate = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    '''  Check VoyageNo If Already Exists
    ''' </summary>
    ''' <param name="pVoyageNo"></param>
    ''' <param name="rMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flCheckVoyage(ByVal pVoyageNo As String, ByVal pVesselCD As String,
                                          ByRef rMsg As String, ByRef rFlag As String) As Boolean
        Try
            flCheckVoyage = False

            'Make VoyageNo UpperCase and Check
            pVoyageNo = pVoyageNo.ToString().ToUpper()
            Dim chckVoy = (From sData In _db.tSchedule
                           Where sData.VoyageNo.ToUpper = pVoyageNo _
                           And sData.Flag = False
                           Select sData).ToList

            If chckVoy.Count > 0 Then
                'Check if UserInput VoyageNo if already in the Schedule
                Dim sCheck = (From sData In _db.tSchedule
                              Where sData.VoyageNo.ToUpper = pVoyageNo _
                              And sData.VesselCD = pVesselCD _
                              And sData.Flag = 0
                              Select sData)
                If sCheck.Count = 0 Then
                    rFlag = C_Flag.CodeE
                    rMsg = fgMsgOut("EBP009", "")
                    Exit Function
                End If
            Else
                rFlag = C_Flag.CodeF
                rMsg = fgMsgOut("QXX001", "")
                Exit Function
            End If

            flCheckVoyage = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' CheckSched Data
    ''' </summary>
    ''' <param name="pBerthID"></param>
    ''' <param name="pETB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flCheckSched(ByVal pBerthID As String, ByVal pETB As Date, ByVal pETD As Date, ByVal pSchedID As Integer) As Boolean
        Try
            flCheckSched = False

            Dim sSched = _db.tSchedule.AsNoTracking.Where(Function(x) (
                                                               x.ETB >= pETB And x.ETB <= pETD Or
                                                               x.ETD >= pETB And x.ETD <= pETD Or
                                                               pETB >= x.ETB And pETB <= x.ETD
                                                           ) _
                                                        And x.BerthID = pBerthID _
                                                        And x.ScheduleID <> pSchedID _
                                                        And x.Flag = False).Count
            If sSched > 0 Then
                Exit Function
            End If

            flCheckSched = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Insert Data
    ''' </summary>
    ''' <param name="pSched"></param>
    ''' <param name="rMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flInsData(ByVal pSched As BerthPlan.tSchedule, ByRef rMsg As String) As Boolean
        Dim oSched As BerthPlan.tSchedule = New BerthPlan.tSchedule

        Try
            flInsData = False

            oSched.VesselCD = fgNullToStr(pSched.VesselCD)
            oSched.VoyageNo = fgNullToStr(pSched.VoyageNo)
            oSched.BerthID = fgNullToZero(pSched.BerthID)
            oSched.ApplicantCD = fgNullToStr(pSched.ApplicantCD)
            oSched.PilotCD = fgNullToStr(pSched.PilotCD)
            oSched.PilotGuide = pSched.PilotGuide
            oSched.Tag = pSched.Tag
            oSched.LineBoat = pSched.LineBoat
            oSched.ETA = pSched.ETA
            oSched.ETB = pSched.ETB
            oSched.ETD = pSched.ETD
            oSched.ETD = pSched.ETD
            oSched.ShipFace = pSched.ShipFace

            oSched.UpdPGID = C_PGID.VesselScheduleRegistration
            oSched.UpdTime = DateTime.Now
            oSched.UpdUserID = GlobalProperties.gsUserID
            oSched.Flag = False

            _db.tSchedule.Add(oSched)
            _db.SaveChanges()

            rMsg = fgMsgOut("IBP001", "")
            flInsData = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Update Data
    ''' </summary>
    ''' <param name="pSched"></param>
    ''' <param name="rMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function flUpdData(ByVal pSched As BerthPlan.tSchedule, ByRef rMsg As String) As Boolean
        Dim oSched As BerthPlan.tSchedule = New BerthPlan.tSchedule

        Try
            flUpdData = False

            oSched = (From sRow In _db.tSchedule.Where(Function(x) x.ScheduleID = pSched.ScheduleID)).FirstOrDefault

            'Check Time Stamp
            If flCheckUpdDate(pSched.UpdTime, oSched.UpdTime) = False Then
                rMsg = fgMsgOut("EXX004", "")
                Exit Function
            End If

            oSched.VesselCD = fgNullToStr(pSched.VesselCD)
            oSched.VoyageNo = fgNullToStr(pSched.VoyageNo)
            oSched.BerthID = fgNullToZero(pSched.BerthID)
            oSched.ApplicantCD = fgNullToStr(pSched.ApplicantCD)
            oSched.PilotCD = fgNullToStr(pSched.PilotCD)
            oSched.PilotGuide = pSched.PilotGuide
            oSched.Tag = pSched.Tag
            oSched.LineBoat = pSched.LineBoat
            oSched.ETA = pSched.ETA
            oSched.ETB = pSched.ETB
            oSched.ETD = pSched.ETD
            oSched.ShipFace = pSched.ShipFace

            oSched.UpdPGID = C_PGID.VesselScheduleRegistration
            oSched.UpdTime = DateTime.Now
            oSched.UpdUserID = GlobalProperties.gsUserID
            oSched.Flag = False

            _db.Entry(oSched).State = EntityState.Modified
            _db.SaveChanges()

            rMsg = fgMsgOut("IBP002", "", "スケジュール")
            flUpdData = True

        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' AutoComplete ApplicantName (UserControl)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function fgVesseNoExist(VoyageNo As String) As BerthPlan.mVessel
        fgVesseNoExist = New BerthPlan.mVessel

        Try
            Dim VesselCD As String = _db.tSchedule.AsNoTracking.Where(Function(s) s.VoyageNo = VoyageNo And s.Flag = False).Select(Function(s) s.VesselCD).FirstOrDefault()
            fgVesseNoExist = _db.mVessel.AsNoTracking.Where(Function(v) v.VesselCD = VesselCD).FirstOrDefault

        Catch ex As Exception

        End Try
    End Function

#End Region

End Class