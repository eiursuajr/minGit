<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema">

<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

<!--

	The content may be processed up to two times.
	Each scan correspsonds to a mode:
		elements
		attributes
		
	1. mode="elements". The content is scanned for descendent elements.
	
	2. mode="attributes". The content is scanned for child attributes.
	
-->


<xsl:template match="/">
	<xs:schema elementFormDefault="qualified" attributeFormDefault="unqualified">
		<xsl:choose>
			<xsl:when test="//*[@ektdesignns_role='root' and not(ancestor::*/@ektdesignns_name)]">
				<xsl:apply-templates mode="elements"/>
			</xsl:when>
			<xsl:otherwise>
				<xs:element name="root">
					<xsl:call-template name="processContent"/>
				</xs:element>
			</xsl:otherwise>
		</xsl:choose>

		<xsl:if test="//ektdesignns_calendar">
		<xs:simpleType name="dateOrBlankType">
			<xs:union memberTypes="xs:date">
				<xs:simpleType>
					<xs:restriction base="xs:string">
						<xs:length value="0" />
					</xs:restriction>
				</xs:simpleType>
			</xs:union>
		</xs:simpleType>
		</xsl:if>

		<xsl:variable name="hasFileLink" select="boolean(//ektdesignns_filelink)"/>
		<xsl:variable name="hasImageOnly" select="boolean(//ektdesignns_imageonly)"/>

		<xsl:if test="$hasFileLink or $hasImageOnly">
		<!-- Adapted from xhtml1-transitional.xsd
			XHTML 1.0 (Second Edition) Transitional in XML Schema
			For further information, see: http://www.w3.org/TR/xhtml1
			Copyright (c) 1998-2002 W3C (MIT, INRIA, Keio), All Rights Reserved. 
		-->
		<xs:attributeGroup name="coreattrs">
			<xs:attribute name="id" type="xs:ID"/>
			<xs:attribute name="class" type="xs:NMTOKENS"/>
			<xs:attribute name="style" type="xs:string"/>
			<xs:attribute name="title" type="xs:string"/>
		</xs:attributeGroup>
		
		<xs:attributeGroup name="i18n">
			<xs:attribute name="lang" type="xs:language"/>
			<!-- xs:attribute ref="xml:lang"/ -->
			<xs:attribute name="dir">
				<xs:simpleType>
					<xs:restriction base="xs:token">
						<xs:enumeration value="ltr"/>
						<xs:enumeration value="rtl"/>
					</xs:restriction>
				</xs:simpleType>
			</xs:attribute>
		</xs:attributeGroup>
		
		<xs:attributeGroup name="attrs">
			<xs:attributeGroup ref="coreattrs"/>
			<xs:attributeGroup ref="i18n"/>
			<!-- xs:attributeGroup ref="events"/ -->
		</xs:attributeGroup>
		</xsl:if>

		<xsl:if test="$hasFileLink">
		<xs:simpleType name="FrameTarget">
			<xs:restriction base="xs:NMTOKEN">
				<xs:pattern value="_(blank|self|parent|top)|[A-Za-z]\c*"/>
			</xs:restriction>
		</xs:simpleType>
		
		<xs:complexType name="aDesignType" mixed="true"> <!-- name must be {elementName}DesignType -->
			<xs:sequence>
				<!-- to avoid including most of the XHTML schema, we'll just allow anything. -->
				<xs:any namespace="##any" processContents="skip" minOccurs="0" maxOccurs="unbounded"/>
			</xs:sequence>
			<xs:attributeGroup ref="attrs"/>
			<!-- xs:attributeGroup ref="focus"/ -->
			<!-- xs:attribute name="charset" type="Charset"/ -->
			<!-- xs:attribute name="type" type="ContentType"/ -->
			<!-- xs:attribute name="name" type="xs:NMTOKEN"/ -->
			<xs:attribute name="href" type="xs:anyURI"/>
			<!-- xs:attribute name="hreflang" type="LanguageCode"/ -->
			<!-- xs:attribute name="rel" type="LinkTypes"/ -->
			<!-- xs:attribute name="rev" type="LinkTypes"/ -->
			<!-- xs:attribute name="shape" default="rect" type="Shape"/ -->
			<!-- xs:attribute name="coords" type="Coords"/ -->
			<xs:attribute name="target" type="FrameTarget"/>
		</xs:complexType>
		</xsl:if>

		<xsl:if test="$hasImageOnly">
		<xs:simpleType name="ImgAlign">
			<xs:restriction base="xs:token">
				<xs:enumeration value="top"/>
				<xs:enumeration value="middle"/>
				<xs:enumeration value="bottom"/>
				<xs:enumeration value="left"/>
				<xs:enumeration value="right"/>
			</xs:restriction>
		</xs:simpleType>

		<xs:simpleType name="Length">
			<xs:restriction base="xs:string">
				<xs:pattern value="[-+]?(\d+|\d+(\.\d+)?%)"/>
			</xs:restriction>
		</xs:simpleType>

		<xs:complexType name="imgDesignType"> <!-- name must be {elementName}DesignType -->
			<xs:attributeGroup ref="attrs"/>
			<xs:attribute name="src" use="required" type="xs:anyURI"/>
			<xs:attribute name="alt" use="required" type="xs:string"/>
			<!-- xs:attribute name="name" type="xs:NMTOKEN"/ -->
			<!-- xs:attribute name="longdesc" type="URI"/ -->
			<xs:attribute name="height" type="Length"/>
			<xs:attribute name="width" type="Length"/>
			<!-- xs:attribute name="usemap" type="URI"/ -->
			<!-- xs:attribute name="ismap">
				<xs:simpleType>
					<xs:restriction base="xs:token">
						<xs:enumeration value="ismap"/>
					</xs:restriction>
				</xs:simpleType>
			</xs:attribute -->
			<xs:attribute name="align" type="ImgAlign"/>
			<xs:attribute name="border" type="Length"/>
			<xs:attribute name="hspace" type="xs:nonNegativeInteger"/>
			<xs:attribute name="vspace" type="xs:nonNegativeInteger"/>
		</xs:complexType>
		</xsl:if>

		<xsl:if test="//ektdesignns_richarea">
		<xs:complexType name="rich" mixed="true">
			<xs:sequence>
				<xs:any namespace="##any" processContents="skip" minOccurs="0" maxOccurs="unbounded"/>
			</xs:sequence>
		</xs:complexType>
		</xsl:if>

		<xsl:if test="//ektdesignns_resource">
		<xs:simpleType name="idtypeType">
			<xs:restriction base="xs:NMTOKEN">
				<xs:pattern value="[a-z]+(:[\w\d\.\-]+)*"/>
			</xs:restriction>
		</xs:simpleType>
		<xs:complexType name="IdRefType">
			<xs:simpleContent>
				<xs:extension base="xs:long">
					<xs:attribute name="datavalue_idtype" use="required" type="idtypeType" />
					<xs:attribute name="datavalue_displayvalue" type="xs:string" use="optional" />
				</xs:extension>
			</xs:simpleContent>
		</xs:complexType>
		<xs:complexType name="ContentIdRefType">
			<xs:simpleContent>
				<xs:restriction base="IdRefType">
					<xs:attribute name="datavalue_idtype" use="required">
						<xs:simpleType>
							<xs:restriction base="idtypeType">
								<xs:pattern value="content(:[\w\d\.\-]+)*"/>
							</xs:restriction>
						</xs:simpleType>
					</xs:attribute>
				</xs:restriction>
			</xs:simpleContent>
		</xs:complexType>
		<xs:complexType name="FolderIdRefType">
			<xs:simpleContent>
				<xs:restriction base="IdRefType">
					<xs:attribute name="datavalue_idtype" use="required">
						<xs:simpleType>
							<xs:restriction base="idtypeType">
								<xs:pattern value="folder(:[\w\d\.\-]+)*"/>
							</xs:restriction>
						</xs:simpleType>
					</xs:attribute>
				</xs:restriction>
			</xs:simpleContent>
		</xs:complexType>
		</xsl:if>

	</xs:schema>
