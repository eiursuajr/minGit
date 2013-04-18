<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LocalizationJobs.aspx.cs" Inherits="MSLocalization_jobs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Shipping Methods</title>
    <asp:literal id="ltr_js" runat="server" />
    <style type="text/css">
        form {position:relative;margin-top:-1px;}
        div.ektronExportHandoffModal {width: 70em !important; margin-left: -25em;}
        .ektronExportHandoffModal iframe {width: 100%; height: 30em !important;}
        div.ektronModalStandard div.ektronModalBody {padding: 0 !important;}
        
        .my_clip_button { color: #5D8FB4; outline: medium none; text-decoration:none; width:150px; cursor:default; }
		.my_clip_button.hover { color: #5D8FB4; outline: medium none; text-decoration:underline; }
		.my_clip_button.active { color: #5D8FB4; outline: medium none; text-decoration:none; }
    </style>
    <script type="text/javascript" language="JavaScript">
       var clip = null;

       function initCopyFilePaths() {
           if (FlashDetect.installed) {
               clip = new ZeroClipboard.Client();
               clip.setHandCursor(true);
               clip.topOffSet = 10;

               clip.addEventListener('mouseOver', function(client) {
                   // update the text on mouse over
                   clip.setText($ektron('#hdnFilePaths').val());
               });

               clip.glue('d_clip_button', 'd_clip_container');
           }
           else {
               $ektron('#pnlCopy').remove();
           }
		}		
    </script>
</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
        <div class="ektronPageInfo">
            <asp:Panel ID="pnl_error" runat="server" Visible="false">
                <div id="status" class="ui-widget">
                    <div class="ui-state-highlight ui-corner-all" style="padding: 0 0.7em; margin-top: 20px;"> 
                        <span class="ui-icon ui-icon-info" style="float: left; margin-right: 0.3em;"></span>
                            <asp:Literal ID="ltr_Error" runat="server"></asp:Literal>
                    </div>
                    <div class="spacer5em"></div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_viewjob" runat="Server">
	            <table id="Table1" class="ektronGrid" runat="server">
                    <tr>
                        <td class="label">Status:</td>
                        <td class="value"><asp:Literal ID="ltr_JobStatus" runat="server" /></td>
                    </tr>
                </table>
                <asp:Panel ID="pnlCopy" runat="server" Visible="false">
	                &#160;&#160;&#160;&#160;&#160;&#160;
	                <span id="d_clip_container">
		                <span id="d_clip_button" class="my_clip_button">Copy file paths to clipboard</span>
	                </span>
	            </asp:Panel>
                <asp:DataGrid ID="dg_viewjob"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <a target="_blank" href="<%=SitePath %>uploadedfiles/localization/<%# DataBinder.Eval(Container, "DataItem.FileUrl") %>"><img src="<%=AppPath %>images/UI/Icons/FileTypes/zip.png"/></a>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="File">
                            <ItemTemplate>
                                <a target="_blank" href="<%=SitePath %>uploadedfiles/localization/<%# DataBinder.Eval(Container, "DataItem.FileUrl") %>"><%# DataBinder.Eval(Container, "DataItem.FileName") %></a>&#160;&#160;
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:HiddenField ID="hdnFilePaths" runat="server" />
                <asp:Repeater runat="server" ID="rptNotReadyContent" Visible="false">
                    <HeaderTemplate>
                        <table class="ektronGrid" cellspacing="0" rules="all" border="1" style="border-collapse: collapse; display: block;">
	                    <thead>
	                    <tr class="title-header">
                           <%-- <th> </th>--%>
		                    <th class="title-header"><%= GetMessage("generic title")%></th>
		                    <th class="title-header center"><%= GetMessage("generic ID")%></th>
		                    <th class="title-header"><%= GetMessage("generic date modified")%></th>
	                    </tr>
	                    </thead>
	                    <tbody>
					</HeaderTemplate>
                    <ItemTemplate>
                        <tr>
		                   <%-- <td><input type="checkbox" id="dependency_<%# Eval("Id") %>" name="dependency_<%# Eval("Id") %>" /></td>--%>
		                    <td class="left"><%# Util_GetItemIcon((Ektron.Cms.BusinessObjects.Localization.ILocalizable)Container.DataItem) %>&#160;<%# Eval("Title") %></td>
		                    <td class="center"><%# Eval("Id") %></td>
		                    <td class="left"><%# Eval("DateModified") %></td>
	                    </tr>
                    </ItemTemplate>
                    <FooterTemplate>
						</tbody>
						</table>
					</FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

