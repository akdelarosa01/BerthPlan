$(function () {
    flGetWharfList();
    $('#btnUpdate').prop('disabled', true);

    checkAllCheckboxesInTable('.CheckboxAll', '.Checked');

    $('#btnDelete').on('click', function (e) {
        e.preventDefault();
        var table = $('#tblWharf').DataTable();
        var mWharf = [];
        $.each(table.context[0].aiDisplay, function (i, x) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                mWharf.push({
                    ID: table.context[0].aoData[x].anCells[0].firstChild.name,
                    UpdTime: ParseDateTime(table.context[0].aoData[x]._aData.UpdTime, "ss")
                });
            }
        });
        if (mWharf.length != 0) {
            bootbox.confirm({
                title: "岸壁を削除する",
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
                        $.ajax({
                            url: "WharfMaster.aspx/DeleteWharf",
                            type: 'POST',
                            data: JSON.stringify({ mWharf: mWharf }),
                            contentType: 'application/json; charset=utf-8',
                            datatype: "json",
                        }).done(function (data, textStatus, xhr) {
                            sessionOut(data.d.Status);
                            if (data.d.Status == 'success') {
                                flGetWharfList();
                                msg(getMsg('I01'), 'success');
                                $(".cls").val("");
                                $('#WharfCD').prop('readonly', false);
                            } else { msg(data.d.Msg, data.d.Status); }
                        }).fail(function (xhr, textStatus, errorThrown) {
                            console.log(errorThrown);
                        });
                    }
                }
            });
        }
        else { msg(getMsg('E03'), 'failed'); }
    });

    $('#btnClear').on('click', function (e) {
        flGetWharfList();
        Clear();
    });

    $('#tblWharf_body').on('click', 'tr', function (e) {
        e.preventDefault;
        var tableapprover = $('#tblWharf').DataTable();
        var data = tableapprover.row($(this)).data();
        hideError('WharfCD');
        hideError('WharfName');
        $('#ID').val(data.ID);
        $('#WharfCD').val(data.WharfCD);
        $('#WharfName').val(data.WharfName);
        $('#UpdTime').val(ParseDateTime(data.UpdTime, "ss"));

        $('#WharfCD').prop('readonly', true);
        $('#btnUpdate').prop('disabled', false);
        $('#btnAdd').prop('disabled', true);
    });

    $('#btnAdd').on('click', function () {
        SaveWharf();
    });

    $('#btnUpdate').on('click', function () {
        SaveWharf();
    });

    $('#btnExcel').on('click', function (e) {
        e.preventDefault();
        var table = $('#tblWharf').DataTable();
        var mWharf = [];
        var valid = true;
        var eLine = "";
        $.each(table.context[0].aiDisplay, function (i, v) {
            var data = table.context[0].aoData[v]._aData;
            mWharf.push({
                WharfCD: data.WharfCD,
                WharfName: data.WharfName
            });
        });

        $.ajax({
            url: "WharfMaster.aspx/PrintWharf",
            type: 'POST',
            data: JSON.stringify({ mWharf: mWharf }),
            contentType: 'application/json; charset=utf-8',
            datatype: "json",
        }).done(function (data, textStatus, xhr) {
            if (data.d.Status == 'success') {
                window.location.href = '.../../../Handlers/FileDownloader.ashx?FilePath=' + data.d.Data[0] + '&FileName=' + data.d.Data[1];
                msg(data.d.Msg, data.d.Status);
            } else { msg(data.d.Msg, data.d.Status); }
        }).fail(function (xhr, textStatus, errorThrown) {
            console.log(errorThrown);
        });
    });

    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1: Block F1
            case 112:
                e.preventDefault();
                break;
            //F2: SAVE
            case 113:
                e.preventDefault();
                if (!$('#btnAdd').is(':disabled')) {
                    $('#btnAdd').click();
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

function SaveWharf() {
    if (FormValidate() == true) {
        var mWharf = {
            ID: ($('#ID').val() != '') ? $('#ID').val() : 0,
            WharfCD: $('#WharfCD').val(),
            WharfName: $('#WharfName').val(),
            UpdTime: $('#UpdTime').val()
        };
        $.ajax({
            url: "WharfMaster.aspx/SaveWharf",
            type: 'POST',
            data: JSON.stringify({ mWharf: mWharf }),
            contentType: 'application/json; charset=utf-8',
            datatype: "json",
        }).done(function (data, textStatus, xhr) {
            sessionOut(data.d.Status);
            if (data.d.Status == "success") {
                flGetWharfList();
                Clear();
                msg(data.d.Msg, data.d.Status);
            } else { msg(data.d.Msg, data.d.Status); }
        }).fail(function (xhr, textStatus, errorThrown) {
            msg("", "error");
        });
    }
}

function flGetWharfList() {
    $.ajax({
        type: 'POST',
        url: "WharfMaster.aspx/flGetWharfList",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        if ($.fn.DataTable.isDataTable('#tblWharf')) {
            $('#tblWharf').DataTable().clear();
        }
        $('#tblWharf').dataTable({
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
                        return '<input type="checkbox" name="' + data.ID + '" class="checkItem " />';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { title: "岸壁コード", data: "WharfCD", width: "15%", sortable: false, orderable: false },
                { title: "岸壁名", data: "WharfName", width: "60%", sortable: false, orderable: false },
                {
                    title: "最後の更新",
                    data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "15%", sortable: false, orderable: false
                },
            ],
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblWharf thead th');

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

                    $('.sort-column').css({
                        "background-color": "#d9edf7",
                        "border": "none",
                        "width": "100%"
                    });
                }

            },
            "fnDrawCallback": function () {
                checkAllCheckboxesInTable("#tblWharf", ".checkAllitem", ".checkItem");

                $("#tblWharf_head tr").css("background-color", "#d9edf7");
            },
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
}

function FormValidate() {
    var invalid = 0;
    $('.required').each(function () {
        var id = $(this).attr('id');
        if ($(this).val() == "") {
            showError(id, getMsg('E06', $(this).attr('data-name')));
            invalid++;
        }
    });

    if (invalid != 0) { return false } 

    if ($('#WharfCD').val().length != 4 && $('#WharfCD').val().length != 0) {
        showError($('#WharfCD').attr('id'), getMsg('E02', 'ワーフコード'));
        return false;
    }

    return true;
}

function Clear() {
    $('.required').each(function () {
        if ($(this).value !== '' || $(this).value !== null) {
            hideError($(this).attr('id'));
        }
    });
    $(".cls").val("");
    $('#WharfCD').prop('readonly', false);
    $('#btnUpdate').prop('disabled', true);
    $('#btnAdd').prop('disabled', false);
    $('.required').removeAttr("style");
    $('#WharfCD').focus();
}


