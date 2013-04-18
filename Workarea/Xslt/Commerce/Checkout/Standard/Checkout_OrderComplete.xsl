<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="MakeOrderCompleteUI">
		<div class="EktronCheckout_OrderComplete EktronCheckout_ProcBlock" >
			<h3><xsl:value-of select="/root/resourceStrings/orderComplete"/></h3>
			<p><b><xsl:value-of select="/root/resourceStrings/thankYou"/></b></p>
			<xsl:if test="/root/orderComplete/orderId!=''">
				<p><b><xsl:value-of select="/root/resourceStrings/orderId"/><span class="spacingContainer"> </span><xsl:value-of select="/root/orderComplete/orderId" /></b></p>
			</xsl:if>
			<p>
				<xsl:value-of select="/root/resourceStrings/orderApproved"/>
			</p>
			<xsl:if test="not(/root/urlShopping='')">
				<p class="EktronCheckout_ContinueShoppingLink">
					<a>
						<xsl:attribute name="href"><xsl:value-of select="/root/urlShopping"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/continueShopping"/>
					</a>
				</p>
			</xsl:if>
			<xsl:if test="/root/usingGuestCheckout='false' and not(/root/urlHistory='')">
				<p class="EktronCheckout_OrderHistoryLink">
					<a>
						<xsl:attribute name="href"><xsl:value-of select="/root/urlHistory"/></xsl:attribute>
						<xsl:value-of select="/root/resourceStrings/orderHistory"/>
					</a>
				</p>
			</xsl:if>
		</div>
	</xsl:template>
	
</xsl:stylesheet>