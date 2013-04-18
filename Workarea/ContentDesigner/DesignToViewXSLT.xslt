<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" extension-element-prefixes="msxsl ekext" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:xslout="alias" xmlns:msxslout="aliasms" xmlns:ekext="urn:ektron:extension-object:common" xmlns:cms="urn:Ektron.Cms.Controls">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="no"/>
<xsl:namespace-alias stylesheet-prefix="xslout" result-prefix="xsl"/>
<xsl:namespace-alias stylesheet-prefix="msxslout" result-prefix="msxsl"/>

<!-- Run on a design page to create an XSLT to transform XML document for view on web page. -->

<xsl:include href="template_paramDesignTo.xslt"/>
<xsl:param name="fieldlistXPath"/>
<xsl:param name="srcPath" select="''"/>

<xsl:variable name="fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

<xsl:template match="/">
	<xslout:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl js dl atom cms" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:dl="urn:datalist" xmlns:cms="urn:Ektron.Cms.Controls" xmlns:atom="http://www.w3.org/2005/Atom">

		<xsl:if test="//ektdesignns_resource">
		<xslout:import href="{$srcPath}template_ektdesignns_resource.xslt"/>
		</xsl:if>

		<xslout:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>
		<xslout:strip-space elements="*"/>

		<!-- used by custom tag ektdesignns_conditional @displayWhen -->
		<xslout:param name="deviceConfiguration"/> <!-- node set -->
		<xslout:param name="userId" select="''"/>
		<xslout:param name="currentDate"/>		
		<xslout:param name="currentDateTime"/>		

		<xslout:variable name="ektdesignns_fieldlist" select="/*/ektdesignpackage_list/fieldlist"/>

		<xsl:call-template name="buildDatalists"/>

		<xslout:template match="/">
			<xsl:attribute name="xml:space">preserve</xsl:attribute>
			<!-- non-breaking space is needed to preserve script element -->
			<span style="display:none">&#160;</span><script language="JavaScript" type="text/javascript" defer="defer"><xslout:comment xml:space="preserve"><xslout:text>
function ektLocalizeDate(date, id) {
	setTimeout(function() {
	if (document.getElementById &amp;&amp; 10 == date.length) {
		var oTempDate = new Date(date.substr(0,4), parseInt(date.substr(5,2),10)-1, date.substr(8,2));
		document.getElementById(id).innerHTML=(oTempDate.toLocaleDateString ? oTempDate.toLocaleDateString() : oTempDate.toLocaleString());
	}
	}, 1); 
}
// </xslout:text></xslout:comment></script>
			<!-- begin diagnostics -->
			<!-- <ul>
				<li>$deviceConfiguration list:
					<ol>
						<xslout:for-each select="$deviceConfiguration">
							<li><xslout:value-of select="."/></li>
						</xslout:for-each>
					</ol>
				</li>
				<li>$userId = '<xslout:value-of select="$userId"/>'</li>
				<li>$currentDate = '<xslout:value-of select="$currentDate"/>'</li>
				<li>$currentDateTime = '<xslout:value-of select="$currentDateTime"/>'</li>
			</ul> -->
			<!-- end diagnostics -->
			<xsl:apply-templates>
				<xsl:with-param name="xpath" select="string($rootXPath)"/>
			</xsl:apply-templates>
		</xslout:template>

		<!-- It's simpler and faster to wrap TEXTAREA content in PRE tags
				than to try to replace CR/LF with BR tag. -->
		<!-- not currently used
		<xslout:template name="replace-substring">
			<xslout:param name="original"/>
			<xslout:param name="substring"/>
			<xslout:param name="replacement" select="''"/>
			<xslout:choose>
				<xslout:when test="contains($original, $substring)">
					<xslout:value-of select="substring-before($original, $substring)"/>
					<xslout:copy-of select="$replacement"/>
					<xslout:call-template name="replace-substring">
						<xslout:with-param name="original" select="substring-after($original, $substring)"/>
						<xslout:with-param name="substring" select="$substring"/>
						<xslout:with-param name="replacement" select="$replacement"/>
					</xslout:call-template>
				</xslout:when>
				<xslout:otherwise>
					<xslout:value-of select="$original"/>
				</xslout:otherwise>
			</xslout:choose>
		</xslout:template>
		-->
	</xslout:stylesheet>
</xsl:template>
	
<xsl:template match="/root">
	<xsl:param name="xpath"/>
	<xsl:apply-templates>
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:apply-templates>
</xsl:template>

