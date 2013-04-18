<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeEditAddressUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeEditAddressUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_ShippingInfo EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_ShippingInfo_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeEditAddressUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeEditAddressUIContents">
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
	
		<div class="EktronCheckout_UserInfo_ShippingAddressBlock_Entry">
			<xsl:variable name="RequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_Required<xsl:if test="/root/editAddress/isDefaultBilling='true'">_Disabled EktronCheckout_Row_Disabled</xsl:if></xsl:variable>
			<xsl:variable name="NotRequiredRowClass">EktronCheckout_SerializableContainer EktronCheckout_NotRequired<xsl:if test="/root/editAddress/isDefaultBilling='true'"> EktronCheckout_Row_Disabled</xsl:if></xsl:variable>
			<div class="EktronCheckout_ShippingNotice">
				<!--<xsl:value-of select="/root/resourceStrings/enterAddress"/>-->
        Please update the address.
        When finished, click the "Ship to this address" button to proceed with your order. Or you may return to your Address Book.
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
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/name"/></xsl:attribute>
						
					</input>
          
          <input class="EktronCheckout_Id" type="hidden" >
						<xsl:attribute name="name">EktronCheckout_ShippingInfo_Id_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronCheckout_ShippingInfo_Id_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/id"/></xsl:attribute>
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
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/company"/></xsl:attribute>
						
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
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/address1"/></xsl:attribute>
						
					</input>
				</li>
				<li>
					<xsl:attribute name="class"><xsl:value-of select="$NotRequiredRowClass"></xsl:value-of></xsl:attribute>
					<label class="EktronCheckout_Row_LeftContents"><xsl:attribute name="for">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></label>
					<input class="EktronCheckout_AddressLine2 EktronCheckout_Row_RightContents" >
						<xsl:attribute name="name">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronCheckout_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/address2"/></xsl:attribute>
						
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
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/city"/></xsl:attribute>
						
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
						
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/editAddress/regions/region" />
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
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/postalCode"/></xsl:attribute>
						
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
						
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/editAddress/countries/country" />
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
						<xsl:attribute name="value"><xsl:value-of select="/root/editAddress/phone1"/></xsl:attribute>
						
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
					<span class="EktronCheckout_UserInfo_NextPageLink">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveAddressInfoAndSelect('EktronCheckout_UserInfo_ShippingAddressBlock_Entry'); return (false);</xsl:attribute>
              <xsl:value-of select="/root/resourceStrings/saveAndShipToThisAddress"/>
						</a>
					</span>
          <span>&#160;&#160;</span>
          <span class="EktronCheckout_UserInfo_PreviousPageLink">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
              <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveAddressInfoAndReturn('EktronCheckout_UserInfo_ShippingAddressBlock_Entry'); return (false);</xsl:attribute>
              <xsl:value-of select="/root/resourceStrings/returnToAddressBook"/>
						</a>
					</span>
          <span>&#160;</span>
          <span class="EktronCheckout_UserInfo_PreviousPageLink">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
              <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').CancelAddressInfoAndReturn(); return (false);</xsl:attribute>
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

	</xsl:template>

</xsl:stylesheet>