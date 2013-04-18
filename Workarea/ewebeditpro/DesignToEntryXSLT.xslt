<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" extension-element-prefixes="msxsl ekext" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:xslout="alias" xmlns:msxslout="aliasms" xmlns:ekext="urn:ektron:extension-object:common">
<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="no"/>
<xsl:namespace-alias stylesheet-prefix="xslout" result-prefix="xsl"/>
<xsl:namespace-alias stylesheet-prefix="msxslout" result-prefix="msxsl"/>

<xsl:include href="template_paramDesignTo.xslt"/>
<xsl:param name="PrefillDataXslt" select="'false'"/>
<xsl:param name="fieldlistXPath"/>

<xsl:template match="/">
	<xslout:stylesheet version="1.0" extension-element-prefixes="msxsl" exclude-result-prefixes="msxsl js dl" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:dl="urn:datalist">

		<xslout:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>
		<xslout:strip-space elements="*"/>

		<xsl:call-template name="buildDatalists"/>

		<xslout:template match="/">
			<xsl:attribute name="xml:space">preserve</xsl:attribute>
			<xsl:apply-templates>
				<xsl:with-param name="xpath" select="string($rootXPath)"/>
			</xsl:apply-templates>
		</xslout:template>
	</xslout:stylesheet>
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
	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<xsl:choose>
			<xsl:when test="name()='textarea'">
				<xsl:attribute name="xml:space">default</xsl:attribute>
				<xsl:if test="@ektdesignns_hidden='true'">
					<xsl:attribute name="style">display:none</xsl:attribute>
				</xsl:if>
				<xsl:call-template name="processContent">
					<xsl:with-param name="xpath" select="$new-xpath"/>
					<xsl:with-param name="default" select="'textarea'"/>
				</xsl:call-template>
			</xsl:when>
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
</xsl:template>


<!-- match custom design tags -->

<!-- higher priority than other 'ektdesignns_' tags -->
<xsl:template match="ektdesignns_calendar" priority="1.1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_calendartag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="ektdesignns_calendar" priority="1.1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_calendartag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>
	
<!-- higher priority than template above '*[@...]' -->
<xsl:template match="*[starts-with(name(),'ektdesignns_')]" priority="1">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_customtag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="*[starts-with(name(),'ektdesignns_')]" priority="1" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_customtag">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_calendartag">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="$PrefillDataXslt='true'">
			<xsl:variable name="new-xpath">
				<xsl:call-template name="buildXPath">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:call-template>
			</xsl:variable>
			<xsl:element name="{name()}">
				<xsl:attribute name="value">
					<xsl:value-of select="concat('{',$new-xpath,'}')"/>
				</xsl:attribute>
				<xsl:apply-templates select="@*[name()!='value']">
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
				<xsl:apply-templates select="node()">
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
			</xsl:element>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="_customtag">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
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
			<xsl:when test="name()='ektdesignns_richarea'">
				<xsl:value-of select="'mixed'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_checklist'">
				<xsl:value-of select="'checklist'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_choices'">
				<xsl:value-of select="'choices'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_calendar'">
				<xsl:value-of select="'date'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'content'"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<!-- convert custom tags to DIV or SPAN tags -->
	<xsl:variable name="displayTag">
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_calendar'">
				<xsl:value-of select="'span'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_imageonly'">
				<xsl:value-of select="'span'"/>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_filelink'">
				<xsl:value-of select="'span'"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="'div'"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<!-- Reasons: (1) a custom tag's contents won't be editable in data entry mode, (2) DIV (or SPAN) renders faster than custom tags -->
	<xsl:element name="{$displayTag}"> 
		<xsl:attribute name="class"><xsl:value-of select="concat('design_',substring-after(name(),'ektdesignns_'))"/></xsl:attribute>
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_richarea'">
				<xsl:attribute name="contenteditable">true</xsl:attribute>
				<xsl:attribute name="onfocus">design_div_focus(this)</xsl:attribute>
				<xsl:attribute name="onblur">design_div_blur(this)</xsl:attribute>
			</xsl:when>
		</xsl:choose>
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<xsl:if test="not(@ektdesignns_nodetype)">
			<xsl:attribute name="ektdesignns_nodetype">element</xsl:attribute>
		</xsl:if>
		<xsl:if test="not(@ektdesignns_content)">
			<xsl:attribute name="ektdesignns_content"><xsl:value-of select="$contentType"/></xsl:attribute>
		</xsl:if>
		<xsl:call-template name="processContent">
			<xsl:with-param name="xpath" select="$new-xpath"/>
			<xsl:with-param name="default" select="$contentType"/>
		</xsl:call-template>
	</xsl:element>
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
	<!-- convert to DIV tag -->
	<!-- Reason: don't show border or caption -->
	<xsl:element name="div"> 
		<xsl:attribute name="class">design_group</xsl:attribute>
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<xsl:if test="not(@ektdesignns_nodetype)">
			<xsl:attribute name="ektdesignns_nodetype">element</xsl:attribute>
		</xsl:if>
		<xsl:if test="not(@ektdesignns_content)">
			<xsl:attribute name="ektdesignns_content">content</xsl:attribute>
		</xsl:if>
		<xsl:call-template name="processContent">
			<xsl:with-param name="xpath" select="$new-xpath"/>
			<xsl:with-param name="default" select="'content'"/>
		</xsl:call-template>
	</xsl:element>
</xsl:template>

<!-- remove legend tag of fieldset set as a 'group' -->
<xsl:template match="fieldset[@class='design_group']/legend"/>
<xsl:template match="fieldset[@class='design_group']/legend" mode="priority-0"/>

