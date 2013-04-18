<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html"/>

<xsl:variable name="updateFunctionName" select="'updateParentDateWithTime'"/>
<xsl:variable name="updateTimeFunction" select="'updateParentDateWithTime(0,0)'"/>

<xsl:include href="template_dtselector_common.xslt"/>
<xsl:include href="template_dtselector_date.xslt"/>
<xsl:include href="template_dtselector_time.xslt"/>

</xsl:stylesheet>