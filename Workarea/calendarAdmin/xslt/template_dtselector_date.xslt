<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="template_month.xslt"/>

<xsl:template name="dateSelection">
	<table cellpadding="6" cellspacing="0">
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

		<xsl:for-each select="calendarday">
			<xsl:call-template name="emptyday"/>
		</xsl:for-each>

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
<a href="#" onclick="return(loadUrlWithDate('dtselectordate',{number(calendaryear[1]/calendarmonth[1]/@month_number)},1,{number(calendaryear[1]/@year)-1},'{servershortdateformat}'));">
<xsl:text>&lt;&lt;&#160;</xsl:text><xsl:value-of select="number(calendaryear[1]/@year)-1"/></a>
				<xsl:text>&#160;&#160;</xsl:text>
				<xsl:variable name="iYear" select="number(calendaryear[1]/@year)"/>
				<select name="selectedYear" onchange="return(loadUrlWithDate('dtselectordate',{number(calendaryear[1]/calendarmonth[1]/@month_number)},1,document.forms[0].selectedYear.options[document.forms[0].selectedYear.selectedIndex].value,'{servershortdateformat}'));">
					<xsl:call-template name="rfn-year-for-loop">
						<xsl:with-param name="i" select="number(calendaryear[1]/@year)-50"/>
						<xsl:with-param name="increment" select="1"/>
						<xsl:with-param name="testValue" select="number(calendaryear[1]/@year)+50"/>
						<xsl:with-param name="defaultSelected" select="$iYear"/>
					</xsl:call-template>
				</select>
				<xsl:text>&#160;&#160;</xsl:text>
<a href="#" onclick="return(loadUrlWithDate('dtselectordate',{number(calendaryear[1]/calendarmonth[1]/@month_number)},1,{number(calendaryear[1]/@year)+1},'{servershortdateformat}'));">
<xsl:value-of select="number(calendaryear[1]/@year)+1"/><xsl:text>&#160;&gt;&gt;</xsl:text></a>
			</td>
		</tr>
	</table>
</xsl:template>

<xsl:template name="MoHeader">
	<xsl:param name="NextMo">0</xsl:param>
	<xsl:param name="PrevMo">0</xsl:param>
	<xsl:param name="NextYear">0</xsl:param>
	<xsl:param name="PrevYear">0</xsl:param>
	<td class="mup_monthHeaderPrev">
	<a href="#" onclick="return(loadUrlWithDate('dtselectordate','{$PrevMo}','1','{$PrevYear}','{../../servershortdateformat}'));">
	&lt;&lt; <xsl:value-of select="../../monthmeta/mometa[@monum=number($PrevMo)]/moabbrev"/></a></td>
	<td class="mup_monthHeader"><xsl:value-of select="../../calstartdatemonth"/></td>
	<td class="mup_monthHeaderNext">
	<a href="#" onclick="return(loadUrlWithDate('dtselectordate','{$NextMo}','1','{$NextYear}','{../../servershortdateformat}'));">
	<xsl:value-of select="../../monthmeta/mometa[@monum=number($NextMo)]/moabbrev"/> &gt;&gt;</a></td>
</xsl:template>

<xsl:template name="emptyday">
	<xsl:choose>
		<xsl:when test="(@weekday_number=0) or (@weekday_number=6)">
			<xsl:if test="number(../../../../showweekend) != 0">
				<xsl:call-template name="daycell">
					<xsl:with-param name="className">mup_weekendCell</xsl:with-param>
				</xsl:call-template>
			</xsl:if>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="daycell">
				<xsl:with-param name="className">mup_weekdayCell</xsl:with-param>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="daycell">
	<xsl:param name="className"/>
	<xsl:param name="dSep" select="../../../../serverdateseperator"/>
	<xsl:variable name="date">
		<xsl:choose>
			<xsl:when test="contains(../../../../requestinfo/workareadateformat,'short')">
				<xsl:value-of select="@shortdate"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@longdate"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<td class="{$className}" id="td_{translate(@servershortdate,$dSep,'_')}"
			onclick="{$updateFunctionName}('{@servershortdate}','{$date}','{@weekday_number}','{@dayofmonth}','{../../@month_number}','{../../../@year}');">
		<input type="hidden" name="z{translate(@servershortdate,$dSep,'_')}" value="{@longdate}"/>
		<xsl:value-of select="@dayofmonth"/>
		<div class="mup_noShow" id="div_{translate(@servershortdate,$dSep,'_')}"><xsl:value-of select="@longdate"/></div>
	</td>
</xsl:template>

<xsl:template name="rfn-year-for-loop">
	<xsl:param name="i" select="1"/>
	<xsl:param name="increment" select="1"/>
	<xsl:param name="testValue" select="1"/>
	<xsl:param name="iteration" select="1"/>
	<xsl:param name="defaultSelected" select="''"/>
	<xsl:variable name="testPassed">
		<!-- hard coded to "$i <= $testValue" -->
		<xsl:if test="$i &lt;= $testValue">
			<xsl:text>true</xsl:text>
		</xsl:if>
	</xsl:variable>
	<xsl:if test="$testPassed = 'true'">
		<option value="{$i}">
			<xsl:if test="$i = $defaultSelected">
				<!-- set current year as default -->
				<xsl:attribute name="selected">
					<xsl:text>selected</xsl:text>
				</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$i"/>
		</option>
		<xsl:call-template name="rfn-year-for-loop">
			<xsl:with-param name="i" select="$i + $increment"/>
			<xsl:with-param name="increment" select="$increment"/>
			<xsl:with-param name="testValue" select="$testValue"/>
			<xsl:with-param name="iteration" select="$iteration + 1"/>
			<xsl:with-param name="defaultSelected" select="$defaultSelected"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>