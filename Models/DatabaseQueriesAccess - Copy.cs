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
        const string spprefix = "PTR";

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
                                MaxProjectNameLength = ConvertObjToInt(or["MaxProjectNameLength"]),
                                EPRequired = ConvertObjToBool(or["EPRequired"]),
                                DefaultTrialStatusID = ConvertObjToInt(or["DefaultTrialStatusID"]),
                                StatusIDforTrials = ConvertObjToInt(or["StatusIDforTrials"]),
                                ValidateProducts = ConvertObjToBool(or["ValidateProducts"])
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
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
                    
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
                                PhaseGate = ConvertObjToBool(or["PhaseGate"]),
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
                    oc.CommandText = spprefix + "GetProjectEvaluationPlans";
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

        public static FullyObservableCollection<MonthlyActivityStatusModel> GetMonthlyProjectStatuses(int projectid)
        {
            FullyObservableCollection<MonthlyActivityStatusModel> projectstatuses = new FullyObservableCollection<MonthlyActivityStatusModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectActivities";
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
                                SalesRegionName = or["SalesRegionName"].ToString() ?? string.Empty,
                                CountryName = or["Country"].ToString() ?? string.Empty,
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
                    oc.CommandText = spprefix + "GetUserFromLogin";
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = loginname;
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
                            user.AllowEditCompletedCancelled = ConvertObjToBool(or["AllowEditCompletedCancelled"]);
                        }
                    }
                }
            }
            //catch (SqlException sqle)
            //{
            //   HandleSQLError(sqle);               
            //}
            catch //(Exception e)
            {
                // ShowError(e);
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
                    oc.CommandText = spprefix + "GetMarketSegments";
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
                    oc.CommandText = spprefix + "GetActivityStatusCodes";
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

        public static FullyObservableCollection<ApplicationCategoriesModel> GetApplicationCategories()
        {
            FullyObservableCollection<ApplicationCategoriesModel> applicationcategories = new FullyObservableCollection<ApplicationCategoriesModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetApplicationCategories";
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
                    oc.CommandText = spprefix + "GetNewBusinessCategories";
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
                    oc.CommandText = spprefix + "GetCountries";
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
                    oc.CommandText = spprefix + "GetSalesDivisions";
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
                    oc.CommandText = spprefix + "GetMilestone";
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
                    oc.CommandText = spprefix + "GetProjectMilestones";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    oc.Parameters.Add("@currentuserid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
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
                    oc.CommandText = spprefix + "GetCountCustomerProjects";
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
                    oc.CommandText = spprefix + "GetCountSalesRegionCustomers";
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
                    oc.CommandText = spprefix + "GetOperatingCompanies";
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
                    oc.CommandText = spprefix + "GetSalesRegions";
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

        //==================================================

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


        public static Collection<string> GetProductGroupNames()
        {
            Collection<string> prodgroupnames = new Collection<string>();
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
                    oc.CommandText = spprefix + "GetSMCodes";
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
                                GOM = new GenericObjModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Deleted = ConvertObjToBool(or["Deleted"])
                                },
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
        //                        GOM = new GenericObjModel()
        //                        {
        //                            ID = ConvertObjToInt(or["ID"]),
        //                            Name = or["Name"].ToString() ?? string.Empty,
        //                            Deleted = ConvertObjToBool(or["Deleted"])
        //                        },
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

        public static FullyObservableCollection<GenericObjModel> GetProjectTypes()
        {
            FullyObservableCollection<GenericObjModel> projecttypes = new FullyObservableCollection<GenericObjModel>();
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
                    oc.CommandText = spprefix + "GetIncompleteEPs";
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
                    oc.CommandText = spprefix + "GetMissingEPs";
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
                    oc.CommandText = spprefix + "GetProjectsRequiringCompletion";
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

        private static int ConvertVersionToInt(string version)
        {
            int progversion = 0;
            string strversion = version.Replace(".",string.Empty);
            progversion = ConvertObjToInt(strversion);
           
            return progversion;
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
            return trialstatuses;
        }

        public static FullyObservableCollection<GenericObjModel> GetSalesStatuses()
        {
            FullyObservableCollection<GenericObjModel> salesstatuses = new FullyObservableCollection<GenericObjModel>();
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
                            salesstatuses.Add(new GenericObjModel
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


        public static FullyObservableCollection<PhaseGateModel> GetProjectPhaseGates(int projectid)
        {
            FullyObservableCollection<PhaseGateModel> phasegates = new FullyObservableCollection<PhaseGateModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectPhaseGates";
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            phasegates.Add(new PhaseGateModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                ToolTip = or["ToolTip"].ToString() ?? string.Empty,
                                Description = or["Description"].ToString() ?? string.Empty,
                                DateSet = ConvertObjToDate(or["DateSet"]),
                                Colour = or["Colour"].ToString() ?? "#FFFFFF",
                                IsEnabled = ConvertObjToBool(or["IsEnabled"]),
                                IsFiller = ConvertObjToBool(or["IsFiller"]),
                                ProjectPhaseGateID = ConvertObjToInt(or["ProjectPhaseGateID"]),
                                ProjectID = projectid
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
            return phasegates;
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
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, Config.MaxProjectNameLength).Value = project.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = project.GOM.Description ?? string.Empty;
                    oc.Parameters.Add("@salesstatusid", SqlDbType.Int).Value = project.SalesStatusID;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = project.EstimatedAnnualSales;
                    oc.Parameters.Add("@ownerid", SqlDbType.Int).Value = project.OwnerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = project.SalesDivisionID;
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = project.ProjectStatusID;
                    oc.Parameters.Add("@marketsegmentid", SqlDbType.Int).Value = project.MarketSegmentID;
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
                    oc.Parameters.Add("@phasegate", SqlDbType.Bit).Value = project.PhaseGate;
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
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = customer.GOM.Name ?? string.Empty;
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
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = user.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = user.LoginName ?? string.Empty;
                    oc.Parameters.Add("@email", SqlDbType.NVarChar, descrfieldlen).Value = user.Email ?? string.Empty;
                    oc.Parameters.Add("@gin", SqlDbType.NVarChar, ginlen).Value = user.GIN ?? string.Empty;
                    oc.Parameters.Add("@administrator", SqlDbType.Bit).Value = user.Administrator;
                    oc.Parameters.Add("@salesdivisions", SqlDbType.NVarChar, multipleidslen).Value = user.SalesDivisions ?? string.Empty;
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
                    oc.CommandText = spprefix + "AddMilestone";
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
                    oc.CommandText = spprefix + "AddEvaluationPlan";
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

        public static int AddProjectPhaseGate(PhaseGateModel pg)
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
                    oc.CommandText = spprefix + "AddProjectPhaseGate";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@phasegateid", SqlDbType.Int).Value = pg.ID;
                    oc.Parameters.Add("@projectid", SqlDbType.Int).Value = pg.ProjectID;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = pg.Description;
                    oc.Parameters.Add("@dateset", SqlDbType.Date).Value = pg.DateSet;
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
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, Config.MaxProjectNameLength).Value = project.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = project.GOM.Description ?? string.Empty;
                    oc.Parameters.Add("@estimatedannualsales", SqlDbType.Decimal).Value = project.EstimatedAnnualSales;
                    oc.Parameters.Add("@ownerid", SqlDbType.Int).Value = project.OwnerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = project.SalesDivisionID;
                    oc.Parameters.Add("@projectstatusid", SqlDbType.Int).Value = project.ProjectStatusID;
                    oc.Parameters.Add("@marketsegmentid", SqlDbType.Int).Value = project.MarketSegmentID;
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
                    oc.Parameters.Add("@phasegate", SqlDbType.Bit).Value = project.PhaseGate;
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
                        throw;
                    }
                }
            }
        }

        public static FullyObservableCollection<MonthlyActivityStatusModel> UpdateMonthlyActivityStatus(FullyObservableCollection<MonthlyActivityStatusModel> activities)
        {
            int result = 0;
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
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = customer.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@number", SqlDbType.NVarChar, namefieldlen).Value = customer.Number ?? string.Empty;
                    oc.Parameters.Add("@location", SqlDbType.NVarChar, namefieldlen).Value = customer.Location ?? string.Empty;
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
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namefieldlen).Value = user.GOM.Name ?? string.Empty;
                    oc.Parameters.Add("@loginname", SqlDbType.NVarChar, namefieldlen).Value = user.LoginName ?? string.Empty;
                    oc.Parameters.Add("@email", SqlDbType.NVarChar, descrfieldlen).Value = user.Email ?? string.Empty;
                    oc.Parameters.Add("@gin", SqlDbType.NVarChar, ginlen).Value = user.GIN ?? string.Empty;
                    oc.Parameters.Add("@administrator", SqlDbType.Bit).Value = user.Administrator;
                    oc.Parameters.Add("@salesdivisions", SqlDbType.NVarChar, multipleidslen).Value = user.SalesDivisions ?? string.Empty;
                    oc.Parameters.Add("@administrationmnu", SqlDbType.NVarChar, multipleidslen).Value = user.AdministrationMnu ?? string.Empty;
                    oc.Parameters.Add("@projectsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ProjectsMnu ?? string.Empty;
                    oc.Parameters.Add("@reportsmnu", SqlDbType.NVarChar, multipleidslen).Value = user.ReportsMnu ?? string.Empty;
                    oc.Parameters.Add("@showothers", SqlDbType.Bit).Value = user.ShowOthers;                  
                    oc.Parameters.Add("@alloweditcompletedcancelled", SqlDbType.Bit).Value = user.AllowEditCompletedCancelled;
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
                
        public static void UpdateActualForecastedSales(int projectid, decimal estimatedannualsales, DateTime expecteddatefirstsales)
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
                    oc.CommandText = spprefix + "UpdateActualForecastedSales";
                    oc.Parameters.Add("@actualannualsales", SqlDbType.Decimal).Value = estimatedannualsales;
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = expecteddatefirstsales;                
                    oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
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

        public static void UpdateStatus10ActualForecastedSales(int projectid, decimal actualannualsales, DateTime expecteddataefirstsales)
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
                    oc.CommandText = spprefix + "UpdateStatus10ActualForecastedSales";
                    oc.Parameters.Add("@actualannualsales", SqlDbType.Decimal).Value = actualannualsales;
                    oc.Parameters.Add("@expecteddatefirstsales", SqlDbType.Date).Value = expecteddataefirstsales;                                       
                    oc.Parameters.Add("@completeddate", SqlDbType.Date).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);                    
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
                    oc.CommandText = spprefix + "UpdateForecastedSalesDate";
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
                        throw;
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
                    oc.CommandText = spprefix + "UpdateProjectStatus";
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
                    oc.CommandText = spprefix + "UpdateMilestone";
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

        public static void UpdateProjectPhaseGate(PhaseGateModel pg)
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
                    oc.CommandText = spprefix + "UpdateProjectPhaseGate";
                    oc.Parameters.Add("@description", SqlDbType.NVarChar, descrfieldlen).Value = pg.Description ?? string.Empty;
                    if (pg.DateSet == null)
                        oc.Parameters.Add("@dateset", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@dateset", SqlDbType.Date).Value = pg.DateSet;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = pg.ProjectPhaseGateID;
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
                    oc.CommandText = spprefix + "DeleteCustomer";
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
                        throw;
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

                            if (cm.ParameterType == "DateTime" && string.IsNullOrEmpty(cm.DefaultValue))
                                cm.DefaultValue = ConvertDateToMonth(DateTime.Now).ToString();

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
                            if (parameters[i].ParameterType == "String")
                                oc.Parameters.Add(parameters[i].Name, SqlDbType.NVarChar).Value = parameters[i].Value ?? string.Empty;
                            else
                                if (parameters[i].ParameterType == "DateTime")
                                oc.Parameters.Add(parameters[i].Name, SqlDbType.Date).Value = Convert.ToDateTime(parameters[i].Value);
                            else
                                if (parameters[i].ParameterType == "Int32")
                                oc.Parameters.Add(parameters[i].Name, SqlDbType.Int).Value = Convert.ToInt32(parameters[i].Value);
                            else
                                if (parameters[i].ParameterType == "Decimal")
                                oc.Parameters.Add(parameters[i].Name, SqlDbType.Decimal).Value = Convert.ToDecimal(parameters[i].Value);
                            else
                                if (parameters[i].ParameterType == "Boolean")
                                oc.Parameters.Add(parameters[i].Name, SqlDbType.Bit).Value = ConvertObjToBool(parameters[i].Value);
                            else
                                oc.Parameters.Add(parameters[i].Name, SqlDbType.NVarChar).Value = parameters[i].Value ?? string.Empty;
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
                                dc.ExtendedProperties.Add("FieldType", 99);//treat as Playbook comment field
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

        //for Version 3.1 and earlier
        //public static DataSet oldGetFilteredProjectsEstSales(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, 
        //    bool useUSD, DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm)
        //{
        //    DataTable dt = new DataTable
        //    {
        //        TableName = "TotalData"
        //    };
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        //get all status codes
        //        //get all months and build dt - add row for each status code, add column for status code
        //        //get sum of each status for each month
        //        //add sums to dt where 
        //        dt.Columns.Add(new DataColumn()
        //        {
        //            Caption = "Status",
        //            ColumnName = "Status",
        //            DataType = Type.GetType("System.Int32")
        //        });

        //        //add month columns for data
        //        AddNumericalReportMonths(ref dt, firstmonth, lastmonth);

        //        DataTable dtCount = new DataTable();
        //        dtCount = dt.Clone();
        //        dtCount.TableName = "ProjectCountData";

        //        DataRow dr;
        //        DataRow drCount;

        //        DateTime newmthdate;
        //        decimal salesvalue = 0;
        //        string colname = string.Empty;

        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetFilteredProjectsEstSales";

        //            foreach (ActivityStatusCodesModel code in ActivityStatusCodes)
        //            {
        //                if (code.GOM.ID != 0)//dont show any 0's
        //                {
        //                    dr = dt.NewRow();
        //                    dr["Status"] = code.GOM.ID;// code.GOM.ID.ToString();
        //                    drCount = dtCount.NewRow();
        //                    drCount["Status"] = code.GOM.ID;// code.GOM.ID.ToString();
        //                    oc.Parameters.Clear();
        //                    oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
        //                    oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
        //                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = code.GOM.ID;
        //                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //                    oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
        //                    oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
        //                    oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //                    oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //                    oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;                       
        //                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

        //                    using (SqlDataReader or = oc.ExecuteReader())
        //                    {
        //                        while (or.Read())
        //                        {
        //                            if (useUSD)
        //                                salesvalue = (decimal)(or["TotalStatusValueUS"]);
        //                            else
        //                                salesvalue = (decimal)(or["TotalStatusValue"]);

        //                            newmthdate = Convert.ToDateTime(or["StatusMonth"].ToString());
        //                            colname = newmthdate.ToString("MMM") + " " + newmthdate.Year.ToString();

        //                            if (salesvalue > 0)
        //                                dr[colname] = ConvertObjToInt(dr[colname]) + (int)salesvalue;

        //                            if ((int)(or["ProjectCount"]) > 0)
        //                                drCount[colname] = ConvertObjToInt(drCount[colname]) + (int)(or["ProjectCount"]);
        //                        }
        //                    }
        //                    dt.Rows.Add(dr);
        //                    dtCount.Rows.Add(drCount);
        //                }
        //            }
        //        }
        //        ds.Tables.Add(dt);
        //        ds.Tables.Add(dtCount);
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return ds;
        //}

        //for Version 3.2 and newer
        public static DataSet GetSalesPipelineReport(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString,
            bool useUSD, DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(SalesPipeline)
            };

            DataTable dtCount = new DataTable
            {
                TableName = GetConstant(SalesPipelineCount)
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
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

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
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

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

        //for Version 3.1 and earlier
        public static DataTable GetSalesPipelineReportToolTip(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, bool useUSD,
            DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(SalesPipelineTooltip)
            };

            try
            {
                //get all status codes
                //get all months and build dt - add row for each status code, add column for status code
                //get sum of each status for each month
                //add sums to dt where 
               // ProjectMonthRange monthrange = GetFirstLastMonths();

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
                        oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = useUSD;
                        oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
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

        //for Version 3.2 and newer
        //public static DataTable GetSalesPipelineReportToolTip(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, bool useUSD,
        //    DateTime firstmonth, DateTime lastmonth, bool kpm, bool showallkpm)
        //{
        //    DataTable dt = new DataTable
        //    {
        //        TableName = "StatusSummaryData"
        //    };

        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetSalesPipelineReportToolTip";
        //            oc.Parameters.Add("@firstmonth", SqlDbType.Date).Value = firstmonth;
        //            oc.Parameters.Add("@lastmonth", SqlDbType.Date).Value = lastmonth;
        //            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //            oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
        //            oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
        //            oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //            oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //            oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
        //            oc.Parameters.Add("@useusd", SqlDbType.Bit).Value = useUSD;
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

        //            using (SqlDataAdapter da = new SqlDataAdapter(oc))
        //            {
        //                da.Fill(dt);
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
        //    return dt;
        //}


        //for Version 3.1 and earlier
        //public static DataTable GetStatusReportFiltered(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, 
        //    string ProjectTypesSrchString, bool kpm, bool showallkpm)
        //{
        //    DataTable newdt = new DataTable
        //    {
        //        TableName = "StatusData"
        //    };
        //    try
        //    {                
        //        DataTable dt = new DataTable
        //        {
        //            TableName = "StatusData"
        //        };

        //        AddReportColumns(ref newdt, "StatusReport");

        //        SqlCommand oc1 = new SqlCommand
        //        {
        //            Connection = Conn,
        //            CommandType = CommandType.StoredProcedure,
        //            CommandText = "GetStatusReportFilteredProjects2"
        //        };
        //        oc1.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //        oc1.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
        //        oc1.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
        //        oc1.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //        oc1.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //        oc1.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;                
        //        oc1.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
        //        using (SqlDataAdapter da = new SqlDataAdapter(oc1))
        //        {
        //            da.Fill(dt);
        //        }
        //        oc1.Dispose();

        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetStatusReportFilteredActivities";

        //            DataRow dr1;
        //            DataRow dr;
        //            int elementctr = 0;
        //            string laststatusid = string.Empty;
        //            int intlaststatusid = 0;
        //            int statusctr = 0;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                dr1 = dt.Rows[i];
        //                oc.Parameters.Clear();
        //                oc.Parameters.Add("@projectid", SqlDbType.Int).Value = ConvertObjToInt(dr1["ProjectID"]);                 
        //                using (SqlDataReader or = oc.ExecuteReader())
        //                {
        //                    if (or.HasRows)
        //                    {
        //                        dr = newdt.NewRow();
        //                        dr["ProjectID"] = ConvertObjToInt(dr1["ProjectID"]);
        //                        dr["SalesDivision"] = dr1["SalesDivision"].ToString() ?? string.Empty;
        //                        dr["Customer"] = dr1["Customer"].ToString() ?? string.Empty;
        //                        dr["ProjectName"] = dr1["ProjectName"].ToString() ?? string.Empty;
        //                        dr["EstimatedAnnualSales"] = ConvertObjToDecimal(dr1["EstimatedAnnualSales"]);
        //                        dr["ActivatedDate"] = ConvertObjToDate(dr1["ActivatedDate"]);
        //                        dr["CultureCode"] = dr1["CultureCode"].ToString() ?? string.Empty;
        //                        dr["ProjectTypeColour"] = dr1["ProjectTypeColour"].ToString() ?? string.Empty;
        //                        elementctr = 0;
        //                        laststatusid = string.Empty;
        //                        intlaststatusid = 0;
        //                        statusctr = 0;
        //                        DateTime laststatusmonth = new DateTime();
        //                        while (or.Read())
        //                        {
        //                            if (elementctr == 0)
        //                            {
        //                                intlaststatusid = ConvertObjToInt(or["StatusID"]);
        //                                laststatusmonth = Convert.ToDateTime(or["StatusMonth"].ToString());
        //                                dr["TrialStatus"] = or["TrialStatus"].ToString() ?? string.Empty; 
        //                                dr["Colour"] = or["Colour"].ToString() ?? string.Empty;
        //                            }
        //                            if (ConvertObjToInt(or["StatusID"]) == intlaststatusid)
        //                                statusctr++;
        //                            else
        //                                break;
        //                            elementctr++;
        //                        }
        //                        //get number of months
        //                        if (elementctr > 0)
        //                        {
        //                            dr["Status"] = GetActivityStatusFromID(intlaststatusid);
        //                            dr["MonthsAtStatus"] = statusctr;
        //                            dr["FirstMonthAtStatus"] = laststatusmonth.AddMonths(-statusctr + 1).ToString("MMM-yyyy");

        //                        }
        //                        newdt.Rows.Add(dr);
        //                    }
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
        //    return newdt;
        //}

        //for Version 3.2 and newer
        public static DataTable GetStatusReportFiltered(string CountriesSrchString)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(StatusReport)
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjectStatusReport";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
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

        //for Version 3.1 and earlier
        //public static DataTable GetProjects(string CountriesSrchString,  string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, 
        //    bool kpm, bool showallkpm)
        //{
        //    DataTable dt = new DataTable
        //    {
        //        TableName = "ProjectsList"
        //    };
        //    try
        //    {
        //        AddReportColumns(ref dt, "ProjectsList");
        //        DataRow dr;
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetProjects";
        //            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //            oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
        //            oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
        //            oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //            oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //            oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;                
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    dr = dt.NewRow();
        //                    dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
        //                    dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
        //                    dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
        //                    dr["ProjectName"] = or["Name"].ToString() ?? string.Empty;
        //                    dr["Products"] = or["Products"].ToString() ?? string.Empty;
        //                    dr["Resources"] = or["Resources"].ToString() ?? string.Empty;
        //                    dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
        //                    dr["ProjectStatus"] = or["ProjectStatus"].ToString() ?? string.Empty;
        //                    dr["ProjectType"] = or["ProjectType"].ToString() ?? string.Empty;
        //                    dr["EstimatedAnnualSales"] = ConvertObjToDecimal(or["EstimatedAnnualSales"]);
        //                    dr["OwnerID"] = ConvertObjToInt(or["OwnerID"]);
        //                    dr["CultureCode"] = or["CultureCode"].ToString() ?? string.Empty;
        //                    dr["ProjectTypeColour"] = or["ProjectTypeColour"].ToString() ?? string.Empty;
        //                    dt.Rows.Add(dr);
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
        //    return dt;
        //}

        //for Version 3.2 and newer
        public static DataTable GetProjects(string CountriesSrchString)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(ProjectReport)
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetProjects";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
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

        //for Version 3.1 and earlier
        //public static FullyObservableCollection<MonthlyActivityStatusModel> GetMonthlyProjectData(int projectid)
        //{
        //    ProjectMonthRange monthrange = GetFirstLastMonthsForProject(projectid);
        //    int startmonth;

        //    if (monthrange.StartDate == null)
        //    {
        //        DateTime? activateddate = GetProjectActivatedDate(projectid);
        //        if (activateddate != null)
        //            startmonth = ConvertDateToMonthInt(activateddate);
        //        else
        //            startmonth = ConvertDateToMonthInt(DateTime.Now.AddMonths(-1));
        //    }
        //    else
        //        startmonth = ConvertDateToMonthInt(((DateTime)monthrange.StartDate));

        //    //_lastmonth;
        //    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //    int lastmonth = ConvertDateToMonthInt(dt);

        //    //create collection of MonthlyActivityStatus
        //    FullyObservableCollection<MonthlyActivityStatusModel> monthlyactivity = new FullyObservableCollection<MonthlyActivityStatusModel>();
        //    FullyObservableCollection<MonthlyActivityStatusModel> selectedprojectactivity = GetMonthlyProjectStatuses(projectid);
        //    MonthlyActivityStatusModel newmonthactivity;

        //    for (int i = lastmonth; i >= startmonth; i--)
        //    {
        //        newmonthactivity = new MonthlyActivityStatusModel
        //        {
        //            ID = 0,
        //            ProjectID = projectid,
        //            StatusID = 0,
        //            TrialStatusID = 0,// (int)TrialStatusType.NoTrial, //could be set to 0 if no NoTrial code
        //            Comments = string.Empty,
        //            StatusMonth = ConvertMonthIntToDateTime(i),
        //            ExpectedDateFirstSales = null,
        //            IsDirty = false
        //        };
        //        var query = selectedprojectactivity.FirstOrDefault(a => ConvertDateToMonthInt(a.StatusMonth) == i);
        //        if (query != null) 
        //        {
        //            newmonthactivity.ID = query.ID;
        //            newmonthactivity.StatusID = query.StatusID;
        //            newmonthactivity.Comments = query.Comments;
        //            newmonthactivity.TrialStatusID = query.TrialStatusID;
        //            newmonthactivity.ShowTrial = RequiredTrialStatuses.Contains(query.StatusID);
        //            newmonthactivity.ExpectedDateFirstSales = query.ExpectedDateFirstSales;
        //        }
        //        monthlyactivity.Add(newmonthactivity);
        //    }
        //    return monthlyactivity;
        //}

        //for Version 3.2 and newer
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

        //for Version 3.2 and newer
        public static DataTable GetMonthlyProjectDataReport(int projectid)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(ProjectReportActivities)
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

        //for Version 3.1 and earlier
        //public static DataTable GetEvaluationPlansReport(string CountriesSrchString, string SalesDivisionSrchString, string ProjectStatusTypesSrchString, string ProjectTypesSrchString, 
        //    bool kpm, bool showallkpm)
        //{            
        //    DataTable dt = new DataTable
        //        {
        //            TableName = "EPReport"
        //        };
        //    try
        //    {                
        //        AddReportColumns(ref dt, "EPReport");
        //        DataRow dr;

        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetEvaluationPlans";
        //            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //            oc.Parameters.Add("@salesdivisionids", SqlDbType.NVarChar, multipleidslen).Value = SalesDivisionSrchString;
        //            oc.Parameters.Add("@statustypesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectStatusTypesSrchString;
        //            oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //            oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //            oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    dr = dt.NewRow();
        //                    dr["ID"] = ConvertObjToInt(or["ID"]);
        //                    dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
        //                    dr["ProjectName"] = or["ProjectName"].ToString() ?? string.Empty;
        //                    if (!string.IsNullOrEmpty(or["ActivatedDate"].ToString()))
        //                        dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
        //                    dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
        //                    dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
        //                    dr["ProjectTypeColour"] = or["ProjectTypeColour"].ToString() ?? string.Empty;
        //                    if (!string.IsNullOrEmpty(or["Created"].ToString()))
        //                        dr["CreatedDate"] = ConvertObjToDate(or["Created"]);
        //                    if (!string.IsNullOrEmpty(or["Discussed"].ToString()))
        //                        dr["DiscussedDate"] = ConvertObjToDate(or["Discussed"]);
        //                    dr["Objectives"] = or["Objectives"].ToString() ?? string.Empty;
        //                    dr["Strategy"] = or["Strategy"].ToString() ?? string.Empty;
        //                    dr["Title"] = or["Title"].ToString() ?? string.Empty;
        //                    dt.Rows.Add(dr);
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
        //    return dt;
        //}

        //for Version 3.2 and newer
        public static DataTable GetEvaluationPlansReport(string CountriesSrchString)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(EvaluationPlanReport)
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetEvaluationPlans";
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
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
                dc.DataType = System.Type.GetType("System.String");
                dc.ExtendedProperties["Alignment"] = "Left";
                dc.ExtendedProperties["FieldType"] = 99;
                dt.Columns.Add(dc);
            }
        }

        //for Version 3.1 and earlier
        //private static void AddNumericalReportMonths(ref DataTable dt, DateTime firstmonth, DateTime lastmonth)
        //{
        //    DataColumn dc;
        //    int intstartmonth = ConvertDateToMonthInt(firstmonth);
        //    int intlastmonth = ConvertDateToMonthInt(lastmonth);

        //    for (int i = intstartmonth; i <= intlastmonth; i++)
        //    {
        //        dc = new DataColumn();
        //        DateTime newmth = ConvertMonthIntToDateTime(i);
        //        dc.Caption = newmth.ToString("MMM") + " " + newmth.Year.ToString();
        //        dc.ColumnName = newmth.ToString("MMM") + " " + newmth.Year.ToString();
        //        dc.DataType = Type.GetType("System.Decimal");
        //        dc.ExtendedProperties["Alignment"] = "Right";
        //        dc.ExtendedProperties["FieldType"] = 99;
        //        dt.Columns.Add(dc);
        //    }
        //}

        //for Version 3.1 and earlier
        //public static DataTable GetNewBusinessReport(string CountriesSrchString, int SelectedDivisionID, string ProjectTypesSrchString, DateTime startmonth, DateTime firstmonth, DateTime lastmonth, 
        //    bool kpm, bool showallkpm)
        //{
        //    string report = "NewBusiness";
        //    DataTable dt = new DataTable
        //    {
        //        TableName = NewBusiness
        //    };
        //    try
        //    {
        //        //add report columns for data
        //        AddReportColumns(ref dt, report);

        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "PBNewBusiness";
        //            oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
        //            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //            oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = SelectedDivisionID;
        //            oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //            oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //            oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

        //            DataRow dr;
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    dr = dt.NewRow();
        //                    dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
        //                    dr["SalesDivision"] = or["SalesDivision"].ToString() ?? string.Empty;
        //                    dr["MarketSegment"] = or["MarketSegment"].ToString() ?? string.Empty;
        //                    dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
        //                    dr["CustomerNumber"] = or["CustomerNumber"].ToString() ?? string.Empty;
        //                    dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
        //                    dr["Application"] = or["Application"].ToString() ?? string.Empty;
        //                    dr["Products"] = or["Products"].ToString() ?? string.Empty;
        //                    dr["EstimatedAnnualSales"] = ConvertObjToDecimal(or["EstimatedAnnualSales"]);
        //                    dr["EstimatedAnnualSalesUSD"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);
        //                    dr["EstimatedAnnualMPC"] = ConvertObjToDecimal(or["EstimatedAnnualMPC"]);
        //                    dr["EstimatedAnnualMPCUSD"] = ConvertObjToDecimal(or["EstimatedAnnualMPCUSD"]);
        //                    if (!string.IsNullOrEmpty(or["ExpectedDateFirstSales"].ToString()))
        //                        dr["ExpectedDateFirstSales"] = ConvertObjToDate(or["ExpectedDateFirstSales"]);
        //                    dr["NewBusinessCategory"] = or["NewBusinessCategory"].ToString() ?? string.Empty;
        //                    dr["PhaseGate"] = or["PhaseGate"].ToString() ?? string.Empty;
        //                    dr["DifferentiatedTechnology"] = ConvertObjToBool(or["DifferentiatedTechnology"]);
        //                    dt.Rows.Add(dr);
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
        //    return dt;
        //}


        //for Version 3.2 and newer
        public static DataTable GetNewBusinessReport(string CountriesSrchString, DateTime startmonth)
        {
            DataTable dt = new DataTable
            {
                TableName = GetConstant(NewBusinessXLTab)
            };
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = spprefix + "GetNewBusinessReport";
                    oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
                    oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;                              
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, GetConstant(NewBusinessReport));
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

        //for Version 3.1 and earlier
        //public static DataTable GetAllSalesFunnelReport(string CountriesSrchString, int SelectedDivisionID, string ProjectTypesSrchString, DateTime startmonth, DateTime firstmonth, DateTime lastmonth, 
        //    bool showMiscColumns, bool kpm, bool showallkpm)
        //{          
        //    string report = "SalesFunnel";
        //    DataTable dt = new DataTable
        //    {
        //        TableName = SalesFunnel// "A-Sls Fnl";
        //    };
        //    try
        //    {
        //        //add report columns for data
        //        AddReportColumns(ref dt, report);

        //        //add comments column data
        //        int intstartmonth = ConvertDateToMonthInt(firstmonth);
        //        int intlastmonth = ConvertDateToMonthInt(lastmonth);

        //        //add month columns for comments
        //        AddReportMonths(ref dt, firstmonth, lastmonth);

        //        Collection<MonthlyActivityStatusModel> projectmonthscoll = new Collection<MonthlyActivityStatusModel>();

        //        using (SqlCommand ocdetails = new SqlCommand())
        //        {
        //            ocdetails.Connection = Conn;
        //            ocdetails.CommandType = CommandType.StoredProcedure;
        //            ocdetails.CommandText = "PBSalesFunnelDetails";
        //            ocdetails.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
        //            ocdetails.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //            ocdetails.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = SelectedDivisionID;
        //            ocdetails.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //            ocdetails.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;
        //            using (SqlDataReader ordetails = ocdetails.ExecuteReader())
        //            {
        //                while (ordetails.Read())
        //                {
        //                    projectmonthscoll.Add(new MonthlyActivityStatusModel()
        //                    {
        //                        StatusID = ConvertObjToInt(ordetails["StatusID"]),
        //                        StatusMonth = ConvertObjToDate(ordetails["StatusMonth"]),
        //                        Comments = ordetails["Comments"].ToString() ?? string.Empty,
        //                        ProjectID = ConvertObjToInt(ordetails["ProjectID"]),
        //                        TrialStatusID = ConvertObjToInt(ordetails["TrialStatusID"])
        //                    });
        //                }
        //            }
        //        }

        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "PBSalesFunnel"; //get all sales funnel projects
        //            oc.Parameters.Add("@startmonth", SqlDbType.Date).Value = startmonth;
        //            oc.Parameters.Add("@countryids", SqlDbType.NVarChar, multipleidslen).Value = CountriesSrchString;
        //            oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = SelectedDivisionID;
        //            oc.Parameters.Add("@typesids", SqlDbType.NVarChar, multipleidslen).Value = ProjectTypesSrchString;
        //            oc.Parameters.Add("@kpm", SqlDbType.Bit).Value = kpm;
        //            oc.Parameters.Add("@showallkpm", SqlDbType.Bit).Value = showallkpm;
        //            oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

        //            DateTime newmthdate;
        //            string colname = string.Empty;
        //            DataRow dr;
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    dr = dt.NewRow();
        //                    dr["SalesDivision"] = or["SalesDivision"].ToString() ?? string.Empty;
        //                    dr["MarketSegment"] = or["MarketSegment"].ToString() ?? string.Empty;
        //                    dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
        //                    dr["ProjectName"] = or["ProjectName"].ToString() ?? string.Empty;
        //                    dr["Resources"] = or["Resources"].ToString() ?? string.Empty;
        //                    dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
        //                    dr["Application"] = or["Application"].ToString() ?? string.Empty;
        //                    dr["Products"] = or["Products"].ToString() ?? string.Empty;
        //                    if (!string.IsNullOrEmpty(or["ActivatedDate"].ToString()))
        //                        dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
        //                    dr["ProbabilityOfSuccess"] = ConvertObjToDecimal(or["ProbabilityOfSuccess"]);
        //                    dr["EstimatedAnnualSalesUSD"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);
        //                    dr["SMCode"] = or["SMCode"].ToString() ?? string.Empty;
        //                    dr["OPCOOpenField"] = ConvertObjToDecimal(or["OPCOOpenField"]);
        //                    if (!string.IsNullOrEmpty(or["ExpectedDateFirstSales"].ToString()))
        //                        dr["ExpectedDateFirstSales"] = ConvertObjToDate(or["ExpectedDateFirstSales"]);
        //                    dr["ProjectTypeColour"] = or["ProjectTypeColour"].ToString() ?? string.Empty;
        //                    dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
        //                    dr["GIN"] = or["GIN"].ToString() ?? string.Empty;
        //                    dr["CustomerNumber"] = or["CustomerNumber"] ?? string.Empty;
        //                    dr["KPM"] = ConvertObjToBool(or["KPM"]);
        //                    dr["NewBusinessCategory"] = or["NewBusinessCategory"].ToString() ?? string.Empty;
        //                    dr["PhaseGate"] = or["PhaseGate"].ToString() ?? string.Empty;
        //                    dr["DifferentiatedTechnology"] = ConvertObjToBool(or["DifferentiatedTechnology"]);
        //                    dr["ProjectType"] = or["ProjectType"].ToString() ?? string.Empty;
        //                    if (ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]) > 0)
        //                        dr["GMPercent"] = ConvertObjToDecimal(or["GMUSD"]) / ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);

        //                    //new 8/11/2019
        //                    dr["EOYSales"] = ConvertObjToDecimal(or["EOYSales"]);
        //                    dr["EOYMPC"] = ConvertObjToDecimal(or["EOYMPC"]);

        //                    var projectmonths = from p in projectmonthscoll
        //                                        where p.ProjectID == Convert.ToInt32(or["ProjectID"])
        //                                        orderby p.StatusMonth descending
        //                                        select p;

        //                    if (projectmonths != null)
        //                    {
        //                        foreach (var pr in projectmonths)
        //                        {
        //                            //if month matches column then add the comment to the 
        //                            newmthdate = (DateTime)pr.StatusMonth;
        //                            colname = newmthdate.ToString("MMM") + " " + newmthdate.Year.ToString();
        //                            if (ConvertDateToMonthInt(newmthdate) >= intstartmonth && ConvertDateToMonthInt(newmthdate) <= intlastmonth)
        //                                dr[colname] = pr.Comments;
        //                        }
        //                    }

        //                    //new 8/11/2019
        //                    dr["SalesFunnelStage"] = or["SalesFunnelStage"].ToString() ?? string.Empty;
        //                    dr["Colour"] = or["LastStatusColour"].ToString() ?? string.Empty; ;
        //                    dr["ProjectStatus"] = or["ProjectStatus"].ToString() ?? string.Empty;                            

        //                    dt.Rows.Add(dr);
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
                    dc.ExtendedProperties.Add("FieldType", 99); //treat as Playbook comment field
                }
            }
        }

        //for Version 3.2 and newer
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
                    oc.Parameters.Add("@userid", SqlDbType.Int).Value = CurrentUser.GOM.ID;

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
                    {
                        da.Fill(dt);
                    }

                    ProcessReportColumns(ref dt, GetConstant(SalesFunnelReport));
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
                    oc.CommandText = spprefix + "GetReportFields";
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

        //for Version 3.1 and earlier
        //public static DataSet oldGetProjectReport(int projectid)
        //{
        //    DataSet ds = new DataSet();

        //    string tablename = "Project Report";
        //    DataTable dt = new DataTable
        //    {
        //        TableName = tablename
        //    };
        //    try
        //    {
        //        //add report columns for data
        //        AddReportColumns(ref dt, "ProjectReport");

        //        DataRow dr;
        //        dr = dt.NewRow();
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = "GetProjectReport";
        //            oc.Parameters.AddWithValue("@projectid", projectid);
        //            using (SqlDataReader or = oc.ExecuteReader())
        //            {
        //                while (or.Read())
        //                {
        //                    dr["ProjectID"] = ConvertObjToInt(or["ProjectID"]);
        //                    dr["Customer"] = or["Customer"].ToString() ?? string.Empty;
        //                    dr["Country"] = or["Country"].ToString() ?? string.Empty;
        //                    dr["SalesDivision"] = or["SalesDivision"].ToString() ?? string.Empty;                           
        //                    dr["ProjectStatus"] = or["ProjectStatus"].ToString()?? string.Empty;
        //                    dr["MarketSegment"] = or["MarketSegment"].ToString() ?? string.Empty;
        //                    dr["Products"] = or["Products"].ToString() ?? string.Empty;
        //                    dr["Resources"] = or["Resources"].ToString() ?? string.Empty;
        //                    dr["ProjectName"] = or["ProjectName"].ToString() ?? string.Empty;
        //                    dr["ExpectedDateFirstSales"] = ConvertObjToDate(or["ExpectedDateFirstSales"]);
        //                    dr["UserName"] = or["UserName"].ToString() ?? string.Empty;
        //                    dr["Description"] = or["Description"].ToString() ?? string.Empty;
        //                    dr["ActivatedDate"] = ConvertObjToDate(or["ActivatedDate"]);
        //                    dr["EstimatedAnnualSalesUSD"] = ConvertObjToDecimal(or["EstimatedAnnualSalesUSD"]);
        //                    dr["TargetedVolume"] = ConvertObjToInt(or["TargetedVolume"]);
        //                    dr["ProjectType"] = or["ProjectType"].ToString() ?? string.Empty;
        //                    dr["KPM"] = or["KPM"].ToString() ?? string.Empty;
        //                    dr["Application"] = or["Application"].ToString() ?? string.Empty;
        //                    dr["GMUSD"] = or["GMUSD"].ToString() ?? string.Empty;
        //                    dr["EstimatedAnnualMPCUSD"] = ConvertObjToDecimal(or["EstimatedAnnualMPCUSD"]);
        //                    dr["DifferentiatedTechnology"] = or["DifferentiatedTechnology"].ToString() ?? string.Empty;
        //                    dr["ProbabilityOfSuccess"] = ConvertObjToDecimal(or["ProbabilityOfSuccess"]);
        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //        }

        //        string activitiestablename = "Activities";
        //        DataTable dtactivities = new DataTable
        //        {
        //            TableName = activitiestablename
        //        };
        //        DataColumn dc;
        //        dc = new DataColumn
        //        {
        //            Caption = "Month",
        //            ColumnName = "StatusMonth",
        //            DataType = Type.GetType("System.DateTime")                  
        //        };
        //        dc.ExtendedProperties["Format"] = "MMM-yyyy";
        //        dc.ExtendedProperties["Alignment"] = "Center";
        //        dc.ExtendedProperties["FieldType"] = 0;                

        //        dtactivities.Columns.Add(dc);
        //        dc = new DataColumn
        //        {
        //            Caption = "Status",
        //            ColumnName = "Status",
        //            DataType = Type.GetType("System.String")
        //        };
        //        dc.ExtendedProperties["Format"] = "0";
        //        dc.ExtendedProperties["Alignment"] = "Center";
        //        dc.ExtendedProperties["FieldType"] = 0;

        //        dtactivities.Columns.Add(dc);
        //        dc = new DataColumn
        //        {
        //            Caption = "Comments",
        //            ColumnName = "Comments",
        //            DataType = Type.GetType("System.String")
        //        };              
        //        dc.ExtendedProperties["Alignment"] = "Left";
        //        dc.ExtendedProperties["FieldType"] = 0;

        //        dtactivities.Columns.Add(dc);
        //        dc = new DataColumn
        //        {
        //            Caption = "Trial Status",
        //            ColumnName = "TrialStatus",
        //            DataType = Type.GetType("System.String")
        //        };              
        //        dc.ExtendedProperties["Alignment"] = "Center";
        //        dc.ExtendedProperties["FieldType"] = 0;

        //        dtactivities.Columns.Add(dc);
        //        DataRow dract;                                                               

        //        FullyObservableCollection<MonthlyActivityStatusModel> monthlyactivities =  GetMonthlyProjectData(projectid);                    
        //        for(int i=0; i<monthlyactivities.Count;i++)
        //        {
        //            dract = dtactivities.NewRow();
        //            dract["StatusMonth"] = monthlyactivities[i].StatusMonth;
        //            dract["Status"] = GetActivityDescriptionFromID(monthlyactivities[i].StatusID);
        //            dract["Comments"] = monthlyactivities[i].Comments;
        //            dract["TrialStatus"] = GetTrialStatusFromID(monthlyactivities[i].TrialStatusID);
        //            dtactivities.Rows.Add(dract);
        //        }                                        

        //        ds.Tables.Add(dt);
        //        ds.Tables.Add(dtactivities);
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return ds;
        //}

        //for Version 3.2 and newer
        public static DataSet GetProjectReport(int projectid)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable
            {
                TableName = GetConstant(SingleProjectReport)
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
                    TableName = GetConstant(SingleProjectReportActivities)
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