$(function () {
    GetSessionExpire();
    $('#MainContent_txtUserID').on('click', function () {
        $('#failed_alert').hide();
        if ($('#MainContent_txtUserID_grp').hasClass('has-error') || $('#MainContent_txtUserID_grp').hasClass('has-warning')) {
            $('#MainContent_txtUserID_grp').removeClass('has-error');
            $('#MainContent_txtUserID_grp').removeClass('has-warning');
            $('#MainContent_txtUserIDmsg').html('');
        }
    });

    $('#MainContent_txtPassword').on('click', function () {
        $('#failed_alert').hide();
        if ($('#MainContent_txtPassword_grp').hasClass('has-error') || $('#MainContent_txtUserID_grp').hasClass('has-warning')) {
            $('#MainContent_txtPassword_grp').removeClass('has-error');
            $('#MainContent_txtPassword_grp').removeClass('has-warning');
            $('#MainContent_txtPasswordmsg').html('');
        }
    });

    //Shorcut Keys
    $(document).on('keydown', function (e) {
        switch (e.keyCode) {
                //F1:
            case 112:
                e.preventDefault();
                break;
                //F3: 
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
                break;
                //F12:
            case 123:
                e.preventDefault();
                break;
            default:
        }
    });

});

function GetSessionExpire() {
    var qs = location.search.substring(1);
    strx = qs.split('&');
    if (qs != '') {
        $('#failed_alert').show();
        $("#failed_alert").html(' <div class="alert alert-danger alert-dismissible" id="failed_alert"><span class="close" data-dismiss="alert" aria-label="close">&times;</span><strong>Failed!</strong> <span id="failed_msg">セッションの有効期限が切れ。アカウントに再ログインします</span></div>');

    }
}