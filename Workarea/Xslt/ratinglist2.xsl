<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">

    <table border="0" width="100%">
      <xsl:for-each select="result/items/item">
        <tr>
          <td align="left">
            <xsl:choose>
                  <xsl:when test="userid&gt;0">
                    <a>
                      <xsl:attribute name="href">_userreviews.aspx?userid=<xsl:value-of select="userid"/></xsl:attribute>
                      <xsl:value-of select="userdisplayname"/>
                    </a>
                  </xsl:when>
                  <xsl:otherwise>Anonymous</xsl:otherwise>
                </xsl:choose>
		<xsl:choose>
               		<xsl:when test="state=0">
			&#160;<font color="red">Pending Approval</font>
			</xsl:when>
                  <xsl:when test="state=2">
      &#160;<font color="red"><b>Rejected</b></font>
      </xsl:when>
		</xsl:choose>
            <br/>
            <a><xsl:attribute name="href">_profilepage.aspx?id=<xsl:value-of select="contentid"/></xsl:attribute>
                      <xsl:value-of select="contenttitle"/>
            </a>
          </td>
        </tr>
        <tr>
          <td align="left">
            <xsl:value-of select="reviewsummary"/>
          </td>
        </tr>
        <tr>
          <td width="14%" align="right">
            <img id="ri1" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;0">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
                  <xsl:when test="rating&lt;1">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri2" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;1">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
                  <xsl:when test="rating&lt;2">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri3" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;2">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
                  <xsl:when test="rating&lt;3">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri4" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;3">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
                  <xsl:when test="rating&lt;4">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri5" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;4">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
                  <xsl:when test="rating&lt;5">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri6" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;5">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
                  <xsl:when test="rating&lt;6">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri7" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;6">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
                  <xsl:when test="rating&lt;7">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri8" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;7">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
                  <xsl:when test="rating&lt;8">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri9" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;8">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
                  <xsl:when test="rating&lt;9">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <img id="ri10" alt="">
              <xsl:attribute name="src">
                <xsl:choose>
                  <xsl:when test="rating&gt;9">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
                  <xsl:when test="rating&lt;10">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
                </xsl:choose>
              </xsl:attribute>
            </img>
          </td>
        </tr>
        <tr>
          <td colspan="2">
            <hr/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
</xsl:stylesheet>