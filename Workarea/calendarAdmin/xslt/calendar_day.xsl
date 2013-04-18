<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>

	<xsl:param name="DaytimeHourFirst">9</xsl:param>
	<xsl:param name="DaytimeHourLast">17</xsl:param>

	<xsl:template match="ektroncalendars" xml:space="preserve">
		<link rel="stylesheet" type="text/css" href="{ektroncalendar[1]/requestinfo/applicationpath}csslib/calendarStyles.css"/>
		<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/calendarDisplayFuncs.js"></script>
		<script language="JavaScript" type="text/javascript">
		<xsl:comment>
		function flipEventDescription(evid) {
			var d = document.getElementById("eventLong_" + evid);
			if(d.className == 'dv_LongDescriptionHidden') {
				d.className = 'dv_LongDescriptionVisible';
			} else {
				d.className = 'dv_LongDescriptionHidden';
			}
		}
		// </xsl:comment>
		</script>
		<xsl:apply-templates select="ektroncalendar"/>
	</xsl:template>

	<xsl:template match="ektroncalendar" xml:space="preserve">
<!-- Background Table -->
<table width="100%" cellpadding="0" cellspacing="0" border="0" class="dv_HourLayer">
	<tr>
		<td>
			<table width="100%" cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td colspan="7" class="dv_DayHeaderBkg" align="center">
						<table cellpadding="3" cellspacing="0" border="0" width="100%">
						<tr>
							<td class="dv_DayHeaderPrev"><a href="#" onclick="return(loadUrlWithSingleDate('day','{calpreviousdateserver}'))"> &lt;&lt; <xsl:value-of select="calpreviousdate"/></a></td>
							<td class="dv_DayHeader">
								<img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
								<a href="#" onclick="return(loadUrlWithDate('month',{calendaryear[1]/calendarmonth[1]/@month_number},1,{calendaryear[1]/@year},'{servershortdateformat}'))">
								<xsl:value-of select="calstartdatelong"/></a>
								<img src="images/blank.gif" width="1" height="3" border="0" alt=""/><br clear="all"/>
							</td>
							<td class="dv_DayHeaderNext"><a href="#" onclick="return(loadUrlWithSingleDate('day','{calnextdateserver}'))">
								<xsl:value-of select="calnextdate"/> &gt;&gt; </a></td>
						</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="dv_BorderCell" width="1"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="dv_BorderCell" width="2"><img src="images/blank.gif" width="2" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="dv_BorderCell" width="25%"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="dv_BorderCell" width="1"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="dv_BorderCell" width="2"><img src="images/blank.gif" width="2" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="dv_BorderCell" width="74%"><div id="Div_EventLayer" class="dv_EventLayer"></div><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="dv_BorderCell" width="1"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
				</tr>
				<xsl:for-each select="timemeta/hourmeta">
				<xsl:sort select="@hournum" data-type="number" order="ascending"/>
				<xsl:variable name="hourClass" xml:space="default">
					<xsl:choose>
						<xsl:when test="($DaytimeHourFirst &lt;= @hournum) and (@hournum &lt;= $DaytimeHourLast)">dv_HourCell</xsl:when>
						<xsl:otherwise>dv_HourCellEve</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<tr>
					<td class="dv_BorderCell"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="{$hourClass}"><img src="images/blank.gif" width="2" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="{$hourClass}"><xsl:value-of select="short"/><br/></td>
					<td class="dv_BorderCell"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="{$hourClass}"><img src="images/blank.gif" width="2" height="1" border="0" alt=""/><br clear="all" /></td>
					<td class="{$hourClass}"></td>
					<td class="dv_BorderCell"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
				</tr>
				<tr>
					<td class="dv_BorderCell" colspan="7"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" /></td>
				</tr>
				</xsl:for-each>
			</table>
		</td>
		<td>&#160;</td>
	</tr>
</table>

<div id="Div_HiddenEventLayer" class="dv_HiddenEventLayer">
	<img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" />
	<table cellpadding="0" cellspacing="0" border="0" >
		<tr>
			<td width="2"><img src="images/blank.gif" width="5" height="1" border="0" alt=""/><br clear="all" /></td>
			<td>
			<table cellpadding="0" cellspacing="0" border="0">
			<tr>
				<xsl:for-each select="calendaryear[1]/calendarmonth[1]/calendarweek[1]/calendarday[1]/event">
				<td valign="top">
					<xsl:call-template name="singleEvent"/>
				</td>
				<td>&#160;</td>
				</xsl:for-each>
			</tr>
			</table>
			</td>
			<td><img src="images/blank.gif" width="3" height="1" border="0" alt=""/><br clear="all" /></td>
		</tr>
	</table>
</div>

<script language="JavaScript" type="text/javascript">
<xsl:comment>
	document.getElementById('Div_EventLayer').innerHTML = document.getElementById('Div_HiddenEventLayer').innerHTML;
// </xsl:comment>
</script>

