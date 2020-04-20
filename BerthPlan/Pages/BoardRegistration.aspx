<%@ Page Title="掲示板登録" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="BoardRegistration.aspx.vb" Inherits="BerthPlan.BoardRegistration" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <section class="content">

        <div class="row">
            <div class="col-md-6">
                <table class="table table-condensed table-striped table-bordered table-link" id="tblBulletin" width="100%">
                    <thead>
                        <tr class="info">
                            <th>
                                <input type="checkbox" class="checkAllitem"/>
                            </th>
                            <th></th>
                            <th>タイトル</th>
                            <th>掲載開始日</th>
                            <th>掲載終了日</th>
                            <th>最後の更新</th>
                        </tr>
                    </thead>
                    <tbody id="tblBulletin_body"></tbody>
                </table>
            </div>
            <div class="col-md-6">
                <table class="table table-condensed table-striped table-bordered table-link" id="tblFiles" width="100%">
                    <thead>
                        <tr class="info">
                            <th>
                                <input type="checkbox" class="checkAllFileItem"/>
                            </th>
                            <th>ファイル名</th>
                            <th>ファイル拡張子</th>
                            <th>ファイルサイズ</th>
                            <th>アップロード日</th>
                        </tr>
                    </thead>
                    <tbody id="tblFiles_body"></tbody>
                </table>
                <div class="row">
                    <div class="col-md-1">
                        <button type="button" class="btn btn-sm btn-danger btn-flat" id="btnDeleteFile">ファイルを削除</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-1">
                <button type="button" id="btnNew" class="btn btn-sm btn-primary btn-block btn-flat">新规(F1)</button>
            </div>
        </div>

        <div id="BulletinBoardRegistration" class="form-horizontal" runat="server">
            <div class="row">
                <div class="col-md-6">
                    <input type="hidden" ID="hdUserID" name="hdUserID" />
                    <input type="hidden" ID="hdStatus" name="hdStatus" class="clear" />
                    <input type="hidden" ID="hdUpdTime" name="hdUpdTime" class="clear" />
                    <input type="hidden" ID="txtBoardID" name="txtBoardID" class="clear" value="0"/>

                    <div class="input-group input-group-sm has-feedback" id="txtTitle_grp">
                        <span class="input-group-addon">タイトル</span>
                        <input type="text" class="form-control enter clear required" id="txtTitle" name="txtTitle" data-name="タイトル"/>
                    </div>
                    <strong class="text-danger" id="txtTitle_msg"></strong>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="input-group input-group-sm has-feedback" id="txtContent_grp">
                        <span class="input-group-addon">内容</span>
                        <textarea id="txtContent" name="txtContent" class="form-control enter clear required" Rows="5" style="resize:none;height:100px;width:100%" data-name="内容"></textarea>
                    </div>
                    <strong class="text-danger" id="txtContent_msg"></strong>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="input-group input-group-sm has-feedback" id="txtLink_grp">
                        <span class="input-group-addon">リンク</span>
                        <input type="url" class="form-control enter clear" id="txtLink" name="txtLink" data-name="リンク"/>
                    </div>
                    <strong class="text-danger" id="txtLink_msg"></strong>
                </div>
            </div>

            <div class="row">
                <div class="col-md-3 padd-left-date">
                    <div class="input-group input-group-sm has-feedback datepicker required-date" id="PostingStartDate_grp" data-name="PostingStartDate" >
                        <span class="input-group-addon">揭載期間</span>
                        <input type="text" class="form-control enter required clear is_datepicker" id="PostingStartDate" name="PostingStartDate" data-name="終了日"/>
                        <span class="input-group-addon">
                            <i class="fa fa-calendar text-danger"></i>
                        </span>
                    </div>
                    <strong class="text-danger" id="PostingStartDate_msg"></strong>
                </div>

                <div class="col-md-3">
                    <div class="input-group input-group-sm has-feedback datepicker required-date" id="PostingEndDate_grp" data-name="PostingEndDate" >
                        <span class="input-group-addon">
                            ~
                        </span>
                        <input type="text" class="form-control enter required clear is_datepicker" id="PostingEndDate" name="PostingEndDate"  data-name="終了日"/>
                        <span class="input-group-addon">
                            <i class="fa fa-calendar text-danger"></i>
                        </span>
                    </div>
                    <strong class="text-danger" id="PostingEndDate_msg"></strong>
                </div>
            </div>

            <div class="row">
                
                <div class="col-md-5">
                    <div class="input-group input-group-sm">
                        <label class="input-group-btn">
                            <span class="btn btn-default">
                                ブラウズ <i class="fa fa-folder-open"></i> 
                                <asp:FileUpload ID="fileAttachment" runat="server" Cssclass="custom-file-input" AllowMultiple="true"/>
                            </span>
                        </label>
                        <input type="text" class="form-control enter file-label" readonly="true">
                    </div>
                </div>
                <div class="col-md-1">
                    <button id="btnUpload" type="button" class="btn btn-sm btn-flat btn-primary btn-block enter">アップロード(F3)</button>
                </div>
            </div>

            <div class="row" style="margin-top:10px">
                <div class="col-md-1">
                    <button id="btnSave" type="button" class="btn btn-info btn-block btn-sm btn-flat enter" runat="server">登録(F2)</button>
                </div>
                <div class="col-md-1 ">
                    <button id="btnClear" type="button" class="btn btn-warning btn-block btn-sm btn-flat enter">クリアー(F4)</button>
                </div>
                <div class="col-md-1 ">
                    <button id="btnDelete" type="button" class="btn btn-danger btn-block btn-sm btn-flat enter">削除(F8)</button>
                </div>
                <div class="col-md-1 col-md-offset-8">
                    <a id="A1" runat="server" href="~/Pages/SystemMenu" class="btn btn-darkgrey btn-block btn-sm btn-flat enter">閉じる(F12)</a>
                </div>
            </div>
        </div>

        
    </section>

    <%--Modal--%>
    <div class="modal fade" id="BoardViewModal" role="dialog" data-backdrop="static">
	    <div class="modal-dialog" role="document">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					    <span aria-hidden="true">&times;</span></button>
				    <h4 class="modal-title" id="msgTitle">掲示板　未読/既読　確認</h4>
			    </div>
                
			    <div class="modal-body">
                    <div class="row">
                        <div class="col-md-12">
                            <table id="tblBoardViewModal" class="table table-condensed table-striped table-bordered" width="100%">
                                <thead>
                                    <tr class="info">
                                        <th>ユーザーID</th>
                                        <th>ユーザー名</th>
                                        <th>既読</th>
                                        <th>閲覧日</th>
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

<asp:Content ID="Content4" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/pages/BoardRegistration.js"></script>
</asp:Content>
