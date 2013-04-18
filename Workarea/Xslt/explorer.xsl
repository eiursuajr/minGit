<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ekt="http://www.ektron.com/InformationArchitecture" exclude-result-prefixes="ekt">
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" omit-xml-declaration="yes" standalone="no"/>

  <xsl:template match="/">
    <script type="text/javascript" src="[[ekapppath]]java/personaldirectory.js"/>
    <input type="hidden" name="[[ekkey]]_inode" id="[[ekkey]]_inode" value="[[eknode]]"/>
    <input type="hidden" name="ekapppath" id="ekapppath" value="[[ekapppath]]"/>
    <link rel="stylesheet" type="text/css" href="/websrc/WorkArea/csslib/personaldirectory.css"></link>
    <xsl:call-template name="Header"/>
    <xsl:call-template name="Body"/>
  </xsl:template>

  <xsl:template name="Header">
    <div id="UserTaxonomyHeader">
      <h3>
        <span class="Label"><xsl:value-of select="result/TaxonomyData/TaxonomyPath"/></span>
        <span class="Search">
          <span class="SearchLabel"><!--Search--></span><!--
          <input type="text" id="UserTaxonomySearch" title="Search"/>
          <input type="button" id="UserTaxonomySearchSubmit" onclick="alert('You clicked search!');return false;" value=" Search "/>-->
        </span>
      </h3>
      <!--xsl:call-template name="Breadcrumbs"/-->
      <xsl:if test="result/editaccess='true'"><xsl:call-template name="Actions"/></xsl:if>
    </div>
  </xsl:template>

  <xsl:template name="Breadcrumbs">
    <ul id="UserTaxonomyBreadcrumbs">
      <xsl:for-each select="//TaxonomyName">
        <li>
          <a href="#" onclick="return false;">
            <xsl:attribute name="title">
              <xsl:value-of select="." disable-output-escaping="yes"/>
            </xsl:attribute>
            <xsl:value-of select="." disable-output-escaping="yes"/>
          </a>
          <xsl:if test="following-sibling::TaxonomyName">Â»</xsl:if>
        </li>
      </xsl:for-each>
    </ul>
  </xsl:template>

  <xsl:template name="Actions">
    <div id="UserTaxonomyActions">
      <p>
        <a id="AddFolder" href="javascript:toggleVisibility('[[ekkey]]_addfolderpanel');" title="Add Folder">Add Folder</a>
        <a id="remove" href="javascript:pdhdlr('remove', '[[ekkey]]');" title="Remove Selected Item(s)">Remove Selected Item(s)</a>
      </p>
      <xsl:call-template name="AddFolderPanel"/>
    </div>
  </xsl:template>

  <xsl:template name="AddFolderPanel">
    <table id="[[ekkey]]_addfolderpanel" class="addfolderpanel" style="display:none;">
      <tbody>
        <tr>
          <th>Folder Name:</th>
          <td>
            <input type="text" id="[[ekkey]]_fName" name="[[ekkey]]_fName"/>
          </td>
        </tr>
        <tr>
          <th>Description:</th>
          <td>
            <textarea name="[[ekkey]]_fDesc" id="[[ekkey]]_fDesc" cols="35" rows="4"/>
          </td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <th></th>
          <th>
            <a title="Add" href="javascript:pdhdlr('add', '[[ekkey]]');">Add</a>&#160;
            <a title="Close" href="javascript:toggleVisibility('[[ekkey]]_addfolderpanel');">Close</a>
          </th>
        </tr>
      </tfoot>
    </table>
  </xsl:template>

  <xsl:template name="Body">
    <div id="UserTaxonomyBody">
      <ul>
        <xsl:for-each select="result/TaxonomyData/Taxonomy/TaxonomyData">
          <xsl:call-template name="Fldr">
            <xsl:with-param name="Class" select="'Folder'"/>
            <xsl:with-param name="Image" select="'[[ekimgpath]]folder.gif'"/>
          </xsl:call-template>
        </xsl:for-each>
        <xsl:for-each select="result/TaxonomyData/TaxonomyItems/TaxonomyItemData[child::TaxonomyItemType='2']">
          <xsl:call-template name="Content">
            <xsl:with-param name="Class" select="'User'"/>
            <xsl:with-param name="Image" select="'[[ekimgpath]]user.gif'"/>
          </xsl:call-template>
        </xsl:for-each>
        <xsl:for-each select="result/TaxonomyData/TaxonomyItems/TaxonomyItemData[child::TaxonomyItemType='7']">
          <xsl:call-template name="Content">
            <xsl:with-param name="Class" select="'Group'"/>
            <xsl:with-param name="Image" select="'[[ekimgpath]]group.gif'"/>
          </xsl:call-template>
        </xsl:for-each>
        <xsl:for-each select="result/TaxonomyData/TaxonomyItems/TaxonomyItemData[child::TaxonomyItemType='1']">
          <xsl:call-template name="Content">
            <xsl:with-param name="Class" select="'Content'"/>
            <xsl:with-param name="Image" select="'[[ekimgpath]]content.gif'"/>
          </xsl:call-template>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
  
    <xsl:template name="Fldr">
    <xsl:param name="Class"/>
    <xsl:param name="Image"/>
    <li class="{$Class}">
      <xsl:if test="/result/editaccess='true'"><input type="checkbox" title="Select To Delete"><xsl:attribute name="id">[[ekkey]]_f<xsl:value-of select="TaxonomyId"/></xsl:attribute></input ></xsl:if>
      <div>
        <img align="left"><xsl:attribute name="src"><xsl:value-of select="$Image"/></xsl:attribute></img>
        <a title="Folder Name">
          <xsl:attribute name="href"><xsl:value-of select="PersistantURL"/></xsl:attribute>
          <xsl:call-template name="GetTitle"/>
        </a>
        <span class="Description">
          <xsl:call-template name="GetDescription"/>
        </span>
      </div>
    </li>
  </xsl:template>

  <xsl:template name="Content">
    <xsl:param name="Class"/>
    <xsl:param name="Image"/>
    <li class="{$Class}">
      <xsl:if test="/result/editaccess='true'"><input type="checkbox" title="Select To Delete"><xsl:attribute name="id">[[ekkey]]_i<xsl:value-of select="TaxonomyItemId"/>_<xsl:value-of select="TaxonomyItemType"/></xsl:attribute></input ></xsl:if>
      <div>
        <img align="left"><xsl:attribute name="src"><xsl:value-of select="$Image"/></xsl:attribute></img>
        <a title="Folder Name">
          <xsl:attribute name="href">#</xsl:attribute>
          <xsl:call-template name="GetTitle"/>
        </a>
        <span class="Description">
          <xsl:call-template name="GetDescription"/>
        </span>
      </div>
    </li>
  </xsl:template>

  <xsl:template name="GetTitle">
    <xsl:choose>
      <xsl:when test="TaxonomyItemTitle">
        <xsl:value-of select="TaxonomyItemTitle" disable-output-escaping="yes"/>
      </xsl:when>
      <xsl:when test="TaxonomyName">
        <xsl:value-of select="TaxonomyName" disable-output-escaping="yes"/>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="GetDescription">
    <xsl:choose>
      <xsl:when test="TaxonomyItemTeaser">
        <xsl:value-of select="TaxonomyItemTeaser" disable-output-escaping="yes"/>
      </xsl:when>
      <xsl:when test="TaxonomyDescription">
        <xsl:value-of select="TaxonomyDescription" disable-output-escaping="yes"/>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