<!-- discard contenteditable attributes required for design mode, but prohibited in data entry mode -->

<xsl:template match="fieldset/@contenteditable"/>
<xsl:template match="fieldset/*/@contenteditable"/>
<xsl:template match="label/@contenteditable"/>
<xsl:template match="label/@unselectable"/>



<!-- match input type=hidden -->

<xsl:template match="input[@type='hidden' and @ektdesignns_nodetype]">
	<!-- simply copy hidden data, no need to xpath the value from the xml doc -->
	<!-- This is needed for attribute-set -->
	<xsl:element name="{name()}">
		<xsl:copy-of select="@*"/>
	</xsl:element>
</xsl:template>

<xsl:template match="input[@type='hidden' and @ektdesignns_nodetype]" mode="priority-0">
	<!-- simply copy hidden data, no need to xpath the value from the xml doc -->
	<!-- This is needed for attribute-set -->
	<xsl:element name="{name()}">
		<xsl:copy-of select="@*"/>
	</xsl:element>
</xsl:template>


<xsl:template match="input[@type='hidden' and @ektdesignns_nodetype and (not(@value) or @value='')]">
	<!-- hidden field with an empty value attribute preserves value -->
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-text">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="input[@type='hidden' and @ektdesignns_nodetype and (not(@value) or @value='')]" mode="priority-0">
	<!-- hidden field with an empty value attribute preserves value -->
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-text">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>



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
	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<!-- content type must be 'value', see processContent -->
		<xsl:if test="not(starts-with($xpath,'design_prototype_xpath'))">
			<xsl:attribute name="value">{<xsl:value-of select="$new-xpath"/>}</xsl:attribute>
		</xsl:if>
	</xsl:element>
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
		<xsl:choose>
			<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
				<xsl:if test="@checked">
					<xsl:attribute name="checked">checked</xsl:attribute>
				</xsl:if>
			</xsl:when>
			<xsl:when test="@value and not(@value='true')">
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
	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$new-xpath"/>
		</xsl:apply-templates>
		<xsl:choose>
			<xsl:when test="starts-with($new-xpath,'design_prototype_xpath')">
				<xsl:apply-templates select="node()">
					<xsl:with-param name="xpath" select="$new-xpath"/>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:otherwise>
				<!-- add option(s) if value is unknown -->
				<xsl:variable name="id">
					<xsl:for-each select="@id"> <!-- set context node -->
						<xsl:call-template name="generateID">
							<xsl:with-param name="xpath" select="$new-xpath"/>
						</xsl:call-template>
					</xsl:for-each>
				</xsl:variable>
				<!-- avoid modifying template_datalist.xslt --> 
				<!-- add the 1st option (if 1st item is not valid) for DataEntry mode -->
				<xsl:variable name="option" select="(descendant::option)[1]"/>
				<xsl:variable name="test-req" select="@ektdesignns_validation='select-req' and (not(@size) or @size=1)"/>
				<xsl:if test="$test-req = true()">
					<option>
						<xsl:copy-of select="$option/@*[name()!='selected']"/>
						<xsl:copy-of select="$option/node()"/>
					</option>
                </xsl:if>

				<xsl:call-template name="beginDatalistAccess"/>

				<xslout:for-each select="{$new-xpath}"><!--  warning: won't work with @multiple attribute type -->
					<xslout:variable name="value" select="string(.)"/>
					<xsl:call-template name="buildDatalistItemDisplayValue"/>
					<xslout:variable name="test-notinlist" select="string-length($value) &gt; 0 and count($display-value)=0"/>
					<xsl:choose>
						<xsl:when test="$test-req = true()">
							<xslout:if test="$test-notinlist = true() and $value != {ekext:xpathLiteralString(string($option/@value))}">
								<option xml:space="default" selected="selected" value="{{$value}}">
									<xslout:value-of select="$value"/>
								</option>
							</xslout:if>
						</xsl:when>
						<xsl:otherwise>
							<xslout:if test="$test-notinlist = true()">
								<option xml:space="default" selected="selected" value="{{$value}}">
									<xslout:value-of select="$value"/>
								</option>
							</xslout:if>
						</xsl:otherwise>
					</xsl:choose>
				</xslout:for-each>
				<!-- add options -->
				<xsl:variable name="captionxpath">
					<xsl:call-template name="getCaptionXPath"/>
				</xsl:variable>
				<xsl:variable name="valuexpath">
					<xsl:call-template name="getValueXPath"/>
				</xsl:variable>
				<xslout:variable name="data" select="{$new-xpath}"/>
				<xslout:variable name="value" select="string($data)"/>
				<xslout:for-each select="$dl">
					<option value="{{{$valuexpath}}}" xml:space="default">
						<xsl:choose>
							<xsl:when test="@ektdesignns_nodetype='element' and @multiple">
								<xslout:if test="$data[.=current()/{$valuexpath}]"> 
									<xslout:attribute name="selected">selected</xslout:attribute>
								</xslout:if>
							</xsl:when>
							<xsl:when test="@ektdesignns_nodetype and @multiple">
								<xslout:if test="contains(concat(' ',$value,' '),concat(' ',{$valuexpath},' '))"> 
									<xslout:attribute name="selected">selected</xslout:attribute>
								</xslout:if>
							</xsl:when>
							<xsl:otherwise>
								<xslout:if test="$value={$valuexpath}">
									<xslout:attribute name="selected">selected</xslout:attribute>
								</xslout:if>
							</xsl:otherwise>
						</xsl:choose>
						<xslout:copy-of select="{$captionxpath}/node()"/>
					</option>
				</xslout:for-each>

				<xsl:call-template name="endDatalistAccess"/>

			</xsl:otherwise>
		</xsl:choose>
	</xsl:element>
