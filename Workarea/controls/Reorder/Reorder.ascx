<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Reorder.ascx.cs" Inherits="controls_Reorder_Reorder" %>
<input type="hidden" name="LinkOrder" id="LinkOrder" value="<%=ReOrderList%>"/>
<div style="padding:1em;">
    <table width="100%">
	<tr>
	    <td width="80%">
		    <select name="OrderList" id="OrderList" size="<%if (ItemList.Count < 20) Response.Write(ItemList.Count); else Response.Write("20");%>" style="margin:0;padding:0;width:100%;" <%Response.Write(GetDisabled());%> >
			<% 
			    Int32 intCounter = 0;
                for (Int32 i = 0; i < ItemList.Count; i++)
                {%>
					<option value="<%=ItemList[intCounter].Value%>" <%if (intCounter == 0) Response.Write("selected"); else Response.Write("");%>><%=ItemList[intCounter].Text%></option>
		        <%
                    intCounter++;   
                } %>
		    
		    </select>
		</td>
	    <td width="20%">
            <div class="ektronTopSpace">
                <a href="#" onclick="javascript:Move('up', document.getElementById('OrderList'), document.getElementById('LinkOrder'));">
                    <img style="cursor:pointer;" src="<%=AppPath%>Images/ui/icons/arrowHeadUp.png" border="0" width="26" height="17" alt="<%=GetMessage("move selection up msg")%>" title="<%=GetMessage("move selection up msg")%>" />
                </a><br />
                <a href="#" onclick="javascript:Move('dn', document.getElementById('OrderList'), document.getElementById('LinkOrder'));">
                    <img style="cursor:pointer;" src="<%=AppPath%>Images/ui/icons/arrowHeadDown.png" border="0" width="26" height="17" alt="<%=GetMessage("move selection down msg")%>" title="<%=GetMessage("move selection down msg")%>" />
                </a><br />
                <br />
            </div>
		</td>
	</tr>
</table>
</div>
