//*****************************************************************************
// Filename: Graph.js
// Description: Plot a column graph and/or line graph.
// Version: 2.0
// Browser Compatibility: IE5.5+, NS6+
//
// COPYRIGHT (C) 2002 WAGNER DOSANJOS
// THIS PROGRAM IS FREE SOFTWARE; YOU CAN REDISTRIBUTE IT AND/OR MODIFY IT
// UNDER THE TERMS OF THE GNU GENERAL PUBLIC LICENSE AS PUBLISHED BY THE FREE
// SOFTWARE FOUNDATION; EITHER VERSION 2 OF THE LICENSE, OR (AT YOUR OPTION)
// ANY LATER VERSION. THIS PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE
// USEFUL, BUT WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF
// MERCHANTABILITY OF FITNESS FOR A PARTICULAR PURPOSE. SEE THE GNU GENERAL
// PUBLIC LICENSE FOR MORE DETAILS.
//
// YOU SHOULD HAVE RECEIVED A COPY OF THE GNU GENERAL PUBLIC LICENSE ALONG
// WITH THIS PROGRAM; IF NOT, WRITE TO:
//
// THE FREE SOFTWARE FOUNDATION, INC.,
// 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA
//
// Bugs/Comments: wanjos@yahoo.com
//
// Change History:
// 10-03-2003:
//  o Release v2.1
//      - Limited support to Netscape 6+
// 09-30-2003:
//  o Release v2.0
//      - Support for vertical orientation
//      - Bug fixes
//      - New properties
// 12-16-2002:
//  o Release v1.0
//
//*****************************************************************************
// Graph.js
//
// This script contains a useful object that can be used to plot column and
// line graphs.
//
// Limitations:
// 1. This script only handles positive values.
// 2. Only supports one graph per page
// 3. In Netscape Line Graphs are not supported
//
// Object Name: Graph
//
// Constructor:
// . new Graph(n) [where n is the number of X items]
//
// Properties:
// . parent (HTMLElement), default: document.body [where to draw the graph]
// . version (String) [version of the object (ie '1.0')]
// . orientation (String), 'horizontal'/'vertical', default: 'horizontal' [graph orientation]
// . adjustMin (Boolean), true/false, default: false [adjust Y Axis start value]
// . showBar (Boolean), true/true, default: true [show column graph]
// . showLine (Boolean), true/true, default: false [show line graph]
// . fontFamily (String), default: 'Arial' [font family used in the graph]
// . title (String) [graph title]= '';
// . titleColor (String), default: 'black' [graph title color]
// . yCaption (String) [Y Axis caption]
// . yCaptionColor (String), default: 'black' [Y Axis caption color]
// . xCaption (String) [X Axis caption]
// . xCaptionColor (String), default: 'black' [X Axis caption color]
// . xCaptionOrientation (String), 'horizontal'/'vertical', default: 'horizontal' [x Caption orientation]
// . xWidth (Integer), default: 60 [width reserved for each x value]
// . xValuesHeight (Integer), default: 100 [height reserved for x caption]
// . barColor (String), default: 'blue' [bar color]
// . barLightBorderColor (String), default: 'light-blue' [bar light border color]
// . barDarkBorderColor (String), default: 'olive' [bar dark border color]
// . lineColor (String), default: 'black' [line color]
// . lineWidth (Integer), default: 1 [line width]
// . markColor (String), default: 'red' [line graph mark color]
// . markSize (Integer), default: 4 [line graph mark size]
// . showValues (Boolean), true/false, default: true [show values]
// . gridColor (String), default: 'gray' [grid color]
// . gridBgColor (String), default: '#d0d0d0' [grid background color]
//
// Methods
// . draw() [draw graph]
//
//*****************************************************************************
var isIE = (document.all) ? true : false;

function Graph(size){
    this.parent = document.body;
    this.version = '2.0';
    this.orientation = 'horizontal';
    this.adjustMin = false;
    this.showBar = true;
    this.showLine = false;
    this.fontFamily = 'Arial';
    this.title = '';
    this.titleColor = 'black';
    this.yCaption = '';
    this.yCaptionColor = 'black';
    this.xCaption = '';
    this.xCaptionColor = 'black';
    this.xCaptionOrientation = 'horizontal';
    this.xWidth = 50;
    this.xValuesHeight = 50;
    this.barColor = 'blue';
    this.barLightBorderColor = 'light-blue';
    this.barDarkBorderColor = 'olive';
    this.lineColor = 'black';
    this.lineWidth = 1;
    this.markColor = 'red';
    this.markSize = 4;
    this.showValues = true;
    this.gridColor = 'gray';
    this.gridBgColor = '#d0d0d0';
    this.xValues = new Array(size);
    this.draw = DrawGraph;
}

