<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xhtml="http://www.w3.org/1999/xhtml" exclude-result-prefixes="msxsl xhtml">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="skinPath" select="''"/>
<xsl:param name="srcPath" select="''"/>
<xsl:param name="baseURL" select="''"/>
<xsl:param name="LangType" select="''"/>

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="name(node())='root'">
			<xsl:apply-templates select="*/node()" />
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- Custom Tags -->
<xsl:template match="ektdesignns_checklist|ektdesignns_choices|ektdesignns_richarea|ektdesignns_mergelist">
	<div class="design_{substring-after(local-name(),'ektdesignns_')}">
		<xsl:apply-templates select="@*|node()" />
	</div>
</xsl:template>
<xsl:template match="ektdesignns_filelink|ektdesignns_imageonly|ektdesignns_mergefield|ektdesignns_resource">
	<span class="design_{substring-after(local-name(),'ektdesignns_')}">
		<xsl:apply-templates select="@*|node()" />
	</span>
</xsl:template>
<!--<xsl:template match="ektdesignns_calendar">
Pass ektdesignns_calendar fields through. They will be processed later.
</xsl:template>-->

<!--<xsl:template match="@background|@dynsrc|@href|@src">
	<xsl:copy-of select="."/>
	<xsl:attribute name="data-ektron-url"><xsl:value-of select="."/></xsl:attribute>
</xsl:template>-->
<xsl:template match="@data-ektron-url"/>

<!-- known xhtml tags (with closing tags) -->
<!-- known xhtml tags treated as placeholder: applet, embed, object, script -->
<xsl:template match="a|abbr|acronym|address|b|bdo|bgsound|big|blink|blockquote|button|
		caption|center|cite|code|colgroup|comment|dd|del|dfn|dir|div|dl|dt|em|fieldset|font|form|
		h1|h2|h3|h4|h5|h6|i|iframe|ins|kbd|label|legend|li|listing|map|marquee|menu|nobr|noembed|noscript|
		ol|optgroup|option|p|plaintext|pre|q|rb|rbc|rp|rt|rtc|ruby|s|samp|select|small|span|strike|strong|style|sub|sup|
		table|tbody|td|textarea|tfoot|th|thead|tr|tt|u|ul|var|wbr|xml|xmp">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

<!-- identity template -->
<!-- identity with closing tags -->
<xsl:template match="@*|node()" priority="-0.5">
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

</xsl:stylesheet>
