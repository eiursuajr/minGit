<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeShippingMethodSelectUI">
		<xsl:choose>
			<xsl:when test="/root/supressFragmentContainer='true'">
				<xsl:call-template name="MakeShippingMethodSelectUIContents" />
			</xsl:when>
			<xsl:otherwise>
				<div class="EktronCheckout_ShippingMethod EktronCheckout_ProcBlock" >
					<xsl:attribute name="id" >EktronCheckout_ShippingMethod_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:call-template name="MakeShippingMethodSelectUIContents" />
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="MakeShippingMethodSelectUIContents">
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
				<xsl:attribute name="class" >present <xsl:if test="basketHasTangibleItems='false'">hiddenStep</xsl:if></xsl:attribute>
				<div class="blue">
					<xsl:value-of select="/root/resourceStrings/method"/>
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

		<span class="EktronCheckout_EstimateNotice"></span>

		<div class="EktronCheckout_ShippingMethod_ServiceMethodSelectContainer EktronCheckout_SerializableContainer">
			<table>
				<tbody>
					<xsl:apply-templates select="/root/shippingMethod/methodTypes/methodType" />
				</tbody>
			</table>
		</div>
		
		<div class="EktronCheckout_ShippingMethodFragment EktronCheckout_UserInfo_PageLinks EktronCheckout_MutableControlContainer">
			<span class="EktronCheckout_UserInfo_PreviousPageLink">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveShippingMethod_GetPreviousPage("EktronCheckout_ShippingMethod"); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/previousPage"/>
				</a>
			</span>
			<span class="EktronCheckout_UserInfo_NextPageLink">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$HrefTarget"/></xsl:attribute>
					<xsl:attribute name="onclick" >Ektron_Ecommerce_CheckoutClass.GetObject('<xsl:value-of select="$ControlId" />').SaveShippingMethod_GetNextPage("EktronCheckout_ShippingMethod"); return (false);</xsl:attribute>
					<xsl:value-of select="/root/resourceStrings/nextPage"/>
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