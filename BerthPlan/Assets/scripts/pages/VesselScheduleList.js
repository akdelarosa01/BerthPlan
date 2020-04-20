$(function () {

    Initialize();

    $("#divStartETA").on("dp.change", function (e) {
        $('#divEndETA').data("DateTimePicker").minDate(e.date);
    });

    $("#divEndETA").on("dp.change", function (e) {
        $('#divStartETA').data("DateTimePicker").maxDate(e.date);
    });

    $('#btnSearch').on('click', function (e) {
        if (FormValidate() == true) {
            if (($("#StartETA").val().length == 1 || $("#EndETA").val().length == 1) || ($("#StartETA").val() > $("#EndETA").val() && $("#EndETA").val().length != 0)) {
                msg("日付形式が間違っています", 'failed');
                $("#StartETA").focus();
            } else {
                GetScheduleList();
            }
        }
    });

    $('#btnDelete').on('click', function (e) {
        e.preventDefault();
        var table = $('#tblSchedule').DataTable();
        var tSchedule = [];
        $.each(table.context[0].aiDisplay, function (i, x) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                tSchedule.push({ 
                    ScheduleID: table.context[0].aoData[x].anCells[0].firstChild.name,
                    UpdTime: ParseDateTime(table.context[0].aoData[x]._aData.UpdTime, "ss"),
                });
            }
        });
        if (tSchedule.length != 0) {
            bootbox.confirm({
                title: "スケジュールを削除する",
                size: 'small',
                message: getMsg('Q01'),
                buttons: {
                    confirm: {
                        label: 'OK',
                        className: 'btn-sm btn-danger'
                    },
                    cancel: {
                        label: 'キャンセル',
                        className: 'btn-sm btn-default'
                    }
                },
                callback: function (result) {
                    if (result) {

                        ShowLoading();
                        $.ajax({
                            url: "VesselScheduleList.aspx/DeleteSchedule",
                            type: 'POST',
                            data: JSON.stringify({ tSchedule: tSchedule }),
                            contentType: 'application/json; charset=utf-8',
                            datatype: "json",
                        }).done(function (data, textStatus, xhr) {
                            sessionOut(data.d.Status);
                            GetScheduleList();
                            HideLoading();
                            msg(getMsg('I01'), 'success');
                        }).fail(function (xhr, textStatus, errorThrown) {
                            HideLoading();
                            msg(errorThrown,textStatus);
                        });
                    }
                }
            });
        }
        else { msg(getMsg('E03'), 'failed'); }
    });

    $('#btnUpdate').on('click', function (e) {
        e.preventDefault();
        ShowLoading();
        var table = $('#tblSchedule').DataTable();
        var tSchedule = [];
        var eLine = "";
        var iCnt = 1;
        $.each(table.context[0].aiDisplay, function (i, v) {
            var data = table.context[0].aoData[v];
            var ETADate = data.anCells[8].firstChild.value
            var ETBDate = data.anCells[9].firstChild.value
            var ETDDate = data.anCells[10].firstChild.value
            var ETATime = data.anCells[8].lastChild.value
            var ETBTime = data.anCells[9].lastChild.value
            var ETDTime = data.anCells[10].lastChild.value
            if (ValidateDate(DateMaskFormat(ETADate) + " " + ETATime, DateMaskFormat(ETBDate) + " " + ETBTime, DateMaskFormat(ETDDate) + " " + ETDTime) == false || EmptyString(ETADate, ETBDate, ETDDate, ETATime, ETBTime, ETDTime) == false) {
                eLine = eLine + "," + iCnt
            }else{
                var ETA = DateMaskFormat(ETADate) + ' ' + ETATime;
                var ETB = DateMaskFormat(ETBDate) + ' ' + ETBTime;
                var ETD = DateMaskFormat(ETDDate) + ' ' + ETDTime;
                tSchedule.push({
                    ScheduleID: data._aData.ScheduleID,
                    VesselCD: data._aData.VesselCD,
                    VoyageNo: data._aData.VoyageNo,
                    BerthID: data._aData.BerthID,
                    ApplicantCD: data._aData.ApplicantCD,
                    PilotCD: data._aData.PilotCD,
                    UpdTime: ParseDateTime(data._aData.UpdTime, "ss"),
                    ETA: ETA,
                    ETB: ETB,
                    ETD: ETD,
                    ShipFace: data.anCells[11].lastElementChild.value,
                    PilotGuide: data.anCells[12].lastElementChild.value,
                    LineBoat: data.anCells[13].lastElementChild.value,
                    Tag: data.anCells[14].lastElementChild.value,
                });
            }
            iCnt++;
        });
        if (eLine == "" && table.context[0].aiDisplay.length != 0) {
            $.ajax({
                url: "VesselScheduleList.aspx/UpdateSchedule",
                type: 'POST',
                data: JSON.stringify({ tSchedule: tSchedule }),
                contentType: 'application/json; charset=utf-8',
                datatype: "json",
            }).done(function (data, textStatus, xhr) {
                HideLoading();
                sessionOut(data.d.Status);
                if (data.d.Status == 'success') {
                    GetScheduleList();
                    msg(data.d.Msg, data.d.Status);
                } else { msg(data.d.Msg, data.d.Status); }
            }).fail(function (xhr, textStatus, errorThrown) {
                HideLoading();
                console.log(errorThrown);
            });
        } else {
            if (table.context[0].aiDisplay.length == 0) {
                msg(getMsg("E08"), "failed");
            } else {
                msg(getMsg('E04', " 行で " + eLine.substring(1, eLine.length)), 'failed');
            }
            HideLoading();
        }
    });

    $('#btnExcel').on('click', function (e) {
        e.preventDefault();
        ShowLoading();
        var table = $('#tblSchedule').DataTable();
        var tSchedule = [];
        var eLine = "";
        $.each(table.context[0].aiDisplay, function (i, v) {
            var data = table.context[0].aoData[v];
            var ETADate = data.anCells[8].firstChild.value
            var ETBDate = data.anCells[9].firstChild.value
            var ETDDate = data.anCells[10].firstChild.value
            var ETATime = data.anCells[8].lastChild.value
            var ETBTime = data.anCells[9].lastChild.value
            var ETDTime = data.anCells[10].lastChild.value
            if (ValidateDate(DateMaskFormat(ETADate) + " " + ETATime, DateMaskFormat(ETBDate) + " " + ETBTime, DateMaskFormat(ETDDate) + " " + ETDTime) == false || EmptyString(ETADate, ETBDate, ETDDate, ETATime, ETBTime, ETDTime) == false) {
                eLine = eLine + "," + (v + 1)
            }else{
                var ETA = DateMaskFormat(ETADate) + ' ' + ETATime;
                var ETB = DateMaskFormat(ETBDate) + ' ' + ETBTime;
                var ETD = DateMaskFormat(ETDDate) + ' ' + ETDTime;
                tSchedule.push({
                    ScheduleID: data._aData.ScheduleID,
                    VesselCD: data._aData.VesselCD,
                    VesselName: data._aData.VesselName,
                    VoyageNo: data._aData.VoyageNo,
                    LOA: data._aData.LOA,
                    IO: data._aData.IO,
                    GrossTon: data._aData.GrossTon,
                    BerthID: data._aData.BerthID,
                    BerthName: data._aData.BerthName,
                    ApplicantCD: data._aData.ApplicantCD,
                    ApplicantName: data._aData.ApplicantName,
                    PilotCD: data._aData.PilotCD,
                    ETA: ETA,
                    ETB: ETB,
                    ETD: ETD,
                    ShipFace: data.anCells[11].lastElementChild.value,
                    PilotGuide: data.anCells[12].lastElementChild.value,
                    LineBoat: data.anCells[13].lastElementChild.value,
                    Tag: data.anCells[14].lastElementChild.value,
                });
            }
        });
        if (eLine == "" && table.context[0].aiDisplay.length != 0) {
            $.ajax({
                url: "VesselScheduleList.aspx/PrintSchedule",
                type: 'POST',
                data: JSON.stringify({ tSchedule: tSchedule }),
                contentType: 'application/json; charset=utf-8',
                datatype: "json",
            }).done(function (data, textStatus, xhr) {
                HideLoading();
                sessionOut(data.d.Status);
                if (data.d.Status == 'success') {
                    window.location.href = '.../../../Handlers/FileDownloader.ashx?FilePath=' + data.d.Data[0] + '&FileName=' + data.d.Data[1];
                    msg(data.d.Msg, data.d.Status);
                } else { msg(data.d.Msg, data.d.Status); }
            }).fail(function (xhr, textStatus, errorThrown) {
                HideLoading();
                console.log(errorThrown);
            });
        } else {
            if (table.context[0].aiDisplay.length == 0) {
                msg(getMsg("E08"), "failed");
            } else {
                msg(getMsg('E04', " 行で " + eLine.substring(1, eLine.length)), 'failed');
            }
            HideLoading();
        }
    });

    $('#tblSchedule_body').on('click', '.schedule-registration', function (e) {
        var tGridView = $('#tblSchedule').DataTable();
        var data = tGridView.row($(this).parent()[0].parentNode).data();
        var StartETA = $("#StartETA").val();
        var EndETA = $("#EndETA").val();
        var VesselCD = $("#MainContent_VesselCD_VesselCD").val();
        var sApplicantCD = $("#MainContent_ApplicantCD_ApplicantCD").val();
        var sPilotCD = $("#MainContent_PilotCD_PilotCD").val();
        window.location.href = "VesselScheduleRegistration?VesselCD=" + data.VesselCD + "&VoyageNo=" + data.VoyageNo +
                                                         "&StartETA=" + StartETA + "&EndETA=" + EndETA + "&sVesselCD=" + VesselCD +
                                                         "&sApplicantCD=" + sApplicantCD + "&sPilotCD=" + sPilotCD;
        
    });

    $('#tblSchedule_body').on('keydown', '.enter', function (e) {
        var self = $(this)
			, form = self.parents('form:eq(0)')
			, focusable
			, next
        ;
                
        if (e.keyCode == 13) {
            e.preventDefault();

            focusable = form.find('.enter').filter(':visible');
            next = focusable.eq(focusable.index(this) + 1);

            if (next.length) {
                next.focus();
            } else {
                if (e.target.type === 'button') {
                    form.submit();
                }
            }
            return false;
        }
    });

    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1: Search
            case 112:
                e.preventDefault();
                if (!$('#btnSearch').is(':disabled')) {
                    $('#btnSearch').focus();
                    $('#btnSearch').click();
                }
                break;
            //F2:
            case 113:
                e.preventDefault();
                break;
            //F3: Update
            case 114:
                e.preventDefault();
                if (!$('#btnUpdate').is(':disabled')) {
                    $('#btnUpdate').focus();
                    $('#btnUpdate').click();
                }
                break
            //F4:
            case 115:
                e.preventDefault();
                break;
            //F6: 
            case 117:
                e.preventDefault();
                break;
            //F8: DELETE
            case 119:
                e.preventDefault();
                if (!$('#btnDelete').is(':disabled')) {
                    $('#btnDelete').focus();
                    $('#btnDelete').click();
                }
                break;
            //F10: Excel
            case 121:
                e.preventDefault();
                if (!$('#btnExcel').is(':disabled')) {
                    $('#btnExcel').focus();
                    $('#btnExcel').click();
                }
                break;
            //F12: CLOSE
            case 123:
                e.preventDefault();
                window.location.href = ".../../../Pages/SystemMenu"
                break;
            default:

        }
    });

});