<!-- nodetype ========================================================================= -->
	
<!-- match any -->
	
<xsl:template match="*[@ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_any">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="*[@ektdesignns_nodetype]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_any">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_any">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:call-template name="conditional">
		<xsl:with-param name="xpath" select="$new-xpath"/>
		<xsl:with-param name="content">
			<xsl:element name="{name()}">
				<!-- filter attributes, eg, contentEditable, class=design_richarea, onblur, etc -->
				<xsl:apply-templates select="@*[name()!='contenteditable' and not(starts-with(.,'design_'))]">
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
				<xsl:choose>
					<xsl:when test="@ektdesignns_nodetype='mixed'">
						<xsl:call-template name="processContent">
							<xsl:with-param name="xpath" select="$new-xpath"/>
							<xsl:with-param name="default" select="'mixed'"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:when test="@ektdesignns_nodetype='text'">
						<xsl:call-template name="processContent">
							<xsl:with-param name="xpath" select="$new-xpath"/>
							<xsl:with-param name="default" select="'text'"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="processContent">
							<xsl:with-param name="xpath" select="$new-xpath"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<!-- match textarea -->

<xsl:template match="textarea[@ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_textarea">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="textarea[@ektdesignns_nodetype]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_textarea">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_textarea">
	<xsl:param name="xpath"/>
	<xsl:if test="not(@ektdesignns_hidden='true')"> <!-- values of hidden fields are not shown -->
		<xsl:variable name="new-xpath">
			<xsl:call-template name="buildXPath">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:call-template>
		</xsl:variable>
		<xsl:call-template name="processContent">
			<xsl:with-param name="xpath" select="$new-xpath"/>
			<xsl:with-param name="default" select="'textarea'"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>


<!-- match custom design tags -->

<xsl:template match="ektdesignns_conditional">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_conditional">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_conditional" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_conditional">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_conditional">
	<xsl:param name="xpath"/>
	<xsl:call-template name="conditional">
		<xsl:with-param name="xpath" select="$xpath"/>
		<xsl:with-param name="condition" select="concat('xpath:',@displayWhen)"/>
		<xsl:with-param name="content">
			<xsl:call-template name="processContent">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:call-template>
		</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<!-- higher priority than template above '*[@...]' -->

<!-- block tags -->

<xsl:template match="ektdesignns_group|ektdesignns_richarea|ektdesignns_checklist|ektdesignns_choices" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_customtag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_group|ektdesignns_richarea|ektdesignns_checklist|ektdesignns_choices" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_customtag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_customtag">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:variable name="contentType">
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_group'">
				<xsl:value-of select="'content'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_richarea'">
				<xsl:value-of select="'mixed'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_checklist'">
				<xsl:value-of select="'checklist'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_choices'">
				<xsl:value-of select="'choices'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'content'"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<!-- convert custom tags to DIV tags -->
	<div>
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<xsl:call-template name="processContent">
			<xsl:with-param name="xpath" select="$new-xpath"/>
			<xsl:with-param name="default" select="$contentType"/>
		</xsl:call-template>
	</div>
</xsl:template>

<!-- inline tags -->

<xsl:template match="ektdesignns_calendar|ektdesignns_imageonly|ektdesignns_filelink" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_customtagnodiv">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_calendar|ektdesignns_imageonly|ektdesignns_filelink" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_customtagnodiv">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_customtagnodiv">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:variable name="contentType">
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_calendar'">
				<xsl:value-of select="'date'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'content'"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<!-- just handle inside of these custom tags -->
	<xsl:call-template name="processContent">
		<xsl:with-param name="xpath" select="$new-xpath"/>
		<xsl:with-param name="default" select="$contentType"/>
	</xsl:call-template>
</xsl:template>

<!-- resource selector -->

