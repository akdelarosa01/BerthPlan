$(function () {

    $('#btnSearchBerth').on('click', function (e) {
        fgDisplayBerthModalTable();

        $('#tblBerthModal_body').on('click', '.selected', function (e) {
            var tableapprover = $('#tblBerthModal').DataTable();
            var data = tableapprover.row($(this).parent()[0].parentNode).data();
            $('#MainContent_BerthID_BerthCD').val(data.BerthCD);
            $('#MainContent_BerthID_BerthName').val(data.BerthName);
            $('#MainContent_BerthID_BerthID').val(data.BerthID);
            $('#BerthModal').modal('hide');
            hideError("MainContent_BerthID_BerthCD");
        });
    });


    $("#MainContent_BerthID_BerthCD").on('change', function (e) {
        var BerthCD = $("#MainContent_BerthID_BerthCD").val();
        $('#MainContent_BerthID_BerthID').val('');
        $.ajax({
            type: "POST",
            url: ".../../../Masters/BerthMaster.aspx/fgBerthName",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{BerthCD: "' + BerthCD + '" }',
        }).done(function (data, textStatus, xhr) {
            if (data.d == '' || data.d == null) {
                $("#MainContent_BerthID_BerthName").val('');
                $('#MainContent_BerthID_BerthID').val('');
            } else {
                $("#MainContent_BerthID_BerthName").val(data.d.BerthName);
                //$('#MainContent_BerthID_BerthID').val(data.d.BerthID);
            }
        }).fail(function (xhr, textStatus, errorThrown) {
            console.log("error");
        });
    });

});

function fgDisplayBerthModalTable() {
    var $table = $('#tblBerthModal');

    $.ajax({
        type: 'POST',
        url: '.../../../Masters/BerthMaster.aspx/flGetBerthList',
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
            "processing": true,
            "responsive": true,
            "order": [3, 'asc'],
            "columns": [
                {
                    data: function (data) {
                        return '<span class="btn btn-primary btn-sm  selected"><i class="fa fa-hand-pointer-o"></i></span>';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { data: "WharfCD" },
                { data: "WharfName" },
                { data: "BerthCD" },
                { data: "BerthName" }
            ]
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });

    $('#mWharfCode').keyup(function () {
        $('#tblBerthModal').DataTable().column(1).search($('#mWharfCode').val()).draw();
    });

    $('#mWharfName').keyup(function () {
        $('#tblBerthModal').DataTable().column(2).search($('#mWharfName').val()).draw();
    });

    $('#mBerthCode').keyup(function () {
        $('#tblBerthModal').DataTable().column(3).search($('#mBerthCode').val()).draw();
    });

    $('#mBerthName').keyup(function () {
        $('#tblBerthModal').DataTable().column(4).search($('#mBerthName').val()).draw();
    });
}

function fgBerth(BerthID) {
    $.ajax({
        type: "POST",
        url: ".../../../Masters/BerthMaster.aspx/fgBerth",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{BerthID: "' + BerthID + '" }',
    }).done(function (data, textStatus, xhr) {
        $("#MainContent_BerthID_BerthName").val(data.d.BerthName);
        $("#MainContent_BerthID_BerthCD").val(data.d.BerthCD);
        $('#MainContent_BerthID_BerthID').val(data.d.BerthID);
    }).fail(function (xhr, textStatus, errorThrown) {
        console.log("error");
    });
}