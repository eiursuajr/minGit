<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="ektroncalendars" xml:space="preserve">
	<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/calendarDisplayFuncs.js"></script>
	<script language="JavaScript" type="text/javascript" src="{ektroncalendar[1]/requestinfo/applicationpath}java/InternCalendarDisplayFuncs.js"></script>
	<link rel="stylesheet" type="text/css" href="{ektroncalendar[1]/requestinfo/applicationpath}csslib/InternCalendarStyles.css"/>	
	<xsl:apply-templates select="ektroncalendar"/>
</xsl:template>

<xsl:template match="ektroncalendar" xml:space="preserve">
<form name="selectorForm">
<table cellpadding="2" cellspacing="0" border="0">
	<tr>
		<td><img src="images/blank.gif" width="1" height="1" border="0" alt=""/><br/></td>
	</tr>
	<tr>
		<td valign="top">
		<xsl:call-template name="dateSelection"/>
		<xsl:call-template name="timeSelection"/>
		<input type="hidden" name="newDateTime" value=""/>
		<input type="hidden" name="newDisplayDateTime" value=""/>
		<input type="hidden" name="new_dow" value=""/>
		<input type="hidden" name="new_dom" value=""/>
		<input type="hidden" name="new_monum" value=""/>
		<input type="hidden" name="new_yrnum" value=""/>
		<input type="hidden" name="new_hr" value=""/>
		<input type="hidden" name="new_mi" value=""/>
		<input type="hidden" name="serveramdesignator" value="{timemeta/serveramdesignator}"/>
		<input type="hidden" name="serverpmdesignator" value="{timemeta/serverpmdesignator}"/>
		<input type="hidden" name="amdesignator" value="{timemeta/amdesignator}"/>
		<input type="hidden" name="pmdesignator" value="{timemeta/pmdesignator}"/>
		<input type="hidden" name="serverdateseperator" value="{serverdateseperator}"/>
		&#160;<br/>
		<div align="center"><input type="button" onclick="doneClick();" value="Done"/>
		&#160;<input type="button" onclick="cancelClick();" value="Cancel"/></div>
		</td>
	</tr>
</table>
</form>
</xsl:template>

<xsl:template name="optLoop">
	<xsl:param name="startrepeat">0</xsl:param>
	<xsl:param name="stoprepeat">23</xsl:param>
	<xsl:param name="step">1</xsl:param>
	<xsl:if test="number($startrepeat) &lt;= number($stoprepeat)">
		<option value="{$startrepeat}">
			<xsl:value-of select="format-number($startrepeat, '00')"/>
		</option>
		<xsl:call-template name="optLoop">
			<xsl:with-param name="startrepeat" select="$startrepeat + $step"/>
			<xsl:with-param name="stoprepeat" select="$stoprepeat"/>
			<xsl:with-param name="step" select="$step"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>