<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.LinkManager" AutoEventWireUp="false" CodeBehind="LinkManager.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" TagName="LinkManagerControl" Src="../Controls/LinkManagerControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ OutputCache Duration="600" VaryByParam="Language;SkinPath" %>

<div id="MainContainer">
    <div class="Ektron_DialogTabstrip_Container">
	    <telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainContainer">
		    <telerik:tab elementid="TabbedHyperlink" onclientclick="SetLinkVariant('link');" selected="True" text="<script>localization.showText('Tab1HeaderText');</script>" />
		    <telerik:tab elementid="TabbedAnchor" onclientclick="SetLinkVariant('anchor')" selected="False" text="<script>localization.showText('Tab2HeaderText');</script>" />
		    <telerik:tab elementid="TabbedEmail" onclientclick="SetLinkVariant('email');" selected="False" text="<script>localization.showText('Tab3HeaderText');</script>" />
	    </telerik:tabcontrol>
    </div>

    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <telerik:LinkManagerControl	id="theLinkManagerControl" runat="server"/>
    </div> 
</div>   

<script language="javascript">
<!--
	function SetLinkVariant(linkVariant)
	{
		<%=this.theLinkManagerControl.ClientID%>.SetLinkVariant(linkVariant);
	}

	function InitControl()
	{
		var args = GetDialogArguments();
		var control = <%=this.theLinkManagerControl.ClientID%>;
		control.Initialize(args);
		control.OnOkClicked = OnLinkOkClicked;
		control.OnCancelClicked = OnLinkCancelClicked;
		
		window.focus();//Mozilla under Mac!
		//ektron start - for accessibility
		$ektron("td.TabCenterSelected").attr("tabindex", "0").keydown(function(e){
		    if (13 == e.keyCode)
		    {
		        $ektron(this).closest("table").click();
		    }
		}).focus();  
		$ektron("td.TabCenter").attr("tabindex", "0").keydown(function(e){
		    if (13 == e.keyCode)
		    {
		        $ektron(this).closest("table").click();
		    }
		}); 
		//$ektron("table.TabSelected").eq(0).focus(); 
		//ektron end
	}

	function OnLinkOkClicked()
	{
        //Ektron Editor starts
        var bCallCloseDlg = true;
        var LinkObj = <%=this.theLinkManagerControl.ClientID%>.GetModifiedLinkObject();
        if (LinkObj.errMsg.length > 0)
        {
            alert(LinkObj.errMsg);
            bCallCloseDlg = false;
        }
        var paramList = EkUtil_parseQuery();
        if(typeof paramList.AccessChecks != "undefined")
        {
            var AccessChecks = paramList.AccessChecks.toLowerCase();
            if ("link" == <%=this.theLinkManagerControl.ClientID%>.LinkVariant && AccessChecks != "none") // hyperlink tab, not anchor, not email
            {
                if ("" == LinkObj.title) // Accessibility Required field(s) is blank.
                {
                    switch (AccessChecks)
                    {
                        case "warn":
                            bCallCloseDlg = true;
                            if (confirm(localization.getText("AccessChecksWarn")))
                            {
                                bCallCloseDlg = false;
                                //eval(document.<%=this.theLinkManagerControl.ClientID%>_.setfocus());
                            }
                            break;
                        case "enforce":
                            alert(localization.getText("AccessChecksEnforce"));
                            bCallCloseDlg = false;
                            break;
                        case "none":
                        default:
				            bCallCloseDlg = true;
				            break;
				    }
		        }
	        }    
	    }  
	    if (true == bCallCloseDlg)
	    {
		    CloseDlg(LinkObj);//CloseDlg(<%=this.theLinkManagerControl.ClientID%>.GetModifiedLinkObject());
	    }
	    //Ektron Editor ends
	}

	function OnLinkCancelClicked()
	{
		CloseDlg();
	}

	AttachEvent(window, "load", InitControl);
	
//-->	
</script>