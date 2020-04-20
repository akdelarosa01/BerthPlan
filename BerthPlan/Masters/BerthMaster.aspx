<%@ Page Title="バースマスター" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="BerthMaster.aspx.vb" Inherits="BerthPlan.BerthMaster" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="content">
        <div id="Form1" class="form-horizontal" runat="server">
            <div class="row">
                <muc:WharfControl runat="server" id="WharfCD" name="WharfCD" class="enter"/>
            </div>

            <div class="row">
                <div class="col-md-3">
                    <div class="input-group input-group-sm has-feedback" id="textBerthCD_grp">
                        <span class="input-group-addon">バースコード<span class="text-danger">*</span></span>
                        <input type="text" ID="textBerthCD" MaxLength="4" class="form-control enter txtBox required upper-case special-characters single-byte" data-name="バースコード">
                    </div>
                    <strong class="text-danger" id="textBerthCD_msg"></strong>
                </div>

                <div class="col-md-4">
                    <div class="input-group input-group-sm has-feedback" id="textBerthName_grp">
                        <span class="input-group-addon">バース名<span class="text-danger">*</span></span>
                        <input type="text" ID="textBerthName" name="textBerthName" MaxLength="10" class="form-control enter txtBox required" data-name="バース名">
                    </div>
                    <strong class="text-danger" id="textBerthName_msg"></strong>
                </div>

                <div class="col-md-1 col-sm-4 col-xs-4">
                    <button type="button" ID="btnNew" class="btn btn-flat btn-primary enter btn-sm btn-block pull-right">新规(F2)</button>
                </div>

                 <div class="col-md-1 col-sm-4 col-xs-4">
                    <button type="button" ID="btnUpdate" class="btn btn-flat btn-success enter btn-sm btn-block">更新(F3)</button>
                </div>

                <div class="col-md-1 col-sm-4 col-xs-4">
                   <button type="button" ID="btnClear" class="btn btn-flat btn-warning enter btn-sm btn-block">クリア(F4)</button>
                </div>
            </div>

        <div class="row">
            <div class="col-md-10 col-sm-12 col-xs-12">
                <div class="table-responsive" style="width:100%">
                    <table id="tblBerth" class="table table-condensed table-striped table-bordered table-link" style="width:100%">
                        <thead id="tblBerth_head"></thead>
                        <tbody id="tblBerth_body"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-1 col-sm-4 col-xs-4">
                <button type="button" ID="btnDelete" class="btn btn-danger enter btn-block btn-sm btn-flat"/>削除(F8)
            </div>
            <div class="col-md-1 col-sm-4 col-xs-4">
                <button type="button" id="btnExcel" class="btn btn-success btn-block btn-flat btn-sm enter"/>印刷(F10)
            </div>
            <div class="col-md-1 col-md-offset-7 col-sm-4 col-xs-4">
                <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-sm enter btn-block btn-flat">閉じる(F12)</a>
            </div>
        </div>

        </div>

    </section>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/masters/BerthMaster.js"></script>
    <script src="../Assets/scripts/modal/WharfModal.js"></script>
</asp:Content>



