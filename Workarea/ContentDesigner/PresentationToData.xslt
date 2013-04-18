<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" extension-element-prefixes="msxsl js ekext" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:js="urn:custom-javascript" xmlns:ekext="urn:ektron:extension-object:common">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

<!-- custom attribute definition can be found in DataDesignerDetailDesign.doc -->

<!--
<msxsl:script language="JavaScript" implements-prefix="js"><![CDATA[
function uniqueValue()
{
	return Math.floor(Math.random() * 1679616).toString(36); // 4 digit alphanum
}
]]></msxsl:script>
-->

<xsl:variable name="uniqueSuffix" select="string(ekext:uniqueValue())"/>
<xsl:include href="template_copyData.xslt"/>

<xsl:template match="/">
	<xsl:choose>
		<xsl:when test="//*[@ektdesignns_role='root' and not(ancestor::*/@ektdesignns_name)]">
			<xsl:apply-templates mode="attributes"/>
			<xsl:apply-templates mode="content"/>
		</xsl:when>
		<xsl:otherwise>
			<root>
				<xsl:apply-templates mode="attributes"/>
				<xsl:apply-templates mode="content"/>
			</root>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- attribute-set nodetype ========================================================================= -->


<xsl:template match="*[@ektdesignns_nodetype='attribute-set']" mode="content">
	<xsl:param name="listitem"/>
	
	<xsl:variable name="attrContainer">
		<!-- The attributes need an element as a container -->
		<!-- Can't use the name of the container element in an XPath of the node-set 
				because it does not work for some unknown reason. Use '*[1]' instead. -->
		<xsl:element name="dummy"> 
			<xsl:apply-templates mode="attributes"/>
		</xsl:element>
	</xsl:variable>
	
	<xsl:apply-templates mode="content">
		<xsl:with-param name="listitem" select="$listitem"/>
		<xsl:with-param name="attribute-set" select="msxsl:node-set($attrContainer)/*[1]/@*"/>
	</xsl:apply-templates>
</xsl:template>


<xsl:template match="*" mode="content">
	<xsl:param name="listitem"/>
	<xsl:param name="attribute-set"/>
	
	<xsl:apply-templates mode="content">
		<xsl:with-param name="listitem" select="$listitem"/>
		<xsl:with-param name="attribute-set" select="$attribute-set"/>
	</xsl:apply-templates>
</xsl:template>

<!-- Stop processing descendents -->
<xsl:template match="*[@ektdesignns_nodetype='attribute-set']" mode="label"/>
<xsl:template match="*[@ektdesignns_nodetype='attribute-set']" mode="attributes"/>


<!-- element nodetype ========================================================================= -->


<!-- match any -->
<!-- note: ektdesignns_label is only applicable if element may contain child nodes -->

