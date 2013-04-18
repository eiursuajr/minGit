<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">

<script type="text/javascript" src="[[ekapppath]]java/rating5star.js"></script>

<input type="hidden" name="[[ekkey]]ekrelected" id="[[ekkey]]ekrelected">
 <xsl:attribute name="value">
  <xsl:choose>
   <xsl:when test="result/general/rated='1'"><xsl:value-of select="result/general/myrating"/></xsl:when>
   <xsl:otherwise><xsl:value-of select="result/general/average"/></xsl:otherwise>
  </xsl:choose>
 </xsl:attribute>
</input>
<input type="hidden" name="ekkey" id="ekkey" value="[[ekkey]]" />
<input type="hidden" name="ekapppath" id="ekapppath" value="[[ekapppath]]" />
<input type="hidden" name="ekimgpath" id="ekimgpath" value="[[ekimgpath]]" />
<input type="hidden" name="[[ekkey]]_israted" id="[[ekkey]]_israted">
  <xsl:attribute name="value">
    <xsl:choose>
      <xsl:when test="result/general/rated='1'">
        1
      </xsl:when>
      <xsl:otherwise>
        0
      </xsl:otherwise>
    </xsl:choose>
  </xsl:attribute>
</input>
<input type="hidden" name="[[ekkey]]ekcontentid" id="[[ekkey]]ekcontentid" value="[[ekcontentid]]" />
<img id="[[ekkey]]ri2" alt="" onclick="rhdlr('[[ekkey]]',2,'click');" onmouseover="rhdlr('[[ekkey]]',2,'over')" onmouseout="rhdlr('[[ekkey]]',2,'out');">
<xsl:attribute name="src">
  <xsl:choose>
   <xsl:when test="result/general/rated='1' and result/general/myrating&gt;0">[[ekimgpath]]star_s.gif</xsl:when>
   <xsl:when test="result/general/rated='1' and result/general/myrating&lt;2">[[ekimgpath]]star_e.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&gt;0">[[ekimgpath]]star_f.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&lt;2">[[ekimgpath]]star_e.gif</xsl:when>
  </xsl:choose>
 </xsl:attribute>
</img><img id="[[ekkey]]ri4" alt="" onclick="rhdlr('[[ekkey]]',4,'click');" onmouseover="rhdlr('[[ekkey]]',4,'over')" onmouseout="rhdlr('[[ekkey]]',4,'out');">
<xsl:attribute name="src">
  <xsl:choose>
   <xsl:when test="result/general/rated='1' and result/general/myrating&gt;2">[[ekimgpath]]star_s.gif</xsl:when>
   <xsl:when test="result/general/rated='1' and result/general/myrating&lt;4">[[ekimgpath]]star_e.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&gt;2">[[ekimgpath]]star_f.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&lt;4">[[ekimgpath]]star_e.gif</xsl:when>
  </xsl:choose>
 </xsl:attribute>
</img><img id="[[ekkey]]ri6" alt="" onclick="rhdlr('[[ekkey]]',6,'click');" onmouseover="rhdlr('[[ekkey]]',6,'over')" onmouseout="rhdlr('[[ekkey]]',6,'out');">
<xsl:attribute name="src">
  <xsl:choose>
   <xsl:when test="result/general/rated='1' and result/general/myrating&gt;4">[[ekimgpath]]star_s.gif</xsl:when>
   <xsl:when test="result/general/rated='1' and result/general/myrating&lt;6">[[ekimgpath]]star_e.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&gt;4">[[ekimgpath]]star_f.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&lt;6">[[ekimgpath]]star_e.gif</xsl:when>
  </xsl:choose>
 </xsl:attribute>
</img><img id="[[ekkey]]ri8" alt="" onclick="rhdlr('[[ekkey]]',8,'click');" onmouseover="rhdlr('[[ekkey]]',8,'over')" onmouseout="rhdlr('[[ekkey]]',8,'out');">
<xsl:attribute name="src">
  <xsl:choose>
   <xsl:when test="result/general/rated='1' and result/general/myrating&gt;6">[[ekimgpath]]star_s.gif</xsl:when>
   <xsl:when test="result/general/rated='1' and result/general/myrating&lt;8">[[ekimgpath]]star_e.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&gt;6">[[ekimgpath]]star_f.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&lt;8">[[ekimgpath]]star_e.gif</xsl:when>
  </xsl:choose>
 </xsl:attribute>
</img><img id="[[ekkey]]ri10" alt="" onclick="rhdlr('[[ekkey]]',10,'click');" onmouseover="rhdlr('[[ekkey]]',10,'over')" onmouseout="rhdlr('[[ekkey]]',10,'out');">
<xsl:attribute name="src">
  <xsl:choose>
   <xsl:when test="result/general/rated='1' and result/general/myrating&gt;8">[[ekimgpath]]star_s.gif</xsl:when>
   <xsl:when test="result/general/rated='1' and result/general/myrating&lt;10">[[ekimgpath]]star_e.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&gt;8">[[ekimgpath]]star_f.gif</xsl:when>
   <xsl:when test="result/general/rated='0' and result/general/average&lt;10">[[ekimgpath]]star_e.gif</xsl:when>
  </xsl:choose>
 </xsl:attribute>
</img>
  </xsl:template>
</xsl:stylesheet>