<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="Collection">
        <xsl:apply-templates>
            <xsl:sort select="@count" data-type="number" order="descending"/>
        </xsl:apply-templates>
    </xsl:template>
    <xsl:template match="Hits">
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
            <tr>
                <td>
                    <xsl:value-of select="HitsMarker"/>
                </td>
            </tr>
            <xsl:for-each select="Content">
                <tr>
                    <td>
                        <a>
                            <xsl:attribute name="href">
                                <xsl:value-of select="QuickLink"/>
                            </xsl:attribute>
                            <xsl:value-of select="Title"/>
                        </a>
                    </td>
                </tr>
            </xsl:for-each>
        </table>
    </xsl:template>
</xsl:stylesheet>