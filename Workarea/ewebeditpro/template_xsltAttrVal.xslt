<?xml version="1.0"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js ekext" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:ekext="urn:ektron:extension-object:common">

<!-- protect attribute values that include { } so they are not interpretted as an attribute value template -->

<!-- IMPORTANT: see packxml.xslt, which has to handle these &apos; and &quot; entities specially -->

<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
function escapeCurly(s)
{
	return s.replace(/\{/g,"{{").replace(/\}/g,"}}");
}
]]></msxsl:script>
-->

<!-- 
	Assume if attr contain {{ that is has already been processed. This is really not a safe assumption,
	but it will work for now.
	For example, in DesignToEntryXSLT.xslt, where $prototype has been processed before.
	<xsl:apply-templates select="$prototype/@*[name()!='id']">
-->

<xsl:template match="@*[contains(.,'{') and not(contains(.,'{{')) and not(starts-with(.,'{$'))]">
	<xsl:call-template name="_attr-template"/>
</xsl:template>

<xsl:template match="@*[contains(.,'{') and not(contains(.,'{{')) and not(starts-with(.,'{$'))]" mode="priority-0">
	<xsl:call-template name="_attr-template"/>
</xsl:template>

<xsl:template name="_attr-template">
	<xsl:attribute name="{name()}"><xsl:value-of select="ekext:escapeCurly(string(.))"/></xsl:attribute>
</xsl:template>

<!-- use this if JavaScript is ever not available 
<xsl:template name="_attr-template">
	<xsl:choose>
	
		<!- - contains both single and double quotes, no known way to delimit - ->
		<!- - can't use xsl:attribute because any following attributes will not be added to the element - ->
		<xsl:when test="contains(.,&quot;'&quot;) and contains(.,'&quot;')">
			<xsl:attribute name="{name()}">VALUE CANNOT CONTAIN CURLY BRACES, SINGLE QUOTES AND DOUBLE QUOTES</xsl:attribute>
		</xsl:when>
		
		<!- - contains single quote, delimit with double quotes - ->
		<xsl:when test="contains(.,&quot;'&quot;)">
			<xsl:attribute name="{name()}">
				<xsl:text>{&quot;</xsl:text>
				<xsl:value-of select="."/>
				<xsl:text>&quot;}</xsl:text>
			</xsl:attribute>
		</xsl:when>
		
		<!- - does not contain a single quote, delimit with single quotes - ->
		<xsl:otherwise>
			<xsl:attribute name="{name()}">
				<xsl:text>{&apos;</xsl:text>
				<xsl:value-of select="."/>
				<xsl:text>&apos;}</xsl:text>
			</xsl:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
-->

</xsl:stylesheet>