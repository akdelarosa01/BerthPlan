$(function () {
    // Initialize input mask on 電話番号/Fax/PostCode
    //$('#Tel').inputmask("999-9999-99999");
    $('#Tel').inputmask("999-9999-99999", { "clearIncomplete": false, "placeholder": " " });
    //$('#Fax').inputmask("999-9999-99999");
    $('#Fax').inputmask("999-9999-99999", { "clearIncomplete": false, "placeholder": " " });
    $('#PostCode').inputmask("999-9999", { "placeholder": " " });


    GetCompanyList();
    checkAllCheckboxesInTable("#tblCompany", '.CheckboxAll', '.Checked');
    $('#btnUpdate').prop('disabled', true);

    $('#tblCompany_body').on('change', '.Checked', function () {
        checkAllIfChecked('#tblCompany_body', '.CheckboxAll', '.Checked');
    });

    $('#btnDelete').on('click', function (e) {
        var table = $('#tblCompany').DataTable();
        var mCompany = [];
        $.each(table.context[0].aiDisplay, function (i, x) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                mCompany.push({
                    ID: table.context[0].aoData[x].anCells[0].firstChild.name,
                    UpdTime: ParseDateTime(table.context[0].aoData[x]._aData.UpdTime, "ss"),
                });
            }
        });
        if (mCompany.length != 0) {
            bootbox.confirm({
                title: "会社を削除する",
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
                            url: "CompanyMaster.aspx/DeleteCompany",
                            type: 'POST',
                            data: JSON.stringify({ mCompany: mCompany }),
                            contentType: 'application/json; charset=utf-8',
                            datatype: "json",
                        }).done(function (data, textStatus, xhr) {
                            sessionOut(data.d.Status);
                            if (data.d.Status == 'success') {
                                GetCompanyList();
                                Clear();
                                msg(data.d.Msg, data.d.Status);
                            } else { msg(data.d.Msg, data.d.Status); }
                        }).fail(function (xhr, textStatus, errorThrown) {
                            console.log(errorThrown);
                        });
                    }
                }
            });
        } else { msg(getMsg('E03'), 'failed'); }
    });

    $('#btnClear').on('click', function (e) {
        GetCompanyList();
        Clear();
    });

    $('#tblCompany_body').on('click', 'tr', function (e) {
        var tableapprover = $('#tblCompany').DataTable();
        var data = tableapprover.row($(this)).data();
        hideError('ApplicantCD');
        hideError('ApplicantName');
        $('#ID').val(data.ID);
        $('#ApplicantCD').val(data.ApplicantCD);
        $('#ApplicantName').val(data.ApplicantName);
        $('#PostCode').val(data.PostCode);
        $('#Address').val(data.Address);
        $('#Tel').val(data.Tel);
        $('#Fax').val(data.Fax);
        $('#Email').val(data.Email);
        $('#Color').val(data.Color);
        $('#UpdTime').val(ParseDateTime(data.UpdTime, "ss"));

        $('#ApplicantCD').prop('readonly', true);
        $('#btnUpdate').prop('disabled', false);
        $('#btnAdd').prop('disabled', true);
    });

    $('#btnAdd').on('click', function () {
        SaveCompany();
    });

    $('#btnUpdate').on('click', function () {
        SaveCompany();
    });

    $('#btnExcel').on('click', function (e) {
        e.preventDefault();
        var table = $('#tblCompany').DataTable();
        var mCompany = [];
        var valid = true;
        var eLine = "";
        $.each(table.context[0].aiDisplay, function (i, v) {
            var data = table.context[0].aoData[v]._aData;
            mCompany.push({
                ApplicantCD: data.ApplicantCD,
                ApplicantName: data.ApplicantName,
                Address: data.Address,
                PostCode: data.PostCode,
                Tel: data.Tel,
                Fax: data.Fax,
                Email: data.Email
            });
        });

        $.ajax({
            url: "CompanyMaster.aspx/PrintCompany",
            type: 'POST',
            data: JSON.stringify({ mCompany: mCompany }),
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

function SaveCompany() {
    if (FormValidate() == true) {
        var mCompany = {
            ID: ($('#ID').val() != '') ? $('#ID').val() : 0,
            ApplicantCD: $('#ApplicantCD').val(),
            ApplicantName: $('#ApplicantName').val(),
            Address: $('#Address').val(),
            PostCode: $('#PostCode').val(),
            Tel: $('#Tel').val(),
            Fax: $('#Fax').val(),
            Email: $('#Email').val(),
            Color: $('#Color').val(),
            UpdTime: $('#UpdTime').val()
        };
        $.ajax({
            url: "CompanyMaster.aspx/SaveCompany",
            type: 'POST',
            data: JSON.stringify({ mCompany: mCompany }),
            contentType: 'application/json; charset=utf-8',
            datatype: "json",
        }).done(function (data, textStatus, xhr) {
            sessionOut(data.d.Status);
            if (data.d.Status == "success") {
                GetCompanyList();
                Clear();
                msg(data.d.Msg, data.d.Status);
            } else { msg(data.d.Msg, data.d.Status); }
        }).fail(function (xhr, textStatus, errorThrown) {
            msg(textStatus, "error");
        });
    }
}

function GetCompanyList() {
    $.ajax({
        type: 'POST',
        url: 'CompanyMaster.aspx/flGetCompanyList',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        if ($.fn.DataTable.isDataTable('#tblCompany')) {
            $('#tblCompany').DataTable().clear();
        }
        $('#tblCompany').dataTable({
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
                    title: '<input type="checkbox" id="Checked" class="checkAllitem" />', data: function (data) {
                        return '<input type="checkbox" name="' + data.ID + '" class="checkItem" />';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { title: "申請者コード", data: "ApplicantCD", sortable: false, orderable: false, width: "10%" },
                { title: "申請者名", data: "ApplicantName", sortable: false, orderable: false, width: "20%" },
                { title: "郵便番号", data: "PostCode", sortable: false, orderable: false, width: "10%" },
                { title: "住所", data: "Address", sortable: false, orderable: false, width: "15%" },
                { title: "電話番号", data: "Tel", sortable: false, orderable: false, width: "7%" },
                { title: "ファクス番号", data: "Fax", sortable: false, orderable: false, width: "8%" },
                { title: "E-メール", data: "Email", sortable: false, orderable: false, width: "10%" },
                {
                    title: "色", data: function (data) {
                        return '<div style="width:100%;height:15px;background-color:' + data.Color + '"></div>';

                    }, sortable: false, orderable: false, width: "5%"
                },
                {
                    title: "最後の更新", data: function (data) {
                        return ParseDate(data.UpdTime);
                    }, width: "10%"
                },
            ],
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblCompany thead th');

                if ($thead.find('select').length < 1) {
                    api.columns().indexes().flatten().each(function (i) {
                        if (i > 0) {
                            if (i != 8) {
                                var column = api.column(i);

                                var title = $thead.eq(i).text();
                                var select = $('<select class="sort-column"><option value="" >' + title + '</option></select>')
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
                                    select.append('<option value="' + d + '" >' + d + '</option>')
                                });
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
            "fnDrawCallback": function () {
                checkAllCheckboxesInTable("#tblCompany", ".checkAllitem", ".checkItem");

                $("#tblCompany_head tr").css("background-color", "#d9edf7");
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

    if ($('#ApplicantCD').val().length != 4 && $('#ApplicantCD').val().length != 0) {
        showError($('#ApplicantCD').attr('id'), getMsg('E02', '会社コード'));
        return false;
    }

    if (!validateEmail($('#Email').val())) {
        showError('Email', getMsg('E07', 'E-メール'));
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
    $('#ApplicantCD').prop('readonly', false);
    $('#btnUpdate').prop('disabled', true);
    $('#btnAdd').prop('disabled', false);
    $('.required').removeAttr("style");
    $('#ApplicantCD').focus();
}

