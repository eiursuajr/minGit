<script language="vb" runat="server">

	Function CheckedAttr(ByVal bChecked as Boolean) as Object
	CheckedAttr = IIf(bChecked, " checked", "")
	End Function

	Function DisabledAttr(ByVal bDisabled as Boolean) as Object
	DisabledAttr = IIf(bDisabled, " disabled", "")
	End Function

	Function SelectedAttr(ByVal bSelected as Boolean) as Object
	SelectedAttr = IIf(bSelected, " selected", "")
	End Function

	Function SelectedValueAttr(ByVal OptionValue as Object, ByVal SelectedValue as Object) as Object
	SelectedValueAttr = " value=""" & OptionValue & """" & SelectedAttr(OptionValue = SelectedValue)
	End Function

	Function BoolToYesNo(ByVal bValue as Boolean) as String
	If bValue Then
	return(gtMess.GetMessage("generic Yes"))
	Else
	return(gtMess.GetMessage("generic No"))
	End If
	End Function

	Function MetaTagTypeBoxTop(ByVal bView as Object, ByVal MetaTagType as Object, ByVal Caption as Object) as Object
	    Dim sb as New System.Text.StringBuilder
	    If Not bView Then
	        sb.Append("<div id=""idMetaTagType_" & MetaTagType & """>")
		    sb.Append("<fieldset>")
		    sb.Append("<legend>" & Caption & "</legend>")
		    sb.Append("<table width=""100%"" class=""ektronForm"">")
		End If
		return(sb.ToString)
	End Function

	Function MetaTagTypeBoxBottom(ByVal bView as Object) as Object
		Dim sb as New System.Text.StringBuilder
		If Not bView Then
			sb.Append("</table>")
	    	sb.Append("</fieldset>")
            sb.Append("</div>")
	    End If
	    return(sb.ToString)
	End Function


	' TODO: Convert to use EkDTSelector -- this should become obsolete.
	function getDateFromString(ByVal strDate as Object) as Object
	dim MonthShortNames, cMonthInfo as Object
	dim sfErrString, sfDatecurrentUserID, sfDateSite as Object
	sfErrString = ""
	dim monthNames, arBoth, arDate, arTime as Object
	dim newDateTime, strAmPm as Object
	if request.cookies("ecm").HasKeys then
	sfDatecurrentUserID = request.cookies("ecm")("user_id")
	sfDateSite = request.cookies("ecm")("site_id")
	else
	sfDatecurrentUserID = 0
	end if

	' cSfTemp = Scripting.Dictionary
	Dim csfTemp As New Collection

	cSfTemp.Add("","DateString")
	cSfTemp.Add("","FormatString")

	dim ekF as new EkFunctions

	cMonthInfo = EkFunctions.GetDateStrings(cSfTemp)

	If sfErrString = "" And cMonthInfo.Count > 0 Then
	MonthShortNames = cMonthInfo("MonthNameString")
	monthNames = split(MonthShortNames, ",")
	Else
	monthNames = " "
	End If

	if(UBound(monthNames) >= 1) then
	arBoth = split(strDate, "T")
	if(UBound(arBoth) >= 1) then
	arDate = split(arBoth(0), "-")
	arTime = split(arBoth(1), ":")

	if(arTime(0) >= 12) then
	strAmPm = "PM"
	arTime(0) = arTime(0) - 12
	else
	strAmPm = "AM"
	end if

	newDateTime = arDate(2) & "-" & monthNames(arDate(1)) & "-" & arDate(0) & " " & arTime(0) & ":" & arTime(1) & " " & strAmPm
	return(newDateTime)
	else
	dim dt as DateTime
	If (DateTime.TryParse(strDate, dt)) Then
	return (dt.ToString("d"))
	Else
	return("")
	End If
	end if
	end if
	return Nothing
	end function


	Function WriteSearchProperties(ByVal AppConfStr as Object, ByVal currentUserID as Object, ByVal Site as Object, ByVal folderId as Object, ByVal szBeginFontInfo as Object, ByVal szEndFontInfo as Object) as String

	Dim contObj as Object
	contObj = sfSiteRef.EkContentRef
	Dim sb as new System.Text.StringBuilder

	Dim CustomSearchProperties, CustomSearchProp as Object
	CustomSearchProperties = contObj.GetCustomSearchProperties(folderId, Nothing, True)
	If CustomSearchProperties.Count > 0 Then
	sb.append("<table width=""100%"" id=""custom_group"">")
		sb.append("<tbody>")
		sb.Append(WriteSearchColumns)
		sb.Append(WriteSectionSeparator)
		For Each CustomSearchProp In CustomSearchProperties
			sb.append(CustomFields.WritesearchProp(CUSTOM_PROP, CustomSearchProp("DataType"), CustomSearchProp("Caption"), "ecmCustom_" & CustomSearchProp("ID"), CustomSearchProp("Items"), "left", szBeginFontInfo, szEndFontInfo))
		Next
		sb.append("</tbody>")
		sb.append("</table>")

		return(sb.ToString)

	' TODO: What are these for?
	 ' Dim StartAdvanced as Boolean
	 ' Dim enableCmsConstraints as Boolean
	 ' Dim enableAssetConstraints as Boolean
	 ' Dim enableFormReInitializing as Boolean
	' StartAdvanced = False
	' enableCmsConstraints = False
	' enableAssetConstraints = False
	' enableFormReInitializing = True
	End If
	return Nothing
End Function

Function WriteSearchColumns As String
	return("<col width=""40%""><col width=""60%"">")
