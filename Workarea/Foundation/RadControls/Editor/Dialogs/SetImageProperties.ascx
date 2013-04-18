<%@ Control Language="c#" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.SetImageProperties" AutoEventWireUp="false" CodeBehind="SetImageProperties.ascx.cs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="ImageInfoControl" Src="../Controls/ImageInfoControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="ImagePropertiesControl" Src="../Controls/ImagePropertiesControl.ascx" %>
<%@ Register TagPrefix="telerik" TagName="ThumbnailCreator" Src="../Controls/ThumbnailCreator.ascx" %>
<%@ Register TagPrefix="telerik" TagName="TabControl" Src="../Controls/TabControl.ascx" %>

<div id="MainContainer">
    <div class="Ektron_DialogTabstrip_Container">
	    <telerik:tabcontrol id="TabHolder" runat="server" ResizeControlId="MainContainer">
		    <telerik:tab elementid="TabbedImageProperties" onclientclick="SetButtonsVisibility(true);" selected="True" text="<script>localization.showText('Tab1HeaderText')</script>" />				
		    <telerik:tab elementid="TabbedThumbnailCreator" onclientclick="SetButtonsVisibility(false);" text="<script>localization.showText('Tab3HeaderText')</script>" />
	    </telerik:tabcontrol>
    </div>

    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <table>
	        <tr>
		        <td class="MainTableContentCell" valign="top">
			        <telerik:ImagePropertiesControl id="theImagePropertiesControl" runat="server"/>
			        <telerik:ThumbnailCreator id="theThumbnailCreator" runat="server"/>
		        </td>
	        </tr>
	        <tr>
	            <td colspan="20" width="100%" align="center">
                <div class="Ektron_TopSpaceSmall"></div>
                <div class="Ektron_StandardLine"></div>
	            <!-- TODO: Ross - For some odd reason, setting the padding below is necessary for sizing the dialog -->
	            <div id="<%=this.ClientID%>_buttonsHolder" style="padding-bottom:27px">
	                <button title="Ok" class="Ektron_StandardButton" onclick="javascript:SubmitChanges();"><script>localization.showText('OK');</script></button>
	                &nbsp;
	                <button title="Cancel" class="Ektron_StandardButton" onclick="javascript:CloseDlg();"><script>localization.showText('Cancel');</script></button>
                </div>
	            </td>
	        </tr>
        </table>
    </div>
</div>

<script language="javascript">
    // Ektron starts
    // #33954: TabHolder1 is the ClientID of TabbedThumbnailCreator
    Ektron.ready(function()  {
        $ektron("#TabHolder1").hide();
    });
    // Ektron ends
	function InitControls()
	{
		var args = GetDialogArguments();
		if (args) //ektron code
		{
		    var imgPropertiesControl = <%=theImagePropertiesControl.ClientID%>;
		    imgPropertiesControl.Initialize(args.imageToModify, args.EditorObj, args.ColorsArray, args.CanAddCustomColors);
		    var theThumbnailCreator;
		    if (<%=theThumbnailCreator.Visible.ToString().ToLower()%>)
		    {
			    theThumbnailCreator = <%=theThumbnailCreator.ClientID%>;
			    theThumbnailCreator.Initialize(imgPropertiesControl.GetOriginalImage(), args.ThumbnailSuffix);
			    if (thumbnailCreatorErrorToken != "")
			    {
				    theThumbnailCreator.SetMessage(thumbnailCreatorErrorToken);
			    }
		    }
		    else
		    {
			    theThumbnailCreator = null;
		    }
		} //ektron code
	}

	function SubmitChanges()
	{
	    //Ektron Editor  starts
        var bCallCloseDlg = true;
        var ImgObj = <%=theImagePropertiesControl.ClientID%>; //this is a TableWizard JavaScript object.
        var paramList = EkUtil_parseQuery();
        if("undefined" != typeof paramList.AccessChecks)
        {
            var AccessChecks = paramList.AccessChecks.toLowerCase();
            if ("" == ImgObj.LongDescriptionHolder.value && "" == ImgObj.AltHolder.value && AccessChecks != "none")
            {
                switch(AccessChecks)
                {
                    case "warn":
                        bCallCloseDlg = true;
                        if (confirm("The content is not accessibility compliance. Would you like to correct it?"))//TODO:localization["AccessChecksWarn"]
                        {
                            bCallCloseDlg = false;
                        }
                        break;
                    case "enforce":
                        alert("Please fill in the accessibility compliance field(s)"); //TODO:localization["AccessChecksEnforce"]
                        bCallCloseDlg = false;
                        break;
                    case "none":
                    default:
                        bCallCloseDlg = true;
                        break;
                }
	            
	        }    
	    }  
	    if (true == bCallCloseDlg)
	    {
		CloseDlg(<%=theImagePropertiesControl.ClientID%>.GetUpdatedImage());
	    }
	    //Ektron Editor ends
	}
	
	function SetButtonsVisibility(Visibility)
	{
	}
	AttachEvent(window, "load", InitControls);
</script>