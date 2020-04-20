$(function () {

    $('#btnSearchPilot').on('click', function (e) {
        fgDisplayPilotModalTable();
    });

    $("#MainContent_PilotCD_PilotCD").on('change', function () {
        var PilotCD = $("#MainContent_PilotCD_PilotCD").val();
        flPilotName(PilotCD);
    });

    $('#tblPilotModal_body').on('click', '.selected', function (e) {
        var tableapprover = $('#tblPilotModal').DataTable();
        var data = tableapprover.row($(this).parent()[0].parentNode).data();
        $('#MainContent_PilotCD_PilotCD').val(data.PilotCD);
        $('#MainContent_PilotCD_PilotName').val(data.PilotName);
        $('#PilotModal').modal('hide');
        hideError("MainContent_PilotCD_PilotCD");
    });

});

function fgDisplayPilotModalTable() {
    var $table = $('#tblPilotModal');

    $.ajax({
        type: 'POST',
        url: '.../../../Masters/PilotMaster.aspx/flGetPilotList',
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
            "order": [[1, "asc"]],
            "columns": [
                {
                    data: function (data) {
                        return '<span class="btn btn-primary btn-sm  selected"><i class="fa fa-hand-pointer-o"></i></span>';
                    }, sortable: false, orderable: false, width: "5%"
                },
                { data: "PilotCD", width: "30%" },
                { data: "PilotName", width: "65%" }
            ]
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });
    //var columns = [
                
    //];

    //initDataTable("#tblPilotModal", ".../../../Masters/PilotMaster.aspx/flGetPilotList", columns, [2, "asc"]);

    $('#mPilotCode').keyup(function () {
        $('#tblPilotModal').DataTable().column(1).search($('#mPilotCode').val()).draw();
    });

    $('#mPilotName').keyup(function () {
        $('#tblPilotModal').DataTable().column(2).search($('#mPilotName').val()).draw();
    });
}


function flPilotName(PilotCD) {
    $.ajax({
        type: "POST",
        url: ".../../../Masters/PilotMaster.aspx/flPilotName",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{PilotCD: "' + PilotCD + '" }',
    }).done(function (data, textStatus, xhr) {
        if (data.d == '' || data.d == null) {
            $("#MainContent_PilotCD_PilotCD").val('');
            $("#MainContent_PilotCD_PilotName").val('');
        } else {
            $("#MainContent_PilotCD_PilotName").val(data.d);
        }
    }).fail(function (xhr, textStatus, errorThrown) {
        console.log("error");
    });
}