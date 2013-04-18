function EkCellPropertiesControl(Id, CssClassSelector, CellAlignmentSelector, BackgroundColorPicker, BgImageDialogCaller, StyleBuilderCaller)
{
      Ektron.Class.inherits(this, new CellPropertiesControl(Id, CssClassSelector, CellAlignmentSelector, BackgroundColorPicker, BgImageDialogCaller, StyleBuilderCaller), "CellPropertiesControl");

      this.abbreviationHolder = document.getElementById(this.Id + "_abbreviation");
	  this.categoriesHolder = document.getElementById(this.Id + "_categories");
	  this.abbreviationBox = new PropertyTextBox(this.abbreviationHolder.id, "ACCESSIBILITY", localization["AlertInvalidProperties"]);
	  this.categoriesBox = new PropertyTextBox(this.categoriesHolder.id, "ACCESSIBILITY", localization["AlertInvalidProperties"]);

      this.LoadPropertyValues = function(cellToModify)
      {
            this.CellPropertiesControl_LoadPropertyValues(cellToModify); // call base class method

            this.abbreviationHolder.value = this.CellToModify.getAttribute("abbr");
	        this.categoriesHolder.value = this.CellToModify.getAttribute("axis");
      };

      this.Update = function(Cell, bMultipleSelected)
      {
            this.CellPropertiesControl_Update(Cell, bMultipleSelected); // call base class method

            this.SetAttribValue ("abbr", this.abbreviationBox.GetValue());
	        this.SetAttribValue ("axis", this.categoriesBox.GetValue()); 
	        return true;
      };
}



