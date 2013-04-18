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
	<xsl:variable name="sNoItems" select="$localeXml/data[@name='sNoItems']/value/text()"/>

	<xsl:template match="/"> <!-- xml:space="preserve" breaks xsl:choose Saxon 6.5.5 & FireFox -->
  
  	<xsl:choose>
		<xsl:when test="count(/select/option | /div/ol/li)=0">
			<div class="noitems">
				<xsl:value-of select="$sNoItems"/>
			</div>
		</xsl:when>
		<xsl:otherwise>
<table id="select" title="{$sSelect}" summary="{$sSelect}" border="0" ektdesignns_name="select" ektdesignns_caption="{$sSelect}" ektdesignns_nodetype="element" ektdesignns_role="root">
<thead>
	<th class="label"><xsl:value-of select="$sSelected"/></th> 
	<th class="label"><xsl:value-of select="$sDispTxt"/></th> 
	<th class="label"><xsl:value-of select="$sValue"/></th> 
	<th class="label"><xsl:value-of select="$sDisabled"/></th> 
</thead>
<tbody ektdesignns_list="true">
	<xsl:for-each select="/div/ol/li">
	<tr title="{$sOption}" ektdesignns_name="option" ektdesignns_caption="{$sOption}" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
		<td>
			<input disabled="disabled" xml:space="default" id="selected{generate-id(./input/@checked)}" title="{$sSelected}" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="{$sSelected}" ektdesignns_nodetype="attribute">
				<xsl:if test="./input/@checked='checked' or ./input/@checked='true' or ./input/@checked='1'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if></input>
		</td>
		<td>
			<input disabled="disabled" class="RadETextBox" id="Text{generate-id(./label/text())}" title="{$sText}" alt="{$sText}" ektdesignns_name="Text" ektdesignns_caption="{$sText}" ektdesignns_nodetype="text" ektdesignns_datatype="string" ektdesignns_basetype="text" value="{./label/text()}"/> 
		</td>
		<td>
			<input disabled="disabled" class="RadETextBox" id="value{generate-id(./input/@value)}" title="{$sValue}" alt="{$sValue}" ektdesignns_name="value" ektdesignns_caption="{$sValue}" ektdesignns_nodetype="attribute" ektdesignns_datatype="string" ektdesignns_basetype="text" value="{./input/@value}"/> 
		</td>
		<td>
			<input disabled="disabled" xml:space="default" id="disabled{generate-id(./input/@disabled)}" title="{$sDisabled}" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="{$sDisabled}" ektdesignns_nodetype="attribute">
				<xsl:if test="./input/@disabled='disabled' or ./input/@disabled='true' or ./input/@disabled='1'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</input>
		</td>
	</tr>
	</xsl:for-each>

	<xsl:for-each select="/select/option">
	<tr title="{$sOption}" ektdesignns_name="option" ektdesignns_caption="{$sOption}" ektdesignns_nodetype="element" ektdesignns_maxoccurs="unbounded">
		<td>
			<input disabled="disabled" xml:space="default" id="selected{generate-id(./@selected)}" title="{$sSelected}" type="checkbox" ektdesignns_name="selected" ektdesignns_caption="{$sSelected}" ektdesignns_nodetype="attribute">
				<xsl:if test="./@selected='true' or ./@selected='1'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if></input>
		</td>
		<td>
			<input disabled="disabled" class="RadETextBox" id="Text{generate-id(./text())}" title="{$sText}" alt="{$sText}" ektdesignns_name="Text" ektdesignns_caption="{$sText}" ektdesignns_nodetype="text" ektdesignns_datatype="string" ektdesignns_basetype="text" value="{./text()}"/> 
		</td>
		<td>
			<input disabled="disabled" class="RadETextBox" id="value{generate-id(./@value)}" title="{$sValue}" alt="{$sValue}" ektdesignns_name="value" ektdesignns_caption="{$sValue}" ektdesignns_nodetype="attribute" ektdesignns_datatype="string" ektdesignns_basetype="text" value="{./@value}"/> 
		</td>
		<td>
			<input disabled="disabled" xml:space="default" id="disabled{generate-id(./@disabled)}" title="{$sDisabled}" type="checkbox" ektdesignns_name="disabled" ektdesignns_caption="{$sDisabled}" ektdesignns_nodetype="attribute">
				<xsl:if test="./@disabled='true' or ./@disabled='1'">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</input>
		</td>
	</tr>
	</xsl:for-each>

</tbody>
</table>
		</xsl:otherwise>
	</xsl:choose>

</xsl:template>
</xsl:stylesheet>