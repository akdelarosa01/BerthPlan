$(function () {
    var events = "";

    fgInit();

    fgGetWharf();

    $('#btnSearch').on('click', function (e) {
        flGetSchedule($("#SearchETA").val(), $('#SearchWharf').val(), $('#visual').fullCalendar('getView').type);
    });

    $('#schedule-registration').on('click', function (e) {
        var tGridView = $('#tblSchedule').DataTable();
        var data = tGridView.row($(this).parent()[0].parentNode).data();
        window.location.href = "VesselScheduleRegistration?VesselCD=" + $("#VesselCD").val() + "&VoyageNo=" + $("#VoyageNo").val();
    });

    $('body').on('click', 'button.fc-prev-button', function () {
        setDateOnPrevNext()
    });

    $('body').on('click', 'button.fc-next-button', function () {
        setDateOnPrevNext()
    });

    $('#btnExcel').on('click', function (e) {
        ShowLoading();

        if ($("#SearchETA").val() === "" || $("#SearchETA").val() === null) {
            msg("Please select a date.", "failed");

            HideLoading();
            e.stopPropagation();
        }
        
        $.ajax({
            url: "VesselScheduleVisual.aspx/flExportExcel",
            type: 'POST',
            data: JSON.stringify({ ETA: $("#SearchETA").val(), Wharf: $('#SearchWharf').val() }),
            contentType: 'application/json; charset=utf-8',
            datatype: "json",
        }).done(function (data, textStatus, xhr) {
            sessionOut(data.d.Status);
            if (data.d.Status === 'success') {
                window.location.href = '.../../../Handlers/FileDownloader.ashx?FilePath=' + data.d.Data[0] + '&FileName=' + data.d.Data[1];
                flGetSchedule($("#SearchETA").val(), $('#SearchWharf').val());
            }
            HideLoading();
            msg(data.d.Msg, data.d.Status);

        }).fail(function (xhr, textStatus, errorThrown) {
            HideLoading();
            msg(errorThrown, textStatus);
        });
    });

    //Shorcut Keys
    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1:
            case 112:
                e.preventDefault();
                flGetSchedule($("#SearchETA").val(), $('#SearchWharf').val());
                break;
            //F2: 
            case 113:
                e.preventDefault();
                break;
            //F3:
            case 114:
                e.preventDefault();
                break
            //F4:
            case 115:
                e.preventDefault();
                break;
            //F6: 
            case 117:
                e.preventDefault();
                break;
            //F8: 
            case 119:
                e.preventDefault();
                break;
            //F10: PRINT
            case 121:
                e.preventDefault();
                $('#btnExcel').click();
            //F12: CLOSE
            case 123:
                e.preventDefault();
                window.location.href = ".../../../Pages/SystemMenu"
                break;
            default:

        }
    });
});

