<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xhtml="http://www.w3.org/1999/xhtml" exclude-result-prefixes="msxsl xhtml" >

	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>

	<!-- identity template -->
	<!-- identity with closing tags -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>

	<!-- See similar templates for identity without closing tags -->
	<!-- identity without closing tags -->
	<xsl:template match="area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" >
		<xsl:copy>
			<xsl:apply-templates select="@*" />
		</xsl:copy>
	</xsl:template>
	
	<!--Strip HTML Attributes 
	Developers can create custom xslt with their own set of rules and
	assign the new xslt file path to the Forum Control Property>FilterXslt 
	For Sample code visit: http://dev.ektron.com/kb_article.aspx?id=485&terms=xslt-->

	<xsl:template match="*[starts-with(@href,'javascript:')]">
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template match="*[starts-with(@href,'vbscript:')]">
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template name="strip" match="@*[starts-with(name(),'on')]"/>
	<xsl:template match="@*[starts-with(name(),'style')]"/>
	<xsl:template match="script"/>

</xsl:stylesheet>
