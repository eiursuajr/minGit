<%@ Control Language="c#" AutoEventWireUp="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="PageProperties.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorDialogControls.PageProperties" %>
<%@ Register TagPrefix="telerik" TagName="ColorPicker" Src="../Controls/ColorPicker.ascx"%>
<%@ Register TagPrefix="telerik" TagName="CssClassSelector" Src="../Controls/CssClassSelector.ascx"%>
<%@ Register TagPrefix="telerik" TagName="ImageDialogCaller" Src="../Controls/ImageDialogCaller.ascx"%>

<div class="Ektron_Dialog_Body_Container">
    <table>
        <tr>
            <td valign="top">
	            <fieldset>
		            <legend>				
			            <script>localization.showText('HTMLAttributes');</script>
		            </legend>
	                <div title="Page Title" class="Ektron_TopLabel"><script>localization.showText('PageTitle');</script></div>
		            <input title="Enter Page Title Text" type="text" id="TitleBox" class="Ektron_WideTextBox" />
		            <div title="Base Location" class="Ektron_TopLabel"><script>localization.showText('BaseLocation');</script></div>
		            <input title="Enter Base Location Text" type="text" id="BaseLocationBox" class="Ektron_WideTextBox" />
		            <div title="Description" class="Ektron_TopLabel"><script>localization.showText('Description');</script></div>		
		            <textarea title="Enter Description Text" class="Ektron_WideTextArea" id="Description"></textarea>
		            <div title="Keywords" class="Ektron_TopLabel"><script>localization.showText('Keywords');</script></div>		
		            <textarea title="Enter Keywords Text" class="Ektron_WideTextArea" id="Keywords"></textarea>
	            </fieldset>            
            </td>
            <td><span class="Ektron_LeftSpaceSmall"></span></td>
            <td valign="top">
	            <fieldset>
	                <legend>
		                <script>localization.showText('BodyAttributes');</script>
	                </legend>
				    <div title="Back Color" class="Ektron_TopLabel"><script>localization.showText('BackColor');</script></div>
				    <telerik:colorpicker id="BgColorPicker" runat="server" />
				    <div title="Top Margin" class="Ektron_TopLabel"><script>localization.showText('TopMargin');</script></div>
				    <input title="Enter Top Margin Value" type="text" size="2" id="topMargin">
				    <div title="Left Margin" class="Ektron_TopLabel"><script>localization.showText('LeftMargin');</script></div>
				    <input title="Enter Left Margin Value" type="text" size="2" id="leftMargin">
				    <div title="Class Name" class="Ektron_TopLabel"><script>localization.showText('ClassName');</script></div>
				    <telerik:cssclassselector id="theCssClassSelector" cssfilter="ALL, BODY"
				        width="250px" popupwidth="250px" popupheight="160px" runat="server" />					
				    <div title="Bottom Margin" class="Ektron_TopLabel"><script>localization.showText('BottomMargin');</script></div>		
				    <input title="Enter Bottom Margin Value" type="text" size="2" id="bottomMargin">
				    <div title="Right Margin" class="Ektron_TopLabel"><script>localization.showText('RightMargin');</script></div>					
				    <input title="Enter Right Margin Value" type="text" size="2" id="rightMargin">
				    <div title="Background Image" class="Ektron_TopLabel"><script>localization.showText('BGImage');</script></div>
				    <telerik:imagedialogcaller id="BgImageDialogCaller" runat="server" />
	            </fieldset>             
            </td>
        </tr>
    </table>
    
    <hr />
</div>

<div class="Ektron_Dialogs_ButtonContainer">
    <button title="Ok" class="Ektron_StandardButton" onclick="CloseDialog(true)">
        <script>localization.showText('OK');</script>
    </button>
    <span class="Ektron_LeftSpaceSmall"></span>
    <button title="Cancel" class="Ektron_StandardButton" onclick="CloseDialog(false)">
        <script>localization.showText('Cancel');</script>
    </button>
</div>

