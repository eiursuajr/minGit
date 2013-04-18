<?xml version='1.0'?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="js dl xslout msxslout" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:xslout="alias" xmlns:msxslout="aliasms" xmlns:dl="urn:datalist">

<!--
	Requires: 

	<xsl:stylesheet xmlns:msxslout="aliasms">
	<xsl:namespace-alias stylesheet-prefix="msxslout" result-prefix="msxsl"/>

	<xsl:param name="fieldlistXPath"/>

	<xsl:template match="/">
		<xslout:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl js dl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:dl="urn:datalist">

-->

<xsl:template name="beginDatalistAccess">
	<xsl:param name="datalistXPath"/>
	<xsl:text disable-output-escaping="yes">
	&lt;xsl:if test="true()" </xsl:text><!-- scope variables in xsl:if to avoid duplicate defn -->
	<xsl:if test="@ektdesignns_xslt">
		<xsl:text>xml:space="default" </xsl:text>
	</xsl:if>
	<xsl:value-of select="@ektdesignns_datanamespaces"/>
	<xsl:text disable-output-escaping="yes">&gt;
	</xsl:text>
	<xsl:call-template name="buildDatalistAccessor">
		<xsl:with-param name="datalistXPath" select="$datalistXPath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="endDatalistAccess">
	<xsl:text disable-output-escaping="yes">
	&lt;/xsl:if&gt;
	</xsl:text>
</xsl:template>

<xsl:template match="option" mode="dl">
	<xsl:if test="not(@disabled) and (position() &gt; 1 or ancestor::select[not(@ektdesignns_validation='select-req') or @size &gt; 1])">
		<xsl:variable name="value">
			<xsl:choose>
				<xsl:when test="@value">
					<xsl:value-of select="@value"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="."/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<item value="{$value}">
			<xsl:value-of select="."/>
			<!--<xsl:call-template name="copyData">
				<xsl:with-param name="data" select="node()"/>
			</xsl:call-template>-->
		</item>
	</xsl:if>
</xsl:template>

<xsl:template match="input[@ektdesignns_nodetype='item']" mode="dl">
	<xsl:param name="display-value" select="@value"/>
	<item value="{@value}">
		<xsl:for-each select="$display-value">
			<xsl:value-of select="."/>
		</xsl:for-each>
		<!--<xsl:copy-of select="$display-value"/>-->
	</item>
</xsl:template>

<xsl:template name="getDatalistName">
	<xsl:choose>
		<xsl:when test="@ektdesignns_datalist">
			<xsl:value-of select="@ektdesignns_datalist"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="generate-id()"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="buildDatalists">
	<xslout:param name="baseURL" select="''"/>
	<xslout:param name="LangType" select="''"/>

	<xsl:for-each select="/*/ektdesignpackage_list/fieldlist/datalist">
		<xsl:choose>
			<!-- key name is used in variable datalistKey defined elsewhere -->
			<xsl:when test="@ektdesignns_captionxpath='.'">
				<!-- <xslout:key name="{@name}" match="{@ektdesignns_dataselect}" use="{@ektdesignns_valuexpath}" {@ektdesignns_datanamespaces}/> -->
				<!-- use this technique to output datanamespace -->
				<xsl:text disable-output-escaping="yes">
  &lt;xsl:key name="</xsl:text>
				<xsl:value-of select="@name"/>
				<xsl:text disable-output-escaping="yes">" match="</xsl:text>
				<xsl:value-of select="@ektdesignns_dataselect"/>
				<xsl:text disable-output-escaping="yes">" use="</xsl:text>
				<xsl:value-of select="@ektdesignns_valuexpath"/>
				<xsl:text disable-output-escaping="yes">" </xsl:text>
				<xsl:value-of select="@ektdesignns_datanamespaces"/>
				<xsl:text disable-output-escaping="yes">/&gt;
</xsl:text>
			</xsl:when>
			<xsl:when test="@ektdesignns_captionxpath">
				<!-- <xslout:key name="{@name}" match="{@ektdesignns_dataselect}/{@ektdesignns_captionxpath}" use="ancestor-or-self::*/{@ektdesignns_valuexpath}" {@ektdesignns_datanamespaces}/> -->
				<!-- use this technique to output datanamespace -->
				<xsl:text disable-output-escaping="yes">
  &lt;xsl:key name="</xsl:text>
				<xsl:value-of select="@name"/>
				<xsl:text disable-output-escaping="yes">" match="</xsl:text>
				<xsl:value-of select="concat(@ektdesignns_dataselect,'/',@ektdesignns_captionxpath)"/>
				<xsl:text disable-output-escaping="yes">" use="ancestor-or-self::*/</xsl:text>
				<xsl:value-of select="@ektdesignns_valuexpath"/>
				<xsl:text disable-output-escaping="yes">" </xsl:text>
				<xsl:value-of select="@ektdesignns_datanamespaces"/>
				<xsl:text disable-output-escaping="yes">/&gt;
