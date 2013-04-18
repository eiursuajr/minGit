<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name="ControlId" select="/root/clientId" />
	<xsl:variable name="HrefTarget" ><xsl:value-of select="/root/appPath"/>JavascriptRequired.aspx</xsl:variable>
	<xsl:variable name="AppImagePath" select="/root/appImagePath" />

	<xsl:template match="/root">
		<div class="EktronProductControl">
			<xsl:attribute name="id">EktronProductControl_<xsl:value-of select="/root/clientId"/></xsl:attribute>

			<xsl:copy-of select="/root/markupTransformed/node()"/>

			<table border="0" width="100%">
				<tbody>
					<tr>
						<td width="50%">
							<xsl:apply-templates select="/root/imageGalleryItems/imageGalleryItem" />
						</td>
						<td width="50%">
							<xsl:if test="/root/kitOptionGroups/kitOptionGroup">
								Base Price:<xsl:text xml:space="preserve" > </xsl:text>
								<span class="basePrice">
									<xsl:value-of select="/root/salePrice"/>
								</span><br />
							</xsl:if>
							Subtotal:<xsl:text xml:space="preserve" > </xsl:text>
							<span class="price">
								<xsl:attribute name="id">ekproductprice<xsl:value-of select="/root/clientId"/></xsl:attribute>
							</span>
							<input type="hidden">
								<xsl:attribute name="id">ekproductsaleprice<xsl:value-of select="/root/clientId"/></xsl:attribute>
								<xsl:attribute name="name">ekproductsaleprice<xsl:value-of select="/root/clientId"/></xsl:attribute>
								<xsl:attribute name="value"><xsl:value-of select="/root/salePrice"/></xsl:attribute>
							</input>
							<xsl:if test="/root/recurrence/recurrenceInfo and /root/recurrence/recurrenceInfo!=''">
								<xsl:text xml:space="preserve" > </xsl:text>
								<xsl:value-of select="/root/recurrence/recurrenceInfo"/>
							</xsl:if>
						</td>
					</tr>
				</tbody>
			</table>

			<xsl:if test="/root/attributeItems/attributeItem">
				<table border="0" width="100%">
					<tbody>
						<xsl:apply-templates select="/root/attributeItems/attributeItem" />
					</tbody>
				</table>
			</xsl:if>

			<xsl:if test="/root/variantItems/variantItem">
				<p>
					<strong>Variants:</strong>
				</p>
				<table cellpadding="5" border="0" width="100%">
					<tbody>
						<xsl:apply-templates select="/root/variantItems/variantItem" />
					</tbody>
				</table>
			</xsl:if>

			<xsl:if test="/root/kitOptionGroups/kitOptionGroup">
				<p>
					<strong>Options</strong>
				</p>
				<input type="hidden">
					<xsl:attribute name="id">ekkitconfig<xsl:value-of select="/root/clientId"/></xsl:attribute>
					<xsl:attribute name="name">ekkitconfig<xsl:value-of select="/root/clientId"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="iConfig"/></xsl:attribute>
				</input>
				<table cellspacing="1" cellpadding="0" border="0" width="100%">
					<tbody>
						<xsl:apply-templates select="/root/kitOptionGroups/kitOptionGroup" />
					</tbody>
				</table>
				<p>
					<xsl:if test="/root/kitOptionGroups/kitOptionGroup">
						Base Price:<xsl:text xml:space="preserve" > </xsl:text>
						<span class="kitBasePrice">
							<xsl:value-of select="/root/salePrice"/>
						</span><br />
					</xsl:if>
					<strong>
						Subtotal:<xsl:text xml:space="preserve"> </xsl:text>
						<span class="kitPrice">
							<xsl:attribute name="id">ekproductkitprice<xsl:value-of select="/root/clientId"/></xsl:attribute>
							<xsl:value-of select="/root/subtotal"/>
						</span>
					</strong>
				</p>
			</xsl:if>
			
			<xsl:if test="/root/bundleItems/bundleItem">
				<p><strong>This Bundle Includes</strong></p>
				<table cellpadding="5" width="100%" border="0">
				  <tbody>
						<xsl:apply-templates select="/root/bundleItems/bundleItem" />
				  </tbody>
			  </table>
			</xsl:if>
			
			<xsl:if test="pricingTable and pricingTable!=''">
				<input type="hidden" >
					<xsl:attribute name="id">pricingTable_<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:attribute name="name">pricingTable<xsl:value-of select="$ControlId"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="pricingTable"/></xsl:attribute>
				</input>
			</xsl:if>
		</div>

	</xsl:template>

	<xsl:template match="imageGalleryItem">
		<!-- xsl:if test="includedInGallery='true'">
			<a class="thickbox" rel="product_images">
				<xsl:attribute name="href">?id=<xsl:value-of select="id"/></xsl:attribute>
				<xsl:attribute name="title"><xsl:value-of select="title"/></xsl:attribute>
				<xsl:attribute name="onclick"><xsl:value-of select="clickAction"/></xsl:attribute>
				<img>
					<xsl:attribute name="src"><xsl:value-of select="source"/></xsl:attribute>
					<xsl:attribute name="title"><xsl:value-of select="title"/></xsl:attribute>
				</img>
			</a>
		</xsl:if -->
	</xsl:template>

	<xsl:template match="attributeItem">
		<tr>
			<td width="100%">
				<xsl:value-of select="name" />: <xsl:value-of select="value" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="variantItem">
		<tr>
			<td align="center" width="5%">
				<input type="radio" class="price_modifier" >
					<xsl:attribute name="id">var_<xsl:value-of select="/root/clientId"/>_<xsl:value-of select="elementIndex"/></xsl:attribute>
					<xsl:attribute name="name">var_<xsl:value-of select="/root/clientId"/></xsl:attribute>
					<xsl:attribute name="data-ektron-variantid"><xsl:value-of select="id"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="itemIndex"/></xsl:attribute>
					<xsl:attribute name="data-ektron-kitgroupweight">0</xsl:attribute>
					<xsl:attribute name="onclick"><xsl:value-of select="onClick"/></xsl:attribute>
					<xsl:if test="selected='true'">
						<xsl:attribute name="checked">checked</xsl:attribute>
					</xsl:if>
				</input>
			</td>
			<td align="center" width="5%">
				<a>
					<xsl:attribute name="href">?id=<xsl:value-of select="id"/></xsl:attribute>
					<img border="0" >
						<xsl:attribute name="src"><xsl:value-of select="imageSource"/></xsl:attribute>
						<xsl:attribute name="title"><xsl:value-of select="title"/></xsl:attribute>
					</img>
				</a>
			</td>

			<td align="left" valign="top">
				<p>
					<a>
						<xsl:attribute name="href">?id=<xsl:value-of select="id"/></xsl:attribute>
						<strong>
							<xsl:value-of select="title"/>
						</strong>
					</a>
					<span class="listprice" ><xsl:value-of select="listPrice"/></span>
					<span class="saleprice" ><xsl:value-of select="salePrice"/></span>
					<br/>
				</p>
				<p>
					<xsl:value-of select="summary" disable-output-escaping ="yes"/>
					<br/>
				</p>
			</td>
		</tr>
		<tr>
			<td valign="top" colspan="3">
				<hr />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="kitOptionGroup">
		<tr>
			<td class="kitOptionGroup" colspan="2">
				<xsl:value-of select="name"/>
				<input type="hidden" >
					<xsl:attribute name="id"><xsl:value-of select="hiddenId"/></xsl:attribute>
					<xsl:attribute name="name"><xsl:value-of select="hiddenName"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="hiddenValue"/></xsl:attribute>
				</input>
			</td>
		</tr>
		<xsl:apply-templates select="groupItems/groupItem" />
	</xsl:template>
	
	<xsl:template match="groupItem">
		<tr>
			<td width="100" rowspan="1"> </td>
			<td class="groupItem" >
				<input type="radio" class="price_modifier" >
					<xsl:attribute name="id"><xsl:value-of select="buttonId"/></xsl:attribute>
					<xsl:attribute name="name"><xsl:value-of select="buttonName"/></xsl:attribute>
					<xsl:attribute name="onclick"><xsl:value-of select="buttonOnClick"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="itemIndex"/></xsl:attribute>
					<xsl:attribute name="data-ektron-kitgroupweight"><xsl:value-of select="groupIndex"/></xsl:attribute>
					<xsl:if test="selected='true'">
						<xsl:attribute name="checked">checked</xsl:attribute>
					</xsl:if>
				</input>
				<xsl:value-of select="name"/>&#160;&#160;&#160;<xsl:value-of select="priceModificationInfo"/>
				<input type="hidden" >
					<xsl:attribute name="id"><xsl:value-of select="optionsHiddenId"/></xsl:attribute>
					<xsl:attribute name="name"><xsl:value-of select="optionsHiddenName"/></xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="optionsHiddenValue"/></xsl:attribute>
				</input>
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="bundleItem">
        <tr>
			<td width="5%" align="center" class="bundleItemImage">
				<a>
					<xsl:attribute name="href">?id=<xsl:value-of select="id"/></xsl:attribute>
					<xsl:attribute name="title"><xsl:value-of select="title"/></xsl:attribute>
					<img border="0" >
						<xsl:attribute name="src"><xsl:value-of select="imageSource"/></xsl:attribute>
						<xsl:attribute name="alt"><xsl:value-of select="title"/></xsl:attribute>
					</img>
				</a>
			</td>
			<td valign="top" align="left" class="bundleItemSummary">
				<p class="summary">
					<a>
						<xsl:attribute name="href">?id=<xsl:value-of select="id"/></xsl:attribute>
						<xsl:attribute name="title"><xsl:value-of select="title"/></xsl:attribute>
						<strong>
							<xsl:value-of select="title"/>
						</strong>
					</a>
					<br />
					<xsl:value-of select="summary" disable-output-escaping ="yes"/>
					<br />
				</p>
			</td>
        </tr>
        <tr>
			<td valign="top" colspan="2" class="bundleItemSpacerRow"><hr/></td>
        </tr>
	</xsl:template>

</xsl:stylesheet>