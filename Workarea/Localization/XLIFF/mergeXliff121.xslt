<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl skl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:ekt="urn:ektron:xliff" xmlns:skl="urn:oasis:names:tc:xliff:document:1.1" 
xmlns:xlf="urn:oasis:names:tc:xliff:document:1.2.1"
xmlns:its="http://www.w3.org/2005/11/its">

<!-- run this transform on the xliff file -->

<xsl:import href="mergeXliffBase.xslt"/>

<xsl:strip-space elements="xlf:target"/>

<xsl:variable name="lang" select="/xlf:xliff/xlf:file/@target-language"/>

<xsl:template match="/">
	<xsl:for-each select="xlf:xliff/xlf:file">
		<xsl:variable name="skeleton" select="concat($path,xlf:header/xlf:skl/xlf:external-file/@href)"/>
		<xsl:apply-templates select="document($skeleton, /)/*" mode="skl"> <!-- use base URI of XLIFF file, if known -->
			<xsl:with-param name="xliff-context" select="xlf:body"/>
		</xsl:apply-templates>
	</xsl:for-each>
</xsl:template>

<xsl:template match="skl:header/skl:prop-group[@name='Ektron']/skl:prop" mode="skl">
	<xsl:param name="xliff-context"/>
	<xsl:variable name="new-xliff-context" select="$xliff-context/../xlf:header/xlf:prop-group[@name='Ektron']/xlf:prop[@prop-type=current()/@prop-type]"/>
	<xsl:choose>
		<xsl:when test="$new-xliff-context">
			<xsl:copy>
				<xsl:copy-of select="@*|$new-xliff-context/@*"/>
				<xsl:copy-of select="$new-xliff-context/node()"/>
			</xsl:copy>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="skl:header/skl:tool[@tool-id='Ektron']/*" mode="skl">
	<xsl:param name="xliff-context"/>
	<xsl:variable name="new-xliff-context" select="$xliff-context/../xlf:header/xlf:tool[@tool-id='Ektron']/*[@name=current()/@name]"/>
	<xsl:choose>
		<xsl:when test="$new-xliff-context">
			<xsl:copy>
				<xsl:copy-of select="@*|$new-xliff-context/@*"/>
				<xsl:copy-of select="$new-xliff-context/node()"/>
			</xsl:copy>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="skl:group[@resname and not(@id)]" mode="skl">
	<xsl:param name="xliff-context"/>

	<xsl:variable name="resname" select="@resname"/>
	<xsl:variable name="new-xliff-context" select="$xliff-context/xlf:group[@resname=$resname]"/>

	<xsl:if test="$new-xliff-context or (not(@ts='nodefault') and not(@ekt:required='false'))">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" mode="skl">
				<xsl:with-param name="xliff-context" select="$new-xliff-context"/>
			</xsl:apply-templates>
		</xsl:copy>
	</xsl:if>
</xsl:template>

<xsl:template match="skl:group[@resname='MetaData']/skl:group/@resname" mode="skl">
	<xsl:param name="xliff-context"/>

	<xsl:variable name="id" select="."/>
	<xsl:variable name="targLang" select="$xliff-context/ancestor::xlf:file/@target-language"/>
	<xsl:variable name="new-id" select="$metaDefns/dl/dd[dfn/@id=$id]/dfn[lang($targLang)]/@id"/>
	<xsl:choose>
		<xsl:when test="$new-id">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="$new-id"/>
			</xsl:attribute>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="AssetData" mode="skl">
	<xsl:param name="xliff-context"/>

	<!--
		<AssetData ekt:id="{unit-id}">
			<Id>0f11fe66-c0b9-4e3b-a173-635719c8f7f6</Id>
			<Version>8b4245288e864d76bfb4403c50fe41db1.wmv</Version>
			<MimeType>video/x-ms-wmv</MimeType>
			<FileName>subfolder/filename.wmv</FileName>
		</AssetData>
	-->
	<xsl:variable name="id" select="@ekt:id"/>
	<xsl:variable name="binUnit" select="$xliff-context/xlf:bin-unit[@id=$id]"/>
	<xsl:variable name="extFile" select="$binUnit/xlf:bin-target/xlf:external-file/@href"/>
	<xsl:if test="string-length($extFile) &gt; 0">
		<xsl:copy>
			<xsl:for-each select="*">
				<xsl:choose>
					<xsl:when test="local-name()='MimeType'">
						<xsl:copy>
							<xsl:value-of select="$binUnit/@mime-type"/>
						</xsl:copy>
					</xsl:when>
					<xsl:when test="local-name()='Version'">
						<xsl:copy>
							<xsl:value-of select="$extFile"/>
						</xsl:copy>
					</xsl:when>
					<xsl:when test="local-name()='FileName'">
						<xsl:copy>
							<xsl:value-of select="$extFile"/>
						</xsl:copy>
					</xsl:when>
					<xsl:otherwise>
						<xsl:copy-of select="."/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</xsl:copy>
	</xsl:if>
