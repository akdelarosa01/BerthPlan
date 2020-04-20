$(function () {

    $('#btnSearchVessel').on('click', function (e) {
        fgDisplayVesselModalTable();
    });

    $("#MainContent_VesselCD_VesselCD").on('change', function () {
        var VesselCD = $("#MainContent_VesselCD_VesselCD").val();
        $.ajax({
            type: "POST",
            url: ".../../../Masters/VesselMaster.aspx/fgVesselName",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{VesselCD: "' + VesselCD + '" }',
        }).done(function (data, textStatus, xhr) {
            if (data.d == '' || data.d == null) {
                $("#MainContent_VesselCD_VesselCD").val('');
                $("#MainContent_VesselCD_VesselName").val('');
            } else {
                $("#MainContent_VesselCD_VesselName").val(data.d);
            }
        }).fail(function (xhr, textStatus, errorThrown) {
            console.log("error");
        });
    });

    $('#tblVesselModal_body').on('click', '.selected', function (e) {
        var tableapprover = $('#tblVesselModal').DataTable();
        var data = tableapprover.row($(this).parent()[0].parentNode).data();
        $('#MainContent_VesselCD_VesselCD').val(data.VesselCD);
        $('#MainContent_VesselCD_VesselName').val(data.VesselName);
        $('#VesselModal').modal('hide');
        hideError("MainContent_VesselCD_VesselCD");
    });
});

function fgDisplayVesselModalTable() {
    var $table = $('#tblVesselModal');

    $.ajax({
        type: 'POST',
        url: '.../../../Masters/VesselMaster.aspx/flGetVesselList',
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
                { data: "VesselCD", width: "30%" },
                { data: "VesselName", width: "65%" }
            ],
            
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });

    $('#mVesselCode').keyup(function () {
        $('#tblVesselModal').DataTable().column(1).search($('#mVesselCode').val()).draw();
    });

    $('#mVesselName').keyup(function () {
        $('#tblVesselModal').DataTable().column(2).search($('#mVesselName').val()).draw();
    });
}