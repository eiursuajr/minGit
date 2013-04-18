<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" />

<xsl:param name="LangType" select="''"/>
<xsl:param name="localeUrl" select="concat('resourcexml.aspx?name=CompatibilityReport&amp;LangType=',$LangType)"/>

<xsl:variable name="localeXml" select="document($localeUrl)/*" />

<xsl:param name="sField" select="$localeXml/data[@name='sField']/value/text()"/> <!--'Field'-->
<xsl:param name="sXPath" select="$localeXml/data[@name='sXPath']/value/text()"/> <!--'XPath'-->
<xsl:param name="sIncompatibleMin" select="$localeXml/data[@name='sIncompatibleMin']/value/text()"/> <!--'The minimum number of fields is higher than it used to be.'-->
<xsl:param name="sIncompatibleMax" select="$localeXml/data[@name='sIncompatibleMax']/value/text()"/> <!--'The maximum number of fields is lower than it used to be.'-->
<xsl:param name="sIncompatibleDatatype" select="$localeXml/data[@name='sIncompatibleDatatype']/value/text()"/> <!--'The data type or validation has changed and MAY be incompatible.'-->
<xsl:param name="sIncompatibleDatalist" select="$localeXml/data[@name='sIncompatibleDatalist']/value/text()"/> <!--'At least one item has been removed from the list of possible values.'-->
<xsl:param name="sMissingXPath" select="$localeXml/data[@name='sMissingXPath']/value/text()"/> <!--'The field has been removed or the structure of the content has changed.'-->
<xsl:param name="sOld" select="$localeXml/data[@name='sOld']/value/text()"/> <!--'Old value'-->
<xsl:param name="sNew" select="$localeXml/data[@name='sNew']/value/text()"/> <!--'New value'-->

<!--

Run on file with initial field list and current field list combined as:

	<root>
		<fieldlist>
			initial field list ($old)
		</fieldlist>
		<fieldlist>
			current field list ($new)
		</fieldlist>
	</root>

Compatible if for each field:

	xpath still exists
	minoccurs >= new value
	maxoccurs <= new value
	datatype is same
	datalist values still exist

-->

<xsl:variable name="EOM" select="'&#13;&#10;&#13;&#10;&#13;&#10;'"/> <!-- 3 new lines indicates separation b/n error messages -->

<xsl:template match="/">
    <xsl:variable name="old" select="*/fieldlist[1]"/>
    <xsl:variable name="new" select="*/fieldlist[2]"/>

    <xsl:for-each select="$old/field">
        <xsl:variable name="same" select="$new/field[@xpath=current()/@xpath]"/>
        <xsl:choose>
            <xsl:when test="$same">
                <xsl:variable name="oldMin">
                    <xsl:choose>
                        <xsl:when test="@minoccurs">
                            <xsl:value-of select="@minoccurs"/>
                        </xsl:when>
                        <xsl:otherwise>1</xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>
                <xsl:variable name="oldMax">
                    <xsl:choose>
                        <xsl:when test="@maxoccurs">
                            <xsl:value-of select="@maxoccurs"/>
                        </xsl:when>
                        <xsl:otherwise>1</xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>
                <xsl:variable name="newMin">
                    <xsl:choose>
                        <xsl:when test="$same/@minoccurs">
                            <xsl:value-of select="$same/@minoccurs"/>
                        </xsl:when>
                        <xsl:otherwise>1</xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>
                <xsl:variable name="newMax">
                    <xsl:choose>
                        <xsl:when test="$same/@maxoccurs">
                            <xsl:value-of select="$same/@maxoccurs"/>
                        </xsl:when>
                        <xsl:otherwise>1</xsl:otherwise>
                    </xsl:choose>
                </xsl:variable>
                <xsl:if test="number($oldMin) &lt; number($newMin)">
					<xsl:call-template name="getErrorMessage">
						<xsl:with-param name="description" select="$sIncompatibleMin"/>
						<xsl:with-param name="oldValue" select="$oldMin"/>
						<xsl:with-param name="newValue" select="$newMin"/>
					</xsl:call-template>
					<xsl:value-of select="$EOM"/>
                </xsl:if>
                <xsl:if test="number($oldMax) &gt; number($newMax) or ($oldMax='unbounded' and $newMax!='unbounded')">
					<xsl:call-template name="getErrorMessage">
						<xsl:with-param name="description" select="$sIncompatibleMax"/>
						<xsl:with-param name="oldValue" select="$oldMax"/>
						<xsl:with-param name="newValue" select="$newMax"/>
					</xsl:call-template>
					<xsl:value-of select="$EOM"/>
                </xsl:if>
                <xsl:if test="@datatype!=$same/@datatype">
					<xsl:call-template name="getErrorMessage">
						<xsl:with-param name="description" select="$sIncompatibleDatatype"/>
						<xsl:with-param name="oldValue" select="@datatype"/>
						<xsl:with-param name="newValue" select="$same/@datatype"/>
					</xsl:call-template>
					<xsl:value-of select="$EOM"/>
                </xsl:if>
                <xsl:if test="@datalist">
                    <xsl:variable name="dl" select="$new/datalist[@name=$same/@datalist]"/>
					<xsl:variable name="items" select="$old/datalist[@name=current()/@datalist]/item[not(@value=$dl/item/@value)]"/>
					<xsl:if test="count($items) &gt; 0">
						<xsl:call-template name="getErrorMessage">
							<xsl:with-param name="description" select="$sIncompatibleDatalist"/>
						</xsl:call-template>
						<xsl:value-of select="$EOM"/>
					</xsl:if>
                </xsl:if>
            </xsl:when>
            <xsl:otherwise>
				<xsl:call-template name="getErrorMessage">
					<xsl:with-param name="description" select="$sMissingXPath"/>
				</xsl:call-template>
				<!-- xsl:variable name="similar" select="$new/field[contains(@xpath,current()/@name)]"/ -->
				<xsl:value-of select="$EOM"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:for-each>
</xsl:template>

<xsl:template name="getErrorMessage">
	<xsl:param name="field" select="."/>
	<xsl:param name="description"/>
	<xsl:param name="oldValue"/>
	<xsl:param name="newValue"/>
	<xsl:value-of select="concat($description,'&#13;&#10;')"/>
	<xsl:if test="$oldValue and $newValue">
		<xsl:value-of select="concat($sOld,': &quot;',$oldValue,'&quot; ',$sNew,': &quot;',$newValue,'&quot;&#13;&#10;')"/>
	</xsl:if>
	<xsl:value-of select="concat($sField,': &quot;',$field,'&quot; ',$sXPath,': ',$field/@xpath)"/>
</xsl:template>

</xsl:stylesheet> 
