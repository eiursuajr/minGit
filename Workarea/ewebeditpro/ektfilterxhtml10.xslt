<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript">

<!-- Ektron revision date: 2006-02-22 -->
<!-- Conform to XHTML 1.0 schema -->

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="no" omit-xml-declaration="yes"/>

<!-- remove -->
<xsl:template match="blink"><xsl:apply-templates/></xsl:template>
<xsl:template match="bgsound"></xsl:template>
<xsl:template match="embed|noembed"></xsl:template>
<xsl:template match="nolayer"></xsl:template>
<xsl:template match="wbr|nobr"></xsl:template>
<xsl:template match="keygen"><xsl:apply-templates/></xsl:template>
<xsl:template match="marquee"><xsl:apply-templates/></xsl:template>
<xsl:template match="multicol"><xsl:apply-templates/></xsl:template>
<xsl:template match="spacer"><xsl:apply-templates/></xsl:template>
<xsl:template match="xml"></xsl:template>
<xsl:template match="@bordercolor"></xsl:template>
<xsl:template match="tr/@width|tr/@height"></xsl:template>

<!-- remove background attributes except when in body tag -->
<xsl:template match="@background"></xsl:template>
<xsl:template match="body/@background">
	<xsl:copy-of select="."/>
</xsl:template>

<!-- remove id attribute from bookmark eg, <a id="n" name="n"></a> -->
<xsl:template match="a[@id=@name]">
	<xsl:copy>
		<xsl:apply-templates select="@*[name()!='id']"/>
		<xsl:apply-templates/>
	</xsl:copy>
</xsl:template>

<!-- transform -->
<xsl:template match="comment">
	<xsl:comment>
		<xsl:apply-templates/>
	</xsl:comment>
</xsl:template>

<xsl:template match="ilayer|layer">
	<div class="{local-name()}">
		<xsl:apply-templates/>
	</div>
</xsl:template>

<xsl:template match="plaintext">
	<pre>
		<xsl:apply-templates/>
	</pre>
</xsl:template>

</xsl:stylesheet>
