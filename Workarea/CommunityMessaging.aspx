<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CommunityMessaging.aspx.cs" Inherits="CommunityMessaging" ValidateRequest="false"  %>
<%@ Reference Control="Controls/Community/Messaging/ViewMessages.ascx" %>
<%@ Reference Control="Controls/Community/Messaging/ViewMessage.ascx" %>
<%@ Reference Control="Controls/Community/Messaging/EditMessage.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title> 
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <style type="text/css">
    	.ViewMsgContainer
		{
			font-size: 12px;
			padding: .5em;
		}

		.ViewMsgSubject
		{
		    /*validation(CSS:2.0)word-wrap is not a known css property name
			word-wrap: break-word;*/
			overflow: hidden;
			-moz-binding: url('csslib/xml/moz_wordwrap.xml#wordwrap'); /* add support for FireFox 2.0 */
		}
		.ViewMsgDate
		{
			float: right;
		}

		.ViewMsgHR
		{
			color: #2E6E9E; /* sets color for IE */
			background-color: #2E6E9E; /* sets color for other browsers */
			height: 1px;
			margin: .5em 0;
			padding: 0;
		}
		.ViewMsgMessage
		{
			overflow: auto;
		}

		.td_RecTargetTargetUI_userSel{}
		.RecTargetTargetUI_userSelImg{}
		.td_MessageTargetUI{}

		/* Styling for the Browse Members/Friends elements */
		.EktMsgTargetCtl{
			font-size: 12px;
			height: 100%;
			overflow: auto;
			position: relative;
			top: 0px;
			left: 0px;
			background-color: white;
		}

		.newMessage {display: none;}
		.messageSpacer {height: 400px;}
		.messageUILoading {position: absolute; width: 128px; height: 128px; top: 50%; left: 50%; margin-top: -64px; margin-left: -64px; background-image: url('images/application/loading_big.gif')}
        .checkBoxColumn {width: 2em;}
       
    </style>
    <asp:literal id="msgJSContainer" runat="server" />

    <script type="text/javascript">
    <!--/*--><![CDATA[/*><!--*/

    function MsgInboxToggleAllCB(cb) {
	    var idx, conObj = document.getElementById("msgCtlContainer");
	    if (conObj){
		    var inObjs = conObj.getElementsByTagName("input");
		    for (idx = 0; idx < inObjs.length; idx++){
			    if (cb != inObjs[idx]){
				    inObjs[idx].checked = cb.checked;
				    MsgInboxToggleCB(inObjs[idx])
			    }
		    }
	    }
    }

    function MsgInboxToggleCB(cb) {
	    var idx;
	    var hdnObj = document.getElementById("MsgInboxSelCBHdn");
	    if (hdnObj){
		    var str = hdnObj.value;
		    if (cb.checked){
			    // add:
			    if (hdnObj.value.indexOf(cb.value) < 0){
				    if (hdnObj.value.length > 0){
					    hdnObj.value += ",";
				    }
				    hdnObj.value += cb.value;
			    }
		    }
		    else {
			    // remove:
			    var items = str.split(",");
			    hdnObj.value = "";
			    for (idx=0; idx < items.length; idx++){
				    if (cb.value != items[idx]){
					    if (hdnObj.value.length > 0){
						    hdnObj.value += ",";
					    }
					    hdnObj.value += items[idx];
				    }
			    }
		    }
	    }
    }

    function SubmitSendMessage(){
	    var hdbObj = document.getElementById('ektouserid');
	    if (hdbObj && (hdbObj.value.length > 0)){
		    hdbObj = document.getElementById('hdnRecipientsValidated');
		    if (hdbObj){
			    hdbObj.value = "1";
			    document.form1.submit();
		    }
	    }
	    else {
		    alert(js_no_items_selected);
	    }
    }

    function DelSelMsgs(viewSentFlag){
	    var hdnObj = document.getElementById("MsgInboxSelCBHdn");
	    if (hdnObj && (hdnObj.value.length > 0)){
		    if (confirm(js_remove_sel_items_confirm)){
			    if (viewSentFlag){
				    document.form1.action = "CommunityMessaging.aspx?action=viewallsent&mode=del";
			    }
			    else
			    {
				    document.form1.action = "CommunityMessaging.aspx?action=viewall&mode=del";
			    }
			    document.form1.submit();
		    }
	    }
	    else {
		    alert(js_no_items_selected);
	    }
    }

    // every time we load this page, we need to refresh the header
    // to update the unread messages count
    Ektron.ready(function()
        {
            if (top.frames['workareatop'] && Ektron.QueryString["action"] == "viewmsg")
            {
                top.frames['workareatop'].location.reload();
            }
        }
    );
    /*]]>*/-->
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="msgCtlContainer">
		<asp:PlaceHolder ID="MsgCtlHolder" runat="server"></asp:PlaceHolder>
    </div>
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        var js_no_items_selected = '<%=m_refMsg.GetMessage("js:no items selected")%>';
        var js_remove_sel_items_confirm = '<%=m_refMsg.GetMessage("js remove sel items confirm")%>';
        //--><!]]>
    </script>
    </form>
</body>
</html>

