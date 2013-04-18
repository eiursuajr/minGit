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
  <!--<xsl:variable name ="userPropertyName" select="/root/items/item/customUserProperties/customUserProperty/userPropertyName" />-->

	<xsl:template match="/root">
		<xsl:choose>
			<xsl:when test="/root/dataMode='SearchResults'">
				<xsl:apply-templates select="items" />
        <xsl:call-template name="EmitResultFilteringXml" />
			</xsl:when>
			<xsl:when test="/root/dataMode='Status'">
				<xsl:call-template name="Status" />
			</xsl:when>
      <xsl:when test="/root/accessDenied='true'">
        <xsl:call-template name="NotAuthorizedMessage" />
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
							<th class="CommunitySearch_ResultTableHeadAvatar">
								<xsl:value-of select="/root/stringAvatar"/>
							</th>
							<th class="CommunitySearch_ResultTableHeadFirstName">
								<xsl:value-of select="/root/stringName"/>
							</th>
							<th class="CommunitySearch_ResultTableHeadLastName">
								<xsl:value-of select="/root/stringLastName"/>
							</th>
							<th class="CommunitySearch_ResultTableHeadInfo">
                <xsl:if test="/root/isLoggedIn='false'">
                  <xsl:attribute name="colspan">2</xsl:attribute>
                </xsl:if>
								<xsl:value-of select="/root/stringInformation"/>
							</th>
              <xsl:if test="/root/isLoggedIn='true'">
							<th class="CommunitySearch_ResultTableHeadStatus">
								<xsl:value-of select="/root/stringStatus"/>
							</th>
              </xsl:if>  
						</tr>
					</thead>
					<tbody>
						<xsl:apply-templates select="item" />
						<tr>
							<td colspan="5" class="CommunitySearch_footer" > 
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
      <td class="CommunitySearch_ResultTableDataAvatar">
        <div class="CommunitySearch_ResultTableDataAvatarContainer">
          <xsl:choose >
            <xsl:when test="templateProfile!=''">
              <a >
					<xsl:attribute name="href" >
						<xsl:value-of select ="/root/sitepath"/><xsl:value-of select="templateProfile"/>?<xsl:value-of select="/root/templateParamName"/>=<xsl:value-of select="userId"/>
					</xsl:attribute>
					<xsl:attribute name="target">
						<xsl:value-of select="/root/templateTarget"/>
					</xsl:attribute>
					<img >
						<xsl:attribute name="src">
							<xsl:value-of select="avatar"/>
						</xsl:attribute>
						<xsl:attribute name="alt">
							<xsl:value-of select="firstName"/>
						</xsl:attribute>
					</img>
				</a>
			</xsl:when>
			<xsl:when test="aliasProfile!=''">
				<a >
					<xsl:attribute name="href" >
						<xsl:value-of select ="/root/sitepath"/>
						<xsl:value-of select="aliasProfile"/>
					</xsl:attribute>
					<xsl:attribute name="target">
						<xsl:value-of select="/root/templateTarget"/>
					</xsl:attribute>
					<img >
						<xsl:attribute name="src">
							<xsl:value-of select="avatar"/>
						</xsl:attribute>
						<xsl:attribute name="alt">
							<xsl:value-of select="firstName"/>
						</xsl:attribute>
					</img>
				</a>
			</xsl:when >
			<xsl:otherwise>
				<img >
					<xsl:attribute name="src">
						<xsl:value-of select="avatar"/>
					</xsl:attribute>
					<xsl:attribute name="alt">
						<xsl:value-of select="firstName"/>
					</xsl:attribute>
				</img>
			</xsl:otherwise>
		</xsl:choose>
	</div>
