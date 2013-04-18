<%@ Page ContentType="text/xml" Language="C#" AutoEventWireup="true" Inherits="cms_config" CodeFile="cms_config.aspx.cs" %>

<config product="eWebEditPro" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="config.xsd">

	<!-- Valid positive values are:  yes, true, 1 -->
	<!-- Valid negative values are:  no, false, 0 -->
	
<%  switch (InterfaceName) {
	case "none":
%>
<interface name="none" allowCustomize="false" />
<% 
		break;
case  "minimal": %>
<interface name="minimal" allowCustomize="false">

   <menu name="formatbar" newRow="true" showButtonsCaptions="false" wrap="true">
		<caption>CMS Toolbar</caption>	
		<%if ((IsForum == true) && (options.ContainsKey("clipboardmenu") == false)) {%>
		<%} else{%>
		<button command="cmdcut" />
		<button command="cmdcopy" />
		<button command="cmdpaste" />				
		<button command="cmdundo" />
		<button command="cmdredo" />
		<bar />
		<% }%>
		<%if ((IsForum == true) && (options.ContainsKey("stylemenu") == false)) {%>
		<%} else{%>
	    <button command="cmdunstyle" />
		<!--  <button command="cmdselstyle" /> -->
	    <button command="cmdheaderlevel" />
		<bar />
		<%}%>
        <%  if (IsForum && (options.ContainsKey("fontmenu") == false)) {%>
        <%} else {%>
		<button command="cmdfontname" />
		<button command="cmdfontsize" />
		<button command="cmdfontcolor" />
    	<% } %>
        <% if (settings_data.EnableFontButtons && settings_data.RemoveStyles == false ) { %>
		<button command="cmdbackcolor" />
        <% } %>
		<bar />
         <%  if( IsForum && (options.ContainsKey("textformatmenu") == false) ) {%>		
         <%} else  {%>
		<button command="cmdbold" />
		<button command="cmditalic" />
		<button command="cmdremoveformat" />
		<bar />
		<% } %>
		<button command="cmdspellcheck" />
		<button command="cmdspellayt" />
		<bar />
		<%if ( IsForum && (options.ContainsKey("linkmenu") == false) ) {%>
		<%} else  { %>		
		<button command="cmdhyperlink" />
		<button command="cmdunlink" />
		<% } %>
		<%if ( IsForum && (options.ContainsKey("library") == false) ) {%>
		<%} else  { %>
		<button command="cmdlibrary" />
		<% }%>
	</menu>
	
	<menu name="pformatbar" newRow="true" showButtonsCaptions="false" wrap="true">
	 <%  if ( IsForum && (options.ContainsKey("paragraphmenu") == false) ) {%>
	 <%} else  {%>
		<caption localeRef="mnuPFmt" />
		<button command="cmdnumbered" />
		<button command="cmdbullets" />
		<button command="cmdindentleft" />
		<button command="cmdindentright" />
		<bar />
		<button command="cmdleft" />
		<button command="cmdcenter" />
		<button command="cmdright" />
		<button command="cmdjustify" />
		<bar />
		<button command="cmdhr" />
		<bar />
	<% }%>
	<% if ( IsForum && true == options.ContainsKey("wmv") ) {%>
	    <button command="cmdinsertwmv" />
	<% }%> 
	<%if ( IsForum && options.ContainsKey("emoticonselect") == true ) {%>
	    <button command="cmdaddemoticon" />
	<% }%>
		
	</menu>
	<menu name="specialcharsbar" newRow="false" showButtonsCaptions="false" wrap="true" visible="false">
		<caption localeRef="mnuSplChr" />
		<button command="cmdchr160" />
		<button command="cmdchr169" />
		<button command="cmdchr174" />
		<button command="jstm" />
		<button command="cmdextchars" popup="extcharspopup" />
		<button command="cmdchr" />
	</menu>
	<menu name="tablebar" newRow="true" showButtonsCaptions="false" wrap="false" visible="false">
		<caption localeRef="cmdTbl" />
		<button command="cmdinserttable" />
		<button command="cmdappendrow" />
		<button command="cmdappendcolumn" />
		<button command="cmdinsertrow" />
		<button command="cmdinsertcolumn" />
		<button command="cmdinsertcell" />
		<button command="cmddeleterows" />
		<button command="cmddeletecolumns" />
		<button command="cmddeletecells" />
		<button command="cmdmergecells" />
		<button command="cmdsplitcell" />
		<button command="cmdtableproperties" />
		<button command="cmdcellproperties" />
	</menu>
	<popup name="extcharspopup">
		<button command="cmdchr149" />
		<button command="cmdchr150" />
		<button command="cmdchr151" />
		<button command="cmdchr134" />
		<button command="cmdchr135" />
		<button command="cmdchr131" />
		<button command="cmdchr133" />
		<button command="cmdchr137" />
		<button command="cmdchr128" />
		<button command="cmdchr130" />
		<button command="cmdchr132" />
		<button command="cmdchr145" />
		<button command="cmdchr146" />
		<button command="cmdchr147" />
		<button command="cmdchr148" />
		<button command="cmdchr139" />
		<button command="cmdchr155" />
		<button command="cmdchr156" />
		<button command="cmdchr140" />
		<button command="cmdchr154" />
		<button command="cmdchr138" />
		<button command="cmdchr158" />
		<button command="cmdchr142" />
		<button command="cmdchr159" />
	</popup>
</interface>
<% 
		break;
case "task":
    %>
<interface name="task" allowCustomize="false">
   <menu name="formatbar" newRow="true" showButtonsCaptions="false" wrap="true">
		<caption>CMS Toolbar</caption>		
		<button command="cmdcut" />
		<button command="cmdcopy" />
		<button command="cmdpaste" />				
		<button command="cmdundo" />
		<button command="cmdredo" />
		<bar />
<% if ( settings_data.EnableFontButtons ) { %>
		<button command="cmdfontname" />
		<button command="cmdfontsize" />
		<button command="cmdfontcolor" />
<% } %>
<% if ( settings_data.EnableFontButtons && settings_data.RemoveStyles == false ) { %>
		<button command="cmdbackcolor" />
<% } %>
		<bar />
		<button command="cmdbold" />
		<button command="cmditalic" />
		<button command="cmdremoveformat" />
		<bar />
		<button command="cmdspellcheck" />
		<button command="cmdspellayt" />
		<bar />
		<button command="cmdhyperlink" />
		<button command="cmdunlink" />
		<button command="cmdlibrary" />
	</menu>
</interface>
<% 
		break;
case "calendar" :%>
<interface name="calendar" allowCustomize="false">
   <menu name="formatbar" newRow="true" showButtonsCaptions="false" wrap="true">
		<caption localeRef="mnuFmt" />		
		<button command="cmdcut" />
		<button command="cmdcopy" />
		<button command="cmdpaste" />				
		<button command="cmdundo" />
		<button command="cmdredo" />
		<bar />
<% if ( settings_data.EnableFontButtons ) { %>
		<button command="cmdfontname" />
		<button command="cmdfontsize" />
		<button command="cmdfontcolor" />
<% } %>
<% if ( settings_data.EnableFontButtons && settings_data.RemoveStyles == false ) { %>
		<button command="cmdbackcolor" />
<% } %>
		<bar />
		<button command="cmdbold" />
		<button command="cmditalic" />
		<button command="cmdremoveformat" />
	</menu>
	<menu name="pformatbar" newRow="true" showButtonsCaptions="false" wrap="true">
		<caption localeRef="mnuPFmt" />
		<button command="cmdnumbered" />
		<button command="cmdbullets" />
		<button command="cmdindentleft" />
		<button command="cmdindentright" />
		<bar />
		<button command="cmdleft" />
		<button command="cmdcenter" />
		<button command="cmdright" />
		<button command="cmdjustify" />
		<bar />
		<button command="cmdspellcheck" />
		<button command="cmdspellayt" />
		<bar />
		<button command="cmdhyperlink" />
		<button command="cmdunlink" />
		<button command="cmdlibrary" />
	</menu>
	<menu name="tablebar" newRow="true" showButtonsCaptions="false" wrap="false" visible="false">
		<caption localeRef="cmdTbl" />
		<button command="cmdinserttable" />
		<button command="cmdappendrow" />
		<button command="cmdappendcolumn" />
		<button command="cmdinsertrow" />
		<button command="cmdinsertcolumn" />
		<button command="cmdinsertcell" />
		<button command="cmddeleterows" />
		<button command="cmddeletecolumns" />
		<button command="cmddeletecells" />
		<button command="cmdmergecells" />
		<button command="cmdsplitcell" />
		<button command="cmdtableproperties" />
		<button command="cmdcellproperties" />
	</menu>
</interface>
<% 
		break;
default: %>
<interface name="<%=InterfaceName%>" allowCustomize="false">
<% if ( ExtUI == "cms" ) { %>
	<menu name="cmsbar" newRow="true" showButtonsCaptions="false" wrap="true">
		<caption localeRef="mnuFmt" />
		<button command="cmspublish" />
		<button command="cmscheckin" />
		<button command="cmssave" />
		<button command="cmsundocheckout" />
	</menu>
<% } %>
	<menu name="editbar" newRow="false" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuEdit" />
<% if ( "wysiwyg" == mode ) { %>
		<button command="cmdselectall" />
		<button command="cmdselectnone" />
<% } %>
		<bar />
		<button command="cmdcut" />
		<button command="cmdcopy" />
		<button command="cmdpaste" />
		<button command="cmdpastetext" />
		<button command="cmdfindreplace" />
		<button command="cmdfindnext" />
		<button command="cmdprint" />
		<bar />
		<button command="cmdundo" />
		<button command="cmdredo" />
		<bar />
		<button command="cmdspellcheck" />
		<button command="cmdspellayt" />
		<button command="cmdvalidate" />
<% if ( "dataentry" == mode ) { %>
		<button command="cmdvalidayt" />
<% } %>
		<bar />
		<button command="cmdbookmark" />
		<button command="cmdhyperlink" />
		<button command="cmdunlink" />
		<button command="cmdlibrary" />
<%  if ( "datadesign" !=  mode &&  "xsltdesign" !=  mode && bWikiButton ) {%>
        <%--<button command="jscmswiki_quick" />--%>
		<button command="jscmswiki" />
<% } %>
<% if ( "wysiwyg" == mode ) { %>
		<button command="cmdmfuuploadall" />
		<button command="jscmstranslate" />
		<button command="jscmscomment" />
<% } %>
		<bar />
		<button command="cmdhr" />
		<button command="cmdmfumedia" />
<% if ( ! MinimalFeatureSet ) { %>
		<button command="cmdmfueditimage" />
		<!-- cmdmfueditimage is only available if WebImageFX is installed -->
<% } %>
		<button command="cmdtable" popup="tablepopup" />
<% if ( "wysiwyg" == mode ) { %>
		<bar />
		<button command="cmdmsword" />
<% } %>
	</menu>
	<menu name="formatbar" newRow="true" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuFmt" />
		<button command="cmdunstyle" />
		<button command="cmdselstyle" />
		<button command="cmdheaderlevel" />
<% if ( settings_data.EnableFontButtons ) { %>
		<button command="cmdfontname" />
		<button command="cmdfontsize" />
		<button command="cmdfontcolor" />
<% } %>
<% if ( settings_data.EnableFontButtons && settings_data.RemoveStyles == false ) { %>
		<button command="cmdbackcolor" />
<% } %>
		<bar />
		<button command="cmdbold" />
		<button command="cmditalic" />
		<button command="cmdunderline" />
		<button command="cmdstrike" />
		<button command="cmdremoveformat" />
		<bar />
		<button command="cmdsup" />
		<button command="cmdsub" />
	</menu>
	<menu name="pformatbar" newRow="true" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuPFmt" />
		<button command="cmdnumbered" />
		<button command="cmdbullets" />
		<button command="cmdindentleft" />
		<button command="cmdindentright" />
		<bar />
		<button command="cmdleft" />
		<button command="cmdcenter" />
		<button command="cmdright" />
		<button command="cmdjustify" />
		<button command="cmdnojustify" />
	</menu>
	<menu name="specialcharsbar" newRow="false" showButtonsCaptions="false" wrap="true">
		<caption localeRef="mnuSplChr" />
		<button command="cmdchr160" />
		<button command="cmdchr169" />
		<button command="cmdchr174" />
		<button command="jstm" />
		<button command="cmdextchars" popup="extcharspopup" />
		<button command="cmdchr" />
	</menu>
<% if ( mode != "datadesign" && mode != "formdesign" && mode != "xsltdesign" ) { %>
	<menu name="viewasbar" newRow="false" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuViewAs" />
<% 
switch (mode) {
	case "dataentry": %>
		<button command="cmddataentry" />
		<button command="cmdvalidate" />
<%  break;
    default: %>
		<button command="cmdviewaswysiwyg" />
<% break;
        
}%>
<% if ( NoSrcView == "" ) { %>
		<button command="cmdviewashtml" />
		<button command="cmdviewasproperties" />
<% } %>
		<button command="cmdpreview" />
<% if ( "datadesign" == mode ) { %>
		<button command="cmdpreviewlist" />
<% } %>
		<bar />
		<button command="cmdshowborders" />
		<button command="cmdshowdetails" />
		<bar />
		<button command="cmdabout" />
	</menu>
<% } %>
		
		
<% if ( ! MinimalFeatureSet && FormToolbarEnabled != "false" ) { %>
	<menu name="formelementbar" newRow="true" showButtonsCaptions="false" wrap="false" visible="<%=FormToolbarVisible %>">
		<caption localeRef="frmElements" />
		<button command="cmdformform" />
		<bar />
		<button command="cmdformbutton" />
		<button command="cmdformsubmit" />
		<button command="cmdformreset" />
		<bar />
		<button command="cmdformhidden" />
		<button command="cmdformtext" />
		<button command="cmdformpassword" />
		<button command="cmdformtextarea" />
		<bar />
		<button command="cmdformradio" />
		<button command="cmdformcheckbox" />
		<button command="cmdformselect" />
		<!-- Uncomment the line below to enable a file field -->
		<!-- bar />			
					<button command="cmdformfile" / -->
		<bar />
		<button command="jsvalidation" />
	</menu>
<% } %>
	<menu name="tablebar" newRow="true" showButtonsCaptions="false" wrap="false" visible="false">
		<caption localeRef="cmdTbl" />
		<button command="cmdinserttable" />
		<button command="cmdappendrow" />
		<button command="cmdappendcolumn" />
		<button command="cmdinsertrow" />
		<button command="cmdinsertrowbelow" />
		<button command="cmdinsertcolumn" />
		<button command="cmdinsertcolumnright" />
		<button command="cmdinsertcell" />
		<button command="cmddeleterows" />
		<button command="cmddeletecolumns" />
		<button command="cmddeletecells" />
		<button command="cmdmoverowup" />
		<button command="cmdmoverowdown" />
		<button command="cmdmovecolleft" />
		<button command="cmdmovecolright" />
		<button command="cmdmergecells" />
		<button command="cmdsplitcell" />
		<button command="cmdtableproperties" />
		<button command="cmdcellproperties" />
	</menu>
	<popup name="tablepopup">
		<button command="cmdinserttable" />
		<button command="cmdappendrow" />
		<button command="cmdappendcolumn" />
		<button command="cmdinsertrow" />
		<button command="cmdinsertrowbelow" />
		<button command="cmdinsertcolumn" />
		<button command="cmdinsertcolumnright" />
		<button command="cmdinsertcell" />
		<button command="cmddeleterows" />
		<button command="cmddeletecolumns" />
		<button command="cmddeletecells" />
		<button command="cmdmoverowup" />
		<button command="cmdmoverowdown" />
		<button command="cmdmovecolleft" />
		<button command="cmdmovecolright" />
		<button command="cmdmergecells" />
		<button command="cmdsplitcell" />
		<button command="cmdtableproperties" />
		<button command="cmdcellproperties" />
	</popup>
	<menu name="posbar" newRow="false" showButtonsCaptions="false" wrap="false" visible="false">
		<caption localeRef="mnuPos" />
		<button command="cmdabspos" />
		<button command="cmdlock" />
		<button command="cmdzfront" />
		<button command="cmdzback" />
		<button command="cmdzforward" />
		<button command="cmdzbackward" />
		<button command="cmdzabovetext" />
		<button command="cmdzbelowtext" />
	</menu>
	<!-- 
	<button command="cmdzorder" popup="zpopup"/>
	<popup name="zpopup">
		<button command="cmdzfront"/>
		<button command="cmdzback"/>
		<button command="cmdzforward"/>
		<button command="cmdzbackward"/>
		<button command="cmdzabovetext"/>
		<button command="cmdzbelowtext"/>
	</popup>
	 -->
	<menu name="dirbar" newRow="false" showButtonsCaptions="false" wrap="false" visible="false">
		<caption localeRef="mnuDir" />
		<button command="cmdltrblk" />
		<button command="cmdrtlblk" />
		<button command="cmdltredit" />
		<button command="cmdrtledit" />
	</menu>
	<menu name="miscbar" newRow="false" showButtonsCaptions="false" wrap="false" visible="false">
		<caption localeRef="mnuMisc" />
		<button command="cmdopen" />
		<button command="cmdsaveas" />
		<bar />			
		<button command="cmdshowborders" />
		<button command="cmdshowdetails" />
		<button command="cmdviewasproperties" />			
		<bar />			
		<button command="cmdabout" />
		<bar />
		<button command="cmddelete" />
	</menu>
	<button command="jshyperlink" />

	<button command="cmddelete" />
	<button command="cmdselectall" />
	<button command="cmdselectnone" />
	
	<button command="cmdnojustify" />
	<!--<button command="cmdprop"/>-->
	 
	<popup name="extcharspopup">
		<button command="cmdchr149" />
		<button command="cmdchr150" />
		<button command="cmdchr151" />
		<button command="cmdchr134" />
		<button command="cmdchr135" />
		<button command="cmdchr131" />
		<button command="cmdchr133" />
		<button command="cmdchr137" />
		<button command="cmdchr128" />
		<button command="cmdchr130" />
		<button command="cmdchr132" />
		<button command="cmdchr145" />
		<button command="cmdchr146" />
		<button command="cmdchr147" />
		<button command="cmdchr148" />
		<button command="cmdchr139" />
		<button command="cmdchr155" />
		<button command="cmdchr156" />
		<button command="cmdchr140" />
		<button command="cmdchr154" />
		<button command="cmdchr138" />
		<button command="cmdchr158" />
		<button command="cmdchr142" />
		<button command="cmdchr159" />
	</popup>
<% switch (mode) {
	case "datadesign":
 %>
	<menu name="designbar" newRow="true" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuDesignBar" />
		<button command="cmddsgfieldset" />
		<button command="cmddsgtabulardata" />
		<bar />
		<button command="cmddsgboolean" />
		<button command="cmddsgplaintext" />
		<button command="cmddsgricharea" />
		<button command="cmddsgchoices" />
		<button command="cmddsglistcontrol" />
		<button command="cmddsgcalc" />
		<button command="cmddsgcalendar" />
		<button command="cmddsgimageonly" />
		<button command="cmddsgfilelink" />
		<bar />
		<button command="cmddsgbutton" />
		<bar />
		<button command="cmddsgprop" />
		<button command="cmdvalidxsd" />
	</menu>
	<button command="cmddsgaddfield" popup="designpopup" />
	<popup name="designpopup">
		<button command="cmddsgfieldset" />
		<button command="cmddsgtabulardata" />
		<bar />
		<button command="cmddsgboolean" />
		<button command="cmddsgplaintext" />
		<button command="cmddsgricharea" />
		<button command="cmddsgchoices" />
		<button command="cmddsglistcontrol" />
		<button command="cmddsgcalc" />
		<button command="cmddsgcalendar" />
		<button command="cmddsgimageonly" />
		<button command="cmddsgfilelink" />
		<bar />
		<button command="cmddsgbutton" />
	</popup>
	<menu name="viewasbar" newRow="false" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuViewAs" />
		<button command="cmddatadesign" />
		<button command="cmddataentry" />
		<button command="cmdvalidate" />
		<button command="cmdpreview" />
		<button command="cmdpreviewlist" />
		<bar />
		<button command="cmdshowborders" />
		<button command="cmdshowdetails" />
		<bar />
		<button command="cmdabout" />
	</menu>
<% break;
case "formdesign": %>
	<menu name="designbar" newRow="true" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuDesignBar" />
		<button command="cmddsgboolean" />
		<button command="cmddsgformhidden" />
		<button command="cmddsgplaintext" />
		<button command="cmddsgformpassword" />
		<button command="cmddsgformtextarea" />
		<button command="cmddsgchoices" />
		<button command="cmddsglistcontrol" />
		<button command="cmddsgcalendar" />
		<bar />
		<button command="cmddsgbutton" />
		<bar />
		<button command="cmddsgprop" />
		<button command="cmdvalidxsd" />
	</menu>
	<menu name="viewasbar" newRow="false" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuViewAs" />
		<button command="cmdformdesign" />
<% if( NoSrcView == "" ) { %>
		<button command="cmdviewashtml" />
<% } %>
		<button command="cmdpreview" />
		<button command="cmdpreviewlist" />
		<bar />
		<button command="cmdshowborders" />
		<button command="cmdshowdetails" />
		<bar />
		<button command="cmdabout" />
	</menu>
<% break;
case "xsltdesign": %>
	<menu name="designbar" newRow="true" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuDesignBar" />
		<button command="cmddsgmergelist" />
		<button command="cmddsgprop" />
		<button command="cmdvalidxsd" />
	</menu>
	<menu name="viewasbar" newRow="false" showButtonsCaptions="false" wrap="false">
		<caption localeRef="mnuViewAs" />
		<button command="cmdxsltdesign" />
<% if ( NoSrcView == "" ) { %>
		<button command="cmdviewashtml" />
<% } %>
		<button command="cmdpreview" />
		<bar />
		<button command="cmdshowborders" />
		<button command="cmdshowdetails" />
		<bar />
		<button command="cmdabout" />
	</menu>
<% break;
   } %>
</interface>
<% break; 
} %>

