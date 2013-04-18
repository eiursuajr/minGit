<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:output  method ="xml" indent ="yes"/>
	<xsl:template match ="/">

		<Highlights>
			<xsl:for-each select="Collection/Content">
				<xsl:variable name ="vDescCount" select ="169" />
				<highlight>
					<h3>
						<a>
							<title>
								<xsl:value-of select ="Title" disable-output-escaping="yes"/>
							</title>
							
						</a>
					</h3>
					<image>
						<xsl:value-of select="Html/root/Image/img/@src"/>
					</image>
					<link>
						<xsl:value-of select="QuickLink"/>
					</link>
					<description>

						<xsl:choose>
							<xsl:when test ="string-length(Teaser) &lt;= $vDescCount" >
								<xsl:value-of select ="Teaser"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="substring(Teaser,1,$vDescCount)"/>
								<xsl:text>...</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</description>
				</highlight>
			</xsl:for-each>
		</Highlights>
	</xsl:template>
</xsl:stylesheet>
