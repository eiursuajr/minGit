<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl skl its" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns="DUMMY" xmlns:its="http://www.w3.org/2005/11/its"
xmlns:skl="urn:oasis:names:tc:xliff:document:1.1" xmlns:ekt="urn:ektron:xliff">

<!-- import this file -->
<!-- after transforming, remove ' xmlns="DUMMY"' from result -->

<xsl:param name="original"></xsl:param> <!-- the xhtml skeleton (skl) file -->
<xsl:param name="sourcelang">en</xsl:param> <!-- http://www.ietf.org/rfc/rfc3066.txt -->
<xsl:param name="targetlang">en</xsl:param> <!-- http://www.ietf.org/rfc/rfc3066.txt -->
<xsl:param name="prefilltarget">false</xsl:param>

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

<xsl:strip-space elements="skl:*"/>

<!-- WARNING: 
	Behavior differs depending on XML processor.
	* Stylus Studio built-in: emits msxsl and js namespaces
	* MS XML 4.0: Works!
	* MS XML .NET 1.x: disable-output-escaping="yes" has no effect
	* ekxsl (.NET 2.0): Works!
-->

<!-- Use mode="skl" so text() nodes are copied rather than converted to a trans-unit -->

<xsl:template match="skl:file" mode="skl">
	<file datatype="xml" original="{$original}" source-language="{$sourcelang}" target-language="{$targetlang}">
		<xsl:apply-templates mode="skl"/>
	</file>
</xsl:template>

<xsl:template match="skl:header" mode="skl">
	<header>
		<skl>
			<external-file href="{$original}"/>
		</skl>
		<xsl:apply-templates mode="skl"/>
		<!--
		This is a low estimate, which could cause friction between 
		customers and translation service providers.
		<xsl:call-template name="buildCountGroup">
			<xsl:with-param name="text" select="../skl:body"/>
		</xsl:call-template>
		-->
	</header>
</xsl:template>

<xsl:template match="skl:body" mode="skl">
	<body>
		<xsl:apply-templates mode="skl"/>
	</body>
</xsl:template>

<xsl:template match="skl:*" mode="skl">
	<!-- copy without namespace -->
	<xsl:element name="{local-name()}">
		<xsl:copy-of select="@*[name()!='ts']"/>
		<xsl:apply-templates mode="skl"/>
	</xsl:element>
</xsl:template>

<xsl:template match="skl:tool/*" mode="skl">
	<!-- copy with only self namespace, if needed -->
	<xsl:element name="{name()}" namespace="{namespace-uri()}">
		<xsl:copy-of select="@*"/>
		<xsl:copy-of select="node()"/>
	</xsl:element>
</xsl:template>

<xsl:template match="fieldlist" mode="skl"/>

<xsl:template match="skeleton" mode="skl">
	<!-- The skeleton tag is an XLIFF extension point (ie, custom tag) 
			that marks the original content. -->
	<!-- begin processing content to produce trans-unit elements -->
	<xsl:apply-templates select="."/> <!-- reapply without mode -->
</xsl:template>

<xsl:template match="AssetData" mode="skl">
	<!--
		<AssetData ekt:id="{unit-id}">
			<Id>0f11fe66-c0b9-4e3b-a173-635719c8f7f6</Id>
			<Version>8b4245288e864d76bfb4403c50fe41db1.wmv</Version>
			<MimeType>video/x-ms-wmv</MimeType>
			<FileName>subfolder/filename.wmv</FileName>
		</AssetData>
	-->
	<xsl:call-template name="buildBinTransUnit">
		<xsl:with-param name="id" select="@ekt:id"/>
		<xsl:with-param name="mimeType" select="normalize-space(MimeType)"/>
		<xsl:with-param name="fileName" select="normalize-space(FileName)"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="style"/>

<xsl:template match="ektdesignpackage_forms">
	<xsl:apply-templates select="ektdesignpackage_form/ektdesignpackage_designs/ektdesignpackage_design"/>
</xsl:template>

<xsl:template match="*[@its:translate='no']"/>

