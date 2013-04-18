<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="timeSelection">
	<div align="center">&#160;<br/>
	<table cellpadding="3" cellspacing="0" border="0">
		<tr>
			<td>
				<xsl:choose>
					<xsl:when test="string-length(timemeta/amdesignator) &gt; 0">
						<select name="hrSelect" onchange="{$updateTimeFunction};">
							<option value="12">12</option>
							<xsl:call-template name="optLoop">
								<xsl:with-param name="startrepeat" select="1"/>
								<xsl:with-param name="stoprepeat" select="11"/>
							</xsl:call-template>
						</select>
					</xsl:when>
					<xsl:otherwise>
						<select name="hrSelect" onchange="{$updateTimeFunction};">
							<xsl:call-template name="optLoop">
								<xsl:with-param name="startrepeat" select="0"/>
								<xsl:with-param name="stoprepeat" select="23"/>
							</xsl:call-template>
						</select>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td><b><xsl:value-of select="timemeta/timeseperator"/></b></td>
			<td>
				<select name="miSelect" onchange="{$updateTimeFunction};">
					<xsl:call-template name="optLoop">
						<xsl:with-param name="startrepeat" select="0"/>
						<xsl:with-param name="stoprepeat" select="59"/>
						<xsl:with-param name="step" select="5"/>
					</xsl:call-template>
				</select>			
			</td>
			<td>
				<xsl:if test="string-length(timemeta/amdesignator) &gt; 0">
					<select name="meridSelect" onchange="{$updateTimeFunction};">
						<option value="a"><xsl:value-of select="timemeta/amdesignator"/></option>
						<option value="p"><xsl:value-of select="timemeta/pmdesignator"/></option>
					</select>
				</xsl:if>
			</td>
		</tr>
	</table>
	<input type="hidden" name="timeSeperator" value="{timemeta/servertimeseperator}"/>
	<input type="hidden" name="localTimeSeperator" value="{timemeta/timeseperator}"/>
	</div>
</xsl:template>

</xsl:stylesheet>