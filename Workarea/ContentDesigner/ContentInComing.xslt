<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xhtml="http://www.w3.org/1999/xhtml" exclude-result-prefixes="msxsl xhtml">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="skinPath" select="''"/>
<xsl:param name="srcPath" select="''"/>
<xsl:param name="baseURL" select="''"/>
<xsl:param name="LangType" select="''"/>
<xsl:param name="mode" select="'design'"/>
<xsl:param name="sEditPropToolTip" select="'Edit Field:'"/>

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="name(node())='root'">
			<xsl:apply-templates select="*/node()" />
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="ektdesignpackage_forms">
	<xsl:apply-templates select="ektdesignpackage_form[1]/ektdesignpackage_designs/ektdesignpackage_design[1]/node()"/>
</xsl:template>

<!-- FIELDSET/DIV[@class='design_membrane'] -->
<xsl:template match="div[@class='design_membrane']">
	<div>
		<xsl:apply-templates select="@*"/>
		<xsl:choose>
			<!-- #38961 use count(node()) rather than string-length() to fix bug where empty
			richarea field would be deleted when within a fieldset -->
			<!-- xsl:when test="string-length(normalize-space(string(.)))=0" -->
			<xsl:when test="count(node())=0">
				<!-- a space is needed in the WYSIWYG view for user to click within the FIELDSET -->
				<xsl:text>&#160;</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates />
			</xsl:otherwise>
		</xsl:choose>
	</div>
</xsl:template>


<!-- Data Designer Custom Tags -->
<xsl:template match="ektdesignns_checklist|ektdesignns_choices|ektdesignns_richarea|ektdesignns_mergelist">
	<div class="{local-name()}" contenteditable="false">
		<xsl:apply-templates select="@*|node()" />
		<xsl:if test="local-name()='ektdesignns_checklist' or local-name()='ektdesignns_choices'">
			<!-- a text node is needed in the WYSIWYG view for user to select and delete the choices field -->
			<xsl:text>&#160;</xsl:text>
		</xsl:if>
	</div>
</xsl:template>
<xsl:template match="ektdesignns_calendar|ektdesignns_filelink|ektdesignns_imageonly|ektdesignns_mergefield|ektdesignns_resource">
	<span class="{local-name()}" contenteditable="false">
		<xsl:apply-templates select="@*|node()" />
	</span>
</xsl:template>
<xsl:template match="*[starts-with(name(),'ektdesignns_') and @ektdesignns_display='table']">
	<xsl:variable name="tagName" select="local-name()"/>
	<table class="{$tagName}" contenteditable="false">
		<xsl:apply-templates select="@*[starts-with(local-name(),'ektdesignns_')]"/>
		<caption>
			<xsl:value-of select="concat('&lt;',substring-after($tagName,'ektdesignns_'),'&gt;')"/>
		</caption>
		<thead>
			<xsl:for-each select="@*[not(starts-with(local-name(),'ektdesignns_'))]">
				<xsl:variable name="className" select="concat($tagName,'_',local-name())"/>
				<tr class="{$className}">
					<th scope="row" contenteditable="false">
						<xsl:value-of select="local-name()"/>
					</th>
					<td class="{$className}" contenteditable="false">
						<xsl:value-of select="."/>
					</td>
				</tr>
			</xsl:for-each>
		</thead>
		<tbody>
			<tr>
				<td colspan="2">
					<xsl:if test="$mode='design'">
						<xsl:attribute name="contenteditable">true</xsl:attribute>
					</xsl:if>
					<xsl:apply-templates select="node()"/>
				</td>
			</tr>		
		</tbody>
	</table>
</xsl:template>

<!-- remove extraneous DIV within LI, which is added by IE; also in ContentOutgoing.xslt -->
<xsl:template match="li[count(*)=1 and normalize-space(text())='' and div[count(@*)=0]]">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:apply-templates select="div/node()"/>
	</xsl:copy>
</xsl:template>

<xsl:template match="@src[starts-with(.,'[skinpath]')]">
	<xsl:attribute name="src"><xsl:value-of select="concat($skinPath,substring-after(.,'[skinpath]'))"/></xsl:attribute>
</xsl:template>
<xsl:template match="@background|@dynsrc|@href|@src">
	<xsl:copy-of select="."/>
	<xsl:attribute name="data-ektron-url"><xsl:value-of select="."/></xsl:attribute>
</xsl:template>
<xsl:template match="@data-ektron-url"/>

