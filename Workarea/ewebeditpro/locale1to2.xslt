<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

<xsl:template match="/locale">
	<locale version="2" product="{@product}" xml:lang="{@xml:lang}">
		<custom>
			<xsl:comment>define custom 'ts' elements here with a unique id</xsl:comment>
		</custom>
		<standard>
			<xsl:for-each select="*">
				<ts id="{name()}"><xsl:value-of select="."/></ts>
			</xsl:for-each>
		</standard>
	</locale>
</xsl:template>

</xsl:stylesheet>