<xsl:template match="ektdesignns_resource" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_resource">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_resource" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_resource">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_resource">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>

	<xsl:variable name="idType" select="concat(':',@datavalue_idtype,':')"/>
	<xsl:variable name="appearance" select="@ektdesignns_appearance"/>
	<xsl:choose>
		<xsl:when test="@ektdesignns_maxoccurs='unbounded' or number(@ektdesignns_maxoccurs) &gt; 1 or number(@ektdesignns_minoccurs) &gt; 1">
			<!-- a list of resources -->
			<xsl:choose>
				<xsl:when test="contains($idType,':content:') and ($appearance='teaser' or $appearance='quicklink')">
					<cms:ContentList runat="server">
						<xsl:attribute name="DisplayXslt">
							<xsl:choose>
								<xsl:when test="$appearance='quicklink'">ecmNavigation</xsl:when>
								<xsl:when test="$appearance='teaser'">ecmTeaser</xsl:when>
								<xsl:otherwise>ecmTeaser</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xslout:attribute name="ContentIds">
							<xslout:for-each select="{$new-xpath}">
								<xslout:if test="position() &gt; 1"><xslout:text>,</xslout:text></xslout:if>
								<xslout:value-of select="."/>
							</xslout:for-each>
						</xslout:attribute>
					</cms:ContentList>
				</xsl:when>
				<xsl:otherwise>
					<ol class="resource">
						<xslout:for-each select="{$new-xpath}">
						<li style="vertical-align: top">
							<xslout:attribute name="class">
								<xslout:value-of select="'resource_'"/>
								<xslout:value-of select="translate(@datavalue_idtype,':','_')"/>
								<xsl:if test="$appearance">
									<xslout:value-of select="' appearance_'"/>
									<xslout:value-of select="substring-before(concat(@datavalue_idtype,':'),':')"/>
									<xslout:value-of select="'_{$appearance}'"/>
								</xsl:if>
							</xslout:attribute>
							<xslout:call-template name="ektdesignns_resource">
								<xslout:with-param name="context" select="."/>
								<xsl:if test="$appearance">
								<xslout:with-param name="appearance" select="'{$appearance}'"/>
								</xsl:if>
							</xslout:call-template>
						</li>
						</xslout:for-each>
					</ol>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xslout:call-template name="ektdesignns_resource">
				<xslout:with-param name="context" select="{$new-xpath}"/>
				<xsl:if test="$appearance">
				<xslout:with-param name="appearance" select="'{$appearance}'"/>
				</xsl:if>
			</xslout:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- match mergelist and mergefield -->

<xsl:template match="ektdesignns_mergelist" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_mergelist">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_mergelist" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_mergelist">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_mergelist">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<!-- define variables for mergefield -->
	<xsl:call-template name="beginDatalistAccess">
		<xsl:with-param name="datalistXPath" select="'$ektdesignns_fieldlist'"/>
	</xsl:call-template>
	<xsl:choose>
		<xsl:when test="@ektdesignns_xslt">
			<xslout:variable name="xpath" select="{$new-xpath}"/>
			<!-- embed supplied XSLT snippet -->
			<xsl:value-of disable-output-escaping="yes" select="@ektdesignns_xslt"/>
		</xsl:when>
		<xsl:when test=".//@ektdesignns_list">
			<!-- some element has the list attribute, so simply process -->
			<xsl:apply-templates select="node()">
				<xsl:with-param name="xpath" select="$new-xpath"/>
			</xsl:apply-templates>
		</xsl:when>
		<xsl:otherwise>
			<!-- no element has the list attribute, so wrap contents in for-each -->
			<xslout:for-each select="{$new-xpath}">
				<xsl:apply-templates select="node()">
					<xsl:with-param name="xpath" select="'.'"/>
				</xsl:apply-templates>
			</xslout:for-each>
		</xsl:otherwise>
	</xsl:choose>
	<xsl:call-template name="endDatalistAccess"/>
</xsl:template>

<xsl:template match="*[@ektdesignns_list]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="*[@ektdesignns_list]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_list">
	<xsl:param name="xpath"/>
	<xsl:copy>
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
		<xslout:for-each select="{$xpath}">
			<xsl:apply-templates select="node()">
				<xsl:with-param name="xpath" select="'.'"/>
			</xsl:apply-templates>
		</xslout:for-each>
	</xsl:copy>
</xsl:template>

