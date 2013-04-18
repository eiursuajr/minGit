<%@ Page Language="C#" AutoEventWireup="true" Inherits="cms_xmlconfig" CodeFile="cms_xmlconfig.aspx.cs"
    ContentType="text/xml" %>

<customtag>
	<!-- Individual Tag Definitions - Allows control over the look of each tag. -->
	<tagdefinitions name="custtags" hideobject="false">
    
        <tagdefault type="vertical" visible="true"  
					style="font-family:arial; font-weight:bold; background-color:#cccccc; border:solid red 1pt; margin:2px; width:95%;" 
					dstyle="font-family:arial; font-weight:normal; background-color:white; padding:4px">
					<!-- The simtaglist is a quick list of tags that follow the default items above.
					      The only offered deviation is the glyph that can be specified.  If no glyph is given
						 then the default glyph is used. -->
				</tagdefault>
	</tagdefinitions>
	<docxml enabled="true" reqfill="false" showroot="false">
		<!-- We seem to need the full path for the XSLT files, including the server. -->
		
		<transform onload="<%=(Request.QueryString["Edit_xslt"])%>" onsave="<%=(Request.QueryString["Save_xslt"])%>"/>
		<loadsch enabled="true">
			<!-- A ns value of "" means a namespace will be determined for the schema
                 based on the path or an internal namespace.
                 The status values are:
                     active - used for offering valid options to user
                     idle - stored for later selection or use (default)
                     disabled - entry is ignored and not loaded
                     - If more than one entry is selected as active then the first entry is used.
                     - If no entry is active then the first schema is used.
            -->
			<xsd status="active" src="<%=(Request.QueryString["Schema"])%>" ns=""/>
		</loadsch>
	</docxml>
</customtag>