function fgGetWharf() {
    var wharfOption = "";
    $('#SearchWharf').html(wharfOption);

    $.ajax({
        type: 'POST',
        url: ".../../../Masters/WharfMaster.aspx/flGetWharfList",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        wharfOption = '<option value="">ALL</option>';

        for (var i = 0; i < data.d.length; i++) {
            var wh = data.d[i];
            wharfOption += '<option value="' + wh.WharfName + '">' + wh.WharfName + '</option>';
        }

        
        $('#SearchWharf').html(wharfOption);

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
}

function setDateOnPrevNext() {
    var oldDate = $('#visual .fc-toolbar .fc-center h2').html();
    var yr = oldDate.substring(0, 4);
    var mm = oldDate.substring(5, 7);
    var dd = oldDate.substring(8, 10);
    var date = yr + '/' + mm + '/' + dd;

    $('#SearchETA').val(date);
    flGetSchedule($("#SearchETA").val(), $('#SearchWharf').val(), $('#visual').fullCalendar('getView').type);
}

function fgInit() {
    $('.textModal').prop('disabled', true);
    $("#SearchETA").val(DateNow());
    flGetSchedule($("#SearchETA").val(), $('#SearchWharf').val(), "timelineDay");
}

function flGetSchedule(ETA, Wharf,Type) {
    ShowLoading();
    $.ajax({
        type: 'POST',
        url: 'VesselScheduleVisual.aspx/ScheduleResource',
        data: '{ ETA: "' + ETA + '", Wharf: "' + Wharf + '"}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        sessionOut(data.d.Status);
        if (data.d.Events.length == 0) {
            msg(getMsg("E08"), "failed");
        }
        $.each(data.d.Events, function (index, value) {
            var ShipFace = (value.ShipFace == true) ? "▼" : "▲";
            data.d.Events[index].title = value.VoyageNo + "\n" +
                                         value.VesselName + "\n" +
                                         value.start + " - " + value.end + "\n" +
                                         ShipFace;
        });

        fgCalendar(data.d.Resource, data.d.Events, ETA, Type);
    }).fail(function (xhr, textStatus, errorThrown) {
        HideLoading();
        console.log('error');
    });
}

function fgCalendar(Resource, Events, ETA, Type) {
    ETA = (ETA == "") ? Date() : ETA;
    $('#visual').fullCalendar('destroy');
    $('#visual').fullCalendar({
        locale: "ja",
        now: DateNow(),
        defaultDate: ETA,
        slotLabelInterval: "01:00:00",
        titleFormat: "YYYY年MM月DD日 ddd",
        editable: false,
        scrollTime: '00:00',
        header: {
            left: 'today prev,next',
            center: 'title',
            right: 'timelineDay,timelineThreeDays,month'
        },
        defaultView: Type,
        buttonText: {
            timelineDay: '日',
            timelineThreeDays: '週',
            w: '週',
            month: '月',
            today: '今日'
        },
        views: {
            timelineThreeDays: {
                type: 'timeline',
                duration: { days: 7 },
                slotLabelFormat: ['MM月DD日 dddd', 'HH:mm'],
            },
            timelineDay: {
                slotLabelFormat: "HH:mm",
            },
            listWeek: {
                listDayAltFormat: "YYYY年MM月DD日",
            }
        },
        dayNamesShort: ['日', '月', '火', '水', '木', '金', '土'],
        dayNames: ['日曜日', '月曜日', '火曜日', '水曜日', '木曜日', '金曜日', '土曜日'],
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        monthNamesShort: ['01月', '02月', '03月', '04月', '05月', '06月', '07月', '08月', '09月', '10月', '11月', '12月'],
        resourceAreaWidth: '20%',
        resourceColumns: [
            {
                group: true,
                labelText: 'ワーフ',
                field: 'WharfName'
            },
            {
                labelText: 'バース',
                field: 'BerthName'
            }
        ],

        resources: Resource,
        events: Events,
        eventClick: function (x) {
            $("#VoyageNo").val(x.VoyageNo);
            $("#VesselCD").val(x.VesselCD);
            $("#VesselName").val(x.VesselName);

            console.log(x.BerthCD);

            $('#BerthID').val(x.BerthID);
            $('#BerthCD').val(x.BerthCD);
            $('#BerthName').val(x.BerthName);
            $('#ApplicantCD').val(x.ApplicantCD);
            $('#ApplicantName').val(x.ApplicantName);
            $('#PilotCD').val(x.PilotCD);
            $('#PilotName').val(x.PilotName);

            $('#ShipFace').val((x.ShipFace.toString() === 'true') ? '右' : '左');
            $('#PilotGuide').val((x.PilotGuide.toString() === 'true') ? 'Y' : 'N');
            $('#Tag').val((x.Tag.toString() === 'true') ? 'Y' : 'N');
            $('#LineBoat').val((x.LineBoat.toString() === 'true') ? 'Y' : 'N');

            $('#ETADate').val(ParseDate(x.ETA));
            $('#ETBDate').val(ParseDate(x.ETB));
            $('#ETDDate').val(ParseDate(x.ETD));
            $('#ETATime').val(ParseTime(x.ETA));
            $('#ETBTime').val(ParseTime(x.ETB));
            $('#ETDTime').val(ParseTime(x.ETD));

            $("#SchedVisualModal").modal('show');
        },
        eventRender: function (x, $el) {
            var ShipFace = (x.ShipFace == true) ? "▼" : "▲";
            $el.popover({
                html: true,
                title: x.VoyageNo + " " + x.VesselName,
                content: "ETA:" + ParseDateTime(x.ETA) + "<br>" +
                         "ETB:" + x.start._i + "<br>" +
                         "ETD:" + x.end._i + "<br>" +
                         "舷:" + ShipFace + '<br>' +
                        "バース:" + x.BerthName,
                trigger: 'hover',
                placement: "top",
                container: 'body'
            });
        },
        height: 600,
        eventAfterAllRender: function (view) {
            $('.fc-license-message').hide();
            $('.fc-view').css("background-color", "#fff");
            $('.fc-button').on('click', function () {
                $('.fc-license-message').hide();
                $('.fc-view').css("background-color", "#fff");
            });
            HideLoading();
            

            $('#visual').find('button.fc-today-button').click(function () {
                var today = new Date();
                var dd = String(today.getDate()).padStart(2, '0');
                var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
                var yyyy = today.getFullYear();

                today = yyyy + '/' + mm + '/' + dd;
                $('#SearchETA').val(today);
            });

        }
    });
}

