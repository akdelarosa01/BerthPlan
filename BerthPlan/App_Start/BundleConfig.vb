Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Optimization

Public Class BundleConfig
    ' バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkId=254726 を参照してください。
    Public Shared Sub RegisterBundles(ByVal bundles As BundleCollection)

        BundleTable.EnableOptimizations = True

        'bundles.Add(New ScriptBundle("~/bundles/WebFormsJs").Include(
        '                "~/Scripts/WebForms/WebForms.js",
        '                "~/Scripts/WebForms/WebUIValidation.js",
        '                "~/Scripts/WebForms/MenuStandards.js",
        '                "~/Scripts/WebForms/Focus.js",
        '                "~/Scripts/WebForms/GridView.js",
        '                "~/Scripts/WebForms/DetailsView.js",
        '                "~/Scripts/WebForms/TreeView.js",
        '                "~/Scripts/WebForms/WebParts.js"))

        '' これらのファイルには明示的な依存関係があり、ファイルが動作するためには順序が重要です
        'bundles.Add(New ScriptBundle("~/bundles/MsAjaxJs").Include(
        '        "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
        '        "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
        '        "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
        '        "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"))
        'bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
        '                "~/Scripts/modernizr-*"))

        '"~/Assets/plugins/datatables/datatable-func.min.js",
        '"~/Assets/plugins/datatables/datatables.all.min.js",
        '"~/Assets/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js",

        bundles.Add(New ScriptBundle("~/bundles/ScriptPlugins").Include(
                        "~/Assets/plugins/jquery/jquery.min.js",
                        "~/Assets/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js",
                        "~/Assets/plugins/moment/min/moment.min.js",
                        "~/Assets/plugins/schedule-visual/fullcalendar.min.js",
                        "~/Assets/plugins/bootstrap/js/bootstrap.min.js",
                        "~/Assets/plugins/datepicker/js/bootstrap-datetimepicker.min.js",
                        "~/Assets/plugins/timepicker/bootstrap-timepicker.min.js",
                        "~/Assets/plugins/datatables.net/js/jquery.dataTables.min.js",
                        "~/Assets/plugins/datatables.net-bs/js/dataTables.bootstrap.min.js",
                        "~/Assets/plugins/bootbox/bootbox.all.min.js",
                        "~/Assets/plugins/bootbox/bootbox.locales.min.js",
                        "~/Assets/plugins/bootstrap-colorpicker/dist/js/bootstrap-colorpicker.min.js",
                        "~/Assets/plugins/tags/src/bootstrap-tagsinput.js",
                        "~/Assets/scripts/common.js"))
    End Sub
End Class
