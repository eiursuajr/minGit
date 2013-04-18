<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html"/>

<xsl:include href="template_month.xslt"/>

<xsl:template match="ektroncalendars" xml:space="preserve">
	<link rel="stylesheet" type="text/css" href="{ektroncalendar[1]/requestinfo/applicationpath}csslib/calendarStyles.css"/>
	<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/calendarDisplayFuncs.js"></script>

	<script language="JavaScript" type="text/javascript">
	<xsl:comment>
	function showDaysEvents(month, day, year) {
		if(document.getElementById('day_' + month + '_'+ day +'_'+ year)) {
			document.getElementById('eventplaceholder').className = '';
			document.getElementById('eventplaceholder').innerHTML = 
				document.getElementById('day_' + month + '_'+ day +'_'+ year).innerHTML;
		}
	}
	function hideDaysEvents(month, day, year) {
		document.getElementById('eventplaceholder').innerHTML = ' ';
	}
	// </xsl:comment>
	</script>
	<xsl:apply-templates select="ektroncalendar"/>
</xsl:template>

<xsl:template match="ektroncalendar" xml:space="preserve">
<table cellpadding="0" cellspacing="0" border="0" width="100%" height="100%">
	<tr>
		<td width="30%"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br/></td>
		<td width="70%"><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br/></td>
	</tr>
	<tr style="height:100%">
		<td valign="top"><!-- Begin Month Calendar Cell -->
			<table cellpadding="3" cellspacing="0" width="100%">
				<tr>
					<td class="mup_monthHeaderBorder" colspan="{5+2*number(number(showweekend) &gt; 0)}">
					<table cellpadding="0" cellspacing="0" border="0" width="100%">
					<tr>
					<xsl:for-each select="calendaryear[1]/calendarmonth[1]"> <!-- set context node -->
						<xsl:call-template name="MonthHeader"/>
					</xsl:for-each>
					</tr>
					</table>
					</td>
				</tr>
				<tr>
				<xsl:for-each select="weekmeta/dowmeta">
					<xsl:choose>
						<xsl:when test="number(../../showweekend)=0">
							<xsl:if test="(@downum &gt; 0) and (@downum &lt; 6)">
								<xsl:call-template name="DOWHeaderCell"/>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="DOWHeaderCell"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
				</tr>
				<xsl:for-each select="calendaryear[1]/calendarmonth[1]/calendarweek">
				<tr>
				<xsl:choose>
					<xsl:when test="number(../../../showweekend)=0">
						<xsl:if test="(number(calendarday[1]/@weekday_number) &gt; 0) and (number(calendarday[1]/@weekday_number) &lt; 6)">
							<xsl:call-template name="loop">
								<xsl:with-param name="repeat" select="(number(calendarday[1]/@weekday_number)-1)"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="loop">
							<xsl:with-param name="repeat" select="(number(calendarday[1]/@weekday_number)-number(../../../weekmeta/@firstdayofweek)+7) mod 7"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>

				<xsl:apply-templates select="calendarday"/>
				
				<xsl:choose>
					<xsl:when test="number(../../../showweekend)=0">
						<xsl:call-template name="loop">
							<xsl:with-param name="repeat" select="(5 - number(calendarday[last()]/@weekday_number)) mod 5"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="loop">
							<xsl:with-param name="repeat" select="(number(../../../weekmeta/@firstdayofweek+6) - number(calendarday[last()]/@weekday_number)+7) mod 7"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
				</tr>
			</xsl:for-each>
				<tr>
					<td class="mup_yearFooter" colspan="{5+2*number(number(showweekend) &gt; 0)}">
<a href="#" onclick="return(loadUrlWithDate('monthupwithevent',{number(calendaryear[1]/calendarmonth[1]/@month_number)},1,{number(calendaryear[1]/@year)-1},'{servershortdateformat}'));">
<xsl:value-of select="number(calendaryear[1]/@year)-1"/></a>
						<xsl:text>..</xsl:text>
						<xsl:value-of select="number(calendaryear[1]/@year)"/>
						<xsl:text>..</xsl:text>
<a href="#" onclick="return(loadUrlWithDate('monthupwithevent',{number(calendaryear[1]/calendarmonth[1]/@month_number)},1,{number(calendaryear[1]/@year)+1},'{servershortdateformat}'));">
<xsl:value-of select="number(calendaryear[1]/@year)+1"/></a>
					</td>
				</tr>
			</table>
		<!-- End Month Calendar Cell -->
		</td>
		<td class="mup_eventDispBkg" valign="top">
			<div id="eventplaceholder" class="mup_eventDisplay"> <br /></div>
		</td>
	</tr>
</table>
<script language="JavaScript" type="text/javascript">
<xsl:comment>
	showDaysEvents('<xsl:value-of select="caldatacreationdateserver/@month"/>','<xsl:value-of select="caldatacreationdateserver/@day"/>','<xsl:value-of select="caldatacreationdateserver/@year"/>');
// </xsl:comment>
</script>
</xsl:template>