</xsl:template>

<xsl:template match="select/@ektdesignns_datasrc"/>
<xsl:template match="select/@ektdesignns_dataselect"/>
<xsl:template match="select/@ektdesignns_captionxpath"/>
<xsl:template match="select/@ektdesignns_valuexpath"/>
<xsl:template match="select/@ektdesignns_datanamespaces"/>


<!-- process select/option -->

<xsl:template match="option[ancestor::select[@ektdesignns_nodetype]]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_option">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="option[ancestor::select[@ektdesignns_nodetype]]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_option">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_option">
	<xsl:param name="xpath"/>
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
	<xsl:choose>
		<xsl:when test="@disabled and not(starts-with($xpath,'design_prototype_xpath'))">
			<xslout:for-each select="{$xpath}"> <!-- note: won't work for @multiple attribute type -->
				<xslout:if test="string(.)={ekext:xpathLiteralString(string($value))}">
					<xsl:element name="{name()}">
						<xsl:attribute name="xml:space">default</xsl:attribute>
						<xsl:apply-templates select="@*[name()!='disabled']">
							<xsl:with-param name="xpath" select="'.'"/>
							<!-- xsl:with-param name="xpath" select="$xpath"/ -->
						</xsl:apply-templates>
						<xsl:attribute name="selected">selected</xsl:attribute>
						<xsl:copy-of select="node()"/>
					</xsl:element>
				</xslout:if>
			</xslout:for-each>
		</xsl:when>
		<xsl:otherwise>
			<xsl:element name="{name()}">
				<xsl:attribute name="xml:space">default</xsl:attribute>
				<xsl:apply-templates select="@*[name()!='selected']">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
				<xsl:choose>
					<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
						<xsl:if test="@selected">
							<xsl:attribute name="selected">selected</xsl:attribute>
						</xsl:if>
					</xsl:when>
					<xsl:when test="current()[ancestor::select[@ektdesignns_nodetype='element' and @multiple]]">
						<xslout:if test="{$xpath}[.={ekext:xpathLiteralString(string($value))}]"> 
							<xslout:attribute name="selected">selected</xslout:attribute>
						</xslout:if>
					</xsl:when>
					<xsl:when test="current()[ancestor::select[@ektdesignns_nodetype and @multiple]]">
						<xslout:if test="contains(concat(' ',{$xpath},' '),concat(' ',{ekext:xpathLiteralString(string($value))},' '))">
							<xslout:attribute name="selected">selected</xslout:attribute>
						</xslout:if>
					</xsl:when>
					<xsl:otherwise>
						<xslout:if test="{$xpath}={ekext:xpathLiteralString(string($value))}">
							<xslout:attribute name="selected">selected</xslout:attribute>
						</xslout:if>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:copy-of select="node()"/>
			</xsl:element>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- item nodetype ====================================================================== -->


<!-- match li/input type=radio -->

<xsl:template match="li[input[@disabled and @type='radio' and @ektdesignns_nodetype='item']]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_li-input-radio-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="li[input[@disabled and @type='radio' and @ektdesignns_nodetype='item']]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_li-input-radio-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_li-input-radio-item">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="not(starts-with($xpath,'design_prototype_xpath'))">
			<xsl:choose>
				<xsl:when test="input/@value">
					<xsl:if test="input/@ektdesignns_role='default'">
						<xslout:if test="not({$xpath})">
							<xsl:copy>
								<xsl:apply-templates select="@*|node()">
									<xsl:with-param name="xpath" select="$xpath"/>
								</xsl:apply-templates>
							</xsl:copy>
						</xslout:if>
					</xsl:if>
					<xslout:if test="{$xpath}={ekext:xpathLiteralString(string(input/@value))}">
						<xsl:copy>
							<xsl:apply-templates select="@*|node()">
								<xsl:with-param name="xpath" select="$xpath"/>
							</xsl:apply-templates>
						</xsl:copy>
					</xslout:if>
				</xsl:when>
				<xsl:otherwise>
					<xslout:if test="not({$xpath})">
						<xsl:copy>
							<xsl:apply-templates select="@*|node()">
								<xsl:with-param name="xpath" select="$xpath"/>
							</xsl:apply-templates>
						</xsl:copy>
					</xslout:if>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy>
				<xsl:apply-templates select="@*|node()">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
			</xsl:copy>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
	

<!-- match input type=radio -->

<xsl:template match="input[@type='radio' and @ektdesignns_nodetype='item']">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-radio-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="input[@type='radio' and @ektdesignns_nodetype='item']" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-radio-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_input-radio-item">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="@disabled and not(starts-with($xpath,'design_prototype_xpath'))">
			<xsl:element name="{name()}">
				<xsl:attribute name="xml:space">default</xsl:attribute>
				<xsl:apply-templates select="@*[name()!='disabled']">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
				<xsl:attribute name="checked">checked</xsl:attribute>
			</xsl:element>
		</xsl:when>
		<xsl:otherwise>
			<xsl:element name="{name()}">
				<xsl:attribute name="xml:space">default</xsl:attribute>
				<xsl:apply-templates select="@*[name()!='checked']">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
				<xsl:choose>
					<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
						<xsl:if test="@checked">
							<xsl:attribute name="checked">checked</xsl:attribute>
						</xsl:if>
					</xsl:when>
					<xsl:when test="@value">
						<xsl:if test="@ektdesignns_role='default'">
							<xslout:if test="not({$xpath})">
								<xslout:attribute name="checked">checked</xslout:attribute>
							</xslout:if>
						</xsl:if>
						<xslout:if test="{$xpath}={ekext:xpathLiteralString(string(@value))}">
							<xslout:attribute name="checked">checked</xslout:attribute>
						</xslout:if>
					</xsl:when>
					<xsl:otherwise>
						<xslout:if test="not({$xpath})">
							<xslout:attribute name="checked">checked</xslout:attribute>
						</xslout:if>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- match li/input type=checkbox -->

