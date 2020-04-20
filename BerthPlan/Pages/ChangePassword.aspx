<%@ Page Title="パスワード変更" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Login.Master" CodeBehind="ChangePassword.aspx.vb" Inherits="BerthPlan.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form action="#" method="post" id="frmChangePassword">

        <div id="AlertMessege"></div>

		<div class="form-group has-feedback">
            <div class="has-feedback" id="CurrentPassword_grp">
                <input type="password" class="form-control required enter" id="CurrentPassword" name="CurrentPassword" placeholder="以前のパスワード" data-name="以前のパスワード" maxlength="8">
            </div>
            <strong class="text-danger" id="CurrentPassword_msg"></strong>
			<span class="glyphicon glyphicon-lock form-control-feedback"></span>
		</div>
        <div class="form-group has-feedback">
            <div class="has-feedback" id="Password_grp">
                <input type="password" class="form-control required enter" id="Password" name="Password" placeholder="新しいパスワード" data-name="新しいパスワード" maxlength="8">
            </div>
            <strong class="text-danger" id="Password_msg"></strong>
			<span class="glyphicon glyphicon-lock form-control-feedback"></span>
		</div>
		<div class="form-group has-feedback">
                        <div class="has-feedback" id="ConfirmPassword_grp">
                <input type="password" class="form-control required enter" id="ConfirmPassword" name="ConfirmPassword" placeholder="新しいパスワードを確認" data-name="新しいパスワードを確認" maxlength="8">
            </div>
            <strong class="text-danger" id="ConfirmPassword_msg"></strong>
			<span class="glyphicon glyphicon-lock form-control-feedback"></span>
		</div>
		<div class="row">
			<div class="col-sm-4">
                <div class="form-group">
                    <button type="submit" class="btn btn-primary btn-block btn-flat" id="btnSubmit">変更する(F2)</button>
                </div>
			</div>

            <div class="col-sm-offset-4 col-sm-4">
                <div class="form-group">
                    <a runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-flat">閉じる(F12)</a>
                </div>
			</div>
		</div>
	</form>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/pages/ChangePassword.js"></script>
</asp:Content>