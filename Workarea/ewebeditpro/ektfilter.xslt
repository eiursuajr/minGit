<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:ms="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl js">

<!-- Ektron revision date: 2007-02-16 -->
<!-- Filters XHTML content -->
<!-- Uncomment xsl:include lines to add more filtering -->

<xsl:output method="xml" version="1.0" encoding="utf-8" indent="no" omit-xml-declaration="yes"/>

<xsl:param name="baseURL" select="''" />

<!-- elements without closing tags -->
<xsl:template match="area|br|img|input">
    <xsl:copy>
    	<xsl:apply-templates select="@*"/>
    </xsl:copy>
</xsl:template>
<!-- line break after these tags -->
<xsl:template match="base|basefont|col|hr|isindex|link|meta|param">
    <xsl:copy>
    	<xsl:apply-templates select="@*"/>
    </xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<!-- line break after these tags -->
<xsl:template match="p|h1|h2|h3|h4|h5|h6|caption|th|td|blockquote|fieldset|textarea|iframe|li|option">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()"/>
	</xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<!-- line break after these open and close tags -->
<xsl:template match="table|thead|tfoot|tbody|tr|ul|ol">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:text>&#13;&#10;</xsl:text>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<!-- identity template -->
<xsl:template match="@*|node()">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()"/>
	</xsl:copy>
</xsl:template>

<!-- Commonly requested filtering -->
<!--<xsl:include href="ektfiltercustom.xslt"/>-->

<!-- Populates ekt_* custom tags:
	ekt_date 	Displays current date
	ekt_toc		Creates table of contents of h1-h6 tags
-->
<!--<xsl:include href="ektfilterekttags.xslt"/>-->

<!-- Conform to XHTML 1.0 schema -->
<!--<xsl:include href="ektfilterxhtml10.xslt"/>-->

</xsl:stylesheet>
