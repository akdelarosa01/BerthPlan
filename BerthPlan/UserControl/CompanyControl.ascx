<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CompanyControl.ascx.vb" Inherits="BerthPlan.CompanyControl" %>
<div class="col-md-2 col-sm-3 col-xs-5" style="padding-right:0px;">
    <div class="input-group input-group-sm has-feedback" id="MainContent_ApplicantCD_ApplicantCD_grp">
        <span class="input-group-addon" id="TitleApplicant">申請者</span>
        <asp:TextBox runat="server" class="form-control enter required upper-case special-characters single-byte" id="ApplicantCD" name="ApplicantCD" data-name="申請者" MaxLength="4" />
        <span class="input-group-btn">
            <button type="button" id="btnSearchCompany" class="btn btn-flat btn-primary" data-toggle="modal" data-target="#CompanyModal"><i class="fa fa-search"></i></button>
	    </span>
    </div>
    <strong class="text-danger" id="MainContent_ApplicantCD_ApplicantCD_msg"></strong>
</div>

<div class="col-md-2 col-sm-3 col-xs-7" style="padding-left:0px;">
    <asp:TextBox runat="server" class="form-control input-sm" id="ApplicantName" name="ApplicantName" readonly="true"/>
</div>

<div class="modal fade" id="CompanyModal" role="dialog" data-backdrop="static">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-label="Close">
					<span aria-hidden="true">&times;</span></button>
				<h4 class="modal-title">会社を選んでください。</h4>
			</div>
			<div class="modal-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">申請者コード</span>
                            <input type="text" class="form-control enter upper-case" id="CompCode" name="CompCode" autofocus/>
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">申請者名前</span>
                            <input type="text" class="form-control enter" id="CompName" name="CompName"/>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="table-responsive">
                            <table id="tblCompanyModal" class="table table-condensed table-striped table-bordered" style="width:100%">
                                <thead>
                                    <tr class="info">
                                        <td style="width:10%"></td>
                                        <td style="width:20%">申請者コード</td>
                                        <td style="width:70%">申請者名</td>
                                    </tr>
                                </thead>
                                <tbody id="tblCompanyModal_body"></tbody>
                            </table>
                        </div>
                    </div>
                </div>
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-darkgrey pull-right btn-flat btn-sm enter" data-dismiss="modal">閉じる</button>
			</div>
		</div>
	</div>
</div>
