/** 
* <summary>
*  掲示板登録JS
* </summary>
* <history>
* ---VERSION----対応日付---------対応者----------対応内容----
*    00.01      2020/03/17       K.Ga　　        コードを作った。
* </history>
*/


//*
var vSchedID = 0;
var vUpdTime = '';
//*

$(function () {
    //Initialize Screen
    fDispClear('I');

    //Get Web URL
    fGetURL();

    //Voyage On Change Event
    $("#Voyage").on('change', function (e) {
        $.ajax({
            type: "POST",
            url: "VesselScheduleRegistration.aspx/fgVesseNoExist",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{VoyageNo: "' + $(this).val() + '" }',
        }).done(function (data, textStatus, xhr) {
            if (data.d == '' || data.d == null) {
                //
            } else {
                $("#MainContent_VesselCD_VesselCD").val(data.d.VesselCD);
                $("#MainContent_VesselCD_VesselName").val(data.d.VesselName);
            }
        }).fail(function (xhr, textStatus, errorThrown) {
            console.log("error");
        });
    });

    //Search OnClick Event
    $('#btnSearch').on('click', function (e) {
        var sVesselCD = $('#MainContent_VesselCD_VesselCD').val();
        var sVoyage = $('#Voyage').val();
        //Check
        if (!fCheck('S')) {
            return false;
        }
        //Search
        fSearchVoyage(sVesselCD, sVoyage);
    });

    //Register OnClick Event
    $('#btnRegister').on('click', function () {
        $('#btnRegister').focus();
        //Check 
        if (!fCheck('R')) {
            return false;
        }
        //Get Data, Register
        var obj = fGetObjectData();
        var berthCD = $('#MainContent_BerthID_BerthCD').val();
        fRegister(obj, berthCD);
    });

    //Back Event
    $('#btnBack').on('click', function (e) {
        var StartETA = '';
        var EndETA = '';
        var VesselCD = '';
        var ApplicantCD = '';
        var PilotCD = '';
        var qs = location.search.substring(1);

        strx = qs.split('&');
        if (qs != '' && strx.length != 2) {
            StartETA = strx[2].substring(strx[2].indexOf("=") + 1);
            EndETA = strx[3].substring(strx[3].indexOf("=") + 1);
            VesselCD = strx[4].substring(strx[4].indexOf("=") + 1);
            ApplicantCD = strx[5].substring(strx[5].indexOf("=") + 1);
            PilotCD = strx[6].substring(strx[6].indexOf("=") + 1);
            window.location.href = "VesselScheduleList?StartETA=" + StartETA +
                "&EndETA=" + EndETA +
                "&VesselCD=" + VesselCD +
                "&ApplicantCD=" + ApplicantCD +
                "&PilotCD=" + PilotCD + "";
        } else if (strx.length == 2) {
            window.location.href = "VesselScheduleVisual";
        } else {
            window.location.href = "SystemMenu";
        }
    });

    //Clear Display
    $('#btnClear').on('click', function (e) {
        fDispClear('I');
        $('.required').each(function () {
            if ($(this).value !== '' || $(this).value !== null) {
                hideError($(this).attr('id'));
            }
        });
    });

    //Shorcut Keys
    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1: SEARCH
            case 112:
                e.preventDefault();
                if (!$('#btnSearch').is(':disabled')) {
                    $('#btnSearch').click();
                }
                break;
            //F2: SAVE
            case 113:
                e.preventDefault();
                if (!$('#btnRegister').is(':disabled')) {
                    $('#btnRegister').click();
                }
                break;
            //F3: 
            case 114:
                e.preventDefault();
                break
            //F4: CLEAR
            case 115:
                e.preventDefault();
                $('#btnClear').click();
                break;
            //F6: Block F6
            case 117:
                e.preventDefault();
                break;
            //F8: 
            case 119:
                e.preventDefault();
                break;
            //F10: 
            case 121:
                e.preventDefault();
            //F12: CLOSE
            case 123:
                e.preventDefault();
                window.location.href = ".../../../Pages/SystemMenu"
                break;
            default:

        }
    });

});

//Get WebURL
function fGetURL() {
    var sVesselCD = '';
    var sVoyageNo = '';

    // get the query string without the ?
    var qs = location.search.substring(1);

    strx = qs.split('&');
    if (qs != '') {
        sVesselCD = strx[0].substring(strx[0].indexOf("=") + 1);
        sVoyageNo = strx[1].substring(strx[1].indexOf("=") + 1);

        $('#MainContent_VesselCD_VesselCD').val(sVesselCD);
        $('#Voyage').val(sVoyageNo);

        //Search Voyage
        fSearchVoyage(sVesselCD, sVoyageNo)
    }
}

