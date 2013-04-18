<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<!-- xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable -->
	<xsl:variable name="HrefTarget" >#</xsl:variable>
	<xsl:variable name="AppImagePath" select="/root/applicationImagePath" />
	
	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="/root/dataMode='buildUI'">
				<xsl:call-template name="BuildUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='buildCartFragment'">
				<xsl:call-template name="BuildCartFragment" />
			</xsl:when>
			<xsl:when test="/root/dataMode='buildSavedCartFragment'">
				<xsl:call-template name="BuildSavedCartFragment" />
			</xsl:when>
		</xsl:choose>
	</xsl:template>
		
	<xsl:template name="BuildUI">
		<div class="ektron EktronCartWrapper">
			<xsl:attribute name="id" >EktronCartCtl_<xsl:value-of select="$ControlId"/></xsl:attribute>
			<div>
				<xsl:attribute name="id" >EktronCartCtl_cartContainer_<xsl:value-of select="$ControlId"/></xsl:attribute>
				<xsl:call-template name="BuildCartFragment" />
			</div>
			<div>
				<xsl:attribute name="id" >EktronCartCtl_savedCartContainer_<xsl:value-of select="$ControlId"/></xsl:attribute>
				<xsl:if test="/root/isLoggedIn='false'"><xsl:attribute name="style" >display: none;</xsl:attribute></xsl:if>
				<xsl:call-template name="BuildSavedCartFragment" />
			</div>
			<xsl:apply-templates select="/root/resourceStrings/javascriptResourceStrings" />
		</div>
	</xsl:template>

	<xsl:template name="BuildCartFragment">
		<table class="cartTable" summary="Cart" cellspacing="0">
			<caption><xsl:value-of select="/root/resourceStrings/stringYourShoppingCart"/></caption>
			<thead>
				<tr>
					<xsl:attribute name="class" >rowCartData<xsl:if test="(/root/browser='ie') and (/root/browserMajorVersion='6')"> BrowserIE6</xsl:if></xsl:attribute>
					<th colspan="2">
						<xsl:attribute name="class" >cartName alignLeft<xsl:if test="(/root/browser='ie') and (/root/browserMajorVersion='7')"> BrowserIE7</xsl:if></xsl:attribute>
						<xsl:if test="/root/isLoggedIn='true'">
							<span class="label"><xsl:value-of select="/root/resourceStrings/stringCartLabel"/></span>
							<a name="RenameCartButton" class="button buttonInline greenHover renameCart" >
								<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').RenameCart(); return false;</xsl:attribute>
								<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringRenameCart"/></xsl:attribute>
								<span name="CartName"><xsl:value-of select="/root/cart/name"/></span>
								<img >
									<xsl:attribute name="src" ><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasket.gif</xsl:attribute>
									<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringRenameCart"/></xsl:attribute>
								</img>
							</a>
							<div name="RenameCartUI" class="divButton" style="display: none;">
								<input name="RenameCartField" type="text" >
									<xsl:attribute name="value"><xsl:value-of select="/root/cart/name"/></xsl:attribute>
									<xsl:attribute name="onkeypress" >return CartClass.GetObject('<xsl:value-of select="$ControlId" />').FilterCartNameKeyStrokes(event);</xsl:attribute>
								</input>
								<img >
									<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').RenameCartOk(); return false;</xsl:attribute>
									<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasketOK.gif</xsl:attribute>
									<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringOk"/></xsl:attribute>
									<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringOk"/></xsl:attribute>
								</img>
								<img>
									<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').RenameCartCancel(); return false;</xsl:attribute>
									<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasketCancel.gif</xsl:attribute>
									<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringCancel"/></xsl:attribute>
									<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringCancel"/></xsl:attribute>
								</img>
							</div>						
						</xsl:if>
					</th>
					<th class="cartActions alignRight" colspan="6">
						<div>
							<xsl:attribute name="class" >cartActionsListWrapper<xsl:if test="(/root/browser='ie')"> BrowserIE</xsl:if></xsl:attribute>
							<xsl:if test="/root/cart/empty!='true'">
								<ul class="cartActionsList">
									<li>
										<a class="button buttonLeft deleteCartButton redHover" >
											<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').RemoveAllCartItems(); return false;</xsl:attribute>
											<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
											<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringEmptyCart"/></xsl:attribute>
											<img>
												<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/deleteCart.gif</xsl:attribute>
												<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringEmptyCart"/></xsl:attribute>
											</img>
											<xsl:value-of select="/root/resourceStrings/stringEmptyCart"/>
										</a>
									</li>
								</ul>
							</xsl:if>
						</div>
					</th>
				</tr>
				<tr class="rowColumnHeadings">
					<th> </th>
					<th><xsl:value-of select="/root/resourceStrings/stringCatalogItem"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringSku"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringQuantity"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringRemove"/></th>
					<th class="nowrap"><xsl:value-of select="/root/resourceStrings/stringMsrp"/></th>
					<th class="nowrap"><xsl:value-of select="/root/resourceStrings/stringPrice"/></th>
					<th class="colTotalHeader"><xsl:value-of select="/root/resourceStrings/stringTotal"/></th>
				</tr>
			</thead>
			<tfoot>
				<tr class="rowSubtotal">
					<td colspan="7" class="noBorderTopBottom alignRight"><xsl:value-of select="/root/resourceStrings/stringSubtotalLabel"/> </td>
					<td class="noBackgroundImage ieBorderFix"><xsl:value-of select="/root/cart/subTotal"/></td>
				</tr>
				<xsl:choose>
					<xsl:when test="/root/cart/error='true'">
						<tr class="rowAdvisoryMessage">
							<td class="noBorderTopBottom" colspan="7">
								<span><xsl:value-of select="/root/cart/errorMessage" /></span>
							</td>
						</tr>
					</xsl:when>
					<xsl:otherwise>
						<xsl:if test="/root/cart/warning='true'">
							<tr class="rowAdvisoryMessage">
								<td class="noBorderTopBottom" colspan="7">
									<span><xsl:value-of select="/root/cart/warningMessage" /></span>
								</td>
							</tr>
						</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
				<tr class="rowContinueShopping">
					<td class="continueShopping" colspan="1">
						<a class="button buttonLeft blueHover" >
							<xsl:attribute name="href"><xsl:value-of select="shoppingUrl"/></xsl:attribute>
							<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringContinueShopping"/></xsl:attribute>
							<img>
								<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/continueShopping.gif</xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringContinueShopping"/></xsl:attribute>
							</img>
							<xsl:value-of select="/root/resourceStrings/stringContinueShopping"/>
						</a>
					</td>
					<xsl:if test="/root/cart/empty='true'">
						<td class="checkout emptyCart" colspan="7">
							<span><xsl:value-of select="/root/resourceStrings/stringTheCartIsEmpty"/></span>
						</td>
					</xsl:if>
					<xsl:if test="/root/cart/empty!='true'">
						<td class="checkout alignRight" colspan="7">
							<div class="checkoutActionsWrapper">
								
									<ul class="checkoutActionsList" >
										<li>
											<xsl:if test="(/root/isValid='false')">
												<xsl:attribute name="style">display: none;</xsl:attribute>
											</xsl:if>
											<a class="button buttonRight checkoutCartButton greenHover" >
												<xsl:attribute name="href"><xsl:value-of select="/root/checkoutUrl"/></xsl:attribute>
												<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringCheckout"/></xsl:attribute>
												<img>
													<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/checkout.gif</xsl:attribute>
													<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringCheckout"/></xsl:attribute>
												</img>
												<xsl:value-of select="/root/resourceStrings/stringCheckout"/>
											</a>
										</li>
										<xsl:if test="/root/enableCoupons='true'">
											<li>
												<a name="ApplyCouponButton" class="button buttonRight applyCartButton blueHover" >
													<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').ApplyCoupon(); return false;</xsl:attribute>
													<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
													<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringApplyCoupon"/></xsl:attribute>
													<img>
														<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/coupon.gif</xsl:attribute>
														<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringApplyCoupon"/></xsl:attribute>
													</img>
													<xsl:value-of select="/root/resourceStrings/stringApplyCoupon"/>
												</a>
											</li>
										</xsl:if>
										<li>
											<div name="ApplyCouponUI" style="display: none;">
												<xsl:attribute name="class" >ApplyCouponUI<xsl:if test="(/root/browser='ie') and (/root/browserMajorVersion='6')"> BrowserIE6</xsl:if></xsl:attribute>
												<span><xsl:value-of select="/root/resourceStrings/stringEnterCouponCodeLabel"/> </span>
												<input name="ApplyCouponField" class="ApplyCouponField" type="text" >
													<xsl:attribute name="onkeypress" >return CartClass.GetObject('<xsl:value-of select="$ControlId" />').FilterCouponNameKeyStrokes(event);</xsl:attribute>
												</input>
												<img>
													<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').ApplyCouponOk(); return false;</xsl:attribute>
													<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasketOK.gif</xsl:attribute>
													<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringOk"/></xsl:attribute>
													<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringOk"/></xsl:attribute>
												</img>
												<img>
													<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').ApplyCouponCancel(); return false;</xsl:attribute>
													<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasketCancel.gif</xsl:attribute>
													<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringCancel"/></xsl:attribute>
													<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringCancel"/></xsl:attribute>
												</img>
											</div>
										</li>
										<li>
											<a class="button buttonRight updateCartButton blueHover" >
												<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').UpdateCart(); return false;</xsl:attribute>
												<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
												<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringUpdateSubtotal"/></xsl:attribute>
												<img>
													<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/updateSubtotal.gif</xsl:attribute>
													<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringUpdateSubtotal"/></xsl:attribute>
												</img>
												<xsl:value-of select="/root/resourceStrings/stringUpdateSubtotal"/>
											</a>
										</li>
									</ul>
							</div>
						</td>
					</xsl:if>
				</tr>
        
        <xsl:if test="/root/cart/empty!='true' and (/root/paypalSupported='true' or /root/googleSupported='true')">
        <tr class="rowContinueShopping">
          <td class="checkout alignRight" colspan="8">
              <div class="checkoutActionsWrapper">
                <ul class="checkoutActionsList" >
                  <xsl:if test="/root/paypalSupported='true'">
                    <li>
                      <a class="paypalbutton buttonRight checkoutCartButton greenHover">
                        <xsl:attribute name="href"><xsl:value-of select="/root/applicationPath"/>commerce/Checkout/PayPal/ExpressCheckout.aspx</xsl:attribute>
                        <img border="0" alt="Fast, easy, secure checkout through PayPal"><xsl:attribute name="src">//www.paypal.com/en_US/i/btn/btn_xpressCheckoutsm.gif</xsl:attribute></img>
                      </a>                     
                    </li>
                  </xsl:if>
                  <xsl:if test="/root/googleSupported='true'">
                    <li>
                      <a class="paypalbutton buttonRight checkoutCartButton greenHover">
                        <xsl:attribute name="href"><xsl:value-of select="/root/applicationPath"/>commerce/Checkout/Google/GoogleCheckout.aspx<xsl:if test="/root/shoppingUrl!=''">?shoppingURL=<xsl:value-of select="/root/shoppingUrlEncoded"/></xsl:if></xsl:attribute>
                        <img border="0" alt="Fast checkout through Google"><xsl:attribute name="src">//checkout.google.com/buttons/checkout.gif?w=180&amp;h=46&amp;style=white&amp;variant=text&amp;loc=en_US</xsl:attribute></img>
                      </a>                   
                    </li>
                  </xsl:if>
                </ul>
              </div>
            </td>
        </tr>
        </xsl:if>
        
			</tfoot>
			<tbody>
				<xsl:apply-templates select="/root/cart/cartItems/cartItem" />
				<xsl:apply-templates select="/root/couponItems/couponItem" />
			</tbody>
		</table>
		<input type="hidden" >
			<xsl:attribute name="id">EktronCartCtl_emptyCartFlag_<xsl:value-of select="$ControlId"/></xsl:attribute>
			<xsl:attribute name="value"><xsl:choose><xsl:when test="/root/cart/empty='true'">1</xsl:when><xsl:otherwise>0</xsl:otherwise></xsl:choose></xsl:attribute>
		</input>
		
	</xsl:template>
			
	<xsl:template name="BuildSavedCartFragment">
		<table class="savedCarts" summary="Saved Carts">
			<caption><xsl:value-of select="/root/resourceStrings/stringYourSavedCarts"/></caption>
			<thead>
				<tr>
					<th><xsl:value-of select="/root/resourceStrings/stringCart"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringLastUpdated"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringCatalogItems"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringSubtotal"/></th>
					<th><xsl:value-of select="/root/resourceStrings/stringDelete"/></th>
				</tr>
			</thead>
			<tfoot>
				<tr class="rowContinueShopping">
					<td>
						<a class="button buttonLeft blueHover" >
							<xsl:attribute name="href"><xsl:value-of select="shoppingUrl"/></xsl:attribute>
							<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringContinueShopping"/></xsl:attribute>
							<img>
								<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/continueShopping.gif</xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringContinueShopping"/></xsl:attribute>
							</img>
							<xsl:value-of select="/root/resourceStrings/stringContinueShopping"/>
						</a>
					</td>
					<xsl:if test="/root/savedCarts/empty='true'">
						<td class="checkout noSavedCarts" colspan="2">
							<span><xsl:value-of select="/root/resourceStrings/stringNoSavedCarts"/></span>
						</td>
					</xsl:if>
					<td>
						<xsl:choose>
							<xsl:when test="/root/savedCarts/empty='true'">
								<xsl:attribute name="colspan">2</xsl:attribute>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name="colspan">4</xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<a name="CreateCartButton"  class="button buttonRight greenHover" >
							<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').CreateCart(); return false;</xsl:attribute>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringCreateNewCart"/></xsl:attribute>
							<img>
								<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/createNewBasket.gif</xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringCreateNewCart"/></xsl:attribute>
							</img>
							<xsl:value-of select="/root/resourceStrings/stringCreateNewCart"/>
						</a>
						<div name="CreateCartUI" class="divButton buttonRight" style="display: none;">
							<input name="CreateCartField" type="text" >
								<xsl:attribute name="onkeypress" >return CartClass.GetObject('<xsl:value-of select="$ControlId" />').FilterCartNameKeyStrokes(event);</xsl:attribute>
							</input>
							<img>
								<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').CreateCartOk(); return false;</xsl:attribute>
								<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasketOK.gif</xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringOk"/></xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringOk"/></xsl:attribute>
							</img>
							<img>
								<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').CreateCartCancel(); return false;</xsl:attribute>
								<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/renameBasketCancel.gif</xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringCancel"/></xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringCancel"/></xsl:attribute>
							</img>
						</div>						
					</td>
				</tr>
			</tfoot>
			<tbody>
				<xsl:apply-templates select="/root/savedCarts/savedCart" />
			</tbody>
		</table>
	</xsl:template>

	<xsl:template match="cartItem">
		<tr class="cartItem">
			<xsl:attribute name="class">rowSku<xsl:if test="oddRow='true'"> stripe</xsl:if></xsl:attribute>
			<td class="colItemImage">
				<input class="itemIdText" type="hidden" >
					<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
				</input>
				<input class="productIdText" type="hidden" >
					<xsl:attribute name="value"><xsl:value-of select="productId"/></xsl:attribute>
				</input>
        <xsl:if test="/root/enableImages='true'">
          <a title="View Product" >
					  <xsl:attribute name="href"><xsl:value-of select="link"/></xsl:attribute>
            <img>
              <xsl:attribute name="src"><xsl:choose><xsl:when test="imagethumbnail and not(imagethumbnail='')"><xsl:value-of select="/root/sitePath"/><xsl:value-of select="imagethumbnail"/></xsl:when><xsl:otherwise><xsl:value-of select="/root/appPath"/>images/application/Commerce/productImageBackground.gif</xsl:otherwise></xsl:choose></xsl:attribute>
              <xsl:attribute name="title"><xsl:if test="nameExtended and not(nameExtended='')"> - <xsl:value-of select="nameExtended"/></xsl:if></xsl:attribute>
              <xsl:attribute name="alt"><xsl:if test="nameExtended and not(nameExtended='')"> - <xsl:value-of select="nameExtended"/></xsl:if></xsl:attribute>
            </img>
				  </a>
        </xsl:if>
			</td>
			<td class="colItemName">
        <a title="View Product" >
					<xsl:attribute name="href"><xsl:value-of select="link"/></xsl:attribute>
					<xsl:value-of select="name"/><xsl:if test="nameExtended and not(nameExtended='')"> - <xsl:value-of select="nameExtended"/></xsl:if>
				</a>
				<xsl:for-each select="kitItems/kitItem">
					<div class="colItemNameKitNames"><xsl:value-of select="name"/>: <xsl:value-of select="optionName"/></div>
				</xsl:for-each>
			</td>
			<td class="colProductId"><xsl:value-of select="sku"/></td>
			<td class="colQty">
        <xsl:choose>
          <xsl:when test="productType='4'"><xsl:value-of select="quantity"/></xsl:when>
          <xsl:otherwise>
            <input class="productQtyText" type="text" >
					    <xsl:attribute name="value"><xsl:value-of select="quantity"/></xsl:attribute>
				    </input>
          </xsl:otherwise>
        </xsl:choose>
			</td>
			<td class="colRemove">
				<a class="removeFromCart" >
					<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').RemoveCartItem('<xsl:value-of select="id"/>', '<xsl:value-of select="productId"/>'); return false;</xsl:attribute>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringRemoveFromCart"/></xsl:attribute>
					<img>
						<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/removefromcart2.gif</xsl:attribute>
						<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringRemoveFromCart"/></xsl:attribute>
					</img>
				</a>
			</td>
			<td class="colItemPrice"><xsl:value-of select="itemPrice"/></td>
			<td class="colEarlyPrice"><xsl:value-of select="earlyPrice"/></td>
			<td class="colTotal ieBorderFix"><xsl:value-of select="extendedPrice"/></td>
		</tr>
	</xsl:template>
	
	<xsl:template match="couponItem">
		<xsl:if test="isValid='true'">
			<tr>
				<xsl:attribute name="class">couponItem rowSku<xsl:if test="oddRow='true'"> stripe</xsl:if> applied_<xsl:value-of select="isApplied"/></xsl:attribute>
				<td colspan="7" class="colDiscountCoupon"><xsl:value-of select="/root/resourceStrings/stringCouponCodeLabel"/> <span class="couponCode"><xsl:value-of select="couponCode"/></span>, <xsl:value-of select="/root/resourceStrings/stringDiscountLabel"/> </td>
				<td class="colTotal ieBorderFix">
					<span class="couponDiscountAmount">-<xsl:value-of select="discountAmount"/></span>
					<a class="removeCoupon" >
						<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').RemoveCoupon('<xsl:value-of select="couponCode"/>'); return false;</xsl:attribute>
						<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
						<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringRemoveCoupon"/></xsl:attribute>
						<img>
							<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/removefromcart2.gif</xsl:attribute>
							<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringRemoveCoupon"/></xsl:attribute>
						</img>
					</a>					
				</td>
			</tr>
		</xsl:if>
	</xsl:template>

	<xsl:template match="savedCart">
		<tr class="savedCart">
			<xsl:if test="oddRow='true'"><xsl:attribute name="class">stripe</xsl:attribute></xsl:if>
			<td class="colCartName">
				<a>
					<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').SelectCart('<xsl:value-of select="id"/>');return false;</xsl:attribute>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringViewCart"/></xsl:attribute>
					<xsl:value-of select="name"/><xsl:if test="isSelected='true'"> - <xsl:value-of select="/root/resourceStrings/stringActiveCart"/></xsl:if>
				</a>
			</td>
			<td class="colLastModified"><xsl:value-of select="dateModified"/></td>
			<td class="colCartItems"><xsl:value-of select="itemCount"/></td>
			<td class="colcartSubtotal"><xsl:value-of select="subTotal"/></td>
			<td class="colDeleteSavedCart">
				<xsl:choose>
					<xsl:when test="id=/root/cart/id">&#160;</xsl:when>
					<xsl:otherwise>
						<a class="button buttonRight" >
							<xsl:attribute name="onclick">CartClass.GetObject('<xsl:value-of select="$ControlId"/>').DeletSavedCart('<xsl:value-of select="id"/>'); return false;</xsl:attribute>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="title"><xsl:value-of select="/root/resourceStrings/stringDeleteCart"/></xsl:attribute>
							<img>
								<xsl:attribute name="src"><xsl:value-of select ="$AppImagePath"/>/commerce/deleteCart.gif</xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/stringDeleteCart"/></xsl:attribute>
							</img>
						</a>
					</xsl:otherwise>
				</xsl:choose>
				
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="javascriptResourceStrings">
		<xsl:for-each select="node()[text()]" >
			<input type="hidden" class="javascriptResourceString">
				<xsl:attribute name="id" >EktronCartCtl_<xsl:value-of select="$ControlId"/>_<xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="name" ><xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="value" ><xsl:value-of select="./child::text()"/></xsl:attribute>
			</input>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>