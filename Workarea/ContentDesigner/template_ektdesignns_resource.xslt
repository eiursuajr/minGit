<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="atom" xmlns:cms="urn:Ektron.Cms.Controls" xmlns:atom="http://www.w3.org/2005/Atom">

	<xsl:param name="workareaPath" select="'/workarea/'"/>
	<xsl:param name="LangType" select="''"/>
	<xsl:param name="userId" select="''"/>
	<xsl:param name="mode" select="''"/>

	<xsl:template name="ektdesignns_resource">
		<xsl:param name="context"/>
		<xsl:param name="appearance"/>
		<xsl:variable name="id" select="$context/text()"/>
		<xsl:variable name="idType" select="concat(':',$context/@datavalue_idtype,':')"/>
		<xsl:choose>
			<xsl:when test="$mode='preview'">
				<xsl:value-of select="$context/@datavalue_displayvalue"/>		
			</xsl:when>
			<xsl:when test="contains($idType,':content:')">
				<xsl:choose>
					<xsl:when test="$appearance='title'">
						<xsl:variable name="url">
							<xsl:call-template name="safeUri">
								<xsl:with-param name="uri" select="concat($workareaPath,'webservices/rest.svc/content/',$id,';',$LangType,'/title.xml')"/>
							</xsl:call-template>
						</xsl:variable>
						<xsl:variable name="title" select="document($url)/*"/>
						<xsl:copy-of select="$title/node()"/>
					</xsl:when>
					<xsl:when test="$appearance='titledcontent'">
						<xsl:variable name="url">
							<xsl:call-template name="safeUri">
								<xsl:with-param name="uri" select="concat($workareaPath,'webservices/rest.svc/content/',$id,';',$LangType,'/title.xml')"/>
							</xsl:call-template>
						</xsl:variable>
						<xsl:variable name="title" select="document($url)/*"/>
						<xsl:if test="$title">
							<h3 class="title"><xsl:copy-of select="$title/node()"/></h3>
							<div class="content">
								<xsl:choose>
									<xsl:when test="contains($idType,':htmlform:')">
										<cms:FormBlock DefaultFormID="{$id}" runat="server" />
									</xsl:when>
									<xsl:otherwise>
										<cms:ContentBlock DefaultContentID="{$id}" runat="server" />
									</xsl:otherwise>
								</xsl:choose>
							</div>
						</xsl:if>
					</xsl:when>
					<xsl:when test="$appearance='quicklink'">
					    <cms:ContentList DisplayXslt="ecmNavigation" ContentIds="{$id}" runat="server" />
					</xsl:when>
					<xsl:when test="$appearance='teaser'">
					    <cms:ContentList DisplayXslt="ecmTeaser" ContentIds="{$id}" runat="server" />
					</xsl:when>
					<xsl:when test="contains($idType,':htmlform:')">
						<cms:FormBlock DefaultFormID="{$id}" runat="server" />
					</xsl:when>
					<xsl:otherwise>
						<cms:ContentBlock DefaultContentID="{$id}" runat="server" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="contains($idType,':folder:')">
				<xsl:choose>
					<xsl:when test="$appearance='breadcrumb'">
						<cms:FolderBreadcrumb DefaultFolderID="{$id}" runat="server" />
					</xsl:when>
					<xsl:when test="$appearance='quicklink'">
						<cms:ListSummary DisplayXslt="ecmNavigation" ContentType="NonLibraryContent" FolderID="{$id}" runat="server" />
					</xsl:when>
					<xsl:otherwise> <!-- $appearance='teaser' -->
						<cms:ListSummary DisplayXslt="ecmTeaser" ContentType="NonLibraryContent" FolderID="{$id}" runat="server" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="contains($idType,':taxonomy:')">
				<!-- NOTE: ajax calls does not work from within the content block -->
				<cms:Directory TaxonomyID="{$id}" runat="server" enablesearch="false" showroot="true" enableajax="false" PreserveUrlParameters="true" />
			</xsl:when>
			<xsl:when test="contains($idType,':collection:')">
				<xsl:choose>
					<xsl:when test="$appearance='quicklink'">
						<cms:Collection DisplayXslt="ecmNavigation" DefaultCollectionID="{$id}" runat="server" WrapTag="div" MemberMenuActive="true" />
					</xsl:when>
					<xsl:otherwise> <!-- $appearance='teaser' -->
						<cms:Collection DisplayXslt="ecmTeaser" DefaultCollectionID="{$id}" runat="server" WrapTag="div" MemberMenuActive="true" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$context/@datavalue_displayvalue"/>			
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>