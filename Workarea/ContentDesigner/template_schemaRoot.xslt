<?xml version="1.0"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="xs" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema">

<!--

To be used when processing a schema.

When including this template, write a template like the one shown here. Add parameters as needed.

<xsl:template match="xs:element[@name]" mode="root">
	<xsl:apply-templates select="."/>
</xsl:template>

-->

<xsl:template match="xs:schema">
	<!-- pick root element, if more than one, choose the first -->
	<xsl:variable name="prefix">
		<xsl:value-of select="name(namespace::*[.=current()/@targetNamespace])"/>
	</xsl:variable>
	<xsl:variable name="prefixC">
		<xsl:choose>
			<xsl:when test="string-length($prefix) &gt; 0">
				<xsl:value-of select="concat($prefix,':')"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="''"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="possibleRoots">
		<xsl:for-each select="xs:element[@name]">
			<xsl:variable name="qname" select="concat($prefixC,@name)"/>
			<xsl:if test="count(//xs:element[@ref=$qname])=0">
				<xsl:value-of select="concat(@name,' ')"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>
	<xsl:variable name="firstRoot" select="substring-before($possibleRoots,' ')"/>
	
	<xsl:apply-templates select="xs:element[@name=$firstRoot]" mode="root">
		<xsl:with-param name="roots" select="$possibleRoots"/> <!-- parameter offered in case template wants to know the other possibilities -->
	</xsl:apply-templates>

</xsl:template>

</xsl:stylesheet>