function Initialize() {
    $("#StartETA").val(DateNow());
    $('#divEndETA').data("DateTimePicker").minDate(DateNow());
    GetURL();

    $('#tblSchedule').dataTable({
        "columnDefs": [
            { "width": "5%", "targets": [0] },
            { "className": "text-center custom-middle-align td-checkbox", "targets": [0] },
        ],
        "ordering": false,
        "destroy": true,
        "lengthChange": false,
        "searching": true,
        "sDom": "rtipl",
        "language": dataTableLanguageVariable(),
        "processing": true,
        "columns": [
            {
                title: '<input type="checkbox" class="checkAllitem" />', width: "3%"
            },
            {
                title: "編集", width: "3%"
            },
            { title: "船名", width: "12%", "sortable": false, "orderable": false },
            { title: "VoyageNo", width: "5%", "sortable": false, "orderable": false },
            {
                title: "LOA",
                width: "5%"
            },
            {
                title: "外/内",
                width: "5%", "sortable": false, "orderable": false
            },
            {
                title: "子定船席",
                width: "6%"
            },
            { title: "着岸バース名", width: "12%", sortable: false, orderable: false },
            {
                title: "ETA",
                width: "11%", sortable: false, orderable: false
            },
            {
                title: "ETB",
                width: "11%", sortable: false, orderable: false
            },
            {
                title: "ETD",
                width: "11%", sortable: false, orderable: false
            },
            {
                title: "右舷/左舷",
                sortable: false, orderable: false, width: "5%"
            },
            {
                title: "水先案内人 要/不要",
                sortable: false, orderable: false, width: "7%"
            },
            {
                title: "ラインボート 要/不要",
                sortable: false, orderable: false, width: "5%"
            },
            {
                title: "タグ要/不要",
                sortable: false, orderable: false, width: "5%"
            },
            {
                title: "最後の更新",
                width: "6%", "sortable": false, "orderable": false
            },
        ],
        "initComplete": function () {
            var api = this.api();
            var $thead = $('#tblSchedule thead th');

            if ($thead.find('select').length < 1) {
                api.columns().indexes().flatten().each(function (i) {
                    if (i > 0) {
                        if (i != 1 && i != 3 && i != 4 && i != 6 && i != 8 && i != 9 && i != 10) {
                            var column = api.column(i);

                            var title = $thead.eq(i).text();
                            var select = $('<select class="sort-column"><option value="">' + title + '</option></select>')
                                .appendTo($(column.header()).empty())
                                .on('change', function () {
                                    var val = $.fn.dataTable.util.escapeRegex(
                                        $(this).val()
                                    );

                                    column
                                        .search(val ? '^' + val + '$' : '', true, false)
                                        .draw();
                                });
                            if (i == 12 || i == 13 || i == 14) {
                                select.append('<option value="true">要</option>' + '<option value="false">不要</option>');
                            } else if (i == 11) {
                                select.append('<option value="true">右</option>' + '<option value="false">左</option>');
                            } else {
                                column.data().unique().sort().each(function (d, j) {
                                    select.append('<option value="' + d + '">' + d + '</option>');
                                });
                            }
                        }
                    }
                });
                $('.sort-column').css({
                    "background-color": "#d9edf7",
                    "border": "none",
                    "width": "100%"
                });
            }

        },
        "drawCallback": function () {
            $('.datepicker').datetimepicker({
                format: 'YYYY/MM/DD',
                locale: 'ja'
            });
            $('.datepicker').on('click', 'input', function () {
                this.setSelectionRange(0, this.value.length)
            });
            $('.timepicker').timepicker({
                showInputs: false,
                minuteStep: 1,
                template: false
            });
            $('.timepicker').on('click', 'input', function () {
                this.setSelectionRange(0, this.value.length)
            });
            $(".date-mask").inputmask("y/m/d", {
                autoUnmask: true
            });
            $("#tblSchedule_head tr").css("background-color", "#d9edf7")
            checkAllCheckboxesInTable("#tblSchedule", ".checkAllitem", ".checkItem");
        },
        "dom": "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
    });

    $('#btnUpdate').prop('disabled', true);
    $('#btnDelete').prop('disabled', true);
    $('#btnExcel').prop('disabled', true);
}

