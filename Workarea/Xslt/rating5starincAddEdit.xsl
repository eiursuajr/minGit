<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">

<script type="text/javascript" src="[[ekapppath]]java/rating5starinc.js"></script>

    <input type="hidden" name="[[ekkey]]ekrelected" id="[[ekkey]]ekrelected">
      <xsl:attribute name="value">
        <xsl:choose>
          <xsl:when test="result/general/rated='1'">
            <xsl:value-of select="result/general/myrating"/>
          </xsl:when>
          <xsl:otherwise>
            0
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
    </input>
    <input type="hidden" name="[[ekkey]]ekcontentid" id="[[ekkey]]ekcontentid" value="[[ekcontentid]]" />
    <input type="hidden" name="[[ekkey]]_rating" id="[[ekkey]]_rating"><xsl:attribute name="value">
        <xsl:value-of select="result/general/myrating"/>
      </xsl:attribute>
    </input>
    <input type="hidden" name="[[ekkey]]_ratingid" id="[[ekkey]]_ratingid">
      <xsl:attribute name="value">
        <xsl:value-of select="result/general/ratingid"/>
      </xsl:attribute>
    </input>
    <input type="hidden" name="ekkey" id="ekkey" value="[[ekkey]]" />
    <input type="hidden" name="ekapppath" id="ekapppath" value="[[ekapppath]]" />
    <input type="hidden" name="ekimgpath" id="ekimgpath" value="[[ekimgpath]]" />
    <input type="hidden" name="[[ekkey]]_wasSubmitted" id="[[ekkey]]_wasSubmitted" value="" />
  <xsl:choose>
    <xsl:when test="result/general/userrated=1">
      <xsl:value-of select="/result/resourceStrings/stringYourReviewAccepted"/>
    </xsl:when>
    <xsl:otherwise>
    <table width="100%">
      <tr>
        <td rowspan="2">&#160;</td>
        <td width="108">
          <b><xsl:value-of select="/result/resourceStrings/stringRating"/>:</b>
        </td>
        <td width="1254">
          <img id="[[ekkey]]ri1" alt="" onclick="rhdlr('[[ekkey]]',1,'select');" onmouseover="rhdlr('[[ekkey]]',1,'over')" onmouseout="rhdlr('[[ekkey]]',1,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;0">[[ekimgpath]]star_l_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;1">[[ekimgpath]]star_l_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;0">[[ekimgpath]]star_l_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;1">[[ekimgpath]]star_l_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri2" alt="" onclick="rhdlr('[[ekkey]]',2,'select');" onmouseover="rhdlr('[[ekkey]]',2,'over')" onmouseout="rhdlr('[[ekkey]]',2,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;1">[[ekimgpath]]star_r_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;2">[[ekimgpath]]star_r_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;1">[[ekimgpath]]star_r_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;2">[[ekimgpath]]star_r_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri3" alt="" onclick="rhdlr('[[ekkey]]',3,'select');" onmouseover="rhdlr('[[ekkey]]',3,'over')" onmouseout="rhdlr('[[ekkey]]',3,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;2">[[ekimgpath]]star_l_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;3">[[ekimgpath]]star_l_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;2">[[ekimgpath]]star_l_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;3">[[ekimgpath]]star_l_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri4" alt="" onclick="rhdlr('[[ekkey]]',4,'select');" onmouseover="rhdlr('[[ekkey]]',4,'over')" onmouseout="rhdlr('[[ekkey]]',4,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;3">[[ekimgpath]]star_r_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;4">[[ekimgpath]]star_r_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;3">[[ekimgpath]]star_r_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;4">[[ekimgpath]]star_r_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri5" alt="" onclick="rhdlr('[[ekkey]]',5,'select');" onmouseover="rhdlr('[[ekkey]]',5,'over')" onmouseout="rhdlr('[[ekkey]]',5,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;4">[[ekimgpath]]star_l_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;5">[[ekimgpath]]star_l_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;4">[[ekimgpath]]star_l_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;5">[[ekimgpath]]star_l_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri6" alt="" onclick="rhdlr('[[ekkey]]',6,'select');" onmouseover="rhdlr('[[ekkey]]',6,'over')" onmouseout="rhdlr('[[ekkey]]',6,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;5">[[ekimgpath]]star_r_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;6">[[ekimgpath]]star_r_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;5">[[ekimgpath]]star_r_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;6">[[ekimgpath]]star_r_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri7" alt="" onclick="rhdlr('[[ekkey]]',7,'select');" onmouseover="rhdlr('[[ekkey]]',7,'over')" onmouseout="rhdlr('[[ekkey]]',7,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;6">[[ekimgpath]]star_l_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;7">[[ekimgpath]]star_l_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;6">[[ekimgpath]]star_l_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;7">[[ekimgpath]]star_l_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri8" alt="" onclick="rhdlr('[[ekkey]]',8,'select');" onmouseover="rhdlr('[[ekkey]]',8,'over')" onmouseout="rhdlr('[[ekkey]]',8,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;7">[[ekimgpath]]star_r_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;8">[[ekimgpath]]star_r_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;7">[[ekimgpath]]star_r_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;8">[[ekimgpath]]star_r_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri9" alt="" onclick="rhdlr('[[ekkey]]',9,'select');" onmouseover="rhdlr('[[ekkey]]',9,'over')" onmouseout="rhdlr('[[ekkey]]',9,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;8">[[ekimgpath]]star_l_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;9">[[ekimgpath]]star_l_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;8">[[ekimgpath]]star_l_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;9">[[ekimgpath]]star_l_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
          <img id="[[ekkey]]ri10" alt="" onclick="rhdlr('[[ekkey]]',10,'select');" onmouseover="rhdlr('[[ekkey]]',10,'over')" onmouseout="rhdlr('[[ekkey]]',10,'out');">
            <xsl:attribute name="src">
              <xsl:choose>
                <xsl:when test="result/general/rated='1' and result/general/myrating&gt;9">[[ekimgpath]]star_r_s.gif</xsl:when>
                <xsl:when test="result/general/rated='1' and result/general/myrating&lt;10">[[ekimgpath]]star_r_e.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&gt;9">[[ekimgpath]]star_r_f.gif</xsl:when>
                <xsl:when test="result/general/rated='0' and result/general/average&lt;10">[[ekimgpath]]star_r_e.gif</xsl:when>
              </xsl:choose>
            </xsl:attribute>
          </img>
        </td>
      </tr>
      <tr>
        <td width="108" valign="top">
          <b><xsl:value-of select="/result/resourceStrings/stringYourReview"/>:</b>
        </td>
        <td width="1254">
          <textarea rows="10" cols="51" id="[[ekkey]]_actualValue" name="[[ekkey]]_actualValue"><xsl:value-of select="result/general/review"/></textarea>
          <br/>
          <!-- uncomment out these lines if you want to add a terms and conditions that needs to be accepted.	
          <input type="checkbox" id="[[ekkey]]_agree" name="[[ekkey]]_agree" />&#160;I agree to the following:
        <p style="text-indent:30px;">
          lorem ipsum aventura lorem ipsum lorem ipsum aventura lorem ipsum lorem ipsum aventura
          lorem ipsum lorem ipsum aventura lorem ipsum lorem ipsum aventura lorem ipsum
        </p>
        &#160;&#160; -->
          <input type="submit" onclick="javascript:return ValidateReviewForm('[[ekkey]]');"  ><xsl:attribute name="value"><xsl:value-of select="/result/resourceStrings/stringSubmit"/></xsl:attribute></input>&#160;&#160;<input type="button" onclick="javascript:ClearReviewForm('[[ekkey]]'); return false;"  ><xsl:attribute name="value"><xsl:value-of select="/result/resourceStrings/stringClear"/></xsl:attribute></input>
        </td>
      </tr>
    </table>
      </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>
</xsl:stylesheet>