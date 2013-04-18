<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:xhtml="http://www.w3.org/1999/xhtml" exclude-result-prefixes="msxsl js xhtml">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="skinPath" select="''"/>
<xsl:param name="srcPath" select="''"/>
<xsl:param name="baseURL" select="''"/>
<xsl:param name="LangType" select="''"/>

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="name(node())='root'">
			<xsl:apply-templates select="*/node()" />
		</xsl:when>
		<xsl:when test="name(node())='p' and p[not(@*)]">
			<xsl:apply-templates select="p/node()" />
		</xsl:when>
		<xsl:otherwise>
			<xsl:apply-templates />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="input[@ektdesignns_hidden='true']" priority="1.0">
	<input>
		<xsl:apply-templates select="@*[name()!='ektdesignns_hidden']"/>
		<xsl:if test="@type='text'">
			<xsl:attribute name="type">hidden</xsl:attribute>
		</xsl:if>
	</input>
</xsl:template>

<xsl:template match="a[@name]/@class[contains(.,'design_bookmark')]" priority="1.0">
	<xsl:choose>
		<xsl:when test="normalize-space(.)='design_bookmark'">
			<!-- discard attribute -->
		</xsl:when>
		<xsl:otherwise>
			<xsl:attribute name="class">
				<xsl:value-of select="normalize-space(substring-before(.,'design_bookmark'))"/>
				<xsl:value-of select="normalize-space(substring-after(.,'design_bookmark'))"/>
			</xsl:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="iframe[@class='design_cache']"/>
<xsl:template match="div[@class='design_iframe_membrane']">
	<xsl:apply-templates/>
</xsl:template>
<xsl:template match="iframe[@class='contenteditable']"/>
    
<!-- Data Designer Custom Tags -->
<xsl:template match="div[@class='ektdesignns_richarea']">
	<xsl:element name="{@class}">
		<xsl:apply-templates select="@*[name()!='class' and name()!='contenteditable']" />
		<xsl:choose>
			<xsl:when test="count(*)=1 and p[not(@*)] and normalize-space(text())='' and not(@ektdesignns_datadisplay='block')">
				<xsl:apply-templates select="p/node()" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="node()" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:element>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>
<xsl:template match="div[@class='ektdesignns_checklist' or @class='ektdesignns_choices' or @class='ektdesignns_mergelist']">
	<xsl:element name="{@class}">
		<xsl:apply-templates select="@*[name()!='class' and name()!='contenteditable']" />
		<xsl:apply-templates select="node()" />
	</xsl:element>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>
<xsl:template match="span[@class='ektdesignns_calendar' or @class='ektdesignns_filelink' or @class='ektdesignns_imageonly' or @class='ektdesignns_mergefield' or @class='ektdesignns_resource']">
	<xsl:element name="{@class}">
		<xsl:apply-templates select="@*[name()!='class' and name()!='contenteditable']" />
		<xsl:if test="@datavalue"> <!-- copy datavalue to value attribute for backward compatibility -->
			<xsl:attribute name="value">
				<xsl:value-of select="@datavalue"/>
			</xsl:attribute>
		</xsl:if>
		<xsl:apply-templates select="node()" />
	</xsl:element>
</xsl:template>
<xsl:template match="table[starts-with(@class,'ektdesignns_')]">
	<xsl:element name="{@class}">
		<xsl:attribute name="ektdesignns_display">table</xsl:attribute>
		<xsl:variable name="attrNamePrefix" select="concat(@class,'_')"/>
		<xsl:for-each select="thead/tr/td[starts-with(@class,$attrNamePrefix)]">
			<xsl:attribute name="{substring-after(@class,$attrNamePrefix)}">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:for-each>
		<xsl:text>&#13;&#10;</xsl:text>
		<xsl:apply-templates select="tbody/tr/td/node()" />
	</xsl:element>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<xsl:template match="li/input[@ektdesignns_nodetype='item']/@id | li[input[@ektdesignns_nodetype='item']]/label/@for">
	<xsl:attribute name="{name()}">
		<xsl:value-of select="generate-id(../..)"/>
	</xsl:attribute>