function GetURL() {
        var StartETA = '';
        var EndETA = '';
        var VesselCD = '';
        var ApplicantCD = '';
        var PilotCD = '';
        var qs = location.search.substring(1);
        strx = qs.split('&');

        if (qs != '') {
            StartETA = strx[0].substring(strx[0].indexOf("=") + 1);
            EndETA = strx[1].substring(strx[1].indexOf("=") + 1);
            VesselCD = strx[2].substring(strx[2].indexOf("=") + 1);
            ApplicantCD = strx[3].substring(strx[3].indexOf("=") + 1);
            PilotCD = strx[4].substring(strx[4].indexOf("=") + 1);

            $('#StartETA').val(StartETA);
            $('#EndETA').val(EndETA);
            $('#MainContent_VesselCD_VesselCD').val(VesselCD);
            $('#MainContent_ApplicantCD_ApplicantCD').val(ApplicantCD);
            $('#MainContent_PilotCD_PilotCD').val(PilotCD);

            GetScheduleList();
        }
    }

function GetScheduleList() {
    ShowLoading();
    $.ajax({
        type: "POST",
        url: "VesselScheduleList.aspx/GetScheduleList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{StartETA: "' + DateMaskFormat($("#StartETA").val()) +
             '",EndETA: "' + DateMaskFormat(($("#EndETA").val()== ""? '9999/12/01':$("#EndETA").val() )) + " 23:59" +
             '",VesselCD: "' + $("#MainContent_VesselCD_VesselCD").val() + 
             '",ApplicantCD: "' + $("#MainContent_ApplicantCD_ApplicantCD").val() + 
             '",PilotCD: "' + $("#MainContent_PilotCD_PilotCD").val() + '" }',
    }).done(function (data, textStatus, xhr) {
        sessionOut(data.d.Status);
        if (data.d.Status != "failed") {
            if (data.d.Data.length != 0) {
                $('#btnUpdate').prop('disabled', false);
                $('#btnDelete').prop('disabled', false);
                $('#btnExcel').prop('disabled', false);
            } else {
                $('#btnUpdate').prop('disabled', true);
                $('#btnDelete').prop('disabled', true);
                $('#btnExcel').prop('disabled', true);
                msg(getMsg("E08"), "failed");
            }
            $('#tblSchedule').DataTable().clear();
            $('#tblSchedule').dataTable({
                "data": data.d.Data,
                "cache": false,
                "order": [[15, 'desc']],
                "scrollX": true,
                "destroy": true,
                "lengthChange": false,
                "sDom": "rtipl",
                "language": dataTableLanguageVariable(),
                "processing": true,
                "responsive": true,
                "columns": [
                    {
                        title: '<input type="checkbox" class="checkAllitem" />',
                        data: function (data) {
                            return '<input type="checkbox" name="' + data.ScheduleID + '" class="checkItem " style="margin-left:5px"/>';
                        }, sortable: false, orderable: false, width: "3%"
                    },
                    {
                        title: "編集",
                        data: function (data) {
                            return '<span class="btn btn-info btn-sm schedule-registration"><i class="fa fa-angle-right"></i></span>';
                        }, sortable: false, orderable: false, width: "3%"
                    },
                    {
                        title: "船名", data: "VesselName", "sortable": false, "orderable": false, width: "10%"
                    },
                    {
                        title: "VoyageNo", data: "VoyageNo", "sortable": false, "orderable": false, width: "5%"
                    },
                    {
                        title: "LOA",
                        data: function (data) {
                            return numberFormat(data.LOA, "###,###,###,###,##0.0000");
                        }, width: "5%"
                    },
                    {
                        title: "外/内",
                        data: function (data) {
                            return (data.IO == true) ? "外" : "内";
                        }, "sortable": false, "orderable": false, width: "5%"
                    },
                    {
                        title: "子定船席",
                        data: function (data) {
                            return numberFormat(data.GrossTon, "###,###,###,###,##0.0000");
                        }, width: "5%"
                    },
                    {
                        title: "着岸バース名", data: "BerthName", sortable: false, orderable: false, width: "5%"
                    },
                    {
                        title: "ETA",
                        data: function (data) {
                            var ETAInput = ""
                            if (data.ETA != null) {
                                var ETA = new Date(parseInt(data.ETA.substr(6)));
                                ETAInput = '<input type="text" class="form-control enter date-mask input-sm" id="ETADate" name="ETADate" ' +
                                                    'autocomplete="off" value="' + ParseDate(data.ETA) + '" style="width:90%"/>';
                                ETAInput += '<input type="text" class="form-control input-sm enter timepicker"id="ETATime" ' +
                                                'name="ETATime" autocomplete="off" value="' + ETA.getHours() + ':' + ETA.getMinutes() + '" style="width:90%"/>';
                            } return ETAInput;
                            if (data.ETA != null) {
                                var ETA = new Date(parseInt(data.ETA.substr(6)));
                                return ETA.toLocaleDateString() + ' ' + ETA.getHours() + ':' + ETA.getMinutes();
                            } else { return ""; }
                        }, sortable: false, orderable: false, width: "8%"
                    },
                    {
                        title: "ETB",
                        data: function (data) {
                            var ETBInput = ""
                            if (data.ETB != null) {
                                var ETB = new Date(parseInt(data.ETB.substr(6)));
                                ETBInput = '<input type="text" class="form-control enter input-sm date-mask" id="ETADate" name="ETBDate" autocomplete="off" ' +
                                                    'value="' + ParseDate(data.ETB) + '" style="width:90%"/>';
                                ETBInput += '<input type="text" class="form-control input-sm enter timepicker" id="ETBTime" ' +
                                                'name="ETATime" autocomplete="off" value="' + ETB.getHours() + ':' + ETB.getMinutes() + '" style="width:90%"/>';
                            } return ETBInput;
                        }, sortable: false, orderable: false, width: "8%"
                    },
                    {
                        title: "ETD",
                        data: function (data) {
                            var ETDInput = ""
                            if (data.ETD != null) {
                                var ETD = new Date(parseInt(data.ETD.substr(6)));
                                ETDInput = '<input type="text" class="form-control input-sm enter date-mask" id="ETDDate" name="ETDDate" ' +
                                                    'autocomplete="off" value="' + ParseDate(data.ETD) + '" style="width:90%"/>';
                                ETDInput += '<input type="text" class="form-control input-sm enter timepicker" id="ETDTime" ' +
                                                'name="ETDTime" autocomplete="off" value="' + ETD.getHours() + ':' + ETD.getMinutes() + '" style="width:90%"/>';
                            } return ETDInput;
                        }, sortable: false, orderable: false, width: "8%"
                    },
                    {
                        title: "右舷/左舷",
                        data: function (data) {
                            var ShipFaceInput = "";
                            var x = (data.ShipFace == true) ? "右" : "左";
                            var migi = (data.ShipFace == true) ? "selected" : "";
                            var hidari = (data.ShipFace == false) ? "selected" : "";
                            ShipFaceInput = '<div id="divShipFaceRow' + data.ScheduleID + '" style="display:none">' + x + '</div><select class="form-control input-sm enter select-ShipFace" id="ShipFace" name="ShipFace" data-ScheduleID="' + data.ScheduleID + '" style="width:100%">' +
                                                '<option value=false></option>' +
                                                '<option value=true ' + migi + '>右</option>' +
                                                '<option value=false ' + hidari + ' name=2>左</option>' +
                                            '</select>';
                            return ShipFaceInput;
                        }, sortable: false, orderable: false, width: "7%"
                    },
                    {
                        title: "水先案内人 要/不要",
                        data: function (data) {
                            var PilotGuideInput = "";
                            var x = (data.PilotGuide == true) ? "要" : "不要";
                            var necessary = (data.PilotGuide == true) ? "selected" : "";
                            var Unnecessary = (data.PilotGuide == false) ? "selected" : "";
                            PilotGuideInput = '<div id="divPilotGuideRow" style="display:none">' + x + '</div><select class="form-control input-sm enter select-PilotGuide" id="PilotGuide" name="PilotGuide" data-ScheduleID="' + data.ScheduleID + '" style="width:100%">' +
                                                '<option value=false></option>' +
                                                '<option value=true ' + necessary + '>要</option>' +
                                                '<option value=false ' + Unnecessary + '>不要</option>' +
                                            '</select>';
                            return PilotGuideInput;
                        }, sortable: false, orderable: false, width: "7%"
                    },
                    {
                        title: "ラインボート 要/不要",
                        data: function (data) {
                            var LineBoatInput = "";
                            var x = (data.LineBoat == true) ? "要" : "不要";
                            var Y = (data.LineBoat == true) ? "selected" : "";
                            var N = (data.LineBoat == false) ? "selected" : "";
                            LineBoatInput = '<div id="divLineBoatRow" style="display:none">' + x + '</div><select class="form-control input-sm enter select-LineBoat" id="LineBoat" name="LineBoat" data-ScheduleID="' + data.ScheduleID + '" style="width:100%">' +
                                                '<option value=false></option>' +
                                                '<option value=true ' + Y + '>要</option>' +
                                                '<option value=false ' + N + '>不要</option>' +
                                            '</select>';
                            return LineBoatInput;
                        }, sortable: false, orderable: false, width: "7%"
                    },
                    {
                        title: "タグ要/不要",
                        data: function (data) {
                            var TagInput = "";
                            var x = (data.Tag == true) ? "要" : "不要";
                            var Y = (data.Tag == true) ? "selected" : "";
                            var N = (data.Tag == false) ? "selected" : "";
                            TagInput = '<div id="divTagRow" style="display:none">' + x + '</div><select class="form-control input-sm enter select-Tag" id="Tag" name="Tag" data-ScheduleID="' + data.ScheduleID + '" style="width:100%">' +
                                                '<option value=false></option>' +
                                                '<option value=true ' + Y + '>要</option>' +
                                                '<option value=false ' + N + '>不要</option>' +
                                            '</select>';
                            return TagInput;
                        }, sortable: false, orderable: false, width: "7%"
                    },
                    {
                        title: "最後の更新",
                        data: function (data) {
                            return ParseDate(data.UpdTime);
                        }, width: "5%", sortable: false, orderable: false
                    },
                ],
                "initComplete": function () {
                    var api = this.api();
                    var $thead = $('#tblSchedule thead th');
                    if ($thead.find('select').length < 1) {
                        api.columns().indexes().flatten().each(function (i) {
                            if (i > 0) {
                                if (i != 1 && i != 3 && i != 4 && i != 6 && i != 8 && i != 9 && i != 10 && i != 15) {
                                    var column = api.column(i);
                                    var title = $thead.eq(i).text();
                                    var select = $('<select class="sort-column" name="' + i + '"><option value="" >' + title + '</option></select>')
                                        .appendTo($(column.header()).empty())
                                        .on('change', function () {
                                            var val = $.fn.dataTable.util.escapeRegex($(this).val());
                                            if ($(this).context.name == 11 || i == 12 || i == 13 || i == 14) {
                                                column.search('^' + val, true, false).draw();
                                            } else {
                                                if ($(this).val() == "" && $(this)[0].selectedIndex != 0) {
                                                    column.search('^$', true, false).draw()
                                                } else {
                                                    column.search(val ? '^' + val + '$' : '', true, false).draw();
                                                }
                                            }
                                        });

                                    if (i == 12 || i == 13 || i == 14) {
                                        select.append('<option value="要">要</option>' + '<option value="不要">不要</option>');
                                    } else if (i == 11) {
                                        select.append('<option value="右">右</option>' + '<option value="左">左</option>');
                                    } else {
                                        column.data().unique().sort().each(function (d, j) {
                                            select.append('<option value="' + d + '">' + d + '</option>');
                                        });
                                    }
                                }
                            }
                        });
                        $('.sort-column').css({
                            "background-color": "#d9edf7",
                            "border": "none",
                            "width": "100%"
                        });
                    }
                
                },
                "drawCallback": function () {
                    $('.datepicker').datetimepicker({
                        format: 'YYYY/MM/DD',
                        locale: 'ja'
                    });

                    $('.datepicker').on('click', 'input', function () {
                        this.setSelectionRange(0, this.value.length)
                    });

                    $('.timepicker').timepicker({
                        showInputs: false,
                        minuteStep: 1,
                        template: false
                    });

                    $('.timepicker').on('click', 'input', function () {
                        this.setSelectionRange(0, this.value.length)
                    });

                    $(".date-mask").inputmask("y/m/d", {
                        autoUnmask: true
                    });

                    checkAllCheckboxesInTable("#tblSchedule", ".checkAllitem", ".checkItem");

                    $("#tblSchedule_head tr").css("background-color", "#d9edf7")
                },
            });
        } else { msg(data.d.Msg, data.d.Status); }

        HideLoading();
        
    }).fail(function (xhr, textStatus, errorThrown) {
        HideLoading();
        msg(errorThrown,textStatus);
    });
}

