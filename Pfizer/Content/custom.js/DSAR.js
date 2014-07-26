/*Commonly Used Functions*/

/*Common DPR based function for building XML string (Can be used in any page where XML string is to be built for SP)*/
/*function expects table ids as parameters(comma seperated, no double qoutes)
  The tables specified above should be given the following attributes:
	1. consider - Whether or not, the table should be considered(NonMandatory, should be specified if isHeader is specified).
	2. parameter - Value is the Node Name for building xml format (Mandatory if above parameter is specified 'true').
	3. isHeader - Whether the table should be considered as Header Node (NonMandatory).
	4. startRow - Index row for fetching values(Mandatory).
*/
function buildXML() {
	var i;
	var tblObj;
	var str="<root>";
	var length = arguments.length;
	for(i=0;i<length;i++) {
		//tblObj=getId(arguments[i].id);
		tblObj=arguments[i];
		str += conditionalTableBuild(tblObj);
	}
	str += "</root>";
	return str;
}

//function used to addrow in a table(Table id)
function addRowInATable(tableID) {
	var tableObj = getId(tableID);
	var rowObj = tableObj.insertRow(tableObj.rows.length);
	addCellInARow(tableObj, rowObj);
	assignRowClass(tableObj);
}
//function to add specific number of rows
function addSpecificRows(tblID, count) {
	for(var i=0;i<count;i++) {
		addRowInATable(tblID);
		//addRowInATable(tblID);
	}
}
//function adds a cell in a row(Table & row object)
function addCellInARow(tableObj, rowObj) {
	for(var i=0; i<tableObj.rows[1].cells.length; i++) {
		var cellObj = rowObj.insertCell(i);
		cellObj.innerHTML = tableObj.rows[1].cells[i].innerHTML;
		clearValuesInACell(cellObj);
	}
}
//function clears values in a Table, 
function dsarClearValues() {
	var length = arguments.length;
	for(var i=0;i<length;i++) {
		var control=arguments[i];
		correspondingClearCall(control);
	}
}
//function calls corresponding function based on the control type
function correspondingClearCall(control) {
	var type=returnControlType(control);
	//alert(control.childNodes[0].tagName);
	if(type=="DIV"){
	}
	else if(type="TABLE") {
		dsarClearValuesInATable(control);
	}
	else if(type=="TR") {
		dsarClearValuesInARow(control);
	}
	else if(type=="TD") {
		clearValuesInACell(control);
	}
	else if(type.match(/INPUT|SELECT|TEXTAREA/gi)) {
		clearValuesInAChildNode(control);
	}
}
//
function returnControlType(control) {
	return control.tagName;
}
/*//function 
function clearValuesInADiv() {
	var length = arguments.length;
	for(var i=0;i<length;i++) {
		clearValuesInATable();
	}
}*/
//function clears values in tables(tables ids as comma seperated and no "")
function dsarClearValuesInTables() {
	var length = arguments.length;
	for(var i=0;i<arguments.length;i++) {
		var tbl=arguments[i];
		dsarClearValuesInATable(tbl);
	}
}
//function clears values in a table
function dsarClearValuesInATable(tbl) {
	for(var i=0;i<tbl.rows.length;i++) {
		var row=tbl.rows[i];
		dsarClearValuesInARow(row);
	}
}
//function clears values in a row
function dsarClearValuesInARow(row) {
	for(var i=0;i<row.cells.length;i++) {
		var cell=row.cells[i];
		clearValuesInACell(cell);
	}
}
//function clears value of a cell (function will not remove dropdown options)(cell).
function clearValuesInACell(cell) {
	for(var i=0;i<cell.childNodes.length;i++) {
		var childNode = cell.childNodes[i];
		clearValuesOfAControl(childNode);
	}
}
// function clears values of a control and also enables them
function clearValuesOfAControl(control) {
	if(control.type == "select-one") {
		control.selectedIndex=0;
		control.disabled=false;
	}
	else if(control.type == "text") {
		control.value = "";
		control.disabled=false;
		if(control.getAttribute("defaultRead")) {
			if(control.getAttribute("defaultRead") == 1)
				control.readOnly=true;
		}
	}
	else if(control.type == "textarea") {
		control.value = "";
		control.disabled=false;
	}
	else if(control.type == "checkbox") {
		control.checked = false;
		control.disabled=false;
	}
	
	else if(control.type == "hidden") {
		control.value = "";
		control.disabled=false;
	}
	else if(control.type == "select-multiple") {
		for(var j=childNode.options.length-1;j>0;j--) {
			if(control.options[j].selected == true)
				control.options[j].selected == false;
				//childNode.remove(j);
		}
	control.disabled=false;
	}
	else if(control.nodeName =="#text")
	{
		control.data ="";
//		control.nodeValue = "";
	}

}
// function deletes a row in a table. Expects a table id as a parameter
function deleteRowInATable(tableId) {
	var tableObj = getId(tableId)
	for(var i=tableObj.rows.length-1;i>=1;i--) {
		var length = tableObj.rows[i].cells[tableObj.rows[i].cells.length-1].childNodes.length;
		for(var j=0;j<length;j++) {
			if(tableObj.rows[i].cells[tableObj.rows[i].cells.length-1].childNodes[j].type=="checkbox") {
				if(tableObj.rows[i].cells[tableObj.rows[i].cells.length-1].childNodes[j].checked==true) {
					var childNode=tableObj.rows[i].cells[tableObj.rows[i].cells.length-1].childNodes[j];
					j=length;
					deleteRowByControl(childNode);
				}
			}
		}
	}
}
//function deletes the row containing the control.(control object)
function deleteRowByControl(control) {
	var row=control.parentNode.parentNode;
	var tableObj=row.parentNode;
	if(row.rowIndex != 1) {
		row.parentNode.deleteRow(row.rowIndex);
		assignRowClass(tableObj);}
	else {
		alert("* This row cannot be deleted.");
		control.checked=false;
		return false;}
}
//function applies alternating class to the rows in a table.(Table Object)
function assignRowClass(tableObj) {
	for(var i=1;i<tableObj.rows.length;i++) {
		var rowObj = tableObj.rows[i];
		if(rowObj.rowIndex%2==0)
			rowObj.className="tablecontent2";
		else
			rowObj.className="tablecontent1";
	}
}
//Validates controls in table. Expects table id as a parameter
function validateControlsInATable(tableID) {
	var tableObj = getId(tableID);
	for(var i=0; i<tableObj.rows.length;i++) {
		var row=tableObj.rows[i];
		for(var j=0;j<row.cells.length;j++) {
			var cell=row.cells[j];
			for(var k=0;k<cell.childNodes.length;k++) {
				var childNode = cell.childNodes[k];
				if((childNode.type == "text")||(childNode.type == "select-one")||
					(childNode.type == "select-multiple")||(childNode.type == "textarea")) {
					if(childNode.valid) {
						var validationParameters=returnValidationParameters(childNode);
						if(validationParameters[0])
							if(validationParameters[0].toLowerCase().match(/m/i)) {
								if(!validateMandatoryControls(childNode)) {
									return false;}}
						if(validationParameters[1])
							if(trim(validationParameters[1])!="")
								if(trim(childNode.value)!="")
									if(!validateSecondaryParam(childNode, validationParameters[1]))
										return false;
					}
				}
			}
		}
	}
	return true;
}
//function validates value type in the control
function validateSecondaryParam(control, charType) {
	var regExp;
	switch(charType) {
		case "i":
			regExp = /^[1-9]\d*|^0$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only integer numbers");
				control.focus();
				return false;}else return true;
		break;
		case "si":
			regExp = /^[+|\-]?[1-9]\d*|^0$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only signed integer values");
				control.focus();
				return false;}else return true;
		break;
		case "r":
			regExp = /^[1-9]\d*\.?\d*|^0\.\d+|^0$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only real numbers");
				control.focus();
				return false;}else return true;
		break;
		case "sr":
			regExp = /^[+|\-]?[1-9]\d*\.?\d*|^0\.\d+|^0$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only signed real numbers");
				control.focus();
				return false;}else return true;
		break;
		case "a":
			regExp = /^[a-zA-Z0-9_\s]*/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only alphanumeric values");
				control.focus();
				return false;}else return true;
		break;
		case "s":
			regExp = /^[\w\W]*/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only some special characters");
				control.focus();
				return false;}else return true;
		break;
		case "n":
			regExp = /^[a-z][a-z0-9\(\)\-\'\.\,\/\\\s]*$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only alphanumeric and special characters \ - ( , ) . / '");
				control.focus();
				return false;}else return true;
		break;
		case "e":
			regExp = /^[a-zA-Z][a-zA-Z0-9\-_.]+[@][a-zA-Z0-9]+[.][a-zA-Z.]+$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only valid e-mail ids");
				control.focus();
				return false;}else return true;
		break;
		case "t":
			regExp = /^[a-zA-Z0-9][a-zA-Z0-9\:\-\s]*/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only alphanumeric, colon and hyphen");
				control.focus();
				return false;}else return true;
		break;
		case "mo":
			regExp = /^[1-9][0-9]{0,5}(\.[0-9]{2})?$/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only 5 digits and two decimal points");
				//alert(control.errname+" accepts only value of the specified format[XXXXX.XX]");
				control.focus();
				return false;}else return true;
		break;
		case "na":
			regExp = /^[a-zA-Z][a-zA-Z\s]*/gi;
			if(control.value.match(regExp)!=control.value) {
				alert(control.errname+" accepts only alphabets & space");
				control.focus();
				return false;}else return true;
		break;
	}
}
//function returns necessary validation parameters
function returnValidationParameters(controlID) {
	return controlID.valid.split("_");
}
//function validates only mandatory controls
function validateMandatoryControls(childNode) {
	if((childNode.type == "text")||(childNode.type == "textarea")) {
		if(trim(childNode.value)=="") {
			alert(returnAlertMessage(childNode)+childNode.errname);
			childNode.focus();
			return false;}}
	if(childNode.type == "select-one") {
		if(trim(childNode.value)=="") {
			alert(returnAlertMessage(childNode)+childNode.errname);
			childNode.focus();
			return false;}}
	if(childNode.type == "select-multiple") {
		if(!validateMutipleDropdown(childNode)) {
			alert(returnAlertMessage(childNode)+childNode.errname);
			childNode.focus();
			return false;}}
	return true;
}
//function returns true if any multiple dropdown option is selected (except Select)
function validateMutipleDropdown(control) {
	for(var i=0;i<control.options.length;i++) {
		if(control.options[i].selected == true) {
			if((control.options[i].value != "")||(control.options[i].value != "0"))
				return true;}
	}
	return false;
}
//function returns alert message according to the control type
function returnAlertMessage(control) {
	var alertMsg="Please ";
	if((control.type == "text")||(control.type == "textarea"))
		alertMsg+="enter ";
	if((control.type == "select-one")||(control.type == "select-multiple"))
		alertMsg+="select ";
	return alertMsg;
}
//function populates a dropdown in the same row.
//Expects a controlID, cellIndex, ChildNodeIndex, Dropdown TextField & ValueField and a Procedure Name
function populateAdjacentDropdownInARow(controlID,cell,ChildNode,TextField,ValueField,SPName, ParamName) {
	var rowObj = controlID.parentElement.parentElement;
	if(controlID.value != "") {
		var spm = new SPMaster ("http://tempuri.org/","../../webservice/Services.asmx", SPName);
		spm.addParam(ParamName, controlID.value);
		var xmlDom = new JXmlDom(spm.execute(),false);
		populateDropDownByXML(rowObj.cells[cell].childNodes[ChildNode],xmlDom,"//row",TextField,ValueField);
	}
	else {
		rowObj.cells[cell].childNodes[ChildNode].selectedIndex=0;
	}
}
//function enables divs(multiple, give as comma seperated(no ' or ""))//
function DSAREnableDivs() {
	var length = arguments.length;
	for(var i=0;i<length;i++) {
		arguments[i].style.display="block";}
}

