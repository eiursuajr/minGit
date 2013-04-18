<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ekt="http://www.ektron.com/cms/workarea/dms/contentmenu" exclude-result-prefixes="ekt">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" standalone="no"/>

	<xsl:template match="/">
		<ul class="dmsMenu">
			<xsl:apply-templates/>
		</ul>
	</xsl:template>
	
	<xsl:template match="ekt:menuItem | ekt:menuLanguageItem">
		<li class="{translate(@label,' ', '')}">
			<xsl:variable name="contentId" select="//ekt:dmsContent/@id"/>
			<xsl:choose>
				<xsl:when test="child::ekt:menuItem | child::ekt:menuLanguageItem">
					<a class="{translate(@label,' ', '')}" href="#">
						<xsl:attribute name="onclick"><xsl:text>dmsMenuShowSubMenu('dmsSubmenu</xsl:text><xsl:value-of select="position()"/><xsl:text>');return false;</xsl:text></xsl:attribute>
						<xsl:value-of select="@label"/>
					</a>
				</xsl:when>
				<xsl:otherwise>
					<a class="label" title="{@label}" href="{@href}">
						<xsl:choose>
							<xsl:when test="@onclick">
								<xsl:attribute name="href"><xsl:text>#</xsl:text></xsl:attribute>
								<xsl:attribute name="onclick">
									<xsl:value-of select="@onclick"/>
									<xsl:if test="not(contains(@onclick, ';return false;'))">
										<xsl:text>;return false;</xsl:text>
									</xsl:if>
								</xsl:attribute>
							</xsl:when>
							<xsl:when test="@href">
								<xsl:attribute name="href"><xsl:value-of select="@onclick"/></xsl:attribute>
							</xsl:when>
						</xsl:choose>
						<xsl:value-of select="@label"/>
					</a>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:if test="child::ekt:menuItem | child::ekt:menuLanguageItem">
				<ul style="display:none;">
					<xsl:attribute name="id"><xsl:text>dmsSubmenu</xsl:text><xsl:value-of select="position()"/></xsl:attribute>
					<xsl:apply-templates/>
				</ul>
			</xsl:if>
		</li>
	</xsl:template>
</xsl:stylesheet>
