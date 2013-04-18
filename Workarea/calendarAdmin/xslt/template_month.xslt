<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="MonthHeader">
	<xsl:choose>
		<xsl:when test="number(@month_number) = 1">
			<xsl:call-template name="MoHeader">
				<xsl:with-param name="NextMo" select="2"/>
				<xsl:with-param name="PrevMo" select="12"/>
				<xsl:with-param name="NextYear" select="number(../@year)"/>
				<xsl:with-param name="PrevYear" select="number(../@year)-1"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:when test="number(@month_number) = 12">
			<xsl:call-template name="MoHeader">
				<xsl:with-param name="NextMo" select="1"/>
				<xsl:with-param name="PrevMo" select="11"/>
				<xsl:with-param name="NextYear" select="number(../@year)+1"/>
				<xsl:with-param name="PrevYear" select="number(../@year)"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="MoHeader">
				<xsl:with-param name="NextMo" select="number(@month_number)+1"/>
				<xsl:with-param name="PrevMo" select="number(@month_number)-1"/>
				<xsl:with-param name="NextYear" select="number(../@year)"/>
				<xsl:with-param name="PrevYear" select="number(../@year)"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="DOWHeaderCell">
	<xsl:variable name="className">
		<xsl:choose>
			<xsl:when test="@downum=0 or @downum=6">mup_weekendDOWHeader</xsl:when>
			<xsl:otherwise>mup_weekdayDOWHeader</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<td class="{$className}" width="14%"><xsl:value-of select="dowabbrev"/></td>
</xsl:template>

<xsl:template name="loop">
	<xsl:param name="repeat">0</xsl:param>
	<xsl:if test="number($repeat) &gt;= 1">
		<td class="mup_nonMonthCell">&#x20;<br/></td>
		<xsl:call-template name="loop">
			<xsl:with-param name="repeat" select="$repeat - 1"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>