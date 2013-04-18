<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<link rel="stylesheet" href="csslib/Community.css" type="text/css" />
		<div class="EktProfileCtl">
			<div class="EktProfileCtl_HeaderBar" >
				<xsl:value-of select="/root/user/displayname"/>
			</div>
			<div class="EktProfileCtl_Body" >
			<div class="EktProfileCtl_ImageContainer">
				<xsl:if test="/root/user/avatar[.!='']">
					<img id="avitarImageForUser_{/root/user/id}" >
						<xsl:attribute name="src"><xsl:value-of select="/root/user/avatar"/></xsl:attribute>
						<xsl:attribute name="alt">an image of <xsl:value-of select="/root/user/displayname"/></xsl:attribute>
					</img>
				</xsl:if>
			</div>
	  <div class="EktProfileCtl_ScrollableBlock" >
      <div class="EktProfileCtl_TagBlockContainer" >
        <div class="EktProfileCtl_TagNameContainer" >
          Personal Tags
        </div>
        <div class="EktProfileCtl_TagsContainer" >
          <xsl:for-each select="root/user/personaltags/personaltag">
            <div class="EktProfileCtl_TagItem" >
              <span class="EktProfileCtl_TagItemName" >
                <xsl:choose>
                  <xsl:when test="not(tagTemplate) or (tagTemplate='')">
                    <xsl:value-of select="tag"/>
                    <br/>
                  </xsl:when >
                  <xsl:otherwise>
                    <a class="EktProfileCtl_TagLinks"><xsl:attribute name="target"><xsl:value-of select="/root/tagTemplateTarget"/></xsl:attribute><xsl:attribute name="href"><xsl:value-of select="tagTemplate"/></xsl:attribute><xsl:value-of select="tag"/></a>
                  </xsl:otherwise>
                </xsl:choose>
              </span>
            </div>
          </xsl:for-each>
        </div>
      </div>
      
			<div class="EktProfileCtl_InfoContainer" >
				<div class="EktProfileCtl_ScreenNameContainer" >
					Screen Name: <xsl:value-of select="/root/user/displayname"/>
				</div>
				<xsl:if test="/root/enableEmailDisplay/text()='true'">
					<div class="EktProfileCtl_EmailContainer" >
						<span class="EktProfileCtl_EmailTitle">Email Address: </span><xsl:value-of select="/root/user/emailAddress"/>
					</div>
				</xsl:if>
				<div class="EktProfileCtl_PropertiesContainer" >
					<div class="EktProfileCtl_PropertiesTitle" >Properties:</div>
					<xsl:for-each select="root/user/attributes/attribute">
						<div class="EktProfileCtl_PropertiesItem" >
							<span class="EktProfileCtl_PropertiesItemName" >
								<xsl:choose>
								<xsl:when test ="name/text()='Subscriptions'">
                                </xsl:when >
									<xsl:otherwise><xsl:value-of select="name"/>:</xsl:otherwise>
								</xsl:choose>
                            </span>
							<span class="EktProfileCtl_PropertiesItemValue" >
								<xsl:choose>
									<xsl:when test ="name/text()='Private Profile'">
										<xsl:choose>
											<xsl:when test="value=1">
												<input type ="checkbox" name="private" disabled="true" checked="checked"></input>
											</xsl:when>
											<xsl:otherwise>
												<input type ="checkbox" name="private" disabled="true" ></input>
											</xsl:otherwise>
										</xsl:choose>
                                    </xsl:when>
									<xsl:when test ="name/text()='Subscriptions'">
										
									</xsl:when >
									<xsl:otherwise>
										<xsl:value-of select="value"/>
									</xsl:otherwise>
								</xsl:choose>
							</span>
						</div>
					</xsl:for-each>
				</div>
			</div>
			<div class="EktProfileCtl_EditLinkContainer">
				<xsl:if test="/root/user/edit/text()='true'">
					<a title="Edit Profile" class="ek_thickbox">
						<xsl:attribute name="href" ><xsl:value-of select="/root/appath" />EditUserProfile.aspx?id=<xsl:value-of select="/root/user/id"/>&amp;EkTB_iframe=true&amp;height=400&amp;width=400</xsl:attribute>
						Edit profile
					</a>
				</xsl:if >
			</div>
		  </div>
		</div>
	</div>
	</xsl:template>
</xsl:stylesheet>