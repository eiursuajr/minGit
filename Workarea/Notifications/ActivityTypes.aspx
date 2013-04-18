<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ActivityTypes.aspx.cs" Inherits="Workarea_Notifications_ActivityTypes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Activity Types</title>
    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript" language="JavaScript">
     function SubmitForm(FormName,Validate)
     {
        resetCPostback();
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
   function resetCPostback()
   {
    document.forms["ActivityTypes"].isCPostData.value = "";
   }
   function VerifyAddActivityType() {
		var es = '' ;
		if(document.forms.ActivityTypes.txtName.value=='') {
			es+= '<asp:Literal id="ltr_nameErr" runat="server" />\n' ;
		}
		if(document.forms.ActivityTypes.ddlActionScope.value=='None'){
			es += '<asp:Literal id="ltr_scopeErr" runat="server" />\n' ;
		}
		if(es!='') {
			alert('<asp:Literal id="ltr_follErr" runat="server" />'  + es) ; return false;
		}
		else {
			return true ;
		}
	}
   function ConfirmDelete() 
    {
			
			return (confirm('<asp:literal id="delActivityTypeMsg" runat="server"/>'));
    }
    </script>

</head>
<body>
    <form id="ActivityTypes" runat="server">
        <div class="ektronPageHeader" id="ektronPageHeader" runat="server">
            <div class="ektronTitlebar" id="divTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="divToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer">
            <div id="dvAddNewActivityType" runat="server">
                <div class="ektronPageInfo">
                    <table class="ektronGrid">
                        <tr>
                            <td class="label" title="Name">
                                <%=msgHelper.GetMessage("generic name")%>
                                :</td>
                            <td class="value">
                                <asp:TextBox ToolTip="Enter Name here" ID="txtName" runat="server" MaxLength="255" /></td>
                        </tr>
                        <tr id="rowObjecttype" runat="server">
                            <td class="label" title="Object Type">
                                <%=msgHelper.GetMessage("lbl object type")%>
                                :
                            </td>
                            <td class="value">
                                <asp:DropDownList ID="ddlType"  runat="server" ToolTip="Select Object Type from Drop Down Menu">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="rowActiontype" runat="server">
                            <td class="label" title="Action Type">
                                <%=msgHelper.GetMessage("lbl action type")%>
                                :
                            </td>
                            <td class="value">
                                <asp:DropDownList ID="ddlActionType"  runat="server" ToolTip="Select Action Type from Drop Down Menu">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" title="Action Scope">
                                <%=msgHelper.GetMessage("lbl action scope")%>
                                :
                            </td>
                            <td class="value">
                                <asp:DropDownList ID="ddlActionScope"  runat="server" ToolTip="Select Action Scope from Drop Down Menu">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="dvActivityGrid" runat="server">
                <div>
                    <asp:GridView ID="ActivityGrid" runat="server" AutoGenerateColumns="False" CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                    </asp:GridView>
                    <p class="pageLinks">
                        <asp:Label ToolTip="Page" runat="server" ID="PageLabel"><%=msgHelper.GetMessage("page lbl")%></asp:Label>
                        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                        <asp:Label ToolTip="of" runat="server" ID="OfLabel"> <%=msgHelper.GetMessage("lbl of")%></asp:Label>
                        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                        <input type="hidden" runat="server" name="hdnUnit" value="hidden" id="hdnUnit" />
                        <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
                    </p>
                    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                        OnCommand="NavigationLink_Click" CommandName="First" />
                    <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="PreviousPage1" Text="[Previous Page]"
                        OnCommand="NavigationLink_Click" CommandName="Prev" />
                    <asp:LinkButton ToolTip="NextPage" runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                        OnCommand="NavigationLink_Click" CommandName="Next" />
                    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                        OnCommand="NavigationLink_Click" CommandName="Last" />
                    <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
                </div>
            </div>
        </div>
        <input type="hidden" runat="server" id="isCPostData" value="false" />
    </form>
</body>
</html>

