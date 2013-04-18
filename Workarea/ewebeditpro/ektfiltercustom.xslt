<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript">

	<!-- Ektron revision date: 2007-02-16 -->
	<!-- Commonly requested filtering -->

	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="no" omit-xml-declaration="yes"/>

	<!-- remove DIV within LI -->
	<!--
	<xsl:template match="li/div[count(@*)=0]">
		<xsl:apply-templates/>
	</xsl:template>
	-->

	<!-- replace B with STRONG -->
	<!--
	<xsl:template match="b">
		<strong>
			<xsl:apply-templates select="@*|node()"/>
		</strong>
	</xsl:template>
	-->

	<!-- replace I with EM -->
	<!--
	<xsl:template match="i">
		<em>
			<xsl:apply-templates select="@*|node()"/>
		</em>
	</xsl:template>
	-->

	<!-- for KB "Resolve URLs when content may be displayed in more than one template in different directories"  -->
	<!-- http://dev.ektron.com/kb_article.aspx?id=8372 -->
	<!-- uncomment xsl:include href="ektfiltercustom.xslt" in ektfilter.xslt -->
	<!-- uncomment eWebEditPro.oncreate in customevents.js -->
	<!-- uncomment xsl:template match="a/@href..." below -->
	<!-- 
	<xsl:template match="a/@href|area/@href|link/@href|script/@src|iframe/@src|bgsound/@src|applet/@src|embed/@src|img/@src|table/@background|th/@background|td/@background|input/@src|@dynsrc">
		<xsl:attribute name="{name()}">
			<xsl:choose>
				<xsl:when test="starts-with(.,'/AREA/SECTION/')">
					<xsl:value-of select="substring-after(.,'/AREA/SECTION/')"/>
				</xsl:when>
				<xsl:when test="starts-with(.,'/AREA/')">
					<xsl:value-of select="concat('../',substring-after(.,'/AREA/'))"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="."/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:template>
	-->

	<!-- 
		Single-space paragraphs
		Add this style to your template page or CSS file.

	<style type="text/css">
	div.ektcontent p { 
		margin:0in;
		margin-bottom:.0001pt;
	}
	</style>
	-->
	<!--
	<xsl:template match="body">
		<xsl:copy>
			<xsl:apply-templates select="@*"/>
			<div class="ektcontent">
				<xsl:apply-templates select="node()"/>
			</div>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="div[@class='ektcontent']">
		<xsl:apply-templates select="node()"/>
	</xsl:template>
	-->

</xsl:stylesheet>
