/** 
* <summary>
*  ユーザーマスター JS
* </summary>
* <history>
* ---VERSION----対応日付---------対応者----------対応内容----
*    00.01      2020/03/13       K.Ga　　        コードを作った。
* </history>
*/

//*
//  Variables
var vIsDeleted = false;
var vUserID = 0;
var vUpdTime = '';
//*

$(function () {
    //Initialize Display
    fDispClear();

    //Get User List
    fGetUsersData();

    //Check all checkboxes
    checkAllCheckboxesInTable("#tblUser", ".checkAllitem", ".checkItem");

    //Add User Event
    $('#btnNew').on('click', function () {
        //Check Req Fields
        if (!fCheck()) {
            return;
        }
        //Save Data
        fUpdData();
    });

    ///Update User Event
    $('#btnUpdate').on('click', function () {
        //Check Req Fields
        if (!fCheck()) {
            return;
        }
        if (vIsDeleted) {
            msg('登録処理に失敗しました。 すでに削除されています。', 'failed');
            return;
        }
        //Update Data
        fUpdData();
    });

    //Delete Event
    $('#btnDelete').on('click', function () {
        var sMsg = getMsg('Q01');
        var table = $('#tblUser').DataTable();
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
            title: "ユーザーを削除する",
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
        var oUserData = [];
        var tUser = $('#tblUser').DataTable();

        $.each(tUser.context[0].aiDisplay, function (indx, val) {
            var data = tUser.context[0].aoData[val];

            oUserData.push({
                ID: data._aData.ID,
                UserID: data._aData.UserID,
                UserName: data._aData.UserName,
                EmailAddress: data._aData.EmailAddress,
                ApplicantCD: data._aData.ApplicantName,   //4.19.2020
                Flag: data._aData.Flag
            });
        });

        if (oUserData.length > 0) {
            fPrintData(oUserData);
        } else {
            msg(getMsg('E08'), 'failed');
        }
    })

    //Clear Event
    $('#btnClear').on('click', function () {
        //Initialize and Clear Display
        fDispClear();
        fGetUsersData()
    });

    //Table Click Event
    $('#tblUser_body').on('click', 'tr', function (e) {
        e.preventDefault;
        var tUser = $('#tblUser').DataTable();
        var sData = tUser.row($(this)).data();

        $('.required').each(function () {
            hideError($(this).attr('id'));
        });

        vUserID = sData.ID;
        vIsDeleted = sData.Flag;
        vUpdTime = ParseDateTime(sData.UpdTime, 'ss');
        $('#textUserID').val(sData.UserID);
        $('#textUserName').val(sData.UserName);
        $('#textPassword').val(sData.Password);
        $('#textEmail').val(sData.EmailAddress);
        $('#MainContent_ApplicantCD_ApplicantCD').val(sData.ApplicantCD);
        $('#MainContent_ApplicantCD_ApplicantName').val(sData.ApplicantName);

        //$('#checkAuthorize').prop('checked', false);
        $('#checkAuthorize').prop('checked', sData.IsAdmin == true ? true : false);
        //if (sData.IsAdmin) {
        //    $('#checkAuthorize').prop('checked', true);
        //}

        $('#textUserID').prop('readonly', true);
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

//Get UserData
function fGetUsersData() {
    var $table = $('#tblUser');

    $.ajax({
        type: 'POST',
        url: 'UserMaster.aspx/fGetData',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        if ($.fn.DataTable.isDataTable('#tblUser')) {
            $table.DataTable().clear();
        }
        $table.dataTable({
            "data": data.d,
            "cache": false,
            "destroy": true,
            "language": dataTableLanguageVariable(),
            "sDom": "rtipl",
            "lengthChange": false,
            //"order": [[7, "desc"]],
            "processing": true,
            "responsive": true,
            "columns": [
                {
                    title: '<input type="checkbox" class="checkAllitem" />',
                    data: function (data) {
                        var checked = (data.Flag == 1) ? ' disabled="disabled"' : '';

                        return '<input type="checkbox" data-time="' + ParseDateTime(data.UpdTime, 'ss') + '" data-id="' + data.ID + '"  name="' + data.ID + '" class="checkItem" ' + checked + '>';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { title: "ユーザーID", data: "UserID", width: "15%", sortable: false, orderable: false },
                { title: "ユーザー名", data: "UserName", width: "20%", sortable: false, orderable: false },
                { title: "メール", data: 'EmailAddress', width: "15%", sortable: false, orderable: false },
                { title: "申請者名", data: "ApplicantName", width: "10%", sortable: false, orderable: false }, //Added 4.19.2020
                {
                    title: "管理権限", data: function (data) {
                        if (data.IsAdmin) {
                            return '〇';
                        } else {
                            return ' ';
                        }
                    }, width: "10%", sortable: false, orderable: false
                },
                {
                    title: "削除した", data: function (data) {
                        if (data.Flag) {
                            return '〇';
                        } else {
                            return ' ';
                        }
                    }, width: "10%", sortable: false, orderable: false
                },
                {
                    title: "最終ログイン日", data: function (data) {
                        if (data.LastLogin) {
                            return ParseDate(data.LastLogin);
                        } else {
                            return '';
                        }
                    }, width: "10%", sortable: false, orderable: false
                },
                {
                    title: "最後の更新", data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "10%", sortable: false, orderable: false
                }
            ],
            "createdRow": function (row, data, dataIndex) {
                if (data.Flag) {
                    $(row).css('background-color', '#ff6266');
                    $(row).css('color', '#fff');
                }
            },
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblUser thead th');

                if ($thead.find('select').length < 1) {
                    api.columns().indexes().flatten().each(function (i) {
                        if (i > 0) {
                            var column = api.column(i);
                            var title = $thead.eq(i).text();
                            var select = $('<select class="sort-column" name="' + i + '"><option value="">' + title + '</option></select>')
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
                checkAllCheckboxesInTable("#tblUser", ".checkAllitem", ".checkItem");

                $("#tblUser_head tr").css("background-color", "#d9edf7");
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
        if ($('#' + id).val() == "") {
            showError(id, getMsg('E06', $(this).attr('data-name')));
            invalid++;
        }
    });
    if (invalid != 0) { return false }

    if ($('#textUserID').val().length != 4) {
        showError('textUserID', getMsg('E02', 'ユーザーID'));
        return false;
    }
    if ($('#textPassword').val().length < 5) {
        showError('textPassword', getMsg('E02', 'パスワード'));
        return false;
    }
    if (!validateEmail($('#textEmail').val())) {
        showError('textEmail', getMsg('E07', 'E-メール'));
        return false;
    }
    return true;
}

//Initiliaze and Clear Display
function fDispClear() {
    vIsDeleted = false;
    vUserID = 0;
    vUpdTime = '';

    $('.txtBox').val('');
    $('#MainContent_ApplicantCD_ApplicantCD').val('');
    $('#MainContent_ApplicantCD_ApplicantName').val('');
    $('#checkAuthorize').prop('checked', false);

    $('#btnUpdate').prop('disabled', true);
    $('#textUserID').prop('readonly', false);
    $('#btnNew').prop('disabled', false);

    $('#textUserID').focus();
}

//Save/Update Function
function fUpdData() {
    var oUser = fGetObjData();
    $.ajax({
        type: 'POST',
        url: 'UserMaster.aspx/flUpdData',
        data: JSON.stringify({ pUserInfo: oUser }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {
            bootbox.dialog({
                title: '<div style="color: #1BA39C"><strong><i class="fa fa-check"></i></strong> ' + jsUcfirst(sResult.Status) + '</div>',
                message: sResult.Msg,
                size: 'small',
                buttons: {
                    ok: {
                        label: "OK!",
                        className: 'btn-sm btn-success',
                        callback: function () {
                            //Refresh Data
                            fGetUsersData();
                            //Clear Display
                            fDispClear();

                            location.reload();
                        }
                    }
                }
            });
        } else {
            if (sResult.Data) {
                $('#MainContent_ApplicantCD_ApplicantCD').val('');
                $('#MainContent_ApplicantCD_ApplicantName').val('');

                showError('MainContent_ApplicantCD_ApplicantCD', getMsg('E06', '申請者'));
            } else {
                msg(sResult.Msg, sResult.Status);
            }
        }
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });
}

//Get ObjectUser Data
function fGetObjData() {
    var obj = {
        ID: vUserID,
        UserID: $('#textUserID').val(),
        UserName: $('#textUserName').val(),
        Password: $('#textPassword').val(),
        EmailAddress: $('#textEmail').val(),
        ApplicantCD: $('#MainContent_ApplicantCD_ApplicantCD').val(), //4.19.2020
        Flag: vIsDeleted,
        IsAdmin: $('#checkAuthorize').is(':checked'),
        UpdTime: vUpdTime
    };

    return obj;
}

//Delete Function
function fDelData(sDelData) {
    $.ajax({
        type: 'POST',
        url: 'UserMaster.aspx/flDelUser',
        data: JSON.stringify({ lUser: sDelData }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        var sResult = data.d;
        sessionOut(sResult.Status);
        if (sResult.Status == 'success') {
            bootbox.dialog({
                title: '<div style="color: #1BA39C"><strong><i class="fa fa-check"></i></strong> ' + jsUcfirst(sResult.Status) + '</div>',
                message: sResult.Msg,
                size: 'small',
                buttons: {
                    ok: {
                        label: "OK!",
                        className: 'btn-sm btn-success',
                        callback: function () {
                            if (sResult.Data) {
                                sessionOut('expire');
                            }
                            //Refresh Data
                            fGetUsersData();
                            //Clear Display
                            fDispClear();
                        }
                    }
                }
            });
        } else {
            msg(sResult.Msg, sResult.Status);
        }
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(textStatus, 'error');
    });

}

//Print Data Function
function fPrintData(oUserData) {
    $.ajax({
        type: 'POST',
        url: 'UserMaster.aspx/flPrint',
        data: JSON.stringify({ lUser: oUserData }),
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

