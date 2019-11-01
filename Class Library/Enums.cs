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

    public enum TrialStatusType
    {
        [Description("No trial")]
        NoTrial = 1,
        [Description("Paused")]
        Paused = 2,
        [Description("Running")]
        Running = 3,
        [Description("Failed")]
        Failed = 4,        
        [Description("Successful")]
        Successful = 5
    }

    public enum SalesStatusType
    {
        [Description("On Track")]
        OnTrack = 1,
        [Description("Trouble")]
        Trouble = 2,
        [Description("Late/Delayed")]
        LateDelayed = 3,
        [Description("Cancelled")]
        Cancelled = 4,
        [Description("Failed Trial")]
        FailedTrial = 5,
        [Description("Complete")]
        Complete = 6
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
        [Description("Missing EP")]
        MissingEP = 1,
        [Description("Incomplete EP")]
        IncompleteEP = 2,
        [Description("Milestone Due")]
        MilestoneDue = 3,
        [Description("Requiring Completion")]
        RequiringCompletion = 4,
        [Description("Overdue Activity")]
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
        ExchangeRateMnu = 5
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
        EvaluationPlans = 4
    }



    public class EnumValue
    {
        public EnumValue() { }

        public Enum Enumvalue
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int ID
        {
            get; set;
        }

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

        public static Collection<EnumValue> TrialStatusTypesList
        {
            get { return EnumerationManager.GetEnumList(typeof(TrialStatusType)); }
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
