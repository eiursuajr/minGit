function Move(sDir, objList, objOrder) {
	if (objList.selectedIndex != null && objList.selectedIndex >= 0) {
		nSelIndex = objList.selectedIndex;
		sSelValue = objList[nSelIndex].value;
		sSelText = objList[nSelIndex].text;
		objList[nSelIndex].selected = false;
		if (sDir == "up" && nSelIndex > 0) {
			sSwitchValue = objList[nSelIndex -1].value;
			sSwitchText = objList[nSelIndex - 1].text;
			objList[nSelIndex].value = sSwitchValue;
			objList[nSelIndex].text = sSwitchText;
			objList[nSelIndex - 1].value = sSelValue;
			objList[nSelIndex - 1].text = sSelText;
			objList[nSelIndex - 1].selected = true;
		}
		else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
			sSwitchValue = objList[nSelIndex + 1].value;
			sSwitchText = objList[nSelIndex +  1].text;
			objList[nSelIndex].value = sSwitchValue;
			objList[nSelIndex].text = sSwitchText;
			objList[nSelIndex + 1].value = sSelValue;
			objList[nSelIndex + 1].text = sSelText;
			objList[nSelIndex + 1].selected = true;
		}
	}
	SaveReorder(objList, objOrder);
}
function SaveReorder(objList, objOrder)
{
	objOrder.value = "";
	for (i = 0; i < objList.length; i++) {
		objOrder.value = objOrder.value + objList[i].value;
		if (i < (objList.length - 1)) {
			objOrder.value = objOrder.value + ",";
		}
	}
}