<xsl:template match="calendarday">
	<xsl:if test="(number(../../../../showweekend)=1) or ((number(../../../../showweekend)=0) and (@weekday_number!=0) and (@weekday_number!=6))">
		<xsl:choose>
			<xsl:when test="event">
				<xsl:choose>
				<xsl:when test="number(../../showweekend)=0">
					<xsl:if test="(@downum &gt; 0) and (@downum &lt; 6)">
						<td class="mup_weekdayEventCell" onclick="showDaysEvents('{../../@month_number}','{@dayofmonth}','{../../../@year}');">
						<xsl:value-of select="@dayofmonth"/>
						<xsl:call-template name="eventdisplay"/>
						</td>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<td class="mup_weekdayEventCell" onclick="showDaysEvents('{../../@month_number}','{@dayofmonth}','{../../../@year}');">
					<xsl:value-of select="@dayofmonth"/>
					<xsl:call-template name="eventdisplay"/>
					</td>								
				</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="emptyday"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>
</xsl:template>				

<xsl:template name="eventdisplay">
	<table cellpadding="0" cellspacing="0" width="100%" border="0" class="mup_eventDispBkg">

	</table>
	<div class="mup_hiddenEvent" id="day_{../../@month_number}_{@dayofmonth}_{../../../@year}">
	<table cellpadding="0" cellspacing="0" width="100%" border="0" class="mup_eventDisplay ">
		<tr>
			<td class="mup_eventDisplayDate"><xsl:value-of select="@shortdate"/></td>
		</tr>
		<xsl:apply-templates select="event"/>
	</table>
	</div>
</xsl:template>

<xsl:template match="event">
	<tr>
		<xsl:choose>
			<xsl:when test="string-length(qlink) &gt; 1">
				<xsl:choose>
					<xsl:when test="contains(launchnewbrowser,'True')">
						<td class="mup_eventDisplayEventTitle">
							<a href="#" onclick="window.open('{qlink}','eventwin');return false;"><xsl:value-of select="eventtitle"/></a>
						</td>
					</xsl:when>
					<xsl:otherwise>
						<td class="mup_eventDisplayEventTitle">
						<a href="{qlink}"><xsl:value-of select="eventtitle"/></a></td>
					</xsl:otherwise>				
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<td class="mup_eventDisplayEventTitle"><xsl:value-of select="eventtitle"/></td>
			</xsl:otherwise>
		</xsl:choose>
	</tr>
	<tr>
		<td class="mup_eventDisplayLocation"><xsl:value-of select="locationlabel"/> <xsl:value-of select="eventlocation"/></td>
	</tr>
	<xsl:if test="contains(showstarttime,'True')">
	<tr>
		<td class="mup_eventDisplayTimes"><xsl:value-of select="startlabel"/> <xsl:value-of select="displaystarttime"/></td>
	</tr>
	</xsl:if>
	<xsl:if test="contains(showendtime,'True')">
	<tr>
		<td class="mup_eventDisplayTimes"><xsl:value-of select="endlabel"/> <xsl:value-of select="displayendtime"/></td>
	</tr>
	</xsl:if>
	<xsl:if test="longdescription">
	<tr>
		<td class="mup_eventDisplayLongDesc"><br /><xsl:copy-of select="longdescription"/>
		</td>
	</tr>
	</xsl:if>
	<tr>
		<td> <br/></td>
	</tr>
</xsl:template>

<xsl:template name="MoHeader">
	<xsl:param name="NextMo">0</xsl:param>
	<xsl:param name="PrevMo">0</xsl:param>
	<xsl:param name="NextYear">0</xsl:param>
	<xsl:param name="PrevYear">0</xsl:param>

	<td class="mup_monthHeaderPrev">
	<a href="#" onclick="return(loadUrlWithDate('monthupwithevent','{$PrevMo}','1','{$PrevYear}','{../../servershortdateformat}'))">
	&lt;&lt; <xsl:value-of select="../../monthmeta/mometa[@monum=number($PrevMo)]/moabbrev"/></a></td>
	<td class="mup_monthHeader"><xsl:value-of select="../../calstartdatemonth"/></td>
	<td class="mup_monthHeaderNext">
	<a href="#" onclick="return(loadUrlWithDate('monthupwithevent','{$NextMo}','1','{$NextYear}','{../../servershortdateformat}'))">
	<xsl:value-of select="../../monthmeta/mometa[@monum=number($NextMo)]/moabbrev"/> &gt;&gt;</a></td>
</xsl:template>

<xsl:template name="emptyday">
	<xsl:choose>
		<xsl:when test="(@weekday_number=0) or (@weekday_number=6)">
			<xsl:if test="number(../../../../showweekend) != 0">
				<xsl:call-template name="weekendcell"/>
			</xsl:if>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="weekdaycell"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="weekdaycell">
	<td class="mup_weekdayCell"><xsl:call-template name="eventdisplay"/><xsl:value-of select="@dayofmonth"/>
		<div class="mup_hiddenEvent">
		<xsl:attribute name="name">day_<xsl:value-of select="../../@month_number"/>_<xsl:value-of select="@dayofmonth"/>_<xsl:value-of select="../../../@year"/></xsl:attribute><xsl:attribute name="id">day_<xsl:value-of select="../../@month_number"/>_<xsl:value-of select="@dayofmonth"/>_<xsl:value-of select="../../../@year"/></xsl:attribute>
		</div>
	</td>
</xsl:template>

<xsl:template name="weekendcell">
	<td class="mup_weekendCell"><xsl:value-of select="@dayofmonth"/></td>
</xsl:template>

</xsl:stylesheet>