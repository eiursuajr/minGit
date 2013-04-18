<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectFolder.ascx.cs" Inherits="Community_DistributionWizard_SelectFolder" %>

<div id="ekFolder<%= ID %>_-1"></div>

<script language="javascript" type="text/javascript">
    <!--
        var url = "<%= SitePath %>Workarea/Community/DistributionWizard/FolderTreeData.aspx";

        function expandCallback(controlId, folderId, data)
        {
            var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
            if(!control) return "";
            control.innerHTML = control.innerHTML.substring(0, control.innerHTML.length - 43);
            control.innerHTML = control.innerHTML.replace(/plusopenfolder\.gif/, "minusopenfolder.gif");
            control.innerHTML += data;
        }

        function ExecScript(url, scriptId)
        {
            if(!document.createElement) return null;
            
            element = document.createElement("script");
            
            if(!element) return null;
            
            element.src = url + "&cleanupid=" + scriptId;
            element.id = scriptId;
            element.type = "text/javascript";
                
            var head = document.getElementsByTagName("head")[0];
            
            if(!head) return null;
            
            head.appendChild(element);
            
            return element;
        }

        function cleanUp(id)
        {
            var item = document.getElementById(id);
            
            if(!item) return null;
            
            var head = document.getElementsByTagName("head")[0];
            
            if(!head) return null;
            
            head.removeChild(item);
        }

        function toggleTree(controlId, folderId)
        {
            var div = document.getElementById("ekDiv" + controlId + "_" + folderId);
            var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
            if(div != null && control != null)
            {
                if(div.style.display == "none")
                {
                    div.style.display = "";
                    control.innerHTML = control.innerHTML.replace(/plusopenfolder\.gif/, 
                                                              "minusopenfolder.gif");
                }
                else
                {
                    div.style.display = "none";
                    control.innerHTML = control.innerHTML.replace(/minusopenfolder\.gif/, 
                                                              "plusopenfolder.gif");
                }
                
                return;
            }
            
            
            getSubTree(controlId, folderId);
        }

        function getSubTree(controlId, folderId)
        {
            var control = document.getElementById("ekFolder" + controlId + "_" + folderId);
            if(!control) return;
            
            control.innerHTML += "<div class=\"ekTreeLoading\">Loading...</div>";

            ExecScript(url + 
                "?controlid=" + controlId + 
                "&folderid="  + folderId + 
                "&CheckAddPermissions=<%= CheckAddPermissions %>" +
                "&ShowSpecialFolders=<%= ShowSpecialFolders %>", 
                "ekScript" + controlId + "_" + folderId);
        }

        function ekSelectFolder<%= ID %>(folderId)
        {
            <%= CallbackFunc %>(folderId);
        }

        getSubTree("<%= ID %>", "-1");
    -->
</script>