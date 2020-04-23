#Region "## インポート ##"
Imports System
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Text
Imports BerthPlan.GlobalFunction

#End Region

''' <summary>
''' 掲示板登録
''' </summary>
''' <history>
''' ---VERSION----対応日付---------対応者----------対応内容----
'''    00.01      2020/03/19      AK.Dela Rosa　　作った。
''' </history>
Public Class BoardFileHandler
    Implements System.Web.IHttpHandler

#Region "## クラス内定数 ## "
#End Region

#Region "## クラス内変数 ## "
    Public Shared _db As BerthPlan.BerthPlanEntities = New BerthPlan.BerthPlanEntities()

    Structure FileInformation
        Public filename As String
        Public filetype As String
        Public filesize As Integer
    End Structure

#End Region

#Region "## コントロールイベントの定義 ## "
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim Auth As Authentication = New Authentication()
        Dim title As String = String.Empty
        Dim contents As String = String.Empty
        Dim link As String = String.Empty
        Dim pSdate As String = String.Empty
        Dim pEdate As String = String.Empty
        Dim hdStatus As String = String.Empty
        Dim hdUserID As String = String.Empty
        Dim hdUpdTime As Date = Nothing
        Dim boardID As Integer = 0
        Dim json As String = String.Empty
        Dim postedFile As HttpPostedFile = Nothing
        Dim folderPath As String = String.Empty
        Dim fileName As String = String.Empty
        Dim fileInf As FileInformation = Nothing
        Dim tBoard As BerthPlan.tBoard = Nothing
        Dim fileId As Integer = 0
        Dim StringPath As String = String.Empty
        Dim exceededFileCount As Boolean = False
        Dim exceededFileSize As Boolean = False
        Dim msgStatus As String = String.Empty
        Dim msgContent As String = String.Empty
        Dim jSonStatusCode As Integer = 0
        Dim lastSavedID As Integer = 0
        Dim CompanyIDs As Object

        Try
            'If Not fgCheckSession() Then
            '    Exit Sub
            'End If

            Integer.TryParse(context.Request.QueryString("FileId"), fileId)

            If fileId > 0 Then
                Dim bytes As Byte()
                Dim dlFileName, contentType As String
                Dim data As List(Of BerthPlan.tBoardFile) = Nothing

                data = _db.tBoardFile.AsNoTracking.Where(Function(bf) bf.Flag = False And bf.BoardFileID = fileId).ToList

                Dim tbFile As BerthPlan.tBoardFile = (From f In data
                                                      Select f).FirstOrDefault()

                bytes = CType(tbFile.Data, Byte())
                contentType = tbFile.ContentType
                dlFileName = tbFile.FileName

                context.Response.StatusCode = CInt(HttpStatusCode.OK)
                context.Response.Clear()
                context.Response.Buffer = True
                context.Response.Charset = ""
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache)

                'Set the File Content Type.
                context.Response.ContentType = contentType

                'Set the Content Disposition Header Value and FileName.
                context.Response.AppendHeader("Content-Disposition", "attachment; filename=" & dlFileName.Replace(" ", "-"))

                'Set the Response Content.
                context.Response.BinaryWrite(bytes)
                context.Response.Flush()
            Else
                With context.Request
                    title = .Form("txtTitle")
                    contents = .Form("txtContent")
                    link = .Form("txtLink")
                    pSdate = .Form("PostingStartDate")
                    pEdate = .Form("PostingEndDate")
                    hdStatus = .Form("hdStatus")
                    hdUserID = .Form("hdUserID")
                    hdUpdTime = If(.Form("hdUpdTime") = "", DateTime.Now, .Form("hdUpdTime"))
                    boardID = If(String.IsNullOrEmpty(.Form("txtBoardID")), 0, Convert.ToInt32(.Form("txtBoardID")))
                    CompanyIDs = .Form("CompanyIDs")
                End With
                Dim save = flSaveBulletinBoard(boardID, title, contents, link, pSdate, pEdate, hdStatus, hdUserID, hdUpdTime, CompanyIDs)

                If Not save.Status = "success" Then
                    json = New JavaScriptSerializer().Serialize(New With {
                               .msg = save.Msg,
                               .status = save.Status
                    })
                    context.Response.ContentType = "text/json"
                    context.Response.StatusCode = CInt(HttpStatusCode.OK)
                    context.Response.Write(json)
                    Exit Sub
                End If

                lastSavedID = Convert.ToInt32(save.Data)

                If context.Request.Files.Count > 0 Then

                    For index = 0 To context.Request.Files.Count - 1
                        If Not boardID = 0 Then
                            Dim exceededSavedFiles = (From s In _db.tBoardFile.AsNoTracking.ToList
                                                  Where s.BoardID = lastSavedID _
                                                  And s.Flag = False
                                                  Select s).ToList

                            If exceededSavedFiles.Count = 5 Then
                                exceededFileCount = True
                                Exit For
                            End If
                        End If

                        If index > 4 Then
                            exceededFileCount = True
                            Exit For
                        End If

                        Dim tBoardFile As BerthPlan.tBoardFile = New BerthPlan.tBoardFile
                        'Fetch the Uploaded File.
                        postedFile = context.Request.Files(index)
                        Dim Filesize As Double = context.Request.Files(index).ContentLength

                        If String.IsNullOrEmpty(postedFile.FileName) Then
                            If hdStatus = "EDIT" Then
                                Exit For
                            End If

                            Exit For
                        End If

                        Dim bytes As Byte()
                        Using br As BinaryReader = New BinaryReader(postedFile.InputStream)
                            bytes = br.ReadBytes(postedFile.ContentLength)
                        End Using

                        'Set the Folder Path.
                        'fileName = Path.GetFileName(postedFile.FileName)
                        'StringPath = "~/Assets/bulletin_files/" & lastSavedID.ToString & "/"
                        'folderPath = context.Server.MapPath(StringPath)

                        'If File.Exists(folderPath + fileName) Then
                        '    Exit For
                        'End If

                        'Dim dir As New DirectoryInfo(folderPath)
                        'If Not dir.Exists Then
                        '    Directory.CreateDirectory(folderPath)
                        'End If

                        ''Save the File in Folder.
                        'postedFile.SaveAs(folderPath + fileName))

                        With tBoardFile
                            .BoardID = lastSavedID
                            .FileName = postedFile.FileName '.Replace(" ", "-")
                            .FileSize = Filesize / 1024
                            .FileType = "." & postedFile.FileName.Split(".")(postedFile.FileName.Split(".").Length - 1)
                            .Path = StringPath & postedFile.FileName
                            .UploadDate = DateAndTime.Now
                            .CreateUserID = hdUserID
                            .CreatedTime = DateTime.Now
                            .UpdTime = DateAndTime.Now
                            .UpdPGID = C_PGID.BoardRegistration
                            .UpdUserID = hdUserID
                            .Flag = False
                            .Data = bytes
                            .ContentType = postedFile.ContentType
                        End With

                        _db.tBoardFile.Add(tBoardFile)
                        _db.SaveChanges()
                    Next

                    msgContent = fgMsgOut("IBP001", "")
                    msgStatus = "success"
                    jSonStatusCode = CInt(HttpStatusCode.OK)

                    If exceededFileCount Then
                        msgContent = fgMsgOut("EBP011", "")
                        msgStatus = "failed"
                    End If

                    If exceededFileSize Then
                        msgContent = fgMsgOut("EBP012", "")
                        msgStatus = "failed"
                    End If


                    json = New JavaScriptSerializer().Serialize(New With {
                         .msg = msgContent,
                         .status = msgStatus,
                         .BoardID = lastSavedID
                    })

                    context.Response.ContentType = "text/json"
                    context.Response.StatusCode = jSonStatusCode
                    context.Response.Write(json)

                Else

                    json = New JavaScriptSerializer().Serialize(New With {
                         .msg = fgMsgOut("IBP001", ""),
                         .status = "success",
                         .BoardID = lastSavedID
                    })

                    context.Response.ContentType = "text/json"
                    context.Response.StatusCode = CInt(HttpStatusCode.OK)
                    context.Response.Write(json)
                End If

            End If




        Catch ex As Exception
            json = New JavaScriptSerializer().Serialize(New With {
                    .msg = ex.Message,
                    .status = "error",
                    .BoardID = "null"
                })

            context.Response.ContentType = "text/json"
            context.Response.StatusCode = CInt(HttpStatusCode.InternalServerError)

            context.Response.Write(json)
            context.Response.End()
        End Try

        context.Response.End()
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
#End Region

#Region "## 内部メソッド ##"
    ''' <summary>
    ''' 保存機能
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function flSaveBulletinBoard(ByVal id As Integer, ByVal title As String, ByVal contents As String, ByVal link As String,
                                               ByVal postSdate As String, ByVal postEdate As String, ByVal hdStatus As String,
                                               ByVal hdUserID As String, ByVal hdUpdTime As DateTime, ByVal CompanyIDs As Object) As MyResult
        Dim Auth As Authentication = New Authentication()
        Dim tBoard As BerthPlan.tBoard
        flSaveBulletinBoard = New MyResult

        Try
            'Check Session
            'If GlobalFunction.fgCheckSession() = False Then
            '    flSaveBulletinBoard.Status = C_Flag.CodeO
            '    Exit Function
            'End If

            Dim validation = flValidateBoard(title, contents, postSdate, postEdate)
            If validation.Data = False Then
                flSaveBulletinBoard.Status = validation.Status
                flSaveBulletinBoard.Msg = validation.Msg
                Exit Function
            End If

            tBoard = New BerthPlan.tBoard
            Select Case hdStatus
                Case "EDIT"
                    tBoard = (From c In _db.tBoard.ToList()
                              Where c.BoardID = id
                              Select c).FirstOrDefault()

                    If flCheckUpdDate(hdUpdTime, (From x In _db.tBoard.AsNoTracking
                                                  Where x.BoardID = id
                                                  Select x.UpdTime).FirstOrDefault) = False Then
                        flSaveBulletinBoard.Status = "failed"
                        flSaveBulletinBoard.Msg = fgMsgOut("EBP005", "")
                        Exit Function
                    End If

                    tBoard.Title = title
                    tBoard.Contents = contents
                    tBoard.HyperLink = link
                    tBoard.PostingStartDate = postSdate
                    tBoard.PostingEndDate = postEdate
                    tBoard.UpdTime = DateTime.Now
                    tBoard.UpdPGID = C_PGID.BoardRegistration
                    tBoard.UpdUserID = hdUserID
                    tBoard.Flag = False

                    _db.Entry(tBoard).State = EntityState.Modified
                Case Else
                    tBoard.Title = title
                    tBoard.Contents = contents
                    tBoard.HyperLink = link
                    tBoard.PostingStartDate = postSdate
                    tBoard.PostingEndDate = postEdate
                    tBoard.CreateUserID = hdUserID
                    tBoard.CreatedTime = DateTime.Now
                    tBoard.UpdTime = DateTime.Now
                    tBoard.UpdPGID = C_PGID.BoardRegistration
                    tBoard.UpdUserID = hdUserID
                    tBoard.Flag = False

                    _db.tBoard.Add(tBoard)
            End Select

            If _db.SaveChanges() > 0 Then

                Dim mCompany As IQueryable(Of BerthPlan.mCompany) = Nothing
                Dim tBoardComp As IQueryable(Of BerthPlan.tBoardCompany) = Nothing

                Dim tbCompany As BerthPlan.tBoardCompany = New BerthPlan.tBoardCompany
                Dim CompID As String() = Nothing

                CompID = CompanyIDs.Split(",")
                mCompany = _db.mCompany.Where(Function(c) CompID.Contains(c.ID))

                tBoardComp = From c In _db.tBoardCompany.AsNoTracking
                             Where c.BoardID = id
                             Select c

                If Not IsNothing(tBoardComp) Then
                    _db.tBoardCompany.Where(Function(c) c.BoardID = id).ToList().ForEach(Function(c) _db.tBoardCompany.Remove(c))
                    _db.SaveChanges()
                    'For Each com As BerthPlan.tBoardCompany In tBoardComp
                    '    com.Flag = True
                    '    _db.SaveChanges()
                    'Next
                End If

                For Each comp As BerthPlan.mCompany In mCompany
                    tbCompany = New BerthPlan.tBoardCompany
                    tbCompany.BoardID = id
                    tbCompany.CompanyID = comp.ID

                    tbCompany.ApplicantCD = comp.ApplicantCD
                    tbCompany.ApplicantName = comp.ApplicantName
                    tbCompany.UpdTime = DateTime.Now
                    tbCompany.UpdUserID = hdUserID
                    tbCompany.UpdPGID = C_PGID.BoardRegistration

                    tbCompany.Flag = False

                    _db.tBoardCompany.Add(tbCompany)

                Next

                _db.SaveChanges()

                flSaveBulletinBoard.Msg = fgMsgOut("IBP001", "")
                flSaveBulletinBoard.Status = "success"
                flSaveBulletinBoard.Data = tBoard.BoardID
            Else
                flSaveBulletinBoard.Status = "failed"
                flSaveBulletinBoard.Msg = fgMsgOut("EBP005", "")
            End If

        Catch ex As Exception
            Dim msg As String = ex.InnerException.Message
            flSaveBulletinBoard = sgErrProc(ex)
        End Try

        Return flSaveBulletinBoard
    End Function

    Private Function flValidateBoard(ByVal title As String, ByVal contents As String, _
                                        ByVal postSdate As String, ByVal postEdate As String) As MyResult
        flValidateBoard = New MyResult

        Try
            flValidateBoard.Status = "failed"
            flValidateBoard.Data = False

            If String.IsNullOrEmpty(title) Or String.IsNullOrEmpty(contents) _
                Or String.IsNullOrEmpty(postSdate) Or String.IsNullOrEmpty(postEdate) Then
                flValidateBoard.Msg = fgMsgOut("EBP006", "")
                flValidateBoard.Status = "failed"
                Exit Function
            End If

            If postSdate > postEdate Then
                flValidateBoard.Msg = fgMsgOut("EBP007", "")
                flValidateBoard.Status = "failed"
                Exit Function
            End If

            flValidateBoard.Data = True

            Exit Function
        Catch ex As Exception
            flValidateBoard = sgErrProc(ex)
            Exit Function
        End Try
        Return flValidateBoard

    End Function

    Private Function getFileInfo(ByVal filePath As String) As FileInformation
        getFileInfo = Nothing
        Dim file As FileInfo = Nothing

        Try
            file = New FileInfo(filePath)

            With getFileInfo
                .filesize = file.Length
                .filename = Path.GetFileName(filePath)
                .filetype = Path.GetExtension(filePath)
            End With


        Catch ex As Exception
            Throw
        End Try
        Return getFileInfo
    End Function
#End Region
End Class