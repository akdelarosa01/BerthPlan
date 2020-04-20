$(function () {

    $('#btnSearchCompany').on('click', function (e) {
        fgDisplayCompanyModalTable();
    });
   
    $("#MainContent_ApplicantCD_ApplicantCD").on('change', function () {
        var ApplicantCD = $("#MainContent_ApplicantCD_ApplicantCD").val();
        fgApplicantName(ApplicantCD);
    });

    $('#tblCompanyModal_body').on('click', '.selected', function (e) {
        var tableapprover = $('#tblCompanyModal').DataTable();
        var data = tableapprover.row($(this).parent()[0].parentNode).data();
        $('#MainContent_ApplicantCD_ApplicantCD').val(data.ApplicantCD);
        $('#MainContent_ApplicantCD_ApplicantName').val(data.ApplicantName);
        $('#CompanyModal').modal('hide');
        hideError("MainContent_ApplicantCD_ApplicantCD");
    });

});

function fgDisplayCompanyModalTable() {
    var $table = $('#tblCompanyModal');

    $.ajax({
        type: 'POST',
        url: '.../../../Masters/CompanyMaster.aspx/flGetCompanyList',
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
                { data: "ApplicantCD", width: "30%" },
                { data: "ApplicantName", width: "65%" }
            ]
        });

    }).fail(function (xhr, textStatus, errorThrown) {
        msg(errorThrown, textStatus);
    });

    //var columns = [
                
    //];

    //initDataTable("#tblCompanyModal", ".../../../Masters/CompanyMaster.aspx/flGetCompanyList", columns, [1, 'asc']);

    $('#CompCode').keyup(function () {
        $('#tblCompanyModal').DataTable().column(1).search($('#CompCode').val()).draw();
    });

    $('#CompName').keyup(function () {
        $('#tblCompanyModal').DataTable().column(2).search($('#CompName').val()).draw();
    });
}


function fgApplicantName(ApplicantCD) {
    $.ajax({
        type: "POST",
        url: ".../../../Masters/CompanyMaster.aspx/fgApplicantName",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{ApplicantCD: "' + ApplicantCD + '" }',
    }).done(function (data, textStatus, xhr) {
        if (data.d == '' || data.d == null) {
            $("#MainContent_ApplicantCD_ApplicantCD").val('');
            $("#MainContent_ApplicantCD_ApplicantName").val('');
        } else {
            $("#MainContent_ApplicantCD_ApplicantName").val(data.d);
        }
    }).fail(function (xhr, textStatus, errorThrown) {
        console.log("error");
    });
}