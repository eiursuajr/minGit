<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="Checkout_Login.xsl" />
	<xsl:import href="Checkout_BillingInfoEntry.xsl" />
	<xsl:import href="Checkout_BillingInfo.xsl" />
	<xsl:import href="Checkout_ShippingInfo.xsl" />
	<xsl:import href="Checkout_ShippingMethod.xsl" />
	<xsl:import href="Checkout_OrderReview.xsl" />
	<xsl:import href="Checkout_PlaceOrder.xsl" />
	<xsl:import href="Checkout_OrderComplete.xsl" />
  <xsl:import href="Checkout_EditAddress.xsl"/>
	<xsl:import href="Checkout_EmptyBasket.xsl" />
	<xsl:import href="Checkout_OnePageUI.xsl" />
	
	<xsl:variable name="ControlId" select="/root/controlId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>

	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="/root/enableWizardMode='false'">
				<div class="EktronCheckout">
					<xsl:attribute name="id" >EktronCheckout_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<div class="EktronCheckout_FullPageUIMode">
						<xsl:choose>
							<xsl:when test="/root/dataMode='Error_UnhandledException' or /root/dataMode='Error_Unknown'">
								<xsl:call-template name="MakeErrorUI" />
							</xsl:when>
							<xsl:when test="/root/dataMode='Complete'">
								<xsl:call-template name="MakeOrderCompleteUI" />
							</xsl:when>
							<xsl:when test="/root/dataMode='Error_EmptyBasket'">
								<xsl:call-template name="MakeEmptyBasketUI" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:call-template name="MakeFullUI" />
							</xsl:otherwise>
						</xsl:choose>
					</div>
				</div>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="isCallback='true'">
						<xsl:call-template name="BuildWizardFragments" />
					</xsl:when>
					<xsl:otherwise>
						<div class="EktronCheckout">
							<xsl:attribute name="id" >EktronCheckout_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<div class="EktronCheckout_WizardMode">
								<xsl:call-template name="BuildWizardFragments" />
							</div>
							<xsl:apply-templates select="/root/resourceStrings/javascriptResourceStrings" />
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="BuildWizardFragments">
		<xsl:choose>
			<xsl:when test="/root/dataMode='Login'">
				<xsl:call-template name="MakeLoginUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='BillingInfoEntry'">
				<xsl:call-template name="MakeBillingInfoEntryUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='BillingInfo'">
				<xsl:call-template name="MakeBillingInfoUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='ShippingInfo'">
				<xsl:call-template name="MakeShippingInfoUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='ShippingMethodSelect'">
				<xsl:call-template name="MakeShippingMethodSelectUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='ReviewOrder'">
				<xsl:call-template name="MakeReviewOrderUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='SubmitOrder'">
				<xsl:call-template name="MakeSubmitOrderUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='Complete'">
				<xsl:call-template name="MakeOrderCompleteUI" />
			</xsl:when>
      <xsl:when test="/root/dataMode='EditAddress'">
        <xsl:call-template name="MakeEditAddressUI" />
      </xsl:when>
			<xsl:when test="/root/dataMode='Error_EmptyBasket'">
				<xsl:call-template name="MakeEmptyBasketUI" />
			</xsl:when>
			<xsl:when test="/root/dataMode='Error_UnhandledException' or /root/dataMode='Error_Unknown'">
				<xsl:call-template name="MakeErrorUI" />
			</xsl:when>
			<!-- Subfragment Creation -->
			<xsl:when test="/root/dataMode='MakeSubfragment_Regions'">
				<xsl:call-template name="AjaxCallbackGetRegions" />
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="AjaxCallbackGetRegions">
		<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>		
		<xsl:apply-templates select="/root/regions/region" />		
	</xsl:template>

	<xsl:template match="region">
		<xsl:if test="enabled='true'">
			<option>
				<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
				<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
				<xsl:value-of select="name"/>
			</option>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="country">
		<xsl:if test="enabled='true'">
			<option>
				<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
				<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
				<xsl:value-of select="name"/>
			</option>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="shippingAddress">
		<!--<option>
			<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
			<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
			<xsl:value-of select="dropDownText"/>
		</option>-->
    <ul class="EktronCheckout_ShippingAddress_Address">
      <xsl:attribute name="class">EktronCheckout_ShippingAddress_Address<xsl:if test="isCurrentShipping='True'"> EktronCheckout_ShippingAddress_AddressDefault</xsl:if></xsl:attribute>
      <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_Select">
        <span class="EktronCheckout_UserInfo_ShippingAddress_MultipleAddressSelect_selectAddress EktronCheckout_MutableControlContainer" >
          <xsl:choose><xsl:when test="isCurrentShipping='True'"><a>
            <xsl:attribute name="class">EktronCheckout_message EktronCheckout_messageLeft</xsl:attribute>
            <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					  <xsl:attribute name="onclick" >return (false);</xsl:attribute>
            <img><xsl:attribute name="src"><xsl:value-of select="/root/appPath"/>images/UI/icons/package.png</xsl:attribute><xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/currentAddress"/></xsl:attribute></img>
            <xsl:value-of select="/root/resourceStrings/currentAddress"/>
          </a></xsl:when><xsl:otherwise><a>
            <xsl:attribute name="class">EktronCheckout_button EktronCheckout_buttonLeft</xsl:attribute>
            <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					  <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SelectShippingAddress(<xsl:value-of select="id"/>); return (false);</xsl:attribute>
            <img><xsl:attribute name="src"><xsl:value-of select="/root/appPath"/>images/UI/icons/package.png</xsl:attribute><xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/shipToThisAddress"/></xsl:attribute></img>
            <xsl:value-of select="/root/resourceStrings/shipToThisAddress"/>
          </a></xsl:otherwise></xsl:choose>
				</span>
      </li>
      <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_Name">
			  <xsl:value-of select="name"/>
		  </li>
		  <!--<xsl:if test="not(company='')">
			  <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_Company">
				  <xsl:value-of select="company"/>
			  </li>
		  </xsl:if>-->
		  <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_StreetAddress">
			  <xsl:value-of select="addressLine1"/>
			  <xsl:if test="not(addressLine2='')">, <xsl:value-of select="addressLine2"/></xsl:if>
		  </li>
		  <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_CityState">
			  <xsl:value-of select="city"/>,
			  <xsl:value-of select="regionCode"/><xsl:text> </xsl:text>
        <xsl:value-of select="postalCode"/>
		  </li>
		  <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_Country">
			  <xsl:value-of select="country"/>
		  </li>
		  <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_Phone">
			  <xsl:value-of select="phone1"/> 
		  </li>
      <li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_Select">
        <xsl:if test="/root/shippingInfo/hasMultipleAddresses='true'">
          <xsl:if test="isDefaultBilling='False'">
            <span class="EktronCheckout_UserInfo_ShippingAddress_MultipleAddressSelect_selectAddress EktronCheckout_MutableControlContainer" >
              <a class="EktronCheckout_button EktronCheckout_buttonLeft">
                <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					      <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').EditAddress('<xsl:value-of select="position()"/>'); return (false);</xsl:attribute>
                <img><xsl:attribute name="src"><xsl:value-of select="/root/appPath"/>images/UI/icons/contentEdit.png</xsl:attribute><xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/edit"/></xsl:attribute></img>
                <xsl:value-of select="/root/resourceStrings/edit"/>
              </a>
            </span>
            <span class="EktronCheckout_SpacerLeft">&#160;</span>
            <span class="EktronCheckout_UserInfo_ShippingAddress_MultipleAddressSelect_selectAddress EktronCheckout_MutableControlContainer" >
              <a class="EktronCheckout_button EktronCheckout_buttonLeft">
                <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					      <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').DeleteAddress(<xsl:value-of select="id"/>); return (false);</xsl:attribute>
                <img><xsl:attribute name="src"><xsl:value-of select="/root/appPath"/>images/UI/icons/delete.png</xsl:attribute><xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/deleteThisAddress"/></xsl:attribute></img>
                <xsl:value-of select="/root/resourceStrings/deleteThisAddress"/>
              </a>
            </span>
          </xsl:if>
          <xsl:if test="isDefaultBilling='True'"><a>
            <xsl:attribute name="class">EktronCheckout_message2 EktronCheckout_messageLeft</xsl:attribute>
            <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					  <xsl:attribute name="onclick" >return (false);</xsl:attribute>
            <img><xsl:attribute name="src"><xsl:value-of select="/root/appPath"/>images/UI/icons/package.png</xsl:attribute><xsl:attribute name="alt"><xsl:value-of select="/root/resourceStrings/currentBillToAddress"/></xsl:attribute></img>
            <xsl:value-of select="/root/resourceStrings/currentBillToAddress"/>
          </a></xsl:if>
        </xsl:if>
      </li>
    </ul>
	</xsl:template>
	
	<xsl:template match="year">
		<option>
			<xsl:attribute name="value"><xsl:value-of select="./text()"/></xsl:attribute>
			<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
			<xsl:value-of select="./text()"/>
		</option>
	</xsl:template>

	<xsl:template match="cardType">
		<option>
			<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
			<xsl:attribute name="data-ektron-cardRegex"><xsl:value-of select="regex"/></xsl:attribute>
			<xsl:attribute name="data-ektron-cardType"><xsl:value-of select="type"/></xsl:attribute>
      <xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
			<xsl:value-of select="name"/>
		</option>
	</xsl:template>
	
	<xsl:template match="methodType">
		<tr class="EktronCheckout_ShippingMethod_ServiceSelectRow">
			<td class="EktronCheckout_ShippingMethod_ServiceSelectButton">
				<input type="radio" class="EktronCheckout_ShippingMethod_ServiceSelect" >
					<xsl:attribute name="name">EktronCheckout_ShippingMethod_MethodSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:attribute name="id">EktronCheckout_ShippingMethod_MethodSelect_<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
					<xsl:if test="selected='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
				</input>
			</td>
			<td class="EktronCheckout_ShippingMethod_ServiceSelectName">
				<xsl:value-of select="name"/>
			</td>
			<td class="EktronCheckout_ShippingMethod_ServiceSelectCharge">
				<xsl:value-of select="charge"/>
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template name="MakeErrorUI">
		<h3><b>Error Occurred</b></h3>
		<p class="EktronCheckout_ErrorText"><b><xsl:value-of select="/root/errorMessage"/></b></p>
	</xsl:template>
	
	<xsl:template match="javascriptResourceStrings">
		<xsl:for-each select="node()[text()]" >
			<input type="hidden" class="javascriptResourceString">
				<xsl:attribute name="id" >EktronCheckout_<xsl:value-of select="$ControlId"/>_<xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="name" ><xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="value" ><xsl:value-of select="./child::text()"/></xsl:attribute>
			</input>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>