<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html"/>

<xsl:include href="template_month.xslt"/>

<xsl:template match="ektroncalendars" xml:space="preserve">
	
	<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/calendarDisplayFuncs.js"></script>
	<script language="JavaScript1.2" type="text/javascript">
	<xsl:comment>
	function showEventDetail(id) {
		document.getElementById('ev'+id).style.visibility="visible";
	}
	function hideEventDetail(id) {
		document.getElementById('ev'+id).style.visibility="hidden";
	}
	// </xsl:comment>
	</script>
	<xsl:apply-templates select="ektroncalendar"/>
</xsl:template>

<xsl:template match="ektroncalendar" xml:space="preserve">
	<xsl:for-each select="calendaryear">
		<xsl:for-each select="calendarmonth">
			<table cellpadding="0" cellspacing="0" border="0" width="100%">
				<xsl:if test="((calendarweek/calendarday[number(@weekday_number)=5]/@dayofmonth) &gt; 0) or (number(../../showweekend) != 0) or (number(calendarweek/@weekofmonth) != 1)">
					<xsl:call-template name="MonthHeader"/>
					<xsl:call-template name="WeekHeader"/>
				</xsl:if>
				<xsl:for-each select="calendarweek">
					<xsl:call-template name="calWeekTemplate"/>
				</xsl:for-each>

			</table>
		</xsl:for-each>	
	</xsl:for-each>

	<xsl:if test="eventtypes/@display='1'">
		<xsl:call-template name="EventTypeControl"/>
	</xsl:if>
</xsl:template>

<xsl:template name="EventTypeControl">
	<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td class="etCtrl_Background">
			<table cellpadding="6" cellspacing="0" border="0" width="100%">
				<tr>
					<td class="etCtrl_InstructCell" width="50%"><xsl:value-of select="evtypeinstructions"/>
					</td>
					<td class="etCtrl_SelCell" width="50%">
						<select class="etCtrl_selectBox" onchange="showEventTypeSel(this);" size="6" multiple="true">
							<option value="0"><xsl:value-of select="evtypeshowalllabel"/></option>
						<xsl:for-each select="eventtypes/eventtype">
							<option value="{@id}"><xsl:value-of select="evtext"/></option>
						</xsl:for-each>	
						</select><br/>
					</td>
				</tr>
			</table>
		</td>
	</tr>		
	</table>
</xsl:template>

<xsl:template name="calWeekTemplate">
	<xsl:if test="((calendarday[number(@weekday_number)=5]/@dayofmonth) &gt; 0) or (number(../../../showweekend) != 0) or (number(@weekofmonth) != 1)">
		<tr style="height:100px">
			<xsl:call-template name="WeekBegin">
				<xsl:with-param name="ShowWeekends" select="number(../../../showweekend)"/>
			</xsl:call-template>
			
			<xsl:call-template name="WeekMid"/>
	
			<xsl:call-template name="WeekEnd">
				<xsl:with-param name="ShowWeekends" select="number(../../../showweekend)"/>
			</xsl:call-template>
		</tr>
	</xsl:if>
</xsl:template>


<xsl:template name="WeekBegin">
	<xsl:param name="ShowWeekends">1</xsl:param>

	<xsl:choose>
		<xsl:when test="$ShowWeekends=1">
			<xsl:call-template name="loopMonthCell">
				<xsl:with-param name="repeat" select="(number(calendarday[1]/@weekday_number)-number(../../../weekmeta/@firstdayofweek)+7) mod 7"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="loopMonthCell">
				<xsl:with-param name="repeat" select="(number(calendarday[1]/@weekday_number) - 1) mod 5"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="WeekMid">
	<xsl:choose>
		<xsl:when test="number(../../../showweekend)=0">
			<xsl:for-each select="calendarday">
				<xsl:if test="(number(@weekday_number) &gt; 0) and (number(@weekday_number) &lt; 6)">
					<xsl:apply-templates select="."/>
				</xsl:if>
			</xsl:for-each>
		</xsl:when>
		<xsl:otherwise>
			<xsl:for-each select="calendarday">
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="WeekEnd">
	<xsl:param name="ShowWeekends">1</xsl:param>
	<xsl:choose>
		<xsl:when test="$ShowWeekends=1">
			<xsl:call-template name="loopMonthCell">
				<xsl:with-param name="repeat" select="(number(../../../weekmeta/@firstdayofweek+6) - number(calendarday[last()]/@weekday_number)+7) mod 7"/>
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="loopMonthCell">
				<xsl:with-param name="repeat" select="(5 - number(calendarday[last()]/@weekday_number)) mod 5"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="loopMonthCell">
	<xsl:param name="repeat">0</xsl:param>
	<xsl:if test="number($repeat) >= 1">
	<td width="14%" class="mv_NonMonthBorder" valign="top">
		<table cellpadding="2" cellspacing="0" border="0" width="100%">
			<tr>
				<td width="75%" class="mv_DateNonMonthDayCell"> </td>
				<td width="25%" align="center" class="mv_DateNonMonthDayCell"> </td>
			</tr>
			<tr>
				<td colspan="2" class="mv_NonMonthCell"> </td>
			</tr>
		</table>
	</td>
		<xsl:call-template name="loopMonthCell">
			<xsl:with-param name="repeat" select="$repeat - 1"/>
		</xsl:call-template>
	</xsl:if>