<xsl:template match="ektdesignns_mergefield" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_mergefield">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_mergefield" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_mergefield">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_mergefield">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:variable name="mergelist" select="ancestor::ektdesignns_mergelist"/>
	<xsl:choose>
		<xsl:when test="@ektdesignns_datalist|$mergelist/@ektdesignns_datalist">
			<xsl:if test="@ektdesignns_datalist">
				<xsl:call-template name="beginDatalistAccess">
					<xsl:with-param name="datalistXPath" select="'$ektdesignns_fieldlist'"/>
				</xsl:call-template>
			</xsl:if>
			<xslout:variable name="value" select="string({$new-xpath})"/>
			<xsl:choose>
				<xsl:when test="$mergelist/@ektdesignns_captionxpath and $mergelist/@ektdesignns_valuexpath">
					<xsl:call-template name="buildDatalistItemDisplayValue">
						<xsl:with-param name="captionxpath" select="$mergelist/@ektdesignns_captionxpath"/>
						<xsl:with-param name="valuexpath" select="$mergelist/@ektdesignns_valuexpath"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="buildDatalistItemDisplayValue"/>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:choose>
						<xslout:when test="$display-value">
							<xslout:copy-of select="$display-value/node()"/>
						</xslout:when>
						<xslout:otherwise>
							<xslout:copy-of select="{$new-xpath}/node()"/>
						</xslout:otherwise>
					</xslout:choose>
				</xsl:with-param>
			</xsl:call-template>
			<xsl:if test="@ektdesignns_datalist">
				<xsl:call-template name="endDatalistAccess"/>
			</xsl:if>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="processContent">
				<xsl:with-param name="xpath" select="$new-xpath"/>
				<xsl:with-param name="default" select="'text'"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- match fieldset/legend -->
	
<!-- higher priority than template above '*[@...]' -->
<xsl:template match="fieldset[@class='design_group']" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_fieldset">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="fieldset[@class='design_group']" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_fieldset">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_fieldset">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:call-template name="conditional">
		<xsl:with-param name="xpath" select="$new-xpath"/>
		<xsl:with-param name="content">
			<xsl:element name="div"> 
				<xsl:apply-templates select="@*">
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
				<xsl:call-template name="processContent">
					<xsl:with-param name="xpath" select="$new-xpath"/>
					<xsl:with-param name="default" select="'content'"/>
				</xsl:call-template>
			</xsl:element>
		</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<!-- remove legend tag of fieldset set as a 'group' -->
<xsl:template match="fieldset[@class='design_group']/legend"/>
<xsl:template match="fieldset[@class='design_group']/legend" mode="priority-0"/>

<!-- discard contentEditable attributes required for design mode -->

<xsl:template match="fieldset/@contenteditable"/>
<xsl:template match="fieldset/*/@contenteditable"/>


<!-- match button -->

<xsl:template match="button[not(@type) or @type='button' or @type='reset']"/>
<xsl:template match="button[not(@type) or @type='button' or @type='reset']" mode="priority-0"/>


<!-- match input type=button, etc -->

<xsl:template match="input[@type='button' or @type='reset']"/>
<xsl:template match="input[@type='button' or @type='reset']" mode="priority-0"/>


<!-- match input type=hidden -->

<xsl:template match="input[@type='hidden' and @ektdesignns_nodetype]"/>
<xsl:template match="input[@type='hidden' and @ektdesignns_nodetype]" mode="priority-0"/>


<!-- match input type=text -->

<xsl:template match="input[(not(@type) or @type='text') and @ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-text">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="input[(not(@type) or @type='text') and @ektdesignns_nodetype]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-text">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_input-text">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:call-template name="processContent">
		<xsl:with-param name="xpath" select="$new-xpath"/>
		<xsl:with-param name="default" select="'value'"/>
	</xsl:call-template>
</xsl:template>


<!-- match input type=radio|checkbox -->

<xsl:template match="input[(@type='radio' or @type='checkbox') and @ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-radio-checkbox">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="input[(@type='radio' or @type='checkbox') and @ektdesignns_nodetype]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-radio-checkbox">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_input-radio-checkbox">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:element name="{name()}">
		<xsl:attribute name="xml:space">default</xsl:attribute>
		<xsl:apply-templates select="@*[name()!='checked']">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<xsl:attribute name="disabled">disabled</xsl:attribute>
		<xsl:choose>
			<xsl:when test="@value and not(@value='true') and not(@value='on')">
				<xslout:if test="{$new-xpath}={ekext:xpathLiteralString(string(@value))}">
					<xslout:attribute name="checked">checked</xslout:attribute>
				</xslout:if>
			</xsl:when>
			<xsl:otherwise>
				<xslout:if test="{$new-xpath}='true' or {$new-xpath}='1'">
					<xslout:attribute name="checked">checked</xslout:attribute>
				</xslout:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:element>
</xsl:template>


<!-- match select/option -->