</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xslout:key name="{@name}" match="datalist[@name='{@name}']/item" use="@value"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:for-each>

	<xsl:if test="not($fieldlistXPath)">
		<!-- datalists -->
		<xsl:variable name="datalists_rtf">
			<root>
				<xsl:for-each select="//select[@ektdesignns_nodetype and not(@ektdesignns_datasrc)]">
					<xsl:variable name="datalistName">
						<xsl:call-template name="getDatalistName"/>
					</xsl:variable>
					<datalist name="{$datalistName}">
						<xsl:apply-templates select="descendant::option" mode="dl"/>
					</datalist>
				</xsl:for-each>
				<xsl:for-each select="//ektdesignns_choices[not(@ektdesignns_datasrc)] | //ektdesignns_checklist[not(@ektdesignns_datasrc)]">
					<xsl:variable name="datalistName">
						<xsl:call-template name="getDatalistName"/>
					</xsl:variable>
					<datalist name="{$datalistName}">
						<xsl:for-each select="ol/li">
							<xsl:apply-templates select="input" mode="dl">
								<xsl:with-param name="display-value" select="label/node()"/>
							</xsl:apply-templates>
						</xsl:for-each>
					</datalist>
				</xsl:for-each>
			</root>
		</xsl:variable>
		<xsl:variable name="datalists" select="msxsl:node-set($datalists_rtf)/*"/>
		<xsl:if test="count($datalists/datalist) &gt; 0">
			<dl:root>
				<!-- Remove duplicates -->
				<xsl:copy-of select="$datalists/datalist[not(@name=preceding-sibling::datalist/@name)]"/>
			</dl:root>
		</xsl:if>
	</xsl:if>

	<xslout:template name="safeUri">
		<xslout:param name="uri" select="''"/>
		<!-- append LangType, if given -->
		<xslout:variable name="uri-lang">
			<xslout:choose>
				<xslout:when test="string-length($LangType) &gt; 0 and contains($uri,'&amp;LangType=-1')">
					<xslout:value-of select="substring-before($uri,'&amp;LangType=-1')"/>
					<xslout:value-of select="concat('&amp;LangType=',$LangType)"/>
					<xslout:value-of select="substring-after($uri,'&amp;LangType=-1')"/>
				</xslout:when>
				<xslout:when test="contains($uri,'&amp;LangType=-1')">
					<xslout:value-of select="substring-before($uri,'&amp;LangType=-1')"/>
					<xslout:value-of select="substring-after($uri,'&amp;LangType=-1')"/>
				</xslout:when>
				<xslout:otherwise>
					<xslout:value-of select="$uri"/>
				</xslout:otherwise>
			</xslout:choose>
		</xslout:variable>
		<!-- prepend baseURL -->
		<xslout:choose>
			<xslout:when test="starts-with($uri-lang,'//') or contains($uri-lang,':')">
				<xslout:value-of select="$uri-lang"/>
			</xslout:when>
			<xslout:when test="starts-with($uri-lang,'/') or substring($baseURL,string-length($baseURL),1)='/'">
				<xslout:value-of select="concat($baseURL,$uri-lang)"/>
			</xslout:when>
			<xslout:otherwise>
				<xslout:value-of select="concat($baseURL,'/',$uri-lang)"/>
			</xslout:otherwise>
		</xslout:choose>
	</xslout:template>

	<!--
	<xslout:template name="safeUri">
		<xslout:param name="uri" select="''"/>
		<xslout:choose>
			<xslout:when test="function-available('js:safeUri')">
				<xslout:value-of select="js:safeUri(string($baseURL),$uri,string($LangType))"/>
			</xslout:when>
			<xslout:otherwise>
				<xslout:value-of select="concat($baseURL,$uri)"/>
			</xslout:otherwise>
		</xslout:choose>
	</xslout:template>

	<msxslout:script language="JavaScript" implements-prefix="js">
		<xsl:text disable-output-escaping="yes">&lt;![CDATA[
	function safeUri(base, uri, langType)
	{
		try
		{
			if (langType.length &gt; 0)
			{
				langType = "&amp;LangType=".concat(langType); // can't use '+' b/c .NET 2.0 parser errors on ConcatString type
			}
			uri = uri.replace("&amp;LangType=-1", langType);
			if (uri.substr(0,2) != "//" &amp;&amp; uri.indexOf(":") == -1)
			{
				if (uri.charAt(0) != "/" &amp;&amp; base.charAt(base.length - 1) != "/")
				{
					uri = base.concat("/", uri); // can't use '+' b/c .NET 2.0 parser errors on ConcatString type
				}
				else
				{
					uri = base.concat(uri); // can't use '+' b/c .NET 2.0 parser errors on ConcatString type
				}
			}
			var xmlDoc = null;
			try
			{
				xmlDoc = new ActiveXObject("Msxml2.DOMDocument.6.0");
	            //xmlDoc.setProperty("AllowDocumentFunction", true);
	            //xmlDoc.setProperty("AllowXsltScript", true);
	            xmlDoc.setProperty("ProhibitDTD", false);
			}
			catch (ex)
			{
				xmlDoc = new ActiveXObject("Msxml2.DOMDocument.4.0");
			}
			xmlDoc.async = false;
			xmlDoc.load(uri); // this may be slow, but ensures document() won't error
			var objErr = xmlDoc.parseError;
			if (objErr.errorCode != 0) 
			{
				return "";
			} 
			else 
			{
				return uri;
			}
		}
		catch (e)
		{
			return uri;
		}
	}
	]]&gt;</xsl:text>
	</msxslout:script>
	-->
	
