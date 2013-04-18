<?xml version="1.0" encoding="UTF-8" ?> 
 <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
 <xsl:template match="/">
  <script type="text/javascript" src="[[ekapppath]]java/rating5starComment.js" /> 
 <input type="hidden" name="[[ekkey]]ekrelected" id="[[ekkey]]ekrelected">
 <xsl:attribute name="value">
 <xsl:choose>
 <xsl:when test="result/general/rated='1'">
  <xsl:value-of select="result/general/myrating" /> 
  </xsl:when>
 <xsl:otherwise>
  <xsl:value-of select="result/general/average" /> 
  </xsl:otherwise>
  </xsl:choose>
  </xsl:attribute>
  </input>
  <input type="hidden" name="ekkey" id="ekkey" value="[[ekkey]]" /> 
  <input type="hidden" name="ekapppath" id="ekapppath" value="[[ekapppath]]" /> 
  <input type="hidden" name="ekimgpath" id="ekimgpath" value="[[ekimgpath]]" /> 
  <input type="hidden" name="[[ekkey]]_mod" id="[[ekkey]]_mod" value="[[mod]]" /> 
 <input type="hidden" name="[[ekkey]]_israted" id="[[ekkey]]_israted">
 <xsl:attribute name="value">
 <xsl:choose>
  <xsl:when test="result/general/rated='1'">1</xsl:when> 
  <xsl:otherwise>0</xsl:otherwise> 
  </xsl:choose>
  </xsl:attribute>
  </input>
  <input type="hidden" name="[[ekkey]]ekcontentid" id="[[ekkey]]ekcontentid" value="[[ekcontentid]]" /> 
 <img id="[[ekkey]]ri2" alt="" onclick="rhdlr('[[ekkey]]',2,'rate');" onmouseover="rhdlr('[[ekkey]]',2,'over')" onmouseout="rhdlr('[[ekkey]]',2,'out');">
 <xsl:attribute name="src">
 <xsl:choose>
  <xsl:when test="result/general/rated='1' and result/general/myrating&gt;0">[[ekimgpath]]star_s.gif</xsl:when> 
  <xsl:when test="result/general/rated='1' and result/general/myrating&lt;2">[[ekimgpath]]star_e.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&gt;0">[[ekimgpath]]star_f.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&lt;2">[[ekimgpath]]star_e.gif</xsl:when> 
  </xsl:choose>
  </xsl:attribute>
  </img>
 <img id="[[ekkey]]ri4" alt="" onclick="rhdlr('[[ekkey]]',4,'rate');" onmouseover="rhdlr('[[ekkey]]',4,'over')" onmouseout="rhdlr('[[ekkey]]',4,'out');">
 <xsl:attribute name="src">
 <xsl:choose>
  <xsl:when test="result/general/rated='1' and result/general/myrating&gt;2">[[ekimgpath]]star_s.gif</xsl:when> 
  <xsl:when test="result/general/rated='1' and result/general/myrating&lt;4">[[ekimgpath]]star_e.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&gt;2">[[ekimgpath]]star_f.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&lt;4">[[ekimgpath]]star_e.gif</xsl:when> 
  </xsl:choose>
  </xsl:attribute>
  </img>
 <img id="[[ekkey]]ri6" alt="" onclick="rhdlr('[[ekkey]]',6,'rate');" onmouseover="rhdlr('[[ekkey]]',6,'over')" onmouseout="rhdlr('[[ekkey]]',6,'out');">
 <xsl:attribute name="src">
 <xsl:choose>
  <xsl:when test="result/general/rated='1' and result/general/myrating&gt;4">[[ekimgpath]]star_s.gif</xsl:when> 
  <xsl:when test="result/general/rated='1' and result/general/myrating&lt;6">[[ekimgpath]]star_e.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&gt;4">[[ekimgpath]]star_f.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&lt;6">[[ekimgpath]]star_e.gif</xsl:when> 
  </xsl:choose>
  </xsl:attribute>
  </img>
 <img id="[[ekkey]]ri8" alt="" onclick="rhdlr('[[ekkey]]',8,'rate');" onmouseover="rhdlr('[[ekkey]]',8,'over')" onmouseout="rhdlr('[[ekkey]]',8,'out');">
 <xsl:attribute name="src">
 <xsl:choose>
  <xsl:when test="result/general/rated='1' and result/general/myrating&gt;6">[[ekimgpath]]star_s.gif</xsl:when> 
  <xsl:when test="result/general/rated='1' and result/general/myrating&lt;8">[[ekimgpath]]star_e.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&gt;6">[[ekimgpath]]star_f.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&lt;8">[[ekimgpath]]star_e.gif</xsl:when> 
  </xsl:choose>
  </xsl:attribute>
  </img>
 <img id="[[ekkey]]ri10" alt="" onclick="rhdlr('[[ekkey]]',10,'rate');" onmouseover="rhdlr('[[ekkey]]',10,'over')" onmouseout="rhdlr('[[ekkey]]',10,'out');">
 <xsl:attribute name="src">
 <xsl:choose>
  <xsl:when test="result/general/rated='1' and result/general/myrating&gt;8">[[ekimgpath]]star_s.gif</xsl:when> 
  <xsl:when test="result/general/rated='1' and result/general/myrating&lt;10">[[ekimgpath]]star_e.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&gt;8">[[ekimgpath]]star_f.gif</xsl:when> 
  <xsl:when test="result/general/rated='0' and result/general/average&lt;10">[[ekimgpath]]star_e.gif</xsl:when> 
  </xsl:choose>
  </xsl:attribute>
  </img>
    
 <span class="rating_avg">
 <xsl:if test="result/general/staraverage > -1">
   <xsl:value-of select="/result/resourceStrings/stringAverage"/><xsl:text> </xsl:text><xsl:value-of select="result/general/staraverage" /><xsl:text> </xsl:text><xsl:value-of select="/result/resourceStrings/stringOutOf5"/>
  </xsl:if>
  </span>
  <br /> 
 <div id="[[ekkey]]Popup" class="ratingFlyoutPopup" style="position: absolute;border: 1px solid black;background-color:white;padding:4px;visibility:hidden;">
  <span id="Flyout_lbOptText" class="OptionalText"><xsl:value-of select="/result/resourceStrings/stringFeedback"/></span> 
  <br /> 
 <textarea name="[[ekkey]]_tbComments" rows="3" cols="24" id="[[ekkey]]_tbComments" class="Comment">
  <xsl:value-of select="result/general/review" /> 
  </textarea>
  <br />
   <input type="button" name="Flyout_btnSubmit" onclick="rhdlr('[[ekkey]]',-1,'click');" id="Flyout_btnSubmit" title="Send This Content" class="Button"><xsl:attribute name="value"><xsl:value-of select="/result/resourceStrings/stringSend"/></xsl:attribute></input>
     
  <input type="button" name="Flyout_btnClose" onclick="dhdlr('[[ekkey]]','close');" id="ctl00_tb1_Flyout_btnClose" title="Close This Window" class="Button" ><xsl:attribute name="value"><xsl:value-of select="/result/resourceStrings/stringClose"/></xsl:attribute></input> 
  </div>
 
 <!--<script language="JavaScript" type="text/javascript">
  <xsl:text>setTimeout("hideTag()",5000);function hideTag(){document.getElementById("[[ekkey]]Popup_Msg").style.display='none';}</xsl:text> 
  </script>-->
  
 <div id="[[ekkey]]Popup_Msg" class="ratingFlyoutPopupMsg" style="position: absolute;border: 1px solid black;background-color:white;padding:4px;visibility:hidden;">
  <span id="Flyout_msg_lbOptText" class="OptionalText"><xsl:value-of select="/result/resourceStrings/stringThankYou"/></span> 
  <br /> 
  <input type="button" name="Flyout_msg_btnClose" value="Close" onclick="dhdlr('[[ekkey]]','closemsg');" id="ctl00_tb1_Flyout_msg_btnClose" title="Close This Window" class="Button" /> 
  </div>



	 <table border="0" width="100%">
		 <tr>
			 <td align="left">
				 <b>Reviews</b>
			 </td>
		 </tr>
		 <xsl:for-each select="result/items/item">
			 <tr>
				 <td>&#160;</td>
			 </tr>
			 <tr>
				 <td align="left">
					 <!--<a>
						 <xsl:attribute name="href">
							 <xsl:value-of select="contentquicklink"/>
						 </xsl:attribute>
						 <xsl:value-of select="contenttitle"/>
					 </a>-->
					 <xsl:choose>
						 <xsl:when test="state=0">
							 &#160;<font color="red">Pending Approval</font>
						 </xsl:when>
						 <xsl:when test="state=2">
							 &#160;<font color="red">
								 <b>Rejected</b>
							 </font>
						 </xsl:when>
					 </xsl:choose>
				 </td>
			 </tr>
			 <tr>
				 <td align="left">
					 <img id="ri1" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;0">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
								 <xsl:when test="rating&lt;1">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri2" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;1">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
								 <xsl:when test="rating&lt;2">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri3" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;2">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
								 <xsl:when test="rating&lt;3">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri4" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;3">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
								 <xsl:when test="rating&lt;4">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri5" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;4">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
								 <xsl:when test="rating&lt;5">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri6" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;5">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
								 <xsl:when test="rating&lt;6">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri7" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;6">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
								 <xsl:when test="rating&lt;7">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri8" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;7">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
								 <xsl:when test="rating&lt;8">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri9" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;8">[[ekimgpath]]smallstar_l_s.gif</xsl:when>
								 <xsl:when test="rating&lt;9">[[ekimgpath]]smallstar_l_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
					 <img id="ri10" alt="">
						 <xsl:attribute name="src">
							 <xsl:choose>
								 <xsl:when test="rating&gt;9">[[ekimgpath]]smallstar_r_s.gif</xsl:when>
								 <xsl:when test="rating&lt;10">[[ekimgpath]]smallstar_r_e.gif</xsl:when>
							 </xsl:choose>
						 </xsl:attribute>
					 </img>
				 </td>
			 </tr>
			 <tr>
				 <td align="left">
					 By <xsl:choose>
						 <xsl:when test="userid&gt;0">
							 <a>
								 <!--<xsl:attribute name="href">
									 _userreviews.aspx?userid=<xsl:value-of select="userid"/>
								 </xsl:attribute>-->
								 <xsl:value-of select="userdisplayname"/>
							 </a>
						 </xsl:when>
						 <xsl:otherwise>Anonymous</xsl:otherwise>
					 </xsl:choose>
					 <br/>
					 <xsl:value-of select="review"/>
				 </td>
			 </tr>
		 </xsl:for-each>
	 </table>
  </xsl:template>
  </xsl:stylesheet>