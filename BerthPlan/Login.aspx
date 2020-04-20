<%@ Page Title="バースプラン" Language="VB" MasterPageFile="~/Layouts/Login.Master" AutoEventWireup="true" CodeBehind="Login.aspx.vb" Inherits="BerthPlan.Login" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <form runat="server">
        <div class="alert alert-danger alert-dismissible" id="failed_alert" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <strong>Failed!</strong> <span runat="server" id="failed_msg"></span>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="form-group has-feedback" id="txtUserID_grp" runat="server">
                    <asp:TextBox runat="server" Cssclass="form-control enter upper-case special-characters single-byte" id="txtUserID" name="txtUserID" placeholder="ユーザーID" MaxLength="4" autofocus/>
                    <span id="txtUserIDmsg" runat="server" class="help-block"></span>
		        </div>
		        <div class="form-group has-feedback" id="txtPassword_grp" runat="server">
                    <asp:TextBox runat="server" type="password" Cssclass="form-control enter" id="txtPassword" name="txtPassword" placeholder="パスワード" MaxLength="8"/>
                    <span id="txtPasswordmsg" runat="server" class="help-block"></span>
		        </div>
            </div>
        </div>
		
		<div class="row">
			<div class="col-sm-4">
                <div class="form-group">
                    <asp:Button Text="ログイン" runat="server" id="btnLogin" Cssclass="btn btn-primary btn-block btn-flat enter"/>
                </div>
			</div>

            <div class="col-sm-offset-4 col-sm-4">
                <div class="form-group">
                    <asp:Button Text="閉じる" runat="server" id="btnClose" Cssclass="btn btn-darkgrey btn-block btn-flat enter"/>
                </div>
			</div>
		</div>
	</form>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="Assets/scripts/pages/Login.js"></script>
</asp:Content>

