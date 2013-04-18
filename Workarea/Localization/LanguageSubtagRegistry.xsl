<?xml version='1.0'?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" version="1.0" encoding="rtf-8" indent="yes"/>

	<xsl:template match="/">
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="registry">
		<xsl:copy>
			<language subtag="" description="(select language)" />
			<xsl:apply-templates select="language[not(contains(description, 'Sign Language')) and
											not(contains(description, 'International Sign'))]">
				<xsl:sort select="description"/>
			</xsl:apply-templates>
			<script subtag="" description="(optional script)" />
			<xsl:apply-templates select="script">
				<xsl:sort select="description"/>
			</xsl:apply-templates>
			<region subtag="" description="(select region)" />
			<xsl:apply-templates select="region">
				<xsl:sort select="description"/>
			</xsl:apply-templates>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="language|script|region">
		<xsl:if test="not(contains(subtag, '.')) and description[1] != 'Private use'">
			<!-- XmlDataSource only binds to attributes -->
			<xsl:copy>
				<xsl:attribute name="subtag">
					<xsl:value-of select="subtag[1]"/>
				</xsl:attribute>
				<xsl:attribute name="description">
					<xsl:value-of select="description[1]"/>
				</xsl:attribute>
			</xsl:copy>
		</xsl:if>
	</xsl:template>

	<xsl:template match="language[subtag='ia']">
		<language subtag="ia" description="Interlingua (IALA)" />
	</xsl:template>

</xsl:stylesheet>