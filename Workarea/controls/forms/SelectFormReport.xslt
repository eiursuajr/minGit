<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- 
	Run on FormReportsManifest.xml OR a field list. 
	Run on a field list to manifest to filter out reports that are not applicable. 
	If running on a field list, pass 'manifest' as the URL to FormReportsManifest.xml.
	Outputs list of OPTION tags.
-->

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="selectedIndex"/> <!-- optional -->
<xsl:param name="lang"/> <!-- optional -->
<xsl:param name="manifest"/> <!-- optional -->

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="$manifest">
			<!-- processing field list; switch to process manifest -->
			<xsl:apply-templates select="document($manifest)/Manifest/Reports/Report">
				<xsl:with-param name="fieldlist" select="fieldlist"/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:otherwise>
			<!-- processing manifest; fieldlist not available -->
			<xsl:apply-templates select="Manifest/Reports/Report"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="Report">
	<xsl:param name="fieldlist"/>

	<xsl:variable name="valid">
		<!-- Valid if any of the fieldlist requirements are met. -->
		<xsl:variable name="options">
			<xsl:if test="$fieldlist">
				<xsl:for-each select="fieldlist">
					<!-- Valid if all of the field requirements are met. -->
					<xsl:variable name="required">
						<xsl:for-each select="field">
							<xsl:variable name="manifestField" select="."/>
							<xsl:variable name="anyField">
								<xsl:for-each select="$fieldlist/field">
									<xsl:variable name="field" select="."/>
									<xsl:variable name="everyAttr">
										<!-- Match all attribute values or existance if attribute="*". -->
										<xsl:for-each select="$manifestField/@*">
											<xsl:variable name="name" select="name()"/>
											<xsl:variable name="attr" select="$field/@*[name()=$name]"/>
											<xsl:choose>
												<xsl:when test=".='*'">
													<xsl:value-of select="boolean($attr)"/>
												</xsl:when>
												<xsl:when test="starts-with(.,'!')">
													<xsl:value-of select="substring-after(.,'!')!=$attr"/>
												</xsl:when>
												<xsl:otherwise>
													<xsl:value-of select=".=$attr"/>
												</xsl:otherwise>
											</xsl:choose>
										</xsl:for-each>
									</xsl:variable>
									<xsl:value-of select="not(contains($everyAttr,'false'))"/>
								</xsl:for-each>
							</xsl:variable>
							<xsl:value-of select="contains($anyField,'true')"/>
						</xsl:for-each>
					</xsl:variable>
					<xsl:value-of select="not(contains($required,'false'))"/>
				</xsl:for-each>
			</xsl:if>
		</xsl:variable>
		<xsl:value-of select="not($fieldlist) or string-length($options)=0 or contains($options,'true')"/>
	</xsl:variable>

	<xsl:if test="$valid='true' and (lang($lang) or not(@xml:lang) or not($lang))">
		<option value="{position()}">
			<xsl:if test="$selectedIndex = position()">
				<xsl:attribute name="selected">selected</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="Title"/>
		</option>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>