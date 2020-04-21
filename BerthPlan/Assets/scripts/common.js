/** 
* <summary>
*  Common Javascript functions
* </summary>
* <history>
* ---VERSION----対応日付---------対応者----------対応内容----
*    00.01      2020/03/12      AK.Dela Rosa　　コードを作った。
* </history>
*/

$(function () {

    /* File inputs Design */
    bs_input_file();

    /* Initilize Date Input */
    $('.datepicker').datetimepicker({


        allowInputToggle: false,
        format: 'YYYY/MM/DD',
        locale: moment.locale('ja'),
        useCurrent: false
    });

    $('.datepicker').on('dp.change', function (e) { $(this).focus; });

    $('.datepicker').on("keydown", 'input', function (e) {
        if (e.keyCode == 32) {
            this.setSelectionRange(0, this.value.length);
            $(this.nextElementSibling).trigger("click");
        }
    });

    $('.is_datepicker').each(function (i,x) {
        var h = jQuery._data($('.datepicker input')[i], "events").keydown[0].handler;
        jQuery._data($('.datepicker input')[i], "events").keydown[0].handler = function (e) {
            var event = $.Event('dpkeydown');
            event.keyCode = e.keyCode;
            $(this).trigger(event);

            h(e);

            var self = $(this)
                , form = self.parents('form:eq(0)')
                , focusable
                , next
            ;

            if (e.keyCode === 13) {
                focusable = form.find('.enter').filter(':visible');
                next = focusable.eq(focusable.index(this) + 1);

                if (next.is(":disabled")) {
                    next = focusable.eq(focusable.index(this) + 2);
                }

                if (next.length) {
                    switch (e.target.type) {
                        case "submit":
                            next.form.submit();
                            break;
                        default:
                            next.focus();
                    }
                    next.focus();
                }
                return false;
            }
            //h(e);
        }
    });

    /* Highlighting Date Input value */
    $('.datepicker').on('click', 'input',function () {
        this.setSelectionRange(0, this.value.length)
    });

    /* Initilize Time Input */
    $('.timepicker').timepicker({
        showInputs: false,
        minuteStep: 1,
        explicitMode: true
    });

    /* Highlighting Time Input value */
    $('.timepicker').on('click', 'input',function () {
        this.setSelectionRange(0, this.value.length)
        console.log(this);
    });

    $(document).on('blur', '.timepicker', function () {
        $(this).timepicker('hideWidget');
    });

    /* DataTable Column adjuster in all Modals */
    $('.modal').on('shown.bs.modal', function () {
        setTimeout(function () {
            $('input:text:visible:first').focus();
        }, 1000);

        var table = $('.table').DataTable();
        table.columns.adjust();
        $('button').focus();
    });

    $('#msg_modal').on('shown.bs.modal', function () {
        $('button').focus();
    });

    /* Block Special Characters && pasting */
    $('.special-characters').on('keypress', function (e) {
        if (!blockSpecialCharacters(e)) {
            e.preventDefault();
        }
    });
    $('.special-characters').on('cut copy paste', function (e) {
        e.preventDefault();
    });

    /* Enter key as Tab key */
    $('body').on('keydown', '.enter', function (e) {
        var self = $(this)
			, form = self.parents('form:eq(0)')
			, focusable
			, next
        ;
        if (e.keyCode == 13 && e.altKey == true) {
            switch (e.target.type) {
                case "textarea":
                    var content = this.value;
                    var caret = getCaret(this);
                    this.value = content.substring(0, caret) + "\n" + content.substring(caret, content.length - 1);
                    e.stopPropagation();
                    break;
                default:
            }
            return false;
        }

        if (e.keyCode == 13) {
            focusable = form.find('.enter').filter(':visible');

            next = focusable.eq(focusable.index(this) + 1);

            if (next.is(":disabled")) {
                next = focusable.eq(focusable.index(this) + 2);
            }

            if (next.length) {
                switch (e.target.type) {
                    case "submit":
                        next.form.submit();
                        break;
                    default:
                        next.focus();
                }
                next.focus();
            }
            return false;
        }
    });

    /* Initilize Date Mask */
    $(".date-mask").inputmask("y/m/d", {
        autoUnmask: false
    });

    /* Initilize Color Input */
    $('.colorpicker').colorpicker()

    /* Hide Error when keydown on components */
    $('.required').on('keydown', function (e) {
        $('.required').each(function (i) {
            if (e.target.value !== '' || e.target.value !== null) {
                hideError(e.target.id);
            }
        });
    });

    $('.required').on('change', function (e) {
        $('.required').each(function (i) {
            if (e.target.value !== '' || e.target.value !== null) {
                hideError(e.target.id);
            }
        });
    });

    $(".required-date").on("dp.change", function (e) {
        if (e.target.value != false) {
            hideError($(this).attr('data-name'));
        }
    });

    /* Make character as Upper Case */
    $('.upper-case').on('change', function () {
        $(this).val($(this).val().toUpperCase());
    })

    /* a-z A-Z 0-9 */
    $(".single-byte").on("input change paste", function () {
        var newVal = $(this).val().replace(/[^a-zA-Z0-9\-@)/+*$,._\\-]/g, '');
        $(this).val(newVal);
    });
});

