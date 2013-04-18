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
                                    <xsl:when test="Type ='Assets' or Type = 8 ">#    </xsl:when>
                                    <xsl:otherwise>
                                        <xsl:choose>
                                            <xsl:when test="contains(QuickLink,'?')">
                                                <xsl:value-of select="concat(QuickLink,'&amp;fragment=', AllowFragments,'&amp;SearchType=', HighlightType, '&amp;terms=', Highlight)"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:value-of select="concat(QuickLink,'?fragment=', AllowFragments,'&amp;SearchType=', HighlightType, '&amp;terms=', Highlight)"/>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </xsl:attribute>
                            <xsl:choose>
                                <xsl:when test="Type ='Assets' or Type = 8 ">
										<xsl:attribute name="href">
											<xsl:value-of select="QuickLink"/>
										</xsl:attribute>
                                </xsl:when>
                            </xsl:choose>
                            <xsl:value-of select="Title"/>
                        </a>
                    </td>
                </tr>
            </xsl:for-each>
        </table>
    </xsl:template>
</xsl:stylesheet>