<!-- known xhtml tags (with closing tags) -->
<!-- known xhtml tags treated as placeholder: applet, embed, object, script, noembed, noscript -->
<xsl:template match="a|abbr|acronym|address|b|bdo|bgsound|big|blink|blockquote|button|
		caption|center|cite|code|colgroup|comment|dd|del|dfn|dir|div|dl|dt|em|fieldset|font|form|
		h1|h2|h3|h4|h5|h6|i|iframe|ins|kbd|label|legend|li|listing|map|marquee|menu|nobr|
		ol|optgroup|option|p|plaintext|pre|q|rb|rbc|rp|rt|rtc|ruby|s|samp|select|small|span|strike|strong|style|sub|sup|
		table|tbody|td|textarea|tfoot|th|thead|tr|tt|u|ul|var|wbr|xml|xmp">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

<xsl:template match="p" priority="1.0">
	<xsl:copy>
		<xsl:apply-templates select="@*" />
		<xsl:attribute name="data-ektron-preserve">true</xsl:attribute>
		<xsl:apply-templates select="node()" />
	</xsl:copy>
</xsl:template>

<xsl:template match="select" priority="1.0">
	<xsl:call-template name="getFieldPropertyButton"/>
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

<!-- identity template -->
<!-- identity with closing tags -->
<xsl:template match="@*|node()" priority="-0.5">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

<!-- See similar templates for identity without closing tags -->
<!-- identity without closing tags -->
<xsl:template match="area[not(node())]|base[not(node())]|basefont[not(node())]|bgsound[not(node())]|
				br[not(node())]|col[not(node())]|frame[not(node())]|hr[not(node())]|
				img[not(node())]|input[not(node())]|isindex[not(node())]|keygen[not(node())]|
				link[not(node())]|meta[not(node())]|param[not(node())]">
	<xsl:copy>
		<xsl:apply-templates select="@*" />
	</xsl:copy>
</xsl:template>

<xsl:template match="input[@type='checkbox']" priority="1.0">
	<xsl:call-template name="getFieldPropertyButton"/>
	<xsl:copy>
		<xsl:apply-templates select="@*" />
	</xsl:copy>
</xsl:template>

<xsl:template match="input[@type='hidden']" priority="1.0">
	<xsl:choose>
		<xsl:when test="ancestor::*[@contenteditable='true'] or $mode='design'">
			<input>
				<xsl:apply-templates select="@*"/>
				<xsl:attribute name="type">text</xsl:attribute>
				<xsl:attribute name="ektdesignns_hidden">true</xsl:attribute>
			</input>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="input[(not(@type) or @type='text') and @ektdesignns_name]|textarea[@ektdesignns_name]" priority="1.0">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:if test="not(contains(@class, 'design_textfield'))">
			<xsl:attribute name="class">
				<xsl:value-of select="normalize-space(concat(@class,' design_textfield'))"/>
			</xsl:attribute>
		</xsl:if>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
</xsl:template>

<!-- 
	#35383 - object expected error when clicking in a certain location in the editor
	Problem is authored JavaScript event handlers running and causing JavaScript errors.
	Changed name of all "on" JavaScript event handler attributes to prevent them from executing.
	Changed back when content is saved.
	source: EkRadEditor.js, ContentInComing.xslt
	version: 7.5.3.09
	changelist: 35251
	See also #37527 and #37532
-->
<xsl:template match="@*[starts-with(name(),'on') and not(starts-with(.,'design_'))]">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<xsl:attribute name="ektron35383_{name()}">
			<xsl:value-of select="." />
		</xsl:attribute>
	</xsl:if>
</xsl:template>

<!-- placeholder tags -->

<xsl:template match="object[starts-with(@type, 'application/x-silverlight')]">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<xsl:variable name="title">
			<xsl:choose>
				<xsl:when test="@title">
					<xsl:value-of select="@title"/>
				</xsl:when>
				<xsl:when test="embed/@title">
					<xsl:value-of select="embed/@title"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="'Silverlight'"/>
					<xsl:if test="@id">
						<xsl:value-of select="concat(' id=',@id)"/>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<img class="design_silverlight"
			alt="{$title}" title="{$title}"
			data-ektron-placeholder="silverlight">
			<xsl:attribute name="src">
				<xsl:value-of select="concat($skinPath,'silverlight.png')"/>
			</xsl:attribute>
			<xsl:call-template name="getPlaceholderAttributes"/>
			<xsl:for-each select="param">
				<xsl:attribute name="data-param-{@name}">
					<xsl:value-of select="@value"/>
				</xsl:attribute>
			</xsl:for-each>
			<xsl:for-each select="a/@*">
				<xsl:attribute name="data-a-{local-name()}">
					<xsl:value-of select="."/>
				</xsl:attribute>
			</xsl:for-each>	
			<xsl:for-each select="a[img/@*]">
				<xsl:attribute name="data-a_img-src">
					<xsl:value-of select="node()/@src"/>
				</xsl:attribute>
				<xsl:attribute name="data-a_img-alt">
					<xsl:value-of select="node()/@alt"/>
				</xsl:attribute>
				<xsl:attribute name="data-a_img-style">
					<xsl:value-of select="node()/@style"/>
				</xsl:attribute>
			</xsl:for-each>	
		</img>
	</xsl:if>
