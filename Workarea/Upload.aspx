<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Upload.aspx.cs" Inherits="Workarea_Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />

    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        function CheckUpload()
        {
            var ofile = document.getElementById('fileupload1');
            if (ofile.value == '') {
               alert('<asp:Literal runat="server" id="litErrSelAvatarUpload" />');
               return false;
            }
            else{
                var szTempFilename = ofile.value;
                if(((szTempFilename.replace(/^.*\\/, '')).indexOf('#') > -1) || ((szTempFilename.replace(/^.*\\/, '')).indexOf('&') > -1) ||  ((szTempFilename.replace(/^.*\\/, '')).indexOf(';') > -1)) {
                    alert("A filename cannot contain '#','&',';'");
                    return false;
                }       
               if (!CheckUploadExt(ofile.value)) {
                   alert('<asp:Literal runat="server" id="litErrAvatarNotValidExtn" />');
                   return false;
               } else {
                CheckUpHelper_ShowControls(false);
                   return true;
               }
            }
        }

        function CheckUpHelper_ShowControls(flag) {
            if ('undefined' == typeof $ektron) {
                return;
            }
            
            if (flag) {
                $ektron('.EktronAvatarUploadUI .UploadUIControls').css('display', 'block');
                $ektron('.EktronAvatarUploadUI .UploadUILoadingImage').css('display', 'none');
            } else {
                $ektron('.EktronAvatarUploadUI .UploadUIControls').css('display', 'none');
                $ektron('.EktronAvatarUploadUI .UploadUILoadingImage').css('display', 'block');
            }
        }

        function CheckUploadExt(filename)
        {
            var extArray = new Array(".jpg", ".jpeg", ".gif", ".png", ".bmp");
            allowSubmit = false;
            if (filename.indexOf("\\") == -1)
            {
               ext = filename.slice(filename.lastIndexOf(".")).toLowerCase();
               for (var i = 0; i < extArray.length; i++)
               {
                if (extArray[i] == ext) { allowSubmit = true; break; }
               }
            }
            while ((filename.indexOf("\\") != -1) || (filename.indexOf("\/") != -1))
            {
               if(filename.indexOf("\\") != -1)
               {
                   filename = filename.slice(filename.lastIndexOf("\\") + 1);
                   ext = filename.slice(filename.lastIndexOf(".")).toLowerCase();
                   for (var i = 0; i < extArray.length; i++)
                   {
                    if (extArray[i] == ext) { allowSubmit = true; break; }
                   }
               }
               if(filename.indexOf("\/") != -1)
               {
                   filename = filename.slice(filename.lastIndexOf("\/") + 1);
                   ext = filename.slice(filename.lastIndexOf(".")).toLowerCase();
                   for (var i = 0; i < extArray.length; i++)
                   {
                    if (extArray[i] == ext) { allowSubmit = true; break; }
                   }
               }
            }
            return allowSubmit;
        }

        function DialogClose()
        {
            if ("function" == typeof self.parent.AvatorDialogClose)
            {
                self.parent.AvatorDialogClose();
            }
            else
            {
                self.parent.ektb_remove();
            }
        }
		//--><!]]>
    </script>

    <style type="text/css">
        <!
        -- /*--><![CDATA[/*><!--*/ /*--><![CDATA[/*><!--*/ .buttonUpload
        {
            background-image: url(images/ui/icons/checkIn.png);
            background-position: .5em center;
        }
        /*]]>*/-- ></style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div class="ektronTopSpace ektronPageInfo EktronAvatarUploadUI">
        <asp:Label ID="lbStatus" runat="server" />
        <div class="UploadUIControls">
        <input type="file" title="Enter File to Upload here" id="fileupload1" runat="server" />
        <div class="ektronTopSpace">
        </div>
        <asp:LinkButton ToolTip="Upload" CssClass="button greenHover buttonInlineBlock buttonUpload"
            ID="uploadButton" runat="server" OnClick="uploadButton_Click" />
        <a title="Cancel" class="button buttonInlineBlock redHover buttonClear" id="close"
            onclick="DialogClose();">
            <asp:Literal ID="cancelButtonText" runat="server" /></a>
        </div>
        <div class="UploadUILoadingImage" style="display: none; margin: 10px;">
            <img src="images\application\ajax-loader_circle_lg.gif" />
        </div>
        <asp:Literal ID="litScript" runat="server" />
    </div>
    </form>
</body>
</html>
