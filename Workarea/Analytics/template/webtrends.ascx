﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="webtrends.ascx.cs" Inherits="Analytics_Template_WebTrends" EnableTheming="false" EnableViewState="false" %>
<!-- START OF SmartSource Data Collector TAG -->
<!-- Copyright (c) 1996-2010 WebTrends Inc.  All rights reserved. -->
<!-- Version: 8.6.2 -->
<!-- Tag Builder Version: 3.0  -->
<!-- Created: 8/16/2010 5:57:23 PM -->
<script src="<asp:literal id='AppPath' runat='server'/>analytics/template/webtrends.js" type="text/javascript"></script>
<!-- ----------------------------------------------------------------------------------- -->
<!-- Warning: The two script blocks below must remain inline. Moving them to an external -->
<!-- JavaScript include file can cause serious problems with cross-domain tracking.      -->
<!-- ----------------------------------------------------------------------------------- -->
<script type="text/javascript">
    //<![CDATA[
    var _tag = new WebTrends();
    _tag.dcsGetId();
    //]]>>
</script>
<script type="text/javascript">
    //<![CDATA[
    // Add custom parameters here.
    //_tag.DCSext.param_name=param_value;
    _tag.dcsCollect();
    //]]>>
</script>
<noscript>
<div><img alt="DCSIMG" id="DCSIMG" width="1" height="1" src="http://statse.webtrendslive.com/[Your dcsId]/njs.gif?dcsuri=/nojavascript&amp;WT.js=No&amp;WT.tv=8.6.2"/></div>
</noscript>
<!-- END OF SmartSource Data Collector TAG -->
