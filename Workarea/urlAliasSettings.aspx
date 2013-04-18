<%@ Page Language="C#" AutoEventWireup="true" CodeFile="urlAliasSettings.aspx.cs"
    Inherits="Workarea_urlAliasSettings_" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>

    <asp:literal id="StyleSheetJS" runat="server" />

    <script type="text/javascript" language="JavaScript">
    Ektron.ready(function()
    {
        $ektron("#ConfirmDialog").modal({
            modal: true,
            overlay: 0,
            trigger: ".launchConfirmModal",
            onShow: function(hash) {
                hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2));
			    hash.o.fadeTo("fast", 0.5, function() {
				    hash.w.fadeIn("fast");
			    });
            },
            onHide: function(hash) {
                hash.w.fadeOut("fast");
			    hash.o.fadeOut("fast", function() {
				    if (hash.o)
				    {
					    hash.o.remove();
			        }
			    });
            }
        });
     });
    
     function SubmitForm(FormName) 
     {
		document.forms[FormName].submit();
        return false;
	 }
	 function CloseConfirmModal()
	 {
	   $ektron("#ConfirmDialog").modalHide();
	   return false;
	 }
    </script>

    <style type="text/css">
            body {font-size: 75%;}            
     </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Modal Dialog: Confirm -->
        <div class="ektronWindow ektronModalStandard" id="ConfirmDialog">
            <div class="ektronModalHeader">
                <h3 title="Url Alias Settings Confirmation">
                     <%=msgHelper.GetMessage("title url setting confirm")%>
                    <asp:HyperLink ToolTip="Close" ID="closeDialogLink2" CssClass="ektronModalClose" runat="server" /></h3>
            </div>
            <div class="ektronModalBody">
                <p class="messages">
                    <%=msgHelper.GetMessage("js url settings confirm msg")%></p>
                <ul class="ektronModalButtonWrapper clearfix">
                    <li>
                        <asp:LinkButton ToolTip="Cancel" ID="btnConfirmCancel" runat="server" CssClass="redHover button cancelButton buttonRight" /></li>
                     <li>
                        <asp:LinkButton ToolTip="Ok" ID="btnConfirmOk" runat="server"  CssClass="greenHover button okButton buttonRight" /></li>
                </ul>
            </div>
        </div>
        <!-- Modal Dialog: Confirm End -->
        
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>        
        <div class="ektronPageContainer ektronPageInfo">
            <table class="ektronGrid" width="100%">
                <tr class="title-header">
                    <td title="Types"><%=msgHelper.GetMessage("lbl types")%></td>
                    <td title="Enabled"><%=msgHelper.GetMessage("enabled")%></td>
                    <td title="Caching"><%=msgHelper.GetMessage("lbl caching")%></td>
                    <td title="Cache Size"><%=msgHelper.GetMessage("lbl cache size")%></td>
                    <td title="Cache Duration(Seconds)"><%=msgHelper.GetMessage("lbl duration")%></td>
                </tr>
                <tr>
                    <td class="label left"><asp:Label ID="lblManualChkbox" Text="Manual" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblManualAliasOnOff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Enable Manual Alias" ID="chkManualAlias" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblCachingonoff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Manual Caching" ID="chkCaching" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter Manual Cache Size here" ID="txtManualCachesize" Width="50px" Text="" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter Manual Cache Duration in seconds here" ID="txtManualCacheDuration" Width="50px" Text="" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label left"><asp:Label ID="lblAutoChkbox" Text="Automatic" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblAutoAliasOnOff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Enable Auto Alias" ID="chkAutoAlias" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblAutoCachingonoff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Enable Auto Caching" ID="chkAutoCaching" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter Auto Cache Size here" ID="txtCacheSize" Width="50px" Text="" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter Auto Cache Duration in seconds here" ID="txtAutoCacheDuration" Width="50px" Text="" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label left"><asp:Label ID="lblRegExp" Text="RegEx" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblRegExOnOff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Enable RegEx Alias" ID="chkRegEx" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblRegExCachingonoff" Text="Off" runat="server" />
                        <asp:CheckBox ToolTip="RegEx Caching" ID="chkRegExCaching" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter RegEx Cache Size here" ID="txtRegExCacheSize" Width="50px" Text="" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter RegEx Cache Duration in seconds here" ID="txtRegExCacheDuration" Width="50px" Text="" runat="server" />
                    </td>
                </tr>   
                 <tr>
                    <td class="label left"><asp:Label ID="lblCommunity" Text="Community" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblCommunityOnOff" Text="Off" runat="server" />
                        <asp:CheckBox ToolTip="Enable Community Alias" ID="chkCommunity" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblCommunityCachingonoff" Text="Off" runat="server" />
                        <asp:CheckBox ToolTip="Community Caching" ID="chkCommunityCaching" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter Community Cache Size here" ID="txtCommunityCacheSize" Width="50px" Text="" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ToolTip="Enter Community Cache Duration in seconds here" ID="txtCommunityCacheDuration" Width="50px" Text="" runat="server" />
                    </td>
                </tr>                
            </table>
            <div class="ektronTopSpace"></div>
            <table class="ektronForm">
                <tr id="tr_defaultPage" visible="false" runat="server">
                    <td class="label"><asp:Label ID="lblDefaultPage" Text="" runat="server" /></td>
                    <td class="value"><asp:TextBox ToolTip="Enter Default Page here" ID="txtDefaultPage" Text="Default.aspx" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblExt" Text="" runat="server" />:</td>
                    <td class="value"><asp:TextBox ToolTip="Enter Extension here" MaxLength="250" ID="txtExt" Text="" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblOverrideTemplate" Text="" runat="server" />:</td>
                    <td class="readOnlyValue">
                        <asp:Label ID="lblOverrideTemplateOnOff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Override Template Option" ID="chkOverrideTemplate" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblOverrideLanguage" Text="" runat="server" />:
                    </td>
                    <td class="readOnlyValue">
                        <asp:Label ID="lblOverrideLanguageOnOff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Override Language Option" ID="chkOverrideLanguage" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label"><asp:Label ID="lblQueryStringAction" Text="" runat="server" />:</td>
                    <td class="value">
                        <asp:DropDownList ToolTip="Select Query String Action from the Drop Down Menu" ID="ddlQueryStringAction"  runat="server" >
                            <asp:ListItem Text="None" Value="0" />
                            <asp:ListItem Text="Replace All Parameters within Alias" Value="1" />
                            <asp:ListItem Text="Append Parameters to Alias" Value="2" />
                            <asp:ListItem Text="Resolve Matched Parameters within Alias" Value="3" />
                        </asp:DropDownList>  
                    </td>
                </tr>
                <tr id="TracingRow" runat="server">
                    <td class="label">
                        <asp:Label ID="lblTracing" Text="Tracing" runat="server" />:</td>
                    <td class="value">
                        <asp:Label ID="lblTracingOnOff" Text="" runat="server" />
                        <asp:CheckBox ToolTip="Tracing Option" ID="chkTracing" runat="server" />
                    </td>
                </tr>
            </table>
            <table class="ektronGrid">
                <tr>
                    <td>
                        <fieldset>
                            <legend title="Quicklink Cache"><%=msgHelper.GetMessage("quicklink cache")%></legend>
                            <table class="ektronGrid">
                                <tr class="title-header">
                                    <td title="Types">
                                        <%=msgHelper.GetMessage("lbl types")%>
                                    </td>
                                    <td title="Enabled">
                                        <%=msgHelper.GetMessage("enabled")%>
                                    </td>
                                    <td title="Cache Size">
                                        <%=msgHelper.GetMessage("lbl cache size")%>
                                    </td>
                                    <td title="Cache Duration (Seconds)">
                                        <%=msgHelper.GetMessage("lbl duration")%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label left">
                                        <asp:Label ID="lblNonAliasCache" runat="server" /></td>
                                    <td>
                                        <asp:Label ID="lblNonAliasonoff" Text="Off" runat="server" />
                                        <asp:CheckBox ToolTip="Non Alias" ID="chkNonAlias" runat="server" />
                                    </td>
                                    <td>
                                        <asp:TextBox ToolTip="Enter Cache Size here" ID="txtNonAliasCacheSize" Width="50px" Text="" runat="server" />
                                    </td>
                                    <td>
                                        <asp:TextBox ToolTip="Enter Cache Duration in seconds here" ID="txtNonAliasCacheDuration" Width="50px" Text="" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

