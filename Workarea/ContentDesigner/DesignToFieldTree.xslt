<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:import href="DesignToFieldList.xslt"/>

<xsl:param name="configUrl"/> <!-- required, to get treeImg from validation spec -->
<xsl:param name="LangType" select="''"/>
<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=ValidateSpec&amp;LangType=',$LangType)"/>
<xsl:param name="srcPath"/> <!-- required --> <!-- must end in '/' -->
<xsl:param name="skinPath" select="$srcPath"/> <!-- must end in '/' -->
<xsl:param name="excludeClass" select="'|ektdesignns_imageonly|ektdesignns_filelink|ektdesignns_richarea|ektdesignns_choices|ektdesignns_checklist|ektdesignns_resource|'" />

<xsl:param name="currentXPath"/>
<xsl:param name="currentDatatype"/>

<xsl:variable name="localeXml" select="document($localeUrl)/*" />

<xsl:param name="dlgOpt" select="$localeXml/data[@name='dlgOpt']/value/text()"/> <!--'Optional'-->
<xsl:param name="sOrMore" select="$localeXml/data[@name='sOrMore']/value/text()"/> <!--'or more'-->
<xsl:param name="sTo" select="$localeXml/data[@name='sTo']/value/text()"/> <!--'to'-->
<xsl:param name="sCurFld" select="$localeXml/data[@name='sCurFld']/value/text()"/> <!--'(current field)'-->
<xsl:param name="sCurrentDateDesc" select="$localeXml/data[@name='sCurrentDateDesc']/value/text()"/> <!--'Current date when the field is validated'-->

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<!--

Outputs XML suitable for telerik Treeview control

<Tree>
	<Node Text="string" Value="string" ToolTip="string" LongDesc="string" Image="URL">
		<Node Text="string" Value="string" EktDatatype="string" ToolTip="string" LongDesc="string" Image="URL"/>
	</Node>
</Tree>

-->

<xsl:variable name="datadesignfeature" select="document($configUrl)//datadesign[1]"/>

<xsl:template match="/">
	<Tree>
		<xsl:if test="$currentDatatype='date'">
			<xsl:variable name="image">
				<xsl:call-template name="getImage">
					<xsl:with-param name="icon">calendar</xsl:with-param>
				</xsl:call-template>
			</xsl:variable>
			<Node Value="$currentDate" EktDatatype="date" ToolTip="$currentDate" LongDesc="{$sCurrentDateDesc}" Text="$currentDate ({$sCurrentDateDesc})" Image="{$image}"/>
		</xsl:if>
		<xsl:apply-templates>
		    <xsl:with-param name="xpath" select="$rootXPath"/>
		</xsl:apply-templates>
	</Tree>
</xsl:template>

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

	<!-- exclude fields which are not supported for selection (yet): 
		image only, file link, richarea, checklist, choices -->
	<xsl:if test="not(contains($excludeClass,concat('|',@class,'|'))) and not(contains($excludeClass,concat('|',name(),'|')))">
		<xsl:variable name="datatype">
			<xsl:call-template name="getDatatype">
				<xsl:with-param name="knowntype" select="$knowntype"/>
			</xsl:call-template>
		</xsl:variable>

		<!-- 'date' types are allowed only if current is a 'date' -->
		<!-- XPath 1.0 does not have 'xor' -->
		<xsl:if test="not($currentDatatype) or ($currentDatatype!='date' and not($datatype='date')) or ($currentDatatype='date' and $datatype='date')">

			<xsl:variable name="basetype">
				<xsl:call-template name="getBasetype">
					<xsl:with-param name="datatype" select="$datatype"/>
				</xsl:call-template>
			</xsl:variable>
	
			<xsl:variable name="caption">
				<xsl:call-template name="getTreeNodeCaption">
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:call-template>
			</xsl:variable>

			<xsl:variable name="image">
				<xsl:call-template name="getImage">
					<xsl:with-param name="icon">
						<xsl:choose>
							<xsl:when test="$basetype='calendar'">calendar</xsl:when>
							<xsl:when test="$basetype='checkbox'">checkbox</xsl:when>
							<xsl:when test="$basetype='hyperlink'">hyperlink</xsl:when>
							<xsl:when test="$basetype='number'">number</xsl:when>
							<xsl:when test="$basetype='password'">password</xsl:when>
							<xsl:when test="$basetype='richarea'">richarea</xsl:when>
							<xsl:when test="$basetype='textbox'">textbox</xsl:when>
							<xsl:when test="(name()='input' and @type='hidden')">hidden</xsl:when>
							<xsl:when test="name()='select' or $datatype='selection'">droplist</xsl:when>
							<xsl:when test="@ektdesignns_calculate">calculation</xsl:when>
							<xsl:when test="@ektdesignns_validation='custom'">
								<xsl:variable name="datatypename" select="@ektdesignns_datatype"/>
								<xsl:variable name="treeImg" select="$datadesignfeature/validation/custom/selections[@name='datatype']/listchoice[@value=$datatypename]/@treeImg"/>
								<xsl:choose>
									<xsl:when test="$treeImg">
										<xsl:value-of select="$treeImg"/>
									</xsl:when>
									<xsl:otherwise>text</xsl:otherwise>
								</xsl:choose>
							</xsl:when>
							<xsl:when test="string-length(normalize-space(@ektdesignns_validation)) &gt; 0">
								<xsl:variable name="valname" select="@ektdesignns_validation"/>
								<xsl:variable name="treeImg" select="$datadesignfeature/validation/choice[@name=$valname]/@treeImg"/>
								<xsl:choose>
									<xsl:when test="$treeImg">
										<xsl:value-of select="$treeImg"/>
									</xsl:when>
									<xsl:when test="contains($valname,'-req')">
										<xsl:variable name="valname-req" select="substring-before(@ektdesignns_validation,'-req')"/>
										<xsl:variable name="treeImg-req" select="$datadesignfeature/validation/choice[@name=$valname-req]/@treeImg"/>
										<xsl:choose>
											<xsl:when test="$treeImg-req">
												<xsl:value-of select="$treeImg-req"/>
											</xsl:when>
											<xsl:otherwise>text</xsl:otherwise>
										</xsl:choose>
									</xsl:when>
									<xsl:otherwise>text</xsl:otherwise>
								</xsl:choose>
							</xsl:when>
							<xsl:otherwise>text</xsl:otherwise>
						</xsl:choose>
					</xsl:with-param>
				</xsl:call-template>
			</xsl:variable>

			<!-- EktDatatype MUST immediately follow Value for reg exp in applyToFieldNames (ekformfields.js) -->
			<Node Value="{$new-xpath}" EktDatatype="{$datatype}" ToolTip="{$new-xpath}" LongDesc="{$new-xpath}" Text="{$caption}" Image="{$image}"/>
		</xsl:if>
	</xsl:if>
