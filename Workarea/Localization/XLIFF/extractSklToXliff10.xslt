<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl skl its ekt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:ekt="urn:ektron:xliff" xmlns:its="http://www.w3.org/2005/11/its"
xmlns:skl="urn:oasis:names:tc:xliff:document:1.1">

<!-- run this transform on the xhtml skeleton (skl) file -->

<xsl:import href="extractSklToXliffBase.xslt" />

<xsl:param name="prefilltarget">true</xsl:param> <!-- Trados requires target element -->

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"
 doctype-public="-//XLIFF//DTD XLIFF//EN" 
 doctype-system="http://www.oasis-open.org/committees/xliff/documents/xliff.dtd"/>

<xsl:variable name="newline">
<xsl:text>
</xsl:text>
</xsl:variable>

<xsl:template match="/">
	<xliff version="1.0">
		<xsl:apply-templates select="skl:xliff/*" mode="skl"/>
		<xsl:value-of select="$newline"/>
	</xliff>
</xsl:template>

<!-- Use mode="skl" so text() nodes are copied rather than converted to a trans-unit -->

<xsl:template match="skl:file" mode="skl">
	<xsl:value-of select="$newline"/>
	<file datatype="xml" original="{$original}" source-language="{$sourcelang}" target-language="{$targetlang}">
		<xsl:apply-templates mode="skl"/>
		<xsl:value-of select="$newline"/>
	</file>
</xsl:template>

<xsl:template match="skl:header" mode="skl">
	<xsl:value-of select="$newline"/>
	<header>
		<xsl:value-of select="$newline"/>
		<skl>
			<external-file href="{$original}"/>
		</skl>
		<xsl:apply-templates mode="skl"/>
		<!--
		This is a low estimate, which could cause friction between 
		customers and translation service providers.
		<xsl:call-template name="buildCountGroup">
			<xsl:with-param name="text" select="../skl:body"/>
		</xsl:call-template>
		-->
		<xsl:value-of select="$newline"/>
	</header>
</xsl:template>

<xsl:template match="skl:body" mode="skl">
	<xsl:value-of select="$newline"/>
	<body>
		<xsl:apply-templates mode="skl"/>
		<xsl:value-of select="$newline"/>
	</body>
</xsl:template>

<xsl:template match="skl:tool" mode="skl">
	<xsl:value-of select="$newline"/>
	<prop-group name="{@tool-id}">
		<xsl:for-each select="*">
		<xsl:value-of select="$newline"/>
		<prop prop-type="{@name}"><xsl:copy-of select="node()"/></prop>
		</xsl:for-each>
		<xsl:value-of select="$newline"/>
	</prop-group>
</xsl:template>

<xsl:template match="skl:*" mode="skl">
	<!-- copy without namespace -->
	<xsl:value-of select="$newline"/>
	<xsl:element name="{local-name()}">
		<xsl:apply-templates select="@*[name()!='ts' and namespace-uri()='']" mode="skl"/>
		<xsl:apply-templates mode="skl"/>
	</xsl:element>
</xsl:template>

<xsl:template match="skl:group/@maxwidth|skl:group/@size-unit" mode="skl"/>

<xsl:template match="skl:phase/@tool-id" mode="skl">
	<xsl:attribute name="tool"><xsl:value-of select="."/></xsl:attribute>
</xsl:template>

<xsl:template match="@*" mode="skl">
	<xsl:copy/>
</xsl:template>


<xsl:template name="buildElementTransUnit">
	<xsl:param name="id"/>
	<xsl:param name="datatype" select="'plaintext'"/>
	<xsl:param name="restype"/>

	<xsl:variable name="source"><xsl:apply-templates mode="source"/></xsl:variable>
	<xsl:value-of select="$newline"/>
	<trans-unit id="{$id}" datatype="{$datatype}">
		<xsl:if test="string-length($restype) &gt; 0 and not(starts-with($restype,'x-'))">
			<xsl:attribute name="restype"><xsl:value-of select="$restype"/></xsl:attribute>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="local-name()='textarea'">
				<xsl:attribute name="size-unit">x-cell</xsl:attribute>
				<xsl:attribute name="xml:space">preserve</xsl:attribute>
			</xsl:when>
		</xsl:choose>
		<xsl:value-of select="$newline"/>
		<source><xsl:copy-of select="$source"/></source>
		<xsl:if test="$prefilltarget='true'">
			<xsl:value-of select="$newline"/>
			<target><xsl:copy-of select="$source"/></target>
		</xsl:if>
		<!--
		<xsl:call-template name="buildCountGroup">
			<xsl:with-param name="text" select="$source"/>
		</xsl:call-template>
		-->
		<xsl:value-of select="$newline"/>
	</trans-unit>
	<xsl:apply-templates select="descendant::*/@*"/>