function FormValidate() {
    var invalid = 0;
    if ($("#StartETA").val() == "") {
        showError($("#StartETA").attr('id'), getMsg('E06', $("#StartETA").attr('data-name')));
        invalid++;
    }

    if (invalid != 0) { return false }

    return true;
}

function ValidateDate(ETA, ETB, ETD) {

    ETA = new Date(ETA);
    ETB = new Date(ETB);
    ETD = new Date(ETD);
    if (ETA > ETB) {
        return false;
    }
    if (ETB >= ETD) {
        return false;
    }
    if (ETD <= ETA || ETD <= ETB) {
        return false;
    }
    return true;
}

function EmptyString(ETAD, ETBD, ETDD, ETAT, ETBT, ETDT) {
    if (ETAD == '' || ETAD == null || ETAD.length == 1) {
        return false;
    }
    if (ETBD == '' || ETBD == null || ETBD.length == 1) {
        return false;
    }
    if (ETDD == '' || ETDD == null || ETDD.length == 1) {
        return false;
    }
    if (ETAT == '' || ETAT == null || ETAT.length == 1) {
        return false;
    }
    if (ETBT == '' || ETBT == null || ETBT.length == 1) {
        return false;
    }
    if (ETDT == '' || ETDT == null || ETDT.length == 1) {
        return false;
    }
    return true;
}

function DateFormat(date) {
    var d = new Date(date);
    var today_date = d.getFullYear() + "/" + ("0" + (d.getMonth() + 1)).slice(-2) + "/" + ("0" + d.getDate()).slice(-2);
    return today_date;
}

