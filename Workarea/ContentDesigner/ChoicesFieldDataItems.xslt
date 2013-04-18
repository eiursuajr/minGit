<?xml version="1.0"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl js dl" xmlns:js="urn:custom-javascript" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:dl="urn:datalist">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>
	<xsl:strip-space elements="*"/>
	<xsl:param name="baseURL" select="''"/>
	<xsl:param name="LangType" select="''"/>
	<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=ChoicesFieldDialog&amp;LangType=',$LangType)"/>

	<xsl:variable name="localeXml" select="document($localeUrl)/*" />

	<xsl:variable name="sOption" select="$localeXml/data[@name='sOption']/value/text()"/>
	<xsl:variable name="sChecked" select="$localeXml/data[@name='sChecked']/value/text()"/>
	<xsl:variable name="sDisabled" select="$localeXml/data[@name='sDisabled']/value/text()"/>
	<xsl:variable name="sSelect" select="$localeXml/data[@name='sSelect']/value/text()"/>
	<xsl:variable name="sSelected" select="$localeXml/data[@name='sSelected']/value/text()"/>
	<xsl:variable name="sValue" select="$localeXml/data[@name='sValue']/value/text()"/>
	<xsl:variable name="sDispTxt" select="$localeXml/data[@name='sDispTxt']/value/text()"/>
	<xsl:variable name="sText" select="$localeXml/data[@name='sText']/value/text()"/>
	<xsl:variable name="sCantBeBlank" select="$localeXml/data[@name='sCantBeBlank']/value/text()"/>

	<xsl:template match="/" xml:space="preserve">
  
<table id="select" title="{$sSelect}" summary="{$sSelect}" border="0" ektdesignns_name="select" ektdesignns_caption="{$sSelect}" ektdesignns_nodetype="element" ektdesignns_role="root">
<thead>
	<th> </th> 
	<th class="label"><xsl:value-of select="$sSelected"/></th> 
	<th class="label"><xsl:value-of select="$sDispTxt"/></th> 
	<th class="label"><xsl:value-of select="$sValue"/></th> 
	<th class="label"><xsl:value-of select="$sDisabled"/></th> 
</thead>
<tfoot class="design_prototype">
	<tr onclick="design_row_setCurrent(event, this)">
		<td colspan="4" unselectable="on">
			<a href="#" onclick="design_row_setCurrent(event, $ektron(this).parent().parent().get(0));design_row_replace();return false;" menutype="button" class="design_dynlist_menu">
			<img class="design_add_graphic" menutype="button" src="[skinpath]additem.gif" width="9" height="9" border="0"/> 
			<xsl:value-of select="$sOption"/>
			</a>
		</td>
	</tr>
	<tr onclick="design_row_setCurrent(event, this)" id="option" title="{$sOption}" ektdesignns_name="option" ektdesignns_caption="{$sOption}" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
		<td class="design_dynlist_first_normal" unselectable="on">
			<a href="#" onclick="design_row_showContextMenu(event, this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(event, this)" onmouseout="design_row_onmouse(event, this)" title="{$sOption}">
			<img class="design_contextmenu_button" menutype="button" src="[skinpath]designmenu.gif" width="11" height="16" border="0"/> 
			</a>
		</td>
		<td>  
			<input xml:space="default" id="selected" title="{$sSelected}" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="{$sSelected}" ektdesignns_nodetype="attribute"/> 
		</td>
		<td>
			<input class="RadETextBox" id="Text" onblur="design_validate_re(/\S+/,this,'{$sCantBeBlank}');" title="{$sText}" alt="{$sText}" maxlength="1500" ektdesignns_name="Text" ektdesignns_caption="{$sText}" ektdesignns_nodetype="text" ektdesignns_validation="string-req" ektdesignns_datatype="string" ektdesignns_basetype="text" ektdesignns_schema="&lt;xs:minLength value='1'/&gt;" ektdesignns_validate="re:/\S+/" ektdesignns_invalidmsg="{$sCantBeBlank}"/> 
		</td>
		<td>
			<input class="RadETextBox" id="value" onblur="validateChoicesFieldItemValue(this);" title="{$sValue}" alt="{$sValue}" maxlength="50" ektdesignns_name="value" ektdesignns_caption="{$sValue}" ektdesignns_nodetype="attribute" ektdesignns_validation="choicesField" /> 
		</td>
		<td>
			<input xml:space="default" id="disabled" title="{$sDisabled}" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="{$sDisabled}" ektdesignns_nodetype="attribute"/> 
		</td>
	</tr>
