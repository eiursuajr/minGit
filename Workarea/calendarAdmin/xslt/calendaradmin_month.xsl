<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="calendar_month.xsl"/>

<xsl:output method="html"/>

<xsl:template match="/">
	<xsl:apply-imports/>
</xsl:template>

<xsl:template match="calendarday">
	<xsl:variable name="bIsToday" select="normalize-space(@shortdate) = normalize-space(../../../../caldatacreationdate)"/>
	<xsl:variable name="bIsWeekday" select="(number(@weekday_number) &gt; 0) and (number(@weekday_number) &lt; 6)"/>
	<td width="14%" valign="top">
	<xsl:attribute name="class"><xsl:choose>
			<xsl:when test="$bIsToday=true()">mv_dayBorderToday</xsl:when>
			<xsl:when test="$bIsWeekday=true()">mv_dayBorder</xsl:when>
			<xsl:otherwise>mv_dayBorderWeekend</xsl:otherwise>
		</xsl:choose>
	</xsl:attribute>
	
		<table cellpadding="2" cellspacing="0" border="0" width="100%">
			<tr>
				<td>
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$bIsToday=true()">mv_DateCellEmptyToday</xsl:when>
							<xsl:when test="$bIsWeekday=true()">mv_DateCellEmpty</xsl:when>
							<xsl:otherwise>mv_DateCellEmptyWeekend</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					&#160;<xsl:call-template name="eventEditButtons"/>
				</td>
				<td width="25%" align="center">
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$bIsToday=true()">mv_DateCellToday</xsl:when>
							<xsl:when test="$bIsWeekday=true()">mv_DateCell</xsl:when>
							<xsl:otherwise>mv_DateCellWeekend</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<a href="#" onclick="return(loadUrlWithSingleDate('day','{@servershortdate}'))">
					<xsl:value-of select="@dayofmonth"/></a>
				</td>					
			</tr>
			<tr>

				<td colspan="2"><xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$bIsToday=true()">mv_DayCellToday</xsl:when>
							<xsl:when test="$bIsWeekday=true()">mv_DayCell</xsl:when>
							<xsl:otherwise>mv_DayCellWeekend</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<xsl:if test="event">
						<table cellpadding="2" cellspacing="0" border="0" width="100%">
						<xsl:for-each select="event">
						<tr>
						<td class="mv_eventTitle">
							<xsl:call-template name="eventDiv"/>
						</td>
						</tr>
						</xsl:for-each>
						</table>
					</xsl:if>
				</td>
			</tr>
		</table>
	</td>
</xsl:template>

<xsl:include href="template_admin_common.xslt"/>

</xsl:stylesheet>