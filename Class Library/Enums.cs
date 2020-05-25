using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace PTR
{
    public enum ProjectStatusType
    {
        [Description("Active")]
        Active = 1,
        [Description("Completed")]
        Completed = 2,
        [Description("Cancelled")]
        Cancelled = 3 
    }
                
    public enum UserPermissionsType
    {
        [Description("Read Only")]
        ReadOnly = 1,
        [Description("Full Access")]
        FullAccess = 2,
        [Description("Edit Activity Only")]
        EditAct = 3
    }

    public enum MaintenanceType
    {
        [Description("MissingEP")]
        MissingEP = 1,
        [Description("IncompleteEP")]
        IncompleteEP = 2,
        [Description("MilestoneDue")]
        MilestoneDue = 3,
        [Description("RequiringCompletion")]
        RequiringCompletion = 4,
        [Description("OverdueActivity")]
        OverdueActivity = 5       
    }

    public enum AdministrationMenuOptions
    {
        [Description("Customers")]
        CustomerMnu = 1,
        [Description("Countries")]
        CountryMnu = 2,
        [Description("Sales Regions")]
        SalesRegionMnu = 3,
        [Description("Users")]
        UserMnu = 4,
        [Description("Exchange Rates")]
        ExchangeRateMnu = 5,
        [Description("Sales Stages")]
        ActivityStatusesMnu = 6,
        [Description("Setup")]
        SetupMnu = 7,
        [Description("Product Names")]
        ProductNameMnu = 8,
        [Description("Project Types")]
        ProjectTypeMnu = 9,
        [Description("Application Categories")]
        ApplicationsMnu = 10,
        [Description("Strategic Move Codes")]
        SMCodesMnu = 11,
        [Description("Trial Statuses")]
        TrialStatusesMnu = 12,
        [Description("New Business Categories")]
        NewBusinessCategoriesMnu = 13,
        [Description("Industry Segments - Applications")]
        IndustrySegmentsApplicationsMnu = 14,
        [Description("Industry Segments")]
        IndustrySegmentsMnu = 15,
        [Description("Incomplete Project Reasons")]
        IncompleteProjectReasonsMnu = 16,
        [Description("Report Fields")]
        ReportFieldsMnu = 17,
        [Description("Miscellaneous Data")]
        MiscellaneousDataMnu = 18,
        [Description("Business Units")]
        BUMnu = 19

    }

    public enum ProjectsMenuOptions
    {
        [Description("Maintenance")]
        ProjectMaintMnu= 1,
        [Description("Master List")]
        ProjectMasterListMnu = 2
    }
    
    public enum ReportsMenuOptions
    {
        [Description("Sales Pipeline")]
        SalesPipelineMnu = 1,
        [Description("Status")]
        StatusMnu = 2,
        [Description("Project List")]
        ProjectListMnu = 3,
        [Description("Evaluation Plans")]
        EvaluationPlans = 4,
        [Description("Custom Reports")]
        CustomReportsMnu = 5
    }

    public enum ReportFieldType
    {
        [Description("Playbook")]
        Playbook = 0,
        [Description("Playbook Comments")]
        PlaybookComments = 99,
        [Description("General")]
        General = 1,
        [Description("System and Hidden")]
        SystemAndHidden = 2,
        [Description("System and Removed")]
        SystemAndRemoved = -1
    }


    public class EnumValue
    {
        public EnumValue() { }
        public Enum Enumvalue { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
    }

    public class EnumerationManager
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static Collection<EnumValue> GetEnumList(Type enumvar)
        {
            Collection<EnumValue> p = new Collection<EnumValue>();
            EnumValue enumvalue;
            foreach (Enum name in Enum.GetValues(enumvar))
            {
                enumvalue = new EnumValue
                {
                    Description = GetEnumDescription(name),
                    Enumvalue = name,
                    ID = Convert.ToInt32(name)
                };
                p.Add(enumvalue);
            }
            return p;
        }
    }

    public static class EnumerationLists
    {
        #region Enumeration Lists

        public static Collection<EnumValue> ProjectStatusTypesList         
        {
            get { return EnumerationManager.GetEnumList(typeof(ProjectStatusType)); } 
        }

        public static Collection<EnumValue> UserPermissionTypeList
        {
            get { return EnumerationManager.GetEnumList(typeof(UserPermissionsType)); }
        }

        public static Collection<EnumValue> AdministrationMenuOptionsList
        {
            get { return EnumerationManager.GetEnumList(typeof(AdministrationMenuOptions)); }
        }
        public static Collection<EnumValue> ProjectsOptionsList
        {
            get { return EnumerationManager.GetEnumList(typeof(ProjectsMenuOptions)); }
        }
        public static Collection<EnumValue> ReportsOptionsList
        {
            get { return EnumerationManager.GetEnumList(typeof(ReportsMenuOptions)); }
        }

        #endregion
    }


}
