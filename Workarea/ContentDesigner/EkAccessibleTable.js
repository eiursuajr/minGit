function EkAccessibleTable(id, alignmentSelector)
{
      Ektron.Class.inherits(this, new AccessibleTable(id, alignmentSelector), "AccessibleTable");

      this.UpdateCellIDs = EkAccessibleTable_UpdateCellIDs;
}

function EkAccessibleTable_UpdateCellIDs()
{
    //this.AccessibleTable_UpdateCellIDs(); // call base class method
    var haveIds = this.CheckHeadersHasIds();
    var haveHeaders = this.CheckCellsHasHeaders();

    if (this.Table && this.Table.tHead && (this.SetCellID || haveIds || haveHeaders))
    {
        var tableID = this.Table.id ? this.Table.id : this.Table.uniqueID;

        if (!tableID)
        {
	        tableID = "table";
        }

        var arrHeaders = null;
        var headingId = 0;

        //insert or remove SCOPE, ID, HEADERS attributes of all cells
        var oTable = this.Table;
        var nRandTbl = EkAccessibleTable.getRandom();
        var oRow;
        var curr_row;
        var curr_cell;
        var nCellId;
        var strTempID;
        var aAction;
        var nTHRows = this.HeadingRows;
        var nTHCols = this.HeadingColumns;
		
        // initialize a 2-D array to store all the corresponding IDs or HEADERS
        var nMatrixX = this.Table.rows.length;
        var nMatrixY = this.GetTableColumns();
        var matrixmap = new Array();
        for ( var x = 0; x < nMatrixX; x++ )
        {
	        for ( var y = 0; y < nMatrixY; y++ )
	        {
		        matrixmap["'" + x + "," + y + "'"] = "";
	        }
        }

        for ( curr_row = 0; curr_row < oTable.rows.length; curr_row++ )	
        {
	        oRow = oTable.rows[curr_row];
	        for ( curr_cell = 0; curr_cell < oRow.cells.length; curr_cell++ )	
	        {
		        // reset all the HEADERS 
		        oRow.cells[curr_cell].removeAttribute("headers");
		        if ( curr_row < nTHRows )	
		        {// add 508 attributes and change tagname to TH for Row Headings
			        oRow.cells[curr_cell].setAttribute("scope", "col");
			        strTempID = EkAccessibleTable.createCellID(curr_row, curr_cell);
			        oRow.cells[curr_cell].setAttribute("id", "tbl" + nRandTbl + "id" + strTempID);
			        aAction = EkAccessibleTable.writeOutMatrix(curr_row, curr_cell, oRow.cells[curr_cell].colSpan, oRow.cells[curr_cell].rowSpan, matrixmap, oRow.cells[curr_cell].id);
		        }
		        else	
		        {// if (curr_row > nTHrows)
			        var matrix_cell = curr_cell;
			        while ( matrixmap["'" + curr_row + "," + matrix_cell + "'"] != "" )
			        {
				        matrix_cell = matrix_cell + 1;
			        }
			        if ( matrix_cell < nTHCols && 0 == matrixmap["'" + curr_row + "," + matrix_cell + "'"] )	
			        {// add 508 attributes and change tagname to TH for Column Headings
				        oRow.cells[curr_cell].setAttribute("scope", "row");
				        strTempID = EkAccessibleTable.createCellID(curr_row, curr_cell);
				        oRow.cells[curr_cell].setAttribute("id", "tbl" + nRandTbl + "id" + strTempID);
				        aAction = EkAccessibleTable.writeOutMatrix(curr_row, matrix_cell, oRow.cells[curr_cell].colSpan, oRow.cells[curr_cell].rowSpan, matrixmap, oRow.cells[curr_cell].id);
				        nCellId = oRow.cells[curr_cell].id;
			        }
			        else	
			        {// these are not Headings, HEADERS are required
				        nCellId = oRow.cells[curr_cell].id;						
				        if ( nTHCols > 0 || nTHRows > 0 )
				        {// add 508 headers attribute, loop through the IDs inside out, and Row IDs before Column IDs
					        var iCol = nTHCols;
					        var iRow = nTHRows;
					        var sHeaders = "";
					        for ( var j = iCol-1; j >= 0; j-- )	
					        {
						        sHeaders = sHeaders + " " + matrixmap["'" + curr_row + "," + j + "'"];
					        }
					        for ( var i = iRow-1; i >= 0; i-- )	
					        {
						        sHeaders = sHeaders + " "  + matrixmap["'" + i + "," + matrix_cell + "'"];
					        }
					        if ( 0 == sHeaders.indexOf(" ") )
					        {
						        sHeaders = sHeaders.substring(1);
					        }
					        aAction = EkAccessibleTable.writeOutMatrix(curr_row,matrix_cell,oRow.cells[curr_cell].colSpan,oRow.cells[curr_cell].rowSpan,matrixmap,sHeaders);
					        if ( sHeaders != "" )
					        {// insert HEADERS attribute if it exists
						        oRow.cells[curr_cell].setAttribute("headers", sHeaders);
					        }
					        if ( " " == oRow.cells[curr_cell].headers )
					        {// removeAttribute("headers") if it is an empty string
						        oRow.cells[curr_cell].removeAttribute("headers");
					        }
				        }
				        else	
				        {// The table has no ID, these cells have no Headers
					        oRow.cells[curr_cell].removeAttribute("headers");
					        oRow.cells[curr_cell].removeAttribute("scope");
					        oRow.cells[curr_cell].removeAttribute("id");
				        }
			        }
		        }
  	        }
        }
    }
}
      
EkAccessibleTable.getRandom = function () 
{// generate a random number as the table id
     return Math.round(Math.random()*1000);
}

EkAccessibleTable.createCellID = function (x,y)
{
	var strCellID = x + "_" + y;
	return	(strCellID);
}

EkAccessibleTable.writeOutMatrix = function (nRow, nCell, nColSpan, nRowSpan, aMatrix, sValue)
{
	if ( nColSpan || nRowSpan )
	{
		for ( var x = 0; x < nRowSpan; x++ )
		{
			var next_row = eval(nRow + x);
			for ( var y = 0; y < nColSpan; y++ )
			{
				var next_cell = eval(nCell + y);
				while ( aMatrix["'" + next_row + "," + next_cell + "'"] != "" )
				{
					next_cell = next_cell + 1;
				}
				aMatrix["'" + next_row + "," + next_cell + "'"] = sValue;
			}
		}
	}
	return (aMatrix);
}
