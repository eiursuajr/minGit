// Copyright 2003 Ektron, Inc.
// Revision Date: 2006-Jul-20

setTimeout('isInstall()',10000);

function ReloadParent()
{
	if (top.opener && !top.opener.closed)
	{
		top.opener.location.reload();
		//document.forms[0].btnClose.value = "Close Window";
		try
		{
		self.setInterval('self.close()', 2000);
		}
		catch (ex) {}
	}
}

function isInstall()
{
	var versionInstalled = ActiveXVersionInstalled(cPROGID) + "";
	var isInstalled = false;
	if (versionInstalled.length > 0)
	{
		if (ActiveXElement_compareVersion(cVERSION,versionInstalled) >= 0)
		{
			versionInstalled = ActiveXVersionInstalled(cIMGEDPROGID) + "";
			if (versionInstalled.length > 0)
			{
				if (ActiveXElement_compareVersion(cIMGEDVERSION,versionInstalled) >= 0)
				{
					isInstalled = true;
				}
			}
		}
	}
	if (isInstalled)
	//if (eWebEditPro.isInstalled)
	{
		ReloadParent();			 
	}
	else
	{
		setTimeout('isInstall()',5000);
	}
}

function ExecuteInstall()
{
	location.href = eWebEditProDefaults.clientInstall;
}

function DomainPath()
{
	var sPath = window.location + "";
	var lpos = -1;
	if("undefined" != typeof given_eWebEditProPath)
	{
		lpos = sPath.indexOf(given_eWebEditProPath);
		if (lpos <= 0)
		{
			lpos = sPath.indexOf(eWebEditProPath);
		}
	}
	if (lpos > 0)
	{
		sPath = sPath.substring(0, lpos);
	}
	return sPath;
}