<xsl:template match="ektdesignns_localize">
	<xsl:if test="not(contains(concat(' ',@exclude,' '),concat(' ',$targetlang,' ')))">
		<xsl:if test="not(@targets) or contains(concat(' ',@targets,' '),concat(' ',$targetlang,' '))">
			<xsl:choose>
				<xsl:when test="@note">
					<group>
						<note>
							<xsl:if test="@author">
								<xsl:attribute name="from">
									<xsl:value-of select="@author"/>
								</xsl:attribute>
							</xsl:if>
							<xsl:value-of select="@note"/>
						</note>
						<xsl:apply-templates/>
					</group>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:if>
</xsl:template>

<xsl:template match="*">
	<xsl:variable name="id" select="@ekt:id"/>
	<xsl:variable name="restype">
		<xsl:choose>
			<xsl:when test="local-name()='skeleton'">x-skeleton</xsl:when>
			<xsl:when test="local-name()='caption'">caption</xsl:when>
			<xsl:when test="local-name()='form'">dialog</xsl:when>
			<xsl:when test="local-name()='fieldset'">groupbox</xsl:when>
			<xsl:when test="local-name()='frame'">frame</xsl:when>
			<xsl:when test="local-name()='head'">header</xsl:when>
			<xsl:when test="local-name()='label'">label</xsl:when>
			<xsl:when test="local-name()='legend'">caption</xsl:when>
			<xsl:when test="local-name()='li'">listitem</xsl:when>
			<xsl:when test="local-name()='menu'">menu</xsl:when>
			<xsl:when test="local-name()='optgroup'">heading</xsl:when>
			<xsl:when test="local-name()='option'">listitem</xsl:when>
			<xsl:when test="local-name()='select'">listbox</xsl:when>
			<xsl:when test="local-name()='table'">table</xsl:when>
			<xsl:when test="local-name()='td'">cell</xsl:when>
			<xsl:when test="local-name()='textarea'">textbox</xsl:when>
			<xsl:when test="local-name()='tfoot'">footer</xsl:when>
			<xsl:when test="local-name()=name()">
				<xsl:value-of select="concat('x-html-',local-name())"/>
			</xsl:when>
			<xsl:otherwise> <!-- different namespace -->
				<xsl:value-of select="concat('x-',translate(name(),':','-'))"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:apply-templates select="@*"/>

	<xsl:variable name="text">
		<!-- concat text nodes that are immediate children -->
		<xsl:for-each select="text()">
			<xsl:value-of select="normalize-space(translate(.,'&#160;',' '))"/>
		</xsl:for-each>
	</xsl:variable>
	<xsl:choose>
		<!-- if contains blocking tags, continue segmenting, ref. xhtml1-transitional.xsd#xpointer(//xs:group[@name="block"]) -->
		<xsl:when test="count(p|h1|h2|h3|h4|h5|h6|div|ul|ol|dl|menu|dir|pre|hr|blockquote|address|center|noframes|form|isindex|fieldset|table) &gt; 0">
			<xsl:apply-templates/>
		</xsl:when>
		<!-- can't use string-length(normalize-space(.)) b/c it includes all descendent text nodes, not just children -->
		<xsl:when test="string-length($text) &gt; 0">
			<xsl:variable name="datatype">
				<xsl:choose>
					<xsl:when test="count(*) &gt; 0">html</xsl:when>
					<xsl:otherwise>plaintext</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<xsl:call-template name="buildElementTransUnit">
				<xsl:with-param name="id" select="$id"/>
				<xsl:with-param name="datatype" select="$datatype"/>
				<xsl:with-param name="restype" select="$restype"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="text()">
	<xsl:if test="string-length(normalize-space(translate(.,'&#160;',' '))) &gt; 0">
		<xsl:variable name="id" select="../@ekt:id"/>
		<xsl:call-template name="buildTextTransUnit">
			<xsl:with-param name="id" select="$id"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<!-- attributes that always need to be translated -->

<xsl:template match="a/@href[not(starts-with(.,'javascript:')) and not(starts-with(.,'mailto:')) and not(starts-with(.,'#'))]">
	<xsl:call-template name="buildAttributeTransUnit"/>
