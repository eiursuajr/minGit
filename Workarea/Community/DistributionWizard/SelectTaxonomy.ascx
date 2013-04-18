<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectTaxonomy.ascx.cs" Inherits="Community_DistributionWizard_SelectTaxonomy" %>
<div id="taxonomyTree">
    <script type="text/javascript" language="javascript">
        
        // Toggles the category expansion icon.
        function toggleExpansionIcon(divListContainer, controlId, taxonomyId)
        {
            var img = document.getElementById("ekIMG" + controlId + "_" + taxonomyId);
            if(divListContainer.style.display == "none")
            {
                img.src = img.src.replace(/plusclosetaxonomy\.gif/, "minusopentaxonomy.gif");
                divListContainer.style.display = "";
            }
            else
            {
                img.src = img.src.replace(/minusopentaxonomy\.gif/, "plusclosetaxonomy.gif");
                divListContainer.style.display = "none";
            }
        }
        
        // Toggles expansion of taxonomy tree nodes.
        function toggleTree(controlId, taxonomyId)
        {
            var div = document.getElementById("ekDiv" + controlId + "_" + taxonomyId);
            var control = document.getElementById("ekTaxonomy" + controlId + "_" + taxonomyId);
            if(div != null && control != null)
            {
                // If the div already exists, the data already exists in the page.
                // Just hide or show it.
                toggleExpansionIcon(div, controlId, taxonomyId);
            }
        }
        
        // Return the checkbox associated with the parent of specified tree node.
        // Tree structure:
        // ...
        //  |- INPUT (checkbox)
        //  |- TEXT (checkbox label)
        //  '- DIV
        //      `- UL
        //          `- LI
        //              |- A (or IMG)
        //              |- INPUT
        //              |- TEXT
        //              |- DIV
        //                  |- ...
        // Note: Any change to the tree markup may require and update of this
        //       function.
        function getParentNodeCheckBox(checkbox)
        {
            var parentNodeCheckBox = null;
            if( checkbox != null )
            {
                var containingLI = checkbox.parentNode;
                if( containingLI != null )
                {
                    var containingUL = containingLI.parentNode;
                    if( containingUL != null )
                    {
                        var containingDIV = containingUL.parentNode;
                        if( containingDIV != null )
                        {
                            if( containingDIV.previousSibling != null )
                            {
                                parentNodeCheckBox = containingDIV.previousSibling.previousSibling;
                            }
                        }
                    }
                }
            }
            
            return parentNodeCheckBox;
        }
        
        // Return the checkboxes associated with the children of specified tree node.
        // Tree structure:
        // ...
        //  |- INPUT (checkbox)
        //  |- TEXT (checkbox label)
        //  '- DIV
        //      `- UL
        //          `- LI
        //              |- A (or IMG)
        //              |- INPUT
        //              |- TEXT
        //              |- DIV
        //                  |- ...
        // Note: Any change to the tree markup may require and update of this
        //       function.
        function getChildNodeCheckBoxes(checkbox)
        {
            var childNodeCheckBoxes = new Array();
            
            var containingLI = checkbox.parentNode;
            if( containingLI != null )
            {
                childDIVs = containingLI.getElementsByTagName("div");
                if( childDIVs.length > 0 )
                {
                    var childUL = childDIVs[0].firstChild;
                    
                    if( childUL != null )
                    {
                        for(i=0;i<childUL.childNodes.length;i++)
                        {
                            var checkboxes = childUL.childNodes[i].getElementsByTagName("INPUT");
                            if( checkboxes.length > 0 )
                            {
                                childNodeCheckBoxes[i] = checkboxes[0];
                            }
                        }
                    }
                }
            }
            
            return childNodeCheckBoxes;
        }
        
        // Checks the node and recursively checks all of the node's
        // parents.
        function checkCategory(checkbox)
        {
            if( checkbox != null )
            {
                checkbox.disabled = true;
                if( !checkbox.checked )
                {
                    checkbox.checked = true;
                    checkCategory( getParentNodeCheckBox(checkbox) );
                }
            }
        }
        
        // Unchecks the node and recursively unchecks all parent node's
        // that weren't explicitly checked by the user.
        function uncheckCategory(checkbox)
        {
            if( checkbox != null )
            {
                var childNodeCheckBoxes = getChildNodeCheckBoxes(checkbox);
                
                var childrenChecked = false;
                for(i=0;i<childNodeCheckBoxes.length && !childrenChecked;i++)
                {
                    childrenChecked = childNodeCheckBoxes[i].checked;
                }
                
                if( !childrenChecked )
                {
                    checkbox.disabled = false;
                    checkbox.checked = false;
                    
                    uncheckCategory(getParentNodeCheckBox(checkbox));
                }
            }
        }
        
        // Clear any parent selections
        function clearParentSelections(checkbox)
        {
            var parentCheckBox = getParentNodeCheckBox(checkbox)
            
            if( parentCheckBox != null )
            {
                parentCheckBox.value = "";
                clearParentSelections(parentCheckBox);
            }
        }
        
        // Returns the IDs of selected category nodes.
        function getSelectedCategoryIDs(controlId, taxonomyId)
        {
            
            var control = document.getElementById("ekTaxonomy" + controlId + "_" + taxonomyId);
            
            if(control != null)
            {
                var checkboxes = control.getElementsByTagName("INPUT");
                
                var selectedIDs = null;
                for(i=0; i<checkboxes.length; i++)
                {
                    if( checkboxes[i].value == "1" ){
                        
                        if( selectedIDs == null )
                        {
                            selectedIDs = checkboxes[i].id.replace("ekCheck" + controlId + "_", "");
                        }
                        else
                        {
                            selectedIDs = selectedIDs + " " + checkboxes[i].id.replace("ekCheck" + controlId + "_", "");
                        }
                    }
                }
                
                return selectedIDs;
            }
            else 
            {
                return "";
            }
        }
        
        // Marks a category as explicitly checked by the user.
        function selectCategory(checkbox)
        {
            if( checkbox.checked )
            {
                // Clear and parent selections (only need to track the
                // deepest selected node in any given branch).
                clearParentSelections(checkbox);
                
                // All user checked nodes are marked with a 1
                checkbox.value = "1";
                
                var parentNodeCheckBox = getParentNodeCheckBox(checkbox);
                if( parentNodeCheckBox != null && !parentNodeCheckBox.disabled )
                {
                    // Recursively check all parents to the root.
                    checkCategory(parentNodeCheckBox);
                }
           }
           else 
           {
                checkbox.value = "";
                uncheckCategory(getParentNodeCheckBox(checkbox));
           }
           
           var selectedCategoriesHiddenField = document.getElementById(selectedCategoriesFieldName);
           if( selectedCategoriesHiddenField != null )
           {
                selectedCategoriesHiddenField.value = getSelectedCategoryIDs(taxonomyTreeWrapperID, 0);
           }
        }
        
        // Returns true if any categories have been selected.
        function categoryIsSelected()
        {
            var inputSelectedCategoryIDs = document.getElementById(selectedCategoriesFieldName);
            if( inputSelectedCategoryIDs != null )
            {
                if( inputSelectedCategoryIDs.value != "" )
                {
                    return true;
                }
            }
            
            return false;
        }
        
        function restoreSelectedCategories(controlId)
        {
            var inputSelectedCategoryIDs = document.getElementById(selectedCategoriesFieldName);
            
            if( inputSelectedCategoryIDs != null )
            {
                var selectedCategoryIDs = inputSelectedCategoryIDs.value.split(" ");
                for(var i=0; i < selectedCategoryIDs.length; i++)
                {
                    var categoryNodeCheckBox = document.getElementById("ekCheck" + controlId + "_" + selectedCategoryIDs[i] );
                    if( categoryNodeCheckBox != null )
                    {
                        categoryNodeCheckBox.checked = true;
                        selectCategory(categoryNodeCheckBox);
                    }
                }
            }
        }
    </script>
    <asp:Literal ID="ltrTreeContainer" runat="server"></asp:Literal>
    <input type="hidden" id="inputSelectedCategoryIDs" runat="server" />
    <script type="text/javascript" language="javascript">
        restoreSelectedCategories(taxonomyTreeWrapperID);
    </script>
</div>