<xsl:template match="select[@ektdesignns_nodetype]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_select">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="select[@ektdesignns_nodetype]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_select">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_select">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>

	<xsl:call-template name="beginDatalistAccess"/>

	<xsl:choose>
		<xsl:when test="@multiple">
			<ul>
				<xsl:variable name="text-style">
					<xsl:call-template name="buildTextStyle"/>
				</xsl:variable>
				<xsl:if test="string-length($text-style) &gt; 0">
					<xsl:attribute name="style"><xsl:value-of select="$text-style"/></xsl:attribute>
				</xsl:if>
				<xslout:for-each select="{$new-xpath}"><!--  warning: won't work with @multiple attribute type -->
					<xslout:variable name="value" select="string(.)"/>
					<xsl:call-template name="buildDatalistItemDisplayValue"/>
					<li>
						<xslout:choose>
							<xslout:when test="$display-value">
								<xslout:copy-of select="$display-value/node()"/>
							</xslout:when>
							<xslout:otherwise>
								<xslout:copy-of select="./node()"/>
							</xslout:otherwise>
						</xslout:choose>
					</li>
				</xslout:for-each>
			</ul>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:variable name="value" select="string({$new-xpath})"/>
					<xsl:call-template name="buildDatalistItemDisplayValue"/>
					<xslout:choose>
						<xslout:when test="$display-value">
							<xslout:copy-of select="$display-value/node()"/>
						</xslout:when>
						<xslout:otherwise>
							<xslout:copy-of select="{$new-xpath}/node()"/>
						</xslout:otherwise>
					</xslout:choose>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>

	<xsl:call-template name="endDatalistAccess"/>

</xsl:template>


<!-- process select/option -->

<xsl:include href="template_datalist.xslt"/>
<xsl:include href="template_copyData.xslt"/>



<!-- item nodetype ====================================================================== -->


<!-- match input type=radio -->

<xsl:template match="input[@type='radio' and @ektdesignns_nodetype='item']" mode="enum">
	<xsl:param name="xpath"/>
	<xsl:param name="display-value" select="@value"/>
	<xsl:choose>
		<xsl:when test="@value">
			<xsl:if test="@ektdesignns_role='default'">
				<xslout:if test="not({$xpath})">
					<xsl:copy-of select="$display-value"/>
				</xslout:if>
			</xsl:if>
			<!--<xslout:if test="{$xpath}={ekext:xpathLiteralString(string(@value))}">
				<xsl:copy-of select="$display-value"/>
			</xslout:if>-->
		</xsl:when>
		<xsl:otherwise>
			<xslout:if test="not({$xpath})">
				<xsl:copy-of select="$display-value"/>
			</xslout:if>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- match input type=checkbox -->
<!-- No longer used
<xsl:template match="input[@type='checkbox' and @ektdesignns_nodetype='item']" mode="enum">
	<xsl:param name="xpath"/>
	<xsl:param name="display-value" select="@value"/>
	<xsl:choose>
		<xsl:when test="@value and contains($xpath,'@')"> <!- - node is an attribute - ->
			<xslout:if test="contains(concat(' ',{$xpath},' '),concat(' ',{ekext:xpathLiteralString(string(@value))},' '))"> 
				<li>
					<xsl:copy-of select="$display-value"/>
				</li>
			</xslout:if>
		</xsl:when>
		<xsl:when test="@value"> <!- - should be element only, but also applies to other node types as written - ->
			<xslout:if test="{$xpath}[.={ekext:xpathLiteralString(string(@value))}]"> 
				<li>
					<xsl:copy-of select="$display-value"/>
				</li>
			</xslout:if>
		</xsl:when>
		<xsl:otherwise>
			<xslout:if test="not({$xpath})">
				<li>
					<xsl:copy-of select="$display-value"/>
				</li>
			</xslout:if>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
-->

<!-- Section 508 unsupported attributes, remove them -->

<xsl:template match="th/@id[../@scope]|td/@id[../@scope]"/>
<xsl:template match="th/@scope|td/@scope"/>
<xsl:template match="td/@headers|th/@headers"/>


<!-- special case: list prototype  ======================================================= -->


