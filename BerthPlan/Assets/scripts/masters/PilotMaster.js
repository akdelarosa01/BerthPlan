/** 
* <summary>
*  掲示板登録JS
* </summary>
* <history>
* ---VERSION----対応日付---------対応者----------対応内容----
*    00.01      2020/03/12       K.Ga　　        コードを作った。
* </history>
*/

//*
//  Variables
var vUpdTime = '';
var vID = 0;
//*

$(function () {
    // Initialize input mask on 電話番号
    $('#textTel').inputmask("999-9999-99999", { "clearIncomplete": false, "placeholder": " " });
    

    //Initialize Display
    fDispClear();

    //Get Pilot List
    flGetPilotList();

    //Check all checkboxes
    checkAllCheckboxesInTable("#tblPilot", ".checkAllitem", ".checkItem");

    ///Add Event
    $('#btnNew').on('click', function () {
        //Check Req Fields
        if (!fCheck()) {
            return;
        }
        //Save Data
        fUpdData();
    });

    ///Update Event
    $('#btnUpdate').on('click', function () {
        //Check Req Fields
        if (!fCheck()) {
            return;
        }
        //Update Data
        fUpdData();
    });

    //Delete Event
    $('#btnDelete').on('click', function () {
        var sMsg = getMsg('Q01');
        var table = $('#tblPilot').DataTable();
        var oDel = [];

        $.each(table.context[0].aiDisplay, function (i, x) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                oDel.push({
                    ID: table.context[0].aoData[x].anCells[0].firstChild.name,
                    UpdTime: ParseDateTime(table.context[0].aoData[x]._aData.UpdTime, "ss"),
                });
            }
        });
        if (oDel.length < 1) {
            msg(getMsg('E03'), 'failed');
            return false;
        }

        bootbox.confirm({
            title: "水先人を削除する",
            size: 'small',
            message: sMsg,
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
                    //Delete Selected Row
                    fDelData(oDel);
                }
            }
        })
    });

    //Print Event
    $('#btnExcel').on('click', function () {
        var oPilotData = [];
        var tPilot = $('#tblPilot').DataTable();

        $.each(tPilot.context[0].aiDisplay, function (indx, val) {
            var data = tPilot.context[0].aoData[val];

            oPilotData.push({
                ID: data._aData.ID,
                PilotCD: data._aData.PilotCD,
                PilotName: data._aData.PilotName,
                Email: data._aData.Email,
                Tel: data._aData.Tel
            });
        });

        if (oPilotData.length > 0) {
            fPrintData(oPilotData);
        } else {
            msg(getMsg('E08'), 'failed');
        }
    });

    //Clear Event
    $('#btnClear').on('click', function () {
        //Initialize and Clear Display
        fDispClear();
        flGetPilotList();
    });

    //Table Click Event
    $('#tblPilot_body').on('click', 'tr', function (e) {
        var tPilot = $('#tblPilot').DataTable();
        var sData = tPilot.row($(this)).data();

        $('.required').each(function () {
            hideError($(this).attr('id'));
        });

        vID = sData.ID;
        vUpdTime = ParseDateTime(sData.UpdTime, 'ss');
        $('#textPilotCode').val(sData.PilotCD);
        $('#textPilotName').val(sData.PilotName);
        $('#textEmail').val(sData.Email);
        $('#textTel').val(sData.Tel);

        $('#textPilotCode').prop('readonly', true);
        $('#btnUpdate').prop('disabled', false);
        $('#btnNew').prop('disabled', true);
    });

    //Shortcut Keys
    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1: Block F1
            case 112:
                e.preventDefault();
                break;
            //F2: SAVE
            case 113:
                e.preventDefault();
                if (!$('#btnNew').is(':disabled')) {
                    $('#btnNew').click();
                }
                break;
            //F3: UPDATE
            case 114:
                e.preventDefault();
                if (!$('#btnUpdate').is(':disabled')) {
                    $('#btnUpdate').click();
                }
                break;
            //F4: CLEAR
            case 115:
                e.preventDefault();
                $('#btnClear').click();
                break;
            //F6: Block F6
            case 117:
                e.preventDefault();
                break;
            //F8: DELETE
            case 119:
                e.preventDefault();
                $('#btnDelete').click();
                break;
            //F10: PRINT
            case 121:
                e.preventDefault();
                if (!$('#btnExcel').is(':disabled')) {
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

//Get PilotData
function flGetPilotList() {
    var $table = $('#tblPilot');

    $.ajax({
        type: 'POST',
        url: 'PilotMaster.aspx/flGetPilotList',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        if ($.fn.DataTable.isDataTable('#tblPilot')) {
            $table.DataTable().clear();
        }
        $table.dataTable({
            "data": data.d,
            "cache": false,
            "destroy": true,
            "language": dataTableLanguageVariable(),
            "sDom": "rtipl",
            "lengthChange": false,
            "ordering": false,
            "processing": true,
            "responsive": true,
            "columns": [
                {
                    title: '<input type="checkbox" class="checkAllitem" />',
                    data: function (data) {
                        return '<input type="checkbox" data-time="' + ParseDateTime(data.UpdTime, 'ss') + '" data-id="' + data.ID + '" name="' + data.ID + '" class="checkItem">';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { title: "水先人コード", data: 'PilotCD', width: "10%", sortable: false, orderable: false },
                { title: "水先人名", data: 'PilotName', width: "40%", sortable: false, orderable: false },
                { title: "E-メール", data: 'Email', width: "20%", sortable: false, orderable: false },
                { title: "電話番号", data: 'Tel', width: "15%", sortable: false, orderable: false },
                {
                    title: "最後の更新",
                    data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, sortable: false, orderable: false, width: "20%"
                }
            ],
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblPilot thead th')

                if ($thead.find('select').length < 1) {
                    api.columns().indexes().flatten().each(function (i) {
                        if (i > 0) {
                            var column = api.column(i);
                            var title = $thead.eq(i).text();

                            var select = $('<select class="sort-column"><option value="">' + title + '</option></select>')
                                .appendTo($(column.header()).empty())
                                .on('change', function () {
                                    var val = $.fn.dataTable.util.escapeRegex(
                                        $(this).val()
                                    );
                                    if ($(this).val() == "" && $(this)[0].selectedIndex != 0) {
                                        column.search('^$', true, false).draw()
                                    } else {
                                        column.search(val ? '^' + val + '$' : '', true, false).draw();
                                    }
                                });

                            column.data().unique().sort().each(function (d, j) {
                                select.append('<option value="' + d + '">' + d + '</option>')
                            });
                        }

                    });

                    $('.sort-column').css("background-color", "#d9edf7");
                    $('.sort-column').css("border", "none");
                    $('.sort-column').css("width", "100%");
                }
            },
            "fnDrawCallback": function () {
                checkAllCheckboxesInTable("#tblPilot", ".checkAllitem", ".checkItem");

                $("#tblPilot_head tr").css("background-color", "#d9edf7");
            },
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
}

//Check Req Fields
function fCheck() {
    var invalid = 0;
    $('.required').each(function () {
        var id = $(this).attr('id');
        if ($(this).val() == "") {
            showError(id, getMsg('E06', $(this).attr('data-name')));
            invalid++;
        }
    });
    if (invalid != 0) { return false } // msg(getMsg('E01'), 'failed');

    if ($('#textPilotCode').val().length != 4) {
        showError($('#textPilotCode').attr('id'), getMsg('E02', '水先人コード'));
        return false;
    }

    return true;
}

//Initiliaze and Clear Display
function fDispClear() {
    vUpdTime = '';
    vID = 0;

    flGetPilotList()

    $('.required').each(function () {
        if ($(this).value !== '' || $(this).value !== null) {
            hideError($(this).attr('id'));
        }
    });
    $('.txtBox').val('');
    $('#btnUpdate').prop('disabled', true);
    $('#textPilotCode').prop('readonly', false);
    $('#btnNew').prop('disabled', false);

    $('#textPilotCode').focus();
}

//Save/Update Function
function fUpdData() {
    var oPilot = fGetObjData();
    $.ajax({
        type: 'POST',
        url: 'PilotMaster.aspx/flUpdData',
        data: JSON.stringify({ pPilotInfo: oPilot }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {
            //Refresh Data
            flGetPilotList();
            //Clear Display
            fDispClear();
        }
        msg(sResult.Msg, sResult.Status);
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });
}

//Get Current Pilot Data
function fGetObjData() {
    var obj = {
        ID: vID,
        PilotCD: $('#textPilotCode').val(),
        PilotName: $('#textPilotName').val(),
        Email: $('#textEmail').val(),
        Tel: $('#textTel').val(),
        UpdTime: vUpdTime,
    };
    return obj;
}

//Delete Function
function fDelData(sPilotData) {
    $.ajax({
        type: 'POST',
        url: 'PilotMaster.aspx/flDelPilot',
        data: JSON.stringify({ lPilot: sPilotData }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {   
            //Refresh Data
            flGetPilotList();
            //Clear Display
            fDispClear();
        }
        msg(sResult.Msg, sResult.Status);
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });
}

//Print Data Function
function fPrintData(oPilotData) {
    $.ajax({
        type: 'POST',
        url: 'PilotMaster.aspx/flPrint',
        data: JSON.stringify({ lPilot: oPilotData }),
        beforeSend: ShowLoading(),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (response, textStatus, xhr) {
        if (response.d.Status == 'success') {
            window.location.href = '.../../../Handlers/FileDownloader.ashx?FilePath=' + response.d.Data[0] + '&FileName=' + response.d.Data[1];
            msg(response.d.Msg, 'success');
        } else {
            msg(response.d.Msg, 'failed');
        }
        HideLoading();
    }).fail(function (xhr, textStatus, xhr) {
        HideLoading();
        msg(textStatus, 'failed');
    });
}