<xsl:template match="li[input[@disabled and @type='checkbox' and @ektdesignns_nodetype='item']]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_li-input-checkbox-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="li[input[@disabled and @type='checkbox' and @ektdesignns_nodetype='item']]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_li-input-checkbox-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_li-input-checkbox-item">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="not(input/@checked) and not(starts-with($xpath,'design_prototype_xpath'))">
			<xsl:choose>
				<xsl:when test="input/@value and contains($xpath,'@')"> <!-- node is an attribute -->
					<xslout:if test="contains(concat(' ',{$xpath},' '),concat(' ',{ekext:xpathLiteralString(string(input/@value))},' '))"> 
						<xsl:copy>
							<xsl:apply-templates select="@*|node()">
								<xsl:with-param name="xpath" select="$xpath"/>
							</xsl:apply-templates>
						</xsl:copy>
					</xslout:if>
				</xsl:when>
				<xsl:when test="input/@value"> <!-- should be element only, but also applies to other node types as written -->
					<xslout:if test="{$xpath}[.={ekext:xpathLiteralString(string(input/@value))}]"> 
						<xsl:copy>
							<xsl:apply-templates select="@*|node()">
								<xsl:with-param name="xpath" select="$xpath"/>
							</xsl:apply-templates>
						</xsl:copy>
					</xslout:if>
				</xsl:when>
				<xsl:otherwise>
					<xslout:if test="not({$xpath})">
						<xsl:copy>
							<xsl:apply-templates select="@*|node()">
								<xsl:with-param name="xpath" select="$xpath"/>
							</xsl:apply-templates>
						</xsl:copy>
					</xslout:if>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy>
				<xsl:apply-templates select="@*|node()">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
			</xsl:copy>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>
	

<!-- match input type=checkbox -->

<xsl:template match="input[@type='checkbox' and @ektdesignns_nodetype='item']">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-checkbox-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="input[@type='checkbox' and @ektdesignns_nodetype='item']" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_input-checkbox-item">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_input-checkbox-item">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="not(@checked) and @disabled and not(starts-with($xpath,'design_prototype_xpath'))">
			<xsl:element name="{name()}">
				<xsl:attribute name="xml:space">default</xsl:attribute>
				<xsl:apply-templates select="@*[name()!='disabled']">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
				<xsl:attribute name="checked">checked</xsl:attribute>
			</xsl:element>
		</xsl:when>
		<xsl:otherwise>
			<xsl:element name="{name()}">
				<xsl:attribute name="xml:space">default</xsl:attribute>
				<xsl:apply-templates select="@*[name()!='checked']">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:apply-templates>
				<xsl:choose>
					<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
						<xsl:if test="@checked">
							<xsl:attribute name="checked">checked</xsl:attribute>
						</xsl:if>
					</xsl:when>
					<xsl:when test="@value and contains($xpath,'@')"> <!-- node is an attribute -->
						<xslout:if test="contains(concat(' ',{$xpath},' '),concat(' ',{ekext:xpathLiteralString(string(@value))},' '))"> 
							<xslout:attribute name="checked">checked</xslout:attribute>
						</xslout:if>
					</xsl:when>
					<xsl:when test="@value"> <!-- should be element only, but also applies to other node types as written -->
						<xslout:if test="{$xpath}[.={ekext:xpathLiteralString(string(@value))}]"> 
							<xslout:attribute name="checked">checked</xslout:attribute>
						</xslout:if>
					</xsl:when>
					<xsl:otherwise>
						<xslout:if test="not({$xpath})">
							<xslout:attribute name="checked">checked</xslout:attribute>
						</xslout:if>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- special case: label ================================================================ -->
<!-- Labels are NOT SUPPORTED for editing, only when inserting a new element -->
<!-- @ektdesignns_label -->
<!-- IMPORTANT: xslout: elements kill following attributes -->
<!-- Fortunately, leaving the ektdesignns_label attribute has no negative effects. -->
<!-- xsl:template match="@ektdesignns_label">
<xsl:param name="xpath"/>

<xslout:if test="{concat($xpath,'/@ektdesignns_label')}">
	<xslout:attribute name="ektdesignns_label"><xsl:value-of select="."/></xslout:attribute>
</xslout:if>
</xsl:template -->

<!-- @ektdesignns_role='label' -->
<xsl:template match="*[@ektdesignns_role='label']">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_label">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="*[@ektdesignns_role='label']" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_label">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_label">
	<xsl:param name="xpath"/>
	<xslout:if test="{concat($xpath,'/@ektdesignns_label')}">
		<xsl:element name="{name()}">
			<xsl:apply-templates select="@*">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
			<xsl:apply-templates select="node()">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
		</xsl:element>
	</xslout:if>
</xsl:template>


<!-- remove form field for a label value -->
<!-- xsl:template match="*[@ektdesignns_nodetype='label']">
<xsl:text>(not available)</xsl:text>
</xsl:template -->


<!-- Section 508 unsupported attributes, remove them -->

<xsl:template match="th/@id[../@scope]|td/@id[../@scope]"/>
<xsl:template match="th/@scope|td/@scope"/>
<xsl:template match="td/@headers|th/@headers"/>


