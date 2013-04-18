<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="eventEditButtons">
	<xsl:call-template name="eventEditButton">
		<xsl:with-param name="action">AddEvent</xsl:with-param>
		<xsl:with-param name="title">Add a new event</xsl:with-param>
		<xsl:with-param name="icon">icon_addevent.gif</xsl:with-param>
	</xsl:call-template>
	<xsl:call-template name="eventEditButton">
		<xsl:with-param name="action">ViewEvents</xsl:with-param>
		<xsl:with-param name="title">View the events</xsl:with-param>
		<xsl:with-param name="icon">icon_viewdate.gif</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<xsl:template name="eventEditButton">
	<xsl:param name="action"/>
	<xsl:param name="title"/>
	<xsl:param name="icon"/>
	<a href="#" onclick="ecmPopUpWindow('{../../../../requestinfo/applicationpath}workarea.aspx?page=cmscalendar.aspx&amp;action={$action}&amp;calendar_id={../../../../@id}&amp;iYear={../../../@year}&amp;iMonth={../../@month_number}&amp;iDay={@dayofmonth}&amp;LangType={../../../../@langid}', 'Admin400', 790, 580, 1, 1);return false;">
		<img height="16" width="16" alt="{$title}" title="{$title}" border="0" src="{../../../../requestinfo/appimgpath}{$icon}?i={@dayofmonth}"/>
	</a>&#160;
</xsl:template>

<xsl:template name="calendarEditButtons">
	<a href="#" onclick="ecmPopUpWindow('{requestinfo/applicationpath}workarea.aspx?page=cmscalendar.aspx&amp;action=ViewCalendar&amp;calendar_id={@id}&amp;LangType={@langid}', 'Admin400', 790, 580, 1, 1);return false;">
		<img height="16" width="16" alt="View the Calendar" title="View the Calendar"  border="0" src="{requestinfo/appimgpath}btn_calendar-nm.gif?i=1" />
	</a>&#160;&#160;&#160;&#160;&#160;<br/>
</xsl:template>

</xsl:stylesheet>