function DrawGraph(){
    if (this.orientation == 'vertical')
        DrawGraphV(this);
    else
        DrawGraphH(this);
}

// Draw horizontal graph
function DrawGraphH(bg){
    var i, e, h, v, g, step, rect, d=document;
    var he, heTop, heAxis, heX;
    var yMin, yMax, yStep, ratio;

    // find the biggest value
    yMax = -1;
    for (i=0; i < bg.xValues.length; i++){
        if (bg.xValues[i][0] > yMax)   yMax = bg.xValues[i][0];
    }

    // find the smallest value
    if (!bg.adjustMin){
        yMin = 0;
    }else{
        yMin = yMax;
        for (i=0; i < bg.xValues.length; i++){
            if (bg.xValues[i][0] < yMin)   yMin = bg.xValues[i][0];
        }
    }

    // Calculate scale values
    var n = Math.pow(10,Math.floor(Math.log(yMax) / Math.LN10));
    var np = n / 10;
    var nn = n * 10;

    if (n > 1){
        if ((yMax % n) == 0){
            yMax += n;
        }

        yMin = Math.floor(yMin / n) * n;
        yMax = Math.ceil(yMax / n) * n;
    }else{
        if ((yMax % n) == 0){
            yMax += n;
        }
    }

    ratio = 300 / (yMax - yMin); // 300 = y axis length in pixels

    yStep = (yMax - yMin) / 8;
    if (yStep <= np)            yStep = np;
    else if (yStep <= (2 * np)) yStep = 2 * np;
    else if (yStep <= (5 * np)) yStep = 5 * np;
    else                        yStep = n;

    step=Math.round(yStep*ratio);  // scale step in pixels
    heTop = 10;
    he = step * ((yMax - yMin) / yStep); // total graph height in pixels
    heAxis = 5;
    heX = bg.xValuesHeight;

    // Build style
    d.writeln('<style>');
    d.writeln('.graph { font-family: ' + bg.fontFamily + '; }');
    d.writeln('.ycaption { font-weight:bolder; font-size: 8pt; writing-mode:tb-rl; }');
    d.writeln('.xcaption { font-weight:bolder; font-size: 10pt; }');
    d.writeln('.yvalue { font-size: 8pt; position=absolute; top=8; }');

    if (bg.xCaptionOrientation == 'horizontal')
        d.writeln('.xvalue { font-size: 6pt; }');
    else
        d.writeln('.xvalue { font-size: 6pt; writing-mode:tb-rl; }');

    d.writeln('.xyvalue { font-size: 6pt; }');
    d.writeln('.bar');
    d.writeln('{');
    d.writeln('background-color: ' + bg.barColor + ';');
    d.writeln('BORDER-RIGHT: 2px outset ' + bg.barDarkBorderColor + ';');
    d.writeln('BORDER-LEFT: 1px outset ' + bg.barLightBorderColor + ';');
    d.writeln('BORDER-TOP: 2px outset ' + bg.barLightBorderColor + ';');
    d.writeln('BORDER-BOTTOM: 0px outset ' + bg.barDarkBorderColor + ';');
    d.writeln('}');
    d.writeln('.gridleft { border-right:1px solid gray; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.grid { background-color: ' + bg.gridBgColor + '; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.gridright { background-color: ' + bg.gridBgColor + '; border-right:1px solid ' + bg.gridColor + '; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.gridbottom { border-right:1px solid gray; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.datatable { position=absolute; top=5; left=0; }');
    d.writeln('.gridtable { position=absolute; top=6; left=0; }');
    d.writeln('</style>');

    // Title
    d.writeln('<table border=0 cellspacing=0 cellpadding=0 id=wrapper class=graph>');
    d.writeln('<tr><td colspan=' + (bg.xValues.length + 3) + ' align=center><h2>' + bg.title + '</h2></td></tr>');

    // Y Caption
    d.writeln('<tr><td style="position:relative">');
    d.writeln('<table border=0 cellspacing=0 cellpadding=0 id=graph>');
    d.writeln('<tr>');
    d.writeln('<td valign=middle nowrap align=center height=' + (heTop + he + heAxis + heX) + '>');
    d.writeln('<SPAN class=ycaption><br>' + bg.yCaption + '</SPAN>');
    d.writeln('</td>');

    // Scale
    d.writeln('<td valign=top style="position:relative" id=scalewrapper>');
    d.writeln('<table cellspacing=0 cellpadding=0 border=0 class=yvalue id=scale>');

    h=heTop;
    for (i=yMax; i >= yMin; i-=yStep){
        d.writeln('<tr><td height=' + h + ' valign=bottom align=right>' + i + '</td></tr>');
        h=step;
    }

    d.writeln('</table>');
    d.writeln('</td>');

    // Left Grid
    d.writeln('<td valign=bottom align=right width=5 style="position:relative">');
    d.writeln('<table cellspacing=0 cellpadding=0 border=0 width="100%" class=gridtable>');

    h=heTop;
    for (i=yMax; i >= yMin; i-=yStep){
        d.writeln('<tr><td height=' + h + '><img src=dot.gif border=0></td></tr>');
        h=step + ' class=gridleft';
    }

    d.writeln('<tr><td height=' + heAxis + ' class=gridleft><img src=dot.gif border=0></td></tr>');
    d.writeln('<tr><td height=' + heX + '><img src=dot.gif border=0></td></tr>');
    d.writeln('</table>');
    d.writeln('</td>');

    // Plot Bars
    for (var x=0; x < bg.xValues.length; x++){
        v=bg.xValues[x][0];
        g = (x < (bg.xValues.length-1)) ? 'grid' : 'gridright';
        d.writeln('<td valign=bottom align=center width=' + bg.xWidth + ' style="position:relative">');

        // Grid
        if (isIE){
            d.writeln('<table cellspacing=0 cellpadding=0 border=0 width="100%" class=gridtable>');

            h=heTop;
            for (i=yMax; i >= yMin; i-=yStep){
                d.writeln('<tr><td height=' + h + '><img src=dot.gif border=0></td></tr>');
                h=step + ' class=' + g;
            }
            d.writeln('</table>');
        }

        // Bar
        h = Math.ceil((yMax-v)*ratio);
        if (h > he) h = he;
        d.writeln('<table cellspacing=0 cellpadding=0 border=0 width="100%" class=datatable>');
        d.writeln('<tr><td valign=bottom align=center height=' + (heTop + h) + ' class=xyvalue>'
                    + (bg.showValues ? v : '<img src=dot.gif border=0>') + '</td></tr>');
        h = he - h;
        d.writeln('<tr><td height=' + h + ' align=center>');
        d.write('<table width="50%" cellspacing=0 cellpadding=0 ' + ((bg.showBar) ? 'class=bar' : '') + ' id=bar>');
        d.writeln('<tr><td height=' + h + '><img src=dot.gif border=0></td></tr></table></td></tr>');
        d.writeln('<tr><td height=' + heAxis + ' class=gridbottom><img src=dot.gif border=0></td></tr>');
        d.writeln('<tr><td height=' + heX + ' valign=top align=center class=xvalue>' + bg.xValues[x][1] + '</td></tr>');
        d.writeln('</table>');
        d.writeln('</td>');
    }

    // X Caption
    d.writeln('</tr>');
    d.writeln('<tr>');
    d.writeln('<td colspan=3></td>');
    d.writeln('<td align=center colspan=' + bg.xValues.length + '><h2>' + bg.xCaption + '</h2></td>');
    d.writeln('</tr>');
    d.writeln('</table>');
    d.writeln('</td></tr></table>');

    // Stablish parenthood
    bg.parent.appendChild(d.getElementById('wrapper'));

    // Adjust scale width
    rect = d.getElementById('scale').getClientRects()(0);
    d.getElementById('scalewrapper').width = rect.right - rect.left;

    // Make wrapper table width fixed
    e = d.getElementById('graph');
    rect = e.getClientRects()(0);
    d.getElementById('wrapper').width = e.width = rect.right - rect.left;

    // Plot Lines
    if (isIE && bg.showLine){
        // Make wrapper table position fixed
        e = d.getElementById('wrapper');
        rect = e.getClientRects()(0);
        e.style.position = 'absolute';
        e.style.top = rect.top;
        e.style.left = rect.left;

        e = d.getElementsByName('bar');
        for (i=1; i < e.length; i++){
            var from = e[i-1].getClientRects()(0);
            var to = e[i].getClientRects()(0);
            DrawLine(bg.lineWidth, bg.lineColor, bg.markSize, bg.markColor,
                    from.left - 4 + (from.right - from.left) / 2, from.top,
                    to.left - 4 + (to.right - to.left) / 2  , to.top);
        }
    }
}

