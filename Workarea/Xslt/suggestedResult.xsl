<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template match="/">
    <xsl:if test="Collection/SuggestedResults/SuggestedResult">
      <div class="suggestedResults">
        <h3>Suggested Results</h3>
        <xsl:for-each select="Collection/SuggestedResults/SuggestedResult">
          <div class="resultPreview">
            <h4>
              <a href="{QuickLink}" title="{Title}"><xsl:value-of select ="Title"/></a>
            </h4>
            <p><xsl:value-of select ="Teaser"/></p>
          </div>
        </xsl:for-each>
      </div>
    </xsl:if>
    <xsl:for-each select="Collection/Content">
      <h4>
        <a href="{QuickLink}" title="{Title}">
          <xsl:value-of select ="Title"/>
        </a>
      </h4>
      <div class="resultPreview">
        <p>
          <xsl:value-of select ="Teaser"/>
        </p>
        <div class="resultPreviewDetails">
          <span class="resultPreviewId"><xsl:value-of select ="ID"/></span>
        </div>
      </div>
    </xsl:for-each>
	</xsl:template>
</xsl:stylesheet>