function getCaret(el) {
    if (el.selectionStart) {
        return el.selectionStart;
    } else if (document.selection) {
        el.focus();

        var r = document.selection.createRange();
        if (r == null) {
            return 0;
        }

        var re = el.createTextRange(),
            rc = re.duplicate();
        re.moveToBookmark(r.getBookmark());
        rc.setEndPoint('EndToStart', re);

        return rc.text.length;
    }
    return 0;
}

/**
* Show Error messages of required components
*/
function showError(id, msg) {
    $('#' + id + '_grp').addClass('has-error');
    $('#' + id + '_msg').html(msg);
}

/**
* Hide Error messages of required components
*/
function hideError(id) {
    $('#' + id + '_grp').removeClass('has-error');
    $('#' + id + '_grp').removeClass('has-warning');
    $('#' + id + '_msg').html('');
}

/**
* Show Loading Overlay
*/
function ShowLoading() {
    $("#loading-overlay").show();
}

/**
* Hide Loading Overlay
*/
function HideLoading() {
    $("#loading-overlay").hide();
}

/**
* Uppercase first letter
* @param  {[String]} word [word to uppercase first letter]
* @return {[String]}      [word with uppercase first letter]
*/
function jsUcfirst(word) {
    return word.charAt(0).toUpperCase() + word.slice(1);
}

/**
* Redesigning File inputs
*/
function bs_input_file() {
    $(".input-file").before(
		function () {
		    if (!$(this).prev().hasClass('input-ghost')) {
		        var element = $("<input type='file' class='input-ghost' style='visibility:hidden; height:0'>");
		        element.attr("name", $(this).attr("name"));
		        element.change(function () {
		            element.next(element).find('input').val((element.val()).split('\\').pop());
		        });
		        $(this).find("button.btn-choose").click(function () {
		            element.click();
		        });
		        $(this).find("button.btn-reset").click(function () {
		            element.val(null);
		            $(this).parents(".input-file").find('input').val('');
		        });
		        $(this).find('input').css("cursor", "pointer");
		        $(this).find('input').mousedown(function () {
		            $(this).parents('.input-file').prev().click();
		            return false;
		        });
		        return element;
		    }
		}
	);
}

/**
* Open Message Modal
* @param  {String} msg_content [message content]
* @param  {String} status [is it success or failed]
*/
function msg(msg_content, status) {
    $('#msg_content').html(msg_content);

    switch (status) {
        case 'success':
            $('#msg_title').css('color', '#1BA39C');
            $('#msg_title').html('<strong><i class="fa fa-check"></i></strong> ' + jsUcfirst(status) + "!");
            break;
        case 'failed':
            $('#msg_title').css('color', '#F36A5A');
            $('#msg_title').html('<strong><i class="fa fa-exclamation-circle"></i></strong> ' + jsUcfirst(status) + "!");
            break;
        case 'error':
            $('#msg_title').css('color', '#E7505A');
            $('#msg_title').html('<strong><i class="fa fa-times"></i></strong> ' + jsUcfirst(status) + "!");
            break;
        default:
            $('#msg_title').css('color', '#1BA39C');
    }
    $('#msg_modal').modal('show');
}

/**
* Check all checkboxes
* @param  {String} checkAllClass     [html classname of check all checkbox]
* @param  {String} checkItemClass    [html classname of each checkboxes]
* @param  {string} deleteButtonID    [Disabling button delete]
*/
function checkAllCheckboxesInTable(tblID, checkAllClass, checkItemClass, deleteButtonID) {
    $(checkAllClass).on('change', function (e) {
        $('input:checkbox' + checkItemClass).not(this).prop('checked', this.checked);

        var checked = 0;
        var table = $(tblID).DataTable();
        for (var x = 0; x < table.context[0].aoData.length; x++) {
            if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
                checked++;
            }
        }

        if (checked > 0) {
            $(deleteButtonID).prop('disabled', false);
        } else {
            $(deleteButtonID).prop('disabled', true);
        }
    });
}

