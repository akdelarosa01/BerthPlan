#Region "## インポート ##"
Imports System.Web.Services
Imports System.Web.Script.Services
Imports BerthPlan.GlobalFunction
Imports ClosedXML.Excel

#End Region

Public Class VesselScheduleList
    Inherits System.Web.UI.Page

#Region "## クラス内変数 ## "
    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities
    Public Shared Auth As Authentication = New Authentication()

    Partial Public Class TableSchedule
        Public Property ScheduleID As Integer
        Public Property VesselCD As String
        Public Property VesselName As String
        Public Property VoyageNo As String
        Public Property BerthID As Integer
        Public Property BerthName As String
        Public Property ApplicantCD As String
        Public Property ApplicantName As String
        Public Property PilotCD As String
        Public Property PilotName As String
        Public Property PilotGuide As Boolean
        Public Property Tag As Boolean
        Public Property LineBoat As Boolean
        Public Property ETA As Date
        Public Property ETB As Date
        Public Property ETD As Date
        Public Property ShipFace As Boolean
        Public Property LOA As Decimal
        Public Property IO As Boolean
        Public Property GrossTon As Decimal
    End Class


    Partial Public Class ConflictDate
        Public Property dbData As Object
        Public Property tableData As Object
    End Class
#End Region

#Region "## コントロールイベントの定義 ## "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("UserID")) Then
            Response.Redirect("~/Login.aspx?SessionExpire")
            Exit Sub
        End If
        Call fgCheckSession()
    End Sub

#End Region