<!-- special case: list prototype  ======================================================= -->


<xsl:template match="thead[following-sibling::tbody[@ektdesignns_list]]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_thead-for-tbody-list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="thead[following-sibling::tbody[@ektdesignns_list]]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_thead-for-tbody-list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_thead-for-tbody-list">
	<xsl:param name="xpath"/>

	<thead>
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
		<xsl:for-each select="tr">
			<xsl:apply-templates select="@*">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
			<th>&#160;</th>
			<xsl:apply-templates select="th">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
		</xsl:for-each>
	</thead>
</xsl:template>


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
	
	<xsl:variable name="prototype0_rtf"> <!-- result tree fragment -->
		<xsl:choose>
			<xsl:when test="../tfoot">
				<xsl:copy-of select="../tfoot/tr[1]"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="buildPrototype0">
					<xsl:with-param name="sample" select="tr[@ektdesignns_nodetype][1]"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="prototype0" select="msxsl:node-set($prototype0_rtf)/*[1]"/>
	
	<xsl:variable name="prototype_rtf"> <!-- result tree fragment -->
		<xsl:choose>
			<xsl:when test="../tfoot">
				<xsl:copy-of select="../tfoot/tr[2]"/>
			</xsl:when>
			<xsl:otherwise>
				<!-- prototype stores initial values -->
				<xsl:call-template name="buildPrototype">
					<xsl:with-param name="xpath" select="'design_prototype_xpath.'"/> <!-- ending period is required -->
					<xsl:with-param name="sample" select="tr[@ektdesignns_nodetype][1]"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="prototype" select="msxsl:node-set($prototype_rtf)/*[1]"/>
			
	<xsl:variable name="prototype2_rtf"> <!-- result tree fragment -->
		<xsl:choose>
			<xsl:when test="../tfoot">
				<xsl:copy-of select="../tfoot/tr[2]"/>
			</xsl:when>
			<xsl:otherwise>
				<!-- prototype stores initial values -->
				<xsl:call-template name="buildPrototype">
					<xsl:with-param name="xpath" select="'.'"/>
					<xsl:with-param name="sample" select="tr[@ektdesignns_nodetype][1]"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="prototype2" select="msxsl:node-set($prototype2_rtf)/*[1]"/>
			
	<xsl:if test="not(../tfoot)">
		<tfoot class="design_prototype">
			<xsl:call-template name="copyData">
				<xsl:with-param name="data" select="$prototype0_rtf"/>
			</xsl:call-template>
			<xsl:call-template name="copyData">
				<xsl:with-param name="data" select="$prototype_rtf"/>
			</xsl:call-template>
		</tfoot>
	</xsl:if>
	
	
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
			<xsl:element name="{name($prototype2)}">
				<!-- exclude id attribute -->
				<xsl:copy-of select="$prototype2/@*[name()!='id']"/>
				<xsl:call-template name="copyData">
					<xsl:with-param name="data" select="$prototype2/node()"/>
				</xsl:call-template>
			</xsl:element>
		</xslout:for-each>
		<xsl:variable name="minoccurs" select="$prototype/@ektdesignns_minoccurs"/>
		<xsl:choose>
			<xsl:when test="$minoccurs='0' and not(@ektdesignns_maxoccurs='unbounded')">
				<xslout:if test="count({$new-xpath})=0">
					<xsl:call-template name="copyData">
						<xsl:with-param name="data" select="$prototype0"/>
					</xsl:call-template>
				</xslout:if>
			</xsl:when>
			<xsl:when test="$minoccurs='1' or not($minoccurs)">
				<xslout:if test="count({$new-xpath})=0">
					<xsl:call-template name="copyData">
						<xsl:with-param name="data">
							<xsl:element name="{name($prototype)}">
								<xsl:copy-of select="$prototype/@*[name()!='id']"/>
								<xsl:copy-of select="$prototype/node()"/>
							</xsl:element>
						</xsl:with-param>
						<xsl:with-param name="numberOfTimes" select="$minoccurs"/>
					</xsl:call-template>
				</xslout:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="buildAddMinOccurs">
					<xsl:with-param name="xpath" select="$new-xpath"/>
					<xsl:with-param name="minoccurs" select="$minoccurs"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
		<!-- + Add another -->
		<xsl:if test="$prototype/@ektdesignns_maxoccurs='unbounded'">
			<xsl:call-template name="buildPrototype0">
				<xsl:with-param name="sample" select="$prototype"/>
				<xsl:with-param name="action" select="'insertAbove'"/>
			</xsl:call-template>
		</xsl:if>
		<xsl:apply-templates select="tr[not(@ektdesignns_nodetype)]">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</xsl:element>
</xsl:template>


<xsl:template match="tr[parent::tbody[@ektdesignns_list] and not(@ektdesignns_nodetype)]">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_tr-for-tbody-list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="tr[parent::tbody[@ektdesignns_list] and not(@ektdesignns_nodetype)]" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_tr-for-tbody-list">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_tr-for-tbody-list">
	<xsl:param name="xpath"/>

	<xsl:element name="{name()}">
		<xsl:apply-templates select="@*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
		<td>&#160;</td>
		<xsl:apply-templates select="*">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:apply-templates>
	</xsl:element>
</xsl:template>


<!-- make id's unique -->

