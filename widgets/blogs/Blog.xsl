<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
	
	<xsl:param name="ShowComment"/>
	<xsl:param name="ShowSummary"/>	
    <xsl:param name="ShowAuthor"/>
	<xsl:param name="ShowDate"/>	
	
	
  <xsl:template match="/">
    <xsl:for-each select="/Collection/Content">
      <xsl:variable name="QuickLink">
        <string>
          <xsl:value-of select="QuickLink" />
        </string>
      </xsl:variable>
      <xsl:variable name="Category">
        <string>
          <xsl:value-of select="Categories/Category" />
        </string>
      </xsl:variable>
      <xsl:variable name="subContent">
        <string>
          <xsl:call-template name="tempRecursive">
            <xsl:with-param name="Content" select="Html" />
            <xsl:with-param name="ContentLength" select="600" />
          </xsl:call-template>
        </string>
      </xsl:variable>
      <div>
		  <a>
			  <xsl:attribute name="href">
				  <xsl:value-of select="$QuickLink"/>
			  </xsl:attribute>
				<h3><xsl:value-of select="Title"/></h3>
		  </a>

		  <xsl:if test ="$ShowAuthor != 'No'">
			<strong>
			  By  <xsl:value-of select="CreatorDisplayname"/>
			</strong>
		  </xsl:if>

		  <xsl:if test ="$ShowDate != 'No'">
			  <xsl:text > at </xsl:text>
			  <xsl:value-of select ="DateCreated"/>
		 </xsl:if>
		  
		  <xsl:if test ="$ShowComment != 'No'">
			  <a class="link">
				 
				  <xsl:attribute name="href">
					  <xsl:value-of select="concat($QuickLink,'#PostComments')"/>
				  </xsl:attribute>
				  Comments

			  </a>
			  <xsl:value-of select="concat(' (',CommentTotal,')')"/>  
		  </xsl:if>
		  <br />&#160;<br />

		  <xsl:if test ="$ShowSummary != 'No'">
			<xsl:value-of select="$subContent"/> 
		  </xsl:if> 
      </div>
    </xsl:for-each>
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
  <xsl:template name="tempRecursive">
    <xsl:param name="Content" />
    <xsl:param name="ContentLength" />
    <xsl:variable name="varContent">
      <xsl:value-of select="substring($Content,0,$ContentLength)" />
    </xsl:variable>
    <xsl:variable name="flag">
      <xsl:choose>
        <xsl:when test="string-length($Content) = string-length($varContent)">
          <xsl:value-of select="1" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="0" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="rtf">
      <string>
        <xsl:call-template name="reform_string">
          <xsl:with-param name="str" select="$varContent" />
          <xsl:with-param name="delim" select="' '" />
          <xsl:with-param name="flag" select="$flag" />
        </xsl:call-template>
      </string>
    </xsl:variable>
    <part>
      <xsl:value-of select="$rtf" />
    </part>
  </xsl:template>
  <xsl:template name="reform_string">
    <xsl:param name="str" />
    <xsl:param name="delim" />
    <xsl:param name="flag" />
    <xsl:choose>
      <xsl:when test="contains($str,' ')">
        <part>
          <xsl:value-of select="concat(substring-before($str,' '),' ')" />
        </part>
        <xsl:call-template name="reform_string">
          <xsl:with-param name="str" select="substring-after($str,' ')" />
          <xsl:with-param name="delim" select="$delim" />
          <xsl:with-param name="flag" select="$flag" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="$flag = 1">
          <part>
            <xsl:value-of select="$str" />
          </part>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>

