<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ContentDesignerDiag.aspx.cs" Inherits="Workarea_diagnostics_ContentDesignerDiag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />  
    <title>Content Designer Diagnostic Messages</title>

	<style type="text/css">
	h1
	{
		font: bold large Verdana;
	}
	p
	{
		font: normal medium Verdana;
	}
	.status
	{
		font-weight: bolder;
	}
	#cookie
	{
		font-family: "Courier New";
	}
	#display
	{
		width: 100%;
		height: 400px;
	}
	</style>
	<script language="JavaScript" type="text/javascript">
	<!--
	    function enableContentDesignerDiagMessages(enable) {
	        $ektron.cookie("Ektron.diagException", enable, { expires: (enable ? 3 : -1), path: "/" });
	        $ektron.cookie("Ektron.Xml.onexception", enable, { expires: (enable ? 3 : -1), path: "/" });
	        $ektron.cookie("Xml_xslTransform.onexception", enable, { expires: (enable ? 3 : -1), path: "/" });
	        $ektron.cookie("ContentDesigner.onexception", enable, { expires: (enable ? 3 : -1), path: "/" });
	        updateStatus();
	    }

	    function enableContentDesignerDiagConsole(enable) {
	        $ektron.cookie("ContentDesigner.console", enable, { expires: (enable ? 3 : -1), path: "/" });
	        updateStatus();
	    }

	    function updateStatus() {
	        var status = $ektron.cookie("ContentDesigner.onexception");
	        var statusMessage = Ektron.String.format("Diagnostic error message alerts for Content Designer are <span class=\"status\">{0}</span>.", status ? "enabled" : "disabled");
	        $ektron("#ContentDesignerAlertStatus").html(statusMessage);
	        status = $ektron.cookie("ContentDesigner.console");
	        statusMessage = Ektron.String.format("Diagnostic Firebug console trace messages for Content Designer are <span class=\"status\">{0}</span>.", status ? "enabled" : "disabled");
	        $ektron("#ContentDesignerConsoleStatus").html(statusMessage);
	        $ektron("#cookie").html(document.cookie);
	    }
	// -->
	</script>
