$(function () {
    flGetBoardList();

    $('.table-link > tbody > tr').on('click', '.title',function () {
        $('#annContent').val($(this).html());
        $('.btnFile > .btn-app-text').html('Template' + $(this).attr('id') + '.xlsx');
    });

    $('.btnFile').on('click', function () {
        msg("File Downloaded","success");
    });

    $('#tblBoardMenu_body').on('click', 'tr', function (e) {

        $('tr.datatableRowActive').removeClass('datatableRowActive');
        $('#tblBoardMenu').DataTable().$(this).toggleClass('datatableRowActive');

        switch (e.target.type) {
            case "checkbox":
                var $checkbox = $(this).find(':checkbox');
                var $table = $('#tblBoardMenu').DataTable();
                var datarow = $table.row(this).data();
                $checkbox.attr('disabled', true);
                $.ajax({
                    type: "POST",
                    url: "SystemMenu.aspx/flSeenUserViewBoard",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{BoardID: "' + datarow.BoardID + '" }',
                }).done(function (data, textStatus, xhr) {
                    sessionOut(data.d.Status);
                    flFileDisplay(datarow.BoardID);
                }).fail(function (xhr, textStatus, errorThrown) {
                    console.log("error");
                });
                e.stopPropagation();
                break;
            default:
                var $table = $('#tblBoardMenu').DataTable();
                var datarow = $table.row($(this)).data();
                $('#annContent').val('<p style="white-space: pre-line">' + datarow.Contents + '</p>');
                $('#annContent').val(datarow.Contents);

                if (datarow.HyperLink !== "") {
                    var aLink = '<strong>リンク: </strong><a href="' + datarow.HyperLink + '" target="_blank"' +
                                'class="text-info" >' + datarow.HyperLink +
                            '</a>';
                    $('#aLink').html(aLink);
                }else{
                    $('#aLink').html("");
                }

                flFileDisplay(datarow.BoardID);
                break;
        }
    });


    //Shorcut Keys
    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
            //F1:
            case 112:
                e.preventDefault();
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
            //F10: 
            case 121:
                e.preventDefault();
            //F12: 
            case 123:
                e.preventDefault();
                break;
            default:

        }
    });


});

function flGetBoardList() {
    $.ajax({
        type: "POST",
        url: "SystemMenu.aspx/flGetBoardList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    }).done(function (data, textStatus, xhr) {
        $("#tblBoardMenu").dataTable().fnClearTable();
        $("#tblBoardMenu").dataTable().fnDestroy();
        $("#tblBoardMenu").dataTable({
            "data":data.d,
            "columnDefs": [
                { "className": "text-center custom-middle-align td-check", "targets": [0] },
                { "targets": 0, "sortable": false, "orderable": false }
            ],
            "lengthChange": false,
            "pageLength": 5,
            "ordering": false,
            "searching": false,
            "sDom": "rtipl",
            "language": {
                "decimal": "",
                "emptyTable": "テーブル内のデータなし",
                "info": '<strong class="text-info">確認したらチェックマークをお願いします。</strong><br/>_TOTAL_エントリのうち_START_から_END_を表示している',
                "infoEmpty": "確認したらチェックマークをお願いします。<br/>0エントリの0から0を表示",
                "infoFiltered": "（_MAX_合計エントリからフィルタリング）",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "_MENU_エントリを表示",
                "loadingRecords": "データを読み込んでいる...",
                "processing": "処理データ...",
                "search": "検索:",
                "zeroRecords": "該当する記録が見つからない。",
                "paginate": {
                    "first": "最初",
                    "last": "最終",
                    "next": "次",
                    "previous": "前"
                },
                "aria": {
                    "sortAscending": ": activate to sort column ascending",
                    "sortDescending": ": activate to sort column descending"
                },
            },
            "processing": true,
            "columns": [
                {
                    data: function (data) {
                        var checked = (data.Seen == 1) ? ' checked disabled="disabled"' : '';
                        return '<input type="checkbox" class="Checkbox" ' + checked + ' />';
                    }, sortable: false, orderable: false, width: "10%"
                },
                {
                    data: function (data) {
                        return data.Title.replace(/(.{30})..+/, "$1&hellip;");
                    }, width: "50%"
                },
                { data: "CreateUserID", width: "20%" },
                    {
                        data: function (data) {
                            return ParseDate(data.PostingStartDate);
                        }, width: "20%"
                    },

            ],
            "drawCallback": function (settings) {
                $('tr.datatableRowActive').removeClass('datatableRowActive');
                flFileDisplay(0);
                $('#annContent').val("");
                $('#aLink').html("");
            }
        });
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus)
    });
}

function flFileDisplay(BoardID) {
    var FileHTML = "";
    var count = 1;
    $.ajax({
        type: "POST",
        url: "SystemMenu.aspx/flFileDisplay",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{BoardID: "' + BoardID + '" }',
    }).done(function (data, textStatus, xhr) {
        console.log(data);
        sessionOut(data.d.Status);
        $.each(data.d.Data, function (i, v) {
            var file = fileExtension(v.FileType);

            //window.location.href = '../../../Handlers/FileDownloader.ashx?FilePath=' + data.d.Data[0] + '&FileName=' + data.d.Data[1];

            FileHTML += '<a class="btn btn-default btn-sq btn-block tip col-md-2 col-sm-12 col-xs-12" style="margin-right: 5px;"' +
                            'href=".../../../Handlers/BoardFileHandler.ashx?FileId=' + v.BoardFileID + '">' +
                            '<i class="' + file.icon + ' fa-3x"></i><br/>' +
                            '<span class="btn-app-text">' + v.FileName + '</span><br/>' +
                            '<small>' + numberFormat(v.FileSize) + 'KB</small>' +
                        '</a>';
        });
        $('#FileDisplay').html(FileHTML);
    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown,textStatus)
    });
}
