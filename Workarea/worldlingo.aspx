<%@ Page Language="C#" AutoEventWireup="true" validateRequest="false" Inherits="worldlingo" CodeFile="worldlingo.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
        <title id="pageTitle" runat="server"></title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <script type="text/javascript" src="ewebeditpro/eweputil.js"></script>
        <script type="text/javascript" src="java/toolbar_roll.js"></script>
        <script type="text/javascript"  src="ContentDesigner/RadWindow.js" ></script>
        <asp:Literal ID="stylesheetjs" runat="server" ></asp:Literal> 
</head>
<body>
        <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        InitializeRadWindow();
        var editorcontent;
        var isContentDesigner = false;
        function loadcontent() 
        {
	        editorcontent = "";
            try
            {
                var args = GetDialogArguments();
                if(args)
                {
                    if(typeof(args.content) != "undefined")
                    {
                        editorcontent = args.content;
                        isContentDesigner = true;
                    }
                }
            }
            catch(e)
            {
                isContentDesigner = false;
            }
            if (null == document.getElementById("wl_data")) return;
            if (isContentDesigner)
	        {
		        if (editorcontent.length > 0) 
		        {
			        document.getElementById("wl_data").value = editorcontent;
			        document.getElementById("origContent").innerHTML = editorcontent;
		        }
		        else
		        {
			        alert("There is no content to translate."); 
		        }
	        }
	        else if (top.opener && top.opener.eWebEditPro)
	        {
		        var objInstance = top.opener.eWebEditPro.instances["<%= htmleditor%>"];
		        if (objInstance)
		        {
			        var objElem = objInstance.linkedElement();
			        if (objElem)
			        {
				        editorcontent = objElem.value;
				        if (editorcontent.length > 0) 
				        {
					        document.mylanguage.wl_data.innerHTML = editorcontent;
					        document.writeln(editorcontent);
				        }
				        else
				        {
					        alert("There is no content to translate."); 
				        }
			        }
			        else
			        {
				        alert("Unable to find element with content.");
			        }
		        }
		        else
		        {
			        document.writeln("Could not find editor '<%= htmleditor%>'.");
		        }
	        }
	        else
	        {
        		
		         document.writeln("This page must be opened by a web page that contains the editor.");
        	   
	        }
        }
        function pasteContent() 
        {
	        if(isContentDesigner)
	        {
	            var retValue = document.getElementById("returnContent").value;
	            CloseDlg(retValue);
	        }
	        else if (top.opener && top.opener.eWebEditPro)
	        {
		        var objInstance = top.opener.eWebEditPro.instances["<%= htmleditor %>"];
		        if (objInstance)
		        {
			        var translatedContent = document.getElementById("returnContent").value;
			        objInstance.load(translatedContent);
			        top.close();
		        }
		        else
		        {
			        alert("Could not find editor '<%= htmleditor%>'.");
		        }
	        }
	        else
	        {
		        alert("This page must be opened by a web page that contains the editor.");
	        }
        }

        function validate() 
        {
	        if (document.getElementById("wl_srclang").value == document.getElementById("wl_trglang").value) 
	        {
		        alert("The Source Language and the Target Language cannot be the same.");
		        return false;
	        }
	        document.getElementById("btnTranslate").value = "Please Wait..."
	        return true;
        }

        Ektron.ready(function(event, eventName)
        {
            loadcontent();
        });
        //--><!]]>
        </script>
    <div class="ektronPageHeader" style="padding-top: 0px;">
        <table width="100%" class="translation-toolbar">
            <tr class="ektronTitlebar">
                <td>
                    <asp:Label ID="TransTitle" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ektronToolbar" id="tblButton" runat="server">
                </td>
            </tr>
        </table>
    </div>
