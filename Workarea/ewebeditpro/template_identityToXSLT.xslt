<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xslout="alias">

<!-- copies tags for an XSLT output -->

<!-- identity template without namespace nodes -->

<xsl:template match="*">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_identity">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="*" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_identity">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_identity">
	<xsl:param name="xpath"/>
	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
		<xsl:apply-templates select="node()">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</xsl:element>
</xsl:template>


<xsl:template match="xsl:*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" priority="0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_identityWoClosingTag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="xsl:*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" priority="0" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_identityWoClosingTag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_identityWoClosingTag">
	<!-- identity without closing tags -->
	<xsl:param name="xpath"/>
	<xsl:copy>
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</xsl:copy>
</xsl:template>


<xsl:template match="@*|text()">
	<xsl:call-template name="_text"/>
</xsl:template>

<xsl:template match="@*|text()" mode="priority-0">
	<xsl:call-template name="_text"/>
</xsl:template>

<xsl:template name="_text">
	<xsl:copy/>
</xsl:template>


<xsl:template match="comment()">
	<xsl:call-template name="_comment"/>
</xsl:template>

<xsl:template match="comment()" mode="priority-0">
	<xsl:call-template name="_comment"/>
</xsl:template>

<xsl:template name="_comment">
	<xslout:comment xml:space="preserve">
		<xsl:value-of select="."/>
	</xslout:comment>
</xsl:template>


<xsl:template match="processing-instruction()">
	<xsl:call-template name="_processing-instruction"/>
</xsl:template>

<xsl:template match="processing-instruction()" mode="priority-0">
	<xsl:call-template name="_processing-instruction"/>
</xsl:template>

<xsl:template name="_processing-instruction">
	<xslout:processing-instruction name="{name()}">
		<xsl:value-of select="."/>
	</xslout:processing-instruction>
</xsl:template>

</xsl:stylesheet>