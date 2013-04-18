<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="html"/>

<xsl:include href="../../controls/template_copyData.xslt"/>

<xsl:template match="ektroncalendars" xml:space="preserve">
	<link rel="stylesheet" type="text/css" href="{ektroncalendar[1]/requestinfo/applicationpath}csslib/calendarStyles.css"/>
	<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/calendarDisplayFuncs.js"></script>
	<xsl:apply-templates select="ektroncalendar"/>
</xsl:template>

<xsl:template match="ektroncalendar" xml:space="preserve">
	<xsl:variable name="event" select="//event[@eventid=/ektroncalendars/ektroncalendar/ekteventhighlight]"/>

<table cellpadding="2" cellspacing="0" border="0" width="100%">
 <tr>
  <td class="evt_TitleCell">
   <img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
   <xsl:value-of select="$event/eventtitle"/><br/>
   </td>
  </tr>
  <tr>
  <td class="evt_DateCell">
  <a href="#" onclick="return(loadUrlWithSingleDate('day','{$event/../@servershortdate}'))">
  <xsl:value-of select="/ektroncalendars/ektroncalendar/calstartdatelong"/></a><br/>
   <img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
  </td>
 </tr>
 <tr>
  <td class="evt_ViewDayCell">
	 <xsl:call-template name="eventdisplay"/>
  </td>
 </tr>
 </table>
   </xsl:template>

	<xsl:template name="eventdisplay">
		<xsl:variable name="event" select="//event[@eventid=/ektroncalendars/ektroncalendar/ekteventhighlight]"/>
		<table cellpadding="0" cellspacing="0" border="0" class="evt_ViewEvent" width="100%">
		<xsl:choose>
			<xsl:when test="string-length($event/qlink) &gt; 1">
				<xsl:choose>
					<xsl:when test="contains($event/launchnewbrowser,'True')">
		<tr>
			<td><a href="javascript://" onclick="window.open('{$event/qlink}','eventwin')"><xsl:value-of select="$event/eventtitle"/></a></td>
		</tr>
					</xsl:when>
					<xsl:otherwise>
		<tr>
			<td><a href="{$event/qlink}"><xsl:value-of select="$event/eventtitle"/></a></td>
		</tr>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise></xsl:otherwise>
		</xsl:choose>
		<xsl:if test="$event/eventlocation">
		<tr>
			<td><xsl:value-of select="/ektroncalendars/ektroncalendar/locationlabel"/><xsl:text> </xsl:text><xsl:value-of select="$event/eventlocation"/></td>
		</tr>
		</xsl:if>
		<xsl:if test="contains($event/showstarttime,'True')">
		<tr>
			<td><xsl:value-of select="/ektroncalendars/ektroncalendar/startlabel"/><xsl:text> </xsl:text><xsl:value-of select="$event/displaystarttime"/></td>
		</tr>
		</xsl:if>
		<xsl:if test="contains($event/showendtime,'True')">
		<tr>
			<td><xsl:value-of select="/ektroncalendars/ektroncalendar/endlabel"/><xsl:text> </xsl:text><xsl:value-of select="$event/displayendtime"/></td>
		</tr>
		</xsl:if>
		<xsl:variable name="longDesc" select="$event/longdescription/node()"/>
		<xsl:if test="$longDesc">
		<tr>
			<td><br/>
			<xsl:call-template name="copyData">
				<xsl:with-param name="data" select="$longDesc"/>
			</xsl:call-template>
			</td>
		</tr>
		</xsl:if>
		</table>
	</xsl:template>
</xsl:stylesheet>