// Draw vertical graph
function DrawGraphV(bg){
    var i, e, v, g, step, rect, d=document;
    var yMin, yMax, yStep, ratio;

    // find the biggest value
    yMax = -1;
    for (i=0; i < bg.xValues.length; i++){
        if (bg.xValues[i][0] > yMax)   yMax = bg.xValues[i][0];
    }

    // find the smallest value
    if (!bg.adjustMin){
        yMin = 0;
    }else{
        yMin = yMax;
        for (i=0; i < bg.xValues.length; i++){
            if (bg.xValues[i][0] < yMin)   yMin = bg.xValues[i][0];
        }
    }

    // Calculate scale values
    var n = Math.pow(10,Math.floor(Math.log(yMax) / Math.LN10));
    var np = n / 10;
    var nn = n * 10;

    if (n > 1){
        if ((yMax % n) == 0){
            yMax += n;
        }

        yMin = Math.floor(yMin / n) * n;
        yMax = Math.ceil(yMax / n) * n;
    }else{
        if ((yMax % n) == 0){
            yMax += n;
        }
    }

    ratio = 300 / (yMax - yMin); // 300 = y axis length in pixels

    yStep = (yMax - yMin) / 8;
    if (yStep <= np)            yStep = np;
    else if (yStep <= (2 * np)) yStep = 2 * np;
    else if (yStep <= (5 * np)) yStep = 5 * np;
    else                        yStep = n;

    step = Math.round(yStep*ratio);  // scale step in pixels

    // Build style
    d.writeln('<style>');
    d.writeln('.graph { font-family: ' + bg.fontFamily + '; }');
    d.writeln('.ycaption { font-weight:bolder; font-size: 14pt; writing-mode:tb-rl; }');
    d.writeln('.xcaption { font-weight:bolder; font-size: 8pt; }');
    d.writeln('.yvalue { font-size: 8pt; }');
    d.writeln('.xvalue { font-size: 6pt; }');
    d.writeln('.xyvalue { font-size: 6pt; }');
    d.writeln('.bar');
    d.writeln('{');
    d.writeln('background-color: ' + bg.barColor + ';');
    d.writeln('BORDER-RIGHT: 2px outset ' + bg.barDarkBorderColor + ';');
    d.writeln('BORDER-LEFT: 1px outset ' + bg.barLightBorderColor + ';');
    d.writeln('BORDER-TOP: 2px outset ' + bg.barLightBorderColor + ';');
    d.writeln('BORDER-BOTTOM: 0px outset ' + bg.barDarkBorderColor + ';');
    d.writeln('}');
    d.writeln('.gridleft { border-right:1px solid gray; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.grid { background-color: ' + bg.gridBgColor + '; border-right:1px solid ' + bg.gridColor + '; }');
    d.writeln('.gridright { background-color: ' + bg.gridBgColor + '; border-right:1px solid ' + bg.gridColor + '; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.gridbottom { border-right:1px solid gray; border-top:1px solid ' + bg.gridColor + '; }');
    d.writeln('.datatable { position=absolute; top=10; left=0; }');
    d.writeln('.gridtable { position=absolute; left=0; }');
    d.writeln('</style>');

    // Title
    d.writeln('<table align=center border=0 cellspacing=0 cellpadding=0 id=wrapper class=graph>');
    d.writeln('<tr><td colspan=2 align=center><h2>' + bg.title + '</h2></td></tr>');

    // X Caption
    d.writeln('<tr>');
    d.writeln('<td valign=middle nowrap align=center width=' + bg.xWidth + '>');
    d.writeln('<SPAN class=ycaption><br>' + bg.xCaption + '</SPAN>');
    d.writeln('</td>');
    d.writeln('<td>');
    d.writeln('<table border=0 cellspacing=0 cellpadding=0 id=graph style="position:relative">');

    // Plot Bars
    for (var x=0; x < bg.xValues.length; x++){
        v=bg.xValues[x][0];

        d.writeln('<tr><td valign=middle align=right height=' + bg.xWidth + ' class=xvalue>' + bg.xValues[x][1] + '</td>');
        d.writeln('<td height=' + bg.xWidth + ' class=gridleft><img src=dot.gif border=0 width=5></td>');
        d.writeln('<td  width=' + Math.ceil(yMax*ratio) + ' height=' + bg.xWidth + ' valign=middle align=left style="position:relative">');

        if (isIE){
            d.writeln('<table cellspacing=0 cellpadding=0 border=0 class=gridtable><tr>');
            g = (x==0) ? 'gridright' : 'grid';
            for (i=yMin; i < yMax; i+=yStep) d.writeln('<td><img src=dot.gif border=0 height=' + bg.xWidth + ' width=' + step + ' class=' + g + '></td>');
            d.writeln('</tr></table>');
        }

        d.writeln('<table cellpadding=0 cellspacing=0 border=0 class=datatable>');
        d.writeln('<tr><td id=bar' + ((bg.showBar) ? ' class=bar' : '') + '><img src=dot.gif border=0 height=' + (bg.xWidth/2) + ' width=' + Math.ceil(v*ratio) + '>');
        d.writeln('<td class=xyvalue>&nbsp;' + v + '</td></tr>');
        d.writeln('</table>');
        d.writeln('</td></tr>');
    }

    // Scale
    d.writeln('<tr><td valign=middle align=right height=5 class=xvalue></td>');
    d.writeln('<td height=5 class=gridleft><img src=dot.gif border=0 width=5></td>');
    d.writeln('<td height=5 valign=top align=left style="position:relative">');

    d.writeln('<table cellspacing=0 cellpadding=0 border=0 class=gridtable><tr>');
    for (i=yMin; i < yMax; i+=yStep) d.writeln('<td align=left class=gridbottom><img src=dot.gif border=0 height=5 width=' + step + ' ></td>');
    d.writeln('</tr></table></td></tr>');

    d.writeln('<tr><td></td><td></td>');
    d.writeln('<td valign=top align=left style="position:relative">');
    d.writeln('<table cellspacing=0 cellpadding=0 border=0 class=gridtable><tr>');
    for (i=yMin; i <= yMax; i+=yStep) d.writeln('<td align=left valign=top class=yvalue>' + i + '<br><img src=dot.gif border=0 height=1 width=' + step + ' ></td>');
    d.writeln('</tr></table></td></tr>');

    // Y Caption
    d.writeln('<tr><td></td><td></td><td valign=top align=center class=xcaption>' + (isIE ? '<br>' : '') + bg.yCaption + '</td></tr>');

    d.writeln('</table></td></tr>'); // id=graph
    d.writeln('</table>'); // id=wrapper

    // Stablish parenthood
    bg.parent.appendChild(d.getElementById('wrapper'));

    // Plot Lines
    if (isIE && bg.showLine){
        // Make wrapper table position fixed
        e = d.getElementById('wrapper');
        rect = e.getClientRects()(0);
        e.style.position = 'absolute';
        e.style.top = rect.top;
        e.style.left = rect.left;

        e = d.getElementsByName('bar');
        for (i=1; i < e.length; i++){
            var from = e[i-1].getClientRects()(0);
            var to = e[i].getClientRects()(0);
            DrawLine(bg.lineWidth, bg.lineColor, bg.markSize, bg.markColor,
                    from.right - 4, from.top - 4 + (from.bottom - from.top) / 2,
                    to.right   - 4, to.top - 4 + (to.bottom - to.top) / 2);
        }
    }
}

