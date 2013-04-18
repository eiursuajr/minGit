' Copyright 2004, Ektron, Inc.
' Revision Date: 2004-01-06
'
' Returns string with version number or empty if not installed
' Version format: n,n,n,n
Function eWebDiffInstalled()
	On Error resume next
	eWebDiffInstalled = "1,0,0,4" 'By default it should be 1,0,0,4 
	'Set objEWebDiff = CreateObject("7CEE8829-F5DA-4330-8293-5CAE78DFFEEB")
	Set objEWebDiff = CreateObject("eWebDiff.Install")
	If IsObject(objEWebDiff) Then		
		eWebDiffInstalled = objEWebDiff.Version
	End If
	
	Set objEWebDiff = Nothing
end function