</xsl:template>

<xsl:template name="eventdisplay">
	<table cellpadding="0" cellspacing="0" border="0" class="mv_eventPopup">
	<tr>
		<td>
			<table cellpadding="3" cellspacing="0" border="0" class="mv_TodayCell">
			<xsl:choose>
				<xsl:when test="string-length(qlink) &gt; 1">
					<xsl:choose>
						<xsl:when test="contains(launchnewbrowser,'True')">
			<tr>
				<td nowrap="true"><a href="#" onclick="window.open('{qlink}','eventwin');return false;"><xsl:value-of select="eventtitle"/></a></td>
			</tr>
						</xsl:when>
						<xsl:otherwise>
			<tr>
				<td nowrap="true"><a href="{qlink}"><xsl:value-of select="eventtitle"/></a></td>
			</tr>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
			<tr>
				<td nowrap="true"><xsl:value-of select="eventtitle"/></td>
			</tr>
				</xsl:otherwise>				
			</xsl:choose>
			<tr>
				<td nowrap="true"><xsl:value-of select="locationlabel"/> <xsl:value-of select="eventlocation"/></td>
			</tr>
			<xsl:if test="contains(showstarttime,'True')">
			<tr>
				<td nowrap="true"><xsl:value-of select="startlabel"/> <xsl:value-of select="displaystarttime"/></td>
			</tr>
			</xsl:if>
			<xsl:if test="contains(showendtime,'True')">
			<tr>
				<td nowrap="true"><xsl:value-of select="endlabel"/> <xsl:value-of select="displayendtime"/></td>
			</tr>
			</xsl:if>
			</table>
		</td>
	</tr>
	</table>
</xsl:template>

<xsl:template name="MoHeader">
	<xsl:param name="NextMo">0</xsl:param>
	<xsl:param name="PrevMo">0</xsl:param>
	<xsl:param name="NextYear">0</xsl:param>
	<xsl:param name="PrevYear">0</xsl:param>

	<tr>
		<td class="mv_MonthHeaderBkg" colspan="{5+2*number(number(../../showweekend) &gt; 0)}">
			<table cellpadding="3" cellspacing="0" border="0" width="100%">
			<tr>
				<td class="mv_MonthHeaderPrev" width="30%"><a href="#" onclick="return(loadUrlWithDate('month','{$PrevMo}','1','{$PrevYear}','{../../servershortdateformat}'))">
					<xsl:value-of select="../../monthmeta/mometa[@monum=number($PrevMo)]/motext"/></a></td>
				<td class="mv_MonthHeader" align="center" width="40%">
					<div class="mv_eventContainer" id="evDisplay"></div>
					<br clear="all"/>

					<xsl:value-of select="../../monthmeta/mometa[@monum=current()/@month_number]/motext"/>&#160;
					<xsl:value-of select="../@year"/><br/>
					
					<br clear="all"/>
				</td>
				<td class="mv_MonthHeaderNext"><a href="#" onclick="return(loadUrlWithDate('month','{$NextMo}','1','{$NextYear}','{../../servershortdateformat}'))">
					<xsl:value-of select="../../monthmeta/mometa[@monum=number($NextMo)]/motext"/> &gt;&gt;</a></td>
			</tr>
			</table>
		</td>
	</tr>
</xsl:template>

<xsl:template name="eventDiv">
	<div style="visibility:visible;" onmouseover="showEventDetail({@eventid})" onmouseout="hideEventDetail({@eventid})">
		<xsl:attribute name="id">evtype<xsl:for-each select="eventtypes/eventtype">_<xsl:value-of select="@id"/></xsl:for-each></xsl:attribute>
		<div class="mv_eventContainer" id="ev{@eventid}"><xsl:call-template name="eventdisplay"/></div>
		<a href="#" onmouseover="showEventDetail({@eventid})"><xsl:value-of select="eventtitle"/></a>
	</div>
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
					&#160;
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

<xsl:template name="WeekHeader">
	<tr>
	<xsl:for-each select="../../weekmeta/dowmeta">
		<xsl:choose>
			<xsl:when test="number(../../showweekend)=0">
				<xsl:if test="(@downum &gt; 0) and (@downum &lt; 6)">
					<td class="mv_weekdayDOWHeader"><xsl:value-of select="dowtext"/></td>
				</xsl:if>
			</xsl:when>
			<xsl:when test="(@downum &gt; 0) and (@downum &lt; 6)">
				<td class="mv_weekdayDOWHeader"><xsl:value-of select="dowtext"/></td>
			</xsl:when>
			<xsl:otherwise>
				<td class="mv_weekendDOWHeader"><xsl:value-of select="dowtext"/></td>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:for-each>
	</tr>
</xsl:template>

</xsl:stylesheet>