function DrawLine(size,color,marksize,markcolor,x1,y1,x2,y2){
    var w = Math.abs(x2 - x1);
    var h = Math.abs(y2 - y1);
    var x, y, xInc, yInc;

    if (w > h){
        xInc = (x1 < x2) ? 1 : -1;
        yInc = (y1 < y2) ? (h / w) : -(h / w);
    }else{
        yInc = (y1 < y2) ? 1 : -1;
        xInc = (x1 < x2) ? (w / h) : -(w / h);
    }

    x = x1;
    y = y1;
    while (   (xInc > 0 && x <= x2) || (xInc < 0 && x >= x2)
           || (yInc > 0 && y <= y2) || (yInc < 0 && y >= y2)){
        document.writeln('<SPAN id=line STYLE=\"position: absolute;');
        document.writeln('left: ' + Math.round(x) + 'px; top: ' + Math.round(y) + 'px;');
        document.writeln('width: ' + size + 'px; height: ' + size + 'px;');
        document.writeln(' clip: rect(0 ' + size + ' ' + size + ' 0); background-color: ' + color + ';\"></SPAN>');
        x += xInc;
        y += yInc;
    }

    document.writeln('<SPAN id=line STYLE=\"position: absolute;');
    document.writeln('left: ' + Math.round(x1) + 'px; top: ' + Math.round(y1) + 'px;');
    document.writeln('width: ' + marksize + 'px; height: ' + marksize + 'px;');
    document.writeln(' clip: rect(0 ' + marksize + ' ' + marksize + ' 0); background-color: ' + markcolor + ';\"></SPAN>');

    document.writeln('<SPAN id=line STYLE=\"position: absolute;');
    document.writeln('left: ' + Math.round(x2) + 'px; top: ' + Math.round(y2) + 'px;');
    document.writeln('width: ' + marksize + 'px; height: ' + marksize + 'px;');
    document.writeln(' clip: rect(0 ' + marksize + ' ' + marksize + ' 0); background-color: ' + markcolor + ';\"></SPAN>');
}