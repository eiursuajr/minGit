<%@ Page Language="C#" AutoEventWireup="true" %>
<div id="overlay-container">
	<% if((!string.IsNullOrEmpty(Request.QueryString["type"])) && (Request.QueryString["type"] == "heatmap")) { %>
	<div class="click" style="left:100px;top:100px"></div>
	<div class="click" style="left:120px;top:120px"></div>
	<div class="click" style="left:130px;top:130px"></div>
	<div class="click" style="left:140px;top:140px"></div>
	<% } %>
	<% else if ((!string.IsNullOrEmpty(Request.QueryString["type"])) && (Request.QueryString["type"] == "abandonpath"))
        { %>
	<ul id="_10PercentBlock" class="PercentBlock10">
		<ul>
			<li>Abandon Rate: <span id="Rate10">10</span>%</li>
			<li>Abandoned Count: 10 Visitors</li>
			<li>Total Visitors: 10 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_20PercentBlock" class="PercentBlock20">
		<ul>
			<li>Abandon Rate: <span id="Rate20">20</span>%</li>
			<li>Abandoned Count: 20 Visitors</li>
			<li>Total Visitors: 20 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_30PercentBlock" class="PercentBlock30">
		<ul>
			<li>Abandon Rate: <span id="Rate30">30</span>%</li>
			<li>Abandoned Count: 30 Visitors</li>
			<li>Total Visitors: 30 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_40PercentBlock" class="PercentBlock40">
		<ul>
			<li>Abandon Rate: <span id="Rate40">40</span>%</li>
			<li>Abandoned Count: 40 Visitors</li>
			<li>Total Visitors: 40 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_50PercentBlock" class="PercentBlock50">
		<ul>
			<li>Abandon Rate: <span id="Rate50">50</span>%</li>
			<li>Abandoned Count: 50 Visitors</li>
			<li>Total Visitors: 50 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_60PercentBlock" class="PercentBlock60">
		<ul>
			<li>Abandon Rate: <span id="Rate60">60</span>%</li>
			<li>Abandoned Count: 60 Visitors</li>
			<li>Total Visitors: 60 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_70PercentBlock" class="PercentBlock70">
		<ul>
			<li>Abandon Rate: <span id="Rate70">70</span>%</li>
			<li>Abandoned Count: 70 Visitors</li>
			<li>Total Visitors: 70 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_80PercentBlock" class="PercentBlock80">
		<ul>
			<li>Abandon Rate: <span id="Rate80">80</span>%</li>
			<li>Abandoned Count: 80 Visitors</li>
			<li>Total Visitors: 80 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_90PercentBlock" class="PercentBlock90">
		<ul>
			<li>Abandon Rate: <span id="Rate90">90</span>%</li>
			<li>Abandoned Count: 90 Visitors</li>
			<li>Total Visitors: 90 Views</li>
		</ul>
		<p></p>
	</ul>
	<ul id="_100PercentBlock" class="PercentBlock100">
		<ul>
			<li>Abandon Rate: <span id="Rate100">100</span>%</li>
			<li>Abandoned Count: 100 Visitors</li>
			<li>Total Visitors: 100 Views</li>
		</ul>
		<p></p>
	</ul>
	<% } %>
</div>

