<%@ Page Title="本船スケジュール一覧" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="VesselScheduleList.aspx.vb" Inherits="BerthPlan.VesselScheduleList" %>


<asp:Content ID="ContentStyles" ContentPlaceHolderID="PageStyles" runat="server">
</asp:Content>

<asp:Content ID="ContentHtml" ContentPlaceHolderID="MainContent" runat="server">

    <section class="content">
        <div class="form-horizontal" >
            <div class="row">
                <div class="col-md-2 col-sm-6 col-xs-12 padd-right-date" id="StartETA_grp">
                     <div class="input-group input-group-sm has-feedback datepicker" id="divStartETA">
                        <span class="input-group-addon">E.T.A.<span class="text-danger">*</span></span>
                        <input type="text" class="form-control enter required is_datepicker" data-name="E.T.A." id="StartETA" name="StartETA" autofocus/>
                         <span class="input-group-addon">
                            <i class="fa fa-calendar text-danger"></i>
                        </span>
                    </div>
                    <strong class="text-danger" id="StartETA_msg"></strong>
                </div>

                <div class="col-md-2 col-sm-6 col-xs-12 padd-left-date">
                    <div class="input-group input-group-sm has-feedback datepicker" id="divEndETA">
                        <span class="input-group-addon">~</span>
                        <input type="text" class="form-control enter is_datepicker" id="EndETA" name="EndETA" autofocus/>
                        <span class="input-group-addon">
                            <i class="fa fa-calendar text-danger"></i>
                        </span>
                    </div>
                </div>

                <muc:VesselControl runat="server" ID="VesselCD"/>
                
                <div class="col-md-1 col-sm-3 col-xs-6">
                    <input type="button" id="btnSearch" value="検索(F1)" class="btn btn-primary enter btn-sm btn-block btn-flat"/>
                </div>

                <div class="col-md-1 col-sm-3 col-xs-6">
                    <a id="btnResgitration" href="VesselScheduleRegistration.aspx" class="btn btn-info enter btn-sm btn-block btn-flat">新規登録</a>
                </div>

            </div>

            <div class="row">
                <muc:CompanyControl runat="server" ID="ApplicantCD" />
                <muc:PilotControl runat="server" ID="PilotCD" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-12" >
                <div class="table-responsive" style="width:100%">
                    <table class="table table-condensed table-striped table-bordered" id="tblSchedule" style="width:100%" >
                        <thead id="tblSchedule_head"></thead>
                        <tbody id="tblSchedule_body"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-1 col-sm-2 col-xs-6">
                <input type="button" value="削除(F8)" class="btn btn-danger btn-block btn-sm btn-flat enter" id="btnDelete"/>
            </div>
            <div class="col-md-1 col-sm-2 col-xs-6">
                <input type="button" id="btnUpdate" value="更新(F3)" class="btn btn-warning btn-block btn-flat btn-sm enter"/>
            </div>
            <div class="col-md-1 col-sm-2 col-xs-6">
                <input type="button" id="btnExcel" value="印刷(F10)" class="btn btn-success btn-block btn-flat btn-sm enter"/>
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6">
                <a id="A2" runat="server" href="~/Pages/VesselScheduleVisual" class="btn btn-info btn-sm btn-block btn-flat enter">本船スケジュール Visual</a>
            </div>
            <div class="col-md-1 col-md-offset-6 col-sm-2 col-xs-12">
                <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-flat btn-sm enter">閉じる(F12)</a>
            </div>
        </div>
    </section>

    <%--Modal--%>
    <div class="modal fade" id="ListConflictModal" role="dialog" data-backdrop="static">
	    <div class="modal-dialog" role="document">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					    <span aria-hidden="true">&times;</span></button>
				    <h4 class="modal-title" id="msgTitle">船舶スケジュールの矛盾日</h4>
			    </div>
                
			    <div class="modal-body">
                    <div class="row">
                        <muc:BerthControl runat="server" ID="BerthID"/>
                        <div class="col-md-12">
                            <h5>船のスケジュールはまだ保存されていませんが、準備中です</h5>
                            <table id="tblNewListConflictModal" class="table table-condensed table-striped table-bordered" width="100%">
                                <thead>
                                    <tr class="info">
                                        <th>VesselNo</th>
                                        <th>船名</th>
                                        <th>ETA</th>
                                        <th>ETB</th>
                                        <th>ETD</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>

                        <div class="col-md-12">
                            <h5>船はすでにバースで予定されています</h5>
                            <table id="tblSaveListConflictModal" class="table table-condensed table-striped table-bordered" width="100%">
                                <thead>
                                    <tr class="info">
                                        <th>VoyageNo</th>
                                        <th>船名</th>
                                        <th>ETA</th>
                                        <th>ETB</th>
                                        <th>ETD</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
			    </div>

			    <div class="modal-footer">
				    <button type="button" class="btn btn-darkgrey pull-right btn-flat btn-sm" data-dismiss="modal">Close</button>
			    </div>
		    </div>
	    </div>
    </div>

</asp:Content>


<asp:Content ID="ContentScripts" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/pages/VesselScheduleList.js"></script>
    <script src="../Assets/scripts/modal/PilotModal.js"></script>
    <script src="../Assets/scripts/modal/CompanyModal.js"></script>
    <script src="../Assets/scripts/modal/VesselModal.js"></script>
</asp:Content>