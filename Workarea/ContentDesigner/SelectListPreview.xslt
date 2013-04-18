<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="msxsl" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
<xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes"/>

<!--
Input:
<root>
	<ektdesignns_mergelist ektdesignns_name="fieldname2" ektdesignns_datatype="selection" ektdesignns_content="type2" ektdesignns_bind="/root/group/element1" ektdesignns_datalist="uniqueIdn">
		<ul ektdesignns_list="true"><li><ektdesignns_mergefield/></li></ul>
	</ektdesignns_mergelist>
	<ektdesignpackage_list>
		<fieldlist>
			<field name="fieldname2" datatype="selection" datalist="uniqueId1" content="type2" xpath="/root/group/element1">display name1</field>
			<datalist name="uniqueIdn">
				<item value="1">displayValue1</item>
				:
				<item value="n">displayValuen</item>
			</datalist>
		</fieldlist>
	</ektdesignpackage_list>
</root>

Output:

		<ul>
			<li>displayValue1</li>
			<li>displayValue2</li>
			<li>displayValue3</li>
			<li>displayValue4</li>
		</ul>

-->

<xsl:template match="/">
	<xsl:apply-templates select="*/*"/>	
</xsl:template>

<!-- match ektdesignns_mergelist -->
<xsl:template match="ektdesignns_mergelist">
	<xsl:variable name="name" select="@ektdesignns_name"/>
	<xsl:variable name="field" select="/*/ektdesignpackage_list/fieldlist/field[@name=string($name)]"/>
	<xsl:variable name="datalist" select="/*/ektdesignpackage_list/fieldlist/datalist[@name=$field/@datalist]"/>
	<xsl:choose>
		<xsl:when test="descendant::*[@ektdesignns_list='true']">
			<xsl:apply-templates select="node()">
				<xsl:with-param name="dataItemPath" select="$datalist"/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="processItems">
				<xsl:with-param name="dataItemPath" select="$datalist"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- template -->
<xsl:template match="*[@ektdesignns_list='true']">
	<xsl:param name="dataItemPath"/>
	<xsl:copy>
		<xsl:copy-of select="@*[not(starts-with(name(),'ektdesignns_'))]"/>
		<xsl:call-template name="processItems">
			<xsl:with-param name="dataItemPath" select="$dataItemPath"/>
		</xsl:call-template>
	</xsl:copy>
</xsl:template>

<xsl:template name="processItems">
	<xsl:param name="dataItemPath"/>
	<xsl:variable name="current" select="."/>
	<xsl:for-each select="$dataItemPath/item[position()&lt;=4]">
		<xsl:apply-templates select="$current/node()">
			<xsl:with-param name="dataItem" select="."/>
		</xsl:apply-templates>
	</xsl:for-each>
</xsl:template>

<xsl:template match="ektdesignns_mergefield">
	<xsl:param name="dataItem"/>
	<xsl:copy-of select="$dataItem/node()"/>
</xsl:template>

<!-- identity template without namespace nodes -->
<xsl:template match="*">
	<xsl:param name="dataItem"/>
	<xsl:param name="dataItemPath" />
	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*|node()">
			<xsl:with-param name="dataItem" select="$dataItem"/>
			<xsl:with-param name="dataItemPath" select="$dataItemPath"/>
		</xsl:apply-templates>
	</xsl:element>
</xsl:template>

<xsl:template match="@*|text()|comment()|processing-instruction()">
	<xsl:param name="dataItem"/>
	<xsl:param name="dataItemPath"/>
	<xsl:copy>
		<xsl:apply-templates select="@*|node()">
			<xsl:with-param name="dataItem" select="$dataItem"/>
			<xsl:with-param name="dataItemPath" select="$dataItemPath"/>
		</xsl:apply-templates>
	</xsl:copy>
</xsl:template>


<!--ignore-->
<xsl:template match="ektdesignpackage_list"/>

</xsl:stylesheet>