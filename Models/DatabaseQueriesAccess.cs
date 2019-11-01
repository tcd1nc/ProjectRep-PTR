using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static PTR.StaticCollections;
using static PTR.Constants;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using PTR.Models;

namespace PTR
{
    static class DatabaseQueries
    {
        const int datefieldlen = 15;
        const int multipleidslen = 255;
        const int projectnamelength = 25;
        const int namefieldlen = 50;
        const int descrfieldlen = 255;
        const int maxdescrlen = 2000;
        const int culturecodelen = 10;
        const int ginlen = 10;

        #region Get Queries

        public static SetupModel GetSetup()
        {
            SetupModel setup = new SetupModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSetup";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            setup = new SetupModel()
                            {
                                Domain = or["Domain"].ToString() ?? string.Empty,
                                Emailformat = or["Emailformat"].ToString() ?? string.Empty,
                                IncompleteEPsEMMessage = or["IncompleteEPsEMMessage"].ToString() ?? string.Empty,
                                IncompleteEPsEMTitle = or["IncompleteEPsEMTitle"].ToString() ?? string.Empty,
                                MissingEPsEMMessage = or["MissingEPsEMMessage"].ToString() ?? string.Empty,
                                MissingEPsEMTitle = or["MissingEPsEMTitle"].ToString() ?? string.Empty,
                                OverdueEMMessage = or["OverdueEMMessage"].ToString() ?? string.Empty,
                                OverdueEMTitle = or["OverdueEMTitle"].ToString() ?? string.Empty,
                                RequiringCompletionEMMessage = or["RequiringCompletionEMMessage"].ToString() ?? string.Empty,
                                RequiringCompletionEMTitle = or["RequiringCompletionEMTitle"].ToString() ?? string.Empty,
                                MilestoneDueEMMessage = or["MilestoneDueEMMessage"].ToString() ?? string.Empty,
                                MilestoneDueEMTitle = or["MilestoneDueEMTitle"].ToString() ?? string.Empty,
                                IsBodyHtml = ConvertObjToBool(or["IsBodyHtml"]),
                                EnableSSL = ConvertObjToBool(or["EnableSSL"]),
                                SMTP = or["SMTP"].ToString() ?? string.Empty,
                                Port = ConvertObjToInt(or["Port"]),
                                UseDefaultCredentials = ConvertObjToBool(or["UseDefaultCredentials"]),
                                UseExtEMCredentials = ConvertObjToBool(or["UseExtEMCredentials"]),
                                EMPWD = or["EMPWD"].ToString() ?? string.Empty,
                                EMUser = or["EMUser"].ToString() ?? string.Empty,
                                TargetName = or["TargetName"].ToString() ?? string.Empty,
                                UseEmail = ConvertObjToBool(or["UseEmail"]),
                                Productformat = or["Productformat"].ToString() ?? string.Empty
                            };
                        }
                    }                    
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return setup;
        }
               
        public static FullyObservableCollection<ProjectReportSummary> GetProjectList(string countries, string projecttypes, string associates, string projectstatuses, int salesdivisionid)
        {
            FullyObservableCollection<ProjectReportSummary> projects = new FullyObservableCollection<ProjectReportSummary>();
            try
            {               
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectList";
                    oc.Parameters.Add("@countries", SqlDbType.NVarChar, multipleidslen).Value = countries ?? string.Empty;                    
                    oc.Parameters.Add("@projecttypes", SqlDbType.NVarChar, multipleidslen).Value = projecttypes ?? string.Empty;
                    oc.Parameters.Add("@projectstatuses", SqlDbType.NVarChar, multipleidslen).Value = projectstatuses ?? string.Empty;
                    oc.Parameters.Add("@users", SqlDbType.NVarChar, multipleidslen).Value = associates ?? string.Empty;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = salesdivisionid;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            projects.Add(new ProjectReportSummary()
                            {
                                ID = ConvertObjToInt(or["ProjectID"]),
                                ProjectName = or["ProjectName"].ToString() ?? string.Empty,
                                EstimatedAnnualSales = ConvertObjToDecimal(or["EstimatedAnnualSales"]),
                                SalesForecastConfirmed = ConvertObjToBool(or["SalesForecastConfirmed"]),
                                Associate = or["UserName"].ToString() ?? string.Empty,
                                SalesDivision = or["SalesDivision"].ToString() ?? string.Empty,
                                Customer = or["CustomerName"].ToString() ?? string.Empty,
                                CultureCode = (ConvertObjToBool(or["UseUSD"])) ? "en-US" : or["CultureCode"].ToString() ?? string.Empty,
                                ProjectStatus = GetProjectStatusFromID(or["ProjectStatusID"]),
                                ExpectedDateNewSales = ConvertObjToDate(or["ExpectedDateFirstSales"]),
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                ProjectType = or["ProjectType"].ToString() ?? string.Empty,
                                KPM = ConvertObjToBool(or["KPM"]),
                                Colour = or["Colour"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return projects;
        }
                
        public static ProjectModel GetSingleProject(int projectid)
        {
            ProjectModel newproject = new ProjectModel();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSingleProject";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            newproject = new ProjectModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = projectid,
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Description = or["Description"].ToString() ?? string.Empty
                                },
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                MarketSegmentID = ConvertObjToInt(or["MarketSegmentID"]),
                                SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]),
                                OwnerID = ConvertObjToInt(or["OwnerID"]),
                                EstimatedAnnualSales = ConvertObjToDecimal(or["EstimatedAnnualSales"]),
                                SalesForecastConfirmed = ConvertObjToBool(or["SalesForecastConfirmed"]),
                                Products = or["Products"].ToString() ?? string.Empty,
                                Resources = or["Resources"].ToString() ?? string.Empty,
                                ProjectStatusID = ConvertObjToInt(or["ProjectStatusID"]),
                                ExpectedDateFirstSales = ConvertObjToDate(or["ExpectedDateFirstSales"]),
                                CompletedDate = ConvertObjToDate(or["CompletedDate"]),
                                TargetedVolume = ConvertObjToInt(or["TargetedVolume"]),
                                ApplicationID = ConvertObjToInt(or["ApplicationID"]),
                                EstimatedAnnualMPC = ConvertObjToDecimal(or["EstimatedAnnualMPC"]),
                                NewBusinessCategoryID = ConvertObjToInt(or["NewBusinessCategoryID"]),
                                ProbabilityOfSuccess = ConvertObjToDecimal(or["ProbabilityOfSuccess"]) * 100,
                                GM = ConvertObjToDecimal(or["GM"]),
                                SMCodeID = ConvertObjToInt(or["SMCodeID"]),
                                ProjectTypeID = ConvertObjToInt(or["ProjectTypeID"]),
                                KPM = ConvertObjToBool(or["KPM"]),
                                EPRequired = ConvertObjToBool(or["EPRequired"]),
                                CDPCCPID = ConvertObjToInt(or["CDPCCPID"]),
                                DifferentiatedTechnology = ConvertObjToBool(or["DifferentiatedTechnology"])

                            };
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return newproject;
        }

        public static ProjectReportSummary GetSimpleProjectDetails(int projectid)
        {
            ProjectReportSummary project = new ProjectReportSummary();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSimpleProjectDetails";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            project = new ProjectReportSummary()
                            {
                                ID = projectid,
                                ProjectName = or["ProjectName"].ToString() ?? string.Empty,
                                EstimatedAnnualSales = ConvertObjToDecimal(or["EstimatedAnnualSales"]),
                                SalesForecastConfirmed = ConvertObjToBool(or["SalesForecastConfirmed"]),
                                CultureCode = or["CultureCode"].ToString() ?? string.Empty,
                                ExpectedDateNewSales = ConvertObjToDate(or["ExpectedDateFirstSales"]),
                                CustomerID = ConvertObjToInt(or["CustomerID"])
                            };
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return project;
        }

        public static EPModel GetEvaluationPlan(int id)
        {
            EPModel ep = new EPModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetEvaluationPlan";
                    oc.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            ep = new EPModel()
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = id,
                                    Description = or["Title"].ToString() ?? string.Empty
                                },
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                Strategy = or["Strategy"].ToString() ?? string.Empty,
                                Objectives = or["Objectives"].ToString() ?? string.Empty,
                                Created = ConvertObjToDate(or["Created"]),
                                Discussed = ConvertObjToDate(or["Discussed"]),
                                CustomerID = ConvertObjToInt(or["CustomerID"])
                            };
                        }                        
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return ep;
        }

        public static FullyObservableCollection<EPModel> GetProjectEvaluationPlans(int projectid)
        {
            FullyObservableCollection<EPModel> eps = new FullyObservableCollection<EPModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectEvaluationPlans";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            eps.Add(new EPModel()
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Description = or["Title"].ToString() ?? string.Empty
                                },
                                Strategy = or["Strategy"].ToString() ?? string.Empty,
                                Objectives = or["Objectives"].ToString() ?? string.Empty,
                                Created = ConvertObjToDate(or["Created"]),
                                Discussed = ConvertObjToDate(or["Discussed"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return eps;
        }

        public static FullyObservableCollection<MonthlyActivityStatusModel> GetMonthlyProjectStatuses(int projectid)
        {            
            FullyObservableCollection<MonthlyActivityStatusModel> projectstatuses = new FullyObservableCollection<MonthlyActivityStatusModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectActivities";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            projectstatuses.Add(new MonthlyActivityStatusModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Comments = or["Comments"].ToString() ?? string.Empty,
                                ProjectID = projectid,
                                StatusID = ConvertObjToInt(or["StatusID"]),
                                StatusMonth = ConvertObjToDate(or["StatusMonth"]),
                                TrialStatusID = ConvertObjToInt(or["TrialStatusID"]),
                                ExpectedDateFirstSales = ConvertObjToDate(or["ExpectedDateFirstSales"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }            
            return projectstatuses;
        }

        public static ProjectMonthRange GetFirstLastMonths()
        {
            ProjectMonthRange projectrange = new ProjectMonthRange();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetFirstLastMonths";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            projectrange.StartDate = ConvertObjToDate(or["FirstMonth"]);
                            projectrange.LastDate = ConvertObjToDate(or["LastMonth"]);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return projectrange;
        }

        public static decimal GetExchangeRate(int countryid, DateTime mth)
        {
            decimal exchangerate = 1;
            Collection<ExchangeRateModel> exchangeratescoll = new Collection<ExchangeRateModel>();
            try
            { 
                using(SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetExchangeRates";
                    oc.Parameters.AddWithValue("@countryid", countryid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            exchangeratescoll.Add(new ExchangeRateModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                ExRateMonth = ConvertObjToDate(or["ExRateMonth"]),
                                ExRate = ConvertObjToDecimal(or["ExRate"])
                            });
                        }
                    }
                }
                
                foreach (ExchangeRateModel er in exchangeratescoll)
                {
                    exchangerate = Convert.ToDecimal(er.ExRate);
                    if (ConvertDateToMonthInt(er.ExRateMonth) == ConvertDateToMonthInt(mth))
                        break;
                }

                if (exchangerate <= 0)
                    exchangerate = 1;
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return 1/exchangerate;
        }

        public static FullyObservableCollection<ExchangeRateModel> GetExchangeRates(int countryid)
        {
            FullyObservableCollection<ExchangeRateModel> exchangeratescoll = new FullyObservableCollection<ExchangeRateModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetExchangeRates";
                    oc.Parameters.AddWithValue("@countryid", countryid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            exchangeratescoll.Add(new ExchangeRateModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                ExRateMonth = ConvertObjToDate(or["ExRateMonth"]),
                                ExRate = ConvertObjToDecimal(or["ExRate"]),
                                CountryID = countryid
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return exchangeratescoll;
        }

        private static ProjectMonthRange GetFirstLastMonthsForProject(int projectid)
        {
            ProjectMonthRange projectrange = new ProjectMonthRange();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetFirstLastMonthsForProject";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            projectrange.StartDate = ConvertObjToDate(or["FirstMonth"]);
                            projectrange.LastDate = ConvertObjToDate(or["LastMonth"]);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return projectrange;
        }

        public static DateTime? GetProjectActivatedDate(int projectid)
        {
            DateTime? activated = null;
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectActivatedDate";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            activated = ConvertObjToDate(or["ActivatedDate"]);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return activated;
        }        

        public static FullyObservableCollection<CustomerModel> GetCustomers()
        {
            FullyObservableCollection<CustomerModel> customers = new FullyObservableCollection<CustomerModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomers";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            customers.Add(new CustomerModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                Number = or["Number"].ToString() ?? string.Empty,
                                Location = or["Location"].ToString() ?? string.Empty,
                                CountryID = ConvertObjToInt(or["CountryID"]),
                                CultureCode = or["CultureCode"].ToString() ?? string.Empty,
                                SalesRegionID = ConvertObjToInt(or["SalesRegionID"]),
                                SalesRegionName = or["SalesRegionName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return customers;
        }
               
        public static FullyObservableCollection<UserModel> GetUsers()
        {
            FullyObservableCollection<UserModel> users = new FullyObservableCollection<UserModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetUsers";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            users.Add(new UserModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                Administrator = ConvertObjToBool(or["Administrator"]),                               
                                AdministrationMnu = or["AdministrationMnu"].ToString() ?? string.Empty,
                                ProjectsMnu = or["ProjectsMnu"].ToString() ?? string.Empty,
                                ReportsMnu = or["ReportsMnu"].ToString() ?? string.Empty,
                                ShowOthers = ConvertObjToBool(or["ShowOthers"]),
                                LoginName = or["LoginName"].ToString() ?? string.Empty,
                                GIN = or["GIN"].ToString() ?? string.Empty,
                                SalesDivisions = or["SalesDivisions"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,
                                ShowNagScreen = ConvertObjToBool(or["ShowNag"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return users;
        }               

        public static UserModel GetUserFromLogin(string loginname)
        {
            UserModel user = new UserModel
            {
                GOM = new GenericObjModel() { }
            };

            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetUserFromLogin";
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, 50).Value = loginname;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            user.GOM = new GenericObjModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Deleted = ConvertObjToBool(or["Deleted"])
                            };
                            user.GIN = or["GIN"].ToString() ?? string.Empty;
                            user.SalesDivisions = or["SalesDivisions"].ToString() ?? string.Empty;
                            user.Administrator = ConvertObjToBool(or["Administrator"]);                          
                            user.AdministrationMnu = or["AdministrationMnu"].ToString() ?? string.Empty;
                            user.ProjectsMnu = or["ProjectsMnu"].ToString() ?? string.Empty;
                            user.ReportsMnu = or["ReportsMnu"].ToString() ?? string.Empty;
                            user.ShowOthers = ConvertObjToBool(or["ShowOthers"]);
                            user.Email = or["Email"].ToString() ?? string.Empty;
                            user.LastAccessed = ConvertObjToDate(or["AccessDate"]);
                            user.ShowNagScreen = ConvertObjToBool(or["ShowNag"]);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
               HandleSQLError(sqle);               
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return user;
        }
               
        public static FullyObservableCollection<MarketSegmentModel> GetMarketSegments()
        {
            FullyObservableCollection<MarketSegmentModel> marketsegments = new FullyObservableCollection<MarketSegmentModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMarketSegments";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            marketsegments.Add(new MarketSegmentModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                IndustryID = ConvertObjToInt(or["IndustryID"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return marketsegments;
        }

        public static Collection<ActivityStatusCodesModel> GetActivityStatusCodes()
        {
            Collection<ActivityStatusCodesModel> activitystatuscodes = new Collection<ActivityStatusCodesModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetActivityStatusCodes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            activitystatuscodes.Add(new ActivityStatusCodesModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Status"].ToString() ?? string.Empty,
                                    Description = or["Description"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                Colour = or["Colour"].ToString() ?? string.Empty,
                                PlaybookDescription = or["PlaybookDescription"].ToString() ?? string.Empty,
                                ReqTrialStatus = ConvertObjToBool(or["ReqTrialStatus"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return activitystatuscodes;
        }
        
        public static FullyObservableCollection<ApplicationCategoriesModel> GetApplicationCategories()
        {
            FullyObservableCollection<ApplicationCategoriesModel> applicationcategories = new FullyObservableCollection<ApplicationCategoriesModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetApplicationCategories";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            applicationcategories.Add(new ApplicationCategoriesModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                IndustryID = ConvertObjToInt(or["IndustryID"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return applicationcategories;
        }

        public static Collection<GenericObjModel> GetNewBusinessCategories()
        {
            Collection<GenericObjModel> newbusinesscats = new Collection<GenericObjModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetNewBusinessCategories";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            newbusinesscats.Add(new GenericObjModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Deleted = ConvertObjToBool(or["Deleted"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return newbusinesscats;
        }

        public static FullyObservableCollection<CountryModel> GetCountries()
        {
            FullyObservableCollection<CountryModel> countries = new FullyObservableCollection<CountryModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCountries";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            countries.Add(new CountryModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                OperatingCompanyID = ConvertObjToInt(or["OperatingCompanyID"]),
                                CultureCode = or["CultureCode"].ToString() ?? string.Empty,
                                UseUSD = ConvertObjToBool(or["UseUSD"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return countries;
        }
               
        public static FullyObservableCollection<GenericObjModel> GetSalesDivisions()
        {
            FullyObservableCollection<GenericObjModel> salesdivisions = new FullyObservableCollection<GenericObjModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSalesDivisions";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            salesdivisions.Add(
                                new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                }
                            );
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return salesdivisions;
        }

        public static MilestoneModel GetMilestone(int id)
        {
            MilestoneModel milestone = new MilestoneModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMilestone";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            milestone = new MilestoneModel()
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Description = or["Description"].ToString() ?? string.Empty
                                },
                                UserID = ConvertObjToInt(or["UserID"]),
                                TargetDate = ConvertObjToDate(or["TargetDate"]),
                                CompletedDate = ConvertObjToDate(or["CompletedDate"]),
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                UserName = or["UserName"].ToString() ?? string.Empty
                               
                            };
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return milestone;
        }                             

        public static FullyObservableCollection<MilestoneModel> GetProjectMilestones(int projectid)
        {
            FullyObservableCollection<MilestoneModel> milestones = new FullyObservableCollection<MilestoneModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectMilestones";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            milestones.Add(new MilestoneModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Description = or["Description"].ToString() ?? string.Empty
                                },
                                UserID = ConvertObjToInt(or["UserID"]),
                                TargetDate = ConvertObjToDate(or["TargetDate"]),
                                CompletedDate = ConvertObjToDate(or["CompletedDate"]),
                                UserName = or["UserName"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return milestones;
        }

        public static FullyObservableCollection<MaintenanceModel> GetOverdueMilestones()
        {
            FullyObservableCollection<MaintenanceModel> milestones = new FullyObservableCollection<MaintenanceModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetOverdueMilestones";
                    oc.Parameters.Add("@currentdate", SqlDbType.Date).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            milestones.Add(new MaintenanceModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                UserName = or["UserName"].ToString() ?? string.Empty,
                                ProjectName = or["Description"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                ActionDate = ConvertObjToDate(or["TargetDate"]),
                                MaintenanceTypeID = MaintenanceType.MilestoneDue
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return milestones;
        }

        public static int GetCountCustomerProjects(int customerid)
        {
            int i = 0;
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCountCustomerProjects";
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    try
                    {
                        bool isnumber = int.TryParse(oc.ExecuteScalar().ToString(), out i);
                    }
                    catch
                    {
                        i = 0;
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return i;
        }

        public static int GetCountSalesRegionCustomers(int salesregionid)
        {
            int i = 0;
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCountSalesRegionCustomers";
                    oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = salesregionid;
                    try
                    {
                        bool isnumber = int.TryParse(oc.ExecuteScalar().ToString(), out i);
                    }
                    catch
                    {
                        i = 0;
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return i;
        }

        public static FullyObservableCollection<GenericObjModel> GetOperatingCompanies()
        {
            FullyObservableCollection<GenericObjModel> opcos = new FullyObservableCollection<GenericObjModel>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetOperatingCompanies";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {

                        while (or.Read())
                        {
                            opcos.Add(new GenericObjModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Deleted = ConvertObjToBool(or["Deleted"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return opcos;
        }

        public static FullyObservableCollection<SalesRegionModel> GetSalesRegions()
        {
            FullyObservableCollection<SalesRegionModel> regions = new FullyObservableCollection<SalesRegionModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSalesRegions";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            regions.Add(new SalesRegionModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                CountryID = ConvertObjToInt(or["CountryID"]),
                                Deleted = ConvertObjToBool(or["Deleted"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return regions;
        }


        //=================================================

        public static FullyObservableCollection<SalesRegionModel> GetCountrySalesRegions(int countryid)
        {
            FullyObservableCollection<SalesRegionModel> salesregions = new FullyObservableCollection<SalesRegionModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCountrySalesRegions";
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            salesregions.Add(new SalesRegionModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                CountryID = countryid
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return salesregions;
        }

       
        #region Customers Treeview
        //================================================
        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVOperatingCompanies()
        {
            return GetAllTVNodes("GetAllTVOperatingCompanies",0);
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVCountries()
        {
            return GetAllTVNodes("GetAllTVCountries",0);
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVSalesRegions()
        {
            return GetAllTVNodes("GetAllTVSalesRegions",0);
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVCustomers()
        {
            return GetAllTVNodes("GetAllTVCustomers",1);            
        }

        private static FullyObservableCollection<TreeViewNodeModel> GetAllTVNodes(string sp, int nodetypeid)
        {
            FullyObservableCollection<TreeViewNodeModel> nodes = new FullyObservableCollection<TreeViewNodeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = sp;                  

                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            nodes.Add(new TreeViewNodeModel
                            {
                                ParentID = ConvertObjToInt(or["ParentID"]),
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsExpanded = true,
                                IsSelected = false,
                                IsEnabled = true,
                                NodeTypeID = nodetypeid
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return nodes;
        }

        #endregion

        //==================================================

        #region User's customer Treeview

        public static FullyObservableCollection<TreeViewNodeModel> GetTVOperatingCompanies()
        {
            return GetTVNodes("GetTVOperatingCompanies");
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetTVCountries(int opcoid)
        {
            return GetTVNodes("GetTVCountries");
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetTVSalesRegions(int countryid)
        {
            return GetTVNodes("GetTVSalesRegions");
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetTVCustomers(int userid)
        {
            return GetTVCustomerNodes("GetTVCustomers", userid);
        }

        private static FullyObservableCollection<TreeViewNodeModel> GetTVCustomerNodes(string sp, int userid)
        {                     
            FullyObservableCollection<TreeViewNodeModel> nodes = new FullyObservableCollection<TreeViewNodeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = sp;             
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            nodes.Add(new TreeViewNodeModel
                            {
                                ParentID = ConvertObjToInt(or["ParentID"]),
                                IsChecked = (ConvertObjToInt(or["AccessID"])>0),
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsExpanded = true,
                                IsFAChecked = (ConvertObjToInt(or["AccessID"]) == (int)UserPermissionsType.FullAccess),
                                IsROChecked = (ConvertObjToInt(or["AccessID"]) == (int)UserPermissionsType.ReadOnly),

                                IsEditActChecked = (ConvertObjToInt(or["AccessID"]) == (int)UserPermissionsType.EditAct),

                                AccessID = ConvertObjToInt(or["AccessID"]),
                                NodeTypeID = 1
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return nodes;
        }       
        
        private static FullyObservableCollection<TreeViewNodeModel> GetTVNodes(string sp)
        {
            FullyObservableCollection<TreeViewNodeModel> nodes = new FullyObservableCollection<TreeViewNodeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = sp;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            nodes.Add(new TreeViewNodeModel
                            {
                                ParentID = ConvertObjToInt(or["ParentID"]),
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,                                
                                IsExpanded = true,
                                IsEnabled = ConvertObjToBool(or["Deleted"]),
                                NodeTypeID = 0
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return nodes;
        }
 
        #endregion


        public static Collection<string> GetProductGroupNames()
        {
            Collection<string> prodgroupnames = new Collection<string>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProductGroupNames";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            prodgroupnames.Add(or["Name"].ToString() ?? string.Empty);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return prodgroupnames;
        }

       


        public static Collection<SMCodeModel> GetSMCodes()
        {
            Collection<SMCodeModel> smcodes = new Collection<SMCodeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSMCodes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            smcodes.Add(new SMCodeModel
                            {
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Description = or["Description"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
                                SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return smcodes;
        }             

        public static FullyObservableCollection<UserCustomerAccessModel> GetUserCustomerAccess(int userid)
        {
            FullyObservableCollection<UserCustomerAccessModel> usercodes = new FullyObservableCollection<UserCustomerAccessModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetUserCustomerAccess";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            usercodes.Add(new UserCustomerAccessModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                UserID = userid,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                AccessID = ConvertObjToInt(or["AccessID"]),
                                CountryID = ConvertObjToInt(or["CountryID"]),
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return usercodes;
        }                

        public static FullyObservableCollection<GenericObjModel> GetProjectTypes()
        {
            FullyObservableCollection<GenericObjModel> projecttypes = new FullyObservableCollection<GenericObjModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectTypes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            projecttypes.Add(new GenericObjModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Description = or["Colour"].ToString() ?? string.Empty,
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return projecttypes;
        }

        public static FullyObservableCollection<GenericObjModel> GetCDPCCP()
        {
            FullyObservableCollection<GenericObjModel> cdpccp = new FullyObservableCollection<GenericObjModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCDPCCP";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        cdpccp.Add(new GenericObjModel() { ID = -1, Name = "None" });
                        while (or.Read())
                        {
                            cdpccp.Add(new GenericObjModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Description = or["Description"].ToString() ?? string.Empty,
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return cdpccp;
        }

        public static FullyObservableCollection<MaintenanceModel> GetOverdueMonthlyUpdates()
        {
            FullyObservableCollection<MaintenanceModel> overdue = new FullyObservableCollection<MaintenanceModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetOverdueMonthlyUpdates";
                    oc.Parameters.Add("@currentmonth", SqlDbType.Date).Value = ConvertDateToMonth(DateTime.Now);
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            overdue.Add(new MaintenanceModel
                            {
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                UserName = or["UserName"].ToString() ?? string.Empty,
                                ProjectName = or["Name"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                MaintenanceTypeID = MaintenanceType.OverdueActivity
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return overdue;
        }

        public static FullyObservableCollection<MaintenanceModel> GetIncompleteEPs()
        {
            FullyObservableCollection<MaintenanceModel> incomplete = new FullyObservableCollection<MaintenanceModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetIncompleteEPs";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            incomplete.Add(new MaintenanceModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                UserName = or["UserName"].ToString() ?? string.Empty,
                                ProjectName = or["Title"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                MaintenanceTypeID = MaintenanceType.IncompleteEP
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return incomplete;
        }
               
        public static FullyObservableCollection<MaintenanceModel> GetMissingEPs()
        {
            FullyObservableCollection<MaintenanceModel> missingeps = new FullyObservableCollection<MaintenanceModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMissingEPs";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            missingeps.Add(new MaintenanceModel
                            {                             
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                UserName = or["UserName"].ToString() ?? string.Empty,
                                ProjectName = or["Name"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                MaintenanceTypeID = MaintenanceType.MissingEP
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return missingeps;
        }
                
        public static FullyObservableCollection<MaintenanceModel> GetProjectsRequiringCompletion()
        {
            FullyObservableCollection<MaintenanceModel> overdue = new FullyObservableCollection<MaintenanceModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectsRequiringCompletion";
                    oc.Parameters.Add("@currentmonth", SqlDbType.Date).Value = ConvertDateToMonth(DateTime.Now);
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            overdue.Add(new MaintenanceModel
                            {
                                ProjectID = ConvertObjToInt(or["ProjectID"]),
                                UserName = or["UserName"].ToString() ?? string.Empty,
                                ProjectName = or["Name"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                MaintenanceTypeID = MaintenanceType.RequiringCompletion
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return overdue;
        }
                
        public static (bool versionok, bool upgrade ) GetVersion()
        {
            try
            {
                string version = string.Empty;
                string userversion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                bool mustupgrade = false;
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetVersion";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            version = or["VersionNumber"].ToString() ?? string.Empty;
                            mustupgrade = ConvertObjToBool(or["MustUpgrade"]);
                        }
                    }              
                }
                return (versionok: (version == userversion), upgrade: mustupgrade);
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (versionok: false, upgrade: true);
        }

        public static DateTime? GetLastUpdatedMonth()
        {
            DateTime? updated = null;             
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetLastUpdatedMonth";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            updated = Convert.ToDateTime(or["MAXUPDATEDATE"].ToString());
                        }
                    }
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);                    
                }
            }            
            return updated;
        }
        #endregion

        #region Insert Queries
       
        public static void LogAccess(int userid)
        {                        
            using (SqlCommand oc = new SqlCommand())
            {
            try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "LogAccess";                  
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                    oc.Parameters.Add("@accessdate", SqlDbType.DateTime).Value = DateTime.Now;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }        
        }        
        
        public static int AddProject(ProjectModel project)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddProject";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = project.CustomerID;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = project.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = project.GOM.Description ?? string.Empty;
                    oc.Parameters.Add("@salesstatusid", SqlDbType.Int).Value = project.SalesStatusID;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = project.EstimatedAnnualSales;
                    oc.Parameters.Add("@salesforecastconfirmed", SqlDbType.Bit).Value = project.SalesForecastConfirmed;
                    oc.Parameters.Add("@ownerid", SqlDbType.Int).Value = project.OwnerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = project.SalesDivisionID;
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = project.ProjectStatusID;
                    oc.Parameters.Add("@marketsegmentid", SqlDbType.Int).Value = project.MarketSegmentID;
                    oc.Parameters.Add("@products", SqlDbType.NVarChar, multipleidslen).Value = project.Products ?? string.Empty;
                    oc.Parameters.Add("@resources", SqlDbType.NVarChar, multipleidslen).Value = project.Resources ?? string.Empty;
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = project.ExpectedDateFirstSales;
                    oc.Parameters.Add("@activateddate", SqlDbType.Date).Value = project.ActivatedDate;
                    oc.Parameters.Add("@targetedvolume", SqlDbType.Decimal).Value = project.TargetedVolume;
                    oc.Parameters.Add("@applicationid", SqlDbType.Int).Value = project.ApplicationID;
                    oc.Parameters.Add("@estimatedannualmpc", SqlDbType.Decimal).Value = project.EstimatedAnnualMPC;
                    oc.Parameters.Add("@newbusinesscategoryid", SqlDbType.Int).Value = project.NewBusinessCategoryID;
                    oc.Parameters.Add("@probabilityofsuccess", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.ProbabilityOfSuccess / 100);
                    oc.Parameters.Add("@gm", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.GM);
                    oc.Parameters.Add("@smcodeid", SqlDbType.Int).Value = project.SMCodeID;
                    oc.Parameters.Add("@projecttypeid", SqlDbType.Int).Value = project.ProjectTypeID;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = project.KPM;
                    oc.Parameters.Add("@eprequired", SqlDbType.Bit).Value = project.EPRequired;
                    oc.Parameters.Add("@cdpccpid", SqlDbType.Int).Value = project.CDPCCPID;
                    oc.Parameters.Add("@differentiatedtechnology", SqlDbType.Bit).Value = project.DifferentiatedTechnology;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();

                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);                
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }

                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }  
            }
            return insertedid;                    
        }
            
        public static int AddCustomer(CustomerModel customer)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddCustomer";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = customer.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@number", SqlDbType.NVarChar, 50).Value = customer.Number ?? string.Empty;
                    oc.Parameters.Add("@location", SqlDbType.NVarChar, 50).Value = customer.Location;
                    oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = customer.SalesRegionID;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = customer.CountryID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
            return insertedid;
        }

        public static int AddUser(UserModel user)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddUser";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = user.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = user.LoginName ?? string.Empty;
                    oc.Parameters.Add("@email", SqlDbType.NVarChar, multipleidslen).Value = user.Email ?? string.Empty;
                    oc.Parameters.Add("@gin", SqlDbType.NVarChar, ginlen).Value = user.GIN ?? string.Empty;
                    oc.Parameters.Add("@administrator", SqlDbType.Bit).Value = user.Administrator;
                    oc.Parameters.Add("@salesdivisions", SqlDbType.NVarChar, multipleidslen).Value = user.SalesDivisions ?? string.Empty;                    
                    oc.Parameters.Add("@administrationmnu", SqlDbType.NVarChar, multipleidslen).Value = user.AdministrationMnu ?? string.Empty;
                    oc.Parameters.Add("@projectsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ProjectsMnu ?? string.Empty;
                    oc.Parameters.Add("@reportsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ReportsMnu ?? string.Empty;
                    oc.Parameters.Add("@showothers", SqlDbType.Bit).Value = user.ShowOthers;
                    oc.Parameters.Add("@shownag", SqlDbType.Bit).Value = user.ShowNagScreen;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
            return insertedid;
        }

        public static int AddCountry(CountryModel country)
        {
            int insertedid = -1;                       
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {  
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddCountry";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = country.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@operatingcompanyid", SqlDbType.Int).Value = country.OperatingCompanyID;
                    oc.Parameters.Add("@culturecode", SqlDbType.NVarChar, culturecodelen).Value = country.CultureCode ?? string.Empty;
                    oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = country.UseUSD;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }                
            }           
            return insertedid;
        }
               
        public static int AddExchangeRate(ExchangeRateModel em)
        {
            int insertedid = -1;                                        
            using (SqlCommand oc = new SqlCommand())
            {
                try
                { 
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddExchangeRate";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = em.CountryID;
                    oc.Parameters.Add("@exratemonth", SqlDbType.Date).Value = em.ExRateMonth;
                    oc.Parameters.Add("@usexchangerate", SqlDbType.Decimal).Value = em.ExRate;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }                       
            return insertedid;
        }       

        public static void AddUserCustomerAccess(int customerid, int userid, int accessid)
        {           
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddUserCustomerAccess";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;                    
                    oc.Parameters.Add("@accessid", SqlDbType.Int).Value = accessid;                    
                    oc.ExecuteNonQuery();
                    transaction.Commit();                   
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }           
        }


        public static void AddMilestone(MilestoneModel milestone)
        {          
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddMilestone";                 
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, maxdescrlen).Value = milestone.GOM.Description ?? string.Empty;
                   
                    if (milestone.TargetDate == null)
                        oc.Parameters.Add("@targetdate", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@targetdate", SqlDbType.Date).Value = milestone.TargetDate;
                    
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = milestone.UserID;
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = milestone.ProjectID;
                 
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }           
        }

        public static void AddEvaluationPlan(EPModel ep)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddEvaluationPlan";
                    oc.Parameters.Add("@title", SqlDbType.NVarChar, descrfieldlen).Value = ep.GOM.Description ?? string.Empty;
                    oc.Parameters.Add("@objectives", SqlDbType.NVarChar, maxdescrlen).Value = ep.Objectives;
                    oc.Parameters.Add("@strategy", SqlDbType.NVarChar, maxdescrlen).Value = ep.Strategy;
                   
                    if (ep.Created == null)
                        oc.Parameters.Add("@created", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@created", SqlDbType.Date).Value = ep.Created;

                    if (ep.Discussed == null)
                        oc.Parameters.Add("@discussed", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@discussed", SqlDbType.Date).Value = ep.Discussed;

                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = ep.ProjectID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();

                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static int AddSalesRegion(SalesRegionModel em)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddSalesRegion";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = em.CountryID;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, descrfieldlen).Value = em.Name;                   
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        #endregion

        #region Update Queries

        public static void UpdateProject(ProjectModel project)
        {                      
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateProject";
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = project.CustomerID;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = project.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = project.GOM.Description ?? string.Empty;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = project.EstimatedAnnualSales;
                    oc.Parameters.Add("@salesforecastconfirmed", SqlDbType.Bit).Value = project.SalesForecastConfirmed;
                    oc.Parameters.Add("@ownerid", SqlDbType.Int).Value = project.OwnerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = project.SalesDivisionID;
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = project.ProjectStatusID;              
                    oc.Parameters.Add("@marketsegmentid", SqlDbType.Int).Value = project.MarketSegmentID;
                    oc.Parameters.Add("@products", SqlDbType.NVarChar, multipleidslen).Value = project.Products ?? string.Empty;
                    oc.Parameters.Add("@resources", SqlDbType.NVarChar, multipleidslen).Value = project.Resources ?? string.Empty;
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = project.ExpectedDateFirstSales;
                    oc.Parameters.Add("@targetedvolume", SqlDbType.Decimal).Value = project.TargetedVolume;
                    oc.Parameters.Add("@applicationid", SqlDbType.Int).Value = project.ApplicationID;
                    oc.Parameters.Add("@estimatedannualmpc", SqlDbType.Decimal).Value = project.EstimatedAnnualMPC;
                    oc.Parameters.Add("@newbusinesscategoryid", SqlDbType.Int).Value = project.NewBusinessCategoryID;
                    if (project.CompletedDate == null)
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = project.CompletedDate;
                                       
                    oc.Parameters.Add("@probabilityofsuccess", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.ProbabilityOfSuccess/100);
                    oc.Parameters.Add("@gm", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.GM);
                    oc.Parameters.Add("@smcodeid", SqlDbType.Int).Value = project.SMCodeID;
                    oc.Parameters.Add("@projecttypeid", SqlDbType.Int).Value = project.ProjectTypeID;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = project.KPM;
                    oc.Parameters.Add("@eprequired", SqlDbType.Bit).Value = project.EPRequired;
                    oc.Parameters.Add("@cdpccpid", SqlDbType.Int).Value = project.CDPCCPID;
                    oc.Parameters.Add("@differentiatedtechnology", SqlDbType.Bit).Value = project.DifferentiatedTechnology;
                                                                         
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = project.GOM.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }                   
        }

        public static void UpdateMonthlyActivityStatus(FullyObservableCollection<MonthlyActivityStatusModel> activities)
        {
            int result = 0;
            foreach (MonthlyActivityStatusModel mas in activities)
            {
                if (mas.IsDirty)
                {
                    if (RequiredTrialStatuses.Count > 0)
                    {
                        if (!RequiredTrialStatuses.Contains(mas.StatusID))
                            mas.TrialStatusID = 0;
                    }

                    if (mas.StatusMonth != null)
                    {
                        if (mas.ID > 0)
                            UpdateMonthlyActivityStatus2(mas);
                        else
                        {
                            if (mas.StatusID > 0 && mas.StatusMonth != null)
                            {
                                result = AddMonthlyActivityStatus(mas);                                
                            }
                        }
                    }
                }
            }
        }

        public static int AddMonthlyActivityStatus(MonthlyActivityStatusModel activity)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddMonthlyActivityStatus";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = activity.ProjectID;
                    oc.Parameters.Add("@statusmonth", SqlDbType.Date).Value = activity.StatusMonth;
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = activity.StatusID;
                    oc.Parameters.Add("@comments", SqlDbType.NVarChar, descrfieldlen).Value = activity.Comments ?? string.Empty;
                    oc.Parameters.Add("@trialstatusid", SqlDbType.Int).Value = activity.TrialStatusID;
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = activity.ExpectedDateFirstSales ?? DateTime.Now.AddMonths(12);
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
            return insertedid;
        }

        public static void UpdateMonthlyActivityStatus2(MonthlyActivityStatusModel activity)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateMonthlyActivityStatus";
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = activity.StatusID;
                    oc.Parameters.Add("@trialstatusid", SqlDbType.Int).Value = activity.TrialStatusID;
                    oc.Parameters.Add("@comments", SqlDbType.NVarChar, descrfieldlen).Value = activity.Comments ?? string.Empty;
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = activity.ExpectedDateFirstSales ?? DateTime.Now.AddMonths(12);
                    oc.Parameters.Add("@activityid", SqlDbType.Int).Value = activity.ID;                           
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }           
        }
       
        public static void UpdateCustomer(CustomerModel customer)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateCustomer";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = customer.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@number", SqlDbType.NVarChar, namefieldlen).Value = customer.Number ?? string.Empty;
                    oc.Parameters.Add("@location", SqlDbType.NVarChar, multipleidslen).Value = customer.Location ?? string.Empty;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = customer.CountryID;
                    oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = customer.SalesRegionID;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = customer.GOM.Deleted;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customer.GOM.ID;                   
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
        }

        public static void UpdateUser(UserModel user)
        {                        
            using (SqlCommand oc = new SqlCommand())
            {
                try
                { 
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateUser";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = user.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = user.LoginName ?? string.Empty;
                    oc.Parameters.Add("@email", SqlDbType.NVarChar, multipleidslen).Value = user.Email ?? string.Empty;
                    oc.Parameters.Add("@gin", SqlDbType.NVarChar, ginlen).Value = user.GIN ?? string.Empty;
                    oc.Parameters.Add("@administrator", SqlDbType.Bit).Value = user.Administrator;
                    oc.Parameters.Add("@salesdivisions", SqlDbType.NVarChar, multipleidslen).Value = user.SalesDivisions ?? string.Empty;                  
                    oc.Parameters.Add("@administrationmnu", SqlDbType.NVarChar, multipleidslen).Value = user.AdministrationMnu ?? string.Empty;
                    oc.Parameters.Add("@projectsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ProjectsMnu ?? string.Empty;
                    oc.Parameters.Add("@reportsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ReportsMnu ?? string.Empty;
                    oc.Parameters.Add("@showothers", SqlDbType.Bit).Value = user.ShowOthers;
                    oc.Parameters.Add("@shownag", SqlDbType.Bit).Value = user.ShowNagScreen;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = user.GOM.Deleted;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = user.GOM.ID;                    
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }           
        }
                
        public static void UpdateCountry(CountryModel country)
        {                        
            using (SqlCommand oc = new SqlCommand())
            {
                try
                { 
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateCountry";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = country.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@operatingcompanyid", SqlDbType.Int).Value = country.OperatingCompanyID;
                    oc.Parameters.Add("@culturecode", SqlDbType.NVarChar, culturecodelen).Value = country.CultureCode ?? string.Empty;
                    oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = country.UseUSD;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = country.GOM.Deleted;                    
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = country.GOM.ID;                    
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }            
        }

        public static void UpdateSalesRegion(SalesRegionModel salesregion)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateSalesRegion";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = salesregion.Name ?? string.Empty;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = salesregion.CountryID;
                    oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = salesregion.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }


        public static void SetLastUpdatedMonth(DateTime dt)
        {           
            
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "SetLastUpdatedMonth";
                    oc.Parameters.Add("@updatedate", SqlDbType.Date).Value = dt;                   
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }            
        }

        public static void UpdateStatus10ProjectsToCompleted()
        {            
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateStatus10ProjectsToCompleted";
                    DateTime previousmonth =(DateTime)ConvertDateToMonth(DateTime.Now.AddMonths(-1));
                    oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = new DateTime(previousmonth.Year, previousmonth.Month, 1);                  
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }           
        }

        public static void UpdateActualForecastedSales(int projectid, object[] values, bool salesconfirmedflag)
        {            
                          
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {  
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateActualForecastedSales";
                    oc.Parameters.Add("@actualannualsales", SqlDbType.Decimal).Value = ConvertObjToDecimal(values[0]);
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = (DateTime) values[1];                 
                    oc.Parameters.Add("@salesconfirmedflag", SqlDbType.Bit).Value = salesconfirmedflag;

                    if (salesconfirmedflag == true)
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    else
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = DBNull.Value;

                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;                    
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }            
        }

        public static void UpdateForecastedSalesDate(int projectid, DateTime salesdate)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateForecastedSalesDate";
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = salesdate;
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
        }

        public static void UpdateProjectStatus(int projectid, int newstatusid)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {                           
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateProjectStatus";
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = newstatusid;
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }            
        }
                      
        public static void UpdateExchangeRate(ExchangeRateModel em)
        {                        
            using (SqlCommand oc = new SqlCommand())
            {
                try
                { 
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateExchangeRate";
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = em.CountryID;
                    oc.Parameters.Add("@exratemonth", SqlDbType.Date).Value = em.ExRateMonth; 
                    oc.Parameters.Add("@usexchangerate", SqlDbType.Float).Value = em.ExRate;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = em.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }            
        }

        public static void UpdateEvaluationPlan(EPModel ep)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateEP";                    
                    oc.Parameters.Add("@title", SqlDbType.NVarChar, descrfieldlen).Value = ep.GOM.Description ?? string.Empty;
                    oc.Parameters.Add("@objectives", SqlDbType.NVarChar, maxdescrlen).Value = ep.Objectives ?? string.Empty;
                    oc.Parameters.Add("@strategy", SqlDbType.NVarChar, maxdescrlen).Value = ep.Strategy ?? string.Empty;
                    if (ep.Created == null)
                        oc.Parameters.Add("@created", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@created", SqlDbType.Date).Value = ep.Created;
                    if (ep.Discussed == null)
                        oc.Parameters.Add("@discussed", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@discussed", SqlDbType.Date).Value = ep.Discussed;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = ep.GOM.Deleted;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = ep.GOM.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateMilestone(MilestoneModel milestone)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateMilestone";
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, maxdescrlen).Value = milestone.GOM.Description ?? string.Empty;
                   
                    if (milestone.TargetDate == null)
                        oc.Parameters.Add("@targetdate", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@targetdate", SqlDbType.Date).Value = milestone.TargetDate;

                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = milestone.UserID;
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = milestone.ProjectID;
                    
                    if (milestone.CompletedDate == null)
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = milestone.CompletedDate;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = milestone.GOM.Deleted;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = milestone.GOM.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateUserCustomerAccess(int customerid, int userid, int accessid)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateUserCustomerAccess";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    oc.Parameters.Add("@accessid", SqlDbType.Int).Value = accessid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }



        //public static void ResetUserCustomerAccess(int userid)
        //{
        //    using (SqlCommand oc = new SqlCommand())
        //    {
        //        try
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            SqlTransaction transaction;
        //            transaction = Conn.BeginTransaction("LocalTransaction");
        //            oc.Transaction = transaction;
        //            oc.CommandText = "ResetUserCustomerAccess";
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
        //            oc.ExecuteNonQuery();
        //            transaction.Commit();
        //        }
        //        catch (SqlException sqle)
        //        {
        //            HandleSQLError(sqle);
        //        }
        //        catch (Exception e)
        //        {
        //            ShowError(e);
        //            try
        //            {
        //                oc.Transaction.Rollback();
        //            }
        //            catch
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //}


        #endregion

        #region Delete Queries

        public static void DeleteCustomer(int customerid)
        {                        
            using (SqlCommand oc = new SqlCommand())
            {
                try
                { 
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "DeleteCustomer";
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
        }

        public static void DeleteSalesRegion(int salesregionid)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "DeleteSalesRegion";
                    oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = salesregionid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void DeleteUserCustomerAccess()
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "DeleteUserCustomerAccess";     
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Report Queries

        public static DataSet GetFilteredProjectsEstSales(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, 
            bool useUSD, DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm, string cdpccpfilter)
        {
            DataTable dt = new DataTable
            {
                TableName = "TotalData"
            };
            DataSet ds = new DataSet();

            try
            {
                //get all status codes
                //get all months and build dt - add row for each status code, add column for status code
                //get sum of each status for each month
                //add sums to dt where 
                dt.Columns.Add(new DataColumn()
                {
                    Caption = "Status",
                    ColumnName = "Status",
                    DataType = System.Type.GetType("System.Int32")
                });

                //add month columns for data
                AddNumericalReportMonths(ref dt, firstmonth, lastmonth);

                DataTable dtCount = new DataTable();
                dtCount = dt.Clone();
                dtCount.TableName = "ProjectCountData";

                DataRow dr;
                DataRow drCount;

                DateTime newmthdate;
                decimal salesvalue = 0;
                string colname = string.Empty;

                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetFilteredProjectsEstSales";

                    foreach (ActivityStatusCodesModel code in ActivityStatusCodes)
                    {
                        if (code.GOM.ID != 0)//dont show any 0's
                        {
                            dr = dt.NewRow();
                            dr["Status"] = code.GOM.ID;// code.GOM.ID.ToString();
                            drCount = dtCount.NewRow();
                            drCount["Status"] = code.GOM.ID;// code.GOM.ID.ToString();
                            oc.Parameters.Clear();
                            oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
                            oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
                            oc.Parameters.Add("@statusid", SqlDbType.Int).Value = code.GOM.ID;
                            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                            oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                            oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                            oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                            oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                            oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                            oc.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                                
                            using (SqlDataReader or = oc.ExecuteReader())
                            {
                                while (or.Read())
                                {
                                    if (useUSD)
                                        salesvalue = (decimal)(or["TotalStatusValueUS"]);
                                    else
                                        salesvalue = (decimal)(or["TotalStatusValue"]);

                                    newmthdate = Convert.ToDateTime(or["StatusMonth"].ToString());
                                    colname = newmthdate.ToString("MMM") + " " + newmthdate.Year.ToString();

                                    if (salesvalue > 0)
                                        dr[colname] = ConvertObjToInt(dr[colname]) + (int)salesvalue;

                                    if ((int)(or["ProjectCount"]) > 0)
                                        drCount[colname] = ConvertObjToInt(drCount[colname]) + (int)(or["ProjectCount"]);
                                }
                            }
                            dt.Rows.Add(dr);
                            dtCount.Rows.Add(drCount);
                        }
                    }
                }
                ds.Tables.Add(dt);
                ds.Tables.Add(dtCount);
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return ds;
        }
        
        public static DataTable GetSummaryByStatusMonth(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, bool useUSD, 
            DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm, string cdpccpfilter)
        {
            DataTable dt = new DataTable
            {
                TableName = "StatusSummaryData"
            };

            try
            {
                //get all status codes
                //get all months and build dt - add row for each status code, add column for status code
                //get sum of each status for each month
                //add sums to dt where 
                ProjectMonthRange monthrange = GetFirstLastMonths();

                DataColumn dc;
                dc = new DataColumn
                {
                    Caption = "Status",
                    ColumnName = "Status",
                    DataType = Type.GetType("System.String")
                };
                dt.Columns.Add(dc);

                //add month columns for data
                AddReportMonths(ref dt, firstmonth, lastmonth);

                DataRow dr;
                DateTime newmthdate;
                decimal EstimatedAnnualSales = 0;
                string colname = string.Empty;
                string culturecode = string.Empty;
                CultureInfo cultinfo;

                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSummaryByStatusMonth";

                    foreach (ActivityStatusCodesModel code in ActivityStatusCodes)
                    {
                        dr = dt.NewRow();
                        dr["Status"] = code.GOM.ID.ToString();
                        oc.Parameters.Clear();
                        oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
                        oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
                        oc.Parameters.Add("@statusid", SqlDbType.Int).Value = code.GOM.ID;
                        oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                        oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                        oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                        oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                        oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                        oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                        oc.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                        oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                        using (SqlDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                newmthdate = Convert.ToDateTime(or["StatusMonth"].ToString());
                                colname = newmthdate.ToString("MMM") + " " + newmthdate.Year.ToString();
                                culturecode = or["CultureCode"].ToString() ?? "en-US";
                                if (useUSD)
                                {
                                    EstimatedAnnualSales = ConvertObjToDecimal(or["EstimatedAnnualSalesUS"]);
                                    culturecode = "en-US";
                                }
                                else
                                    EstimatedAnnualSales = ConvertObjToDecimal(or["EstimatedAnnualSales"]);

                                cultinfo = new CultureInfo(culturecode);
                                if (dr[colname].ToString().Length > 0)
                                    dr[colname] = dr[colname].ToString() + "\n";

                                dr[colname] = dr[colname].ToString() + (or["ID"]).ToString() + " " + (or["Name"]).ToString() + " : " + EstimatedAnnualSales.ToString("C0", cultinfo);
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return dt;
        }

        public static DataTable GetStatusReportFiltered(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, 
            string ProjectTypesSrchString, bool kpm, bool showallkpm, string cdpccpfilter)
        {
            DataTable newdt = new DataTable
            {
                TableName = "StatusData"
            };
            try
            {                
                DataTable dt = new DataTable
                {
                    TableName = "StatusData"
                };

                AddReportColumns(ref newdt, "StatusReport");

                SqlCommand oc1 = new SqlCommand
                {
                    Connection = Conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "GetStatusReportFilteredProjects2"
                };
                oc1.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                oc1.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                oc1.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                oc1.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                oc1.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                oc1.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                oc1.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                oc1.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                using (SqlDataAdapter da = new SqlDataAdapter(oc1))
                {
                    da.Fill(dt);
                }
                oc1.Dispose();
                
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetStatusReportFilteredActivities";

                    DataRow dr1;
                    DataRow dr;
                    int elementctr = 0;
                    string laststatusid = string.Empty;
                    int intlaststatusid = 0;
                    int statusctr = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dr1 = dt.Rows[i];
                        oc.Parameters.Clear();
                        oc.Parameters.Add("@projectid", SqlDbType.Int).Value = ConvertObjToInt(dr1["ProjectID"]);                 
                        using (SqlDataReader or = oc.ExecuteReader())
                        {
                            if (or.HasRows)
                            {
                                dr = newdt.NewRow();
                                dr["ProjectID"] = ConvertObjToInt(dr1["ProjectID"]);
                                dr["SalesDivision"] = dr1["SalesDivision"].ToString() ?? string.Empty;
                                dr["Customer"] = dr1["Customer"].ToString() ?? string.Empty;
                                dr["ProjectName"] = dr1["ProjectName"].ToString() ?? string.Empty;
                                dr["EstimatedAnnualSales"] = ConvertObjToDecimal(dr1["EstimatedAnnualSales"]);
                                dr["ActivatedDate"] = ConvertObjToDate(dr1["ActivatedDate"]);
                                dr["CultureCode"] = dr1["CultureCode"].ToString() ?? string.Empty;
                                dr["ProjectTypeColour"] = dr1["ProjectTypeColour"].ToString() ?? string.Empty;
                                elementctr = 0;
                                laststatusid = string.Empty;
                                intlaststatusid = 0;
                                statusctr = 0;
                                DateTime laststatusmonth = new DateTime();
                                while (or.Read())
                                {
                                    if (elementctr == 0)
                                    {
                                        intlaststatusid = ConvertObjToInt(or["StatusID"]);
                                        laststatusmonth = Convert.ToDateTime(or["StatusMonth"].ToString());
                                        dr["TrialStatus"] = GetTrialStatusFromID(or["TrialStatusID"]);
                                        dr["Colour"] = or["Colour"].ToString() ?? string.Empty;
                                    }
                                    if (ConvertObjToInt(or["StatusID"]) == intlaststatusid)
                                        statusctr++;
                                    else
                                        break;
                                    elementctr++;
                                }
                                //get number of months
                                if (elementctr > 0)
                                {
                                    dr["Status"] = GetActivityStatusFromID(intlaststatusid);
                                    dr["MonthsAtStatus"] = statusctr;
                                    dr["FirstMonthAtStatus"] = laststatusmonth.AddMonths(-statusctr + 1).ToString("MMM-yyyy");

                                }
                                newdt.Rows.Add(dr);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }            
            return newdt;
        }

        public static DataTable GetProjects(string CountriesSrchString,  string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, 
            bool kpm, bool showallkpm, string cdpccpfilter)
        {
            DataTable dt = new DataTable
            {
                TableName = "ProjectsList"
            };
            try
            {
                AddReportColumns(ref dt, "ProjectsList");
                DataRow dr;
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjects";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                    oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                    oc.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            dr = dt.NewRow();

                            dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
                            dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
                            dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
                            dr["ProjectName"] = or["Name"].ToString() ?? string.Empty;
                            dr["Products"] = or["Products"].ToString() ?? string.Empty;
                            dr["Resources"] = or["Resources"].ToString() ?? string.Empty;
                            dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
                            dr["ProjectStatus"] = GetProjectStatusFromID(or["ProjectStatusID"]);
                            dr["ProjectType"] = or["ProjectType"].ToString() ?? string.Empty;
                            dr["EstimatedAnnualSales"] = ConvertObjToDecimal(or["EstimatedAnnualSales"]);
                            dr["OwnerID"] = ConvertObjToInt(or["OwnerID"]);
                            dr["CultureCode"] = or["CultureCode"].ToString() ?? string.Empty;
                            dr["ProjectTypeColour"] = or["ProjectTypeColour"].ToString() ?? string.Empty;

                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return dt;
        }
        
        public static FullyObservableCollection<MonthlyActivityStatusModel> GetMonthlyProjectData(int projectid)
        {
            ProjectMonthRange monthrange = GetFirstLastMonthsForProject(projectid);
            int startmonth;

            if (monthrange.StartDate == null)
            {
                DateTime? activateddate = GetProjectActivatedDate(projectid);
                if (activateddate != null)
                    startmonth = ConvertDateToMonthInt(activateddate);
                else
                    startmonth = ConvertDateToMonthInt(DateTime.Now.AddMonths(-1));
            }
            else
                startmonth = ConvertDateToMonthInt(((DateTime)monthrange.StartDate));

            //_lastmonth;
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            int lastmonth = ConvertDateToMonthInt(dt);

            //create collection of MonthlyActivityStatus
            FullyObservableCollection<MonthlyActivityStatusModel> monthlyactivity = new FullyObservableCollection<MonthlyActivityStatusModel>();
            FullyObservableCollection<MonthlyActivityStatusModel> selectedprojectactivity = GetMonthlyProjectStatuses(projectid);
            MonthlyActivityStatusModel newmonthactivity;
           
            for (int i = lastmonth; i >= startmonth; i--)
            {
                newmonthactivity = new MonthlyActivityStatusModel
                {
                    ID = 0,
                    ProjectID = projectid,
                    StatusID = 0,
                    TrialStatusID = (int)TrialStatusType.NoTrial,
                    Comments = string.Empty,
                    StatusMonth = ConvertMonthIntToDateTime(i),
                    ExpectedDateFirstSales = null,
                    IsDirty = false
                };
                var query = selectedprojectactivity.FirstOrDefault(a => ConvertDateToMonthInt(a.StatusMonth) == i);
                if (query != null) 
                {
                    newmonthactivity.ID = query.ID;
                    newmonthactivity.StatusID = query.StatusID;
                    newmonthactivity.Comments = query.Comments;
                    newmonthactivity.TrialStatusID = query.TrialStatusID;
                    newmonthactivity.ShowTrial = RequiredTrialStatuses.Contains(query.StatusID);
                    newmonthactivity.ExpectedDateFirstSales = query.ExpectedDateFirstSales;
                }
                monthlyactivity.Add(newmonthactivity);
            }
            return monthlyactivity;
        }
        
        public static DataTable GetEvaluationPlansReport(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, 
            bool kpm, bool showallkpm, string cdpccpfilter)
        {            
            DataTable dt = new DataTable
                {
                    TableName = "EPReport"
                };
            try
            {                
                AddReportColumns(ref dt, "EPReport");
                DataRow dr;

                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetEvaluationPlans";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                    oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                    oc.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            dr = dt.NewRow();
                            dr["ID"] = ConvertObjToInt(or["ID"]);
                            dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
                            dr["ProjectName"] = or["ProjectName"].ToString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(or["ActivatedDate"].ToString()))
                                dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
                            dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
                            dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
                            dr["ProjectTypeColour"] = or["ProjectTypeColour"].ToString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(or["Created"].ToString()))
                                dr["CreatedDate"] = ConvertObjToDate(or["Created"]);
                            if (!string.IsNullOrEmpty(or["Discussed"].ToString()))
                                dr["DiscussedDate"] = ConvertObjToDate(or["Discussed"]);
                            dr["Objectives"] = or["Objectives"].ToString() ?? string.Empty;
                            dr["Strategy"] = or["Strategy"].ToString() ?? string.Empty;
                            dr["Title"] = or["Title"].ToString() ?? string.Empty;
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return dt;
        }

        private static void AddReportColumns(ref DataTable dt, string report)
        {          
            DataColumn dc;
            //Get fields for report from DB and create new columns in datatable
            Collection<ReportFields> fields = GetReportFields(report);
            foreach (ReportFields f in fields)
            {
                dc = new DataColumn();
                dc.ExtendedProperties.Add("Alignment", f.Alignment);
                dc.ExtendedProperties.Add("Format", f.Format);
                dc.ExtendedProperties.Add("FieldType", f.FieldType);
                dc.Caption = f.Caption;
                dc.ColumnName = f.FieldName;
                dc.DataType = System.Type.GetType("System." + f.DataType);
                dt.Columns.Add(dc);
            }            
        }

        private static void AddReportMonths(ref DataTable dt, DateTime firstmonth, DateTime lastmonth)
        {
            DataColumn dc;
            int intstartmonth = ConvertDateToMonthInt(firstmonth);
            int intlastmonth = ConvertDateToMonthInt(lastmonth);
            
            for (int i = intstartmonth; i <= intlastmonth; i++)
            {
                dc = new DataColumn();
                DateTime newmth = ConvertMonthIntToDateTime(i);
                dc.Caption = newmth.ToString("MMM") + " " + newmth.Year.ToString();
                dc.ColumnName = newmth.ToString("MMM") + " " + newmth.Year.ToString();
                dc.DataType = System.Type.GetType("System.String");
                dc.ExtendedProperties["Alignment"] = "Left";
                dc.ExtendedProperties["FieldType"] = 99;
                dt.Columns.Add(dc);
            }
        }

        private static void AddNumericalReportMonths(ref DataTable dt, DateTime firstmonth, DateTime lastmonth)
        {
            DataColumn dc;
            int intstartmonth = ConvertDateToMonthInt(firstmonth);
            int intlastmonth = ConvertDateToMonthInt(lastmonth);

            for (int i = intstartmonth; i <= intlastmonth; i++)
            {
                dc = new DataColumn();
                DateTime newmth = ConvertMonthIntToDateTime(i);
                dc.Caption = newmth.ToString("MMM") + " " + newmth.Year.ToString();
                dc.ColumnName = newmth.ToString("MMM") + " " + newmth.Year.ToString();
                dc.DataType = Type.GetType("System.Decimal");
                dc.ExtendedProperties["Alignment"] = "Right";
                dc.ExtendedProperties["FieldType"] = 99;
                dt.Columns.Add(dc);
            }
        }

        public static DataTable GetNewBusinessReport(string CountriesSrchString, int SelectedDivisionID, string ProjectTypesSrchString, DateTime startmonth, DateTime firstmonth, DateTime lastmonth, 
            bool kpm, bool showallkpm, string cdpccpfilter)
        {
            string report = "NewBusiness";
            DataTable dt = new DataTable
            {
                TableName = NewBusiness
            };
            try
            {
                //add report columns for data
                AddReportColumns(ref dt, report);

                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "PBNewBusiness";
                    oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = SelectedDivisionID;
                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                    oc.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

                    //add month columns for comments
                    AddReportMonths(ref dt, firstmonth, lastmonth);

                    DataRow dr;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            dr = dt.NewRow();
                            dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
                            dr["SalesDivision"] = or["SalesDivision"].ToString() ?? string.Empty;
                            dr["MarketSegment"] = or["MarketSegment"].ToString() ?? string.Empty;
                            dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
                            dr["CustomerNumber"] = or["CustomerNumber"].ToString() ?? string.Empty;
                            dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
                            dr["Application"] = or["Application"].ToString() ?? string.Empty;
                            dr["Products"] = or["Products"].ToString() ?? string.Empty;
                            dr["EstimatedAnnualSales"] = ConvertObjToDecimal(or["EstimatedAnnualSales"]);
                            dr["EstimatedAnnualSalesUSD"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);
                            dr["EstimatedAnnualMPC"] = ConvertObjToDecimal(or["EstimatedAnnualMPC"]);
                            dr["EstimatedAnnualMPCUSD"] = ConvertObjToDecimal(or["EstimatedAnnualMPCUSD"]);
                            if (!string.IsNullOrEmpty(or["ExpectedDateFirstSales"].ToString()))
                                dr["ExpectedDateFirstSales"] = ConvertObjToDate(or["ExpectedDateFirstSales"]);
                            dr["NewBusinessCategory"] = or["NewBusinessCategory"].ToString() ?? string.Empty;

                            dr["CDPCCP"] = or["CDPCCP"].ToString() ?? string.Empty;
                            dr["DifferentiatedTechnology"] = ConvertObjToBool(or["DifferentiatedTechnology"]);
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return dt;
        }
               
        public static DataTable GetAllSalesFunnelReport(string CountriesSrchString, int SelectedDivisionID, string ProjectTypesSrchString, DateTime startmonth, DateTime firstmonth, DateTime lastmonth, 
            bool showMiscColumns, bool kpm, bool showallkpm, string cdpccpfilter)
        {
            string report = "SalesFunnel";
            DataTable dt = new DataTable
            {
                TableName = SalesFunnel// "A-Sls Fnl";
            };
            try
            {
                //add report columns for data
                AddReportColumns(ref dt, report);

                //add comments column data
                int intstartmonth = ConvertDateToMonthInt(firstmonth);
                int intlastmonth = ConvertDateToMonthInt(lastmonth);

                //add month columns for comments
                AddReportMonths(ref dt, firstmonth, lastmonth);

                Collection<MonthlyActivityStatusModel> projectmonthscoll = new Collection<MonthlyActivityStatusModel>();

                using (SqlCommand ocdetails = new SqlCommand())
                {
                    ocdetails.Connection = Conn;
                    ocdetails.CommandType = CommandType.StoredProcedure;
                    ocdetails.CommandText = "PBSalesFunnelDetails";
                    ocdetails.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
                    ocdetails.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    ocdetails.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = SelectedDivisionID;
                    ocdetails.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    ocdetails.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    using (SqlDataReader ordetails = ocdetails.ExecuteReader())
                    {
                        while (ordetails.Read())
                        {
                            projectmonthscoll.Add(new MonthlyActivityStatusModel()
                            {
                                StatusID = ConvertObjToInt(ordetails["StatusID"]),
                                StatusMonth = ConvertObjToDate(ordetails["StatusMonth"]),
                                Comments = ordetails["Comments"].ToString() ?? string.Empty,
                                ProjectID = ConvertObjToInt(ordetails["ProjectID"]),
                                TrialStatusID = ConvertObjToInt(ordetails["TrialStatusID"])
                            });
                        }
                    }
                }

                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "PBSalesFunnel"; //get all sales funnel projects
                    oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = SelectedDivisionID;
                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                    oc.Parameters.Add("@cdpccpids", SqlDbType.NVarChar, multipleidslen).Value = cdpccpfilter;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    int laststatusid = 0;
                    DateTime newmthdate;
                    string colname = string.Empty;
                    int elementctr = 0;
                    int trialstatusid = 0;
                    DataRow dr;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            dr = dt.NewRow();
                            dr["SalesDivision"] = or["SalesDivision"].ToString() ?? string.Empty;
                            dr["MarketSegment"] = or["MarketSegment"].ToString() ?? string.Empty;
                            dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
                            dr["ProjectName"] = or["ProjectName"].ToString() ?? string.Empty;
                            dr["Resources"] = or["Resources"].ToString() ?? string.Empty;
                            dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
                            dr["Application"] = or["Application"].ToString() ?? string.Empty;
                            dr["Products"] = or["Products"].ToString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(or["ActivatedDate"].ToString()))
                                dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
                            dr["ProbabilityOfSuccess"] = ConvertObjToDecimal(or["ProbabilityOfSuccess"]);
                            dr["EstimatedAnnualSalesUSD"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);
                            dr["SMCode"] = or["SMCode"].ToString() ?? string.Empty;
                            dr["OPCOOpenField"] = ConvertObjToDecimal(or["OPCOOpenField"]);
                            if (!string.IsNullOrEmpty(or["ExpectedDateFirstSales"].ToString()))
                                dr["ExpectedDateFirstSales"] = ConvertObjToDate(or["ExpectedDateFirstSales"]);
                            dr["ProjectTypeColour"] = or["ProjectTypeColour"].ToString() ?? string.Empty;
                            dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
                            dr["GIN"] = or["GIN"].ToString() ?? string.Empty;
                            dr["CustomerNumber"] = or["CustomerNumber"] ?? string.Empty;
                            dr["KPM"] = ConvertObjToBool(or["KPM"]);

                            dr["NewBusinessCategory"] = or["NewBusinessCategory"].ToString() ?? string.Empty;
                            dr["CDPCCP"] = or["CDPCCP"].ToString() ?? string.Empty;
                            dr["DifferentiatedTechnology"] = ConvertObjToBool(or["DifferentiatedTechnology"]);

                            dr["ProjectType"] = or["ProjectType"].ToString() ?? string.Empty;
                            if (ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]) > 0)
                                dr["GMPercent"] = ConvertObjToDecimal(or["GMUSD"]) / ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);

                            if (!string.IsNullOrEmpty(or["ExpectedDateFirstSales"].ToString()))
                            {
                                decimal remainingfraction = ConvertObjToDecimal((12 - (decimal)((DateTime)ConvertObjToDate(or["ExpectedDateFirstSales"])).Month) / 12);
                                dr["EOYSales"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]) * remainingfraction;
                                dr["EOYMPC"] = ConvertObjToDecimal(or["EstimatedAnnualMPCUSD"]) * remainingfraction;
                            }

                            var projectmonths = from p in projectmonthscoll
                                                where p.ProjectID == Convert.ToInt32(or["ProjectID"])
                                                orderby p.StatusMonth descending
                                                select p;

                            if (projectmonths != null)
                            {
                                elementctr = 0;
                                laststatusid = 0;
                                trialstatusid = 0;
                                foreach (var pr in projectmonths)
                                {
                                    //if month matches column then add the comment to the 
                                    newmthdate = (DateTime)pr.StatusMonth;
                                    colname = newmthdate.ToString("MMM") + " " + newmthdate.Year.ToString();
                                    if (ConvertDateToMonthInt(newmthdate) >= intstartmonth && ConvertDateToMonthInt(newmthdate) <= intlastmonth)
                                        dr[colname] = pr.Comments;

                                    if (elementctr == 0)
                                        laststatusid = pr.StatusID;

                                    elementctr++;
                                    if (pr.TrialStatusID > 0 && trialstatusid == 0)
                                        trialstatusid = pr.TrialStatusID;
                                }
                            }
                            dr["SalesFunnelStage"] = GetPlaybookDescriptionFromID(laststatusid);
                            dr["Colour"] = GetActivityStatusColorFromID(laststatusid);
                            dr["ProjectStatus"] = GetSalesEffortStatusFromID(trialstatusid, or["ProjectStatusID"]);
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return dt;
        }

        public static Collection<ReportFields> GetReportFields(string reportname)
        {
            Collection<ReportFields> fields = new Collection<ReportFields>();
            try
            { 
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetReportFields";
                    oc.Parameters.Add("@reportname", SqlDbType.NVarChar, namefieldlen).Value = reportname;

                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            fields.Add(new ReportFields()
                            {
                                Caption = or["Caption"].ToString() ?? string.Empty,
                                FieldName = or["Name"].ToString() ?? string.Empty,
                                DataType = or["DataType"].ToString() ?? string.Empty,
                                Alignment = or["Alignment"].ToString() ?? string.Empty,
                                Format = or["Format"].ToString() ?? string.Empty,
                                FieldType = ConvertObjToInt(or["FieldType"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return fields;
        }

        public static DataSet GetProjectReport(int projectid)
        {
            DataSet ds = new DataSet();

            string tablename = "Project Report";
            DataTable dt = new DataTable
            {
                TableName = tablename
            };
            try
            {
                //add report columns for data
                AddReportColumns(ref dt, "ProjectReport");

                DataRow dr;
                dr = dt.NewRow();
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetProjectReport";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
                            dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
                            dr["Country"] = or["Country"].ToString() ?? string.Empty;
                            dr["SalesDivision"] = or["SalesDivision"].ToString() ?? string.Empty;
                            dr["ProjectStatus"] = GetProjectStatusFromID(or["ProjectStatusID"]);
                            dr["MarketSegment"] = or["MarketSegment"].ToString() ?? string.Empty;
                            dr["Products"] = or["Products"].ToString() ?? string.Empty;
                            dr["Resources"] = or["Resources"].ToString() ?? string.Empty;
                            dr["ProjectName"] = or["ProjectName"].ToString() ?? string.Empty;
                            dr["ExpectedDateFirstSales"] = ConvertObjToDate(or["ExpectedDateFirstSales"]);
                            dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
                            dr["Description"] = or["Description"].ToString() ?? string.Empty;
                            dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
                            dr["EstimatedAnnualSalesUSD"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);
                            dr["TargetedVolume"] = ConvertObjToInt(or["TargetedVolume"]);
                            dr["ProjectType"] = or["ProjectType"].ToString() ?? string.Empty;
                            dr["KPM"] = or["KPM"].ToString() ?? string.Empty;
                            dr["Application"] = or["Application"].ToString() ?? string.Empty;
                            dr["GMUSD"] = or["GMUSD"].ToString() ?? string.Empty;
                            dr["EstimatedAnnualMPCUSD"] = ConvertObjToDecimal(or["EstimatedAnnualMPCUSD"]);
                            dr["DifferentiatedTechnology"] = or["DifferentiatedTechnology"].ToString() ?? string.Empty;
                            dr["ProbabilityOfSuccess"] = ConvertObjToDecimal(or["ProbabilityOfSuccess"]);

                            dt.Rows.Add(dr);
                        }
                    }
                }

                string activitiestablename = "Activities";
                DataTable dtactivities = new DataTable
                {
                    TableName = activitiestablename
                };
                DataColumn dc;
                dc = new DataColumn
                {
                    Caption = "Month",
                    ColumnName = "StatusMonth",
                    DataType = Type.GetType("System.DateTime")                  
                };
                dc.ExtendedProperties["Format"] = "MMM-yyyy";
                dc.ExtendedProperties["Alignment"] = "Center";
                dc.ExtendedProperties["FieldType"] = 0;                

                dtactivities.Columns.Add(dc);
                dc = new DataColumn
                {
                    Caption = "Status",
                    ColumnName = "Status",
                    DataType = Type.GetType("System.String")
                };
                dc.ExtendedProperties["Format"] = "0";
                dc.ExtendedProperties["Alignment"] = "Center";
                dc.ExtendedProperties["FieldType"] = 0;
                
                dtactivities.Columns.Add(dc);
                dc = new DataColumn
                {
                    Caption = "Comments",
                    ColumnName = "Comments",
                    DataType = Type.GetType("System.String")
                };              
                dc.ExtendedProperties["Alignment"] = "Left";
                dc.ExtendedProperties["FieldType"] = 0;
                
                dtactivities.Columns.Add(dc);
                dc = new DataColumn
                {
                    Caption = "Trial Status",
                    ColumnName = "TrialStatus",
                    DataType = Type.GetType("System.String")
                };              
                dc.ExtendedProperties["Alignment"] = "Center";
                dc.ExtendedProperties["FieldType"] = 0;

                dtactivities.Columns.Add(dc);
                DataRow dract;                                                               
               
                FullyObservableCollection<MonthlyActivityStatusModel> monthlyactivities =  GetMonthlyProjectData(projectid);                    
                for(int i=0; i<monthlyactivities.Count;i++)
                {
                    dract = dtactivities.NewRow();
                    dract["StatusMonth"] = monthlyactivities[i].StatusMonth;
                    dract["Status"] = GetActivityDescriptionFromID(monthlyactivities[i].StatusID);
                    dract["Comments"] = monthlyactivities[i].Comments;
                    dract["TrialStatus"] = GetTrialStatusFromID(monthlyactivities[i].TrialStatusID);
                    dtactivities.Rows.Add(dract);
                }                                        
                
                ds.Tables.Add(dt);
                ds.Tables.Add(dtactivities);
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return ds;
        }

        #endregion

        #region Helper functions
     
        private static string GetProjectStatusFromID(object obj)
        {
            bool isnumber = int.TryParse(obj.ToString(), out int id);
            foreach (EnumValue ev in EnumerationLists.ProjectStatusTypesList)            
                if (ev.ID == id)                
                    return ev.Description;                            
            return string.Empty;
        }                                           
        
        //[Description("No trial")]
        //NoTrial = 1,
        //[Description("Paused")]
        //Paused = 2,
        //[Description("Running")]
        //Running = 3,
        //[Description("Failed")]
        //Failed = 4,        
        //[Description("Successful")]
        //Successful = 5                   

        private static string GetSalesEffortStatusFromID(int trialstatusid, object obj)
        {
            //if project status is cancelled then return cancelled 3
            //if project status is complete then return complete 6
            //if project's last trial activity was a failed trial =4 then return failed trial 5
            //if project's last trial activity was paused =2 then return Late/Delayed 3
            //if project's last trial activity was running =3 or successful =4 or no trial =1 then return Ontrack 1

            bool isnumber = int.TryParse(obj.ToString(), out int projectstatusid);

            if (projectstatusid == (int)ProjectStatusType.Cancelled)  //==3)
                return EnumerationManager.GetEnumDescription(SalesStatusType.Cancelled);
            if (projectstatusid == (int)ProjectStatusType.Completed) //==2)
                return EnumerationManager.GetEnumDescription(SalesStatusType.Complete);
            if (trialstatusid == (int)TrialStatusType.Failed) //==4)
                return EnumerationManager.GetEnumDescription(SalesStatusType.FailedTrial);
            if (trialstatusid == (int)TrialStatusType.Paused) //==2)
                return EnumerationManager.GetEnumDescription(SalesStatusType.LateDelayed);
            if (trialstatusid == (int)TrialStatusType.Running || trialstatusid == (int)TrialStatusType.Successful || trialstatusid == (int)TrialStatusType.NoTrial)
                return EnumerationManager.GetEnumDescription(SalesStatusType.OnTrack);         

            return EnumerationManager.GetEnumDescription(SalesStatusType.OnTrack);
        }
        
        private static string GetPlaybookDescriptionFromID(int id)
        {
            foreach (ActivityStatusCodesModel asc in ActivityStatusCodes)            
                if (asc.GOM.ID == id)                
                    return asc.PlaybookDescription;                            
            return string.Empty;
        }
    
        public static object ConvertType(DataColumn dc,  object obj)
        {
            if (dc.DataType == Type.GetType("System.Decimal"))            
                return (decimal)ConvertObjToDecimal(obj);            
            else
                if (dc.DataType == Type.GetType("System.String"))            
                return obj.ToString();            
            else
                    if (dc.DataType == Type.GetType("System.Int32"))            
                return ConvertObjToInt(obj);            
            else
                        if (dc.DataType == Type.GetType("System.DateTime"))            
                return ConvertObjToDate(obj);            
            else
                            if (dc.DataType == Type.GetType("System.Boolean"))            
                return ConvertObjToBool(obj);            
            else
                                if (dc.DataType == Type.GetType("System.Double"))            
                return ConvertObjToDouble(obj);            
            else
                return null;

        }

        public static int ConvertObjToInt(object obj)
        {
            bool isnumber = int.TryParse(obj.ToString(), out int id);
            return id;
        }

        public static decimal ConvertObjToDecimal(object obj)
        {
            bool isnumber = decimal.TryParse(obj.ToString(), out decimal id);
            return id;
        }

        private static double ConvertObjToDouble(object obj)
        {
            bool isnumber = double.TryParse(obj.ToString(), out double id);
            return id;
        }

        private static bool ConvertObjToBool(object obj)
        {
            bool isbool = bool.TryParse(obj.ToString(), out bool boolval);
            return boolval;
        }

        private static string ConvertObjToYesNoBool(object obj)
        {
            bool isbool = bool.TryParse(obj.ToString(), out bool boolval);
            if (boolval)
                return "Yes";
            else
                return "No";
        }
        
        public static DateTime? ConvertObjToDate(object obj)
        {
            bool isdate = DateTime.TryParse(obj.ToString(), out DateTime dt);
            if (isdate)
                return dt;
            else
                return null;
        }

        public static int ConvertDateToMonthInt(DateTime? dt)
        {
            return ((DateTime)dt).Month - 1 + ((DateTime)dt).Year * 12;
        }

        public static DateTime ConvertMonthIntToDateTime(int monthint)
        {
            int mth = monthint % 12 + 1;
            int yr = (monthint - monthint % 12) / 12;
            return new DateTime(yr, mth, 1);
        }

        public static string ConvertMonthDateToString(DateTime? dt)
        {
            if (dt == null) return string.Empty;
            return ((DateTime)dt).ToString("dd") + "-" + ((DateTime)dt).ToString("MMM") + "-" + ((DateTime)dt).ToString("yyyy");
        }

        public static DateTime? ConvertDateToMonth(DateTime? dt)
        {
            if (dt == null) return null;
            return new DateTime(((DateTime)dt).Year, ((DateTime)dt).Month, 1);
        }

        private static void ShowError(Exception e, [CallerMemberName] string operationtype = null)
        {
            IMessageBoxService msg = new MessageBoxService();
            msg.ShowMessage("Error during " + operationtype + " operation\n" + e.Message.ToString(), operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            msg = null;
        }

        public static void HandleSQLError(SqlException e, [CallerMemberName] string operationtype = null)
        {
            if (e.Number == 4060)            
                App.splashScreen.AddMessage("Unable to connect to database.\nProgram now shutting down.",3000);            
            else            
                App.splashScreen.AddMessage("SQL Error Occurred.\nProgram now shutting down.\n" + e.Number.ToString(),3000);               
            
            App.splashScreen?.LoadComplete();
            App.CloseProgram();

           // throw new Exception("SQL Error");

        }

        #endregion

    }
}