<features>
<%=EKSLK%>
	<!-- Possible command styles are:
            Icon Button:   "default", "icon", not specified
            Icon Toggle:   "toggle"
            Text Listbox:  "list", "listbox"
            Text Editbox:  "edit", "text"
 	-->
	<external>
<% if ( ExtUI == "cms" ) { %>
		<command name="cmspublish">
			<image key="publish" />
			<caption>Publish</caption>
			<tooltiptext>Publish Content</tooltiptext>
		</command>
		<command name="cmscheckin">
			<image key="checkin" />
			<caption>Check In</caption>
			<tooltiptext>Check In Content</tooltiptext>
		</command>
		<command name="cmssave">
			<image key="saveyel" />
			<caption>Save</caption>
			<tooltiptext>Save Content</tooltiptext>
		</command>
		<command name="cmsundocheckout">
			<image key="cancel" />
			<caption>Undo Checkout</caption>
			<tooltiptext>Undo Checkout of Content</tooltiptext>
		</command>
<% } %>
		<cmd name="jscmscomment" key="comment" ref="cmdComment" />
		<command name="jscmstranslate">
			<image key="translate" />
			<caption>Translate</caption>
			<tooltiptext>Translate content</tooltiptext>
		</command>
<%  if ( "datadesign" != mode && "xsltdesign" != mode && bWikiButton ) {%>
		<command name="jscmswiki">
		    <image key="addlinkpage" />
		    <caption>Add Page</caption>
			<tooltiptext>Add Wiki Link.</tooltiptext>
		</command>
		<command name="jscmswiki_quick">
		    <image key="addlinkpage" />
		    <caption>Wiki Quick link</caption>
			<tooltiptext>Add Wiki quick link</tooltiptext>
		</command>
<% } %>
		<!-- Form Validation Command -->
		<command name="jsvalidation">
			<image src="[eWebEditProPath]/btnFormValidate.bmp" />
			<caption>Validate</caption>
			<tooltiptext>Set validation on form elements</tooltiptext>
		</command>
		<command name="cmdlibrary">
			<image key="books4" />
			<caption>Library</caption>
			<tooltiptext><%=m_refMsg.GetMessage("alt editor library button text")%></tooltiptext>
		</command>
		<command name="jstm">
			<caption localeRef="btnCapTM" />
			<tooltiptext localeRef="cmdTM" />
		</command>
		<cmd name="jshyperlink" key="hyperlinkstar" ref="cmdNewHyp" />
		<!-- Place custom commands to be processed by JavaScript here. -->
	</external>
	<!-- values for charencode: utf-8, binary, charref, latin -->
	<clean charencode="utf-8" cr="cr" lf="lf" showonsize="5000" preferfonttag="false" reducetags="true" showdonemsg="true" prompt="true" hideobject="true" mswordfilter="true">
		<remove>
			<tagWoAttr>SPAN</tagWoAttr>
		<% 
			if (settings_data.EnableFontButtons == false) {
				Response.Write("<tagonly>font</tagonly>");
			}
			//'If (settings_data.RemoveStyles = True) ) {
			//' 	response.write("<attribute>style</attribute>")
			//'}
			 %>
		</remove>
		<xsltFilter src="[eWebEditProPath]ektfilter.xslt" />
	</clean>
		
