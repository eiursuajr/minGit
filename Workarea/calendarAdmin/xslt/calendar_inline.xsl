<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>

	<xsl:include href="../../controls/template_copyData.xslt"/>
   
	<xsl:template match="ektroncalendars" xml:space="preserve">   
		<link rel="stylesheet" type="text/css" href="{ektroncalendar[1]/requestinfo/applicationpath}csslib/calendarStyles.css"/>	
		<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/calendarDisplayFuncs.js"></script>
		<xsl:apply-templates select="ektroncalendar"/>
	</xsl:template>

	<xsl:template match="ektroncalendar" xml:space="preserve">
	<table cellpadding="2" cellspacing="0" border="0" width="100%">
	 <tr>
	  <td class="ILViewHeaderBkg">
	   <img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
	   <xsl:value-of select="calstartdatelong"/>
	   <xsl:if test="calstartdate != calenddate">
	   -&#160;
	   <xsl:value-of select="calenddatelong"/><br/>
	   </xsl:if>
	   <img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
	  </td>
	 </tr>
	 <xsl:for-each select="calendaryear/calendarmonth">
	   <xsl:for-each select="calendarweek">
		 <xsl:for-each select="calendarday">
		 <xsl:if test="event">
	 <tr>
	  <td class="ILViewDate">
	  <xsl:value-of select="../../month_text"/> <xsl:value-of select="@shortdate"/><br/>
	  </td>
	 </tr>
		 <xsl:for-each select="event">
			 <tr>
			  <td class="ILViewDayCell">
				 <xsl:call-template name="eventdisplay"/>
			  </td>
			 </tr>
		 </xsl:for-each>
		 </xsl:if>
	   </xsl:for-each>
	  </xsl:for-each>
	 </xsl:for-each>
	 </table>
	</xsl:template>

	<xsl:template name="eventdisplay">
				<table cellpadding="0" cellspacing="0" border="0" class="ILViewEvent" width="100%">
				<xsl:choose>
					<xsl:when test="string-length(qlink) &gt; 1">
						<xsl:choose>
							<xsl:when test="contains(launchnewbrowser,'True')">
				<tr>
					<td><a href="javascript://" onclick="window.open('{qlink}','eventwin');"><xsl:value-of select="eventtitle"/></a></td>
				</tr>
							</xsl:when>
							<xsl:otherwise>
				<tr>
					<td><a href="{qlink}"><xsl:value-of select="eventtitle"/></a></td>
				</tr>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>
				<tr>
					<td><xsl:value-of select="eventtitle"/></td>
				</tr>
					</xsl:otherwise>				
				</xsl:choose>
				<tr>
					<td><xsl:value-of select="eventlocation"/></td>
				</tr>
				<xsl:if test="contains(showstarttime,'True')">
				<tr>
					<td><xsl:value-of select="startlabel"/><xsl:value-of select="displaystarttime"/></td>
				</tr>
				</xsl:if>
				<xsl:if test="contains(showendtime,'True')">
				<tr>
					<td><xsl:value-of select="endlabel"/><xsl:value-of select="displayendtime"/></td>
				</tr>
				</xsl:if>
				<xsl:if test="longdescription/node()">
				<tr>
					<td>
						<xsl:variable name="longDesc" select="longdescription/node()"/>
						<xsl:call-template name="copyData"><xsl:with-param name="data" select="$longDesc"/></xsl:call-template>
					</td>
				</tr>
				</xsl:if>
				</table>
	</xsl:template>
 
</xsl:stylesheet>