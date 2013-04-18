<?xml version='1.0'?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<xsl:template name="buildCellCheckAll">
	<xsl:param name="caption" select="'(All)'"/>
	<xsl:param name="name" select="'chkSelectAll'"/>
	<xsl:param name="action" select="'SelectAll(this)'"/>
	<xsl:param name="valign" select="'top'"/>
	<xsl:param name="rowspan"/>
	<th class="{$tableHeaderClass}" nowrap="true" align="center" valign="{$valign}">
		<xsl:if test="$rowspan">
			<xsl:attribute name="rowspan"><xsl:value-of select="$rowspan"/></xsl:attribute>
		</xsl:if>
		<xsl:copy-of select="$caption"/>
		<br />
		<input type="checkbox" id="{$name}" name="{$name}" onclick="{$action}" />
	</th>
</xsl:template>

<xsl:template name="buildCellCheckRow">
	<xsl:param name="name" select="'ektChk'"/>
	<xsl:param name="action" select="'CheckIt(this)'"/>
	<xsl:param name="attributes">
		<!-- The attributes need an element as a container -->
		<attribute-set/> 
	</xsl:param>

	<!-- the checkbox doesn't truly center unless you use the text-align style here -->
	<td align="center" style="text-align:center;">
		<!-- Can't use the name of the container element in an XPath of the node-set 
				because it does not work for some unknown reason. Use '*[1]' instead. -->
		<xsl:copy-of select="msxsl:node-set($attributes)/*[1]/@*"/>
		<input type="checkbox" id="{$name}" name="{$name}" onclick="{$action}" />
	</td>
</xsl:template>

</xsl:stylesheet>