</xsl:template>



<!-- element nodetype ========================================================================= -->


<!-- match any -->

<xsl:template match="*[(@ektdesignns_nodetype='element' or (starts-with(name(),'ektdesignns_') and not(@ektdesignns_nodetype))) and @ektdesignns_name]" mode="elements">
	<xs:element name="{@ektdesignns_name}">
		<xsl:call-template name="getOccurrence"/>
		
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_richarea'">
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'mixed'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_checklist' or @ektdesignns_content='checklist'">
				<!-- the checklist element is a special case -->
				<!-- instead of <tag>value1 value2 value3</tag>, it produces <tag>value1</tag><tag>value2</tag><tag>value3</tag> -->
				<xsl:variable name="items" select="descendant::input[@type='checkbox' and @ektdesignns_nodetype='item']"/>
				<xsl:call-template name="setOccurrence">
					<xsl:with-param name="minOccurs">
						<xsl:choose>
							<xsl:when test="number(*/@ektdesignns_minoccurs) > 0">
								<xsl:value-of select="*/@ektdesignns_minoccurs"/>
							</xsl:when>
							<xsl:otherwise>0</xsl:otherwise>
						</xsl:choose>
					</xsl:with-param> 
					<xsl:with-param name="maxOccurs">
						<xsl:choose>
							<xsl:when test="number(*/@ektdesignns_maxoccurs)">
								<xsl:value-of select="*/@ektdesignns_maxoccurs"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="'unbounded'"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:with-param>
				</xsl:call-template>
				<xsl:choose>
					<xsl:when test="@ektdesignns_datalist">
						<!-- list is dynamic, so don't create a static enumeration -->
						<xsl:call-template name="createType"/>
					</xsl:when>
					<xsl:otherwise>
						<xs:simpleType>
							<xs:restriction>
								<xsl:call-template name="createBase"/>
								<xsl:apply-templates select="$items" mode="enum"/>
							</xs:restriction>
						</xs:simpleType>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_choices'">
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'choices'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_calendar'">
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'date'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_resource'">
				<xsl:call-template name="getDefault"/>
				<xsl:attribute name="type">
					<xsl:choose>
						<xsl:when test="starts-with(@datavalue_idtype,'content')">ContentIdRefType</xsl:when>
						<xsl:when test="starts-with(@datavalue_idtype,'folder')">FolderIdRefType</xsl:when>
						<xsl:otherwise>IdRefType</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="processContent"/>
			</xsl:otherwise>
		</xsl:choose>
	</xs:element>