</xsl:template>

<xsl:template match="skeleton//text()">
	<xsl:param name="xliff-context"/>

	<xsl:variable name="id" select="../@ekt:id"/>
	<xsl:variable name="tu" select="$xliff-context/xlf:trans-unit[@id=$id]"/>
	<xsl:variable name="target" select="$tu/xlf:target"/>
	<xsl:variable name="prevVer" select="$tu/xlf:alt-trans[@alttranstype='previous-version']/xlf:target[@state-qualifier='exact-match']"/>
	<xsl:choose>
		<xsl:when test="count($target) &gt; 0">
			<xsl:apply-templates select="$target/node()" mode="xliff">
				<xsl:with-param name="xliff-context" select="$xliff-context"/>
				<xsl:with-param name="xhtml-context" select="."/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:when test="count($prevVer) &gt; 0">
			<xsl:apply-templates select="$prevVer/node()" mode="xliff">
				<xsl:with-param name="xliff-context" select="$xliff-context"/>
				<xsl:with-param name="xhtml-context" select="."/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="processTransUnit">
	<xsl:param name="xliff-context"/>

	<xsl:variable name="id" select="@ekt:id"/>
	<xsl:variable name="tu" select="$xliff-context/xlf:trans-unit[@id=$id]"/>
	<xsl:variable name="target" select="$tu/xlf:target"/>
	<xsl:variable name="prevVer" select="$tu/xlf:alt-trans[@alttranstype='previous-version']/xlf:target[@state-qualifier='exact-match']"/>
	<xsl:choose>
		<xsl:when test="count($target) &gt; 0">
			<xsl:apply-templates select="$target/node()" mode="xliff">
				<xsl:with-param name="xliff-context" select="$xliff-context"/>
				<xsl:with-param name="xhtml-context" select="."/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:when test="count($prevVer) &gt; 0">
			<xsl:apply-templates select="$prevVer/node()" mode="xliff">
				<xsl:with-param name="xliff-context" select="$xliff-context"/>
				<xsl:with-param name="xhtml-context" select="."/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:when test="$xliff-context/xlf:group[@id=$id and @ts='hs-split']"> <!-- Heartsome split segment -->
			<xsl:variable name="new-xliff-context" select="$xliff-context/xlf:group[@id=$id and @ts='hs-split']"/>
			<xsl:apply-templates select="$new-xliff-context/xlf:trans-unit/xlf:target/node()" mode="xliff">
				<xsl:with-param name="xliff-context" select="$new-xliff-context"/>
				<xsl:with-param name="xhtml-context" select="."/>
			</xsl:apply-templates>				
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates>
				<xsl:with-param name="xliff-context" select="$xliff-context"/>
			</xsl:apply-templates>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="xlf:g|xlf:x" mode="xliff">
	<xsl:param name="xliff-context"/>
	<xsl:param name="xhtml-context"/>

	<xsl:variable name="id" select="@id"/>
	<!-- #30587 <xsl:variable name="new-xhtml-context" select="((($xhtml-context/ancestor-or-self::*)[@ekt:id])[last()])/node()[position()=$id]"/>-->
	<xsl:variable name="new-xhtml-context" select="$xhtml-context//*[@ekt:id=$id]"/>
	<xsl:variable name="name" select="name($new-xhtml-context)"/>
	<xsl:variable name="ns" select="namespace-uri($new-xhtml-context)"/>
	<xsl:choose>
		<xsl:when test="not($name)">
			<!-- inline element added by translator that was not in the original -->
			<xsl:choose>
				<xsl:when test="@ctype='lb'">
					<br />
				</xsl:when>
				<xsl:when test="contains('|bold|italic|underlined|',concat('|',@ctype,'|'))">
					<xsl:variable name="tagName">
						<xsl:choose>
							<xsl:when test="@ctype='bold'">b</xsl:when>
							<xsl:when test="@ctype='italic'">i</xsl:when>
							<xsl:when test="@ctype='underlined'">u</xsl:when>
						</xsl:choose>
					</xsl:variable>
					<xsl:element name="{$tagName}">
						<xsl:call-template name="addAttributes">
							<xsl:with-param name="xliff-context" select="$xliff-context"/>
							<xsl:with-param name="xhtml-context" select="$xhtml-context"/>
						</xsl:call-template>
						<!-- process attributes separately to maintain consistent position() -->
						<xsl:apply-templates select="node()" mode="xliff">
							<xsl:with-param name="xliff-context" select="$xliff-context"/>
							<xsl:with-param name="xhtml-context" select="$xhtml-context"/>
						</xsl:apply-templates>
					</xsl:element>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="node()" mode="xliff">
						<xsl:with-param name="xliff-context" select="$xliff-context"/>
						<xsl:with-param name="xhtml-context" select="$xhtml-context"/>
					</xsl:apply-templates>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$name='script'">
			<xsl:copy-of select="$new-xhtml-context"/>
		</xsl:when>
		<!-- empty tags -->
		<xsl:when test="contains('|area|bgsound|br|hr|img|input|param|',concat('|',$name,'|'))">
			<xsl:element name="{$name}" namespace="{$ns}">
				<xsl:call-template name="addAttributes">
					<xsl:with-param name="xliff-context" select="$xliff-context"/>
					<xsl:with-param name="xhtml-context" select="$new-xhtml-context"/>
				</xsl:call-template>
			</xsl:element>
		</xsl:when>
		<xsl:otherwise>
			<xsl:element name="{$name}" namespace="{$ns}">
				<xsl:call-template name="addAttributes">
					<xsl:with-param name="xliff-context" select="$xliff-context"/>
					<xsl:with-param name="xhtml-context" select="$new-xhtml-context"/>
				</xsl:call-template>
				<!-- process attributes separately to maintain consistent position() -->
				<xsl:apply-templates select="node()" mode="xliff">
					<xsl:with-param name="xliff-context" select="$xliff-context"/>
					<xsl:with-param name="xhtml-context" select="$new-xhtml-context"/>
				</xsl:apply-templates>
			</xsl:element>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="xlf:*" mode="xliff">
	<xsl:param name="xliff-context"/>
	<xsl:param name="xhtml-context"/>
	<xsl:apply-templates select="node()" mode="xliff">
		<xsl:with-param name="xliff-context" select="$xliff-context"/>
		<xsl:with-param name="xhtml-context" select="$xhtml-context"/>
	</xsl:apply-templates>
