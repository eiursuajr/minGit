<script language="C#" runat="server">
	string callBackPage = string.Empty;
	public string getCallBackupPage(string defval)
	{
		string returnValue=string.Empty;
		string tmpStr = string.Empty;
		if (!String.IsNullOrEmpty(Request.QueryString["callbackpage"]))
		{
			tmpStr = (string) (Request.QueryString["callbackpage"]);
            if ((tmpStr == "cmsform.aspx") && !String.IsNullOrEmpty(Request.QueryString["fldid"]))
			{
				tmpStr = System.Convert.ToString(tmpStr + "?folder_id=" + Request.QueryString["fldid"] + "&" + Request.QueryString["parm1"] + "=" + Request.QueryString["value1"]);
			}
			else
			{
				tmpStr = System.Convert.ToString(tmpStr + "?" + Request.QueryString["parm1"] + "=" + Request.QueryString["value1"]);
			}
			
			if (!String.IsNullOrEmpty(Request.QueryString["parm2"]))
			{
				tmpStr = System.Convert.ToString(tmpStr + "&" + Request.QueryString["parm2"] + "=" + Request.QueryString["value2"]);
				if (!String.IsNullOrEmpty(Request.QueryString["parm3"]))
				{
					tmpStr = System.Convert.ToString(tmpStr + "&" + Request.QueryString["parm3"] + "=" + Request.QueryString["value3"]);
					if (!String.IsNullOrEmpty(Request.QueryString["parm4"]))
					{
						tmpStr = System.Convert.ToString(tmpStr + "&" + Request.QueryString["parm4"] + "=" + Request.QueryString["value4"]);
					}
				}
			}
			returnValue = tmpStr;
		}
		else
		{
			returnValue = defval;
		}
		return returnValue;
	}
	//This function will pass pack the url paremeter so that they can be passed along
	public string BuildCallBackParms(string leadingChar)
	{
		string returnValue=string.Empty;
		string strTmp2 = string.Empty;
		if (!String.IsNullOrEmpty(Request.QueryString["callbackpage"]))
		{
			strTmp2 = (string) ("callbackpage=" + Request.QueryString["callbackpage"] + "&parm1=" + Request.QueryString["parm1"] + "&value1=" + Request.QueryString["value1"]);
			if (Request.QueryString["parm2"] != "")
			{
				strTmp2 = System.Convert.ToString(strTmp2 + "&parm2=" + Request.QueryString["parm2"] + "&value2=" + Request.QueryString["value2"]);
				if (Request.QueryString["parm3"] != "")
				{
					strTmp2 = System.Convert.ToString(strTmp2 + "&parm3=" + Request.QueryString["parm3"] + "&value3=" + Request.QueryString["value3"]);
					if (Request.QueryString["parm4"] != "")
					{
						strTmp2 = System.Convert.ToString(strTmp2 + "&parm4=" + Request.QueryString["parm4"] + "&value4=" + Request.QueryString["value4"]);
					}
				}
			}
			strTmp2 = System.Convert.ToString(leadingChar + strTmp2);
			returnValue = strTmp2;
		}
		else
		{
			returnValue = "";
		}
		return returnValue;
	}
	
	public string getCallingpage(string defVal)
	{
		string returnValue;
		string tmp2;
		if (!String.IsNullOrEmpty(Request.QueryString["callbackpage"]))
		{
			tmp2 = (string) (Request.QueryString["callbackpage"]);
			returnValue = tmp2;
		}
		else
		{
			returnValue = defVal;
		}
		return returnValue;
	}	
</script>


