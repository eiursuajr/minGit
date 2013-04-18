<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/controlId" />
	<!--<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>-->
	<xsl:variable name="HrefTarget" >#</xsl:variable>

	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:choose>
					<xsl:when test="/root/dataMode='MakeSubfragment_Regions'">
						<xsl:call-template name="AjaxCallbackGetRegions" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="MakeUI" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<div>
					<xsl:attribute name="id" >EktronMyAccount_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:attribute name="class" >EktronMyAccount<xsl:if test="(/root/browser='ie') and (/root/browserMajorVersion='6')"> EktronMyAccount_BrowserIE6</xsl:if></xsl:attribute>
					<xsl:call-template name="MakeUI" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="MakeUI">
		<div class="EktronMyAccountHeader"></div>
		<xsl:choose>
			<xsl:when test="/root/error='true'">
				<xsl:call-template name="MakeErrorUI" />
			</xsl:when>
			<xsl:when test="/root/isLoggedIn='false'">
				<xsl:call-template name="MakeUnauthenticatedUI" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="MakePersonalInfoUI" />
				<xsl:call-template name="MakeBillingInfoEntryUI" />
				<xsl:call-template name="MakeShippingInfoEntryUI" />
			</xsl:otherwise>
		</xsl:choose>
		<div class="EktronMyAccountFooter"></div>
		<xsl:apply-templates select="/root/resourceStrings/javascriptResourceStrings" />
		<xsl:if test="/root/activeDirectoryEnabled='false'">
			<input type="hidden" class="passwordValidationString" >
				<xsl:attribute name="value"><xsl:value-of select="/root/passwordValidationString" /></xsl:attribute>
			</input>
			<input type="hidden" class="passwordValidationErrorMessage">
				<xsl:attribute name="value"><xsl:value-of select="/root/passwordValidationErrorString"/></xsl:attribute>
			</input>
		</xsl:if>
	</xsl:template>
	

	<xsl:template name="MakePersonalInfoUI">
		<div class="EktronMyAccount_PersonalInfo viewBlock">
			<xsl:if test="/root/validationFailingSectionName!=''"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
			<h3>
				<xsl:value-of select="/root/resourceStrings/personalInformation"/>
			</h3>
			<ul>
				<li><xsl:value-of select="/root/userInfo/firstName"/></li>
				<li><xsl:value-of select="/root/userInfo/lastName"/></li>
				<li><xsl:value-of select="/root/userInfo/emailAddress"/></li>
				<li><xsl:value-of select="/root/resourceStrings/password"/> <xsl:text xml:space="preserve"> ********</xsl:text></li>
				<xsl:apply-templates mode="view" select="/root/userInfo/customProperties/property" />
			</ul>
			<span class="EktronMyAccount_PersonalInfo_EditLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Edit('EktronMyAccount_PersonalInfo_Edit'); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/edit"/>
				</a>
			</span>
		</div>
		<div class="EktronMyAccount_PersonalInfo_Edit editBlock" >
			<xsl:if test="/root/validationFailingSectionName!='UserInfo'"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
			<h3>
				<xsl:value-of select="/root/resourceStrings/editPersonalInformation"/>
			</h3>
			<ul>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_PersonalInfo_FirstName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/firstName"/>
					</label>
          <xsl:choose>
            <xsl:when test="/root/userInfo/thirdPartyAuthenticationEnabled='true'">
              <xsl:if test="/root/userInfo/firstName=''">&#160;</xsl:if><xsl:value-of select="/root/userInfo/firstName"/>
            </xsl:when>
            <xsl:otherwise>
              <input>
					      <xsl:attribute name="name">EktronMyAccount_PersonalInfo_FirstName_<xsl:value-of select="$ControlId"/></xsl:attribute>
					      <xsl:attribute name="id">EktronMyAccount_PersonalInfo_FirstName_<xsl:value-of select="$ControlId"/></xsl:attribute>
					      <xsl:attribute name="value"><xsl:value-of select="/root/userInfo/firstName"/></xsl:attribute>
					      <xsl:if test="/root/activeDirectoryEnabled='true'">
						      <xsl:attribute name="type">hidden</xsl:attribute>
						      <xsl:text xml:space="preserve">  </xsl:text>
						      <xsl:value-of select="/root/userInfo/firstName"/>
					      </xsl:if>
				      </input>
            </xsl:otherwise>
          </xsl:choose>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_PersonalInfo_LastName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/lastName"/>
					</label>
          <xsl:choose>
            <xsl:when test="/root/userInfo/thirdPartyAuthenticationEnabled='true'">
              <xsl:if test="/root/userInfo/lastName=''">&#160;</xsl:if><xsl:value-of select="/root/userInfo/lastName"/>
            </xsl:when>
            <xsl:otherwise>
              <input>
						    <xsl:attribute name="name">EktronMyAccount_PersonalInfo_LastName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="id">EktronMyAccount_PersonalInfo_LastName_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="value"><xsl:value-of select="/root/userInfo/lastName"/></xsl:attribute>
						    <xsl:if test="/root/activeDirectoryEnabled='true'">
							    <xsl:attribute name="type">hidden</xsl:attribute>
							    <xsl:text xml:space="preserve">  </xsl:text>
							    <xsl:value-of select="/root/userInfo/lastName"/>
						    </xsl:if>
					    </input>
            </xsl:otherwise>
          </xsl:choose>
					
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_PersonalInfo_EmailAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/emailAddress"/>
					</label>
          <xsl:choose>
            <xsl:when test="/root/userInfo/thirdPartyAuthenticationEnabled='true'">
              <xsl:if test="/root/userInfo/emailAddress=''">&#160;</xsl:if><xsl:value-of select="/root/userInfo/emailAddress"/>
            </xsl:when>
            <xsl:otherwise>
              <input class="EktronMyAccount_EmailAddressField">
						    <xsl:attribute name="name">EktronMyAccount_PersonalInfo_EmailAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="id">EktronMyAccount_PersonalInfo_EmailAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						    <xsl:attribute name="value"><xsl:value-of select="/root/userInfo/emailAddress"/></xsl:attribute>
						    <xsl:if test="/root/activeDirectoryIntegrationEnabled='true'">
							    <xsl:attribute name="type">hidden</xsl:attribute>
							    <xsl:text xml:space="preserve">  </xsl:text>
							    <xsl:value-of select="/root/userInfo/emailAddress"/>
						    </xsl:if>
					    </input>
            </xsl:otherwise>
          </xsl:choose>
					
				</li>
        <xsl:if test="/root/userInfo/thirdPartyAuthenticationEnabled='false'">
			    <li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
				    <label>
					    <xsl:attribute name="for">EktronMyAccount_PersonalInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    <span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/password"/>
				    </label>
				    <input>
					    <xsl:attribute name="name">EktronMyAccount_PersonalInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    <xsl:attribute name="id">EktronMyAccount_PersonalInfo_Password_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    <xsl:attribute name="value">unchanged</xsl:attribute>
					    <xsl:choose>
						    <xsl:when test="/root/activeDirectoryEnabled='false'">
							    <xsl:attribute name="type">password</xsl:attribute>
							    <xsl:attribute name="onfocus">this.value=''</xsl:attribute>
						    </xsl:when>
						    <xsl:otherwise>
							    <xsl:attribute name="type">hidden</xsl:attribute>
							    <xsl:text xml:space="preserve">  ********</xsl:text>
						    </xsl:otherwise>
					    </xsl:choose>
				    </input>
			    </li>
			    <li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
				    <label>
					    <xsl:attribute name="for">EktronMyAccount_PersonalInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    <span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/confirmPassword"/>
				    </label>
				    <input>
					    <xsl:attribute name="name">EktronMyAccount_PersonalInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    <xsl:attribute name="id">EktronMyAccount_PersonalInfo_ConfirmPassword_<xsl:value-of select="$ControlId"/></xsl:attribute>
					    <xsl:attribute name="value">unchanged</xsl:attribute>
					    <xsl:choose>
						    <xsl:when test="/root/activeDirectoryEnabled='false'">
							    <xsl:attribute name="type">password</xsl:attribute>
							    <xsl:attribute name="onfocus">this.value=''</xsl:attribute>
						    </xsl:when>
						    <xsl:otherwise>
							    <xsl:attribute name="type">hidden</xsl:attribute>
							    <xsl:text xml:space="preserve">  ********</xsl:text>
						    </xsl:otherwise>
					    </xsl:choose>
				    </input>
			    </li>
        </xsl:if>
				<xsl:apply-templates mode="edit" select="/root/userInfo/customProperties/property" />
				<li class="infoBlock">
					<span class="EktronMyAccount_RequiredNotice">
						<xsl:value-of select="/root/resourceStrings/anAsteriskIndicatesRequired"/>
					</span>
				</li>
				<li class="infoBlock">
					<span class="EktronMyAccount_UsernameNotice">
						<xsl:value-of select="/root/resourceStrings/emailIsUsername"/>
					</span>
				</li>
				<li class="buttonBlock">
					<span class="EktronMyAccount_UserInfo_PersonalInfo_SaveLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').SaveInfo('EktronMyAccount_PersonalInfo_Edit', 'save_personalinfo'); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/saveChanges"/>
						</a>
					</span>
					<xsl:if test="/root/validationFailingSectionName=''">
						<span class="EktronMyAccount_UserInfo_PersonalInfo_CancelLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
							<a>
								<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Cancel(); return (false);</xsl:attribute>
								<xsl:value-of select="/root/resourceStrings/cancel"/>
							</a>
						</span>
					</xsl:if>
					<span class="EktronMyAccount_AjaxBusyImageContainer" style="display: none;">
						<img alt="">
							<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
						</img>
					</span>
				</li>
			</ul>
		</div>
	</xsl:template>

	<xsl:template name="MakeBillingInfoEntryUI">
		<div class="EktronMyAccount_BillingInfo viewBlock">
			<xsl:if test="/root/validationFailingSectionName!=''"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
			<h3>
				<xsl:value-of select="/root/resourceStrings/billingAddress"/>
			</h3>
			<ul>
				<li><xsl:value-of select="/root/billingInfo/name"/></li>
				<xsl:if test="/root/billingInfo/company!=''" ><li><xsl:value-of select="/root/billingInfo/company"/></li></xsl:if>
				<li><xsl:value-of select="/root/billingInfo/address1"/></li>
				<xsl:if test="/root/billingInfo/address2!=''" ><li><xsl:value-of select="/root/billingInfo/address2"/></li></xsl:if>
				<li><xsl:value-of select="/root/billingInfo/city"/><xsl:if test="/root/billingInfo/city!='' and /root/billingInfo/regionSelected/name!=''" >, </xsl:if></li>
				<xsl:if test="/root/billingInfo/regionSelected/name!=''" ><li><xsl:value-of select="/root/billingInfo/regionSelected/name"/></li></xsl:if>
				<li><xsl:value-of select="/root/billingInfo/countrySelected/name"/></li>
				<li><xsl:value-of select="/root/billingInfo/postalCode"/></li>
				<li><xsl:value-of select="/root/billingInfo/phone1"/></li>
				<xsl:if test="/root/billingInfo/phone2!=''" ><li><xsl:value-of select="/root/billingInfo/phone2"/></li></xsl:if>
        <xsl:if test="/root/billingInfo/iscommercial!=''" >
          <li>
            <input type="checkbox" disabled="disabled" ><xsl:if test="/root/billingInfo/iscommercial = 'true'">
              <xsl:attribute name="checked"><xsl:text>checked</xsl:text></xsl:attribute></xsl:if>
            </input>
            <xsl:value-of select="/root/resourceStrings/iscommercial"/>
          </li>
        </xsl:if>
			</ul>
			<span class="EktronMyAccount_BillingInfo_EditLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Edit('EktronMyAccount_BillingInfo_Edit'); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/edit"/>
				</a>
			</span>
		</div>
		<div class="EktronMyAccount_BillingInfo_Edit editBlock" >
			<xsl:if test="/root/validationFailingSectionName!='BillingInfo'"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
			<h3>
				<xsl:value-of select="/root/resourceStrings/editBillingAddress"/>
			</h3>
			<ul>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/name"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_NotRequired EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/company"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/company"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/address1"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_NotRequired EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/address2"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/city"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
					</label>
					<select class="EktronMyAccount_StateSelect" >
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/billingInfo/regions/region" />
					</select>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
					</label>
					<input class="EktronMyAccount_NumericField" >
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/postalCode"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
					</label>
					<select class="EktronMyAccount_CountrySelect" >
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="onchange">Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronMyAccount_BillingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronMyAccount_BillingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/billingInfo/countries/country" />
					</select>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_BillingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
					</label>
					<input class="EktronMyAccount_TelephoneField" >
						<xsl:attribute name="name">EktronMyAccount_BillingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_BillingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/billingInfo/phone1"/></xsl:attribute>
					</input>
				</li>
        <li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
          <label>
            <xsl:attribute name="for">EktronMyAccount_BillingInfo_IsCommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:value-of select="/root/resourceStrings/iscommercial"/>
          </label>
          <input class="EktronMyAccount_TelephoneField" type="checkbox" >
            <xsl:attribute name="name">EktronMyAccount_BillingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:attribute name="id">EktronMyAccount_BillingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:if test="/root/billingInfo/iscommercial = 'true'">
              <xsl:attribute name="checked"><xsl:text>checked</xsl:text></xsl:attribute>
            </xsl:if> 
          </input>
        </li>
				<li class="infoBlock">
					<span class="EktronMyAccount_RequiredNotice">
						<xsl:value-of select="/root/resourceStrings/anAsteriskIndicatesRequired"/>
					</span>
				</li>
				<li class="buttonBlock">
					<span class="EktronMyAccount_UserInfo_BillingAddress_SaveLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').SaveInfo('EktronMyAccount_BillingInfo_Edit', 'save_billinginfo'); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/saveChanges"/>
						</a>
					</span>
					<xsl:if test="/root/validationFailingSectionName=''">
						<span class="EktronMyAccount_UserInfo_BillingAddress_CancelLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
							<a>
								<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Cancel(); return (false);</xsl:attribute>
								<xsl:value-of select="/root/resourceStrings/cancel"/>
							</a>
						</span>
					</xsl:if>
					<span class="EktronMyAccount_AjaxBusyImageContainer" style="display: none;">
						<img alt="">
							<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
						</img>
					</span>
				</li>
			</ul>
		</div>
	</xsl:template>
	
	<xsl:template name="MakeShippingInfoEntryUI">
		<div class="EktronMyAccount_ShippingInfo viewBlock" >
			<xsl:if test="/root/validationFailingSectionName!=''"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
			<h3>
				<xsl:value-of select="/root/resourceStrings/shippingAddress"/>
			</h3>
			<ul class="EktronMyAccount_MultipleAddressBlock">
				<xsl:if test="/root/shippingInfo/hasMultipleAddresses='true'">
					<li>
						<select class="EktronMyAccount_MultipleAddressSelect" >
							<xsl:attribute name="name">EktronMyAccount_ShippingInfo_MultipleAddressSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:attribute name="id">EktronMyAccount_ShippingInfo_MultipleAddressSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
							<xsl:apply-templates select="/root/shippingInfo/shippingAddresses/shippingAddress" />
						</select>
					</li>
					<li class="EktronMyAccount_ShippingAddress_SelectLink EktronMyAccount_MutableControlContainer" >
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').SelectShippingAddress(); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/setAsDefault"/>
						</a>
					</li>
					<li>
						<span class="EktronMyAccount_AjaxBusyImageContainer" style="display: none;">
							<img alt="">
								<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
							</img>
						</span>
					</li>
				</xsl:if>
			</ul>
			<ul class="EktronMyAccount_ShippingAddressDetails" >
				<xsl:choose>
					<xsl:when test="/root/shippingIsBillingAddress='true'">
						<li><xsl:value-of select="/root/resourceStrings/sameAsBilling"/></li>
					</xsl:when>
					<xsl:otherwise>
						<li><xsl:value-of select="/root/shippingInfo/name"/></li>
						<xsl:if test="/root/shippingInfo/company!=''" ><li><xsl:value-of select="/root/shippingInfo/company"/></li></xsl:if>
						<li><xsl:value-of select="/root/shippingInfo/address1"/></li>
						<xsl:if test="/root/shippingInfo/address2!=''" ><li><xsl:value-of select="/root/shippingInfo/address2"/></li></xsl:if>
						<li><xsl:value-of select="/root/shippingInfo/city"/><xsl:if test="/root/shippingInfo/city!='' and /root/shippingInfo/regionSelected/name!=''" >, </xsl:if></li>
						<xsl:if test="/root/shippingInfo/regionSelected/name!=''" ><li><xsl:value-of select="/root/shippingInfo/regionSelected/name"/></li></xsl:if>
						<li><xsl:value-of select="/root/shippingInfo/countrySelected/name"/></li>
						<li><xsl:value-of select="/root/shippingInfo/postalCode"/></li>
						<li><xsl:value-of select="/root/shippingInfo/phone1"/></li>
						<xsl:if test="/root/shippingInfo/phone2!=''" ><li><xsl:value-of select="/root/shippingInfo/phone2"/></li></xsl:if>
            <xsl:if test="/root/shippingInfo/iscommercial!=''" >
              <li>
                <input type="checkbox" disabled="disabled" >
                  <xsl:if test="/root/shippingInfo/iscommercial = 'true'"><xsl:attribute name="checked"><xsl:text>checked</xsl:text></xsl:attribute></xsl:if>
                </input>
                <xsl:value-of select="/root/resourceStrings/iscommercial"/>
              </li>
            </xsl:if>
					</xsl:otherwise>
				</xsl:choose>
			</ul>
			<xsl:if test="/root/shippingIsBillingAddress!='true'">
              <span class="EktronMyAccount_ShippingInfo_EditLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
              <a>
                <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
                <xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Edit('EktronMyAccount_ShippingInfo_Edit'); return (false);</xsl:attribute>
                <xsl:value-of select="/root/resourceStrings/edit"/>
              </a>
            </span>
           </xsl:if>
			<span class="EktronMyAccount_UserInfo_ShippingAddress_AddNewAddressLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton" >
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').AddNewAddress(); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/addNewAddress"/>
				</a>
			</span>
      <xsl:if test="/root/shippingIsBillingAddress!='true' and /root/shippingInfo/hasMultipleAddresses = 'true' ">
      <span class="EktronMyAccount_UserInfo_ShippingAddress_DeleteAddressLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
        <a>
          <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
          <xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').DeleteAddress('<xsl:value-of select="/root/shippingInfo/id" />'); return (false);</xsl:attribute>
          <xsl:value-of select="/root/resourceStrings/deleteAddress"/>
        </a>
      </span>
      </xsl:if>
		</div>
		
		<div class="EktronMyAccount_ShippingInfo_Edit editBlock" >
			<xsl:if test="/root/validationFailingSectionName!='ShippingInfo'"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
			<h3>
				<xsl:value-of select="/root/resourceStrings/editShippingAddress"/>
			</h3>
			<ul>
			
				<li class="EktronMyAccount_SerializableContainer EktronMyAccount_ShippingAddressSelectBilling">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/sameAsBilling"/>
					</label>
					<input type="checkbox">
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_ShippingIsBillingAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').ShippingIsBillingAddress(this, "EktronMyAccount_ShippingInfo_Edit");</xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'">
							<xsl:attribute name="checked">checked</xsl:attribute>
						</xsl:if>
					</input>
				</li>
			
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/name"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="EktronMyAccount_NotRequired EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/company"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/company"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address1"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="EktronMyAccount_NotRequired EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address2"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/city"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
					</label>
					<select class="EktronMyAccount_StateSelect" >
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/shippingInfo/regions/region" />
					</select>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
					</label>
					<input class="EktronMyAccount_NumericField" >
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/postalCode"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
					</label>
					<select class="EktronMyAccount_CountrySelect" >
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="onchange">Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronMyAccount_ShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronMyAccount_ShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/shippingInfo/countries/country" />
					</select>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
					</label>
					<input class="EktronMyAccount_TelephoneField" >
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/phone1"/></xsl:attribute>
						<xsl:if test="/root/shippingIsBillingAddress='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
        <li class="EktronMyAccount_SerializableContainer">
          <label>
            <xsl:attribute name="for">EktronMyAccount_ShippingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:value-of select="/root/resourceStrings/iscommercial"/>
          </label>
          <input type="checkbox" class="EktronMyAccount_DefaultAddressField" >
            <xsl:attribute name="name">EktronMyAccount_ShippingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:attribute name="id">EktronMyAccount_ShippingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:if test="/root/shippingInfo/iscommercial='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
          </input>
        </li>
				<li class="EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_ShippingInfo_DefaultAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/defaultAddress"/>
					</label>
					<input type="checkbox" class="EktronMyAccount_DefaultAddressField" >
						<xsl:attribute name="name">EktronMyAccount_ShippingInfo_DefaultAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_ShippingInfo_DefaultAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:if test="/root/shippingInfo/isDefaultShipping='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
						<xsl:if test="/root/shippingIsBillingAddress='true' or /root/shippingInfo/isDefaultShipping='true'"><xsl:attribute name="disabled">disabled</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="infoBlock">
					<span class="EktronMyAccount_RequiredNotice">
						<xsl:value-of select="/root/resourceStrings/anAsteriskIndicatesRequired"/>
					</span>
				</li>
				<li class="buttonBlock">
					<span class="EktronMyAccount_ShippingAddress_SaveLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').SaveInfo('EktronMyAccount_ShippingInfo_Edit', 'save_shippinginfo'); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/saveChanges"/>
						</a>
					</span>
					<xsl:if test="/root/validationFailingSectionName=''">
						<span class="EktronMyAccount_ShippingAddress_CancelLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
							<a>
								<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Cancel(); return (false);</xsl:attribute>
								<xsl:value-of select="/root/resourceStrings/cancel"/>
							</a>
						</span>
					</xsl:if>
					<span class="EktronMyAccount_AjaxBusyImageContainer" style="display: none;">
						<img alt="">
							<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
						</img>
					</span>
				</li>
			</ul>
		</div>

		<div class="EktronMyAccount_ShippingInfo_Add editBlock" style="display: none;">
			<h3>
				<xsl:value-of select="/root/resourceStrings/addShippingAddress"/>
			</h3>
			<ul>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/name"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_Name_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/name"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_NotRequired EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/company"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_Company_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/company"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/address"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_AddressLine1_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address1"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_NotRequired EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_AddressLine2_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/address2"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/city"/>
					</label>
					<input>
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_City_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/city"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/stateProvince"/>
					</label>
					<select class="EktronMyAccount_StateSelect" >
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/shippingInfo/regions/region" />
					</select>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/postalCode"/>
					</label>
					<input class="EktronMyAccount_NumericField" >
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_PostalCode_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/postalCode"/></xsl:attribute>
					</input>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/country"/>
					</label>
					<select class="EktronMyAccount_CountrySelect" >
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="onchange">Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').CountryChanged('EktronMyAccount_NewShippingInfo_CountrySelect_<xsl:value-of select="$ControlId"/>', 'EktronMyAccount_NewShippingInfo_RegionSelect_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>
						<option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
						<xsl:apply-templates select="/root/shippingInfo/countries/country" />
					</select>
				</li>
				<li class="EktronMyAccount_Required EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<span class="EktronMyAccount_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/phone"/>
					</label>
					<input class="EktronMyAccount_TelephoneField" >
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_Phone_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/shippingInfo/phone1"/></xsl:attribute>
					</input>
				</li>
        <li class="EktronMyAccount_SerializableContainer">
          <label>
            <xsl:attribute name="for">EktronMyAccount_NewShippingInfo_Iscommercial_<xsl:value-of select="$ControlId"/> </xsl:attribute>
            <xsl:value-of select="/root/resourceStrings/iscommercial"/>
          </label>
          <input type="checkbox" class="EktronMyAccount_DefaultAddressField" >
            <xsl:attribute name="name">EktronMyAccount_NewShippingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:attribute name="id">EktronMyAccount_NewShippingInfo_Iscommercial_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:if test="/root/shippingInfo/iscommercial='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
          </input>
        </li>
				<li class="EktronMyAccount_SerializableContainer">
					<label>
						<xsl:attribute name="for">EktronMyAccount_NewShippingInfo_DefaultAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/defaultAddress"/>
					</label>
					<input type="checkbox" class="EktronMyAccount_DefaultAddressField" >
						<xsl:attribute name="name">EktronMyAccount_NewShippingInfo_DefaultAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_NewShippingInfo_DefaultAddress_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:if test="/root/shippingInfo/isDefaultShipping='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
					</input>
				</li>
				<li class="infoBlock">
					<span class="EktronMyAccount_RequiredNotice">
						<xsl:value-of select="/root/resourceStrings/anAsteriskIndicatesRequired"/>
					</span>
				</li>
				<li class="buttonBlock">
					<span class="EktronMyAccount_ShippingAddress_SaveLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').SaveNewAddress(); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/saveChanges"/>
						</a>
					</span>
					<span class="EktronMyAccount_ShippingAddress_CancelLink EktronMyAccount_MutableControlContainer EktronMyAccount_InlineButton">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
							<xsl:attribute name="onclick" >Ektron_Ecommerce_MyAccountClass.GetObject('<xsl:value-of select="$ControlId" />').Cancel(); return (false);</xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/cancel"/>
						</a>
					</span>
					<span class="EktronMyAccount_AjaxBusyImageContainer" style="display: none;">
						<img alt="">
							<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
						</img>
					</span>
				</li>
			</ul>
		</div>

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
		<option>
			<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
			<xsl:if test="selected='true'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if>
			<xsl:value-of select="dropDownText"/>
		</option>
	</xsl:template>
	
	<xsl:template mode="view" match="property">
		<li class="EktronMyAccount_UserProperty">
			<xsl:choose>			
				<xsl:when test="valuetype='5'">
					<span class="EktronMyAccount_UserPropertyName"><xsl:value-of select="name"/></span>
					<span class="EktronMyAccount_UserPropertyDelimiter">:</span>
					<br/>
					<xsl:for-each select="subscriptions/subscription">
					<input type="checkbox" disabled="true" class="subscription"><xsl:if test="subscribed='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
						<xsl:attribute name="name">EktronMyAccount_UserPropertyValue<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_UserPropertyValue<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="value"/></xsl:attribute>
					</input>&#160;<xsl:value-of select="name"/><br/></xsl:for-each>
				</xsl:when>
				<xsl:otherwise><!--<span class="EktronMyAccount_UserPropertyId"><xsl:value-of select="id"/></span>-->
				<span class="EktronMyAccount_UserPropertyName"><xsl:value-of select="name"/></span>
				<span class="EktronMyAccount_UserPropertyDelimiter">:</span>
				<span class="EktronMyAccount_UserPropertyValue"><xsl:value-of select="value"/></span></xsl:otherwise>
			</xsl:choose>
		</li>
	</xsl:template>
	
	<xsl:template mode="edit" match="property">
		<li>
			<xsl:attribute name="class" >EktronMyAccount_SerializableContainer <xsl:if test="required='true'">EktronMyAccount_Required </xsl:if>EktronMyAccount_Edit_UserProperty</xsl:attribute>
			<label class="EktronMyAccount_Label_UserPropertyName">
				<xsl:attribute name="for">EktronMyAccount_PersonalInfo_CustomProperty_<xsl:value-of select="$ControlId"/>_<xsl:value-of select="id"/></xsl:attribute>
				<xsl:value-of select="name"/>:
			</label>
			<xsl:choose>			
				<xsl:when test="valuetype='7' or valuetype='8'"><select><xsl:if test="valuetype='8'"><xsl:attribute name="multiple">multiple</xsl:attribute></xsl:if><xsl:attribute name="name">EktronMyAccount_PersonalInfo_CustomPropertyItem<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute><xsl:attribute name="id">EktronMyAccount_PersonalInfo_CustomPropertyItem<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute><xsl:for-each select="options/option">
						<option><xsl:if test="selected='True'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if><xsl:attribute name="value"><xsl:value-of select="value"/></xsl:attribute><xsl:value-of select="name"/></option>
					</xsl:for-each></select></xsl:when>
				<xsl:when test="valuetype='5'">
					<br/>
					<xsl:for-each select="subscriptions/subscription">
					<input type="checkbox" class="subscription"><xsl:if test="subscribed='true'"><xsl:attribute name="checked">checked</xsl:attribute></xsl:if>
						<xsl:attribute name="name">EktronMyAccount_PersonalInfo_CustomPropertyItem<xsl:value-of select="../../id"/>-<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_PersonalInfo_CustomPropertyItem<xsl:value-of select="../../id"/>-<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="id"/></xsl:attribute>
					</input>&#160;<xsl:value-of select="name"/><br/></xsl:for-each>
				</xsl:when>
				<xsl:otherwise><input>
						<xsl:attribute name="name">EktronMyAccount_PersonalInfo_CustomPropertyItem<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronMyAccount_PersonalInfo_CustomPropertyItem<xsl:value-of select="id"/>_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="value"/></xsl:attribute>
					</input></xsl:otherwise>
			</xsl:choose>
			
		</li>
	</xsl:template>
	
	<xsl:template name="MakeUnauthenticatedUI">
		<h3 class="EktronMyAccount_NotAuthenticatedTitle"><xsl:value-of select="/root/resourceStrings/unrecognizedUser"/></h3>
		<p class="EktronMyAccount_NotAuthenticatedMessage"><xsl:value-of select="/root/resourceStrings/youAreNotLoggedIn"/></p>
	</xsl:template>

	<xsl:template name="MakeErrorUI">
		<h3 class="EktronMyAccount_ErrorText"><b>Error Occurred</b></h3>
		<p class="EktronMyAccount_ErrorText"><b><xsl:value-of select="/root/errorMessage"/></b></p>
	</xsl:template>

	<xsl:template match="javascriptResourceStrings">
		<xsl:for-each select="node()[text()]" >
			<input type="hidden" class="javascriptResourceString">
				<xsl:attribute name="id" >EktronMyAccount_<xsl:value-of select="$ControlId"/>_<xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="name" ><xsl:value-of select="name()"/></xsl:attribute>
				<xsl:attribute name="value" ><xsl:value-of select="./child::text()"/></xsl:attribute>
			</input>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>