</xsl:template>

<!-- match fieldset/legend and tabular data (process fields inside fieldset) -->

<!-- higher priority than template above '*[@...]' -->
<!-- was fieldset[@class='design_group'] -->
<xsl:template match="fieldset[@ektdesignns_nodetype='element']|table[@ektdesignns_nodetype='element']|tr[@ektdesignns_nodetype='element']|div[@class='ektdesignns_group']" priority="1">
	<xsl:param name="xpath"/>

	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>

	<xsl:variable name="caption">
		<xsl:call-template name="getTreeNodeCaption">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:call-template>
	</xsl:variable>

	<xsl:variable name="image">
		<xsl:call-template name="getImage">
			<xsl:with-param name="icon">
				<xsl:choose>
					<xsl:when test="name()='fieldset' or @class='ektdesignns_group'">fieldset</xsl:when>
					<xsl:when test="name()='table'">table</xsl:when>
					<xsl:when test="name()='tr'">tablerow</xsl:when>
					<xsl:otherwise></xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:variable>

	<Node Value="{$new-xpath}" ToolTip="{$new-xpath}" LongDesc="{$new-xpath}" Text="{$caption}" Image="{$image}">
		<xsl:apply-templates>
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
	</Node>
</xsl:template>

<xsl:template name="getTreeNodeCaption">
	<xsl:param name="xpath"/>

	<xsl:variable name="caption">
		<xsl:call-template name="getCaption"/>
	</xsl:variable>

	<xsl:variable name="displayName">
		<xsl:choose>
			<xsl:when test="$caption != @ektdesignns_name">
				<xsl:value-of select="concat(' (',$caption,')')"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="''"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="name">
		<xsl:choose>
			<xsl:when test="@ektdesignns_nodetype='attribute'">
				<xsl:value-of select="concat('@',@ektdesignns_name)"/>
			</xsl:when>
			<xsl:when test="@ektdesignns_nodetype='text'">
				<xsl:value-of select="'node()'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@ektdesignns_name"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="min">
		<xsl:choose>
			<xsl:when test="@ektdesignns_minoccurs">
				<xsl:value-of select="@ektdesignns_minoccurs"/>
			</xsl:when>
			<xsl:otherwise>1</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="max">
		<xsl:choose>
			<xsl:when test="@ektdesignns_maxoccurs">
				<xsl:value-of select="@ektdesignns_maxoccurs"/>
			</xsl:when>
			<xsl:otherwise>1</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="repeatability">
		<xsl:choose>
			<xsl:when test="$min='0' and $max='1'">
				<xsl:value-of select="concat(' (',$dlgOpt,')')"/>
			</xsl:when>
			<xsl:when test="$max='unbounded'">
				<xsl:value-of select="concat(' (',$min,' ',$sOrMore,')')"/>
			</xsl:when>
			<xsl:when test="$min != '1' or $max != '1'">
				<xsl:value-of select="concat(' (',$min,' ',$sTo,' ',$max,')')"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="''"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="iscurrent">
		<xsl:choose>
			<xsl:when test="$currentXPath=$xpath">
				<xsl:value-of select="concat(' ',$sCurFld)"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="''"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:value-of select="concat($name,$displayName,$repeatability,$iscurrent)"/>
</xsl:template>

<xsl:template name="getImage">
	<xsl:param name="icon"/>
	<xsl:value-of select="concat($skinPath,$icon,'.gif')"/>
</xsl:template>


</xsl:stylesheet>