<% if (! (MinimalFeatureSet) && (FormToolbarEnabled != "false")) { %>
	<formelements>
		<cmd name="cmdformform" key="form" ref="frmForm" />
		<cmd name="cmdformradio" key="optionbox" ref="frmOptionBox" />
		<cmd name="cmdformcheckbox" key="checkbox" ref="frmCheckbox" />
		<cmd name="cmdformbutton" key="bbtn" ref="frmBBtn" />
		<cmd name="cmdformsubmit" key="sbtn" ref="frmSBtn" />
		<cmd name="cmdformreset" key="rbtn" ref="frmRBtn" />
		<cmd name="cmdformhidden" key="hiddenfld" ref="frmHiddenFld" />
		<cmd name="cmdformtext" key="textfld" ref="frmTextFld" />
		<cmd name="cmdformpassword" key="pwdfld" ref="frmPasswordFld" />
		<cmd name="cmdformtextarea" key="textbox" ref="frmTextarea" />
		<cmd name="cmdformfile" key="fileup" ref="frmFormFile" />
		<cmd name="cmdformselect" key="droplist" ref="frmDropList" />
	</formelements>
<% } %>

	<!-- selection options are "html" and "xhtml" -->
	<standard mode="<%=DefaultGetContentType%>" autoclean="true" selection="xhtml" publishinvalid="false" publishviewassource="false" continueparagraph="false">
		<!-- equivClass options are "strict", "loose" and "all" -->
		<style publishstyles="false" href="" equivClass="all" wrapstylewithdiv="false" preservewordstyles="<%=PresWrdStyl%>" preservewordclasses="<%=PresWrdCls%>">
		</style>
		<!-- accessibility options are "strict", "loose" and "none" -->
		<validate accessibility="<%=sAccess%>" suggestdefaultval="true">
            <xslt enabled="<%=bAccessEval %>" src="[eWebEditProPath]/ektaccesseval.xslt" />				 <!-- w3c provides XHTML 1.0 Strict, XHTML 1.0 Transitional from their site. -->
			 <!-- eWebEditPro includes a copy of it. -->
			 <!-- uncomment the one to use for your schema validation -->
			 <!-- might also need to uncomment some codes in the ektfilter.xslt -->
			 <!-- NOTE: DataDesigner and DataEntry are not XHTML and would fail with these schema. -->
				 
<% if ( "wysiwyg" == mode ) {
	if (  "strict" == sAccess  || "loose" == sAccess ) { %>
			<schema enabled="false" src="[eWebEditProPath]/xhtml1-strict.xsd"></schema>
<% } 
} %>

				 
<% if ( "wysiwyg" == mode ) { 
	if ( "strict" == sAccess  || "loose" == sAccess ) { %>
			<schema enabled="false" src="[eWebEditProPath]/xhtml1-transitional.xsd"></schema>
<% } 
} %>

			 <!-- These are some samples of online validation. -->
			 <!-- Only one online validation is allowed at a time. -->
			 <!-- Enable the one to use for your online validation. -->
			 <!-- type options are "string" and "file" -->
			 <!-- keywordsearchresult options are "success" and "failure" -->
				 
<% if ( "wysiwyg" == mode ) { 
	if ( "strict" == sAccess  || "loose" == sAccess ) { %>
			<online enabled="false" type="string" contentname="fragment" src="http://validator.w3.org/check" keywordsearchresult="failure" keyword="&lt;span class=&quot;err_type&quot;&gt;Error&lt;/span&gt;"></online>
<% } 
} %>
			 <online enabled="false" type="string" contentname="myHTML" src="http://www.hermish.com/check_this.cfm" keywordsearchresult="failure" keyword="images/icon_violation.gif">
				<data name="URLtest">Your HTML</data>
				<data name="CheckURL">1</data>
				<data name="p1">1</data>
				<data name="p2">1</data>
				<data name="p3">1</data>
				<data name="s508">1</data>
		     </online>
		     <!-- This is designed to use WAVE 3.0. WAVE has changed their webpage to use WAVE 4.0 beta. It is decided to temporery removed this option until later. -->
             <!-- <online enabled="false" type="file" contentname="upload" src="http://wave.webaim.org/wave/Output.jsp" keywordsearchresult="failure" keyword="ACCESSIBILTY FEATURE: Null ">
				<data name="Submit">Submit</data>
				<data name="IPAddress">127.0.0.1</data>
		     </online>  -->
        </validate>
		<cmd name="cmdvalidate" key="validate" ref="cmdVal" />
		<cmd name="cmdprint" key="print" ref="sPrint" />
<% if ( settings_data.EnableFontButtons && settings_data.RemoveStyles == false ) { %>
		<cmd name="cmdbackcolor" key="bgcolor" ref="cmdBC" />
<% } %>
			
<% if ( settings_data.EnableFontButtons && settings_data.RemoveStyles == false ) { %>
		<command name="cmdbackcolorvalue" style="list">
			<caption localeRef="cmdBCVal" />
			<tooltiptext localeRef="cmdBCVal" />
			<selections name="backcolorlist">
				<listchoice command="cmdbackcolor" localeRef="colorPal" />
				<listchoice data="-1" localeRef="sTransparent" />
				<listchoice data="#00FFFF" localeRef="colorAqua" />
				<listchoice data="#000000" localeRef="colorBlack" />
				<listchoice data="#0000FF" localeRef="colorBlue" />
				<listchoice data="#FF00FF" localeRef="colorFuchsia" />
				<listchoice data="#808080" localeRef="colorGray" />
				<listchoice data="#008000" localeRef="colorGreen" />
				<listchoice data="#00FF00" localeRef="colorLime" />
				<listchoice data="#800000" localeRef="colorMaroon" />
				<listchoice data="#000080" localeRef="colorNavy" />
				<listchoice data="#808000" localeRef="colorOlive" />
				<listchoice data="#800080" localeRef="colorPurple" />
				<listchoice data="#FF0000" localeRef="colorRed" />
				<listchoice data="#C0C0C0" localeRef="colorSilver" />
				<listchoice data="#008080" localeRef="colorTeal" />
				<listchoice data="#FFFF00" localeRef="colorYellow" />
				<listchoice data="#FFFFFF" localeRef="colorWhite" />
			</selections>
		</command>
<% } %>
			
<%  if ( bEnableFontButtons ) {%>
		<cmd name="cmdfontcolor" key="fontcolor" ref="cmdFC" />
<% } %>

			
<% if ( settings_data.EnableFontButtons ) { %>
		<command name="cmdfontcolorvalue" style="list">
			<caption localeRef="cmdFCVal" />
			<tooltiptext localeRef="cmdFCVal" />
			<selections name="fontcolorlist">
				<listchoice command="cmdfontcolor" localeRef="colorPal" />
				<listchoice data="#00FFFF" localeRef="colorAqua" />
				<listchoice data="#000000" localeRef="colorBlack" />
				<listchoice data="#0000FF" localeRef="colorBlue" />
				<listchoice data="#FF00FF" localeRef="colorFuchsia" />
				<listchoice data="#808080" localeRef="colorGray" />
				<listchoice data="#008000" localeRef="colorGreen" />
				<listchoice data="#00FF00" localeRef="colorLime" />
				<listchoice data="#800000" localeRef="colorMaroon" />
				<listchoice data="#000080" localeRef="colorNavy" />
				<listchoice data="#808000" localeRef="colorOlive" />
				<listchoice data="#800080" localeRef="colorPurple" />
				<listchoice data="#FF0000" localeRef="colorRed" />
				<listchoice data="#C0C0C0" localeRef="colorSilver" />
				<listchoice data="#008080" localeRef="colorTeal" />
				<listchoice data="#FFFF00" localeRef="colorYellow" />
				<listchoice data="#FFFFFF" localeRef="colorWhite" />
			</selections>
		</command>
<% } %>

<%  if ( bEnableFontButtons ) {%>
		<command name="cmdfontname" style="list" maxwidth="12">
			<caption localeRef="cmdFNm" />
			<tooltiptext localeRef="cmdFNm" />
			<%=FontList%>
		</command>
<% } %>

