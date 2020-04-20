$(function () {

    $('#btnSearchWharf').on('click', function (e) {
        fgDisplayWharfModalTable();
    });

    $("#MainContent_WharfCD_WharfCD").on('change', function () {
        var WharfCD = $("#MainContent_WharfCD_WharfCD").val();
        $.ajax({
            type: "POST",
            url: ".../../../Masters/WharfMaster.aspx/fgWharfName",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{WharfCD: "' + WharfCD + '" }',
        }).done(function (data, textStatus, xhr) {
            if (data.d == '' || data.d == null) {
                $("#MainContent_WharfCD_WharfCD").val('');
                $("#MainContent_WharfCD_WharfName").val('');
            } else {
                $("#MainContent_WharfCD_WharfName").val(data.d);
            }
        }).fail(function (xhr, textStatus, errorThrown) {
            console.log("error");
        });
    });

    $('#tblWharfModal_body').on('click', '.selected', function (e) {
        var tableapprover = $('#tblWharfModal').DataTable();
        var data = tableapprover.row($(this).parent()[0].parentNode).data();
        $('#MainContent_WharfCD_WharfCD').val(data.WharfCD);
        $('#MainContent_WharfCD_WharfName').val(data.WharfName);
        $('#WharfModal').modal('hide');
        hideError("MainContent_WharfCD_WharfCD");
    });

});

function fgDisplayWharfModalTable() {
    var $table = $('#tblWharfModal');

    $.ajax({
        type: 'POST',
        url: ".../../../Masters/WharfMaster.aspx/flGetWharfList",
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
            "order": [[1, "desc"]],
            "processing": true,
            "responsive": true,
            "columns": [
                {
                    data: function (data) {
                        return '<span class="btn btn-primary btn-sm  selected"><i class="fa fa-hand-pointer-o"></i></span>';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { data: "WharfCD", width: "30%" },
                { data: "WharfName", width: "65%" }
            ]
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
    //var columns = [
                
    //];

    //initDataTable("#tblWharfModal", ".../../../Masters/WharfMaster.aspx/flGetWharfList", columns, [2, "asc"]);

    $('#mWharfCode').keyup(function () {
        $('#tblWharfModal').DataTable().column(1).search($('#mWharfCode').val()).draw();
    });

    $('#mPilotName').keyup(function () {
        $('#mWharfName').DataTable().column(2).search($('#mWharfName').val()).draw();
    });
}