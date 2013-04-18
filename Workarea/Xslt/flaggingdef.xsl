<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">

<script type="text/javascript" src="[[ekapppath]]java/flaggingdef.js"></script>
    <input type="hidden" name="[[ekkey]]ekrelected" id="[[ekkey]]ekrelected">
      <xsl:attribute name="value">
        <xsl:choose>
          <xsl:when test="ContentFlagData/flagged='1'"><xsl:value-of select="ContentFlagData/myflag"/></xsl:when>
          <xsl:otherwise>0</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
    </input>
    <input type="hidden" name="ekkey" id="ekkey" value="[[ekkey]]" />
	<input type="hidden" name="[[ekkey]]_lang" id="[[ekkey]]_lang" value="[[eklang]]" />
	<input type="hidden" name="ekapppath" id="ekapppath" value="[[ekapppath]]" />
    <input type="hidden" name="ekimgpath" id="ekimgpath" value="[[ekimgpath]]" />
    <input type="hidden" name="[[ekkey]]_israted" id="[[ekkey]]_israted">
      <xsl:attribute name="value">
        <xsl:choose>
          <xsl:when test="ContentFlagData/flagged='1'">1</xsl:when>
          <xsl:otherwise>0</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
    </input>
    <input type="hidden" name="[[ekkey]]ekcontentid" id="[[ekkey]]ekcontentid" value="[[ekcontentid]]" />

    <div id="[[ekkey]]_flagItPanel" class="flagitpanel" style="display:none;">
      <span class="flagTopLabel">
        Flag It
      </span >
      <br/>
      <span class="flagDesc">
        Enter a reason.
      </span>
      <br/>
      <select name="[[ekkey]]_flagReason" id="[[ekkey]]_flagReason">
        <option value="">(Select a reason)</option>
        <xsl:for-each select="ContentFlagData/FlagDefinition/Items/FlagItemData">
          <option>
            <xsl:attribute name="value">
              <xsl:value-of select="ID"/>
            </xsl:attribute>
            <xsl:if test="/ContentFlagData/FlagId=ID">
              <xsl:attribute name="selected">selected</xsl:attribute>
            </xsl:if>
            <xsl:value-of select="Name"/>
          </option>
        </xsl:for-each>
      </select>
      <br />
      <br />
      <span class="standardLabel">
        Comments (optional)<br />
      </span>

      <textarea align="left" name="[[ekkey]]_flagMessage" id="[[ekkey]]_flagMessage" type="text" cols="40" rows="4" >
        <xsl:value-of select="ContentFlagData/FlagComment"/>
      </textarea>

      <br />
      <a href="javascript:fhdlr('[[ekkey]]', document.getElementById('[[ekkey]]_flagReason').options[document.getElementById('[[ekkey]]_flagReason').selectedIndex].value ,'click');" class="buttonD_92">Flag</a>
      &#160;&#160;<a href="javascript:toggleVisibility('[[ekkey]]_flagItPanel');">Close</a>

      <div id="[[ekkey]]flagItError" class="ekflagredText" style="display: none;"></div>
      <div id="[[ekkey]]flagItMsg" class="ekflagblueText" style="display: none;"></div>
    </div>
    <a href="javascript:toggleVisibility('[[ekkey]]_flagItPanel');" name="flag">
		<xsl:choose><xsl:when test="ContentFlagData/FlagImage=''"><xsl:choose><xsl:when test="ContentFlagData/FlagText=''">Flag</xsl:when><xsl:otherwise><xsl:value-of select="ContentFlagData/FlagText"/></xsl:otherwise></xsl:choose></xsl:when><xsl:otherwise><img border="0"><xsl:attribute name="src"><xsl:value-of select="ContentFlagData/FlagImage"/></xsl:attribute></img></xsl:otherwise></xsl:choose>
	</a>
    
  </xsl:template>
</xsl:stylesheet>