<%  if ( bEnableFontButtons ) {%>
		<command name="cmdfontsize" style="list" maxwidth="6">
			<caption localeRef="cmdFS" />
			<tooltiptext localeRef="cmdFS" />
			<!-- There can only be seven selections for sizes -->
			<selections name="fontsizelist">
				<listchoice command="cmdfontsize1" localeRef="mnuFS1" />
				<listchoice command="cmdfontsize2" localeRef="mnuFS2" />
				<listchoice command="cmdfontsize3" localeRef="mnuFS3" />
				<listchoice command="cmdfontsize4" localeRef="mnuFS4" />
				<listchoice command="cmdfontsize5" localeRef="mnuFS5" />
				<listchoice command="cmdfontsize6" localeRef="mnuFS6" />
				<listchoice command="cmdfontsize7" localeRef="mnuFS7" />
			</selections>
		</command>
<% } %>
		<command name="cmdselstyle" style="list">
			<caption localeRef="cmdStyle" />
			<tooltiptext localeRef="cmdStyle" />
			<selections name="stylelist" sorted="true" />
		</command>
		<cmd name="cmdunstyle" key="removestyle" ref="cmdUnSty" />
		<command name="cmdheaderlevel" style="list">
			<caption localeRef="btnCapHdg" />
			<tooltiptext localeRef="cmdHdg" />
			<selections name="headinglist">
				<listchoice command="cmdheadingstd" localeRef="txtHdgNorm" />
				<listchoice command="cmdheading1" localeRef="txtHdg1" />
				<listchoice command="cmdheading2" localeRef="txtHdg2" />
				<listchoice command="cmdheading3" localeRef="txtHdg3" />
				<listchoice command="cmdheading4" localeRef="txtHdg4" />
				<listchoice command="cmdheading5" localeRef="txtHdg5" />
				<listchoice command="cmdheading6" localeRef="txtHdg6" />
				<listchoice command="cmdaddress" localeRef="cmdAddr" />
			</selections>
		</command>
		<cmd name="cmdabout" key="about" ref="cmdAbt" />
		<cmd name="cmdopen" key="open" ref="cmdOpen" />
		<cmd name="cmdsaveas" key="save" ref="cmdSave" />
		<cmd name="cmdcut" key="cut" ref="cmdCut" />
		<cmd name="cmdcopy" key="copy" ref="cmdCp" />
		<cmd name="cmddelete" key="delete" ref="cmdDel" />
		<cmd name="cmdpaste" key="paste" ref="cmdPas" />
		<cmd name="cmdpastetext" key="pastetext" ref="cmdPasTxt" />
		<cmd name="cmdundo" key="undo" ref="cmdUndo" />
		<cmd name="cmdredo" key="redo" ref="cmdRedo" />
		<cmd name="cmdfind" key="find" ref="cmdFind" />
		<cmd name="cmdfindreplace" key="find" ref="cmdReplace" />
		<cmd name="cmdfindnext" key="findnext" ref="cmdFindNext" />
		<cmd name="cmdbold" key="bold" ref="cmdBold" style="toggle" />
		<cmd name="cmditalic" key="italic" ref="cmdItal" style="toggle" />
		<cmd name="cmdunderline" key="underline" ref="cmdUndln" style="toggle" />
		<cmd name="cmdremoveformat" key="plain" ref="sNormal" />
		<command name="cmdnumbered" style="toggle">
			<image key="numbered" />
			<caption localeRef="btnCapNumL" />
			<tooltiptext localeRef="cmdNumL" />
		</command>
		<command name="cmdbullets" style="toggle">
			<image key="bullets" />
			<caption localeRef="btnCapBul" />
			<tooltiptext localeRef="cmdBul" />
		</command>
		<command name="cmdindentleft">
			<image key="indentleft" />
			<caption localeRef="btnCapIndl" />
			<tooltiptext localeRef="cmdIndl" />
		</command>
		<command name="cmdindentright">
			<image key="indentright" />
			<caption localeRef="btnCapIndr" />
			<tooltiptext localeRef="cmdIndr" />
		</command>
		<command name="cmdleft" style="toggle">
			<image key="left" />
			<caption localeRef="btnCapAL" />
			<tooltiptext localeRef="cmdAL" />
		</command>
		<command name="cmdcenter" style="toggle">
			<image key="center" />
			<caption localeRef="btnCapAC" />
			<tooltiptext localeRef="cmdAC" />
		</command>
		<command name="cmdright" style="toggle">
			<image key="right" />
			<caption localeRef="btnCapAR" />
			<tooltiptext localeRef="cmdAR" />
		</command>
		<cmd name="cmdjustify" key="justify" ref="sJustify" style="toggle" />
		<cmd name="cmdnojustify" key="nojustify" ref="cmdANo" style="toggle" />
		<cmd name="cmdselectnone" key="selectnone" ref="cmdSelNo" />
		<cmd name="cmdselectall" key="selectall" ref="cmdSelA" />
		<cmd name="cmdbookmark" key="bookmark" ref="cmdBkm" />
		<command name="cmdhyperlink">
			<image key="hyperlink" />
			<caption localeRef="cmdHyp" />
			<tooltiptext localeRef="cmdHyp" />
		</command>
		<command name="cmdinsertwmv">
		    <image src="[eWebEditProPath]/btninsertwmv.gif" />
			<caption>Add Windows Media Video</caption>
			<tooltiptext>Add Windows Media Video</tooltiptext>
		</command>
		<command name="cmdaddemoticon">
			<image src="[eWebEditProPath]/emoticon.gif" />
			<caption>Add Emoticon</caption>
			<tooltiptext>Add Emoticon</tooltiptext>
		</command>
		<cmd name="cmdunlink" key="removelink" ref="cmdUnlnk" />
		<cmd name="cmdstrike" key="strikethrough" ref="cmdStrike" style="toggle" />
		<cmd name="cmdsub" key="subscript" ref="cmdSub" style="toggle" />
		<cmd name="cmdsup" key="superscript" ref="cmdSup" style="toggle" />
		<command name="cmdhr">
			<image key="horzrule" />
			<caption localeRef="btnCapHR" />
			<tooltiptext localeRef="cmdHR" />
		</command>
		<command name="cmdclean">
			<image key="clean" />
			<caption localeRef="btnCapClean" />
			<tooltiptext localeRef="cmdClean" />
		</command>
		<command name="cmdshowborders" style="toggle">
			<image key="borders" />
			<caption localeRef="btnCapShBord" />
			<tooltiptext localeRef="cmdShBord" />
		</command>
		<command name="cmdshowdetails" style="toggle">
			<image key="glyphs" />
			<caption localeRef="btnCapGlyph" />
			<tooltiptext localeRef="cmdGlyph" />
		</command>
		<!--<cmd name="cmdprop" key="properties" ref="sProp" />-->
		<cmd name="cmdabspos" key="abspos" style="toggle" ref="cmdAbsPos" />
		<cmd name="cmdlock" key="lock" style="toggle" ref="sLock" />
		<cmd name="cmdzfront" key="front" ref="cmdZF" />
		<cmd name="cmdzback" key="back" ref="cmdZB" />
		<cmd name="cmdzforward" key="forward" ref="cmdZFw" />
		<cmd name="cmdzbackward" key="backward" ref="cmdZBw" />
		<cmd name="cmdzabovetext" key="abovetext" ref="cmdZAT" />
		<cmd name="cmdzbelowtext" key="belowtext" ref="cmdZBT" />
		<cmd name="cmdzorder" key="zordermenu" ref="sMove" />
		<cmd name="cmdltrblk" key="ltrblock" style="toggle" ref="cmdLRB" />
		<cmd name="cmdrtlblk" key="rtlblock" style="toggle" ref="cmdRLB" />
		<cmd name="cmdltredit" key="ltredit" style="toggle" ref="cmdLRE" />
		<cmd name="cmdrtledit" key="rtledit" style="toggle" ref="cmdRLE" />
		<command name="cmdchr160">
			<caption localeRef="btnCapnbsp" />
			<tooltiptext localeRef="cmdnbsp" />
		</command>
		<command name="cmdchr169">
			<caption>&#169;</caption>
			<tooltiptext localeRef="cmdC169" />
		</command>
		<command name="cmdchr174">
			<caption>&#174;</caption>
			<tooltiptext localeRef="cmdC174" />
		</command>
		<command name="cmdchr" style="list" maxwidth="16">
			<caption localeRef="cmdMore" />
			<tooltiptext localeRef="cmdMore" />
			<selections name="chrlist">
				<listchoice delimited="|">&#161;|&#162;|&#163;|&#164;|&#165;|&#166;</listchoice>
				<listchoice delimited="|">&#167;|&#168;|&#169;|&#170;|&#171;|&#172;</listchoice>
				<listchoice delimited="|">&#173;|&#174;|&#175;|&#176;|&#177;|&#178;</listchoice>
				<listchoice delimited="|">&#179;|&#180;|&#181;|&#182;|&#183;|&#184;</listchoice>
				<listchoice delimited="|">&#185;|&#186;|&#187;|&#188;|&#189;|&#190;</listchoice>
				<listchoice delimited="|">&#191;|&#192;|&#193;|&#194;|&#195;|&#196;</listchoice>
				<listchoice delimited="|">&#197;|&#198;|&#199;|&#200;|&#201;|&#202;</listchoice>
				<listchoice delimited="|">&#203;|&#204;|&#205;|&#206;|&#207;|&#208;</listchoice>
				<listchoice delimited="|">&#209;|&#210;|&#211;|&#212;|&#213;|&#214;</listchoice>
				<listchoice delimited="|">&#215;|&#216;|&#217;|&#218;|&#219;|&#220;</listchoice>
				<listchoice delimited="|">&#221;|&#222;|&#223;|&#224;|&#225;|&#226;</listchoice>
				<listchoice delimited="|">&#227;|&#228;|&#229;|&#230;|&#231;|&#232;</listchoice>
				<listchoice delimited="|">&#233;|&#234;|&#235;|&#236;|&#237;|&#238;</listchoice>
				<listchoice delimited="|">&#239;|&#240;|&#241;|&#242;|&#243;|&#244;</listchoice>
				<listchoice delimited="|">&#245;|&#246;|&#247;|&#248;|&#249;|&#250;</listchoice>
				<listchoice delimited="|">&#251;|&#252;|&#253;|&#254;|&#255;</listchoice>
			</selections>
		</command>
		<cmd name="cmdextchars" key="charsmenu" ref="cmdExtCh" />
		<cmd name="cmdchr128" key="euro" />
		<cmd name="cmdchr130" key="lsquor" />
		<cmd name="cmdchr131" key="fnof" />
		<cmd name="cmdchr132" key="ldquor" />
		<cmd name="cmdchr133" key="hellip" />
		<cmd name="cmdchr134" key="dagger" />
		<cmd name="cmdchr135" key="ddagger" />
		<cmd name="cmdchr137" key="permil" />
		<cmd name="cmdchr138" key="sscaron" />
		<cmd name="cmdchr139" key="lsaquo" />
		<cmd name="cmdchr140" key="oeoelig" />
		<cmd name="cmdchr142" key="zzcaron" />
		<cmd name="cmdchr145" key="lsquo" />
		<cmd name="cmdchr146" key="rsquo" />
		<cmd name="cmdchr147" key="ldquo" />
		<cmd name="cmdchr148" key="rdquo" />			
		<cmd name="cmdchr149" key="bull" />
		<cmd name="cmdchr150" key="ndash" />
		<cmd name="cmdchr151" key="mdash" />
		<cmd name="cmdchr153" key="trade" />
		<cmd name="cmdchr154" key="scaron" />
		<cmd name="cmdchr155" key="rsaquo" />
		<cmd name="cmdchr156" key="oelig" />
		<cmd name="cmdchr158" key="zcaron" />
		<cmd name="cmdchr159" key="yyuml" />
		<cmd name="cmdchr913" key="alphauc" />
		<cmd name="cmdchr914" key="betauc" />
		<cmd name="cmdchr915" key="gammauc" />
		<cmd name="cmdchr916" key="deltauc" />
		<cmd name="cmdchr917" key="epsilonuc" />
		<cmd name="cmdchr918" key="zetauc" />
		<cmd name="cmdchr919" key="etauc" />
		<cmd name="cmdchr920" key="thetauc" />
		<cmd name="cmdchr921" key="iotauc" />
		<cmd name="cmdchr922" key="kappauc" />
		<cmd name="cmdchr923" key="lambdauc" />
		<cmd name="cmdchr924" key="muuc" />
		<cmd name="cmdchr925" key="nuuc" />
		<cmd name="cmdchr926" key="xiuc" />
		<cmd name="cmdchr927" key="omicronuc" />
		<cmd name="cmdchr928" key="piuc" />
		<cmd name="cmdchr929" key="rhouc" />
		<cmd name="cmdchr931" key="sigmauc" />
		<cmd name="cmdchr932" key="tauuc" />
		<cmd name="cmdchr933" key="upsilonuc" />
		<cmd name="cmdchr934" key="phiuc" />
		<cmd name="cmdchr935" key="chiuc" />
		<cmd name="cmdchr936" key="psiuc" />
		<cmd name="cmdchr937" key="omegauc" />
		<cmd name="cmdchr945" key="alpha" />
		<cmd name="cmdchr946" key="beta" />
		<cmd name="cmdchr947" key="gamma" />
		<cmd name="cmdchr948" key="delta" />
		<cmd name="cmdchr949" key="epsilon" />
		<cmd name="cmdchr950" key="zeta" />
		<cmd name="cmdchr951" key="eta" />
		<cmd name="cmdchr952" key="theta" />
		<cmd name="cmdchr953" key="iota" />
		<cmd name="cmdchr954" key="kappa" />
		<cmd name="cmdchr955" key="lambda" />
		<cmd name="cmdchr956" key="mu" />
		<cmd name="cmdchr957" key="nu" />
		<cmd name="cmdchr958" key="xi" />
		<cmd name="cmdchr959" key="omicron" />
		<cmd name="cmdchr960" key="pi" />
		<cmd name="cmdchr961" key="rho" />
		<cmd name="cmdchr962" key="sigmaf" />
		<cmd name="cmdchr963" key="sigma" />
		<cmd name="cmdchr964" key="tau" />
		<cmd name="cmdchr965" key="upsilon" />
		<cmd name="cmdchr966" key="phi" />
		<cmd name="cmdchr967" key="chi" />
		<cmd name="cmdchr968" key="psi" />
		<cmd name="cmdchr969" key="omega" />
		<cmd name="cmdchr8592" key="larrow" />
		<cmd name="cmdchr8596" key="harrow" />
		<cmd name="cmdchr8594" key="rarrow" />
		<cmd name="cmdchr8593" key="uarrow" />
		<cmd name="cmdchr8595" key="darrow" />
		<cmd name="cmdchr8660" key="harrow2" />
		<cmd name="cmdchr8658" key="rarrow2" />
		<cmd name="cmdchr8776" key="asymp" />
		<cmd name="cmdchr8801" key="equiv" />
		<cmd name="cmdchr8800" key="nemath" />
		<cmd name="cmdchr8804" key="lemath" />
		<cmd name="cmdchr8805" key="gemath" />
		<cmd name="cmdchr8722" key="minus" />
		<cmd name="cmdchr8730" key="radical" />
		<cmd name="cmdchr8747" key="integral" />
		<cmd name="cmdchr8706" key="partmath" />
		<cmd name="cmdchr8733" key="propmath" />
		<cmd name="cmdchr8734" key="infin" />
		<cmd name="cmdchr8712" key="isinmath" />
		<cmd name="cmdchr8715" key="nimath" />
		<cmd name="cmdchr8834" key="submath" />
		<cmd name="cmdchr8835" key="supmath" />
		<cmd name="cmdchr8838" key="subemath" />
		<cmd name="cmdchr8839" key="supemath" />
		<cmd name="cmdchr8743" key="andmath" />
		<cmd name="cmdchr8744" key="ormath" />
		<cmd name="cmdchr8745" key="capmath" />
		<cmd name="cmdchr8746" key="cupmath" />
		<cmd name="cmdchr8736" key="angle" />
		<cmd name="cmdchr8869" key="perp" />
		<cmd name="cmdchr8756" key="there4" />
		<cmd name="cmdchr8704" key="forall" />
		<cmd name="cmdchr8707" key="exist" />
		<cmd name="cmdchr8711" key="nabla" />
		<cmd name="cmdchr8853" key="oplus" />
		<cmd name="cmdchr8719" key="prod" />
		<cmd name="cmdchr8721" key="sum" />
	</standard>
	<!-- mode options are "body" and "whole" -->
	<!-- view options are "wysiwyg", "source", and "preview" -->
		