//function disables divs(multiple, give as comma seperated(no ' or ""))//
function DSARDisableDivs() {
	var length = arguments.length;
	for(var i=0;i<length;i++) {
		arguments[i].style.display="none";}
}

//function removes spaces and returns the string
function trim(str) {
	return str.toString().replace(/(^\s*|\s*$)/gi,"");
}
//
function showPopupPage(url) {
	window.open(url,"","height=400,width=600,toolbar=no,minimize=no,status=no,memubar=no,location=no, scrollbars=yes");
}
//function returns object of any element / control, given the id.
function getId(controlID) {
	return document.getElementById(controlID);
}
/*Commonly Used Functions*/

/****************************************************************************************************************************/
												/*DSAR specific Functions*/

//function disables the following divs when Cancel is clicked.
function cancelForm() {
	jsfClearValues(tblHeader);
	jsfClearValues(tblInstutionDtls);
	jsfClearValues(tblFamilyDtls);
	DSARDisableDivs(AddGeneralDetails, CompanyHeading, AddOtherDetails, divClinicAddress, divbutton);
}
function submitForm() {
	var str;
	if(!validatePage(tblHeader, tblStockDtls, tblCompDetails)) {
		return false;
	}
	else {
		str=buildXML(tblHeader, tblStockDtls, tblCompDetails);
		getId("hdnXML").value=str;
	}
	//alert(str);
	return true;
	//return false;
}
/******************************************* DSAR Validation begins here ***************************************************/
//validate the header in DSAR
function validateDSARHeader(tableID) {
	if(validateControlsInATable(tableID)) {
		if(getId("drpWorkedAlone").value==1) {
			if(getId("drpWorkedWithPerson").value=="") {
				getId("drpWorkedWithPerson").focus();
				alert("Please select "+getId("drpWorkedWithPerson").errname); }
			else {
				//if(validateControlsInATable(tableID))
				displayDetailsGrid();}
		}		
		else{displayDetailsGrid();}
	}
}
//validate the page
function validatePage() {
	var rexp = /^[a-zA-Z0-9_\s]*/gi;
	var argsLength = arguments.length;
	for(var i=0;i<argsLength;i++) {
		var tableObj=arguments[i];
		if(!validateAtleastOneRowInATable(tableObj)) {
			return false;
		}
	}
	if(trim(getId("ctrlRemarks_txtRemarks").value) == "") {
	alert("Please enter " +getId("ctrlRemarks_txtRemarks").errname);
	getId("ctrlRemarks_txtRemarks").focus();
	return false;
	}
	else {
		if(getId("ctrlRemarks_txtRemarks").value.match(rexp)!=getId("ctrlRemarks_txtRemarks").value) {
			alert(getId("ctrlRemarks_txtRemarks").errname +"accepts only alpha-numeric values");
			getId("ctrlRemarks_txtRemarks").focus();
			return false;
		}
	}
	if(validateControlsInATable("tblHeader")) {
		if(getId("drpWorkedAlone").value==1) {
			if(getId("drpWorkedWithPerson").value=="") {
				getId("drpWorkedWithPerson").focus();
				alert("Please select "+getId("drpWorkedWithPerson").errname);
				return false;
			}
		}
	}
	else {
		return false;
	}
	return true
}
//Validates for atleast one row to be selected//
function validateAtleastOneRowInATable(tbl) {
	var flag=0;
	for(var i=1;i<tbl.rows.length;i++) {
		var row=tbl.rows[i];
		if(row.cells[0].childNodes[0].value!="")
			flag=1;
	}
	if(flag==0) {
		alert("Please enter data for atleast one row !");
		row.cells[0].childNodes[0].focus();
		return false;
	}
	else {
		if(!validateCompleteRowSelected(row)) {
			return false;
		}
	}
	if(!validateDependency(tbl)) {
		return false;
	}
	return true;
}
//
function validateDependency(tbl) {
	var irow, jrow, child;
	if((tbl.rows.length>2) && (tbl.id!="tblHeader")){
		for(var i=1;i<tbl.rows.length-1;i++) {
			irow=tbl.rows[i];
			for(var j=2;j<tbl.rows.length;j++) {
				jrow=tbl.rows[j];
				if(i!=j) {
					if(tbl.id == "tblStockDtls") {
						if( (irow.cells[1].childNodes[0].value!="") && (jrow.cells[1].childNodes[0].value!="") ) {
							if(irow.cells[1].childNodes[0].value == jrow.cells[1].childNodes[0].value) {
								alert(tbl.errname+" has more than one row with the same values !");
								tbl.rows[tbl.rows.length-1].cells[1].childNodes[0].focus();
								return false;
							}
						}
					}
					else {
						if( (irow.cells[2].childNodes[0].value!="") && (jrow.cells[2].childNodes[0].value!="") ) {
							if(irow.cells[2].childNodes[0].value == jrow.cells[2].childNodes[0].value) {
								alert(tbl.errname+" has more than one row with the same values !");
								tbl.rows[tbl.rows.length-1].cells[2].childNodes[0].focus();
								return false;
							}
						}
					}
				}
			}
		}
	}
	return true;
}
//
function validateCompleteRowSelected(row) {
	if(row.cells[0].childNodes[0].value !="") {
		for(i=0;i<row.cells.length;i++) {
			var cell=row.cells[i];
			for(var j=0;j<cell.childNodes.length;j++) {
				var childNode=cell.childNodes[j];
				if(childNode.value=="") {
					alert(returnAlertMessage(childNode)+childNode.errname);
					childNode.focus();
					return false;
				}
				else {
					if(!validateControlValue(childNode))
						return false;
				}
			}
		}
	}
return true;
}
//
function validateControlValue(control) {
	if(control.valid) {
		var charType=returnValidationParameters(control);
		if(trim(charType[1])!="") {
			if(validateSecondaryParam(control, charType[1])) {
				if(!jsfCheckMaxLength(control)) {
					return false; }
			}else { return false; }
		}
	}
	return true;
}

