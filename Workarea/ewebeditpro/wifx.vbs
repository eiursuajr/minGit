' Copyright 2001, Ektron, Inc.
' Revision Date: 2001-07-05
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
			If objEkVersion.Exists Then
				ActiveXVersionInstalled = objEkVersion.Version
			End If
		End If
	End If
	Set objEkVersion = Nothing
End Function