</xsl:template>
<xsl:template match="img/@src">
	<xsl:call-template name="buildAttributeTransUnit"/>
</xsl:template>
<xsl:template match="embed/@src">
  <xsl:call-template name="buildAttributeTransUnit"/>
</xsl:template>
<xsl:template match="object/param[@name='movie']/@value">
  <xsl:call-template name="buildAttributeTransUnit"/>
</xsl:template>
<xsl:template match="input[not(@type) or @type='text' or @type='submit' or @type='reset' or @type='button']/@value">
	<xsl:call-template name="buildAttributeTransUnit"/>
</xsl:template>
<xsl:template match="@abbr|@accesskey|@alt|@label|@standby|@summary|@title">
	<xsl:call-template name="buildAttributeTransUnit"/>
</xsl:template>


<xsl:template name="buildElementTransUnit">
	<xsl:param name="id"/>
	<xsl:param name="datatype" select="'plaintext'"/>
	<xsl:param name="restype"/>

	<xsl:variable name="source"><xsl:apply-templates mode="source"/></xsl:variable>
	<trans-unit id="{$id}" datatype="{$datatype}">
		<xsl:if test="string-length($restype) &gt; 0 and not(starts-with($restype,'x-'))">
			<xsl:attribute name="restype"><xsl:value-of select="$restype"/></xsl:attribute>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="local-name()='textarea'">
				<xsl:attribute name="size-unit">x-cell</xsl:attribute>
				<xsl:attribute name="xml:space">preserve</xsl:attribute>
			</xsl:when>
		</xsl:choose>
		<source><xsl:copy-of select="$source"/></source>
		<xsl:if test="$prefilltarget='true'">
			<target><xsl:copy-of select="$source"/></target>
		</xsl:if>
		<!--
		<xsl:call-template name="buildCountGroup">
			<xsl:with-param name="text" select="$source"/>
		</xsl:call-template>
		-->
	</trans-unit>
	<xsl:apply-templates select="descendant::*/@*"/>
</xsl:template>

<xsl:template name="buildAttributeTransUnit">
	<xsl:variable name="id" select="concat((((ancestor::*)[@ekt:id])[last()])/@ekt:id,'.',count(preceding::*),'-attr-',local-name())"/>
	<trans-unit id="{$id}" datatype="plaintext" resname="{local-name()}">
		<xsl:choose>
			<xsl:when test="local-name()='accesskey'">
				<xsl:attribute name="size-unit">char</xsl:attribute>
				<xsl:attribute name="maxwidth">1</xsl:attribute>
			</xsl:when>
			<xsl:when test="local-name()='label'">
				<xsl:attribute name="restype">label</xsl:attribute>
			</xsl:when>
			<xsl:when test="local-name()='src' and local-name(..)='img'">
				<xsl:variable name="width">
					<xsl:choose>
						<xsl:when test="../@width"><xsl:value-of select="../@width"/></xsl:when>
						<xsl:otherwise>#</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:variable name="height">
					<xsl:choose>
						<xsl:when test="../@height"><xsl:value-of select="../@height"/></xsl:when>
						<xsl:otherwise>#</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:attribute name="coord"><xsl:value-of select="concat('#;#;',$width,';',$height)"/></xsl:attribute>
				<xsl:attribute name="reformat">coord-cx coord-cy</xsl:attribute>
			</xsl:when>
		</xsl:choose>
		<source><xsl:value-of select="."/></source>
		<xsl:if test="$prefilltarget='true'">
			<target><xsl:value-of select="."/></target>
		</xsl:if>
		<!-- <xsl:call-template name="buildCountGroup"/> -->
	</trans-unit>
</xsl:template>

<xsl:template name="buildTextTransUnit">
	<xsl:param name="id"/>
	<xsl:variable name="source" select="normalize-space(.)"/>
	<xsl:if test="string-length($source) &gt; 0">
		<trans-unit id="{$id}" datatype="plaintext">
			<source><xsl:copy-of select="$source"/></source>
			<xsl:if test="$prefilltarget='true'">
				<target><xsl:copy-of select="$source"/></target>
			</xsl:if>
			<!-- <xsl:call-template name="buildCountGroup"/> -->
		</trans-unit>
	</xsl:if>