/**
* Identifying if all checkboxes in table were checked
* @param  {String} tblBodyID [html id of datatable body]
* @param  {String} checkAllClass [html classname of check all checkbox]
* @param  {String} checkItemClass [html classname of each checkboxes]
*/
function checkAllIfChecked(tblID, checkAllClass, checkBoxItemClass,btnID) {
    var checked = 0;
    var table = $(tblID).DataTable();
    for (var x = 0; x < table.context[0].aoData.length; x++) {
        if (table.context[0].aoData[x].anCells[0].firstChild.checked == true) {
            checked++;
        }
    }

    if (checked > 0) {
        $(btnID).prop('disabled', false);
    } else {
        $(checkAllClass).prop('checked', false);
        $(btnID).prop('disabled', true);
    }
}

/**
* DataTable Language Setting
* Currently set in Nihongo
*/
function dataTableLanguageVariable() {
    var Lang = {
        "decimal": "",
        "emptyTable": "テーブル内のデータなし",
        "info": "_TOTAL_エントリのうち_START_から_END_を表示している",
        "infoEmpty": "0エントリの0から0を表示",
        "infoFiltered": "（_MAX_合計エントリからフィルタリング）",
        "infoPostFix": "",
        "thousands": ",",
        "lengthMenu": "_MENU_エントリを表示",
        "loadingRecords": "データを読み込んでいる...",
        "processing": "処理データ...",
        "search": "検索:",
        "zeroRecords": "該当する記録が見つからない。",
        "paginate": {
            "first": "最初",
            "last": "最終",
            "next": "次",
            "previous": "前"
        },
        "aria": {
            "sortAscending": ": activate to sort column ascending",
            "sortDescending": ": activate to sort column descending"
        },
        //"processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
    }
    return Lang;
}

/**
* DataTable Instantiation and Options
* @param  {String} tblID        [ID of table to make it a DataTable]
* @param  {String} AjaxURL      [Serverside Url to process data.]
* @param  {Array} ColumnFormat  [Specific format of columns]
* @param  {Array} OrderByColumn [Set Ordering of Columns] EX. [0,"asc"]
*/
function initDataTable(tblID, AjaxURL, ColumnFormat, OrderByColumn,withDelete) {
    $(tblID).dataTable().fnClearTable();
    $(tblID).dataTable().fnDestroy();
    $(tblID).dataTable({
        "columnDefs": [
            { "width": "5%", "targets": [0] },
            { "className": "text-center custom-middle-align td-checkbox", "targets": [0] },
            { "targets": 0, "sortable": false, "orderable": false },
        ],
        "destroy": true,
        "lengthChange": false,
        "ordering": false,
        "searching": true,
        "sDom": "rtipl",
        "order": OrderByColumn,
        "language": dataTableLanguageVariable(),
        "processing": true,
        "serverSide": true,
        "responsive": true,
        "ajax": {
            "url": AjaxURL,
            "contentType": "application/json",
            "type": "GET",
            "dataType": "JSON",
            "data": function (d) {
                return d;
            },
            "dataSrc": function (json) {
                json.draw = json.d.draw;
                json.recordsTotal = json.d.recordsTotal;
                json.recordsFiltered = json.d.recordsFiltered;
                json.data = json.d.data;

                var return_data = json;
                return return_data.data;
            }
        },
        "columns": ColumnFormat,
        "fnDrawCallback": function () {
            $('.datepicker').datetimepicker({
                format: 'YYYY/MM/DD',
                locale: 'ja'
            });
            $('.datepicker').on('click', 'input', function () {
                this.setSelectionRange(0, this.value.length)
            });
            $('.timepicker').timepicker({
                showInputs: false,
                minuteStep: 1,
                template: false
            });
            $('.timepicker').on('click', 'input', function () {
                this.setSelectionRange(0, this.value.length)
            });
            $(".date-mask").inputmask("y/m/d", {
                autoUnmask: true
            });
        },
        "dom": "<'row' <'col-md-12'B>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
    });
    
}

