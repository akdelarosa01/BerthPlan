<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PilotControl.ascx.vb" Inherits="BerthPlan.PilotControl" %>

<div class="col-md-2 col-sm-3 col-xs-5" style="padding-right:0px;">
    <div class="input-group input-group-sm has-feedback" id="MainContent_PilotCD_PilotCD_grp">
        <span class="input-group-addon" id="TitlePilot">水先人</span>
        <input runat="server" type="text" class="form-control enter required upper-case special-characters single-byte" id="PilotCD" name="PilotCD" MaxLength="4" data-name="水先人" autofocus/>
        <span class="input-group-btn">
            <button type="button" id="btnSearchPilot" class="btn btn-flat btn-primary" data-toggle="modal" data-target="#PilotModal"><i class="fa fa-search"></i></button>
		</span>
    </div>
    <strong class="text-danger" id="MainContent_PilotCD_PilotCD_msg"></strong>
</div>

<div class="col-md-3 col-sm-3 col-xs-7" style="padding-left:0px;">
    <asp:TextBox runat="server" type="text" class="form-control input-sm" id="PilotName" name="PilotName" readonly="true"/>
</div>

<div class="modal fade" id="PilotModal" role="dialog" data-backdrop="static">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-label="Close">
					<span aria-hidden="true">&times;</span></button>
				<h4 class="modal-title">水先人を選んでください。</h4>
			</div>
			<div class="modal-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">水先人コード</span>
                            <input type="text" class="form-control enter upper-case" id="mPilotCode" name="mPilotCode"/>
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">水先人名</span>
                            <input type="text" class="form-control enter" id="mPilotName" name="mPilotName"/>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="table-responsive">
                            <table id="tblPilotModal" class="table table-condensed table-striped table-bordered" style="width:100%">
                                <thead>
                                    <tr class="info">
                                        <td style="width:5%"></td>
                                        <td style="width:30%">水先人コード</td>
                                        <td style="width:65%">水先人名</td>
                                    </tr>
                                </thead>
                                <tbody id="tblPilotModal_body"></tbody>
                            </table>
                        </div>
                        
                    </div>
                </div>
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-darkgrey pull-right btn-flat btn-sm enter" data-dismiss="modal">Close</button>
			</div>
		</div>
	</div>
</div>