<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <xsl:apply-templates select="Collection"/>
    </xsl:template>
    <xsl:template match="Collection">
        <link type="text/css"  rel="STYLESHEET" href="$appPath$csslib/ektron.workarea.css" />
        <table width="100%" border="0" cellpadding="2" cellspacing="2">
            <xsl:variable name="nonImageItems" select="Content[not(LibraryTypeID='1')]"/>
            <xsl:variable name="imageItems" select="Content[LibraryTypeID='1']"/>
            <tr>
                <td colspan="2" >
                    Total results:&#160;<xsl:value-of select="TotalResults"/>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <xsl:if test="count($imageItems) &gt; 0">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <xsl:for-each select="$imageItems">
                                <tr>
                                    <td colspan="2">&#160;</td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <hr/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">&#160;</td>
                                </tr>
                                <tr>
                                    <td class="search_result_image_cell" width="10%">
                                        <a target="_blank" href="$appPath$DisplayResult.aspx?libid={LibraryID}">
                                            <img src="{LibraryThumbnail}" alt="{LibraryTitle}" class="search_result"/>
                                        </a>
                                    </td>
                                    <td align="Left" valign="top">
                                        <xsl:call-template name="copyData">
                                            <xsl:with-param name="data" select="Teaser/node()"/>
                                        </xsl:call-template>
                                    </td>
                                </tr>
                            </xsl:for-each>
                            <tr>
                                <td colspan="2">
                                    <hr/>
                                </td>
                            </tr>
                        </table>
                    </xsl:if>
                    <xsl:if test="count($nonImageItems) &gt; 0">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td width="20%">&#160;&#160;&#160;&#160;</td>
                                <td width="5%">&#160;&#160;</td>
                                <td width="75%">&#160;</td>
                            </tr>
                            <xsl:for-each select="$nonImageItems">
                                <xsl:variable name="evenOrOdd">
                                    <xsl:variable name="n">
                                        <xsl:number count="Content[not(LibraryTypeID='1')]" level="single"/>
                                    </xsl:variable>
                                    <xsl:choose>
                                        <xsl:when test="number($n) mod 2 = 0">even</xsl:when>
                                        <xsl:otherwise>odd</xsl:otherwise>
                                    </xsl:choose>
                                </xsl:variable>
                                <tr class="search_result_{$evenOrOdd}row">
                                    <td align="left" valign="top" class="search_result_{$evenOrOdd}row">
                                        <xsl:if test="Icon">
                                            <img src="{Icon/FileName}" width="16" height="16" border="0" alt="{Icon/Description}"/>&#160;
                                        </xsl:if><xsl:if test="(Type='Content') or (Type='Forms') or (Type='Archive Content')">
                                            <a href="{QuickLink}">
                                                <xsl:call-template name="copyData">
                                                    <xsl:with-param name="data" select="Title/node()"/>
                                                </xsl:call-template>
                                            </a>
                                        </xsl:if><xsl:if test="(Type='Assets')">
                                            <a href="{QuickLink}">
                                                <xsl:call-template name="copyData">
                                                    <xsl:with-param name="data" select="Title/node()"/>
                                                </xsl:call-template>
                                            </a>
                                        </xsl:if><xsl:if test="not(Type='Content')">
                                            <a target="_blank" href="{LibraryFileName}">
                                                <xsl:call-template name="copyData">
                                                    <xsl:with-param name="data" select="LibraryTitle/node()"/>
                                                </xsl:call-template>
                                            </a>
                                        </xsl:if>&#160;&#160;&#160;&#160;
                                    </td>
                                    <td align="left" valign="top" class="search_result_{$evenOrOdd}row">&#160;&#160;</td>
                                    <td align="left" valign="top" class="search_result_{$evenOrOdd}row">
                                        <xsl:call-template name="copyData">
                                            <xsl:with-param name="data" select="Teaser/node()"/>
                                        </xsl:call-template>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="left" valign="top" class="search_result_{$evenOrOdd}row">
                                        <br/>
                                    </td>
                                </tr>
                            </xsl:for-each>
                        </table>
                    </xsl:if>
                </td>
            </tr>
        </table>
    </xsl:template>
    <xsl:template name="copyData">
        <xsl:param name="data"/>
        <xsl:param name="numberOfTimes" select="1"/>
        <xsl:param name="i" select="1"/>
        <xsl:if test="($i &lt;= $numberOfTimes) or not($numberOfTimes)">
            <!-- copies an RTF but adds a closing tag to empty elements with a few exceptions -->
            <xsl:apply-templates select="$data" mode="resultTreeFragment"/>
        </xsl:if>
        <xsl:if test="$i &lt; $numberOfTimes">
            <xsl:call-template name="copyData">
                <xsl:with-param name="data" select="$data"/>
                <xsl:with-param name="numberOfTimes" select="$numberOfTimes"/>
                <xsl:with-param name="i" select="$i + 1"/>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
    <xsl:template match="@*|node()" mode="resultTreeFragment">
        <!-- identity with closing tags -->
        <xsl:copy>
            <xsl:apply-templates select="@*|node()" mode="resultTreeFragment"/>
        </xsl:copy>
    </xsl:template>
    <!-- See similar templates for identity without closing tags -->
    <xsl:template match="xsl:*[not(node())]|area[not(node())]|bgsound[not(node())]|br[not(node())]|hr[not(node())]|img[not(node())]|input[not(node())]|param[not(node())]" mode="resultTreeFragment">
        <!-- identity without closing tags -->
        <xsl:copy>
            <xsl:apply-templates select="@*" mode="resultTreeFragment"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
