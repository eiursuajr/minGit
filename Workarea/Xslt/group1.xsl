<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
		<xsl:choose>
			<xsl:when test="result/displaymode='error'">ERROR: <xsl:value-of select="result/errormessage"/></xsl:when>
			<xsl:when test="result/displaymode='edit'"/>
			<xsl:when test="result/displaymode='users'"/>
			<xsl:otherwise>
				<div class="ektronCommunityGroup">
					<h3><xsl:value-of select="result/CommunityGroupData/GroupName" disable-output-escaping="yes"/></h3>
					<div class="ektronCommunityGroupWrapper">
						<div class="ektronCommunityGroupDetails">
							<div class="ektronCommunityGroupIconContainer">
								<img class="ektronCommunityGroupIcon">
									<xsl:attribute name="alt"><xsl:value-of select="result/CommunityGroupData/GroupName" disable-output-escaping="yes"/></xsl:attribute>
									<xsl:attribute name="title"><xsl:value-of select="result/CommunityGroupData/GroupName" disable-output-escaping="yes"/></xsl:attribute>
									<xsl:choose>
										<xsl:when test="not (result/CommunityGroupData/GroupImage) or (result/CommunityGroupData/GroupImage='')"><xsl:attribute name="src">[[ekimgpath]]group.jpg</xsl:attribute ></xsl:when>
										<xsl:otherwise><xsl:attribute name="src"><xsl:value-of select="result/CommunityGroupData/GroupImage"/></xsl:attribute></xsl:otherwise>
									</xsl:choose>
								</img>
							</div>
							<p class="ektronCommunityGroupDescription"><xsl:value-of select="result/CommunityGroupData/GroupLongDescription"/></p>
							<table>
								<tbody>
									<tr>
										<th>Tags:</th>
										<td><xsl:for-each select ="result/CommunityGroupData/GroupTags/GroupTag"><xsl:choose><xsl:when test="not (position()=1)">,&#160;</xsl:when></xsl:choose><xsl:choose><xsl:when test="not (TagLink='')"><a><xsl:attribute name="href"><xsl:value-of select="TagLink" disable-output-escaping="yes"/></xsl:attribute ><xsl:attribute name="target"><xsl:value-of select="result/tagtemplatetarget" disable-output-escaping="yes"/></xsl:attribute ><xsl:value-of select="TagText" disable-output-escaping="yes"/></a></xsl:when><xsl:otherwise><xsl:value-of select="TagText" disable-output-escaping="yes"/></xsl:otherwise></xsl:choose></xsl:for-each></td>
									</tr>
									<tr>
										<th>Type:</th>
										<td>
											<xsl:choose>
												<xsl:when test="result/CommunityGroupData/GroupEnroll='true'">Public Membership</xsl:when>
												<xsl:otherwise>Private Membership</xsl:otherwise>
											</xsl:choose>
										</td>
									</tr>
									<tr>
										<th>Founded: </th>
										<td><xsl:value-of select="result/CommunityGroupData/GroupCreatedDateInfo/GroupCreatedMonth"/>/<xsl:value-of select="result/CommunityGroupData/GroupCreatedDateInfo/GroupCreatedDay"/>/<xsl:value-of select="result/CommunityGroupData/GroupCreatedDateInfo/GroupCreatedYear"/></td>
									</tr>
									<tr>
										<th>Location: </th>
										<td><xsl:value-of select="result/CommunityGroupData/GroupLocation"/></td>
									</tr>
									<tr>
										<th>Members: </th>
										<td><xsl:value-of select="result/CommunityGroupData/TotalMember"/></td>
									</tr>
								</tbody>
							</table>
						</div>
						<div class="ektronCommunityGroupAdmin">
							<xsl:if test="(result/CommunityGroupData/GroupAdmin/Avatar) and not (result/CommunityGroupData/GroupAdmin/Avatar='')">
								<p class="ektronCommunityGroupAdminAvatar">
									<xsl:choose>
										<xsl:when test="result/groupadminlink=''">
											<img alt="Administrator Avatar" title="Administrator Avatar"><xsl:attribute name="src"><xsl:value-of select="result/CommunityGroupData/GroupAdmin/Avatar"/></xsl:attribute></img>
										</xsl:when>
										<xsl:otherwise>
											<a class="leaderLink" id="leaderLink">
												<xsl:attribute name="href"><xsl:value-of select="result/groupadminlink"/></xsl:attribute>
												<xsl:attribute name="target"><xsl:value-of select="result/groupadminlinktarget"/></xsl:attribute>
												<img alt="Administrator Avatar" title="Administrator Avatar"><xsl:attribute name="src"><xsl:value-of select="result/CommunityGroupData/GroupAdmin/Avatar"/></xsl:attribute></img>
											</a>
										</xsl:otherwise>
									</xsl:choose>
								</p>
							</xsl:if>
							<table class="ektronCommunityGroupAdministration">
								<tbody>
									<tr>
										<th>Group Admin:</th>
										<td>
											<xsl:choose>
												<xsl:when test="result/groupadminlink=''"><xsl:value-of select="result/CommunityGroupData/GroupAdmin/DisplayName "/></xsl:when>
												<xsl:otherwise>
													<a class="leaderLink" id="leaderLink">
														<xsl:attribute name="href"><xsl:value-of select="result/groupadminlink"/></xsl:attribute>
														<xsl:attribute name="target"><xsl:value-of select="result/groupadminlinktarget"/></xsl:attribute>
														<xsl:value-of select="result/CommunityGroupData/GroupAdmin/DisplayName "/>
													</a>
												</xsl:otherwise>
											</xsl:choose>
										</td>
									</tr>
								</tbody>
							</table>
							<xsl:if test="result/CanEdit=1">
								<p class="ektronCommunityGroupEdit">
									<a  title="Edit Group" class="ek_thickbox">
										<xsl:attribute name="href">
											<xsl:text>[[ekapppath]]communitygroupaddedit.aspx?id=</xsl:text>
											<xsl:value-of select="result/CommunityGroupData/GroupId"></xsl:value-of>
											<xsl:text>&amp;thickbox=true&amp;EkTB_iframe=true&amp;height=600&amp;width=600</xsl:text>
										</xsl:attribute>
										<xsl:text>Edit Group</xsl:text>
									</a>
								</p>
							</xsl:if>
						</div>
						<div class="ektronCommunityGroupFooter"></div>
					</div>
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
