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
    Private Shared _db As BerthPlanEntities = New BerthPlanEntities
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

            'Check Session
            If fgCheckSession() = False Then
                flSearchVoyage.Status = C_Flag.CodeO
                Exit Function
            End If

            pVoyageNo = fgNullToStr(pVoyageNo)
            pVesselCD = fgNullToStr(pVesselCD)
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
    Private Shared Function flGetVessel(ByVal pVesselCD As String, _
                                        ByVal pVoyageNo As String, _
                                        ByRef rObj As Object) As Boolean
        Try
            flGetVessel = False

            'Check Vessel in master
            Dim iVessel = _db.mVessel.Where(Function(x) x.VesselCD = pVesselCD And x.Flag = False).ToList
            If iVessel.Count < 1 Then
                Exit Function
            End If

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
                rObj = (From x In iVessel
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
    Public Shared Function flRegister(ByVal pSched As tSchedule) As MyResult
        Dim sMsg As String = String.Empty

        Try
            flRegister = New MyResult
            flRegister.Status = C_Flag.CodeF

            'Check Session
            If fgCheckSession() = False Then
                flRegister.Status = C_Flag.CodeO
                Exit Function
            End If

            'Check Req Fields
            If fgNullToStr(pSched.ApplicantCD) = String.Empty Or fgNullToStr(pSched.PilotCD) = String.Empty Then
                flRegister.Msg = fgMsgOut("EBP006", "")
                Exit Function
            End If

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
    ''' Check Date
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function flCheckDate(ByVal pETA As DateTime, _
                                    ByVal pETB As DateTime, _
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
    Private Shared Function flCheckVoyage(ByVal pVoyageNo As String, ByVal pVesselCD As String, _
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
    Private Shared Function flInsData(ByVal pSched As tSchedule, ByRef rMsg As String) As Boolean
        Dim oSched As tSchedule = New tSchedule

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
    Public Shared Function flUpdData(ByVal pSched As tSchedule, ByRef rMsg As String) As Boolean
        Dim oSched As tSchedule = New tSchedule

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
    Public Shared Function fgVesseNoExist(VoyageNo As String) As mVessel
        fgVesseNoExist = New mVessel
        Try
            Dim VesselCD As String = _db.tSchedule.AsNoTracking.Where(Function(s) s.VoyageNo = VoyageNo And s.Flag = False).Select(Function(s) s.VesselCD).FirstOrDefault()
            fgVesseNoExist = _db.mVessel.AsNoTracking.Where(Function(v) v.VesselCD = VesselCD).FirstOrDefault
        Catch ex As Exception

        End Try
    End Function

#End Region

End Class