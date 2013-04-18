<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript">

<!-- Ektron revision date: 2006-06-13 -->
<!-- Populates ekt_* custom tags:
	ekt_date 	Displays current date
	ekt_toc		Creates table of contents of h1-h6 tags
			@type may be bullets (default), numbers, or outline
-->

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="no" omit-xml-declaration="yes"/>

<!-- today's date field -->
<xsl:template match="ekt_date">
	<xsl:copy>
		<script language="JavaScript" type="text/javascript"><xsl:comment xml:space="preserve">
<xsl:text>var oTempDate = new Date(); document.write(oTempDate.toLocaleDateString ? oTempDate.toLocaleDateString() : oTempDate.toLocaleString()); // </xsl:text></xsl:comment></script>
	</xsl:copy>
</xsl:template>

<!-- table of contents -->
<xsl:template match="ekt_toc">
<!-- @type="bullets|numbers|outline" -->
	<xsl:variable name="toc_rtf">
		<xsl:variable name="list_rtf">
			<xsl:apply-templates select="//h1|//h2|//h3|//h4|//h5|//h6" mode="toc">
				<xsl:with-param name="type" select="@type"/>
			</xsl:apply-templates>
		</xsl:variable>
		<xsl:variable name="list" select="msxsl:node-set($list_rtf)/*"/>
		<xsl:variable name="level" select="$list[1]/@level"/>
		<xsl:apply-templates select="$list[@level &lt;= $level]" mode="toc"/>
	</xsl:variable>
	<xsl:variable name="toc" select="msxsl:node-set($toc_rtf)/*"/>
	<!-- retain tag -->
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:if test="count($toc) &gt; 0">
			<a name="ekt_toc"></a>
			<xsl:choose>
				<xsl:when test="@type='bullets'">
					<ul>
						<xsl:apply-templates select="$toc"/>
					</ul>
				</xsl:when>
				<xsl:when test="@type='numbers'">
					<ol>
						<xsl:apply-templates select="$toc"/>
					</ol>
				</xsl:when>
				<xsl:when test="@type='outline'">
					<xsl:apply-templates select="$toc"/>
				</xsl:when>
				<xsl:otherwise> <!-- bullets -->
					<ul>
						<xsl:apply-templates select="$toc"/>
					</ul>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:copy>
</xsl:template>

<xsl:template match="toc" mode="toc">
	<!-- convert list to hierarchy -->
	<xsl:variable name="peer" select="(following-sibling::toc)[@level &lt;= current()/@level][1]"/>
	<xsl:variable name="list" select="(following-sibling::toc)[not(@pos &gt;= $peer/@pos)]"/>
	<xsl:variable name="level" select="$list[1]/@level"/>
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:apply-templates select="$list[@level &lt;= $level]" mode="toc"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="h1|h2|h3|h4|h5|h6" mode="toc">
	<xsl:param name="type" select="'bullets'"/>
	<toc type="{$type}" level="{number(substring(local-name(),2))}" pos="{position()}" id="{generate-id()}" text="{.}"/>
</xsl:template>

<xsl:template match="toc[@type='bullets' or @type='' or not(@type)]">
	<li>
		<xsl:call-template name="hyperlink"/>
		<xsl:if test="count(*) &gt; 0">
			<ul>
				<xsl:apply-templates select="*"/>
			</ul>
		</xsl:if>
	</li>
</xsl:template>

<xsl:template match="toc[@type='numbers']">
	<li>
		<xsl:call-template name="hyperlink"/>
		<xsl:if test="count(*) &gt; 0">
			<ol>
				<xsl:apply-templates select="*"/>
			</ol>
		</xsl:if>
	</li>
</xsl:template>

<xsl:template match="toc[@type='outline']">
	<div>
		<xsl:number level="multiple"/>
		<xsl:call-template name="hyperlink"/>
	</div>
	<xsl:apply-templates select="*"/>
</xsl:template>

<xsl:template name="hyperlink">
	<a href="#ekt_toc_{@id}"><xsl:value-of select="@text"/></a>
</xsl:template>

<!-- insert anchor in heading tags -->
<xsl:template match="h1|h2|h3|h4|h5|h6" priority="1">
	<a href="#ekt_toc">Top</a>
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<a name="ekt_toc_{generate-id()}"></a>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<xsl:template match="a[@href='#ekt_toc']" priority="1"/>
<xsl:template match="a[starts-with(@name,'ekt_toc')]" priority="1"/>

</xsl:stylesheet>
