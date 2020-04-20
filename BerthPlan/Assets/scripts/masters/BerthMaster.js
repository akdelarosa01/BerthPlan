/** 
* <summary>
* バースマスター JS
* </summary>
* <history>
* ---VERSION----対応日付---------対応者----------対応内容----
*    00.01      2020/03/12       K.Ga　　        コードを作った。
* </history>
*/

//*
//  Variables
var vChanged = true;

var vID = 0;
var vOldBCD = '';       //OldBerthCD
var vOldWCD = '';       //OldWharfCD
var vUpdTime = '';
//*

$(function () {
    //Clear Display
    fDispClear();

    //Get Berth List
    fGetData();

    //Check all checkboxes
    checkAllCheckboxesInTable("#tblBerth", ".checkAllitem", ".checkItem");

    //Add click Event
    $('#btnNew').on('click', function () {
        //Check Req Fields
        if (!fCheck()) {
            return false;
        }
        //Save New Berth
        vChanged = true;
        fUpdData();
    });

    //Update click event
    $('#btnUpdate').on('click', function () {
        var sWharfCD = '';
        var sBerthCD = '';

        //Check Req Fields
        if (!fCheck()) {
            return false;
        }
        //Check if BerthCD or WharfCD has changed
        sWharfCD = $('#MainContent_WharfCD_WharfCD').val().trim().toUpperCase();
        sBerthCD = $('#textBerthCD').val().trim().toUpperCase();
        vChanged = false;
        if (sWharfCD != vOldWCD || sBerthCD != vOldBCD) {
            vChanged = true;
        }

        //Save New Berth
        fUpdData();
    });

    //Delete click event
    $('#btnDelete').on('click', function () {
        var sMsg = getMsg('Q01');
        var table = $('#tblBerth').DataTable();
        var oDel = [];

        $.each(table.context[0].aiDisplay, function (i, x) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                oDel.push({
                    BerthID: table.context[0].aoData[x].anCells[0].firstChild.name,
                    UpdTime: ParseDateTime(table.context[0].aoData[x]._aData.UpdTime, "ss"),
                });
            }
        });
        if (oDel.length < 1) {
            msg(getMsg('E03'), 'failed');
            return false;
        }

        bootbox.confirm({
            title: "バースを削除する",
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
                    //Delete
                    fDelData(oDel);
                }
            }
        })

    });

    //Print click event
    $('#btnExcel').on('click', function () {
        var oData = [];
        var tBerth = $('#tblBerth').DataTable();

        $.each(tBerth.context[0].aiDisplay, function (index, val) {
            var data = tBerth.context[0].aoData[val];

            oData.push({
                BerthID: data._aData.BerthID,
                WharfCD: data._aData.WharfCD,
                BerthCD: data._aData.BerthCD,
                BerthName: data._aData.BerthName,
            });
        });

        if (oData.length > 0) {
            fPrintData(oData);
        } else {
            msg(getMsg('E08'), 'failed');
        }
    });

    //Clear display
    $('#btnClear').on('click', function () {
        //Initialize and Clear Display
        fDispClear();
        fGetData();
    });

    //Table body click event
    $('#tblBerth_body').on('click', 'tr', function (e) {
        e.preventDefault;
        var tBerth = $('#tblBerth').DataTable();
        var sData = tBerth.row($(this)).data();

        $('.required').each(function () {
            hideError($(this).attr('id'));
        });

        vID = sData.BerthID;
        vOldBCD = sData.BerthCD.trim().toUpperCase();       //OldBerthCD
        vOldWCD = sData.WharfCD.trim().toUpperCase();       //OldWharfCD
        vUpdTime = ParseDateTime(sData.UpdTime, 'ss');

        $('#MainContent_WharfCD_WharfCD').val(sData.WharfCD);
        $('#MainContent_WharfCD_WharfName').val(sData.WharfName);
        $('#textBerthCD').val(sData.BerthCD);
        $('#textBerthName').val(sData.BerthName);

        $('#btnUpdate').prop('disabled', false);
        $('#btnNew').prop('disabled', true);
    });

    //Shorcut Keys
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

