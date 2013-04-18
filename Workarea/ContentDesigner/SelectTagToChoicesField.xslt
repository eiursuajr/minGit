<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>
	<xsl:param name="elemName" select="'Field'"/>
	<xsl:param name="nodeType" select="'radio'"/>
	<!--
Expecting input:
<select>
	<option selected="true" value="11" disabled="true">First</option>
	<option selected="false" value="22" disabled="false">Second</option>
</select>

Output:
<ol contenteditable="false" unselectable="on">
	<li>
		<input checked="true" type="radio" ektdesignns_nodetype="item" name="Field" id="id0x07f65898" value="11"/>
		<label for="id0x07f65898" contenteditable="true" unselectable="off">First</label>
	</li>
	<li>
		<input disabled="true" type="radio" ektdesignns_nodetype="item" name="Field" id="id0x086ba180" value="22"/>
		<label for="id0x086ba180" contenteditable="true" unselectable="off">Second</label>
	</li>
</ol>
-->
	<xsl:template match="/">
		<ol contenteditable="false" unselectable="on">
			<xsl:for-each select="/select/option">
				<xsl:variable name="id" select="concat($elemName,generate-id(.))"/>
				<li>
					<input type="{$nodeType}" ektdesignns_nodetype="item" name="{$elemName}" id="{$id}">
						<xsl:choose>
							<xsl:when test="string-length(normalize-space(@value))&gt;0">
								<xsl:attribute name="value"><xsl:value-of select="@value"/></xsl:attribute>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name="value"><xsl:value-of select="text()"/></xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="@selected='selected' or @selected='true' or @selected='1'">
							<xsl:attribute name="checked">checked</xsl:attribute>
						</xsl:if>
						<xsl:if test="@disabled='disabled' or @disabled='true' or @disabled='1'">
							<xsl:attribute name="disabled">disabled</xsl:attribute>
						</xsl:if>
					</input>
					<label for="{$id}" contenteditable="true" unselectable="off">
						<xsl:value-of select="./text()"/>
					</label>
				</li>
			</xsl:for-each>
		</ol>
	</xsl:template>

</xsl:stylesheet>