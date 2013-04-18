<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl ekext" exclude-result-prefixes="msxsl ekext" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:ekext="urn:ektron:extension-object:prefilterskl"
xmlns:xlf="urn:oasis:names:tc:xliff:document:1.1" xmlns:ekt="urn:ektron:xliff"
xmlns:its="http://www.w3.org/2005/11/its">

<!-- run this transform on the xhtml skeleton (skl) file -->

<xsl:key name="FieldByXPath" match="fieldlist/field" use="@xpath"/>

<!-- 
The output is an xhtml skeleton file with id's and 
markers necessary for reliable extraction and merging. 
-->

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

<xsl:strip-space elements="xlf:*"/>

<!-- WARNING: 
	Behavior differs depending on XML processor.
	* Stylus Studio built-in: emits msxsl and js namespaces
	* MS XML 4.0: Works!
	* MS XML .NET 1.x: disable-output-escaping="yes" has no effect
	* ekxsl (.NET 2.0): Works!
-->

<xsl:template match="/">
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:apply-templates mode="skl"/>
	</xsl:copy>
</xsl:template>

<!-- Use mode="skl" so text() nodes are copied rather than marked for extraction -->

<xsl:template match="xlf:file" mode="skl">
	<xsl:variable name="id" select="ekext:resetUnitId()"/>
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<!-- 
		ITS spec states "a prefixed ITS version attribute (e.g. its:version) MUST be provided 
		at the root element of the document", however, xlf:xliff tag does not support 
		other-namespaced attributes. The xlf:file tag does (in XLIFF 1.2), but the its.xsd 
		does not define a global its:version attribute (it is embedded in a group definition 
		in the schema).
		See http://www.w3.org/TR/its/#its-version-attribute 
		So, we'll ignore the its:version requirement.
		<xsl:attribute name="its:version" namespace="http://www.w3.org/2005/11/its">1.0</xsl:attribute>
		-->
		<xsl:apply-templates mode="skl"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="*" mode="skl">
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:apply-templates mode="skl"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="comment()" mode="skl">
	<xsl:copy-of select="."/>
</xsl:template>

<xsl:template match="fieldlist" mode="skl"/>

<xsl:template match="skeleton" mode="skl">
	<!-- The skeleton tag is an XLIFF extension point (ie, custom tag) 
			that marks the original content. -->
	<!-- begin processing content to id and mark for extraction -->
	<xsl:apply-templates select="."/> <!-- reapply without mode -->
</xsl:template>

<xsl:template match="AssetData" mode="skl">
	<xsl:variable name="id" select="ekext:generateNextUnitId()"/>
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:attribute name="ekt:id"><xsl:value-of select="$id"/></xsl:attribute>
		<xsl:copy-of select="node()"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="html" mode="skl">
	<xsl:variable name="id" select="ekext:resetUnitId()"/>
	<xsl:apply-templates select="."/> <!-- reapply without mode -->
</xsl:template>

<xsl:template match="style">
	<xsl:copy-of select="."/>
</xsl:template>

