<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- copies richly formatted data -->

<!-- defect #34900 -->
<xsl:template match="br[@class='khtml-block-placeholder']" mode="rich"/>
<!-- defect #34779 -->
<xsl:template match="br[@_moz_editor_bogus_node]" mode="rich"/>
<xsl:template match="@*[starts-with(name(),'_moz')]" mode="rich"/>

<xsl:template match="*" mode="rich">
	<xsl:copy>
 		<xsl:apply-templates select="@*|node()" mode="rich"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" mode="rich">
	<xsl:element name="{local-name()}">
    	<xsl:apply-templates select="@*" mode="rich"/>
	</xsl:element>
</xsl:template>

<xsl:template match="@*|text()|comment()|processing-instruction()" mode="rich">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" mode="rich"/>
	</xsl:copy>
</xsl:template>

</xsl:stylesheet>