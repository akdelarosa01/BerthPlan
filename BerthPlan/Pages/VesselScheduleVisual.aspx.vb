#Region "## インポート ##"

Imports System.Web.Services
Imports System.Web.Script.Services
Imports BerthPlan.GlobalFunction
Imports ClosedXML.Excel

#End Region

''' <summary>
''' カレンダークラス
''' </summary>
''' <remarks></remarks>
Partial Public Class Calendar
    Public Property Resource As Object
    Public Property Events As Object
    Public Property Status As String
End Class

''' <summary>
''' 本船スケジュールのVisual
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/12      AK.Dela Rosa　　デザインを作った。
'''    00.02      2020/03/31      AV.Tung         コーディングを開始しました。
'''    00.03      2020/04/02      AK.Dela Rosa    EXCEL レポートを作った。
''' </history>

Public Class VesselScheduleVisual
    Inherits System.Web.UI.Page

#Region "## クラス内変数 ## "
    Public Shared db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities
    ''' <summary>EXCELファイルの名前</summary>
    Public Shared C_ExcelName As String = "本船スケジュール.xlsx"

    Structure Column
        Public ColLetter As String
        Public ColTime As String
    End Structure

#End Region

#Region "## コントロールイベントの定義 ## "
    ''' <summary>
    ''' ページ読み込み機能
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("UserID")) Then
            Response.Redirect("~/Login.aspx?SessionExpire")
            Exit Sub
        End If
        Call fgCheckSession()
    End Sub
