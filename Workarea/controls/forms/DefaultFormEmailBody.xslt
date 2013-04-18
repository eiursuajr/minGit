<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_FormDataDisplay.xslt"/>

<xsl:output method="html" indent="yes"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="/">
	<div id="FormDataSubmitted">
	<p><xsl:copy-of select="*/Mail/MessageBody/node()"/></p>
	<p>Data from form "<xsl:value-of select="*/FormTitle"/>" was received on <xsl:value-of select="*/Date"/>.</p>
	<p><xsl:copy-of select="*/FormDescription/node()"/></p>
	<xsl:apply-templates select="*/Data"/>
	<xsl:if test="*/Mail">
		<p style="font-size:smaller">Email "<xsl:value-of select="*/Mail/Subject"/>" originally sent to <a href="mailto:{*/Mail/To}"><xsl:value-of select="*/Mail/To"/></a> from <a href="mailto:{*/Mail/From}"><xsl:value-of select="*/Mail/From"/></a> on <xsl:value-of select="*/Date"/><xsl:text>.</xsl:text>
		<xsl:if test="string-length(normalize-space(*/Mail/CC)) &gt; 0">
			<xsl:text> The following were also sent a copy: </xsl:text><xsl:value-of select="*/Mail/CC"/><xsl:text>.</xsl:text>
		</xsl:if>
		</p>
	</xsl:if>
	</div>
</xsl:template>

</xsl:stylesheet>