</xsl:template>

<xsl:template name="addAttributes">
	<xsl:param name="xliff-context"/>
	<xsl:param name="xhtml-context" select="."/>
	<xsl:apply-templates select="$xhtml-context/@*[name()!='xml:space']">
		<xsl:with-param name="xliff-context" select="$xliff-context"/>
	</xsl:apply-templates>
	<xsl:if test="count($xliff-context) &gt; 0">
		<xsl:variable name="id-prefix" select="concat(((($xhtml-context/ancestor-or-self::*)[@ekt:id])[last()])/@ekt:id,'.',count($xhtml-context/preceding::*),'-attr-')"/>
		<xsl:for-each select="$xliff-context/xlf:trans-unit[starts-with(@id,$id-prefix) and @resname]">
			<!-- preceding::* is the number of complete elements, not opening tags, so id-prefix may not
					be unique and therefore may apply to more than one tag. Test if the attribute already
					exists in the skl to reduce the possibility of applying the target to more than one tag. -->
			<xsl:if test="$xhtml-context/@*[name()=current()/@resname]">
				<xsl:variable name="prevVer" select="../xlf:alt-trans[@alttranstype='previous-version']/xlf:target[@state-qualifier='exact-match']"/>
				<xsl:choose>
					<xsl:when test="xlf:target">
					<xsl:attribute name="{@resname}"><xsl:value-of select="xlf:target"/></xsl:attribute>
					</xsl:when>
					<xsl:when test="$prevVer">
						<xsl:attribute name="{@resname}"><xsl:value-of select="$prevVer"/></xsl:attribute>
					</xsl:when>
				</xsl:choose>
				<xsl:if test="@resname='src' and @coord">
					<!-- @width and @height are stored in @src's trans-unit/@coord="#;#;width;height" -->
					<xsl:variable name="coord-cx">
						<xsl:value-of select="substring-before(substring-after(@coord,'#;#;'),';')"/>
					</xsl:variable>
					<xsl:variable name="coord-cy">
						<xsl:value-of select="substring-after(substring-after(@coord,'#;#;'),';')"/>
					</xsl:variable>
					<xsl:if test="not($coord-cx='#') and string-length($coord-cx) &gt; 0">
						<xsl:attribute name="width"><xsl:value-of select="$coord-cx"/></xsl:attribute>
					</xsl:if>
					<xsl:if test="not($coord-cy='#') and string-length($coord-cy) &gt; 0">
						<xsl:attribute name="height"><xsl:value-of select="$coord-cy"/></xsl:attribute>
					</xsl:if>
				</xsl:if>
			</xsl:if>
		</xsl:for-each>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
