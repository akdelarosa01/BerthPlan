<%@ Page Title="本船スケジュール登録" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="VesselScheduleRegistration.aspx.vb" Inherits="BerthPlan.VesselScheduleRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="content">
        <div id="Div1" class="form-horizontal" runat="server">
            
            <div class="row">

                <div class="col-md-3 col-sm-3">
                    <div class="input-group input-group-sm has-feedback" id="Voyage_grp">
                        <span class="input-group-addon">Voyage No.<span class="text-danger">*</span></span>
                        <input type="text" class="form-control cls enter required upper-case special-characters single-byte" id="Voyage" name="Voyage" data-name="Voyage No." maxlength="10" autofocus>
                    </div>
                    <strong class="text-danger" id="Voyage_msg"></strong>
                </div>

                <muc:VesselControl runat="server" ID="VesselCD" />
                


                <div class="col-md-1 col-sm-3 col-xs-4">
                    <input type="button" value="検索(F1)" id="btnSearch" class="btn btn-primary btn-sm btn-block btn-flat enter"/>
                </div>

            </div>

            <div class="row">
                <div class="col-md-5">
                    <div class="panel">
                        <div class="panel-body">
                            <div class="col-md-12">
                                <span style="margin-left:5px;">総 ト ン 数 : <span id="GrossTon" class="cls"></span> ト ン</span><span> LOA: <span id="LOA" class="cls"></span> 外 航 船</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">

                <muc:BerthControl runat="server" ID="BerthID"/>
                <muc:CompanyControl runat="server" ID="ApplicantCD" />

            </div>

            <div class="row">
                <muc:PilotControl runat="server" ID="PilotCD" />
            </div>

            <div class="row">
                <div class="col-md-4">
                    <div class="col-md-8 col-sm-8 col-xs-8" style="padding-right:0px;" id="ETAdate_grp">
                        <div class="input-group input-group-sm datepicker required-date" id="divETA" data-name="ETAdate">
                            <span class="input-group-addon">E.T.A.<span class="text-danger">*</span></span>
                            <input type="text" class="form-control is_datepicker enter cls disble-fld required" data-name="ETADate" id="ETAdate" name="ETAdate" autocomplete="off"/>
                            <span class="input-group-addon datetimepicker-addon">
                                <i class="fa fa-calendar text-danger"></i>
                            </span>
                        </div>
                        <strong class="text-danger" id="ETAdate_msg"></strong>
                    </div>
                    <div class="col-md-4 col-sm-4 col-xs-4" style="padding-left:0px;" id="ETAtime_grp">
                        <input type="text" class="form-control input-sm timepicker enter cls disble-fld required" data-name="ETAtime" id="ETAtime" name="ETAtime" autocomplete="off"/>
                         <strong class="text-danger" id="ETAtime_msg"></strong>
                    </div>

                    <div class="col-md-8 col-sm-8 col-xs-8" style="padding-right:0px;" id="ETBdate_grp">
                        <div class="input-group input-group-sm datepicker required-date" id="divETB" data-name="ETBdate">
                            <span class="input-group-addon">E.T.B.<span class="text-danger">*</span></span>
                            <input type="text" class="form-control is_datepicker enter cls disble-fld required" id="ETBdate" data-name="ETBdate" name="ETBdate" autocomplete="off"/>
                            <span class="input-group-addon datetimepicker-addon">
                                <i class="fa fa-calendar text-danger"></i>
                            </span>
                        </div>
                        <strong class="text-danger" id="ETBdate_msg"></strong>
                    </div>
                    <div class="col-md-4 col-sm-4 col-xs-4" style="padding-left:0px;" id="ETBtime_grp">
                        <input type="text" class="form-control enter input-sm timepicker cls disble-fld required" id="ETBtime" data-name="ETBtime" name="ETBtime" autocomplete="off"/>
                        <strong class="text-danger" id="ETBtime_msg"></strong>
                    </div>

                    <div class="col-md-8 col-xs-8" style="padding-right:0px;" id="ETDdate_grp">
                        <div class="input-group input-group-sm datepicker required-date" id="divETD" data-name="ETDdate">
                            <span class="input-group-addon">E.T.D.<span class="text-danger">*</span></span>
                            <input type="text" class="form-control is_datepicker enter cls disble-fld required" id="ETDdate" data-name="ETDdate" name="ETDdate" autocomplete="off"/>
                            <span class="input-group-addon datetimepicker-addon">
                                <i class="fa fa-calendar text-danger"></i>
                            </span>
                        </div>
                        <strong class="text-danger" id="ETDdate_msg"></strong>
                    </div>
                    <div class="col-md-4 col-xs-4" style="padding-left:0px;" id="ETDtime_grp">
                        <input type="text" class="form-control input-sm timepicker enter cls disble-fld required" id="ETDtime" data-name="ETDtime" name="ETDtime" autocomplete="off"/>
                        <strong class="text-danger" id="ETDtime_msg"></strong>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="col-md-12">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">水先案内人要/不要</span>
                            <select class="form-control enter cls disble-fld" id="PilotRequired" name="PilotRequired">
                                <option></option>
                                <option value=true>Y</option>
                                <option value=false>N</option>
                            </select>
                        </div>
                    </div>

                    <div class="col-md-12">
                        <div class="input-group input-group-sm" id="ShipFacing_grp">
                            <span class="input-group-addon">右舷／左舷<span class="text-danger">*</span></span>
                            <select class="form-control enter cls disble-fld required" id="ShipFacing" data-name="右舷／左舷" name="ShipFacing">
                                <option></option>
                                <option value=true>右</option>
                                <option value=false>左</option>
                            </select>
                            <strong class="text-danger" id="ShipFacing_msg"></strong>
                        </div>
                    </div>

                    <div class="col-md-12">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">タグ</span>
                            <select class="form-control enter cls disble-fld" id="TugRequired" name="TugRequired">
                                <option></option>
                                <option value=true>Y</option>
                                <option value=false>N</option>
                            </select>
                        </div>
                    </div>

                    <div class="col-md-12">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">ラインボート</span>
                            <select class="form-control enter cls disble-fld" id="LineRequired" name="LineRequired">
                                <option></option>
                                <option value=true>Y</option>
                                <option value=false>N</option>
                            </select>
                        </div>
                    </div>
                </div>

            </div>

            <div class="row">
                <div class="col-md-1 col-md-offset-5 col-sm-4 col-xs-4">
                    <input type="button" ID="btnRegister" class="btn btn-info btn-block btn-sm btn-flat" value="登録(F2)"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button" ID="btnClear" class="btn btn-warning btn-block btn-sm btn-flat" value="クリア(F4)"/>
                </div>
                <div class="col-md-1 col-sm-4 col-xs-4">
                    <input type="button" id="btnBack" value="閉じる(F12)" class="btn btn-darkgrey btn-block btn-sm btn-flat"/>
                </div>
            </div>
        </div>

        
    </section>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/pages/VesselScheduleRegistration.js"></script>
    <script src="../Assets/scripts/modal/PilotModal.js"></script>
    <script src="../Assets/scripts/modal/BerthModal.js"></script>
    <script src="../Assets/scripts/modal/CompanyModal.js"></script>
    <script src="../Assets/scripts/modal/VesselModal.js"></script>
</asp:Content>