</xsl:template>

<xsl:template name="buildDatalistAccessor">
	<xsl:param name="datalistXPath"/>
	<xsl:choose>
		<xsl:when test="@ektdesignns_datasrc and @ektdesignns_dataselect">
			<xslout:variable name="dl-url">
				<xslout:call-template name="safeUri">
					<xslout:with-param name="uri" select="'{@ektdesignns_datasrc}'"/>
				</xslout:call-template>
			</xslout:variable>
			<xslout:variable name="dl" select="document(normalize-space($dl-url)){@ektdesignns_dataselect}"/>
			<xslout:variable name="datalistKey" select="'{@ektdesignns_datalist}'"/>
		</xsl:when>
		<xsl:when test="$fieldlistXPath">
			<xsl:variable name="fieldName">
				<xsl:call-template name="getDesignName"/>
			</xsl:variable>
			<xslout:variable name="datalistName" select="{$fieldlistXPath}/field[@name='{$fieldName}']/@datalist"/>
			<xslout:variable name="dl" select="{$fieldlistXPath}/datalist[@name=$datalistName]/item"/>
			<xslout:variable name="datalistKey" select="$datalistName"/>
		</xsl:when>
		<xsl:when test="$datalistXPath">
			<xsl:variable name="datalistName">
				<xsl:call-template name="getDatalistName"/>
			</xsl:variable>
			<xslout:variable name="dl" select="{$datalistXPath}/datalist[@name='{$datalistName}']/item"/>
			<xslout:variable name="datalistKey" select="'{$datalistName}'"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:variable name="datalistName">
				<xsl:call-template name="getDatalistName"/>
			</xsl:variable>
			<xslout:variable name="dl" select="document('')/*/dl:*/datalist[@name='{$datalistName}']/item"/>
			<xslout:variable name="datalistKey" select="'{$datalistName}'"/>
		</xsl:otherwise>
	</xsl:choose>
	<xslout:variable name="datalist" select="$dl"/>
</xsl:template>

<xsl:template name="buildDatalistItemDisplayValue">
	<xsl:param name="valueName" select="'$value'"/>
	<xsl:param name="captionxpath">
		<xsl:call-template name="getCaptionXPath"/>
	</xsl:param>
	<xsl:param name="valuexpath">
		<xsl:call-template name="getValueXPath"/>
	</xsl:param>
	<xslout:variable name="display-value" select="($dl[{$valuexpath}={$valueName}])[1]/{$captionxpath}"/>
</xsl:template>

<xsl:template name="getCaptionXPath">
	<xsl:choose>
		<xsl:when test="@ektdesignns_captionxpath">
			<xsl:value-of select="@ektdesignns_captionxpath"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="'.'"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="getValueXPath">
	<xsl:choose>
		<xsl:when test="@ektdesignns_valuexpath">
			<xsl:value-of select="@ektdesignns_valuexpath"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="'@value'"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>