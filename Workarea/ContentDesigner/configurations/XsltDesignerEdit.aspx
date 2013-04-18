<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XsltDesignerEdit.aspx.cs" Inherits="ContentDesigner_configurations_XsltDesignerEdit" %>
<root>
	<modules>
		<module name="RadEditorNodeInspector" dockingZone="Module" enabled="true" visible="true" dockable="false" />
		<!-- module name="EkRadEditorXhtmlValidator" dockingZone="Module" enabled="true" visible="true" dockable="false" /-->
		<!-- RadEditorHtmlInspector causes defect #30021 - cursor goes back and forth in the plain text field in data entry mode -->
		<module name="RadEditorHtmlInspector" dockingZone="Module" enabled="false" visible="false" />
	</modules>
	<tools name="Edit" enabled="true" dockable="false">
		<tool separator="true"/>
		<tool name="SelectAll" />
		<tool separator="true"/>
		<tool name="Cut" />
		<tool name="Copy" />
		<tool name="Paste" />
		<tool name="PasteFromWordNoFontsNoSizes" />
		<tool name="PastePlainText" />
		<tool name="FindAndReplace" />
		<tool separator="true"/>
		<tool name="Undo" />
		<tool name="Redo" />
		<tool separator="true"/>
		<tool name="AjaxSpellCheck" />
		<tool separator="true"/>
		<tool name="InsertAnchor"/>	
		<tool name="LinkManager" />
		<tool name="Unlink" />
		<tool name="EkLibrary" />
		<%--<tool name="EkAddLinkPage" />--%>
        <tool name="EkLocalize" />
		<tool separator="true"/>
		<tool name="EkConditional" />
		<tool name="InsertHorizontalRule" />
		<tool name="InsertSymbol" />
		<!--<tool name="AbsolutePosition" />-->
	</tools>	
	<tools name="Format" dockable="false">
		<tool separator="true"/>
		<tool name="EkInsertRemoveTempMarkers" />
		<tool name="EkHideShowElements" />
		<tool name="ApplyClass"/>
	    <tool name="FormatBlock"/>
	    <tool separator="true"/>
	    <tool name="Bold" />
	    <tool name="Italic" />
	    <tool name="Underline" />
	    <tool name="StrikeThrough" />
	    <tool separator="true"/>
	    <tool name="Superscript" />
	    <tool name="Subscript" />
<%		if ((settings_data == null) || settings_data.EnableFontButtons == true) { %>
		<tool name="FontName"/>
		<tool name="FontSize"/>
		<tool name="RealFontSize"/>
		<tool name="ForeColor" />
		<tool name="BackColor"/>
<% 	 	}%>
	</tools>
	<tools name="Paragraph Format" enabled="true" dockable="false">
		<tool separator="true"/>
		<tool name="InsertOrderedList" />
		<tool name="InsertUnorderedList"/>
		<tool name="Outdent" />
		<tool name="Indent" />
		<tool separator="true"/>
		<tool name="JustifyLeft" />
		<tool name="JustifyCenter" />
		<tool name="JustifyRight" />
		<tool name="JustifyFull" />
		<tool name="JustifyNone" />
	</tools>
	<tools name="Table" dockable="false" enabled="true">
		<tool separator="true"/>
		<tool name="InsertTable" />
		<tool name="InsertRowAbove" />
		<tool name="InsertRowBelow" />
		<tool name="InsertColumnLeft" />
		<tool name="InsertColumnRight" />
		<tool name="DeleteRow" />
		<tool name="DeleteColumn" />
		<tool name="DeleteCell" />
		<tool name="MergeColumns" />
		<tool name="MergeRows" />
		<tool name="SplitCell" />
		<tool name="SetTableProperties" />
		<tool name="SetCellProperties" />
		<tool name="ToggleTableBorder" />
	</tools>
	<!-- Ektron Editor Start -->
	<tools name="Data Designer" dockable="false">
		<tool separator="true"/>
		<tool name="EkMergeField" />
		<tool name="EkValidateDesign" />
		<tool name="EkPreviewFld" />
	</tools>
	<dialogParameters>
		<dialog name="GroupBox"/>
		<dialog name="TabularDataBox"/>
		<dialog name="CheckBoxField"/>
		<dialog name="PlainTextField"/>
		<dialog name="RichAreaField"/>
		<dialog name="ChoicesField"/>
		<dialog name="SelectListField"/>
		<dialog name="CalculatedField"/>
		<dialog name="CalendarField"/>
		<dialog name="ImageOnlyField"/>
		<dialog name="FileLinkField"/>
	</dialogParameters>
	<!-- Ektron Editor ends -->	
</root>

