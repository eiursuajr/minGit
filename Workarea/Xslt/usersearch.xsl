<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:variable name ="ControlId" select="/root/controlId" />
	<xsl:variable name ="TemplateLocation" select="/root/templateLocation" />
	<xsl:variable name ="TemplateProfile" select="/root/templateProfile" />
	<xsl:variable name ="PageSize" select="/root/pageSize" />
	<xsl:variable name ="CurrentPage" select="/root/currentPage" />
	<xsl:variable name ="PrevPage" select="/root/prevPage" />
	<xsl:variable name ="NextPage" select="/root/nextPage" />
	<xsl:variable name ="LocationImage" select="/root/locationImage" />

	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="/root/dataMode='SearchResults'">
				<xsl:apply-templates select="items" />
			</xsl:when>
			<xsl:when test="/root/dataMode='Status'">
				<xsl:call-template name="Status" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="MakeUI" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="items">
		<xsl:choose>
			<xsl:when test="item" >
				<table class="EktUserSearch_ResultTable">
					<thead class="EktUserSearch_ResultTableHead">
						<tr>
							<th class="EktUserSearch_ResultTableHeadAvatar">
								Avatar
							</th>
							<th class="EktUserSearch_ResultTableHeadMember">
								User
							</th>
							<th class="EktUserSearch_ResultTableHeadStatus">
								Status
							</th>
						</tr>
					</thead>
					<tbody>
						<xsl:apply-templates select="item" />
					</tbody>
				</table>
				<xsl:choose >
					<xsl:when test="$CurrentPage=$PrevPage">
						<span class="EktUserSearch_TargetsPagePreviousBtn">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_prev_d.gif</xsl:attribute>
							</img>
						</span>
					</xsl:when>
					<xsl:otherwise>
						<span class="EktUserSearch_TargetsPagePreviousBtn">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_prev.gif</xsl:attribute>
								<xsl:attribute name="onclick" >__LoadUserSearch<xsl:value-of select="$ControlId" />('','','<xsl:value-of select="$PrevPage"/>')</xsl:attribute>
								<xsl:attribute name="ondblclick" >__LoadUserSearch<xsl:value-of select="$ControlId" />('','','<xsl:value-of select="$PrevPage"/>')</xsl:attribute>
							</img>
						</span>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:choose >
					<xsl:when test="$CurrentPage=$NextPage">
						<span class="EktUserSearch_TargetsPageNextBtn">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_next_d.gif</xsl:attribute>							
							</img>
						</span>
					</xsl:when>
					<xsl:otherwise>
						<span class="EktUserSearch_TargetsPageNextBtn">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_next.gif</xsl:attribute>
								<xsl:attribute name="onclick" >__LoadUserSearch<xsl:value-of select="$ControlId" />('','','<xsl:value-of select="$NextPage"/>')</xsl:attribute>
								<xsl:attribute name="ondblclick" >__LoadUserSearch<xsl:value-of select="$ControlId" />('','','<xsl:value-of select="$NextPage"/>')</xsl:attribute>
							</img>
						</span>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<div class="EktUserSearch_ResultNoResults">
					No Users Found.
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="item">
		<tr>
			<xsl:choose>
				<xsl:when test="oddrow='true'" >
					<xsl:attribute name="class">EktUserSearch_ResultTableOddRow</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">EktUserSearch_ResultTableEvenRow</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<td class="EktUserSearch_ResultTableDataAvatar">
				<div class="EktUserSearch_ResultTableDataAvatarContainer">
					<xsl:choose >
						<xsl:when test="$TemplateProfile=''">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="avatar"/></xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="displayName"/></xsl:attribute>
							</img>
						</xsl:when>
						<xsl:otherwise>
							<a >
								<xsl:attribute name="href" ><xsl:value-of select="$TemplateProfile"/><xsl:value-of select="/root/templateParamName"/>=<xsl:value-of select="userId"/></xsl:attribute>
								<xsl:attribute name="target"><xsl:value-of select="/root/templateTarget"/></xsl:attribute>
								<img >
									<xsl:attribute name="src"><xsl:value-of select="avatar"/></xsl:attribute>
									<xsl:attribute name="alt"><xsl:value-of select="displayName"/></xsl:attribute>
								</img>
							</a>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</td>
			<td class="EktUserSearch_ResultTableDataMember">
				<div class="EktUserSearch_Result_MemberName">
					<xsl:choose >
						<xsl:when test="fitsResultProfile='true'">
							<xsl:choose >
								<xsl:when test="$TemplateProfile=''">
									<a target="_self" href="#">
										<xsl:if test="/root/mouseOverInfo='true'">
											<xsl:attribute name="onmouseover">EkUserSearch_ShowInfo('EkUserSearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
											<xsl:attribute name="onmouseout">EkUserSearch_DelayedHideInfo('EkUserSearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
										</xsl:if>
										<xsl:value-of select="displayName"/></a>
								</xsl:when>
								<xsl:otherwise>
									<a ><xsl:attribute name="href" ><xsl:value-of select="$TemplateProfile"/><xsl:value-of select="/root/templateParamName"/>=<xsl:value-of select="userId"/></xsl:attribute>
										<xsl:attribute name="target"><xsl:value-of select="/root/templateTarget"/></xsl:attribute>
										<xsl:if test="/root/mouseOverInfo='true'">
											<xsl:attribute name="onmouseover">EkUserSearch_ShowInfo('EkUserSearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
											<xsl:attribute name="onmouseout">EkUserSearch_DelayedHideInfo('EkUserSearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
										</xsl:if>
										<xsl:value-of select="displayName"/></a>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="displayName"/>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:call-template name="BuildInfoBlock" />
				</div>
				<div class="EktUserSearch_Result_MemberPTags">
					<xsl:apply-templates select="personalTags" />
					<xsl:if test="personalTagsTruncated='true'">
						<span class="EktUserSearch_UserTagLinkContainer_Truncated" >...</span>
					</xsl:if>
				</div>
				<xsl:if test="not($TemplateLocation='') and (fitsResultProfile='true') and (/root/isLoggedIn='true')">
					<div class="EktUserSearch_Result_MemberLocationLink">
						<span class="EktUserSearch_UserLocationSearch">
							<a >
								<xsl:attribute name="href"><xsl:value-of select="$TemplateLocation"/><xsl:value-of select="/root/templateParamName"/>=<xsl:value-of select="userId"/></xsl:attribute>
								<xsl:attribute name="target"><xsl:value-of select="/root/templateTarget"/></xsl:attribute>
								<xsl:choose>
									<xsl:when test="not($LocationImage='')">
										<img class="EktUserSearch_ItemLocationSearchImage" >
											<xsl:attribute name="src" ><xsl:value-of select="$LocationImage" /></xsl:attribute>
										</img>
									</xsl:when>
									<xsl:otherwise>
										Location
									</xsl:otherwise>
								</xsl:choose>
							</a>
						</span>
					</div>
				</xsl:if>
			</td>
			<td class="EktUserSearch_ResultTableDataStatus">
				<xsl:attribute name="id" >EkUSR_Status_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId" /></xsl:attribute>
				<xsl:if test="not(isSelf='true')">
					<xsl:if test="/root/isLoggedIn='true'">
						<xsl:choose >
							<xsl:when test="isAFriend='true'">
								Is a friend
							</xsl:when>
							<xsl:when test="isAPendingFriend='true'">
								Is a pending friend
							</xsl:when>
							<xsl:when test="isARequestedFriend='true'">
								Sent a friend request
							</xsl:when>
							<xsl:otherwise>
								<a target="_self" href="#">
									<xsl:attribute name="onclick" >return __LoadUserSearch<xsl:value-of select="$ControlId" />('__addfid=<xsl:value-of select="userId" />', '<xsl:value-of select="userId" />', '<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
									Add as Friend
								</a>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:if>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>

	<xsl:template name="MakeUI">
		<div class="EktUserSearchCtl">
			<div class="EktUserSearch_HeaderBar">
				User Search
			</div>
			<div class="EktUserSearch_Body">
				<xsl:choose>
					<xsl:when test="/root/accessDenied='true'">
						<div class="EktUserSearch_accessDenied" >
							You must be logged in to access this!
						</div>
					</xsl:when>
					<xsl:otherwise>
						<div class="EktUserSearch_Options" >
							<select >
								<xsl:attribute name="id" >UserSearch_ModeSel<xsl:value-of select="$ControlId" /></xsl:attribute>
								<xsl:attribute name="name" >UserSearch_ModeSel<xsl:value-of select="$ControlId" /></xsl:attribute>
								<option value="personal_tags" selected="selected" >Personal Tags</option>
								<option value="display_name" >Display Name</option>
								<xsl:if test="showNameEmail='true'">
									<option value="first_name" >First Name</option>
									<option value="last_name" >Last Name</option>
									<option value="email" >Email</option>
								</xsl:if>
							</select>
							<xsl:if test="not($TemplateLocation='') and (/root/isLoggedIn='true')">
								<a ><xsl:attribute name="href"><xsl:value-of select="$TemplateLocation"/></xsl:attribute>
									<xsl:attribute name="target"><xsl:value-of select="/root/templateTarget"/></xsl:attribute>
									<xsl:choose>
										<xsl:when test="not($LocationImage='')">
											<img class="EktUserSearch_MainLocationSearchImage" >
												<xsl:attribute name="src" ><xsl:value-of select="$LocationImage" /></xsl:attribute>
											</img>
											<span class="EktUserSearch_TopLocationSearchText">Location Search</span>
										</xsl:when>
										<xsl:otherwise>
											<span class="EktUserSearch_TopLocationSearchText">Location Search</span>
										</xsl:otherwise>
									</xsl:choose>
								</a>
							</xsl:if>
							<span class="EktUserSearch_Categories" >
								<xsl:attribute name="id">"EktUserSearch_Categories<xsl:value-of select="$ControlId" /></xsl:attribute>
								<a href='#' ><xsl:attribute name="onclick">UserSearch_ShowCategories<xsl:value-of select="$ControlId" />(); return (false);</xsl:attribute>Categories</a>
							</span>
							<div class="EktUserSearch_CategoryContainer" >
								<div class="EktUserSearch_InnerCategoryContainer" style="display: none;">
									<xsl:attribute name="id">__catdv<xsl:value-of select="$ControlId" /></xsl:attribute>
									<div class="EktUserSearch_CategoryContainerHeader" >Categories
										<div class="EktUserSearch_CategoryContainerCloser" >
											<xsl:attribute name="onclick">UserSearch_HideCategories<xsl:value-of select="$ControlId" />(); return (false);</xsl:attribute>
											<img >
												<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute>
											</img>
										</div>
									</div>
									<div class="EktUserSearch_CategoryFriendsContainer" >
										<xsl:attribute name="onclick">UserSearch_ToggleFriends<xsl:value-of select="$ControlId" />(this);</xsl:attribute>
										<xsl:choose>
											<xsl:when test="returnFriendsOnly='true'">
												<img align="absmiddle" style="width: 16px; height: 16px;" >
													<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>maps/tree/iconCheckAll.gif</xsl:attribute>
												</img>&#160;Friends Only
											</xsl:when>
											<xsl:otherwise>
												<img align="absmiddle" style="width: 16px; height: 16px;" >
													<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>maps/tree/iconUnCheckAll.gif</xsl:attribute>
												</img>&#160;Friends Only
											</xsl:otherwise>
										</xsl:choose>
									</div>
									<table width="100%" cellspacing="0" cellpadding="4" class="b3">
										<tr>
											<td>
												<div id="__MapCategoryTreePane" style="display: block;">
													&#160;
												</div>
												<br />
											</td>
										</tr>
									</table>
								</div>
							</div>
						</div>
						<div class="EktUserSearch_Inputs" >
							<xsl:attribute name="id" >UserSearchDivInputs<xsl:value-of select="$ControlId" /></xsl:attribute>
							<input class="EktUserSearch_TextInput" type="text" value="">
								<xsl:attribute name="id" >User_Search_txt<xsl:value-of select="$ControlId" /></xsl:attribute>
								<xsl:attribute name="name" >User_Search_txt<xsl:value-of select="$ControlId" /></xsl:attribute>
								<xsl:attribute name="onkeypress" >return EkUserSearch_CheckKey<xsl:value-of select="$ControlId" />(event);</xsl:attribute>
							</input>&#160;<input class="EktUserSearch_SearchButton" type="button" value="Search" >
								<xsl:attribute name="id" >User_Search_btn<xsl:value-of select="$ControlId" /></xsl:attribute>
								<xsl:attribute name="name" >User_Search_btn<xsl:value-of select="$ControlId" /></xsl:attribute>
								<xsl:attribute name="onclick" >__LoadUserSearch<xsl:value-of select="$ControlId" />('', '')</xsl:attribute>
							</input>
						</div>
						<div class="EktUserSearch_Result" >
							<xsl:attribute name="id" >UserSearchDivResults<xsl:value-of select="$ControlId" /></xsl:attribute>
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</div>
		</div>
	</xsl:template>

	<xsl:template name="BuildInfoBlock">
		<div class="EktUserSearch_InfoBlock" style="display: none;" onclick="return EkUserSearch_HideInfo(this);">
			<xsl:attribute name="id" >EkUserSearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/></xsl:attribute>
			<div class="EktUserSearch_InfoBlockHeader">
				<div class="EktUserSearch_InfoBlockCloser">
					<img >
						<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute>
					</img>
				</div>
			</div>
			<div class="EktUserSearch_InfoBlockBody">
				<div class="EktUserSearch_InfoBlockItem">
					<span class="EktUserSearch_InfoBlockLabel">Display Name</span>
					<span class="EktUserSearch_InfoBlockData"><xsl:value-of select="displayName"/></span>
				</div>
				<div class="EktUserSearch_InfoBlockItem">
					<span class="EktUserSearch_InfoBlockLabel">First Name</span>
					<span class="EktUserSearch_InfoBlockData"><xsl:value-of select="firstName"/></span>
				</div>
				<div class="EktUserSearch_InfoBlockItem">
					<span class="EktUserSearch_InfoBlockLabel">Last Name</span>
					<span class="EktUserSearch_InfoBlockData"><xsl:value-of select="lastName"/></span>
				</div>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="personalTags">
		<xsl:apply-templates select="personalTag" />
	</xsl:template>

	<xsl:template match="personalTag">
			<span class="EktUserSearch_UserTagLinkContainer" >
				<a href="#" target="_self" class="EktUserSearch_UserTagLink" >
					<xsl:attribute name="onclick">return EkUserSearch_SearchUserTag<xsl:value-of select="$ControlId" />('<xsl:value-of select="text()"/>');</xsl:attribute>
					<xsl:value-of select="text()"/>
				</a>
			</span>
	</xsl:template>
	
	<xsl:template name="Status">
		<xsl:if test="not(isSelf='true')">
			<xsl:if test="/root/isLoggedIn='true'">
				<xsl:if test="/root/addTargInfo='true'">
					[targ_info_begin]EkUSR_Status_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId" />[targ_info_end]
				</xsl:if>
				<xsl:choose >
					<xsl:when test="isAFriend='true'">
						Is a friend
					</xsl:when>
					<xsl:when test="isAPendingFriend='true'">
						Is a pending friend
					</xsl:when>
					<xsl:when test="isARequestedFriend='true'">
						Sent a friend request
					</xsl:when>
					<xsl:otherwise>
						<a target="_self" href="#">
							<xsl:attribute name="onclick" >return __LoadUserSearch<xsl:value-of select="$ControlId" />('__addfid=<xsl:value-of select="userId" />', '<xsl:value-of select="userId" />', '<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
							Add as Friend
						</a>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>