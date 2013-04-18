<%@ Control Language="C#" AutoEventWireup="true" CodeFile="editvirtualstaging.ascx.cs" Inherits="editvirtualstaging" %>
<script language="javascript">
	<!--
		var UniqueID="<asp:literal id="jsUniqueID" runat="server"/>_";
		var jsContentLanguage="<asp:literal id="jsContentLanguage" runat="server"/>";
		
		function VerifyForm() {
		    if (document.getElementById("Password").value != document.getElementById("ConfirmPassword").value)
		    {
			    alert('<%=m_refMsg.GetMessage("js: alert user cannot confirm password")%>');
			    document.virtualstaging.Password.focus();
			    return false;
	        }
	        
	        //debugger;
	        var path = document.getElementById("asset_loc");
	        var strAsset = "assets";
	        var strAsset1 = "assets/";
	        var strAsset2 = "assets\\";
	        if ((endsWith(path.value.toLowerCase(), strAsset) == false) && (endsWith(path.value.toLowerCase(), strAsset1) == false) && (endsWith(path.value.toLowerCase(), strAsset2) == false)) {
			    alert('<%=m_refMsg.GetMessage("js: alert path must end in assets")%>');
			    path.focus();
		        return false;
	        }
	    
	        path = document.getElementById("private_asset_loc");
	        strAsset = "privateassets";
	        strAsset1 = "privateassets/";
	        strAsset2 = "privateassets\\";
	    
	        if ((endsWith(path.value.toLowerCase(), strAsset) == false) && (endsWith(path.value.toLowerCase(), strAsset1) == false) && (endsWith(path.value.toLowerCase(), strAsset2) == false)) {
			    alert('<%=m_refMsg.GetMessage("js: alert path must end in private assets")%>');
		        path.focus();
		        return false;
	        }
	        
			return true;
		}
		
		function endsWith(path, strAsset)
		{
            if (path.length - strAsset.length < 0) 
		        return false;
	        return (path.lastIndexOf(strAsset) == path.length - strAsset.length);
	    }
		
		
		function SubmitForm(FormName, Validate) {
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[FormName].submit();
					return false;
				}
				else {
					return false;
				}
			}
			else {
				document.forms[FormName].submit();
				return false;
			}
		}
	
	//-->
</script>
<table>
	<tr>
		<td class="info" title="Asset Location"><%=m_refMsg.GetMessage("asset location")%></td>
		<td>&nbsp;</td>
		<td id="td_asset_loc" runat="server"></td>
	</tr>
	<tr>
		<td class="info" title="Private Asset Location"><%=m_refMsg.GetMessage("private asset location")%></td>
		<td>&nbsp;</td>
		<td id="td_private_asset_loc" runat="server"></td>
	</tr>
	
	<tr>
		<td class="info" title="Domain/User Name"><%=m_refMsg.GetMessage("lbl domain username")%></td>
		<td>&nbsp;</td>
		<td id="td_DomainUserName" runat="server"></td>
	</tr>
	<tr>
		<td class="info" title="Password"><%=m_refMsg.GetMessage("lbl password")%></td>
		<td>&nbsp;</td>
		<td id="td_Password" runat="server"></td>
	</tr>

	<tr>
		<td class="info" title="Confirm Password"><%=m_refMsg.GetMessage("lbl confirm password")%></td>
		<td>&nbsp;</td>
		<td id="td_ConfirmPassword" runat="server"></td>
	</tr>
</table>