</xsl:template>

<xsl:template name="buildBinTransUnit">
	<xsl:param name="id"/>
	<xsl:param name="mimeType"/>
	<xsl:param name="fileName"/>
	<xsl:if test="string-length($fileName) &gt; 0">
		<bin-unit id="{$id}" mime-type="{$mimeType}">
			<bin-source>
				<external-file href="{$fileName}"/>
			</bin-source>
			<xsl:if test="$prefilltarget='true'">
				<bin-target>
					<external-file href="{$fileName}"/>
				</bin-target>
			</xsl:if>
			<!-- <xsl:if test="/skl:xliff/skl:file/skl:header/skl:reference">
				<note annotates="source" from="Ektron extraction tool" xml:lang="en-US">
					<xsl:text>This file is generated from the referenced file(s).</xsl:text>
				</note>
			</xsl:if> -->
		</bin-unit>
	</xsl:if>
</xsl:template>

<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
function wordCount(s)
{
	// Estimate word count by counting spaces.
	if (typeof s != "string") return 0;
	if (0 == s.length) return 0;
	var a = s.match(/\s+/g);
	if (null == a) return 1;
	return a.length + 1 + s;
}
]]></msxsl:script>

<xsl:template name="buildCountGroup">
	<xsl:param name="text" select="."/>
	<xsl:if test="function-available('js:wordCount')">
		<count-group name="estimate">
			<count count-type="new" unit="word"><xsl:value-of 
				select="js:wordCount(normalize-space($text))"/></count>
		</count-group>
	</xsl:if>
</xsl:template>
-->

<xsl:template match="*" mode="source">
	<xsl:variable name="id" select="@ekt:id"/>
	<xsl:choose>
		<!-- inline empty tags -->
		<xsl:when test="contains('|area|bgsound|br|hr|img|input|param|script|style|',concat('|',local-name(),'|'))">
			<x id="{$id}">
				<xsl:choose>
					<xsl:when test="local-name()='br'">
						<xsl:attribute name="ctype">lb</xsl:attribute>
					</xsl:when>
					<xsl:when test="local-name()='img'">
						<xsl:attribute name="ctype">image</xsl:attribute>
					</xsl:when>
					<xsl:when test="local-name()=name()">
						<xsl:attribute name="ctype"><xsl:value-of select="concat('x-html-',local-name())"/></xsl:attribute>
					</xsl:when>
					<xsl:otherwise> <!-- different namespace -->
						<xsl:attribute name="ctype"><xsl:value-of select="concat('x-',translate(name(),':','-'))"/></xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
			</x>
		</xsl:when>
		<!-- inline non-empty tags -->
		<xsl:otherwise>
			<g id="{$id}">
				<xsl:choose>
					<xsl:when test="local-name()='a'">
						<xsl:attribute name="ctype">link</xsl:attribute>
					</xsl:when>
					<xsl:when test="local-name()='b'">
						<xsl:attribute name="ctype">bold</xsl:attribute>
					</xsl:when>
					<xsl:when test="local-name()='i'">
						<xsl:attribute name="ctype">italic</xsl:attribute>
					</xsl:when>
					<xsl:when test="local-name()='u'">
						<xsl:attribute name="ctype">underlined</xsl:attribute>
					</xsl:when>
					<xsl:when test="local-name()=name()">
						<xsl:attribute name="ctype"><xsl:value-of select="concat('x-html-',local-name())"/></xsl:attribute>
					</xsl:when>
					<xsl:otherwise> <!-- different namespace -->
						<xsl:attribute name="ctype"><xsl:value-of select="concat('x-',translate(name(),':','-'))"/></xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:apply-templates mode="source"/>
			</g>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="text()" mode="source">
	<xsl:copy/>
</xsl:template>

<xsl:template match="@*"/>

</xsl:stylesheet>