End Function

Function WriteSectionSeparator As String
	return("<tr><td style=""line-height:1ex;"">&nbsp;</td></tr>")
		End Function

		Function IIf(ByVal expr as Object, ByVal truepart as Object, ByVal falsepart as Object) as Object
		If expr Then
		IIf = truepart
		Else
		IIf = falsepart
		End If
		End Function

		' I don't know of a way to detect whether the argument is an object or not in VBScript.
		' On Error Goto is not supported, TypeOf is not supported, TypeName is too specific.
		Function IIfSet(ByVal expr as Object, ByVal truepart as Object, ByVal falsepart as Object) as Object
		If expr Then
		IIfSet = truepart
		Else
		IIfSet = falsepart
		End If
		End Function

		Function WriteMetadataForView(ByVal cMetadataTypes as Object) As String
		Dim cMetadataType as Object
		Dim cRet as new System.Text.StringBuilder

		for each cMetadataType in cMetadataTypes
		If cMetadataType("MetaTagType") >= MetaTagType_Searchable And cMetadataType("MetaEditable") Then ' ONLY SHOW CUSTOM SEARCH PROPERTIES
		cret.append(WriteMetadataTypeForView(cMetaDataType("MetaNameTitle"), cMetadataType("MetaTypeName"), cMetadataType("MetaText")))
		End If
		Next

		return(cret.ToString)
		End Function

		Function WriteMetadataTypeForView(ByVal DataType as Object, ByVal Caption as Object, ByVal Value as Object) As String
		Dim sb as New System.Text.StringBuilder
		sb.append("<tr>")
		sb.append("<td class=""label"">" & Caption & ":</td>")
		sb.append("<td class=""readOnlyValue"">" & WriteMetadataValue(DataType, Value) & "</td>")
	sb.append("</tr>")
		return(sb.ToString)
		End Function

		Function WriteMetadataValue(ByVal DataType as Object, ByVal Value as Object) as Object
		If DATE_PROP = DataType Then
		return(getDateFromString(Value))
		Else
		return(Value)
		End If
		End Function

		Function WriteMetadataTypeForEdit(ByVal cMetadataType as Object, ByVal lValidCounter as Object, ByVal ty as Object) as Object
		return(WriteMetadataTypeForEditEx(False, cMetadataType, cMetadataType("MetaNameTitle"), lValidCounter, ty))
		End Function

		Function WriteMetadataDefaultForEdit(ByVal cMetadataType as Object, ByVal DataType as Object) as Object
		return(WriteMetadataTypeForEditEx(True, cMetadataType, DataType, DataType, ""))
		End Function

		' See mediauploader.aspx and library.aspx for similar definition of fields
		Function WriteMetadataTypeForEditEx(ByVal bIsDefault as Object, ByVal cMetadataType as Object, ByVal DataType as Object, ByVal lValidCounter as Object, ByVal ty as Object) as Object
		Dim sb as New System.Text.StringBuilder
		Dim nMetaTagType as Object
		Dim strName as Object
		Dim strSelName as Object
		Dim IdAttr as Object
		Dim bIsEditable as Object
		Dim bSelectableOnly as Object
		Dim bAllowMulti as Object
		Dim bMetaRequired as Object
		Dim strLabelAlign as Object
		Dim strDefaultNotes as Object
		Dim strMetaTypeName as Object
		Dim nMetaTypeID as Object
		Dim strMetaDefault as Object
		Dim strMetaSeparator as Object
		Dim strMetaSelectableText as Object
		Dim strMetaValue as Object
		strDefaultNotes = ""
		If bIsDefault Then
		nMetaTagType = MetaTagType_Searchable
		strName = "frm_metadefault_" & nMetaTagType & "_" & DataType
		IdAttr = " id=""idSearchPropStyle_" & DataType & """"
		strLabelAlign = "left"
		bIsEditable = True
		bSelectableOnly = (SELECT1_PROP = DataType Or SELECT_PROP = DataType Or BOOLEAN_PROP = DataType)
		bAllowMulti = (SELECT_PROP = DataType)
		bMetaRequired = False
		strMetaTypeName = gtMess.GetMessage("lbl default")
		If (BOOLEAN_PROP = DataType) Then
		strDefaultNotes = "&nbsp;&nbsp;" & gtMess.GetMessage("lbl (Checked for Yes)") 
		End If
		Else
		nMetaTagType = cMetadataType("MetaTagType")
		strName = "frm_text_" & lValidCounter ' many locations depend on this name
		IdAttr = ""
		strLabelAlign = "right"
		bIsEditable = cMetadataType("MetaEditable")
		bSelectableOnly = cMetadataType("SelectableOnly")
		bAllowMulti = cMetadataType("AllowMulti")
		bMetaRequired = cMetadataType("MetaRequired")
		strMetaTypeName = cMetadataType("MetaTypeName")
	End If
	If cMetadataType Is Nothing Then
		nMetaTypeID = 0
		strMetaDefault = ""
		strMetaSeparator = ";"
		strMetaSelectableText = ""
		strMetaValue = ""
	Else
		nMetaTypeID = CLng(cMetadataType("MetaTypeID"))
		strMetaDefault = cMetadataType("MetaDefault")
		strMetaSeparator = cMetadataType("MetaSeparator")
		strMetaSelectableText = cMetadataType("MetaSelectableText")
		if (ty = "update") Then
			strMetaValue = cMetadataType("MetaText")
		else
			strMetaValue = cMetadataType("MetaDefault")
		end if
	End If
	strSelName = "frm_text_sel" & lValidCounter ' many locations depend on this name
	sb.Append("<tr " & IdAttr & ">" & vbcrlf)
		' Label
		If bSelectableOnly then
			sb.Append("<td class=""label"" align=""" & strLabelAlign & """ nowrap=""true"">" & vbcrlf)
				sb.Append("<label for=""" & strSelName & """>" & vbcrlf)
				if bMetaRequired then
					sb.Append("<span style=""color:red"">" & vbcrlf)
				end if
				sb.Append(strMetaTypeName & ":" & vbcrlf)
				if bMetaRequired then
					sb.Append("*</span>" & vbcrlf)
				end if
				sb.Append("</label>" & vbcrlf)
			sb.Append("</td>" & vbcrlf)
		else
			sb.Append("<td class=""label"" align=""" & strLabelAlign & """ nowrap=""true"">" & vbcrlf)
				sb.Append("<label for=""" & strName & """>" & vbcrlf)
				if bMetaRequired then
					sb.Append("<span style=""color:red"">" & vbcrlf)
				end if
				sb.Append(strMetaTypeName & ":" & vbcrlf)
				if bMetaRequired then
					sb.Append("*</span>" & vbcrlf)
				end if
				sb.Append("</label>" & vbcrlf)
			sb.Append("</td>" & vbcrlf)
		end if

		' Data Value
		if (bIsEditable) then
			sb.Append("<td class=""readOnlyValue"">" & vbcrlf)
			sb.Append("<input type=""hidden"" name=""frm_meta_type_id_" & (lValidCounter) & """ id=""frm_meta_type_id_" & (lValidCounter) & """ value=""" & nMetaTypeID & """>" & vbcrlf)
			sb.Append("<input type=""hidden"" name=""frm_meta_required_" & (lValidCounter) & """ id=""frm_meta_required_" & (lValidCounter) & """ value=""" & clng(bMetaRequired) & """>" & vbcrlf)
			sb.Append("<input type=""hidden"" name=""" & strName & "default"" id=""" & strName & "default"" value=""" & Server.HTMLEncode(strMetaDefault) & """>" & vbcrlf)
			sb.Append("<input type=""hidden"" name=""MetaSeparator_" & (lValidCounter) & """ id=""MetaSeparator_" & (lValidCounter) & """ value=""" & Server.HTMLEncode(strMetaSeparator) & """>" & vbcrlf)
      ' Note, the following value was hard coded to one, changed to zero to fix defect #14817, BCB.
      sb.Append("<input type=""hidden"" name=""MetaSelect_" & (lValidCounter) & """ id=""MetaSelect_" & (lValidCounter) & """ value=""0"">" & vbcrlf)

		  dim arrMetaOptions, arrMetaText, i_option, toption, nOptionCount as Object
		  arrMetaOptions = Split(Server.HtmlDecode(strMetaSelectableText), strMetaSeparator)
		  nOptionCount = UBound(arrMetaOptions) - LBound(arrMetaOptions) + 1
		  arrMetaText = Split(Server.HtmlDecode(strMetaValue), strMetaSeparator)

		  If BOOLEAN_PROP = DataType Then
		  Dim strChecked, strValue as Object
		  strChecked = ""
		  If nOptionCount < 2 Then
						' Avoid an error
						arrMetaOptions = Split(BoolToYesNo(False) & strMetaSeparator & BoolToYesNo(True), strMetaSeparator)
						nOptionCount = 2
					End If
					If arrMetaOptions(0) = strMetaValue Or "" = strMetaValue Then
						strValue = arrMetaOptions(0)
						strChecked = ""
					Else
						strValue = arrMetaOptions(1)
						strChecked = " checked"
					End If

					sb.append("<input type=""hidden"" name=""" & strName & """ value=""" & strValue & """>" & vbcrlf)
					sb.append("<input type=""checkbox"" name=""" & strSelName & """ id=""" & strSelName & """ " & strChecked & " onclick=""JavaScript:booleanSelected(document.forms[0]." & strSelName & ",document.forms[0]." & strName & ",'" & arrMetaOptions(0) & "','" & arrMetaOptions(1) & "');"">" & strDefaultNotes & vbcrlf)
					sb.append("<scr" & "ipt  type=""text/javascript"">" & vbcrlf)
					sb.append("<!--" & vbcrlf)
					sb.append("if(top.mediauploader)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
					sb.append("var objForm = document.LibraryItem;" & vbcrlf)
					sb.append("if(objRemoteForm && objForm)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("if(objRemoteForm." & strSelName & ".value == ""false"")" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("objForm." & strSelName & ".checked = false;	" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("else" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("objForm." & strSelName & ".checked = true;" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("objForm." & strName & ".value = objRemoteForm." & strName & ".value;" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("// -->" & vbcrlf)
					sb.append("</scr" & "ipt>" & vbcrlf)
				ElseIf bSelectableOnly then
					if bAllowMulti then
					sb.Append("<SCR" & "IPT type='text/javascript'>" & vbcrlf)
					sb.Append("var opt" & (lValidCounter) & " = new OptionTransfer(""list1_" & (lValidCounter) & """,""list2_" & (lValidCounter) & """,""myinput" & (lValidCounter) & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".setAutoSort(true);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".setDelimiter(""" & strMetaSeparator & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".saveRemovedLeftOptions(""removedLeft" & (lValidCounter) & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".saveRemovedRightOptions(""removedRight" & (lValidCounter) & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".saveAddedLeftOptions(""addedLeft" & (lValidCounter) & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".saveAddedRightOptions(""addedRight" & (lValidCounter) & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".saveNewLeftOptions(""newLeft" & (lValidCounter) & """);" & vbcrlf)
					sb.Append("opt" & (lValidCounter) & ".saveNewRightOptions(""" & strName & """);" & vbcrlf)
					sb.Append("</SC" & "RIPT>" & vbcrlf)

					sb.Append("<input type=""hidden"" name=""delimiter"" value=""" & Server.HTMLEncode(strMetaSeparator) & """/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=""autosort"" value=""Y""/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=removedLeft" & (lValidCounter) & " value=""""/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=removedRight" & (lValidCounter) & " value=""""/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=addedLeft" & (lValidCounter) & " value=""""/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=addedRight" & (lValidCounter) & " value=""""/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=newLeft" & (lValidCounter) & " value=""""/>" & vbcrlf)
					sb.Append("<input type=""hidden"" name=" & strName & " value=""""/>" & vbcrlf)

					sb.Append("<SCR" & "IPT type='text/javascript'>" & vbcrlf)
					sb.Append("if(top.mediauploader)" & vbcrlf)
					sb.Append("{" & vbcrlf)
					sb.Append(" var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
					sb.Append(" var objForm = document.LibraryItem;" & vbcrlf)
					sb.Append(" if(objRemoteForm && objForm)" & vbcrlf)
					sb.Append(" {" & vbcrlf)
					sb.Append("   objForm.removedLeft" & (lValidCounter) & ".value = objRemoteForm.removedLeft" & (lValidCounter) & ".value;" & vbcrlf)
					sb.Append("   objForm.removedRight" & (lValidCounter) & ".value = objRemoteForm.removedRight" & (lValidCounter) & ".value;" & vbcrlf)
					sb.Append("   objForm.addedLeft" & (lValidCounter) & ".value = objRemoteForm.addedLeft" & (lValidCounter) & ".value;" & vbcrlf)
					sb.Append("   objForm.addedRight" & (lValidCounter) & ".value = objRemoteForm.addedRight" & (lValidCounter) & ".value;" & vbcrlf)
					sb.Append("   objForm.newLeft" & (lValidCounter) & ".value = objRemoteForm.newLeft" & (lValidCounter) & ".value;" & vbcrlf)
					sb.Append("   objForm." & strName & ".value = objRemoteForm." & strName & ".value;" & vbcrlf)
					sb.Append(" }" & vbcrlf)
					sb.Append("}" & vbcrlf)
					sb.Append("</SCR" & "IPT>" & vbcrlf)

          sb.append("<div id=""dvMetadata"">")
					sb.Append("<table>" & vbcrlf)
					sb.Append("<tbody>" & vbcrlf)
					sb.Append("<tr><td>" & gtMess.GetMessage("lbl Not Included:") & "</td>")
                    sb.Append("<td></td>")
                    sb.Append("<td>" & gtMess.GetMessage("lbl Included:") & "</td>")
                    sb.Append("</tr>" & vbcrlf)
					sb.Append("<TR>" & vbcrlf)
					sb.Append("<td width=""47%"">")
                    sb.Append("<select ondblclick=opt" & (lValidCounter) & ".transferRight() multiple size=10 name=list1_" & (lValidCounter) & " id=""" & strSelName & """ style=""width:200px""> " & vbcrlf)

					For i_option = 0  to UBound(arrMetaOptions)
						toption = Trim(arrMetaOptions(i_option))
						if toption <> "" Then
							if Not ( Testselection(arrMetaText ,toption) ) Then
								sb.Append("<option value=""" & toption & """>" & toption & "</option>" & vbcrlf)
							end if
						end if
					Next
					sb.Append("</select>" & vbcrlf)

					sb.append("<scr" & "ipt type='text/javascript'>" & vbcrlf)
					sb.append("if(top.mediauploader)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append(" var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
					sb.append(" var objForm = document.LibraryItem;" & vbcrlf)
					sb.append(" if(objRemoteForm && objForm)" & vbcrlf)
					sb.append(" {" & vbcrlf)
					sb.append("   if(objForm.newLeft" & (lValidCounter) & ".value != """" && objForm." & strName & ".value != """")" & vbcrlf)
					sb.append("   {" & vbcrlf)
					sb.append("     var wordArray = new Array;" & vbcrlf)
					sb.append("     wordArray = objForm.newLeft" & (lValidCounter) & ".value.split(objForm.delimiter.value);" & vbcrlf)
					sb.append("     var path = objForm.list1_" & (lValidCounter) & ".options;" & vbcrlf)
					sb.append("     for(var i=0; i < wordArray.length; i++)" & vbcrlf)
					sb.append("     {" & vbcrlf)
					sb.append("       path[i].value = wordArray[i];" & vbcrlf)
					sb.append("       path[i].text = wordArray[i];" & vbcrlf)
					sb.append("     }" & vbcrlf)
					sb.append("     for(var j=path.length-1; j >= i; j--)" & vbcrlf)
					sb.append("     {" & vbcrlf)
					sb.append("       path[j]=null;" & vbcrlf)
					sb.append("     }" & vbcrlf)
					sb.append("   }" & vbcrlf)
					sb.append(" }" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("</scr" & "ipt>" & vbcrlf)

					If nMetaTagType < MetaTagType_Searchable Then
						sb.append("<br />" & vbcrlf)
						sb.append("<input name=""myinput" & (lValidCounter) & """ type=""text"" size=""15"" ID=""Text3"">" & vbcrlf)
					End If
					sb.append("</td>" & vbcrlf)
					sb.append("<td class=""moveMeta center"">" & vbcrlf)
					sb.append("<a style=""background-color: #D0E5F5;"" class=""button greenHover buttonAdd"" onclick=opt" & (lValidCounter) & ".transferRight() type=""button"" title=""" & gtMess.GetMessage("generic add title") & """ name=""right""></a>" & vbcrlf)
					sb.append("<a style=""background-color: #D0E5F5;"" class=""button greenHover buttonAddAll"" onclick=opt" & (lValidCounter) & ".transferAllRight() type=""button"" title=""" & gtMess.GetMessage("generic add all title") & """ name=""right""></a>" & vbcrlf)
					sb.append("<a style=""background-color: #D0E5F5;"" class=""button redHover buttonLeftRemove"" onclick=opt" & (lValidCounter) & ".transferLeft() type=""button"" title=""" & gtMess.GetMessage("generic remove") & """ name=""left"" ></a>" & vbcrlf)
					sb.append("<a style=""background-color: #D0E5F5;"" class=""button redHover buttonRemoveAll"" onclick=opt" & (lValidCounter) & ".transferAllLeft() type=""button"" title=""" & gtMess.GetMessage("btn remove all") & """ name=""left""></a>" & vbcrlf)
					If nMetaTagType < MetaTagType_Searchable Then
					sb.append("<input onClick=opt" & (lValidCounter) & ".newinput() type=""button"" value=""&gt;&gt;"" name=""newinput""/>" & vbcrlf)
					End If
					sb.append("</td>" & vbcrlf)
					sb.append("<td width=""40%"">")
          sb.append("<select ondblclick=opt" & (lValidCounter) & ".transferLeft() multiple size=10 name=list2_" & (lValidCounter) & " style=""width:200px"">" & vbcrlf)
    				'If (ty = "update") Then
    					For i_option = 0 to UBound(arrMetaText)
							toption = Trim(arrMetaText(i_option))
							if toption <> ""  Then
								sb.append("<option value=""" & toption & """>" & toption & "</option>" & vbcrlf)
							end if
						next
					'End If

						sb.append("</select>" & vbcrlf)
						sb.append("<scr" & "ipt type='text/javascript'>" & vbcrlf)
						sb.append("if(top.mediauploader)" & vbcrlf)
						sb.append("{" & vbcrlf)
						sb.append(" var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
						sb.append(" var objForm = document.LibraryItem;" & vbcrlf)
						sb.append(" if(objRemoteForm && objForm)" & vbcrlf)
						sb.append(" {" & vbcrlf)
						sb.append("   if(objForm.newLeft" & (lValidCounter) & ".value != """" && objForm." & strName & ".value != """")" & vbcrlf)
						sb.append("   {" & vbcrlf)
						sb.append("     var wordArray = new Array;" & vbcrlf)
						sb.append("     wordArray = objForm." & strName & ".value.split(objForm.delimiter.value);" & vbcrlf)
						sb.append("     var path = objForm.list2_" & (lValidCounter) & ".options;" & vbcrlf)
						sb.append("     var j = path.length;" & vbcrlf)
						sb.append("     for(var i=0; i < wordArray.length; i++)" & vbcrlf)
						sb.append("     {" & vbcrlf)
						sb.append("       path[j + i]=new Option(wordArray[i]);" & vbcrlf)
						sb.append("     }" & vbcrlf)
						sb.append("   }" & vbcrlf)
						sb.append(" }" & vbcrlf)
						sb.append("}" & vbcrlf)
						sb.append("</scr" & "ipt>" & vbcrlf)
						sb.append("</td>" & vbcrlf)
						sb.append("</tr>" & vbcrlf)
						sb.append("</tbody>" & vbcrlf)
            sb.append("</table>" & vbcrlf)
            sb.append("</div>")

						sb.append("<scr" & "ipt  type=""text/JavaScript"">" & vbcrlf)
						sb.append("opt" & (lValidCounter) & ".init(document.forms[0]);" & vbcrlf)
						sb.append("</scr" & "ipt>" & vbcrlf)
					else
						sb.append("<select name=""" & strSelName & """ id=""" & strSelName & """ onChange=""JavaScript:outputSelected(document.forms[0]." & strSelName & ".options,'" & strName & "','" & strMetaSeparator & "');"">" & vbcrlf)
							sb.append("<option value="""">(No Selection)</option>" & vbcrlf)
							 For i_option = 0  to UBound(arrMetaOptions)
								toption = Trim(arrMetaOptions(i_option))
								if toption <> "" Then
									sb.append("<option value=""" & toption & """")
									if (Testselection(arrMetaText ,toption)) Then
										sb.append(" selected")
									end if
									sb.append("><" & toption & "</option>" & vbcrlf)
								end if
							Next
						sb.append("</select>" & vbcrlf)
						sb.append("<input type=""hidden"" name=""" & strName & """ value=""" & Server.HTMLEncode(strMetaValue) & """>" & vbcrlf)
						sb.append("<scr" & "ipt  type=""text/javascript"">" & vbcrlf)
						sb.append("<!--" & vbcrlf)
						sb.append("if(top.mediauploader)" & vbcrlf)
						sb.append("{" & vbcrlf)
						sb.append("var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
						sb.append("var objForm = document.LibraryItem;" & vbcrlf)
						sb.append("if(objRemoteForm && objForm)" & vbcrlf)
						sb.append("{" & vbcrlf)
						sb.append("objForm." & strName & ".value = objRemoteForm." & strName & ".value;" & vbcrlf)
						sb.append("objForm." & strSelName & ".selectedIndex = objRemoteForm." & strSelName & ".value;" & vbcrlf)
						sb.append("}" & vbCrLf)
						sb.append("}" & vbcrlf)
						sb.append("// -->" & vbcrlf)
						sb.append("</scr" & "ipt>" & vbcrlf)
					end if

					If bIsDefault Then
						sb.append("<br /><br /><a class=""button buttonInline greenHover buttonUpdate"" style=""margin:0;"" href=""#"" id=""frm_metadefault_" & nMetaTagType & "_" & DataType & "_update"" disabled onclick=""UpdateSelectList(this, '" & strSelName & "'); return false;"">Update list</a><br /><br />" & vbcrlf)
          End If

ElseIf ((NUMBER_PROP = DataType) _
OrElse (BYTE_PROP = DataType) _
OrElse (DOUBLE_PROP = DataType) _
OrElse (FLOAT_PROP = DataType) _
OrElse (INTEGER_PROP= DataType) _
OrElse (LONG_PROP = DataType) _
OrElse (SHORT_PROP = DataType)) Then
sb.append("<input type=""hidden"" name=""" & strSelName & """ id=""" & strSelName & """ value=""""/>" & vbcrlf)
					sb.append("<input type=""text"" name=""" & strName & """ id=""" & strName & """ size=""10"" maxlength=""500"" style=""text-align:right;"" value=""" & Server.HTMLEncode(strMetaValue) & """/>&nbsp; &nbsp;" & vbcrlf)
					If Not bIsDefault Then
						sb.append(WriteDefaultButton(strName))
					End If
					sb.append("<scr" & "ipt  type=""text/javascript"">" & vbcrlf)
					sb.append("<!--" & vbcrlf)
					sb.append("if(top.mediauploader)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
					sb.append("var objForm = document.LibraryItem;" & vbcrlf)
					sb.append("if(objRemoteForm && objForm)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("objForm." & strName & ".value = objRemoteForm." & strName & ".value;" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("}" & vbcrlf)
					sb.append("// -->" & vbcrlf)
					sb.append("</scr" & "ipt>" & vbcrlf)

				ElseIf DATE_PROP = DataType Then
					'sb.append("<input type=""hidden"" name=""" & strName & """ id=""" & strName & """ value=""" & Server.HTMLEncode(strMetaValue) & """/>" & vbcrlf)
					'sb.append("<input type=""text"" name=""" & strSelName & """ id=""" & strSelName & """ onClick=""javascript:OpenCalendarForElement('" & strSelName & "', '" & strName & "');return false;"" size=""18"" style=""text-align:center;"" value=""" & Server.HTMLEncode(getDateFromString(strMetaValue)) & """ />&#160;<a ")
					'sb.append("href=""#"" onClick=""javascript:OpenCalendarForElement('" & strSelName & "', '" & strName & "');return false;""><img ")
					'sb.append("src=""" & AppImgPath & "btn_calendar-nm.gif"" alt=""pick date"" id=""" & strName & "Img""></a>&nbsp; &nbsp;" & vbcrlf)
                    Dim dso As EkDTSelector
                    dso = sfSiteRef.EkDTSelectorRef
                    dso.formName = "metadefinition"
                    If (strMetaValue <> "") Then
                        dso.targetDate = Convert.ToDateTime(strMetaValue)
                    End If
                    sb.Append(dso.displayCultureDate(True, strName & "default_span", strName))
					sb.append("<scr" & "ipt  type=""text/javascript"">" & vbcrlf)
					sb.append("<!--" & vbcrlf)
					sb.append("if(top.mediauploader)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
					sb.append("var objForm = document.LibraryItem;" & vbcrlf)
					sb.append("if(objRemoteForm && objForm)" & vbcrlf)
					sb.append("{" & vbcrlf)
					sb.append("objForm." & strName & ".value = objRemoteForm." & strName & ".value;" & vbcrlf)
					sb.append("objForm." & strSelName & ".value = objRemoteForm." & strSelName & ".value;" & vbcrlf)
					sb.append("}" & vbCrLf)
					sb.append("}" & vbcrlf)
					sb.append("// -->" & vbcrlf)
					sb.append("</scr" & "ipt>" & vbcrlf)
				Else
					sb.append("<input type=""hidden"" name=""" & strSelName & """ id=""" & strSelName & """ value=""""/>" & vbcrlf)
					sb.append("<textarea name=""" & strName & """ id=""" & strName & """ wrap=""soft"" style=""width:100%"" ")
					sb.append("onclick=""UpdateTextCounter('" & strName & "', 2000)"" onkeyup=""UpdateTextCounter('" & strName & "', 2000)"" onchange=""UpdateTextCounter('" & strName & "', 2000)"" ")
					sb.append(">" & Server.HTMLEncode(strMetaValue) & "</textarea><br />" & vbcrlf)
					If Not bIsDefault Then
						sb.append(WriteDefaultButton(strName))
						sb.append("&nbsp;&nbsp;")
						End If
						sb.append("<div class=""ektronCaption"">" & gtMess.GetMessage("lbl current character count") & " &nbsp;<span id=""" & strName & "_len"">0</span>&nbsp; (2000 " & gtMess.GetMessage("abbreviation for maximum") & ")</div>")
					If MetaTagType_Meta = nMetaTagType Then
						sb.append("&nbsp;&nbsp; &nbsp;&nbsp;")
						sb.append(gtMess.GetMessage("generic Separator Character") & " """ & strMetaSeparator & """")
					End If
					sb.append("<br /><br />" & vbcrlf)
					sb.Append("<scr" & "ipt  type=""text/javascript"">" & vbcrlf)
					sb.Append("<!--" & vbcrlf)
					sb.Append("if(top.mediauploader)" & vbcrlf)
					sb.Append("{" & vbcrlf)
					sb.Append("var objRemoteForm = top.mediauploader.document.LibraryItem;" & vbcrlf)
					sb.Append("var objForm = document.LibraryItem;" & vbcrlf)
					sb.Append("if(objRemoteForm && objForm)" & vbcrlf)
					sb.Append("{" & vbcrlf)
					sb.Append("objForm." & strName & ".value = objRemoteForm." & strName & ".value;" & vbcrlf)
					sb.Append("}" & vbCrLf)
					sb.Append("}" & vbcrlf)
					sb.Append("// -->" & vbcrlf)
					sb.Append("</scr" & "ipt>" & vbcrlf)
					sb.Append("<scr" & "ipt  type=""text/javascript"">" & vbcrlf)
					sb.Append("<!--" & vbcrlf)
					sb.Append("UpdateTextCounter('" & strName & "', 2000);" & vbcrlf)
					sb.Append("// -->" & vbcrlf)
					sb.Append("</scr" & "ipt>" & vbcrlf)
			end if  ' End of SelectableOnly

			If ((bIsDefault) and (nMetaTypeID <> 0)) Then
				sb.append("<input type=""hidden"" name=""needed_validation"" value=""" & strName & "," & DataType & """>" & vbcrlf)
			End if

			sb.append("</td>" & vbcrlf)
		else
			sb.append("<td>" & WriteMetadataValue(DataType, strMetaValue) & "</td>" & vbcrlf)
		end if
	sb.append("</tr>")

		return(sb.ToString)
		End Function

		Function WriteDefaultButton(ByVal id as Object) as Object
		Dim sb as New System.Text.StringBuilder
		sb.append("<a href=""#"" onclick=""SetDefault('" & id & "');return false;""><img ")
	sb.append("src=""" & gtMess.GetMessage("ImagePath") & "btn_default.gif"" ")
	sb.append("alt=""" & gtMess.GetMessage("alt default button text") & """ title=""" & gtMess.GetMessage("alt default button text") & """></a>")

		Return(sb.ToString)

		End Function

		Function Testselection(byRef arrofdata as Object, ByVal MetaOptions as Object) as Object
		Dim i, match as Object
		match = False
		For i = 0 to  UBound(arrofdata)
		if (Trim(arrofdata(i)) = MetaOptions) Then
		match = True
		end if
		Next
		Testselection = match
		End Function

		Function WriteMetadataForEdit(ByVal cMetadataTypes as Object, ByVal bSearchableOnly as Object, ByVal ty as Object) as Object
		Dim sb as New System.Text.StringBuilder
		Dim lValidCounter as Integer = 0
		Dim nNumCustomSearchProperties as Object
		dim strReqFields, firstField, strValidateFields, firstValid, cMetadataType as Object
		strValidateFields = ""
		strReqFields = ""
		firstField = "true"
		firstValid = "true"
		nNumCustomSearchProperties = 0
		if (cMetadataTypes.count > 0) then
		for each cMetadataType in cMetadataTypes
		If cMetadataType("MetaTagType") >= MetaTagType_Searchable And cMetadataType("MetaEditable") Then
		nNumCustomSearchProperties = nNumCustomSearchProperties + 1
		End If
		Next
		End If
		sb.Append("<sc" & "ript type=""text/JavaScript"" ")
	sb.Append("type=""text/javascript"" src=""java/optiontransfer.js""></scr" & "ipt>")

	If Not bSearchableOnly Then
		' A fieldset is needed to properly stack the regions for some unknown reason,
		' otherwise the search properties display to the right of the non-search properties.
		sb.Append("<fieldset>")
		sb.Append("<table width=""100%"">")
		sb.Append("<col width=""5%""><col width=""95%"">")
		For each cMetadataType in cMetadataTypes
			If cMetadataType("MetaTagType") < MetaTagType_Searchable And cMetadataType("MetaEditable") Then
				lValidCounter = lValidCounter + 1
				sb.Append(WriteMetadataTypeForEdit(cMetadataType, lValidCounter, ty))
				if (cMetadataType("MetaRequired")) then
					if(firstField = "true") then
						strReqFields = "frm_text_" & lValidCounter
						firstField = "false"
					else
						strReqFields = strReqFields & ",frm_text_" & lValidCounter
					end if
				end if
				if(firstValid = "true") then
					strValidateFields = "frm_text_" & lValidCounter & "," & cMetadataType("MetaNameTitle")
					firstValid = "false"
				else
					strValidateFields = strValidateFields & ":frm_text_" & lValidCounter & "," & cMetadataType("MetaNameTitle")
				end if
			End If
		Next ' not MetaTagType_Searchable
		sb.Append("</table></fieldset>")
	End If
	If nNumCustomSearchProperties > 0 Then
	sb.append("<fieldset><legend>Search Data</legend>")
		sb.append("<table width=""100%"">")
			sb.append("<col width=""5%""><col width=""95%"">")
		for each cMetadataType in cMetadataTypes
			If cMetadataType("MetaTagType") >= MetaTagType_Searchable And cMetadataType("MetaEditable") Then
				lValidCounter = lValidCounter + 1
				sb.Append(WriteMetadataTypeForEdit(cMetadataType, lValidCounter, ty))
				if (cMetadataType("MetaRequired")) then
					if(firstField = "true") then
						strReqFields = "frm_text_" & lValidCounter
						firstField = "false"
					else
						strReqFields = strReqFields & ",frm_text_" & lValidCounter
					end if
				end if
				if(firstValid = "true") then
					strValidateFields = "frm_text_" & lValidCounter & "," & cMetadataType("MetaNameTitle")
					firstValid = "false"
				else
					strValidateFields = strValidateFields & ":frm_text_" & lValidCounter & "," & cMetadataType("MetaNameTitle")
				end if
			End If
		Next
		sb.append("</table>")
	sb.append("</fieldset>")
	End If
	If Len(strReqFields) > 0 Then
	sb.Append("<table width=""100%"">")
	sb.Append("<tr>")
	sb.Append("<td width=""20%"" nowrap=""true"" align=""right"">")
	sb.Append("<font color=""red""><b>" & gtMess.GetMessage("explanation of asterisk") & "</b></font>")
	sb.Append("</td>")
	sb.Append("<td></td>")
	sb.Append("</tr>")
	sb.Append("</table>")
	End If
	sb.append("<input type=""hidden"" name=""frm_validcounter"" value=""" & (lValidCounter) & """>")
	sb.append("<input type=""hidden"" name=""req_fields"" value=""" & (strReqFields) & """>")
	sb.append("<input type=""hidden"" name=""needed_validation"" value=""" & (strValidateFields) & """>")
	sb.append("<scr" & "ipt language=""javascript"">")
	sb.append("var nLimit, temp;")
	sb.append("var temp = """ & (lValidCounter) & """;")
	sb.append("if (temp == """") {")
	sb.append("nLimit = 0;")
	sb.append("}")
	sb.append("else {")
	sb.append("nLimit = parseInt(temp, 10);")
	sb.append("if (isNaN(nLimit)) {")
	sb.append("nLimit = 0;")
	sb.append("}")
	sb.append("}")
	sb.append("if (nLimit && typeof document.forms[0].frm_text_1 != ""undefined"") {")
	sb.append("if (document.forms[0].frm_text_1.type  != ""hidden"") {")
	sb.append("try {")
	sb.append("SetFullScreenView(false); //  Ensure that controls are not hidden.")
	sb.append("document.forms[0].frm_text_1.focus();")
	sb.append("}")
	sb.append("catch(er) {")
	sb.append("}")
	sb.append("}")
	sb.append("}")
	sb.append("</scr" & "ipt>")

	return (sb.Tostring)
end function

Function WriteSearchEditProp(ByVal DataType as Object, ByVal Required as Object, ByVal Name as Object, ByVal Items as Object, ByVal Value as Object, ByVal DefaultValue as Object) as Object
	Dim sb as New System.Text.StringBuilder
	Dim addTextInput as Boolean = False

	Select Case DataType
		Case TEXT_PROP
			sb.append("<textarea name=""" & Name & """ id=""" & Name & """ onkeyup=""textCounter(this, document.forms[0]." & Name & "_len, 500)"" onkeydown=""textCounter(this, document.forms[0]." & Name & "_len, 500)"" wrap=""soft"" style=""width:100%"">" & Server.HTMLEncode(Value) & "</textarea><br />")
			sb.append("<input readonly type=""text"" name=""" & Name & "_len"" size=""3"" maxlength=""3"" value=""500"">characters left")
			sb.append("<scr" & "ipt language=""javascript"">textCounter(document.forms[0]." & Name & ", document.forms[0]." & Name & "_len, 500)</scr" & "ipt>")
			if Required then
				sb.append("<span style=""COLOR: red"">* " & gtMess.GetMessage("metadata required msg") & "</span>")
			end if
			If (DefaultValue <> "") Then
				sb.append("<br />")
				sb.append("<a href=""#"" OnClick=""SetDefault('" & Name & "');return false;""><img src=""" & sfSiteRef.AppImgPath & "btn_default.gif"" alt=""" & gtMess.getMessage("alt default button text") & """ title=""" & gtMess.getMessage("alt default button text") & """></a>")
			End If
		Case NUMBER_PROP
			addTextInput = True
		Case BYTE_PROP
			addTextInput = True
		Case DOUBLE_PROP
			addTextInput = True
		Case FLOAT_PROP
			addTextInput = True
		Case INTEGER_PROP
			addTextInput = True
		Case LONG_PROP
			addTextInput = True
		Case SHORT_PROP
			addTextInput = True
		Case DATE_PROP
			addTextInput = True
		Case SELECT_PROP, SELECT1_PROP
			sb.append("<select name=""" & Name & """ " & IIf(SELECT1_PROP = DataType, " size=1", " size=5") & " " & IIf(SELECT1_PROP = DataType, "", " multiple") & ">")
			sb.append("<option value="""">(No selection)</option>")
			Dim objItem as Object
			If Not Items Is Nothing Then
				For Each objItem in Items
					sb.append("<option value=""" & objItem & """>" & objItem & "</option>")
				Next
			End If
			sb.append("</select>")
		Case BOOLEAN_PROP
			sb.append("<input type=""checkbox"" name=""" & Name & """>")
		Case Else
	End Select
	If (addTextInput) Then
		sb.append("<input type=""text"" name=""" & Name & """ value=""" & Value & """>")
	End If
	Return(sb.ToString)
End Function

</script>
