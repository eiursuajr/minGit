<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
            <xsl:for-each select="Collection/Content">
                <tr>
                    <td>
                        <a>
                            <xsl:attribute name="href">
                                <xsl:choose>
                                    <xsl:when test="Type ='Assets' or Type = 8 ">
                                        javascript:void window.open('showcontent.aspx?id=<xsl:value-of select="ID"/>','showcontent','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=700,height=600')
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <xsl:value-of select="QuickLink"/>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </xsl:attribute>
                            <xsl:value-of select="Title"/>
                        </a>
                    </td>
                </tr>
            </xsl:for-each>
        </table>
    </xsl:template>
</xsl:stylesheet>