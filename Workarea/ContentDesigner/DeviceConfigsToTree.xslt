<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:ektcmsdev="http://schemas.datacontract.org/2004/07/Ektron.Cms.Device"
xmlns:ektcms="http://schemas.datacontract.org/2004/07/Ektron.Cms"
exclude-result-prefixes="ektcmsdev ektcms msxsl">

<!-- Run on workarea/webservices/rest.svc/deviceconfigs.xml?LangType=1033 -->

<xsl:param name="LangType" select="''"/>
<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=ConditionalExamples&amp;LangType=',$LangType)"/>
<xsl:param name="srcPath"/> <!-- required --> <!-- must end in '/' -->
<xsl:param name="skinPath" select="$srcPath"/> <!-- must end in '/' -->

<xsl:variable name="localeXml" select="document($localeUrl)/*" />

<xsl:param name="sDevCfg" select="$localeXml/data[@name='sDevCfg']/value/text()"/> <!--'Device configuration'-->
<xsl:param name="sDevCfgs" select="$localeXml/data[@name='sDevCfgs']/value/text()"/> <!--'Device configurations'-->
<xsl:param name="sCurrentDateDesc" select="$localeXml/data[@name='sCurrentDateDesc']/value/text()"/> <!--'Date as yyyy-MM-dd when condition is evaluated'-->
<xsl:param name="sCurrentDateTimeDesc" select="$localeXml/data[@name='sCurrentDateTimeDesc']/value/text()"/> <!--'Date and time as yyyy-MM-ddThh:mm:ssZ when condition is evaluated'-->

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<!--

Outputs XML suitable for telerik Treeview control

<Tree>
	<Node Text="string" Value="string" ToolTip="string" LongDesc="string" Image="URL">
		<Node Text="string" Value="string" ToolTip="string" LongDesc="string" Image="URL"/>
	</Node>
</Tree>

-->

<xsl:template match="/">
	<Tree>
		<xsl:if test="ektcmsdev:ArrayOfCmsDeviceConfigurationData/ektcmsdev:CmsDeviceConfigurationData">
			<Node Value="$deviceConfiguration" ToolTip="$deviceConfiguration" LongDesc="{$sDevCfg}" Text="{$sDevCfgs} ($deviceConfiguration)">
				<xsl:apply-templates select="ektcmsdev:ArrayOfCmsDeviceConfigurationData/ektcmsdev:CmsDeviceConfigurationData">
					<xsl:sort select="Order" order="ascending" data-type="number"/>
				</xsl:apply-templates>
			</Node>
		</xsl:if>

		<xsl:variable name="image">
			<xsl:call-template name="getImage">
				<xsl:with-param name="icon">calendar</xsl:with-param>
			</xsl:call-template>
		</xsl:variable>
		<Node Value="$currentDate" EktDatatype="date" ToolTip="$currentDate" LongDesc="{$sCurrentDateDesc}" Text="$currentDate ({$sCurrentDateDesc})" Image="{$image}"/>
		<!--<Node Value="$currentDateTime" ToolTip="$currentDateTime" LongDesc="{$sCurrentDateTimeDesc}" Text="$currentDateTime ({$sCurrentDateTimeDesc})" Image="{$image}"/>-->
	</Tree>
</xsl:template>

<xsl:template match="ektcmsdev:CmsDeviceConfigurationData">
	<xsl:variable name="image">
		<xsl:call-template name="getImage">
			<xsl:with-param name="icon">phone</xsl:with-param>
		</xsl:call-template>
	</xsl:variable>
	<Node Value="'{ektcmsdev:Name}'" ToolTip="{$sDevCfg}" LongDesc="" Text="{ektcmsdev:Name} ({ektcms:Id})" Image="{$image}"/>
</xsl:template>

<xsl:template name="getImage">
	<xsl:param name="icon"/>
	<xsl:value-of select="concat($skinPath,$icon,'.gif')"/>
</xsl:template>

</xsl:stylesheet>