<% if ( ! MinimalFeatureSet ) { %>
	<viewas mode="body" unicode="true" view="<%=mode%>">
		<cmd name="cmdviewaswysiwyg" key="page" ref="cmdVAW" />
		<cmd name="cmdviewashtml" key="pagetag" ref="cmdVAH" />
		<cmd name="cmdpreview" key="preview" ref="cmdPrevw" />
		<cmd name="cmdviewasproperties" key="viewprop" ref="cmdVAProp" />			
	</viewas>
<% } %>
<asp:literal id="internal" runat="server" text="" />
<% if ( ! MinimalFeatureSet ) { %>
	<edithtml />
<% } %>
	<spellcheck langid="0" dictionary2="EkWinterTreeSC2.CWinterTreeSC">
		<spellayt autostart="false" markmisspelledsrc="[eWebEditProPath]/wavyred.gif" delay="20" />
		<spellingsuggestion max="4" />
		<cmd name="cmdspellayt" key="spellayt" ref="cmdSplayt" style="toggle" />
		<cmd name="cmdspellcheck" key="spellcheck" ref="cmdSplck" />
	</spellcheck>
<% if ( ! MinimalFeatureSet ) { %>
	<msword>
		<cmd name="cmdmsword" key="msword" ref="cmdMSW" style="toggle" />
	</msword>
<% } %>
	<table>
		<cmd name="cmdtable" key="tablemenu" ref="cmdTbl" />
		<cmd name="cmdinserttable" key="instable" ref="mnuITbl" />
		<cmd name="cmdappendrow" key="addrow" ref="mnuARow" />
		<cmd name="cmdappendcolumn" key="addcol" ref="mnuACol" />
		<cmd name="cmdinsertrow" key="insabove" ref="mnuIRow" />
		<cmd name="cmdinsertrowbelow" key="insbelow" ref="mnuIRowB" />
		<cmd name="cmdinsertcolumn" key="insleft" ref="mnuICol" />
		<cmd name="cmdinsertcolumnright" key="insright" ref="mnuIColR" />
		<cmd name="cmdinsertcell" key="inscell" ref="mnuICell" />
		<cmd name="cmddeleterows" key="delrow" ref="mnuDRow" />
		<cmd name="cmddeletecolumns" key="delcol" ref="mnuDCol" />
		<cmd name="cmddeletecells" key="delcell" ref="mnuDCell" />
		<cmd name="cmdmoverowup" key="moveup" ref="mnuMRowU" />
		<cmd name="cmdmoverowdown" key="movedown" ref="mnuMRowD" />
		<cmd name="cmdmovecolleft" key="moveleft" ref="mnuMColL" />
		<cmd name="cmdmovecolright" key="moveright" ref="mnuMColR" />
		<cmd name="cmdmergecells" key="mergecell" ref="mnuMC" />
		<cmd name="cmdsplitcell" key="splitcell" ref="mnuSC" />
		<cmd name="cmdtableproperties" key="tableprop" ref="mnuTProp" />
		<cmd name="cmdcellproperties" key="cellprop" ref="mnuCProp" />
		<cmd name="cmd508table" key="table508" ref="mnu508table" />
	</table>
<% if ( ! MinimalFeatureSet ) { %>
	<mediafiles>	
		<cmd name="cmdmfumedia" key="picture" ref="cmdPic" />
		<cmd name="cmdmfuuploadall" key="upload" ref="cmdUpldFiles" />
		<!-- The command below will only be enabled when the Ektron WebImageFX tool is installed. -->
		<cmd name="cmdmfueditimage" key="freehand" ref="cmdImgEdit" />
		<!-- 0 is unlimited size -->
		<maxsizek>0</maxsizek>
		<validext><%=extensions%></validext>	
<%  if ( InterfaceName == "minimal" ) {%>
		<mediaconfig allowedit="false" enabled="false" />
<% } else  { %>
		<mediaconfig allowedit="true" enabled="false" />
<% } %>
		<!-- If this section is not defined it will default to FTP with no settings -->
		<!-- The attribute 'type' values "ftp", "file", "post", and "none" are handled within the editor. -->
		<!-- If a page is specified in the type attribute then it is used for GUI file selections. -->
		<transport allowupload="true" type="ftp" xfer="binary" pasv="true">
			<!-- autoupload defines the upload action when local files exist in the content. -->
			<!-- If this section is not defined it will default to ASP,  -->
			<!-- unless ASP is not available, then it defaults to FTP. -->
			<!-- The attribute 'type' values "ftp" and "none" are handled within the editor. -->
			<!-- If a page is specified in the type attribute then it is used to receive files -->
			<!-- automatically sent up by the client.  (User intervention is always required.) -->
			<!-- Set showlistonsave to false to skip the pending upload image list dialog when -->
			<!-- collecting publish content.  This value is default to TRUE (show the image list dialog). -->
			<autoupload type="[eWebEditProPath]/ewepreceive.asp" />
			<!-- Encrypt username and password using Ektron's encrypt.exe program. -->
			<!-- Leave blank to force the user to enter the values. -->
			<username encrypted="false"></username>
			<password encrypted="false"></password>
			<!-- Set to 0 for default port number -->
			<port>0</port>
			<!-- The domain to use for upload.  This is normally used by FTP. -->
			<!-- Upload location is: [domain]+[xferdir]+[filename] -->
			<!-- e.g., ftp.mydomain.com -->
			<!-- If this is blank then the domain specified in xferdir is used. -->
			<domain></domain>
			<!-- The logical FTP/Web/other directory to transfer into. -->
			<!-- (FTP upload directories normally do not match the corresponding web directory.) -->
			<!-- svrlocaleref is the Localization for the FTP/Server folder displayed. -->
			<!-- Its string value is in the locale files. -->
			<xferdir src="[eWebEditProPath]/upload" svrlocaleref="xferDispName" />
			<!-- The directory where a file is referenced by a browser once uploaded. -->
			<!-- Referencing a file through HTTP is: [webroot]+[filename] -->
			<!-- If webroot is blank then it defaults to xferdir value. -->
			<webroot src="" />
			<!-- Possible values for resolvepath are: full, host, local, given -->
			<resolvemethod value="host" src="" allowoverride="true" />
		</transport>
		<imageedit ondblclickactivate="true">
			<control src="[WebImageFXPath]/ImageEditConfig.xml" />
		</imageedit>
	</mediafiles>
<% } %>

<% if (!MinimalFeatureSet)
   { %>
	<customtag>
		<!-- Control the internal custom tag dialogs with these elements and attributes  -->
		<taginsdlg enabled="true" allownew="true" allowdelete="true" allowprop="true" />
		<tagpropdlg enabled="true" listall="true" allowchange="true" />
		<tagattrdlg enabled="true" allownew="true" allowdelete="true" allowchange="true" />
		
		<!-- Custom Tag Commands -->
		<command name="cmdcusttaginsert">
			<caption>Insert Tag</caption>
			<tooltiptext>Insert Custom Tag</tooltiptext>
		</command>			
		<command name="cmdcusttagprop">
			<caption>Tag Properties</caption>
			<tooltiptext>View Custom Tag Properties</tooltiptext>
		</command>
		<command name="cmdcusttagattrs">
			<caption>Tag Attributes</caption>
			<tooltiptext>View Custom Tag Attributes</tooltiptext>
		</command>			
		<command name="cmdcusthidetags" style="toggle">
			<caption>Hide Tags</caption>
			<tooltiptext>Hide All Custom Tags</tooltiptext>
		</command>
		<command name="cmdelements" style="list">
			<caption>Elements</caption>
			<tooltiptext>Valid Elements</tooltiptext>
			<!-- The list items of this command are filled in automatically
			     by the editor with elements that are valid to the cursor's
			     context.
			     This is only visible when a schema is loaded with no errors
			-->
		</command>

		<!-- These two commands are available to use in place of the toggle states 
			of the cmdcusthidetags command.  Best for scripts.  -->
		<cmd name="cmdcustshow" />
		<cmd name="cmdcusthide" />
		<!-- This command will apply a tag specified in the text parameter.  Best for scripts. -->
		<cmd name="cmdcustapplytag" />
		
		<!-- Individual Tag Definitions - Allows control over the look of each tag. -->
		<tagdefinitions name="custtags">
		
			<!-- This default tag defines the values to use when a custom tag is encountered that
			     is not defined in this section.  It also defines values to use when a tag does
				 not define a value in its definition section.  
			     Valid types: "vertical", "horizontal", "nonempty", "empty".
			     Note: vertical and horizontal are blocking tags; nonempty is nonblocking.
			-->
			<tagdefault type="vertical" visible="true" style="font-family:arial; font-weight:bold; background-color:#cccccc; border:solid blue 1pt; margin:2px; width:95%;" dstyle="font-family:arial; font-weight:normal; background-color:white; padding:4px" astyle="font-size:normal;font-weight:normal" ashow="true">
				<!-- The simtaglist is a quick list of tags that follow the default items above.
				      The only offered deviation is the glyph that can be specified.  If no glyph is given
					 then the default glyph is used. -->
				
			</tagdefault>
			
				
			<!-- These are sample tags.  Your own tags must be defined here.
			     Attributes not included here will derive their values from the tagdefault defined above. 
			-->
			<tagspec name="mycomment" type="empty" visible="false" style="background-color:transparent" ashow="false">
				<caption>Comment</caption>
				<glyph src="[eWebEditProPath]/comment.gif" visible="true" width="16" height="16" />
			</tagspec>

			<!-- These custom tags are defined in ektfilterekttags.xslt, 
					which must be included in ektfilter.xslt in order to be processed.
				ekt_date 	Displays current date
				ekt_toc		Creates table of contents of h1-h6 tags
					@type may be bullets (default), numbers, or outline
			-->
			<tagspec name="ekt_date" type="empty" visible="false" style="" dstyle="" ashow="false">
				<caption>Current Date</caption>
				<glyph src="[eWebEditProPath]/tagcalendar.gif" visible="true" width="0" height="0" />
			</tagspec>
			<tagspec name="ekt_toc" type="vertical" visible="true" edit="false" style="font-family:arial;font-weight:normal;border:solid buttonface 1px;background-color:buttonface;margin:2px;" dstyle="font-family:arial;font-weight:normal;border:solid white 1px;background-color:white;padding:2px" astyle="font-size:normal;font-weight:normal" ashow="true">
				<caption>Table of Contents</caption>
			</tagspec>
			<% if (IsForum && options.ContainsKey("wmv"))
      {%>
			<tagspec name="embed" type="empty" visible="false" style="background-color:transparent" ashow="false">
				<caption>Windows Media Video</caption>
				<glyph src="[eWebEditProPath]/wmv.png" visible="true" width="200" height="160" />
			</tagspec>
			<% } %>
<% if ("datadesign" == mode || "formdesign" == mode)
   { %>
			<tagspec name="ektdesignns_richarea" type="vertical" visible="false" edit="true" style="border:inset 2px; margin:0px; width:98%;" dstyle="border-style:none; padding:1px; width:98%; height:5em; overflow:auto; vertical-align:top; font-size:x-small;" astyle="" ashow="false">
				<caption>RichArea</caption>
			</tagspec>
			<tagspec name="ektdesignns_checklist" type="vertical" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:4px" astyle="" ashow="false">
				<caption>Checklist</caption>
			</tagspec>
			<tagspec name="ektdesignns_choices" type="vertical" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:4px" astyle="" ashow="false">
				<caption>Choices</caption>
			</tagspec>
			<tagspec name="ektdesignns_calendar" type="nonempty" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:1px" astyle="" ashow="false">
				<caption>Date</caption>
			</tagspec>
			<tagspec name="ektdesignns_imageonly" type="nonempty" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:1px" astyle="" ashow="false">
				<caption>Image Only</caption>
			</tagspec>
			<tagspec name="ektdesignns_filelink" type="nonempty" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:1px" astyle="" ashow="false">
				<caption>File Link</caption>
			</tagspec>
			<tagspec name="ektdesignns_mergefield" type="nonempty" edit="false" dstyle="background-color:#99ccff; border:1.0px solid #6699cc; padding-left:2px; padding-right:2px;" ashow="false">
				<caption>Field</caption>
			</tagspec>
			<tagspec name="ektdesignns_mergelist" type="vertical" visible="false" edit="true" style="background-color:#ffffff;border:3px solid #6699cc;" dstyle="background-color:#99ccff; border:1px solid #6699cc; padding:7px; overflow:auto; vertical-align:top;" astyle="" ashow="false">
				<caption>List</caption>
			</tagspec>
<% }
   else if ("xsltdesign" == mode)
       { %>
			<tagspec name="ektdesignns_richarea" type="vertical" visible="false" edit="true" style="border:inset 2px; margin:0px; width:98%;" dstyle="border-style:none; padding:1px; width:98%; height:5em; overflow:auto; vertical-align:top; font-size:x-small;" astyle="" ashow="false">
				<caption>RichArea</caption>
			</tagspec>
			<tagspec name="ektdesignns_checklist" type="vertical" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:4px" astyle="" ashow="false">
				<caption>Checklist</caption>
			</tagspec>
			<tagspec name="ektdesignns_choices" type="vertical" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:4px" astyle="" ashow="false">
				<caption>Choices</caption>
			</tagspec>
			<tagspec name="ektdesignns_imageonly" type="nonempty" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:1px" astyle="" ashow="false">
				<caption>Image Only</caption>
			</tagspec>
			<tagspec name="ektdesignns_filelink" type="nonempty" visible="false" edit="false" style="border:dotted buttonface 2px; margin:2px;" dstyle="border-style:none; padding:1px" astyle="" ashow="false">
				<caption>File Link</caption>
			</tagspec>
			<tagspec name="ektdesignns_mergefield" type="nonempty" edit="false" dstyle="background-color:#99ccff; border:1.0px solid #6699cc; padding-left:2px; padding-right:2px;" ashow="false">
				<caption>Field</caption>
			</tagspec>
			<tagspec name="ektdesignns_mergelist" type="vertical" visible="false" edit="true" style="background-color:#ffffff;border:3px solid #6699cc;" dstyle="background-color:#99ccff; border:1px solid #6699cc; padding:7px; overflow:auto; vertical-align:top;" astyle="" ashow="false">
				<caption>List</caption>
			</tagspec>
			<tagspec name="ektdesignns_calendar" type="nonempty" dstyle="background-color:#99ccff; border:1.0px solid #6699cc; padding-left:2px; padding-right:2px;" ashow="false">
				<caption>Date</caption>
			</tagspec>
<%      } %>
		</tagdefinitions>
		<docxml enabled="true" reqfill="true" showroot="true">
			<transform onload="" onsave="" />
			<loadsch enabled="true">
    			<!-- A ns value of "" means a namespace will be determined
    				based on the path or an internal namespace.
    				The status values are:
    						active - used for offering valid options to user
    						idle - stored for later selection or use (default)
    						disabled - entry is ignored and not loaded
    				- If more than one entry is selected as active then the first entry is used.
    				- If no entry is active then the first file is used.
    			-->
		        <xsd status="active" src="" ns="" />
		    </loadsch>
		</docxml>
	</customtag>
<% } %>

