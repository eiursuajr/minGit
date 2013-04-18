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
		<h3><b>Error Occurred</b></h3>
		<p class="EktronEktronProductSearch_ErrorText"><b><xsl:value-of select="/root/errorMessage"/></b></p>
	</xsl:template>
	
	<xsl:template name="MakeSearchResultUI">
		<div class="ektron EktronEktronProductSearchresultWrapper">
			<div class="Results">
				<div class="searchResultsHeader">
					<h3>
						<xsl:choose>
							<xsl:when test="(/root/searchResults/searchResult) and not(/root/searchResults/searchResult='')">
								<span class="resultslabel">Results</span>
								<span class="beginPageCount"><xsl:value-of select="beginPageCount"/></span>
								-
								<span class="endPageCount"><xsl:value-of select="endPageCount"/></span>
								of
								<span class="totalCount"><xsl:value-of select="totalCount"/></span>
								<!-- for<span class="searchTerms"><xsl:value-of select="searchText"/>hard</span>. -->
								(<span class="searchDuration"><xsl:value-of select="timeTaken"/></span> seconds)
								</xsl:when>
							<xsl:otherwise>
								<span class="resultslabel">
                  <xsl:value-of select="/root/resourceStrings/noResults"/>
                </span>
                <h4>
                  <xsl:value-of select="/root/resourceStrings/searchDidNotMatchAnyProducts"/>
                </h4>
                <p>
                  <xsl:value-of select="/root/resourceStrings/suggestionsLabel"/>
                </p>
                <ul>
                  <li>
                    <xsl:value-of select="/root/resourceStrings/ensureWordsSpelledCorrectly"/>
                  </li>
                  <li>
                    <xsl:value-of select="/root/resourceStrings/tryDifferentKeywords"/>
                  </li>
                  <li>
                    <xsl:value-of select="/root/resourceStrings/tryGeneralKeywords"/>
                  </li>
                </ul>
							</xsl:otherwise>
						</xsl:choose>
					</h3>
				</div>				
				<table cellpadding="" cellspacing="" class="searchresulttable">
					<xsl:apply-templates select="/root/searchResults/searchResult" />
				</table>
			</div>
		</div>
	</xsl:template>
	
	<xsl:template match="searchResult">
		<tr>
			<xsl:if test="evenRow='true'"><xsl:attribute name="class" >evenRow</xsl:attribute></xsl:if>
			<td class="productsearchResult_imagecolumn">
				<a>
					<xsl:attribute name="href" >content.aspx?action=View&amp;folder_id=<xsl:value-of select="catalogId"/>&amp;id=<xsl:value-of select="contentID"/>&amp;LangType=<xsl:value-of select="contentLanguage"/>&amp;backpage=history</xsl:attribute>
					<xsl:choose>
						<xsl:when test="not(image=$SitePath)">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="image"/></xsl:attribute>
							</img>
						</xsl:when>
						<xsl:otherwise>
								<img ><xsl:attribute name="src"><xsl:value-of select="$AppImagePath"/>Commerce/productImageBackground.gif</xsl:attribute></img>
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
					<a><xsl:attribute name="href">content.aspx?action=View&amp;folder_id=<xsl:value-of select="catalogId"/>&amp;id=<xsl:value-of select="contentID"/>&amp;LangType=<xsl:value-of select="contentLanguage"/>&amp;backpage=history</xsl:attribute><xsl:value-of select="title"/></a>
				</div>
				<div class="sku">
					SKU: <xsl:value-of select="skuNumber"/>
				</div>
				<div class="prodcut_catalog">
					<span class="productid">Product ID: <xsl:value-of select="contentID"/></span>, <span class="catalogid">Catalog # <xsl:value-of select="catalogId"/></span>
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
					List Price: <xsl:value-of select="listPrice"/>
				</div>
				<div class="saleprice">
					Sale Price: <xsl:value-of select="salePrice"/>
				</div>
			</td>
		</tr>
	</xsl:template>
	
</xsl:stylesheet>