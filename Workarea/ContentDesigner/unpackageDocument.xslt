<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_documents/ektdesignpackage_document">
			<xsl:copy-of select="ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_documents/ektdesignpackage_document[1]/node()"/>
		</xsl:when>
		<xsl:when test="ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_initialDocuments/ektdesignpackage_initialDocument[1]">
			<xsl:copy-of select="ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_initialDocuments/ektdesignpackage_initialDocument[1]/node()"/>
		</xsl:when>
		<xsl:otherwise>
			<root/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>