<script>
	var theEditor = null;
	var editorDocument = null;
	var editorHead = null;
	var editorBase = null;
		
	//Body attributes
	var oBodyAttribs = 
	{	
		bottomMargin:null,
		leftMargin:null,
		rightMargin:null,
		topMargin:null
	};
			
	var oMetaNames = 
	{
		Description: null,
		Keywords: null
	};
	
			
	window.onload = function(e)
	{
		if (!e) 
		{
			e = window.event;
		}
				
		var dialogArgs = GetDialogArguments();
		if (dialogArgs)
		{			
			theEditor = dialogArgs.EditorObj;
			editorDocument = theEditor.Document;			
			editorHead = editorDocument.getElementsByTagName("HEAD")[0];			
			LoadData();
		}
	};
	
	function GetMetaTag(name)
	{
		var metas = editorDocument.getElementsByTagName("META");			
		name = name.toLowerCase();
		var theMeta = null;
		
		for (var i = 0; i < metas.length; i++)
		{
			var meta = metas[i];					
			var metaName = ("" + meta.getAttribute("name")).toLowerCase();
			if (name == metaName)
			{			
				theMeta = meta;
			}
		}		
		return theMeta;	
	}
	

	function LoadData()
	{	
		if (editorDocument)
		{					
			//alert ("HTML " + editorDocument.getElementsByTagName("HTML")[0].innerHTML);			
			var args = GetDialogArguments();
			
			//TITLE
			document.getElementById("TitleBox").value = editorDocument.title;
			
			var txtDescr = document.getElementById("Description");
			txtDescr.value = "";
			var txtKeywords = document.getElementById("Keywords");
			txtKeywords.value = "";
			
			//&lt;meta name="keywords" content="keywords,keyword,keyword phrase,etc."&gt;	
			var meta = GetMetaTag ("Description");
			if (meta)
			{
				oMetaNames["Description"] = meta.getAttribute("content");
				txtDescr.value = oMetaNames["Description"];
			}
						
			meta = GetMetaTag ("Keywords");
			if (meta)
			{
				oMetaNames["Keywords"] = meta.getAttribute("content");
				txtKeywords.value = oMetaNames["Keywords"];
			}			
			
			// CSS classes
			var availableCssClasses = args.CssClasses;
			var cssClassSelector = <%=theCssClassSelector.ClientID%>;
			cssClassSelector.Initialize(availableCssClasses);
			
			cssClassSelector.SelectCssClass(editorDocument.body.className);
			
			// BG image dialog caller
			var imageDialogCaller = <%=BgImageDialogCaller.ClientID%>
			imageDialogCaller.Initialize(args.EditorObj);
			
			var imagePath = editorDocument.body.getAttribute("background");
			if (!imagePath)
				imagePath = "";
			imageDialogCaller.SetImagePath(imagePath);
			
			//BASES
			var bases = editorDocument.getElementsByTagName("BASE");						
			if (bases.length > 0)
			{
				editorBase = bases[0];
				document.getElementById("BaseLocationBox").value = editorBase.getAttribute("href");
			}
						
			//Body attributes
			for (var item in oBodyAttribs)
			{			
				
				var oBox = document.getElementById(item);
				
				var oVal = GetAttribute(editorDocument.body, item);
				if (oVal)
				{
					oBox.value = oVal;
				}
				else oBox.value = "";				
			}	
			
			//BgColor
			var colorSelector = <%=BgColorPicker.ClientID%>;
			colorSelector.SelectColor(editorDocument.body.bgColor);
		}
	}
	
	function CloseDialog(update)
	{
		if (update)
		{
			UpdateEditorDocument();
		}		
		CloseDlg();
	}
	

	function UpdateEditorDocument()
	{
		// Title
		editorDocument.title = document.getElementById("TitleBox").value;
				
		// Description, Keywords			
		for (var item in oMetaNames)
		{			
			var editorMetaTag = null;
			
			var strDescr = document.getElementById(item).value;											
			if (!oMetaNames[item] && strDescr)
			{			
				if (document.all)
				{
					editorMetaTag = editorHead.appendChild(editorDocument.createElement("<META NAME='" + item + "'></META>"));
				}
				else
				{
					editorMetaTag = editorHead.appendChild(editorDocument.createElement("META"));
					editorMetaTag.setAttribute("name", item);
				}
			}
			else editorMetaTag = GetMetaTag (item);
			
			if (editorMetaTag)
			{			   
				editorMetaTag.setAttribute("content", strDescr);
			}
		}
		
		// CSS
		var cssClassSelector = <%=theCssClassSelector.ClientID%>;
		editorDocument.body.className = cssClassSelector.GetSelectedClassName();
		
		// Base
		var strBase = document.getElementById("BaseLocationBox").value;
		if (!editorBase && strBase)
		{
			editorBase = editorHead.appendChild(editorDocument.createElement("BASE"));
		}
			
		SetAttribute(editorBase, "href", strBase);
		
		// COLOR
		var colorSelector = <%=BgColorPicker.ClientID%>;
		if (colorSelector.SelectedColor) {
			editorDocument.body.bgColor = colorSelector.SelectedColor
		} else {
			editorDocument.body.bgColor = '';
		}
		
		// set the bg image
		var imageDialogCaller = <%=BgImageDialogCaller.ClientID%>
		SetAttribute(editorDocument.body,"background", imageDialogCaller.GetImagePath());
		
		for (var item in oBodyAttribs)
		{			
			SetAttribute(editorDocument.body,
						 item,
						 document.getElementById(item).value);
		}			

		if (theEditor)
		{
			theEditor.FullPage = true;
		}
	}
				
	function GetAttribute(element, attributeName)
	{		
		if (!element || !attributeName) return;		
		return element.getAttribute(attributeName);		
	}
						
	function SetAttribute(element, attributeName, attributeValue)
	{		
		if (!element || !attributeName) return;			
		if (attributeValue)
		{			
			element.setAttribute(attributeName, attributeValue);
		}
		else
		{		
			element.removeAttribute(attributeName);
		}
	}
</script>