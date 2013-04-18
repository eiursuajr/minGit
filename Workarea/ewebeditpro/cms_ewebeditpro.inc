<%
dim m_siteRef_eweb as New SiteAPI
dim language_data_eweb as Object
dim BrowserCode_eweb as Object

If (m_siteRef_eweb.ContentLanguage=-1) Then
	BrowserCode_eweb = "en"
Else
	language_data_eweb = m_siteRef_eweb.GetLanguageById(m_siteref_eweb.ContentLanguage)
	BrowserCode_eweb = language_data_eweb.BrowserCode
End If
%>
<script language="JavaScript1.2">
var LicenseKeys = "<%= var2 %>";
var eWebEditProPath = "<%= AppeWebPath %>";
var WIFXPath= "<%= AppeWebPath %>";
var WebImageFXPath = "<%= AppeWebPath %>";

<%

 If Not (BrowserCode_eweb = "ar" Or BrowserCode_eweb = "da" Or BrowserCode_eweb = "de" Or BrowserCode_eweb = "en" Or BrowserCode_eweb = "es" Or BrowserCode_eweb = "fr" Or BrowserCode_eweb = "he" Or BrowserCode_eweb = "it" Or BrowserCode_eweb = "ja" Or BrowserCode_eweb = "ko" Or BrowserCode_eweb = "nl" Or BrowserCode_eweb = "pt" Or BrowserCode_eweb = "ru" Or BrowserCode_eweb = "sv" Or BrowserCode_eweb = "zh") Then
            BrowserCode_eweb = "en"
        End If
%>
var eWebEditProMsgsFilename = "ewebeditpromessages" + "<%=BrowserCode_eweb%>" + ".js";

function InformationPassingParameters()
{
    var strLoadPage = "";
	var strParamChar = "?";
    
    if("undefined" != typeof eWebEditProPath)
    {
	    strLoadPage += strParamChar + "instewep=";
    	strLoadPage += eWebEditProPath;
		strParamChar = "&";
    }
	else
	{
		strLoadPage += strParamChar + "instewep=undefined";
		strParamChar = "&";
	}
	
    if("undefined" != typeof LicenseKeys)
    {
    	strLoadPage += strParamChar + "licnewep=";
    	strLoadPage += LicenseKeys;
		strParamChar = "&";
	}    
	else
	{
		strLoadPage += strParamChar + "licnewep=undefined";
		strParamChar = "&";
	}
    if("undefined" != typeof WebImageFXPath)
    {
        if(WebImageFXPath.length > 0)
        {
            strLoadPage += "&instwifx=";
            strLoadPage += WebImageFXPath;
			strParamChar = "&";
        }
    }
    
    if("undefined" != typeof WifxLicenseKeys)
    {
        if(WifxLicenseKeys.length > 0)
        {
            strLoadPage += "&licnwifx=";
            strLoadPage += WifxLicenseKeys;
			strParamChar = "&";
        }
    }
    
    return(strLoadPage);
}
</script>

<script language="JavaScript1.2" src="<%= AppeWebPath %>cms_ewebeditpro.js"></script>
<script language="JavaScript1.2">	
// The install popup was correctly created at the beginning
// (within the eWebEditProinstallPopupUrl variable)
// but it needs to be set into the installPopup.url value
// for it to automatically be used to install the editor.
// Otherwise, it sits in the variable doing nothing.
// eWebEditPro.parameters.installPopup.url = eWebEditProinstallPopupUrl + InformationPassingParameters();
</script>
<script language=vb runat=server>
	Function eWebEditProEditor(FieldName as string, Width as string, Height as string, ContentHtml as string) As String
		response.write("<input type=""hidden"" name=""" & FieldName & """ value=""" & Server.HTMLEncode(ContentHtml) & """>") 
		response.write("<script language=""JavaScript1.2"">" & VBCrLf)
		response.write("<!--" & VBCrLf)
		If TypeName(Width) = "String" Then
			Width = """" & Width & """"
		End If
		If TypeName(Height) = "String" Then
			Height = """" & Height & """"
		End If
		response.write("eWebEditPro.create(""" & FieldName & """, " & Width & ", " & Height & ");" & VBCrLf)
		response.write("eWebEditPro.instances[""" & FieldName & """].onerror = myOnErrorHandler;" & VBCrLf)
		response.write("function myOnErrorHandler()" & VBCrLf)
		response.write("{" & VBCrLf)
		response.write("	if (EWEP_STATUS_INVALID == this.status && save == this.event.source)" & VBCrLf)
		response.write("	{" & VBCrLf)
		response.write("		var strMsg = 'Content is invalid.';" & VBCrLf)
		response.write("		strMsg += '\nError Reason:'  + this.event.message; " & VBCrLf)
		response.write("		alert(strMsg); " & VBCrLf)
		response.write("		return false;  // prevent save" & vbCrlf)
		response.write("	}" & VBCrLf)
		response.write("}" & vbCrlf)
		response.write("//-->" & VBCrLf)
		response.write("</scr")
		response.write("ipt>")
		Return ""
	End Function
	Function eWebEditProPopupButton(ButtonName as string, FieldName as string) As String
		response.write("<script language=""JavaScript1.2"">" & VBCrLf)
		response.write("<!--" & VBCrLf)
		response.write("eWebEditPro.createButton(""" & ButtonName & """, """ & FieldName & """);" & VBCrLf)
		response.write("//-->" & VBCrLf)
		response.write("</scr")
		response.write("ipt>")
		Return ""
	End Function
</script>