#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' スケジュール
    ''' </summary>
    ''' <param name="ETA"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function ScheduleResource(ByVal ETA As Date, ByVal Wharf As String) As Object
        ScheduleResource = New Calendar
        Dim Resource As Object = Nothing
        Dim Events As Object = Nothing
        ScheduleResource.Status = "failed"
        Try
            'Check Session
            If fgCheckSession() = False Then
                ScheduleResource.Status = "expire"
                Exit Function
            End If

            Dim GetResource = (From s In db.tSchedule.AsNoTracking.ToList
                            Where (s.ETA >= ETA And s.ETA <= ETA.AddMonths(1)) And s.Flag = False
                            Select New With
                            {
                                .id = s.BerthID,
                                .ScheduleID = s.ScheduleID,
                                .WharfName = (From b In db.mBerth.AsNoTracking.Where(Function(x) x.Flag = False And x.BerthID = s.BerthID).ToList
                                                Group Join w In db.mWharf.AsNoTracking.Where(Function(x) x.Flag = False).ToList
                                                On w.WharfCD Equals b.WharfCD Into gr = Group
                                                From x In gr.DefaultIfEmpty()
                                                Select If(x Is Nothing, Nothing, x.WharfName)).FirstOrDefault,
                                .BerthName = GlobalFunction.fgNullToStr((From b In db.mBerth.AsNoTracking.ToList
                                                                         Where b.Flag = 0 And b.BerthID = s.BerthID
                                                                        Select b.BerthName).FirstOrDefault)
                            }).OrderBy(Function(s) s.WharfName)

            Dim GetEvents = (From s In db.tSchedule.AsNoTracking.ToList
                          Where (s.ETA >= ETA And s.ETA <= ETA.AddMonths(1)) And s.Flag = False
                          Select New With
                            {
                            .id = s.ScheduleID,
                            .resourceId = s.BerthID,
                            .start = s.ETB.ToString("yyyy/MM/dd HH:mm"),
                            .end = s.ETD.ToString("yyyy/MM/dd HH:mm"),
                            .backgroundColor = GlobalFunction.fgNullToStr((From c In db.mCompany.AsNoTracking.ToList Where c.Flag = 0 And c.ApplicantCD = s.ApplicantCD Select c.Color).FirstOrDefault),
                            .borderColor = GlobalFunction.fgNullToStr((From c In db.mCompany.AsNoTracking.ToList Where c.Flag = 0 And c.ApplicantCD = s.ApplicantCD Select c.Color).FirstOrDefault),
                            .title = "",
                            .ScheduleID = s.ScheduleID,
                            .VesselCD = s.VesselCD,
                            .VesselName = GlobalFunction.fgNullToStr((From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.VesselName).FirstOrDefault),
                            .LOA = GlobalFunction.fgNullToStr((From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.LOA).FirstOrDefault),
                            .GrossTon = GlobalFunction.fgNullToStr((From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.GrossTon).FirstOrDefault),
                            .VoyageNo = s.VoyageNo,
                            .BerthID = s.BerthID,
                            .BerthName = GlobalFunction.fgNullToStr((From b In db.mBerth.AsNoTracking.ToList Where b.Flag = 0 And b.BerthID = s.BerthID Select b.BerthName).FirstOrDefault),
                            .ApplicantCD = s.ApplicantCD,
                            .ApplicantName = GlobalFunction.fgNullToStr((From c In db.mCompany.AsNoTracking.ToList Where c.Flag = 0 And c.ApplicantCD = s.ApplicantCD Select c.ApplicantName).FirstOrDefault),
                            .PilotCD = s.PilotCD,
                            .PilotName = GlobalFunction.fgNullToStr((From p In db.mPilot.AsNoTracking.ToList Where p.Flag = 0 And p.PilotCD = s.PilotCD Select p.PilotName).FirstOrDefault),
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
                            .Flag = s.Flag,
                            .WharfName = GlobalFunction.fgNullToStr((From b In db.mBerth.AsNoTracking.Where(Function(x) x.Flag = False And x.BerthID = s.BerthID).ToList
                                            Group Join w In db.mWharf.AsNoTracking.Where(Function(x) x.Flag = False).ToList
                                            On w.WharfCD Equals b.WharfCD Into gr = Group
                                            From x In gr.DefaultIfEmpty()
                                            Select If(x Is Nothing, Nothing, x.WharfName)).FirstOrDefault)
                    })

            If Wharf = String.Empty Or Wharf.ToUpper = "NULL" Then
                Resource = GetResource.ToList
                Events = GetEvents.ToList
            Else
                Resource = GetResource.Where(Function(s) s.WharfName = Wharf).ToList
                Events = GetEvents.Where(Function(s) s.WharfName = Wharf).ToList
            End If

            ScheduleResource.Resource = Resource
            ScheduleResource.Events = Events
            ScheduleResource.Status = "success"
        Catch ex As Exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' EXCELファイルを印刷
    ''' </summary>
    ''' <param name="ETA"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()>
    Public Shared Function flExportExcel(ByVal ETA As Date, ByVal Wharf As String) As MyResult
        flExportExcel = New MyResult
        flExportExcel.Msg = fgMsgOut("EBP005", "")
        flExportExcel.Status = "failed"

        Dim iCount As Integer = 0

        Dim lastWharf As String = String.Empty
        Dim wharfStartRow As Integer = 0
        Dim wharfLastRow As Integer = 0

        Dim objWorkBook As XLWorkbook = Nothing
        Dim objWorkSheet As IXLWorksheet = Nothing

        Dim sOpenFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/Templates/")
        Dim sOpenFileName As String = C_ExcelName

        ' ダウンロードファイル
        Dim sSaveFilePath As String = HttpContext.Current.Server.MapPath("~/Assets/SaveFile/")
        Dim sSaveFileName As String = String.Empty
        Dim ArrWharf() As String = Nothing
        Dim ArrBerth(,) As String = Nothing
        Dim Resource As Object = Nothing
        Dim Events As Object = Nothing

        Try
            'Check Session
            If fgCheckSession() = False Then
                flExportExcel.Status = "expire"
                Exit Function
            End If

            If String.IsNullOrEmpty(ETA.ToString) Then
                flExportExcel.Msg = fgMsgOut("EBP005", "")
                flExportExcel.Status = "failed"

                Exit Function
            End If

            Dim GetResource = (From s In db.tSchedule.AsNoTracking.ToList
                            Where s.ETA.ToString("yyyy/MM/dd") >= ETA.ToString("yyyy/MM/dd") _
                            And s.ETA.ToString("yyyy/MM/dd") <= ETA.AddDays(6).ToString("yyyy/MM/dd") _
                            And s.Flag = False
                                Select New With
                                {
                                    .id = s.BerthID,
                                    .ScheduleID = s.ScheduleID,
                                    .WharfName = (From b In db.mBerth.AsNoTracking.Where(Function(x) x.Flag = False And x.BerthID = s.BerthID).ToList
                                                    Group Join w In db.mWharf.AsNoTracking.Where(Function(x) x.Flag = False).ToList
                                                    On w.WharfCD Equals b.WharfCD Into gr = Group
                                                    From x In gr.DefaultIfEmpty()
                                                    Select If(x Is Nothing, Nothing, x.WharfName)).FirstOrDefault,
                                    .BerthName = GlobalFunction.fgNullToStr((From b In db.mBerth.AsNoTracking.ToList
                                                                             Where b.Flag = 0 And b.BerthID = s.BerthID
                                                                            Select b.BerthName).FirstOrDefault)
                                }).OrderBy(Function(s) s.WharfName).ToArray
            Dim GetEvents = (From s In db.tSchedule.AsNoTracking.ToList
                    Where s.ETA.ToString("yyyy/MM/dd") >= ETA.ToString("yyyy/MM/dd") _
                    And s.ETA.ToString("yyyy/MM/dd") <= ETA.AddDays(7).ToString("yyyy/MM/dd") _
                    And s.Flag = False
                        Select New With
                        {
                            .id = s.ScheduleID,
                            .resourceId = s.BerthID,
                            .start = s.ETB,
                            .end = s.ETD,
                            .backgroundColor = GlobalFunction.fgNullToStr((From c In db.mCompany.AsNoTracking.ToList Where c.Flag = 0 And c.ApplicantCD = s.ApplicantCD Select c.Color).FirstOrDefault),
                            .ScheduleID = s.ScheduleID,
                            .VesselCD = s.VesselCD,
                            .VesselName = GlobalFunction.fgNullToStr((From v In db.mVessel.AsNoTracking.ToList Where v.Flag = 0 And v.VesselCD = s.VesselCD Select v.VesselName).FirstOrDefault),
                            .VoyageNo = s.VoyageNo,
                            .BerthID = s.BerthID,
                            .BerthName = GlobalFunction.fgNullToStr((From bt In db.mBerth.AsNoTracking.ToList Where bt.Flag = 0 And bt.BerthID = s.BerthID Select bt.BerthName).FirstOrDefault),
                            .ApplicantCD = s.ApplicantCD,
                            .ETA = s.ETA,
                            .ETB = s.ETB,
                            .ETD = s.ETD,
                            .ShipFace = s.ShipFace,
                            .WharfName = GlobalFunction.fgNullToStr((From b In db.mBerth.AsNoTracking.Where(Function(x) x.Flag = False And x.BerthID = s.BerthID).ToList
                                        Group Join w In db.mWharf.AsNoTracking.Where(Function(x) x.Flag = False).ToList
                                        On w.WharfCD Equals b.WharfCD Into gr = Group
                                        From x In gr.DefaultIfEmpty()
                                        Select If(x Is Nothing, Nothing, x.WharfName)).FirstOrDefault)
                    }).OrderBy(Function(s) s.ETB).ToArray

            If Wharf = String.Empty Or Wharf.ToUpper = "NULL" Then
                Resource = GetResource.ToArray
                Events = GetEvents.ToArray
            Else
                Resource = GetResource.Where(Function(s) s.WharfName = Wharf).ToArray
                Events = GetEvents.Where(Function(s) s.WharfName = Wharf).ToArray
            End If


            objWorkBook = New XLWorkbook(sOpenFilePath & sOpenFileName)

            sSaveFileName = "本船スケジュール_" & Date.Now.ToString("yyyy-MM-dd-HH-mm-ss") & ".xlsx"

            'Making of Reports
            slPlaceWharfBerth(objWorkBook, objWorkSheet, ETA, ArrWharf, Resource, Events)

            objWorkBook.Worksheet(1).SetTabActive()

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
            flExportExcel.Data = ReturnData
            flExportExcel.Msg = fgMsgOut("IBP004", "", sSaveFileName)
            flExportExcel.Status = "success"

            ' ダウンロードしたファイルをサーバーから削除する
            'System.IO.File.Delete(sSaveFilePath & sSaveFileName)
            If IsNothing(objWorkSheet) = False Then
                objWorkSheet = Nothing
            End If

            If IsNothing(objWorkBook) = False Then
                objWorkBook.Dispose()
                objWorkBook = Nothing
            End If

        Catch ex As Exception
            flExportExcel = sgErrProc(ex)
        End Try
        Return flExportExcel
    End Function

    ''' <summary>
    ''' レポートを作る
    ''' </summary>
    ''' <param name="objWorkBook"></param>
    ''' <param name="objWorkSheet"></param>
    ''' <param name="ETA"></param>
    ''' <param name="ArrWharf"></param>
    ''' <param name="wharfEvents"></param>
    ''' <param name="berthEvents"></param>
    ''' <remarks></remarks>
    Public Shared Sub slPlaceWharfBerth(ByVal objWorkBook As IXLWorkbook, ByVal objWorkSheet As IXLWorksheet, _
                                 ByVal ETA As Date, ArrWharf() As String, ByVal wharfEvents As Array, ByVal berthEvents As Array)
        Try
            For index = 1 To 7
                objWorkSheet = objWorkBook.Worksheet(index)
                Dim sheetname As String = ETA.AddDays(index - 1).ToString("yyyy年MM月dd日")

                objWorkSheet.Name = sheetname

                objWorkSheet.Cell("DE1").Value = sheetname
                objWorkSheet.Cell("DV1").Value = ETA.ToString("yyyy年MM月dd日")
                objWorkSheet.Cell("EH1").Value = ETA.AddDays(6).ToString("yyyy年MM月dd日")

                Dim wIndex = 0
                Dim bIndex = 0
                ReDim ArrWharf(wharfEvents.Length - 1)

                For Each w In wharfEvents
                    If Not ArrWharf.Contains(w.WharfName) Then
                        ArrWharf(wIndex) = w.WharfName

                        wIndex += 1

                    End If
                Next

                'count berth in wharf
                Dim wharfRow = 6
                Dim nxtWharf = 0
                Dim iRow As Integer = 6
                Dim rowMulti As Integer = 4
                Dim lastColumn As String = "EQ"

                With objWorkSheet
                    For Each w In ArrWharf
                        If Not IsNothing(w) Then
                            Dim berths = (From wf In wharfEvents
                                          Where wf.WharfName = w
                                          Select wf.BerthName).Distinct()
                            For Each b In berths

                                'Setting additional Row
                                Dim RowBy4 As Integer = iRow + rowMulti

                                'Merging Column B rows.
                                .Range("B" & iRow.ToString() & ":B" & RowBy4.ToString()).Column(1).Merge()

                                'Writing Berth names.
                                .Cell(iRow, "B").Value = b

                                'Designing B columns.
                                .Cell(iRow, "B").Style.Alignment.WrapText = True
                                .Cell(iRow, "B").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                .Cell(iRow, "B").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)

                                'Drawing Bottom border from Column A to LAST Column.
                                .Range("A" & RowBy4.ToString() & ":" & lastColumn & RowBy4.ToString()).Style.Border.BottomBorder = XLBorderStyleValues.Thin

                                'Get Berth ID etc.
                                Dim berthInfo = (From wf In wharfEvents
                                                Where wf.WharfName = w And wf.BerthName = b
                                                Select wf.id, wf.ScheduleID, wf.BerthName).ToArray

                                For Each bi In berthInfo
                                    'Get Berth Events 
                                    Dim berthE = (From bb In berthEvents
                                                  Where bi.ScheduleID = bb.ScheduleID And _
                                                  bi.id = bb.BerthID
                                                  Select bb).ToList
                                    Dim startCell = ""
                                    Dim endCell = ""

                                    For Each bE In berthE

                                        'Check if ETB DATE is for different sheet
                                        If Not CDate(bE.ETB).ToString("yyyy年MM月dd日") = sheetname Then
                                            Exit For
                                        End If

                                        startCell = iColumn(bE.start) & iRow.ToString
                                        endCell = iColumn(bE.end, 0) & RowBy4.ToString

                                        Dim etdDay As Integer = CDate(bE.ETD).Day
                                        Dim etbDay As Integer = CDate(bE.ETB).Day

                                        'Getting Background Color for events
                                        Dim bgColor As String = If(String.IsNullOrEmpty(bE.backgroundColor), "#3a87ad", bE.backgroundColor)

                                        'Checking schedule if have overnight event
                                        If (etdDay - etbDay) > 0 Then
                                            endCell = lastColumn & RowBy4.ToString

                                            'Coloring Events
                                            .Range(startCell, endCell).Style.Fill.BackgroundColor = XLColor.FromHtml(bgColor)

                                            'Setting Information for events
                                            .Cell(iColumn(bE.start) & iRow.ToString).Value = bE.VoyageNo
                                            .Cell(iColumn(bE.start) & (iRow + 1).ToString).Value = bE.VesselName
                                            .Cell(iColumn(bE.start) & (iRow + 2).ToString).Value = "ETB: " & CDate(bE.start).ToString("yyyy年MM月dd日 HH:mm")
                                            .Cell(iColumn(bE.start) & (iRow + 3).ToString).Value = "ETD: " & CDate(bE.end).ToString("yyyy年MM月dd日 HH:mm")
                                            .Cell(iColumn(bE.start) & (iRow + 4).ToString).Value = If(bE.ShipFace, "▼", "▲")

                                            Dim EndCellDate = CDate(bE.end).ToString("HH:mm")
                                            Dim checkEndCellDate = CDate(EndCellDate)

                                            If checkEndCellDate > #12:00:00 AM# And index < 7 Then

                                                'Continue overnight events for next day
                                                Dim nxtWorkSheet = objWorkBook.Worksheet(index + 1)
                                                startCell = "D" & iRow.ToString
                                                endCell = iColumn(bE.end, 0) & RowBy4.ToString

                                                'Continue coloring event for overnight event
                                                nxtWorkSheet.Range(startCell, endCell).Style.Fill.BackgroundColor = XLColor.FromHtml(bgColor)

                                                'Setting Information for overnight events
                                                nxtWorkSheet.Cell("D" & iRow.ToString).Value = bE.VoyageNo
                                                nxtWorkSheet.Cell("D" & (iRow + 1).ToString).Value = bE.VesselName
                                                nxtWorkSheet.Cell("D" & (iRow + 2).ToString).Value = "ETB: " & CDate(bE.start).ToString("yyyy年MM月dd日 HH:mm")
                                                nxtWorkSheet.Cell("D" & (iRow + 3).ToString).Value = "ETD: " & CDate(bE.end).ToString("yyyy年MM月dd日 HH:mm")
                                                nxtWorkSheet.Cell("D" & (iRow + 4).ToString).Value = If(bE.ShipFace, "▼", "▲")
                                            End If
                                            
                                        Else
                                            'Coloring Events
                                            .Range(startCell, endCell).Style.Fill.BackgroundColor = XLColor.FromHtml(bgColor)

                                            'Setting Information for events
                                            .Cell(iColumn(bE.start) & iRow.ToString).Value = bE.VoyageNo
                                            .Cell(iColumn(bE.start) & (iRow + 1).ToString).Value = bE.VesselName
                                            .Cell(iColumn(bE.start) & (iRow + 2).ToString).Value = "ETB: " & CDate(bE.start).ToString("yyyy年MM月dd日 HH:mm")
                                            .Cell(iColumn(bE.start) & (iRow + 3).ToString).Value = "ETD: " & CDate(bE.end).ToString("yyyy年MM月dd日 HH:mm")
                                            .Cell(iColumn(bE.start) & (iRow + 4).ToString).Value = If(bE.ShipFace, "▼", "▲")
                                        End If
                                    Next

                                Next

                                iRow += rowMulti + 1
                                nxtWharf = iRow
                            Next

                            If Not IsNothing(berths) Then
                                'Setting the value of WharfName
                                .Cell(wharfRow, "A").Value = w
                                'Designing Wharf Column
                                .Range("A" & wharfRow.ToString() & ":A" & (nxtWharf - 1).ToString()).Column(1).Merge()
                                .Cell(wharfRow, "A").Style.Alignment.WrapText = True
                                .Cell(wharfRow, "A").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                .Cell(wharfRow, "A").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                wharfRow = nxtWharf
                            End If
                        End If
                    Next

                End With
            Next
        Catch ex As Exception
            Throw
        End Try

    End Sub

    ''' <summary>
    ''' Get Column Value
    ''' </summary>
    ''' <param name="estimatedDate"></param>
    ''' <param name="addTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function iColumn(ByVal estimatedDate As Date, Optional ByVal addTime As Integer = 1) As String
        iColumn = String.Empty
        Dim letter() As String = Nothing
        Dim time() As String = Nothing
        Dim fromDBDate As Date = Nothing
        Dim timeCol As Date = Nothing
        Dim timeColnext As Date = Nothing

        Try
            ReDim letter(96)
            ReDim time(96)

            'Excel Column
            letter = {"D", "E", "F", "G", "H", "I", "J", "K", "L", "M", _
                      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", _
                      "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", _
                      "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", _
                      "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", _
                      "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", _
                      "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", _
                      "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", _
                      "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", _
                      "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", _
                      "CZ", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", _
                      "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", _
                      "DT", "DU", "DV", "DW", "DX", "DY", "DZ", "EA", "EB", "EC", _
                      "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", _
                      "EN", "EO", "EP", "EQ"}

            'Time By 10 minutes
            time = {"00:00", "00:10", "00:20", "00:30", "00:40", "00:50", "01:00", "01:10", "01:20", "01:30", "01:40", "01:50", _
                    "02:00", "02:10", "02:20", "02:30", "02:40", "02:50", "03:00", "03:10", "03:20", "03:30", "03:40", "03:50", _
                    "04:00", "04:10", "04:20", "04:30", "04:40", "04:50", "05:00", "05:10", "05:20", "05:30", "05:40", "05:50", _
                    "06:00", "06:10", "06:20", "06:30", "06:40", "06:50", "07:00", "07:10", "07:20", "07:30", "07:40", "07:50", _
                    "08:00", "08:10", "08:20", "08:30", "08:40", "08:50", "09:00", "09:10", "09:20", "09:30", "09:40", "09:50", _
                    "10:00", "10:10", "10:20", "10:30", "10:40", "10:50", "11:00", "11:10", "11:20", "11:30", "11:40", "11:50", _
                    "12:00", "12:10", "12:20", "12:30", "12:40", "12:50", "13:00", "13:10", "13:20", "13:30", "13:40", "13:50", _
                    "14:00", "14:10", "14:20", "14:30", "14:40", "14:50", "15:00", "15:10", "15:20", "15:30", "15:40", "15:50", _
                    "16:00", "16:10", "16:20", "16:30", "16:40", "16:50", "17:00", "17:10", "17:20", "17:30", "17:40", "17:50", _
                    "18:00", "18:10", "18:20", "18:30", "18:40", "18:50", "19:00", "19:10", "19:20", "19:30", "19:40", "19:50", _
                    "20:00", "20:10", "20:20", "20:30", "20:40", "20:50", "21:00", "21:10", "21:20", "21:30", "21:40", "21:50", _
                    "22:00", "22:10", "22:20", "22:30", "22:40", "22:50", "23:00", "23:10", "23:20", "23:30", "23:40", "23:50"}

            fromDBDate = estimatedDate.ToString("yyyy/MM/dd HH:mm")

            For i = 0 To time.Length - 1

                timeCol = CDate(estimatedDate.ToString("yyyy/MM/dd") & " " & time(i))
                timeColnext = CDate(estimatedDate.ToString("yyyy/MM/dd") & " " & time(i + 1))

                If fromDBDate >= timeCol And _
                    fromDBDate <= timeColnext Then

                    If fromDBDate = timeColnext And addTime > 0 Then
                        iColumn = letter(i + 1)
                        Exit For
                    End If

                    iColumn = letter(i)
                    Exit For
                End If
            Next

        Catch ex As Exception
            Throw
        End Try

        Return iColumn
    End Function

#End Region

End Class