#Region "## WebMethod ##"

    ''' <summary>
    ''' フォーム検証  データ取得機能
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function GetScheduleList(StartETA As DateTime, EndETA As DateTime, VesselCD As String, ApplicantCD As String, PilotCD As String) As MyResult
        GetScheduleList = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                GetScheduleList.Status = "expire"
                Exit Function
            End If

            If (From c In db.mVessel.AsNoTracking.ToList Where c.VesselCD = VesselCD And c.Flag = False).Count = 0 And VesselCD <> "" Then
                GetScheduleList.Msg = fgMsgOut("EBP002", "", "本船")
                GetScheduleList.Status = "failed"
                Exit Function
            End If

            If (From c In db.mCompany.AsNoTracking.ToList Where c.ApplicantCD = ApplicantCD And c.Flag = False).Count = 0 And ApplicantCD <> "" Then
                GetScheduleList.Msg = fgMsgOut("EBP002", "", "申請者")
                GetScheduleList.Status = "failed"
                Exit Function
            End If

            If (From c In db.mPilot.AsNoTracking.ToList Where c.PilotCD = PilotCD And c.Flag = False).Count = 0 And PilotCD <> "" Then
                GetScheduleList.Msg = fgMsgOut("EBP002", "", "パイロット")
                GetScheduleList.Status = "failed"
                Exit Function
            End If

            GetScheduleList.Data = (From s In db.tSchedule.AsNoTracking.ToList
                 Where (s.ETA >= StartETA And s.ETA <= EndETA And s.VesselCD.ToUpper.Contains(VesselCD.ToUpper) And
                        s.ApplicantCD.ToUpper.Contains(ApplicantCD) And
                        s.PilotCD.ToUpper.Contains(PilotCD)) And s.Flag = False
                 Select New With
                    {
                      .ScheduleID = s.ScheduleID,
                      .VesselCD = s.VesselCD,
                      .VesselName = (From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.VesselName).FirstOrDefault,
                      .LOA = (From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.LOA).FirstOrDefault,
                      .GrossTon = (From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.GrossTon).FirstOrDefault,
                      .IO = (From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.IO).FirstOrDefault,
                      .VoyageNo = s.VoyageNo,
                      .BerthID = s.BerthID,
                      .BerthName = (From b In db.mBerth.AsNoTracking.ToList Where b.Flag = 0 And b.BerthID = s.BerthID Select b.BerthName).FirstOrDefault,
                      .ApplicantCD = s.ApplicantCD,
                      .ApplicantName = (From c In db.mCompany.AsNoTracking.ToList Where c.Flag = 0 And c.ApplicantCD = s.ApplicantCD Select c.ApplicantName).FirstOrDefault,
                      .PilotCD = s.PilotCD,
                      .PilotName = (From p In db.mPilot.AsNoTracking.ToList Where p.Flag = 0 And p.PilotCD = s.PilotCD Select p.PilotName).FirstOrDefault,
                      .PilotGuide = s.PilotGuide,
                      .Tag = s.Tag,
                      .LineBoat = s.LineBoat,
                      .ETA = s.ETA,
                      .ETB = s.ETB,
                      .ETD = s.ETD,
                      .ShipFace = s.ShipFace,
                      .UpdTime = s.UpdTime,
                      .UpdUserID = s.UpdUserID,
                      .UpdPGID = s.UpdPGID,
                      .TimeStamp = s.TimeStamp,
                      .Flag = s.Flag
                }).OrderBy(Function(x) x.VesselName).ThenBy(Function(x) x.VoyageNo).ToList
        Catch ex As Exception
            GetScheduleList = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 保存機能
    ''' </summary>
    ''' <param name="tSchedule"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function UpdateSchedule(tSchedule As List(Of BerthPlan.tSchedule)) As MyResult
        UpdateSchedule = New MyResult
        UpdateSchedule.Status = "failed"
        Dim iCount As Integer = 0

        Try
            'Check Session
            If fgCheckSession() = False Then
                UpdateSchedule.Status = "expire"
                Exit Function
            End If


            Dim ScheduleID = tSchedule.Select(Function(x) x.ScheduleID).ToArray
            Dim dbtSchedule = db.tSchedule.AsNoTracking.Where(Function(x) Not ScheduleID.Contains(x.ScheduleID)).ToList
            Dim Conflict As New ConflictDate
            For Each s In tSchedule
                iCount += 1

                If flCheckUpdDate(s.UpdTime, (From x In _db.tSchedule.AsNoTracking.ToList() Where x.ScheduleID = s.ScheduleID Select x.UpdTime).FirstOrDefault()) = False Then
                    UpdateSchedule.Msg = fgMsgOut("EXX004", "")
                    UpdateSchedule.Status = "failed"
                    Exit Function
                End If

                Dim dbData = tSchedule.Where(Function(x) (x.ETB >= s.ETB And x.ETB <= s.ETD Or x.ETD >= s.ETB And x.ETD <= s.ETD Or s.ETB >= x.ETB And s.ETB <= x.ETD) And x.BerthID = s.BerthID And x.ScheduleID <> s.ScheduleID And x.Flag = False).ToList
                Dim tableData = dbtSchedule.Where(Function(x) (x.ETB >= s.ETB And x.ETB <= s.ETD Or x.ETD >= s.ETB And x.ETD <= s.ETD Or s.ETB >= x.ETB And s.ETB <= x.ETD) And x.BerthID = s.BerthID And x.ScheduleID <> s.ScheduleID And x.Flag = False).ToList

                If dbData.Count <> 0 Or tableData.Count <> 0 Then
                    UpdateSchedule.Msg = "登録時にテーブル表示にいくつかの矛盾する日付があります."
                    UpdateSchedule.Status = "failed"
                    Conflict.dbData = dbData
                    Conflict.tableData = tableData
                    UpdateSchedule.Data = Conflict
                    Exit Function
                End If

            Next

            For Each s In tSchedule
                Dim Update = (From sRow In db.tSchedule.Where(Function(x) x.ScheduleID = s.ScheduleID)).FirstOrDefault

                Update.PilotCD = s.PilotCD
                Update.ApplicantCD = s.ApplicantCD
                Update.BerthID = s.BerthID
                Update.VoyageNo = s.VoyageNo
                Update.VesselCD = s.VesselCD
                Update.PilotGuide = s.PilotGuide
                Update.Tag = s.Tag
                Update.LineBoat = s.LineBoat
                Update.ETA = s.ETA
                Update.ETB = s.ETB
                Update.ETD = s.ETD
                Update.ShipFace = s.ShipFace
                Update.UpdPGID = "VesselScheduleList"
                Update.UpdTime = Date.Now
                Update.UpdUserID = Auth.userID
                Update.Flag = False

                db.Entry(Update).State = EntityState.Modified
                db.SaveChanges()
            Next

            UpdateSchedule.Msg = fgMsgOut("IBP002", "", "本船スケジュール")
            UpdateSchedule.Status = "success"
        Catch ex As Exception
            UpdateSchedule = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 印刷
    ''' </summary>
    ''' <param name="tSchedule"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
                       <WebMethod()>
    Public Shared Function PrintSchedule(tSchedule As List(Of TableSchedule)) As MyResult
        PrintSchedule = New MyResult
        PrintSchedule.Status = "failed"
        Dim iCount As Integer = 0
        Dim iRow As Integer = 4
        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing

        Dim sOpenFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/Templates/")
        Dim sOpenFileName As String = "本船スケジュール一覧.xlsx"

        ' ダウンロードファイル
        Dim sSaveFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/SaveFile/")
        Dim sSaveFileName As String = String.Empty
        Try
            'Check Session
            If fgCheckSession() = False Then
                PrintSchedule.Status = "expire"
                Exit Function
            End If

            Dim ScheduleID = tSchedule.Select(Function(x) x.ScheduleID).ToArray
            Dim dbtSchedule = db.tSchedule.AsNoTracking.Where(Function(x) Not ScheduleID.Contains(x.ScheduleID)).ToList
            For Each s In tSchedule
                iCount += 1

                If tSchedule.Where(Function(x) (x.ETB >= s.ETB And x.ETB <= s.ETD Or x.ETD >= s.ETB And x.ETD <= s.ETD Or s.ETB >= x.ETB And s.ETB <= x.ETD) And x.BerthID= s.BerthID And x.ScheduleID <> s.ScheduleID).Count <> 0 Then
                    PrintSchedule.Msg = "登録時にテーブル表示にいくつかの矛盾する日付があります. 行 " + iCount.ToString
                    PrintSchedule.Status = "failed"
                    Exit Function
                End If

                If dbtSchedule.Where(Function(x) (x.ETB >= s.ETB And x.ETB <= s.ETD Or x.ETD >= s.ETB And x.ETD <= s.ETD Or s.ETB >= x.ETB And s.ETB <= x.ETD) And x.BerthID= s.BerthID And x.ScheduleID <> s.ScheduleID And x.Flag = False).Count <> 0 Then
                    PrintSchedule.Msg = fgMsgOut("EBP003", "行 " + iCount.ToString)
                    PrintSchedule.Status = "failed"
                    Exit Function
                End If
            Next

            objWorkBook = New XLWorkbook(sOpenFilePath & sOpenFileName)

            sSaveFileName = "本船スケジュール一覧" & Date.Now.ToString("yyyyMMddHHmmssfff") & ".xlsx"
            objWorkSheet = objWorkBook.Worksheet(1)


            With objWorkSheet.Range("A4", "M" & (tSchedule.Count + 3).ToString)
                .Style.Border.TopBorder = XLBorderStyleValues.Thin
                .Style.Border.InsideBorder = XLBorderStyleValues.Thin
                .Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                .Style.Border.LeftBorder = XLBorderStyleValues.Thin
                .Style.Border.RightBorder = XLBorderStyleValues.Thin
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin
            End With

            With objWorkSheet
                .Style.NumberFormat.Format = "@"
                For Each s In tSchedule
                    .Cell(iRow, 1).Value = s.VesselName
                    .Cell(iRow, 2).Value = s.VoyageNo
                    .Cell(iRow, 3).Value = Format(s.LOA, "###,###,###,###,##0.0###")
                    .Cell(iRow, 4).Value = If(s.IO = True, "外", "内")
                    .Cell(iRow, 5).Value = Format(s.GrossTon, "###,###,###,###,##0.0###")
                    .Cell(iRow, 6).Value = s.BerthName
                    .Cell(iRow, 7).Value = s.ETA.ToString("yyyy/MM/dd HH:mm")
                    .Cell(iRow, 8).Value = s.ETB.ToString("yyyy/MM/dd HH:mm")
                    .Cell(iRow, 9).Value = s.ETD.ToString("yyyy/MM/dd HH:mm")
                    .Cell(iRow, 10).Value = If(s.ShipFace = True, "右", "左")
                    .Cell(iRow, 11).Value = If(s.PilotGuide = True, "要", "不要")
                    .Cell(iRow, 12).Value = If(s.LineBoat = True, "要", "不要")
                    .Cell(iRow, 13).Value = If(s.Tag = True, "要", "不要")
                    iRow += 1
                Next
            End With

            'ワークブックを保存する
            objWorkBook.SaveAs(sSaveFilePath & sSaveFileName)
            '後処理
            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If

            'ファイルダウンロード
            Dim ReturnData = New String() {sSaveFilePath, sSaveFileName}
            PrintSchedule.Data = ReturnData
            PrintSchedule.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            PrintSchedule.Status = "success"

            ' ダウンロードしたファイルをサーバーから削除する
            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If
            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If
        Catch ex As Exception
            PrintSchedule = sgErrProc(ex)
        End Try
    End Function

    ''' <summary>
    ''' 掲示板を削除する機能
    ''' </summary>
    ''' <param name="tSchedule"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
                                                           <WebMethod()>
    Public Shared Function DeleteSchedule(tSchedule As List(Of BerthPlan.tSchedule)) As MyResult
        DeleteSchedule = New MyResult
        Try
            'Check Session
            If fgCheckSession() = False Then
                DeleteSchedule.Status = "expire"
                Exit Function
            End If

            For Each s In tSchedule
                If flCheckUpdDate(s.UpdTime, (From x In _db.tSchedule.AsNoTracking.ToList() Where x.ScheduleID = s.ScheduleID Select x.UpdTime).FirstOrDefault()) = False Then
                    DeleteSchedule.Msg = fgMsgOut("EXX004", "")
                    DeleteSchedule.Status = "failed"
                    Exit Function
                End If
            Next

            For Each s In tSchedule
                Dim DSchedule = (From c In db.tSchedule
                                 Where c.ScheduleID = s.ScheduleID And c.Flag = False
                                 Select c).FirstOrDefault()
                DSchedule.UpdPGID = "VesselScheduleList"
                DSchedule.UpdTime = Date.Now
                DSchedule.UpdUserID = Auth.userID
                DSchedule.Flag = True
                db.Entry(DSchedule).State = EntityState.Modified
                db.SaveChanges()
                DeleteSchedule.Msg = fgMsgOut("IBP003", "")
                DeleteSchedule.Status = "success"
            Next
        Catch ex As Exception
            Exit Function
        End Try
    End Function

#End Region

End Class

