using System;
using System.Collections.Generic;
using System.Text;
using StoryVerse.Core.Models;

namespace StoryVerse.AcceptanceTests
{
    public class FitnesseExperiment_TestTestFixture : fit.ColumnFixture
    {
        public double numerator = 0.0;
        public double denominator = 0.0;

        public double GetQuotient()
        {
            return numerator / denominator;
        }

        public int AddCompany()
        {
            Company c = new Company();
            c.Name = "Test Company";
            try
            {
                c.SaveAndFlush();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
    }
}