<xsl:template match="tbody[@ektdesignns_list]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_tbody-list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="tbody[@ektdesignns_list]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_tbody-list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_tbody-list">
	<xsl:param name="xpath"/>
	
	<xsl:variable name="prototype_rtf"> <!-- result tree fragment -->
		<xsl:choose>
			<xsl:when test="../tfoot">
				<xsl:copy-of select="../tfoot/tr[2]"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="tr[@ektdesignns_nodetype][1]"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:variable name="prototype" select="msxsl:node-set($prototype_rtf)/*[1]"/>

	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
		<xsl:variable name="prototype-name">
			<xsl:for-each select="$prototype"> <!-- hack to set the context node -->
				<xsl:call-template name="getDesignName"/>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="template-xpath">
			<xsl:if test="@ektdesignns_listitem_template">
				<xsl:variable name="idref" select="@ektdesignns_listitem_template"/>
				<xsl:variable name="template" select="../tfoot//*[@id=$idref]"/>
				<!-- hack: use for-each to set the context node for call-template -->
				<xsl:for-each select="$template">
					<xsl:call-template name="buildXPath">
						<xsl:with-param name="xpath" select="''"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:if>
		</xsl:variable>
		<xsl:variable name="new-xpath" select="concat($xpath,$template-xpath,'/',$prototype-name)"/>
		<xslout:for-each select="{$new-xpath}">
			<xsl:element name="{name($prototype)}">
				<xsl:apply-templates select="$prototype/@*[name()!='id']">
					<!-- exclude id attribute -->
					<xsl:with-param name="xpath" select="'.'"/>
				</xsl:apply-templates>
				<xsl:apply-templates select="$prototype/node()">
					<xsl:with-param name="xpath" select="'.'"/>
				</xsl:apply-templates>
			</xsl:element>
		</xslout:for-each>
		<xsl:apply-templates select="tr[not(@ektdesignns_nodetype)]">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</xsl:element>
</xsl:template>


<!-- remove label tags -->

<xsl:template match="label">
	<xsl:param name="xpath"/>
	<xsl:apply-templates>
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:apply-templates>
	<xslout:text>&#160;</xslout:text> <!-- padding -->
</xsl:template>

<xsl:template match="label" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:apply-templates mode="priority-0">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:apply-templates>
	<xslout:text>&#160;</xslout:text> <!-- padding -->
</xsl:template>


<xsl:include href="template_xsltAttrVal.xslt"/>


<!-- remove prototypes, don't process -->
<xsl:template match="*[@class='design_prototype']"/>
<xsl:template match="*[@class='design_prototype']" mode="priority-0"/>

<!-- remove ektdesignns_ attributes -->
<xsl:template match="@*[starts-with(name(),'ektdesignns_')]"/>
<xsl:template match="@*[starts-with(name(),'ektdesignns_')]" mode="priority-0"/>

<!-- remove design-time only attributes -->
<xsl:template match="@class[.='show_design_border']"/>
<xsl:template match="@class[.='show_design_border']" mode="priority-0"/>

<!-- remove -->
<xsl:template match="ektdesignpackage_list"/>


<xsl:include href="template_identityToXSLT.xslt"/>
<xsl:include href="template_xpathLiteralString.xslt"/>

<!-- remove /html/body tags, which were mostly likely gratuitously added to make well-formed XML -->

<xsl:template match="/html[body and not(head)]">
	<xsl:param name="xpath"/>
	<xsl:apply-templates select="body/node()">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:apply-templates>
</xsl:template>


<!-- high priority templates ========================================================================== -->

<!-- Note: these template reapply templates using a different mode. It is not possible to use 
import/apply-imports because only template with the same match rule are applied. Also, the params
would be lost. There is no way to set the priority when applying templates, so we have to use
a different mode, which makes the templates themselves ugly, but I don't know of any other way to
do this. Ironically, a highly recursive processing language like XSLT can't easily recursively 
re-process. If the mode attribute could take a list of modes, it would resolve this problem.
-->

<!-- match ektdesignns_minoccurs='0' or ektdesignns_use='optional' -->

<xsl:template match="*[@ektdesignns_minoccurs='0' or @ektdesignns_use='optional']" priority="1">
	<xsl:param name="xpath"/>
	
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
						
	<xslout:if test="count({$new-xpath}) &gt; 0">
		<xsl:apply-templates select="." mode="priority-0">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</xslout:if>
</xsl:template>


<!-- match ektdesignns_maxoccurs='unbounded' -->

<!-- must have priority over minOccurs -->
<xsl:template match="*[(@ektdesignns_maxoccurs='unbounded' or number(@ektdesignns_maxoccurs) &gt; 1 or number(@ektdesignns_minoccurs) &gt; 1)
and name() != 'select' and name() != 'ektdesignns_checklist' and name() != 'ektdesignns_choices' and name() != 'ektdesignns_resource']" priority="1.1">
	<xsl:param name="xpath"/>
	
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>

	<xslout:for-each select="{$new-xpath}">
		<!-- prototype template -->
		<xsl:call-template name="buildPrototype">
			<xsl:with-param name="xpath" select="'..'"/>
		</xsl:call-template>
	</xslout:for-each>
