<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeShippingInfoUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeShippingInfoUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_ShippingInfo EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_ShippingInfo_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeShippingInfoUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeShippingInfoUIContents">
		<div class="wizardStep">
			<div class="past">
				<div class="bluechecked">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=BillingInfo</xsl:attribute><xsl:value-of select="/root/resourceStrings/billing"/></a>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >present <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="blue">
					<xsl:value-of select="/root/resourceStrings/shipping"/>
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

		<xsl:choose>
			<xsl:when test="/root/isLoggedIn='true'">
				<div class="EktronCheckout_UserInfo_ShippingAddressBlock_Review">
					<ul class="EktronCheckout_Rows_Container">
						<li class="EktronCheckout_ShippingNotice">
							<xsl:value-of select="/root/resourceStrings/enterAddress"/>
						</li>
						<xsl:choose>
							<xsl:when test="/root/shippingIsBillingAddress='true'">
								<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingUsingBillingAddress">
									<xsl:value-of select="/root/resourceStrings/usingBillingAddress"/>
								</li>
							</xsl:when>
							<xsl:otherwise>
								<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_SeperateShippingAddress">
									<xsl:value-of select="/root/resourceStrings/separateShipToAddress"/>
								</li>
							</xsl:otherwise>
						</xsl:choose>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_MultipleAddressSelect">
							<!--<select class="EktronCheckout_MultipleAddressSelect" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_MultipleAddressSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_MultipleAddressSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:apply-templates select="/root/shippingInfo/shippingAddresses/shippingAddress" />
							</select>-->
                <xsl:apply-templates select="/root/shippingInfo/shippingAddresses/shippingAddress" />
							<!--<span class="EktronCheckout_UserInfo_ShippingAddress_MultipleAddressSelect_selectAddress EktronCheckout_MutableControlContainer" >
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SelectShippingAddress(); return (false);</xsl:attribute>
									<xsl:value-of select="/root/resourceStrings/shipToThisAddress"/>
								</a>
							</span>-->
						</li>
						<!--<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_Name">
							<xsl:value-of select="/root/shippingInfo/name"/>
						</li>
						<xsl:if test="not(/root/shippingInfo/company='')">
							<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_ShippingAddress_Company">
								<xsl:value-of select="/root/shippingInfo/company"/>
							</li>
						</xsl:if>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_StreetAddress">
							<xsl:value-of select="/root/shippingInfo/address1"/>
							<xsl:if test="not(/root/shippingInfo/address2='')">, <xsl:value-of select="/root/shippingInfo/address2"/></xsl:if>
						</li>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_CityState">
							<xsl:value-of select="/root/shippingInfo/city"/>,
							<xsl:value-of select="/root/shippingInfo/regionSelected/name"/><xsl:text> </xsl:text>
              <xsl:value-of select="/root/shippingInfo/postalCode"/>
						</li>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_Country">
							<xsl:value-of select="/root/shippingInfo/countrySelected/name"/>
						</li>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_Phone">
							<xsl:value-of select="/root/shippingInfo/phone1"/>
						</li>-->
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_ShippingAddress_EditLinkContainer EktronCheckout_MutableControlContainer">
							<!--<span class="EktronCheckout_UserInfo_ShippingAddress_EditLink" >
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').Edit('EktronCheckout_UserInfo_ShippingAddressBlock_EditInfo'); return (false);</xsl:attribute>
									<xsl:value-of select="/root/resourceStrings/editInfo"/>
								</a>
							</span>-->
							<span class="EktronCheckout_UserInfo_ShippingAddress_AddNewAddressLink" >
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').Edit('EktronCheckout_UserInfo_ShippingAddressBlock_AddAddress'); return (false);</xsl:attribute>
									<xsl:value-of select="/root/resourceStrings/addNewAddress"/>
								</a>
							</span>
							<!--
							<xsl:if test="/root/shippingInfo/hasMultipleAddresses='true'">
								<span class="EktronCheckout_UserInfo_ShippingAddress_DeleteAddressLink" >
									<a>
										<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
										<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').DeleteAddress(); return (false);</xsl:attribute>
										Delete Address
									</a>
								</span>
							</xsl:if>
							-->
						</li>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
							<span class="EktronCheckout_UserInfo_PreviousPageLink">
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').PreviousPage(); return (false);</xsl:attribute>
									<xsl:value-of select="/root/resourceStrings/previousPage"/>
								</a>
							</span>
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
						
					<div>
						<xsl:attribute name="class" >EktronCheckout_UserInfo_ShippingAddressBlock_EditInfo EktronCheckout_modalContainer <xsl:if test="/root/shippingInfo/validationFailure='true'">EktronCheckout_validationFailure</xsl:if></xsl:attribute>
						<div>
							<xsl:variable name="RequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_Required<xsl:if test="/root/shippingIsBillingAddress='true'">_Disabled EktronCheckout_Row_Disabled</xsl:if></xsl:variable>
							<xsl:variable name="NotRequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_NotRequired<xsl:if test="/root/shippingIsBillingAddress='true'"> EktronCheckout_Row_Disabled</xsl:if></xsl:variable>
							<h4>
								<xsl:value-of select="/root/resourceStrings/editShippingInformation"/>
							</h4>
							<div class="EktronCheckout_SerializableContainer EktronCheckout_ShippingAddressSelectBilling">
								<label>
									<xsl:attribute name="for">EktronCheckout_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
									<xsl:value-of select="/root/resourceStrings/sameAsBilling"/>
								</label>
								<input type="checkbox">
									<xsl:attribute name="name">EktronCheckout_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
									<xsl:attribute name="id">EktronCheckout_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').ShippingIsBillingAddress(this, "EktronCheckout_UserInfo_ShippingAddressBlock_EditInfo");</xsl:attribute>
									<xsl:if test="/root/shippingIsBillingAddress='true'">
										<xsl:attribute name="checked">checked</xsl:attribute>
									</xsl:if>
								</input>
							</div>
							<ul class="EktronCheckout_Rows_Container">
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
									</label>
									<input class="EktronCheckout_Name EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/name"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/company"/>
									</label>
									<input class="EktronCheckout_Company EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/company"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
									</label>
									<input class="EktronCheckout_Address EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address1"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents"><xsl:attribute name="for">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></label>
									<input class="EktronCheckout_AddressLine2 EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address2"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
									</label>
									<input class="EktronCheckout_City EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/city"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
									</label>
									<select class="EktronCheckout_StateSelect EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
										<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
										<xsl:apply-templates select="/root/shippingInfo/regions/region" />
									</select>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
									</label>
									<input class="EktronCheckout_PostalCode EktronCheckout_PostalField EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/postalCode"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
									</label>
									<select class="EktronCheckout_CountrySelect EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
										<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
										<xsl:apply-templates select="/root/shippingInfo/countries/country" />
									</select>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
									</label>
									<input class="EktronCheckout_Phone EktronCheckout_TelephoneField EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/phone1"/></xsl:attribute>
										<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
									</input>
								</li>
								<li>
									<span class="EktronCheckout_RequiredNotice">
										<xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
									</span>
								</li>
								<li>
									<span class="EktronCheckout_Row_LeftContents EktronCheckout_UserInfo_ShippingAddress_SaveLink EktronCheckout_MutableControlContainer">
										<a>
											<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
											<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveShippingInfo('EktronCheckout_UserInfo_ShippingAddressBlock_EditInfo'); return (false);</xsl:attribute>
											<xsl:value-of select="/root/resourceStrings/saveChanges"/>
										</a>
									</span>
									<xsl:if test="/root/shippingInfo/validationFailure='false'">
										<span class="EktronCheckout_UserInfo_ShippingAddress_CancelLink EktronCheckout_MutableControlContainer">
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
					
					<div class="EktronCheckout_UserInfo_ShippingAddressBlock_AddAddress EktronCheckout_modalContainer">
						<div>
							<xsl:variable name="RequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_Required</xsl:variable>
							<xsl:variable name="NotRequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_NotRequired</xsl:variable>
							<h4>
								<xsl:value-of select="/root/resourceStrings/addShippingInformation"/>
							</h4>
							<ul class="EktronCheckout_Rows_Container">
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
									</label>
									<input class="EktronCheckout_Name EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/company"/>
									</label>
									<input class="EktronCheckout_Company EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
									</label>
									<input class="EktronCheckout_Address EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents"><xsl:attribute name="for">EktronCheckout_NewShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></label>
									<input class="EktronCheckout_AddressLine2 EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
									</label>
									<input class="EktronCheckout_City EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
									</label>
									<select class="EktronCheckout_StateSelect EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
										<xsl:apply-templates select="/root/shippingInfo/regions/region" />
									</select>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
									</label>
									<input class="EktronCheckout_PostalCode EktronCheckout_PostalField EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
									</label>
									<select class="EktronCheckout_CountrySelect EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronCheckout_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronCheckout_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
										<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
										<xsl:apply-templates select="/root/shippingInfo/countries/country" />
									</select>
								</li>
								<li>
									<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
									<label class="EktronCheckout_Row_LeftContents">
										<xsl:attribute name="for">EktronCheckout_NewShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
									</label>
									<input class="EktronCheckout_Phone EktronCheckout_TelephoneField EktronCheckout_Row_RightContents" >
										<xsl:attribute name="name">EktronCheckout_NewShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
										<xsl:attribute name="id">EktronCheckout_NewShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
									</input>
								</li>
								<li>
									<span class="EktronCheckout_RequiredNotice">
										<xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
									</span>
								</li>
								<li>
									<span class="EktronCheckout_Row_LeftContents EktronCheckout_UserInfo_ShippingAddress_SaveLink EktronCheckout_MutableControlContainer">
										<a>
											<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
											<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveNewAddress('EktronCheckout_UserInfo_ShippingAddressBlock_AddAddress'); return (false);</xsl:attribute>
											Save Address
										</a>
									</span>
									<span class="EktronCheckout_UserInfo_ShippingAddress_CancelLink EktronCheckout_MutableControlContainer">
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
								</li>
							</ul>
						</div>
					</div>					
				</div>
			</xsl:when>
			
			<xsl:otherwise>
				<div class="EktronCheckout_UserInfo_ShippingAddressBlock_Entry">
					<xsl:variable name="RequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_Required<xsl:if test="/root/shippingIsBillingAddress='true'">_Disabled EktronCheckout_Row_Disabled</xsl:if></xsl:variable>
					<xsl:variable name="NotRequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_NotRequired<xsl:if test="/root/shippingIsBillingAddress='true'"> EktronCheckout_Row_Disabled</xsl:if></xsl:variable>
					<div class="EktronCheckout_ShippingNotice">
						<xsl:value-of select="/root/resourceStrings/enterAddress"/>
					</div>
					<div class="EktronCheckout_SerializableContainer EktronCheckout_ShippingAddressSelectBilling">
						<label>
							<xsl:attribute name="for">EktronCheckout_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/sameAsBilling"/>
						</label>
						<input type="checkbox">
							<xsl:attribute name="name">EktronCheckout_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronCheckout_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').ShippingIsBillingAddress(this, "EktronCheckout_UserInfo_ShippingAddressBlock_Entry");</xsl:attribute>
							<xsl:if test="/root/shippingIsBillingAddress='true'">
								<xsl:attribute name="checked">checked</xsl:attribute>
							</xsl:if>
						</input>
					</div>
					<ul class="EktronCheckout_Rows_Container">
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
							</label>
							<input class="EktronCheckout_Name EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/name"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/company"/>
							</label>
							<input class="EktronCheckout_Company EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/company"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
							</label>
							<input class="EktronCheckout_Address EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address1"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents"><xsl:attribute name="for">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></label>
							<input class="EktronCheckout_AddressLine2 EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address2"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
							</label>
							<input class="EktronCheckout_City EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/city"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
							</label>
							<select class="EktronCheckout_StateSelect EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
								<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
								<xsl:apply-templates select="/root/shippingInfo/regions/region" />
							</select>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
							</label>
							<input class="EktronCheckout_PostalCode EktronCheckout_PostalField EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/postalCode"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
							</label>
							<select class="EktronCheckout_CountrySelect EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronCheckout_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronCheckout_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
								<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
								<xsl:apply-templates select="/root/shippingInfo/countries/country" />
							</select>
						</li>
						<li>
							<xsl:attribute name="class"><xsl:value-of select="$RequiredRowClass"></xsl:value-of></xsl:attribute>
							<label class="EktronCheckout_Row_LeftContents">
								<xsl:attribute name="for">EktronCheckout_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
							</label>
							<input class="EktronCheckout_Phone EktronCheckout_TelephoneField EktronCheckout_Row_RightContents" >
								<xsl:attribute name="name">EktronCheckout_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="id">EktronCheckout_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/phone1"/></xsl:attribute>
								<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
							</input>
						</li>
						<li>
							<span class="EktronCheckout_RequiredNotice">
								<xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
							</span>
						</li>
						<li class="EktronCheckout_EmailNotice">
							<xsl:value-of select="/root/resourceStrings/emailIsSecureExplanation"/>
						</li>
						<li class="EktronCheckout_UserInfo_ShippingAddressFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
							<span class="EktronCheckout_UserInfo_PreviousPageLink">
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').PreviousPage(); return (false);</xsl:attribute>
									Previous Page
								</a>
							</span>
							<span class="EktronCheckout_UserInfo_NextPageLink">
								<a>
									<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
									<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveShippingGetNextPage('EktronCheckout_UserInfo_ShippingAddressBlock_Entry'); return (false);</xsl:attribute>
									Next Page
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
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>