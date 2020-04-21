/** 
* <summary>
*  掲示板登録JS
* </summary>
* <history>
* ---VERSION----対応日付---------対応者----------対応内容----
*    00.01      2020/03/12      AK.Dela Rosa　　コードを作った。
* </history>
*/

var pgTitle = "掲示板登録";
var frmData = new FormData();
var FileData = {};
var FileTableData = [];
var fileId = 0;
var fileDataHolder = [];
/**
* 
* Page Events
*
*/
$(function () {

    flPageInit()

    // Button 'New' Events
    $('#btnNew').off('click');
    $('#btnNew').on('click', function () {
        flPageViewState('ADD');
        $('#PostingStartDate_grp').data("DateTimePicker").maxDate(false);
        $('#PostingEndDate_grp').data("DateTimePicker").minDate(false);

        $('#PostingStartDate_grp').data("DateTimePicker").minDate(DateNow());
        $('#PostingEndDate_grp').data("DateTimePicker").minDate(DateNow());
        $('#btnClear').html("キャンセル(F4)");
        flGetFileData(0);
        FileData = {};
        FileTableData = [];
        fileDataHolder = [];
        $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");
        $('#MainContent_FileAttachment').val("");
    });

    // Prevent selecting out of range date
    $("#PostingStartDate_grp").on("dp.change", function (e) {
        $('#PostingEndDate_grp').data("DateTimePicker").minDate(e.date);
    });
    $("#PostingEndDate_grp").on("dp.change", function (e) {
        $('#PostingStartDate_grp').data("DateTimePicker").maxDate(e.date);
    });

    // Save button on click Event
    $('#MainContent_btnSave').on('click', function (e) {
        if (flValidate()) {
            var filetxt = $(':file').parents('.input-group').find(':text').val();

            if (filetxt !== "ファイルを選択してください。") {
                msg('保存する前にファイルをアップロードしてください。', 'failed');
            } else {
                ShowLoading();

                frmData.append($('#txtBoardID').attr('id'), $('#txtBoardID').val());
                frmData.append($('#txtTitle').attr('id'), $('#txtTitle').val());
                frmData.append($('#txtContent').attr('id'), $('#txtContent').val());
                frmData.append($('#txtLink').attr('id'), $('#txtLink').val());
                frmData.append($('#PostingStartDate').attr('id'), $('#PostingStartDate').val());
                frmData.append($('#PostingEndDate').attr('id'), $('#PostingEndDate').val());
                frmData.append($('#hdStatus').attr('id'), $('#hdStatus').val());
                frmData.append($('#hdUserID').attr('id'), $('#hdUserID').val());
                frmData.append($('#hdUpdTime').attr('id'), $('#hdUpdTime').val());

                for (var i = 0; i < fileDataHolder.length; i++) {
                    frmData.append(fileDataHolder[i].key, fileDataHolder[i].data);
                }

                $.ajax({
                    url: '.../../../Handlers/BoardFileHandler.ashx',
                    type: 'POST',
                    data: frmData,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        sessionOut(response.status);
                        if (response.status === 'success') {
                            FileData = {};
                            FileTableData = [];
                            fileDataHolder = [];
                            flGetFileData(($('#txtBoardID').val() == "") ? 0 : $('#txtBoardID').val());
                            clear('.clear');
                            flBulletinDatatable();

                            $("#MainContent_fileAttachment").val("");
                            $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");


                            flPageViewState();
                            HideLoading();
                            msg(response.msg, response.status)
                        } else {
                            HideLoading();
                            msg(response.msg, 'failed')
                            frmData = new FormData();
                        }

                    },
                    xhr: function () {
                        var fileXhr = $.ajaxSettings.xhr();
                        if (fileXhr.upload) {
                            $("progress").show();
                            fileXhr.upload.addEventListener("progress", function (e) {
                                if (e.lengthComputable) {
                                    $("#fileProgress").attr({
                                        value: e.loaded,
                                        max: e.total
                                    });
                                }
                            }, false);
                        }
                        return fileXhr;
                    },
                    error: function (response) {
                        fileDataHolder = [];
                        FileTableData = [];
                        flGetFileData(($('#txtBoardID').val() == "") ? 0 : $('#txtBoardID').val());
                        clear('.clear');
                        flBulletinDatatable();
                        $("#MainContent_fileAttachment").val("");
                        $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");
                        flPageViewState();
                        HideLoading();
                        msg(response.responseJSON.msg, response.responseJSON.status);

                    }
                });

                frmData = new FormData();
            }

        }

    });

    $('#btnUpload').on('click', function () {
        FileData = {};
        var iCount = FileTableData.length;

        if ($(':file').parents('.input-group').find(':text').val() == "ファイルを選択してください。") {
            msg(getMsg("E08"), "failed");
        }else {
            $.each($('input[type=file]')[0].files, function (i, file) {
                FileData[i] = file;
            });

            var error = 0;
            for (var i in FileData) {
                if (iCount >= 5) {
                    msg('アップロードするファイルは5を超えてはなりません。', 'failed');
                    error++;
                }
                iCount++;

                for (var ind = 0; ind < FileTableData.length; ind++) {
                    if (FileTableData[ind].FileName == FileData[i].name) {
                        msg('ファイルの1つはすでにアップロードされています。', 'failed');
                        error++;
                    }
                }

                var checkSize = FileData[i].size / 1024 / 1024;

                if (checkSize > 3) {
                    var size = checkSize.toFixed(2);

                    msg('このファイル' + FileData[i].name + 'は大きすぎ、' + size + 'MB サイズです。 3MB以下のファイルサイズをアップロードしてください。', 'failed');
                    error++;
                }

                if (!error > 0) {
                    var filetype = FileData[i].name.split('.');

                    fileDataHolder.push({
                        key: i,
                        data: FileData[i],
                        BoardFileID: fileId++,
                        FileName: FileData[i].name,
                        FileType: '.' + filetype[1],
                        FileSize: (FileData[i].size / 1024).toFixed(2),
                        UploadDate: '',
                        BoardID: ($('#txtBoardID').val() == "") ? 0 : $('#txtBoardID').val(),
                    });
                }

            }
            FileData = {};
            $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");
            $('#MainContent_FileAttachment').val("");
            flGetFileData(($('#txtBoardID').val() == "") ? 0 : $('#txtBoardID').val());
        }
    });

    // Button 'Clear' on click Event
    $('#btnClear').off('click');
    $('#btnClear').on('click', function () {
        flPageViewState();
        $(this).html("クリアー(F4)");
        clear(".clear");
        FileData = {};
        FileTableData = [];
        fileDataHolder = [];
        flGetFileData(0);

        $(".checkItem").prop("checked", false);
        $(".checkAllitem").prop("checked", false);

        $('#MainContent_fileAttachment').val("");
        $('#file-label').val("");
        $('#PostingStartDate_grp').data("DateTimePicker").maxDate(false);
        $('#PostingEndDate_grp').data("DateTimePicker").minDate(false);

        $('#PostingStartDate_grp').data("DateTimePicker").minDate(DateNow());
        $('#PostingEndDate_grp').data("DateTimePicker").minDate(DateNow());

        $('.required').each(function (i, x) {
            hideError(x.id);
        });

        $('.required-date').each(function (i, x) {
            hideError(x.id);
        });
    });

    // Button 'Delete' on click Event
    $('#btnDelete').off('click');
    $('#btnDelete').on('click', function () {
        var ids = [];
        var msgs = getMsg('Q01');
        var table = $('#tblBulletin').DataTable();

        for (var x = 0; x < table.context[0].aoData.length; x++) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                ids.push(table.context[0].aoData[x].anCells[0].firstChild.attributes['data-id'].value)
            }
        }

        if (ids.length === 0) {
            msg(getMsg('E03'), 'failed');
        }

        if (ids.length > 0) {

            bootbox.confirm({
                title: pgTitle,
                size: 'small',
                message: msgs,
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
                        var id = "";

                        for (var i = 0; i < ids.length; i++) {
                            id += ids[i];

                            if (i !== (ids.length - 1)) {
                                id += ",";
                            }
                        }

                        ShowLoading();
                        $.ajax({
                            url: '.../../../Pages/BoardRegistration.aspx/flDeleteBoard',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            datatype: "json",
                            data: JSON.stringify({ ids: id }),
                        }).done(function (data, textStatus, xhr) {
                            FileData = {};
                            FileTableData = [];
                            fileDataHolder = [];
                            clear('.clear');
                            flBulletinDatatable();
                            flGetFileData(0);
                            HideLoading();
                            msg(data.d.Msg, data.d.Status);
                        }).fail(function (xhr, textStatus, errorThrown) {
                            HideLoading();
                            msg('掲示板登録: ' + errorThrown, 'error');
                        });
                    }
                }
            });
        }
    });

    // Button 'Delete File' on click Event
    $('#btnDeleteFile').off('click');
    $('#btnDeleteFile').on('click', function () {
        var ids = [];
        var msgs = getMsg('Q01');
        var table = $('#tblFiles').DataTable();
        var BoardID = table.context[0].aoData[0]._aData.BoardID;

        for (var x = 0; x < table.context[0].aoData.length; x++) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                console.log(table.context[0].aoData[x].anCells[0].firstChild);
                ids.push(table.context[0].aoData[x].anCells[0].firstChild.attributes['data-id'].value)
            }
        }

        if (ids.length === 0) {
            msg(getMsg('E03'), 'failed');
        }


        if (ids.length > 0) {

            bootbox.confirm({
                title: pgTitle,
                size: 'small',
                message: msgs,
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
                        var id = "";

                        for (var i = 0; i < ids.length; i++) {
                            id += ids[i];

                            if (i !== (ids.length - 1)) {
                                id += ",";
                            }
                        }

                        ShowLoading();

                        $.ajax({
                            url: '.../../../Pages/BoardRegistration.aspx/flDeleteBoardFiles',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            datatype: "json",
                            data: JSON.stringify({ ids: id }),
                        }).done(function (data, textStatus, xhr) {
                            //clear('.clear');
                            flGetFileData(BoardID);
                            HideLoading();
                            msg(data.d.Msg, data.d.Status);
                        }).fail(function (xhr, textStatus, errorThrown) {
                            HideLoading();
                            msg('掲示板登録: ' + errorThrown, 'error');
                        });


                    }
                }
            });
        }
    });

    // Posting Start Date Keydown Event
    $('#PostingStartDate').on('keydown', function () {
        $('#PostingDate_grp').removeClass('has-error');
        $('#PostingDate_msg').html('');
    });

    // Posting End Date Keydown Event
    $('#PostingEndDate').on('keydown', function () {
        $('#PostingDate_grp').removeClass('has-error');
        $('#PostingDate_msg').html('');
    });

    // Upload file input on change Event
    $(document).on('change', ':file', function () {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [numFiles, label]);
    });

    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1: Block F1
            case 112:
                e.preventDefault();
                if (!$('#btnNew').is(':disabled')) {
                    $('#btnNew').click();
                }
                break;
            //F2: SAVE
            case 113:
                e.preventDefault();
                if (!$('#MainContent_btnSave').is(':disabled')) {
                    $('#MainContent_btnSave').click();
                }
                break;
            //F3: UPDATE
            case 114:
                e.preventDefault();
                if (!$('#btnUpload').is(':disabled')) {
                    $('#btnUpload').click();
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
                if (!$('#btnDelete').is(':disabled')) {
                    $('#btnDelete').click();
                }
                break;
            //F10: 
            case 121:
                e.preventDefault();
                break;
            //F12: CLOSE
            case 123:
                e.preventDefault();
                window.location.href = ".../../../Pages/SystemMenu"
                break;
            default:

        }
    });

    $(':file').on('fileselect', function (event, numFiles, label) {
        var input = $(this).parents('.input-group').find(':text'),
            log = numFiles > 1 ? numFiles + ' つのファイルを選択' : label;

        if (input.length) {
            input.val(log);
        } else {
            if (log) alert(log);
        }

    });

    $("#MainContent_fileAttachment").val("");
    $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");

    $('#txtLink').on('keydown', function (e) {
        if (e.target.value !== '' || e.target.value !== null) {
            hideError(e.target.id);
        }
    });

    // DataTables: Pagination on click Event
    $('#tblBulletin_paginate').on('click', 'a', function () {
        if ($('.checkAllitem').is(':checked')) {
            $('.checkAllitem').attr('checked', false);
        }
    });

    // DataTables: CheckBox checked Event
    $('#tblBulletin_body').on('change', '.checkItem', function () {
        checkAllIfChecked('#tblBulletin', '.checkAllitem', '.checkItem', "#btnDelete");
    });

    // DataTables: Row Click event
    $('#tblBulletin_body').off('click', 'tr');
    $("#tblBulletin_body").on('click', 'tr', function (e) {
        switch (e.target.type) {
            case "button":
                flPageViewState();
                clear(".clear");
                e.stopPropagation();
                break;
            case "checkbox":
                var $checkbox = $(this).find(':checkbox');
                if (!$checkbox.is(':checked')) {
                    flPageViewState();
                    clear(".clear");
                    $('#btnDelete').prop('disabled', true);
                } else {
                    $('#btnDelete').prop('disabled', false);
                }
                e.stopPropagation();
                break;
            default:
                var table = $('#tblBulletin').DataTable();
                var data = table.row(this).data();
                FileData = {};
                FileTableData = [];
                fileDataHolder = [];
                $('#txtBoardID').val(data.BoardID);
                $('#txtTitle').val(data.Title);
                $('#txtContent').val(data.Contents);
                $('#txtLink').val(data.HyperLink);
                $('#PostingStartDate').val(ParseDate(data.PostingStartDate));
                $('#PostingEndDate').val(ParseDate(data.PostingEndDate));
                $('#PostingEndDate_grp').data("DateTimePicker").minDate(ParseDate(data.PostingStartDate));
                $('#PostingStartDate_grp').data("DateTimePicker").maxDate(ParseDate(data.PostingEndDate));

                $('#hdUpdTime').val(ParseDateTime(data.UpdTime, "ss"));

                $('#MainContent_fileAttachment').val("");
                $('#file-label').html("")

                $('#hdStatus').val("EDIT");

                flGetFileData(data.BoardID);
                $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");
                $('#MainContent_FileAttachment').val("");

                flPageViewState("EDIT");
                e.stopPropagation();
                break;
        }
        $(this).unbind('click');
    });

    // DataTables: View users have viewed the messages
    $('#tblBulletin_body').off('click', '.btn-view-user');
    $('#tblBulletin_body').on('click', '.btn-view-user', function () {
        //$('#msgTitle').text($(this).attr('data-title'));
        $('#BoardViewModal').modal('show');
        flUserViewDatatable($(this).attr('data-id'));
    });

    // DataTables: CheckBox checked Event Instance
    checkAllCheckboxesInTable(".checkAllFileItem", ".checkFileItem");

    // DataTables: Pagination on click Event
    $('#tblFiles_paginate').on('click', 'a', function () {
        if ($('.checkAllFileItem').is(':checked')) {
            $('.checkAllFileItem').attr('checked', false);
        }
    });

    // DataTables: CheckBox checked Event
    $('#tblFiles_body').on('change', '.checkFileItem', function () {
        checkAllIfChecked('#tblFiles', '.checkAllFileItem', '.checkFileItem', '#btnDeleteFile');
    });
});