</xsl:template>

<xsl:template name="buildPrototype">
	<xsl:param name="xpath"/>

	<div>
		<xsl:apply-templates select="." mode="priority-0">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</div>
</xsl:template>


<!-- templates ========================================================================== -->

<xsl:include href="template_getDesignName.xslt"/>
<xsl:include href="template_buildXPath.xslt"/>


<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
function getTextStyleProperties(style)
{
	// format of style: "property : value ;"
	// allows only: background*, color, font*, text*, word*
	var strStyle = "";
	var aryProps = style.split(";");
	var aryNameValue;
	for (var i = 0; i < aryProps.length; i++)
	{
		aryNameValue = aryProps[i].split(":");
		if (2 == aryNameValue.length)
		{
			if (aryNameValue[0].match(/\s*((background[\S]*)|color|(font[\S]*)|(text[\S]*)|(word[\S]*))\s*/i))
			{
				strStyle += aryProps[i] + ";"; 
			}
		}
	}
	return strStyle;
}
]]></msxsl:script>
-->
<xsl:template name="buildTextStyle">
	<xsl:param name="style" select="@style"/>
	<xsl:value-of select="ekext:getTextStyleProperties(string($style))"/>
</xsl:template>

<xsl:template name="applyTextStyle">
	<xsl:param name="style" select="@style"/>
	<xsl:param name="content"/>
	<xsl:variable name="text-style">
		<xsl:call-template name="buildTextStyle">
			<xsl:with-param name="style" select="$style"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:choose>
		<xsl:when test="string-length($text-style) &gt; 0">
			<span style="{$text-style}">
				<xsl:copy-of select="$content"/>
			</span>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="$content"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="conditional">
	<xsl:param name="xpath"/>
	<xsl:param name="condition" select="@ektdesignns_relevant"/>
	<xsl:param name="content"/>
	<xsl:choose>
		<xsl:when test="starts-with($condition,'xpath:')">
			<xslout:if test="{substring-after($condition,'xpath:')}">
				<xsl:copy-of select="$content"/>
			</xslout:if>
		</xsl:when>
		<xsl:when test="starts-with($condition,'xpathr:')">
			<!-- set $xpath to be the context node -->
			<!-- since $xpath is the conditional element, we can remove it but not add it -->
			<xslout:for-each select="{$xpath}[1]">
				<xslout:if test="{substring-after($condition,'xpathr:')}">
					<xsl:copy-of select="$content"/>
				</xslout:if>
			</xslout:for-each>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="$content"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- processContent -->

<xsl:template name="processContent">
	<xsl:param name="xpath"/>
	<xsl:param name="default">content</xsl:param>
	<xsl:variable name="contentType">
		<xsl:choose>
			<xsl:when test="@ektdesignns_content">
				<xsl:value-of select="@ektdesignns_content"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$default"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:choose>
	
		<xsl:when test="$contentType='choices'">
			<xsl:call-template name="beginDatalistAccess"/>
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="style" select="ol/@style"/>
				<xsl:with-param name="content">
					<xsl:for-each select="ol/li">
						<xsl:apply-templates select="input[@ektdesignns_role='default']" mode="enum">
							<xsl:with-param name="xpath" select="$xpath"/>
							<xsl:with-param name="display-value" select="label/node()"/>
						</xsl:apply-templates>
					</xsl:for-each>
					<xslout:variable name="value" select="string({$xpath})"/>
					<xsl:call-template name="buildDatalistItemDisplayValue"/>
					<xslout:choose>
						<xslout:when test="$display-value">
							<xslout:copy-of select="$display-value/node()"/>
						</xslout:when>
						<xslout:otherwise>
							<xslout:copy-of select="{$xpath}/node()"/>
						</xslout:otherwise>
					</xslout:choose>
				</xsl:with-param>
			</xsl:call-template>
			<xsl:call-template name="endDatalistAccess"/>
		</xsl:when>
		
		<xsl:when test="$contentType='checklist'">
			<xsl:call-template name="beginDatalistAccess"/>
			<ul>
				<xsl:variable name="text-style">
					<xsl:call-template name="buildTextStyle">
						<xsl:with-param name="style" select="ol/@style"/>
					</xsl:call-template>
				</xsl:variable>
				<xsl:if test="string-length($text-style) &gt; 0">
					<xsl:attribute name="style"><xsl:value-of select="$text-style"/></xsl:attribute>
				</xsl:if>
				<!--<xsl:for-each select="ol/li">
					<xsl:apply-templates select="input" mode="enum">
						<xsl:with-param name="xpath" select="$xpath"/>
						<xsl:with-param name="display-value" select="label/node()"/>
					</xsl:apply-templates>
				</xsl:for-each>-->
				<xslout:for-each select="{$xpath}">
					<xslout:variable name="value" select="string(.)"/>
					<xsl:call-template name="buildDatalistItemDisplayValue"/>
					<li>
						<xslout:choose>
							<xslout:when test="$display-value">
								<xslout:copy-of select="$display-value/node()"/>
							</xslout:when>
							<xslout:otherwise>
								<xslout:copy-of select="./node()"/>
							</xslout:otherwise>
						</xslout:choose>
					</li>
				</xslout:for-each>
			</ul>
			<xsl:call-template name="endDatalistAccess"/>
		</xsl:when>

		<xsl:when test="$contentType='date'">
			<!-- cannot define variable for date here because it is global and could be a duplicate definition  -->
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="style" select=".//input/@style"/>
				<xsl:with-param name="content">
				<xslout:if test="true()"> <!-- scope variables -->
					<xslout:variable name="date" select="{$xpath}"/> <!-- format: CCYY-MM-DD (ISO8601) -->
					<xslout:variable name="id" select="generate-id($date)"/>
					<span id="{{$id}}"><xslout:value-of select="$date"/></span>
					<script language="JavaScript" type="text/javascript" defer="defer"><xslout:comment xml:space="preserve">
