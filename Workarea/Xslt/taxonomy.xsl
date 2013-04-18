<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
  <xsl:template match="/">
    <div class="main" id="mainblock">
      <xsl:for-each select="./TaxonomyData/Taxonomy/TaxonomyData">
        <div class="taxonomyHeader">
          <table border="0" cellpadding="0" cellspacing="0">
            <tr>
              <td align="left">
                <xsl:value-of select="TaxonomyName"/>
              </td>
            </tr>
          </table>
        </div>
        <div style="display:none;">
          <xsl:attribute name="id">
            <xsl:value-of select="concat('Taxon', TaxonomyId)"/>
          </xsl:attribute>
          <table border="0" cellpadding="0" cellspacing="0">
            <tr>
              <xsl:for-each select="./Taxonomy/TaxonomyData">
                <td>
                  <a>
                    <xsl:value-of select="TaxonomyName"/> |
                  </a>
                </td>
              </xsl:for-each>
            </tr>
          </table>
        </div>
      </xsl:for-each>
    </div>
  </xsl:template>
</xsl:stylesheet>
