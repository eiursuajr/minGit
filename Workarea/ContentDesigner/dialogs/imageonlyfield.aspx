<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="imageonlyfield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.ImageOnlyField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldNameControl" Src="ucFieldName.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldUseControl" Src="ucFieldUse.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAllowControl" Src="ucFieldAllow.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDataStyleControl" Src="ucFieldDataStyle.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldValueOptionControl" Src="ucFieldValueOption.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldAdvancedControl" Src="ucFieldAdvanced.ascx" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Image Only Field</title>
</head>
<body class="dialog">
<form id="form1" runat="server">   
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
        <Tabs>
            <radTS:Tab ID="General" Text="General" Value="General" />
            <radTS:Tab ID="DataStyle" Text="Data Style" Value="DataStyle" />
            <radTS:Tab ID="Advanced" Text="Advanced" Value="Advanced" />
        </Tabs>
    </radTS:RadTabStrip> 
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
        <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
	            <table width="100%">
		            <ek:FieldNameControl ID="name" runat="server" />
		            <tr>
		                <td colspan="3">
		                    <table style="width:100%" class="Ektron_TopSpaceVeryVerySmall">
	                        <tr>
	                            <td style="width:50%">
	                                <ek:FieldUseControl ID="use" runat="server" />
		                        </td>
		                        <td>&nbsp;&nbsp;</td>
	                            <td style="width:50%">
	                                <ek:FieldAllowControl ID="allow" runat="server" />
		                        </td>
	                        </tr>
		                    </table>
		                </td>
		            </tr>
		            <ek:FieldValueOptionControl runat="server" />
		            <tr>
		                <td colspan="3">
                            <fieldset class="Ektron_TopSpaceVeryVerySmall">
	                            <legend id="lblDefault" runat="server">Default</legend>
	                            <div>
	                                <table width="100%">
	                                <tr>
	                                    <td><label title="Image Location" for="txtURL" id="lblImageLocation" runat="server">Image Location:</label></td>
                                        <td><input title="Enter <%=GetMessage("lbl img loc")%> here" type="text" name="txtURL" id="txtURL" value="" class="Ektron_StandardTextBox" /></td>
	                                </tr> 
	                                <tr>
	                                    <td><input title="<%=GetMessage("lbl cannot be blank") %> Option" type="checkbox" name="chkNBlank" id="chkNBlank" /><label title="Cannot be blank" for="chkNBlank" id="lblCannotBeBlank" runat="server">Cannot be blank</label></td>
	                                    <td>
	                                        <asp:button runat="server" cssclass="Button" ID="cmdSelect" runat="server" onclientclick="return popupLibrary();" Text="From File..." />
	                                    </td>
	                                </tr>
	                                <tr>
	                                    <td><label title="Description" for="txtDescription" id="lblDescription" runat="server">Description:</label></td>
                                        <td><input title="Enter <%=GetMessage("generic description") %> here" type="text" name="txtDescription" id="txtDescription" value="" class="Ektron_StandardTextBox" /></td>
	                                </tr>
	                                </table>
	                            </div>
	                        </fieldset>
		                </td>
		            </tr>
	            </table>
	        </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server" >
                <table width="100%">
                    <ek:FieldDataStyleControl runat="server" />
                </table> 
	        </radTS:PageView>
            <radTS:PageView id="Pageview3" runat="server" >
                <table width="100%">
                    <ek:FieldAdvancedControl ID="fieldAdvanced" NodeTypeVisible="true" runat="server" />
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
    Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("div#RadTabStrip1 a").eq(0).focus();
        BindOnRadWindowKeyDown();
    });
    	
	var ResourceText = 
	{
		sSelectPicture: "<asp:literal id="sSelectPicture" runat="server"/>"
	,	sCannotBeBlank: "<asp:literal id="sCannotBeBlank" runat="server"/>"
	};
    var m_objFormField = null;
    var m_oFieldElem = null;
    var m_oEditor = null;

	function initField()
	{
	    m_objFormField = new EkFormFields();
	    m_oFieldElem = null;
	    
        var oFieldElem = null;
	    var bIsRootLoc = false;
	    var sDefaultId = "";
	    var sDefaultPrefix = "";
        var args = GetDialogArguments();
	    if (args)
	    {
	        oFieldElem = args.selectedField;
	        bIsRootLoc = args.isRootLocation;
	        sDefaultPrefix = args.fieldPrefix;
	        sDefaultId = args.fieldId;
	        m_oEditor = args.EditorObj;
	    }
	    ekFieldAdvancedControl.setRootTagVisible(bIsRootLoc);
	    
        if (m_objFormField.isDDFieldElement(oFieldElem) && "SPAN" == oFieldElem.tagName && $ektron(oFieldElem).hasClass("ektdesignns_imageonly"))
        {
            ekFieldNameControl.read(oFieldElem);
	        ekFieldUseControl.read(oFieldElem);
	        ekFieldAllowControl.read(oFieldElem);
	        ekFieldAdvancedControl.read(oFieldElem);
	        ekFieldValueOptionControl.read(oFieldElem);
	        ekFieldDataStyleControl.read(oFieldElem);

            document.getElementById("chkNBlank").checked = ("content-req" == oFieldElem.getAttribute("ektdesignns_validation"));

			var objImg = $ektron(oFieldElem).find("img").get(0);
            if (objImg && !m_oEditor.sfInstance.isFieldButton(objImg))
            {
                document.getElementById("txtURL").value = decodeURI(objImg.getAttribute("src")); // Default Value
                document.getElementById("txtDescription").value = objImg.getAttribute("alt");
            }
            else
            {
                document.getElementById("txtURL").value = "";
                document.getElementById("txtDescription").value = "";
            }
            m_oFieldElem = oFieldElem;
            ekFieldNameControl.initSetting(true);
        }
        else
		{
		    ekFieldNameControl.setDefaultFieldNames(sDefaultPrefix, sDefaultId);
	        ekFieldAdvancedControl.setName(sDefaultPrefix + sDefaultId);
	        ekFieldNameControl.initSetting(false);
		}
        ekFieldAdvancedControl.setDefaultVal("URL");
        ekFieldValueOptionControl.setDefaultVal(ekFieldAdvancedControl, "<img />");
        ekFieldValueOptionControl.updateOptions(ekFieldAdvancedControl);
        ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
	}
	
	function insertField()
	{
	    var oFieldElem = m_oFieldElem;
	    var oImg = null;
	    var strUrl = document.getElementById("txtURL").value;
	    if (null == oFieldElem)
	    {
		    oFieldElem = document.createElement("span");
		    oFieldElem.className = "ektdesignns_imageonly";
		    oFieldElem.setAttribute("contenteditable", "false");
		}
		if (oFieldElem)
		{
		    while (oFieldElem.firstChild)
		    {
		        oFieldElem.removeChild(oFieldElem.firstChild);
		    }
		}
		
		if (strUrl.length > 0)
		{    
	        oImg = oFieldElem.ownerDocument.createElement("img");
	        oFieldElem.appendChild(oImg); 
	    }
	    var objTextNode = oFieldElem.ownerDocument.createTextNode(" ");
	    oFieldElem.appendChild(objTextNode);
	    	    
	    var strImgUrl = "<%=ResolveUrl(this.SkinControlsPath)%>ContentDesigner/btnimageonly.gif"; 

        var objFieldBtn = oFieldElem.ownerDocument.createElement("img");  
        objFieldBtn.setAttribute("unselectable", "on");
        objFieldBtn.className = "design_fieldbutton";
        objFieldBtn.src = strImgUrl;
        objFieldBtn.alt = ResourceText.sSelectPicture;
        objFieldBtn.width = "16";
        objFieldBtn.height = "16";
        oFieldElem.appendChild(objFieldBtn);

	    ekFieldAdvancedControl.updateControl(ekFieldNameControl, ekFieldAllowControl.isRepeatable());
	    ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
        ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
        ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
	    ekFieldNameControl.update(oFieldElem);
		ekFieldUseControl.update(oFieldElem);
		ekFieldAllowControl.update(oFieldElem);
		ekFieldAdvancedControl.update(oFieldElem);
		ekFieldValueOptionControl.update(oFieldElem);
		ekFieldDataStyleControl.update(oFieldElem);

		if (document.getElementById("chkNBlank").checked)
		{
		    if (ekFieldValueOptionControl.isUrlOnly()) 
            {
                oFieldElem.setAttribute("ektdesignns_datatype", "anyURI");
            }
            else
            {
                oFieldElem.removeAttribute("ektdesignns_datatype");
            }
            oFieldElem.setAttribute("ektdesignns_validation", "content-req");
            var strAttOnBlur = "design_validate_re(/<img[\\S\\s]*<img/i,this,\"" + ResourceText.sCannotBeBlank + "\");";
            oFieldElem.setAttribute("onblur", Ektron.String.escapeJavaScriptAttributeValue(strAttOnBlur));
		}
		else
		{
            oFieldElem.removeAttribute("ektdesignns_datatype");
            oFieldElem.removeAttribute("ektdesignns_validation");
            oFieldElem.removeAttribute("onblur");
		}
        if (oImg)
        {
            oImg.src = strUrl;
            oImg.setAttribute("data-ektron-url", strUrl);
            oImg.alt = document.getElementById("txtDescription").value;
        }
        
		CloseDlg(oFieldElem);	
	}
	
	function popupLibrary()
	{
        var args = null;
        if (m_oEditor)
        {
            m_oEditor.ShowDialog(
            m_oEditor.workareaPath + "mediamanager.aspx?actiontype=library&scope=images&autonav=0&showthumb=false"
            , args
            , 790
            , 550
            , setImageValue
            , null
            , "Select Image");
        }
        return false;
	}
	function setImageValue(returnValue)
	{
	    if (returnValue && returnValue.sFilename) //Library Object
	    {
	        document.getElementById("txtURL").value = returnValue.sFilename;
            document.getElementById("txtDescription").value = returnValue.sCaption;	    
	    }
	}
	
	function ClientTabSelectedHandler(sender, eventArgs)
	{
	    var tab = eventArgs.Tab;  
	    var tabSelected = tab.Value.toLowerCase();
	    switch(tabSelected)
	    {
	        case "advanced":
	            ekFieldValueOptionControl.updateOptions(ekFieldAdvancedControl);
	            ekFieldAdvancedControl.updateControl(ekFieldNameControl, ekFieldAllowControl.isRepeatable()); 
	            //Advanced field examples changes based on the value option selected
	            ekFieldValueOptionControl.updateDisplay(ekFieldAdvancedControl);  
	            break;
	        case "general":
	            ekFieldValueOptionControl.updateOptions(ekFieldAdvancedControl);
	            ekFieldAdvancedControl.updateFieldNameControl(ekFieldNameControl);
				ekFieldAdvancedControl.updateFieldAllowControl(ekFieldAllowControl);
                ekFieldAdvancedControl.updateFieldUseControl(ekFieldUseControl);
	            break;
	        default:
	            ekFieldNameControl.updateName(ekFieldAdvancedControl); 
	            break;
	    }
	}
//-->
</script>
</body>
</html>
