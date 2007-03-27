/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Text;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Validators;

using StoryVerse.Core.Validators;

namespace StoryVerse.Attributes
{
    [Serializable]
    public class ValidateNumberRangeAttribute : AbstractValidationAttribute        
    {

        public ValidateNumberRangeAttribute()
            : this(0, 0, null) { }

        public ValidateNumberRangeAttribute(double minValue, double maxValue, string errorMessage) 
            : base(new NumberRangeValidator(maxValue, minValue, errorMessage)) { }
    }
}
