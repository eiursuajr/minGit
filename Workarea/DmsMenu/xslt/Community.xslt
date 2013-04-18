<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ekt="http://www.ektron.com/cms/workarea/dms/dmsmenu" exclude-result-prefixes="ekt">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" standalone="no"/>
	<xsl:param name="dynamicContentBox"/>
	<xsl:param name="dmsEktControlID"/>
	<xsl:param name="dmsMenuGuid"/>

	<xsl:template match="/">
		<ul class="dmsMenuWrapper  dmsMenuCommunityXSLT">
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
							<xsl:value-of select="@label"/>
							<xsl:text>:&#160;</xsl:text>
							<xsl:value-of select="//ekt:dmsContentItem/@title" disable-output-escaping="yes"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:choose>
					<xsl:when test="$dynamicContentBox = 'true' and @menuItemType = 'DmsMenuView'">
						<xsl:attribute name="onclick">
							<xsl:text>Ektron.DMSMenu.DestroyMenu();ShowDynamicContentBox(this);return false;</xsl:text>
						</xsl:attribute>
						<xsl:attribute name="href">
							<xsl:value-of select="@href"/>&amp;EkTB_iframe=true&amp;height=480&amp;width=640
						</xsl:attribute>
					</xsl:when>

					<xsl:when test="@menuItemType = 'DmsMenuDelete'">
            <xsl:choose>
              <xsl:when test="//ekt:dmsMenu/@menuType = 'Favorites'">
                <xsl:attribute name="onclick">
                  DeleteFavoriteItem(<xsl:value-of select="../@id"/>,'<xsl:value-of select="../@title"/>','<xsl:value-of select="$dmsEktControlID" />');return false;
                </xsl:attribute>
                <xsl:attribute name="href">
                  <xsl:text>#</xsl:text>
                </xsl:attribute>
              </xsl:when>
              <xsl:otherwise>
                <xsl:attribute name="onclick">
                  DeleteItem(<xsl:value-of select="../@id"/>,'<xsl:value-of select="../@title"/>','<xsl:value-of select="$dmsEktControlID" />');return false;
                </xsl:attribute>
                <xsl:attribute name="href">
                  <xsl:text>#</xsl:text>
                </xsl:attribute>
              </xsl:otherwise>
            </xsl:choose>
					</xsl:when>
					<xsl:otherwise>
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
					</xsl:otherwise>
				</xsl:choose>
				<span class="dmsMenuItemLabel">
					<xsl:value-of select="@label"/>
				</span>
			</a>
		</li>
		<xsl:if test="following-sibling::ekt:dmsMenuItem[1]/@group != @group">
			<li class="sectionBreak"></li>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