</head>
<body onload="updateStatus()">
    <h1>Content Designer Diagnostic Messages</h1>
   
    <p>
		<span id="ContentDesignerAlertStatus"></span>
		<button onclick="enableContentDesignerDiagMessages(true)">Enable</button>
		<button onclick="enableContentDesignerDiagMessages(false)">Disable</button>
	</p>
    <p>
		<span id="ContentDesignerConsoleStatus"></span>
		<button onclick="enableContentDesignerDiagConsole(true)">Enable</button>
		<button onclick="enableContentDesignerDiagConsole(false)">Disable</button>
	</p>
    
    <p>
		<label for="displayOptions">Display:</label>
		<select id="displayOptions" onchange="document.getElementById('refresh').onclick()">
			<option value="../ContentDesigner/ekformsiframe.aspx">(Select)</option>
			<optgroup label="Calculated Field">
				<option value="../ContentDesigner/CalculatedFieldExamples.xml">CalculatedFieldExamples.xml</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=CalculatedFieldExamples.xml&xslt=LocalizeComboBox.xslt&arg0=bExamples%3dtrue&arg1=localeUrl%3dresourcexml.aspx%3fname%3dCalculatedFieldExamples%26LangType%3d{LangType}">Calculated Field Examples Combo Items given LangType</option>
			</optgroup>
			<optgroup label="Choices Field">
				<option value="../ContentDesigner/DataListSpec.xml">DataListSpec.xml</option>
				<option value="../ContentDesigner/ekajaxvalidation.aspx?xml=DataListSpec.xml&xsd=DataListSpec.xsd">Validate DataListSpec.xml</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=DataListSpec.xml&xslt=DataListSpecToDataListCombo.xslt&arg0=LangType%3d{LangType}">DataList Combo Items given LangType</option>
				<option value="../ContentDesigner/cmsfolderdatalist.aspx?id={id}&recursive=false&LangType={LangType}">CMS Folder DataList of folder with given ID and LangType</option>
				<option value="../ContentDesigner/cmsdatalist.aspx?id={id}&LangType={LangType}">CMS DataList from content with given ID and LangType</option>
				<option value="../ContentDesigner/ekajaxvalidation.aspx?xml=cmsdatalist.aspx%3fid%3d{id}%26LangType%3d{LangType}&xsd=CmsDataList.xsd">Validate CMS DataList given ID and LangType</option>
				<option value="../ContentDesigner/countries.xml">List of Countries (countries.xml)</option>
				<option value="../ContentDesigner/languages.xml">List of Languages (languages.xml)</option>
				<option value="../ContentDesigner/uspsca.xsd">List of Canada (uspsca.xsd)</option>
				<option value="../ContentDesigner/uspsus.xsd">List of U.S. (uspsus.xsd)</option>
			</optgroup>
			<optgroup label="Merge List Field">
				<option value="../ContentDesigner/MergeListStyle.xml">MergeListStyle.xml</option>
				<option value="../ContentDesigner/ekajaxvalidation.aspx?xml=MergeListStyle.xml&xsd=MergeListStyle.xsd">Validate MergeListStyle.xml</option>
			</optgroup>
			<optgroup label="Validation">
				<option value="../ContentDesigner/ValidateSpec.xml">ValidateSpec.xml</option>
				<option value="../ContentDesigner/ekajaxvalidation.aspx?xml=ValidateSpec.xml&xsd=ValidateSpec.xsd">Validate ValidateSpec.xml</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=ValidateSpec.xml&xslt=ValidateSpecToValidationCombo.xslt&arg0=validationName%3d{validationName}&arg1=LangType%3d{LangType}">Validation Combo Items given Validation Name and LangType</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=ValidateSpec.xml&xslt=ValidateSpecToDatatypeCombo.xslt&arg0=validationName%3d{validationName}&arg1=LangType%3d{LangType}">Validation Datatype Items given Validation Name and LangType</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=ValidateSpec.xml&xslt=ValidateSpecToExamplesCombo.xslt&arg0=validationName%3d{validationName}&arg1=LangType%3d{LangType}">Validation Examples Items given Validation Name and LangType</option>
			</optgroup>
			<optgroup label="Relevance">
				<option value="../ContentDesigner/RelevantExamples.xml">RelevantExamples.xml</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=RelevantExamples.xml&xslt=LocalizeComboBox.xslt&arg0=bExamples%3dtrue&arg1=localeUrl%3dresourcexml.aspx%3fname%3dRelevantExamples%26LangType%3d{LangType}">Relevance Examples Combo Items given LangType</option>
			</optgroup>
			<optgroup label="Conditional Section">
				<option value="../ContentDesigner/ConditionalExamples.xml">ConditionalExamples.xml</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=ConditionalExamples.xml&xslt=LocalizeComboBox.xslt&arg0=bExamples%3dtrue&arg1=localeUrl%3dresourcexml.aspx%3fname%3dConditionalExamples%26LangType%3d{LangType}">Conditional Examples Combo Items given LangType</option>
				<option value="../webservices/rest.svc/deviceconfigs.xml">Device configurations (XML format)</option>
				<option value="../ContentDesigner/ekajaxtransform.aspx?xml=../webservices/rest.svc/deviceconfigs.xml&xslt=DeviceConfigsToTree.xslt">Device configurations (Tree XML format)</option>
			</optgroup>
			<optgroup label="Restful Web Services">
				<option value="../webservices/rest.svc/test.xml">Test 'rest' service (XML format)</option>
				<option value="../webservices/rest.svc/test.json">Test 'rest' service (JSON format)</option>
				<option value="../webservices/cmis.svc/test">Test CMIS service</option>
				<option value="../webservices/feeds.svc/test.atom">Test Atom feed service</option>
				<option value="../webservices/feeds.svc/test.rss">Test RSS feed service</option>
				<option value="../webservices/rest.svc/resourcetext.xml?key=generic last editor&LangType={LangType}">Resource text 'Last Editor' given LangType</option>
				<option value="../webservices/cmis.svc/folders/{id}/children">Folder items given ID (CMIS format)</option>
				<option value="../webservices/rest.svc/content/{id}.xml?LangType={LangType}">Content XML given ID and LangType</option>
				<option value="../webservices/rest.svc/xmlconfigs/{id}.xsd">Smart Form Design Schema given ID</option>
				<option value="../webservices/rest.svc/deviceconfigs.xml">Device configurations (XML format)</option>
				<option value="../webservices/rest.svc/deviceconfigs/{id}.xml">Device configuration given ID (XML format)</option>
			</optgroup>
			<optgroup label="Resource Xml">
				<option value="../ContentDesigner/resourcexml.aspx?name=CalculatedFieldExamples&id=sSzTxt">CalculatedFieldExamples id=Size of a text string (user lang)</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=CalculatedFieldExamples&LangType={LangType}">CalculatedFieldExamples given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=ChoicesFieldDialog&id=sCantBeBlank">ChoicesFieldDialog id=Cannot be blank (user lang)</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=ChoicesFieldDialog&LangType={LangType}">ChoicesFieldDialog given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=CompatibilityReport&LangType={LangType}">CompatibilityReport given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=DataListSpec&LangType={LangType}">DataListSpec given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=DataListSpec&loc=fr-CA">DataListSpec (Canadian French)</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=DataListSpec&lang=fr">DataListSpec (French)</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=DataListSpec&id=sCaPrvnc">DataListSpec id=Canadian Provinces (user lang)</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=FontName&LangType={LangType}">FontName given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=FontSize&LangType={LangType}">FontSize given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=RelevantExamples&LangType={LangType}">RelevantExamples given LangType</option>
				<option value="../ContentDesigner/resourcexml.aspx?name=ValidateSpec&LangType={LangType}">ValidateSpec given LangType</option>
			</optgroup>
			<optgroup label="Tidy">
				<option value="../ContentDesigner/ekajaxtidy.aspx?html=%3Cb%3Ehi%3CBR%3E%3C%2Fb%3E">Tidy "&lt;b&gt;hi&lt;BR&gt;&lt;/b&gt;"</option>
			</optgroup>
			<optgroup label="configurations">
				<option value="../ContentDesigner/configurations/DataDesignerEdit.aspx">Smart Form Design (DataDesignerEdit.aspx)</option>
				<option value="../ContentDesigner/configurations/DataEntryEdit.aspx">Smart Form Data Entry (DataEntryEdit.aspx)</option>
				<option value="../ContentDesigner/configurations/DesignerEdit.aspx">HTML Form Design (DesignerEdit.aspx)</option>
				<option value="../ContentDesigner/configurations/InterfaceBasic.xml">Basic (InterfaceBasic.xml)</option>
				<option value="../ContentDesigner/configurations/InterfaceBlog.aspx">Blog (InterfaceBlog.aspx)</option>
				<option value="../ContentDesigner/configurations/InterfaceCalendar.xml">Calendar (InterfaceCalendar.xml)</option>
				<option value="../ContentDesigner/configurations/InterfaceEditInContext.aspx">Edit in Context (InterfaceEditInContext.aspx)</option>
				<option value="../ContentDesigner/configurations/InterfaceMinimal.xml">Minimal (InterfaceMinimal.xml)</option>
				<option value="../ContentDesigner/configurations/InterfaceTask.xml">Task (InterfaceTask.xml)</option>
				<option value="../ContentDesigner/configurations/NoToolbars.xml">No Toolbars (NoToolbars.xml)</option>
				<option value="../ContentDesigner/configurations/StandardEdit.aspx">Standard HTML Edit (StandardEdit.aspx)</option>
				<option value="../ContentDesigner/configurations/XsltDesignerEdit.aspx">HTML Form Message (XsltDesignerEdit.aspx)</option>
			</optgroup>
		</select>
		
		<label for="givenId">ID:</label>
		<input type="text" id="givenId" value="0" size="8"  style="text-align: right;" />
		
		<label for="validationName">Validation Name:</label>
		<select id="validationName" onchange="document.getElementById('refresh').onclick()">
			<option value="calculation">Calculation</option>
			<option value="calendar">Calendar</option>
			<option value="checkbox">Checkbox</option>
			<option value="datatype">Datatype</option>
			<option value="textarea">Multiline Textarea</option>
			<option value="password">Password</option>
			<option value="plaintext" selected="selected">Plain Text</option>
			<option value="richarea">Rich Area</option>
		</select>
		
		<label for="langType">Language:</label>
		<select id="langType" onchange="document.getElementById('refresh').onclick()">
			<option value="1025">Arabic</option>
			<option value="1028">Chinese</option>
			<option value="1043">Dutch</option>
			<option value="1033" selected="selected">English (U.S.)</option>
			<option value="2057">English (U.K.)</option>
			<option value="1036">French</option>
			<option value="1031">German</option>
			<option value="1037">Hebrew</option>
			<option value="1040">Italian</option>
			<option value="1041">Japanese</option>
			<option value="1042">Korean</option>
			<option value="1034">Spanish</option>
		</select>
		
		<input type="button" id="refresh" value="Refresh" onclick="document.getElementById('display').src = document.getElementById('displayOptions').value.replace(/\{id\}/ig, document.getElementById('givenId').value).replace(/\{LangType\}/ig, document.getElementById('langType').value).replace(/\{validationName\}/ig, document.getElementById('validationName').value)" />
    </p>
    
    <iframe id="display" src="../ContentDesigner/ekformsiframe.aspx" frameborder="1"></iframe>
   
    <p>document.cookie: <span id="cookie">&#160;</span></p>
</body>
</html>