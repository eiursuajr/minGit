<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="calendaradmin_month.xsl"/>

<xsl:output method="html"/>

<xsl:template match="ektroncalendars" xml:space="preserve">
	<!--
    Change Log:Udai, 11/28/05 commented these two lines it seems it is never been used and no file exists for the output location.
    <script language="JavaScript" type="text/javascript" src="calendarDisplayFuncs.js"></script>
	<link rel="stylesheet" type="text/css" href="calendarStyles.css"/>
    -->
	<xsl:apply-imports/>
</xsl:template>

<xsl:template name="eventEditButton">
	<xsl:param name="action"/>
	<xsl:param name="title"/>
	<xsl:param name="icon"/>
	<a href="{../../../../requestinfo/applicationpath}cmscalendar.aspx?displaymod=editworkarea&amp;action={$action}&amp;callback=ViewEvents&amp;calendar_id={../../../../@id}&amp;iYear={../../../@year}&amp;iMonth={../../@month_number}&amp;iDay={@dayofmonth}&amp;LangType={../../../../@langid}">
		<img height="16" width="16" alt="{$title}" title="{$title}" border="0" src="{../../../../requestinfo/appimgpath}{$icon}?i={../@dayofmonth}"/>
	</a>&#160;
</xsl:template>

</xsl:stylesheet>