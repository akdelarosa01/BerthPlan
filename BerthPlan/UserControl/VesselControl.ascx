<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VesselControl.ascx.vb" Inherits="BerthPlan.VesselControl" %>

<div class="col-md-2 col-sm-3 col-xs-5 " style="padding-right:0px;">
    <div class="input-group input-group-sm has-feedback" id="MainContent_VesselCD_VesselCD_grp">
        <span class="input-group-addon" id="TitleVessel">本船</span>
        <asp:TextBox runat="server" type="text" class="form-control enter required upper-case special-characters single-byte" id="VesselCD" name="VesselCD" data-name="本船" MaxLength="4" autofocus="true"/>
        <span class="input-group-btn">
             <button type="button" id="btnSearchVessel" class="btn btn-flat btn-primary" data-toggle="modal" data-target="#VesselModal"><i class="fa fa-search"></i></button>
		</span>
    </div>
    <strong class="text-danger" id="MainContent_VesselCD_VesselCD_msg"></strong>
</div>

<div class="col-md-3 col-sm-3 col-xs-7" style="padding-left:0px;">
    <asp:TextBox runat="server" type="text" class="form-control input-sm" id="VesselName" name="VesselName" readonly="true"/>
</div>

<div class="modal fade" id="VesselModal" role="dialog" data-backdrop="static">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-label="Close">
					<span aria-hidden="true">&times;</span></button>
				<h4 class="modal-title">本船を選んでください。</h4>
			</div>
			<div class="modal-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">本船コード</span>
                            <input type="text" class="form-control enter upper-case" id="mVesselCode" name="mVesselCode" autofocus/>
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">本船名</span>
                            <input type="text" class="form-control enter" id="mVesselName" name="mVesselName"/>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="table-responsive">
                            <table id="tblVesselModal" class="table table-condensed table-striped table-bordered" style="width:100%" >
                                <thead>
                                    <tr class="info">
                                        <td style="width:5%"></td>
                                        <td style="width:20%">本船コード</td>
                                        <td style="width:65%">本船名</td>
                                    </tr>
                                </thead>
                                <tbody id="tblVesselModal_body"></tbody>
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