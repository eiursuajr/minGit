<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="no" omit-xml-declaration="yes"/>

<xsl:param name="baseURL" select="''"/>
<xsl:param name="LangType" select="''"/>

<xsl:template match="@background|@dynsrc|@href|@src">
	<xsl:choose>
		<xsl:when test="starts-with(.,'/')">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="concat($baseURL,.)"/>
			</xsl:attribute>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- identity template -->
<!-- known xhtml tags (with closing tags) -->
<xsl:template match="a|abbr|acronym|address|b|bdo|bgsound|big|blink|blockquote|button|
		caption|center|cite|code|colgroup|comment|dd|del|dfn|dir|div|dl|dt|em|fieldset|font|form|
		h1|h2|h3|h4|h5|h6|i|iframe|ins|kbd|label|legend|li|listing|map|marquee|menu|nobr|noembed|noscript|
		ol|optgroup|option|p|plaintext|pre|q|rb|rbc|rp|rt|rtc|ruby|s|samp|select|small|span|strike|strong|style|sub|sup|
		table|tbody|td|textarea|tfoot|th|thead|tr|tt|u|ul|var|wbr|xml|xmp">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

<!-- See similar templates for identity without closing tags -->
<!-- identity without closing tags -->
<xsl:template match="area[not(node())]|base[not(node())]|basefont[not(node())]|bgsound[not(node())]|
				br[not(node())]|col[not(node())]|frame[not(node())]|hr[not(node())]|
				img[not(node())]|input[not(node())]|isindex[not(node())]|keygen[not(node())]|
				link[not(node())]|meta[not(node())]|param[not(node())]">
	<xsl:copy>
		<xsl:apply-templates select="@*" />
	</xsl:copy>
</xsl:template>

<!-- identity template -->
<!-- identity with closing tags -->
<xsl:template match="@*|node()" priority="-0.5">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

</xsl:stylesheet>