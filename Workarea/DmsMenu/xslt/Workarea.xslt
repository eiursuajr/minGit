<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ekt="http://www.ektron.com/cms/workarea/dms/dmsmenu" exclude-result-prefixes="ekt">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" standalone="no"/>
	<xsl:param name="dmsMenuGuid"/>

	<xsl:template match="/">
		<ul class="dmsMenuWrapper dmsMenuWorkareaXSLT">
			<xsl:attribute name="id">
				<xsl:text>dmsMenuWrapper</xsl:text>
				<xsl:value-of select="//ekt:dmsContentItem/@id"/>
				<xsl:value-of select="//ekt:dmsContentItem/@languageId"/>
				<xsl:value-of select="$dmsMenuGuid"/>
			</xsl:attribute>
			<xsl:apply-templates/>
		</ul>
	</xsl:template>

	<xsl:template match="ekt:dmsMenuItem">
		<li>
			<a class="{translate(@menuItemType,' .', '')}" href="{@href}">
				<xsl:attribute name="title">
					<xsl:choose>
						<xsl:when test="@toolTip">
							<xsl:value-of select="@toolTip" disable-output-escaping="yes"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@label" disable-output-escaping="yes"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:choose>
					<xsl:when test="@onclick">
						<xsl:attribute name="href">
							<xsl:value-of select="@href" disable-output-escaping="yes"/>
						</xsl:attribute>
						<xsl:attribute name="onclick">
							<xsl:value-of select="@onclick" disable-output-escaping="yes"/>
						</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="onclick">
							<xsl:text>Ektron.DMSMenu.DestroyMenu();</xsl:text>
						</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<span>
					<xsl:value-of select="@label"/>
				</span>
			</a>
		</li>
		<xsl:if test="following-sibling::ekt:dmsMenuItem[1]/@group != @group">
			<li class="sectionBreak"></li>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
