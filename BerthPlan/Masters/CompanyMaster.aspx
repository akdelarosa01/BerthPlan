<%@ Page Title="会社マスター" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="CompanyMaster.aspx.vb" Inherits="BerthPlan.CompanyMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="content">
        <div class="form-horizontal">
            <div class="row">
                <input type="text"  class="cls " id="ID" name="ID" value="0" hidden/>
                <input type="text"  class="cls " id="UpdTime" name="UpdTime" value="" hidden/>

                <div class="col-md-2 col-sm-5 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="ApplicantCD_grp">
                        <span class="input-group-addon">申請者コード<span class="text-danger">*</span></span>
                        <input type="text" class="form-control enter cls required upper-case special-characters single-byte" id="ApplicantCD" name="ApplicantCD" maxlength="4" data-name="申請者コード" autofocus/>
                    </div>
                    <strong class="text-danger" id="ApplicantCD_msg"></strong>
                </div>

                <div class="col-md-3 col-sm-7 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="ApplicantName_grp">
                        <span class="input-group-addon">申請者名<span class="text-danger">*</span></span>
                         <input type="text" class="form-control enter cls required" id="ApplicantName" name="ApplicantName" data-name="申請者名" maxlength="20" />
                    </div>
                    <strong class="text-danger" id="ApplicantName_msg"></strong>
                </div>

                <div class="col-md-5 col-sm-8 col-xs-6">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">住所</span>
                        <input type="text" class="form-control enter cls" id="Address" name="Address" maxlength="30" />
                    </div>
                </div>

                <div class="col-md-2 col-sm-4 col-xs-6">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">郵便番号</span>
                        <input type="text" class="form-control enter cls" id="PostCode" name="PostCode" maxlength="10"/>
                    </div>
                </div>

            </div>

            <div class="row">
                

                <div class="col-md-2 col-sm-6 col-xs-6">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">電話番号</span>
                        <input type="text" class="form-control enter cls" id="Tel" name="Tel" maxlength="16"/>
                    </div>
                </div>

                <div class="col-md-3 col-sm-6 col-xs-6">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">ファクス番号</span>
                        <input type="text" class="form-control enter cls" id="Fax" name="Fax" maxlength="18" />
                    </div>
                </div>

                <div class="col-md-5 col-sm-7 col-xs-7">
                    <div class="input-group input-group-sm has-feedback" id="Email_grp">
                        <span class="input-group-addon">E-メール</span>
                        <input type="text" ID="Email" MaxLength="50" name="Email" data-name="E-メール<" class="form-control enter cls single-byte" >
                    </div>
                    <strong class="text-danger" id="Email_msg"></strong>
                </div>

                <div class="col-md-2 col-sm-5 col-xs-5">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">色</span>
                        <input type="text" class="form-control colorpicker enter cls" id="Color" name="Color" maxlength="50" readonly style="background-color:white!important;"/>
                    </div>
                </div>

            </div>

            <div class="row">
                <div class="col-md-offset-4 col-md-1 col-sm-4 col-xs-4">
                    <input type="button" ID="btnAdd" Value="新规(F2)" class="btn btn-flat btn-primary enter btn-sm btn-block pull-right"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                     <input type="button" ID="btnUpdate" Value="更新(F3)" class="btn btn-flat btn-success enter btn-sm btn-block"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                     <input type="button"  ID="btnClear" Value="クリア(F4)"  class="btn btn-flat btn-warning enter btn-sm btn-block"/>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="table-responsive" style="width:100%">
                        <table class="table table-condensed table-striped table-bordered datatable table-link" id="tblCompany" style="width:100%">
                            <thead id="tblCompany_head"></thead>
                            <tbody id="tblCompany_body"></tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button"  ID="btnDelete" class="btn btn-danger btn-block btn-flat btn-sm enter" value="削除(F8)"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button" id="btnExcel" value="印刷(F10)" class="btn btn-success btn-block btn-flat btn-sm enter"/>
                </div>
                <div class="col-md-1 col-md-offset-9 col-sm-4 col-xs-4">
                    <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-sm btn-block btn-flat enter">閉じる(F12)</a>
                </div>
            </div>

        </div>
    </section>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
 <script src="../Assets/scripts/masters/CompanyMaster.js"></script>
</asp:Content>
