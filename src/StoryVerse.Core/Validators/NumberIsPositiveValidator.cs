/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord.Framework.Validators;

namespace StoryVerse.Core.Validators
{
    public class NumberIsPositiveValidator : AbstractValidator
    {
        public NumberIsPositiveValidator()
            : base() { }

        public NumberIsPositiveValidator(string errorMessage)
        {
            if (errorMessage == null)
            {
                BuildErrorMessage();
            }
            else
            {
                ErrorMessage = errorMessage;
            }
        }

        public override bool Perform(object instance, object parms)
        {
            if (parms == null) return true;

            double? d = null;
            try
            {
                d = Convert.ToDouble(parms);
                return d >= 0;
            }
            catch (Exception ex)
            {
                string message = String.Format(
                    "Error: try to convert to double the value {0} of type {1}",
                    d, d.GetType().Name);
                throw new InvalidOperationException(message, ex);
            }
        }

        protected override string BuildErrorMessage()
        {
            return string.Format("The value cannot be negative");
        }
    }
}
