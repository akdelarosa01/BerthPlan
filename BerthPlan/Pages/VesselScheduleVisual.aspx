<%@ Page Title="本船スケジュール Visual" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="VesselScheduleVisual.aspx.vb" Inherits="BerthPlan.VesselScheduleVisual" %>

<asp:Content ID="Content3" ContentPlaceHolderID="PageStyles" runat="server">
    <link href="../Assets/plugins/schedule-visual/fullcalendar.min.css" rel="stylesheet" />
    <link href="../Assets/plugins/schedule-visual/fullcalendar.print.min.css" rel="stylesheet"  media="print"/>
    <link href="../Assets/plugins/schedule-visual/scheduler.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="content" id="SectionVisual">
        <div class="form-horizontal">
            <div class="row">
                <div class="col-md-2 col-sm-3 col-xs-12">
                    <div class="input-group input-group-sm datepicker">
                        <span class="input-group-addon">E.T.A.</span>
                        <input type="text" class="form-control enter is_datepicker" id="SearchETA" name="SearchETA" />
                        <span class="input-group-addon datetimepicker-addon">
                            <i class="fa fa-calendar text-danger"></i>
                        </span>
                    </div>
                </div>

                <div class="col-md-2 col-sm-3 col-xs-12">
                    <div class="input-group input-group-sm has-feedback">
                        <span class="input-group-addon">ワーフ</span>
                        <select class="form-control enter" id="SearchWharf" name="SearchWharf"></select>
                    </div>
                </div>

                <div class="col-md-1 col-sm-3 col-xs-6">
                    <input type="button" id="btnSearch" class="btn btn-sm btn-primary enter btn-block btn-flat" value="検索(F1)" />
                </div>

                <div class="col-md-1 col-md-offset-6 col-sm-3 col-xs-6">
                    <input type="button" id="btnExcel" class="btn btn-sm btn-success btn-block btn-flat enter input" value="印刷(F10)" />
                </div>

            </div>

        </div>

        <div class="row">
            <div class="col-md-12">
                <div id="visual"></div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-1 col-md-offset-11 col-sm-offset-8 col-sm-4 col-xs-offset-8 col-xs-4">
                <div id="tooltip"></div>
                <a id="btnClose" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-sm btn-block enter btn-flat" >閉じる(F12)</a>
            </div>
        </div>
    </section>

    <div class="modal fade" id="SchedVisualModal" role="dialog" data-backdrop="static">
	    <div class="modal-dialog" role="document">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					    <span aria-hidden="true">&times;</span></button>
				    <h4 class="modal-title">本船スケジュール 情報。</h4>
			    </div>
			    <div class="modal-body">
                    <div class="row">
                         <div class="col-md-3 col-xs-5" style="padding-right:0px;">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">本船</span>
                                <input type="text" class="form-control" id="VesselCD" name="VesselCD" MaxLength="4" readonly=""/>
                            </div>
                        </div>
                    

                        <div class="col-md-9 col-xs-7" style="padding-left:0px;">
                            <input type="text" class="form-control input-sm" id="VesselName" name="VesselName" readonly=""/>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">Voyage</span>
                                <input type="text" class="form-control textModal" id="VoyageNo" name="VoyageNo" readonly=""/>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-7 col-xs-12"> <%--style="padding-right:0px;"--%>
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">着岸バース</span>
                                <input type="text" class="form-control input-sm" id="BerthName" name="BerthName" readonly="" />
                                <%--<input type="text" class="form-control" id="BerthCD" name="BerthCD" MaxLength="4" readonly=""/>--%>
                            </div>
                        </div>

                        <%--<div class="col-md-9 col-xs-7" style="padding-left:0px;">
                            
                        </div>--%>
                    </div>

                    <div class="row">
                        <div class="col-md-3 col-xs-5" style="padding-right:0px;">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">申請者</span>
                                <input class="form-control" id="ApplicantCD" name="ApplicantCD"  MaxLength="4" readonly=""/>
                            </div> 
                        </div>

                        <div class="col-md-9 col-xs-7" style="padding-left:0px;">
                            <input class="form-control input-sm" id="ApplicantName" name="ApplicantName" readonly=""/>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-3 col-xs-5" style="padding-right:0px;">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">水先人</span>
                                <input type="text" class="form-control" id="PilotCD" name="PilotCD" MaxLength="4" readonly=""/>
                            </div>
                        </div>

                        <div class="col-md-9 col-xs-7" style="padding-left:0px;">
                            <input type="text" class="form-control input-sm" id="PilotName" name="PilotName" readonly=""/>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4 col-xs-8" style="padding-right:0px;">
                            <div class="input-group input-group-sm datepicker">
                                <span class="input-group-addon">E.T.A.</span>
                                <input type="text" class="form-control textModal" id="ETADate" name="ETADate" readonly=""/>
                            </div>
                        </div>
                        <div class="col-md-4 col-xs-4" style="padding-left:0px;">
                            <input type="text" class="form-control input-sm timepicker textModal" id="ETATime" name="ETATime" readonly=""/>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4 col-xs-8" style="padding-right:0px;">
                            <div class="input-group input-group-sm datepicker">
                                <span class="input-group-addon">E.T.B.</span>
                                <input type="text" class="form-control textModal" id="ETBDate" name="ETBDate" readonly=""/>
                            </div>
                        </div>
                        <div class="col-md-4 col-xs-4" style="padding-left:0px;">
                            <input type="text" class="form-control input-sm timepicker textModal" id="ETBTime" name="ETBTime" readonly=""/>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4 col-xs-8" style="padding-right:0px;">
                            <div class="input-group input-group-sm datepicker">
                                <span class="input-group-addon">E.T.D.</span>
                                <input type="text" class="form-control textModal" id="ETDDate" name="ETDDate" readonly=""/>
                            </div>
                        </div>
                        <div class="col-md-4 col-xs-4" style="padding-left:0px;">
                            <input type="text" class="form-control input-sm timepicker textModal" id="ETDTime" name="ETDTime" readonly=""/>
                        </div>
                    </div>
                     
                    <div class="row">
                        <div class="col-md-8">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">水先案内人要/不要</span>
                                <input type="text" class="form-control input-sm" id="PilotGuide" name="PilotGuide" readonly=""/>
                                <%--<select class="form-control textModal" id="PilotGuide" name="PilotGuide" disabled="">
                                    <option></option>
                                    <option value=true>Y</option>
                                    <option value=false>N</option>
                                </select>--%>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-8">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">右舷／左舷</span>
                                <input type="text" class="form-control input-sm" id="ShipFace" name="ShipFace" readonly=""/>
                                <%--<select class="form-control textModal" id="ShipFace" name="ShipFace" disabled="">
                                    <option></option>
                                    <option value=true>右</option>
                                    <option value=false>左</option>
                                </select>--%>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-8">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">タグ</span>
                                <input type="text" class="form-control input-sm" id="Tag" name="Tag" readonly=""/>
                                <%--<select class="form-control textModal" id="Tag" name="Tag" disabled="">
                                    <option></option>
                                    <option value=true>Y</option>
                                    <option value=false>N</option>
                                </select>--%>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-8">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">ラインボート</span>
                                <input type="text" class="form-control input-sm" id="LineBoat" name="LineBoat" readonly=""/>
                                <%--<select class="form-control textModal" id="LineBoat" name="LineBoat" disabled="">
                                    <option></option>
                                    <option value=true>Y</option>
                                    <option value=false>N</option>
                                </select>--%>
                            </div>
                        </div>
                    </div>

			    </div>
			    <div class="modal-footer">
                    <span class="btn btn-success btn-sm" id="schedule-registration">更新</span>
				    <button type="button" class="btn btn-darkgrey pull-right btn-sm" data-dismiss="modal">Close</button>
			    </div>
		    </div>
	    </div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/plugins/schedule-visual/locale/ja.js"></script>
    <script src="../Assets/plugins/schedule-visual/scheduler.min.js"></script>
    <script src="../Assets/scripts/pages/VesselScheduleVisual.js"></script>
</asp:Content>