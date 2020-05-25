using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using static PTR.StaticCollections;
using static PTR.Constants;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using PTR.Models;
using System.Collections.Generic;
using System.Linq;

namespace PTR
{
    static class DatabaseQueries
    {        
        const int multipleidslen = 255;
        const int namefieldlen = 50;
        const int descrfieldlen = 255;
        const int maxdescrlen = 2000;
        const int culturecodelen = 10;
        const int ginlen = 10;
        const int colourlen = 20;
        const string spprefix = "PTR";
        const int commentslen = 500;
        //const int datatypelen = 20;
        const int alignmentlen = 20;
        //const int reportfieldformatlen = 20;

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
                    oc.CommandText = spprefix + "GetSetup";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            setup = new SetupModel()
                            {
                                Domain = or["Domain"].ToString() ?? string.Empty,
                                Emailformat = or["Emailformat"].ToString() ?? string.Empty,                                
                                MaxProjectNameLength = ConvertObjToInt(or["MaxProjectNameLength"]),
                                EPRequired = ConvertObjToBool(or["EPRequired"]),
                                DefaultTrialStatusID = ConvertObjToInt(or["DefaultTrialStatusID"]),
                                StatusIDforTrials = ConvertObjToInt(or["StatusIDforTrials"]),
                                ValidateProducts = ConvertObjToBool(or["ValidateProducts"]),
                                ColourisePlaybookReport = ConvertObjToBool(or["ColourisePlaybookReport"]),
                                DefaultSalesStatuses = or["DefaultSalesStatuses"].ToString() ?? string.Empty,
                                ProductDelimiter = ConvertObjToChar(or["ProductDelimiter"]),
                                DefaultMasterListStartMonth = (DateTime)ConvertObjToDate(or["DefaultMasterListStartMonth"]),
                                DisablePreviousMonths = ConvertObjToBool(or["DisablePreviousMonths"])
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

        public static DataTable GetProjectList(string countries, string associates)
        {            
            DataTable dt = new DataTable(UserProjectList);
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectList";
                    oc.Parameters.Add("@countries", SqlDbType.NVarChar, multipleidslen).Value = countries ?? string.Empty;
                    oc.Parameters.Add("@users", SqlDbType.NVarChar, multipleidslen).Value = associates ?? string.Empty;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
                    
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, dt.TableName);
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

        public static ProjectModel GetSingleProject(int projectid)
        {
            ProjectModel newproject = new ProjectModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSingleProject";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            newproject = new ProjectModel
                            {
                                ID = projectid,
                                Name = or["Name"].ToString() ?? string.Empty,
                                Description = or["Description"].ToString() ?? string.Empty,
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                IndustrySegmentID = ConvertObjToInt(or["IndustrySegmentID"]),
                                SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]),
                                OwnerID = ConvertObjToInt(or["OwnerID"]),
                                EstimatedAnnualSales = ConvertObjToDecimal(or["EstimatedAnnualSales"]),
                                Products = or["Products"].ToString() ?? string.Empty,
                                Resources = or["Resources"].ToString() ?? string.Empty,
                                ProjectStatusID = ConvertObjToInt(or["ProjectStatusID"]),                              
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
                                DifferentiatedTechnology = ConvertObjToBool(or["DifferentiatedTechnology"]),
                                Comments = or["Comments"].ToString() ?? string.Empty,
                                IsNewBusiness = ConvertObjToBool(or["IsNewBusiness"]),
                                IncompleteReasonID = ConvertObjToInt(or["IncompleteReasonID"]),
                                PriorityID = ConvertObjToInt(or["PriorityID"]),
                                SponsorID = ConvertObjToInt(or["SponsorID"]),
                                MiscDataID = ConvertObjToInt(or["MiscellaneousDataID"]),
                                AllowNonOwnerEdits = ConvertObjToBool(or["AllowNonOwnerEdits"]),
                                AllowNonOwnerMileStoneAccess = ConvertObjToBool(or["AllowNonOwnerMilestoneAccess"]),
                                CreatorID = ConvertObjToInt(or["CreatorID"]),
                                UnitCost = ConvertObjToDecimal(or["UnitCost"])
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

        public static DataRowView GetBasicProjectDetails(int projectid, int userid)
        {
            DataTable project = new DataTable(BasicProjectDetails);
            DataRowView dr = null;           
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSimpleProjectDetails";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                    
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(project);
                    }
                    
                    dr = project.DefaultView[0];
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
            return dr;
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
                    oc.CommandText = spprefix + "GetEvaluationPlan";
                    oc.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            ep = new EPModel()
                            {
                                ID = id,
                                Description = or["Title"].ToString() ?? string.Empty,                               
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
                    oc.CommandText = spprefix + "GetProjectEvaluationPlans";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            eps.Add(new EPModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Description = or["Title"].ToString() ?? string.Empty,
                                ProjectID = projectid,
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

        //public static FullyObservableCollection<MonthlyActivityStatusModel> GetMonthlyProjectStatuses(int projectid)
        //{
        //    FullyObservableCollection<MonthlyActivityStatusModel> projectstatuses = new FullyObservableCollection<MonthlyActivityStatusModel>();
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = spprefix + "GetProjectActivities";
        //            oc.Parameters.AddWithValue("@projectid", projectid);
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    projectstatuses.Add(new MonthlyActivityStatusModel()
        //                    {
        //                        ID = ConvertObjToInt(or["ID"]),
        //                        Comments = or["Comments"].ToString() ?? string.Empty,
        //                        ProjectID = projectid,
        //                        StatusID = ConvertObjToInt(or["StatusID"]),
        //                        StatusMonth = ConvertObjToDate(or["StatusMonth"]),
        //                        TrialStatusID = ConvertObjToInt(or["TrialStatusID"]),
        //                        ExpectedDateFirstSales = ConvertObjToDate(or["ExpectedDateFirstSales"])                                
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return projectstatuses;
        //}                     

        public static FullyObservableCollection<ExchangeRateModel> GetExchangeRates(int countryid)
        {
            FullyObservableCollection<ExchangeRateModel> exchangeratescoll = new FullyObservableCollection<ExchangeRateModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetExchangeRates";
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
                                CountryID = countryid,
                                IsDirty = false
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
        
        public static FullyObservableCollection<CustomerModel> GetCustomers()
        {
            FullyObservableCollection<CustomerModel> customers = new FullyObservableCollection<CustomerModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetCustomers";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            customers.Add(new CustomerModel
                            {                                
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Deleted = ConvertObjToBool(or["Deleted"]),
                                Number = or["Number"].ToString() ?? string.Empty,
                                Location = or["Location"].ToString() ?? string.Empty,
                                CountryID = ConvertObjToInt(or["CountryID"]),
                                CultureCode = or["CultureCode"].ToString() ?? string.Empty,
                                SalesRegionID = ConvertObjToInt(or["SalesRegionID"]),
                                SalesRegionName = or["SalesRegionName"].ToString() ?? string.Empty,
                                CountryName = or["Country"].ToString() ?? string.Empty,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
                    oc.CommandText = spprefix + "GetUsers";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            users.Add(new UserModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Deleted = ConvertObjToBool(or["Deleted"]),
                                Administrator = ConvertObjToBool(or["Administrator"]),
                                AdministrationMnu = or["AdministrationMnu"].ToString() ?? string.Empty,
                                ProjectsMnu = or["ProjectsMnu"].ToString() ?? string.Empty,
                                ReportsMnu = or["ReportsMnu"].ToString() ?? string.Empty,
                                ShowOthers = ConvertObjToBool(or["ShowOthers"]),
                                LoginName = or["LoginName"].ToString() ?? string.Empty,
                                GIN = or["GIN"].ToString() ?? string.Empty,
                                BusinessUnits = or["SalesDivisions"].ToString() ?? string.Empty,
                                Email = or["Email"].ToString() ?? string.Empty,                                
                                AllowEditCompletedCancelled = ConvertObjToBool(or["AllowEditCompletedCancelled"])
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
            UserModel user = new UserModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetUserFromLogin";
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = loginname;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            user.ID = ConvertObjToInt(or["ID"]);
                            user.Name = or["Name"].ToString() ?? string.Empty;
                            user.Deleted = ConvertObjToBool(or["Deleted"]);
                            user.GIN = or["GIN"].ToString() ?? string.Empty;
                            user.BusinessUnits = or["SalesDivisions"].ToString() ?? string.Empty;
                            user.Administrator = ConvertObjToBool(or["Administrator"]);
                            user.AdministrationMnu = or["AdministrationMnu"].ToString() ?? string.Empty;
                            user.ProjectsMnu = or["ProjectsMnu"].ToString() ?? string.Empty;
                            user.ReportsMnu = or["ReportsMnu"].ToString() ?? string.Empty;
                            user.ShowOthers = ConvertObjToBool(or["ShowOthers"]);
                            user.Email = or["Email"].ToString() ?? string.Empty;                      
                            user.AllowEditCompletedCancelled = ConvertObjToBool(or["AllowEditCompletedCancelled"]);
                        }
                    }
                }
            }
            catch
            {
                user.ID = 0;
            }
            return user;
        }

        public static FullyObservableCollection<IndustrySegmentModel> GetIndustrySegments()
        {
            FullyObservableCollection<IndustrySegmentModel> industrysegments = new FullyObservableCollection<IndustrySegmentModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetIndustrySegments";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            industrysegments.Add(new IndustrySegmentModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IndustryID = ConvertObjToInt(or["IndustryID"]),
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return industrysegments;
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
                    oc.CommandText = spprefix + "GetActivityStatusCodes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            activitystatuscodes.Add(new ActivityStatusCodesModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Status"].ToString() ?? string.Empty,
                                Description = or["Description"].ToString() ?? string.Empty,
                                Colour = or["Colour"].ToString() ?? string.Empty,
                                PlaybookDescription = or["PlaybookDescription"].ToString() ?? string.Empty
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

        public static FullyObservableCollection<ApplicationModel> GetApplications()
        {
            FullyObservableCollection<ApplicationModel> Applications = new FullyObservableCollection<ApplicationModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetApplications";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            Applications.Add(new ApplicationModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return Applications;
        }

        public static Collection<IndustrySegmentApplicationJoinModel> GetIndustrySegmentApplicationJoin()
        {
            Collection<IndustrySegmentApplicationJoinModel> mktsegappcats = new Collection<IndustrySegmentApplicationJoinModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetIndustrySegmentApplicationJoin";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            mktsegappcats.Add(new IndustrySegmentApplicationJoinModel
                            {
                                IndustrySegmentID = ConvertObjToInt(or["IndustrySegmentID"]),
                                ApplicationID = ConvertObjToInt(or["ApplicationID"])                                
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
            return mktsegappcats;
        }

        public static FullyObservableCollection<IndustrySegmentApplicationJoinModel> GetIndustrySegmentApplicationJoinCRUD()
        {
            FullyObservableCollection<IndustrySegmentApplicationJoinModel> mktsegappcats = new FullyObservableCollection<IndustrySegmentApplicationJoinModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetIndustrySegmentApplicationJoinCRUD";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            mktsegappcats.Add(new IndustrySegmentApplicationJoinModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                IndustrySegmentID = ConvertObjToInt(or["IndustrySegmentID"]),
                                IndustrySegmentIndustryID = ConvertObjToInt(or["IndustrySegmentIndustryID"]),
                                ApplicationID = ConvertObjToInt(or["ApplicationID"]),
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return mktsegappcats;
        }

        public static FullyObservableCollection<ModelBaseVM> GetNewBusinessCategories()
        {
            FullyObservableCollection<ModelBaseVM> newbusinesscats = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetNewBusinessCategories";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            newbusinesscats.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
                    oc.CommandText = spprefix + "GetCountries";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            countries.Add(new CountryModel
                            {                                
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
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

        public static FullyObservableCollection<ModelBaseVM> GetBusinessUnits()
        {
            FullyObservableCollection<ModelBaseVM> bus = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSalesDivisions";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            bus.Add(
                                new ModelBaseVM()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    IsChecked = false,
                                    IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return bus;
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
                    oc.CommandText = spprefix + "GetMilestone";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            milestone = new MilestoneModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Description = or["Description"].ToString() ?? string.Empty,
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
                    oc.CommandText = spprefix + "GetProjectMilestones";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    oc.Parameters.Add("@currentuserid", SqlDbType.Int).Value = CurrentUser.ID;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            milestones.Add(new MilestoneModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Description = or["Description"].ToString() ?? string.Empty,
                                ProjectID = projectid,
                                UserID = ConvertObjToInt(or["UserID"]),
                                TargetDate = ConvertObjToDate(or["TargetDate"]),
                                CompletedDate = ConvertObjToDate(or["CompletedDate"]),
                                UserName = or["UserName"].ToString() ?? string.Empty,
                                IsEnabled = ConvertObjToBool(or["IsEnabled"])
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
                    oc.CommandText = spprefix + "GetOverdueMilestones";
                    oc.Parameters.Add("@currentdate", SqlDbType.Date).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
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

        //public static int GetCountCustomerProjects(int customerid)
        //{
        //    int i = 0;
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = spprefix + "GetCountCustomerProjects";
        //            oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
        //            try
        //            {
        //                bool isnumber = int.TryParse(oc.ExecuteScalar().ToString(), out i);
        //            }
        //            catch
        //            {
        //                i = 0;
        //            }
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return i;
        //}

        //public static int GetCountSalesRegionCustomers(int salesregionid)
        //{
        //    int i = 0;
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = spprefix + "GetCountSalesRegionCustomers";
        //            oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = salesregionid;
        //            try
        //            {
        //                bool isnumber = int.TryParse(oc.ExecuteScalar().ToString(), out i);
        //            }
        //            catch
        //            {
        //                i = 0;
        //            }
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return i;
        //}

        public static FullyObservableCollection<ModelBaseVM> GetOperatingCompanies()
        {
            FullyObservableCollection<ModelBaseVM> opcos = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetOperatingCompanies";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {

                        while (or.Read())
                        {
                            opcos.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty 
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
                    oc.CommandText = spprefix + "GetSalesRegions";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            regions.Add(new SalesRegionModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                CountryID = ConvertObjToInt(or["CountryID"])                                
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

        public static FullyObservableCollection<SalesRegionModel> GetCountrySalesRegions(int countryid)
        {
            FullyObservableCollection<SalesRegionModel> salesregions = new FullyObservableCollection<SalesRegionModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetCountrySalesRegions";
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            salesregions.Add(new SalesRegionModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                CountryID = countryid,
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return GetAllTVNodes(spprefix + "GetAllTVOperatingCompanies", 0);
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVCountries()
        {
            return GetAllTVNodes(spprefix + "GetAllTVCountries", 0);
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVSalesRegions()
        {
            return GetAllTVNodes(spprefix + "GetAllTVSalesRegions", 0);
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetAllTVCustomers()
        {
            return GetAllTVNodes(spprefix + "GetAllTVCustomers", 1);
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

        #region User's customer Treeview

        public static FullyObservableCollection<TreeViewNodeModel> GetTVOperatingCompanies()
        {
            return GetTVNodes(spprefix + "GetTVOperatingCompanies");
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetTVCountries(int opcoid)
        {
            return GetTVNodes(spprefix + "GetTVCountries");
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetTVSalesRegions(int countryid)
        {
            return GetTVNodes(spprefix + "GetTVSalesRegions");
        }

        public static FullyObservableCollection<TreeViewNodeModel> GetTVCustomers(int userid)
        {
            return GetTVCustomerNodes(spprefix + "GetTVCustomers", userid);
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
                                IsChecked = (ConvertObjToInt(or["AccessID"]) > 0),
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

        public static FullyObservableCollection<ModelBaseVM> GetProductGroupNames()
        {
            FullyObservableCollection<ModelBaseVM> prodgroupnames = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProductGroupNames";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            prodgroupnames.Add(new ModelBaseVM()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"]),
                                IsSelected = ConvertObjToBool(or["UpperCase"])
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
            return prodgroupnames;
        }

        public static FullyObservableCollection<SMCodeModel> GetSMCodes()
        {
            FullyObservableCollection<SMCodeModel> smcodes = new FullyObservableCollection<SMCodeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSMCodes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            smcodes.Add(new SMCodeModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Description = or["Description"].ToString() ?? string.Empty,
                                IndustryID = ConvertObjToInt(or["IndustryID"]),
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
                    oc.CommandText = spprefix + "GetUserCustomerAccess";
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
                                CountryID = ConvertObjToInt(or["CountryID"])
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

        public static FullyObservableCollection<UserModel> GetCustomerUserAccess(int customerid)
        {
            FullyObservableCollection<UserModel> users = new FullyObservableCollection<UserModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetCustomerUserAccess";
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            users.Add(new UserModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Deleted = ConvertObjToBool(or["Deleted"]),
                                LoginName = or["LoginName"].ToString() ?? string.Empty,
                                Administrator = ConvertObjToBool(or["Administrator"])
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

        //public static FullyObservableCollection<UserModel> GetCustomerFullUserAccess(int customerid)
        //{
        //    FullyObservableCollection<UserModel> users = new FullyObservableCollection<UserModel>();
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetCustomerFullUserAccess";
        //            oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    users.Add(new UserModel
        //                    {
        //                        
        //                        ID = ConvertObjToInt(or["ID"]),
        //                        Name = or["Name"].ToString() ?? string.Empty,
        //                        Deleted = ConvertObjToBool(or["Deleted"]),
        //                        
        //                        LoginName = or["LoginName"].ToString() ?? string.Empty,
        //                        Administrator = ConvertObjToBool(or["Administrator"])
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return users;
        //}

        public static FullyObservableCollection<ProjectTypeModel> GetProjectTypes()
        {
            FullyObservableCollection<ProjectTypeModel> projecttypes = new FullyObservableCollection<ProjectTypeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectTypes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            projecttypes.Add(new ProjectTypeModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Colour = or["Colour"].ToString() ?? string.Empty,
                                Description = or["Description"].ToString() ?? string.Empty,                                
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"]),
                                ShowSponsor = ConvertObjToBool(or["ShowSponsor"]),
                                MiscellaneousDataLabel = or["MiscellaneousDataLabel"].ToString() ?? string.Empty,
                                ShowUnitCost = ConvertObjToBool(or["ShowUnitCost"]),
                                ShowPriority = ConvertObjToBool(or["ShowPriority"]),
                                ProductRequired = ConvertObjToBool(or["ProductRequired"]),
                                SalesRequired = ConvertObjToBool(or["SalesRequired"]),
                                SalesVolumeRequired = ConvertObjToBool(or["SalesVolumeRequired"]),
                                GMRequired = ConvertObjToBool(or["GMRequired"]),
                                MPCRequired = ConvertObjToBool(or["MPCRequired"]),
                                ProbabilityRequired = ConvertObjToBool(or["ProbabilityRequired"]),
                                OpportunityCatRequired = ConvertObjToBool(or["OpportunityCatRequired"]),
                                ShowKPM = ConvertObjToBool(or["ShowKPM"]),
                                ShowDifferentiatedTech = ConvertObjToBool(or["ShowDifferentiatedTech"]),
                                UnitPriceRequired = ConvertObjToBool(or["UnitPriceRequired"]),
                                
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

        public static FullyObservableCollection<MaintenanceModel> GetOverdueMonthlyUpdates()
        {
            FullyObservableCollection<MaintenanceModel> overdue = new FullyObservableCollection<MaintenanceModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetOverdueMonthlyUpdates";
                    oc.Parameters.Add("@currentmonth", SqlDbType.Date).Value = ConvertDateToMonth(DateTime.Now);
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
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
                    oc.CommandText = spprefix + "GetIncompleteEPs";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
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
                    oc.CommandText = spprefix + "GetMissingEPs";
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
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
                    oc.CommandText = spprefix + "GetProjectsRequiringCompletion";
                    oc.Parameters.Add("@currentmonth", SqlDbType.Date).Value = ConvertDateToMonth(DateTime.Now);
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
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

        public static (bool versionok, bool upgrade, string url, string executable) GetVersion()
        {
            try
            {
                string latestversion = string.Empty;
                string userversion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                bool mustupgrade = false;
                string strurl = string.Empty;
                string executablename = string.Empty;
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetVersion";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            latestversion = or["VersionNumber"].ToString() ?? string.Empty;
                            mustupgrade = ConvertObjToBool(or["MustUpgrade"]);
                            strurl = or["URL"].ToString() ?? string.Empty;
                            executablename = or["ExecutableName"].ToString() ?? string.Empty;
                        }
                    }
                }
                return (versionok: (ConvertVersionToInt(userversion) >= ConvertVersionToInt(latestversion)), upgrade: mustupgrade, url: strurl, executable: executablename);
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (versionok: false, upgrade: true, url: string.Empty, executable: string.Empty);
        }
                
        public static FullyObservableCollection<TrialStatusModel> GetTrialStatuses()
        {
            FullyObservableCollection<TrialStatusModel> trialstatuses = new FullyObservableCollection<TrialStatusModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetTrialStatuses";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            trialstatuses.Add(new TrialStatusModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return trialstatuses;
        }
        
        public static FullyObservableCollection<ModelBaseVM> GetReasonsForIncompleteProject()
        {
            FullyObservableCollection<ModelBaseVM> reasons = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetReasonsForIncompleteProject";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            reasons.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return reasons;
        }

        public static FullyObservableCollection<ModelBaseVM> GetSalesStatuses()
        {
            FullyObservableCollection<ModelBaseVM> salesstatuses = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSalesStatuses";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            salesstatuses.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty
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
            return salesstatuses;
        }

        public static Collection<ReportFieldsDataTypesModel> GetReportFieldsDataTypes()
        {
            Collection<ReportFieldsDataTypesModel> items = new Collection<ReportFieldsDataTypesModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetReportFieldsDataTypes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            items.Add(new ReportFieldsDataTypesModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                DataType = or["DataType"].ToString() ?? string.Empty,
                                DataFormat = or["DataFormat"].ToString() ?? string.Empty,
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
            return items;
        }

        public static FullyObservableCollection<ModelBaseVM> GetReportNames()
        {
            FullyObservableCollection<ModelBaseVM> items = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetReportNames";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            items.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty
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
            return items;
        }

        public static FullyObservableCollection<ModelBaseVM> GetReportFieldsAlignmentTypes()
        {
            FullyObservableCollection<ModelBaseVM> items = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetReportFieldsAlignmentTypes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            items.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty
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
            return items;
        }

        public static Collection<ReportFieldDataTypeModel> GetReportFieldTypes()
        {
            Collection<ReportFieldDataTypeModel> items = new Collection<ReportFieldDataTypeModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetReportFieldTypes";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            items.Add(new ReportFieldDataTypeModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                FieldType = ConvertObjToInt(or["FieldType"]),
                                FieldName = or["FieldName"].ToString() ?? string.Empty
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
            return items;
        }
                                             
        public static Dictionary<string, string> GetSystemConstants()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSystemConstants";                  
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                            dict.Add(or["ConstantName"].ToString(), or["ConstantValue"].ToString());                        
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
            return dict;
        }

        public static FullyObservableCollection<ModelBaseVM> GetPriorities()
        {
            FullyObservableCollection<ModelBaseVM> items = new FullyObservableCollection<ModelBaseVM>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetPriorities";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            items.Add(new ModelBaseVM
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty
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
            return items;
        }

        public static FullyObservableCollection<MiscellaneousDataModel> GetMiscellaneousData()
        {
            FullyObservableCollection<MiscellaneousDataModel> items = new FullyObservableCollection<MiscellaneousDataModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetMiscellaneousData";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            items.Add(new MiscellaneousDataModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                FKID = ConvertObjToInt(or["FKID"]),
                                IsChecked = false,
                                IsEnabled = ConvertObjToBool(or["IsDeletable"])
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
            return items;
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
                    oc.CommandText = spprefix + "LogAccess";
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
                        throw;
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
                    oc.CommandText = spprefix + "AddProject";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = project.CustomerID;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, Config.MaxProjectNameLength).Value = project.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = project.Description ?? string.Empty;
                    oc.Parameters.Add("@salesstatusid", SqlDbType.Int).Value = project.SalesStatusID;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = project.EstimatedAnnualSales;
                    oc.Parameters.Add("@ownerid", SqlDbType.Int).Value = project.OwnerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = project.SalesDivisionID;
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = project.ProjectStatusID;
                    oc.Parameters.Add("@industrysegmentid", SqlDbType.Int).Value = project.IndustrySegmentID;
                    oc.Parameters.Add("@products", SqlDbType.NVarChar, descrfieldlen).Value = project.Products ?? string.Empty;
                    oc.Parameters.Add("@resources", SqlDbType.NVarChar, descrfieldlen).Value = project.Resources ?? string.Empty;                    
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
                    oc.Parameters.Add("@differentiatedtechnology", SqlDbType.Bit).Value = project.DifferentiatedTechnology;
                    oc.Parameters.Add("@comments", SqlDbType.NVarChar, commentslen).Value = project.Comments ?? string.Empty;
                    oc.Parameters.Add("@incompletereasonid", SqlDbType.Int).Value = project.IncompleteReasonID;
                    oc.Parameters.Add("@priorityid", SqlDbType.Int).Value = project.PriorityID;
                    oc.Parameters.Add("@sponsorid", SqlDbType.Int).Value = project.SponsorID;
                    oc.Parameters.Add("@miscellaneousdataid", SqlDbType.Int).Value = project.MiscDataID;
                    oc.Parameters.Add("@allownonowneredits", SqlDbType.Bit).Value = project.AllowNonOwnerEdits;
                    oc.Parameters.Add("@allownonownermilestoneaccess", SqlDbType.Bit).Value = project.AllowNonOwnerMileStoneAccess;
                    oc.Parameters.Add("@creatorid", SqlDbType.Int).Value =CurrentUser.ID;
                    oc.Parameters.Add("@unitcost", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.UnitCost);
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
                    oc.CommandText = spprefix + "AddCustomer";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = customer.Name ?? string.Empty;
                    oc.Parameters.Add("@number", SqlDbType.NVarChar, namefieldlen).Value = customer.Number ?? string.Empty;
                    oc.Parameters.Add("@location", SqlDbType.NVarChar, namefieldlen).Value = customer.Location;
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
                        throw;
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
                    oc.CommandText = spprefix + "AddUser";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = user.Name ?? string.Empty;
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = user.LoginName ?? string.Empty;
                    oc.Parameters.Add("@email", SqlDbType.NVarChar, descrfieldlen).Value = user.Email ?? string.Empty;
                    oc.Parameters.Add("@gin", SqlDbType.NVarChar, ginlen).Value = user.GIN ?? string.Empty;
                    oc.Parameters.Add("@administrator", SqlDbType.Bit).Value = user.Administrator;
                    oc.Parameters.Add("@salesdivisions", SqlDbType.NVarChar, multipleidslen).Value = user.BusinessUnits ?? string.Empty;
                    oc.Parameters.Add("@administrationmnu", SqlDbType.NVarChar, multipleidslen).Value = user.AdministrationMnu ?? string.Empty;
                    oc.Parameters.Add("@projectsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ProjectsMnu ?? string.Empty;
                    oc.Parameters.Add("@reportsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ReportsMnu ?? string.Empty;
                    oc.Parameters.Add("@showothers", SqlDbType.Bit).Value = user.ShowOthers;                   
                    oc.Parameters.Add("@alloweditcompletedcancelled", SqlDbType.Bit).Value = user.AllowEditCompletedCancelled;
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
                    oc.CommandText = spprefix + "AddCountry";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = country.Name ?? string.Empty;
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
                        throw;
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
                    oc.CommandText = spprefix + "AddExchangeRate";
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
                        throw;
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
                    oc.CommandText = spprefix + "AddUserCustomerAccess";
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

        public static int AddMilestone(MilestoneModel milestone)
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
                    oc.CommandText = spprefix + "AddMilestone";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, maxdescrlen).Value = milestone.Description ?? string.Empty;

                    if (milestone.TargetDate == null)
                        oc.Parameters.Add("@targetdate", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@targetdate", SqlDbType.Date).Value = milestone.TargetDate;

                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = milestone.UserID;
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = milestone.ProjectID;
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

        public static int AddEvaluationPlan(EPModel ep)
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
                    oc.CommandText = spprefix + "AddEvaluationPlan";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@title", SqlDbType.NVarChar, descrfieldlen).Value = ep.Description ?? string.Empty;
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
                    oc.CommandText = spprefix + "AddSalesRegion";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = em.CountryID;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = em.Name;
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

        public static int AddProductName(ModelBaseVM em)
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
                    oc.CommandText = spprefix + "AddProductName";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = em.Name;
                    oc.Parameters.Add("@uppercase", SqlDbType.Bit).Value = em.IsSelected;
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

        public static int AddProjectType(ProjectTypeModel item)
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
                    oc.CommandText = spprefix + "AddProjectType";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = item.Description ?? string.Empty;
                    oc.Parameters.Add("@colour", SqlDbType.NVarChar, colourlen).Value = item.Colour ?? string.Empty;
                    oc.Parameters.Add("@showsponsor", SqlDbType.Bit).Value = item.ShowSponsor;
                    oc.Parameters.Add("@miscellaneousdatalabel", SqlDbType.NVarChar, namefieldlen).Value = item.MiscellaneousDataLabel ?? string.Empty;
                    oc.Parameters.Add("@showunitcost", SqlDbType.Bit).Value = item.ShowUnitCost;
                    oc.Parameters.Add("@salesrequired", SqlDbType.Bit).Value = item.SalesRequired;
                    oc.Parameters.Add("@salesvolumerequired", SqlDbType.Bit).Value = item.SalesVolumeRequired;
                    oc.Parameters.Add("@gmrequired", SqlDbType.Bit).Value = item.GMRequired;
                    oc.Parameters.Add("@mpcrequired", SqlDbType.Bit).Value = item.MPCRequired;
                    oc.Parameters.Add("@probabilityrequired", SqlDbType.Bit).Value = item.ProbabilityRequired;
                    oc.Parameters.Add("@productrequired", SqlDbType.Bit).Value = item.ProductRequired;
                    oc.Parameters.Add("@opportunitycatrequired", SqlDbType.Bit).Value = item.OpportunityCatRequired;
                    oc.Parameters.Add("@showdifferentiatedtech", SqlDbType.Bit).Value = item.ShowDifferentiatedTech;
                    oc.Parameters.Add("@showkpm", SqlDbType.Bit).Value = item.ShowKPM;
                    oc.Parameters.Add("@showpriority", SqlDbType.Bit).Value = item.ShowPriority;                   
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

        public static int AddApplication(ApplicationModel item)
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
                    oc.CommandText = spprefix + "AddApplication";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty; 
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

        public static int AddSMCode(SMCodeModel item)
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
                    oc.CommandText = spprefix + "AddSMCode";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = item.Description ?? string.Empty;
                    oc.Parameters.Add("@industryid", SqlDbType.Int).Value = item.IndustryID;
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

        public static int AddTrialStatus(TrialStatusModel item)
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
                    oc.CommandText = spprefix + "AddTrialStatus";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
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

        public static int AddNewBusinessCategory(ModelBaseVM em)
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
                    oc.CommandText = spprefix + "AddNewBusinessCategory";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, descrfieldlen).Value = em.Name ?? string.Empty;
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

        public static int AddIndustrySegmentApplication(IndustrySegmentApplicationJoinModel item)
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
                    oc.CommandText = spprefix + "AddIndustrySegmentApplication";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@industrysegmentid", SqlDbType.Int).Value = item.IndustrySegmentID;
                    oc.Parameters.Add("@applicationid", SqlDbType.Int).Value = item.ApplicationID;
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

        public static int AddIndustrySegment(IndustrySegmentModel item)
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
                    oc.CommandText = spprefix + "AddIndustrySegment";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@industryid", SqlDbType.Int).Value = item.IndustryID;
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
        
        public static int AddReasonForIncompleteProject(ModelBaseVM item)
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
                    oc.CommandText = spprefix + "AddReasonForIncompleteProject";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, descrfieldlen).Value = item.Name ?? string.Empty;
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

        public static int AddReportField(ReportFields item)
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
                    oc.CommandText = spprefix + "AddReportField";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@reportname", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@caption", SqlDbType.NVarChar, namefieldlen).Value = item.Caption ?? string.Empty;
                    oc.Parameters.Add("@fieldname", SqlDbType.NVarChar, namefieldlen).Value = item.FieldName ?? string.Empty;
                    oc.Parameters.Add("@datatypeid", SqlDbType.Int).Value = item.DataTypeID;
                    oc.Parameters.Add("@alignment", SqlDbType.NVarChar, alignmentlen).Value = item.Alignment ?? string.Empty;
                    oc.Parameters.Add("@fieldtype", SqlDbType.Int).Value = item.FieldType;
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
        
        public static int AddMiscellaneousData(MiscellaneousDataModel item)
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
                    oc.CommandText = spprefix + "AddMiscellaneousData";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@fkid", SqlDbType.Int).Value = item.FKID;                  
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

        public static Collection<string> GetColours()
        {
            Collection<string> colors = new Collection<string>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetColors";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            colors.Add(or["Color"].ToString() ?? string.Empty);
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
            return colors;
        }

        public static int AddBU(ModelBaseVM item)
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
                    oc.CommandText = spprefix + "AddBU";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
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
                    oc.CommandText = spprefix + "UpdateProject";
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = project.CustomerID;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, Config.MaxProjectNameLength).Value = project.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = project.Description ?? string.Empty;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = project.EstimatedAnnualSales;
                    oc.Parameters.Add("@ownerid", SqlDbType.Int).Value = project.OwnerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = project.SalesDivisionID;
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = project.ProjectStatusID;
                    oc.Parameters.Add("@industrysegmentid", SqlDbType.Int).Value = project.IndustrySegmentID;
                    oc.Parameters.Add("@products", SqlDbType.NVarChar, descrfieldlen).Value = project.Products ?? string.Empty;
                    oc.Parameters.Add("@resources", SqlDbType.NVarChar, descrfieldlen).Value = project.Resources ?? string.Empty;                    
                    oc.Parameters.Add("@targetedvolume", SqlDbType.Decimal).Value = project.TargetedVolume;
                    oc.Parameters.Add("@applicationid", SqlDbType.Int).Value = project.ApplicationID;
                    oc.Parameters.Add("@estimatedannualmpc", SqlDbType.Decimal).Value = project.EstimatedAnnualMPC;
                    oc.Parameters.Add("@newbusinesscategoryid", SqlDbType.Int).Value = project.NewBusinessCategoryID;
                    if (project.CompletedDate == null)
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = project.CompletedDate;
                    oc.Parameters.Add("@probabilityofsuccess", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.ProbabilityOfSuccess / 100);
                    oc.Parameters.Add("@gm", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.GM);
                    oc.Parameters.Add("@smcodeid", SqlDbType.Int).Value = project.SMCodeID;
                    oc.Parameters.Add("@projecttypeid", SqlDbType.Int).Value = project.ProjectTypeID;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = project.KPM;
                    oc.Parameters.Add("@eprequired", SqlDbType.Bit).Value = project.EPRequired;
                    oc.Parameters.Add("@differentiatedtechnology", SqlDbType.Bit).Value = project.DifferentiatedTechnology;
                    oc.Parameters.Add("@comments", SqlDbType.NVarChar, commentslen).Value = project.Comments ?? string.Empty;
                    oc.Parameters.Add("@incompletereasonid", SqlDbType.Int).Value = project.IncompleteReasonID;
                    oc.Parameters.Add("@priorityid", SqlDbType.Int).Value = project.PriorityID;
                    oc.Parameters.Add("@sponsorid", SqlDbType.Int).Value = project.SponsorID;
                    oc.Parameters.Add("@miscellaneousdataid", SqlDbType.Int).Value = project.MiscDataID;
                    oc.Parameters.Add("@allownonowneredits", SqlDbType.Bit).Value = project.AllowNonOwnerEdits;
                    oc.Parameters.Add("@allownonownermilestoneaccess", SqlDbType.Bit).Value = project.AllowNonOwnerMileStoneAccess;
                    oc.Parameters.Add("@creatorid", SqlDbType.Int).Value = CurrentUser.ID;
                    oc.Parameters.Add("@unitcost", SqlDbType.Decimal).Value = ConvertObjToDecimal(project.UnitCost);
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = project.ID;
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

        public static FullyObservableCollection<MonthlyActivityStatusModel> UpdateMonthlyActivityStatus(FullyObservableCollection<MonthlyActivityStatusModel> activities)
        {
            int result = 0;
            var q = activities.Where(x => x.StatusID == 10).FirstOrDefault();
            if (q != null)
                //update Project Status table
                UpdateForecastedSalesDate(q.ProjectID, q.StatusID, q.EstimatedAnnualSales);
            else
            {
                var q2 = activities.Where(x => x.StatusID > 0 && x.StatusID < 10).FirstOrDefault();
                if (q2 != null)
                    UpdateForecastedSalesDate(q2.ProjectID, q2.StatusID, q2.EstimatedAnnualSales);
            }
            foreach (MonthlyActivityStatusModel mas in activities)
            {
                if (mas.IsDirty)
                {
                    if (mas.StatusID != Config.StatusIDforTrials || mas.TrialStatusID < Config.DefaultTrialStatusID)
                    {
                        mas.TrialStatusID = 0;
                    }

                    if (mas.StatusMonth != null)
                    {
                        if (mas.ID > 0)                                                    
                            UpdateMonthlyActivityStatus2(mas);                        
                        else
                        {
                            if (mas.StatusID > 0)
                            {
                                result = AddMonthlyActivityStatus(mas);
                                mas.ID = result;
                            }
                        }                       
                    }
                }
            }
            return activities;
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
                    oc.CommandText = spprefix + "AddMonthlyActivityStatus";
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
                        throw;
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
                    oc.CommandText = spprefix + "UpdateMonthlyActivityStatus";
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
                        throw;
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
                    oc.CommandText = spprefix + "UpdateCustomer";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = customer.Name ?? string.Empty;
                    oc.Parameters.Add("@number", SqlDbType.NVarChar, namefieldlen).Value = customer.Number ?? string.Empty;
                    oc.Parameters.Add("@location", SqlDbType.NVarChar, namefieldlen).Value = customer.Location ?? string.Empty;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = customer.CountryID;
                    oc.Parameters.Add("@salesregionid", SqlDbType.Int).Value = customer.SalesRegionID;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = customer.Deleted;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customer.ID;
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
                    oc.CommandText = spprefix + "UpdateUser";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = user.Name ?? string.Empty;
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = user.LoginName ?? string.Empty;
                    oc.Parameters.Add("@email", SqlDbType.NVarChar, descrfieldlen).Value = user.Email ?? string.Empty;
                    oc.Parameters.Add("@gin", SqlDbType.NVarChar, ginlen).Value = user.GIN ?? string.Empty;
                    oc.Parameters.Add("@administrator", SqlDbType.Bit).Value = user.Administrator;
                    oc.Parameters.Add("@salesdivisions", SqlDbType.NVarChar, multipleidslen).Value = user.BusinessUnits ?? string.Empty;
                    oc.Parameters.Add("@administrationmnu", SqlDbType.NVarChar, multipleidslen).Value = user.AdministrationMnu ?? string.Empty;
                    oc.Parameters.Add("@projectsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ProjectsMnu ?? string.Empty;
                    oc.Parameters.Add("@reportsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ReportsMnu ?? string.Empty;
                    oc.Parameters.Add("@showothers", SqlDbType.Bit).Value = user.ShowOthers;                  
                    oc.Parameters.Add("@alloweditcompletedcancelled", SqlDbType.Bit).Value = user.AllowEditCompletedCancelled;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = user.Deleted;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = user.ID;
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
                    oc.CommandText = spprefix + "UpdateCountry";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = country.Name ?? string.Empty;
                    oc.Parameters.Add("@operatingcompanyid", SqlDbType.Int).Value = country.OperatingCompanyID;
                    oc.Parameters.Add("@culturecode", SqlDbType.NVarChar, culturecodelen).Value = country.CultureCode ?? string.Empty;
                    oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = country.UseUSD;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = country.Deleted;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = country.ID;
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
                    oc.CommandText = spprefix + "UpdateSalesRegion";
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
                      
        public static void UpdateForecastedSalesDate( int projectid, int statusid, decimal estimatedannualsales)
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
                    oc.CommandText = spprefix + "UpdateForecastedSalesDate";
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = statusid;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = estimatedannualsales;
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
                        throw;
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
                    oc.CommandText = spprefix + "UpdateExchangeRate";
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
                        throw;
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
                    oc.CommandText = spprefix + "UpdateEP";
                    oc.Parameters.Add("@title", SqlDbType.NVarChar, descrfieldlen).Value = ep.Description ?? string.Empty;
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
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = ep.Deleted;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = ep.ID;
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
                    oc.CommandText = spprefix + "UpdateMilestone";
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, maxdescrlen).Value = milestone.Description ?? string.Empty;

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
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = milestone.Deleted;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = milestone.ID;
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
                    oc.CommandText = spprefix + "UpdateUserCustomerAccess";
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

        public static void UpdateActivityStatusCode(ActivityStatusCodesModel code)
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
                    oc.CommandText = spprefix + "UpdateActivityStatusCode";
                    oc.Parameters.Add("@colour", SqlDbType.NVarChar, colourlen).Value = code.Colour;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, namefieldlen).Value = code.Description ?? string.Empty;                    
                    oc.Parameters.Add("@playbookdescription", SqlDbType.NVarChar, namefieldlen).Value = code.PlaybookDescription ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = code.ID;
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

        public static void UpdateSetup(SetupModel su)
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
                    oc.CommandText = spprefix + "UpdateSetup";
                    oc.Parameters.Add("@domain", SqlDbType.NVarChar, namefieldlen).Value = su.Domain;
                    oc.Parameters.Add("@emailformat", SqlDbType.NVarChar, namefieldlen).Value = su.Emailformat ?? string.Empty;
                    oc.Parameters.Add("@eprequired", SqlDbType.Bit).Value = su.EPRequired;
                    oc.Parameters.Add("@statusidfortrials", SqlDbType.Int).Value = su.StatusIDforTrials;
                    oc.Parameters.Add("@defaulttrialstatusid", SqlDbType.Int).Value = su.DefaultTrialStatusID;
                    oc.Parameters.Add("@validateproducts", SqlDbType.Bit).Value = su.ValidateProducts;
                    oc.Parameters.Add("@maximumprojectnamelength", SqlDbType.Int).Value = su.MaxProjectNameLength;
                    oc.Parameters.Add("@colouriseplaybookreport", SqlDbType.Bit).Value = su.ColourisePlaybookReport;
                    oc.Parameters.Add("@defaultsalesstatuses", SqlDbType.NVarChar, multipleidslen).Value = su.DefaultSalesStatuses ?? string.Empty;
                    oc.Parameters.Add("@productdelimiter", SqlDbType.Char, 1).Value = su.ProductDelimiter;
                    oc.Parameters.Add("@defaultmasterliststartmonth", SqlDbType.DateTime).Value = su.DefaultMasterListStartMonth;
                    oc.Parameters.Add("@disablepreviousmonths", SqlDbType.Bit).Value = su.DisablePreviousMonths;
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
        
        public static void UpdateProductName(ModelBaseVM item)
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
                    oc.CommandText = spprefix + "UpdateProductName";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@uppercase", SqlDbType.Bit).Value = item.IsSelected;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateProjectType(ProjectTypeModel item)
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
                    oc.CommandText = spprefix + "UpdateProjectType";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = item.Description ?? string.Empty;
                    oc.Parameters.Add("@colour", SqlDbType.NVarChar, colourlen).Value = item.Colour ?? string.Empty;
                    oc.Parameters.Add("@showsponsor", SqlDbType.Bit).Value = item.ShowSponsor;
                    oc.Parameters.Add("@miscellaneousdatalabel", SqlDbType.NVarChar, namefieldlen).Value = item.MiscellaneousDataLabel ?? string.Empty;
                    oc.Parameters.Add("@showunitcost", SqlDbType.Bit).Value = item.ShowUnitCost;
                    oc.Parameters.Add("@salesrequired", SqlDbType.Bit).Value = item.SalesRequired;
                    oc.Parameters.Add("@salesvolumerequired", SqlDbType.Bit).Value = item.SalesVolumeRequired;
                    oc.Parameters.Add("@gmrequired", SqlDbType.Bit).Value = item.GMRequired;
                    oc.Parameters.Add("@mpcrequired", SqlDbType.Bit).Value = item.MPCRequired;
                    oc.Parameters.Add("@probabilityrequired", SqlDbType.Bit).Value = item.ProbabilityRequired;
                    oc.Parameters.Add("@productrequired", SqlDbType.Bit).Value = item.ProductRequired;
                    oc.Parameters.Add("@opportunitycatrequired", SqlDbType.Bit).Value = item.OpportunityCatRequired;
                    oc.Parameters.Add("@showdifferentiatedtech", SqlDbType.Bit).Value = item.ShowDifferentiatedTech;
                    oc.Parameters.Add("@showkpm", SqlDbType.Bit).Value = item.ShowKPM;
                    oc.Parameters.Add("@showpriority", SqlDbType.Bit).Value = item.ShowPriority;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateApplication(ApplicationModel item)
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
                    oc.CommandText = spprefix + "UpdateApplication";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateSMCode(SMCodeModel item)
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
                    oc.CommandText = spprefix + "UpdateSMCode";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = item.Description ?? string.Empty;
                    oc.Parameters.Add("@industryid", SqlDbType.Int).Value = item.IndustryID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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
        
        public static void UpdateTrialStatus(TrialStatusModel item)
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
                    oc.CommandText = spprefix + "UpdateTrialStatus";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateNewBusinessCategory(ModelBaseVM item)
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
                    oc.CommandText = spprefix + "UpdateNewBusinessCategory";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, descrfieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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
                
        public static void UpdateIndustrySegmentApplication(IndustrySegmentApplicationJoinModel item)
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
                    oc.CommandText = spprefix + "UpdateIndustrySegmentApplication";
                    oc.Parameters.Add("@industrysegmentid", SqlDbType.Int).Value = item.IndustrySegmentID;
                    oc.Parameters.Add("@applicationid", SqlDbType.Int).Value = item.ApplicationID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateIndustrySegment(IndustrySegmentModel item)
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
                    oc.CommandText = spprefix + "UpdateIndustrySegment";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@industryid", SqlDbType.Int).Value = item.IndustryID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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
        
        public static void UpdateReasonForIncompleteProject(ModelBaseVM item)
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
                    oc.CommandText = spprefix + "UpdateReasonForIncompleteProject";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, descrfieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateReportField(ReportFields item)
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
                    oc.CommandText = spprefix + "UpdateReportField";
                    oc.Parameters.Add("@caption", SqlDbType.NVarChar, namefieldlen).Value = item.Caption ?? string.Empty;
                    oc.Parameters.Add("@fieldname", SqlDbType.NVarChar, namefieldlen).Value = item.FieldName ?? string.Empty;
                    oc.Parameters.Add("@datatypeid", SqlDbType.Int).Value = item.DataTypeID;
                    oc.Parameters.Add("@alignment", SqlDbType.NVarChar, alignmentlen).Value = item.Alignment ?? string.Empty;
                    oc.Parameters.Add("@fieldtype", SqlDbType.Int).Value = item.FieldType;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateMiscellaneousData(MiscellaneousDataModel item)
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
                    oc.CommandText = spprefix + "UpdateMiscellaneousData";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@fkid", SqlDbType.Int).Value = item.FKID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = item.ID;
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

        public static void UpdateBU(ModelBaseVM item)
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
                    oc.CommandText = spprefix + "UpdateBU";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, descrfieldlen).Value = item.Name ?? string.Empty;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = item.Deleted;
                    oc.Parameters.Add("@buid", SqlDbType.Int).Value = item.ID;
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

        #region Delete Queries

        //public static void DeleteCustomer(int customerid)
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
        //            oc.CommandText = spprefix + "DeleteCustomer";
        //            oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
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
                    oc.CommandText = spprefix + "DeleteSalesRegion";
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
                    oc.CommandText = spprefix + "DeleteUserCustomerAccess";
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
        
        public static void DeleteProductName(int id)
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
                    oc.CommandText = spprefix + "DeleteProductName";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteProjectType(int id)
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
                    oc.CommandText = spprefix + "DeleteProjectType";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteApplication(int id)
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
                    oc.CommandText = spprefix + "DeleteApplication";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteSMCode(int id)
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
                    oc.CommandText = spprefix + "DeleteSMCode";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteTrialStatus(int id)
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
                    oc.CommandText = spprefix + "DeleteTrialStatus";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteNewBusinessCategory(int id)
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
                    oc.CommandText = spprefix + "DeleteNewBusinessCategory";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteIndustrySegmentApplication(int id)
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
                    oc.CommandText = spprefix + "DeleteIndustrySegmentApplication";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteIndustrySegment(int id)
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
                    oc.CommandText = spprefix + "DeleteIndustrySegment";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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
                
        public static void DeleteIncompleteProjectReason(int id)
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
                    oc.CommandText = spprefix + "DeleteIncompleteProjectReason";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteReportField(int id)
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
                    oc.CommandText = spprefix + "DeleteReportField";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteMiscellaneousData(int id)
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
                    oc.CommandText = spprefix + "DeleteMiscellaneousData";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

        public static void DeleteBusinessUnit(int id)
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
                    oc.CommandText = spprefix + "DeleteBusinessUnit";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
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


        #region Custom Report

        public static FullyObservableCollection<CustomReportModel> GetCustomReports()
        {
            FullyObservableCollection<CustomReportModel> customreports = new FullyObservableCollection<CustomReportModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetCustomReports";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            customreports.Add(new CustomReportModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                SPName = or["SPName"].ToString() ?? string.Empty,
                                CombineTables = ConvertObjToBool(or["CombineTables"])
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
            return customreports;
        }

        public static FullyObservableCollection<CustomReportParametersModel> GetCustomReportParameters(int customreportid)
        {
            FullyObservableCollection<CustomReportParametersModel> paramcol = new FullyObservableCollection<CustomReportParametersModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetCustomReportParameters";
                    oc.Parameters.Add("@customreportid", SqlDbType.Int).Value = customreportid;
                    CustomReportParametersModel cm;

                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            cm = new CustomReportParametersModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                ParameterType = or["ParameterType"].ToString() ?? string.Empty,
                                DefaultValue = or["DefaultValue"].ToString() ?? string.Empty,
                                ToolTip = or["ToolTip"].ToString() ?? string.Empty,
                                DisplayName = or["DisplayName"].ToString() ?? string.Empty
                            };

                            switch (cm.ParameterType)
                            {
                                case "DateTime":
                                    if (string.IsNullOrEmpty(cm.DefaultValue))
                                        cm.Value = ConvertDateToMonth(DateTime.Now).ToString();
                                    else
                                    {
                                        bool blnDate = DateTime.TryParse(cm.DefaultValue, out DateTime enteredDate);
                                        if (blnDate)
                                            cm.Value = cm.DefaultValue;
                                        else
                                            cm.Value = ConvertDateToMonth(DateTime.Now).ToString();
                                    }
                                    break;

                                case "Int32":
                                    if (string.IsNullOrEmpty(cm.DefaultValue))
                                        cm.Value = default(int).ToString();
                                    else
                                    {
                                        bool blnInteger = int.TryParse(cm.DefaultValue, out int enteredInteger);
                                        if (blnInteger)
                                            cm.Value = cm.DefaultValue;
                                        else
                                            cm.Value = default(int).ToString();
                                    }
                                    break;

                                case "Decimal":
                                    if (string.IsNullOrEmpty(cm.DefaultValue))
                                        cm.Value = default(decimal).ToString();
                                    else
                                    {
                                        bool blnDecimal = decimal.TryParse(cm.DefaultValue, out decimal enteredDecimal);
                                        if (blnDecimal)
                                            cm.Value = cm.DefaultValue;
                                        else
                                            cm.Value = default(decimal).ToString();
                                    }
                                    break;

                                case "Boolean":
                                    if (string.IsNullOrEmpty(cm.DefaultValue))
                                        cm.Value = default(bool).ToString();
                                    else
                                    {
                                        bool blnBool = bool.TryParse(cm.DefaultValue, out bool enteredBool);
                                        if (blnBool)                                        
                                            cm.Value = cm.DefaultValue;                                        
                                        else
                                            cm.Value = default(bool).ToString();
                                    }
                                    break;

                                case "String":
                                    if (string.IsNullOrEmpty(cm.DefaultValue))
                                        cm.Value = string.Empty;
                                    else
                                        cm.Value = cm.DefaultValue;
                                    break;

                            }
                            paramcol.Add(cm);
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
            return paramcol;
        }

        public static DataSet GetCustomReportData(CustomReportModel custreport, FullyObservableCollection<CustomReportParametersModel> parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = custreport.SPName;

                    if (parameters.Count > 0)
                    {
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            switch (parameters[i].ParameterType)
                            {
                                case "String":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.NVarChar, descrfieldlen).Value = parameters[i].Value ?? string.Empty;
                                    break;

                                case "DateTime":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Date).Value = Convert.ToDateTime(parameters[i].Value);
                                    break;

                                case "Int32":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Int).Value = Convert.ToInt32(parameters[i].Value);
                                    break;

                                case "Decimal":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Decimal).Value = Convert.ToDecimal(parameters[i].Value);
                                    break;

                                case "Boolean":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Bit).Value = ConvertObjToBool(parameters[i].Value);
                                    break;

                                default:
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.NVarChar, descrfieldlen).Value = parameters[i].Value ?? string.Empty;
                                    break;
                            }                           
                        }
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(ds);
                    }

                    //for each datatable...
                    //get fields from ReportFields wher custreport.name
                    //match fields with table fields
                    //update table caption and format and extended properties
                    Collection<ReportFields> rptflds = GetReportFields(custreport.Name);
                    bool found = false;
                    int tblctr = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        dt.TableName = "Table" + tblctr.ToString();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            found = false;
                            foreach (ReportFields f in rptflds)
                            {
                                if (f.FieldName == dc.ColumnName)
                                {
                                    dc.Caption = f.Caption;
                                    //dc.DataType = System.Type.GetType("System." + f.DataType); <======cannot change datatype after column is filled
                                    dc.ExtendedProperties.Add("Alignment", f.Alignment);
                                    dc.ExtendedProperties.Add("Format", f.Format);
                                    dc.ExtendedProperties.Add("FieldType", f.FieldType);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                dc.ExtendedProperties.Add("Alignment", "Left");
                                dc.ExtendedProperties.Add("FieldType", (int)ReportFieldType.PlaybookComments);//treat as Playbook comment field
                            }
                        };
                        tblctr++;
                    }
                };
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

        public static DataSet GetSalesPipelineReport(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString,
            bool useUSD, DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm)
        {
            DataTable dt = new DataTable
            {
                TableName = SalesPipeline
            };

            DataTable dtCount = new DataTable
            {
                TableName = SalesPipelineCount
            };

            DataSet ds = new DataSet();

            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSalesPipelineReport";
                    oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
                    oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                    oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                    oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = useUSD;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }
                }

                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSalesPipelineProjectCountReport";
                    oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
                    oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                    oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                    oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = useUSD;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dtCount);
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

        public static DataTable GetSalesPipelineReportToolTip(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, bool useUSD,
            DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm)
        {
            DataTable dt = new DataTable
            {
                TableName = SalesPipelineTooltip
            };

            try
            {
                //get all status codes
                //get all months and build dt - add row for each status code, add column for status code
                //get sum of each status for each month
                //add sums to dt where 

                DataColumn dc;
                dc = new DataColumn
                {
                    Caption = "Status",
                    ColumnName = "Status",
                    DataType = Type.GetType("System.String")
                };
                dt.Columns.Add(dc);

                //add month columns for comments data
                AddReportCommentsMonths(ref dt, firstmonth, lastmonth);

                DataRow dr;
                string colname = string.Empty;
                string culturecode = string.Empty;
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetSalesPipelineToolTip";

                    foreach (ActivityStatusCodesModel code in ActivityStatusCodes)
                    {
                        dr = dt.NewRow();
                        dr["Status"] = code.ID.ToString();
                        oc.Parameters.Clear();
                        oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
                        oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
                        oc.Parameters.Add("@statusid", SqlDbType.Int).Value = code.ID;
                        oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                        oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
                        oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
                        oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
                        oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
                        oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
                        oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = useUSD;
                        oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
                        using (SqlDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                colname = or["StatusMonth"].ToString();
                                if (dr[colname].ToString().Length > 0)
                                    dr[colname] = dr[colname].ToString() + "\n";

                                dr[colname] = dr[colname].ToString() + or["Summary"].ToString() ?? string.Empty;
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
              
        public static DataTable GetStatusReportFiltered(string CountriesSrchString)
        {
            DataTable dt = new DataTable
            {
                TableName = StatusReport
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectStatusReport";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, dt.TableName);
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
                
        public static DataTable GetProjects(string CountriesSrchString)
        {
            DataTable dt = new DataTable
            {
                TableName = ProjectReport
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjects";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, dt.TableName);
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
            FullyObservableCollection<MonthlyActivityStatusModel> monthlyactivities = new FullyObservableCollection<MonthlyActivityStatusModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetMonthlyActivitiesForProject";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            monthlyactivities.Add(new MonthlyActivityStatusModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Comments = or["Comments"].ToString() ?? string.Empty,
                                ProjectID = projectid,
                                StatusID = ConvertObjToInt(or["StatusID"]),
                                StatusMonth = ConvertObjToDate(or["StatusMonth"]),
                                TrialStatusID = ConvertObjToInt(or["TrialStatusID"]),
                                ExpectedDateFirstSales = ConvertObjToDate(or["ExpectedDateFirstSales"]),
                                ShowStatus10 = false,
                                IsDirty = false
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
            return monthlyactivities;
        }

        public static DataTable GetMonthlyProjectDataReport(int projectid)
        {
            DataTable dt = new DataTable
            {
                TableName = ProjectReportActivities
            };

            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectMonthlyActivitiesReport";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, dt.TableName);
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
               
        public static DataTable GetEvaluationPlansReport(string CountriesSrchString)
        {
            DataTable dt = new DataTable
            {
                TableName = EvaluationPlanReport
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetEvaluationPlans";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, dt.TableName);
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

        //private static void AddReportColumns(ref DataTable dt, string report)
        //{
        //    DataColumn dc;
        //    //Get fields for report from DB and create new columns in datatable
        //    Collection<ReportFields> fields = GetReportFields(report);
        //    foreach (ReportFields f in fields)
        //    {
        //        dc = new DataColumn();
        //        dc.ExtendedProperties.Add("Alignment", f.Alignment);
        //        dc.ExtendedProperties.Add("Format", f.Format);
        //        dc.ExtendedProperties.Add("FieldType", f.FieldType);
        //        dc.Caption = f.Caption;
        //        dc.ColumnName = f.FieldName;
        //        dc.DataType = System.Type.GetType("System." + f.DataType); 
        //        dt.Columns.Add(dc);
        //    }
        //}

        private static void AddReportCommentsMonths(ref DataTable dt, DateTime firstmonth, DateTime lastmonth)
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
                dc.DataType = Type.GetType("System.String");
                dc.ExtendedProperties["Alignment"] = "Left";
                dc.ExtendedProperties["FieldType"] = (int)ReportFieldType.PlaybookComments;
                dt.Columns.Add(dc);
            }
        }
                
        //public static DataTable GetNewBusinessReport(string CountriesSrchString, DateTime startmonth)
        //{
        //    DataTable dt = new DataTable
        //    {
        //        TableName = GetConstant(NewBusinessXLTab)
        //    };
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = spprefix + "GetNewBusinessReport";
        //            oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
        //            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;                              
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;

        //            using (SqlDataAdapter da = new SqlDataAdapter(oc))
        //            {
        //                da.Fill(dt);
        //            }

        //            ProcessReportColumns(ref dt, Constants.NewBusinessReport);
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return dt;
        //}

        private static void ProcessReportColumns(ref DataTable dt, string report)
        {
            Collection<ReportFields> rptflds = GetReportFields(report);
            bool found = false;
            foreach (DataColumn dc in dt.Columns)
            {
                found = false;
                foreach (ReportFields f in rptflds)
                {
                    if (f.FieldName == dc.ColumnName)
                    {
                        dc.Caption = f.Caption;
                        //dc.DataType = System.Type.GetType("System." + f.DataType); <======cannot change datatype after column is filled
                        dc.ExtendedProperties.Add("Alignment", f.Alignment);
                        dc.ExtendedProperties.Add("Format", f.Format);
                        dc.ExtendedProperties.Add("FieldType", f.FieldType);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    dc.ExtendedProperties.Add("Alignment", "Left");
                    dc.ExtendedProperties.Add("FieldType", (int)ReportFieldType.PlaybookComments); //treat as Playbook comment field
                }
            }

            foreach (ReportFields f in rptflds)
            {
                if (dt.Columns.Contains(f.FieldName) && (int)dt.Columns[f.FieldName].ExtendedProperties["FieldType"] == (int)ReportFieldType.SystemAndRemoved)
                    dt.Columns.Remove(f.FieldName);                
            }
        }

        public static DataTable GetAllSalesFunnelReport(string CountriesSrchString, DateTime startmonth, DateTime firstmonth, DateTime lastmonth)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(SalesFunnelXLTab)// "A-Sls Fnl";
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetMasterProjectList"; //get all sales funnel projects
                    oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
                    oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
                    oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;              
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.ID;

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, SalesFunnelReport);
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

        public static FullyObservableCollection<ReportFields> GetReportFields(string reportname)
        {
            FullyObservableCollection<ReportFields> fields = new FullyObservableCollection<ReportFields>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetReportFields";
                    oc.Parameters.Add("@reportname", SqlDbType.NVarChar, namefieldlen).Value = reportname;

                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            fields.Add(new ReportFields()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Caption = or["Caption"].ToString() ?? string.Empty,
                                FieldName = or["Name"].ToString() ?? string.Empty,
                                DataType = or["DataType"].ToString() ?? string.Empty,
                                Alignment = or["Alignment"].ToString() ?? string.Empty,
                                Format = or["Format"].ToString() ?? string.Empty,
                                FieldType = ConvertObjToInt(or["FieldType"]),
                                System = ConvertObjToBool(or["System"]),
                                DataTypeID = ConvertObjToInt(or["DataTypeID"])
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
            DataTable dt = new DataTable
            {
                TableName = SingleProjectReport
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectReport";
                    oc.Parameters.AddWithValue("@projectid", projectid);
                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, dt.TableName);
                }

                DataTable dtactivities = new DataTable
                {
                    TableName = SingleProjectReportActivities
                };

                dtactivities = GetMonthlyProjectDataReport(projectid);

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

        private static int ConvertVersionToInt(string version)
        {
            int progversion = 0;
            string strversion = version.Replace(".", string.Empty);
            progversion = ConvertObjToInt(strversion);

            return progversion;
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

        //private static double ConvertObjToDouble(object obj)
        //{
        //    bool isnumber = double.TryParse(obj.ToString(), out double id);
        //    return id;
        //}

        public static bool ConvertObjToBool(object obj)
        {
            bool isbool = bool.TryParse(obj.ToString(), out bool boolval);
            return boolval;
        }



        public static char ConvertObjToChar(object obj)
        {            
            bool ok = char.TryParse(obj.ToString(), out char charval);
            if (ok)
                return charval;
            else
                return ',';
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

        public static DateTime? ConvertDateToMonth(DateTime? dt)
        {
            if (dt == null) return null;
            return new DateTime(((DateTime)dt).Year, ((DateTime)dt).Month, 1);
        }

        public static string GetConstant(string constantname)
        {
            bool result = DictConstants.TryGetValue(constantname, out string s);
            return (result == true) ? s : constantname;
        }


        private static void ShowError(Exception e, [CallerMemberName] string operationtype = null)
        {
            IMessageBoxService msg = new MessageBoxService();
            msg.ShowMessage("Error during " + operationtype + " operation\n" + e.Message.ToString(), operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            msg = null;
        }

        public static void HandleSQLError(SqlException e, [CallerMemberName] string operationtype = null)
        {
            IMessageBoxService msg = new MessageBoxService();
            string msgtext = string.Empty;
            switch (e.Number)
            {
                case 4060:
                    msgtext = "Unable to connect to database";
                    break;

                case 2812:
                    msgtext = "Database is missing a stored procedure";
                    break;

                case -1:
                    msgtext = "Cannot locate database - Please check the VPN connection";
                    break;

                default:
                    msgtext = "SQL error " + e.Number.ToString();
                    break;
            }

            msg.ShowMessage(msgtext + "\nError occurred in " + operationtype + " function\n" + e.Message.ToString() + "\nProgram now shutting down", operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            msg = null;
            App.CloseProgram();

        }

        #endregion

    }
}