</xsl:template>

<xsl:template match="a[img and contains(@onclick, 'MyImage')]">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<img class="ektron_thumbnail" data-ektron-placeholder="thumbnail" src="{img/@src}">
			<xsl:call-template name="getPlaceholderAttributes"/>
			<xsl:for-each select="img/@*">
				<xsl:attribute name="data-img-{local-name()}">
					<xsl:value-of select="."/>
				</xsl:attribute>
				<xsl:attribute name="{local-name()}">
					<xsl:value-of select="."/>
				</xsl:attribute>
			</xsl:for-each>
		</img>  
	</xsl:if>
</xsl:template>	
	
<xsl:template match="script[comment()]">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<textarea class="design_fixedsize_placeholder design_placeholder_script" data-placeholder="script" title="JavaScript">
			<xsl:if test="@id">
				<xsl:attribute name="data-attr-id">
					<xsl:value-of select="@id"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:variable name="text" select="comment()"/>
			<xsl:choose>
				<xsl:when test="'// '=substring($text, string-length($text) - 2, 3)">
					<xsl:value-of select="substring($text, 1, string-length($text) - 3)" disable-output-escaping="yes"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$text" disable-output-escaping="yes"/>
				</xsl:otherwise>
			</xsl:choose>
		</textarea>
	</xsl:if>
</xsl:template>

<xsl:template match="embed[@type='application/x-mplayer2']">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<img class="design_wmv" width="{@width}" height="{@height}" 
			data-ektron-placeholder="wmv" data-ektron-wmv-src="{@src}" 
			src="{$skinPath}wmv.png" alt="{@title}" title="{@title}" />
	</xsl:if>
</xsl:template>

<xsl:template match="object[embed/@type='application/x-shockwave-flash']">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<xsl:variable name="title">
			<xsl:choose>
				<xsl:when test="@title">
					<xsl:value-of select="@title"/>
				</xsl:when>
				<xsl:when test="embed/@title">
					<xsl:value-of select="embed/@title"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="'Flash'"/>
					<xsl:if test="@id">
						<xsl:value-of select="concat(' id=',@id)"/>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<img class="design_flash" data-ektron-placeholder="flash" data-placeholder="object"
			alt="{$title}" title="{$title}">
			<xsl:choose>
				<xsl:when test="starts-with(embed/@src,'http://www.youtube.com')">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($skinPath,'youtube.png')"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($skinPath,'flash.png')"/>
					</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:call-template name="getPlaceholderAttributes"/>
			<xsl:for-each select="param">
				<xsl:attribute name="data-param-{@name}">
					<xsl:value-of select="@value"/>
				</xsl:attribute>
			</xsl:for-each>
			<xsl:for-each select="embed/@*">
				<xsl:attribute name="data-embed-{local-name()}">
					<xsl:value-of select="."/>
				</xsl:attribute>
			</xsl:for-each>
		</img>
	</xsl:if>
</xsl:template>

<xsl:template match="object[@class='BrightcoveExperience']">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<xsl:variable name="title">
			<xsl:value-of select="@id"/>
		</xsl:variable>
		<img class="design_brightcove"
			alt="{$title}" title="{$title}"
			data-ektron-placeholder="brightcove">
			<xsl:attribute name="src">
				<xsl:value-of select="concat($skinPath,'brightcove.png')"/>
			</xsl:attribute>
			<xsl:call-template name="getPlaceholderAttributes"/>
			<xsl:for-each select="param">
				<xsl:variable name="paramName">
					<xsl:call-template name="string-replace-all">
						<xsl:with-param name="text" select="@name" />
						<xsl:with-param name="replace" select="'@'" />
						<xsl:with-param name="by" select="'_ektat_'" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:attribute name="data-param-{$paramName}">
					<xsl:value-of select="@value"/>
				</xsl:attribute>
			</xsl:for-each>
		</img>
	</xsl:if>