<xsl:template match="*[@ektdesignns_nodetype='element' or (starts-with(name(),'ektdesignns_') and not(@ektdesignns_nodetype) and not(@ektdesignns_display))]" mode="content">
	<xsl:param name="listitem"/>
	<xsl:param name="attribute-set"/>
	
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	
	<xsl:variable name="include">
		<xsl:call-template name="isIncluded"/>
	</xsl:variable>
	
	<xsl:if test="$include='true'">

		<xsl:variable name="id">
			<xsl:if test="@ektdesignns_label='before' or @ektdesignns_label='after'">
				<xsl:variable name="idAttr" select="string(.//*[@ektdesignns_nodetype='attribute' and @ektdesignns_name='id']/@value)"/>
				<xsl:choose>
					<xsl:when test="string-length($idAttr) &gt; 0">
						<xsl:value-of select="$idAttr"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat(generate-id(),$uniqueSuffix)"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:if>
		</xsl:variable>
		
		<xsl:if test="@ektdesignns_label='before'">
			<xsl:apply-templates mode="label">
				<xsl:with-param name="for" select="$id"/>
			</xsl:apply-templates>
		</xsl:if>
		
		<xsl:choose>
			<!-- the checklist element is a special case -->
			<!-- instead of <tag>value1 value2 value3</tag>, it produces <tag>value1</tag><tag>value2</tag><tag>value3</tag> -->
			<xsl:when test="name()='ektdesignns_checklist' or @ektdesignns_content='checklist'">
				<!-- note: descendant axis does not allow nested list -->
				<xsl:for-each select="descendant::input[@type='checkbox' and @ektdesignns_nodetype='item' and @checked]">
					<xsl:element name="{$name}">
						<!-- getAttributes (using a variable failed to output the attributes, so this duplicated code) -->
						<xsl:if test="$attribute-set">
							<!-- tree-frag: xsl:apply-templates select="$attribute-set" mode="attributes"/ -->
							<xsl:copy-of select="$attribute-set"/>
						</xsl:if>
						<xsl:if test="string-length($id) &gt; 0">
							<xsl:attribute name="id">
								<xsl:value-of select="$id"/>
							</xsl:attribute>
						</xsl:if>
						<xsl:apply-templates mode="attributes"/>
						
						<xsl:apply-templates select="." mode="enum"/> 

					</xsl:element>
					<xsl:text>&#13;&#10;</xsl:text>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="data">
					<xsl:element name="{$name}">
						<!-- getAttributes (using a variable failed to output the attributes, so this duplicated code) -->
						<xsl:if test="$attribute-set">
							<!-- tree-frag: xsl:apply-templates select="$attribute-set" mode="attributes"/ -->
							<xsl:copy-of select="$attribute-set"/>
						</xsl:if>
						<xsl:if test="string-length($id) &gt; 0">
							<xsl:attribute name="id">
								<xsl:value-of select="$id"/>
							</xsl:attribute>
						</xsl:if>
						<xsl:apply-templates mode="attributes"/>
				
						<xsl:choose>
							<xsl:when test="name()='ektdesignns_richarea'">
								<xsl:call-template name="processContent">
									<xsl:with-param name="default" select="'mixed'"/>
									<xsl:with-param name="listitem" select="$listitem"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:when test="name()='ektdesignns_checklist'">
								<xsl:call-template name="processContent">
									<xsl:with-param name="default" select="'checklist'"/>
									<xsl:with-param name="listitem" select="$listitem"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:when test="name()='ektdesignns_choices'">
								<xsl:call-template name="processContent">
									<xsl:with-param name="default" select="'choices'"/>
									<xsl:with-param name="listitem" select="$listitem"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:when test="name()='ektdesignns_calendar' or @class='design_calendar'">
								<xsl:call-template name="processContent">
									<xsl:with-param name="default" select="'date'"/>
									<xsl:with-param name="listitem" select="$listitem"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:when test="name()='ektdesignns_resource'">
								<xsl:call-template name="processContent">
									<xsl:with-param name="default" select="'datavalue'"/>
									<xsl:with-param name="listitem" select="$listitem"/>
								</xsl:call-template>
							</xsl:when>
							<xsl:otherwise>
								<!-- <xsl:text>&#13;&#10;</xsl:text> This breaks validation of enumeration -->
								<xsl:call-template name="processContent">
									<xsl:with-param name="listitem" select="$listitem"/>
								</xsl:call-template>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:element>
					<xsl:text>&#13;&#10;</xsl:text>
				</xsl:variable>
				<xsl:call-template name="copyData">
					<xsl:with-param name="data" select="$data"/>
					<!-- Can't easily know when to replicate by minoccurs. This transform
						may be creating initial document (in which case it should) or may
						be converting data entered to xml document (in which case it shouldn't).
						Could look at preceding-sibling and following-sibling to determine,
						but populating the initial document isn't that important and checking
						would slow down the transform. -->
					<!-- xsl:with-param name="numberOfTimes" select="@ektdesignns_minoccurs"/ -->
					<xsl:with-param name="numberOfTimes">
						<xsl:choose>
							<xsl:when test="starts-with(name(),'ektdesignns_') and @ektdesignns_minoccurs and not(msxsl:node-set($data)/*/node())">
								<xsl:value-of select="@ektdesignns_minoccurs"/>
							</xsl:when>
							<xsl:otherwise>1</xsl:otherwise>
						</xsl:choose>
					</xsl:with-param>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	
		<xsl:if test="@ektdesignns_label='after'">
			<xsl:apply-templates mode="label">
				<xsl:with-param name="for" select="$id"/>
			</xsl:apply-templates>
		</xsl:if>
		
	</xsl:if> <!-- required or included -->
</xsl:template>


<!-- Stop processing descendents -->
<xsl:template match="*[@ektdesignns_nodetype='element' or (starts-with(name(),'ektdesignns_') and not(@ektdesignns_nodetype) and not(@ektdesignns_display))]" mode="label"/>
<xsl:template match="*[@ektdesignns_nodetype='element' or (starts-with(name(),'ektdesignns_') and not(@ektdesignns_nodetype) and not(@ektdesignns_display))]" mode="attributes"/>


<!-- match input type=text|hidden -->
<!-- note: ektdesignns_label not supported -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden') and @ektdesignns_nodetype='element']" mode="content">
	<xsl:variable name="include">
		<xsl:call-template name="isIncluded"/>
	</xsl:variable>
	
	<xsl:if test="$include='true'">

		<xsl:variable name="name">
			<xsl:call-template name="getDesignName"/>
		</xsl:variable>
	
		<xsl:element name="{$name}">
			<xsl:if test="string-length(@value) &gt; 0">
				<xsl:value-of select="@value"/>
			</xsl:if>
		</xsl:element>
		<xsl:text>&#13;&#10;</xsl:text>
	
	</xsl:if> <!-- required or included -->
</xsl:template>


<!-- match input type=radio|checkbox -->
<!-- note: ektdesignns_label not supported -->

<xsl:template match="input[(@type='radio' or @type='checkbox') and @ektdesignns_nodetype='element']" mode="content">
	<xsl:call-template name="processBooleanInput"/>
</xsl:template>


<!-- match select/option -->
<!-- note: ektdesignns_label not supported -->
<!-- note: descendant axis does not allow nested select -->

<xsl:template match="select[@ektdesignns_nodetype='element']" mode="content">
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:element name="{$name}">
		<xsl:apply-templates select="descendant::option[@selected]" mode="enum"/> 
	</xsl:element>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>

<!-- the select multiple element is a special case -->
<!-- instead of <tag>value1 value2 value3</tag>, it produces <tag>value1</tag><tag>value2</tag><tag>value3</tag> -->
<xsl:template match="select[@multiple and @ektdesignns_nodetype='element']" mode="content">
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<!-- Remove duplicates -->
	<xsl:variable name="elements-rtf">
		<xsl:for-each select="descendant::option[@selected]">
			<xsl:element name="{$name}">
				<xsl:apply-templates select="." mode="enum"/> 
			</xsl:element>
			<xsl:text>&#13;&#10;</xsl:text>
		</xsl:for-each>
	</xsl:variable>
	<xsl:if test="string-length($elements-rtf)=0">
		<xsl:element name="{$name}">
		</xsl:element>
	</xsl:if>
	<xsl:variable name="elements" select="msxsl:node-set($elements-rtf)/*"/>
	<xsl:copy-of select="$elements[not(text()=preceding-sibling::*/text())]"/>
</xsl:template>


<!-- match textarea -->
<!-- note: ektdesignns_label not supported -->

<xsl:template match="textarea[@ektdesignns_nodetype='element']" mode="content">
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:element name="{$name}">
		<xsl:call-template name="processContent">
			<xsl:with-param name="default" select="'text'"/>
		</xsl:call-template>
	</xsl:element>
	<xsl:text>&#13;&#10;</xsl:text>
</xsl:template>


<!-- match tbody ektdesignns_listitem_template -->

<xsl:template match="tbody[@ektdesignns_listitem_template]" mode="content">
	<xsl:param name="attribute-set"/>

	<xsl:variable name="idref" select="@ektdesignns_listitem_template"/>
	<xsl:variable name="template" select="../tfoot//*[@id=$idref]"/>
	<xsl:for-each select="tr">
		<xsl:apply-templates select="$template" mode="content">
			<xsl:with-param name="listitem" select="."/>
			<xsl:with-param name="attribute-set" select="$attribute-set"/>
		</xsl:apply-templates>
	</xsl:for-each>
</xsl:template>

<xsl:template match="*[@ektdesignns_content='apply-item']" mode="content">
	<xsl:param name="listitem"/>
	<xsl:param name="attribute-set"/>
	
	<xsl:apply-templates select="msxsl:node-set($listitem)" mode="content">
		<xsl:with-param name="attribute-set" select="$attribute-set"/>
	</xsl:apply-templates>
</xsl:template>



<!-- mixed nodetype ========================================================================= -->


<!-- match any -->
<!-- note: any value attribute is ignored, therefore, this should not be used with input -->

<xsl:template match="*[@ektdesignns_nodetype='mixed']" mode="content"> 
	<!-- copy without namespace nodes -->
	<xsl:apply-templates mode="rich"/>
</xsl:template>

<xsl:template match="*[@ektdesignns_nodetype='mixed' and starts-with(@ektdesignns_content,'element=')]" mode="content">
	<xsl:variable name="elementName" select="substring-after(@ektdesignns_content,'element=')"/>
	<!-- copy without namespace nodes -->
	<xsl:apply-templates select="./*[name()=$elementName][not(starts-with(@class,'design_'))]" mode="rich"/>
</xsl:template>

<xsl:include href="template_richMode.xslt"/>

<!-- text nodetype ============================================================================ -->


<!-- match any -->

<xsl:template match="*[@ektdesignns_nodetype='text']" mode="content"> 
	<xsl:value-of select="."/>
</xsl:template>


<!-- match any with @ektdesignns_content='checklist' -->
<!-- note: descendant axis does not allow nested list -->

<xsl:template match="*[@ektdesignns_nodetype='text' and @ektdesignns_content='checklist']" mode="content"> 
	<xsl:apply-templates select="descendant::input[@type='checkbox' and @ektdesignns_nodetype='item' and @checked]" mode="enum"/> 
</xsl:template>


<!-- match any with @ektdesignns_content='choices' -->
<!-- note: descendant axis does not allow nested list -->

<xsl:template match="*[@ektdesignns_nodetype='text' and @ektdesignns_content='choices']" mode="content"> 
	<xsl:apply-templates select="descendant::input[@type='radio' and @ektdesignns_nodetype='item' and @checked]" mode="enum"/> 
</xsl:template>


<!-- match input type=text|hidden -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden') and @ektdesignns_nodetype='text']" mode="content">
	<xsl:value-of select="@value"/>
</xsl:template>


<!-- match input type=radio|checkbox -->

<xsl:template match="input[(@type='radio' or @type='checkbox') and @ektdesignns_nodetype='text']" mode="content">
	<xsl:call-template name="processBooleanInput"/>
</xsl:template>


<!-- match select/option -->
<!-- note: descendant axis does not allow nested select -->

<xsl:template match="select[@ektdesignns_nodetype='text']" mode="content">
	<xsl:apply-templates select="descendant::option[@selected]" mode="enum"/>
</xsl:template>




<!-- attribute nodetype ====================================================================== -->


<!-- match any -->

<xsl:template match="*[@ektdesignns_nodetype='attribute']" mode="attributes"> 
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:variable name="value" select="normalize-space(string(.))"/>
	<xsl:call-template name="createAttribute">
		<xsl:with-param name="name" select="$name"/>
		<xsl:with-param name="value" select="$value"/>
	</xsl:call-template>
</xsl:template>

<!-- match any with @ektdesignns_content='date', ektdesignns_calendar -->

<xsl:template match="*[@ektdesignns_nodetype='attribute' and @ektdesignns_content='date']|ektdesignns_calendar[@ektdesignns_nodetype='attribute']" mode="attributes">
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:call-template name="createAttribute">
		<xsl:with-param name="name" select="$name"/>
		<xsl:with-param name="value" select="@value"/>
	</xsl:call-template>
</xsl:template>

<!-- match any with @ektdesignns_content='element=*/@*' -->

<xsl:template match="*[@ektdesignns_nodetype='attribute' and starts-with(@ektdesignns_content,'element=') and contains(@ektdesignns_content,'/@')]" mode="attributes">
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:call-template name="createAttribute">
		<xsl:with-param name="name" select="$name"/>
		<xsl:with-param name="value">
			<xsl:call-template name="processContent"/>
		</xsl:with-param>
	</xsl:call-template>
</xsl:template>


<!-- match any with @ektdesignns_content='checklist' -->
<!-- note: descendant axis does not allow nested list -->

<xsl:template match="*[@ektdesignns_nodetype='attribute' and @ektdesignns_content='checklist']|ektdesignns_checklist[@ektdesignns_nodetype='attribute']" mode="attributes"> 
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:attribute name="{$name}">
		<xsl:apply-templates select="descendant::input[@type='checkbox' and @ektdesignns_nodetype='item' and @checked]" mode="enum"/> 
	</xsl:attribute>
</xsl:template>


<!-- match any with @ektdesignns_content='choices' -->
<!-- note: descendant axis does not allow nested list -->

<xsl:template match="*[@ektdesignns_nodetype='attribute' and @ektdesignns_content='choices']|ektdesignns_choices[@ektdesignns_nodetype='attribute']" mode="attributes"> 
	<xsl:variable name="value">
		<xsl:apply-templates select="descendant::input[@type='radio' and @ektdesignns_nodetype='item' and @checked]" mode="enum"/>
	</xsl:variable>
	<xsl:if test="string-length($value) &gt; 0">
		<xsl:variable name="name">
			<xsl:call-template name="getDesignName"/>
		</xsl:variable>
		<xsl:attribute name="{$name}">
			 <xsl:value-of select="$value"/>
		</xsl:attribute>
	</xsl:if>
</xsl:template>


<!-- match input type=text|hidden -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden') and @ektdesignns_nodetype='attribute']" mode="attributes">
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:call-template name="createAttribute">
		<xsl:with-param name="name" select="$name"/>
		<xsl:with-param name="value" select="@value"/>
	</xsl:call-template>
</xsl:template>


<!-- match input type=radio|checkbox -->

<xsl:template match="input[(@type='radio' or @type='checkbox') and @ektdesignns_nodetype='attribute']" mode="attributes">
	<xsl:call-template name="processBooleanInput"/>
</xsl:template>


<!-- match select/option -->
<!-- note: descendant axis does not allow nested select -->

<xsl:template match="select[@ektdesignns_nodetype='attribute']" mode="attributes">
	<xsl:call-template name="createAttribute">
		<xsl:with-param name="name">
			<xsl:call-template name="getDesignName"/>
		</xsl:with-param>
		<xsl:with-param name="value">
			<xsl:apply-templates select="descendant::option[@selected]" mode="enum"/> 
		</xsl:with-param>
	</xsl:call-template>
</xsl:template>



<!-- label nodetype ========================================================================== -->


<xsl:template match="*[@ektdesignns_nodetype='label']" mode="label">
	<xsl:param name="for"/>
	
	<xsl:element name="label">
		<xsl:attribute name="for"><xsl:value-of select="$for"/></xsl:attribute>
		<xsl:attribute name="contenteditable">true</xsl:attribute>
		<xsl:attribute name="unselectable">off</xsl:attribute>
		<xsl:apply-templates mode="attributes"/>
		<xsl:call-template name="processContent"/>
	</xsl:element>
</xsl:template>

<!-- Stop processing descendents -->
<xsl:template match="*[@ektdesignns_nodetype='label']" mode="attributes"/>
<xsl:template match="*[@ektdesignns_nodetype='label']" mode="content"/>


<!-- Continue processing descendents -->
<xsl:template match="*" mode="label">
	<xsl:param name="for"/>
	
	<xsl:apply-templates mode="label">
		<xsl:with-param name="for" select="$for"/>
	</xsl:apply-templates>
</xsl:template>



<!-- templates ========================================================================== -->


<!-- process input type=radio|checkbox -->

<xsl:template name="processBooleanInput">
	
	<xsl:variable name="name">
		<xsl:call-template name="getDesignName"/>
	</xsl:variable>
	<xsl:choose>
	
		<!-- optional -->
		<xsl:when test="string-length(@value) &gt; 0 and not(@value='true') and not(@value='on')">
			<xsl:if test="@checked">
				<xsl:call-template name="createNode">
					<xsl:with-param name="nodetype" select="@ektdesignns_nodetype"/>
					<xsl:with-param name="name" select="$name"/>
					<xsl:with-param name="value" select="string(@value)"/>
				</xsl:call-template>
			</xsl:if>
		</xsl:when>
		
		<!-- boolean -->
		<xsl:otherwise>
			<xsl:call-template name="createNode">
				<xsl:with-param name="nodetype" select="@ektdesignns_nodetype"/>
				<xsl:with-param name="name" select="$name"/>
				<xsl:with-param name="value" select="boolean(@checked)"/> <!-- XPath 1.0 boolean returns true if @checked exists, regardless of its value -->
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<xsl:template name="createNode">
	<xsl:param name="nodetype"/>
	<xsl:param name="name"/>
	<xsl:param name="value"/>
	
	<xsl:choose>
		<xsl:when test="$nodetype='element'">
			<xsl:element name="{$name}">
				<xsl:copy-of select="$value"/>
			</xsl:element>
			<xsl:text>&#13;&#10;</xsl:text>
		</xsl:when>
		<xsl:when test="$nodetype='attribute'">
			<xsl:attribute name="{$name}">
				<xsl:value-of select="$value"/>
			</xsl:attribute>
		</xsl:when>
		<xsl:when test="$nodetype='text'">
			<xsl:value-of select="$value"/>
		</xsl:when>
		<xsl:otherwise> <!-- 'mixed' or other -->
			<xsl:copy-of select="$value"/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- process selection list -->

<xsl:template match="input" mode="enum">
	<xsl:if test="position() &gt; 1">
		<xsl:text> </xsl:text>
	</xsl:if>
	<xsl:value-of select="@value"/>
</xsl:template>


<!-- process select/option -->

<xsl:template match="option" mode="enum">
	<!-- 
		Warning: don't use @selected as predicate in match expression because 
		elements that don't match will be processed by the built-in template
		because the mode is specified and it will render the content text.
	-->
	<xsl:if test="position() &gt; 1">
		<xsl:text> </xsl:text>
	</xsl:if>
	<xsl:choose>
		<xsl:when test="position() = 1 and ancestor::select[@ektdesignns_validation='select-req']">
			<xsl:value-of select="@value"/>
		</xsl:when>
		<xsl:when test="@value">
			<xsl:value-of select="@value"/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<xsl:include href="template_getDesignName.xslt"/>


<!-- createAttribute -->

<xsl:template name="createAttribute">
	<xsl:param name="name"/>
	<xsl:param name="value" select="''"/>
	
	<xsl:if test="string-length($value) &gt; 0">
		<xsl:attribute name="{$name}">
			<xsl:value-of select="$value"/>
		</xsl:attribute>
		
		<xsl:if test="$name='ektdesignns_nodetype' and $value='item'"> 
			<xsl:attribute name="name">
				<xsl:value-of select="concat(generate-id(),$uniqueSuffix)"/>
			</xsl:attribute>
		</xsl:if>
		
		<xsl:if test="$name='ektdesignns_validation' and $value!='none'">
			<xsl:attribute name="onblur">
				<xsl:text>design_validate(</xsl:text>
				<!-- regular expression -->
				<xsl:choose>
					<xsl:when test="$value='none'">/.*/</xsl:when>
					<xsl:when test="$value='string-req'">/\S+/</xsl:when>
					<xsl:when test="$value='nonNegInt'">/^\d*$/</xsl:when>
					<xsl:when test="$value='nonNegInt-req'">/^\d+$/</xsl:when>
					<xsl:when test="starts-with($value,'/')">
						<xsl:value-of select="$value"/>
					</xsl:when>
					<xsl:otherwise>/.*/</xsl:otherwise>
				</xsl:choose>
				<xsl:text>,this</xsl:text>
				<xsl:if test="@ektdesignns_content">
					<xsl:value-of select="concat(',this.',@ektdesignns_content)"/>
				</xsl:if>
				<xsl:text>)</xsl:text>
			</xsl:attribute>
		</xsl:if>

	</xsl:if>

</xsl:template>


<!-- processContent -->

<xsl:template name="processContent">
	<xsl:param name="default">content</xsl:param>
	<xsl:param name="listitem"/>
	
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
	
		<xsl:when test="starts-with($contentType,'element=')">
			<!-- see also DesignToEntryXSLT -->
			<xsl:choose>
				<xsl:when test="contains($contentType,'/@')">
					<xsl:variable name="elementName" select="substring-before(substring-after($contentType,'element='),'/@')"/>
					<xsl:variable name="attrName" select="substring-after($contentType,'/@')"/>
					<xsl:value-of select="(./*[name()=$elementName][not(starts-with(@class,'design_'))])/@*[name()=$attrName]"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:variable name="elementName" select="substring-after($contentType,'element=')"/>
					<!-- copy without namespace nodes -->
					<xsl:apply-templates select="./*[name()=$elementName][not(starts-with(@class,'design_'))]" mode="rich"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='mixed'">
			<xsl:choose>
				<xsl:when test="count(*)=1 and p[not(@*)] and normalize-space(text())='' and (string(p)='&#160;' or string(p)='') and (count(p/*)=0 or (count(p/*)=1 and p/br))"/>
				<xsl:when test="count(*|text())=1 and br[not(@id)]"/> <!-- may have class="khtml-block-placeholder" -->
				<xsl:when test="count(*)=0 and string(text())='&#160;'"/>
				<xsl:when test="count(node())=1 and count(text())=1">
					<xsl:value-of select="normalize-space(text())" />
				</xsl:when>
				<xsl:when test="count(*)=1 and p[not(@*)] and normalize-space(text())='' and not(@ektdesignns_datadisplay='block')">
					<xsl:apply-templates select="p/node()" mode="rich" />
				</xsl:when>
				<xsl:otherwise>
					<!-- copy without namespace nodes -->
					<xsl:apply-templates mode="rich"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='text'">
			<xsl:value-of select="."/>
		</xsl:when>
		
		<xsl:when test="$contentType='value'">
			<xsl:value-of select="@value"/>
		</xsl:when>
		
		<xsl:when test="$contentType='datavalue'">
			<xsl:copy-of select="@*[starts-with(name(),'datavalue_')]"/>
			<xsl:value-of select="@datavalue"/>
		</xsl:when>
		
		<xsl:when test="$contentType='date'">
			<xsl:value-of select="@datavalue"/>
		</xsl:when>
		
		<xsl:when test="$contentType='checklist'">
			<!-- note: descendant axis does not allow nested list -->
			<xsl:apply-templates select="descendant::input[@type='checkbox' and @ektdesignns_nodetype='item' and @checked]" mode="enum"/> 
		</xsl:when>
		
		<xsl:when test="$contentType='choices'">
			<!-- note: descendant axis does not allow nested list -->
			<xsl:apply-templates select="descendant::input[@type='radio' and @ektdesignns_nodetype='item' and @checked]" mode="enum"/> 
		</xsl:when>
		
		<xsl:otherwise> <!-- 'content' -->
			<xsl:apply-templates mode="content">
				<xsl:with-param name="listitem" select="$listitem"/>
			</xsl:apply-templates>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- isIncluded -->

<xsl:template name="isIncluded">
	<xsl:choose>
		<xsl:when test="@ektdesignns_isrelevant='false'">false</xsl:when>
		<xsl:when test="(@ektdesignns_minoccurs='0' or @ektdesignns_use='optional') and not(@ektdesignns_maxoccurs='unbounded' or number(@ektdesignns_maxoccurs) &gt; 1)">
			<!-- source: tbody/tr[2]/td/* target: tbody/tr[1]/td/input/@checked -->
			<xsl:variable name="checkbox" select="../../../tr[1]/td/input[@name='include']"/>
			<xsl:value-of select="not($checkbox) or boolean($checkbox/@checked)"/>
		</xsl:when>
		<xsl:otherwise>true</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- ignore corrupt TR #28834 -->
<xsl:template match="table/tr" priority="10.0" mode="label"/>
<xsl:template match="table/tr" priority="10.0" mode="attributes"/>
<xsl:template match="table/tr" priority="10.0" mode="content"/>
<xsl:template match="table/tr" priority="10.0"/>

<!-- ignore prototypes -->
<xsl:template match="*[@class='design_prototype']" mode="label"/>
<xsl:template match="*[@class='design_prototype']" mode="attributes"/>
<xsl:template match="*[@class='design_prototype']" mode="content"/>
<xsl:template match="*[@class='design_prototype']"/>


<!-- ignore disabled elements -->
<xsl:template match="*[@disabled]" mode="label"/>
<xsl:template match="*[@disabled]" mode="attributes"/>
<xsl:template match="*[@disabled]" mode="content"/>
<xsl:template match="*[@disabled]"/>


<!-- remove text and attribute values -->
<xsl:template match="text()|@*" mode="label"/>
<xsl:template match="text()|@*" mode="attributes"/>
<xsl:template match="text()|@*" mode="content"/>
<xsl:template match="text()|@*"/>

</xsl:stylesheet>
