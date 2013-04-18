<?xml version="1.0"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="xs" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema">

<!-- To be used when processing a schema -->
<!-- Gets the qualified name (ie, including namespace unless it's the targetNamespace) of the value -->

<xsl:template name="QName">
	<xsl:param name="value" select="''"/>
	
	<xsl:choose>
		<xsl:when test="contains($value,':')">
			<xsl:variable name="prefix" select="substring-before($value,':')"/>
			<xsl:variable name="NCName" select="substring-after($value,':')"/>
			<xsl:variable name="namespace" select="/xs:schema/namespace::*[name()=$prefix]"/>
			<xsl:choose>
				<xsl:when test="/xs:schema/@targetNamespace = $namespace">
					<xsl:value-of select="$NCName"/>
				</xsl:when>
				<xsl:when test="namespace-uri(/xs:schema) = $namespace">
					<!-- standardize on 'xs:' prefix for schema namespace -->
					<xsl:value-of select="concat('xs:',$NCName)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$value"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:choose>
				<xsl:when test="string-length($value) &gt; 0 and name(/xs:schema)=local-name(/xs:schema)">
					<!-- standardize on 'xs:' prefix for schema namespace -->
					<xsl:value-of select="concat('xs:',$value)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$value"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>