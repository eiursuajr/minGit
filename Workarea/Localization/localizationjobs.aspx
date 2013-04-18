<%@ Page Language="C#" AutoEventWireup="false" CodeFile="localizationjobs.aspx.cs" Inherits="Workarea_localizationjobs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Localization Jobs</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link rel="stylesheet" href="../csslib/ektron.workarea.css" type="text/css"/>
    <style type="text/css">
	#tvJobs td
	{
		vertical-align:top;
	}
	/* total width:80ex */
	.L10nJob
	{
		clear:both;
	}
	.L10nJob div
	{
		float:left;
	}
	.L10nJobTitle
	{
		width:50ex;
	}
	.L10nJobStartTime
	{
		width:15ex;
	}
	.L10nJobState
	{
		width:15ex;
	}
	.L10nJobProgress
	{
		width:100px;
		padding-left:0 !important;
		padding-right:0 !important;
		border:solid 1px;
	}
	a.L10nJobCancel, a.L10nJobDelete, a.L10nJobDeleteAllPrevious
	{
	    float: right;
		margin-left:2ex;
	}
	
	.L10nZip
	{
		clear:both;
	}
	.L10nZipLink
	{
		font-weight:bold;
		color:blue;
		cursor:pointer;
		clear:both;
	}
	.L10nZip div, .L10nZipLink div
	{
		padding-left:2px;
		padding-right:2px;
		margin:0;
		float:left;
	}
	.L10nZipFileName
	{
		width:50ex;
	}
	.L10nZipFileSize
	{
		width:20ex;
		text-align:right;
	}
	
	.L10nSkl
	{
		clear:both;
	}
	.L10nSkl div
	{
		padding-left:2px;
		padding-right:2px;
		margin:0;
		float:left;
	}
	.L10nSklItemID
	{
		width:10ex;
		text-align:right;
	}
	.L10nSklTitle
	{
		width:55ex;
	}
	.L10nSklDate
	{
		width:15ex;
	}
	
	.L10nXlf
	{
		clear:both;
	}
	.L10nXlf div
	{
		padding-left:2px;
		padding-right:2px;
		margin:0;
		float:left;
	}
	.L10nXlfFileName
	{
		width:40ex;
	}
	.L10nXlfDate
	{
		width:15ex;
	}
	.L10nXlfState
	{
		width:20ex;
	}
	
	img.L10nErrorAlert 
	{
		vertical-align:middle;
		border-style:none;
	}
	.L10nMessage
	{
		background-color:#FFFFCC; 
		border:gray 1px dotted; 
		padding:2px; 
		margin-bottom:2px;
	}
	.L10nErrorMessage
	{
		background-color:#CCCCFF; 
		border:#3300cc 1px inset; 
		padding:2px; 
		margin-bottom:2px;
	}
	img.L10nFlag
	{
		border-style:none;
		vertical-align:top;
	}
	
    body
    {
		width:96%;
    }
    </style>
    <script type="text/javascript">
    <!--//--><![CDATA[//><!--
    function confirmDeleteAction()
    {
        return confirm('<asp:Literal ID="ltrConfirmDelMsg" runat="server"></asp:Literal>');
    }
	function myTVNodeHover(oElem)
	{
		if (!oElem) return;
		try
		{
			while (oElem.tagName != "TD")
			{
				oElem = oElem.parentNode;
			}
			oElem.style.backgroundColor = "#DDEAFB";
		}
		catch (e)
		{
			// ignore
		}
	}
	function myTVNodeUnhover(oElem)
	{
		if (!oElem) return;
		try
		{
			while (oElem.tagName != "TD")
			{
				oElem = oElem.parentNode;
			}
			oElem.style.backgroundColor = "#FFFFFF";
		}
		catch (e)
		{
			// ignore
		}
	}
	
	// Define WebForm_InitCallback in case it is missing or not defined yet.
	if (typeof WebForm_InitCallback != "function")
	{
		WebForm_InitCallback = new Function();
	}
	
	// Define tvJobs_Data b/c the TreeView won't define it until
	// the page is fully loaded, which may result in a JavaScript
	// error when the mouse hovers over items if the tree is long.
	var tvJobs_Data = new Object();
	tvJobs_Data.images = new Array();
	tvJobs_Data.collapseToolTip = "Collapse";
	tvJobs_Data.expandToolTip = "Expand";
	tvJobs_Data.expandState = null;
	tvJobs_Data.selectedNodeID = null;
	tvJobs_Data.hoverClass = 'tvJobs_6';
	tvJobs_Data.hoverHyperLinkClass = 'tvJobs_5';
	tvJobs_Data.lastIndex = 0;
	tvJobs_Data.populateLog = null;
	tvJobs_Data.treeViewID = 'tvJobs';
	tvJobs_Data.name = 'tvJobs_Data';
    //--><!]]>
	</script>
</head>
<body>
    <form id="form1" runat="server">
		<asp:TreeView ID="tvJobs" runat="server" NodeIndent="10" ExpandDepth="0" EnableViewState="False" NodeWrap="True">
			<SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" VerticalPadding="0px" />
			<NodeStyle HorizontalPadding="5px" />
		</asp:TreeView>
    </form>
</body>
</html>