<xsl:template match="*">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:choose>
			<xsl:when test="name()='skeleton'"></xsl:when>
			<xsl:otherwise><xsl:value-of select="concat($xpath,'/',local-name())"/></xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="id" select="ekext:generateNextUnitId()"/>
	<xsl:copy>
		<xsl:call-template name="buildTranslatable">
			<xsl:with-param name="field" select="key('FieldByXPath',$new-xpath)"/>
		</xsl:call-template>
		<xsl:copy-of select="@*"/>
		<xsl:attribute name="ekt:id"><xsl:value-of select="$id"/></xsl:attribute>
		<xsl:variable name="text">
			<!-- concat text nodes that are immediate children -->
			<xsl:for-each select="text()">
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:for-each>
		</xsl:variable>
		<xsl:choose>
			<!-- if contains blocking tags, continue segmenting, ref. xhtml1-transitional.xsd#xpointer(//xs:group[@name="block"]) -->
			<xsl:when test="count(p|h1|h2|h3|h4|h5|h6|div|ul|ol|dl|menu|dir|pre|hr|blockquote|address|center|noframes|form|isindex|fieldset|table) &gt; 0">
				<xsl:apply-templates>
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
			</xsl:when>
			<!-- can't use string-length(normalize-space(.)) b/c it includes all descendent text nodes, not just children -->
			<xsl:when test="string-length($text) &gt; 0">
				<!-- This will produce a trans-unit -->
				<!-- #30587 <xsl:copy-of select="node()"/>-->
				<xsl:apply-templates mode="source"/>
			</xsl:when>
			<xsl:when test="not(node()) and (name()='area' or name()='bgsound' or name()='br' or name()='hr' or name()='img' or name()='input' or name()='param')"></xsl:when>
			<!--
			<xsl:when test="not(node())">
				<xsl:text>&#8203;</xsl:text> text node (of zero-width space) to ensure full end tag
			</xsl:when>
			-->
			<xsl:otherwise>
				<xsl:apply-templates>
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:copy>
</xsl:template>

<!--
	#30587 - Error during the import of XLIFF
	Was using position() to map inline elements between XLIFF and Skeleton. Instead use ekt:id on all tags.
	source: workarea/xslt/prefilterSkl.xslt, extractSklToXliff*.xslt, mergeXliff*.xslt
-->
<xsl:template match="*" mode="source">
	<xsl:variable name="id" select="ekext:generateNextUnitId()"/>
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:attribute name="ekt:id"><xsl:value-of select="$id"/></xsl:attribute>
		<xsl:apply-templates mode="source"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="xsl:*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" mode="source">
	<xsl:variable name="id" select="ekext:generateNextUnitId()"/>
	<xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:attribute name="ekt:id"><xsl:value-of select="$id"/></xsl:attribute>
	</xsl:copy>
</xsl:template>

<xsl:template match="text()" mode="source">
	<xsl:copy/>
</xsl:template>


<xsl:template name="buildTranslatable">
	<xsl:param name="field"/>
	<xsl:for-each select="$field[1]"> <!-- make context -->
		<xsl:choose>
			<xsl:when test="@translate='true' or @translate='1' or @translate='yes'">
				<xsl:attribute name="its:translate">yes</xsl:attribute>
			</xsl:when>
			<xsl:when test="@translate='false' or @translate='0' or @translate='no'">
				<xsl:attribute name="its:translate">no</xsl:attribute>
			</xsl:when>
			<xsl:when test="@datalist">
				<xsl:attribute name="its:translate">no</xsl:attribute>
			</xsl:when>
			<xsl:when test="@basetype='number' or @basetype='password' or @basetype='checkbox' or @basetype='calendar' or @basetype='id'">
				<xsl:attribute name="its:translate">no</xsl:attribute>
			</xsl:when>
			<xsl:when test="@datatype='date' or @datatype='dateTime' or starts-with(@datatype,'email')">
				<xsl:attribute name="its:translate">no</xsl:attribute>
			</xsl:when>
		</xsl:choose>
	</xsl:for-each>
</xsl:template>

<xsl:template match="text()">
	<xsl:choose>
		<xsl:when test="string-length(normalize-space(.)) &gt; 0">
			<xsl:variable name="id" select="ekext:generateNextUnitId()"/>
			<ekt:tu ekt:id="{$id}"><xsl:copy-of select="."/></ekt:tu>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="comment()">
	<xsl:copy-of select="."/>
</xsl:template>

<xsl:template match="ekt:tu">
	<xsl:apply-templates/>
</xsl:template>

<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
var g_unit_id = 0;
function generateNextUnitId()
{
	g_unit_id += 1;
	return g_unit_id;
}
function getUnitId()
{
	return g_unit_id;
}
function resetUnitId()
{
	g_unit_id = 0;
	return g_unit_id;
}
]]></msxsl:script>
-->

</xsl:stylesheet>