/******************************************* DSAR Validation ends here ***************************************************/
//function displays the Stock and Competitor grid and populates the Brand by Outlet.
function displayDetailsGrid() {
	getId("errCtrl_errLabel").innerText="";
	DSAREnableDivs(divStockDtls, divXSL, divTrgtCompDtls, divRemarks, divBtn);
	var outletFKID = getId("drpOutletName").value;
	var spm = new SPMaster ("http://tempuri.org/","../../webservice/Services.asmx","GetBrandsForOutletByXML");
	spm.addParam("OutletFKID", outletFKID);
	var xmlDom = new JXmlDom(spm.execute(),false);
	populateDropDownByXML(getId("stockDtlsCtrl_drpBrand"),xmlDom,"//row","BrandName","BrandFKID");
	deleteRowsBeforePopulation(tblStockDtls);
	deleteRowsBeforePopulation(tblCompDetails);
	addSpecificRows("tblStockDtls", 2);
	addSpecificRows("tblCompDetails", 2);
}
//
function deleteRowsBeforePopulation(tbl) {
	for(var i=tbl.rows.length-1;i>=2;i--) {
		var row=tbl.rows[i];
		var cell=row.cells[row.cells.length-1];
		cell.childNodes[0].checked=true;
		deleteRowInATable(tbl.id);
	}
}
//
function showXSLDetails(url) {
	//showPopupPage(url);
	showPopupPage("DSARCallDetails.aspx?CallDate='"+getId("hdrCtrl_ctrlDate_txtDateCall").value+"'&OutletFKID="+getId("drpOutletName").value+"'");
}
/***************************************Population of Grids begin here****************************************************/
function commonSPCalling(input) {
	var spm = new SPMaster ("http://tempuri.org/", "../../webservice/Services.asmx", "DSAREditLoad");
	spm.addParam("PKID", getId("hdnPKID").value);
	var xmlDom = new JXmlDom(spm.execute(), false);
	//alert(xmlDom.dom.xml);
	var nodeHeader = xmlDom.selectSingleNode("//dhdr");
	var nodeStock = xmlDom.selectNodes("//dhdr/dsd");
	var nodeComp = xmlDom.selectNodes("//CompHeader/dcd");
	display(nodeHeader, nodeStock, nodeComp);
}
function display(nodeHeader, nodeStock, nodeComp) {
	populateHeader(nodeHeader);
	populateStockDtls(nodeStock);
	populateCompDtls(nodeComp)
}
function populateHeader(node) {
	//alert("HeaderNode:    "+node.xml);
	getId("hdrCtrl_ctrlDate_txtDateCall").value=node.getAttribute("CallDate");
	var tbl=getId("tblHeader");
	for(var i=1;i<tbl.rows.length;i++) {
		var row=tbl.rows[i];
		for(var j=0;j<row.cells.length;j++) {
			var cell=row.cells[j];
			for(var k=0;k<cell.childNodes.length;k++) {
				var childNode=cell.childNodes[k];
				for(var l=0; l<node.attributes.length;l++) {
					if(childNode.parameter) {
						if(childNode.getAttribute("parameter").toLowerCase()==node.attributes[l].name.toLowerCase()) {
							//alert(node.attributes[i].name);
							childNode.value = node.attributes[l].nodeValue;
							if(childNode.onchange)
								childNode.onchange();
						}
					}
				}
			}
		}
	}
	
	//alert(getId("btnGo").onclick);
	validateDSARHeader(tbl.id);
	getId("ctrlRemarks_txtRemarks").value=node.getAttribute("Remarks");
}
function populateStockDtls(node) {
	var tbl=getId("tblStockDtls");
	//alert(tbl.rows.length);
	if(tbl.rows.length>2)
		deleteRowsBeforePopulation(tbl);
	//alert(node.length);
	//alert("StockNode:    "+node[0].xml);
	if(node.length>1) {
		for(var i=0;i<node.length-1;i++) {
			addRowInATable(getId("tblStockDtls").id);}
	}
	for(var i=0;i<node.length;i++) {
		var row=tbl.rows[i+1];
		populateStockRows(node[i],row);
	}
}
function populateStockRows(node, row) {
	//alert(node.xml);
	row.cells[0].childNodes[0].value=node.firstChild.getAttribute("BrandSKUHdrFKID");
	row.cells[0].childNodes[0].onchange();
	for(var i=1;i<row.cells.length;i++) {
		var cell=row.cells[i];
		for(var j=0;j<cell.childNodes.length;j++) {
			var childNode=cell.childNodes[0];
			for(var l=0; l<node.attributes.length;l++) {
				if(childNode.parameter) {
					if(childNode.getAttribute("parameter").toLowerCase()==node.attributes[l].name.toLowerCase()) {
						//alert(node.attributes[i].name);
						childNode.value = node.attributes[l].nodeValue;
						if(childNode.onchange)
							childNode.onchange();
					}
				}
			}
		}
	}
}
function populateCompDtls(node) {
	var tbl=getId("tblCompDetails");
	if(tbl.rows.length>2)
		deleteRowsBeforePopulation(tbl);
	//alert(node.length);
	//alert("StockNode:    "+node[0].xml);
	if(node.length>1) {
		for(var i=0;i<node.length-1;i++) {
			addRowInATable(getId("tblCompDetails").id);}
	}
	for(var i=0;i<node.length;i++) {
		var row=tbl.rows[i+1];
		populateCompRows(node[i],row);
	}
}
function populateCompRows(node, row) {
	//alert(node.xml);
	row.cells[0].childNodes[0].value=node.firstChild.getAttribute("CompanyFKID");
	row.cells[1].childNodes[0].value=node.firstChild.getAttribute("BrandSegmentFKID");
	row.cells[1].childNodes[0].onchange();
	for(var i=2;i<row.cells.length;i++) {
		var cell=row.cells[i];
		for(var j=0;j<cell.childNodes.length;j++) {
			var childNode=cell.childNodes[j];
			if(childNode.parameter) {
				for(var l=0; l<node.attributes.length;l++) {
					if(childNode.getAttribute("parameter").toLowerCase()==node.attributes[l].name.toLowerCase()) {
						childNode.value = node.attributes[l].nodeValue;
						if(childNode.onchange)
							childNode.onchange();
					}
				}
			}
		}
	}
}

