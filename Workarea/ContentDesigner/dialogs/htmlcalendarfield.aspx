﻿<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="htmlcalendarfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.HtmlCalendarField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValidateControl" Src="ucFieldValidation.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDataStyleControl" Src="ucFieldDataStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>
<%@ Register TagPrefix="ek" TagName="DatePicker" Src="../../controls/generic/date/DatePicker.ascx" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Calendar Field</title>
<style type="text/css">
.DatePickerContainer input.DatePicker_input
{
	width: 15em;
	margin-right: 4px;
}
</style>
</head>
<body class="dialog">
<form id="Form1" runat="server">   
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">

            <Tabs>
                <radTS:Tab ID="General" Text="General" Value="General" />
                <radTS:Tab ID="Validation" Text="Validation" Value="Validation" />
                <radTS:Tab ID="DataStyle" Text="Data Style" Value="DataStyle" />
            </Tabs>
        </radTS:RadTabStrip>
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
		            <ek:FieldNameControl ID="name" runat="server" IndexedEnabled="false" />
		            <tr>
		                <td><label title="Defaut value" for="txtDefVal" class="Ektron_StandardLabel" id="lblDefVal" runat="server">Default value:</label></td>
		                <td colspan="2">
							<ek:DatePicker ID="txtDefVal" LabelText="" runat="server" />
		                </td>
		            </tr>
		        </table>
	        </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server" >
                <table width="100%">
                    <ek:FieldValidateControl id="validateControl" runat="server" Enabled="true" />
                </table>  
            </radTS:PageView>
	        <radTS:PageView id="Pageview3" runat="server" >
                <table width="100%">
                    <ek:FieldDataStyleControl runat="server" />
                </table> 
            </radTS:PageView>
        </radTS:RadMultiPage>
	</div>
	
    <div class="Ektron_Dialogs_LineContainer">
        <div class="Ektron_TopSpaceSmall"></div>
        <div class="Ektron_StandardLine"></div>
    </div>		
	
	<ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />
</form> 
<script language="javascript" type="text/javascript">
<!--
    if ("undefined" == typeof RadTabStrip1) RadTabStrip1 = <%= RadTabStrip1.ClientID %>;
    RadTabStrip1.ClientID = "<%= RadTabStrip1.ClientID %>";

	var ResourceText = 
	{
	};

    var g_skinPath = "<%=ResolveUrl(this.SkinControlsPath)%>ContentDesigner/";
    
   Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
//-->
</script>
</body>
</html>
