<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl js dl xslout msxslout" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:xslout="alias" xmlns:msxslout="aliasms" xmlns:dl="urn:datalist">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<!--

Run on design page.

Outputs 

<fieldlist>
	<field name="fieldname1" datatype="validationName1" content="type1" xpath="/root/group/element1" title="title1" indexed="true">display name1</field>
	<field name="fieldname2" datatype="selection" datalist="uniqueId1" content="type2" xpath="/root/group/element2"  title="title2">display name1</field>
	...
	<field name="fieldnamen" datatype="validationNamen" content="typen" xpath="/root/group/elementn" title="titlen">display namen</field>
	- Note: datalist is same as in configdataentryfeature -
	<datalist name="uniqueId1">
		<item value="1">displayValue1</item>
		:
		<item value="n">displayValuen</item>
	</datalist>
	:
	<datalist name="uniqueIdn">
		<item value="1">displayValue1</item>
		:
		<item value="n">displayValuen</item>
	</datalist>
</fieldlist>

-->

<!-- Needs to be /root to signify that these fields are from the editor. Don't use '/*' -->
<xsl:include href="template_paramDesignTo.xslt"/>

<xsl:param name="fieldlistXPath"/> <!-- required for template_datalist.xslt -->

<xsl:template match="/">
	<xsl:variable name="fields_rtf">
		<root>
			<xsl:apply-templates>
			    <xsl:with-param name="xpath" select="$rootXPath"/>
			</xsl:apply-templates>
		</root>
	</xsl:variable>
	<xsl:variable name="fields" select="msxsl:node-set($fields_rtf)/*"/>
	<xsl:if test="count($fields/field) &gt; 0">
		<fieldlist>
			<xsl:copy-of select="$fields/field"/>
			<xsl:copy-of select="$fields/datalist[not(@name=preceding-sibling::datalist/@name)]"/>
		</fieldlist>
	</xsl:if>
</xsl:template>

<!-- processElement -->

<xsl:template name="processElement">
	<xsl:param name="xpath"/>
	<xsl:param name="xpath-extension" select="''"/>
	<xsl:param name="knowntype" select="''"/>

	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
		<xsl:value-of select="$xpath-extension"/>
	</xsl:variable>

	<!-- override any datatypes we want to output here, otherwise, we use the ektdesignns_datatype -->
	<xsl:variable name="datatype">
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_checklist' or (name()='select' and @multiple)">
				<xsl:value-of select="'selection'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_choices' or (name()='select' and not(@multiple))">
				<xsl:value-of select="'choice'"/>
			</xsl:when>
			<xsl:when test="@ektdesignns_validation and @ektdesignns_validation!='content' and @ektdesignns_validation!='content-req' and @ektdesignns_validation!='string-req'">
				<xsl:choose>
					<xsl:when test="substring(@ektdesignns_validation, string-length(@ektdesignns_validation)-3, 4) = '-req'">
						<xsl:value-of select="substring(@ektdesignns_validation, 1, string-length(@ektdesignns_validation)-4)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="@ektdesignns_validation"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="@ektdesignns_datatype">
				<xsl:value-of select="@ektdesignns_datatype"/>
			</xsl:when>
			<xsl:when test="$knowntype">
				<xsl:value-of select="$knowntype"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_calendar'">
				<xsl:value-of select="'date'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_richarea'">
				<xsl:value-of select="'anyType'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'string'"/>
				<!-- string by default -->
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="basetype">
		<xsl:choose>
			<xsl:when test="@ektdesignns_basetype">
				<xsl:value-of select="@ektdesignns_basetype"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_calendar' or $datatype='date'">
				<xsl:value-of select="'calendar'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_richarea'">
				<xsl:value-of select="'html'"/>
			</xsl:when>
			<xsl:when test="name()='textarea'">
				<xsl:value-of select="'textbox'"/>
			</xsl:when>
			<xsl:when test="(name()='input' and @type='checkbox') or $datatype='boolean'">
				<xsl:value-of select="'checkbox'"/>
			</xsl:when>
			<xsl:when test="(name()='input' and @type='hidden')">
				<!-- use 'text' rather than 'hidden' b/c it fundamentally 
					doesn't matter whether the data was hidden or not -->
				<xsl:value-of select="'text'"/> 
			</xsl:when>
			<xsl:when test="(name()='input' and @type='password')">
				<xsl:value-of select="'password'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'text'"/>
				<!-- text by default -->
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<xsl:variable name="datalistName">
		<xsl:call-template name="getDatalistName"/>
	</xsl:variable>

	<field name="{@ektdesignns_name}" datatype="{$datatype}" basetype="{$basetype}" xpath="{$new-xpath}" title="{@title}">
		<xsl:choose>
			<xsl:when test="@ektdesignns_content">
				<xsl:attribute name="content"><xsl:value-of select="@ektdesignns_content"/></xsl:attribute>
			</xsl:when>
			<xsl:when test="name()='textarea'">
				<xsl:attribute name="content">textarea</xsl:attribute>
			</xsl:when>
		</xsl:choose>
		<xsl:if test="@ektdesignns_indexed='true' or @ektdesignns_indexed='1'">
			<xsl:attribute name="indexed">true</xsl:attribute>
		</xsl:if>
		<xsl:if test="$datatype='selection' or $datatype='choice'">
			<xsl:attribute name="datalist"><xsl:value-of select="$datalistName"/></xsl:attribute>
		</xsl:if>
		<xsl:if test="@ektdesignns_calculate">
			<xsl:attribute name="translate">false</xsl:attribute>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="@ektdesignns_caption">
				<xsl:value-of select="@ektdesignns_caption"/>
			</xsl:when>
			<xsl:when test="@ektdesignns_label"> <!-- legacy, deprecated -->
				<xsl:value-of select="@ektdesignns_label"/>
			</xsl:when>
			<xsl:when test="@title"> <!-- legacy, now used for tooltip -->
				<xsl:value-of select="@title"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@ektdesignns_name"/>
			</xsl:otherwise>
		</xsl:choose>
	</field>
	<xsl:if test="$datatype='selection' or $datatype='choice'">
		<datalist name="{$datalistName}">
			<xsl:choose>
				<xsl:when test="@ektdesignns_datasrc">
					<xsl:copy-of select="@ektdesignns_datasrc|@ektdesignns_dataselect|@ektdesignns_captionxpath|@ektdesignns_valuexpath|@ektdesignns_datanamespaces"/>
				</xsl:when>
				<xsl:when test="name()='select'">
					<xsl:apply-templates select="descendant::option" mode="dl"/>
				</xsl:when>
				<xsl:when test="name()='ektdesignns_choices' or name()='ektdesignns_checklist'">
					<xsl:for-each select="ol/li">
						<xsl:apply-templates select="input" mode="dl">
							<xsl:with-param name="display-value" select=".//label/node()"/>
						</xsl:apply-templates>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="descendant::option|descendant::input" mode="dl"/>
				</xsl:otherwise>
			</xsl:choose>
		</datalist>
	</xsl:if>
</xsl:template>


<!-- match fieldset/legend, tabular data (process fields inside fieldset) -->

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


<!-- match input type=text|hidden|password -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden' or @type='password') and @ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="@type='password'">
			<xsl:call-template name="processElement">
				<xsl:with-param name="xpath" select="$xpath"/>
				<xsl:with-param name="knowntype" select="'password'"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="processElement">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- match textarea -->

<xsl:template match="textarea[@ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="processElement">
		<xsl:with-param name="xpath" select="$xpath"/>
		<xsl:with-param name="knowntype" select="'textarea'"/>
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

<!-- item nodetype ====================================================================== -->

<!-- match input type=checkbox|radio -->

<xsl:template match="input[(@type='checkbox' or @type='radio') and @ektdesignns_nodetype='item']" mode="dl">
	<xsl:variable name="li" select="ancestor::li[1]"/>
	<xsl:variable name="label" select="$li//label[@for=current()/@id][1]"/>
	<item value="{@value}">
		<xsl:choose>
			<xsl:when test="$label">
				<xsl:value-of select="$label"/>
				<!--<xsl:call-template name="copyData">
					<xsl:with-param name="data" select="$label/node()"/>
				</xsl:call-template>-->
			</xsl:when>
			<xsl:when test="string-length($li) &gt; 0">
				<xsl:value-of select="$li"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@value"/>
			</xsl:otherwise>
		</xsl:choose>
	</item>
</xsl:template>


<!-- templates ========================================================================== -->

<xsl:include href="template_copyData.xslt"/>
<xsl:include href="template_getDesignName.xslt"/>
<xsl:include href="template_buildXPath.xslt"/>
<xsl:include href="template_datalist.xslt"/>

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