</xsl:template>

<xsl:template match="*[starts-with(name(),'ektdesignns_merge')]" mode="elements"/>


<!-- Stop processing descendents -->
<xsl:template match="*[@ektdesignns_nodetype='element' or (starts-with(name(),'ektdesignns_') and not(@ektdesignns_nodetype))]" mode="attributes"/>


<!-- match input type=text|hidden -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden' or @type='password') and @ektdesignns_nodetype='element']" mode="elements">
	<xs:element name="{@ektdesignns_name}">
		<xsl:call-template name="getOccurrence"/>
		<xsl:call-template name="processInput"/>
	</xs:element>
</xsl:template>


<!-- match input type=checkbox -->

<xsl:template match="input[@type='checkbox' and @ektdesignns_nodetype='element']" mode="elements">
	<xsl:choose>
	
		<!-- optional -->
		<xsl:when test="string-length(@value) &gt; 0 and not(@value='true') and not(@value='on')">
			<xs:element name="{@ektdesignns_name}" minOccurs="0">
				<xsl:call-template name="getOccurrence"/>
				<xsl:call-template name="getDefault"/>
				<xs:simpleType>
					<xs:restriction>
						<xsl:call-template name="createBase"/>
						<xs:enumeration value="{@value}"/>
					</xs:restriction>
				</xs:simpleType>
			</xs:element>
		</xsl:when>
		
		<!-- boolean -->
		<xsl:otherwise>
			<xs:element name="{@ektdesignns_name}" type="xs:boolean">
				<xsl:call-template name="getOccurrence"/>
				<xsl:choose>
					<xsl:when test="@checked">
						<xsl:call-template name="getDefault">
							<xsl:with-param name="value" select="'true'" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="getDefault">
							<xsl:with-param name="value" select="'false'" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xs:element>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- match select/option -->

