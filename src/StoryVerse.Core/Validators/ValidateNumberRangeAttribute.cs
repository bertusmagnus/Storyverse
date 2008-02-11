/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.Components.Validator;
using StoryVerse.Core.Validators;

namespace StoryVerse.Core.Validators
{
    [Serializable]
    public class ValidateNumberRangeAttribute : AbstractValidationAttribute        
    {
        private readonly double minValue;
        private readonly double maxValue;
        private readonly string errorMessage;

        public ValidateNumberRangeAttribute()
            : this(0, 0, null) { }

        public ValidateNumberRangeAttribute(double minValue, double maxValue, string errorMessage) 
            : base(errorMessage)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.errorMessage = errorMessage;
        }

        public override IValidator Build()
        {
            return new NumberRangeValidator(maxValue, minValue, errorMessage);
        }
    }
}
