<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="result" >
		<xsl:variable name="ViewingReceivedItems" select="(displaymode='messages') and not(folderid = 1)" />
		<xsl:variable name="ViewingSentItems" select="(displaymode='messages') and (folderid = 1)" />
		<xsl:variable name="Composing" select="(displaymode='write')and not((items/item/privatemessageid) &gt; 0)" />
		<xsl:variable name="Replying" select="(displaymode='write') and (forward='0') and((items/item/privatemessageid) &gt; 0)" />
		<xsl:variable name="Forwarding" select="(displaymode='write') and (forward='1') and((items/item/privatemessageid) &gt; 0)" />

		<xsl:choose >
			<xsl:when test="datamode='ajax'">

			</xsl:when>
			<xsl:otherwise>
				<div class="EktMessageCtl">
					<div class="EktMessageCtl_header">
						<span class="EktMessageCtl_header_title">
							<!-- Messaging:&#160;&#160; -->
							<xsl:choose >
								<xsl:when test="$ViewingReceivedItems">
									Inbox
								</xsl:when>
								<xsl:when test="$ViewingSentItems">
									Sent Items
								</xsl:when>
								<xsl:when test="displaymode='message'">
									View Message
								</xsl:when>
								<xsl:when test="$Composing">
									Compose
								</xsl:when>
								<xsl:when test="$Replying">
									Reply
								</xsl:when>
								<xsl:when test="$Forwarding">
									Forward
								</xsl:when>
								<xsl:otherwise>

								</xsl:otherwise>
							</xsl:choose>
						</span>
						<div class="EktMessageCtl_buttonsContainer">
							<div>
								<xsl:choose >
									<xsl:when test="$ViewingReceivedItems">
										<xsl:attribute name="class">EktMessageCtl_buttonsSelected</xsl:attribute>
									</xsl:when>
									<xsl:otherwise>
										<xsl:attribute name="class">EktMessageCtl_buttons</xsl:attribute>
										<xsl:attribute name="onclick">window.location.href='?<xsl:value-of select="/result/dynamicModeParameter" />=messages<xsl:value-of select="/result/externalQuerystringValues"/>';</xsl:attribute>
									</xsl:otherwise>
								</xsl:choose>
								Inbox
							</div>
							<div>
								<xsl:choose >
									<xsl:when test="$Composing">
										<xsl:attribute name="class">EktMessageCtl_buttonsSelected</xsl:attribute>
									</xsl:when>
									<xsl:otherwise>
										<xsl:attribute name="class">EktMessageCtl_buttons</xsl:attribute>
										<xsl:attribute name="onclick">window.location.href='?<xsl:value-of select="/result/dynamicModeParameter" />=pmessage<xsl:value-of select="/result/externalQuerystringValues"/>';</xsl:attribute>
									</xsl:otherwise>
								</xsl:choose>
								Compose
							</div>
							<div class="EktMessageCtl_buttons">
								<xsl:choose >
									<xsl:when test="$ViewingSentItems">
										<xsl:attribute name="class">EktMessageCtl_buttonsSelected</xsl:attribute>
									</xsl:when>
									<xsl:otherwise>
										<xsl:attribute name="class">EktMessageCtl_buttons</xsl:attribute>
										<xsl:attribute name="onclick">window.location.href='?<xsl:value-of select="/result/dynamicFolderIdParameter" />=1<xsl:value-of select="/result/externalQuerystringValues"/>';</xsl:attribute>
									</xsl:otherwise>
								</xsl:choose>
								Sent Items
							</div>
						</div>
					</div>
					<div class="EktMessageCtl_main">
						<xsl:choose>
							<xsl:when test="displaymode='messages'">
								<table class="EktMessageCtl_inboxDataTbl">
									<thead>
										<tr>
											<th class="EktMessageCtl_inboxDataFrom">
												<xsl:choose>
													<xsl:when test="$ViewingSentItems">
														To
													</xsl:when>
													<xsl:otherwise>
														From
													</xsl:otherwise>
												</xsl:choose>
											</th>
											<th class="EktMessageCtl_inboxDataSubject">
												Subject
											</th>
											<th class="EktMessageCtl_inboxDataDate">
												Date
											</th>
											<th class="EktMessageCtl_inboxDataDelete">
												Delete
											</th>
										</tr>
									</thead>
									<tbody>
										<xsl:choose>
											<xsl:when test="count(items/item) &gt; 0">
												<xsl:for-each select="items/item">
													<tr>
														<xsl:attribute name="class">
															<xsl:choose>
																<xsl:when test="position() mod 2 = 1">EktMessageCtl_oddRow<xsl:if test="messageread = 1"> EktMessageCtl_msgRead</xsl:if><xsl:if test="messageread = 0"> EktMessageCtl_msgNotRead</xsl:if></xsl:when>
																<xsl:otherwise>EktMessageCtl_evenRow<xsl:if test="messageread = 1"> EktMessageCtl_msgRead</xsl:if><xsl:if test="messageread = 0"> EktMessageCtl_msgNotRead</xsl:if></xsl:otherwise>
															</xsl:choose>
														</xsl:attribute>
														<td class="EktMessageCtl_inboxDataFrom">
															<xsl:choose>
																<xsl:when test="$ViewingSentItems">
																	<xsl:for-each select="recipients/touser">
																		<xsl:value-of select="node()" /><xsl:choose><xsl:when test="count(../touser)>position()">, </xsl:when></xsl:choose>
																	</xsl:for-each>
																</xsl:when>
																<xsl:otherwise>
																	<xsl:value-of select="fromuser" />
																</xsl:otherwise>
															</xsl:choose>
														</td>
														<td class="EktMessageCtl_inboxDataSubject">
															<a target="_self" title="click to view">
																<xsl:attribute name="href">?<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="privatemessageid" /><xsl:if test="folderid='1'">&amp;<xsl:value-of select="/result/dynamicFolderIdParameter" />=<xsl:value-of select="folderid" /></xsl:if><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>
																<xsl:value-of select="subject" />
															</a>
														</td>
														<td class="EktMessageCtl_inboxDataDate">
															<xsl:value-of select="datecreated" />
														</td>
														<td class="EktMessageCtl_inboxDataDelete">
															<a target="_self" title="click to delete" onclick="if(!confirm('Delete this message?'))return (false);">
																<xsl:attribute name="href">?<xsl:value-of select="/result/dynamicModeParameter" />=delmsg&amp;<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="privatemessageid" /><xsl:if test="folderid='1'">&amp;<xsl:value-of select="/result/dynamicFolderIdParameter" />=1</xsl:if><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>
																<img style="border: none;"   alt="click to delete" >
																	<xsl:attribute name="src"><xsl:value-of select="appImgPath"/>delete2.gif</xsl:attribute>
																</img >
															</a>
														</td>
													</tr>
												</xsl:for-each>
											</xsl:when>
											<xsl:otherwise>
												<tr>
													<td colspan="4"  class="EktMessageCtl_noMessages">
														You have no messages.
													</td>
												</tr>
											</xsl:otherwise>
										</xsl:choose>

									</tbody>
								</table>
							</xsl:when>

							<xsl:when test="displaymode='message'">
								<div class="EktMessageCtl_viewMessageInfo">
									<table class="EktMessageCtl_viewMessageInfoLinksTable" >
										<tr>
											<td >
												<span class="EktMessageCtl_viewMessageInfoReplyLinkSpan">
													<xsl:choose>
														<xsl:when test="items/item/canreply=1">
															<a><xsl:attribute name="href">?<xsl:value-of select="/result/dynamicModeParameter" />=pmessage&amp;<xsl:value-of select="/result/dynamicForwardParameter" />=0&amp;<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="items/item/privatemessageid"/>&amp;<xsl:value-of select="/result/dynamicUserIdParameter" />=<xsl:value-of select="items/item/fromuserid" /><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>Reply</a>
														</xsl:when>
														<xsl:otherwise>&#160;</xsl:otherwise>
													</xsl:choose>
												</span>
											</td>

											<xsl:if test="items/item/canreplyall='true'">
								        <td >
									        <span class="EktMessageCtl_viewMessageInfoReplyAllLinkSpan">
												    <a><xsl:attribute name="href">?<xsl:value-of select="/result/dynamicModeParameter" />=pmessage&amp;<xsl:value-of select="/result/dynamicForwardParameter" />=0&amp;<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="items/item/privatemessageid"/>&amp;<xsl:value-of select="/result/dynamicUserIdParameter" />=<xsl:value-of select="items/item/fromuserid" /><xsl:value-of select="/result/externalQuerystringValues"/>&amp;replyall=1</xsl:attribute>Reply-All</a>
									        </span>
								        </td>
											</xsl:if>
                      
                      					<td >
												<span class="EktMessageCtl_viewMessageInfoForwardMsgLinkSpan">
													<a>
														<xsl:attribute name="href">?<xsl:value-of select="/result/dynamicModeParameter" />=pmessage&amp;<xsl:value-of select="/result/dynamicForwardParameter" />=1&amp;<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="items/item/privatemessageid"/><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>
														Forward
													</a>
												</span>
											</td>
											<td >
												<span class="EktMessageCtl_viewMessageInfoPrintMsgLinkSpan">
													<a href="#" onclick="window.print();">Print</a>
												</span>
											</td>
											<td >
												<span class="EktMessageCtl_viewMessageInfoDeleteMsgLinkSpan">
													<a target="_self" title="click to delete" onclick="if(!confirm('Delete this message?'))return (false);">
														<xsl:attribute name="href">?<xsl:value-of select="/result/dynamicModeParameter" />=delmsg&amp;<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="items/item/privatemessageid"/><xsl:if test="folderid='1'">&amp;<xsl:value-of select="/result/dynamicFolderIdParameter" />=1</xsl:if><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>
														Delete
													</a>
												</span>
											</td>
											<td >
												<xsl:choose >
													<xsl:when test="not(items/item/prevmessageid='0')">
														<span class="EktMessageCtl_viewMessageInfoPrevMsgLinkSpan">
															<a target="_self" title="click for previous message" >
																<xsl:attribute name="href">?<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="items/item/prevmessageid"/><xsl:if test="folderid='1'">&amp;<xsl:value-of select="/result/dynamicFolderIdParameter" />=1</xsl:if><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>
																Previous
															</a>
														</span>
													</xsl:when>
													<xsl:otherwise>
														<span class="EktMessageCtl_viewMessageInfoPrevMsgLinkSpan_Disabled">
															Previous
														</span>
													</xsl:otherwise>
												</xsl:choose>
											</td>
											<td >
												<xsl:choose >
													<xsl:when test="not(items/item/nextmessageid='0')">
														<span class="EktMessageCtl_viewMessageInfoNextMsgLinkSpan">
															<a target="_self" title="click for next message" >
																<xsl:attribute name="href">?<xsl:value-of select="/result/dynamicMessageIDParameter" />=<xsl:value-of select="items/item/nextmessageid"/><xsl:if test="folderid='1'">&amp;<xsl:value-of select="/result/dynamicFolderIdParameter" />=1</xsl:if><xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>
																Next
															</a>
														</span>
													</xsl:when>
													<xsl:otherwise>
														<span class="EktMessageCtl_viewMessageInfoNextMsgLinkSpan_Disabled">
															Next
														</span>
													</xsl:otherwise>
												</xsl:choose>
											</td>
											<td height="22">&#160;</td>
										</tr>
									</table>

									<table cellspacing="0" cellpadding="0" border="0" class="EktMessageCtl_MsgHeadertable">
										<tr>
											<td class="EktMessageCtl_tdLeft">
												From:
											</td>
											<td class="EktMessageCtl_tdRight">
												<xsl:value-of select="items/item/fromuser"/>
											</td>
										</tr>
										<tr>
											<td class="EktMessageCtl_tdLeft">
												Sent:
											</td>
											<td class="EktMessageCtl_tdRight">
												<xsl:value-of select="items/item/datecreated"/>
											</td>
										</tr>
										<tr>
											<td class="EktMessageCtl_tdLeft">
												To:
											</td>
											<td class="EktMessageCtl_tdRight">
												<xsl:for-each select="items/item/recipients/touser">
													<xsl:value-of select="node()" /><xsl:choose><xsl:when test="count(/result/items/item/recipients/touser)>position()">, </xsl:when></xsl:choose>
												</xsl:for-each>
											</td>
										</tr>
										<tr>
											<td class="EktMessageCtl_tdLeft">
												Subject:
											</td>
											<td class="EktMessageCtl_tdRight">
												<xsl:value-of select="items/item/subject"/>
											</td>
										</tr>
									</table>
								</div>
								<div class="EktMessageCtl_viewMessageBody">
									<xsl:value-of select="items/item/message" disable-output-escaping="yes" />
								</div>
							</xsl:when>


							<xsl:when test="displaymode='deleted'">
								<table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top: 12px;">
									<tr>
										<td>
											<table cellspacing="0" cellpading="0" width="100%" border="0" style="margin-bottom: 13px;">
												<tr>
													<td width="170" align="left" valign="top">
														<span class="EktMessageCtl_WriteMsgLinkSpan">
                              <a><xsl:attribute name="href">?<xsl:value-of select="/result/dynamicModeParameter" />=pmessage<xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>Write Message</a>
														</span>
														<br/>
														<span class="EktMessageCtl_InboxLinkSpan">
															<a><xsl:attribute name="href">?<xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>Inbox</a>
														</span>
														<br/>
														<span class="EktMessageCtl_SentItemsLinkSpan">
                              <a><xsl:attribute name="href">?<xsl:value-of select="/result/dynamicFolderIdParameter" />=1<xsl:value-of select="/result/externalQuerystringValues"/></xsl:attribute>Sent Items</a>
														</span>

													</td>
												</tr>

											</table>
										</td>
									</tr>
									<tr>
										<td height="22" align="center">
											<script language="javascript" type="text/javascript">
                        var externalQuerystringValues = '<xsl:value-of select="/result/externalQuerystringValues" disable-output-escaping="yes"/>';
												<xsl:text disable-output-escaping="yes"><![CDATA[
						  <!--
						  function MessagingRedirect(){
							  var loc = location.search;
							  var idx = loc.indexOf('&folder=1');
							  if (idx > 0){
								  window.location.href = location.pathname + '?folder=1' + externalQuerystringValues;
							  }
							  else{
								  window.location.href = location.pathname + externalQuerystringValues.replace("&", "?");
							  }
						  }
						  MessagingRedirect();
						  //-->
						  ]]></xsl:text>
											</script>
										</td>
									</tr>
								</table>

							</xsl:when >

							<xsl:when test="displaymode='notloggedin'">
								<b>You must be logged in to access this!</b>
							</xsl:when >

							<xsl:when test="displaymode='write'">
								<table class="EktMessageCtl_content" cellspacing="1" cellpadding="0">
									<tr>
										<td class="EktMessageCtl_postformheader">To:</td>
										<td class="EktMessageCtl_post">
											<input type="text" class="EktMessageCtl_edit" size="55">
												<xsl:attribute name="id">ekpmsgto<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="name">ekpmsgto<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="disabled" />
												<xsl:attribute name="value"><xsl:value-of select="items/item/to"/></xsl:attribute>
											</input >
											<a class="ek_thickbox" >
												<xsl:attribute name="href">#EkTB_inline?height=480&amp;width=450&amp;caption=false&amp;inlineId=<xsl:value-of select="controlid"/>_MessageTargetUI&amp;modal=true</xsl:attribute>
												<xsl:attribute name="onclick" >CommunityMsgClass.GetObject('<xsl:value-of select="controlid"/>').MsgShowMessageTargetUI('ektouserid<xsl:value-of select="controlid"/>', <xsl:value-of select="enablepresearch"/>);</xsl:attribute>
												<xsl:choose >
                          <xsl:when test="items/item/togroupid&gt;'0'"></xsl:when>
                          <xsl:when test="recipients='friendsonly'">
														<img alt="Browse Friends"  style="border: none;" >
                              <xsl:attribute name="src"><xsl:value-of select="items/item/appImgPath" />folder_who.png</xsl:attribute >
														</img> Browse Friends
													</xsl:when>
													<xsl:otherwise>
														<img alt="Browse Users" style="border: none;" >
															<xsl:attribute name="src"><xsl:value-of select="items/item/appImgPath" />folder_who.png</xsl:attribute >
														</img>Browse Users
													</xsl:otherwise>
												</xsl:choose>
											</a>
										</td>
									</tr>
									<tr>
										<td class="EktMessageCtl_postformheader">Subject:</td>
										<td class="EktMessageCtl_post">
											<input type="text" class="EktMessageCtl_edit" size="55" >
												<xsl:attribute name="id">ekpmsgsubject<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="name">ekpmsgsubject<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:choose >
													<xsl:when test="$Replying">
														<xsl:attribute name="value"><xsl:value-of select="items/item/subject"/></xsl:attribute>
													</xsl:when>
													<xsl:when test="$Forwarding">
														<xsl:attribute name="value">
															<xsl:value-of select="items/item/subject"/>
														</xsl:attribute>
													</xsl:when>
													<xsl:otherwise>
														<xsl:attribute name="value"></xsl:attribute>
													</xsl:otherwise>
												</xsl:choose>
											</input>
										</td>
									</tr>
									<tr>
										<td class="EktMessageCtl_postformheader" vAlign="top">
											Message:<br/>&#160;
										</td>
										<td class="EktMessageCtl_post">
											<msgeditor></msgeditor>
										</td>
									</tr>

									<tr>
										<td align="middle" colSpan="2" class="EktMessageCtl_footer1">
                      <input type="hidden">
												<xsl:attribute name="id" >ektogroupid<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="name">ektogroupid<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="value"><xsl:value-of select="items/item/togroupid"/></xsl:attribute>
											</input>
											<input type="hidden">
												<xsl:attribute name="id" >ektouserid<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="name">ektouserid<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="value"><xsl:value-of select="items/item/touserid"/></xsl:attribute>
											</input>
											<input type="submit" value=" Send " class="EktMessageCtl_pbutton" >
												<xsl:attribute name="name">ekpostpostreply<xsl:value-of select="controlid"/></xsl:attribute>
												<xsl:attribute name="onclick" >ekpmsgsubject<xsl:value-of select="controlid"/>.value=StripHtml(ekpmsgsubject<xsl:value-of select="controlid"/>.value);return CommunityMsgClass.GetObject('<xsl:value-of select="controlid"/>').SendMessage()</xsl:attribute>
											</input>
											<input type="button" value="Cancel" class="EktMessageCtl_pbutton" >
												<xsl:attribute name="onclick">window.location.href='?<xsl:value-of select="/result/dynamicModeParameter" />=messages<xsl:value-of select="/result/externalQuerystringValues"/>';</xsl:attribute>
												<xsl:attribute name="name">ekpostcancel<xsl:value-of select="controlid"/></xsl:attribute>
											</input>
										</td>
									</tr>
								</table>
								<xsl:call-template name="EmitJavascriptForComposing" />
							</xsl:when>
							<xsl:when test="displaymode='sentmessage'">
								<div class="EktMessageCtl_msgSent" >
									Your message has been sent.
								</div>
							</xsl:when>
							<xsl:when test="displaymode='error'">
								<div class="EktMessageCtl_Error" >
									ERROR! (unkown cause)
								</div>
							</xsl:when>
							<xsl:otherwise>

							</xsl:otherwise>
						</xsl:choose>
					</div>
				</div>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

	<xsl:template name="EmitJavascriptForComposing">
		<xsl:text disable-output-escaping="yes">
		  <![CDATA[

		  ]]>
		</xsl:text>
	</xsl:template>
</xsl:stylesheet>