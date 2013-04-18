<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="calendar_monthupwithevent.xsl"/>

<xsl:output method="html"/>

<xsl:template match="/">
	<xsl:apply-imports/>
</xsl:template>

<xsl:template match="calendarday">				
	<xsl:choose>
		<xsl:when test="(@weekday_number=0) or (@weekday_number=6)">
			<xsl:if test="number(../../../../showweekend)!=0">
				<xsl:variable name="className">
					<xsl:choose>
						<xsl:when test="event">mup_weekenddayEventCell</xsl:when>
						<xsl:otherwise>mup_weekenddayLoggedInCell</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<td class="{$className}" onclick="showDaysEvents('{../../@month_number}','{@dayofmonth}','{../../../@year}');">
				<xsl:value-of select="@dayofmonth"/>
				<xsl:call-template name="eventdisplay"/>
				</td>
			</xsl:if>
		</xsl:when>
		<xsl:otherwise>
			<xsl:variable name="className">
				<xsl:choose>
					<xsl:when test="event">mup_weekdayEventCell</xsl:when>
					<xsl:otherwise>mup_weekdayLoggedInCell</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<td class="{$className}" onclick="showDaysEvents('{../../@month_number}','{@dayofmonth}','{../../../@year}');">
			<xsl:value-of select="@dayofmonth"/>
			<xsl:call-template name="eventdisplay"/>
			</td>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="eventdisplay">
	<div class="mup_hiddenEvent" id="day_{../../@month_number}_{@dayofmonth}_{../../../@year}">
	<table cellpadding="0" cellspacing="0" width="100%" border="0" class="mup_eventDisplay">
		<tr>
			<td class="mup_eventDisplayDate">
				<table cellpadding="0" cellspacing="0" border="0">
					<tr>
						<td nowrap="true" class="mup_eventDisplayDate" valign="middle">&#160;<xsl:call-template name="eventEditButtons"/>&#160;</td>
						<td class="mup_eventDisplayDate" width="99%"><xsl:value-of select="@shortdate"/></td>
					</tr>
				</table>
			</td>
		</tr>
		<xsl:apply-templates select="event"/>
	</table>
	</div>
</xsl:template>

<xsl:include href="template_admin_common.xslt"/>

</xsl:stylesheet>