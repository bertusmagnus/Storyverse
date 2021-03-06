/* 
 * Copyright � Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

var saving = false;

function handleFormLoad()
{   
    var f = document.forms[0];
    if (f != null && (f.name == 'edit' || f.name == 'new'))
    {
        setDetailActionButtons();
    }
}

function deleteEntity(itemTypeName)
{
	if (!confirmAction("delete this " + itemTypeName)) return false;

    submitToAction("delete");
    return true;
}

function createEntity()
{
    markClean();
    submitToAction("create");
    return true;
}

function search()
{
    submitToAction("search");
    return true;
}

function submitToAction(actionName)
{
    document.forms[0].action = actionName + '.rails';
	document.forms[0].submit();
	return true;
}

function goToUrl(url)
{ 
    window.location.href = url; 
    return false;
}

function confirmAction(action)
{
	return confirm("Are you sure you want to " + action + "?");
}

function confirmUnloadIfDirty()
{
    var isDirty = document.getElementById('isDirty');
    if (!saving && isDirty!= null && isDirty.value == "true")
    {
        return "YOU WILL LOSE YOUR CHANGES";
    }
}

function enterKeySubmitAction(actionName, e)
{
    var keycode;
    if (window.event) 
    {
        keycode = window.event.keyCode;
    }
    else if (e) 
    {
        keycode = e.which;
    }
    else 
    {
        return true;
    }

    if (keycode == 13)
    {
       submitToAction(actionName);
       return false;
    }
    else
    {
       return true;
    }
}

function markDirty()
{
    document.getElementById('isDirty').value = 'true';
    setDetailActionButtons();
    
    var error = document.getElementById('error');
    if (error != null)
    {
        clearChildElements(error); 
    }
       
    //ToDo: this is a hack.  There are two elements with id of 'actionResult'.  
    //This is done to allow ajaxhelper to populate the result message in two places
    var spanElements = document.getElementsByTagName('span');   
    for (var i = 0; i < spanElements.length; i++)
    {
        if (spanElements[i].id == 'actionResult')
        {
            clearChildElements(spanElements[i]);
        }
    }
}

function clearChildElements(element)
{
    if  (element != null && element.hasChildNodes())
    {
        while (element.childNodes.length > 0)
        {
            element.removeChild(element.firstChild);       
        }
    } 
}

function markClean()
{
    document.getElementById('isDirty').value = 'false';
    setDetailActionButtons();
}

function updateEstimate()
{
    if (!confirmAction('update the remaining hours for this task')) return false;
    
    saving = true;
    submitToAction("updateEstimate");
    return true;
}

function addStory()
{   
    saving = true;
    submitToAction('addStory');
}

function removeStory()
{
    saving = true;
    submitToAction('removeStory');
}

function addTask()
{   
    saving = true;
    submitToAction('addTask');
}

function removeTask()
{
    saving = true;
    submitToAction('removeTask');
}

function newTask()
{   
    saving = true;
    submitToAction('newTask');
}

function setDetailActionButtons()
{
    if (document.forms[0] == null) return;

    var buttonTop;
    var buttonBottom;
    if (document.forms[0].name == 'edit')
    {
        buttonTop = document.getElementById('saveButtonTop');
        buttonBottom = document.getElementById('saveButtonBottom');
        
    }
    else if (document.forms[0].name == 'new')
    {
        buttonTop = document.getElementById('createButtonTop');
        buttonBottom = document.getElementById('createButtonBottom');
    }

    var canEdit = document.getElementById('isDirty').value == 'true' &&
                  document.getElementById('userCanEdit').value == 'True';

    if (buttonTop!= null) 
    {
        buttonTop.disabled = !canEdit;
    }
    if (buttonBottom!= null)
    {
        buttonBottom.disabled = !canEdit;
    }
}

function clearMultiSelect(name)
{
    control = document.getElementById(name);
    for (var i = 0; i < control.options.length; i++)
    {
        control.options[i].selected = false;
    }
}

function SortLink(orderBy)
{
    document.list.sortExpression.value = orderBy
    document.list.submit();
}

function deleteTest(theCell, testId) 
{
	var r=new Ajax.Request('deleteTest.rails',{parameters: 'testId=' + testId});
	if( document.createElement && document.childNodes ) 
	{
		var thisRow = theCell.parentNode.parentNode;
		thisRow.parentNode.removeChild(thisRow);
	}
} 

function addComponentRow()
{
    var handler = new ChildTableHandler("Component", "Name", "ComponentsArray", false, false, 0);
    handler.addRow();
}

function removeComponentRow(src)
{
    var handler = new ChildTableHandler("Component", "Name", "ComponentsArray", false, false, 0);
    handler.removeRow(src);
}

function addTestRow()
{
    var handler = new ChildTableHandler("Test", "Body", "TestsArray", true, true, 1);
    handler.addRow();
}

function removeTestRow(src)
{
    var handler = new ChildTableHandler("Test", "Body", "TestsArray", true, true, 1);
    handler.removeRow(src);
}

function ChildTableHandler(childClassName, contentPropertyName, arrayName, multiLine, numbered, headerRowCount)
{
    this.ChildClassName = childClassName;
    this.ArrayName = arrayName;
    this.ListIdName = childClassName.toLowerCase() + "List";
    this.CountIdName = childClassName.toLowerCase() + "Count";
    this.IndexPlaceholder = "{0}";
    this.IdId = "entity_" + this.ArrayName + "_" + this.IndexPlaceholder + "_Id";
    this.IdName = "entity." + this.ArrayName + "[" + this.IndexPlaceholder + "].Id";
    this.ContentId = "entity_" + this.ArrayName + "_" + this.IndexPlaceholder + "_" + contentPropertyName;
    this.ContentName = "entity." + this.ArrayName + "[" + this.IndexPlaceholder + "]." + contentPropertyName;
    this.ContentControlType = multiLine ? "textarea" : "input";
    this.MultiLine = multiLine;
    this.Numbered = numbered;
    this.HeaderRowCount = headerRowCount;
}

ChildTableHandler.prototype.ChildClassName;   
ChildTableHandler.prototype.ArrayName;
ChildTableHandler.prototype.ListIdName;
ChildTableHandler.prototype.CountIdName;
ChildTableHandler.prototype.IndexPlaceholder;
ChildTableHandler.prototype.IdId;
ChildTableHandler.prototype.IdName;
ChildTableHandler.prototype.ContentId;
ChildTableHandler.prototype.ContentName
ChildTableHandler.prototype.MultiLine;
ChildTableHandler.prototype.Numbered
ChildTableHandler.prototype.HeaderRowCount;
ChildTableHandler.prototype.ContentControlType;

ChildTableHandler.prototype.addRow = function()
{       
    var tbl = document.getElementById(this.ListIdName);
    var nextArrayIndex = tbl.rows.length - this.HeaderRowCount;
    var newContentBox = document.getElementById("new" + this.ChildClassName + "Content");
    var newContent= newContentBox.value;
    newContentBox.value = "";
    var row = tbl.insertRow(tbl.rows.length);
    var cellPos = 0;

    if (this.Numbered)
    {
        var cellNumber = row.insertCell(cellPos);
        cellPos++;
    }

    var cellInputText = row.insertCell(cellPos);
    
    var el = document.createElement('input');
    el.setAttribute('id', this.IdId.replace(this.IndexPlaceholder, nextArrayIndex));
    el.setAttribute('name', this.IdName.replace(this.IndexPlaceholder, nextArrayIndex));
    el.setAttribute('type', 'hidden');
    el.setAttribute('value', "00000000-0000-0000-0000-000000000000");
    cellInputText.appendChild(el); 
    el = document.createElement(this.ContentControlType);
   
    if (this.MultiLine)
    {
        el.setAttribute('rows', 4)
        el.innerHTML = newContent;
    }
    else
    {
        el.setAttribute('type', 'text');
        el.setAttribute('value', newContent);
    }
    el.setAttribute('id', this.ContentId.replace(this.IndexPlaceholder, nextArrayIndex));
    el.setAttribute('name', this.ContentName.replace(this.IndexPlaceholder, nextArrayIndex));
    el.setAttribute('onchange', "onchange='markDirty()'");
    el.setAttribute('style', 'width: 99%; font-family: Arial; font-size: 12px;');
    cellInputText.appendChild(el); 
    cellPos++;
    
    var cellRemoveLink = row.insertCell(cellPos);
    cellRemoveLink.setAttribute('style', "vertical-align:middle;"); 
    var el = document.createElement('a');
    el.setAttribute('href', "javascript:void(0);");
    el.setAttribute('onclick', "remove" + this.ChildClassName + "Row(this); return false;");
    el.innerHTML = "Remove"
    cellRemoveLink.appendChild(el); 
    
    this.setItemCount();
    markDirty();    
}

ChildTableHandler.prototype.removeRow = function(src)
{
    var row = src.parentNode.parentNode;    
    document.getElementById(this.ListIdName).deleteRow(row.rowIndex); 
    markDirty();
    this.renumberItems(); 
}

ChildTableHandler.prototype.renumberItems = function()
{
    var colIndex = this.Numbered ? 1 : 0;
    var table = document.getElementById(this.ListIdName);
    for (var i = 0 + this.HeaderRowCount; i < table.rows.length; i++)
    {
        var idInput = table.rows[i].cells[colIndex].getElementsByTagName("input")[0];
        idInput.id = this.IdId.replace(this.IndexPlaceholder, i);
        idInput.name = this.IdName.replace(this.IndexPlaceholder, i);

        var contentInput = table.rows[i].cells[colIndex]
            .getElementsByTagName(this.ContentControlType)[this.MultiLine ? 0 : 1];
        contentInput.id = this.ContentId.replace(this.IndexPlaceholder, i);
        contentInput.name = this.ContentName.replace(this.IndexPlaceholder, i);
    }
    this.setItemCount()
}

ChildTableHandler.prototype.setItemCount = function()
{
    var itemCount = document.getElementById(this.CountIdName);
    itemCount.value = document.getElementById(this.ListIdName).rows.length;
}