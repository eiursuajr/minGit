<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeBillingInfoUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeBillingInfoUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_BillingInfo EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_BillingInfo_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeBillingInfoUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeBillingInfoUIContents">
		<div class="wizardStep">
			<div class="present">
				<div class="blue">
					<xsl:value-of select="/root/resourceStrings/billing"/>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >future <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="grey">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=ShippingInfo</xsl:attribute><xsl:value-of select="/root/resourceStrings/shipping"/></a>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >future <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="grey">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=ShippingMethodSelect</xsl:attribute><xsl:value-of select="/root/resourceStrings/method"/></a>
				</div>
			</div>
			<div class="future">
				<div class="grey">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=ReviewOrder</xsl:attribute><xsl:value-of select="/root/resourceStrings/review"/></a>
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

		<div class="EktronCheckout_UserInfo_BillingAddressBlock_Review">
			<ul class="EktronCheckout_Rows_Container">
				<li class="EktronCheckout_BillingNotice">
					<xsl:value-of select="/root/resourceStrings/enterBillingInformationPreventDelays"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_BillingAddress_Name">
					<xsl:value-of select="/root/billingInfo/name"/>
				</li>
				<xsl:if test="not(/root/billingInfo/company='')">
					<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_BillingAddress_Company">
						<xsl:value-of select="/root/billingInfo/company"/>
					</li>
				</xsl:if>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_BillingAddress_EmailAddress">
					<xsl:value-of select="/root/userInfo/emailAddress"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_BillingAddress_StreetAddress">
					<xsl:value-of select="/root/billingInfo/address1"/>
					<xsl:if test="not(/root/billingInfo/address2='')">, <xsl:value-of select="/root/billingInfo/address2"/></xsl:if>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_BillingAddress_CityState">
					<xsl:value-of select="/root/billingInfo/city"/>,
					<xsl:value-of select="/root/billingInfo/regionSelected/name"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_BillingAddress_PostalCode">
					<xsl:value-of select="/root/billingInfo/postalCode"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_BillingAddress_Country">
					<xsl:value-of select="/root/billingInfo/countrySelected/name"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_BillingAddress_Phone">
					<xsl:value-of select="/root/billingInfo/phone1"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_BillingAddress_EditLinkContainer EktronCheckout_MutableControlContainer">
					<a>
						<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
						<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').Edit('EktronCheckout_UserInfo_BillingAddressBlock_EditInfo'); return (false);</xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/editInfo"/>
					</a>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
					<span class="EktronCheckout_UserInfo_NextPageLink">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').NextPage(); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/nextPage"/>
						</a>
					</span>
					<span class="EktronCheckout_AjaxBusyImageContainer">
						<img alt="">
							<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
						</img>
					</span>
				</li>
			</ul>
		</div>

		<div>
			<xsl:attribute name="class" >EktronCheckout_UserInfo_BillingAddressBlock_EditInfo EktronCheckout_modalContainer <xsl:if test="/root/billingInfo/validationFailure='true'">EktronCheckout_validationFailure</xsl:if></xsl:attribute>
			<div class="EktronCheckout_UserInfo_BillingAddressBlock_Edit">
				<h4>
					<xsl:value-of select="/root/resourceStrings/editBillingInfo"/>
				</h4>
				<ul class="EktronCheckout_Rows_Container">
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
						</label>
						<input class="EktronCheckout_Name EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/name"/></xsl:attribute>
						</input>
					</li>
					<li class="EktronCheckout_NotRequired EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/company"/>
						</label>
						<input class="EktronCheckout_Company EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/company"/></xsl:attribute>
						</input>
					</li>
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
						</label>
						<input class="EktronCheckout_Address EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/address1"/></xsl:attribute>
						</input>
					</li>
					<li class="EktronCheckout_NotRequired EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						</label>
						<input class="EktronCheckout_AddressLine2 EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/address2"/></xsl:attribute>
						</input>
					</li>
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
						</label>
						<input class="EktronCheckout_City EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/city"/></xsl:attribute>
						</input>
					</li>
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
						</label>
						<select class="EktronCheckout_StateSelect EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
							<xsl:apply-templates select="/root/billingInfo/regions/region" />
						</select>
					</li>
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
						</label>
						<input class="EktronCheckout_PostalCode EktronCheckout_PostalField EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/postalCode"/></xsl:attribute>
						</input>
					</li>
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
						</label>
						<select class="EktronCheckout_CountrySelect EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronCheckout_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronCheckout_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
							<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
							<xsl:apply-templates select="/root/billingInfo/countries/country" />
						</select>
					</li>
					<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
						<label class="EktronCheckout_Row_LeftContents">
							<xsl:attribute name="for">EktronCheckout_BillingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
						</label>
						<input class="EktronCheckout_Phone EktronCheckout_TelephoneField EktronCheckout_Row_RightContents" >
							<xsl:attribute name="name">EktronCheckout_BillingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_BillingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/phone1"/></xsl:attribute>
						</input>
					</li>
					<li>
						<span class="EktronCheckout_RequiredNotice">
							<xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
						</span >
					</li>
					<li class="EktronCheckout_linkRow">
						<span class="EktronCheckout_Row_LeftContents EktronCheckout_UserInfo_BillingAddress_SaveLink EktronCheckout_MutableControlContainer" >
							<a>
								<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveBillingInfo('EktronCheckout_UserInfo_BillingAddressBlock_EditInfo'); return (false);</xsl:attribute>
								<xsl:value-of select="/root/resourceStrings/saveChanges"/>
							</a>
						</span>
            			<xsl:if test="not(/root/urlBasket='')">
              				<span class="EktronCheckout_Row_LeftContents EktronCheckout_UserInfo_BillingAddress_CartLink EktronCheckout_MutableControlContainer">
		            			<a class="EktronCheckout_ReviewOrder_EditCartBtn" >
			            			<xsl:attribute name="href"><xsl:value-of select="/root/urlBasket"/></xsl:attribute>
			            			<xsl:value-of select="/root/resourceStrings/editYourCart"/>
		            			</a>
              				</span>
            			</xsl:if>
            			<xsl:if test="not(/root/urlShopping='')">
              				<span class="EktronCheckout_Row_LeftContents EktronCheckout_UserInfo_BillingAddress_ContinueShoppingLink EktronCheckout_MutableControlContainer">
		            			<a class="EktronCheckout_ContinueShoppingLink">
			            			<xsl:attribute name="href"><xsl:value-of select="/root/urlShopping"/></xsl:attribute>
			            			<xsl:value-of select="/root/resourceStrings/continueShopping"/>
		            			</a>
              			</span>
            			</xsl:if>
						<xsl:if test="/root/billingInfo/validationFailure='false'">
							<span class="EktronCheckout_UserInfo_BillingAddress_CancelLink EktronCheckout_MutableControlContainer">
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').Cancel(); return (false);</xsl:attribute>
									<xsl:value-of select="/root/resourceStrings/cancel"/>
								</a>
							</span>
							<span class="EktronCheckout_AjaxBusyImageContainer">
								<img alt="">
									<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
								</img>
							</span>
						</xsl:if>
					</li>
				</ul>
			</div>
		</div>
	</xsl:template>

</xsl:stylesheet>