<% if ("datadesign" == mode)
   { %>
	<datadesign xmlns:xs="http://www.w3.org/2001/XMLSchema">
		<indexing visible="true" enabled="true">
			<override command="cmddsgricharea" visible="false" />
		</indexing>
		<command name="cmdpreviewlist" style="list">
			<caption localeRef="cmdPrevw" />
			<tooltiptext localeRef="cmdPrevw" />
			<selections name="previewlist">
				<listchoice command="cmdpreview" data="#EC5D0C" localeRef="cmdViewDat" />
				<listchoice command="cmdpreview" data="#5CEA" localeRef="cmdViewXSD" />
				<listchoice command="cmdpreview" data="#F1E1D5" localeRef="cmdViewFld" />
				<listchoice command="cmdpreview" data="#1DEC5" localeRef="cmdViewIdx" /> <!-- indexing -->
				<listchoice command="cmdpreview" data="#EC51" localeRef="cmdViewXSL" />
			</selections>
		</command>
		<cmd name="cmddatadesign" key="design" ref="cmdDDesign" />
		<cmd name="cmddsggroup" key="group" ref="cmdGroupFld" />
		<cmd name="cmddsgfieldset" key="fieldset" ref="cmdFieldset" />
		<cmd name="cmddsgtabulardata" key="table" ref="cmdTabData" />
		<cmd name="cmddsgboolean" key="checkbox" ref="cmdBoolean" />
		<cmd name="cmddsgplaintext" key="textfld" ref="cmdPlainTxt" />
		<cmd name="cmddsgricharea" key="richtextfld" ref="cmdRichArea" />
		<command name="cmddsgchoices">
			<image key="choices" />
			<caption localeRef="cmdChoiceFld" />
			<tooltiptext localeRef="cmdChoiceFld" />
			<selections name="datalists">
				<listchoice data="ageRange" localeRef="sAgeRng" />
				<listchoice data="numRange" localeRef="sNumRng" />
				<listchoice data="gender" localeRef="sGender" />
				<listchoice data="maritalStatus" localeRef="sMariStatus" />
				<asp:Literal id="DataDesignChoiceFld" runat="server"></asp:Literal>
				<!-- <listchoice data="MYDATALIST">My Data List</listchoice> -->
			</selections>
		</command>
		<command name="cmddsglistcontrol">
			<image key="droplist" />
			<caption localeRef="cmdListFld" />
			<tooltiptext localeRef="cmdListFld" />
			<selections name="datalists">
				<listchoice data="languages" localeRef="sLanguages" />
				<listchoice data="countries" localeRef="sCountries" />
				<listchoice data="USPS-US" localeRef="sUSState" />
				<listchoice data="USPS-CA" localeRef="sCaPrvnc" />
				<listchoice data="ageRange" localeRef="sAgeRng" />
				<listchoice data="numRange" localeRef="sNumRng" />
				<listchoice data="years" localeRef="sYrs" />
				<asp:Literal id="DataDesignListFld" runat="server"></asp:Literal>
				<!-- <listchoice data="MYDATALIST">My Data List</listchoice> -->
			</selections>
		</command>
		<cmd name="cmddsgcalendar" key="calendar" ref="cmdCalendar" />
		<cmd name="cmddsgimageonly" key="imageonly" ref="cmdImageOnly" />
		<command name="cmddsgfilelink">
			<image key="filelink" />
			<caption localeRef="cmdFileLink" />
			<tooltiptext localeRef="cmdFileLink" />
		</command>
		<command name="cmddsgcalc">
			<image key="calculation" />
			<caption localeRef="cmdCalcFld" />
			<tooltiptext localeRef="cmdCalcFld" />
			<selections name="examples">
				<listchoice value="{X} + {Y}" localeRef="sAdd2N" />
				<listchoice value="{X} - {Y}" localeRef="sSub2N" />
				<listchoice value="{X} * {Y}" localeRef="sMul2N" />
				<listchoice value="format-number( {X} div {Y} ,'0.###')" localeRef="sDiv2N" />
				<listchoice value="format-number( {X} div {Y} ,'#0%')" localeRef="sFmtPct" />
				<listchoice value="{X} * (number( {X} &gt; 0)*2-1)" localeRef="sAbsVal" />
				<listchoice value="{X} * number( {X} &lt;= {Y} ) + {Y} * number( {X} &gt; {Y} )" localeRef="sMin2" />
				<listchoice value="{X} * number( {X} &gt;= {Y} ) + {Y} * number( {X} &lt; {Y} )" localeRef="sMax2" />
				<listchoice value="( {X} - {Y} ) * number(( {X} - {Y} ) &gt; 0)" localeRef="s0IfNeg" />
				<listchoice value="{X} * ( {Y} * number( {Z} ='true') + number( {Z} !='true'))" localeRef="sMulIf" />
				<listchoice value="round( {X} )" localeRef="sRnd" />
				<listchoice value="ceiling( {X} )" localeRef="sRndUp" />
				<listchoice value="floor( {X} )" localeRef="sRndDn" />
				<listchoice value="sum( {X}[text()] | {Y}[text()] | {Z}[text()] )" localeRef="sTotFlds" />
				<listchoice value="sum( {X}[text()] )" localeRef="sTotList" />
				<listchoice value="format-number( {X} ,'0.00')" localeRef="sFmtDecN2" />
				<listchoice value="format-number(sum( {X}[text()] ) div count( {X} ),'0.###')" localeRef="sAveList" />
				<listchoice value="count( {X} )" localeRef="sCntList" />
				<listchoice value="translate( {X} ,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')" localeRef="sLCase" />
				<listchoice value="translate( {X} ,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')" localeRef="sUCase" />
				<listchoice value="normalize-space( {X} )" localeRef="sRemSp" />
				<listchoice value="concat( {X} , ', ' , {Y} )" localeRef="sConcat" />
				<listchoice value="string-length( {X} )" localeRef="sSzTxt" />
			</selections>
		</command>
		<cmd name="cmddsgprop" key="properties" ref="cmdFldProp" />
		<command name="cmddsgbutton" style="list" maxwidth="12">
			<image key="bbtn" />
			<caption localeRef="cmdBtn" />
			<tooltiptext localeRef="cmdBtn" />
			<selections name="commands">
				<listchoice command="" localeRef="sSelBtn" />
				<!-- remove the following listchoice and add your own -->
				<listchoice command="mybtn" localeRef="cmdBtn" />
			</selections>
		</command>
		<cmd name="cmddsgaddfield" key="design" ref="cmdAddFld" />
		<cmd name="cmdvalidxsd" key="validate" ref="cmdVal" />	
		<validation name="plaintext">
			<choice name="none" treeImg="text">
				<caption localeRef="dlgNV8n" />
				<schema datatype="string" />
			</choice>
			<choice name="string-req" treeImg="text">
				<caption localeRef="dlgNBlank" />
				<schema datatype="string">
					<xs:minLength value="1" /> 
				</schema>
				<validate>
					<regexp>/\S+/</regexp>
				</validate>
			</choice>
			<choice name="nonNegInt" treeImg="number">
				<caption localeRef="dlgNNN" />
				<schema>			
					<xs:simpleType>
						<xs:union memberTypes="xs:nonNegativeInteger">
							<xs:simpleType>
								<xs:restriction base="xs:string">
									<xs:length value="0" />
								</xs:restriction>
							</xs:simpleType>
						</xs:union>
					</xs:simpleType>
				</schema>
				<validate>
					<regexp>/^\d*$/</regexp>
				</validate>
			</choice>
			<choice name="nonNegInt-req" treeImg="number">
				<caption localeRef="dlgNNNReqd" />
				<schema datatype="nonNegativeInteger" />
				<validate>
					<regexp>/^\d+$/</regexp>
				</validate>
			</choice>
			<choice name="decnum" treeImg="number">
				<caption localeRef="sDecN" />
				<schema>			
					<xs:simpleType>
						<xs:union memberTypes="xs:decimal">
							<xs:simpleType>
								<xs:restriction base="xs:string">
									<xs:length value="0" />
								</xs:restriction>
							</xs:simpleType>
						</xs:union>
					</xs:simpleType>
				</schema>
				<validate>
					<regexp pattern="^(\-?(\d*\.)?\d+)$|^$" />
				</validate>
			</choice>
			<choice name="decnum2-req" treeImg="number">
				<caption localeRef="sDecN2Reqd" />
				<schema datatype="decimal">
					<xs:fractionDigits value="2" />
				</schema>
				<calculate>
					<regexp>/(\-?(\d*\.\d{1,2}|\d+))/</regexp> <!-- normalize to 2 decimal digits -->
				</calculate>
				<validate>
					<regexp pattern="\-?(\d*\.\d{1,2}|\d+)" wholeline="true" />
				</validate>
			</choice>
			<choice name="percent-req" treeImg="number">
				<caption localeRef="sPctReqd" />
				<schema datatype="nonNegativeInteger">
					<xs:minInclusive value="0" />
					<xs:maxInclusive value="100" />
				</schema>
				<validate>
					<regexp pattern="[0-9]|[1-9][0-9]|100" wholeline="true" />
				</validate>
			</choice>
			<!-- RFC 2822 defines email address format -->
			<choice name="email" treeImg="text" ref="sEmailAddr" pattern="[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*" />
			<!-- semicolon-delimited list of email addresses -->
			<choice name="emailList" treeImg="text" ref="sEmailAddrList" pattern="[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*(\s*\;\s*[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*)*" />
			<!-- ZIP code (US) 99999 or 99999-9999 -->
			<choice name="ZipCode" treeImg="text" ref="sZIP" pattern="[0-9]{5}(\-[0-9]{4})?" />
			<!-- Social Security Number (US) 999-99-9999 -->
			<choice name="SSN" treeImg="text" ref="sSSN" pattern="[0-9]{3}\-?[0-9]{2}\-?[0-9]{4}" />
			<!-- Postal Code (Canada) A9A 9A9 -->
			<choice name="PostalCodeCA" treeImg="text" ref="sPCCA" pattern="[A-Z][0-9][A-Z] [0-9][A-Z][0-9]" />
			<!-- Social Insurance Number (Canada) 999 999 999 -->
			<choice name="SIN" treeImg="text" ref="sSIN" pattern="[0-9]{3} ?[0-9]{3} ?[0-9]{3}" />
			<!-- Telephone Number (US and Canada) [+]1 (999) 999-9999 -->
			<choice name="phoneUSCA" treeImg="text" ref="sPhoneUSCA" pattern="((\+?1[\. \-]?)?\(?[2-9][0-9]{2}\)?[\. \-\/]*)?[2-9][0-9]{2}[\. \-]?[0-9]{4}" />
			<!-- RFC 2396 and RFC 2732 defines URL format -->
			<choice name="url" treeImg="hyperlink">
				<caption localeRef="sURL" />
				<schema>			
					<xs:simpleType>
						<xs:union memberTypes="xs:anyURI">
							<xs:simpleType>
								<xs:restriction base="xs:string">
									<xs:length value="0" />
								</xs:restriction>
							</xs:simpleType>
						</xs:union>
					</xs:simpleType>
				</schema>
				<!-- don't bother with a reg exp because it allows most anything -->
			</choice>
			<!-- ISBN-10 or ISBN-13 http://www.isbn-international.org/ -->
			<choice name="ISBN" treeImg="text">
				<caption localeRef="sISBN"/>
				<schema>
					<xs:simpleType>
						<xs:restriction base="xs:string">
							<xs:pattern value="([0-9]{9}[0-9X])|([0-9]{13})"/>
						</xs:restriction>
					</xs:simpleType>
				</schema>
				<calculate>
					<script value="this.text=design_normalize_isbn(this.text)"></script>
				</calculate>
				<validate>
					<script value="design_validate_isbn(this.text)"></script>
				</validate>
			</choice>
			<!-- ISSN-8 http://www.issn.org/ -->
			<choice name="ISSN" treeImg="text">
				<caption localeRef="sISSN"/>
				<schema>
					<xs:simpleType>
						<xs:restriction base="xs:string">
							<xs:pattern value="[0-9]{7}[0-9X]"/>
						</xs:restriction>
					</xs:simpleType>
				</schema>
				<calculate>
					<script value="this.text=design_normalize_issn(this.text)"></script>
				</calculate>
				<validate>
					<script value="design_validate_issn(this.text)"></script>
				</validate>
			</choice>
			<custom>
				<caption localeRef="dlgCustom" />
				<selections name="datatype">
					<listchoice value="string" treeImg="text" localeRef="sPText" />
					<listchoice value="anyURI" treeImg="hyperlink" localeRef="sURL" />
					<listchoice value="integer" treeImg="number" localeRef="sWhNum" />
					<listchoice value="decimal" treeImg="number" localeRef="sDecNum" />
					<listchoice value="double" treeImg="number" localeRef="sFPNum" />
				</selections>
				<selections name="examples">
					<listchoice value="{X} &lt; . and . &lt; {Y}" localeRef="sN2Vals" />
					<listchoice value=". = {X}" localeRef="sEquN" />
					<listchoice value=". != {X}" localeRef="sNeqN" />
					<listchoice value="string-length(.) &lt;= {X}" localeRef="sMaxLen" />
				</selections>
			</custom>
		</validation>
		<validation name="password">
			<choice name="none" treeImg="password">
				<caption localeRef="dlgNV8n" />
				<schema datatype="string" />
			</choice>
			<choice name="string-req" treeImg="password">
				<caption localeRef="dlgNBlank" />
				<schema datatype="string">
					<xs:minLength value="1" /> 
				</schema>
				<validate>
					<regexp>/\S+/</regexp>
				</validate>
			</choice>
			<choice name="mix8charstr-req" treeImg="password">
				<caption localeRef="sPwd8StrReqd" />
				<validate>
					<script value="this.text.length &gt;= 8 &amp;&amp; /^\w*\d\w*$/.test(this.text)" />
				</validate>
			</choice>
		</validation>
		<validation name="textarea">
			<choice name="none" treeImg="textbox">
				<caption localeRef="dlgNV8n" />
				<schema datatype="string" />
			</choice>
			<choice name="string-req" treeImg="textbox">
				<caption localeRef="dlgNBlank" />
				<schema datatype="string">
					<xs:minLength value="1" /> 
				</schema>
				<validate>
					<regexp>/\S+/</regexp>
				</validate>
			</choice>
			<choice name="max1000Chars" treeImg="textbox">
				<caption localeRef="sMax1000Chars" />
				<validate>
					<script value="this.text.length &lt;= 1000" />
				</validate>
			</choice>
		</validation>
		<validation name="calculation">
			<choice name="none" treeImg="text">
				<caption localeRef="dlgNV8n" />
				<schema datatype="string" />
			</choice>
			<choice name="nonNeg" treeImg="number">
				<caption localeRef="sNNN" />
				<schema datatype="decimal">
					<xs:minInclusive value="0" />
				</schema>
				<validate>
					<xpath select=". &gt;= 0" />
				</validate>
			</choice>
			<custom>
				<caption localeRef="dlgCustom" />
				<selections name="datatype">
					<listchoice value="integer" treeImg="number" localeRef="sWhNum" />
					<listchoice value="decimal" treeImg="number" localeRef="sDecNum" />
					<listchoice value="double" treeImg="number" localeRef="sFPNum" />
				</selections>
				<selections name="examples">
					<listchoice value=". &gt; {X}" localeRef="sGTN" />
					<listchoice value=". &lt; {X}" localeRef="sLTN" />
					<listchoice value=". = {X}" localeRef="sEquN" />
					<listchoice value=". != {X}" localeRef="sNeqN" />
					<listchoice value="string-length(.) &lt;= {X}" localeRef="sMaxLen" />
				</selections>
			</custom>
		</validation>
		<validation name="calendar">
			<choice name="none" treeImg="calendar">
				<caption localeRef="dlgNV8n" />
			</choice>
			<choice name="date-req" treeImg="calendar">
				<caption localeRef="dlgNBlank" />
				<schema datatype="date" />
				<validate>
					<regexp pattern="[0-9]{4}\-(0[1-9]|1[0-2])\-(0[1-9]|[1-2][0-9]|3[0-1])" wholeline="true" />
				</validate>
			</choice>
			<custom>
				<caption localeRef="dlgCustom" />
				<selections name="datatype">
					<listchoice value="date" treeImg="calendar" localeRef="sDate" />
				</selections>
				<selections name="examples">
					<listchoice value=". &gt;= {X}" localeRef="sDateAfter" />
					<listchoice value=". &lt;= {X}" localeRef="sDateBefore" />
				</selections>
			</custom>
		</validation>
		<!-- mode="whole|body" method="xml|html|text" encoding="string" omit-xml-declaration="yes|no" indent="yes|no" 
			standalone="yes|no" cdata-section-elements="qnames" media-type="string" -->
		<xsltoutput name="datapresentationxslt" mode="body" method="xml" encoding="utf-8" omit-xml-declaration="yes" />
	</datadesign>
<% }
   else if ("formdesign" == mode)
       { %>
	<datadesign xmlns:xs="http://www.w3.org/2001/XMLSchema">
		<indexing visible="false" enabled="false" />
		<cmd name="cmdformdesign" key="design" ref="cmdFDesign" />
		<cmd name="cmddsgformhidden" key="hiddenfld" ref="frmHiddenFld" />
		<cmd name="cmddsgformpassword" key="pwdfld" ref="frmPasswordFld" />
		<cmd name="cmddsgformtextarea" key="textbox" ref="frmTextarea" />
		<cmd name="cmddsgboolean" key="checkbox" ref="cmdBoolean" />
		<cmd name="cmddsgplaintext" key="textfld" ref="cmdPlainTxt" />
		<command name="cmddsgchoices">
			<image key="choices" />
			<caption localeRef="cmdChoiceFld" />
			<tooltiptext localeRef="cmdChoiceFld" />
			<selections name="datalists">
				<listchoice data="ageRange" localeRef="sAgeRng" />
				<listchoice data="numRange" localeRef="sNumRng" />
				<listchoice data="gender" localeRef="sGender" />
				<listchoice data="maritalStatus" localeRef="sMariStatus" />
				<asp:Literal id="FormDesignChoiceFld" runat="server"></asp:Literal>
				<!-- <listchoice data="MYDATALIST">My Data List</listchoice> -->
			</selections>
		</command>
		<command name="cmddsglistcontrol">
			<image key="droplist" />
			<caption localeRef="cmdListFld" />
			<tooltiptext localeRef="cmdListFld" />
			<selections name="datalists">
				<listchoice data="languages" localeRef="sLanguages" />
				<listchoice data="countries" localeRef="sCountries" />
				<listchoice data="USPS-US" localeRef="sUSState" />
				<listchoice data="USPS-CA" localeRef="sCaPrvnc" />
				<listchoice data="ageRange" localeRef="sAgeRng" />
				<listchoice data="numRange" localeRef="sNumRng" />
				<listchoice data="years" localeRef="sYrs" />
				<asp:Literal id="FormDesignListFld" runat="server"></asp:Literal>
				<!-- <listchoice data="MYDATALIST">My Data List</listchoice> -->
			</selections>
		</command>
		<cmd name="cmddsgcalendar" key="calendar" ref="cmdCalendar" />
		<cmd name="cmddsgprop" key="properties" ref="cmdFldProp" />
		<command name="cmdpreviewlist" style="list">
			<caption localeRef="cmdPrevw" />
			<tooltiptext localeRef="cmdPrevw" />
			<listchoice command="cmdpreview" localeRef="mnuFORM" />
			<listchoice command="cmdpreview" data="#F1E1D5" localeRef="cmdViewFld" />
		</command>
		<command name="cmddsgbutton" style="list" maxwidth="12">
			<image key="bbtn" />
			<caption localeRef="cmdBtn" />
			<tooltiptext localeRef="cmdBtn" />
			<selections name="commands">
				<listchoice command="" localeRef="sSelBtn" />
				<listchoice command="cmddsgsubmitbtn" localeRef="frmSBtn" />
				<listchoice command="cmddsgresetbtn" localeRef="frmRBtn" />
				<!-- remove the following listchoice and add your own -->
				<listchoice command="mybtn" localeRef="cmdBtn" />
			</selections>
		</command>
		<cmd name="cmddsgaddfield" key="design" ref="cmdAddFld" />
		<cmd name="cmdvalidxsd" key="validate" ref="cmdVal" />	
		<validation name="plaintext">
			<choice name="none" treeImg="text">
				<caption localeRef="dlgNV8n" />
			</choice>
			<choice name="string-req" treeImg="text">
				<caption localeRef="dlgNBlank" />
				<validate>
					<regexp>/\S+/</regexp>
				</validate>
			</choice>
			<choice name="nonNegInt" treeImg="number">
				<caption localeRef="dlgNNN" />
				<validate>
					<regexp>/^\d*$/</regexp>
				</validate>
			</choice>
			<choice name="nonNegInt-req" treeImg="number">
				<caption localeRef="dlgNNNReqd" />
				<validate>
					<regexp>/^\d+$/</regexp>
				</validate>
			</choice>
			<choice name="decnum" treeImg="number">
				<caption localeRef="sDecN" />
				<validate>
					<regexp pattern="^(\-?(\d*\.)?\d+)$|^$" />
				</validate>
			</choice>
			<choice name="decnum2-req" treeImg="number">
				<caption localeRef="sDecN2Reqd" />
				<calculate>
					<regexp>/(\-?(\d*\.\d{1,2}|\d+))/</regexp> <!-- normalize to 2 decimal digits -->
				</calculate>
				<validate>
					<regexp pattern="\-?(\d*\.\d{1,2}|\d+)" wholeline="true" />
				</validate>
			</choice>
			<choice name="percent-req" treeImg="number">
				<caption localeRef="sPctReqd" />
				<validate>
					<regexp pattern="[0-9]|[1-9][0-9]|100" wholeline="true" />
				</validate>
			</choice>
			<!-- RFC 2822 defines email address format -->
			<choice name="email" treeImg="text" ref="sEmailAddr" pattern="[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*" />
			<!-- semicolon-delimited list of email addresses -->
			<choice name="emailList" treeImg="text" ref="sEmailAddrList" pattern="[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*(\s*\;\s*[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*@[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+(\.[A-Za-z0-9!#-'\*\+\-\/=\?\^_`\{-~]+)*)*" />
			<!-- ZIP code (US) 99999 or 99999-9999 -->
			<choice name="ZipCode" treeImg="text" ref="sZIP" pattern="[0-9]{5}(\-[0-9]{4})?" />
			<!-- Social Security Number (US) 999-99-9999 -->
			<choice name="SSN" treeImg="text" ref="sSSN" pattern="[0-9]{3}\-?[0-9]{2}\-?[0-9]{4}" />
			<!-- Postal Code (Canada) A9A 9A9 -->
			<choice name="PostalCodeCA" treeImg="text" ref="sPCCA" pattern="[A-Z][0-9][A-Z] [0-9][A-Z][0-9]" />
			<!-- Social Insurance Number (Canada) 999 999 999 -->
			<choice name="SIN" treeImg="text" ref="sSIN" pattern="[0-9]{3} ?[0-9]{3} ?[0-9]{3}" />
			<!-- Telephone Number (US and Canada) [+]1 (999) 999-9999 -->
			<choice name="phoneUSCA" treeImg="text" ref="sPhoneUSCA" pattern="((\+?1[\. \-]?)?\(?[2-9][0-9]{2}\)?[\. \-\/]*)?[2-9][0-9]{2}[\. \-]?[0-9]{4}" />
			<!-- RFC 2396 and RFC 2732 defines URL format -->
			<choice name="url" treeImg="hyperlink">
				<caption localeRef="sURL" />
				<!-- don't bother with a reg exp because it allows most anything -->
			</choice>
			<!-- ISBN-10 or ISBN-13 http://www.isbn-international.org/ -->
			<choice name="ISBN" treeImg="text">
				<caption localeRef="sISBN"/>
				<calculate>
					<script value="this.text=design_normalize_isbn(this.text)"></script>
				</calculate>
				<validate>
					<script value="design_validate_isbn(this.text)"></script>
				</validate>
			</choice>
			<!-- ISSN-8 http://www.issn.org/ -->
			<choice name="ISSN" treeImg="text">
				<caption localeRef="sISSN"/>
				<calculate>
					<script value="this.text=design_normalize_issn(this.text)"></script>
				</calculate>
				<validate>
					<script value="design_validate_issn(this.text)"></script>
				</validate>
			</choice>
		</validation>
		<validation name="password">
			<choice name="none" treeImg="password">
				<caption localeRef="dlgNV8n" />
			</choice>
			<choice name="string-req" treeImg="password">
				<caption localeRef="dlgNBlank" />
				<validate>
					<regexp>/\S+/</regexp>
				</validate>
			</choice>
			<choice name="mix8charstr-req" treeImg="password">
				<caption localeRef="sPwd8StrReqd" />
				<validate>
					<script value="this.text.length &gt;= 8 &amp;&amp; /^\w*\d\w*$/.test(this.text)" />
				</validate>
			</choice>
		</validation>
		<validation name="textarea">
			<choice name="none" treeImg="textbox">
				<caption localeRef="dlgNV8n" />
			</choice>
			<choice name="string-req" treeImg="textbox">
				<caption localeRef="dlgNBlank" />
				<validate>
					<regexp>/\S+/</regexp>
				</validate>
			</choice>
			<choice name="max1000Chars" treeImg="textbox">
				<caption localeRef="sMax1000Chars" />
				<validate>
					<script value="this.text.length &lt;= 1000" />
				</validate>
			</choice>
		</validation>
		<validation name="calendar">
			<choice name="none" treeImg="calendar">
				<caption localeRef="dlgNV8n" />
			</choice>
			<choice name="date-req" treeImg="calendar">
				<caption localeRef="dlgNBlank" />
				<validate>
					<regexp pattern="[0-9]{4}\-(0[1-9]|1[0-2])\-(0[1-9]|[1-2][0-9]|3[0-1])" wholeline="true" />
				</validate>
			</choice>
		</validation>
	</datadesign>
<% }
       else if ("xsltdesign" == mode)
           { %>
	<datadesign xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
		<indexing visible="false" enabled="false" />
		<cmd name="cmdxsltdesign" key="design" ref="cmdPDesign" />
		<cmd name="cmddsgmergefield" />
		<command name="cmddsgmergelist">
			<selections name="liststyle">
				<listchoice data="bulletedList" localeRef="cmdBul" />
				<listchoice data="numberedList" localeRef="cmdNumL" />
				<listchoice data="horzTable" localeRef="sHorzTable" />
				<listchoice data="vertTable" localeRef="sVertTable" />
				<listchoice data="headingList" localeRef="sHeadingList" />
				<listchoice data="delimitedList" localeRef="sDelimitedList" />
			</selections>
		</command>
		<cmd name="cmddsgprop" key="properties" ref="cmdFldProp" />
	    <cmd name="cmdvalidxsd" key="validate" ref="cmdVal" />	
		<liststyle name="bulletedList">
			<itemtemplate>
				<ul ektdesignns_list="true">
					<li>
						<ektdesignns_mergefield />
					</li>
				</ul>
			</itemtemplate>
		</liststyle>
		<liststyle name="numberedList">
			<itemtemplate>
				<ol ektdesignns_list="true">
					<li>
						<ektdesignns_mergefield />
					</li>
				</ol>
			</itemtemplate>
		</liststyle>
		<liststyle name="horzTable">
			<itemtemplate>
				<table border="1">
					<tr ektdesignns_list="true">
						<td>
							<ektdesignns_mergefield />
						</td>
					</tr>
				</table>
			</itemtemplate>
		</liststyle>
		<liststyle name="vertTable">
			<itemtemplate>
				<table ektdesignns_list="true" border="1">
					<tr>
						<td>
							<ektdesignns_mergefield />
						</td>
					</tr>
				</table>
			</itemtemplate>
		</liststyle>
		<liststyle name="headingList">
			<itemtemplate>
				<h3>
					<ektdesignns_mergefield />
				</h3>
			</itemtemplate>
		</liststyle>
		<liststyle name="delimitedList">
			<xslt>
				<xsl:for-each select="$xpath">
					<xsl:if test="position() != 1">, </xsl:if>
					<xsl:value-of select="." />
				</xsl:for-each>
			</xslt>
		</liststyle>
	</datadesign>
<%   } %>

