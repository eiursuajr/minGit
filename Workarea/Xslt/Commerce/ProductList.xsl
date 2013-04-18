<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>

	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="/root/error='true'">
				<xsl:call-template name="MakeErrorUI" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="/root/dataMode='full'">
						<xsl:call-template name="MakeUI" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="MakeInnerUI" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="MakeUI">
		<div class="EktronProductList">
			<xsl:attribute name="id" >EktronProductList_<xsl:value-of select="$ControlId"/></xsl:attribute>
			<div class="ektron EktronProductListWrapper">
				<!--<h3>Products</h3>-->
				<div class="products">
					<xsl:if test="/root/filtering/enableFiltering='true'">
						<xsl:call-template name="MakeFilteringUI" />
					</xsl:if>
					<xsl:call-template name="MakeInnerUI" />
				</div>
			</div>
			<xsl:call-template name="RenderJavascript" />
		</div>
	</xsl:template>
	
	<xsl:template name="MakeInnerUI">
		<div class="InnerUI">
			<ul class="productList clearfix">
				<xsl:apply-templates select="/root/products/product" />
			</ul>
			<xsl:if test="/root/paging/enablePaging='true'">
				<xsl:call-template name="MakePagingUI" />
			</xsl:if>
		</div>
	</xsl:template>
	
	<xsl:template name="MakeErrorUI">
		<h3><b><xsl:value-of select="/root/resourceStrings/internalErrorOccurred"/></b></h3>
		<p class="EktronProductList_ErrorText"><b><xsl:value-of select="/root/errorMessage"/></b></p>
	</xsl:template>
	
	<xsl:template match="product">
		<li>
			<xsl:choose>
				<xsl:when test="/root/urlProduct and /root/urlProduct!=''">
					<xsl:attribute name="onclick">window.location.href='<xsl:value-of select="/root/urlProduct"/>?id=<xsl:value-of select="id"/>';return false;</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="onclick">window.location.href='<xsl:value-of select="quicklink"/>';return false;</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<span class="productImage">
				<img class="productImage" >
					<xsl:attribute name="alt"><xsl:value-of select="name"/></xsl:attribute>
					<xsl:if test="image!=''">
						<xsl:attribute name="src"><xsl:value-of select="/root/sitePath"/><xsl:value-of select="imageThumbnail"/></xsl:attribute>
					</xsl:if>
				</img>
			</span>
			<span class="productName">
				<a>
					<xsl:choose>
						<xsl:when test="/root/urlProduct and /root/urlProduct!=''">
							<xsl:attribute name="href"><xsl:value-of select="/root/urlProduct"/>?id=<xsl:value-of select="id"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="href"><xsl:value-of select="quicklink"/></xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:value-of select="name"/>
				</a>
			</span>
			<ul class="prices">
				<xsl:choose>
						<xsl:when test="not (msrpvalue = '0.00') and not (msrp = listPrice)">
							<li class="priceOld"><xsl:value-of select="msrp"/></li>
						</xsl:when>
					</xsl:choose>
				<li class="priceCurrent"><xsl:value-of select="listPrice"/>
					<xsl:if test="recurrenceInfo and recurrenceInfo!=''">
						<xsl:text disable-output-escaping="yes" xml:space="preserve" > </xsl:text>
						<xsl:value-of select="recurrenceInfo"/>
					</xsl:if>
				</li>
        <xsl:if test="/root/getAnalyticsData='true'">
          <li><xsl:apply-templates select="analyticsInfo" /></li>
          <li>[<xsl:value-of select="analyticsInfo/contentRatingCount"/>]</li>
        </xsl:if>
        <!--<li>Average: <xsl:value-of select="analyticsInfo/contentRatingAverage"/></li>
        <li>Views: <xsl:value-of select="analyticsInfo/contentViewCount"/></li>-->
			</ul>
		</li>
	</xsl:template>
	
	<xsl:template name="MakePagingUI">
		<div class="EktronProductListPaging">
			<xsl:if test="/root/paging/skippedPreviousPages='true'">
				<span class="EktronProductList_MutableControlContainer">
					<a class="EktronProductListPageLink">
						<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
						<xsl:attribute name="onclick" >Ektron_Ecommerce_ProductListClass.GetObject('<xsl:value-of select="$ControlId" />').GetPage('1'); return (false);</xsl:attribute>				
						<xsl:value-of select="/root/resourceStrings/first"/>
					</a> ... 
				</span>
			</xsl:if>
			<xsl:apply-templates select="/root/paging/pages/page" />
			<xsl:if test="/root/paging/skippedLaterPages='true'">
				<span class="EktronProductList_MutableControlContainer">
					 ... <a class="EktronProductListPageLink">
						<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
						<xsl:attribute name="onclick" >Ektron_Ecommerce_ProductListClass.GetObject('<xsl:value-of select="$ControlId" />').GetPage('<xsl:value-of select="/root/paging/lastPage"/>'); return (false);</xsl:attribute>				
						<xsl:value-of select="/root/resourceStrings/last"/>
					</a>
				</span>
			</xsl:if>
			<span class="EktronProductList_AjaxBusyImageContainer" style="display: none;">
				<img alt="">
					<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
				</img>
			</span>
		</div>
	</xsl:template>
	
	<xsl:template match="page">
		<span class="EktronProductList_MutableControlContainer">
			<a>
				<xsl:choose>
					<xsl:when test="selected='true'">
						<xsl:attribute name="class">EktronProductListPageLink_Selected</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="class">EktronProductListPageLink</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
				<xsl:attribute name="onclick" >Ektron_Ecommerce_ProductListClass.GetObject('<xsl:value-of select="$ControlId" />').GetPage('<xsl:value-of select="id"/>'); return (false);</xsl:attribute>				
				<xsl:value-of select="id"/>
			</a>
		</span>
	</xsl:template>

  <xsl:template match="analyticsInfo">
    <img id="ri1" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;0"><xsl:value-of select="/root/appPath"/>images/ui/icons/starLeft.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;1"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyLeft.png</xsl:when>
        </xsl:choose>
      </xsl:attribute>
    </img>
    <img id="ri2" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;1"><xsl:value-of select="/root/appPath"/>images/ui/icons/starRight.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;2"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyRight.png</xsl:when>
        </xsl:choose>
      </xsl:attribute>
    </img>
    <img id="ri3" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;2"><xsl:value-of select="/root/appPath"/>images/ui/icons/starLeft.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;3"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyLeft.png</xsl:when>
        </xsl:choose>
      </xsl:attribute>
    </img>
    <img id="ri4" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;3"><xsl:value-of select="/root/appPath"/>images/ui/icons/starRight.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;4"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyRight.png</xsl:when>
        </xsl:choose>
      </xsl:attribute>
    </img>
    <img id="ri5" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;4"><xsl:value-of select="/root/appPath"/>images/ui/icons/starLeft.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;5"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyLeft.png</xsl:when>
            </xsl:choose>
      </xsl:attribute>
    </img>
    <img id="ri6" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;5"><xsl:value-of select="/root/appPath"/>images/ui/icons/starRight.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;6"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyRight.png</xsl:when>
        </xsl:choose>
      </xsl:attribute>
    </img>
    <img id="ri7" alt="">
      <xsl:attribute name="src">
        <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;6"><xsl:value-of select="/root/appPath"/>images/ui/icons/starLeft.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;7"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyLeft.png</xsl:when>
            </xsl:choose>
          </xsl:attribute>
        </img>
        <img id="ri8" alt="">
          <xsl:attribute name="src">
            <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;7"><xsl:value-of select="/root/appPath"/>images/ui/icons/starRight.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;8"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyRight.png</xsl:when>
            </xsl:choose>
          </xsl:attribute>
        </img>
        <img id="ri9" alt="">
          <xsl:attribute name="src">
            <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;8"><xsl:value-of select="/root/appPath"/>images/ui/icons/starLeft.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;9"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyLeft.png</xsl:when>
            </xsl:choose>
          </xsl:attribute>
        </img>
        <img id="ri10" alt="">
          <xsl:attribute name="src">
            <xsl:choose>
              <xsl:when test="contentRatingAverage&gt;9"><xsl:value-of select="/root/appPath"/>images/ui/icons/starRight.png</xsl:when>
              <xsl:when test="contentRatingAverage&lt;10"><xsl:value-of select="/root/appPath"/>images/ui/icons/starEmptyRight.png</xsl:when>
        </xsl:choose>
      </xsl:attribute>
    </img>
  </xsl:template>
  
	<xsl:template name="MakeFilteringUI">
		<div class="EktronProductListFiltering">
			<xsl:value-of select="/root/resourceStrings/sortBy"/><span class="spacingContainer"> </span>
			<select >
				<xsl:attribute name="onchange">Ektron_Ecommerce_ProductListClass.GetObject('<xsl:value-of select="$ControlId" />').FilterResultsSelect(this); return (false);</xsl:attribute>
				<xsl:apply-templates select="/root/filtering/filters/filter" />
			</select>
		</div>
	</xsl:template>

	<xsl:template match="filterlink">
		<a>
			<xsl:attribute name="onclick">Ektron_Ecommerce_ProductListClass.GetObject('<xsl:value-of select="$ControlId" />').FilterResults(<xsl:value-of select="id"/>); return (false);</xsl:attribute>
			<xsl:attribute name="href">#</xsl:attribute>
			<xsl:value-of select="name"/>
		</a>&#160;
	</xsl:template>
	
	<xsl:template match="filter">
		<option>
			<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
			<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
			<xsl:value-of select="name"/>
		</option>
	</xsl:template>
	
	<xsl:template name="RenderJavascript">
		<script id="EktronProductListJs" type="text/javascript">
			<xsl:comment xml:space="preserve">
				<![CDATA[
				
				$ektron().ready(function(){
					EktronProductList_initialize();
				});
				
				$ektron(document).mousemove(function(e){
					//get full-sized image
					var	fullImage = $ektron(".fullImage");
					if (fullImage.length > 0)
					{
						//set image position
						EktronProductList_setImagePosition(fullImage, e);
					}
				});
				
				function EktronProductList_initialize() {
					//bind hover to product li's		
					$ektron("div.EktronProductListWrapper ul.productList li").hover(
						function() {
							$ektron(this).addClass("hover");
						},
						function() {
							$ektron(this).removeClass("hover");
						}
					);
				
					//bind hover to product img's		
					$ektron("div.EktronProductListWrapper ul.productList li span.productImage").hover(
						function(e) {
							//create image clone
							var fullImage = $ektron(this).children("img.productImage").clone();
              var noImageSrc = $ektron(this).css("background-image");
              
              noImageSrc = noImageSrc.replace('url(', '');
              noImageSrc = noImageSrc.replace(')', '');
              noImageSrc = noImageSrc.replace('\"','');
              noImageSrc = noImageSrc.replace('\"','');
                          
              if(fullImage[0].src == ""){
                fullImage[0].setAttribute("src",noImageSrc);
              }
              
              fullImage.removeClass("productImage");
							fullImage.addClass("fullImage");
							
							//append image clone to list item
							$ektron(this).parent().prepend(fullImage);
							
							//set image position
							EktronProductList_setImagePosition(fullImage, e);
													
						},
						function() {
							$ektron(this).parent().children("img.fullImage").remove();
						}
					);
				}
				
				$ektron.addLoadEvent(function() {
					EktronProductList_alignImages();
				});
				
				//align product images center & middle
				function EktronProductList_alignImages() {
					var productImages = $ektron("div.EktronProductListWrapper ul.productList li span.productImage img");
					$ektron.each(productImages, function(){
					    if (this.src.length > 0){
						    var newTop = 0;
						    var newLeft = 0;
    						
						    //get image dimensions
						    var imageHeight = $ektron(this).height();
						    var imageWidth = $ektron(this).width();
    						
						    //get parent dimensions
						    var parentHeight = $ektron(this).parent().height();
						    var parentWidth = $ektron(this).parent().width();
    						
						    //calculate middle
						    if (imageHeight > parentHeight)
							    newTop = "-" + ((imageHeight - parentHeight) / 2) + "px";
						    if (imageHeight < parentHeight)
							    newTop = ((parentHeight - imageHeight) / 2) + "px";
    							
						    //calculate center
						    if (imageWidth > parentWidth)
							    newLeft = "-" + ((imageWidth - parentWidth) / 2) + "px";
						    if (imageWidth < parentWidth) 
							    newLeft = ((parentWidth - imageWidth) / 2) + "px";
    					
						    //set top & left of product image center & middle of parent
						    $ektron(this).css("top", newTop);
						    $ektron(this).css("left", newLeft);
						    $ektron(this).fadeIn("slow");
                        }
					});
				}
				
				//helper functions
				function EktronProductList_setImagePosition(imageObject, e)
				{
					//buffer value in pixels
					var buffer = 18;
					
					//full-sized image top & left values
					var top = e.pageY;
					var left = e.pageX;
					
					//height & widths for image & body
					var imageHeight =  imageObject.height();
					var imageWidth =  imageObject.width();
					var bodyHeight =  $ektron("body").height();
					var bodyWidth =  $ektron("body").width();
					
					//determine image location - from top or from bottom
					if (imageHeight > (bodyHeight - top)){
						top = top - imageHeight - buffer;
					} else {
						top = top + buffer;
					}
					
					//determine image location - from right or from left
					if (imageWidth > (bodyWidth - left)){
						left = left - imageWidth - buffer;
					} else {
						left = left + buffer;
					}
					
					$ektron(".fullImage").css("top", top);
					$ektron(".fullImage").css("left", left);
				}        
				
			// ]]></xsl:comment>
		</script>
	</xsl:template>

</xsl:stylesheet>