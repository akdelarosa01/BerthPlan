
$(function () {

    $('#CurrentPassword').focus();
    

    $('#frmChangePassword').on('submit', function (e) {
        e.preventDefault();
        if (FormValidate() == true) {
            $.ajax({
                url: ".../../../Pages/ChangePassword.aspx/ChangePassword",
                type: 'POST',
                data: JSON.stringify({ CurrentPassword: $('#CurrentPassword').val(), Password: $('#Password').val() }),
                beforeSend: ShowLoading(),
                contentType: 'application/json; charset=utf-8',
                datatype: "json",
            }).done(function (data, textStatus, xhr) {
                HideLoading();
                sessionOut(data.d.Status);
                if (data.d.Status != "danger") {
                    AlertMessege(data.d.Msg, data.d.Status);
                    bootbox.dialog({
                        title: '<div style="color: #1BA39C"><strong><i class="fa fa-check"></i></strong> ' + jsUcfirst(data.d.Status) + '</div>',
                        message: "アカウントに再度ログインします",
                        size: 'small',
                        buttons: {
                            ok: {
                                label: "OK!",
                                className: 'btn-sm btn-success',
                                callback: function () {
                                    sessionOut("expire");
                                }
                            }
                        }
                    });
                } else { AlertMessege(data.d.Msg, data.d.Status); }
            }).fail(function (xhr, textStatus, errorThrown) {
                msg(data.d.Msg, "error");
            });
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
                if (!$('#btnSubmit').is(':disabled')) {
                    $('#frmChangePassword').submit();
                }
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
                //F10: PRINT
            case 121:
                e.preventDefault();
                //F12: CLOSE
            case 123:
                e.preventDefault();
                window.location.href = ".../../../Pages/SystemMenu"
                break;
            default:

        }
    });

});

function FormValidate() {
    var invalid = 0;
    $('.required').each(function () {
        var id = $(this).attr('id');
        if ($(this).val() == "") {
            showError(id, getMsg('E06', $(this).attr('data-name')));
            invalid++;
        } else {
            $('#' + id).removeAttr("style");
        }
    });
    if (invalid != 0) { return false } 

    if ($('#Password').val().length < 5 && $('#Password').val().length != 0) {
        AlertMessege(getMsg('E02', 'パスワード'), "danger");
        return false;
    }

    if ($('#Password').val() != $('#ConfirmPassword').val()) {
        AlertMessege(getMsg('E05', 'パスワード'), "danger");
        invalid++;
    }

    if ($('#Password').val() == $('#CurrentPassword').val()) {
        AlertMessege("古いパスワードと新しいパスワードは同じです", "danger");
        return false;
    }
    return true 
}

function AlertMessege(msg, status) {
    var s = (status == "danger") ? "Failed" : "Successfull"
    $("#AlertMessege").html(' <div class="alert alert-' + status + ' alert-dismissible" id="failed_alert"><span class="close" data-dismiss="alert" aria-label="close">&times;</span><strong>' + s + '!</strong> <span id="failed_msg">' + msg + '</span></div>');
}