<xsl:template match="select[@ektdesignns_nodetype='element']" mode="elements">
	<xs:element name="{@ektdesignns_name}">
		<xsl:call-template name="getOccurrence"/>
		<xsl:call-template name="getDefaultSelectOption"/>
		<xsl:choose>
			<xsl:when test="@ektdesignns_datalist">
				<!-- list is dynamic, so don't create a static enumeration -->
				<xsl:call-template name="createType"/>
			</xsl:when>
			<xsl:otherwise>
				<xs:simpleType>
					<xs:restriction>
						<xsl:call-template name="createBase"/>
						<xsl:if test="number(@ektdesignns_minoccurs)=0">
							<xs:enumeration value=""/>
						</xsl:if>
						<xsl:choose>
							<xsl:when test="@ektdesignns_validation='select-req' and (not(@size) or number(@size)=1)">
								<xsl:apply-templates select="descendant::option[position() != 1]" mode="enum"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:apply-templates select="descendant::option" mode="enum"/>
							</xsl:otherwise>
						</xsl:choose>
					</xs:restriction>
				</xs:simpleType>
			</xsl:otherwise>
		</xsl:choose>
	</xs:element>
</xsl:template>

<xsl:template match="select[@multiple and @ektdesignns_nodetype='element']" mode="elements"> 
	<xs:element name="{@ektdesignns_name}">
		<xsl:call-template name="getOccurrence">
			<xsl:with-param name="minOccurs" select="'0'"/>
			<xsl:with-param name="maxOccurs" select="'unbounded'"/>
		</xsl:call-template>
		<!-- no default because value is a list -->

		<xsl:choose>
			<xsl:when test="@ektdesignns_datalist">
				<!-- list is dynamic, so don't create a static enumeration -->
				<xsl:call-template name="createType"/>
			</xsl:when>
			<xsl:otherwise>
				<xs:simpleType>
					<xs:restriction>
						<xsl:call-template name="createBase"/>
						<xsl:apply-templates select="descendant::option" mode="enum"/>
					</xs:restriction>
				</xs:simpleType>
			</xsl:otherwise>
		</xsl:choose>
	</xs:element>
</xsl:template>


<!-- match textarea -->

<xsl:template match="textarea[@ektdesignns_nodetype='element']" mode="elements">
	<xs:element name="{@ektdesignns_name}">
		<xsl:call-template name="getOccurrence"/>
		<xsl:call-template name="getDefault">
			<xsl:with-param name="value" select="text()"/>
		</xsl:call-template>
		<xsl:call-template name="createType"/>
	</xs:element>
</xsl:template>



<!-- mixed nodetype ========================================================================= -->


<!-- mixed="true" attribute in complexType should have already been set -->
<!-- Note: namespace="##any" may make the schema non-deterministic (e.g., other child elements), but 
 		should not use ##other because the application may not use another namespace. -->
<xsl:template match="*[@ektdesignns_nodetype='mixed']" mode="elements">
	<xs:any namespace="##any" processContents="skip" minOccurs="0" maxOccurs="unbounded"/>
</xsl:template>

<xsl:template match="*[@ektdesignns_nodetype='mixed' and starts-with(@ektdesignns_content,'element=')]" mode="elements">
	<xsl:variable name="elementName" select="substring-after(@ektdesignns_content,'element=')"/>
	<xs:element name="{$elementName}" type="{$elementName}DesignType">
		<xsl:attribute name="minOccurs">
			<xsl:choose>
				<xsl:when test="@ektdesignns_validation='content-req'">1</xsl:when>
				<xsl:otherwise>0</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xs:element>
</xsl:template>

<!-- Stop processing descendents -->
<xsl:template match="*[@ektdesignns_nodetype='mixed']" mode="attributes"/>



<!-- text nodetype ========================================================================= -->


<!-- mixed="true" attribute in complexType should have already been set -->
<xsl:template match="*[@ektdesignns_nodetype='text']" mode="elements"/>

<!-- Stop processing descendents -->
<xsl:template match="*[@ektdesignns_nodetype='text']" mode="attributes"/>


<!-- attribute nodetype ====================================================================== -->


<!-- match any -->

