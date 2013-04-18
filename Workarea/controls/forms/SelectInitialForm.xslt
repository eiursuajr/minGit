<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- Run on InitialFormsManifest.xml -->

<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="lang"/>
<xsl:param name="delimiter" select="';'"/>

<xsl:decimal-format NaN="0"/>

<xsl:template match="/">
	<xsl:variable name="theForms" select="Manifest/Forms/Form[lang($lang) or not(@xml:lang) or not($lang)]"/>
	<div>
	<script type="text/javascript">
	<xsl:comment>
	var aryInitialForms = [
	<xsl:apply-templates select="$theForms" mode="script"/>
	];
	<xsl:apply-templates select="$theForms" mode="scriptInit"/>
	// </xsl:comment>
	</script>
	<xsl:apply-templates select="$theForms"/>
	</div>
</xsl:template>

<xsl:template match="Form" mode="script">
	<xsl:if test="position() &gt; 1">,</xsl:if>
	{ title:"<xsl:value-of select="Title"/>"
	, description:"<xsl:value-of select="Description"/>"
	<xsl:if test="@type">
	, type:"<xsl:value-of select="@type"/>"
	</xsl:if>
	<xsl:if test="Mail">
	, mail:<xsl:apply-templates select="Mail" mode="script"/>
	</xsl:if>
	<xsl:if test="Submit">
	, submit:<xsl:apply-templates select="Submit" mode="script"/>
	</xsl:if>
	, designSrc:"<xsl:value-of select="Design/@src"/>"
	, responseSrc:"<xsl:value-of select="Response/@src"/>" }
</xsl:template>

<xsl:template match="Mail" mode="script">
	<xsl:if test="position() &gt; 1">,</xsl:if>
	{ to:"<xsl:apply-templates select="To" mode="script"/>"
	, from:"<xsl:apply-templates select="From" mode="script"/>"
	, cc:"<xsl:apply-templates select="CC" mode="script"/>"
	, subject:"<xsl:apply-templates select="Subject" mode="script"/>"
	, messageBody:"<xsl:apply-templates select="MessageBody" mode="script"/>" }
</xsl:template>

<xsl:template match="Mail/*" mode="script">
	<xsl:if test="position() &gt; 1"><xsl:value-of select="$delimiter"/></xsl:if>
	<xsl:choose>
		<xsl:when test="@field">
			<xsl:value-of select="concat('&#171;',@field,'&#187;')"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="Submit" mode="script">
	<xsl:if test="position() &gt; 1">,</xsl:if>
	{ limit:<xsl:value-of select="format-number(@limit, '0')"/>
	, autofill:<xsl:value-of select="@autofill"/>}
</xsl:template>

<xsl:template match="Form" mode="scriptInit">
	<xsl:if test="@default='true'">
	<xsl:variable name="index" select="position() - 1"/>
	setTimeout("onSelectInitialForm(aryInitialForms[<xsl:value-of select="$index"/>])", 1);
	</xsl:if>
</xsl:template>

<xsl:template match="Form">
	<xsl:variable name="index" select="position() - 1"/>
	<xsl:variable name="id" select="concat('initialForm',$index)"/>
	<div style="margin-top: .5em;">
		<input type="radio" id="{$id}" name="initialForm" onclick="onSelectInitialForm(aryInitialForms[{$index}])">
			<xsl:if test="@default='true'">
				<xsl:attribute name="checked">checked</xsl:attribute>
			</xsl:if>
		</input>&#160;<label for="{$id}" style="color:navy;">
			<xsl:call-template name="copyData">
				<xsl:with-param name="data" select="Title/node()"/>
			</xsl:call-template>
		</label>
		<xsl:if test="string-length(Design/@src) &gt; 0">
			<xsl:text></xsl:text>
			<a href="PreviewForm.aspx?design=controls/forms/{Design/@src}" title="Preview the form in a new window" target="_blank" style="display: inline-block; margin-left: .5em"><img
			src="images/ui/icons/contentViewPublished.png" width="16" height="16" alt="preview icon" title="Preview the form in a new window" border="0" /></a>
		</xsl:if>
		<div style="margin-left: 2.12em">
			<span class="ektronCaption">
				<xsl:call-template name="copyData">
					<xsl:with-param name="data"
									select="Description/node()"/>
				</xsl:call-template>
			</span>
		</div>
	</div>
</xsl:template>

<xsl:include href="../template_copyData.xslt"/>

</xsl:stylesheet>