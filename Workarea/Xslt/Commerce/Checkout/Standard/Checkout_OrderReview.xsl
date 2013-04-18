<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeReviewOrderUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeReviewOrderUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_ReviewOrder EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_ReviewOrder_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeReviewOrderUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeReviewOrderUIContents">
		<div class="wizardStep">
			<div class="past">
				<div class="bluechecked">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=BillingInfo</xsl:attribute><xsl:value-of select="/root/resourceStrings/billing"/></a>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >past <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="bluechecked">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=ShippingInfo</xsl:attribute><xsl:value-of select="/root/resourceStrings/shipping"/></a>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >past <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="bluechecked">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=ShippingMethodSelect</xsl:attribute><xsl:value-of select="/root/resourceStrings/method"/></a>
				</div>
			</div>
			<div class="present">
				<div class="blue">
					<xsl:value-of select="/root/resourceStrings/review"/>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >future <xsl:if test="isZeroCost='true'">hiddenStep</xsl:if></xsl:attribute>
				<div class="grey">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=SubmitOrder</xsl:attribute><xsl:value-of select="/root/resourceStrings/payment"/></a>
				</div>
			</div>
			<div class="wizardEnd"> </div>
		</div>

		<table class="EktronCheckout_ReviewOrder_OrderListing">
			<thead>
				<tr>
					<th><xsl:value-of select="/root/resourceStrings/productDescription"/></th>
					<th><xsl:value-of select="/root/resourceStrings/quantity"/></th>
					<th><xsl:value-of select="/root/resourceStrings/total"/></th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="/root/reviewOrderInfo/orderItems/orderItem" />
			</tbody>
		</table>
		<table class="EktronCheckout_ReviewOrder_OrderCharges">
			<tbody>
				<tr class="ChargesSubTotal">
					<td><xsl:value-of select="/root/resourceStrings/subTotal"/></td>
					<td><xsl:value-of select="/root/reviewOrderInfo/subTotal"/></td>
				</tr>
				<tr class="ChargesShipping">
					<td><xsl:value-of select="/root/resourceStrings/shipping"/></td>
					<td><xsl:value-of select="/root/reviewOrderInfo/shipping"/></td>
				</tr>
				<xsl:if test="/root/reviewOrderInfo/hasDiscount='true'">
					<tr class="ChargesDiscount">
						<td><xsl:value-of select="/root/resourceStrings/discount"/></td>
						<td>-<xsl:value-of select="/root/reviewOrderInfo/discount"/></td>
					</tr>
				</xsl:if>
				<tr class="ChargesTax">
					<td><xsl:value-of select="/root/resourceStrings/tax"/></td>
					<td><xsl:value-of select="/root/reviewOrderInfo/tax"/></td>
				</tr>
				<xsl:if test="/root/reviewOrderInfo/ApplyTaxestoShipping = 'true'">
                <tr class="ChargesTax">
                    <td><xsl:value-of select="/root/resourceStrings/shippingtax"/></td>
                    <td> <xsl:value-of select="/root/reviewOrderInfo/shippingtaxestotal"/></td>
                </tr>
               </xsl:if>
				<tr class="ChargesTotal">
					<td><xsl:value-of select="/root/resourceStrings/total"/></td>
					<td><xsl:value-of select="/root/reviewOrderInfo/total"/></td>
				</tr>
			</tbody>
		</table>

		<a class="EktronCheckout_ReviewOrder_EditCartBtn" >
			<xsl:attribute name="href"><xsl:value-of select="/root/urlBasket"/></xsl:attribute>
			<xsl:value-of select="/root/resourceStrings/editYourCart"/>
		</a>

		<div class="AdvisoryMessage">
			<xsl:if test="/root/cartValidationErrorMessage and /root/cartValidationErrorMessage!=''">
				<xsl:attribute name="style">display: block;</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="/root/cartValidationErrorMessage"/>
		</div>
		
		<div class="EktronCheckout_ReviewOrder EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
			<span class="EktronCheckout_UserInfo_PreviousPageLink">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').PreviousPage(); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/previousPage"/>
				</a>
			</span>
      <xsl:choose >
        <xsl:when test="(basketHasTangibleItems='false') and (isZeroCost='true')">
			    <input class="EktronCheckout_SubmitOrderBtn" type="button" >
			      <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').NextPage(); return (false);</xsl:attribute>
				    <xsl:attribute name="value"><xsl:value-of select="/root/resourceStrings/submitOrder"/></xsl:attribute>
			    </input>
        </xsl:when>
        <xsl:otherwise>
		      <span class="EktronCheckout_UserInfo_NextPageLink">
			      <xsl:if test="/root/cartValidationErrorMessage and /root/cartValidationErrorMessage!=''">
				      <xsl:attribute name="style">display: none;</xsl:attribute>
			      </xsl:if>
			      <a>
				      <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
				      <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').NextPage(); return (false);</xsl:attribute>
				      <xsl:value-of select="/root/resourceStrings/nextPage"/>
			      </a>
		      </span>
        </xsl:otherwise>
      </xsl:choose>
			<span class="EktronCheckout_AjaxBusyImageContainer">
				<img alt="">
					<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
				</img>
			</span>
		</div>
	</xsl:template>

	<xsl:template match="orderItem">
		<tr>
			<xsl:if test="altRow"><xsl:attribute name="class">EktronCheckout_AltRow</xsl:attribute></xsl:if>
			<td class="EktronCheckout_ReviewOrder_ItemDescription">
				<xsl:value-of select="skuName"></xsl:value-of>
				<xsl:if test="skuAuxullaryName!=''">
					<xsl:text> </xsl:text> - <xsl:value-of select="skuAuxullaryName"></xsl:value-of>
				</xsl:if>
				<xsl:for-each select="kitItems/kitItem">
					<div class="colItemNameKitNames"><xsl:value-of select="name"/>: <xsl:value-of select="optionName"/></div>
				</xsl:for-each>
			</td>
			<td class="EktronCheckout_ReviewOrder_ItemQuantity">
				<xsl:value-of select="quantity"></xsl:value-of>
			</td>
			<td class="EktronCheckout_ReviewOrder_ItemPrice">
				<xsl:value-of select="adjustedPrice"></xsl:value-of>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>