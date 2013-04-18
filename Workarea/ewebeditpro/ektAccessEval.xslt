<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="xhtml" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xhtml="http://www.w3.org/1999/xhtml">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>
	
	<!-- This XSLT is created with a reference of Rick Jelliffe's "Schematron schema for WAI" -->
	<!-- The original schematron can be found at the following website. -->
	<!-- http://xml.ascc.net/resource/schematron/wai.xml -->
	<xsl:template match="xhtml:img|img">
		<xsl:if test="(not(@alt) or @alt='') and (not(@longdesc) or @longdesc='')">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 1</strong>
			<img src='{@src}' alt="image does not have an alt tag" width="100" />
			An image element should have some descriptive text: an alt or longdesc attribute.</li>
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:a|a">
		<xsl:if test="not(@title) or normalize-space(@title)=''">
			<li>The hyperlink element "<xsl:value-of select="."/>" should have some descriptive text: a title attribute.</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="xhtml:input|input">
		<xsl:if test="@type='image' and (not(@alt) or normalize-space(@alt)='')">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 1</strong>
			An input element with "Image" type should have some descriptive text: an alt attribute.</li>
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:applet|applet">
		<xsl:if test="not(@alt) or normalize-space(@alt)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 1</strong>
			An applet element should have some descriptive text: an alt attribute.</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="xhtml:map|map">
		<xsl:if test="(not(xhtml:area/@alt|area/@alt) or normalize-space(xhtml:area/@alt|area/@alt)='') and not(xhtml:a|a)">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 1</strong>
			A map element should have some descriptive text: an alt attribute or a link.</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="xhtml:object|object">
		<xsl:if test="string-length(text()) &gt; 0">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 1</strong>
			An object element should contain some descriptive text.</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="xhtml:table|table">
		<xsl:if test="not(@summary) or normalize-space(@summary)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 5</strong>
			A table should have a summary attribute.</li>
		</xsl:if>
		<xsl:if test="not(xhtml:caption|caption) or normalize-space(xhtml:caption|caption)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 5</strong>
			A table should have a caption.</li>
		</xsl:if>			 
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="xhtml:td|td">
		<xsl:if test="(not(@axis)or normalize-space(@axis)='') and (not(@headers)or normalize-space(@headers)='') and (not(@scope)or normalize-space(@scope)='')">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 5</strong>
			Table data should identify its scope, headers, axis in attributes "<xsl:value-of select="."/>".</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="xhtml:th|th">
		<xsl:if test="not(@abbr) or normalize-space(@abbr)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 5</strong>
			A table header should have an abbr attribute to give abbreviation "<xsl:value-of select="."/>".</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="*[@onmouseup]">
		<xsl:if test="not(@onkeyup) or normalize-space(@onkeyup)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 6</strong>
			If you specify an "onmouseup" attribute on an element, you should also specify an "onkeyup" attribute</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="*[@onmousedown]">
		<xsl:if test="not(@onkeydown) or normalize-space(@onkeydown)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 6</strong>
			If you specify an "onmousedown" attribute on an element, you should also specify an "onkeydown" attribute</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="*[@onclick]">
		<xsl:if test="not(@onkeypress) or normalize-space(@onkeypress)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 6</strong>
			If you specify an "onclick" attribute on an element, you should also specify an "onkeypress" attribute</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="*[local-name()='marquee']">
	<!--<xsl:template match="marquee">-->
		<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 6</strong>
		The marquee element is not good HTML</li>
	</xsl:template>

	<xsl:template match="*[local-name()='blink']">
		<li><strong>Web Content Accessibility Guidelines 1.0, Guideline 6</strong>
		The blink element is not good HTML</li>
	</xsl:template>

	<xsl:template match="xhtml:fieldset|fieldset">
		<xsl:if test="not(xhtml:legend|legend) or normalize-space(xhtml:legend|legend)=''">
			<li><strong>Web Content Accessibility Guidelines 1.0</strong>
			A fieldset should have a legend.</li>
		</xsl:if>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="text()"/>
</xsl:stylesheet>