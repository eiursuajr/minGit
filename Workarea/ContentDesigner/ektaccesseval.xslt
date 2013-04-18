<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="xhtml" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xhtml="http://www.w3.org/1999/xhtml">
	<xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>
	
	<xsl:param name="baseURL"/>
	<xsl:param name="outputFormat" select="'text'"/>
	<xsl:variable name="newline">
<xsl:text>
</xsl:text>
	</xsl:variable>
	
	<xsl:template match="/">
		<xsl:choose>
			<xsl:when test="$outputFormat='html'">
			<html>
				<xsl:if test="$baseURL!=''">
				<head>
					<base href="{$baseURL}"/>
				</head>
				</xsl:if>
				<body>
					<ul>
						<xsl:apply-templates select="*/body"/>
					</ul>
				</body>
			</html>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="*/body"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- This XSLT is created with a reference of Rick Jelliffe's "Schematron schema for WAI" -->
	<!-- The original schematron can be found at the following website. -->
	<!-- http://xml.ascc.net/resource/schematron/wai.xml -->
	<xsl:template match="xhtml:img|img">
		<xsl:if test="(not(@alt) or @alt='') and (not(@longdesc) or @longdesc='')">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 1'"/>
			<xsl:variable name="Msg" select="'An image element should have some descriptive text: an alt or longdesc attribute.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong>
					<xsl:if test="$baseURL!=''"><img src='{@src}' alt="image does not have an alt tag" width="100" /></xsl:if>
					<xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:value-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/>'<xsl:value-of select="@src"/>'<xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:a[@href]|a[@href]">
		<xsl:if test="not(@title) or normalize-space(@title)=''">
			<xsl:variable name="Msg1" select="'The hyperlink element'"/>
			<xsl:variable name="Msg2" select="'should have some descriptive text: a title attribute.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><xsl:copy-of select="$Msg1"/> "<xsl:value-of select="."/>" <xsl:copy-of select="$Msg2"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Msg1"/> "<xsl:value-of select="."/>" <xsl:copy-of select="$Msg2"/>'<xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>		
	</xsl:template>

	<xsl:template match="xhtml:input|input">
		<xsl:if test="@type='image' and (not(@alt) or normalize-space(@alt)='')">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 1'"/>
			<xsl:variable name="Msg" select="'An input element with &quot;Image&quot; type should have some descriptive text: an alt attribute.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:applet|applet">
		<xsl:if test="not(@alt) or normalize-space(@alt)=''">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 1'"/>
			<xsl:variable name="Msg" select="'An applet element should have some descriptive text: an alt attribute.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:map|map">
		<xsl:if test="(not(xhtml:area/@alt|area/@alt) or normalize-space(xhtml:area/@alt|area/@alt)='') and not(xhtml:a|a)">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 1'"/>
			<xsl:variable name="Msg" select="'A map element should have some descriptive text: an alt attribute or a link.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:object|object">
		<xsl:if test="string-length(text()) &gt; 0">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 1'"/>
			<xsl:variable name="Msg" select="'An object element should contain some descriptive text.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:table|table">
		<xsl:if test="descendant::xhtml:th|descendant::th">
			<xsl:if test="not(@summary) or normalize-space(@summary)=''">
				<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 5'"/>
				<xsl:variable name="Msg" select="'A table should have a summary attribute.'"/>
				<xsl:choose>
					<xsl:when test="$outputFormat='html'">
						<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
					</xsl:when>
					<xsl:otherwise>
	* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
					</xsl:otherwise>
				</xsl:choose>	
			</xsl:if>
			<xsl:if test="not(xhtml:caption|caption) or normalize-space(xhtml:caption|caption)=''">
				<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 5'"/>
				<xsl:variable name="Msg" select="'A table should have a caption.'"/>
				<xsl:choose>
					<xsl:when test="$outputFormat='html'">
						<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
					</xsl:when>
					<xsl:otherwise>
	* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
					</xsl:otherwise>
				</xsl:choose>	
			</xsl:if>	
		</xsl:if>		 
	</xsl:template>

	<xsl:template match="xhtml:td|td">
		<xsl:if test="(not(@axis)or normalize-space(@axis)='') and (not(@headers)or normalize-space(@headers)='') and (not(@scope)or normalize-space(@scope)='')">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 5'"/>
			<xsl:variable name="Msg" select="'Table data should identify its scope, headers, axis in attributes'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/> "<xsl:value-of select="."/>".</li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/> "<xsl:value-of select="."/>".<xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="xhtml:th|th">
		<xsl:if test="not(@abbr) or normalize-space(@abbr)=''">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 5'"/>
			<xsl:variable name="Msg" select="'A table header should have an abbr attribute to give abbreviation'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
			<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/> "<xsl:value-of select="."/>".</li>
			</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/> "<xsl:value-of select="."/>".<xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[@onmouseup]">
		<xsl:if test="not(@onkeyup) or normalize-space(@onkeyup)=''">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 6'"/>
			<xsl:variable name="Msg" select="'If you specify an &quot;onmouseup&quot; attribute on an element, you should also specify an &quot;onkeyup&quot; attribute.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[@onmousedown]">
		<xsl:if test="not(@onkeydown) or normalize-space(@onkeydown)=''">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 6'"/>
			<xsl:variable name="Msg" select="'If you specify an &quot;onmousedown&quot; attribute on an element, you should also specify an &quot;onkeydown&quot; attribute.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[@onclick]">
		<xsl:if test="not(@onkeypress) or normalize-space(@onkeypress)=''">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 6'"/>
			<xsl:variable name="Msg" select="'If you specify an &quot;onclick&quot; attribute on an element, you should also specify an &quot;onkeypress&quot; attribute'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='marquee']">
		<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 6'"/>
		<xsl:variable name="Msg" select="'The marquee element is not good HTML.'"/>
		<xsl:choose>
				<xsl:when test="$outputFormat='html'">	
					<!--<xsl:template match="marquee">-->
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
	</xsl:template>

	<xsl:template match="*[local-name()='blink']">
		<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0, Guideline 6'"/>
		<xsl:variable name="Msg" select="'The blink element is not good HTML.'"/>
		<xsl:choose>
			<xsl:when test="$outputFormat='html'">
				<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
			</xsl:when>
			<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
			</xsl:otherwise>
		</xsl:choose>	
	</xsl:template>

	<xsl:template match="xhtml:fieldset|fieldset">
		<xsl:if test="not(xhtml:legend|legend) or normalize-space(xhtml:legend|legend)=''">
			<xsl:variable name="Header" select="'Web Content Accessibility Guidelines 1.0'"/>
			<xsl:variable name="Msg" select="'A fieldset should have a legend.'"/>
			<xsl:choose>
				<xsl:when test="$outputFormat='html'">
					<li><strong><xsl:copy-of select="$Header"/></strong><xsl:copy-of select="$Msg"/></li>
				</xsl:when>
				<xsl:otherwise>
* <xsl:copy-of select="$Header"/><xsl:value-of select="$newline"/><xsl:copy-of select="$Msg"/><xsl:value-of select="$newline"/><xsl:value-of select="$newline"/>
				</xsl:otherwise>
			</xsl:choose>	
		</xsl:if>
	</xsl:template>

	<xsl:template match="text()"/>
</xsl:stylesheet>
