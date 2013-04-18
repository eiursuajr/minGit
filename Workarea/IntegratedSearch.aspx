<%@ Page Language="C#" AutoEventWireup="true" Inherits="IntegratedSearch" CodeFile="IntegratedSearch.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
        <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
        <asp:Literal ID="ltrStyleSheet" runat="server" ></asp:Literal>
		<title>
			<asp:Literal id="ltrTitle" runat="server"/>
		</title>
        
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
	            function SubmitForm(FormName, Validate)
	            {
	                if (Validate.length > 0)
		            {
			            if (eval(Validate))
			            {
				            $ektron('#pleaseWait').modalShow();
				            document.forms[FormName].submit();
				            return false;
			            }
			            else
			            {
				            return false;
			            }
		            }
		            else
		            {
			            $ektron('#pleaseWait').modalShow();
			            document.forms[FormName].submit();
			            return false;
		            }
	            }
		            function VerifyIntegratedSearchForm ()
	            {
	                //check for name empty
		            document.integratedform.frm_directoryname.value = Trim(document.integratedform.frm_directoryname.value);
		            if (document.integratedform.frm_directoryname.value == "")
		            {
			            alert('<asp:Literal runat="server" id="ltr_dirRequired" />');
			            document.integratedform.frm_directoryname.focus();
			            return false;
		            }
		            //check for duplicates
		            if (!ValidateFolderNameNotUsed())
		            {
			            return false;
		            }
		            //check for name validation
		            for (var lLoop = 0; lLoop < document.integratedform.frm_directoryname.value.length; lLoop++)
		            {
			            var Char = document.integratedform.frm_directoryname.value.substring(lLoop, lLoop + 1);
			            if ((Char == '\\') || (Char == '/') || (Char == ':') || (Char == '*') || (Char == '?') || (Char == '\"') || (Char == '<') || (Char == '>') || (Char == '|'))
			            {
				            alert('<asp:Literal runat="server" id="ltr_nonChar" />');
				            document.integratedform.frm_directoryname.focus();
				            return false;
			            }
		            }

		            if (document.getElementById("Password").value != document.getElementById("ConfirmPassword").value)
		            {
			            alert('<asp:Literal runat="server" id="ltr_pwdCnfrm" />');
			            document.integratedform.Password.focus();
			            return false;
	                }

		            return true;
	            }

	            function ValidateFolderNameNotUsed()
	            {
		            var namesObj = window.document.getElementById("integrated_fol_names");
		            if (null == namesObj)
		            {
			            // We return true if this hidden field doesn't exist,
			            // as this indicates that we are NOT creating new Meta:
			            return true;
		            }
		            else
		            {
			            var newNameObj = document.getElementById("DirectoryName");
			            if (null == newNameObj)
			            {
				            return false;
			            }
			            else
			            {
				            var newMetaName = newNameObj.value;
				            var folderNames = namesObj.value.split(",");
				            for (var idx=0; idx < folderNames.length; idx++)
				            {
					            if (folderNames[idx] == newMetaName)
					            {
						            alert('<asp:Literal runat="server" id="ltr_nameInUse" />');
						            return false;
					            }
				            }
				            return true;
			            }
		            }
	            }


	            function ConfirmDelete()
	            {
		            if (!confirm("Do you want to delete this integrated search folder"))
		            {
			            return false;
		            }
		            return confirm("<asp:Literal runat='server' id='ltr_confmMoment' />");
	            }
	            
	            Ektron.ready( function (){
	                $ektron("#pleaseWait").modal(
                    {
                        trigger: '',
                        modal: true,
                        toTop: true,
                        onShow: function(hash){
                            hash.o.fadeIn();
                            hash.w.fadeIn();
                        },
                        onHide: function(hash) 
                        {
                            hash.w.fadeOut("fast");
	                        hash.o.fadeOut("fast", function(){
	                            if (hash.o)
	                            {
		                            hash.o.remove();
                                }
                            });
                        }
                    });   
	            });
            //--><!]]>
        </script>
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
            div#pleaseWait
            {
                width: 128px;
                height: 128px;
                margin: -64px 0 0 -64px;
                background-color: #fff;
                background-image: url("images/ui/loading_big.gif");
                backgground-repeat: no-repeat;
                text-indent: -10000px;
                border: none;
                padding: 0;
                top: 50%;
            }
            /*]]>*/-->
        </style>
            
	</head>
    <body>
    <asp:Literal id="DebugErrLit" runat="server"></asp:Literal>
        <asp:Panel ID="pnlViewAllIntegratedFolders" Visible="false"  runat="server" >
            <div class="ektronPageHeader">
	                <div class="ektronTitlebar" title="View Integrated Search Folders">
		                <%=objStyle.GetTitleBar(gtMess.GetMessage("alt view integrated search folders"))%>
		            </div>
		            <div class="ektronToolbar">
			            <table>
				            <tr>
					            <%=objStyle.GetButtonEventsWCaption(AppPath + "images/UI/Icons/add.png", "integratedsearch.aspx?action=AddIntegratedFolder", gtMess.GetMessage("alt Click here to add Integrated Search Folder"), gtMess.GetMessage("lbl Search Folder"), "")%>
					            <%=StyleHelper.ActionBarDivider%>
								<td><%=objStyle.GetHelpButton(action,"")%></td>
				            </tr>
			            </table>
                    </div>
            </div>
            <div class="ektronPageContainer">
	                <table class="ektronGrid">
	                    <tr class="title-header">
		                    <td nowrap="nowrap" title="Integrated Search Folder"><%=gtMess.GetMessage("lbl Integrated search folder")%></td>
		                    <td nowrap="nowrap" title="ID"><%=gtMess.GetMessage("generic ID")%></td>
		                    <td nowrap="nowrap" title="Exclude Extensions"><%=gtMess.GetMessage("lbl exclude extension")%></td>
		                    <td nowrap="nowrap" title="Include Extensions"><%=gtMess.GetMessage("lbl include extensions")%></td>
		                    <td nowrap="nowrap" title="Exclude Directories"><%=gtMess.GetMessage("lbl exclude directories")%></td>
		                    <td nowrap="nowrap" title="Domain/User Name"><%=gtMess.GetMessage("lbl domain username")%></td>
	                    </tr>
	                   <asp:Literal ID="ltrFolderList" runat="server" ></asp:Literal>
	                </table>
		</div>
        </asp:Panel>
        
        <asp:Panel ID="pnlAddEditViewIntegratedFolder" Visible="false"  runat="server">
             <div class="ektronWindow" id="pleaseWait">
                <h3><asp:Literal ID="LoadingImg" runat="server" /></h3>
            </div>
            <form action="integratedsearch.aspx?action=<%=strAction%>" name="integratedform" method="post">
		        <input type="hidden" name="integrated_id" value="<%=id%>" />
		         <div class="ektronPageHeader">
		                <div class="ektronTitlebar">
		                        <%=objStyle.GetTitleBar(strTitleMsg)%>
		                </div>
			            <div class="ektronToolbar">
				            <table>
					            <tr>
						            <asp:Literal ID="ltrToolBar" runat="server" ></asp:Literal>
					            </tr>
				            </table>
			            </div>
			      </div>
			      <div class="ektronPageContainer ektronPageInfo" >
			          <div id="ektronPageHolder" runat="server"  >
			          </div>
			          <%if (!bView)
                      { %>
                          <tr>
				                <td class="label"><label title="Password" for="Password"><%=gtMess.GetMessage("lbl password")%></label></td>
				                <td class="value">
				                    <input type="password" title="Enter Password here" id="Password" name="Password" maxlength="255" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(cIntegratedFolder.Password)%>"/>
				                    <div class="ektronCaption" title="<%= gtMess.GetMessage("lbl password") %>"><%=gtMess.GetMessage("lbl password")%></div>
				                </td>
			              </tr>
                          <tr>
				                <td class="label"><label title="Confirm Password" for="ConfirmPassword"><%=gtMess.GetMessage("lbl confirm password")%></label></td>
				                <td class="value">
				                    <input type="password" title="Enter Password here to confirm" id="ConfirmPassword" name="ConfirmPassword" maxlength="255" value="<%=Ektron.Cms.Common.EkFunctions.HtmlEncode(cIntegratedFolder.Password)%>"/>
				                    <div class="ektronCaption"><%=gtMess.GetMessage("lbl confirm password")%></div>				                    
				                </td>
			              </tr>
			          
			          <%} %>
			          
		          </div>
		          </form>
        </asp:Panel>
	</body>
</html>

