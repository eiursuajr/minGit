<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="no"/>
	<xsl:variable name="imgPath" select="/result/config/imagePath"/>
	<xsl:variable name="uniqueId" select="/result/config/uniqueId"/>
	<xsl:variable name="defaultOptionalMsg" select="/result/config/defaultOptionalMsg"/>
	<xsl:template match="/">
		<xsl:choose>
			<xsl:when test="/result/config/displayMode='baseUI'">
				<xsl:apply-templates select="/result" mode="baseUI"/>
			</xsl:when>
			<xsl:when test="/result/config/displayMode='responseUI'">
				<xsl:apply-templates select="/result" mode="responseUI"/>
			</xsl:when>
			<xsl:when test="/result/config/displayMode='notLoggedIn'">
				<xsl:apply-templates select="/result" mode="notLoggedIn"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="/result" mode="baseUI"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="/result" mode="baseUI">
		<div class="EktInviteCtl">
			<xsl:if test="string-length(normalize-space(background/itemImage)) &gt; '0'">
				<xsl:attribute name="style">
					background-image: url(<xsl:value-of select="$imgPath"/><xsl:value-of select="background/itemImage"/>);
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="normalize-space(titlebar/itemEnabled)=true()">
				<h2 class="EktInviteCtl_HeaderBar">
					<xsl:if test="string-length(normalize-space(titlebar/itemText)) &gt; '0'">
						<xsl:value-of select="titlebar/itemText"/>
					</xsl:if>
					<xsl:if test="string-length(normalize-space(titlebar/itemImage)) &gt; '0'">
						<img alt="invite title image">
							<xsl:attribute name="src"><xsl:value-of select="$imgPath"/><xsl:value-of select="titlebar/itemImage"/></xsl:attribute>
						</img>
					</xsl:if>
				</h2>
			</xsl:if>
			<div class="EktInviteCtl_body">
				<xsl:if test="normalize-space(info/itemEnabled)=true()">
					<h3 class="EktInviteCtl_infoContainer">
						<xsl:if test="string-length(normalize-space(info/itemText)) &gt; '0'">
							<xsl:value-of select="info/itemText"/>
						</xsl:if>
						<xsl:if test="string-length(normalize-space(info/itemImage)) &gt; '0'">
							<img alt="invite info image">
								<xsl:attribute name="src"><xsl:value-of select="$imgPath"/><xsl:value-of select="info/itemImage"/></xsl:attribute>
							</img>
						</xsl:if>
					</h3>
				</xsl:if>
				<div class="EktInviteCtl_addressContainer">
					<div class="EktInviteCtl_addressInfo">
						<label class="EktInviteCtl_addressTitleContainer"><xsl:attribute name="for"><xsl:value-of select="$uniqueId"/>_inviteEmailAddress</xsl:attribute>Recipient email addresses:</label>
						<span class="EktInviteCtl_addressHelpLink">
							<a href="#" onclick="return false;">Help
								<div class="EktInviteCtl_addressHelpContainer">
									<xsl:attribute name="id"><xsl:value-of select="$uniqueId" />_inviteEmailAddressHelp</xsl:attribute>
									Seperate addresses using either commas, spaces, semi-colons, or new-lines.
								</div>
							</a>
						</span>
					</div>
					<textarea onfocus="InitInviteTextArea(this);">
						<xsl:attribute name="id"><xsl:value-of select="$uniqueId"/>_inviteEmailAddress</xsl:attribute>
						<xsl:attribute name="name"><xsl:value-of select="$uniqueId"/>_inviteEmailAddress</xsl:attribute>
						<xsl:attribute name="cols"><xsl:value-of select="emails/columns"/><xsl:if test="string-length(normalize-space(message/columns)) = '0'">30</xsl:if></xsl:attribute>
						<xsl:attribute name="rows"><xsl:value-of select="emails/rows"/><xsl:if test="string-length(normalize-space(message/rows)) = '0'">30</xsl:if></xsl:attribute>
						<xsl:text></xsl:text>
					</textarea>
				</div>
				<xsl:if test="normalize-space(message/itemEnabled)=true()">
					<div class="EktInviteCtl_messageContainer">
						<label class="EktInviteCtl_messageTitleContainer">
							<xsl:attribute name="for"><xsl:value-of select="$uniqueId"/>_inviteMessage</xsl:attribute>
							<xsl:if test="string-length(normalize-space(message/itemText)) &gt; '0'">
								<xsl:value-of select="message/itemText"/>
							</xsl:if>
							<xsl:if test="string-length(normalize-space(message/itemImage)) &gt; '0'">
								<img alt="invite message image">
									<xsl:attribute name="src"><xsl:value-of select="$imgPath"/><xsl:value-of select="message/itemImage"/></xsl:attribute>
								</img>
							</xsl:if>
						</label>
						<textarea>
							<xsl:attribute name="id"><xsl:value-of select="$uniqueId"/>_inviteMessage</xsl:attribute>
							<xsl:attribute name="name"><xsl:value-of select="$uniqueId"/>_inviteMessage</xsl:attribute>
							<xsl:attribute name="cols"><xsl:value-of select="message/columns"/><xsl:if test="string-length(normalize-space(message/columns)) = '0'">30</xsl:if></xsl:attribute>
							<xsl:attribute name="rows"><xsl:value-of select="message/rows"/><xsl:if test="string-length(normalize-space(message/rows)) = '0'">30</xsl:if></xsl:attribute>
							<xsl:value-of select="$defaultOptionalMsg"/>
						</textarea>
					</div>
				</xsl:if>
				<div class="EktInviteCtl_sendButtonContainer">
					<input id="inviteSendBtn" name="inviteSendBtn" value="Send Invitations" onclick="return(SendInviteMsg(this));">
						<xsl:attribute name="id"><xsl:value-of select="$uniqueId"/>_inviteSendBtn</xsl:attribute>
						<xsl:attribute name="name"><xsl:value-of select="$uniqueId"/>_inviteSendBtn</xsl:attribute>
						<xsl:choose>
							<xsl:when test="string-length(normalize-space(button/itemImage)) &gt; '0'">
								<xsl:attribute name="type">image</xsl:attribute>
								<xsl:attribute name="src"><xsl:value-of select="$imgPath"/><xsl:value-of select="button/itemImage"/></xsl:attribute>
							</xsl:when>
							<xsl:otherwise>
								<xsl:attribute name="type">button</xsl:attribute>
							</xsl:otherwise>
						</xsl:choose>
						<xsl:if test="string-length(normalize-space(button/itemText)) &gt; '0'">
							<xsl:attribute name="value"><xsl:value-of select="button/itemText"/></xsl:attribute>
						</xsl:if>
					</input>
				</div>
			</div>
			<input type="hidden" value="">
				<xsl:attribute name="id"><xsl:value-of select="$uniqueId"/>_inviteHdn</xsl:attribute>
				<xsl:attribute name="name"><xsl:value-of select="$uniqueId"/>_inviteHdn</xsl:attribute>
			</input>
			<script language="javascript" type="text/javascript">
				<xsl:text disable-output-escaping="yes"><![CDATA[
					<!--
					function SendInviteMsg(btnObj){
						var uniqueId = btnObj.id.substr(0, btnObj.id.indexOf('_'));
						var emailObj = document.getElementById(uniqueId + '_inviteEmailAddress');
						if (emailObj.inviteInitialized && (emailObj.value.length > 0)){
							var email = InviteCleanEmails(emailObj.value);
							if (email.length < 100){
								if (InviteValidateEmail(email)) {
									var hdnObj = document.getElementById(uniqueId + '_inviteHdn');
									hdnObj.value = '1'; // this validates action for server...
									var act = document.forms[0].action;
									if (act){
										if (act.indexOf('?') > 0){
											act += '&invid=' + uniqueId;
										}
										else{
											act += '?invid=' + uniqueId;
										}
										emailObj.value = email;
										document.forms[0].action = act;
										document.forms[0].submit();
									}
									return (true);
								}
								else{ 
									alert('Error: Invalid email address detected!');
									return (false);
								}
							}
							else{
								alert('Error: Email address too long!');
								return (false);
							}
						}
						else {
							alert('Error: you need to enter the recipients email address!');
							return (false);
						}
					}
					
					function InitInviteTextArea(taObj){
						if (!taObj.inviteInitialized){
							taObj.inviteInitialized = true;
							taObj.value = '';
						}
					}

					function InviteCleanEmails(emails){
						var result = emails;
						var delim = ';';
						var idx = 0;
						var emailAdd;
						result = InviteReplaceAll(result, '\n', delim); // NewLine
						result = InviteReplaceAll(result, '\'', delim);
						result = InviteReplaceAll(result, String.fromCharCode(13, 10), delim); // <CR>
						result = InviteReplaceAll(result, String.fromCharCode(13), delim); // <CR>
						result = InviteReplaceAll(result, String.fromCharCode(10), delim); // <LF>
						result = InviteReplaceAll(result, String.fromCharCode(9), delim); // <Tab>
						result = InviteReplaceAll(result, '|', delim);
						result = InviteReplaceAll(result, '"', delim);
						result = InviteReplaceAll(result, ',', delim);
						result = InviteReplaceAll(result, ' ', delim);
						
						// Remove repeating-delimiters:
						result = InviteReplaceAll(result, delim + delim, delim);
						
						// Remove delimiter at the very beginning, if it exists:
						if ((result.length > 0) && (delim == result.substr(0, 1))){
							result = result.substr(1);
						}

						// Remove any trailing delimiter:
						if (result.length && (delim == result.substr(result.length - 1))){
							result = result.substr(0, result.length - 1);
						}
						
						// Remove duplicates:
						var emailArray = result.split(';');
						result = '';
						for (var idxOuter = 0; idxOuter < emailArray.length; idxOuter++){
							var dupeFound = false;
							for (var idxInner = idxOuter + 1; idxInner < emailArray.length; idxInner++){
								if (emailArray[idxOuter] == emailArray[idxInner]){
									dupeFound = true;
									break;
								}
							}
							if (!dupeFound){
								if (result.length){
									result += delim;
								}
								result += emailArray[idxOuter];
							}
						}
						
						return (result);
					}

					function InviteReplaceAll(text, origVal, newVal){
						var result = text;
						var flag = (origVal.length > 0);
						while (flag){
							result = result.replace(origVal, newVal);
							flag = (result.indexOf(origVal) >= 0);
						}
						return (result);
					}
					
					function InviteValidateEmail(emails){
						var result = false;
						var idx = 0;
						var emailAdd;
						var emailArray = emails.split(';');
						for (idx=0; idx < emailArray.length; idx++){
							emailAdd = emailArray[idx].replace(' ', '').replace(';', '');
							if (emailAdd.length > 0){
								if (!InviteIsEmail(emailAdd)){
									result = false;
									break;
								}
								else {
									result = true;
								}
							}
						}
						return (result);
					}
					
					function InviteIsEmail(strData) {
						var	iCtr,jCtr,sLength,atPos,cs,cTemp;

						varErrMsg='Invalid email address detected!';
						if (InviteIsEmpty(strData))
							if (isEmail.arguments.length == 1)
								return false;
							else
								return (isEmail.arguments[1] == true);

							   
						if (InviteIsWhitespace(strData))
							return false;
							    
						iCtr = 1;
						sLength = strData.length;

						iCtr = strData.indexOf('@');
						atPos = iCtr;
							    
						if ( iCtr < 0 )
							return false;
						else 
							iCtr+=2;
						
						iCtr = strData.lastIndexOf('.');
						if ( iCtr < 0 )
							return false;
						else 
							iCtr++;			
								    
						if (iCtr > sLength)
							return false;
								
						cTemp	= '';
						cs		= '';
						for(jCtr = atPos+1; jCtr < strData.length; jCtr++) {
							cTemp =  strData.charAt(jCtr);
							if( (cTemp != '.') && (cTemp != '-') )
								cs += cTemp;
						}
								
						iCtr = strData.lastIndexOf( ' ' );
						if ( iCtr > 0 )
							return false;
						else 
							iCtr++;	
						for(i=0;i<strData.length;i++)
						{
							cTemp =  strData.charAt(i);
							if((cTemp=='?')||(cTemp=='(')||(cTemp==')')||(cTemp=='=')||(cTemp=='+')||(cTemp=='~')||(cTemp=='`')||(cTemp=='!')||(cTemp=='#')||(cTemp=='$')||(cTemp=='%')||(cTemp=='^')||(cTemp=='&')||(cTemp=='*'))
							{
								return false;
							}
						}
						return true;
					}
					
					function InviteIsEmpty(strData) {
						return ((strData == null) || (strData.length == 0));
					}

					var InviteWhitespace = ' \t\n\r';
					
					function InviteIsWhitespace(strData) {
						var iCtr,cTemp;
						if (InviteIsEmpty(strData))
							return true;
						for (iCtr = 0; iCtr < strData.length; iCtr++) {   
							var cTemp = strData.charAt(iCtr);
							if (InviteWhitespace.indexOf(cTemp) == -1)
								return false;
						}
						return true;
					}

					//-->
					]]></xsl:text>
			</script>
		</div>
	</xsl:template>
	<xsl:template match="/result" mode="responseUI">
		<div class="EktInviteCtl">
			<xsl:if test="string-length(normalize-space(background/itemImage)) &gt; '0'">
				<xsl:attribute name="style">background-image: url(<xsl:value-of select="$imgPath"/><xsl:value-of select="background/itemImage"/>);</xsl:attribute>
			</xsl:if>
			<xsl:if test="normalize-space(titlebar/itemEnabled)=true()">
				<h2 class="EktInviteCtl_HeaderBar">
					<xsl:if test="string-length(normalize-space(titlebar/itemText)) &gt; '0'">
						<xsl:value-of select="titlebar/itemText"/>
					</xsl:if>
					<xsl:if test="string-length(normalize-space(titlebar/itemImage)) &gt; '0'">
						<img alt="invite title image">
							<xsl:attribute name="src"><xsl:value-of select="$imgPath"/><xsl:value-of select="titlebar/itemImage"/></xsl:attribute>
						</img>
					</xsl:if>
				</h2>
			</xsl:if>
			<div class="EktInviteCtl_body">
				<xsl:if test="normalize-space(info/itemEnabled)=true()">
					<div class="EktInviteCtl_infoContainer">
						<xsl:if test="string-length(normalize-space(info/itemText)) &gt; '0'">
							<xsl:value-of select="info/itemText"/>
						</xsl:if>
						<xsl:if test="string-length(normalize-space(info/itemImage)) &gt; '0'">
							<img alt="invite info image">
								<xsl:attribute name="src"><xsl:value-of select="$imgPath"/><xsl:value-of select="info/itemImage"/></xsl:attribute>
							</img>
						</xsl:if>
					</div>
					<ul>
						<xsl:apply-templates select="items"/>
					</ul>
				</xsl:if>
			</div>
		</div>
	</xsl:template>
	<xsl:template match="items/item">
		<li>
			<xsl:if test="(email) and not(email='')">
				<xsl:value-of select="email"/>
			</xsl:if>
			<xsl:choose>
				<xsl:when test="(errorMsg) and not(errorMsg='')">
					<span class="ekError"><xsl:value-of select="errorMsg"/></span>
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="'true'=isUser">&#160;<xsl:value-of select="/result/config/alreadyUserMsg"/>
						</xsl:when>
						<xsl:otherwise>&#160;<xsl:value-of select="/result/config/sentInvitationMsg"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</li>
	</xsl:template>
	<xsl:template match="/result" mode="notLoggedIn">
		<b>You must be logged in to use this!</b>
	</xsl:template>
</xsl:stylesheet>