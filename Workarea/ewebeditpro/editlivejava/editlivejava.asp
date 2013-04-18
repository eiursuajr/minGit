<%

'**********************************************************************
'	Copyright (c) 2002 Ephox Corporation. All rights reserved.
'**********************************************************************


%>
<SCRIPT LANGUAGE="VBScript" RUNAT="Server">

Class EditLiveForJavaGlobal

	Private m_DownloadDirectory

	Private m_LocalDeployment

	Private m_JREDownloadURL

	Private m_Cookie

	Private m_NotAutoSubmit

	Private m_DebugLevel

	Private m_DisableSystemRequirementsError

	Private m_MinimumJREVersion

	Private m_LoadingMessage

	Public Property Let DownloadingMessage(val)
		m_LoadingMessage = val
	End Property

	Public Property Get DownloadingMessage
		DownloadingMessage = m_LoadingMessage
	End Property

	Public Property Let DownloadDirectory(val)
		m_DownloadDirectory = val
	End Property

	Public Property Let MinimumJREVersion(val)
		m_MinimumJREVersion = val
	End Property

	Public Property Get MinimumJREVersion
		MinimumJREVersion = m_MinimumJREVersion
	End Property

	Public Property Get DownloadDirectory
		DownloadDirectory = m_DownloadDirectory
	End Property

	Public Property Let Cookie(val)
		m_Cookie = val
	End Property

	Public Property Get Cookie
		Cookie = m_Cookie
	End Property

	Public Property Let LocalDeployment(val)
		m_LocalDeployment = val
	End Property

	Public Property Get LocalDeployment
		LocalDeployment = m_LocalDeployment
	End Property

	Public Property Let JREDownloadURL(val)
		m_JREDownloadURL = val
	End Property

	Public Property Get JREDownloadURL
		JREDownloadURL = m_JREDownloadURL
	End Property

	Public Property Let AutoSubmit(val)
		m_NotAutoSubmit =  not val
	End Property

	Public Property Get AutoSubmit
		AutoSubmit = not m_NotAutoSubmit
	End Property

	Public Property Let DebugLevel(val)
		m_DebugLevel = val
	End Property

	Public Property Get DebugLevel
		DebugLevel = m_DebugLevel
	End Property

	Public Property Let ShowSystemRequirementsError(val)
		m_DisableSystemRequirementsError = not val
	End Property

	Public Property Get ShowSystemRequirementsError
		ShowSystemRequirementsError = not m_DisableystemRequirementsError
	End Property

	Public Sub Init()
		Response.Write("<scr" & "ipt src=""" & m_DownloadDirectory & "/editlivejava.js""></scr" & "ipt>" & vbNewLine)
		Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalDownloadDirectory = """ & m_DownloadDirectory & """;</scr" & "ipt>" & vbNewLine)
		Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalDebugLevel = """ & m_DebugLevel & """;</scr" & "ipt>" & vbNewLine)
		Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalLoadingMessage = """ & m_LoadingMessage & """;</scr" &"ipt>" & vbNewLine)

		if strComp(m_MinimumJREVersion,"")<>0 then
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalMinimumJREVersion = """ & m_MinimumJREVersion & """;</scr" & "ipt>" & vbNewLine)
		else
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalMinimumJREVersion = """";</scr" & "ipt>" & vbNewLine)
		end if
		if strComp(m_Cookie,"")<>0 then
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalCookie = " & m_Cookie & ";</scr" & "ipt>" & vbNewLine)
		else
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalCookie = """";</scr" & "ipt>" & vbNewLine)
		end if
		if m_LocalDeployment = false or m_LocalDeployment = "false" then
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalLocalDeployment = false;</scr" & "ipt>" & vbNewLine)
		else
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalLocalDeployment = true;</scr" & "ipt>" & vbNewLine)
		end if
		Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalJREDownloadURL = """ & m_JREDownloadURL & """;</scr" & "ipt>" & vbNewLine)
		if m_NotAutoSubmit = false or m_NotAutoSubmit = "false" then
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalAutoSubmit = true;</scr" & "ipt>" & vbNewLine)
		else
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalAutoSubmit = false;</scr" & "ipt>" & vbNewLine)
		end if

		if m_DisableSystemRequirementsError = false or m_DisableSystemRequirementsError = "false" then
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalShowSystemRequirementsError = true;</scr" & "ipt>" & vbNewLine)
		else
			Response.Write("<scr" & "ipt language=""JavaScript"">EditLiveForJavaGlobalShowSystemRequirementsError = false;</scr" & "ipt>" & vbNewLine)
		end if
	End Sub

End Class

