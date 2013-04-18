<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template name="buildXPath">
	<xsl:param name="xpath"/>
	<xsl:variable name="nodeType">
		<xsl:choose>
			<xsl:when test="@ektdesignns_nodetype">
				<xsl:value-of select="@ektdesignns_nodetype"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'element'"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:choose>
		<xsl:when test="@ektdesignns_bind">
			<xsl:value-of select="@ektdesignns_bind"/>
		</xsl:when>
		<xsl:when test="$nodeType='element'">
			<xsl:choose>
				<xsl:when test="$xpath='design_prototype_xpath'">
					<xsl:value-of select="concat($xpath,'.')"/>
				</xsl:when>
				<xsl:when test="$xpath='/'">
					<xsl:value-of select="concat('//',@ektdesignns_name,'[not(ancestor::',@ektdesignns_name,')]')"/>
				</xsl:when>
				<xsl:when test="$xpath='..'">
					<xsl:value-of select="'.'"/>
				</xsl:when>
				<xsl:when test="@ektdesignns_role='root' and @ektdesignns_name and $xpath='/root'">
					<xsl:value-of select="concat('/',@ektdesignns_name)"/>
				</xsl:when>
				<xsl:when test="@ektdesignns_name">
					<xsl:value-of select="concat($xpath,'/',@ektdesignns_name)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$xpath"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="$nodeType='mixed'">
			<xsl:value-of select="concat($xpath,'/node()')"/>
		</xsl:when>
		<xsl:when test="$nodeType='text'">
			<xsl:value-of select="concat($xpath,'/text()')"/>
		</xsl:when>
		<xsl:when test="$nodeType='attribute'">
			<xsl:value-of select="concat($xpath,'/@',@ektdesignns_name)"/>
		</xsl:when>
		<xsl:when test="$nodeType='label'">
			<!-- Special case: label may be an attribute ektdesignns_label of element. -->
			<xsl:value-of select="concat($xpath,'/@ektdesignns_label|',$xpath,'/../label[@for=current()/@id]')"/>
		</xsl:when>
		<!-- note: nodetype="attribute-set" xpath is unknown. -->
		<xsl:otherwise>
			<xsl:value-of select="$xpath"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>
