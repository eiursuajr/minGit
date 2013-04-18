<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>
	<xsl:variable name="AppImagePath" select="/root/appImagePath" />

	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="isCallback='true'">
				<xsl:call-template name="BuildUIFragments" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronOrderList">
					<xsl:attribute name="id" >EktronOrderList_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="BuildUIFragments" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="BuildUIFragments">
		<xsl:choose>
			<xsl:when test="/root/dataMode='orderStatus'">
				<xsl:call-template name="orderStatus" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="orderList" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- List all of the current customers orders -->
	<xsl:template name="orderList">
    		<div class="ektron EktronOrderHistoryWrapper">
				<table class="orderHistory">
					<xsl:attribute name="summary"><xsl:value-of select="/root/resourceStrings/orderHistory"/></xsl:attribute>
					<caption>
						<span class="emphasize"><xsl:value-of select="/root/resourceStrings/orderHistory"/></span>
						<span class="understate"><xsl:value-of select="/root/resourceStrings/viewOrdersStatusTrackPackages"/></span>
					</caption>
					<thead>
						<tr class="rowColumnHeadings">
							<th><xsl:value-of select="/root/resourceStrings/orderDate"/></th>
							<th><xsl:value-of select="/root/resourceStrings/confirmationNumber"/></th>
							<th><xsl:value-of select="/root/resourceStrings/statusLabel"/></th>
						</tr>
					</thead>
					<tfoot>
						<tr class="rowPaging">
							<td colspan="4">
								<ul>
									<xsl:choose>
										<xsl:when test="paging/enablePagingPrevious='true'">
											<li>
												<a title="View Previous Pages">
													<xsl:attribute name="href"><xsl:value-of select="paging/previousPageLink"/></xsl:attribute>
													<img alt="View Previous Page"><xsl:attribute name="src"><xsl:value-of select="$AppImagePath"/>/commerce/previous.gif</xsl:attribute></img>
												</a>
											</li>
										</xsl:when>
										<xsl:otherwise>
											<li>
												<img alt="View Previous Page" ><xsl:attribute name="src"><xsl:value-of select="$AppImagePath"/>/commerce/previousDisabled.gif</xsl:attribute></img>
											</li>
										</xsl:otherwise>
									</xsl:choose>
									<xsl:apply-templates select="paging/pages/page" />
									<xsl:choose>
										<xsl:when test="paging/enablePagingNext='true'">
											<li>
												<a title="View Next Pages">
													<xsl:attribute name="href"><xsl:value-of select="paging/nextPageLink"/></xsl:attribute>
													<img alt="View Next Page" title="View Next Page" ><xsl:attribute name="src"><xsl:value-of select="$AppImagePath"/>/commerce/next.gif</xsl:attribute></img>
												</a>
											</li>
										</xsl:when>
										<xsl:otherwise>
											<li>
												<img alt="View Next Page" title="View Next Page" ><xsl:attribute name="src"><xsl:value-of select="$AppImagePath"/>/commerce/nextDisabled.gif</xsl:attribute></img>
											</li>
										</xsl:otherwise>
									</xsl:choose>
								</ul>
							</td>
						</tr>
					</tfoot>
					<tbody>
						<tr>
							<td><xsl:apply-templates select="/root/orderList/order" /></td>
						</tr>
						
					</tbody>
				</table>

          <xsl:if test="/root/guestOrderView='true'">
          
        <table class="orderHistory">
          <tbody>
            <tr>
              <td colspan="2" class="EktronOrderHistoryGuestMessage">
                <strong><xsl:value-of select="/root/resourceStrings/viewOrderStatus"/></strong>(*<xsl:value-of select="/root/resourceStrings/requiredFields"/>)<br/>
                <xsl:value-of select="/root/resourceStrings/checkMessage"/>
                <xsl:if test="/root/error='true'">
                  <div class="EktronOrderHistoryGuestError">
                    <xsl:value-of select="/root/errorMessage"/>
                  </div>
                </xsl:if>
              </td>
            </tr>
            <tr>
              <td class="EktronOrderHistoryGuestLabel">
                * <label><xsl:value-of select="/root/resourceStrings/orderNumber"/></label>:
              </td>
              <td class="EktronOrderHistoryGuestInput">
                <input type="text" maxlength="30" name="ekGuestOrderNumber" id="ekGuestOrderNumber"/>
              </td>
            </tr>

            <tr>
              <td class="EktronOrderHistoryGuestLabel">
                * <label><xsl:value-of select="/root/resourceStrings/email"/>:</label>
              </td>
              <td class="EktronOrderHistoryGuestInput">
                <input type="text" maxlength="150" name="ekGuestEmailAddress" id="ekGuestEmailAddress"/>
								</td>
            </tr>

            <tr>
              <td></td>
              <td class="EktronOrderHistoryGuestFind">
                <input type="submit" name="VIEWORDER_BUTTON">
                  <xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/viewOrderStatus"/></xsl:attribute>
                  <xsl:attribute name="value"><xsl:value-of select="/root/resourceStrings/viewOrderStatus"/></xsl:attribute>
                </input>
              </td>
            </tr>
          </tbody>
        </table>
          
          </xsl:if>
          
			</div>
	</xsl:template>

	<!-- Helper template; List the orders -->
	<xsl:template match="order">
		<tr>
			<xsl:attribute name="class">rowSku<xsl:if test="oddRow='true'"> stripe</xsl:if></xsl:attribute>
			<td class="colOrderData"><xsl:value-of select="DateCreated/ToLongDateString" /></td>
			<td class="colConfirmationNumber">
				<a title="View Confirmation" >
					<xsl:attribute name="href"><xsl:value-of select="orderLink"/></xsl:attribute>
					<xsl:value-of select="orderId" />
				</a>
			</td>
			<td class="colStatus"><xsl:value-of select="status" /></td>
		</tr>
	</xsl:template>
		
	<!-- Show the details of a specific order -->
	<xsl:template name="orderStatus">
		<div class="ektron EktronOrderStatusWrapper">
		  <h2><xsl:value-of select="/root/resourceStrings/orderStatus"/></h2>
		  <xsl:if test ="/root/HasDefaultOrderId='false'">
             <div class="viewAll">
                 <div>
                  <a class="button viewAllOrders greenHover" title="View All Orders" onclick="if (window.back) window.back(); else window.history.back(); return false;" >
                     <xsl:attribute name="href">
                       <xsl:value-of select="paging/previousPageLink"/>
                     </xsl:attribute>
                    <img alt="View All Orders" title="View All Orders" >
                      <xsl:attribute name="src">
                        <xsl:value-of select="$AppImagePath"/>/commerce/viewAllOrders.gif
                      </xsl:attribute>
                    </img>
                      <xsl:value-of select="/root/resourceStrings/viewAllOrders"/>
                   </a>
                 </div>
              </div>
           </xsl:if>
		   <table class="orderInfo" summary="Order Information">
				<tbody>
					<tr>
						<th><xsl:value-of select="/root/resourceStrings/statusLabel"/></th>
						<td><xsl:value-of select="/root/orderStatus/status"/></td>
					</tr>
					<tr>
						<th><xsl:value-of select="/root/resourceStrings/orderDateLabel"/></th>
						<td><xsl:value-of select="/root/orderStatus/dateCreated/ToLongDateString" /></td>
					</tr>
					<tr>
						<th><xsl:value-of select="/root/resourceStrings/orderNumberLabel"/></th>
						<td><xsl:value-of select="/root/orderStatus/orderId"/></td>
					</tr>
				</tbody>
			</table>
			<xsl:apply-templates select="/root/orderStatus/parts/part" />
		</div>
	</xsl:template>

	<!-- Helper template; List each order item -->
	<xsl:template match="part">
		<fieldset>
			<xsl:if test="/root/orderStatus/returnPeriodExpired='true'">
				<p class="notice">
					<xsl:value-of select="/root/resourceStrings/returnPeriodExpired"/>
				</p>
			</xsl:if>
			<table class="shippingBilling" summary="Shipping and Billing Information">
				<thead>
					<tr>
						<th><xsl:value-of select="/root/resourceStrings/shipToLabel"/></th>
						<th><xsl:value-of select="/root/resourceStrings/billToLabel"/></th>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td>
							<ul>
								<li><xsl:value-of select="shippingAddress/shippingInfo/name"/></li>
								<xsl:if test="shippingAddress/shippingInfo/company!=''">
									<li><xsl:value-of select="shippingAddress/shippingInfo/company"/></li>
								</xsl:if>
								<li><xsl:value-of select="shippingAddress/shippingInfo/address1"/></li>
								<xsl:if test="shippingAddress/shippingInfo/company!=''">
									<li><xsl:value-of select="shippingAddress/shippingInfo/address2"/></li>
								</xsl:if>
								<li><xsl:value-of select="shippingAddress/shippingInfo/city"/>, <xsl:value-of select="shippingAddress/shippingInfo/region/name"/></li>
								<li><xsl:value-of select="shippingAddress/shippingInfo/country/name"/></li>
								<li><xsl:value-of select="shippingAddress/shippingInfo/postalCode"/></li>
								<li>&#160;</li>
								<xsl:if test="shipMethod!=''"><li>Via <xsl:value-of select="shipMethod" /></li></xsl:if>
								<xsl:if test="trackingNumber!=''"><xsl:if test="trackingSupported='true'"><br/><xsl:value-of select="/root/resourceStrings/trackingNumber"/><br/><a target="_blank"><xsl:attribute name="href"><xsl:value-of select="trackingURL" /></xsl:attribute><xsl:value-of select="trackingNumber" /></a></xsl:if><xsl:if test="trackingSupported='false'"><br/><xsl:value-of select="/root/resourceStrings/trackingNumber"/><br/><xsl:value-of select="trackingNumber" /></xsl:if></xsl:if>
								<!-- li><xsl:value-of select="shippingAddress/shippingInfo/"/><span style="color: red;">TODO: Get Real Ship-To Data</span></li>
								<li><xsl:value-of select="shippingAddress/shippingInfo/"/></li>
								<li>Method: <a href="#trackingNumber" class="openNewWindow" onclick="alert('Track Shipment (Opens New Window)');return false" title="Track Shipment (Opens New Window)"><span style="color: red;">TODO</span></a></li>
								<li>Date Shipped: <span style="color: red;">TODO</span></li -->
							</ul>
						</td>
						<td>
							<ul>
								<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/name"/></li>
								<xsl:if test="/root/orderStatus/last4!=''">
									<li>************<xsl:value-of select="/root/orderStatus/last4"/></li>
								</xsl:if>
								<xsl:if test="/root/orderStatus/billingAddress/billingInfo/company!=''">
									<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/company"/></li>
								</xsl:if>
								<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/address1"/></li>
								<xsl:if test="/root/orderStatus/billingAddress/billingInfo/company!=''">
									<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/address2"/></li>
								</xsl:if>
								<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/city"/>, <xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/region/name"/></li>
								<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/country/name"/></li>
								<li><xsl:value-of select="/root/orderStatus/billingAddress/billingInfo/postalCode"/></li>
							</ul>
						</td>
					</tr>
				</tbody>
			</table>
			<table class="orderDetails" summary="Order Details">
				<thead>
					<tr>
						<th><xsl:value-of select="/root/resourceStrings/description"/></th>
						<th><xsl:value-of select="/root/resourceStrings/quantity"/></th>
						<th><xsl:value-of select="/root/resourceStrings/salePrice"/></th>
						<th><xsl:value-of select="/root/resourceStrings/total"/></th>
					</tr>
				</thead>
				<tfoot>
					<xsl:if test="/root/orderStatus/couponUsed='true'">
						<tr>
							<th colspan="3"><xsl:value-of select="/root/resourceStrings/totalCouponDiscountLabel"/></th>
							<td class="priceColumn">(<xsl:value-of select="/root/orderStatus/couponTotal"/>)</td>
						</tr>
					</xsl:if>
					<tr>
						<th colspan="3"><xsl:value-of select="/root/resourceStrings/subtotalLabel"/></th>
						<td class="priceColumn emphasis"><xsl:value-of select="subTotal"/></td>
					</tr>
					<tr class="stripe">
						<th colspan="3"><xsl:value-of select="/root/resourceStrings/taxLabel"/></th>
						<td class="priceColumn"><xsl:value-of select="taxCharge"/></td>
					</tr>
					<tr>
						<th colspan="3"><xsl:value-of select="/root/resourceStrings/shippingLabel"/></th>
						<td class="priceColumn"><xsl:value-of select="shippingAndHandlingCharge"/></td>
					</tr>
					<xsl:if test ="/root/orderStatus/ApplyTaxestoShipping = 'true'">
                    <tr>
                        <th colspan="3"><xsl:value-of select="/root/resourceStrings/shippingtax"/>:</th>
                        <td class="priceColumn"><xsl:value-of select="shippingtaxes"/></td>
                    </tr>
                    </xsl:if>
					<tr class="stripe">
						<th colspan="3" class="bottomBorder"><xsl:value-of select="/root/resourceStrings/totalLabel"/></th>
						<td class="priceColumn emphasis"><xsl:value-of select="/root/orderStatus/orderTotal"/></td>
					</tr>
				</tfoot>
				<tbody>
					<xsl:apply-templates select="lines/line" />
				</tbody>
			</table>
		</fieldset>
	</xsl:template>	
	
	<!-- Helper template; List each order item -->
	<xsl:template match="line">
		
		<tr>
			<xsl:if test="oddRow='true'"><xsl:attribute name="class">stripe</xsl:attribute></xsl:if>
			<td class="item">
				<xsl:choose>
					<xsl:when test="templateProduct and not(templateProduct='') and dynamicProductParameter and not(dynamicProductParameter='')">
						<a class="productDetailsLink">
							<xsl:attribute name="href"><xsl:value-of select="templateProduct"/>?<xsl:value-of select="dynamicProductParameter"/>=<xsl:value-of select="productId"/></xsl:attribute>
							<xsl:value-of select="productName"/>
							<xsl:if test="nameExtended and not(nameExtended='')"> - <xsl:value-of select="nameExtended"/></xsl:if>
						</a>
					</xsl:when>
					<xsl:otherwise>
						<a class="productDetailsLink">
							<xsl:attribute name="href"><xsl:value-of select="quickLink"/></xsl:attribute>
							<xsl:value-of select="productName"/>
							<xsl:if test="nameExtended and not(nameExtended='')"> - <xsl:value-of select="nameExtended"/></xsl:if>
						</a>
					</xsl:otherwise>
				</xsl:choose>
				<!-- <sup><a title="Return and Exchange Policy" href="policy1">1</a></sup> -->
				<!--<p class="sku">Part Number: <xsl:value-of select="productId"/></p>-->
				<xsl:for-each select="configuration/option">
					<br/>&#160;&#160;&#160;<xsl:value-of select="groupname"/>: <xsl:value-of select="groupoptionname"/>
				</xsl:for-each>
			</td>
			<td><xsl:value-of select="quantity"/></td>
			<td class="priceColumn"><xsl:value-of select="priceEach"/></td>
			<td class="priceColumn"><xsl:value-of select="priceSubTotal"/></td>
		</tr>
	</xsl:template>
	
	<!-- Helper template; list pages, for paging -->
	<xsl:template match="page">
		<li>
			<xsl:choose>
				<xsl:when test="active='false'">
					<a class="pageNumberLink">
						<xsl:attribute name="href"><xsl:value-of select="../../pageLink"/><xsl:value-of select="id"/></xsl:attribute>
						<xsl:value-of select="id"/>
					</a>
				</xsl:when>
				<xsl:otherwise>
					<span class="pageNumber"><xsl:value-of select ="id"/></span>
				</xsl:otherwise>
			</xsl:choose>
		</li>
	</xsl:template>
	
</xsl:stylesheet>