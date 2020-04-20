<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WharfControl.ascx.vb" Inherits="BerthPlan.WharfControl" %>


<div class="col-md-2 col-xs-5" style="padding-right:0px;">
    <div class="input-group input-group-sm has-feedback" id="MainContent_WharfCD_WharfCD_grp">
        <span class="input-group-addon" id="TitleWharf">ワーフ</span>
        <asp:TextBox runat="server" type="text" class="form-control enter required upper-case special-characters single-byte" id="WharfCD" name="WharfCD" MaxLength="4" data-name="ワーフ"/>
        <span class="input-group-btn">
            <button type="button" id="btnSearchWharf" class="btn btn-flat btn-primary" data-toggle="modal" data-target="#WharfModal"><i class="fa fa-search"></i></button>
		</span>
    </div>
    <strong class="text-danger" id="MainContent_WharfCD_WharfCD_msg"></strong>
</div>

<div class="col-md-3 col-xs-7" style="padding-left:0px;">
    <asp:TextBox runat="server" type="text" class="form-control input-sm" id="WharfName" name="WharfName" readonly="true"/>
</div>


<div class="modal fade" id="WharfModal" role="dialog" data-backdrop="static">
	<div class="modal-dialog" role="document">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-label="Close">
					<span aria-hidden="true">&times;</span></button>
				<h4 class="modal-title">ワーフを選んでください。</h4>
			</div>
			<div class="modal-body">
                <div id="Form1" runat="server" class="form-horizontal">
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
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="table-responsive">
                            <table id="tblWharfModal" class="table table-condensed table-striped table-bordered"  style="width:100%">
                                <thead>
                                    <tr class="info">
                                        <td style="width:5%"></td>
                                        <td style="width:30%">ワーフコード</td>
                                        <td style="width:65%">ワーフ名</td>
                                    </tr>
                                </thead>
                                <tbody id="tblWharfModal_body"></tbody>
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