<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" CodeFile="mergefield.aspx.cs" Inherits="Ektron.ContentDesigner.Dialogs.MergeField" AutoEventWireup="false" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<%@ Register TagPrefix="radcb" Namespace="Telerik.WebControls" Assembly="RadComboBox.NET2" %>
<%@ Register TagPrefix="radTS" Namespace="Telerik.WebControls" Assembly="RadTabStrip.NET2" %>
<%@ Register TagPrefix="radT" Namespace="Telerik.WebControls" Assembly="RadTreeView.NET2" %>
<%@ Register TagPrefix="rada" Namespace="Telerik.WebControls" Assembly="RadAjax.NET2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title id="Title" runat="server">Merge Field</title>
</head>
<body class="dialog">
<form id="form1" runat="server">
    
    <div class="Ektron_DialogTabstrip_Container">
    <radTS:RadTabStrip id="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" ReorderTabRows="true" 
			OnClientTabSelected="ClientTabSelectedHandler" SkinID="TabstripDialog">
        <Tabs>
            <radTS:Tab ID="General" Text="General" Value="General" />
            <radTS:Tab ID="ListStyle" Text="List Style" Value="ListStyle" Enabled="false" />
        </Tabs>
    </radTS:RadTabStrip>
    </div>
    <div class="Ektron_Dialog_Tabs_BodyContainer">
         <radTS:RadMultiPage id="RadMultiPage1" runat="server" SelectedIndex="0">
            <radTS:PageView id="Pageview1" runat="server"> 
                <div class="Ektron_TopSpaceSmall">
                    <fieldset id="fsTree">
                        <legend id="lblSelectField" runat="server" title="To complete the Condition, select a field to insert">Select a field to insert:</legend>
                        <div style="overflow: auto; height: 200px;" id="divTree">
                            <radT:RadTreeView id="fieldListTree" 
                                SingleExpandPath="True" runat="server" 
                                BeforeClientClick="ResetTabs"
                                BeforeClientDoubleClick="OnDoubleClick" />
                        </div>
                    </fieldset>
                    <rada:RadAjaxManager id="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
                        <ajaxsettings>
                        <rada:AjaxSetting AjaxControlID="RadAjaxManager1">
                                <UpdatedControls>
                                <rada:AjaxUpdatedControl ControlID="fieldListTree"></rada:AjaxUpdatedControl>
                                </UpdatedControls>
                        </rada:AjaxSetting>
                        </ajaxsettings>
                    </rada:RadAjaxManager>
                </div>        
            </radTS:PageView>
            <radTS:PageView id="Pageview2" runat="server" >
                <div>
                    <label title="List Style" for="cboStyleList" class="Ektron_StandardLabel" id="lblListStyle" runat="server">List Style:</label>
                    <br />
                    <radcb:RadComboBox
                        id="cboStyleList"
                        runat="server"
                        Width="350px"
                        OnClientSelectedIndexChanged="OnClientSelectedIndexChanged"
                        AllowCustomText="True"> 
                        <Items>
                            <radcb:RadComboBoxItem runat="server" DisplayName="cmdBul" Text="Bulleted List" Value="bulletedList" />
                            <radcb:RadComboBoxItem runat="server" DisplayName="cmdNumL" Text="Numbered List" Value="numberedList" />
                            <radcb:RadComboBoxItem runat="server" DisplayName="sHorzTable" Text="Horizontal Table" Value="horzTable" />
                            <radcb:RadComboBoxItem runat="server" DisplayName="sVertTable" Text="Vertical Table" Value="vertTable" />
                            <radcb:RadComboBoxItem runat="server" DisplayName="sHeadingList" Text="Heading Formatted" Value="headingList" />
                            <radcb:RadComboBoxItem runat="server" DisplayName="sDelimitedList" Text="Delimited List" Value="delimitedList" />
                        </Items>
                    </radcb:RadComboBox>   
                </div>
                <div>
                    <fieldset id="fsPreview" style="height:160px;">
                        <legend id="lblPreview" runat="server">Preview:</legend>
                        <div id="divPreview"></div>
                    </fieldset>
                </div>
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
    //poll dom to figure out when rad treeview has finished
    var RadTreeReady = setInterval(function(){
        if ($ektron("div#fieldListTree span").length > 0)
        {
            $ektron("div#fieldListTree span").each(function(i, e){
                // assigned tabindex to "0" then, it will follow the flow in the dialog.
                $ektron(this).attr("tabindex", 0); 
            });
            var treeview = <%=fieldListTree.ClientID %>;
            $ektron("div#fieldListTree span").keydown(function(e)
	        {
	            if (treeview != null)
                {
                    if (/*space bar*/ 32 == e.keyCode)
                    {
                        treeview.UnSelectAllNodes();
                        var element = e.srcElement ? e.srcElement: e.target;
                        var $curFocusNode = $ektron(element);
                        var selectedVal = $curFocusNode.text();
                        var node = treeview.FindNodeByText(selectedVal);
                        if  (node != null)
                        {
                            if (node.Selected != true)
                            {
                                node.Select();
                            } 
                            else
                            {
                                node.UnSelect();
                            }
                        }
                    }
                }
            });
            //set focus to first node
            window.focus();
	        var $firstNode = $ektron("span.TreeNode:first");
	        $firstNode.focus();
	        if (treeview != null && 0 == treeview.SelectedNodesCount())
            {
                var node = treeview.FindNodeByText($firstNode.text());
                if  (node != null)
                {
                    if (node.Selected != true)
                    {
                        node.Select();
                    } 
                }
            }
            clearInterval(RadTreeReady);
        }
    }, 100);
    
    Ektron.ready(function(){
        initField();
        BindOnRadWindowKeyDown();
    });

	var m_oFieldElem = null; // from dialog arguments, either mergefield or mergelist
	var m_aryFieldList = null; // from dialog arguments
	var m_oSelectedField = null; // item of aryFieldList
	var m_aryStyleList = null; // from MergeListStyle.xml
	var m_cboStyleList = <%= cboStyleList.ClientID %>;
    var m_langType = theEditor ? (theEditor.ekParameters.userLanguage+"") : "";
	var m_aryDatalistCache = new Array();
	
	var path = window.location.href;
	var m_srcPath = path.substr(0, path.length - "dialogs/mergefield.aspx".length);
	var m_ekXml = new Ektron.Xml({ srcPath:m_srcPath });
	
	Ektron.DataListManager.init({ srcPath: m_srcPath, langType: m_langType, ekXml: m_ekXml });
	
	function initField()
	{  
	    var objFormField = new EkFormFields();
		m_oFieldElem = null;

	    var args = GetDialogArguments();
	    if (args)
	    {
	        var oFieldElem = args.selectedField;
	        var sContentTree = args.contentTree;
	        m_aryFieldList = args.fieldListArray;
	        
			LoadFieldTree(sContentTree, m_aryFieldList);
			
			if (oFieldElem)
			{
				var tagName = oFieldElem.tagName;
				var joFieldElem = $ektron(oFieldElem);
				if (objFormField.isDDFieldElement(oFieldElem) && 
					(("SPAN" == tagName && joFieldElem.hasClass("ektdesignns_mergefield")) || 
					("DIV" == tagName && joFieldElem.hasClass("ektdesignns_mergelist"))))
				{
					m_oFieldElem = oFieldElem;
				}
			}
	    }
		m_aryStyleList = LoadStyleList(m_cboStyleList);
	}
	
	function insertField()
	{
		var strHtml = "";
		if (m_oSelectedField)
		{
		    var sDataType = m_oSelectedField.datatype.toLowerCase();
		    if ("selection" == sDataType)
		    {
				var strStyleName = m_cboStyleList.GetValue();
				if (!strStyleName) strStyleName = m_aryStyleList[0].styleName;
				var oStyle = GetStyleByName(strStyleName);
			    
		        //<ektdesignns_mergelist ektdesignns_name="Field2" ektdesignns_datatype="selection" 
		        //ektdesignns_basetype="text" ektdesignns_bind="/*/Data/Field2" 
                //ektdesignns_datalist="numRange" ektdesignns_liststyle="numberedlist" title="Field 2">
                //<ol ektdesignns_list="true"><li>
                //<ektdesignns_mergefield>«Field 2»</ektdesignns_mergefield>
                // &#160;</li></ol>
                //</ektdesignns_mergelist>
                strHtml += "<ektdesignns_mergelist";
                strHtml += " ektdesignns_name=\"" + m_oSelectedField.name + "\"";
                strHtml += serializeAttribute("title", m_oSelectedField.displayName);
                strHtml += serializeAttribute("ektdesignns_datatype", m_oSelectedField.datatype);
                strHtml += serializeOptionalAttribute("ektdesignns_basetype", m_oSelectedField.basetype);
                strHtml += serializeOptionalAttribute("ektdesignns_content", m_oSelectedField.content);
			    strHtml += serializeDatalistAttributes(m_oSelectedField);
                strHtml += serializeAttribute("ektdesignns_bind", m_oSelectedField.xpath);
                strHtml += serializeAttribute("ektdesignns_liststyle", strStyleName);
		        if ("xslt" == oStyle.styleTemplateType)
		        {
					// Can't use .setAttribute(oStyle.styleTemplate) b/c it needs to be HTML encoded.
					// Can't use .setAttribute($ektron.htmlEncode(oStyle.styleTemplate)) b/c IE 7 will 
					// encode it, yielding double-encoding of the XSLT snippet.
					strHtml += " ektdesignns_xslt=\"" + $ektron.htmlEncode(oStyle.styleTemplate) + "\"";
					strHtml += ">\n";
					strHtml += "<table cellspacing=\"0\"><tr><td contenteditable=\"false\">";
                    strHtml += "&#171;" + m_oSelectedField.displayName + "&#187;";
					strHtml += "</td></tr><tr><td>";
                    strHtml += PrepareCustomXSLT(oStyle.styleTemplate);
					strHtml += "</td></tr></table>\n";
		        }
		        else
		        {
		            var sMergeField = oStyle.styleTemplate;
					sMergeField = sMergeField.replace(/<ektdesignns_mergefield\s*\/>/g, 
							"<ektdesignns_mergefield>&#171;" + m_oSelectedField.displayName + "&#187;</ektdesignns_mergefield>");
		            strHtml += ">\n";
		            strHtml += sMergeField;
		        }
		        strHtml += "</ektdesignns_mergelist>";
		    }
		    else
		    {
		        //<ektdesignns_mergefield ektdesignns_name="Field1" ektdesignns_datatype="choice" 
		        //ektdesignns_basetype="text" ektdesignns_datalist="ageRange" 
                //ektdesignns_bind="/*/Data/Field1">«Field 1»</ektdesignns_mergefield> 
                var sTag = "";
                if (sDataType && "date" == sDataType.substr(0,4))
                {
			        sTag = "ektdesignns_calendar";
			    }
			    else
			    {
			        sTag = "ektdesignns_mergefield";
			    }
			    strHtml += "<" + sTag;
                strHtml += " ektdesignns_name=\"" + m_oSelectedField.name + "\"";
                strHtml += serializeAttribute("ektdesignns_datatype", m_oSelectedField.datatype);
                strHtml += serializeOptionalAttribute("ektdesignns_basetype", m_oSelectedField.basetype);
                strHtml += serializeOptionalAttribute("ektdesignns_content", m_oSelectedField.content);
			    strHtml += serializeDatalistAttributes(m_oSelectedField);
                strHtml += serializeAttribute("ektdesignns_bind", m_oSelectedField.xpath);
			    strHtml += ">";
                strHtml += "&#171;" + m_oSelectedField.displayName + "&#187;";
                strHtml += "</" + sTag + ">";
		    }
		}
		CloseDlg(strHtml);	
	}
	
	function LoadFieldTree(sContentTree, aryFieldList)
	{
	    if (aryFieldList)
	    {
			// Append default fields
			var sDefaultNodes = "";
	        var aryNodes = sContentTree.match(/<\/Node>/g);
            var iStartFrom = (aryNodes ? aryNodes.length : 0);
            for (var i = iStartFrom; i < aryFieldList.fields.length; i++)
            {
			    var field = aryFieldList.fields[i];
                sDefaultNodes += "<Node Text=\"" + $ektron.htmlEncode(field.displayName) + "\"";
                sDefaultNodes += " Value=\"" + field.xpath + "\"";
                sDefaultNodes += " ToolTip=\"" + field.xpath + "\"";
                sDefaultNodes += " LongDesc=\"" + field.xpath + "\"";
                sDefaultNodes += " Image=\"<%=ResolveUrl(this.SkinControlsPath)%>ContentDesigner/" + field.content + ".gif\" />\r\n";
            }
	        sContentTree = sContentTree.replace(/<\/Tree>/, sDefaultNodes + "</Tree>");
	    }
	    var ajaxManager = <%=RadAjaxManager1.ClientID %>;
        ajaxManager.AjaxRequest(sContentTree);
    }
    
    function PreFillDialog()
    {
        if (m_oFieldElem != null)
        {
            var treeview = <%=fieldListTree.ClientID %>;
            if (treeview != null)
            {
                var node = treeview.FindNodeByValue(m_oFieldElem.getAttribute("ektdesignns_bind"));
                if  (node != null)
                {
                    if (node.Selected != true)
                    {
                        node.Select();
                    }  
                    m_oSelectedField = getFieldByXPath(node.Value);
                }
            }
            if (m_cboStyleList != null)
            {
				var sListStyle = m_oFieldElem.getAttribute("ektdesignns_liststyle");
                if (sListStyle)
                {
                    m_cboStyleList.SetText(sListStyle);
                    var item = m_cboStyleList.FindItemByValue(sListStyle);
                    if (item != null)
                    {
                        item.Select();
                    }
                }
            }
        }
    }
    
	function OnClientSelectedIndexChanged(item)
    {
        ShowPreview();
    }
    
    function OnDoubleClick(node)
    {
		ResetTabs(node);
		insertField();
    }
    
	function ResetTabs(node)
	{
        m_oSelectedField = getFieldByXPath(node.Value);
        if (m_oSelectedField)
        {
            var bMultipleSelect = ("selection" == m_oSelectedField.datatype);

            var tabStrip = <%= RadTabStrip1.ClientID %>;
	        for (var i = 0; i < tabStrip.Tabs.length; i++)
            {
	            if ("<%=ListStyle.ClientID%>" == tabStrip.Tabs[i].ID)
                {
                    //Multiple Selection list allow a varity of list styles.
                    if (bMultipleSelect)
                    { 
                        tabStrip.Tabs[i].Enable();   
                        tabStrip.Tabs[i].DomElement.focus();
                    }
                    else
                    {
                        tabStrip.Tabs[i].Disable();
                    }
                }
            }
        }
	}
	
	function getFieldByXPath(xpath)
	{
	    if (m_aryFieldList && m_aryFieldList.fields)
	    {
	        for (var i = 0; i < m_aryFieldList.fields.length; i++)
            {
                if (xpath == m_aryFieldList.fields[i].xpath)
                {
                    return m_aryFieldList.fields[i];
                }
            }
        }
        return null;
	}
	
	function getDatalistItemsByName(sDataList)
	{
		var dl = getDatalistByName(sDataList);
		if (dl)
		{
			if (0 == dl.dataItem.length && dl.datasrc)
			{
				if (!m_aryDatalistCache[sDataList])
				{
					m_aryDatalistCache[sDataList] = Ektron.DataListManager.getDataList(dl.datasrc, dl.dataselect, dl.captionxpath, dl.valuexpath, dl.datanamespaces);
				}
				if (m_aryDatalistCache[sDataList].length > 0)
				{
					var objOptions = $ektron(m_aryDatalistCache[sDataList]).find("option");
					for (var i = 0; i < objOptions.length; i++)
					{
						var objOption = objOptions[i];
						dl.dataItem[i] = { value: objOption.value, displayValue: objOption.text };
					}
				}
			}
			return dl.dataItem;	
		}
        return null;
	}
	
	function getDatalistByName(sDataList)
	{
	    for (var i = 0; i < m_aryFieldList.datalists.length; i++)
        {
            if (sDataList == m_aryFieldList.datalists[i].name)
            {
                return m_aryFieldList.datalists[i];
            }
        }
        return null;
	}
	
	function serializeDatalistAttributes(oField)
	{
		if (!oField) return "";
		var strHtml = "";
		strHtml += serializeOptionalAttribute("ektdesignns_datalist", oField.datalist);
		if (strHtml.length > 0)
		{
			var dl = getDatalistByName(oField.datalist);
			strHtml += serializeOptionalAttribute("ektdesignns_datasrc", dl.datasrc);
			strHtml += serializeOptionalAttribute("ektdesignns_dataselect", dl.dataselect);
			strHtml += serializeOptionalAttribute("ektdesignns_captionxpath", dl.captionxpath);
			strHtml += serializeOptionalAttribute("ektdesignns_valuexpath", dl.valuexpath);
			strHtml += serializeOptionalAttribute("ektdesignns_datanamespaces", dl.datanamespaces);
		}
		return strHtml;
	}
	
	function serializeAttribute(name, value)
	{
		return " " + name + "=\"" + $ektron.htmlEncode(value) + "\"";
	}

	function serializeOptionalAttribute(name, value)
	{
		if ("string" == typeof value && value.length > 0)
		{
			return serializeAttribute(name, value);
		}
		return "";
	}
	
	function PrepareCustomXSLT(sDataXSLT)
	{
        var sCustomXslt = "";
        var aryData = getDatalistItemsByName(m_oSelectedField.datalist); 
        if (aryData)
        {
            var sDataPkg = CreateDataPkg(aryData, sDataXSLT);
            var sXSLT = [
            "<?xml version='1.0'?>",
            "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">",
            "<xsl:output method=\"xml\" version=\"1.0\" encoding=\"UTF-8\" indent=\"yes\" omit-xml-declaration=\"yes\"/>",
			"<xsl:key name=\"" + $ektron.htmlEncode(m_oSelectedField.datalist) + "\" match=\"datalist[@name='{@name}']/item\" use=\"@value\"/>",
            "<xsl:template match=\"ektdesignns_mergelist\">",
            "<xsl:variable name=\"name\" select=\"@ektdesignns_name\"/>",
			"<xsl:variable name=\"fieldName\" select=\"@ektdesignns_name\"/>",
            "<xsl:variable name=\"field\" select=\"/*/ektdesignpackage_list/fieldlist/field[@name=string($name)]\"/>",
			"<xsl:variable name=\"datalistName\" select=\"$field/@datalist\"/>",
            "<xsl:variable name=\"dataItemPath\" select=\"/*/ektdesignpackage_list/fieldlist/datalist[@name=$datalistName]\"/>",
			"<xsl:variable name=\"dl\" select=\"$dataItemPath/item\"/>",
			"<xsl:variable name=\"datalistKey\" select=\"$datalistName\"/>",
			"<xsl:variable name=\"datalist\" select=\"$dl\"/>",
            "<xsl:variable name=\"xpath\" select=\"$dl[position() &lt;= 4]\"/>",
            sDataXSLT,
            "</xsl:template>",
            "<!--ignore-->",
            "<xsl:template match=\"ektdesignpackage_list\"/>",
            "</xsl:stylesheet>"].join('\n');

            sCustomXslt = m_ekXml.xslTransform(sDataPkg, sXSLT);
            sCustomXslt = $ektron.trim(sCustomXslt);
        }
        return sCustomXslt;
    }
	
	function CreateDataPkg(aryDataXML, sStyle)
	{
        if (0 == aryDataXML.length) return "";
    
//        expected output XML format that feed into the XSLT:
//        <?xml version="1.0"?>
//        <root xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
//            <ektdesignns_mergelist ektdesignns_name="fieldname2" ektdesignns_datatype="selection" ektdesignns_content="type2" ektdesignns_bind="/root/group/element1" ektdesignns_datalist="uniqueIdn">
//                <ul style="list-style-type:square" ektdesignns_list="true"><li a="1"><ektdesignns_mergefield/></li></ul>
//            </ektdesignns_mergelist>
//            <ektdesignpackage_list>
//                <fieldlist>
//                    <field name="fieldname2" datatype="selection" datalist="uniqueId1" content="type2" xpath="/root/group/element1">display name1</field>
//                    <datalist name="uniqueIdn">
//                        <item value="1">displayValue1</item>
//                        <item value="2">displayValue2</item>
//                        <item value="3">displayValue3</item>
//                        <item value="4">displayValue4</item>
//                        <item value="n">displayValuen</item>
//                    </datalist>
//                </fieldlist>
//            </ektdesignpackage_list>
//        </root>

        sStyle = "<ektdesignns_mergelist ektdesignns_name=\"" + $ektron.htmlEncode(m_oSelectedField.name) + "\">" + sStyle + "</ektdesignns_mergelist>";
        var sMergeList = "<root xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" + sStyle;
        sMergeList += "<ektdesignpackage_list>" + CreateFieldListFromArray(aryDataXML);
        sMergeList += "</ektdesignpackage_list></root>";
        return sMergeList;
    }
    
    function CreateFieldListFromArray(aryDataXML)
    {
        var sFieldList = "<fieldlist>";
        sFieldList += "<field name=\"" + $ektron.htmlEncode(m_oSelectedField.name) + "\" datatype=\"selection\" ";
        sFieldList += "datalist=\"" + $ektron.htmlEncode(m_oSelectedField.datalist) + "\" ";
        sFieldList += "content=\"" + $ektron.htmlEncode(m_oSelectedField.datalist) + "\" ";
        sFieldList += "xpath=\"" + $ektron.htmlEncode(m_oSelectedField.xpath) + "\">" + m_oSelectedField.displayName + "</field>";
        sFieldList += "<datalist name=\"" + $ektron.htmlEncode(m_oSelectedField.datalist) + "\">";
        for (var i = 0; i < aryDataXML.length; i++)
        {
            sFieldList += "<item value=\"" + $ektron.htmlEncode(aryDataXML[i].value) + "\">" + aryDataXML[i].displayValue + "</item>";
        }
        sFieldList += "</datalist></fieldlist>";
        return sFieldList;
    }
    	
	function PrepareSampleData(sStyle)
	{
	    var sSampleData = sStyle;
	    var aryData = getDatalistItemsByName(m_oSelectedField.datalist); 
	    if (aryData)
	    {
            var sDataPkg = CreateDataPkg(aryData, sStyle);
            if (sDataPkg.length > 0) 
            {
                sSampleData = m_ekXml.xslTransform(sDataPkg, "[srcpath]SelectListPreview.xslt");
            }
            else
            {
                // error handling case
                var sMergeField = "<span class=\"ektdesignns_mergefield\">&#171;" & $ektron.htmlEncode(m_oSelectedField.displayName) & "&#187;</span>";
                sSampleData = sSampleData.replace(/\<ektdesignns_mergefield[ ]?\/\>/g, sMergeField);
            }
        }
        return sSampleData;
	}
	
	function CreateDisplayContent(oStyle)
	{
		var sContent = "";
	    if (oStyle)
	    {
			if ("xslt" == oStyle.styleTemplateType)
			{
				sContent = PrepareCustomXSLT(oStyle.styleTemplate);
			}
			else
			{
				sContent = PrepareSampleData(oStyle.styleTemplate);
			}
        }
        return sContent;
	}
	
	function ShowPreview()
	{
		var strStyleName = m_cboStyleList.GetValue();
		var oStyle = GetStyleByName(strStyleName);
		var sContent = CreateDisplayContent(oStyle);
        document.getElementById("divPreview").innerHTML = sContent;
	}
	
	function GetStyleByName(sStyleName)
	{
	    for (var i = 0; i < m_aryStyleList.length; i++)
        {
            if (sStyleName == m_aryStyleList[i].styleName)
            {
                return m_aryStyleList[i];
            }
        }
        return null;
	}
	
	function LoadStyleList(cboStyleList)
	{
		var aryStyleList = [];
	    var sListStyleUrl = "[srcpath]MergeListStyle.xml"; 
        var xmlDoc = m_ekXml.loadXml(sListStyleUrl);
        
        for (var i = 0; i < cboStyleList.Items.length; i++)
        {
            var item = cboStyleList.Items[i];
            var oStyleNode = xmlDoc.selectSingleNode("//liststyle[@name='" + item.Value + "']/*[1]");
			var strTemplateType = "";
			var strTemplate = "";
            if (oStyleNode)
            {
				strTemplateType = oStyleNode.nodeName;
				var strTemplateXml = Ektron.Xml.serializeXml(oStyleNode);
				// Remove outer tags
				var reTagOpen = new RegExp("^[\\s]*<" + strTemplateType + "[^>]*>[\\s]*");
				var reTagClose = new RegExp("[\\s]*</" + strTemplateType + ">[\\s]*$");
				strTemplate = strTemplateXml.replace(reTagOpen, "").replace(reTagClose, "");
            }
			aryStyleList[i] = 
			{ 
				caption: item.Text,
				styleName: item.Value,
				styleTemplateType: strTemplateType,
				styleTemplate: strTemplate
			};
        }
        return aryStyleList;
	}
	
	function ClientTabSelectedHandler(sender, eventArgs)
	{        
	    var tab = eventArgs.Tab;  
	    var tabSelected = tab.Value.toLowerCase();
	    if ("liststyle" == tabSelected)
	    {
	        ShowPreview();
	    }
	}
//-->
</script>
</body>
</html>