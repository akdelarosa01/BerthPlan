<%@ Page Title="ユーザーマスター" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="UserMaster.aspx.vb" Inherits="BerthPlan.UserMaster" %>

<asp:Content ID="UserMasterContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="content">
        <div id="Form1" class="form-horizontal" runat="server">
            <div class="row">

                <div class="col-md-2 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="textUserID_grp">
                        <span class="input-group-addon">ユーザーID<span class="text-danger">*</span></span>
                        <input type="text" ID="textUserID" name="textUserID" MaxLength="4" class="form-control enter txtBox required upper-case special-characters single-byte" data-name="ユーザーID"/>
                    </div>
                    <strong class="text-danger" id="textUserID_msg"></strong>
                </div>

                <div class="col-md-3 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="textPassword_grp">
                        <span class="input-group-addon">パスワード<span class="text-danger">*</span></span>
                        <input type="password" ID="textPassword" name="textPassword" MaxLength="8" class="form-control enter txtBox required" data-name="パスワード"/>
                    </div>
                    <strong class="text-danger" id="textPassword_msg"></strong>
                </div>

                <div class="col-md-3">
                    <div class="input-group input-group-sm has-feedback" id="textEmail_grp">
                        <span class="input-group-addon">E-メール<span class="text-danger">*</span></span>
                        <input type="email" ID="textEmail" MaxLength="50" name="email" class="form-control enter txtBox required single-byte" data-name="E-メール"/>
                    </div>
                    <strong class="text-danger" id="textEmail_msg"></strong>
                </div>

                <muc:CompanyControl runat="server" id="ApplicantCD" name="ApplicantCD" />   

            </div>

            <div class="row">
                <div class="col-md-3 col-xs-7">
                    <div class="input-group input-group-sm has-feedback" id="textUserName_grp">
                        <span class="input-group-addon">ユーザー名<span class="text-danger">*</span></span>
                        <input type="text" ID="textUserName" MaxLength="25" class="form-control enter txtBox required" data-name="ユーザー名"/>
                    </div>
                    <strong class="text-danger" id="textUserName_msg"></strong>
                </div>

                <div class="col-md-2 col-xs-5">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">
                            <input type="checkbox" ID="checkAuthorize" class="enter">
                        </span>
                        <input type="text" class="form-control" value="管理権限" readonly="" >
                    </div>
                </div>

                <div class="col-md-1 col-xs-4">
                    <input type="button" ID="btnNew" class="btn btn-flat btn-primary enter btn-sm btn-block pull-right enter" value="新规(F2)" />
                </div>

                <div class="col-md-1 col-xs-4">
                    <button type="button" ID="btnUpdate" class="btn btn-flat btn-success enter btn-sm btn-block enter">更新(F3)</button>
                </div>

                <div class="col-md-1 col-xs-4">
                   <button type="button" ID="btnClear" class="btn btn-flat btn-warning enter btn-sm btn-block">クリア(F4)</button>
                </div>
            </div>

        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="table-responsive" style="width:100%">

                    <table id="tblUser" class="table table-condensed table-striped table-bordered table-link" style="width:100%">
                        <thead id="tblUser_head"></thead>
                        <tbody id="tblUser_body"></tbody>
                    </table>
                </div>
                
                
            </div>
        </div>

        <div id="Div1" class="row" runat="server">
            <div class="col-md-1 col-xs-4">
                <button type="button" ID="btnDelete" class="btn btn-danger btn-block btn-sm btn-flat enter">削除(F8)</button>
            </div>
            <div class="col-md-1 col-xs-4">
                <button type="button" id="btnExcel" class="btn btn-success btn-block btn-flat btn-sm enter"/>印刷(F10)
            </div>
            <div class="col-md-1 col-md-offset-9 col-xs-4">
                <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-sm btn-flat enter">閉じる(F12)</a>
            </div>
        </div>

       </div>
       
    </section>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/masters/UserMaster.js"></script>
    <script src="../Assets/scripts/modal/CompanyModal.js"></script>
</asp:Content>