<xsl:template match="*[@ektdesignns_nodetype='attribute']|ektdesignns_checklist|ektdesignns_choices|ektdesignns_calendar" mode="attributes">
	<xs:attribute name="{@ektdesignns_name}">
		<xsl:call-template name="getUse"/>
		
		<xsl:choose>
			<xsl:when test="name()='ektdesignns_checklist'">
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'checklist'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_choices'">
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'choices'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="name()='ektdesignns_calendar'">
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'date'"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="processContent">
					<xsl:with-param name="default" select="'text'"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xs:attribute>
</xsl:template>



<!-- match input type=text|hidden -->

<xsl:template match="input[((not(@type) or @type='text') or @type='hidden' or @type='password') and @ektdesignns_nodetype='attribute']" mode="attributes">
	<xs:attribute name="{@ektdesignns_name}">
		<xsl:call-template name="getUse"/>
		<xsl:call-template name="processInput"/>
	</xs:attribute>
</xsl:template>



<!-- match input type=checkbox -->

<xsl:template match="input[@type='checkbox' and @ektdesignns_nodetype='attribute']" mode="attributes">
	<xsl:choose>
	
		<!-- optional -->
		<xsl:when test="string-length(@value) &gt; 0 and not(@value='true') and not(@value='on')">
			<xs:attribute name="{@ektdesignns_name}" use="optional">
				<xsl:call-template name="getUse"/>
				<xsl:call-template name="getDefault"/>
				<xs:simpleType>
					<xs:restriction>
						<xsl:call-template name="createBase"/>
						<xs:enumeration value="{@value}"/>
					</xs:restriction>
				</xs:simpleType>
			</xs:attribute>
		</xsl:when>
		
		<!-- boolean -->
		<xsl:otherwise>
			<xs:attribute name="{@ektdesignns_name}" type="xs:boolean">
				<xsl:call-template name="getUse"/>
				<xsl:choose>
					<xsl:when test="@checked">
						<xsl:call-template name="getDefault">
							<xsl:with-param name="value" select="'true'" />
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="getDefault">
							<xsl:with-param name="value" select="'false'" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xs:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- match select/option -->

<xsl:template match="select[@ektdesignns_nodetype='attribute']" mode="attributes">
	<xs:attribute name="{@ektdesignns_name}">
		<xsl:call-template name="getUse"/>
		<xsl:call-template name="getDefaultSelectOption"/>
		<xs:simpleType>
			<xs:restriction>
				<xsl:call-template name="createBase"/>
				<xsl:choose>
					<xsl:when test="@ektdesignns_datalist">
					</xsl:when>
					<xsl:when test="@ektdesignns_validation='select-req'">
						<xsl:apply-templates select="descendant::option[position() != 1]" mode="enum"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="descendant::option" mode="enum"/>
					</xsl:otherwise>
				</xsl:choose>
			</xs:restriction>
		</xs:simpleType>
	</xs:attribute>
</xsl:template>

<xsl:template match="select[@multiple and @ektdesignns_nodetype='attribute']" mode="attributes">
	<xs:attribute name="{@ektdesignns_name}">
		<xsl:call-template name="getUse"/>
		<!-- no default because value is a list -->
		<xs:simpleType>
			<xs:list>
				<xs:simpleType>
					<xs:restriction>
						<xsl:call-template name="createBase"/>
						<xsl:if test="not(@ektdesignns_datalist)">
							<xsl:apply-templates select="descendant::option" mode="enum"/>
						</xsl:if>
					</xs:restriction>
				</xs:simpleType>
			</xs:list>
		</xs:simpleType>
	</xs:attribute>
</xsl:template>



<!-- templates ========================================================================== -->


<!-- processContent -->

