<%
dim unPackDisplayXslt

unPackDisplayXslt = "<?xml version=""1.0"" encoding=""UTF-8""?>" _
& "<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">" _
& "<xsl:output method=""xml"" version=""1.0"" encoding=""UTF-8"" indent=""yes""/>" _
& "<xsl:template match=""/"">" _
& "<xsl:choose>" _
& "<xsl:when test=""ektdesignpackage_forms/ektdesignpackage_form[1]/ektdesignpackage_views/ektdesignpackage_view[2]"">" _
& "<xsl:copy-of select=""ektdesignpackage_forms/ektdesignpackage_form		[1]/ektdesignpackage_views/ektdesignpackage_view[2]/node()""/>" _
& "</xsl:when>" _
& "	<xsl:otherwise>" _
& "<xsl:copy-of select=""ektdesignpackage_forms/ektdesignpackage_form		[1]/ektdesignpackage_views/ektdesignpackage_view[1]/node()""/>" _
& "</xsl:otherwise>" _
& "</xsl:choose>" _
& "</xsl:template>" _
& "</xsl:stylesheet>"
%>
