/**
/**
 * Javascript Chart.
 * Draws a chart based on structured data. Light-weight and simple to use.
 * Supports any number of items in either a column or bar style chart.
 * Value-labels and debug info can be switched on or off.
 * Use it as you see fit.
 *
 * Usage:
 * var chart1 = Chart;
 * chart1.init("chart1"); // Pass the element id of the chart container.
 * chart1.title = "Bar chart";
 * chart1.type = "column"; // Set either "column" or "bar".
 * chart1.debug = true;
 * chart1.showValues = false;
 * chart1.setData("A;1;B;2;C;3"); // Pass the data to the chart as a semi-colon delimited string.
 * chart1.render();
 * 
 * Author: Magnus Lundahl, Ekakan Systems AB. magnus.efferus@gmail.com
 * Version: 1.1
 * Revision history:
 * 1.1, 2007-may-01. Handle errors for incorrect data and more debug info.
 * 1.0, 2007-apr-01. Created.
 */
 
var Chart = {
	
	/**
	  * The graph object. Short-hand for "this". Set in the init-method.
	  */
	o				: null,
	
	/**
	  * The graph's container element. Passed as a string argument to the init-method.
	  */
	container		: null,
	
	/**
	  *
	  */
	title			: "",
	
	/**
	  * The chart type, bars or columns.
	  */
	type			: null,
	
	/**
	  * The graph data as an array.
	  */
	data			: null,
	
	/**
	  * The delimiter used for separating the data string passed
	  * to the method setData.
.	  */
	dataDelimiter 	: ";",
	
	/**
	  * The max value in the chart based on the current data. Will change
	  * If the data changes.
	  */
	ceiling			: 0,
	
	/**
	  * Flag if data item labels should be displayed.
	  */
	showValues		: true,
	
	/**
	  * Controls how far above the columns the values are positioned.
	  */
	valuesOffsetY	: -13,
	
	/**
	  *
	  */
	chartHeight			: "300px",
	
	/**
	  *
	  */
	chartWidth			: "100%",
	
	/**
	  * The labels for the data points.
	  */
	labels			: null,
	
	/**
	  * List of colors.
	  */
	colors			: new Array(
							"#5095D0",
							"#7A67E5",
							"#C956E9",
							"#FC79C7",
							"#CF6164",
							"#B0783B",
							"#A6A06C",
							"#7F9B46",
							"#8DB789",
							"#3FBD76"
							),
	
	/**
	  * Marker to keep track of which color will be used.
	  */
	colorIndex		: 0,
	
	/**
	  * Debug flag.
	  */
	debug			: false,
	
	/**
	  * A string array containing debug information.
	  */
	debugInfo		: null,
	
	/**
	  * Initialize the graph object.
	  */
	init				: function(containerId) {
		o = this;
		o.debug = false;
		o.debugInfo = new Array();
		o.data = null;
		o.ceiling = 0;
		o.showValues = true;
		o.labels = null;
		o.colorIndex = 0;
		
		o.container	= document.getElementById(containerId);
		
		o.debugInfo.push("init: Initializing the chart.");
				
		if (o.container == null) {
			o.debugInfo.push("init: (ERROR) Could not find the chart container element.");
			return;
		}			
	},
	
	/**
	  * Set the chart data from a delimited string in the following format:
	  * "item1;value1;item2;value2".
	  */
	setData				: function(dataString) {
		o.data = dataString.split(o.dataDelimiter);
		if (o.data.length%2) {
			o.debugInfo.push("setData: (ERROR) Data items does not match data values.");
			o.data = new Array();
		}
	},
	
	/**
	  * Sets the max value for the current chart. Done by finding the
	  * item with the highest value in the o.data array and then rounding
	  * it up to the next appropriate value.
	  */
	setCeiling			: function() {
		if (o.data == null || o.data.length < 1) {
			o.debugInfo.push("setCeiling: Data is null or length is zero.");
			return false;
		}
		
		// Iterate through all the data values and find the highest value.
		for (i=0;i<o.data.length;i+=2) {
			var dataValue = new Number(o.data[i+1].replace(/,/g, "."));
			
			if (dataValue > o.ceiling) {
				o.ceiling = dataValue;
			}
		}
		
		/*
		  * Here we take the highest value that we found and round it up
		  * to an appropriate number which will represent our top value
		  * in the chart.
		  */
		var p;
		p = o.ceiling < 10 ? 0 : p; // By 1
		p = o.ceiling >= 10 && o.ceiling < 100 ? -1 : p; // 10
		p = o.ceiling >= 100 && o.ceiling < 1000 ? -2 : p; // 100
		p = o.ceiling >= 1000 && o.ceiling < 10000 ? -3 : p; // 1000
		p = o.ceiling >= 10000 && o.ceiling < 100000 ? -4 : p; // 10000
		p = o.ceiling >= 100000 && o.ceiling < 1000000 ? -5 : p; // 100000
		p = o.ceiling >= 1000000 && o.ceiling < 10000000 ? -6 : p; // 100000
		
		o.ceiling = Math.ceil(o.ceiling * Math.pow(10,p))/Math.pow(10,p);
		
		o.debugInfo.push("setCeiling: Ceiling is set to: " + o.ceiling);
	},
		
	/**
	  * Render the graph on screen. Appends elements to the container element.
	  * This method sets some crusial style properties to the elements to make
	  * the chart render correctly.
	  */
	render				: function() {
		// Specify heights in percent.
		var labelsHeight	= 15;
		
		var chartArea;
		var chartTitle;
		var plotArea;
		
        o.container.style.width = o.chartWidth;
        o.container.style.height = o.chartHeight;
		
		// Create the chart title.
		chartTitle = document.createElement("div");
		chartTitle.id = "chartTitle";
		chartTitle.className = "chartTitle";
		chartTitle.style.position = "relative";
		chartTitle.style.height = "10px";
		chartTitle.style.textAlign = "left";
		chartTitle.innerHTML = o.title;
		
		// Create the outer-most chart container called the "Chart area".
		chartArea = document.createElement("div");
		chartArea.id = "chartArea";
		chartArea.className = "chartArea";
		chartArea.style.position = "relative";
		chartArea.style.height = "100%";
		
		// Create the "Plot area".
		plotArea = document.createElement("div");
		plotArea.id = "plotArea";
		plotArea.className = "plotArea";
		plotArea.style.position = "relative";
		plotArea.style.height = (95 - labelsHeight) + "%";
		plotArea.style.marginLeft = "5px";
		plotArea.style.marginRight = "5px";
		plotArea.style.marginTop = "5px";
		plotArea.style.marginBottom = "5px";
		
		// Check if the chart has been provided with any data.
		if (o.data == null) {
			o.debugInfo.push("render: (ERROR) Data appears to be null.");
			return;
		}
		
		o.debugInfo.push("render: " + (o.data.length / 2) + " data items found.");
		
		// Calculate the ceiling value for the chart.
		o.setCeiling();
		
		// Create the grid lines.
		if (o.type == "column") {
			plotArea.appendChild(o.getColumnGrid());
		} else if (o.type == "bar") {
			plotArea.appendChild(o.getBarGrid());
		}
		
		// Create the bars/columns table.
		if (o.type == "column") {
			plotArea.appendChild(o.getColumns());
		} else if (o.type == "bar") {
			plotArea.appendChild(o.getBars());
		}
		
		// Append the the plot area.
		chartArea.appendChild(plotArea);
		
		// Append the labels.
        if (o.labels == null) {
		    o.debugInfo.push("render: (ERROR): Legend is null.");
        } else {
		    chartArea.appendChild(o.labels);
		}
		
		o.container.appendChild(chartTitle);
		o.container.appendChild(chartArea);
		
		// Append the debug info if debug is true.
		if (o.debug) {
			var debugPane = document.createElement("div");
			debugPane.className = "debugPane";
			debugPane.style.height = "20%";
			debugPane.style.overflow = "auto";
			debugPane.innerHTML = o.getDebugInfo("<br />");
			
			o.container.appendChild(debugPane);
		}
	},
	
	/**
	  * Generate column grid lines spaced according to the highest
	  * data point value.
	  * Returns a div element containing all grid line elements.
	  */
	getColumnGrid		: function() {
		var gridContainer;
		
		gridContainer = document.createElement("div");
		gridContainer.id = "gridContainer";
		gridContainer.className = "gridContainer";
		gridContainer.style.position = "absolute";
		gridContainer.style.width = "100%";
		gridContainer.style.height = "100%";
		
		// Find out the increment for each grid line.
		var increment = o.getGridIncrement();
				
		var marker = increment;
		while (marker < o.ceiling) {
			var gridLine = document.createElement("div");
			gridLine.className = "gridLine";
			gridLine.style.position = "absolute";
			gridLine.style.width = "100%";
			
			gridLine.style.borderBottomWidth = "1px";
			gridLine.style.borderBottomStyle = "solid";
			gridLine.style.borderBottomColor = "";
				
			gridLine.style.bottom = ((marker / o.ceiling) * 100) + "%";
		
			gridLine.innerHTML = marker;
		
			gridContainer.appendChild(gridLine);
			
			
			marker += increment;
		}
				
		return gridContainer;
	},
	
	/**
	  * Get the increment for the grid lines depending on th highest value
	  * in the chart.
	  */
	getGridIncrement	: function() {
		var increment = 0;
		increment = o.ceiling <= 10 ? 1 : increment; // 1
		increment = o.ceiling > 10 && o.ceiling <= 100 ? 10 : increment; // 10
		increment = o.ceiling > 100 && o.ceiling <= 1000 ? 100 : increment; // 100
		increment = o.ceiling > 1000 && o.ceiling <= 10000 ? 1000 : increment; // 1000
		increment = o.ceiling > 10000 && o.ceiling <= 100000 ? 10000 : increment; // 10000
		increment = o.ceiling > 100000 && o.ceiling <= 1000000 ? 100000 : increment; // 100000
		increment = o.ceiling > 1000000 && o.ceiling <= 10000000 ? 1000000 : increment; // 1000000
		increment = o.ceiling > 10000000 && o.ceiling <= 100000000 ? 10000000 : increment; // 10000000
		increment = o.ceiling > 100000000 && o.ceiling <= 1000000000 ? 100000000 : increment; // 100000000
		
		return increment;
	},
	
	/**
	  * Generates a columns table. A table is actually nice
	  * because it eases the spacing between the columns.
	  * This method contains some crucial style properties for
	  * making the chart render correctly.
	  */
	 getColumns			: function() {		
		var table = document.createElement("table");
		table.id = "plotTable";
		table.cellSpacing = "0";
		table.cellPadding = "0";
		table.border = "0";
		table.style.position = "absolute";
		table.style.width = "90%";
		table.style.height = "100%";
		table.style.right = "0px";
		
		var body = document.createElement("tbody");		
		var row = document.createElement("tr");
				
		// Iterate the data array and create columns for each data item.
		for (i=0;i<o.data.length;i+=2) {
			var cell;
			
			cell = document.createElement("td");
			cell.style.verticalAlign = "bottom";
			cell.style.textAlign = "middle";
			cell.style.height = "100%";
			cell.style.width = (parseInt(table.style.width) / (o.data.length / 2)) + "%";
			
			/*
			 * Append the data item column to the cell. The column is
			 * returned ready-formatted with the proper height, color
			 * and labels.
			 */
			var dataItemLabel = o.data[i];
			var dataItemValue = o.data[i+1].replace(/,/g, ".");
			var columnElement = o.getColumn(dataItemLabel, dataItemValue);
			cell.appendChild(columnElement);
			
			// If value labels should be displayed we append them to the column.
			if (o.showValues) {
				var valueElement = o.getColumnValue(dataItemValue);
				columnElement.appendChild(valueElement);
			}
			
			// Append each cell to the row.
			row.appendChild(cell);
		}
		
        var labelsRow = document.createElement("tr");
        // Iterate the data array and create labels for each data item.
		for (i=0;i<o.data.length;i+=2) {
		
            var cell = document.createElement("td");
			cell.style.verticalAlign = "top";
			cell.style.textAlign = "middle";
			cell.style.height = "100%";
			cell.innerHTML = o.data[i];
			labelsRow.appendChild(cell);
		}
		
		// Append the rows to the table.
		body.appendChild(row);
		body.appendChild(labelsRow);
		table.appendChild(body);
		
		return table;
	 },
	
	/**
	  * Generates a column representing a data item.
	  * Returns the element.
	  */
	getColumn			: function(dataItemLabel, dataItemValue) {
		// Create the actual column and set the hight according to the data item's value.
		var column;
		column = document.createElement("div");
		column.className = "dataPoint";
		column.style.position = "relative";
		column.style.width = "90%";
		column.style.height = ((dataItemValue / o.ceiling) * 100) + "%";
		if (parseInt(column.style.height) == 0) {
			column.style.height = "1px";
		}
		if (o.showValues == false) {
			column.style.fontSize = "0px";
		}
		
		// Find the correct color for the data point.
		var dataItemColor = o.getColor();
		column.style.background = dataItemColor;
		
		// Append the the item to the labels.
		o.appendToLabels(dataItemLabel);
		
		return column;
	},
	
	/**
	  * Get the element for the column value label.
	  */
	getColumnValue		: function(dataItemValue) {
		// Create the value text associated with the data item.
		var value;
		value = document.createElement("div");
		value.className = "value";
		value.style.position = "absolute";
		value.style.left = "0px";
		value.style.width = "100%";
		value.style.marginTop = o.valuesOffsetY + "px";
		value.style.textAlign = "center";
		
		value.innerHTML = dataItemValue;
		
		return value;
	},
	
	/**
	  * Generate bar grid lines spaced according to the highest
	  * data point value.
	  * Returns a div element containing all grid line elements.
	  */
	getBarGrid			: function() {
		var gridContainer;
		
		gridContainer = document.createElement("div");
		gridContainer.id = "gridContainer";
		gridContainer.className = "gridContainer";
		gridContainer.style.position = "absolute";
		gridContainer.style.width = "100%";
		gridContainer.style.height = "100%";
		
		// Find out the increment for each grid line.
		var increment = o.getGridIncrement();
				
		var marker = increment;
		while (marker < o.ceiling) {
			var gridLine = document.createElement("div");
			gridLine.className = "gridLine";
			gridLine.style.position = "absolute";
			gridLine.style.height = "100%";
			
			gridLine.style.borderLeftWidth = "1px";
			gridLine.style.borderLeftStyle = "solid";
			gridLine.style.borderLeftColor = "";
				
			gridLine.style.left = ((marker / o.ceiling) * 100) + "%";
		
			gridLine.innerHTML = marker;
		
			gridContainer.appendChild(gridLine);
			
			
			marker += increment;
		}
				
		return gridContainer;
	},
	
	/**
	  * Generates a bars table. A table is actually nice
	  * because it eases the spacing between the bars.
	  * This method contains some crucial style properties for
	  * making the chart render correctly.
	  */
	 getBars			: function() {		
		var table = document.createElement("table");
		table.id = "plotTable";
		table.cellSpacing = "0";
		table.cellPadding = "0";
		table.border = "0";
		table.style.position = "absolute";
		table.style.width = "100%";
		table.style.height = "90%";
		table.style.bottom = "0px";
		
		var body = document.createElement("tbody");		
				
		// Iterate the data array and create rows for each data item.
		for (i=0;i<o.data.length;i+=2) {
			var row = document.createElement("tr");
			
			var labelCell = document.createElement("td");
			labelCell.style.width = "100%";
			labelCell.innerHTML = o.data[i];		
			
			var cell = document.createElement("td");
			cell.style.width = "100%";
			cell.style.height = (parseInt(table.style.height) / (o.data.length / 2)) + "%";
						
			/*
			  * Append the data item to the cell. The bar is
			  * returned ready-formatted with the proper height, color
			  * and labels.
			  */
			  
			var dataItemValue = o.data[i+1].replace(/,/g, ".");
			var barElement = o.getBar(dataItemValue);
			cell.appendChild(barElement);
			
			// Append the cells to the row.
			row.appendChild(labelCell);
			row.appendChild(cell);
			
			// Append the row to the table.
			body.appendChild(row);
		}
		
		table.appendChild(body);
		
		return table;
	 },
	
	/**
	  * Generates a bar representing a data item.
	  * Returns the element.
	  */
	getBar			: function(dataItemValue) {
		// Create the actual bar and set the width according to the data item's value.
		var bar;
		bar = document.createElement("div");
		bar.className = "dataPoint";
		bar.style.position = "relative";
		bar.style.height = "90%";
		bar.style.width = ((dataItemValue / o.ceiling) * 100) + "%";
		if (parseInt(bar.style.width) == 0) {
			bar.style.width = "1px";
		}
		if (o.showValues == false) {
			bar.style.fontSize = "0px";
		}
				
		// Find the correct color for the data point.
		var dataItemColor = o.getColor();
		bar.style.background = dataItemColor;
		
		// If value markers should be displayed we append them to the bar.
		if (o.showValues) {
			var valueElement = o.getBarValue(dataItemValue);
			bar.appendChild(valueElement);
		}
		
		return bar;
	},
	
	/**
	  * Get the element for the bar value label.
	  */
	getBarValue		: function(dataItemValue) {
		// Create the value text associated with the data item.
		var value;
		value = document.createElement("div");
		value.className = "value";
		value.style.position = "absolute";
		value.style.width = "100%";
		value.style.height = "100%";
		value.style.textAlign = "right";
		
		value.innerHTML = dataItemValue;
		
		return value;
	},
	
	/**
	  * Gets a color by o.colorIndex
	  */
	getColor			: function() {
		if (o.colorIndex >= o.colors.length) {
			o.colorIndex = 0;
		}
		
		var color = o.colors[o.colorIndex];
		o.colorIndex++;
		
		return color;
	},
	
	/**
	  * Debug the chart object. Iterates the debugInfo array and creates
	  * a string. The method only returns the formatted string. It is up to
	  * you to print it to the browser or alert it.
	  */
	getDebugInfo		: function(linebreakString) {
		var debugString = "";
		
		for (i=0;i<o.debugInfo.length;i++) {
			debugString += "- " + o.debugInfo[i] + linebreakString;
		}
		
		return debugString;
	}
}