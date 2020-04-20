<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BerthControl.ascx.vb" Inherits="BerthPlan.BerthControl" %>

<div class="col-md-2 col-xs-5" style="padding-right:0px;">
    <div class="input-group input-group-sm has-feedback" id="MainContent_BerthID_BerthCD_grp">
        <span class="input-group-addon" id="TitleBerth">着岸バース</span>
        <asp:TextBox runat="server" type="text" class="form-control enter" id="BerthID" name="BerthID"  style="display:none" />
        <asp:TextBox runat="server" type="text" class="form-control enter required upper-case special-characters single-byte" id="BerthCD" data-name="着岸バース" name="BerthCD" MaxLength="4" />
        <span class="input-group-btn">
            <button type="button" id="btnSearchBerth" class="btn btn-flat btn-primary" data-toggle="modal" data-target="#BerthModal"><i class="fa fa-search"></i></button>
		</span>
    </div>
    <strong class="text-danger" id="MainContent_BerthID_BerthCD_msg"></strong>
</div>

<div class="col-md-3 col-xs-7" style="padding-left:0px;">
    <asp:TextBox runat="server" type="text" class="form-control input-sm" id="BerthName" name="BerthName" readonly="true" />
</div>

<div class="modal fade" id="BerthModal" role="dialog" data-backdrop="static">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-label="Close">
					<span aria-hidden="true">&times;</span></button>
				<h4 class="modal-title">着岸バースを選んでください。</h4>
			</div>
			<div class="modal-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">ワーフコード</span>
                            <input type="text" class="form-control enter upper-case" id="mWharfCode" name="mWharfCode" autofocus/>
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">ワーフ名</span>
                            <input type="text" class="form-control enter" id="mWharfName" name="mWharfName"/>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">バースコード</span>
                            <input type="text" class="form-control enter" id="mBerthCode" name="mBerthCode" autofocus/>
                        </div>
                    </div>
                    <div class="col-md-7">
                        <div class="input-group input-group-sm">
                            <span class="input-group-addon">バース名</span>
                            <input type="text" class="form-control enter" id="mBerthName" name="mBerthName"/>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="table-responsive">
                            <table id="tblBerthModal" class="table table-condensed table-striped table-bordered">
                                <thead>
                                    <tr class="info">
                                        <td style="width:5%"></td>
                                        <td style="width:20%">ワーフコード</td>
                                        <td style="width:70%">ワーフ名</td>
                                        <td style="width:20%">バースコード</td>
                                        <td style="width:70%">バース名</td>
                                    </tr>
                                </thead>
                                <tbody id="tblBerthModal_body"></tbody>
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