</td>
<td class="CommunitySearch_ResultTableDataFirstName">
	<div class="CommunitySearch_Result_FirstName">
		<xsl:choose >
			<xsl:when test="templateProfile!=''">
				<a >
					<xsl:attribute name="href" >
						<xsl:value-of select ="/root/sitepath"/><xsl:value-of select="templateProfile"/>?<xsl:value-of select="templateParameter"/>=<xsl:value-of select="userId"/>
					</xsl:attribute>
					<xsl:attribute name="target">
						<xsl:value-of select="/root/templateTarget"/>
					</xsl:attribute>
					<xsl:if test="(/root/mouseOverInfo='true') and (fitsResultProfile='true')">
						<xsl:attribute name="onmouseover">
							CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
						</xsl:attribute>
						<xsl:if test="/root/mouseOutHidesInfo='true'">
							<xsl:attribute name="onmouseout">
								CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DelayedHideInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
							</xsl:attribute>
						</xsl:if>
					</xsl:if>
					<xsl:choose >
						<xsl:when test="isUser='true'">
							<xsl:value-of select="firstName"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="displayName"/>
						</xsl:otherwise>
					</xsl:choose>
				</a>
			</xsl:when>
			<xsl:when test="aliasProfile!=''">
				<a >
					<xsl:attribute name="href" >
						<xsl:value-of select ="/root/sitepath"/>
						<xsl:value-of select="aliasProfile"/>
					</xsl:attribute>
					<xsl:attribute name="target">
						<xsl:value-of select="/root/templateTarget"/>
					</xsl:attribute>
					<xsl:if test="(/root/mouseOverInfo='true') and (fitsResultProfile='true')">
						<xsl:attribute name="onmouseover">
							CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
						</xsl:attribute>
						<xsl:if test="/root/mouseOutHidesInfo='true'">
							<xsl:attribute name="onmouseout">
								CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DelayedHideInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
							</xsl:attribute>
						</xsl:if>
					</xsl:if>
					<xsl:choose >
						<xsl:when test="isUser='true'">
							<xsl:value-of select="firstName"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="displayName"/>
						</xsl:otherwise>
					</xsl:choose>
				</a>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose >
					<xsl:when test="isUser='true'">
						<xsl:value-of select="firstName"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="displayName"/>
					</xsl:otherwise>
				</xsl:choose>
            </xsl:otherwise>
          </xsl:choose>
        </div>
        <div class="CommunitySearch_Result_InfoBlockContainer">
          <xsl:call-template name="BuildInfoBlock" />
        </div>
      </td>
      <td class="CommunitySearch_ResultTableDataLastName">
        <div class="CommunitySearch_Result_LastName">
			<xsl:if test="isUser='true'">
			  <xsl:choose >
				<xsl:when test="templateProfile!=''">
					<a >
						<xsl:attribute name="href" >
							<xsl:value-of select ="/root/sitepath"/><xsl:value-of select="templateProfile"/>?<xsl:value-of select="templateParameter"/>=<xsl:value-of select="userId"/>
						</xsl:attribute>
						<xsl:attribute name="target">
							<xsl:value-of select="/root/templateTarget"/>
						</xsl:attribute>
						<xsl:if test="(/root/mouseOverInfo='true') and (fitsResultProfile='true')">
							<xsl:attribute name="onmouseover">
								CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
							</xsl:attribute>
							<xsl:if test="/root/mouseOutHidesInfo='true'">
								<xsl:attribute name="onmouseout">
									CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DelayedHideInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
								</xsl:attribute>
							</xsl:if>
						</xsl:if>
						<xsl:value-of select="lastName"/>
					</a>
				</xsl:when>
				  <xsl:when test="aliasProfile!=''">
					  <a >
						  <xsl:attribute name="href" >
							  <xsl:value-of select ="/root/sitepath"/><xsl:value-of select="aliasProfile"/>
						  </xsl:attribute>
						  <xsl:attribute name="target">
							  <xsl:value-of select="/root/templateTarget"/>
						  </xsl:attribute>
						  <xsl:if test="(/root/mouseOverInfo='true') and (fitsResultProfile='true')">
							  <xsl:attribute name="onmouseover">
								  CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
							  </xsl:attribute>
							  <xsl:if test="/root/mouseOutHidesInfo='true'">
								  <xsl:attribute name="onmouseout">
									  CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DelayedHideInfo('EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/>', this);
								  </xsl:attribute>
							  </xsl:if>
						  </xsl:if>
						  <xsl:value-of select="lastName"/>
					  </a>
				  </xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="lastName"/>
				</xsl:otherwise>
			  </xsl:choose>
			</xsl:if>
        </div>
      </td>
      <td class="CommunitySearch_ResultTableDataPhone">
        <xsl:if test="/root/isLoggedIn='false'">
          <xsl:attribute name="colspan">2</xsl:attribute>
        </xsl:if>
        <div class="CommunitySearch_Result_Phone">
					<xsl:choose >
						<xsl:when test="fitsResultProfile='true'">
              <xsl:choose >
                <xsl:when test="templateProfile=''">
                  <xsl:for-each select="customUserProperties/customUserProperty">
                    <div class="CustomUserPropertyDisplay">
                      <span class="CustomUserPropertyName"><strong><xsl:value-of select="userPropertyName"/>:&#160;</strong></span>
                      <span class="CustomUserPropertyValue"><xsl:value-of select="userPropertyValue"/></span>
                    </div>
                  </xsl:for-each>
                  <span><strong>Email:&#160;</strong></span>
                  <a >
                    <xsl:attribute name="href" >mailto:<xsl:value-of select="email"/></xsl:attribute>
                    <span><xsl:value-of select="email"/></span>
                  </a>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:for-each select="customUserProperties/customUserProperty">
                    <div>
                      <span><strong><xsl:value-of select="userPropertyName"/>:&#160;</strong></span>
                      <span><xsl:value-of select="userPropertyValue"/></span>
                    </div>
                  </xsl:for-each>
                  <span><strong>Email:&#160;</strong></span>
                  <a >
                    <xsl:attribute name="href" >mailto:<xsl:value-of select="email"/></xsl:attribute>
                    <span><xsl:value-of select="email"/></span>
                  </a>
                  <div class="CommunitySearch_Result_MemberPTags">
                    <span class="CommunitySearch_UserTagLinkContainer"><strong>Tags: </strong></span>
                    <xsl:if test="personalTags=''">
                      <span class="CommunitySearch_UserTagLinkContainer"><xsl:value-of select="'No user tags to display'" /></span>
                    </xsl:if>
                    <xsl:apply-templates select="personalTags" />
                  </div>
                  <xsl:if test="(enableMapForUser='true') and (fitsResultProfile='true') and (/root/isLoggedIn='true')">
                    <div class="CommunitySearch_Result_MemberLocationLink"><strong>User Location: </strong>
                      <span class="CommunitySearch_UserLocationSearch">
                        <a >
                          <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ShowLocationForUser(<xsl:value-of select="userJsonData"/>);</xsl:attribute>
                          <xsl:attribute name="title"><xsl:value-of select="/root/stringClickForLocationSearch"/></xsl:attribute>
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
                </xsl:otherwise>
              </xsl:choose>
            </xsl:when>
						<xsl:otherwise>
              <div class="CommunitySearch_Result_MemberPTags">
                <span class="CommunitySearch_UserTagLinkContainer"><strong>Tags: </strong></span>
                <xsl:if test="personalTags=''">
                  <span class="CommunitySearch_UserTagLinkContainer"><xsl:value-of select="'No user tags to display'" /></span>
                </xsl:if>
                <xsl:apply-templates select="personalTags" />
              </div>
            </xsl:otherwise>
          </xsl:choose>
        </div>
      </td>
      <xsl:if test="/root/isLoggedIn='true'">
      <td class="CommunitySearch_ResultTableDataStatus">
        <xsl:attribute name="id" >EkUSR_Status_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId" /></xsl:attribute>
        <xsl:if test="not(isSelf='true')">
          <xsl:if test="/root/isLoggedIn='true'">
				<xsl:choose>
					<xsl:when test="isUser='true'">
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
					</xsl:when>
					<xsl:otherwise>
						<xsl:choose >
							<xsl:when test="groupStatus='request'">
								<xsl:value-of select="/root/stringGroupJoinRequested"/>
							</xsl:when>
							<xsl:when test="groupStatus='member'">
								<xsl:value-of select="/root/stringGroupMember"/>
							</xsl:when>
							<xsl:when test="groupStatus='leader'">
								<xsl:value-of select="/root/stringGroupLeader"/>
							</xsl:when>
							<xsl:otherwise>
								<a target="_self" href="#">
									<xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoAjaxSearch('__joingid=<xsl:value-of select="userId" />', '<xsl:value-of select="userId" />', '<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
									<xsl:value-of select="/root/stringGroupJoin"/>
								</a>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:otherwise>
				</xsl:choose>
          </xsl:if>
        </xsl:if>
      </td>
      </xsl:if>
    </tr>
  </xsl:template>
  
  <xsl:template name="MakeUI">
    <div class="CommunitySearchCtl">
      <xsl:attribute name="id" >CommunitySearchCtl_<xsl:value-of select="$ControlId" /></xsl:attribute>
      <div class="CommunitySearch_DirectorySearchTab CommunitySearch_TabSelected" >
        <xsl:attribute name="id" >CommunitySearch_DirectorySearchTab_<xsl:value-of select="$ControlId" /></xsl:attribute>
        <xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SelectTabReset('directory');CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('@id > 0','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
		  <xsl:value-of select="/root/stringDirectorySearch"/>
      </div>

      <div class="CommunitySearch_BasicSearchTab">
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
      
      <div class="CommunitySearch_DirectoryContainer">
        <xsl:attribute name="id" >CommunitySearch_DirectoryContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
        <div class="CommunitySearch_DirectoryDescription">
          <xsl:value-of select="/root/stringDirectoryDesc"/>
        </div>
		<div class="CommunitySearch_labelTabContainer">
          <div class="CommunitySearch_labelTabs" >
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('@id > 0','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              ALL
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('A*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              A
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#" >
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('B*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              B
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#" >
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('C*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              C
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('D*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              D
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('E*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              E
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('F*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              F
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('G*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              G
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('H*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              H
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('I*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              I
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('J*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              J
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('K*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              K
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('L*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              L
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('M*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              M
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('N*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              N
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('O*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              O
          </a>
        </div>
        <div class="CommunitySearch_labelTabs">
          <a href="#">
            <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('P*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
            P
          </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('Q*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              Q
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('R*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              R
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('S*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              S
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('T*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              T
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('U*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              U
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('V*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              V
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('W*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              W
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('X*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              X
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('Y*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              Y
            </a>
          </div>
          <div class="CommunitySearch_labelTabs">
            <a href="#">
              <xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoDirectoryFilterName('Z*','','<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
              Z
            </a>
          </div>
          <div class="CommunitySearch_sortByContainer">
            <span class="communitySearch_DirectorySortByContainer">
              <label><strong>&#160;<xsl:value-of select="/root/stringSearchBy"/>&#160;</strong></label>
            </span>
            <select class="SortByDirectoryDropdownList" name="SortByDirectoryDropdownList">
              <xsl:attribute name="id">CommunitySearch_SortByDirectoryDropdownList_<xsl:value-of select="$ControlId"/></xsl:attribute>
              <option id="first_name" value="first_name" selected="selected" ><xsl:value-of select="/root/stringFirstName"/></option>
              <option id="last_name" value="last_name" ><xsl:value-of select="/root/stringLastName"/></option>
              <option id="display_name" value="display_name" ><xsl:value-of select="/root/stringDisplayName"/></option>
			</select>
          </div>
        </div>
      </div>
      
      <div class="CommunitySearch_BasicContainer" >
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
        <xsl:if test="(/root/EnableUserResults='true')">
          <fieldset class="CommunitySearch_UserFilterFieldset">
            <legend><xsl:value-of select="/root/stringUsers"/></legend>
            <div class="CommunitySearch_UserFilterContainer">
              <xsl:attribute name="id" >userFilterContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
            </div>
            <div class="CommunitySearch_UserAddFilterButtonContainer">
              <input type="button" class="UserAddFilterButton">
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
            </div>
          </fieldset>
        </xsl:if>
        <xsl:if test="(/root/EnableGroupResults='true')">
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
        </xsl:if>
        <div class="CommunitySearch_SearchButtonContainer">
          <input type="button" >
            <xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoAdvancedSearch();</xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="/root/stringSearch"/></xsl:attribute>
          </input>
        </div>
        
        <div class="CommunitySearch_CategoryContainer" >
          <div class="CommunitySearch_InnerCategoryContainer" style="display: none;">
            <xsl:attribute name="id" >CommunitySearch_UserCategoryContainer<xsl:value-of select="$ControlId" /></xsl:attribute>
            <div class="CommunitySearch_CategoryContainerHeader" >
              <xsl:value-of select="/root/stringUserCategories"/>
              <div class="CommunitySearch_CategoryContainerCloser" >
                <xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').HideCategories(false); return (false);</xsl:attribute>
                <img><xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute></img>
              </div>
            </div>
            <table width="100%" cellspacing="0" cellpadding="4" class="b3">
              <tr>
                <td>
                  <div id="CommunitySearch_UserCategoryTreePane" class="CommunitySearch_UserCategoryTreePane" style="display: block;">&#160;
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
                </img>
              </div>
            </div>
            <table width="100%" cellspacing="0" cellpadding="4" class="b3">
              <tr>
                <td>
                  <div id="CommunitySearch_GroupCategoryTreePane" class="CommunitySearch_GroupCategoryTreePane" style="display: block;">&#160;
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
      <xsl:if test="/root/EnableFilteringResults='true'">
        <div class="CommunitySearch_OuterResultFilterCategoryContainer">
          <div class="CommunitySearch_FilterCategoriesButton" style="display: none;">
            <xsl:attribute name="id" >CommunitySearch_FilterCategoriesButton<xsl:value-of select="$ControlId" /></xsl:attribute>
            <xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').ToggleFilterCategories(this);</xsl:attribute>
            <xsl:value-of select="/root/stringFilterByCategories"/>
          </div>
          <div class="CommunitySearch_InnerResultFilterCategoryContainer" style="display: none; ">
            <xsl:attribute name="id" >CommunitySearch_ResultFilterCategoryContainer<xsl:value-of select="$ControlId" /></xsl:attribute>
            <fieldset class="CommunitySearch_ResultFilterFieldset">
              <legend>
                <xsl:value-of select="/root/stringCategories"/>
              </legend>
              <table width="100%" cellspacing="0" cellpadding="4" class="b3" >
                <tr>
                  <td>
                    <div style="display: block;" class="CommunitySearch_ResultFilterDiv">
                      <xsl:attribute name="id" >CommunitySearch_ResultFilterCategoryTreePane<xsl:value-of select="$ControlId" /></xsl:attribute>
                    </div>
                    <br />
                  </td>
                </tr>
              </table>
            </fieldset>
          </div>
        </div>
      </xsl:if>
      <div class="CommunitySearch_ResultsContainer">
        <xsl:attribute name="id" >CommunitySearch_ResultsContainer_<xsl:value-of select="$ControlId" /></xsl:attribute>
      </div>
    </div>
  </xsl:template>
  
  <xsl:template name="BuildInfoBlock">
    <div class="CommunitySearch_InfoBlock" style="display: none;" >
      <xsl:attribute name="id" >EkCommunitySearchInfo_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId"/></xsl:attribute>
      <div class="CommunitySearch_InfoBlockHeader">
        <div class="CommunitySearch_InfoBlockCloser">
          <xsl:attribute name="onclick" >CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').HideInfo(this.parentNode.parentNode);</xsl:attribute>
          <img >
            <xsl:attribute name="src"><xsl:value-of select="/root/appImgPath"/>close_red_sm.jpg</xsl:attribute>
          </img>
        </div>
      </div>
      <xsl:choose>
        <xsl:when test="isUser='true'">
          <div class="CommunitySearch_InfoBlockBody">
            <div class="CommunitySearch_InfoBlockItem CommunitySearch_Result_DisplayName">
              <span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringDisplayName"/></span>
              <span class="CommunitySearch_InfoBlockData"><xsl:value-of select="displayName"/></span>
            </div>
            <div class="CommunitySearch_InfoBlockItem CommunitySearch_Result_FirstName">
              <span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringFirstName"/></span>
              <span class="CommunitySearch_InfoBlockData"><xsl:value-of select="firstName"/></span>
            </div>
            <div class="CommunitySearch_InfoBlockItem CommunitySearch_Result_LastName">
              <span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringLastName"/></span>
              <span class="CommunitySearch_InfoBlockData"><xsl:value-of select="lastName"/></span>
            </div>
            <xsl:if test="/root/infoBoxShowsEmail='true'">
              <div class="CommunitySearch_InfoBlockItem CommunitySearch_Result_MemberEmail">
                <span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="/root/stringEmailLabel"/></span>
                <span class="CommunitySearch_InfoBlockData"><xsl:value-of select="email"/></span>
              </div>
            </xsl:if>
            <xsl:apply-templates select="customUserProperties" />
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

    <xsl:if test="../personalTagsTruncated='true'">
		<xsl:call-template name="additionalPersonalTags" />
    </xsl:if>
  </xsl:template>
  
  <xsl:template match="personalTag">
    <span class="CommunitySearch_UserTagLinkContainer" >
      <a href="#" target="_self" class="CommunitySearch_UserTagLink" >
        <xsl:attribute name="onclick">return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SearchTag('<xsl:value-of select="text()"/>', !(<xsl:value-of select="../../isUser" />));</xsl:attribute>
        <xsl:value-of select="text()"/>
      </a>
    </span>
  </xsl:template>

  <xsl:template name="additionalPersonalTags">
      <div class="CommunitySearch_UserTagLinkContainer_Truncated" >
		  <div class="CommunitySearch_UserTag_TruncatedLinkContainer_Less">
			  <span class="CommunitySearch_more_control">
				  <xsl:attribute name="onclick">this.parentNode.setAttribute("class","CommunitySearch_UserTag_TruncatedLinkContainer_More");this.parentNode.setAttribute("className","CommunitySearch_UserTag_TruncatedLinkContainer_More");return false;</xsl:attribute>
				  <xsl:value-of select="/root/stringMore"/>
			  </span>
			  <span class="CommunitySearch_less_control">
				  <xsl:attribute name="onclick">this.parentNode.setAttribute("class","CommunitySearch_UserTag_TruncatedLinkContainer_Less");this.parentNode.setAttribute("className","CommunitySearch_UserTag_TruncatedLinkContainer_Less");return false;</xsl:attribute>
				  <xsl:value-of select="/root/stringLess"/>
			  </span>
			  <div class="CommunitySearch_Result_additionalPersonalTags">
				  <xsl:apply-templates select="additionalPersonalTag" />
			  </div>
		  </div>
	  </div>
  </xsl:template>

	<xsl:template match="additionalPersonalTag">
    <span class="CommunitySearch_UserTagLinkContainer" >
      <a href="#" target="_self" class="CommunitySearch_UserTagLink" >
        <xsl:attribute name="onclick">return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').SearchTag('<xsl:value-of select="text()"/>', !(<xsl:value-of select="../../isUser" />));</xsl:attribute>
        <xsl:value-of select="text()"/>
      </a>
    </span>
  </xsl:template>
	
  <xsl:template match="customUserProperties">
    <xsl:apply-templates select="customUserProperty" />
  </xsl:template>
  
  <xsl:template match="customUserProperty">
    <div class="CommunitySearch_InfoBlockItem CommunitySearch_Result_MemberCustomProperties">
      <span class="CommunitySearch_InfoBlockLabel"><xsl:value-of select="userPropertyName"/></span>
      <span class="CommunitySearch_InfoBlockData"><xsl:value-of select="userPropertyValue"/></span>
    </div>
  </xsl:template>
  
  <xsl:template name="Status">
    <xsl:if test="not(isSelf='true')">
      <xsl:if test="/root/isLoggedIn='true'">
        <xsl:if test="/root/addTargInfo='true'">
          [targ_info_begin]EkUSR_Status_<xsl:value-of select="$ControlId" />_<xsl:value-of select="userId" />[targ_info_end]
        </xsl:if>
			<xsl:choose>
				<xsl:when test="isUser='true'">
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
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose >
						<xsl:when test="groupStatus='request'">
							<xsl:value-of select="/root/stringGroupJoinRequested"/>
						</xsl:when>
						<xsl:when test="groupStatus='member'">
							<xsl:value-of select="/root/stringGroupMember"/>
						</xsl:when>
						<xsl:when test="groupStatus='leader'">
							<xsl:value-of select="/root/stringGroupLeader"/>
						</xsl:when>
						<xsl:otherwise>
							<a target="_self" href="#">
								<xsl:attribute name="onclick" >return CommunitySearchClass.GetObject('<xsl:value-of select="$ControlId" />').DoAjaxSearch('__joingid=<xsl:value-of select="userId" />', '<xsl:value-of select="userId" />', '<xsl:value-of select="$CurrentPage" />');</xsl:attribute>
								<xsl:value-of select="/root/stringGroupJoin"/>
							</a>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
      </xsl:if>
    </xsl:if>
  </xsl:template>
  
  <xsl:template name="EmitResultFilteringXml">
    <wrapper_resultFilteringCategoryXml><xsl:copy-of select="/root/resultFilteringCategoryXml"/></wrapper_resultFilteringCategoryXml>
  </xsl:template>

  <xsl:template name="NotAuthorizedMessage">
    <div class="CommunitySearchCtl">
        <xsl:value-of select="/root/stringAccessDenied"/>
      </div>
  </xsl:template>
</xsl:stylesheet>