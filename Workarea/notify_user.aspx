<%@ Page Language="C#" AutoEventWireup="true" Inherits="notify_user" CodeFile="notify_user.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>notify_user</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
    //hide drag drop uploader frame/////
	top.HideDragDropWindow();

	//The following code is duplicated in content/forms/library it is due when we work on V6.0
    if ((typeof(top["ek_nav_bottom"]["NavIframeContainer"]) != 'undefined')
        && (typeof(top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]) != 'undefined')
        && (typeof(top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["LibraryTree"])!='undefined')) {
	    var treeobj=top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["LibraryTree"];
	    if(treeobj.document.getElementById("selected_folder_id")!=null){
		    var SelectedTreeId=treeobj.document.getElementById("selected_folder_id").value;
		    var CurrentFolderId='<asp:Literal id="ltr_folderId" runat="server" />';
		    if(CurrentFolderId==0 && SelectedTreeId!=0) {
			    var stylenode = treeobj.document.getElementById( SelectedTreeId );
			    if(stylenode!=null){
				    stylenode.style["background"] = "#ffffff";
				    stylenode.style["color"] = "#000000";
				    var stylenode = treeobj.document.getElementById( CurrentFolderId );
				    if (stylenode != null)
				    {
					    stylenode.style["background"] = "#3366CC";
					    stylenode.style["color"] = "#ffffff";
				    }
			    }
		    }
	    }
	}
	////////////////////////////////////
	//--><!]]>
    </script>
  </head>
	<body>
	    <div class="ektronPageHeader">
	        <div class="ektronTitlebar">&nbsp;</div>
	    </div>
		<table style="width:100%; height:100%">
			<tr>
				<td class="label" id="notifyUser" style="text-align:center;" runat="server"></td>
			</tr>
		</table>
	</body>
</html>