<xsl:template name="processContent">
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
	
		<xsl:when test="starts-with($contentType,'element=')">
			<xsl:choose>
				<xsl:when test="contains($contentType,'/@')">
					<xsl:variable name="elementName" select="substring-before(substring-after($contentType,'element='),'/@')"/>
					<xsl:variable name="attrName" select="substring-after($contentType,'/@')"/>
					<xsl:call-template name="getDefault">
						<xsl:with-param name="value" select="(./*[name()=$elementName][not(starts-with(@class,'design_'))])/@*[name()=$attrName]"/>
					</xsl:call-template>
					<xsl:call-template name="createType"/>
				</xsl:when>
				<xsl:otherwise>
					<!-- no default because value is mixed, default must be simpleType -->
					<xs:complexType>
						<xs:sequence>
							<xsl:choose>
								<xsl:when test="@ektdesignns_schema">
									<xsl:value-of select="@ektdesignns_schema" disable-output-escaping="yes"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:variable name="elementName" select="substring-after($contentType,'element=')"/>
									<xs:element name="{$elementName}" type="{$elementName}DesignType">
										<xsl:attribute name="minOccurs">
											<xsl:choose>
												<xsl:when test="@ektdesignns_validation='content-req'">1</xsl:when>
												<xsl:otherwise>0</xsl:otherwise>
											</xsl:choose>
										</xsl:attribute>
									</xs:element>
								</xsl:otherwise>
							</xsl:choose>
						</xs:sequence>
					</xs:complexType>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
	
		<xsl:when test="$contentType='mixed'">
			<!-- no default because value is mixed, default must be simpleType -->
			<xsl:choose>
				<xsl:when test="@ektdesignns_schema">
					<xs:complexType mixed="true">
						<xs:sequence>
							<xsl:value-of select="@ektdesignns_schema" disable-output-escaping="yes"/>
						</xs:sequence>
					</xs:complexType>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="type">rich</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='text'">
			<xsl:call-template name="getDefault">
				<xsl:with-param name="value" select="text()"/>
			</xsl:call-template>
			<xsl:call-template name="createType"/>
		</xsl:when>
		
		<xsl:when test="$contentType='value'">
			<xsl:call-template name="getDefault"/>
			<xsl:call-template name="createType"/>
		</xsl:when>
		
		<xsl:when test="$contentType='checklist'">
			<!-- no default because value is a list -->
			<xs:simpleType>
				<xs:list>
					<xs:simpleType>
						<xs:restriction>
							<xsl:call-template name="createBase"/>
							<xsl:if test="number(*/@ektdesignns_minoccurs) > 0">
								<xs:minLength value="{*/@ektdesignns_minoccurs}"/>
							</xsl:if>
							<xsl:if test="number(*/@ektdesignns_maxoccurs)">
								<xs:maxLength value="{*/@ektdesignns_maxoccurs}"/>
							</xsl:if>
							<xsl:if test="not(@ektdesignns_datalist)">
								<xsl:apply-templates select="descendant::input[@type='checkbox' and @ektdesignns_nodetype='item']" mode="enum"/>
							</xsl:if>
						</xs:restriction>
					</xs:simpleType>
				</xs:list>
			</xs:simpleType>
		</xsl:when>
		
		<xsl:when test="$contentType='choices'">
			<xsl:call-template name="getDefault">
				<xsl:with-param name="value" select="descendant::input[@checked and @type='radio' and @ektdesignns_nodetype='item'][1]/@value"/>
			</xsl:call-template>
			<xsl:choose>
				<xsl:when test="@ektdesignns_datalist">
					<xsl:if test="not(number(*/@ektdesignns_minoccurs) > 0)"> <!-- only 0 (default) or 1 make sense -->
						<xsl:attribute name="minOccurs">0</xsl:attribute>
					</xsl:if>
					<!-- list is dynamic, so don't create a static enumeration -->
					<xsl:call-template name="createType"/>
				</xsl:when>
				<xsl:otherwise>
					<xs:simpleType>
						<xs:restriction>
							<xsl:call-template name="createBase"/>
							<!-- xs:list allows an empty value, but here we need to explicitly allow it -->
							<xsl:if test="not(number(*/@ektdesignns_minoccurs) > 0)"> <!-- only 0 (default) or 1 make sense -->
								<xs:enumeration value=""/>
							</xsl:if>
							<xsl:apply-templates select="descendant::input[@type='radio' and @ektdesignns_nodetype='item']" mode="enum"/>
						</xs:restriction>
					</xs:simpleType>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:when test="$contentType='date'">
			<xsl:call-template name="getDefault"/>
			<xsl:choose>
				<xsl:when test="@ektdesignns_schema">
					<xsl:call-template name="createType">
						<xsl:with-param name="default" select="'xs:date'"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:when test="@ektdesignns_datatype!='string'">
					<xsl:attribute name="type">
						<xsl:value-of select="concat('xs:',@ektdesignns_datatype)"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="type">dateOrBlankType</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		
		<xsl:otherwise> <!-- 'content' -->
			<xs:complexType>
				<!-- This test may set mixed even if it should not be, but it shouldn't matter. -->
				<!-- The robust algorithm would apply-templates in a mode="mixed" to determine
				 		if any nodetype is 'mixed' or 'text'. -->
				<xsl:if test="descendant::*[@ektdesignns_nodetype='mixed' or @ektdesignns_nodetype='text']">
					<xsl:attribute name="mixed">true</xsl:attribute>
				</xsl:if>
				<xs:sequence>
					<xsl:apply-templates mode="elements"/>
				</xs:sequence>
				<xsl:apply-templates mode="attributes"/>
			</xs:complexType>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="input" mode="enum">
	<xs:enumeration value="{@value}" />