</tfoot>
<tbody ektdesignns_list="true">
	<xsl:for-each select="/div/ol/li">
	<tr onclick="design_row_setCurrent(event, this)" title="{$sOption}" ektdesignns_name="option" ektdesignns_caption="{$sOption}" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
		<td class="design_dynlist_first_normal" unselectable="on">
			<a href="#" onclick="design_row_showContextMenu(event, this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(event, this)" onmouseout="design_row_onmouse(event, this)" title="{$sOption}">
			<img class="design_contextmenu_button" menutype="button" src="[skinpath]designmenu.gif" width="11" height="16" border="0"/> 
			</a>
		</td>
		<td>
			<input xml:space="default" id="selected{generate-id(./input)}" title="{$sSelected}" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="{$sSelected}" ektdesignns_nodetype="attribute">
								<xsl:if test="./input/@checked='checked' or ./input/@checked='true' or ./input/@checked='1'">
									<xsl:attribute name="checked">checked</xsl:attribute>
								</xsl:if></input>
		</td>
		<td>
			<input class="RadETextBox" id="Text{generate-id(./label/text())}" onblur="design_validate_re(/\S+/,this,'{$sCantBeBlank}');" title="{$sText}" alt="{$sText}" maxlength="1500" ektdesignns_name="Text" ektdesignns_caption="{$sText}" ektdesignns_nodetype="text" ektdesignns_validation="string-req" ektdesignns_datatype="string" ektdesignns_basetype="text" ektdesignns_schema="&lt;xs:minLength value='1'/&gt;" ektdesignns_validate="re:/\S+/" ektdesignns_invalidmsg="{$sCantBeBlank}" value="{./label/text()}"/> 
		</td>
		<td>
			<input class="RadETextBox" id="value{generate-id(./input/@value)}" onblur="validateChoicesFieldItemValue(this);" title="{$sValue}" alt="{$sValue}" maxlength="50" ektdesignns_name="value" ektdesignns_caption="{$sValue}" ektdesignns_nodetype="attribute" ektdesignns_validation="choicesField" value="{./input/@value}"/> 
		</td>
		<td>
			<input xml:space="default" id="disabled{generate-id(./input)}" title="{$sDisabled}" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="{$sDisabled}" ektdesignns_nodetype="attribute">
				<xsl:if test="./input/@disabled='disabled' or ./input/@disabled='true' or ./input/@disabled='1'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</input>
		</td>
	</tr>
	</xsl:for-each>

	<xsl:for-each select="/select/option">
	<tr onclick="design_row_setCurrent(event, this)" title="{$sOption}" ektdesignns_name="option" ektdesignns_caption="{$sOption}" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
		<td class="design_dynlist_first_normal" unselectable="on">
			<a href="#" onclick="design_row_showContextMenu(event, this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(event, this)" onmouseout="design_row_onmouse(event, this)" title="{$sOption}">
			<img class="design_contextmenu_button" menutype="button" src="[skinpath]designmenu.gif" width="11" height="16" border="0"/> 
			</a>
		</td>
		<td>
			<input xml:space="default" id="selected{generate-id(.)}" title="{$sSelected}" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="{$sSelected}" ektdesignns_nodetype="attribute">
								<xsl:if test="./@selected='selected' or ./@selected='true' or ./@selected='1'">
									<xsl:attribute name="checked">checked</xsl:attribute>
								</xsl:if></input>
		</td>
		<td>
			<input class="RadETextBox" id="Text{generate-id(./text())}" onblur="design_validate_re(/\S+/,this,'{$sCantBeBlank}');" title="{$sText}" alt="{$sText}" maxlength="1500" ektdesignns_name="Text" ektdesignns_caption="{$sText}" ektdesignns_nodetype="text" ektdesignns_validation="string-req" ektdesignns_datatype="string" ektdesignns_basetype="text" ektdesignns_schema="&lt;xs:minLength value='1'/&gt;" ektdesignns_validate="re:/\S+/" ektdesignns_invalidmsg="{$sCantBeBlank}" value="{./text()}"/> 
		</td>
		<td>
			<input class="RadETextBox" id="value{generate-id(./@value)}" onblur="validateChoicesFieldItemValue(this);" title="{$sValue}" alt="{$sValue}" maxlength="50" ektdesignns_name="value" ektdesignns_caption="{$sValue}" ektdesignns_nodetype="attribute" ektdesignns_validation="choicesField" value="{./@value}"/> 
		</td>
		<td>
			<input xml:space="default" id="disabled{generate-id(.)}" title="{$sDisabled}" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="{$sDisabled}" ektdesignns_nodetype="attribute">
				<xsl:if test="./@disabled='disabled' or ./@disabled='true' or ./@disabled='1'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</input>
		</td>
	</tr>
	</xsl:for-each>

	<xsl:if test="count(/select/option | /div/ol/li)=0">
	<tr onclick="design_row_setCurrent(event, this)" title="{$sOption}" ektdesignns_name="option" ektdesignns_caption="{$sOption}" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
		<td class="design_dynlist_first_normal" unselectable="on">
			<a href="#" onclick="design_row_showContextMenu(event, this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(event, this)" onmouseout="design_row_onmouse(event, this)" title="{$sOption}">
			<img class="design_contextmenu_button" menutype="button" src="[skinpath]designmenu.gif" width="11" height="16" border="0"/> 
			</a>
		</td>
		<td>
			<input xml:space="default" id="selected" title="{$sSelected}" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="{$sSelected}" ektdesignns_nodetype="attribute"/> 
		</td>
		<td>
			<input class="RadETextBox" id="Text" onblur="design_validate_re(/\S+/,this,'{$sCantBeBlank}');" title="{$sText}" alt="{$sText}" maxlength="1500" ektdesignns_name="Text" ektdesignns_caption="{$sText}" ektdesignns_nodetype="text" ektdesignns_validation="string-req" ektdesignns_datatype="string" ektdesignns_basetype="text" ektdesignns_schema="&lt;xs:minLength value='1'/&gt;" ektdesignns_validate="re:/\S+/" ektdesignns_invalidmsg="{$sCantBeBlank}"/> 
		</td>
		<td>
			<input class="RadETextBox" id="value" onblur="validateChoicesFieldItemValue(this);" title="{$sValue}" alt="{$sValue}" maxlength="50" ektdesignns_name="value" ektdesignns_caption="{$sValue}" ektdesignns_nodetype="attribute" ektdesignns_validation="choicesField" /> 
		</td>
		<td>
			<input xml:space="default" id="disabled" title="{$sDisabled}" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="{$sDisabled}" ektdesignns_nodetype="attribute"/> 
		</td>
	</tr>
	</xsl:if>

	<tr onclick="design_row_setCurrent(event, this)">
		<td colspan="5" unselectable="on">
			<a href="#" onclick="design_row_setCurrent(event, this.parentElement.parentElement);design_row_insertAbove();return false;" menutype="button" class="design_dynlist_menu">
			<img class="design_add_graphic" menutype="button" src="[skinpath]additem.gif" width="9" height="9" border="0"/> 
			<xsl:value-of select="$sOption"/> 
			</a>
		</td>
	</tr>
</tbody>
</table>

</xsl:template>
</xsl:stylesheet>