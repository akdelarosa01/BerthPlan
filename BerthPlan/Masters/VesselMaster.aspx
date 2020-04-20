<%@ Page Title="本船マスター" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="VesselMaster.aspx.vb" Inherits="BerthPlan.VesselMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="content">
        <div class="form-horizontal">
            <input type="text"  class="cls " id="ID" name="ID" value="0" hidden/>
            <input type="text"  class="cls " id="UpdTime" name="UpdTime" value="" hidden/>
            <div class="row">

                <div class="col-md-2 col-xs-6">
                    <div class="input-group input-group-sm has-feedback" id="VesselCD_grp">
                        <span class="input-group-addon">船コード<span class="text-danger">*</span></span>
                        <input type="text" class="form-control enter required cls upper-case special-characters single-byte" id="VesselCD" name="VesselCD" maxlength="4" data-name="船コード" autofocus/>
                    </div>
                    <strong class="text-danger" id="VesselCD_msg"></strong>
                </div>

                <div class="col-md-2 col-xs-6">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">IMO</span>
                        <input type="text" class="form-control enter cls" id="IMO" name="IMO" maxlength="10"/>
                    </div>
                </div>

                <div class="col-md-4 col-xs-7">
                    <div class="input-group input-group-sm has-feedback" id="VesselName_grp">
                        <span class="input-group-addon">船名<span class="text-danger">*</span></span>
                        <input type="text" class="form-control enter required cls" id="VesselName" name="VesselName" maxlength="10" data-name="船名"/>
                    </div>
                    <strong class="text-danger" id="VesselName_msg"></strong>
                </div>

                <div class="col-md-4 col-xs-5">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">総トン数</span>
                         <input type="number" class="form-control enter cls" id="GrossTon" name="GrossTon" onKeyPress="if(this.value.length==15) return false;"/>
                    </div>
                </div>

            </div>

            <div class="row">

                <div class="col-md-2 col-xs-4">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">LOA</span>
                         <input type="number" class="form-control enter cls" id="LOA" name="LOA" onKeyPress="if(this.value.length==15) return false;" />
                    </div>
                </div>

                <div class="col-md-4 col-xs-8">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">船籍</span>
                        <input type="text" class="form-control enter cls" id="Nationality" name="Nationality" maxlength="20"/>
                    </div>
                </div>

                <muc:CompanyControl runat="server" id="ApplicantCD" name="ApplicantCD" />

                <div class="col-md-2 col-xs-6">
                    <div class="input-group input-group-sm">
                        <span class="input-group-addon">外/内</span>
                        <select class="form-control enter cls" id="IO" name="IO">
                            <option></option>
                            <option value=true>外</option>
                            <option value=false>内</option>
                        </select>
                    </div>
                </div>
                
            </div>

            <div class="row text-center">
                <div class="col-md-offset-4 col-md-1 col-xs-4">
                    <input type="button" ID="btnAdd" Value="新规(F2)" class="btn btn-flat btn-primary btn-sm btn-block pull-right enter"/>
                </div>
                <div class="col-md-1 col-xs-4">
                     <input type="button" ID="btnUpdate" Value="更新(F3)" class="btn btn-flat btn-success btn-sm btn-block enter"/>
                </div>
                <div class="col-md-1 col-xs-4">
                     <input type="button"  ID="btnClear" Value="クリア(F4)"  class="btn btn-flat btn-warning btn-sm btn-block enter"/>
                </div>
            </div>
        

            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="table-responsive" style="width:100%">
                        <table class="table table-condensed table-striped table-bordered table-link" id="tblVessel" style="width:100%">
                            <thead id="tblVessel_head">
                            </thead>
                            <tbody id="tblVessel_body">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-1 col-xs-4">
                    <input type="button" ID="btnDelete" class="btn btn-danger btn-block btn-flat btn-sm enter" value="削除(F8)"/>
                </div>
                <div class="col-md-1 col-xs-4">
                    <input type="button" id="btnExcel" value="印刷" class="btn btn-success btn-block btn-flat btn-sm enter"/>
                </div>
                <div class="col-md-1 col-md-offset-9 col-xs-4">
                    <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-flat btn-sm enter">閉じる(F12)</a>
                </div>
            </div>
        </div>
    </section>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    
    <script src="../Assets/scripts/modal/CompanyModal.js"></script>
    <script src="../Assets/scripts/masters/VesselMaster.js"></script>
</asp:Content>
