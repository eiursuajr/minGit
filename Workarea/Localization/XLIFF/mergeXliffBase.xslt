<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl skl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:ekt="urn:ektron:xliff" xmlns:skl="urn:oasis:names:tc:xliff:document:1.1" 
xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
xmlns:its="http://www.w3.org/2005/11/its">

<!-- run this transform on the xliff file -->

<xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<!-- 'path' is prepended to the external-file href value -->
<xsl:param name="path" select="''"/> <!-- if provided, MUST end in '\' -->

<xsl:variable name="metaDefns" select="document('../../Xslt/metadataDefinitions.xml')"/>

<xsl:template match="skl:file/@original" mode="skl">
	<xsl:param name="xliff-context"/>
	<xsl:attribute name="{name()}">
		<xsl:value-of select="$xliff-context/../@original"/>
	</xsl:attribute>
</xsl:template>

<xsl:template match="skl:file/@source-language" mode="skl">
	<xsl:param name="xliff-context"/>
	<xsl:attribute name="{name()}">
		<xsl:value-of select="$xliff-context/../@source-language"/>
	</xsl:attribute>
</xsl:template>

<xsl:template match="skl:file/@target-language" mode="skl">
	<xsl:param name="xliff-context"/>
	<xsl:attribute name="{name()}">
		<xsl:value-of select="$xliff-context/../@target-language"/>
	</xsl:attribute>
</xsl:template>

<xsl:template match="*" mode="skl">
	<xsl:param name="xliff-context"/>

	<xsl:copy> 
		<xsl:apply-templates select="@*|node()" mode="skl">
			<xsl:with-param name="xliff-context" select="$xliff-context"/>
		</xsl:apply-templates>
	</xsl:copy>
</xsl:template>

<xsl:template match="@*|comment()" mode="skl">
	<xsl:copy-of select="."/>
</xsl:template>

<xsl:template match="skeleton" mode="skl">
	<xsl:param name="xliff-context"/>

	<xsl:apply-templates select="."> <!-- reapply without mode -->
		<xsl:with-param name="xliff-context" select="$xliff-context"/>
	</xsl:apply-templates>
</xsl:template>

<!-- empty HTML tags -->

<xsl:template match="area|bgsound|br|hr|img|input|param">
	<xsl:param name="xliff-context"/>

	<xsl:variable name="id" select="@ekt:id"/>
	<xsl:copy>
		<xsl:call-template name="addAttributes">
			<xsl:with-param name="xliff-context" select="$xliff-context"/>
		</xsl:call-template>
	</xsl:copy>
</xsl:template>

<xsl:template match="*">
	<xsl:param name="xliff-context"/>

	<xsl:copy> 
		<xsl:call-template name="addAttributes">
			<xsl:with-param name="xliff-context" select="$xliff-context"/>
		</xsl:call-template>
		<!-- process attributes separately to maintain consistent position() -->
		<xsl:call-template name="processTransUnit">
			<xsl:with-param name="xliff-context" select="$xliff-context"/>
		</xsl:call-template>
	</xsl:copy>
</xsl:template>

<xsl:template match="ekt:tu">
	<xsl:param name="xliff-context"/>

	<xsl:call-template name="processTransUnit">
		<xsl:with-param name="xliff-context" select="$xliff-context"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="@ekt:id"/>

<xsl:template match="@its:translate"/>

<xsl:template match="ektdesignns_localize">
	<xsl:param name="xliff-context"/>

	<xsl:if test="not(contains(concat(' ',@exclude,' '),concat(' ',$lang,' ')))">
		<xsl:if test="not(@targets) or contains(concat(' ',@targets,' '),concat(' ',$lang,' '))">
			<xsl:apply-templates select="node()">
				<xsl:with-param name="xliff-context" select="$xliff-context"/>
			</xsl:apply-templates>
		</xsl:if>
	</xsl:if>
</xsl:template>

<xsl:template match="@*|comment()">
	<xsl:copy-of select="."/>
</xsl:template>

<xsl:template match="*" mode="xliff">
	<xsl:param name="xliff-context"/>
	<xsl:param name="xhtml-context"/>
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" mode="xliff">
			<xsl:with-param name="xliff-context" select="$xliff-context"/>
			<xsl:with-param name="xhtml-context" select="$xhtml-context"/>
		</xsl:apply-templates>
	</xsl:copy>
</xsl:template>

<xsl:template match="@*|text()" mode="xliff">
	<xsl:copy/>
</xsl:template>

</xsl:stylesheet>
