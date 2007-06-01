/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;

namespace StoryVerse.WebUI.ViewComponents
{
    public class ChartProperties
    {
        private IDictionary<object, decimal> source;
        private string title;
        private string cssClass;
        private int? longestBarPixels;
        private int? barWidthPixels;
        private string labelFormat;
        private string emptyMessage;
        private ChartOrientation? orientation;

        public IDictionary<object, decimal> Source
        {
            get { return source; }
            set { source = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string CssClass
        {
            get { return cssClass ?? "chart"; }
            set { cssClass = value; }
        }

        public int LongestBarPixels
        {
            get { return longestBarPixels ?? 600; }
            set { longestBarPixels = value; }
        }

        public string LabelFormat
        {
            get { return labelFormat; }
            set { labelFormat = value; }
        }

        public string EmptyMessage
        {
            get { return emptyMessage ?? "No data to display"; }
            set { emptyMessage = value; }
        }

        public ChartOrientation Orientation
        {
            get { return orientation ?? ChartOrientation.Vertical; }
            set { orientation = value; }
        }

        public int BarWidthPixels
        {
            get { return barWidthPixels ?? 30; }
            set { barWidthPixels = value; }
        }
    }
}