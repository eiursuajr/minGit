<%@ Page Language="cs" AutoEventWireup="false" CodeFile="ContentFallback.aspx.cs" Inherits="Localization_Content_Fallback" %>
<%@ Register src="../controls/Reorder/Reorder.ascx" tagname="Reorder" tagprefix="workarea" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
    <title>Content Fallback</title>
    <asp:literal id="ltr_js" runat="server" />
    <script type="text/javascript">
        function RemoveLocale() {
            var selectedVal = $ektron('#OrderList :selected').val();
            var selectedText = $ektron('#OrderList :selected').text();
            if (selectedVal != '' && selectedText != '') {
                $ektron("#OrderList option[value='" + selectedVal + "']").remove();
                if (selectedVal != "0|0") {
                    $ektron("#lstLocales").append('<option value="' + selectedVal + '">' + selectedText + '</option>');
                }
                $ektron('#OrderList').attr('size', 4);
                $ektron('#lstLocales').attr('size', 4);
                SaveReorder(document.getElementById('OrderList'), document.getElementById('LinkOrder'));
            }
        }
        function AddLocale() {
            var selectedVal = $ektron('#lstLocales :selected').val();
            var selectedText = $ektron('#lstLocales :selected').text();
            if (selectedVal != '' && selectedText != '') {
                $ektron("#lstLocales option[value='" + selectedVal + "']").remove();
                $ektron("#OrderList").append('<option value="' + selectedVal + '">' + selectedText + '</option>');
                $ektron('#OrderList').attr('size', 4);
                $ektron('#lstLocales').attr('size', 4);
                SaveReorder(document.getElementById('OrderList'), document.getElementById('LinkOrder'));
            }
        }
        function AddEndPoint() {
            var selectedVal = "0|0";
            var selectedText = "- [-]"
            $ektron("#lstLocales option[value='" + selectedVal + "']").remove();
            $ektron("#OrderList").append('<option value="' + selectedVal + '">' + selectedText + '</option>');
            $ektron('#OrderList').attr('size', 4);
            SaveReorder(document.getElementById('OrderList'), document.getElementById('LinkOrder'));
        }
        function UseGlobal(glbl) {
            if (glbl)
            {
                $ektron('#OrderList').attr('disabled', 'disabled');
                $ektron('#lstLocales').attr('disabled', 'disabled');
            }
            else{
                $ektron('#OrderList').removeAttr('disabled');
                $ektron('#lstLocales').removeAttr('disabled');
            }
        }
    </script>
    <style type="text/css">
        .ektronPageInfo { position: inline; }
    </style>
</head>
<body onclick="MenuUtil.hide()">
    <form id="form1" runat="server">
        <div class="ektronPageContainer ektronPageGrid" >
			<div class="ektronPageInfo">
				<br />
				<asp:CheckBox ID="chkGlobalFallback" runat="server" onclick="UseGlobal(this.checked);" />Use Global Fallback
				<hr />Selected Locales:<workarea:Reorder ID="Reorder1" runat="server" />
				Available Locales:
				<img height="17" border="0" width="26" src="../Images/ui/icons/arrowHeadUP.png" style="cursor: pointer;" onclick="AddLocale();"/>&#160;&#160;<img height="17" border="0" width="26" src="../Images/ui/icons/arrowHeadDown.png" style="cursor: pointer;" onclick="RemoveLocale();"/>
				
				&#160;&#160;<img border="0" src="../Images/ui/icons/translationNotTranslatable.png" style="cursor: pointer;" onclick="AddEndPoint();"/>&#160;&#160;Add Endpoint
				
				<div style="padding: 1em;">
					<table width="100%">
					<tbody><tr>
						<td width="80%">
							<asp:Listbox ID="lstLocales" runat="server" mu>
							</asp:Listbox>
						</td>
						<td width="20%">
							<div class="ektronTopSpace">
								&#160;
							</div>
						</td>
					</tr>
				</tbody></table>
				</div>
			</div>
        <div>
    </form>
</body>
</html>