/**
* Page Initialization funciton
*/
function flGetFileData(BoardID) {
    $.ajax({
        type: 'POST',
        url: '.../../../Pages/BoardRegistration.aspx/flGetFiles',
        data: '{ BoardID: ' + BoardID + ' }',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        flFileDatatable(data.d);
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
}

/**
* Page Initialization funciton
*/
function flPageInit() {
    $('#hdStatus').val('VIEW');
    clear('.clear');
    flPageViewState();
    flBulletinDatatable();
    flGetFileData(0);

    $('.hide').each(function (i, x) {
        $(this).css('display', 'none');
    });
}

/**
* View State funciton
*/
function flPageViewState(state) {
    switch (state) {
        case 'EDIT':
            $('#btnNew').attr('disabled', true);
            $('#txtTitle').attr('disabled', false);
            $('#txtContent').attr('disabled', false);
            $('#txtLink').attr('disabled', false);
            $('#PostingStartDate').attr('disabled', false);
            $('#PostingEndDate').attr('disabled', false);
            $('#MainContent_fileAttachment').attr('disabled', false);
            $('.btn-choose').attr('disabled', false);
            $('.btn-reset').attr('disabled', false);
            $('#MainContent_btnSave').attr('disabled', false);
            $('#btnUpload').attr('disabled', false);
            $('#btnClear').attr('disabled', false);
            $('#hdStatus').val("EDIT");
            $('#tblFiles .checkAllFileItem').attr('disabled', false);
            break;
        case 'ADD':
            $('#btnNew').attr('disabled', true);
            $('#txtTitle').attr('disabled', false);
            $('#txtContent').attr('disabled', false);
            $('#txtLink').attr('disabled', false);
            $('#PostingStartDate').attr('disabled', false);
            $('#PostingEndDate').attr('disabled', false);
            $('#MainContent_fileAttachment').attr('disabled', false);
            $('#MainContent_btnSave').attr('disabled', false);
            $('#btnUpload').attr('disabled', false);
            $('#btnClear').attr('disabled', false);
            $(".checkItem").prop("disabled", true);
            $(".checkAllitem").prop("disabled", true);
            $('#hdStatus').val("ADD");
            $('#tblFiles .checkAllFileItem').attr('disabled', true);
            break;
        default:
            $('#btnNew').attr('disabled', false);
            $('#txtTitle').attr('disabled', true);
            $('#txtContent').attr('disabled', true);
            $('#txtLink').attr('disabled', true);
            $('#PostingStartDate').attr('disabled', true);
            $('#PostingEndDate').attr('disabled', true);
            $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");
            $('#MainContent_fileAttachment').attr('disabled', true);
            $(".checkItem").prop("disabled", false);
            $(".checkAllitem").prop("disabled", false);
            $('#MainContent_btnSave').attr('disabled', true);
            $('#btnUpload').attr('disabled', true);
            $('#btnClear').attr('disabled', true);
            $('#btnDelete').attr('disabled', true);
            $('#btnDeleteFile').attr('disabled', true);
            $('#hdStatus').val("");
            $('#tblFiles .checkAllFileItem').attr('disabled', true);
            break;
    }
    $('#hdUserID').val($('#lblUserID').html());
}

/**
* Get Bulletin Board Data Function
*/
function flBulletinDatatable() {
    var $table = $('#tblBulletin');

    $.ajax({
        type: 'POST',
        url: '.../../../Pages/BoardRegistration.aspx/flGetBulletin',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        $table.dataTable().fnClearTable();
        $table.dataTable().fnDestroy();
        $table.dataTable({
            "data": data.d,
            "language": dataTableLanguageVariable(),
            "sDom": "rtipl",
            "lengthChange": false,
            "order": [[5, "desc"]],
            "processing": true,
            "responsive": true,
            "pageLength": 5,
            "columnDefs": [
                { "width": "3%", "targets": [0] },
                { "targets": 0, "sortable": false, "orderable": false },
            ],
            "columns": [
                {
                    data: function (data) {
                        return '<input type="checkbox" data-id="' + data.BoardID + '" class="checkItem" />';
                    }, sortable: false, orderable: false, width: "3%",
                    title: '<input type="checkbox" class="checkAllitem" />'
                },
                {
                    data: function (data) {
                        return '<button type="button" data-id="' + data.BoardID + '" data-title="' + data.Title + '" ' +
                                    'class="btn btn-sm btn-primary btn-view-user">' +
                                    '<i class="fa fa-eye"></i>' +
                                '</button>';
                    }, sortable: false, orderable: false, width: "5%",
                    title: ''
                },
                {
                    data: function (data) {
                        var t = data.Title;
                        return t.replace(/(.{30})..+/, "$1&hellip;");
                    }, width: "37%", sortable: false, orderable: false,
                    title: 'タイトル'
                },
                {
                    data: function (data) {
                        return ParseDate(data.PostingStartDate)
                    }, width: "20%", sortable: false, orderable: false,
                    title: '掲載開始日'
                },
                {
                    data: function (data) {
                        return ParseDate(data.PostingEndDate)
                    }, width: "20%", sortable: false, orderable: false,
                    title: '掲載終了日'
                },
                {
                    data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "20%", sortable: false, orderable: false,
                    title: '最後の更新'
                },
            ],
            "initComplete": function () {
                var api = this.api();
                var $thead = $('#tblBulletin thead th')

                if ($thead.find('select').length < 1) {
                    api.columns().indexes().flatten().each(function (i) {
                        if (i > 1) {
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

                            column.data().unique().sort().each(function (d, j) {
                                select.append('<option value="' + d + '">' + d + '</option>')
                            });
                        }

                        $('.sort-column').css("background-color", "#d9edf7");
                        $('.sort-column').css("border", "none");
                        $('.sort-column').css("width", "100%");

                    });
                }

            },
            "fnDrawCallback": function () {
                checkAllCheckboxesInTable("#tblBulletin", ".checkAllitem", ".checkItem", "#btnDelete");
            },
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });


}

/**
* Bulletin FILES Table Events
*/
function flFileDatatable(data) {
    var $table = $('#tblFiles');
    var iRow = data.length;
    for (var i = 0; i < fileDataHolder.length; i++) {
        data.push({
            BoardFileID: fileDataHolder[i].key,
            FileName: fileDataHolder[i].FileName,
            FileType: fileDataHolder[i].FileType,
            FileSize: fileDataHolder[i].FileSize,
            UploadDate: '',
            BoardID: ($('#txtBoardID').val() == "") ? 0 : $('#txtBoardID').val(),
        });
    }
    FileTableData = data;

    $table.dataTable().fnClearTable();
    $table.dataTable().fnDestroy();
    $table.dataTable({
        "data": data,
        "columnDefs": [
            { "width": "3%", "targets": [0] },
            { "targets": 0, "sortable": false, "orderable": false },
        ],
        "language": dataTableLanguageVariable(),
        "sDom": "rtipl",
        "lengthChange": false,
        "searching": false,
        "order": [[4, "desc"]],
        "processing": true,
        "responsive": true,
        "columns": [
            {
                data: function (data) {
                    if (data.UploadDate == "") {
                        return '<span class=" text-danger btnRemoveFile" data-id="' + data.BoardFileID + '" data-BoardID="' + data.BoardID + '"><i class="fa fa-times"></i></span>';
                    } else {
                        return '<input type="checkbox" data-id="' + data.BoardFileID + '" class="checkFileItem" />';
                    }
                }, sortable: false, orderable: false, width: "3%"
            },
            { data: "FileName", width: "37%" },
            {
                data: function (data) {
                    if (data.FileType !== null || data.FileType !== "") {
                        var file = fileExtension(data.FileType);
                        return '<i class="' + file.icon + '"></i> ' + file.ext;
                    }
                    return data.FileType;
                }, width: "20%"
            },
            {
                data: function (data) {
                    return numberFormat(data.FileSize) + "kb"
                }, width: "20%"
            },
            {
                data: function (data) {
                    return ParseDate(data.UploadDate)
                }, width: "20%"
            },
        ],
        "initComplete": function () {
            HideLoading();
        },
        "fnDrawCallback": function () {
            checkAllCheckboxesInTable("#tblFiles", ".checkAllFileItem", ".checkFileItem", "#btnDeleteFile");
            // Button 'Remove File' on click Event
            $('#tblFiles_body').off('click', '.btnRemoveFile');
            $('#tblFiles_body').on('click', '.btnRemoveFile', function (e) {
                fileDataHolder.splice($(this).attr('data-id'),1);
                flGetFileData($(this).attr('data-BoardID'));
            });


        },
    });
}

function flReDrawFileTable(data) {
    var $table = $('#tblFiles');

    $table.dataTable().fnClearTable();
    $table.dataTable().fnDestroy();
    $table.dataTable({
        "data": data,
        "columnDefs": [
            { "width": "3%", "targets": [0] },
            { "targets": 0, "sortable": false, "orderable": false },
        ],
        "language": dataTableLanguageVariable(),
        "sDom": "rtipl",
        "lengthChange": false,
        "searching": false,
        "order": [[4, "desc"]],
        "processing": true,
        "responsive": true,
        "columns": [
            {
                data: function (data) {
                    return '<button type="button" class="btn btn-sm btn-danger" data-id="' + data.BoardFileID + '"><i class="fa fa-times"></i></button>';
                }, sortable: false, orderable: false, width: "3%"
            },
            { data: "FileName", width: "37%" },
            {
                data: function (data) {
                    if (data.FileType !== null || data.FileType !== "") {
                        var file = fileExtension(data.FileType);
                        return '<i class="' + file.icon + '"></i> ' + file.ext;
                    }
                    return data.FileType;
                }, width: "20%"
            },
            {
                data: function (data) {
                    return numberFormat(data.FileSize) + "kb"
                }, width: "20%"
            },
            {
                data: function (data) {
                    return ParseDate(data.UploadDate)
                }, width: "20%"
            },
        ],
        "initComplete": function () {
            $(':file').parents('.input-group').find(':text').val("ファイルを選択してください。");
        },
    });
}

/** 
* Get Users have already viewed the messages
*/
function flUserViewDatatable(BoardID) {
    var $table = $('#tblBoardViewModal');

    $.ajax({
        type: 'POST',
        url: '.../../../Pages/BoardRegistration.aspx/flGetViewedUsers',
        data: '{ BoardID: ' + BoardID + '}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        $table.dataTable().fnClearTable();
        $table.dataTable().fnDestroy();
        $table.dataTable({
            "data": data.d,
            "columnDefs": [
                { "width": "5%", "targets": [0] },
                { "className": "text-center custom-middle-align td-checkbox", "targets": [0] },
                { "targets": 0, "sortable": false, "orderable": false },
            ],
            "language": dataTableLanguageVariable(),
            "sDom": "rtipl",
            "lengthChange": false,
            "searching": false,
            "order": [[3, "desc"]],
            "processing": true,
            "responsive": true,
            "columns": [
                { data: "UserID", width: "20%" },
                { data: "UserName", width: "30%" },
                { data: "IsChecked", width: "20%" },
                {
                    data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "30%"
                },
            ],
            "createdRow": function (row, data, dataIndex) {
                if (data.IsChecked === '○') {
                    $(row).css('background-color', '#8abaae');
                    //$(row).css('color', '#fff');
                }
            }
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        console.log('error');
    });
}

/**
* Checking validation
*/
function flValidate() {
    var invalid = 0;
    $('.required').each(function (e) {
        var $this = $(this);
        if ($this.val() === "") {
            //if ($this.attr('id') === "PostingStartDate" || $this.attr('id') === "PostingEndDate") {
            //    $('#PostingDate_grp').addClass('has-error');
            //    $('#PostingDate_msg').html(getMsg('E06', $this.attr('data-name')));
            //} else {

            //}

            showError($this.attr('id'), getMsg('E06', $this.attr('data-name')));
            invalid++;
        }
    });

    if ($('#txtLink').val() !== "" && !is_url($('#txtLink').val())) {
        showError('txtLink', getMsg('E06', $('#txtLink').attr('data-name')));
        return false;
    }

    if (invalid > 0) {
        return false;
    }

    return true;
}

/**
* Validation for URL
*/
function is_url(str) {
    regexp = /^(?:(?:https?|ftp):\/\/)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/\S*)?$/;
    if (regexp.test(str)) {
        return true;
    }

    return false;
}

function flGetCompanies(BoardID) {
    var $table = $('#tblCompany');

    $.ajax({
        type: 'POST',
        url: '.../../../Pages/BoardRegistration.aspx/flGetCompanyList',
        data: '{ BoardID: ' + BoardID + '}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
    }).done(function (data, textStatus, xhr) {
        $table.dataTable().fnClearTable();
        $table.dataTable().fnDestroy();
        $table.dataTable({
            "data": data.d,
            "columnDefs": [
                { "width": "5%", "targets": [0] },
                { "className": "text-center custom-middle-align td-checkbox", "targets": [0] },
                { "targets": 0, "sortable": false, "orderable": false },
            ],
            "language": dataTableLanguageVariable(),
            "sDom": "rtipl",
            "lengthChange": false,
            "searching": false,
            "order": [[3, "desc"]],
            "processing": true,
            "responsive": true,
            "columns": [
                { data: "UserID", width: "20%" },
                { data: "UserName", width: "30%" },
                { data: "IsChecked", width: "20%" },
                {
                    data: function (data) {
                        return ParseDate(data.UpdTime)
                    }, width: "30%"
                },
            ],
            "createdRow": function (row, data, dataIndex) {
                if (data.IsChecked === '○') {
                    $(row).css('background-color', '#8abaae');
                    //$(row).css('color', '#fff');
                }
            }
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        console.log('error');
    });
}