</xsl:template>

<xsl:template match="@ektdesignns_datasrc">
	<xsl:attribute name="{name()}">
		<xsl:choose>
			<xsl:when test="contains(.,'[srcpath]')">
				<xsl:value-of select="concat($srcPath,substring-after(.,'[srcpath]'))"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:attribute>
</xsl:template>

<xsl:template match="@class[.='show_design_border']"/>
<xsl:template match="@class[.='design_placeholder-hidden']"/>

<!-- Remove content inserted by Ektron.SelectionRange.ensureContentUsability -->
<!-- Remove contentUsability paragraphs and inline span (which have nbsp). -->
<xsl:template match="p[contains(@class,'contentUsability')]">
	<xsl:if test="not(text()='&#160;' and count(node())=1)">
		<p>
			<xsl:apply-templates select="@*[name()!='class']"/>
			<xsl:apply-templates select="node()"/>
		</p>
	</xsl:if>
</xsl:template>
<xsl:template match="span[contains(@class,'contentUsability')]">
	<xsl:if test="not(text()='&#160;' and count(node())=1)">
		<xsl:apply-templates select="node()"/>
	</xsl:if>
</xsl:template>
<xsl:template match="p/node()[position()!=1 and position()=last() and name()='' and .='&#160;']"/>
<xsl:template match="div/node()[position()!=1 and position()=last() and name()='' and .='&#160;']"/>
<xsl:template match="li/node()[position()!=1 and position()=last() and name()='' and .='&#160;']"/>
<xsl:template match="th/node()[position()!=1 and position()=last() and name()='' and .='&#160;']"/>
<xsl:template match="td/node()[position()!=1 and position()=last() and name()='' and .='&#160;']"/>
<xsl:template match="@data-ektron-preserve"/>

<!-- These templates (w/ diff mode) are in ContentOutGoing.xslt and template_richMode.xslt -->
<!-- defect #34900 -->
<xsl:template match="br[@class='khtml-block-placeholder']"/>
<!-- defect #34779 -->
<xsl:template match="br[@_moz_editor_bogus_node]"/>
<xsl:template match="@*[starts-with(name(),'_moz')]"/>
<xsl:template match="*[name()='p' or name()='div' or name()='h1' or name()='h2' or name()='h3' or name()='h4' or name()='h5' or name()='h6' or name()='th' or name()='td' or name()='blockquote' or name()='li']/br[not(following-sibling::node()[not(contains(@class,'contentUsability'))]) and name((preceding-sibling::node())[last()])!='br']"/>

<!-- remove extraneous DIV within LI, which is added by IE; also in ContentInComing.xslt -->
<xsl:template match="li[count(*)=1 and normalize-space(text())='' and div[count(@*)=0]]">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:apply-templates select="div/node()"/>
	</xsl:copy>
</xsl:template>

<!-- identity template -->
<!-- identity with closing tags -->
<xsl:template match="@*|node()">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()" />
	</xsl:copy>
</xsl:template>

<!-- elements without closing tags -->
<xsl:template match="area|br|img|input">
    <xsl:copy>
    	<xsl:apply-templates select="@*"/>
    </xsl:copy>
</xsl:template>
<!-- line break after these tags -->
<xsl:template match="base|basefont|col|frame|hr|isindex|keygen|link|meta|param">
    <xsl:copy>
    	<xsl:apply-templates select="@*"/>
    </xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<!-- line break after these tags -->
<xsl:template match="p|div|h1|h2|h3|h4|h5|h6|caption|th|td|blockquote|textarea|iframe|li|option|legend">
	<xsl:copy>
		<xsl:apply-templates select="@*|node()"/>
	</xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<!-- line break after these open tags -->
<xsl:template match="select">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:text>&#13;&#10;</xsl:text>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
</xsl:template>

