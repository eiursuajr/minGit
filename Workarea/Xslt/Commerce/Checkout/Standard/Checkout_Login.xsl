<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:template name="MakeLoginUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeLoginUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_AccountLogin EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_AccountLogin_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeLoginUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeLoginUIContents">
		<div class="EktronCheckout_ReturningCustomer">
			<span><xsl:value-of select="/root/resourceStrings/returningCustomer"/></span><span class="spacingContainer"> </span><xsl:value-of select="/root/resourceStrings/enterEmailAndPassword"/>
		</div>
		<div class="EktronCheckout_AuthError">
			<p class="EktronCheckout_ErrorText"><b><xsl:value-of select="/root/authorizationErrorMessage"/></b></p>
		</div>
    <xsl:if test="/root/paypalInfo/showMessage='true'">
      <div class="EktronCheckout_UserExistPayPal">
  			<p class="EktronCheckout_ExternalPaymentText"><b><xsl:value-of select="/root/resourceStrings/paypalUserMessage"/></b></p>
		  </div>
    </xsl:if>
		<div class="EktronCheckout_LoginBlock">
			<ul class="EktronCheckout_Rows_Container">
				<li class="EktronCheckout_LoginEmail">
					<span><xsl:value-of select="/root/resourceStrings/emailAddress"/></span>
          <input type="text"><xsl:attribute name="value"><xsl:if test="/root/paypalInfo/isUserNameAvailable='false'"><xsl:value-of select="/root/userInfo/emailAddress"/></xsl:if></xsl:attribute></input>
				</li>
				<li class="EktronCheckout_LoginPassword">
					<span><xsl:value-of select="/root/resourceStrings/password"/></span>
					<input type="password" />
				</li>
				<li class="EktronCheckout_LoginButton">
          <span>&#160;</span>
					<input type="submit">
						<xsl:attribute name="value"><xsl:value-of select="/root/resourceStrings/login"/></xsl:attribute>
						<xsl:attribute name="onclick" >return (Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').Login());</xsl:attribute>
					</input>
					<input type="hidden" value="">
						<xsl:attribute name="id" >EktronCheckout_LoginBlock_Hidden_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="name" >EktronCheckout_LoginBlock_Hidden_<xsl:value-of select="$ControlId"/></xsl:attribute>
					</input>
				</li>
				<xsl:if test="not(/root/urlRecoverPassword='')">
					<li class="EktronCheckout_RecoverPasswordLink">
						<a>
							<xsl:attribute name="href"><xsl:value-of select="/root/urlRecoverPassword"/></xsl:attribute>
							<xsl:value-of select="/root/resourceStrings/recoverPassword"/>
						</a>
					</li>
				</xsl:if>
			</ul>
		</div>
		<div class="EktronCheckout_NewCustomer">
			<xsl:choose>
				<xsl:when test="/root/enableWizardMode='false'">
					<span><xsl:value-of select="/root/resourceStrings/newCustomer"/></span><span class="spacingContainer"> </span><xsl:value-of select="/root/resourceStrings/enterFollowingInformation"/>
				</xsl:when>
				<xsl:otherwise>
					<span class="EktronCheckout_NewCustomerAlert"><xsl:value-of select="/root/resourceStrings/newCustomer"/></span><span class="spacingContainer"> </span><xsl:value-of select="/root/resourceStrings/enterInformationOnFollowingPages"/>
					<div class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
						<span class="EktronCheckout_UserInfo_NextPageLink">
							<a>
								<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').NewCustomerCheckout(); return (false);</xsl:attribute>
								<xsl:value-of select="/root/resourceStrings/createProfileAndCheckout"/>
							</a>
						</span>
						<span class="EktronCheckout_AjaxBusyImageContainer">
							<img alt="">
								<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
							</img>
						</span>
					</div>
				</xsl:otherwise>
			</xsl:choose>
		</div>
    <xsl:if test="/root/allowGuestCheckout='true'">
      <div class="EktronCheckout_GuestCustomer">
			  <xsl:choose>
				  <xsl:when test="/root/enableWizardMode='false'">
					  <span><xsl:value-of select="/root/resourceStrings/guestCustomer"/></span><span class="spacingContainer"> </span><xsl:value-of select="/root/resourceStrings/alwayCreateProfileLater"/>
				  </xsl:when>
				  <xsl:otherwise>
					  <span class="EktronCheckout_NewCustomerAlert"><xsl:value-of select="/root/resourceStrings/guestCustomer"/></span><span class="spacingContainer"> </span><xsl:value-of select="/root/resourceStrings/alwaysCreateProfileLater"/>
					  <div class="EktronCheckout_UserInfo_BillingAddressFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
						  <span class="EktronCheckout_UserInfo_NextPageLink">
							  <a>
								  <xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
								  <xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').GuestCheckout(); return (false);</xsl:attribute>
								  <xsl:value-of select="/root/resourceStrings/checkoutWithoutProfile"/>
							  </a>
						  </span>
						  <span class="EktronCheckout_AjaxBusyImageContainer">
							  <img alt="">
								  <xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
							  </img>
						  </span>
					  </div>
				  </xsl:otherwise>
			  </xsl:choose>
		  </div>
    </xsl:if>
	</xsl:template>

</xsl:stylesheet>