</xsl:template>

	<xsl:template name="singleEvent">
		<table cellpadding="0" cellspacing="0" border="0">
			<tr class="dv_Event">
				<td valign="top" class="dv_EventTransparent">
				<xsl:call-template name="loopToStartHour"/>
				</td>
			</tr>
			<tr>
				<td class="dv_EventTitle" nowrap="true" onclick="eventDisplay('{@eventid}','{../@servershortdate}')">
					<xsl:call-template name="EventDisplay"/>
				</td>
			</tr>
		</table>
	</xsl:template>
	
	<xsl:template name="EventDisplay">
		<!-- Display Event Title -->
		&#160;
		<xsl:choose>
			<xsl:when test="string-length(eventtitle) &lt; 20">
				<xsl:value-of select="eventtitle"/>&#160;<img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring(eventtitle,1,20)"/>&#160;[...]&#160;<img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" />
			</xsl:otherwise>
		</xsl:choose>		
		<xsl:call-template name="loopToEndHour" />

	</xsl:template>
	
	<xsl:template name="loopToEndHour">
		<xsl:param name="tSep" select="../../../../../timemeta/servertimeseperator"/>
		<xsl:param name="aDes" select="../../../../../timemeta/serveramdesignator"/>
		<xsl:param name="pDes" select="../../../../../timemeta/serverpmdesignator"/>
		<xsl:param name="stTime" select="substring-after(starttime,' ')"/>
		<xsl:param name="startHour" select="number(substring-before($stTime,$tSep))"/>
		<xsl:param name="startPM" select="contains($stTime,$pDes)"/>
		<xsl:param name="startAM" select="contains($stTime,$aDes)"/>

		<!-- First determine what meridian we're in (if we're not already on a 24HR format) and turn hours to 24HR format -->
		<xsl:choose>
			<xsl:when test="$startPM">
				<xsl:choose>
					<xsl:when test="number($startHour)=12">
						<xsl:call-template name="gotoEnd"><xsl:with-param name="startHour" select="12"/></xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="gotoEnd"><xsl:with-param name="startHour" select="number($startHour)+12"/></xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$startAM">
						<xsl:choose>
							<xsl:when test="number($startHour) &gt; 11">
								<xsl:call-template name="gotoEnd"><xsl:with-param name="startHour" select="number(0)"/></xsl:call-template>
							</xsl:when>
							<xsl:otherwise>
								<xsl:call-template name="gotoEnd"><xsl:with-param name="startHour" select="number($startHour)"/></xsl:call-template>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="gotoEnd"><xsl:with-param name="startHour" select="number($startHour)"/></xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="gotoEnd">
		<xsl:param name="startHour">0</xsl:param>
		<xsl:param name="tSep" select="../../../../../timemeta/servertimeseperator"/>
		<xsl:param name="aDes" select="../../../../../timemeta/serveramdesignator"/>
		<xsl:param name="pDes" select="../../../../../timemeta/serverpmdesignator"/>
		<xsl:param name="enTime" select="substring-after(endtime,' ')"/>
		<xsl:param name="endHour" select="number(substring-before($enTime,$tSep))"/>
		<xsl:param name="endPM" select="contains($enTime,$pDes)"/>
		<xsl:param name="endAM" select="contains($enTime,$aDes)"/>
		<xsl:choose>
			<xsl:when test="$endPM">
				<xsl:call-template name="loop">
					<xsl:with-param name="repeat" select="number($endHour) + 11 - number($startHour)"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$endAM">
						<xsl:choose>
							<xsl:when test="number($startHour) &gt; 11">
							</xsl:when>
							<xsl:otherwise>
								<xsl:if test="(number($endHour)-1-number($startHour)) &gt; 0">
									<xsl:call-template name="loop">
										<xsl:with-param name="repeat" select="number($endHour) - 1 - number($startHour)"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<xsl:otherwise>
							<xsl:if test="(number($endHour)-1-number($startHour)) &gt; 0">
								<xsl:call-template name="loop">
									<xsl:with-param name="repeat" select="number($endHour) - 1 - number($startHour)"/>
								</xsl:call-template>
							</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>

	<xsl:template name="loopToStartHour">
		<xsl:choose>
			<xsl:when test="contains(displaystarttime,'AM')"><xsl:comment>This is an AM Time, Loop straight, unless 12</xsl:comment>
				<xsl:choose>
				<xsl:when test="number(substring-before(displaystarttime,':')) &gt; 11">
					
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="loop"><xsl:with-param name="repeat" select="number(substring-before(displaystarttime,':'))"/></xsl:call-template>
				</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
				<xsl:when test="contains(displaystarttime,'PM')"><xsl:comment>This is a PM Time, add 12</xsl:comment>
					<xsl:choose>
						<xsl:when test="number(substring-before(displaystarttime,':')) = 12">
							<xsl:call-template name="loop"><xsl:with-param name="repeat" select="12"/></xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="loop"><xsl:with-param name="repeat" select="number(substring-before(displaystarttime,':')+12)"/></xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
					
				</xsl:when>
				<xsl:otherwise><xsl:comment>This is a 24HR Format</xsl:comment>
					<xsl:call-template name="loop"><xsl:with-param name="repeat" select="number(substring-before(displaystarttime,':'))"/></xsl:call-template>
				</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="loop">
		<xsl:param name="repeat">0</xsl:param>
		<xsl:if test="number($repeat) >= 1">
			&#160;<br/><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br clear="all" />
			<xsl:call-template name="loop">
			<xsl:with-param name="repeat" select="$repeat - 1"/>
			</xsl:call-template>
		</xsl:if>
	
	</xsl:template>
</xsl:stylesheet>