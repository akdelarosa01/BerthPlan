#Region "## インポート ##"
Imports System
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Text
Imports BerthPlan.GlobalFunction
#End Region

Public Class FileDownloader
    Implements System.Web.IHttpHandler

#Region "## クラス内変数 ## "
    Public Shared _db As BerthPlanEntities = New BerthPlanEntities()


#End Region

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim FilePath As String = ""
        Dim FileName As String = ""
        Dim sFilePath As String = ""
        Dim sdlFileName As String = ""
        Try
            FilePath = context.Request.QueryString("FilePath")
            FileName = context.Request.QueryString("FileName")

            '相対パスから物理ファイルパス取得
            sFilePath = FilePath & FileName

            'ダウンロードするファイル名
            'ファイル名が日本語の場合を考慮したダウンロードファイル名を作成
            If context.Request.Browser.Browser = "IE" Or _
               context.Request.Browser.Browser = "InternetExplorer" Then

                'IEの場合はファイル名をURLエンコード
                sdlFileName = HttpUtility.UrlEncode(FileName)
            Else
                'IE以外の場合はそのままでOK
                sdlFileName = FileName
            End If

            'ダウンロード処理

            'Response情報クリア
            context.Response.ClearContent()

            'バッファリング
            context.Response.Buffer = True

            'HTTPヘッダー情報・MIMEタイプ設定
            context.Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", sdlFileName))
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

            'ファイルを書き出し
            context.Response.WriteFile(sFilePath)
            context.Response.Flush()
            System.IO.File.Delete(sFilePath)
            context.Response.End()
        Catch ex As Exception

        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class