<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xslout="alias" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:cms="urn:Ektron.Cms.Controls">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" cdata-section-elements="msxsl:script"/>

<xsl:template match="/">
	<xsl:apply-templates/>
</xsl:template>

<!-- remove prototypes -->
<xsl:template match="*[@class='design_prototype']"/>


<xsl:template match="@*|*|processing-instruction()">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="comment()"/>

<xsl:template match="text()[.='&#160;']">
	<xsl:text disable-output-escaping="yes">&amp;#160;</xsl:text>
</xsl:template>

<xsl:template match="*[not(node())]">
	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*"/>
	</xsl:element>
</xsl:template>


</xsl:stylesheet>