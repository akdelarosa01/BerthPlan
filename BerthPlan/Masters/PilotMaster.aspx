<%@ Page Title="水先人マスター" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="PilotMaster.aspx.vb" Inherits="BerthPlan.PilotMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="content">
        <div class="form-horizontal" >
            <div class="row">

                <div class="col-md-3 col-sm-5 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="textPilotCode_grp">
                        <span class="input-group-addon">水先人コード<span class="text-danger">*</span></span>
                        <input type="text" ID="textPilotCode" name="textPilotCode" MaxLength="4" class="form-control enter txtBox required upper-case special-characters single-byte" data-name="水先人コード"/>
                    </div>
                    <strong class="text-danger" id="textPilotCode_msg"></strong>
                </div>

                <div class="col-md-3 col-sm-7 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="textEmail_grp">
                        <span class="input-group-addon">E-メール<span class="text-danger">*</span></span>
                        <input type="email" ID="textEmail" name="email" MaxLength="50" class="form-control enter txtBox required single-byte" data-name="E-メール"/>
                    </div>
                    <strong class="text-danger" id="textEmail_msg"></strong>
                </div>

            </div>

            <div class="row">

                <div class="col-md-3 col-sm-5 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="textTel_grp">
                        <span class="input-group-addon">電話番号<span class="text-danger">*</span></span>
                        <input type="text" ID="textTel" name="telNumber" MaxLength="16" class="form-control enter txtBox required" data-name="電話番号"/>
                    </div>
                    <strong class="text-danger" id="textTel_msg"></strong>
                </div>

                <div class="col-md-3 col-sm-7 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="textPilotName_grp">
                        <span class="input-group-addon">水先人名<span class="text-danger">*</span></span>
                        <input type="text" ID="textPilotName" name="textPilotName" MaxLength="10" class="form-control enter txtBox required" data-name="水先人名"/>
                    </div>
                    <strong class="text-danger" id="textPilotName_msg"></strong>
                </div>

            </div>

            <div class="row">
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <button type="button" ID="btnNew" class="btn btn-flat btn-primary btn-sm btn-block pull-right enter">新规(F2)</button>
                </div>

                <div class="col-md-1 col-sm-4 col-xs-4">
                    <button type="button" ID="btnUpdate" class="btn btn-flat btn-success btn-sm btn-block enter">更新(F3)</button>
                </div>

                <div class="col-md-1 col-sm-4 col-xs-4">
                   <button type="button" ID="btnClear" class="btn btn-flat btn-warning btn-sm btn-block enter">クリア(F4)</button>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8 col-sm-12 col-xs-12">
                    <div class="table-responsive" style="width:100%">
                        <table id="tblPilot" class="table table-condensed table-striped table-bordered table-link" style="width:100%" >
                            <thead id="tblPilot_head"></thead>
                            <tbody id="tblPilot_body"></tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <button type="button" ID="btnDelete" class="btn btn-danger btn-block btn-sm btn-flat enter">削除(F8)</button>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <button type="button" id="btnExcel" class="btn btn-success btn-block btn-flat btn-sm enter"/>印刷(F10)
                </div>
                <div class="col-md-1 col-md-offset-5 col-sm-4 col-xs-4">
                    <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-sm btn-flat enter">閉じる(F12)</a>
                </div>
            </div>

        </div>

    </section>

</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/masters/PilotMaster.js"></script>
</asp:Content>