/**
* Parsing of date from DB
* @param  {string} date_value     [Date value from database.]
*/
function ParseDate(date_value) {
    if (date_value !== null || date_value !== "" || typeof(date_value) != undefined ) {
        var d = new Date(parseInt(date_value.substring(6))),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();
        
        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        fdate = [year, month, day].join('/');

        if (fdate === '1/01/01' || fdate === 'NaN/NaN/NaN') {
            return ""
        }

        return fdate
    } else {
        return '';
    }
}

/**
* Parsing of date from DB
* @param  {string} date_value     [Date value from database.]
* @param  {string} format         ['24' = 24 Hours or AMPM]
*/
function ParseTime(date_value, format) {
    if (date_value !== null || date_value !== "") {
        var d = new Date(parseInt(date_value.substring(6)));
        if (format == "24") {
            return d.getHours() + ':' + (d.getMinutes() < 10 ? '0' : '') + d.getMinutes();
        } else {
            var hours = d.getHours();
            var AMPM = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            return hours + ':' + (d.getMinutes() < 10 ? '0' : '') + d.getMinutes() + ' ' + AMPM;
        }
    } else {
        return '';
    }
}
/**
* Parsing of Time from DB
* @param  {string} date_value     [Date value from database.]
*/
function ParseDateTime(date_value,format) {
    if ((date_value !== null || date_value !== "" || typeof (date_value) != undefined) && (format === undefined)) {
        var d = new Date(parseInt(date_value.substr(6)));
        return ParseDate(date_value) + ' ' + (d.getHours() < 10 ? '0' : '') + d.getHours() + ':' + (d.getMinutes() < 10 ? '0' : '') + d.getMinutes();
    } else if ((date_value !== null || date_value !== "" || typeof (date_value) != undefined) && (format == "ss")) {
        var d = new Date(parseInt(date_value.substr(6)));
        return ParseDate(date_value) + ' ' + (d.getHours() < 10 ? '0' : '') + d.getHours() + ':' + (d.getMinutes() < 10 ? '0' : '') + d.getMinutes() + ':' + d.getSeconds();
    }
    return '';
}