<form method="POST" action="http://www.worldlingo.com/S-1.1/api" name="mylanguage" id="frmWordLingo" runat="server">
<div class="ektronPageContainer ektronPageInfo" id="formpage" runat="server">
<table width="100%" class="ektronGrid">
    <tr> 
      <td class="label"><asp:label ID="lblSrcLang" runat="server" />:</td>
      <td>
        <asp:ListBox id="wl_srclang" runat="server" Rows="1" >
             <asp:ListItem Text="Chinese Simplified" Value="zh_cn" />
             <asp:ListItem Text="Chinese Traditional" Value="zh_tw" />
             <asp:ListItem Text="Dutch" Value="nl" />
             <asp:ListItem Text="English" Value="en" />
             <asp:ListItem Text="French" Value="fr" />
             <asp:ListItem Text="German" Value="de" />
             <asp:ListItem Text="Greek" Value="el" />
             <asp:ListItem Text="Italian" Value="it" />
             <asp:ListItem Text="Japanese" Value="ja" />
             <asp:ListItem Text="Korean" Value="ko" />
             <asp:ListItem Text="Portuguese (Brazilian)" Value="pt" />
             <asp:ListItem Text="Russian" Value="ru" />
             <asp:ListItem Text="Spanish" Value="es" />
         </asp:ListBox>
      </td>
    </tr> 
    <tr> 
      <td class="label"><asp:label ID="lblTrgLang" runat="server" />:</td>
      <td>
        <asp:ListBox id="wl_trglang" runat="server" Rows="1" >
             <asp:ListItem Text="Chinese Simplified" Value="zh_cn" />
             <asp:ListItem Text="Chinese Traditional" Value="zh_tw" />
             <asp:ListItem Text="Dutch" Value="nl" />
             <asp:ListItem Text="English" Value="en" />
             <asp:ListItem Text="French" Value="fr" />
             <asp:ListItem Text="German" Value="de" />
             <asp:ListItem Text="Greek" Value="el" />
             <asp:ListItem Text="Italian" Value="it" />
             <asp:ListItem Text="Japanese" Value="ja" />
             <asp:ListItem Text="Korean" Value="ko" />
             <asp:ListItem Text="Portuguese (Brazilian)" Value="pt" />
             <asp:ListItem Text="Russian" Value="ru" />
             <asp:ListItem Text="Spanish" Value="es" />
         </asp:ListBox>
      </td>
    </tr>    
    <tr> 
      <td class="label"><asp:label id="lblGlossary" runat="server" />:</td>
      <td>
        <select name="wl_gloss" id="wl_gloss" runat="server">
          <option value="gl1" title="General">General</option>
          <option value="gl2" title="Automotive">Automotive</option>
          <option value="gl3" title="Aviation/Space">Aviation/Space</option>
          <option value="gl4" title="Chemistry">Chemistry</option>
          <option value="gl5" title="Colloquial">Colloquial</option>
          <option value="gl6" title="Computers/IT">Computers/IT</option>
          <option value="gl7" title="Earth Sciences">Earth Sciences</option>
          <option value="gl8" title="Economics/Business">Economics/Business</option>
          <option value="gl9" title="Electronics">Electronics</option>
          <option value="gl10" title="Food Science">Food Science</option>
          <option value="gl11" title="Legal">Legal</option>
          <option value="gl12" title="Life Sciences">Life Sciences</option>
          <option value="gl13" title="Mathematics">Mathematics</option>
          <option value="gl14" title="Mechanical Engineering">Mechanical Engineering</option>
          <option value="gl15" title="Medicine">Medicine</option>
          <option value="gl16" title="Metallurgy">Metallurgy</option>
          <option value="gl17" title="Military Science">Military Science</option>
          <option value="gl18" title="Naval/Maritime">Naval/Maritime</option>
          <option value="gl19" title="Photography/Optics">Photography/Optics</option>
          <option value="gl20" title="Physics/Atomic Energy">Physics/Atomic Energy</option>
          <option value="gl21" title="Political Science">Political Science</option>
        </select>
      </td>
    </tr>
    <tr>
		<td></td>
		<td><asp:Button ID="btnTranslate" runat="server" OnClientClick="validate()" onclick="btnTranslate_Click"  /></td>
	</tr>
</table>
<hr />
    <div id="origContent" runat="server">
    </div>
    <textarea title="Enter Origional Content Text here" style="display: none;" id="wl_data" runat="server"></textarea>
</div>
    <div id="resultPage" class="ektronPageContainer ektronPageInfo" runat="server">
        <p align="center">
            <input title="Paste Content" name="btn" type="button" value="Paste Content" onclick="pasteContent()" /></p>
        <hr />
        <div id="displaycontent" runat="server">
        </div>
        <textarea title="Enter Return/Displayed Content here" style="display: none;" id="returnContent" runat="server"></textarea>
    </div>

</form>
</body>
</html>