<xsl:template match="@id">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_uniqueId">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="@id" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_uniqueId">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="label/@for">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_uniqueId">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="label/@for" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_uniqueId">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_uniqueId">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="starts-with(.,'staticID-')">
			<xsl:copy-of select="."/>
		</xsl:when>
		<xsl:when test="not($xpath) or $xpath=$rootXPath or $xpath=concat($rootXPath,'/',.) or $xpath=concat($rootXPath,'/@',.)">
			<xsl:copy-of select="."/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:attribute name="{name()}">
				<xsl:call-template name="generateID">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:call-template>
			</xsl:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="input/@name|textarea/@name">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_uniqueName">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template match="input/@name|textarea/@name" mode="priority-0">
	<xsl:param name="xpath"/>
	<xsl:call-template name="_uniqueName">
		<xsl:with-param name="xpath" select="$xpath"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="_uniqueName">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="starts-with(.,'design_')">
			<xsl:copy-of select="."/>
		</xsl:when>
		<xsl:when test="not($xpath) or $xpath=$rootXPath or $xpath=concat($rootXPath,'/',.) or $xpath=concat($rootXPath,'/@',.)">
			<xsl:copy-of select="."/>
		</xsl:when>
		<xsl:when test="../@type='button' or ../@type='reset' or ../@type='submit' or ../@type='image'">
			<xsl:copy-of select="."/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:attribute name="{name()}">
				<xsl:call-template name="generateID">
					<xsl:with-param name="xpath" select="$xpath"/>
				</xsl:call-template>
			</xsl:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="generateID">
	<xsl:param name="xpath"/>
	<xsl:choose>
		<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
			<xsl:value-of select="."/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="concat(.,'{generate-id(',$xpath,')}')"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<xsl:include href="template_xsltAttrVal.xslt"/>


<!-- copy prototypes, don't process -->
<xsl:template match="*[@class='design_prototype']">
	<xsl:call-template name="_design_prototype"/>
</xsl:template>

<xsl:template match="*[@class='design_prototype']" mode="priority-0">
	<xsl:call-template name="_design_prototype"/>
</xsl:template>

<xsl:template name="_design_prototype">
	<xsl:copy-of select="."/>
</xsl:template>


<!-- remove design-time only attributes -->
<xsl:template match="@class[.='show_design_border']"/>
<xsl:template match="@class[.='show_design_border']" mode="priority-0"/>


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

<!-- Note: this template reapply templates using a different mode. It is not possible to use 
import/apply-imports because only template with the same match rule are applied. Also, the params
would be lost. There is no way to set the priority when applying templates, so we have to use
a different mode, which makes the templates themselves ugly, but I don't know of any other way to
do this. Ironically, a highly recursive processing language like XSLT can't easily recursively 
re-process. If the mode attribute could take a list of modes, it would resolve this problem.
-->

<!-- match ektdesignns_minoccurs or ektdesignns_maxoccurs  or ektdesignns_use='optional' -->

<!-- this template is uniquely for data entry and disallows round-tripping the content -->
<xsl:template match="*[(@ektdesignns_name or @ektdesignns_bind) and 
(@ektdesignns_maxoccurs='unbounded' or number(@ektdesignns_maxoccurs) &gt; 1 
or number(@ektdesignns_minoccurs) &gt; 1 or @ektdesignns_minoccurs='0' or @ektdesignns_use='optional') 
and name() != 'select' and name() != 'ektdesignns_checklist' and name() != 'ektdesignns_choices']" priority="2">
	<xsl:param name="xpath"/>
	
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPathNoPrototype">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>

	<xsl:variable name="prototype0"> <!-- result tree fragment -->
		<xsl:call-template name="buildPrototype0"/>
	</xsl:variable>
	
	<xsl:variable name="prototype"> <!-- result tree fragment -->
		<!-- prototype stores initial values -->
		<xsl:call-template name="buildPrototype">
			<xsl:with-param name="xpath" select="'design_prototype_xpath'"/>
		</xsl:call-template>
	</xsl:variable>
	
	<div class="design_dynlist_container" contenteditable="false" unselectable="on">
		<table class="design_dynlist_area" cellspacing="0" cellpadding="0" border="0">
			<tfoot class="design_prototype">
				<xsl:call-template name="copyData">
					<xsl:with-param name="data" select="$prototype0"/>
				</xsl:call-template>
				<xsl:call-template name="copyData">
					<xsl:with-param name="data" select="$prototype"/>
				</xsl:call-template>
			</tfoot>
			<tbody ektdesignns_list="true">	
				<xsl:call-template name="buildSpacer"/>
				<xslout:for-each select="{$new-xpath}">
					<!-- prototype template -->
					<xsl:call-template name="buildPrototype">
						<xsl:with-param name="xpath" select="'..'"/>
					</xsl:call-template>
				</xslout:for-each>
				<xsl:choose>
					<xsl:when test="(@ektdesignns_minoccurs='0' or @ektdesignns_use='optional') and not(@ektdesignns_maxoccurs='unbounded')">
						<xslout:if test="count({$new-xpath})=0">
							<xsl:call-template name="copyData">
								<xsl:with-param name="data" select="$prototype0"/>
							</xsl:call-template>
						</xslout:if>
					</xsl:when>
					<xsl:when test="@ektdesignns_minoccurs='1' or not(@ektdesignns_minoccurs)">
						<xslout:if test="count({$new-xpath})=0">
							<xsl:call-template name="copyData">
								<xsl:with-param name="data" select="$prototype"/>
								<xsl:with-param name="numberOfTimes" select="@ektdesignns_minoccurs"/>
							</xsl:call-template>
						</xslout:if>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="buildAddMinOccurs">
							<xsl:with-param name="xpath" select="$new-xpath"/>
							<xsl:with-param name="minoccurs" select="@ektdesignns_minoccurs"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
				<!-- + Add another -->
				<xsl:if test="@ektdesignns_maxoccurs='unbounded'">
					<xsl:call-template name="buildPrototype0">
						<xsl:with-param name="action" select="'insertAbove'"/>
					</xsl:call-template>
				</xsl:if>
			</tbody>
		</table>
	</div>