/********************************************Population of Grids end here****************************************************/
function makeReadOnly() {
	jsfReadOnly(frmCommonPanel);
	getId("stockDtlsCtrl:btnAddMoreCtrl:btnAdd").disabled = true;
	getId("stockDtlsCtrl:btnAddMoreCtrl:btnAdd").disabled = true;
	getId("stockDtlsCtrl:btnAddMoreCtrl:btnDelete").disabled = true;
	getId("compDtlsCtrl:btnAddoreCtrl:btnAdd").disabled = true;
	getId("compDtlsCtrl:btnAddoreCtrl:btnDelete").disabled = true;
}

function releaseReadOnly() {
	jsfReleaseReadOnly(frmCommonPanel);
	getId("stockDtlsCtrl:btnAddMoreCtrl:btnAdd").disabled = false;
	getId("stockDtlsCtrl:btnAddMoreCtrl:btnDelete").disabled = false;
	getId("compDtlsCtrl:btnAddoreCtrl:btnAdd").disabled = false;
	getId("compDtlsCtrl:btnAddoreCtrl:btnDelete").disabled = false;
}
													/*DSAR specific Functions*/
/****************************************************************************************************************************/

function conditionalTableBuild(tbl) {
	var str="";
	if(considerTable(tbl)==1) {
		if(checkIsHeader(tbl)==0)
			str += buildTableElements(tbl);
		else
			str += buildHeaderTableElements(tbl);
	}
	return str;
}
function buildHeaderTableElements(tbl) {
	var i=assignStartRowValue(tbl);
	var row;
	var str="";
	str += "<"+tbl.parameter+" ";
	for(i;i<tbl.rows.length;i++) {
		row=tbl.rows[i];
		str += buildRowElements(tbl,row);
	}
	str += "/>";
	return str;
}
function buildTableElements(tbl) {
	var i=assignStartRowValue(tbl);
	var row;
	var str="";
	str += "<"+tbl.parameter+">";
	for(i;i<tbl.rows.length;i++) {
		row=tbl.rows[i];
		str += buildRowElements(tbl,row);
	}
	str += "</"+tbl.parameter+">";
	return str;
}
function buildRowElements(tbl,row) {
	var cell;
	var str="";
	str += assignRowStartTag(tbl, row);
	for(var i=0;i<row.cells.length;i++) {
		cell=row.cells[i];
		str += buildCellElements(cell);
	}
	str += assignRowEndTag(tbl, row);
	return str;
}
function assignRowStartTag(tbl,row) {
	if(tbl.isHeader) {
		if(tbl.isHeader==0)
			return "<"+tbl.parameter+" ";
		else
			return "";
	}
	return "<"+tbl.parameter+" ";
}
function assignRowEndTag(tbl,row) {
	if(tbl.isHeader) {
		if(tbl.isHeader==0)
			return "/>";
		else
			return "";
	}
	return "/>";
}
function buildCellElements(cell) {
	var childNode;
	var str="";
	for(var i=0;i<cell.childNodes.length;i++) {
		childNode=cell.childNodes[i];
		str += buildChildNodeElements(childNode);
	}
	return str;
}
function buildChildNodeElements(child) {
	var str="";
	if((child.type!="button") && child.parameter) {
		str += child.parameter;
		str += "='";
		str += child.value;
		str += "' ";
	}
	return str;
}
function assignStartRowValue(tbl) {
	if(tbl.startRow)
		return tbl.startRow-1;
}
function considerTable(tbl) {
	if(tbl.consider) {
		if(tbl.consider==0)
			return 0;
		else
			return 1;
	}
	return 0;
}
function checkIsHeader(tbl) {
	if(tbl.isHeader) {
		if(tbl.isHeader==1)
			return 1;
		else
			return 0;
	}
	return 0;
}
/*Common DPR based functions for building xml string*/
/*
function buildXSL() {
	var spm = new SPMaster ("http://tempuri.org/","../../WebService/Services.asmx","DSARDtlsGrid");
	getId("divXSL").innerHTML = spm.executeTransform("~/xsl/DSARDtls.xsl")
}*/
function returnChildElement() {
}