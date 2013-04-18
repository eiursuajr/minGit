<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeBillingInfoEntryUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeBillingInfoEntryUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_BillingInfo EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_BillingInfo_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeBillingInfoEntryUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeBillingInfoEntryUIContents">
		<div class="wizardStep">
			<div class="present">
				<div class="blue">
					<xsl:value-of select="/root/resourceStrings/billing"/>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >future <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="grey">
					<xsl:value-of select="/root/resourceStrings/shipping"/>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >future <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="grey">
					<xsl:value-of select="/root/resourceStrings/method"/>
				</div>
			</div>
			<div class="future">
				<div class="grey">
					<xsl:value-of select="/root/resourceStrings/review"/>
				</div>
			</div>
			<div>
				<xsl:attribute name="class" >future <xsl:if test="isZeroCost='true'">hiddenStep</xsl:if></xsl:attribute>
				<div class="grey">
					<xsl:value-of select="/root/resourceStrings/payment"/>
				</div>
			</div>
			<div class="wizardEnd"> </div>
		</div>
				
		<div class="EktronCheckout_UserInfo_BillingAddressBlock_Entry">
      <xsl:if test="/root/paypalInfo/showMessage='true'">
			  <div class="EktronCheckout_UserExistPayPal">
  					<p class="EktronCheckout_ExternalPaymentText"><b><xsl:value-of select="/root/resourceStrings/paypalUserMessage"/></b></p>
				  </div>
			</xsl:if>
			<div class="EktronCheckout_BillingNotice">
				<xsl:value-of select="/root/resourceStrings/enterBillingInformationPreventDelays"/>
			</div>
			<ul class="EktronCheckout_Rows_Container">
        <li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
					<label class="EktronCheckout_Row_LeftContents">
						<xsl:attribute name="for">EktronCheckout_BillingInfo_EmailAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/emailAddress"/>
					</label>
					<input class="EktronCheckout_EmailAddress EktronCheckout_EmailAddressField EktronCheckout_Row_RightContents" >
						<xsl:attribute name="name">EktronCheckout_BillingInfo_EmailAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronCheckout_BillingInfo_EmailAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/userInfo/emailAddress"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
					<label class="EktronCheckout_Row_LeftContents">
						<xsl:attribute name="for">EktronCheckout_BillingInfo_FirstName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/firstName"/>
					</label>
					<input class="EktronCheckout_FirstName EktronCheckout_Row_RightContents" >
						<xsl:attribute name="name">EktronCheckout_BillingInfo_FirstName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronCheckout_BillingInfo_FirstName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/userInfo/firstName"/></xsl:attribute>
					</input>
				</li>

				<li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
					<label class="EktronCheckout_Row_LeftContents">
						<xsl:attribute name="for">EktronCheckout_BillingInfo_LastName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/lastName"/>
					</label>
					<input class="EktronCheckout_LastName EktronCheckout_Row_RightContents" >
						<xsl:attribute name="name">EktronCheckout_BillingInfo_LastName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronCheckout_BillingInfo_LastName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/userInfo/lastName"/></xsl:attribute>
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
        <xsl:choose>
          <xsl:when test="/root/usingGuestCheckout='false'">
            <li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
					    <label class="EktronCheckout_Row_LeftContents">
						    <xsl:attribute name="for">EktronCheckout_BillingInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/password"/>
					    </label>
					    <input type="password" class="EktronCheckout_Password EktronCheckout_Row_RightContents" >
						    <xsl:attribute name="name">EktronCheckout_BillingInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="id">EktronCheckout_BillingInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    </input>
				    </li>
				    <li class="EktronCheckout_Required EktronCheckout_SerializableContainer">
					    <label class="EktronCheckout_Row_LeftContents">
						    <xsl:attribute name="for">EktronCheckout_BillingInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/confirmPassword"/>
					    </label>
					    <input type="password" class="EktronCheckout_Password EktronCheckout_Row_RightContents" >
						    <xsl:attribute name="name">EktronCheckout_BillingInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="id">EktronCheckout_BillingInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    </input>
				    </li>
          </xsl:when>
          <xsl:otherwise>
            <!--<li class="EktronCheckout_SerializableContainer">
					    <input type="password" class="EktronCheckout_Password EktronCheckout_Row_RightContents" >
						    <xsl:attribute name="name">EktronCheckout_BillingInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="id">EktronCheckout_BillingInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    </input>
				    </li>
				    <li class="EktronCheckout_SerializableContainer">
					    <input type="password" class="EktronCheckout_Password EktronCheckout_Row_RightContents" >
						    <xsl:attribute name="name">EktronCheckout_BillingInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="id">EktronCheckout_BillingInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    </input>
				    </li>-->
          </xsl:otherwise>
        </xsl:choose>
				<li>
					<span class="EktronCheckout_RequiredNotice">
						<xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
					</span >
				</li>
				<li class="EktronCheckout_EmailNotice">
					<xsl:value-of select="/root/resourceStrings/emailIsSecureExplanation"/>
				</li>
				<li class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
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
							<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveBillingGetNextPage('EktronCheckout_UserInfo_BillingAddressBlock_Entry'); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/nextPage"/>
						</a>
					</span>
					<span class="EktronCheckout_waitCreatingAccountMessageContainer">
						<xsl:value-of select="/root/resourceStrings/waitWhileAccountCreated"/>
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