</xsl:template>

<xsl:include href="template_copyData.xslt"/>


<xsl:template name="buildSpacer">
	<tr class="design_spacer">
		<td>&#160;</td>
		<td><img class="design_dynlist_spacer" /></td>
		<td>&#160;</td>
	</tr>
</xsl:template>

<xsl:template name="getDescriptiveName">
	<xsl:param name="sample"/>
	<xsl:choose>
		<xsl:when test="$sample">
			<xsl:choose>
				<xsl:when test="$sample/@ektdesignns_caption">
					<xsl:value-of select="$sample/@ektdesignns_caption"/>
				</xsl:when>
				<xsl:when test="$sample/@ektdesignns_label"> <!-- legacy, deprecated -->
					<xsl:value-of select="$sample/@ektdesignns_label"/>
				</xsl:when>
				<xsl:when test="$sample/@title"> <!-- legacy, now used for tooltip -->
					<xsl:value-of select="$sample/@title"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$sample/@ektdesignns_name"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:otherwise>
			<xsl:choose>
				<xsl:when test="@ektdesignns_caption">
					<xsl:value-of select="@ektdesignns_caption"/>
				</xsl:when>
				<xsl:when test="@ektdesignns_label"> <!-- legacy, deprecated -->
					<xsl:value-of select="@ektdesignns_label"/>
				</xsl:when>
				<xsl:when test="@title"> <!-- legacy, now used for tooltip -->
					<xsl:value-of select="@title"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@ektdesignns_name"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="buildPrototype0">
	<xsl:param name="sample"/>
	<xsl:param name="action" select="'replace'"/>

	<xsl:variable name="descriptiveName">
		<xsl:call-template name="getDescriptiveName">
			<xsl:with-param name="sample" select="$sample"/>
		</xsl:call-template>
	</xsl:variable>
	
	<tr onclick="design_row_setCurrent(this)">
		<xsl:variable name="colspan">
			<xsl:choose>
				<xsl:when test="$sample">
					<xsl:value-of select="count($sample/td|$sample/th)"/>
				</xsl:when>
				<xsl:otherwise>2</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<td colspan="{$colspan}" unselectable="on">
			<a href="#" onclick="design_row_setCurrent(this.parentElement.parentElement);design_row_{$action}();return false;" menutype="button" class="design_dynlist_menu"><img class="design_add_graphic" menutype="button" src="[srcpath]additem.gif" width="9" height="9" border="0"/><xsl:value-of select="$descriptiveName"/></a>
		</td>
		<xsl:if test="not($sample)">
			<td class="design_dynlist_last_normal" unselectable="on">&#160;</td>
		</xsl:if>
	</tr>
</xsl:template>

<xsl:template name="buildPrototype">
	<xsl:param name="xpath"/>
	<xsl:param name="sample"/>

	<xsl:variable name="descriptiveName">
		<xsl:call-template name="getDescriptiveName">
			<xsl:with-param name="sample" select="$sample"/>
		</xsl:call-template>
	</xsl:variable>
	
	<tr onclick="design_row_setCurrent(this)">
		<xsl:if test="$sample">
			<xsl:apply-templates select="$sample/@*" mode="priority-0">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
		</xsl:if>
		<td class="design_dynlist_first_normal" unselectable="on">
			<a href="#" onclick="design_row_showContextMenu(this);return false;" menutype="button" class="design_dynlist_menu" onmouseover="design_row_onmouse(this)" onmouseout="design_row_onmouse(this)" title="{$descriptiveName}"><img class="design_contextmenu_button" menutype="button" src="[srcpath]designmenu.gif" width="11" height="16" border="0"/></a>
		</td>
		<xsl:choose>
			<xsl:when test="$sample">
				<xsl:for-each select="$sample/td|$sample/th">
					<xsl:apply-templates select="." mode="priority-0">
						<xsl:with-param name="xpath" select="$xpath"/>
					</xsl:apply-templates>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<td class="design_dynlist_normal">
				
					<xsl:apply-templates select="." mode="priority-0">
						<xsl:with-param name="xpath" select="$xpath"/>
					</xsl:apply-templates>
					
				</td>
				<td class="design_dynlist_last_normal" unselectable="on">&#160;</td>
			</xsl:otherwise>
		</xsl:choose>
	</tr>
</xsl:template>

<xsl:template name="buildAddMinOccurs">
	<xsl:param name="xpath"/>
	<xsl:param name="minoccurs" select="1"/>
	<xsl:param name="id" select="generate-id()"/>
	<!-- preload with minimum number of elements -->
	<xslout:if test="count({$xpath}) &lt; {number($minoccurs)}">
		<tr id="{$id}"><td ektdesignns_addmin="{{{number($minoccurs)} - count({$xpath})}}">&#160;</td></tr>
	</xslout:if>
</xsl:template>




<!-- templates ========================================================================== -->

<xsl:include href="template_getDesignName.xslt"/>
<xsl:include href="template_buildXPath.xslt"/>
<xsl:include href="template_datalist.xslt"/>

<!-- buildXPathNoPrototype -->

