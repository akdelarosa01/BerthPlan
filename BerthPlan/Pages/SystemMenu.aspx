<%@ Page Title="メニュー" Language="vb" AutoEventWireup="false" MasterPageFile="~/Layouts/Site.Master" CodeBehind="SystemMenu.aspx.vb" Inherits="BerthPlan.SystemMenu" %>

<asp:Content ID="SystemMenu" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datatableRowActive {
      background: #85C1E9 !important;
        }
    </style>
    <section class="content-header">
        <h3>
			お知らせ
		</h3>
    </section>

	<section class="content">
        <div class="row">
            <div class="col-md-6">
                <table class="table table-striped table-bordered table-condensed table-link" id="tblBoardMenu" style="width:100%">
                    <thead>
                        <tr class="info">
                            <th>確認</th>
                            <th>タイトル</th>
                            <th>掲載者</th>
                            <th>掲載開始日</th>
                        </tr>
                    </thead>
                    <tbody id="tblBoardMenu_body"></tbody>
                </table>
                
            </div>

            <div class="col-md-6">
                <div class="form-group">
                    <textarea id="annContent" class="form-control input-sm ann-content" readonly=""></textarea>
                </div>

                <div class="row">
                    <div class="col-md-12" id="aLink">
                    </div>
                </div>

                <div class="row" id="FileDisplay"></div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-3">
                <h4 class="menu-title"><i class="fa fa-calendar"></i> スケジュール</h4>
                <ul class="nav nav-pills nav-stacked">
                    <li class="page-link"><a href="~/Pages/VesselScheduleList" runat="server">本船スケジュール一覧</a></li>
                    <li class="page-link"><a href="~/Pages/VesselScheduleRegistration" runat="server">本船スケジュール登録</a></li>
                    <li class="page-link"><a href="~/Pages/VesselScheduleVisual" runat="server">本船スケジュール Visual</a></li>
                    <li class="page-link"><a href="~/Pages/BoardRegistration.aspx" runat="server">掲示板登録</a></li>
                </ul>
            </div>
            <div class="col-md-3" id="menuMasters" runat="server">
                <h4 class="menu-title"><i class="fa fa-cogs"></i> マスター登録</h4>
                <ul class="nav nav-pills nav-stacked">
                    <li id="Li1" class="page-link"><a href="~/Masters/UserMaster" runat="server">ユーザーマスター</a></li>
                    <li id="Li2" class="page-link"><a href="~/Masters/VesselMaster" runat="server">本船マスター</a></li>
                    <li id="Li3" class="page-link"><a href="~/Masters/BerthMaster" runat="server">バースマスター</a></li>
                    <li id="Li4" class="page-link"><a href="~/Masters/CompanyMaster" runat="server">会社マスター</a></li>
                    <li id="Li5" class="page-link"><a href="~/Masters/PilotMaster" runat="server">水先人マスター</a></li>
                    <li id="Li6" class="page-link"><a id="A1" href="~/Masters/WharfMaster" runat="server">ワーフマスター</a></li>
                </ul>
            </div>
        </div>
	</section>


	    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="../Assets/scripts/pages/SystemMenu.js"></script>
</asp:Content>