/**
* Get Date Today
* @param  
*/
function DateNow() {
    var d = new Date();
    var today_date = d.getFullYear() + "/" +
        ("0" + (d.getMonth() + 1)).slice(-2) + "/" +
        ("0" + d.getDate()).slice(-2);
    return today_date;
}
/**
* Fix the format of yyyyMMdd to yyyy/MM/dd
* @param  
*/
function DateMaskFormat(date_value) {
    if (date_value == "") {
        return "";
    } else {
        var d = date_value.replace(/\//g, "");
        return d.substring(0, 4) + "/" + d.substring(4, 6) + "/" + d.substring(6);
    }
}

/**
* Parsing of date from DB
* @param  {string} clearClass     [Class name of element to clear]
*/
function clear(clearClass) {
    $(clearClass).val("");
    //$(clearClass).val("").datepicker("update");
}

/**
* Get File Icon
* @param  {string} ext     [Extention name]
*/
function fileExtension(ext) {
    var data = {
        icon: "",
        ext: ""
    };
    switch (ext.toLowerCase()) {
        // Audio files
        case ".cda":
        case ".mid":
        case ".midi":
        case ".mp3":
        case ".mpa":
        case ".ogg":
        case ".wav":
        case ".wma":
        case ".wpl":
        case ".aif":
            data.icon = "fa fa-file-audio-o";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Compressed files
        case ".7z":
        case ".tar.gz":
        case ".deb":
        case ".rpm":
        case ".arj":
        case ".pkg":
        case ".rar":
        case ".z":
        case ".zip":
            data.icon = "fa fa-file-archive-o";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Image Files
        case ".gif":
        case ".bmp":
        case ".svg":
        case ".ps":
        case ".psd":
        case ".png":
        case ".ai":
        case ".ico":
        case ".jpeg":
        case ".jpg":
        case ".tif":
        case ".tiff":
            data.icon = "fa fa-file-image-o";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Internet Files
        case ".aps":
        case "aspx":
        case ".cer":
        case ".cfm":
        case ".css":
        case ".html":
        case ".htm":
        case ".js":
        case ".jsp":
        case ".part":
        case ".php":
        case ".py":
        case ".rss":
        case ".xhtml":
            data.icon = "fa fa-file-code-o";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Presentation Files
        case ".key":
        case ".odp":
        case ".pps":
        case ".ppt":
        case ".pptx":
            data.icon = "fa fa-file-powerpoint-o text-danger";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Spreadsheet
        case ".ods":
        case ".xls":
        case ".xlsm":
        case ".xlsx":
            data.icon = "fa fa-file-excel-o text-success";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Video files
        case ".3g2":
        case ".3gp":
        case ".avi":
        case ".flv":
        case ".h264":
        case ".m4v":
        case ".mkv":
        case ".mov":
        case ".mp4":
        case ".mpg":
        case ".mpeg":
        case ".rm":
        case ".wmv":
            data.icon = "fa fa-file-video-o";
            data.ext = ext.split('.').join("");
            return data;
            break;

            // Word Docu files
        case ".txt":
        case ".csv":
            data.icon = "fa fa-file-text-o text-warning";
            data.ext = ext.split('.').join("");
            return data;
            break;

        case ".pdf":
            data.icon = "fa fa-pdf-o text-danger";
            data.ext = ext.split('.').join("");
            return data;
            break;

        case ".docx":
        case ".doc":
        case ".odt":
        case ".rtf":
        case ".tex":
        case ".wpd":
            data.icon = "fa fa-file-word-o";
            data.ext = ext.split('.').join("");
            return data;
            break;

        default:
            data.icon = "fa fa-file-o";
            data.ext = ext.split('.').join("");
            return data;
            break;
    }
}

/**
* Output Message String (Error/Success)
    * Q01 - Delete the selected row. Is it OK?
    * E01 - Please fill in the required fields.
    * E02 - %1 is too short. 
    * E03 - Please select atleast one
    * E04 - Please check date
    * E05 - New password and Confirm new password did not match
    * E06 - %1 is required.
    * E07 - %1 value is not valid.
    * E09 - Please choose at least 1 file or maximum of 5 files.
    * I01 - Data deleted successfully
* @param  {[String]} sMode [error message mode]
* @param  {[String]} sData [data to output]
* @return {[String]}      [error message string]
*/
function getMsg(sMode, sData) {
    var rString = '';
    if (sData == undefined) {
        sData = '';
    }

    switch (sMode) {
        case 'Q01':
            rString = '選択行を削除します。よろしいですか？';             
            break;
        case 'E01':
            rString = '必須フィールドに記入してください。';               
            break;
        case 'E02':
            rString = '' + sData + 'が短すぎます。';
            break;
        case 'E03':
            rString = '少なくとも１つは選択してください。';
            break;
        case 'E04':
            rString = '日付を確認してください。' + sData;
            break;
        case 'E05':
            rString = '新しいパスワードと新しいパスワードの確認が一致しませんでした。'; 
            break;
        case 'E06':
            rString = '' + sData + 'が必要です。';
            break;
        case 'E07':
            rString = '' + sData + 'の値が無効です。';
            break;
        case 'E08':
            rString = 'データがありません。';
            break;
        case 'E09':
            rString = '少なくとも1つのファイルまたは最大5つのファイルを選択してください。';
            break;
        case 'I01':
            rString = 'データが正常に削除されました。';
            break;
        default:
            rString = sData;
            break;
    }
    return rString;
}

/* Block special characters && pasting */
function blockSpecialCharacters(e) {
    var key = e.key;
    var keyCharCode = key.charCodeAt(0);

    // 0-9
    if (keyCharCode >= 48 && keyCharCode <= 57) {
        return key;
    }
    // A-Z
    if (keyCharCode >= 65 && keyCharCode <= 90) {
        return key;
    }
    // a-z
    if (keyCharCode >= 97 && keyCharCode <= 122) {
        return key;
    }
    //space
    //if (keyCharCode == 32) {
    //    return key;
    //}

    return false;
}

/* Prevent copy & paste */
function preventPaste(sName) {
    $(sName).on('cut copy paste', function (e) {
        e.preventDefault();
    });
}

/**
* Formatting number values
* @param  {integer} number     [Number value]
* @param  {string} style       [Style of value]
* @param  {string} currency    [Currency sign]
*/
function numberFormat(number, style, currency) {
    var num;
    if (style === '' && currency === '') {
        num = new Intl.NumberFormat('ja-JP', {
                        style: style,
                        currency: currency
        }).format(number)

        return num;
    }

    num = new Intl.NumberFormat('ja-JP').format(number)

    return num;
    
}

function sessionOut(sStatus) {
    if (sStatus == 'expire') {
        location.reload();
    }
}

/**
* Email validation
* @param  {string} email     [Email Address]
*/
function validateEmail(email) {
    var regex = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,6})?$/;
    return regex.test(email);
}