//Search Voyage
function fSearchVoyage(sVesselCD, sVoyage) {
    $.ajax({
        type: 'POST',
        url: 'VesselScheduleRegistration.aspx/flSearchVoyage',
        data: JSON.stringify({ pVoyageNo: sVoyage, pVesselCD: sVesselCD }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sData = data.d;
        sessionOut(sData.Status);
        if (sData.Status == 'failed') {
            if (sData.Data) {
                $('#MainContent_VesselCD_VesselCD').val('');
                $('#MainContent_VesselCD_VesselName').val('');

                showError('MainContent_VesselCD_VesselCD', getMsg('E06', $('#MainContent_VesselCD_VesselCD').attr('data-name')));
            } else {
                bootbox.confirm({
                    title: "新規データ",
                    size: 'small',
                    message: sData.Msg,
                    buttons: {
                        confirm: {
                            label: 'OK',
                            className: 'btn-sm btn-success'
                        },
                        cancel: {
                            label: 'キャンセル',
                            className: 'btn-sm btn-default'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            fSearch(sVesselCD, sVoyage);
                        }
                    }
                })
            }
        } else if (sData.Status == 'error') {
            msg(sData.Msg, 'failed');
        } else {
            fSearch(sVesselCD, sVoyage);
        }

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });
}

//Get Data
function fSearch(sVesselCD, sVoyage) {
    $.ajax({
        type: 'POST',
        url: 'VesselScheduleRegistration.aspx/flSearch',
        data: JSON.stringify({ pVesselCD: sVesselCD, pVoyageNo: sVoyage }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sData = data.d;

        if (sData.Status == 'failed' || sData.Status == 'error') {
            msg(sData.Msg, sData.Status);
        } else {
            sData = '';
            sData = data.d.Data;

            $("#TitleBerth").html('着岸バース<span class="text-danger">*</span>');
            $("#TitleVessel").html('本船<span class="text-danger">*</span>');
            $("#TitlePilot").html('水先人<span class="text-danger">*</span>');
            $("#TitleApplicant").html('申請者<span class="text-danger">*</span>');

            $('#MainContent_VesselCD_VesselName').val(sData.VesselName);
            $('#GrossTon').text(numberFormat(sData.GrossTon));
            $('#LOA').text(numberFormat(sData.LOA) + 'm');

            vSchedID = sData.ScheduleID;
            if (vSchedID != 0) {
                var ETA = new Date(parseInt(sData.ETA.substr(6)));
                var ETB = new Date(parseInt(sData.ETB.substr(6)));
                var ETD = new Date(parseInt(sData.ETD.substr(6)));
                vUpdTime = ParseDateTime(sData.UpdTime, 'ss');

                $('#MainContent_BerthID_BerthID').val(sData.BerthID);
                $('#MainContent_BerthID_BerthCD').val(sData.BerthCD);
                $('#MainContent_BerthID_BerthName').val(sData.BerthName);
                $('#MainContent_ApplicantCD_ApplicantCD').val(sData.ApplicantCD);
                $('#MainContent_ApplicantCD_ApplicantName').val(sData.ApplicantName);
                $('#MainContent_PilotCD_PilotCD').val(sData.PilotCD);
                $('#MainContent_PilotCD_PilotName').val(sData.PilotName);

                $('#PilotRequired').val(sData.PilotGuide.toString());
                $('#TugRequired').val(sData.Tag.toString());
                $('#LineRequired').val(sData.LineBoat.toString());
                $('#ShipFacing').val(sData.ShipFace.toString());

                $('#ETAdate').val(ParseDate(sData.ETA));
                $('#ETBdate').val(ParseDate(sData.ETB));
                $('#ETDdate').val(ParseDate(sData.ETD));
                $('#ETAtime').val(fFormatAMPM(ETA));
                $('#ETBtime').val(fFormatAMPM(ETB));
                $('#ETDtime').val(fFormatAMPM(ETD));

                $('#divETB').data("DateTimePicker").minDate(ParseDate(sData.ETA));
                $('#divETB').data("DateTimePicker").maxDate(ParseDate(sData.ETD));
                $('#divETD').data("DateTimePicker").minDate(ParseDate(sData.ETB));
                $('#divETA').data("DateTimePicker").minDate(false);
                $('#divETA').data("DateTimePicker").maxDate(ParseDate(sData.ETB));
            }
            fDispClear('S');
        }
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });
}

//Register Schedule
function fRegister(obj, berthCD) {
    $.ajax({
        type: 'POST',
        url: 'VesselScheduleRegistration.aspx/flRegister',
        beforeSend: ShowLoading(),
        data: JSON.stringify({ pSched: obj, berthCD: berthCD }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {
            fDispClear('I');
        }
        if (sResult.Data) {
            var data = sResult.Data;
            if (data[0] == 'Error') { //Berth
                $('#MainContent_BerthID_BerthID').val('');
                $('#MainContent_BerthID_BerthCD').val('');
                $('#MainContent_BerthID_BerthName').val('');

                showError('MainContent_BerthID_BerthCD', getMsg('E06', $('#MainContent_BerthID_BerthCD').attr('data-name')));
            }
            if (data[1] == 'Error') { //Applicant
                $('#MainContent_ApplicantCD_ApplicantCD').val('');
                $('#MainContent_ApplicantCD_ApplicantName').val('');

                showError('MainContent_ApplicantCD_ApplicantCD', getMsg('E06', $('#MainContent_ApplicantCD_ApplicantCD').attr('data-name')));
            }
            if (data[2] == 'Error') { //Pilot
                $('#MainContent_PilotCD_PilotCD').val('');
                $('#MainContent_PilotCD_PilotName').val('');

                showError('MainContent_PilotCD_PilotCD', getMsg('E06', $('#MainContent_PilotCD_PilotCD').attr('data-name')));
            }
        } else {
            msg(sResult.Msg, sResult.Status);
        }
        HideLoading();
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
        HideLoading()
    });
}

//FormatDate(AM/PM)
function fFormatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;

    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

//Check Required Fields
function fCheck(sMode) {
    var invalid = 0;

    if (sMode == 'S') {
        if ($('#MainContent_VesselCD_VesselCD').val() == "") {
            showError('MainContent_VesselCD_VesselCD', getMsg('E06', $('#MainContent_VesselCD_VesselCD').attr('data-name')));
            invalid++;
        }
        if ($('#Voyage').val() == '') {
            showError('Voyage', getMsg('E06', $('#Voyage').attr('data-name')));
            invalid++;
        }
        if (invalid != 0) { return false }
        if ($('#Voyage').val().length < 4) {
            showError('Voyage', getMsg('E02', 'VoyageNo'));
            return false;
        }
    } else {
        $('.required').each(function () {
            var id = $(this).attr('id');
            if ($(this).val() == '') {
                showError(id, getMsg('E06', $('#' + id).attr('data-name')));
                invalid++;
            }
        });

        if ($('#ETAtime').val().length <= 5) {
            msg(getMsg('E04'), 'failed');
            return false
        }
        if ($('#ETBtime').val().length <= 5) {
            msg(getMsg('E04'), 'failed');
            return false
        }
        if ($('#ETDtime').val().length <= 5) {
            msg(getMsg('E04'), 'failed');
            return false
        }

        if (!fCheckYear($('#ETAdate').val())) {
            msg(getMsg('E04'), 'failed');
            return false
        }
        if (!fCheckYear($('#ETBdate').val())) {
            msg(getMsg('E04'), 'failed');
            return false
        }
        if (!fCheckYear($('#ETDdate').val())) {
            msg(getMsg('E04'), 'failed');
            return false
        }

        if (invalid != 0) {
            return false
        }

    }

    return true;
}

//Initialize Display
function fDispClear(sStatus) {
    var state = [];
    switch (sStatus) {
        case 'I':
            vSchedID = 0;
            vUpdTime = '';

            //-------------Block Previous Date-------------------
            $("#divETA").on("dp.change", function (e) {
                $('#divETB').data("DateTimePicker").minDate(e.date);
                if ($('#ETBdate').val() == '') {
                    $('#divETD').data("DateTimePicker").minDate(e.date);
                }
            });
            $("#divETB").on("dp.change", function (e) {
                $('#divETA').data("DateTimePicker").maxDate(e.date);
                $('#divETD').data("DateTimePicker").minDate(e.date);
            });
            $("#divETD").on("dp.change", function (e) {
                $('#divETB').data("DateTimePicker").maxDate(e.date);
            });

            $('#divETB').data("DateTimePicker").maxDate(false);
            $('#divETA').data("DateTimePicker").maxDate(false);
            $('#divETB').data("DateTimePicker").minDate(DateNow());
            $('#divETD').data("DateTimePicker").minDate(DateNow());
            $('#divETA').data("DateTimePicker").minDate(DateNow());
            //----------------------------------------------------

            $("#TitleBerth").html('着岸バース<span class="text-danger">*</span>');
            $("#TitleVessel").html('本船<span class="text-danger">*</span>');
            $("#TitlePilot").html('水先人<span class="text-danger">*</span>');
            $("#TitleApplicant").html('申請者<span class="text-danger">*</span>');

            $('.cls').val('');
            $('.disble-fld').prop('disabled', true);

            $('#GrossTon').text('');
            $('#LOA').text('');

            state = ['#MainContent_PilotCD_PilotCD', '#MainContent_BerthID_BerthCD', '#MainContent_ApplicantCD_ApplicantCD',
                           '#btnSearchBerth', '#btnSearchCompany', '#btnSearchPilot'];
            $.each(state, function (idx, val) {
                $(val).prop('disabled', true);
            });

            //$('#MainContent_PilotCD_PilotCD').prop('disabled', true);
            //$('#MainContent_BerthID_BerthCD').prop('disabled', true);
            //$('#MainContent_ApplicantCD_ApplicantCD').prop('disabled', true);
            //$('#btnSearchBerth').prop('disabled', true);
            //$('#btnSearchCompany').prop('disabled', true);  
            //$('#btnSearchPilot').prop('disabled', true);

            $('.required').val('');
            $('#MainContent_VesselCD_VesselName').val('');
            $('#MainContent_BerthID_BerthID').val('');
            $('#MainContent_BerthID_BerthCD').val('');
            $('#MainContent_BerthID_BerthName').val('');
            $('#MainContent_ApplicantCD_ApplicantName').val('');
            $('#MainContent_PilotCD_PilotName').val('');

            //$('#MainContent_VesselCD_VesselCD').val('');
            //$('#MainContent_VesselCD_VesselName').val('');
            //$('#MainContent_BerthID_BerthID').val('');
            //$('#MainContent_BerthID_BerthCD').val('');
            //$('#MainContent_BerthID_BerthName').val('');
            //$('#MainContent_ApplicantCD_ApplicantCD').val('');
            //$('#MainContent_ApplicantCD_ApplicantName').val('');
            //$('#MainContent_PilotCD_PilotCD').val('');
            //$('#MainContent_PilotCD_PilotName').val('');

            $('#Voyage').prop('disabled', false);
            $('#btnRegister').prop('disabled', true);
            $('#btnSearch').prop('disabled', false);
            $('#btnSearchVessel').prop('disabled', false);

            $('#MainContent_VesselCD_VesselCD').prop('disabled', false);
            $('#Voyage').focus();
            break;
        case 'S':
            $('.disble-fld').prop('disabled', false);
            $('.required').prop('disabled', false);

            state = ['#btnSearchBerth', '#btnSearchCompany', '#btnSearchPilot', '#btnRegister'];
            $.each(state, function (idx, val) {
                $(val).prop('disabled', false);
            });

            $('#btnRegister').prop('disabled', false);
            $('#btnSearch').prop('disabled', true);
            $('#Voyage').prop('disabled', true);
            $('#btnSearchVessel').prop('disabled', true);

            $('#MainContent_VesselCD_VesselCD').prop('disabled', true);
            $('#MainContent_BerthID_BerthCD').focus();
            break;
    }
}

//Get Current Data
function fGetObjectData() {
    var obj = {
        ScheduleID: vSchedID,
        VesselCD: $('#MainContent_VesselCD_VesselCD').val(),
        VoyageNo: $('#Voyage').val(),
        BerthID: $('#MainContent_BerthID_BerthID').val() == '' ? 0 : $('#MainContent_BerthID_BerthID').val(),
        ApplicantCD: $('#MainContent_ApplicantCD_ApplicantCD').val(),
        PilotCD: $('#MainContent_PilotCD_PilotCD').val(),
        PilotGuide: $('#PilotRequired option:selected').val() == '' ? false : $('#PilotRequired option:selected').val(),
        Tag: $('#TugRequired option:selected').val() == '' ? false : $('#TugRequired option:selected').val(),
        LineBoat: $('#LineRequired option:selected').val() == '' ? false : $('#LineRequired option:selected').val(),
        ETA: DateMaskFormat($('#ETAdate').val() + ' ' + $('#ETAtime').val()),
        ETB: DateMaskFormat($('#ETBdate').val() + ' ' + $('#ETBtime').val()),
        ETD: DateMaskFormat($('#ETDdate').val() + ' ' + $('#ETDtime').val()),
        ShipFace: $('#ShipFacing  option:selected').val(),
        UpdTime: vUpdTime
    };
    return obj
}

//Check Year
function fCheckYear(date_value) {
    var d = date_value.replace(/\//g, "");
    var iYear = d.substring(0, 4);
    if (parseInt(iYear) < 1900) {
        return false;
    }
    return true;
}