</xsl:template>

<xsl:template name="buildAttributeTransUnit">
	<xsl:variable name="id" select="concat((((ancestor::*)[@ekt:id])[last()])/@ekt:id,'.',count(preceding::*),'-attr-',local-name())"/>
	<xsl:value-of select="$newline"/>
	<trans-unit id="{$id}" datatype="plaintext" resname="{local-name()}">
		<xsl:choose>
			<xsl:when test="local-name()='accesskey'">
				<xsl:attribute name="size-unit">char</xsl:attribute>
				<xsl:attribute name="maxwidth">1</xsl:attribute>
			</xsl:when>
			<xsl:when test="local-name()='label'">
				<xsl:attribute name="restype">label</xsl:attribute>
			</xsl:when>
			<xsl:when test="local-name()='src' and local-name(..)='img'">
				<xsl:variable name="width">
					<xsl:choose>
						<xsl:when test="../@width"><xsl:value-of select="../@width"/></xsl:when>
						<xsl:otherwise>#</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:variable name="height">
					<xsl:choose>
						<xsl:when test="../@height"><xsl:value-of select="../@height"/></xsl:when>
						<xsl:otherwise>#</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:attribute name="coord"><xsl:value-of select="concat('#;#;',$width,';',$height)"/></xsl:attribute>
			</xsl:when>
		</xsl:choose>
		<xsl:value-of select="$newline"/>
		<source><xsl:value-of select="."/></source>
		<xsl:if test="$prefilltarget='true'">
			<xsl:value-of select="$newline"/>
			<target><xsl:value-of select="."/></target>
		</xsl:if>
		<!-- <xsl:call-template name="buildCountGroup"/> -->
		<xsl:value-of select="$newline"/>
	</trans-unit>
</xsl:template>

<xsl:template name="buildTextTransUnit">
	<xsl:param name="id"/>
	<xsl:variable name="source" select="normalize-space(.)"/>
	<xsl:if test="string-length($source) &gt; 0">
		<xsl:value-of select="$newline"/>
		<trans-unit id="{$id}" datatype="plaintext">
			<xsl:value-of select="$newline"/>
			<source><xsl:copy-of select="$source"/></source>
			<xsl:if test="$prefilltarget='true'">
				<xsl:value-of select="$newline"/>
				<target><xsl:copy-of select="$source"/></target>
			</xsl:if>
			<!-- <xsl:call-template name="buildCountGroup"/> -->
			<xsl:value-of select="$newline"/>
		</trans-unit>
	</xsl:if>
</xsl:template>

<xsl:template name="buildBinTransUnit">
	<xsl:param name="id"/>
	<xsl:param name="mimeType"/>
	<xsl:param name="fileName"/>
	<xsl:if test="string-length($fileName) &gt; 0">
		<xsl:value-of select="$newline"/>
		<bin-unit id="{$id}" mime-type="{$mimeType}">
			<xsl:value-of select="$newline"/>
			<bin-source>
				<xsl:value-of select="$newline"/>
				<external-file href="{$fileName}"/>
				<xsl:value-of select="$newline"/>
			</bin-source>
			<xsl:if test="$prefilltarget='true'">
				<xsl:value-of select="$newline"/>
				<bin-target>
					<xsl:value-of select="$newline"/>
					<external-file href="{$fileName}"/>
					<xsl:value-of select="$newline"/>
				</bin-target>
			</xsl:if>
			<!-- <xsl:if test="/skl:xliff/skl:file/skl:header/skl:reference">
				<xsl:value-of select="$newline"/>
				<note annotates="source" from="Ektron extraction tool" xml:lang="en-US">
					<xsl:text>This file is generated from the referenced file(s).</xsl:text>
				</note>
			</xsl:if> -->
			<xsl:value-of select="$newline"/>
		</bin-unit>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
