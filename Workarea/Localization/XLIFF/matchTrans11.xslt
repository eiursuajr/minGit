<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns="urn:oasis:names:tc:xliff:document:1.1"
xmlns:xlf="urn:oasis:names:tc:xliff:document:1.1" xmlns:ekt="urn:ektron:xliff"
extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl xlf" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<!-- only effective if same XLIFF namespace (ie, version) between both XLIFF files -->

<xsl:param name="previousXliffFile"/>
<xsl:param name="onMatch" select="'add-alt-trans'"/> <!-- 'omit-trans-unit', 'add-alt-trans' or '' -->

<xsl:variable name="previous" select="msxsl:node-set(document($previousXliffFile))"/>

<xsl:variable name="toolId" select="/xlf:xliff/xlf:file/xlf:header/xlf:tool/@tool-id"/>
<xsl:variable name="phaseName" select="/xlf:xliff/xlf:file/xlf:header/xlf:phase-group/xlf:phase/@phase-name"/>

<xsl:template match="/">
	<xsl:apply-templates/>
</xsl:template>

<xsl:template match="xlf:trans-unit">
	<xsl:variable name="match" select="$previous/xlf:xliff/xlf:file/xlf:body//xlf:trans-unit[xlf:source=current()/xlf:source]"/>
	<xsl:variable name="prevTranslation" select="$match[1]/xlf:target"/>
	<xsl:if test="not($onMatch='omit-trans-unit' and $prevTranslation)">
		<xsl:copy>
			<xsl:apply-templates select="@*"/>
			<xsl:apply-templates select="xlf:*"/>
			<xsl:if test="$onMatch='add-alt-trans' and $prevTranslation">
				<alt-trans tool-id="{$toolId}">
					<target state="translated" state-qualifier="exact-match">
						<xsl:apply-templates select="$prevTranslation/@*"/>
						<xsl:apply-templates select="$prevTranslation/node()"/>
					</target>
				</alt-trans>
			</xsl:if>
			<xsl:apply-templates select="*[namespace-uri()!='urn:oasis:names:tc:xliff:document:1.1']"/>
		</xsl:copy>
	</xsl:if>
</xsl:template>

<xsl:template match="*">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="@*|text()">
	<xsl:copy/>
</xsl:template>

</xsl:stylesheet>