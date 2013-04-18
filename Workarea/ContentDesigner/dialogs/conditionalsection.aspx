<%@ Page Language="C#" AutoEventWireup="true" CodeFile="conditionalsection.aspx.cs"
    Inherits="Ektron.ContentDesigner.Dialogs.ConditionalSection" %>
<%@ Register TagPrefix="ek" TagName="FieldXPath" Src="ucFieldXPath.ascx" %>
<%@ Register TagPrefix="ek" TagName="FieldDialogButtons" Src="ucFieldDialogButtons.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title id="Title" runat="server">Conditional Section</title>
</head>
<body class="dialog">
    <form id="form1" runat="server">
    
    <div id="origContent" class="ektronPageInfo" runat="server">
        <div class="Ektron_Dialogs_LineContainer">
            <div class="Ektron_TopSpaceSmall"></div>
            <div class="ektronToolbar" id="divToolBar" runat="server"></div>
        </div>
        
        <div class="Ektron_Dialog_BodyContainer">
            <div class="Ektron_DialogBodyContainer">
                <asp:Localize ID="DialogCaption" runat="server" Text="The content within a Conditional Section will display only when the condition is true. The condition is an XPath expression. Use the examples to get started. Add 'or', 'and', and parentheses to create complex conditions. Several system variables are provided: $deviceConfiguration, $currentDate, $currentDateTime, and $userId." />
            </div>
            
            <div class="Ektron_TopSpaceSmall"></div>
            
            <ek:FieldXPath id="RelevanceXPath" FieldType="Conditional" runat="server" />
        </div>

        <div class="Ektron_Dialogs_LineContainer">
            <div class="Ektron_TopSpaceSmall"></div>
            <div class="Ektron_StandardLine"></div>
        </div>
        
        <ek:FieldDialogButtons ID="btnSubmit" OnOK="return insertField();" runat="server" />
    </div>
    </form>
    <script language="javascript" type="text/javascript">
    <!--
	Ektron.ready(function(){
        initField();
        window.focus();
        $ektron("input:text:first").focus();
        BindOnRadWindowKeyDown();
    });
    
	var ResourceText = 
	{
	};
    var m_objFormField = null;
    var m_oFieldElem = null;
    var m_content = "";
    var m_expressionType = "xpath:";
	
	function initField()
	{
	    m_objFormField = new EkFormFields();
	    m_oFieldElem = null;

	    var treeXml = "";
	    var editor = null;
	    var oFieldElem = null;
	    var oContentElement = null; 
	    var args = GetDialogArguments();
	    if (args)
	    {
	        editor = args.EditorObj;
	        oContentElement = args.contentElement;
	        oFieldElem = args.selectedField;
	        m_content = args.selectedHtml;
	        treeXml = args.contentTree;
	    }

	    if (editor)
	    {
	        var srcPath = editor.ekParameters.srcPath;
	        var skinPath = editor.ekParameters.skinPath;
	        var langType = editor.ekParameters.userLanguage;
	        var strXSLT = "DeviceConfigsToTree.xslt";
	        var args = [
		      { name: "srcPath", value: srcPath }
		    , { name: "skinPath", value: skinPath }
		    , { name: "LangType", value: langType }
		    ];

            var waPath = "<%= new Ektron.Cms.SiteAPI().ApplicationPath %>";
            var deviceConfigsUrl = location.protocol + "//" + location.host + waPath + "webservices/rest.svc/deviceconfigs.xml?LangType=" + langType;
            var deviceTreeXml = editor.ekXml.xslTransform(deviceConfigsUrl, strXSLT, args);

            if (treeXml)
            {
                // Merge trees
                treeXml = treeXml.replace("<Tree>", "");
                treeXml = treeXml.replace("</Tree>", "");
                treeXml = deviceTreeXml.replace("<Tree>", "<Tree>" + treeXml);
            }
            else
            {
                treeXml = deviceTreeXml;
            }
            // TODO merge both treeXml 
	        
	        //Ektron.ContentDesigner.trace("deviceConfigsUrl:\n" + deviceConfigsUrl);
	        //Ektron.ContentDesigner.trace("treeXml:\n" + treeXml);
	    }
	    
        ekFieldXPathControl.init(oContentElement, "ektdesignns_conditional_displayWhen", m_expressionType);
        if (treeXml)
        {
            ekFieldXPathControl.loadContentTree(treeXml);
        }

	    if (oFieldElem != null && "TABLE" == oFieldElem.tagName)
	    {
	        var $fieldElem = $ektron(oFieldElem);
	        var conditionXPath = "";
	        var conditionElem = $fieldElem.find("td.ektdesignns_conditional_displayWhen");
	        if (conditionElem != null && conditionElem.length > 0)
	        {
	            conditionXPath = m_expressionType + conditionElem.text();
	        }
	        oFieldElem.setAttribute("ektdesignns_conditional_displayWhen", conditionXPath);
	        ekFieldXPathControl.read(oFieldElem);
            
	        var contentElem = $fieldElem.find("> tbody > tr > td");
	        if (contentElem != null && contentElem.length > 0)
	        {
	            m_content = Ektron.Xml.serializeXhtml(contentElem[0].childNodes);
	        }
            m_oFieldElem = oFieldElem;
	    }
	    	    
	    ekFieldXPathControl.initXPathExpression();
    }
	
	function insertField()
	{
	    if (false == validateDialog())
	    {
	        return false;
	    }

	    var tempElem = document.createElement("div");
	    ekFieldXPathControl.update(tempElem);
	    var conditionXPath = tempElem.getAttribute("ektdesignns_conditional_displayWhen") + "";
	    tempElem = null;
		
		var sElem = "<ektdesignns_conditional ektdesignns_display=\"table\"";
		sElem += " displayWhen=\"" + $ektron.htmlEncode(conditionXPath.substring(m_expressionType.length)) + "\""; // strip off "xpath:"
		sElem += ">" + m_content + "</ektdesignns_conditional>";
		CloseDlg(sElem);
	}
    
    function validateDialog()
	{
	    var bContinue = true;
	    var ret = [];
	    $ektron(document).trigger("onValidateDialog", [ret]);
	    if (ret && ret.length > 0)
	    {
	        bContinue = m_objFormField.promptOnValidateAction(ret[0]);
	    }
	    return bContinue;
	}
    //-->
    </script>
</body>
</html>
