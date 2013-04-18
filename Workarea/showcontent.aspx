<%@ Page Language="C#" AutoEventWireup="true" Inherits="showcontent" CodeFile="showcontent.aspx.cs" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>showcontent</title>
        <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
        <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
        <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
		        .ektdmsiframe
		        {
		            height: 900px;
		        }
	        /*]]>*/-->
		</style>
    </head>
    <body>
        <form id="Form1" method="post" runat="server">
            <div style="width: 100%; text-align: right; text-decoration: none; border: none 0px black;">
                <asp:Literal ID="DownloadAssetLink" runat="server" />
            </div>
            <table style="height: 100%; vertical-align: top; width: 100%">
                <tr>
                    <td style="vertical-align: top; width: 20%;">
                        <table style="margin-top: 30px; width: 100%;" runat="server" id="ContentDetails">
                            <tr>
                                <td style="font-size: .75em; font-weight: bold; text-align: right; width: 50%;">
                                    Last Modified:
                                </td>
                                <td style="font-size: .75em; padding-left: 5px; width: 50%;">
                                    <asp:Label ID="lblLastModifiedDate" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: .75em; font-weight: bold; text-align: right; width: 50%;">
                                    Modified By:
                                </td>
                                <td style="font-size: .75em; padding-left: 5px; width: 50%;">
                                    <asp:Label ID="lblLastModifiedBy" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="height: 100%; vertical-align: top; width: 100%">
                        <CMS:SocialBar ID="SocialBar" runat="server" DynamicObjectParameter="id" ObjectType="Content" Items="Addto, Email, Print" />
                        <div style="margin-bottom: 20px; margin-top: 15px;">
                            <CMS:ContentList ID="ContentList" IncludeIcons="true" runat="server" DisplayXslt="ecmTeaser" />
                        </div>
                        <div id="contentBody" runat="server">
                            <CMS:ContentBlock Style="vertical-align: top;" runat="server" ID="ContentBlock" WrapTag="div" DynamicParameter="id" />
                        </div>
                        <CMS:MessageBoard ID="MessageBoard" runat="server" DynamicObjectParameter="id" ObjectType="Content" />
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>

