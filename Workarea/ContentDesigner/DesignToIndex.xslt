<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

<!--

Outputs 

<indexable>
	<xpath type="string">/root/group/element</xpath>
</indexable>

-->

<xsl:include href="template_paramDesignTo.xslt"/>

<xsl:template match="/">
	<xsl:element name="indexable">
		<xsl:apply-templates>
		    <xsl:with-param name="xpath" select="$rootXPath"/>
		</xsl:apply-templates>
	</xsl:element>
</xsl:template>

<!-- processElement -->

<xsl:template name="processElement">
	<xsl:param name="xpath"/>
	<xsl:param name="xpath-extension" select="''"/>
	<xsl:param name="knowntype" select="''"/>
	
	<xsl:if test="@ektdesignns_indexed='true' or @ektdesignns_indexed='1'">

		<xsl:variable name="new-xpath">
			<xsl:call-template name="buildXPath">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:call-template>
			<xsl:value-of select="$xpath-extension"/>
		</xsl:variable>
		
		<!-- override any datatypes we want to output here, otherwise, we use the ektdesignns_datatype -->
		<xsl:variable name="datatype">
			<xsl:choose>
				<xsl:when test="$knowntype">
					<xsl:value-of select="$knowntype"/>
				</xsl:when>
				<xsl:when test="name()='ektdesignns_checklist' or name()='ektdesignns_choices' or name()='select'">
					<xsl:value-of select="'selection'"/>
				</xsl:when>
				<xsl:when test="name()='ektdesignns_calendar'">
					<xsl:value-of select="'date'"/>
				</xsl:when>
				<xsl:when test="name()='ektdesignns_richarea'">
					<xsl:value-of select="'anyType'"/>
				</xsl:when>
				<xsl:when test="@ektdesignns_datatype">
					<xsl:value-of select="@ektdesignns_datatype"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''"/> <!-- string by default -->
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
	
		<xsl:element name="xpath">
			<xsl:if test="$datatype != ''">
				<xsl:attribute name="type">
					<xsl:value-of select="$datatype"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$new-xpath"/>
		</xsl:element>
		
	</xsl:if>
</xsl:template>


<!-- match fieldset/legend (process fields inside fieldset) -->
	
<!-- higher priority than template above '*[@...]' -->
<!-- was fieldset[@class='design_group'] -->
<xsl:template match="ektdesignns_group|fieldset[@ektdesignns_nodetype='element']|table[@ektdesignns_nodetype='element']|tr[@ektdesignns_nodetype='element']" priority="1">
	<xsl:param name="xpath"/>
	
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	
	<xsl:apply-templates>
		<xsl:with-param name="xpath" select="$new-xpath"/>
	</xsl:apply-templates>
</xsl:template>

<!-- match any -->

<xsl:template match="*[@ektdesignns_nodetype or starts-with(name(),'ektdesignns_')]">
	<xsl:param name="xpath"/>
	
	<xsl:choose>
	
		<xsl:when test="name()='ektdesignns_imageonly'">
			<xsl:choose>
				<xsl:when test="@ektdesignns_nodetype='attribute'">
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="knowntype" select="'anyURI'"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:when test="contains(@ektdesignns_content,'/@')">
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="xpath-extension" select="'/text()'"/>
						<xsl:with-param name="knowntype" select="'anyURI'"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="xpath-extension" select="'/img/@alt'"/>
					</xsl:call-template>
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="xpath-extension" select="'/img/@src'"/>
						<xsl:with-param name="knowntype" select="'anyURI'"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="name()='ektdesignns_filelink'">
			<xsl:choose>
				<xsl:when test="@ektdesignns_nodetype='attribute'">
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="knowntype" select="'anyURI'"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:when test="contains(@ektdesignns_content,'/@')">
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="xpath-extension" select="'/text()'"/>
						<xsl:with-param name="knowntype" select="'anyURI'"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="xpath-extension" select="'/a/text()'"/>
					</xsl:call-template>
					<xsl:call-template name="processElement">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="xpath-extension" select="'/a/@href'"/>
						<xsl:with-param name="knowntype" select="'anyURI'"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="name()='ektdesignns_resource'">
			<xsl:call-template name="processElement">
				<xsl:with-param name="xpath" select="$xpath"/>
				<xsl:with-param name="knowntype" select="'long'"/>
			</xsl:call-template>
			<xsl:call-template name="processElement">
				<xsl:with-param name="xpath" select="$xpath"/>
				<xsl:with-param name="xpath-extension" select="'/@datavalue_idtype'"/>
			</xsl:call-template>
		</xsl:when>
		
		<xsl:otherwise>
			<xsl:call-template name="processElement">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:call-template>			
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- match checklist|choices -->

<xsl:template match="*[@ektdesignns_nodetype and (@ektdesignns_content='checklist' or @ektdesignns_content='choices')]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>


<!-- match input type=text|hidden -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden') and @ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>


<!-- match input type=checkbox -->

<xsl:template match="input[@type='checkbox' and @ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
		<xsl:with-param name="knowntype" select="'boolean'"/>
	</xsl:call-template>
</xsl:template>


<!-- match select/option -->

<xsl:template match="select[@ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="select[@multiple and @ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>


<!-- match textarea -->

<xsl:template match="textarea[@ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>


<!-- templates ========================================================================== -->

<xsl:include href="template_buildXPath.xslt"/>

<!-- ignore prototypes -->
<xsl:template match="*[@class='design_prototype']"/>

<!-- remove text and attribute values -->
<xsl:template match="text()|@*"/>


<!-- ignore other non-design tags, but pass through the xpath parameter to child node template handlers -->
<xsl:template match="*[not(@ektdesignns_name|@ektdesignns_bind)]">
	<xsl:param name="xpath"/>
	<xsl:apply-templates>
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:apply-templates>
</xsl:template>

<xsl:template match="*[@class='design_membrane']">
	<xsl:param name="xpath"/>
	<xsl:apply-templates>
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:apply-templates>
</xsl:template>

</xsl:stylesheet>
