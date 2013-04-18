<%@ Page Language="C#" AutoEventWireup="true" CodeFile="attachments.aspx.cs" Inherits="threadeddisc_attachments"
    ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Attachments</title>
    <style type="text/css">
    	#htmToolBar{
    		display: none;
    	}
    	.ektronPageContainer{
    		top: 47px !important;
    	}
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data" method="post">
    <div class="ektronPageContainer">
        <table class="ektronPageGrid" style="width: 100%;">
            <tr>
                <td>
                    <table id="Table7" class="ektronPageGrid attachment-table" style="width: 100%;">
                        <tr>
                            <td id="txtTitleBar" class="ektronTitlebar forceTitlebar">
                                <span id="ektronTitlebar">
                                	<asp:Literal ID="ltr_addfile" runat="server" />
                                </span>
                                 <span style="display: none" id="_WorkareaTitlebar"></span>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <table class="ektronPageGrid" style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:FileUpload ID="ul_file" runat="server" TabIndex="-1" Width="100%" /><br />
                                            <asp:Literal ID="ltr_allowedext" runat="server" />

                                            <div style="visibility: hidden;" id="dvHoldMessage">
                                                <strong>
                                                    <asp:Literal ID="ltr_uploadmsg" runat="Server" /></strong>
                                            </div>
                                            <div style="visibility: hidden;" id="dvErrorMessage">
                                                <span class="important"><strong>
                                                    <asp:Literal ID="ltr_error" runat="Server" /></strong></span>
                                            </div>
                                        </td>
                                        <td width="32" align="right" style="vertical-align:top;">
                                            <asp:Button ID="cmd_attach" OnClick="cmd_attach_Click" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="Table2" class="ektronPageGrid  attachment-table" style="width: 100%;">
                        <tr>
                            <td id="Td1" colspan="2" class="ektronTitlebar forceTitlebar">
                                <span id="Span1"><asp:Literal ID="ltr_currentfiles" runat="Server" /></span>
                                <span style="display: none" id="Span2"></span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table class="ektronPageGrid" style="width: 100%;">
                                    <tr>
                                        <td>
                                            <fieldset>
                                                <table width="100%" class="ektronPageGrid">
                                                    <tr>
                                                        <td>
                                                            <div id="ek_filelist">
                                                                <asp:Literal ID="ltr_filelist" runat="server" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </td>
                                        <td width="32" align="right" valign="top">
                                            <asp:Button ID="cmd_remove" OnClick="cmd_remove_Click" runat="server" UseSubmitBehavior="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="cmd_close" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdn_ofilelist" runat="server" />
        <input type="hidden" id="hdn_ieval" name="hdn_ieval" value="" />

        <script language="javaScript" type="text/javascript">
            <asp:Literal ID="ltr_bottomjs" runat="server"/>
        </script>
    </div>
    </form>
</body>
</html>