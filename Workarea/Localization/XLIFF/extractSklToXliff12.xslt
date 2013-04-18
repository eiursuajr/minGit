<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl skl its"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns="urn:oasis:names:tc:xliff:document:1.2" xmlns:its="http://www.w3.org/2005/11/its"
xmlns:skl="urn:oasis:names:tc:xliff:document:1.1" xmlns:ekt="urn:ektron:xliff">

<!-- run this transform on the xhtml skeleton (skl) file -->

<xsl:import href="extractSklToXliffBase.xslt" />

<xsl:template match="/">
	<xliff version="1.2" xmlns="urn:oasis:names:tc:xliff:document:1.2"
			xmlns:ekt="urn:ektron:xliff">
		<xsl:apply-templates select="skl:xliff/*" mode="skl"/>
	</xliff>
</xsl:template>

</xsl:stylesheet>
