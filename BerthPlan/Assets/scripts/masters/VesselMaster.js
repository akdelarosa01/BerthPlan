$(function () {
    getVesselDataTables();
    $('#btnUpdate').prop('disabled', true);

    $('#tblVessel').off('click');
    $("#tblVessel").on("click", ".CheckboxAll", function () {
        var id = $(this).attr('id');
        $('.' + id).prop('checked', $(this).is(":checked"));
    });

    $('#btnDelete').on('click', function (e) {
        e.preventDefault();
        var table = $('#tblVessel').DataTable();
        var mVessel = [];
        $.each(table.context[0].aiDisplay, function (i, x) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                mVessel.push({
                    ID: table.context[0].aoData[x].anCells[0].firstChild.name,
                    UpdTime: ParseDateTime(table.context[0].aoData[x]._aData.UpdTime, "ss"),
                });
            }
        });
        if (mVessel.length != 0) {
            bootbox.confirm({
                title: "本船を削除する",
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
                            url: "VesselMaster.aspx/DeleteVessel",
                            type: 'POST',
                            data: JSON.stringify({ mVessel: mVessel }),
                            contentType: 'application/json; charset=utf-8',
                            datatype: "json",
                        }).done(function (data, textStatus, xhr) {
                            sessionOut(data.d.Status);
                            if (data.d.Status == 'success') {
                                getVesselDataTables();
                                msg(getMsg('I01'), 'success');
                                $(".cls").val("");
                                $('#VesselCD').prop('readonly', false);
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
        getVesselDataTables();
        Clear();
    });

    $('#tblVessel_body').on('click', 'tr', function (e) {
        e.preventDefault;
        var tableapprover = $('#tblVessel').DataTable();
        var data = tableapprover.row($(this)).data();
        hideError('VesselCD');
        hideError('VesselName');
        hideError('MainContent_ApplicantCD_ApplicantCD');
        $('#ID').val(data.ID);
        $('#VesselCD').val(data.VesselCD);
        $('#IMO').val(data.IMO);
        $('#VesselName').val(data.VesselName);
        $('#GrossTon').val(data.GrossTon);
        $('#Nationality').val(data.Nationality);
        $('#MainContent_ApplicantCD_ApplicantCD').val(data.ApplicantCD);
        $('#MainContent_ApplicantCD_ApplicantName').val(data.ApplicantName);
        $('#LOA').val(data.LOA);
        $('#IO').val(data.IO.toString());
        $('#UpdTime').val(ParseDateTime(data.UpdTime, "ss"));

        $('#VesselCD').prop('readonly', true);
        $('#btnUpdate').prop('disabled', false);
        $('#btnAdd').prop('disabled', true);
    });

    $('#btnAdd').on('click', function () {
        SaveVessel();
    });

    $('#btnUpdate').on('click', function () {
        SaveVessel();
    });

    $('#btnExcel').on('click', function (e) {
        e.preventDefault();
        var table = $('#tblVessel').DataTable();
        var mVessel = [];
        var valid = true;
        var eLine = "";
        $.each(table.context[0].aiDisplay, function (i, v) {
            var data = table.context[0].aoData[v]._aData;
            mVessel.push({
                VesselCD: data.VesselCD,
                IMO: data.IMO,
                VesselName: data.VesselName,
                GrossTon: data.GrossTon,
                Nationality: data.Nationality,
                ApplicantCD: data.ApplicantCD,
                LOA: data.LOA,
                IO: data.IO
            });
        });

        $.ajax({
            url: "VesselMaster.aspx/PrintVessel",
            type: 'POST',
            data: JSON.stringify({ mVessel: mVessel }),
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

function SaveVessel() {
    if (FormValidate() == true) {
        var mVessel = {
            ID: ($('#ID').val() != '') ? $('#ID').val() : 0,
            VesselCD: $('#VesselCD').val(),
            IMO: $('#IMO').val(),
            VesselName: $('#VesselName').val(),
            GrossTon: ($('#GrossTon').val() != '') ? $('#GrossTon').val() : 0,
            Nationality: $('#Nationality').val(),
            ApplicantCD: $('#MainContent_ApplicantCD_ApplicantCD').val(),
            LOA: ($('#LOA').val() != '') ? $('#LOA').val() : 0,
            IO: ($('#IO').val() != '') ? $('#IO').val() : false , 
            UpdTime: $('#UpdTime').val()
        };
        $.ajax({
            url: "VesselMaster.aspx/SaveVessel",
            type: 'POST',
            data: JSON.stringify({ mVessel: mVessel }),
            contentType: 'application/json; charset=utf-8',
            datatype: "json",
        }).done(function (data, textStatus, xhr) {
            sessionOut(data.d.Status);
            if (data.d.Status == "success") {
                getVesselDataTables();
                Clear();
                msg(data.d.Msg, data.d.Status);
            } else {
                if (data.d.Data == "MainContent_ApplicantCD_ApplicantCD") {
                    showError($('#MainContent_ApplicantCD_ApplicantCD').attr('id'), data.d.Msg);
                } else {
                    msg(data.d.Msg, data.d.Status);
                }
            }
        }).fail(function (xhr, textStatus, errorThrown) {
            msg(errorThrown, "error");
        });
    }
}

function getVesselDataTables() {
    var $table = $('#tblVessel');

    $.ajax({
        type: 'POST',
        url: 'VesselMaster.aspx/flGetVesselList',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        if ($.fn.DataTable.isDataTable('#tblVessel')) {
            $('#tblVessel').DataTable().clear();
        }
        $('#tblVessel').dataTable({
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
                    title: '<input type="checkbox" class="checkItem" />',
                    data: function (data) {
                        return '<input type="checkbox" name="' + data.ID + '" class="Checked " />';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { title: "船コード", data: "VesselCD", width: "7%", sortable: false, orderable: false },
                { title: "IMO", data: "IMO", width: "7%", sortable: false, orderable: false },
                { title: "船名", data: "VesselName", width: "20%", sortable: false, orderable: false },
                {
                    title: "総トン数",
                    data: function (data) {
                        return numberFormat(data.GrossTon, "###,###,###,###,##0.0000")
                    }, width: "10%", sortable: false, orderable: false
                },
                {
                    title: "LOA",
                    data: function (data) {
                        return numberFormat(data.LOA, "###,###,###,###,##0.0000")
                    }, width: "10%", sortable: false, orderable: false
                },
                { title: "船籍", data: "Nationality", width: "10%", sortable: false, orderable: false },
                {
                    title: "外/内",
                    data: function (data) {
                        return (data.IO == true) ? "外" : "内";
                    }, width: "5%", sortable: false, orderable: false
                },
                { title: "申請者コード", data: "ApplicantCD", width: "7%", sortable: false, orderable: false },
                { title: "申請者名", data: "ApplicantName", width: "20%", sortable: false, orderable: false },
                {
                    title: "最後の更新",
                    data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "10%", sortable: false, orderable: false
                },
            ],
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblVessel thead th');

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
                checkAllCheckboxesInTable("#tblVessel", ".checkItem", ".Checked");

                $("#tblVessel_head tr").css("background-color", "#d9edf7");
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

    if ($('#VesselCD').val().length != 4 && $('#VesselCD').val().length != 0) {
        showError($('#VesselCD').attr('id'), getMsg('E02', '本船コード'));
        return false;
    }

    if (!$.isNumeric($('#GrossTon').val())) {
        showError($('#GrossTon').attr('id'), "正しい番号を入力してください");
        return false;
    }

    if (!$.isNumeric($('#LOA').val())) {
        showError($('#LOA').attr('id'), "正しい番号を入力してください");
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
    $('#VesselCD').prop('readonly', false);
    $('#btnUpdate').prop('disabled', true);
    $('#btnAdd').prop('disabled', false);
    $('#MainContent_ApplicantCD_ApplicantCD').val("");
    $('#MainContent_ApplicantCD_ApplicantName').val("");
    $('.required').removeAttr("style");
    $('#VesselCD').focus();
}