Class EditLiveForJava

	Private m_Height, m_Width, m_Name, m_Styles, m_RedirectPage, m_ConfigurationFile, m_Document
	Private m_Body, m_ConfigurationText, m_ReturnBodyOnly, m_UseWebDAV, m_Locale, m_UseMathML
	Private m_Preload, m_Head, m_BaseURL, m_OutputCharset, m_CommentTemplate, m_ShowButtonText
	Private m_HideButtonText, m_HideButtonIconURL, m_ShowButtonIconURL, m_HttpLayerManager, m_AddJar, m_LoadClass

	Private Sub Class_Initialize()
		m_Height = 400
		m_Width = 700
	End Sub

	Public Property Let Height(val)
		m_Height = val
	End Property

	Public Property Get Height
		Height = m_Height
	End Property

	Public Property Let Name(val)
		m_Name = val
	End Property

	Public Property Get Name
		Name = m_Name
	End Property

	Public Property Get Styles
		Styles = m_Styles
	End Property

	Public Property Let Styles(val)
		m_Styles = val
	End Property

	Public Property Let RedirectPage(strURL)
		m_RedirectPage = strURL
	End Property

	Public Property Get RedirectPage
		RedirectPage = m_RedirectPage
	End Property

	Public Property Let OutputCharset(val)
		m_OutputCharset = val
	End Property

	Public Property Get OutputCharset
		Value = m_OutputCharset
	End Property

	Public Property Let CommentTemplate(val)
		m_CommentTemplate = val
	End Property

	Public Property Get CommentTemplate
		Value = m_CommentTemplate
	End Property

	Public Property Let Document(val)
		m_Document = val
	End Property

	Public Property Get Document
		Value = m_Document
	End Property

	Public Property Let Body(val)
		m_Body = val
	End Property

	Public Property Get Body
		Value = m_Body
	End Property

	Public Property Let Width(val)
		m_Width = val
	End Property

	Public Property Get Width
		Width = m_Width
	End Property

	Public Property Let ConfigurationFile(val)
		m_ConfigurationFile = val
	End Property

	Public Property Get ConfigurationFile
		ConfigurationFile = m_ConfigurationFile
	End Property

	Public Property Let HttpLayerManager(val)
		m_HttpLayerManager = val
	End Property

	Public Property Get HttpLayerManager
		HttpLayerManager = m_HttpLayerManager
	End Property

	Public Property Let ConfigurationText(val)
		m_ConfigurationText = val
	End Property

	Public Property Get ConfigurationText
		ConfigurationText = m_ConfigurationText
	End Property

	Public Property Let ReturnBodyOnly(val)
		m_ReturnBodyOnly = val
	End Property

	Public Property Get ReturnBodyOnly
		ReturnBodyOnly = m_ReturnBodyOnly
	End Property

	Public Property Get UseWebDAV
		UseWebDAV = m_UseWebDAV
	End Property

	Public Property Let UseWebDAV(val)
		m_UseWebDAV = val
	End Property

	Public Property Get UseMathML
		UseMathML = m_UseMathML
	End Property

	Public Property Let UseMathML(val)
		m_UseMathML = val
	End Property

	Public Property Get Head
		Head = m_Head
	End Property

	Public Property Let Head(val)
		m_Head = val
	End Property

	Public Property Get Locale
		Locale = m_Locale
	End Property

	Public Property Let Locale(val)
		m_Locale = val
	End Property

	Public Property Get Preload
		Preload = m_Preload
	End Property

	Public Property Let Preload(val)
		m_Preload = val
	End Property

	Public Property Get BaseURL
		BaseURL = m_BaseURL
	End Property

	Public Property Let BaseURL(val)
		m_BaseURL = val
	End Property

	Public Property Get ShowButtonText
		ShowButtonText = m_ShowButtonText
	End Property

	Public Property Let ShowButtonText(val)
		m_ShowButtonText = val
	End Property

	Public Property Get HideButtonText
		HideButtonText = m_HideButtonText
	End Property

	Public Property Let HideButtonText(val)
		m_HideButtonText = val
	End Property

	Public Property Get ShowButtonIconURL
		ShowButtonIconURL = m_ShowButtonIconURL
	End Property

	Public Property Let HideButtonIconURL(val)
		m_HideButtonIconURL = val
	End Property

    Public Property Let AddJar(val)
		m_AddJar = val
	End Property

	Public Property Let LoadClass(val)
	    m_LoadClass = val
	End Property

	Public Property Get AddJar
		AddJar = m_AddJar
	End Property

	Public Function CommonShow()

		With Response
			.Write "<scr" & "ipt language=""JavaScript"">" & vbNewLine

			.Write "var " & m_Name & "_js;" & vbNewLine

			.Write m_Name & "_js = new EditLiveJava(""" & m_Name & """, """ & m_Width & """, """ & m_Height & """);" & vbNewLine

			If Not m_ReturnBodyOnly = "" Then
			.Write m_Name &"_js.setReturnBodyOnly(""" & m_ReturnBodyOnly & """);" & vbNewLine
			End If

			If Not m_ConfigurationText = "" Then
			.Write m_Name &"_js.setXML(""" & Server.URLEncode(m_ConfigurationText) & """);" & vbNewLine
			End If

			If Not m_ConfigurationFile = "" Then
				.Write m_Name &"_js.setXMLURL(""" & m_ConfigurationFile & """);" & vbNewLine
			End If

			If Not m_HttpLayerManager = "" Then
				.Write m_Name &"_js.setHttpLayerManager(""" & m_HttpLayerManager & """);" & vbNewLine
			End If

			 If Not m_Document = "" Then
				.Write m_Name &"_js.setDocument(""" & Server.URLEncode(m_Document) & """);" & vbNewLine
			 End If

			 If Not m_Styles = "" Then
				.Write m_Name &"_js.setStyles(""" & Server.URLEncode(m_Styles) & """);" & vbNewLine
			 End If

			 If Not m_Body = "" Then
				.Write m_Name &"_js.setBody(""" & Server.URLEncode(m_Body) & """);" & vbNewLine
			 End If

			 If Not m_Head = "" Then
				.Write m_Name &"_js.setHead(""" & Server.URLEncode(m_Head) & """);" & vbNewLine
			 End If

			 If Not m_OutputCharset = "" Then
				.Write m_Name & "_js.setOutputCharset(""" & m_OutputCharset & """);" & vbNewLine
			 End If

			 If Not m_CommentTemplate = "" Then
				.Write m_Name & "_js.setCommentTemplate(""" & m_CommentTemplate & """);" & vbNewLine
			 End If


			 If Not m_Locale = "" Then
				.Write m_Name &"_js.setLocale(""" & m_Locale & """);" & vbNewLine
			 End If

			If m_UseWebDAV = true Then
				.Write m_Name & "_js.setUseWebDAV(true);" & vbNewLine
			End If

			If m_UseMathML = true Then
				.Write m_Name & "_js.setUseMathML(true);" & vbNewLine
			End If

			If Not m_Preload = "" Then
				.Write m_Name & "_js.setPreload(""" & m_Preload & """);" &vbNewLine
			End If

			If Not m_AddJar = "" And Not m_LoadClass = "" Then
				.Write m_Name & "_js.addJar(""" & m_AddJar & """, """ & m_LoadClass & """);" &vbNewLine
			End If


			.Write m_Name &"_js.setDownloadDirectory(EditLiveForJavaGlobalDownloadDirectory);" & vbNewLine

			.Write m_Name &"_js.setLocalDeployment(EditLiveForJavaGlobalLocalDeployment);" & vbNewLine

			.Write "if (EditLiveForJavaGlobalJREDownloadURL != """") {" & vbNewLine
			.Write m_Name & "_js.setJREDownloadURL(EditLiveForJavaGlobalJREDownloadURL);" & vbNewLine
			.Write "}" & vbNewLine

			.Write "if (EditLiveForJavaGlobalMinimumJREVersion != """") {" & vbNewLine
			.Write m_Name &"_js.setMinimumJREVersion(EditLiveForJavaGlobalMinimumJREVersion);" & vbNewLine
			.Write "}" & vbNewLine

			.Write "if (EditLiveForJavaGlobalLoadingMessage != """") {" & vbNewLine
			.Write m_Name &"_js.setDownloadingMessage(EditLiveForJavaGlobalLoadingMessage);" & vbNewLine
			.Write "}" & vbNewLine

			.Write m_Name &"_js.setCookie(escape(EditLiveForJavaGlobalCookie));" & vbNewLine

			.Write m_Name &"_js.setAutoSubmit(EditLiveForJavaGlobalAutoSubmit);" & vbNewLine

			.Write m_Name &"_js.setDebugLevel(EditLiveForJavaGlobalDebugLevel);" & vbNewLine

			If Not m_BaseURL = "" Then
				.Write m_Name &"_js.setBaseURL(""" & m_BaseURL & """);" & vbNewLine
			End If

			.Write m_Name &"_js.setShowSystemRequirementsError(EditLiveForJavaGlobalShowSystemRequirementsError);" & vbNewLine
		End With
	End Function

	Public Function ShowAsButton(value)
		Call CommonShow
		With Response
			If Not m_ShowButtonText = "" Then
				.Write m_Name &"_js.setShowButtonText(""" & Server.URLEncode(m_ShowButtonText) & """);" & vbNewLine
			End If
			If Not m_ShowButtonIconURL = "" Then
				.Write m_Name &"_js.setShowButtonIconURL(""" & m_ShowButtonIconURL & """);" & vbNewLine
			End If
			If Not m_HideButtonIconURL = "" Then
				.Write m_Name &"_js.setHideButtonIconURL(""" & m_HideButtonIconURL & """);" & vbNewLine
			End If
			If Not m_HideButtonText = "" Then
				.Write m_Name &"_js.setHideButtonText(""" & Server.URLEncode(m_HideButtonText) & """);" & vbNewLine
			End If
			.Write m_Name &"_js.showAsButton(""" & value & """);" & vbNewLine
			.Write "</scr" & "ipt>" & vbNewLine
		End With
	End Function

	Public Function Show()
		Call CommonShow
		With Response

			.Write m_Name &"_js.show();" & vbNewLine

			.Write "</scr" & "ipt>" & vbNewLine
		End With

	End Function

End Class


</SCRIPT>
