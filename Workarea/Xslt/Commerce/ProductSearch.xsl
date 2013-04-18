<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>
	<xsl:variable name="AppImagePath" select="/root/appImagePath" />
	<xsl:variable name="SitePath" select="/root/sitePath" />
	
	<xsl:template match="/root">
		<div class="EktronProductSearch">
			<xsl:attribute name="id" >EktronEktronProductSearch_<xsl:value-of select="$ControlId"/></xsl:attribute>
			<xsl:choose>
				<xsl:when test="/root/error='true'">
					<xsl:call-template name="MakeErrorUI" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:if test="not(/root/beginPageCount='0')">
					  <xsl:call-template name="MakeSearchResultUI" />
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>

	<xsl:template name="MakeErrorUI">
		<h3><b><xsl:value-of select="/root/resourceStrings/internalError"/>Error Occurred</b></h3>
		<p class="EktronEktronProductSearch_ErrorText"><b><xsl:value-of select="/root/errorMessage"/></b></p>
	</xsl:template>
	
	<xsl:template name="MakeSearchResultUI">
		<div class="ektron EktronEktronProductSearchresultWrapper">
			<div class="Results">
				<div class="searchResultsHeader">
					<h3>
						<xsl:choose>
							<xsl:when test="(/root/searchResults/searchResult) and not(/root/searchResults/searchResult='')">
								<span class="resultslabel"></span>
								<xsl:value-of select="/root/resourceStrings/msgSearchResultCountAndTime"/>
							</xsl:when>
							<xsl:otherwise>
								<span class="resultslabel"><xsl:value-of select="/root/resourceStrings/noResults"/></span>
							</xsl:otherwise>
						</xsl:choose>
					</h3>
				</div>				
				<table cellpadding="" cellspacing="" class="searchresulttable">
					<xsl:choose>
						<xsl:when test="(/root/searchResults/searchResult) and not(/root/searchResults/searchResult='')">
							<xsl:apply-templates select="/root/searchResults/searchResult" />
						</xsl:when>
						<xsl:otherwise>
							<tr>
								<td class="productsearchResult_noresultscolumn">
									<div class="noResultsPanel">
										<div class="resultPreview">
											<h4><xsl:value-of select="/root/resourceStrings/searchDidNotMatchAnyProducts"/></h4>
											<p><xsl:value-of select="/root/resourceStrings/suggestionsLabel"/></p>
											<ul>
												<li><xsl:value-of select="/root/resourceStrings/ensureWordsSpelledCorrectly"/></li>
												<li><xsl:value-of select="/root/resourceStrings/tryDifferentKeywords"/></li>
												<li><xsl:value-of select="/root/resourceStrings/tryGeneralKeywords"/></li>
											</ul>
										</div>
									</div>
								</td>
							</tr>
						</xsl:otherwise>
					</xsl:choose>
				</table>
			</div>
		</div>
	</xsl:template>
	
	<xsl:template match="searchResult">
		<tr>
			<xsl:if test="evenRow='true'"><xsl:attribute name="class" >evenRow</xsl:attribute></xsl:if>
			<td class="productsearchResult_imagecolumn">
				<a>
					<xsl:choose>
						<xsl:when test="/root/templateProduct and not(/root/templateProduct='')">
							<xsl:attribute name="href" ><xsl:value-of select="/root/templateProduct"/>?<xsl:value-of select="/root/dynamicProductParameter" />=<xsl:value-of select="contentID"/></xsl:attribute>
							<xsl:if test="/root/linkTargetCode != '0'"><xsl:attribute name="target" ><xsl:value-of select="/root/linkTarget"/></xsl:attribute></xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="href" ><xsl:value-of select="quickLink"/></xsl:attribute>
							<xsl:if test="/root/linkTargetCode != '0'"><xsl:attribute name="target" ><xsl:value-of select="/root/linkTarget"/></xsl:attribute></xsl:if>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:choose>
						<xsl:when test="not(image=$SitePath)">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="image"/></xsl:attribute>
							</img>
						</xsl:when>
						<xsl:otherwise>
								<img >
									<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>Commerce/productImageBackground.gif</xsl:attribute>
								</img>
						</xsl:otherwise>
					</xsl:choose>
				</a>
				<input type="hidden">
					<xsl:attribute name="id">productSearchResult_itemQuickLink_<xsl:value-of select="contentID"/></xsl:attribute>
					<xsl:attribute name="value" ><xsl:value-of select="quickLink"/></xsl:attribute>
				</input>
			</td>
			<td class="productsearchResult_detailscolumn">
				<div class="title">
					<xsl:choose>
						<xsl:when test="/root/templateProduct and not(/root/templateProduct='')">
							<a><xsl:if test="/root/linkTargetCode != '0'"><xsl:attribute name="target" ><xsl:value-of select="/root/linkTarget"/></xsl:attribute></xsl:if><xsl:attribute name="href" ><xsl:value-of select="/root/templateProduct"/>?<xsl:value-of select="/root/dynamicProductParameter" />=<xsl:value-of select="contentID"/></xsl:attribute><xsl:value-of select="title"/></a>	
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="title"/>
						</xsl:otherwise>
					</xsl:choose>
				</div>
				<div class="sku">
					<xsl:value-of select="/root/resourceStrings/skuLabel"/><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="skuNumber"/>
				</div>
				<div class="prodcut_catalog">
					<span class="productid"><xsl:value-of select="/root/resourceStrings/productIdLabel"/><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="contentID"/></span>, <span class="catalogid"><xsl:value-of select="/root/resourceStrings/catalogNumber"/><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="catalogId"/></span>
				</div>
				<div class="prod_cat">
					
				</div>
				<!--<div class="stockstatus" style="display: none;">
					Stock Status: 	
					<xsl:choose>
						<xsl:when test="inStock='true'">
							<span class="instock">In Stock</span>
							<span class="instockcount"> (Amount on hand: <xsl:value-of select="unitsInStock"/>)</span>
						</xsl:when>
						<xsl:otherwise>
							<span class="outofstock">Back Ordered</span>
						</xsl:otherwise>
					</xsl:choose>
				</div>-->
				<!--<xsl:if test="not((height='0') and (width='0') and (length='0')) and not((height='') and (width='') and (length=''))">
					<div class="dimensions">
						Dimensions: height <xsl:value-of select="height"/>, width <xsl:value-of select="width"/>, length <xsl:value-of select="length"/>
					</div>
				</xsl:if>
				<xsl:if test="not(weight='') and not(weight='0')">
					<div class="weight">
						Weight: <xsl:value-of select="weight"/>
					</div>
				</xsl:if>-->
				<div class="details">
					<xsl:value-of select="description"/>
				</div>
			</td>
			<td class="productsearchResult_pricecolumn">
				<div class="listprice">
					<xsl:value-of select="/root/resourceStrings/listPriceLabel"/><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="listPrice"/>
				</div>
				<div class="saleprice">
					<xsl:value-of select="/root/resourceStrings/salePriceLabel"/><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="salePrice"/>
				</div>
				<xsl:if test="recurrence/recurrenceInfo and recurrence/recurrenceInfo!=''">
					<div class="recurrenceinfo">
						<xsl:value-of select="recurrence/recurrenceInfo"/>
					</div>
				</xsl:if>				
				<xsl:if test="canAddToCart='true' and /root/templateCart and not(/root/templateCart='')">
					<div class="addtocartblock">
						<a><xsl:attribute name="href" ><xsl:value-of select="/root/templateCart"/>?product=<xsl:value-of select="contentID"/></xsl:attribute><xsl:value-of select="/root/resourceStrings/addToCart"/></a>	
					</div>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>