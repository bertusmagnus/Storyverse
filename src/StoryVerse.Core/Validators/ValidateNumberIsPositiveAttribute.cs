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
    public class ValidateNumberIsPositiveAttribute : AbstractValidationAttribute        
    {
        private readonly string errorMessage;
        
        public ValidateNumberIsPositiveAttribute()
            : this(null) { }

        public ValidateNumberIsPositiveAttribute(string errorMessage)
            : base(errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        public override IValidator Build()
        {
            return new NumberIsPositiveValidator(errorMessage);
        }
    }
}
