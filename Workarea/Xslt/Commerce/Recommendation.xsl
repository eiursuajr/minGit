<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>

	<xsl:template match="/root">
		<div class="EktronRecommendation">
			<xsl:attribute name="id" >EktronRecommendation_<xsl:value-of select="$ControlId"/></xsl:attribute>
			<xsl:choose>
				<xsl:when test="/root/error='true'">
					<xsl:call-template name="MakeErrorUI" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="MakeUI" />
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>

	<xsl:template name="MakeErrorUI">
		<h3><b><xsl:value-of select="/root/resourceStrings/internalErrorOccurred"/></b></h3>
		<p class="EktronRecommendation_ErrorText"><b><xsl:value-of select="/root/errorMessage"/></b></p>
	</xsl:template>
	
	<xsl:template name="MakeUI">
		<div class="ektron EktronRecommendationWrapper">
			<xsl:choose>
				<xsl:when test="/root/recommendationType=0">
					<h3><xsl:value-of select="/root/resourceStrings/youMightAlsoLike"/></h3>
				</xsl:when>
				<xsl:otherwise>
					<h3><xsl:value-of select="/root/resourceStrings/wantToUpgrade"/></h3>
				</xsl:otherwise>
			</xsl:choose>
			<div class="recommendations">
				<ul class="Recommendation clearfix">
					<xsl:apply-templates select="/root/recommendations/recommendation" />
				</ul>
			</div>
		</div>
	</xsl:template>
	
	<xsl:template match="recommendation">
		<li>
			<!-- xsl:attribute name="onclick">window.location.href='<xsl:value-of select="/root/urlrecommendation"/>?id=<xsl:value-of select="id"/>';return false;</xsl:attribute -->
			<div class="imageColumn">
				<img class="productImage" >
					<!-- xsl:attribute name="alt"><xsl:value-of select="title"/></xsl:attribute -->
					<xsl:choose>
						<xsl:when test="image!=''">
							<xsl:attribute name="src"><xsl:value-of select="/root/sitePath"/><xsl:value-of select="imageThumbnail"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="src"><xsl:value-of select="/root/appImagePath"/>Commerce/productImageBackground.gif</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
				</img>
			</div>
			<div class="titleColumn">
				<xsl:choose>
					<xsl:when test="productLink and productLink!=''">
						<a><xsl:attribute name="href" ><xsl:value-of select ="productLink"/></xsl:attribute><xsl:value-of select="title"/></a>
					</xsl:when>
					<xsl:otherwise>
						<span class="titleNoLink" ><xsl:value-of select="title"/></span>
					</xsl:otherwise>
				</xsl:choose>
			</div>
			<div class="priceColumn">
				<span class="price"><xsl:value-of select="listPrice"/></span>
			</div>
			<div class="addToCartButton">
				<xsl:if test="addToCartLink!=''">
					<a><xsl:attribute name="href"><xsl:value-of select ="addToCartLink"/></xsl:attribute><xsl:value-of select="/root/resourceStrings/addToCart"/></a>
				</xsl:if>
			</div>
		</li>
	</xsl:template>
	
</xsl:stylesheet>