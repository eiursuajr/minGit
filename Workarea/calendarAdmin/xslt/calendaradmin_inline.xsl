<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="calendar_inline.xsl"/>

<xsl:output method="html"/>

<xsl:template match="/">
	<xsl:apply-imports/>
</xsl:template>
	
<xsl:template match="ektroncalendar" xml:space="preserve">
	<table cellpadding="2" cellspacing="0" border="0" width="100%">
	 <tr>
	  <td class="ILViewHeaderBkg">
		<table cellpadding="0" cellspacing="0" border="0" width="100%">
			<tr>
				<td width="50%" class="ILViewHeader">
					<img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
					<xsl:value-of select="calstartdatelong"/>
					<xsl:if test="calstartdate != calenddate">
					-&#160;
					<xsl:value-of select="calenddatelong"/><br/>
					</xsl:if>
					<img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
				</td>
				<td align="right" width="50%"><xsl:call-template name="calendarEditButtons"/></td>
			</tr>
		</table>
	  </td>
	 </tr>
	 <xsl:for-each select="calendaryear/calendarmonth">
	   <xsl:for-each select="calendarweek">
		 <xsl:for-each select="calendarday">
		 <xsl:if test="event">
	 <tr>
	  <td class="ILViewDate">
		  <xsl:value-of select="@shortdate"/><br/>
	  </td>
	 </tr>
	 	<xsl:variable name="buttons">
			<xsl:call-template name="eventEditButtons"/>
		</xsl:variable>
		 <xsl:for-each select="event">
			 <tr>
			  <td class="ILViewDayCell">
			  	<!-- CAUTION: copy-of can be used instead of copyData because the output is HTML.
					Note that copyData cannot be used because the $buttons does not resolve to a nodeset. -->
			  	<xsl:copy-of select="$buttons"/><br/>
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

<xsl:include href="template_admin_common.xslt"/>

</xsl:stylesheet>