<!-- line break after these open and close tags -->
<xsl:template match="table|thead|tfoot|tbody|tr|ul|ol|optgroup|fieldset|html|head|body">
	<xsl:copy>
		<xsl:apply-templates select="@*"/>
		<xsl:text>&#13;&#10;</xsl:text>
		<xsl:apply-templates select="node()"/>
	</xsl:copy>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<xsl:template match="@background|@dynsrc|@href|@src">
	<xsl:choose>
		<xsl:when test="../@data-ektron-url">
			<xsl:attribute name="{name()}">
				<xsl:value-of select="../@data-ektron-url"/>
			</xsl:attribute>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
<xsl:template match="@data-ektron-url"/>

<!-- placeholder tags -->

<xsl:template match="img[@data-ektron-placeholder='wmv']">
	<embed name="MediaPlayer" type="application/x-mplayer2" width="{@width}" height="{@height}" src="{@data-ektron-wmv-src}" title="{@title}" autostart="0">
		<xsl:apply-templates/> <!-- ensure separate closing tag -->
	</embed>
</xsl:template>

<xsl:template match="img[@data-ektron-placeholder='flash' and @data-placeholder='object']">
	<object>
		<xsl:call-template name="getPlaceholderAttributes"/>
		<xsl:for-each select="@*[starts-with(name(), 'data-param-')]">
			<xsl:variable name="name" select="substring-after(name(), 'data-param-')"/>
			<param name="{$name}" value="{.}" />
		</xsl:for-each>
		<xsl:if test="@*[starts-with(name(), 'data-embed-')]">
			<embed>
				<xsl:call-template name="getPlaceholderAttributes">
					<xsl:with-param name="prefix" select="'data-embed-'"/>
				</xsl:call-template>
				<xsl:apply-templates/> <!-- ensure separate closing tag -->
			</embed>
		</xsl:if>
	</object>
</xsl:template>

<xsl:template match="textarea[@data-placeholder='script']">
	<script type="text/javascript">
		<xsl:if test="@data-attr-id">
			<xsl:attribute name="id">
				<xsl:value-of select="@data-attr-id"/>
			</xsl:attribute>
		</xsl:if>
		<xsl:comment>
			<xsl:text>&#13;&#10;</xsl:text>
			<xsl:copy-of select="node()"/>
			<xsl:text>//</xsl:text>
		</xsl:comment>
	</script>
</xsl:template>

<xsl:template match="img[@data-ektron-placeholder='thumbnail']">
	<a>
		<xsl:call-template name="getPlaceholderAttributes"/>
		<img>
			<xsl:call-template name="getPlaceholderAttributes">
				<xsl:with-param name="prefix" select="'data-img-'"/>
			</xsl:call-template>
			<xsl:if test="@vspace">
				<xsl:attribute name="vspace">
					<xsl:value-of select="@vspace"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@hspace">
				<xsl:attribute name="hspace">
					<xsl:value-of select="@hspace"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@border">
				<xsl:attribute name="border">
					<xsl:value-of select="@border"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@align">
				<xsl:attribute name="align">
					<xsl:value-of select="@align"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@style">
				<xsl:attribute name="style">
					<xsl:value-of select="@style"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@alt">
				<xsl:attribute name="alt">
					<xsl:value-of select="@alt"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@title">
				<xsl:attribute name="title">
					<xsl:value-of select="@title"/>
				</xsl:attribute>
			</xsl:if>
		</img>
	</a>
</xsl:template>

<xsl:template match="img[@data-ektron-placeholder='silverlight']">
	<object>
		<xsl:call-template name="getPlaceholderAttributes"/>
		<xsl:for-each select="@*[starts-with(name(), 'data-param-')]">
			<xsl:variable name="name" select="substring-after(name(), 'data-param-')"/>
			<param name="{$name}" value="{.}" />
		</xsl:for-each>
		<xsl:if test="@data-a-href">
		<a>
			<xsl:call-template name="getPlaceholderAttributes">
				<xsl:with-param name="prefix" select="'data-a-'"/>
			</xsl:call-template>
			<img>
				<xsl:call-template name="getPlaceholderAttributes">
					<xsl:with-param name="prefix" select="'data-a_img-'"/>
				</xsl:call-template>
			</img>
		</a>
		</xsl:if>
	</object>
