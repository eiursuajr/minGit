<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>
	<xsl:variable name="AppImagePath" select="/root/appImagePath" />

	<xsl:template match="/root">
		<xsl:value-of select="/root/resourceStrings/selectYourCurrency"/><br/>
		<select>
			<xsl:attribute name="onchange">ekCurrencyChange_<xsl:value-of select="controlId"/>(this);</xsl:attribute>
			<xsl:attribute name="id">ekcurrencysel_<xsl:value-of select="controlId"/></xsl:attribute>
			<xsl:attribute name="name">ekcurrencysel_<xsl:value-of select="controlId"/></xsl:attribute>
			<xsl:apply-templates select="/root/currencyList/currency" />
		</select>
	</xsl:template>

	<!-- Helper template; List each order item -->
	<xsl:template match="currency">
		<option>
			<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
			<xsl:attribute name="value"><xsl:value-of select="Id"/></xsl:attribute><xsl:value-of select="Name"/>&#160;<xsl:value-of select="AlphaIsoCode"/>
		</option>
	</xsl:template>

</xsl:stylesheet>