</xsl:template>


<!-- processInput -->

<xsl:template name="processInput">
	<xsl:call-template name="getDefault"/>
	<xsl:call-template name="createType"/>
	<!-- 
	
	NOTE: Now that custom types are supported, it doesn't make sense to force the maxLength facet.
	
	WARNING: The maxLength facet is only allowed with certain simple data types (e.g., string), but
	not others (e.g., integer). It is also not allowed when the length facet is specified and must be
	larger than minLength. Given these potential hazards, it doesn't make sense to set the maxLength
	facet based on the maxlength attribute of the INPUT tag.
	
	<xsl:choose>
		<xsl:when test="@maxlength">
			<xs:simpleType>
				<xs:restriction>
					<xsl:call-template name="createBase"/>
					<xs:maxLength value="{@maxlength}" />
				</xs:restriction>
			</xs:simpleType>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="createType"/>
		</xsl:otherwise>
	</xsl:choose>
	-->
</xsl:template>


<!-- process select/option -->

<xsl:template match="option" mode="enum">
	<xsl:choose>
		<xsl:when test="@value">
			<xs:enumeration value="{@value}"/>
		</xsl:when>
		<xsl:otherwise>
			<xs:enumeration value="."/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- returns the schema type of the context element -->

<xsl:template name="createType">
	<xsl:param name="default" select="'xs:string'"/>
	<xsl:choose>
		<xsl:when test="@ektdesignns_schema or (@ektdesignns_validation and (not(@ektdesignns_datatype) or contains(@ektdesignns_datatype,':')))">
			<xs:simpleType>
				<xs:restriction>
					<xsl:call-template name="createBase">
						<xsl:with-param name="default" select="$default"/>
					</xsl:call-template>
				</xs:restriction>
			</xs:simpleType>
		</xsl:when>
		<xsl:when test="@ektdesignns_datatype[not(contains(.,':'))]">
			<xsl:attribute name="type">
				<xsl:value-of select="concat('xs:',@ektdesignns_datatype)"/>
			</xsl:attribute>
		</xsl:when>
		<xsl:otherwise>
			<xsl:attribute name="type">
				<xsl:value-of select="$default"/>
			</xsl:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="createBase">
	<xsl:param name="default" select="'xs:string'"/>
	<xsl:choose>
		<xsl:when test="@ektdesignns_datatype[not(contains(.,':'))]|@ektdesignns_schema">
			<xsl:if test="@ektdesignns_datatype[not(contains(.,':'))]">
				<xsl:attribute name="base">
					<xsl:value-of select="concat('xs:',@ektdesignns_datatype)"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@ektdesignns_schema">
				<xsl:value-of select="@ektdesignns_schema" disable-output-escaping="yes"/>
			</xsl:if>
		</xsl:when>
		<!-- using ektdesignns_validation for schema defn is legacy -->
		<xsl:when test="@ektdesignns_validation='none'">
			<xsl:attribute name="base">xs:string</xsl:attribute>
		</xsl:when>
		<xsl:when test="@ektdesignns_validation='string-req' or @ektdesignns_validation='select-req'">
			<xsl:attribute name="base">xs:string</xsl:attribute>
			<xsl:choose>
				<xsl:when test="@ektdesignns_minlength">
					<xs:minLength value="{@ektdesignns_minlength}"/> 
				</xsl:when>
				<xsl:otherwise>
					<xs:minLength value="1"/> 
				</xsl:otherwise>
			</xsl:choose>
		</xsl:when>
		<xsl:when test="@ektdesignns_validation='nonNegInt'">
			<!-- may be empty so can't use numeric type -->
			<xs:simpleType>
				<xs:union memberTypes="xs:nonNegativeInteger">
					<xs:simpleType>
						<xs:restriction base="xs:string">
							<xs:length value="0"/>
						</xs:restriction>
					</xs:simpleType>
				</xs:union>
			</xs:simpleType>
		</xsl:when>
		<xsl:when test="@ektdesignns_validation='nonNegInt-req'">
			<xsl:attribute name="base">xs:nonNegativeInteger</xsl:attribute>
		</xsl:when>
		<xsl:otherwise>
			<xsl:attribute name="base">
				<xsl:value-of select="$default"/>
			</xsl:attribute>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!-- element occurrence, minOccurs and maxOccurs -->

