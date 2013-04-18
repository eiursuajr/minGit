<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html"></xsl:output>
<xsl:template match="/">
<table width="100%">
<xsl:apply-templates/>
</table>
</xsl:template>

<xsl:template match="navigation">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="menu">

<span class="handcur">
<span>
<xsl:attribute name="onClick">showBranch('<xsl:value-of select="@id"/>');</xsl:attribute>
<img src="images/application/ico_menu-closed.gif"><xsl:attribute name="id">I<xsl:value-of select="@id"/></xsl:attribute></img>
<xsl:value-of select="@title"/>
</span>&#160;
<img>
<xsl:attribute name="src" >images/application/icon_add_item.gif</xsl:attribute>
<xsl:attribute name="width">10px</xsl:attribute>
<xsl:attribute name="height">12px</xsl:attribute>
<xsl:attribute name="align">absbottom</xsl:attribute>
<xsl:attribute name="alt">Click here to add item to this menu</xsl:attribute>
<xsl:attribute name="title">Click here to add item to this menu</xsl:attribute>
<xsl:attribute name="onClick" >onSubMenuAddItem("<xsl:value-of select="@id"/>", "<xsl:value-of select="@folder"/>")</xsl:attribute>
</img>&#160;
<xsl:if test="count(item)+count(menu) &gt; 1">
<img>
<xsl:attribute name="src" >images/application/icon_reorder_items.gif</xsl:attribute>
<xsl:attribute name="width">10px</xsl:attribute>
<xsl:attribute name="height">12px</xsl:attribute>
<xsl:attribute name="align">absbottom</xsl:attribute>
<xsl:attribute name="alt">Click here to reorder items in this menu</xsl:attribute>
<xsl:attribute name="title">Click here to reorder items in this menu</xsl:attribute>
<xsl:attribute name="onClick" >onSubMenuOrderItem("<xsl:value-of select="@id"/>", "<xsl:value-of select="@folder"/>")</xsl:attribute>
</img>&#160;
</xsl:if>
<img>
<xsl:attribute name="src" >images/application/icon_edit.gif</xsl:attribute>
<xsl:attribute name="width">10px</xsl:attribute>
<xsl:attribute name="height">12px</xsl:attribute>
<xsl:attribute name="align">absbottom</xsl:attribute>
<xsl:attribute name="alt">Click here to edit this menu</xsl:attribute>
<xsl:attribute name="title">Click here to edit this menu</xsl:attribute>
<xsl:attribute name="onClick" >onSubMenuEdit("<xsl:value-of select="@id"/>")</xsl:attribute>
</img>&#160;
<img>
<xsl:attribute name="src" >images/application/icon_delete.gif</xsl:attribute>
<xsl:attribute name="width">10px</xsl:attribute>
<xsl:attribute name="height">12px</xsl:attribute>
<xsl:attribute name="align">absbottom</xsl:attribute>
<xsl:attribute name="alt">Click here to delete this menu</xsl:attribute>
<xsl:attribute name="title">Click here to delete this menu</xsl:attribute>
<xsl:attribute name="onClick" >onSubMenuDelete("<xsl:value-of select="@id"/>")</xsl:attribute>
</img>&#160;
<br/>
</span>
<span class="branch"><xsl:attribute name="id"><xsl:value-of select="@id"/></xsl:attribute>
<xsl:apply-templates/>
</span>
</xsl:template>

<xsl:template match="item">
<xsl:choose>
	<xsl:when test="@type='library'">
		<img width="16px" height="16px" align="absbottom" src="images/ui/icons/image.png"/>
	</xsl:when>
	<xsl:when test="@type='form'">
		<img width="16px" height="16px" align="absbottom" src="images/ui/icons/contentForm.png"/>
	</xsl:when>
	<xsl:when test="@type='link'">
		<img width="16px" height="16px" align="absbottom" src="images/ui/icons/link.png"/>
	</xsl:when>
	<xsl:otherwise>
		<img width="16px" height="16px" align="absbottom" src="images/ui/icons/file.png"/>
	</xsl:otherwise>
</xsl:choose>
<xsl:value-of select="@title"/>&#160;
<span class="handcur">
<img>
<xsl:attribute name="src" >images/application/icon_edit.gif</xsl:attribute>
<xsl:attribute name="width">10px</xsl:attribute>
<xsl:attribute name="height">12px</xsl:attribute>
<xsl:attribute name="align">absbottom</xsl:attribute>
<xsl:attribute name="alt">Click here to edit this item</xsl:attribute>
<xsl:attribute name="title">Click here to edit this item</xsl:attribute>
<xsl:attribute name="onClick" >onMenuItemEdit("<xsl:value-of select="@itemid"/>", "<xsl:value-of select="(ancestor::menu)[last()]/@id"/>","<xsl:value-of select="@type"/>")</xsl:attribute>
</img>&#160;
<img>
<xsl:attribute name="src" >images/application/icon_delete.gif</xsl:attribute>
<xsl:attribute name="width">10px</xsl:attribute>
<xsl:attribute name="height">12px</xsl:attribute>
<xsl:attribute name="align">absbottom</xsl:attribute>
<xsl:attribute name="alt">Click here to delete this item</xsl:attribute>
<xsl:attribute name="title">Click here to delete this item</xsl:attribute>
<xsl:attribute name="onClick" >onMenuItemDelete("<xsl:value-of select="@itemid"/>","<xsl:value-of select="(ancestor::menu)[last()]/@id"/>","<xsl:value-of select="@type"/>")</xsl:attribute>
</img>&#160;
</span>
<br/>
</xsl:template>

<!-- avoid output of text node with default template -->
<xsl:template match="@Title"/>

</xsl:stylesheet>
