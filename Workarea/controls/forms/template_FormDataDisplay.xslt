<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:import href="template_FormFieldValue.xslt"/>

<xsl:template match="Data">
	<xsl:param name="fieldlist"/>
	<table border="1" cellpadding="4" cellspacing="0">
		<col valign="top" />
		<col valign="top" />
		<thead>
			<tr>
				<th>Field</th>
				<th>Value</th>
			</tr>
		</thead>
		<tbody>
			<xsl:apply-imports/>
		</tbody>
	</table>
</xsl:template>

</xsl:stylesheet>