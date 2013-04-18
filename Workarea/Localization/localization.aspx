<%@ Page Language="C#" AutoEventWireup="false" CodeFile="localization.aspx.cs" Inherits="Workarea_localization" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Localization</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />    
    <asp:Literal id="StyleSheetJS" runat="server" />
	<script type="text/javascript">
    <!--//--><![CDATA[//><!--	
	function SubmitForm(FormName, Validate) 
	{   
	    $ektron('#pleaseWait').modalShow();
		try
		{
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[0].submit();
					return false;
				}
				else {
					return false;
				}
			}
			else {
				document.forms[0].submit();
				return false;
			}
		}
		catch (e)
		{
			$ektron('#pleaseWait').modalHide();
			if ("TypeError" == e.name)
			{
				alert("<asp:Literal id='ltr_fileMissing' runat='server' />");
				
			}
			else
			{
				alert(e.message);
			}
		}
	}

	function validate()
	{
		var valid = true;
		var numSelected = 0;
		var objForm = document.forms[0];
		var objElem = null;
		
		if (valid)
		{
			for (var iCount = 0; ; iCount++)
			{
				objElem = objForm.elements["FileUpload" + iCount];
				if (null == objElem) break;
				var strFileExt = objElem.value;
				strFileExt = strFileExt.substr(strFileExt.length - 4, 4).toLowerCase();
				if (".xlf" == strFileExt || ".xml" == strFileExt || ".zip" == strFileExt) // .xml for Trados
				{
					numSelected++;
				}
				else if (strFileExt.length > 0)
				{
					valid = false;
					break;
				}
			}
			if (!valid)
			{
				alert("<asp:Literal runat='server' id='ltr_Permitted' />");
				valid = false;
			}
			else if (0 == numSelected) 
			{
				alert("<asp:Literal runat='server' id='ltr_selectOne' />");
				valid = false;
			} 
		}
		if (!valid)
		{
			$ektron('#pleaseWait').modalHide();			
		}
		return valid;
	}
	
	Ektron.ready( function() {
        // PLEASE WAIT MODAL
        $ektron("#pleaseWait").modal({
            trigger: '',
            modal: true,
            toTop: true,
            onShow: function(hash) {
                hash.o.fadeIn();
                hash.w.fadeIn();
            },
            onHide: function(hash) {
                hash.w.fadeOut("fast");
                hash.o.fadeOut("fast", function()
                {
                    if (hash.o) {
                        hash.o.remove();
                    }
                });
            }
        });
    });
    //--><!]]>	
	</script>	
    <style type="text/css">
        <!--/*--><![CDATA[/*><!--*/
	        div#pleaseWait { width: 128px; height: 128px; margin: -64px 0 0 -64px; background-color: #fff; background-image: url("../images/ui/loading_big.gif"); backgground-repeat: no-repeat; text-indent: -10000px; border: none; padding: 0; top: 50%; }
        /*]]>*/-->
    </style>
</head>
<body>
    <form id="myform" name="myform" method="post" runat="server">
        <div id="dhtmltooltip"></div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
        </div>
        <div class="ektronPageContainer ektronPageInfo">
            <div class="ektronWindow" id="pleaseWait">
                <h3><asp:Literal ID="LoadingImg" runat="server" /></h3>
            </div>

            <label class="ektronCaption"><%= GetMessage("lbl Select XLIFF files")%></label>
            <div class="ektronTopSpace"></div>
            <asp:FileUpload ID="FileUpload0" runat="server" />
            <div class="ektronTopSpaceSmall"></div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <div class="ektronTopSpaceSmall"></div>
            <asp:FileUpload ID="FileUpload2" runat="server" />
            <div class="ektronTopSpaceSmall"></div>
            <asp:FileUpload ID="FileUpload3" runat="server" />
            <div class="ektronTopSpaceSmall"></div>
            <asp:FileUpload ID="FileUpload4" runat="server" />
            <div class="ektronTopSpaceSmall"></div>
            <asp:Label ID="FileUploadLabel0" CssClass="important" runat="server" />
            <asp:Label ID="FileUploadLabel1" CssClass="important" runat="server" />
            <asp:Label ID="FileUploadLabel2" CssClass="important" runat="server" />
            <asp:Label ID="FileUploadLabel3" CssClass="important" runat="server" />
            <asp:Label ID="FileUploadLabel4" CssClass="important" runat="server" />

            <div class="ektronTopSpace"></div>
            <div class="ektronHeader"><%= GetMessage("lbl generic History")%></div>
            
            <div class="ektronBorder">
                <iframe src="localizationjobs.aspx" height="360" width="100%"></iframe>
            </div>            
        </div>
    </form>
</body>
</html>
