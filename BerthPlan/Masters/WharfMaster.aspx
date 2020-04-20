<%@ Page Title="ワーフマスター" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="WharfMaster.aspx.vb" Inherits="BerthPlan.WharfMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="content">
        <div class="form-horizontal">
            <input type="text"  class="cls " id="ID" name="ID" value="0" hidden/>
            <input type="text"  class="cls " id="UpdTime" name="UpdTime" value="" hidden/>
            <div class="row">
                <div class="col-md-3">
                    <div class="input-group input-group-sm has-feedback" id="WharfCD_grp">
                        <span class="input-group-addon">ワーフコード<span class="text-danger">*</span></span>
                        <input type="text" class="form-control enter required cls upper-case special-characters single-byte" id="WharfCD" name="WharfCD" maxlength="4" data-name="ワーフコード" autofocus/>
                    </div>
                    <strong class="text-danger" id="WharfCD_msg"></strong>
                </div>
            </div>

            <div class="row">
                <div class="col-md-4">
                    <div class="input-group input-group-sm has-feedback" id="WharfName_grp">
                        <span class="input-group-addon">ワーフ名<span class="text-danger">*</span></span>
                        <input type="text" class="form-control required cls enter" id="WharfName" name="WharfName" data-name="ワーフ名" maxlength="10"/>
                    </div>
                    <strong class="text-danger" id="WharfName_msg"></strong>
                </div>

                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button" ID="btnAdd" Value="新规(F2)" class="btn btn-flat btn-primary btn-sm btn-block pull-right enter"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                     <input type="button" ID="btnUpdate" Value="更新(F3)" class="btn btn-flat btn-success btn-sm btn-block enter"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button" ID="btnClear" value="クリア(F4)"  class="btn btn-flat btn-warning btn-sm btn-block enter"/>
                </div>
            </div>
        

            <div class="row">
                <div class="col-md-8 col-sm-12 col-xs-12">
                    <div class="table-responsive" style="width:100%">
                        <table class="table table-condensed table-striped table-bordered table-link" id="tblWharf" style="width:100%">
                            <thead id="tblWharf_head"></thead>
                            <tbody id="tblWharf_body"></tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-1 col-sm-4 col-xs-4">
                     <input type="button" ID="btnDelete" class="btn btn-danger btn-block btn-flat btn-sm enter" value="削除(F8)"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button" id="btnExcel" value="印刷(F10)" class="btn btn-success btn-block btn-flat btn-sm enter"/>
                </div>
                <div class="col-md-1 col-md-offset-5 col-sm-4 col-xs-4">
                    <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-flat btn-sm enter">閉じる(F12)</a>
                </div>
            </div>
        </div>
    </section>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/masters/WharfMaster.js"></script>
</asp:Content>
