<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeSubmitOrderUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeSubmitOrderUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_SubmitOrder EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_SubmitOrder_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeSubmitOrderUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeSubmitOrderUIContents">
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
			<div class="past">
				<div class="bluechecked">
					<a target="_self" ><xsl:attribute name="href"><xsl:value-of select="/root/serverVariables/url"/>?phase=ReviewOrder</xsl:attribute><xsl:value-of select="/root/resourceStrings/review"/></a>
				</div>
			</div>
			<div class="present">
				<div class="blue">
					<xsl:value-of select="/root/resourceStrings/payment"/>
				</div>
			</div>
			<div class="wizardEnd"> </div>
		</div>

		<div class="EktronCheckout_AuthError">
			<p class="EktronCheckout_ErrorText"><b><xsl:value-of select="/root/authorizationErrorMessage"/></b></p>
		</div>

		<xsl:choose>
			<xsl:when test="/root/submitOrder/paymentsAccepted&gt;1">
			  <div class="EktronCheckout_PaymentType EktronCheckout_NotRequired EktronCheckout_SerializableContainer">
				  <label class="EktronCheckout_PaymentTypeTitle">
					  <xsl:attribute name="for">EktronCheckout_SubmitOrder_PaymentTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/paymentMethod"/>
				  </label>
				  <select class="EktronCheckout_PaymentTypeSelect" >
					  <xsl:attribute name="name">EktronCheckout_SubmitOrder_PaymentTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <xsl:attribute name="id">EktronCheckout_SubmitOrder_PaymentTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').PaymentTypeChanged(); return (false);</xsl:attribute>
				<xsl:if test="/root/submitOrder/acceptsCCPayment='true'"><option value="0"><xsl:if test="/root/submitOrder/defaultPaymentMethod='cc'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if><xsl:value-of select="/root/resourceStrings/paymentMethodCreditCard"/></option></xsl:if>
				<xsl:if test="/root/submitOrder/acceptsCheckPayment='true'"><option value="1"><xsl:if test="/root/submitOrder/defaultPaymentMethod='check'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if><xsl:value-of select="/root/resourceStrings/paymentMethodCheck"/></option></xsl:if>
				<xsl:if test="/root/submitOrder/acceptsPayPalPayment='true'"><option value="2"><xsl:if test="/root/submitOrder/defaultPaymentMethod='paypal'"><xsl:attribute name="selected">selected</xsl:attribute></xsl:if><xsl:value-of select="/root/resourceStrings/paymentMethodPayPal"/></option></xsl:if>
				<!-- <xsl:if test="/root/submitOrder/acceptsGooglePayment='true'"><option value="3"><xsl:value-of select="/root/resourceStrings/paymentMethodGoogle"/></option></xsl:if> -->
				  </select>
			  </div>
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_PaymentType EktronCheckout_NotRequired EktronCheckout_SerializableContainer">
					<input type="hidden">
						<xsl:attribute name="name">EktronCheckout_SubmitOrder_PaymentTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="id">EktronCheckout_SubmitOrder_PaymentTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/submitOrder/defaultPaymentId"/></xsl:attribute>
					</input>
				</div>
			</xsl:otherwise>
		</xsl:choose>

    <div>
		<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='paypal'">EktronCheckout_PayPal</xsl:when><xsl:otherwise>EktronCheckout_PayPal EktronCheckout_Hidden</xsl:otherwise></xsl:choose></xsl:attribute>
      <!-- paypal -->
      <div class="EktronCheckout_PayPal_Image EktronCheckout_SerializableContainer">
        
        <img src="//www.paypal.com/en_US/i/logo/PayPal_mark_37x23.gif"/>
		  
      <input type="hidden">
				<xsl:attribute name="name">EktronCheckout_SubmitOrder_Token_<xsl:value-of select="$ControlId"/></xsl:attribute>
				<xsl:attribute name="id">EktronCheckout_SubmitOrder_Token_<xsl:value-of select="$ControlId"/></xsl:attribute>
        <xsl:attribute name="value"><xsl:value-of select="/root/submitOrder/PayPalPaymentToken"/></xsl:attribute>
			</input>
      <input type="hidden">
				<xsl:attribute name="name">EktronCheckout_SubmitOrder_PayerId_<xsl:value-of select="$ControlId"/></xsl:attribute>
				<xsl:attribute name="id">EktronCheckout_SubmitOrder_PayerId_<xsl:value-of select="$ControlId"/></xsl:attribute>
				<xsl:attribute name="value"><xsl:value-of select="/root/submitOrder/PayPalPaymentPayerId"/></xsl:attribute>
			</input>
		  
      </div>
      <br />
      
      <!-- paypal -->
    </div>
    
    <div class="EktronCheckout_Google EktronCheckout_Hidden">
      <!-- google -->
      <!-- google -->
    </div >
    
    <div>
		<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='check'">EktronCheckout_Check</xsl:when><xsl:otherwise>EktronCheckout_Check EktronCheckout_Hidden</xsl:otherwise></xsl:choose></xsl:attribute>
      <!-- check -->      
      <div>
		  <xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='check'">EktronCheckout_BankName EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_BankName EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
			  <label class="EktronCheckout_BankNameTitle">
				  <xsl:attribute name="for">EktronCheckout_SubmitOrder_BankName_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/bankName"/>
			  </label>
			  <input maxlength="20" autocomplete="off" >
				  <xsl:attribute name="name">EktronCheckout_SubmitOrder_BankName_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="id">EktronCheckout_SubmitOrder_BankName_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="onkeypress" >return Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').FilterSpecialCharacters(event);</xsl:attribute>
			  </input>
		  </div>
      <div>
		  <xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='check'">EktronCheckout_BankAccountNumber EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_BankAccountNumber EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
			  <label class="EktronCheckout_BankAccountNumberTitle">
				  <xsl:attribute name="for">EktronCheckout_SubmitOrder_BankAccountNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/bankaccountNumber"/>
			  </label>
			  <input maxlength="20" class="EktronCheckout_NumericField" autocomplete="off" >
				  <xsl:attribute name="name">EktronCheckout_SubmitOrder_BankAccountNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="id">EktronCheckout_SubmitOrder_BankAccountNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="onkeypress" >return Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').FilterNonNumerics(event);</xsl:attribute>
			  </input>
		  </div>
      <div>
		  <xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='check'">EktronCheckout_BankABARouting EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_BankABARouting EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
			  <label class="EktronCheckout_BankABARoutingTitle">
				  <xsl:attribute name="for">EktronCheckout_SubmitOrder_BankABARouting_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/bankABARouting"/>
			  </label>
			  <input maxlength="9" class="EktronCheckout_NumericField" autocomplete="off" >
				  <xsl:attribute name="name">EktronCheckout_SubmitOrder_BankABARouting_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="id">EktronCheckout_SubmitOrder_BankABARouting_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="onkeypress" >return Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').FilterNonNumerics(event);</xsl:attribute>
			  </input>
		  </div>
		<div class="EktronCheckout_RoutingAccountNumberHelp">
			<div class="innerContainer" style="display: none;">
				<img alt="routing and account number help image">
					<xsl:attribute name="src"><xsl:value-of select="/root/appImagePath"/>Commerce/Checkout/1033/check_routing_account.gif</xsl:attribute>
				</img>
			</div>
			<span class="EktronCheckout_checkHelpLabel">
				<img alt="small routing and account number help image">
					<xsl:attribute name="src"><xsl:value-of select="/root/appImagePath"/>Commerce/Checkout/check_routing_account_sm.gif</xsl:attribute>
				</img>
			</span>
		</div>
      <div class="EktronCheckout_RequiredNotice">
        <xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
      </div>
    <!-- check -->
    </div>

    <div class="EktronCheckout_CreditCard"><!-- cc -->
		<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='cc'">EktronCheckout_CreditCard</xsl:when><xsl:otherwise>EktronCheckout_CreditCard EktronCheckout_Hidden</xsl:otherwise></xsl:choose></xsl:attribute>
		<div>
			<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='cc'">EktronCheckout_CreditCardType EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_CreditCardType EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
			  <label class="EktronCheckout_CreditCardTypeTitle">
				  <xsl:attribute name="for">EktronCheckout_SubmitOrder_CreditCardTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/creditCardType"/>
			  </label>
			  <select class="EktronCheckout_CreditCardTypeSelect" >
				  <xsl:attribute name="name">EktronCheckout_SubmitOrder_CreditCardTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="id">EktronCheckout_SubmitOrder_CreditCardTypeSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').CardTypeChanged('EktronCheckout_SubmitOrder_CreditCardTypeSelect_<xsl:value-of select="$ControlId"/>', 'EktronCheckout_SubmitOrder_CCNumber_<xsl:value-of select="$ControlId"/>'); return (false);</xsl:attribute>				
				  <option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
				  <xsl:apply-templates select="/root/submitOrder/cardTypes/cardType" />
			  </select>
		  </div>
      <div>
        <xsl:attribute name="class">EktronCheckout_CardHolderName EktronCheckout_SerializableContainer EktronCheckout_MaestroCard EktronCheckout_SoloCardEktron Checkout_NotRequired EktronCheckout_Required_Hidden</xsl:attribute>
        <label class="EktronCheckout_CardHolderNameTitle">
          <xsl:attribute name="for">EktronCheckout_SubmitOrder_CardHolderName_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <span class="EktronCheckout_RequiredSymbol">*</span>
          <xsl:value-of select="/root/resourceStrings/cardHolderName"/>
        </label>
        <input class="wideTextField" autocomplete="off" >
          <xsl:attribute name="name">EktronCheckout_SubmitOrder_CardHolderName_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <xsl:attribute name="id">EktronCheckout_SubmitOrder_CardHolderName_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <xsl:attribute name="value"><xsl:value-of select="/root/userInfo/billingName"/></xsl:attribute>
          <xsl:attribute name="data-ektron-validation-minimumLength">1</xsl:attribute>
        </input>
      </div>
		<div>
			<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='cc'">EktronCheckout_CreditCardNumber EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_CreditCardNumber EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
			  <label class="EktronCheckout_CreditCardNumberTitle">
				  <xsl:attribute name="for">EktronCheckout_SubmitOrder_CCNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/creditCardNumber"/>
			  </label>
			  <input maxlength="20" class="EktronCheckout_NumericField" autocomplete="off" >
				  <xsl:attribute name="name">EktronCheckout_SubmitOrder_CCNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="id">EktronCheckout_SubmitOrder_CCNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="onkeypress" >return Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').FilterNonNumerics(event);</xsl:attribute>
			  </input>
		  </div>
      
      <div>
          <xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='cc'">EktronCheckout_CCID EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_CCID EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
			  <label class="EktronCheckout_CCIDTitle">
				  <xsl:attribute name="for">EktronCheckout_SubmitOrder_CCID_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/ccid"/>
			  </label>
			  <input maxlength="4" class="EktronCheckout_NumericField" autocomplete="off" >
				  <xsl:attribute name="name">EktronCheckout_SubmitOrder_CCID_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="id">EktronCheckout_SubmitOrder_CCID_<xsl:value-of select="$ControlId"/></xsl:attribute>
				  <xsl:attribute name="onkeypress" >return Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').FilterNonNumerics(event);</xsl:attribute>
				  <xsl:attribute name="data-ektron-validation-minimumLength">3</xsl:attribute>
			  </input>
		  </div>
      
      <div class="EktronCheckout_StartMonth EktronCheckout_MaestroCard EktronCheckout_SoloCard">
        <span class="EktronCheckout_StartDateTitle">
          <xsl:value-of select="/root/resourceStrings/startDate"/>
        </span>
        <span>
          <xsl:attribute name="class">EktronCheckout_StartMonth EktronCheckout_SerializableContainer EktronCheckout_MaestroCard EktronCheckout_SoloCardEktron Checkout_NotRequired EktronCheckout_Required_Hidden</xsl:attribute>
          <label class="EktronCheckout_StartDateMonthTitle">
            <xsl:attribute name="for">EktronCheckout_SubmitOrder_StartMonthSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <span class="EktronCheckout_RequiredSymbol">*</span>
            <xsl:value-of select="/root/resourceStrings/month"/>
          </label>
          <select class="EktronCheckout_StartMonthSelect" >
            <xsl:attribute name="name">EktronCheckout_SubmitOrder_StartMonthSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:attribute name="id">EktronCheckout_SubmitOrder_StartMonthSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <xsl:attribute name="onchange">Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SetValidationTestSkip(this, '.EktronCheckout_StartMonth', '.EktronCheckout_CreditCardIssueNumber'); return (false);</xsl:attribute>
            <option value="0">
              <xsl:value-of select="/root/resourceStrings/selectHyphen"/>
            </option>
            <option value="01">01</option>
            <option value="02">02</option>
            <option value="03">03</option>
            <option value="04">04</option>
            <option value="05">05</option>
            <option value="06">06</option>
            <option value="07">07</option>
            <option value="08">08</option>
            <option value="09">09</option>
            <option value="10">10</option>
            <option value="11">11</option>
            <option value="12">12</option>
          </select>
        </span>

        <span>
          <xsl:attribute name="class">EktronCheckout_StartMonth EktronCheckout_SerializableContainer EktronCheckout_MaestroCard EktronCheckout_SoloCardEktron Checkout_NotRequired EktronCheckout_Required_Hidden</xsl:attribute>
          <label class="EktronCheckout_StartDateYearTitle">
            <xsl:attribute name="for">EktronCheckout_SubmitOrder_StartYearSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <span class="EktronCheckout_RequiredSymbol">*</span>
            <xsl:value-of select="/root/resourceStrings/year"/>
          </label>
          <select class="EktronCheckout_StartYearSelect" >
            <xsl:attribute name="name">EktronCheckout_SubmitOrder_StartYearSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:attribute name="id">EktronCheckout_SubmitOrder_StartYearSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
            <xsl:apply-templates select="/root/submitOrder/startingYears/year" />
            <option value="0" selected="selected"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
          </select>
        </span>
      </div>

      <div>
        <xsl:attribute name="class">EktronCheckout_CreditCardIssueNumber EktronCheckout_SerializableContainer EktronCheckout_MaestroCard EktronCheckout_SoloCard EktronCheckout_NotRequired EktronCheckout_Required_Hidden</xsl:attribute>
        <label class="EktronCheckout_CreditCardIssueNumberTitle">
          <xsl:attribute name="for">EktronCheckout_SubmitOrder_CreditCardIssueNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <span class="EktronCheckout_RequiredSymbol">*</span>
          <xsl:value-of select="/root/resourceStrings/issueNumber"/>
        </label>
        <input class="EktronCheckout_NumericField" autocomplete="off" >
          <xsl:attribute name="name">EktronCheckout_SubmitOrder_CreditCardIssueNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <xsl:attribute name="id">EktronCheckout_SubmitOrder_CreditCardIssueNumber_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <xsl:attribute name="onkeypress" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').FilterNonNumerics(event);
            return Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SetValidationTestSkip(this, '.EktronCheckout_CreditCardIssueNumber', '.EktronCheckout_StartMonth');</xsl:attribute>
          <xsl:attribute name="data-ektron-validation-minimumLength">1</xsl:attribute>
        </input>
      </div>

      <div class="EktronCheckout_ExpirationDate">
			  <span class="EktronCheckout_ExpirationDateTitle"><xsl:value-of select="/root/resourceStrings/expirationDate"/></span>
			<span>
				<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='cc'">EktronCheckout_ExpirationMonth EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_ExpirationMonth EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
				  <label class="EktronCheckout_ExpirationDateMonthTitle">
					  <xsl:attribute name="for">EktronCheckout_SubmitOrder_ExpirationMonthSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/month"/> 
				  </label>
				  <select class="EktronCheckout_ExpirationMonthSelect" >
					  <xsl:attribute name="name">EktronCheckout_SubmitOrder_ExpirationMonthSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <xsl:attribute name="id">EktronCheckout_SubmitOrder_ExpirationMonthSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
					  <option value="01">01</option>
					  <option value="02">02</option>
					  <option value="03">03</option>
					  <option value="04">04</option>
					  <option value="05">05</option>
					  <option value="06">06</option>
					  <option value="07">07</option>
					  <option value="08">08</option>
					  <option value="09">09</option>
					  <option value="10">10</option>
					  <option value="11">11</option>
					  <option value="12">12</option>
				  </select>
			  </span>

			<span>
				<xsl:attribute name="class"><xsl:choose><xsl:when test ="/root/submitOrder/defaultPaymentMethod='cc'">EktronCheckout_ExpirationYear EktronCheckout_Required EktronCheckout_SerializableContainer</xsl:when><xsl:otherwise>EktronCheckout_ExpirationYear EktronCheckout_Required_Hidden EktronCheckout_SerializableContainer</xsl:otherwise></xsl:choose></xsl:attribute>
				  <label class="EktronCheckout_ExpirationDateYearTitle">
					  <xsl:attribute name="for">EktronCheckout_SubmitOrder_ExpirationYearSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <span class="EktronCheckout_RequiredSymbol">*</span><xsl:value-of select="/root/resourceStrings/year"/>
				  </label>
				  <select class="EktronCheckout_ExpirationYearSelect" >
					  <xsl:attribute name="name">EktronCheckout_SubmitOrder_ExpirationYearSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <xsl:attribute name="id">EktronCheckout_SubmitOrder_ExpirationYearSelect_<xsl:value-of select="$ControlId"/></xsl:attribute>
					  <option value="0"><xsl:value-of select="/root/resourceStrings/selectHyphen"/></option>
					  <xsl:apply-templates select="/root/submitOrder/expirationYears/year" />
				  </select>
			  </span>
		  </div>

      <div>
        <xsl:attribute name="class">EktronCheckout_CardBankName EktronCheckout_SerializableContainer EktronCheckout_MaestroCard EktronCheckout_SoloCardEktron Checkout_NotRequired EktronCheckout_Required_Hidden</xsl:attribute>
        <label class="EktronCheckout_CardBankNameTitle">
          <xsl:attribute name="for">EktronCheckout_SubmitOrder_CardBankName_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <span class="EktronCheckout_RequiredSymbol">*</span>
          <xsl:value-of select="/root/resourceStrings/cardBankName"/>
        </label>
        <input class="wideTextField" autocomplete="off" >
          <xsl:attribute name="name">EktronCheckout_SubmitOrder_CardBankName_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <xsl:attribute name="id">EktronCheckout_SubmitOrder_CardBankName_<xsl:value-of select="$ControlId"/></xsl:attribute>
          <xsl:attribute name="data-ektron-validation-minimumLength">1</xsl:attribute>
        </input>
      </div>

      <div class="EktronCheckout_RequiredNotice">
        <xsl:value-of select="/root/resourceStrings/indicateRequiredFields"/>
      </div>
    <!-- cc -->
    </div>
		<!--<div class="EktronCheckout_RedeemGiftCard">
			<a class="EktronCheckout_RedeemGiftCardTitle">
				<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
				Redeem Gift Card
			</a>
		</div>-->

		<div class="AdvisoryMessage">
			<xsl:if test="/root/cartValidationErrorMessage and /root/cartValidationErrorMessage!=''">
				<xsl:attribute name="style">display: block;</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="/root/cartValidationErrorMessage"/>
		</div>

		<div class="EktronCheckout_SubmitOrderBtn EktronCheckout_MutableControlContainer">
			<xsl:if test="/root/cartValidationErrorMessage and /root/cartValidationErrorMessage!=''">
				<xsl:attribute name="style">display: none;</xsl:attribute>
			</xsl:if>
			<!--
			<input class="EktronCheckout_SubmitOrderBtn" type="button" value="Submit Order" >
				<xsl:attribute name="onclick" >getElementById('EktronCheckout_SubmitOrder_<xsl:value-of select="$ControlId"/>').value="1"; document.forms[0].submit(); return (false);</xsl:attribute>
			</input>
			-->
			<input class="EktronCheckout_SubmitOrderBtn" type="button" >
				<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SubmitCheckoutOrder("EktronCheckout_SubmitOrder"); return (false);</xsl:attribute>
				<xsl:attribute name="value"><xsl:value-of select="/root/resourceStrings/submitOrder"/></xsl:attribute>
			</input>
		</div>

		<div class="EktronCheckout_SubmitOrder EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
			<span class="EktronCheckout_UserInfo_PreviousPageLink">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').PreviousPage(); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/previousPage"/>
				</a>
			</span>
			<span class="EktronCheckout_AjaxBusyImageContainer">
				<img alt="">
					<xsl:attribute name="src"><xsl:value-of select="/root/loadingImage"/></xsl:attribute>
				</img>
			</span>
		</div>
	</xsl:template>

</xsl:stylesheet>