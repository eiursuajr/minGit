<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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
  <xsl:template match="/">
    <table cellSpacing="0" cellPadding="12" width="100%" border="0">
      <tr>
        <td>
          <xsl:for-each select="Collection/Content">
            <xsl:if test="NewDay='true'">
              <table cellSpacing="0" cellPadding="3" width="100%" bgColor="gainsboro" border="0">
                <tr>
                  <td align="left" width="100%">
                    &#160;<b>
                      <xsl:value-of select="EntryDate"/>
                    </b>
                  </td>
                  <td align="right">
                    <a title="Permanent link" href="#">
                      <img height="15" alt="Permanent link" src="images/application/permanentlink.gif" width="12" border="0"/>
                    </a>
                  </td>
                </tr>
              </table>
              <br/>
            </xsl:if>

            <b>
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
              </a>&#160;
            </b>
            (<xsl:for-each select="Categories/Category">
              <xsl:value-of select="Title"/>,
            </xsl:for-each>)

            <table>
              <tr>
                <td width="18">&#160;</td>
                <td>
                  <xsl:call-template name="copyData">
                    <xsl:with-param name="data" select="Teaser"/>
                  </xsl:call-template>
                </td>
              </tr>
            </table>
            <xsl:choose>
              <xsl:when test="Comments">
                <p>
                  <a>
                    <xsl:attribute name="href">
                      <xsl:choose>
                        <xsl:when test="Type ='Assets' or Type = 8 ">
                          javascript:void window.open('showcontent.aspx?id=<xsl:value-of select="ID"/>','showcontent','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=700,height=600')
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="QuickLink"/>#PostComments
                        </xsl:otherwise>
                      </xsl:choose>
                    </xsl:attribute>
                    Comments (<xsl:value-of select="CommentTotal"/>)
                  </a>
                </p>
                <hr/>
                <xsl:for-each select="Comments/Comment">
                  <table cellSpacing="0" cellPadding="0" border="0" valign="top">
                    <tr>
                      <td>
                        <xsl:value-of select="Message"/>
                        <p>
                          Posted by: <xsl:value-of select="DisplayName"/> (<xsl:value-of select="Email"/>) on <xsl:value-of select="DateCreated"/>
                        </p>
                      </td>
                    </tr>
                  </table>
                </xsl:for-each>

              </xsl:when>
              <xsl:otherwise>
                <p>&#160;</p>
              </xsl:otherwise>
            </xsl:choose>

          </xsl:for-each>
        </td>
      </tr>
    </table>
  </xsl:template>
</xsl:stylesheet>