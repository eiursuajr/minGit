<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddImage.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.Tabs.Media.AddImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Media: Add Image</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                #dvHoldMessage {visbility:hidden;font-weight:bold;}
                #dvErrorMessage {visbility:hidden;}
                .loginError {font-weight:bold;color:Maroon;}
                
                div.wrapper {padding:0em .5em;}
                div.wrapper fieldset.image {margin-bottom:1em;}
                div.wrapper fieldset.image span {padding:0 0 0 22px;background-image:url('css/images/image.gif');background-position:0px 0px;background-repeat:no-repeat;}
                div.wrapper fieldset.thumbnails span {padding:0 0 0 22px;background-image:url('css/images/thumbnails.gif');background-position:0px 0px;background-repeat:no-repeat;}
                div.wrapper table {width:100%;border-collapse:collapse;}
                div.wrapper table thead th {text-align:left;margin:0;padding:0em 1em 1em 1em;font-weight:normal;}
                div.wrapper table thead th p {margin:0;padding:0;}
                div.wrapper table tfoot td {text-align:right;padding:0em;}
                div.wrapper table tfoot td p.uploadButton {border-top:1px dotted silver;margin:1em 1em 0em 1em;padding:1em 0 0 0;}
                div.wrapper table tbody th {width:35%;text-align:right;font-weight:normal;padding-right:.5em;}
                div.wrapper table tbody td {width:65%;text-align:left;}
                
                div.wrapper fieldset.thumbnails p {margin:0 0 .5em 1em;padding:0;}
                div.wrapper fieldset.thumbnails ul {margin:0 0 0 1em;}
            /*]]>*/-->
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <asp:MultiView ID="mvAddImage" runat="server">
                <asp:View ID="vwForm" runat="server">
                    <div class="wrapper">
                        <fieldset class="image">
                            <legend><span class="image"><asp:Literal ID="legendImage" runat="server" /></span></legend>
                            <table>
                                <thead>
                                    <tr>
                                        <th colspan="2">
                                            <p><asp:Literal ID="litImageHeader" runat="server" /></p>
                                        </th>
                                    </tr>
                                </thead>
                                <tfoot>
                                    <tr>
                                        <td colspan="2">
                                            <p class="uploadButton"><asp:Button ID="btnUpload" OnClick="btnUpload_Click" runat="server" /></p>
                                        </td>
                                    </tr>
                                </tfoot>
                                <tbody>
                                    <tr>
                                        <th>
                                            <img alt="<% =GetMessage("alt media title") %>" title="<% =GetMessage("alt media title") %>" src="css/images/about.gif" />
                                            <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <img alt="<% =GetMessage("alt media alt")%>" title="<% =GetMessage("alt media alt")%>" src="css/images/about.gif" />
                                            <asp:Label ID="lblAlt" runat="server" AssociatedControlID="txtAlt"></asp:Label>
                                        </th>
                                        <td>
                                            <asp:TextBox ID="txtAlt" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <img alt="<% =GetMessage("alt media file upload") %>" title="<% =GetMessage("alt media file upload") %>" src="css/images/about.gif" />
                                            <asp:Label ID="lblImage" runat="server" AssociatedControlID="fuImage">Browse to the image to be uploaded:</asp:Label>
                                        </th>
                                        <td>
                                            <asp:FileUpload ID="fuImage" runat="server" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </fieldset>
                        <asp:Placeholder ID="phThumbnails" runat="server">
                            <fieldset class="thumbnails">
                                <legend><span><asp:Literal ID="legendThumbnails" runat="server" /></span></legend>
                                <p><% =GetMessage("lbl thumbnail spec")%></p>
                                <ul>
                                    <asp:Repeater ID="rptThumbnails" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <%#DataBinder.Eval(Container.DataItem, "Width")%>
                                                &#160;x&#160;
                                                <%#DataBinder.Eval(Container.DataItem, "Height")%>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </fieldset>
                        </asp:Placeholder>
                        <div id="dvHoldMessage">
	                        <asp:Literal ID="ltrHoldMessage" runat="Server"></asp:Literal>
                        </div>
                        <div id="dvErrorMessage">
	                        <span class="error">
	                            <asp:Literal ID="litError" runat="Server"></asp:Literal>
	                        </span>
                        </div>
                    </div>
                    <script type="text/javascript" language="javascript">
          
			                function justtoggle(eElem, toshow){
            	                if (toshow == true) {
            	                    eElem.style.visibility = 'visible';
            	                } else {
            	                    eElem.style.visibility='hidden'; 
            	                }
            	            }

            	            function checkntoggle(me, you){
                                if (!checkForEmptyTitleAndAlt()) {
                                   return false;
                                }
                                
                                var bProceed = false; 
                                var ofile = document.getElementById('<% =fuImage.UniqueID %>'); 
                                
                                if ( (ofile.type == 'file') && (ofile.value != '') ) { 
                                    bProceed = true; 
                                } 
                                
                                if (bProceed){
                                    me.style.visibility='visible';
                                    you.style.visibility='hidden';
                                } else {
                                    me.style.visibility='hidden';
                                    you.style.visibility='visible';
                                    alert('File not selected.');
                                }
                                return bProceed;
            	            }

                           function checkForEmptyTitleAndAlt()
                           {
                               var title = document.getElementById('<% =txtTitle.UniqueID%>').value;
                               var alt = document.getElementById('<% =txtAlt.UniqueID %>').value;
                               var filename = document.getElementById('<% =fuImage.UniqueID %>').value;
	                           var ExtensionList = '<%=lib_settings_data.ImageExtensions.Replace(";", "")%>';
	                           var strRet = '';
                                
                                if ((title.indexOf("<") > -1) || (title.indexOf(">") > -1))
                                {
                                    alert('<% =GetMessage("alert commerce media image title") %>');
                                    return false;
                                }
                                    
                                if ((alt.indexOf("<") > -1) || (alt.indexOf(">") > -1))
                                {
                                    alert('<% =GetMessage("alert commerce media image alt") %>');
                                    return false;
                                }
                                	
				               if (ExtensionList.length > 0) {
					               var ExtensionArray = ExtensionList.split(",");
					               var FileExtension = filename.split(".");
					               for (var i = 0; i < ExtensionArray.length; i++) {
						               if (FileExtension[FileExtension.length - 1].toLowerCase() == ExtensionArray[i].toLowerCase()) {
							               strRet='true';
						               }
					               }
					               if( strRet != 'true') {
					                   alert('<% =_ContentApi.EkMsgRef.GetMessage("invalid file extension") %>');
					                   return false;
					               }
				               }
                               if(title == '' || alt == '') {
                                   alert('<% =GetMessage("js alert title alt not empty") %>');
                                   return false;
                               } else {
                                   return true;
                               }
                           }
			            //--><!]]>
                    </script>
                    <script language="javaScript" type="text/javascript">
                        <!--//--><![CDATA[//><!--
                            <asp:Literal ID="ltrErrorJS" runat="server"></asp:Literal>
                            <asp:Literal ID="ltrAddMediaJS" runat="server"></asp:Literal>
                        //--><!]]>
                    </script>
                </asp:View>
                <asp:View ID="vwError" runat="server">
                    <p class="loginError">Please log-in as a CMS user.</p>
                </asp:View>
            </asp:MultiView>
        </form>
    </body>
</html>