ektLocalizeDate('<xslout:value-of select="$date"/>','<xslout:value-of select="$id"/>')
// </xslout:comment></script>
				</xslout:if>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:when>
		
		<xsl:when test="contains($xpath,'@')"> <!-- and $contentType!='date' and $contentType!='choices' and $contentType!='checklist' -->
			<!-- xpath is an attribute -->
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:value-of select="{$xpath}"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:when>
		
		<xsl:when test="$contentType='text'">
			<!-- text() will duplicate content if more than one of these in an element -->
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:value-of select="{$xpath}"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:when>
		
		<xsl:when test="$contentType='mixed'">
			<xsl:choose>
				<xsl:when test="contains($xpath,'/node()')">
					<xslout:copy-of select="{$xpath}"/>
				</xsl:when>
				<xsl:otherwise>
					<xslout:copy-of select="{$xpath}/node()"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="starts-with($contentType,'element=') and contains($contentType,'/@')">
			<!-- text() will duplicate content if more than one of these in an element -->
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:copy-of select="{concat($xpath,'/text()')}"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:when>
		
		<xsl:when test="starts-with($contentType,'element=')">
			<xslout:copy-of select="{concat($xpath,'/node()')}"/>
		</xsl:when>
		
		<xsl:when test="$contentType='textarea'">
			<!-- 
				Believe it or not, the line breaks in a text node whose element has 
				attributes are a single char matched by CR (&#xD;) or LF (&#xA;).
				But in an element with no attributes the line break matches the $newline variable, 
				but not both CR and LF chars separately.
				Confused? So am I.
			-->
			<!-- replace LF with <br /> for presentation view of a textarea -->
			<!--
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:call-template name="replace-substring">
						<xslout:with-param name="original" select="string({$xpath})"/>
						<xslout:with-param name="substring" select="'&#xA;'"/>
						<xslout:with-param name="replacement"><br /></xslout:with-param>
					</xslout:call-template>
				</xsl:with-param>
			</xsl:call-template>
			-->
			<!-- It's simpler and faster to wrap TEXTAREA content in PRE tags -->
			<!-- Avoid extra space b/n PRE tags and content -->
			<pre><xsl:variable name="text-style">
					<xsl:call-template name="buildTextStyle"/>
					</xsl:variable>
				<xsl:if test="string-length($text-style) &gt; 0">
					<xsl:attribute name="style"><xsl:value-of select="$text-style"/></xsl:attribute>
				</xsl:if><xslout:copy-of select="{concat($xpath,'/node()')}"/></pre>
		</xsl:when>
		
		<xsl:when test="$contentType='value'">
			<xsl:call-template name="applyTextStyle">
				<xsl:with-param name="content">
					<xslout:value-of select="{$xpath}"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:when>
		
		<xsl:otherwise>
			<!-- normal -->
			<xsl:apply-templates select="node()">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>
