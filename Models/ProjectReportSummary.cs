﻿using System;

namespace PTR.Models
{
    public class ProjectReportSummary : ViewModelBase
    {
        public int ID { get; set; }

        public string ProjectName { get; set; }

        public string Description { get; set; }

        public string Customer { get; set; }

        public int CustomerID { get; set; }

        public string Associate { get; set; }

        public decimal EstimatedAnnualSales { get; set; }

        public decimal EstimatedAnnualMPC { get; set; }

        public int TargetedVolume { get; set; }

        public string ProjectStatus { get; set; }

        public string SalesDivision { get; set; }

        public string MarketSegment { get; set; }

        public string Products { get; set; }

        public DateTime? ExpectedDateNewSales { get; set; }

        public string Country { get; set; }

        public int CountryID { get; set; }

        public string CultureCode { get; set; }

        public bool SalesForecastConfirmed { get; set; }

        public string TrialStatus { get; set; }

        public DateTime? ActivatedDate { get; set; }

        public int UserID { get; set; }

        public int ProjectTypeID { get; set; }

        public string ProjectType { get; set; }

        public string Colour { get; set; }

        public bool UseUSD { get; set; }

        public bool KPM { get; set; }

        public decimal ExRate { get; set; }
        
        public bool ValidEP { get; set; }

        public DateTime? CreatedDate { get; set; }       

        public string Title { get; set; }

        public string Email { get; set; }

        public bool EPRequired { get; set; }

        public bool MilestoneDue { get; set; }

        public bool OverdueActivity { get; set; }

        public bool IncompleteEP { get; set; }

        public bool RequiringCompletion { get; set; }
    }
}