<xsl:template name="getOccurrence">
	<xsl:if test="(@ektdesignns_minoccurs and number(@ektdesignns_minoccurs)!=1) or @ektdesignns_use='optional' or @ektdesignns_relevant">
		<xsl:attribute name="minOccurs">
			<xsl:choose>
				<xsl:when test="@ektdesignns_minoccurs">
					<xsl:value-of select="@ektdesignns_minoccurs"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>0</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:if>
	<xsl:if test="(@ektdesignns_maxoccurs and number(@ektdesignns_maxoccurs)!=1)">
		<xsl:attribute name="maxOccurs">
			<xsl:choose>
				<xsl:when test="@ektdesignns_maxoccurs">
					<xsl:value-of select="@ektdesignns_maxoccurs"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>unbounded</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:if>
</xsl:template>

<!-- element occurrence, minOccurs and maxOccurs -->

<xsl:template name="setOccurrence">
	<xsl:param name="minOccurs">1</xsl:param>
	<xsl:param name="maxOccurs">1</xsl:param>
	
	<xsl:attribute name="minOccurs">
		<xsl:choose>
			<xsl:when test="@ektdesignns_minoccurs">
				<xsl:value-of select="@ektdesignns_minoccurs"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$minOccurs"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:attribute>
	
	<xsl:attribute name="maxOccurs">
		<xsl:choose>
			<xsl:when test="@ektdesignns_maxoccurs">
				<xsl:value-of select="@ektdesignns_maxoccurs"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$maxOccurs"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:attribute>
</xsl:template>
		


<!-- attribute use: optional or required -->

<xsl:template name="getUse">
	<xsl:if test="@ektdesignns_use">
		<xsl:attribute name="use">
			<xsl:value-of select="@ektdesignns_use"/>
		</xsl:attribute>
	</xsl:if>
</xsl:template>

<!-- attribute: default or fixed -->

<xsl:template name="getDefault">
	<xsl:param name="value" select="@value"/>
	<xsl:param name="readonly" select="@readonly or @type='hidden'"/>

	<xsl:if test="$value and not(@ektdesignns_calculate)">
		<xsl:choose>
			<xsl:when test="$readonly">
				<xsl:attribute name="fixed">
					<xsl:value-of select="$value"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="default">
					<xsl:value-of select="$value"/>
				</xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:if>
</xsl:template>

<xsl:template name="getDefaultSelectOption">
	<xsl:variable name="selectedOption" select="descendant::option[@selected and (position()!=1 or not(../@ektdesignns_validation='select-req'))][1]"/>
	<xsl:if test="$selectedOption">
		<xsl:call-template name="getDefault">
			<xsl:with-param name="value">
				<xsl:choose>
					<xsl:when test="$selectedOption/@value">
						<xsl:value-of select="$selectedOption/@value"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$selectedOption"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:if>
</xsl:template>


<!-- ignore prototypes -->
<xsl:template match="*[@class='design_prototype']" mode="attributes"/>
<xsl:template match="*[@class='design_prototype']" mode="elements"/>
<xsl:template match="*[@class='design_prototype']"/>


<!-- remove text and attribute values -->
<xsl:template match="text()|@*" mode="attributes"/>
<xsl:template match="text()|@*" mode="elements"/>
<xsl:template match="text()|@*"/>

</xsl:stylesheet>