</xsl:template>
<xsl:template name="string-replace-all">
	<xsl:param name="text" />
	<xsl:param name="replace" />
	<xsl:param name="by" />
	<xsl:choose>
		<xsl:when test="contains($text, $replace)">
			<xsl:value-of select="substring-before($text,$replace)" />
			<xsl:value-of select="$by" />
			<xsl:call-template name="string-replace-all">
				<xsl:with-param name="text" select="substring-after($text,$replace)" />
				<xsl:with-param name="replace" select="$replace" />
				<xsl:with-param name="by" select="$by" />
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$text" />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="embed[@type='application/x-shockwave-flash']">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<xsl:variable name="title">
			<xsl:choose>
				<xsl:when test="@title">
					<xsl:value-of select="@title"/>
				</xsl:when>
				<xsl:when test="@id">
					<xsl:value-of select="concat('Flash id=',@id)"/>
				</xsl:when>
				<xsl:when test="@name">
					<xsl:value-of select="concat('Flash name=',@name)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="'Flash'"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<img class="design_flash" data-ektron-placeholder="flash" data-placeholder="embed"
			alt="{$title}" title="{$title}">
			<xsl:choose>
				<xsl:when test="starts-with(@src,'http://www.youtube.com')">
					<xsl:attribute name="src">
						<xsl:value-of select="concat($skinPath,'youtube.png')"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($skinPath,'flash.png')"/>
					</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:call-template name="getPlaceholderAttributes"/>
		</img>
	</xsl:if>
</xsl:template>

<xsl:template match="*" priority="-0.25">
	<xsl:if test="ancestor::*[@contenteditable='true'] or $mode='design'">
		<xsl:variable name="title">
			<xsl:value-of select="name()"/>
			<xsl:if test="@id">
				<xsl:value-of select="concat(' id=',@id)"/>
			</xsl:if>
			<xsl:if test="@src">
				<xsl:value-of select="concat(' ',@src)"/>
			</xsl:if>
			<xsl:if test="@href">
				<xsl:value-of select="concat(' ',@href)"/>
			</xsl:if>
		</xsl:variable>
		<xsl:variable name="id" select="concat('ektdesignns_placeholder_',generate-id())"/>
		<img src="{$skinPath}transparent.gif" border="1"
			alt="{$title}" title="{$title}" 
			data-placeholder="{local-name()}">
			<xsl:choose>
				<xsl:when test="@width or @height">
					<xsl:attribute name="class">
						<xsl:value-of select="concat('design_resizable_placeholder design_placeholder_',local-name())"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">
						<xsl:value-of select="concat('design_fixedsize_placeholder design_placeholder_',local-name())"/>
					</xsl:attribute>
					<xsl:attribute name="unselectable">on</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:if test="starts-with(name(),'asp:') or starts-with(name(),'cms:')">
				<xsl:attribute name="data-placeholder"><xsl:value-of select="name()"/></xsl:attribute>
			</xsl:if>
			<xsl:if test="*">
				<xsl:attribute name="id"><xsl:value-of select="$id"/></xsl:attribute>
			</xsl:if>
			<xsl:call-template name="getPlaceholderAttributes"/>
			<xsl:if test="node() and not(*)">
				<xsl:attribute name="data-text">
					<xsl:value-of select="node()"/>
				</xsl:attribute>
			</xsl:if>
		</img>
		<xsl:if test="*">
			<xsl:element name="{$id}">
				<xsl:copy-of select="node()"/>
			</xsl:element>
		</xsl:if>
	</xsl:if>
</xsl:template>

<xsl:template name="getPlaceholderAttributes">
	<xsl:if test="@height">
		<xsl:attribute name="height">
			<xsl:value-of select="@height"/>
		</xsl:attribute>
	</xsl:if>
	<xsl:if test="@width">
		<xsl:attribute name="width">
			<xsl:value-of select="@width"/>
		</xsl:attribute>
	</xsl:if>
	<xsl:for-each select="@*">
		<xsl:attribute name="{concat('data-attr-',local-name())}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:for-each>
</xsl:template>

<!-- #55836: FF does not preserve style attribute in image -->
<xsl:template match="img[@style]">
	<img>
		<xsl:attribute name="data-ektron-style">
			<xsl:value-of select="@style"/>
		</xsl:attribute>
		<xsl:apply-templates select="@*"/>
	</img>
</xsl:template>

<!--field property button for SELECT and INPUT[@type='checkbox']-->
<xsl:template name="getFieldPropertyButton" >
	<xsl:if test="$mode='design' and @ektdesignns_name">
		<xsl:variable name="tooltip" select="concat($sEditPropToolTip, ' ', @ektdesignns_caption)"/>
		<span class="design_edit_fieldprop" data-ektron-forfield="{@id}" title="{$tooltip}" alt="{$tooltip}" unselectable="on" contenteditable="false" />
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