//Get BerthData
function fGetData() {
    var $table = $('#tblBerth');

    $.ajax({
        type: 'POST',
        url: 'BerthMaster.aspx/flGetBerthList',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        if ($.fn.DataTable.isDataTable('#tblBerth')) {
            $table.DataTable().clear();
        }
        $table.dataTable({
            "data": data.d,
            "cache": false,
            "destroy": true,
            "language": dataTableLanguageVariable(),
            "sDom": "rtipl",
            "lengthChange": false,
            "processing": true,
            "responsive": true,
            "ordering": false,
            "columns": [
                {
                    title: '<input type="checkbox" class="checkAllitem" />',
                    data: function (data) {
                        return '<input type="checkbox" data-time="' + ParseDateTime(data.UpdTime, 'ss') + '" data-id="' + data.BerthID + '" name="' + data.BerthID + '" class="checkItem">';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { title: "ワーフコード", data: 'WharfCD', width: "20%", sortable: false, orderable: false },
                { title: "ワーフ名", data: 'WharfName', width: "20%", sortable: false, orderable: false },
                { title: "バースコード", data: 'BerthCD', width: "20%", sortable: false, orderable: false },
                { title: "バース名", data: 'BerthName', width: "20%", sortable: false, orderable: false },
                {
                    title: "最後の更新", data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "15%"
                }
            ],
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblBerth thead th');

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
                checkAllCheckboxesInTable("#tblBerth", ".checkAllitem", ".checkItem");

                $("#tblBerth_head tr").css("background-color", "#d9edf7");
            },
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
}

//Delete Function
function fDelData(sDelData) {
    $.ajax({
        type: 'POST',
        url: 'BerthMaster.aspx/flDelBerth',
        data: JSON.stringify({ lBerth: sDelData }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {
            //Refresh Data
            fGetData();
            //Clear Display
            fDispClear();
        }
        msg(sResult.Msg, sResult.Status);
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(data.d.Msg, 'error');
    });
}

//Print BerthMaster
function fPrintData(oBerthData) {
    $.ajax({
        type: 'POST',
        url: 'BerthMaster.aspx/flPrint',
        data: JSON.stringify({ lBerth: oBerthData }),
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

//Check Required Fields
function fCheck() {
    var sWharfCD = $('#MainContent_WharfCD_WharfCD').val();
    var sBerthCD = $('#textBerthCD').val();
    var sBerthName = $('#textBerthName').val();
    var iCnt = 0;

    $('.required').each(function () {
        if ($(this).val() === '') {
            showError($(this).attr('id'), getMsg('E06', $(this).attr('data-name')));
            iCnt++;
        }
    }); 
    if (iCnt > 0) {
        return false
    }
    if (sBerthCD.length < 4) {
        showError($('#textBerthCD').attr('id'), getMsg('E02', 'バースコード'));
        return false;
    }

    return true;
}

//Clear Display
function fDispClear() {
    $('.required').each(function () {
        if ($(this).value !== '' || $(this).value !== null) {
            hideError($(this).attr('id'));
        }
    });
    $("#TitleWharf").html('ワーフ<span class="text-danger">*</span>');
    $('#btnNew').prop('disabled', false);
    $('#btnUpdate').prop('disabled', true);

    $('.txtBox').val('');
    $('#MainContent_WharfCD_WharfCD').val('')
    $('#MainContent_WharfCD_WharfName').val('')

    vChanged = true;
    vID = 0;
    vOldBCD = '';
    vOldWCD = '';
    vUpdTime = '';
        
    $('#textBerthCD').val('');
    $('#MainContent_WharfCD_WharfCD').focus();
}

//Save/Update Berth
function fUpdData() {
    var obj = fGetObjectData();
    $.ajax({
        type: 'POST',
        url: 'BerthMaster.aspx/flUpdData',
        data: JSON.stringify({ pBerthInfo: obj, pIsChanged: vChanged }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {
            //Refresh Data
            fGetData();
            //Clear Display
            fDispClear();
        }
        msg(sResult.Msg, sResult.Status);
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });
}

//Get Current Berth Data
function fGetObjectData() {
    var obj = {
        BerthID: vID,
        WharfCD: $('#MainContent_WharfCD_WharfCD').val(),
        BerthCD: $('#textBerthCD').val(),
        BerthName: $('#textBerthName').val(),
        UpdTime: vUpdTime,
    };
    return obj
}