<!-- remove leading 'design_prototype_xpath', if it exists -->
<xsl:template name="buildXPathNoPrototype">
	<xsl:param name="xpath"/>
	<xsl:variable name="new-xpath">
		<xsl:call-template name="buildXPath">
			<xsl:with-param name="xpath" select="$xpath"/>
		</xsl:call-template>
	</xsl:variable>
	<xsl:choose>
		<xsl:when test="starts-with($new-xpath,'design_prototype_xpath')">
			<xsl:value-of select="substring-after($new-xpath,'design_prototype_xpath')"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="$new-xpath"/>
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
	
		<xsl:when test="contains($xpath,'|') and contains($xpath,'@')">
			<!-- Special case: label may be an attribute ektdesignns_label of element. -->
			<!-- format of xpath: xpath-to-attr|xpath-to-mixed-node -->
			<xsl:variable name="xpathAttr" select="substring-before($xpath,'|')"/>
			<xsl:variable name="xpathMixed" select="substring-after($xpath,'|')"/>
			<xsl:choose>
				<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
					<xsl:copy-of select="./node()"/>
				</xsl:when>
				<xsl:otherwise>
					<xslout:choose>
						<xslout:when test="{$xpathAttr}">
							<xslout:value-of select="{$xpathAttr}"/>
						</xslout:when>
						<xslout:otherwise>
							<xslout:copy-of select="{concat($xpathMixed,'/node()')}"/>
						</xslout:otherwise>
					</xslout:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='date'">
			<xsl:if test="not(starts-with($xpath,'design_prototype_xpath'))">
				<xsl:attribute name="value">{<xsl:value-of select="$xpath"/>}</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates select="node()">
				<xsl:with-param name="xpath" select="$xpath"/>
			</xsl:apply-templates>
		</xsl:when>
		
		<xsl:when test="starts-with($contentType,'element=')">
			<!-- see also PresentationToData -->
			<xsl:choose>
				<xsl:when test="contains($contentType,'/@')">
					<xsl:variable name="elementName" select="substring-before(substring-after($contentType,'element='),'/@')"/>
					<xsl:variable name="attrName" select="substring-after($contentType,'/@')"/>
					<xsl:choose>
						<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
							<xslout:value-of select="''"/> <!-- placeholder to ensure tags don't collapse -->
							<xsl:copy-of select="./*[name()=$elementName][not(starts-with(@class,'design_'))]"/>
						</xsl:when>
						<xsl:when test="$elementName='a'">
							<a>
								<xsl:copy-of select="./a[not(starts-with(@class,'design_'))]/@*"/>
								<xsl:attribute name="href">{<xsl:value-of select="$xpath"/>}</xsl:attribute>
								<xsl:attribute name="title">{<xsl:value-of select="$xpath"/>}</xsl:attribute>
								<xslout:value-of select="{$xpath}"/>
							</a>
						</xsl:when>
						<xsl:when test="$elementName='img'">
							<img>
								<xsl:copy-of select="./img[not(starts-with(@class,'design_'))]/@*"/>
								<xsl:attribute name="src">{<xsl:value-of select="$xpath"/>}</xsl:attribute>
								<xsl:attribute name="alt">{<xsl:value-of select="$xpath"/>}</xsl:attribute>
							</img>
						</xsl:when>
						<xsl:otherwise>
							<!-- should not get here -->
							<xslout:copy-of select="{concat($xpath,'/',$elementName,'[1]')}"/>
						</xsl:otherwise>
					</xsl:choose>
					<!-- copy content after to the content element -->
					<xsl:apply-templates select="./node()[name()!=$elementName or starts-with(@class,'design_')]">
						<xsl:with-param name="xpath" select="$xpath"/>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<xsl:variable name="elementName" select="substring-after($contentType,'element=')"/>
					<!-- copy the content element -->
					<xsl:choose>
						<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
							<xslout:value-of select="''"/> <!-- placeholder to ensure tags don't collapse -->
							<xsl:copy-of select="./*[name()=$elementName][not(starts-with(@class,'design_'))]"/>
						</xsl:when>
						<xsl:otherwise>
							<xslout:copy-of select="{concat($xpath,'/',$elementName,'[1]')}"/>
						</xsl:otherwise>
					</xsl:choose>
					<!-- copy content after to the content element -->
					<xsl:apply-templates select="./node()[name()!=$elementName or starts-with(@class,'design_')]">
						<xsl:with-param name="xpath" select="$xpath"/>
					</xsl:apply-templates>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="contains($xpath,'@') and $contentType!='choices' and $contentType!='checklist'"> <!-- and $contentType!='date' -->
			<xsl:choose>
				<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
					<xsl:value-of select="."/>
				</xsl:when>
				<xsl:otherwise>
					<!-- xpath is an attribute -->
					<xslout:value-of select="{$xpath}"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='mixed'">
			<xsl:choose>
				<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
					<xslout:value-of select="''"/> <!-- placeholder to ensure tags don't collapse -->
					<xsl:copy-of select="./node()"/>
				</xsl:when>
				<xsl:when test="contains($xpath,'/node()')">
					<xslout:copy-of select="{$xpath}"/>
				</xsl:when>
				<xsl:otherwise>
					<xslout:copy-of select="{$xpath}/node()"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='text'">
			<xsl:choose>
				<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
					<xsl:copy-of select="./text()"/>
				</xsl:when>
				<xsl:otherwise>
					<xslout:value-of select="{$xpath}"/>
					<!-- text() will duplicate content if more than one of these in an element -->
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='textarea'">
			<xsl:choose>
				<xsl:when test="starts-with($xpath,'design_prototype_xpath')">
					<xsl:value-of select="."/>
				</xsl:when>
				<xsl:otherwise>
					<xslout:value-of select="{$xpath}"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='value'">
			<xsl:if test="not(starts-with($xpath,'design_prototype_xpath'))">
				<xsl:attribute name="value">{<xsl:value-of select="$xpath"/>}</xsl:attribute>
			</xsl:if>
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