</xsl:template>

<xsl:template match="img[@data-ektron-placeholder='brightcove']">
	<object>
		<xsl:call-template name="getPlaceholderAttributes"/>
		<xsl:for-each select="@*[starts-with(name(), 'data-param-')]">
			<xsl:variable name="name">
				<xsl:call-template name="string-replace-all">
					<xsl:with-param name="text" select="substring-after(name(), 'data-param-')" />
					<xsl:with-param name="replace" select="'_ektat_'" />
					<xsl:with-param name="by" select="'@'" />
				</xsl:call-template>
			</xsl:variable>
			<xsl:choose>
				<xsl:when test="$name='playerid'">
					<param name="playerID" value="{.}" />
				</xsl:when> 
				<xsl:when test="$name='playerkey'">
					<param name="playerKey" value="{.}" />
				</xsl:when>
				<xsl:when test="$name='isvid'">
					<param name="isVid" value="{.}" />
				</xsl:when>
				<xsl:when test="$name='isui'">
					<param name="isUI" value="{.}" />
				</xsl:when>
				<xsl:when test="$name='dynamicstreaming'">
					<param name="dynamicStreaming" value="{.}" />
				</xsl:when>
				<xsl:when test="$name='@videoplayer'">
					<param name="@videoPlayer" value="{.}" />
				</xsl:when>
				<xsl:when test="$name='publisherid'">
					<param name="publisherID" value="{.}" />
				</xsl:when>
				<xsl:when test="$name='@playlisttabs'">
					<param name="@playlistTabs " value="{.}" />
				</xsl:when>
				<xsl:otherwise>
			<param name="{$name}" value="{.}" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
	</object>
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

<xsl:template match="img[@data-placeholder]" priority="0.4">
	<xsl:element name="{@data-placeholder}">
		<xsl:call-template name="getPlaceholderAttributes"/>
		<xsl:if test="@data-text">
			<xsl:value-of select="@data-text"/>
		</xsl:if>
		<xsl:if test="@id">
			<xsl:value-of select="concat('[',@id,']')"/>
		</xsl:if>
	</xsl:element>
</xsl:template>

<xsl:template name="getPlaceholderAttributes">
	<xsl:param name="prefix" select="'data-attr-'"/>
	<xsl:variable name="img" select="."/>
	<xsl:for-each select="@*[starts-with(name(), $prefix)]">
		<xsl:variable name="name" select="substring-after(name(), $prefix)"/>
		<xsl:choose>
			<xsl:when test="$name='height'">
				<xsl:attribute name="height">
					<xsl:value-of select="$img/@height"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:when test="$name='width'">
				<xsl:attribute name="width">
					<xsl:value-of select="$img/@width"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="{$name}">
					<xsl:value-of select="."/>
				</xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:for-each>
</xsl:template>

<xsl:template match="text()[normalize-space(.)='' and contains(.,'&#10;')]"/>

<xsl:template match="span[@class='design_edit_fieldprop']" />

<!-- #55836: FF does not preserve style attribute in image -->
<xsl:template match="img[@data-ektron-style]">
	<img>
		<xsl:attribute name="style">
			<xsl:value-of select="@data-ektron-style"/>
		</xsl:attribute>
		<xsl:apply-templates select="@*[name()!='data-ektron-style']"/>
	</img>
</xsl:template>

<!-- in IE, these styles are sticky in workarea, also line break after the option tags -->
<xsl:template match="option[@style='COLOR: menutext' or @style='COLOR: graytext']" >
	<option>
		<xsl:apply-templates select="@*[name()!='style']"/>
		<xsl:apply-templates select="node()"/>
	</option>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

</xsl:stylesheet>
