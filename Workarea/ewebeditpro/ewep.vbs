' Copyright 2001-2006, Ektron, Inc.
' Revision Date: 2006-Jun-28
'

Function ActiveXVersionInstalled(strProgId)
' Returns string with version number or empty if not installed
' Version format: n,n,n,n
	On Error Resume Next
	ActiveXVersionInstalled = ""
	Dim objEkVersion
	Set objEkVersion = CreateObject("EkVersion.ekVersionInterface")
	If IsObject(objEkVersion) Then
		If Not (objEkVersion Is Nothing) Then
			objEkVersion.AssignUID strProgId
			If objEkVersion.Exists >= 3 Then
				ActiveXVersionInstalled = objEkVersion.Version
			End If
		End If
	End If
	Set objEkVersion = Nothing
End Function
