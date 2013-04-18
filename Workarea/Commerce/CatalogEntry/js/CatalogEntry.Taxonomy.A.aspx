<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CatalogEntry.Taxonomy.A.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.CatalogEntry.CatalogEntry_Taxonomy_A_Js" %>

    var taxonomytreearr="<asp:Literal ID="litTaxonomyTreeIdList" runat="server" />".split(",");
    var taxonomytreedisablearr="<asp:Literal ID="litTaxonomyTreeParentIdList" runat="server" />".split(",");
    var __TaxonomyOverrideId="<asp:Literal ID="litTaxonomyOverrideId" runat="server" />".split(",");
    var m_fullScreenView=false;
    var __EkFolderId = <asp:Literal ID="litFolderId" runat="server" />;


    function fetchtaxonomyid(pid){
        for(var i=0; i < taxonomytreearr.length; i++){
            if(taxonomytreearr[i]==pid){
                return true;
                break;
            }
        }
        return false;
    }
    function fetchdisabletaxonomyid(pid){
        for(var i=0; i < taxonomytreedisablearr.length; i++){
            if(taxonomytreedisablearr[i]==pid){
                return true;
                break;
            }
        }
        return false;
    }
    function updatetreearr(pid,op){
        if(op=="remove"){
            for(var i=0; i < taxonomytreearr.length; i++){
                if(taxonomytreearr[i]==pid){
                    taxonomytreearr.splice(i,1);break;
                }
            }
        }
        else{
            taxonomytreearr.splice(0,0,pid);
        }
        
        document.getElementById("taxonomyselectedtree").value="";
        for(var i=0;i < taxonomytreearr.length;i++){
            if(document.getElementById("taxonomyselectedtree").value==""){
                document.getElementById("taxonomyselectedtree").value=taxonomytreearr[i];
            }else{
                document.getElementById("taxonomyselectedtree").value=document.getElementById("taxonomyselectedtree").value+","+taxonomytreearr[i];
            }
        }
    }
   function selecttaxonomy(control){
        var pid=control.value;
        
        if (window.CheckTaxonomyDelegate)
            CheckTaxonomyDelegate(control);
            
        if(control.checked)
        {
            updatetreearr(pid,"add");
        }
        else
        {
            updatetreearr(pid,"remove");
        }
        var currval=eval(document.getElementById("chkTree_T"+pid).value);
        var node = document.getElementById( "T" + pid );
        var newvalue=!currval;
        document.getElementById("chkTree_T"+pid).value=eval(newvalue);
        if(control.checked)
          {
            Traverse(node,true);
          }
        else
          {
            Traverse(node,false);
            var hasSibling = false;
            if (taxonomytreearr != "")
              { for(var i = 0 ;i < taxonomytreearr.length;i++)
                    {
                      if(taxonomytreearr[i] != "")
                        {
                          var newnode = document.getElementById( "T" + taxonomytreearr[i]);
                            if(newnode != null && newnode.parentNode == node.parentNode)
                               {Traverse(node,true);hasSibling=true;break;}
                        }
                    }
              }
            if(hasSibling == false)
            { 
             checkParent(node);
            }  
          }
    }
   
    function checkParent(node)
    { if(node!= null)
        {
              var subnode = node.parentNode;
              if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
              {
                        for(var j=0;j < subnode.childNodes.length;j++)
                          {var pid=subnode.childNodes[j].id;
                           if(document.getElementById("chkTree_"+pid).value == true || document.getElementById("chkTree_"+pid).value == "true")
                              {Traverse(subnode.childNodes[j],true);return;}
                          }
               checkParent(subnode.parentNode);
              }
        }
    }
    function Traverse(node,newvalue){
        if(node!=null){
            subnode=node.parentElement;
            if(subnode!=null && subnode.id!="T0" &&  subnode.id!=""){
                for(var j=0;j < subnode.childNodes.length;j++){
                    var n=subnode.childNodes[j]
                    if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                        var pid=subnode.id;
                        updatetreearr(pid.replace("T",""),"remove");
                        document.getElementById("chkTree_"+pid).value=eval(newvalue);
                        n.setAttribute("checked",eval(newvalue));
                        n.setAttribute("disabled",eval(newvalue));
                        
                    }
                }
                if(HasChildren(subnode) && subnode.getAttribute("checked")){
                       subnode.setAttribute("checked",true);
                        subnode.setAttribute("disabled",true);  
                }
                Traverse(subnode,newvalue);
            }
        }
    }
    function HasChildren(subnode){
        if(subnode!=null){
            for(var j=0; j < subnode.childNodes.length; j++)
            {
                for(var j=0; j < subnode.childNodes.length; j++){
                    var n=subnode.childNodes[j]
                    if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                        var pid=subnode.id;
                        var v=document.getElementById("chkTree_"+pid).value;
                        if(v==true || v=="true"){
                        return true;break;
                        }
                    }
                }
            }
        }
        return false;
    }
    function SaveCategory()
    {
        var selected_nodes = document.getElementById('taxonomyselectedtree');
        var target = parent.document.getElementById('ekcategoryselection');
        if( target != null ) {
            target.value = selected_nodes.value;
        }
        parent.CloseCategorySelect(false);
    }