<% if ("datadesign" == mode || "formdesign" == mode || "xsltdesign" == mode || "dataentry" == mode)
   { %>
	<dataentry>
		<cmd name="cmddataentry" key="fillin" ref="cmdDEntry" />
		<command name="cmdfilelink">
			<image key="filelink" />
			<caption localeRef="cmdFileLink" />
			<tooltiptext localeRef="cmdFileLink" />
		</command>
		<cmd name="cmdvalidayt" key="validayt" ref="cmdValayt" style="toggle" />			
		<datalist name="languages" src="[eWebEditProPath]/languages.xml" cache="false" select="/select/option" captionxpath="." valuexpath="@value" />
		<datalist name="countries" src="[eWebEditProPath]/countries.xml" cache="false" select="/select/option" captionxpath="." valuexpath="@value" />
		<datalist name="USPS-US" src="[eWebEditProPath]/uspsus.xsd" cache="false" select="/xsd:schema/xsd:simpleType/xsd:restriction/xsd:enumeration" captionxpath="xsd:annotation/xsd:documentation" valuexpath="@value" namespaces="xmlns:xsd='http://www.w3.org/2001/XMLSchema'" validation="select-req">
			<item value="" localeRef="sSel" />
		</datalist>
		<datalist name="USPS-CA" src="[eWebEditProPath]/uspsca.xsd" cache="false" select="/xsd:schema/xsd:simpleType/xsd:restriction/xsd:enumeration" captionxpath="xsd:annotation/xsd:documentation" valuexpath="@value" namespaces="xmlns:xsd='http://www.w3.org/2001/XMLSchema'" validation="select-req">
			<item value="" localeRef="sSel" />
		</datalist>
		<datalist name="ageRange" validation="select-req">
			<item value="" localeRef="sSel" />
			<item value="10">0-15</item>
			<item value="20">16-25</item>
			<item value="30">26-35</item>
			<item value="40">36-45</item>
			<item value="50">46-55</item>
			<item value="60">56-65</item>
			<item value="70">66-75</item>
			<item value="80">76-85</item>
			<item value="90">86+</item>
		</datalist>
		<datalist name="numRange" validation="select-req">
			<item value="" localeRef="sSel" />
			<item value="1">1-10</item>
			<item value="2">11-25</item>
			<item value="3">26-50</item>
			<item value="4">51-100</item>
			<item value="5">101-250</item>
			<item value="6">251-1000</item>
			<item value="7">&gt; 1000</item>
		</datalist>
		<datalist name="years">
			<schema datatype="nonNegativeInteger" />
			<item default="true">2004</item>
			<item>2005</item>
			<item>2006</item>
			<item>2007</item>
			<item>2008</item>
			<item>2009</item>
			<item>2010</item>
			<item>2011</item>
			<item>2012</item>
			<item>2013</item>
			<item>2014</item>
		</datalist>
		<datalist name="gender">
			<item value="M" localeRef="sMale" />
			<item value="F" localeRef="sFemale" />
		</datalist>
		<datalist name="maritalStatus">
			<!-- source: HR-XML Enrollment-2_0.xsd, http://ns.hr-xml.org/Enrollment/Enrollment-2_0 -->
			<item value="Divorced" localeRef="sDivorced" />
            <item value="Legally Separated" localeRef="sLSeparated" />
            <item value="Married" localeRef="sMarried" />
            <item value="Registered Domestic Partner" localeRef="sPartner" />
            <item value="Separated" localeRef="sSeparated" />
            <item value="Single" localeRef="sSingle" />
            <item value="Unmarried" localeRef="sUMarried" />
            <item value="Unreported" localeRef="sUReported" />
            <item value="Widowed" localeRef="sWidowed" />
		</datalist>
		<asp:Literal id="DataLists" runat="server"></asp:Literal>
		<!--
		<datalist name="MYDATALIST" src="[eWebEditProPath]/cmsfolderdatalist.aspx?id=MYFOLDERID&amp;recursive=false&amp;LangType=<%=ContentLanguage %>" cache="false" select="/select/option" captionxpath="." valuexpath="@value" validation="select-req">
			<item value="">(Select)</item>
		</datalist>
		-->
	</dataentry>
<% }   %>
</features>
</config>