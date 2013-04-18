<?xml version='1.0'?>
<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" extension-element-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:xslout="alias" xmlns:msxslout="aliasms">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="no"/>
<xsl:namespace-alias stylesheet-prefix="xslout" result-prefix="xsl"/>
<xsl:namespace-alias stylesheet-prefix="msxslout" result-prefix="msxsl"/>

<!--

Run on a field list.

Outputs an XSLT that will produce a field list when run on any XML (e.g., <root/>) 

<fieldlist>
	<field name="fieldname1" datatype="validationName1" content="type1" xpath="/root/group/element1" title="title1" indexed="true">display name1</field>
	<field name="fieldname2" datatype="selection" datalist="uniqueId1" content="type2" xpath="/root/group/element2"  title="title2">display name1</field>
	...
	<field name="fieldnamen" datatype="validationNamen" content="typen" xpath="/root/group/elementn" title="titlen">display namen</field>
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

-->

<xsl:variable name="fieldlistXPath"/>

<xsl:template match="/">
	<xslout:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl js dl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:dl="urn:datalist">

		<xslout:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>
		<xslout:strip-space elements="*"/>

		<xsl:call-template name="buildDatalists"/>

		<xslout:template match="/">
			<xsl:attribute name="xml:space">preserve</xsl:attribute>
			<xsl:apply-templates/>
		</xslout:template>
	</xslout:stylesheet>
</xsl:template>

<xsl:template match="datalist[@ektdesignns_datasrc]">
	<xsl:variable name="captionxpath">
		<xsl:call-template name="getCaptionXPath"/>
	</xsl:variable>
	<xsl:variable name="valuexpath">
		<xsl:call-template name="getValueXPath"/>
	</xsl:variable>
	<xsl:call-template name="beginDatalistAccess"/>
	<datalist name="{@name}">
		<xslout:for-each select="$dl">
			<item value="{{{$valuexpath}}}">
				<xslout:value-of select="{$captionxpath}"/>
				<!--<xslout:copy-of select="{$captionxpath}/node()"/>-->
			</item>
		</xslout:for-each>
	</datalist>
	<xsl:call-template name="endDatalistAccess"/>		
</xsl:template>

<xsl:include href="template_datalist.xslt"/>
<xsl:include href="template_identityToXSLT.xslt"/>
<xsl:include href="template_copyData.xslt"/>
<xsl:include href="template_getDesignName.xslt"/>

</xsl:stylesheet>