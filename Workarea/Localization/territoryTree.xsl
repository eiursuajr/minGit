<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- Transform on CLDR/common/main/{$lang}.xml -->

<xsl:output method="html" version="1.0" indent="yes" encoding="utf-8"/>

<xsl:param name="countriesInTree" select="false()"/>

<xsl:include href="includeForList.xsl"/>

<xsl:key name="territory" match="/ldml/localeDisplayNames/territories/territory" use="@type"/>

<xsl:template match="/">
	<ul lang="{$lang}">
		<xsl:apply-templates select="key('territory','001')" mode="tree"/> <!-- World -->
	</ul>
</xsl:template>

<xsl:template match="territory[not(@alt)]" mode="tree">
	<!-- Cyprus (CY) is in the EU (European Union), but not always considered in Europe -->
	<xsl:if test="($countriesInTree) or (not($countriesInTree) and $data/territoryContainment/group[@type=current()/@type])">
		<xsl:variable name="contained_rtf">
			<xsl:apply-templates select="../territory[contains($data/territoryContainment/group[@type=current()/@type]/@contains,@type)]" mode="tree">
				<xsl:sort select="text()" lang="{$lang}"/>
			</xsl:apply-templates>
		</xsl:variable>
		<xsl:variable name="contained" select="msxsl:node-set($contained_rtf)/*"/>
		<xsl:variable name="subterritories" select="$contained[not(@id=$contained/ul/li/@id)]"/>
		<xsl:choose>
			<xsl:when test="$registry/region[subtag=current()/@type]">
				<li id="{@type}"><xsl:value-of select="text()"/>
					<xsl:if test="count($subterritories) &gt; 0">
						<ul>
							<xsl:copy-of select="$subterritories"/>
						</ul>
					</xsl:if>
				</li>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="$subterritories[not(@id=$contained/@id)]"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>