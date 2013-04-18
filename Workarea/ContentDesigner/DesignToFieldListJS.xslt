<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:import href="DesignToFieldList.xslt"/>

<xsl:output method="text" version="1.0" encoding="UTF-8" indent="yes"/>

<!--
from DesignToFieldList.xslt:
<fieldlist>
	<field name="fieldname1" datatype="validationName1" basetype="number" content="type1" xpath="/root/group/element1" minoccurs="0" maxoccurs="unbounded">display name1</field>
	<field name="fieldname2" datatype="selection" basetype="text" datalist="uniqueId1" content="type2" xpath="/root/group/element1">display name1</field>
	...
	<field name="fieldnamen" datatype="validationNamen" content="typen" xpath="/root/group/elementn"/>display namen</field>
	- Note: datalist is same as in configdataentryfeature -
	<datalist name="uniqueId1">
		<item value="1">displayValue1</item>
		:
		<item value="n">displayValuen</item>
	</datalist>
	:
	<datalist name="uniqueIdn">
		<item value="1">displayValue1</item>
		:
		<item value="n">displayValuen</item>
	</datalist>
</fieldlist>

Outputs 
{
	fields:
	[
		{name:"fieldname1",displayName:"display name1",datatype:"validationName1",basetype:"number",xpath:"/root/group/element1",content:"type1",minoccurs:"0",maxoccurs:"unbounded"},
		{name:"fieldname1",displayName:"display name2",datatype:"selection",basetype:"text",datalist="uniqueID1",xpath:"/root/group/element2",content:"type2",minoccurs:"1",maxoccurs:"1"},
		{...},
		...,
		{name:"fieldnamen",displayName:"display nameN",datatype:"validationNamen",xpath:"/root/group/elementn",content:"typeN",minoccurs:"1",maxoccurs:"1"}
	],
	datalists:
	[
		{
			name:"uniqueID1",
			dataItem:
			[
				{value:"itemvalue",displayValue:""},
				...,
				{value:"itemvalue",displayValue:""}
			]
		},
		{
			name:"uniqueID1",
			dataItem:[],
			datasrc:"dataSrcUrl",
			dataselect:"dataSelectXPath",
			captionxpath:"captionXPath",
			valuexpath:"valueXPath",
			datanamespaces:"dataNamespacesList"
		},
		...,
		{
			name:"uniqueID2",
			dataItem:
			[
				{value:"itemvalue",displayValue:""},
				...,
				{value:"itemvalue",displayValue:""}
			]
		}
	]
}


-->

<xsl:template match="/">
	<xsl:variable name="list">
		<xsl:apply-imports/>
	</xsl:variable>
	
	<!--<xsl:copy-of select="$list"/>
	<xsl:text>
	===========================
	</xsl:text>-->
	
	<!--<xsl:variable name="dataItem">
		<xsl:for-each select="msxsl:node-set($list)/*/datalist/item[ancestor::datalist/@name=current()/@name]">
			<xsl:if test="position() != 1">,</xsl:if>
			<xsl:value-of disable-output-escaping="yes" select="concat('{value:&quot;',@value,'&quot;,displayValue:&quot;',node(),'&quot;}')"/>	
		</xsl:for-each>
	</xsl:variable>-->

	<xsl:text>{fields:[</xsl:text>
	<xsl:for-each select="msxsl:node-set($list)/*/field">
		<xsl:value-of select="'&#13;&#10;'"/>
		<xsl:if test="position() != 1">,</xsl:if>
		<xsl:value-of disable-output-escaping="yes" select="concat('{name:&quot;',@name,'&quot;,displayName:&quot;',text(),'&quot;,datatype:&quot;',@datatype,'&quot;,basetype:&quot;',@basetype,'&quot;,content:&quot;',@content,'&quot;,xpath:&quot;',@xpath,'&quot;')"/>	
		<xsl:if test="@indexed != ''"><xsl:value-of disable-output-escaping="yes" select="concat(',indexed:&quot;',@indexed,'&quot;')"/></xsl:if>
		<xsl:if test="@datalist != ''"><xsl:value-of disable-output-escaping="yes" select="concat(',datalist:&quot;',@datalist,'&quot;')"/></xsl:if>
		<xsl:if test="@minoccurs"><xsl:value-of disable-output-escaping="yes" select="concat(',minoccurs:&quot;',@minoccurs,'&quot;')"/></xsl:if>
		<xsl:if test="not(@minoccurs)"><xsl:value-of disable-output-escaping="yes" select="',minoccurs:&quot;1&quot;'"/></xsl:if>
		<xsl:if test="@maxoccurs"><xsl:value-of disable-output-escaping="yes" select="concat(',maxoccurs:&quot;',@maxoccurs,'&quot;')"/></xsl:if>
		<xsl:if test="not(@maxoccurs)"><xsl:value-of disable-output-escaping="yes" select="',maxoccurs:&quot;1&quot;'"/></xsl:if>
		<xsl:text>}</xsl:text>
	</xsl:for-each>
	<xsl:value-of select="'&#13;&#10;'"/>
	<xsl:text>],datalists:[</xsl:text>
	<xsl:for-each select="msxsl:node-set($list)/*/datalist">
		<xsl:value-of select="'&#13;&#10;'"/>
		<xsl:if test="position() != 1">,</xsl:if>
		<xsl:variable name="dataItem">
			<xsl:for-each select="item">
				<xsl:value-of select="'&#13;&#10;&#9;'"/>
				<xsl:if test="position() != 1">,</xsl:if>
				<xsl:value-of disable-output-escaping="yes" select="concat('{value:&quot;',@value,'&quot;,displayValue:&quot;',node(),'&quot;}')"/>	
			</xsl:for-each>
		</xsl:variable>
		<xsl:value-of disable-output-escaping="yes" select="concat('{name:&quot;',@name,'&quot;,dataItem:[',$dataItem,']')"/>
		<xsl:if test="@ektdesignns_datasrc"><xsl:value-of disable-output-escaping="yes" select="concat(',datasrc:&quot;',@ektdesignns_datasrc,'&quot;')"/></xsl:if>
		<xsl:if test="@ektdesignns_dataselect"><xsl:value-of disable-output-escaping="yes" select="concat(',dataselect:&quot;',@ektdesignns_dataselect,'&quot;')"/></xsl:if>
		<xsl:if test="@ektdesignns_captionxpath"><xsl:value-of disable-output-escaping="yes" select="concat(',captionxpath:&quot;',@ektdesignns_captionxpath,'&quot;')"/></xsl:if>
		<xsl:if test="@ektdesignns_valuexpath"><xsl:value-of disable-output-escaping="yes" select="concat(',valuexpath:&quot;',@ektdesignns_valuexpath,'&quot;')"/></xsl:if>
		<xsl:if test="@ektdesignns_datanamespaces"><xsl:value-of disable-output-escaping="yes" select="concat(',datanamespaces:&quot;',@ektdesignns_datanamespaces,'&quot;')"/></xsl:if>
		<xsl:text>}</xsl:text>
	</xsl:for-each>
	<xsl:text>]}</xsl:text>
</xsl:template>


</xsl:stylesheet>