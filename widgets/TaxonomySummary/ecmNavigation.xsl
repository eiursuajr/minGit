<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="/">
		<ul class="itemlist">
			<xsl:apply-templates select="//TaxonomyItemData" />
		</ul>
	</xsl:template>
	<xsl:template match="TaxonomyItemData">
		<li>
			<a class="title">
				<xsl:attribute name="href"><xsl:value-of select="TaxonomyItemQuickLink"/></xsl:attribute>				
				<xsl:attribute name="title"><xsl:value-of select="TaxonomyItemTitle"/></xsl:attribute>
				<xsl:value-of select="TaxonomyItemTitle" disable-output-escaping="yes"/>
			</a>			
		</li>
	</xsl:template>
</xsl:stylesheet>

