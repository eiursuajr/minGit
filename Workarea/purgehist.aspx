<%@ Page Language="C#" AutoEventWireup="true" Inherits="purgehist" CodeFile="purgehist.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title><%=m_refAPI.AppName%> Purge History</title>
		<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
	    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	    <meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE" />
	    <meta content="JavaScript" name="vs_defaultClientScript" />
	    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />	    	    
	    <asp:literal id="StyleSheetJS" runat="server" />
	    
	    <script type="text/javascript">
	        <!--//--><![CDATA[//><!--
		    function SubmitForm()
		    {		 
			    var dtPurge;
			    var blnEmptyDate=false;
			    var blnAnswer;

			    dtPurge=document.getElementById("frm_purge_date");

			    if (("object"==typeof(dtPurge)) && (dtPurge!= null))
			    {	
				    if ((dtPurge.value == null) || (dtPurge.value == ""))
				    {
					    blnEmptyDate=true;		
				    }				 
			    }		 
    					
			    if (blnEmptyDate==true) {
			        var NoDateMsg = "<asp:Literal runat='server' ID='ltr_NodateMsg' /> ";
			        window.alert(NoDateMsg);
				    return false;				
			    }
    		 
			    document.forms[0].submit();
		    }
		    //--><!]]>
	    </script>	
	    <style type="text/css">
	        <!--/*--><![CDATA[/*><!--*/
	        .ektronGrid span.ektronTextSmall { width:200px; overflow: hidden;}
	        /*]]>*/-->
	    </style>	    
	</head>
	<body>
		<form id="frmMain" method="post" runat="server">
            <div id="dhtmltooltip"></div>
			<div class="ektronPageHeader">
			    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
			    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
			</div>
            <div class="ektronPageContainer ektronPageInfo">
			    <table class="ektronGrid">
				    <tr>
					    <td class="label"><asp:Label id="lblPurgeForFolder" ToolTip="Purge content history for folder"  Text="Purge content history for folder:" runat="server" /></td>
					    <td class="value"><asp:Literal id="litFolderName" runat="server" /></td>
				    </tr>
				    <tr>
					    <td class="label"><asp:Label id="lblFolderPath" ToolTip="Folder Path" Text="Folder Path:" runat="server" /></td>
					    <td class="value"><asp:Literal id="litFolderPath" runat="server" /></td>
				    </tr>
				    <tr>
					    <td class="label"><asp:Label id="lblPurgeDate" ToolTip="Only purge historical versions before" Text="Only purge historical versions before:" runat="server" /></td>
						<td class="value"><asp:Literal id="litCal" runat="server" /></td>
				    </tr>
				</table>
				
				<asp:CheckBox ToolTip="Recursive Purge Option" id="chkRecursivePurge" Text="Recursive Purge" runat="server" />
				<br />
				<asp:CheckBox ToolTip='Purge versions marked as "Published" Option' id="chkPurgePublished" Text='Purge versions marked as "Published"' runat="server" />
			</div>
			
			<input id="frm_folder_id" type="hidden" runat="server" />
	    </form>
	</body>
</html>

