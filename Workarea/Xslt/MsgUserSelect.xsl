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
				<table class="CommunitySearch_ResultTable">
					<xsl:attribute name="id">CommunitySearch_ResultsContainer_Table_<xsl:value-of select="$ControlId" /></xsl:attribute>
					<thead class="CommunitySearch_ResultTableHead">
						<tr>
							<th class="CommunitySearch_ResultTableHeadSelect">
								<input type="checkbox" >
									<xsl:attribute name="onclick">CommunityMsgClass.GetObject('<xsl:value-of select="$ControlId" />').UserSelectCtl_CheckAll(this, '<xsl:value-of select="$ControlId" />')</xsl:attribute>
								</input>
							</th>
							<th class="CommunitySearch_ResultTableHeadAvatar">
								<xsl:value-of select="/root/stringAvatar"/>
							</th>
							<th class="CommunitySearch_ResultTableHeadMember">
								<xsl:value-of select="/root/stringUser"/>
							</th>
						</tr>
					</thead>
					<tbody>
						<xsl:apply-templates select="item" />
						<tr>
							<td colspan="3" class="CommunitySearch_footer" >
								<xsl:choose >
									<xsl:when test="$CurrentPage=$PrevPage">
										<span class="CommunitySearch_TargetsPagePreviousBtn">
											<img >
												<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_prev_d.gif</xsl:attribute>
											</img>
										</span>
									</xsl:when>
									<xsl:otherwise>
										<span class="CommunitySearch_TargetsPagePreviousBtn">
											<img >
												<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_prev.gif</xsl:attribute>
												<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').TriggerSearch('','','<xsl:value-of select="$PrevPage"/>')</xsl:attribute>
												<xsl:attribute name="ondblclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').TriggerSearch('','','<xsl:value-of select="$PrevPage"/>')</xsl:attribute>
											</img>
										</span>
									</xsl:otherwise>
								</xsl:choose>
								<xsl:choose >
									<xsl:when test="$CurrentPage=$NextPage">
										<span class="CommunitySearch_TargetsPageNextBtn">
											<img >
												<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_next_d.gif</xsl:attribute>							
											</img>
										</span>
									</xsl:when>
									<xsl:otherwise>
										<span class="CommunitySearch_TargetsPageNextBtn">
											<img >
												<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>but_next.gif</xsl:attribute>
												<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').TriggerSearch('','','<xsl:value-of select="$NextPage"/>')</xsl:attribute>
												<xsl:attribute name="ondblclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').TriggerSearch('','','<xsl:value-of select="$NextPage"/>')</xsl:attribute>
											</img>
										</span>
									</xsl:otherwise>
								</xsl:choose>
							</td>
						</tr>
					</tbody>
				</table>
			</xsl:when>
			<xsl:otherwise>
				<div class="CommunitySearch_ResultNoResults">
					<xsl:value-of select="/root/stringEmptyResult"/>
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="item">
		<tr>
			<xsl:choose>
				<xsl:when test="oddrow='true'" >
					<xsl:attribute name="class">CommunitySearch_ResultTableOddRow</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">CommunitySearch_ResultTableEvenRow</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<td class="CommunitySearch_ResultTableDataSelect">
				<input type="checkbox" >
					<xsl:attribute name="name"><xsl:value-of select="userId"/></xsl:attribute>
					<xsl:attribute name="alt"><xsl:value-of select="displayName"/></xsl:attribute>
					<xsl:attribute name="onclick">CommunityMsgClass.GetObject('<xsl:value-of select="$ControlId" />').UserSelectCtl_CheckboxClicked(this, '<xsl:value-of select="$ControlId" />','<xsl:value-of select="userId"/>',"<xsl:value-of select="displayName"/>")</xsl:attribute>
				</input>
			</td>
			<td class="CommunitySearch_ResultTableDataAvatar">
				<div class="CommunitySearch_ResultTableDataAvatarContainer">
					<xsl:choose >
						<xsl:when test="templateProfile=''">
							<img >
								<xsl:attribute name="src"><xsl:value-of select="avatar"/></xsl:attribute>
								<xsl:attribute name="alt"><xsl:value-of select="displayName"/></xsl:attribute>
							</img>
						</xsl:when>
						<xsl:otherwise>
							<a >
								<xsl:attribute name="href" ><xsl:value-of select="templateProfile"/>?<xsl:value-of select="/root/templateParamName"/>=<xsl:value-of select="userId"/></xsl:attribute>
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
			<td class="CommunitySearch_ResultTableDataMember">
				<div class="CommunitySearch_Result_MemberName">
					<xsl:choose >
						<xsl:when test="fitsResultProfile='true'">
							<xsl:choose >
								<xsl:when test="templateProfile=''">
									<a target="_self" href="#">
										<xsl:if test="/root/mouseOverInfo='true'">
											<xsl:attribute name="onmouseover">CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
											<xsl:attribute name="onmouseout">CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DelayedHideInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
										</xsl:if>
										<xsl:value-of select="displayName"/></a>
								</xsl:when>
								<xsl:otherwise>
									<a ><xsl:attribute name="href" ><xsl:value-of select="templateProfile"/>?<xsl:value-of select="/root/templateParamName"/>=<xsl:value-of select="userId"/></xsl:attribute>
										<xsl:attribute name="target"><xsl:value-of select="/root/templateTarget"/></xsl:attribute>
										<xsl:if test="/root/mouseOverInfo='true'">
											<xsl:attribute name="onmouseover">CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
											<xsl:attribute name="onmouseout">CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DelayedHideInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);</xsl:attribute>
										</xsl:if>
										<xsl:value-of select="displayName"/></a>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="displayName"/>
						</xsl:otherwise>
					</xsl:choose>
					<div class="CommunitySearch_Result_InfoBlockContainer">
						<xsl:call-template name="BuildInfoBlock" />
					</div>
				</div>
				<div class="CommunitySearch_Result_MemberPTags">
					<xsl:apply-templates select="personalTags" />
					<xsl:if test="personalTagsTruncated='true'">
						<span class="CommunitySearch_UserTagLinkContainer_Truncated" >...</span>
					</xsl:if>
				</div>
				<xsl:if test="(enableMapForUser='true') and (fitsResultProfile='true') and (/root/isLoggedIn='true')">
					<div class="CommunitySearch_Result_MemberLocationLink">
						<span class="CommunitySearch_UserLocationSearch">
							<a >
								<xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowLocationForUser(<xsl:value-of select="userJsonData"/>);</xsl:attribute>
								<xsl:choose>
									<xsl:when test="not($LocationImage='')">
										<img class="CommunitySearch_ItemLocationSearchImage" >
											<xsl:attribute name="src" ><xsl:value-of select="$LocationImage" /></xsl:attribute>
										</img>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="/root/stringLocation"/>
									</xsl:otherwise>
								</xsl:choose>
							</a>
						</span>
					</div>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>

	<xsl:template name="MakeUI">
		<div class="CommunitySearchCtl">
			<xsl:attribute name="id" >CommunitySearchCtl_<xsl:value-of select="$ControlId" /></xsl:attribute>
			<div class="CommunitySearch_BasicSearchTab CommunitySearch_TabSelected" >
				<xsl:attribute name="id" >CommunitySearch_BasicSearchTab_<xsl:value-of select="$ControlId" /></xsl:attribute>
				<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SelectTabReset('basic');</xsl:attribute>
				<xsl:value-of select="/root/stringBasicSearch"/>
			</div>
			<div class="CommunitySearch_AdvancedSearchTab">
				<xsl:attribute name="id" >CommunitySearch_AdvancedSearchTab_<xsl:value-of select="$ControlId" /></xsl:attribute>
				<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SelectTabReset('advanced');</xsl:attribute>
				<xsl:value-of select="/root/stringAdvancedSearch"/>
			</div>
			<div class="CommunitySearch_LocationSearchTab">
				<xsl:attribute name="id" >CommunitySearch_LocationSearchTab_<xsl:value-of select="$ControlId" /></xsl:attribute>
				<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SelectTabReset('location');</xsl:attribute>
				<xsl:if test="/root/EnableMap='false'">
					<xsl:attribute name="style">display: none;</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="/root/stringLocationSearch"/>
			</div>
			<div class="CommunitySearch_BasicContainer">
				<xsl:attribute name="id" >CommunitySearch_BasicContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
				<span class="CommunitySearch_BasicTextboxContainer">
					<input type="text" >
						<xsl:attribute name="id" >CommunitySearch_BasicTextbox_<xsl:value-of select="$ControlId" /></xsl:attribute>
						<xsl:attribute name="onkeypress" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').CheckKey(event);</xsl:attribute>
					</input>
				</span>
				<span class="CommunitySearch_BasicSearchButtonContainer">
					<input type="button" >
						<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoBasicSearch();</xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/stringSearch"/></xsl:attribute>
					</input>
				</span>
			</div>
			<div class="CommunitySearch_AdvancedContainer">
				<xsl:attribute name="id" >CommunitySearch_AdvancedContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
				<fieldset class="CommunitySearch_UserFilterFieldset">
					<legend><xsl:value-of select="/root/stringUsers"/></legend>
					<div class="CommunitySearch_UserFilterContainer">
						<xsl:attribute name="id" >userFilterContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
					</div>
					<input type="button" class="CommunitySearch_SearchButtonContainer">
						<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoAdvancedSearch();</xsl:attribute>
						<xsl:attribute name="value"><xsl:value-of select="/root/stringSearch"/></xsl:attribute>
					</input>
					<input type="button" class="CommunitySearch_UserAddFilterButtonContainer">
						<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').AddFilter(false, false);</xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/stringAddFilter"/></xsl:attribute>
					</input>
					<input type="checkbox" value="Friends Only" class="FriendsOnlyButton" >
						<xsl:attribute name="id" >CommunitySearch_Advanced_FriendsOnlyButton_<xsl:value-of select="$ControlId" /></xsl:attribute>
						<xsl:if test="/root/returnFriendsOnly='true'">
							<xsl:attribute name="checked" >checked</xsl:attribute>
						</xsl:if>
					</input>
					<label class="FriendsOnlyButtonLabel">
						<xsl:attribute name="for" >CommunitySearch_Advanced_FriendsOnlyButton_<xsl:value-of select="$ControlId" /></xsl:attribute>
							<xsl:value-of select="/root/stringFriendsOnly"/>
					</label>
				</fieldset>

				<fieldset class="CommunitySearch_GroupFilterFieldset">
					<legend><xsl:value-of select="/root/stringGroups"/></legend>
					<div class="CommunitySearch_GroupFilterContainer">
						<xsl:attribute name="id" >groupFilterContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
					</div>
					<div class="CommunitySearch_GroupAddFilterButtonContainer">
						<input type="button" >
							<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').AddFilter(true, false);</xsl:attribute>
							<xsl:attribute name="value"><xsl:value-of select="/root/stringAddFilter"/></xsl:attribute>
						</input>
					</div>
				</fieldset>

				<div class="CommunitySearch_CategoryContainer" >
					<div class="CommunitySearch_InnerCategoryContainer" style="display: none;">
						<xsl:attribute name="id" >CommunitySearch_UserCategoryContainer<xsl:value-of select="$ControlId" /></xsl:attribute>
						<div class="CommunitySearch_CategoryContainerHeader" >
							<xsl:value-of select="/root/stringUserCategories"/>
							<div class="CommunitySearch_CategoryContainerCloser" >
								<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').HideCategories(false); return (false);</xsl:attribute>
								<img>
									<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute>
									<xsl:attribute name ="alt"></xsl:attribute>
								</img>
							</div>
						</div>
						<table width="100%" cellspacing="0" cellpadding="4" class="b3">
							<tr>
								<td>
									<div id="CommunitySearch_UserCategoryTreePane" style="display: block;">
										&#160;
									</div>
									<br />
								</td>
							</tr>
						</table>
					</div>
				</div>

				<div class="CommunitySearch_CategoryContainer" >
					<div class="CommunitySearch_InnerCategoryContainer" style="display: none;">
						<xsl:attribute name="id" >CommunitySearch_GroupCategoryContainer<xsl:value-of select="$ControlId" /></xsl:attribute>
						<div class="CommunitySearch_CategoryContainerHeader" >
							<xsl:value-of select="/root/stringGroupCategories"/>
							<div class="CommunitySearch_CategoryContainerCloser">
								<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').HideCategories(true); return (false);</xsl:attribute>
								<img>
									<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute>
									<xsl:attribute name ="alt"></xsl:attribute>
								</img>
							</div>
						</div>
						<table width="100%" cellspacing="0" cellpadding="4" class="b3">
							<tr>
								<td>
									<div id="CommunitySearch_GroupCategoryTreePane" style="display: block;">
										&#160;
									</div>
									<br />
								</td>
							</tr>
						</table>
					</div>
				</div>

			</div>
			<div class="CommunitySearch_LocationContainer">
				<xsl:attribute name="id" >CommunitySearch_LocationContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
			</div>
			<div class="CommunitySearch_ResultsContainer">
				<xsl:attribute name="id" >CommunitySearch_ResultsContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
			</div>
		</div>
	</xsl:template>

	<xsl:template name="BuildInfoBlock">
		<div class="CommunitySearch_InfoBlock" style="display: none;" >
			<xsl:attribute name="id" >EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/></xsl:attribute>
			<xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').HideInfo(this);</xsl:attribute>
			<div class="CommunitySearch_InfoBlockHeader">
				<div class="CommunitySearch_InfoBlockCloser">
					<img >
						<xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute>
						<xsl:attribute name ="alt"></xsl:attribute>
					</img>
				</div>
			</div>
			<xsl:choose>
				<xsl:when test="isUser='true'">
					<div class="CommunitySearch_InfoBlockBody">
						<div class="CommunitySearch_InfoBlockItem">
									<span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringDisplayName"/></span>
							<span class="CommunitySearch_InfoBlockData"><xsl:value-of select="displayName"/></span>
						</div>
						<div class="CommunitySearch_InfoBlockItem">
									<span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringFirstName"/></span>
							<span class="CommunitySearch_InfoBlockData"><xsl:value-of select="firstName"/></span>
						</div>
						<div class="CommunitySearch_InfoBlockItem">
									<span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringLastName"/></span>
							<span class="CommunitySearch_InfoBlockData"><xsl:value-of select="lastName"/></span>
						</div>
					</div>
				</xsl:when>
				<xsl:otherwise>
					<div class="CommunitySearch_GroupInfoBlockBody">
						<div class="CommunitySearch_GroupInfoBlockItem">
							<span class="CommunitySearch_GroupInfoBlockLabel">Group Name</span>
							<span class="CommunitySearch_GroupInfoBlockData"><xsl:value-of select="displayName"/></span>
						</div>
						<div class="CommunitySearch_GroupInfoBlockItem">
							<span class="CommunitySearch_GroupInfoBlockLabel">Location</span>
							<span class="CommunitySearch_GroupInfoBlockData"><xsl:value-of select="firstName"/></span>
						</div>
						<div class="CommunitySearch_GroupDescriptionInfoBlockItem">
							<span class="CommunitySearch_GroupInfoBlockLabel">Description</span>
							<span class="CommunitySearch_GroupInfoBlockData"><xsl:value-of select="lastName"/></span>
						</div>
					</div>
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>

	<xsl:template match="personalTags">
		<xsl:apply-templates select="personalTag" />
	</xsl:template>

	<xsl:template match="personalTag">
			<span class="CommunitySearch_UserTagLinkContainer" >
				<a href="#" target="_self" class="CommunitySearch_UserTagLink" >
					<xsl:attribute name="onclick">return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SearchTag('<xsl:value-of select="text()"/>');</xsl:attribute>
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
						<xsl:value-of select="/root/stringIsAFriend"/>
					</xsl:when>
					<xsl:when test="isAPendingFriend='true'">
						<xsl:value-of select="/root/stringPendingFriend"/>
					</xsl:when>
					<xsl:when test="isARequestedFriend='true'">
						<xsl:value-of select="/root/stringSentFriendRequest"/>
					</xsl:when>
					<xsl:otherwise>
						<a target="_self" href="#">
							<xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoAjaxSearch('__addfid=<xsl:value-of select="userId" />', '<xsl:value-of select="userId" />', '<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
							<xsl:value-of select="/root/stringAddFriend"/>
						</a>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>