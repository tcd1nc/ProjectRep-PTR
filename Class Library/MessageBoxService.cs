using System;
using System.Windows;

namespace PTR
{
    public interface IMessageBoxService
    {
        GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons);
        bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType);
        void Show(string message, string caption);
        string OpenFileDlg(string title, bool validatenames, bool multiselect, string filter, Window owner);
        string SaveFileDlg(string title, string filter, string suggestedfilename, Window owner);
        GenericMessageBoxResult ShowMessage(string text, string caption, GenericMessageBoxButton messagebutton, GenericMessageBoxIcon messageicon);
        bool OpenProjectDlg(Window owner, int id);
        bool OpenProjectCommentsDlg(Window owner, int id);
        bool OpenDialog(string name);
        object[] CompletedProjectDialog(Window owner, decimal estimatedannualsales, DateTime? expecteddatefirstsales, DateTime statusmonth);
        DateTime? ConfirmExpectedSalesDateDialog(Window owner, DateTime? ExpectedDateFirstSales, DateTime? statusmonth);
        bool EvaluationPlanDialog(Window owner, int id, int projectid);
        bool MilestoneDialog(Window owner, int id, int projectid);          
    }

    public enum GenericMessageBoxButton
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum GenericMessageBoxResult
    {
        OK,
        Cancel,
        No,
        Yes
    }

    public enum GenericMessageBoxIcon
    {
        Asterisk,
        Error,
        Exclamation,
        Hand,
        Information,
        None,
        Question,
        Stop,
        Warning
    }
   
    public class MessageBoxService : IMessageBoxService
    {      
        public bool OpenProjectDlg(Window owner, int projectid)
        {
            if (owner == null)
                owner = Application.Current.Windows[0];
            bool result;
            if(projectid == 0)
            {
                Views.ProjectView dlg = new Views.ProjectView
                {
                    Owner = owner
                };
                dlg.ShowDialog();               
                result = (bool)dlg.DialogResult;
            }
            else
            {
                Views.ProjectView dlg = new Views.ProjectView(projectid)
                {
                    Owner = owner
                };
                dlg.ShowDialog();
                result = (bool)dlg.DialogResult;
            }
            return result;                      
        }

        public bool OpenProjectCommentsDlg(Window owner, int projectid)
        {
            bool result = false;
            if (owner == null)
                owner = Application.Current.Windows[0];
            if (projectid > 0)
            {
                Views.ProjectCommentsView dlg = new Views.ProjectCommentsView(projectid)
                {
                    Owner = owner
                };
                dlg.ShowDialog();
                result = (bool)dlg.DialogResult;
            }
            return result;
        }
               
        public bool OpenDialog(string name)
        {
            Window owner;
            owner = Application.Current.Windows[0];
          
            switch (name)
            {                
                case "SalesFunnelReport":
                    Views.SalesFunnelReportView rv = new Views.SalesFunnelReportView
                    {
                        Owner = owner
                    };
                    rv.ShowDialog();
                    return (bool)rv.DialogResult;
                     
                case "StatusReport":
                    Views.StatusReportView srv = new Views.StatusReportView
                    {
                        Owner = owner
                    };
                    srv.ShowDialog();
                    return (bool)srv.DialogResult;                    
                
                case "ProjectReport":
                    Views.ProjectReportView prv = new Views.ProjectReportView
                    {
                        Owner = owner
                    };
                    prv.ShowDialog();
                    return (bool)prv.DialogResult;

                case "CustomReports":
                    Views.CustomReportsView cr = new Views.CustomReportsView
                    {
                        Owner = owner
                    };
                    cr.ShowDialog();
                    return false;

                case "EditUser":
                    Views.AssociatesView eav = new Views.AssociatesView
                    {
                        Owner = owner
                    };
                    eav.ShowDialog();
                    return (bool)eav.DialogResult;                                        
                                    
                case "EditCustomer":
                    Views.EditCustomerView ecv = new Views.EditCustomerView
                    {
                        Owner = owner
                    };
                    ecv.ShowDialog();
                    return (bool)ecv.DialogResult;

                case "EditBU":
                    Views.EditBUView ebu = new Views.EditBUView
                    {
                        Owner = owner
                    };
                    ebu.ShowDialog();
                    return (bool)ebu.DialogResult;

                case "EditCountry":
                    Views.EditCountryView ecountryv = new Views.EditCountryView
                    {
                        Owner = owner
                    };
                    ecountryv.ShowDialog();
                    return (bool)ecountryv.DialogResult;
                     
                case "ProjectMasterReport":
                    Views.PlaybookView playbookvw = new Views.PlaybookView
                    {
                        Owner = owner
                    };
                    playbookvw.ShowDialog();
                    return (bool)playbookvw.DialogResult;                                                                            
                     
                case "ExchangeRateView":
                    Views.ExchangeRateView exratevw = new Views.ExchangeRateView
                    {
                        Owner = owner
                    };
                    exratevw.ShowDialog();
                    return (bool)exratevw.DialogResult;
                     
                case "EvaluationPlans":
                    Views.EvaluationPlansView evplansvw = new Views.EvaluationPlansView
                    {
                        Owner = owner
                    };
                    evplansvw.ShowDialog();
                    return (bool)evplansvw.DialogResult;
                    
                case "AboutView":
                    Views.AboutView aboutvw = new Views.AboutView
                    {
                        Owner = owner
                    };
                    aboutvw.ShowDialog();
                    return (bool)aboutvw.DialogResult;                     

                case "MaintenanceDue":
                    Views.MaintenanceView maintvw = new Views.MaintenanceView
                    {
                        Owner = owner
                    };
                    maintvw.ShowDialog();
                    return (bool)maintvw.DialogResult;

                case "SalesRegionView":
                    Views.SalesRegionView salesregvw = new Views.SalesRegionView
                    {
                        Owner = owner
                    };
                    salesregvw.ShowDialog();
                    return (bool)salesregvw.DialogResult;

                case "ProjectSalesStagesView":
                    Views.ProjectSalesStagesView projsalesstagesvw = new Views.ProjectSalesStagesView
                    {
                        Owner = owner
                    };
                    projsalesstagesvw.ShowDialog();
                    return (bool)projsalesstagesvw.DialogResult;

                case "SetupView":
                    Views.SetupView setupvw = new Views.SetupView
                    {
                        Owner = owner
                    };
                    setupvw.ShowDialog();
                    return (bool)setupvw.DialogResult;

                case "ProductNamesView":
                    Views.ProductNameView prnamevw = new Views.ProductNameView
                    {
                        Owner = owner
                    };
                    prnamevw.ShowDialog();
                    return (bool)prnamevw.DialogResult;

                case "ProjectTypesView":
                    Views.ProjectTypesView prtypesvw = new Views.ProjectTypesView
                    {
                        Owner = owner
                    };
                    prtypesvw.ShowDialog();
                    return (bool)prtypesvw.DialogResult;

                case "ApplicationsView":
                    Views.ApplicationsView appcatsvw = new Views.ApplicationsView
                    {
                        Owner = owner
                    };
                    appcatsvw.ShowDialog();
                    return (bool)appcatsvw.DialogResult;
                    
                case "SMCodesView":
                    Views.SMCodesView smcodesvw = new Views.SMCodesView
                    {
                        Owner = owner
                    };
                    smcodesvw.ShowDialog();
                    return (bool)smcodesvw.DialogResult;
                    
                case "TrialStatusesView":
                    Views.TrialStatusesView trialstatvw = new Views.TrialStatusesView
                    {
                        Owner = owner
                    };
                    trialstatvw.ShowDialog();
                    return (bool)trialstatvw.DialogResult;

                case "NewBusinessCategoriesView":
                    Views.NewBusinessCategoryView newbizvw = new Views.NewBusinessCategoryView
                    {
                        Owner = owner
                    };
                    newbizvw.ShowDialog();
                    return (bool)newbizvw.DialogResult;

                    
                case "IndustrySegmentsApplicationsView":
                    Views.IndustrySegmentsApplicationsView mktsegsappvw = new Views.IndustrySegmentsApplicationsView
                    {
                        Owner = owner
                    };
                    mktsegsappvw.ShowDialog();
                    return (bool)mktsegsappvw.DialogResult;

                    
                case "IndustrySegmentsView":
                    Views.IndustrySegmentsView mktsegsvw = new Views.IndustrySegmentsView
                    {
                        Owner = owner
                    };
                    mktsegsvw.ShowDialog();
                    return (bool)mktsegsvw.DialogResult;
                    
                case "IncompleteProjectReasonsView":
                    Views.IncompleteProjectReasonsView incompprjrreasonsvw = new Views.IncompleteProjectReasonsView
                    {
                        Owner = owner
                    };
                    incompprjrreasonsvw.ShowDialog();
                    return (bool)incompprjrreasonsvw.DialogResult;
                    
                case "ReportFieldsView":
                    Views.ReportFieldsView reportfldsvw = new Views.ReportFieldsView
                    {
                        Owner = owner
                    };
                    reportfldsvw.ShowDialog();
                    return (bool)reportfldsvw.DialogResult;


                case "MiscellaneousDataView":
                    Views.MiscellaneousDataView miscdatavw = new Views.MiscellaneousDataView
                    {
                        Owner = owner
                    };
                    miscdatavw.ShowDialog();
                    return (bool)miscdatavw.DialogResult;

                default:
                    return false;
            }           
        }
               
        public object[] CompletedProjectDialog(Window owner, decimal estimatedannualsales, DateTime? expecteddatefirstsales, DateTime statusmonth)
        { 
            if(owner == null)
                owner = Application.Current.Windows[0];
            Views.CompletedProjectDialogView compproject = new Views.CompletedProjectDialogView(estimatedannualsales, expecteddatefirstsales, statusmonth)
            {
                Owner = owner
            };
            compproject.ShowDialog();            
            return (object[])(compproject.Tag);           
        }
        
        public DateTime? ConfirmExpectedSalesDateDialog(Window owner, DateTime? ExpectedDateFirstSales, DateTime? statusmonth)
        {
            if(owner ==null)
                owner = Application.Current.Windows[0];
            Views.ConfirmExpectedSalesDateDialogView confirmdate = new Views.ConfirmExpectedSalesDateDialogView(ExpectedDateFirstSales, statusmonth)
            {
                Owner = owner
            };
            confirmdate.ShowDialog();
            return (DateTime?)(confirmdate.Tag);
        }

        public bool EvaluationPlanDialog(Window owner, int id, int projectid)
        {
            if(owner == null)
                owner = Application.Current.Windows[0];
            Views.EPView confirmupdate = new Views.EPView(id, projectid)
            {
                Owner = owner
            };
            confirmupdate.ShowDialog();
            return (bool)(confirmupdate.Tag);
        }

        public bool MilestoneDialog(Window owner, int id, int projectid)
        {
            if (owner == null)
                owner = Application.Current.Windows[0];

            Views.MilestoneView milestonevw = new Views.MilestoneView(id, projectid)
            {
                Owner = owner
            };
            milestonevw.ShowDialog();
            return (bool)(milestonevw.Tag);
        }
               
        public string OpenFileDlg(string title, bool validatenames, bool multiselect, string filter, Window owner)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Title = title,
                ValidateNames = validatenames,
                Multiselect = multiselect
            };
            if (owner == null)
                dlg.ShowDialog();
            else
                dlg.ShowDialog(owner);
            string filename = dlg.FileName ?? string.Empty;
            return filename;
        }

        public string SaveFileDlg(string title, string filter, string suggestedfilename, Window owner)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                Title = title,
                ValidateNames = true,
                FileName = suggestedfilename
            };

            if (owner == null)
                dlg.ShowDialog();
            else
                dlg.ShowDialog(owner);
            string filename = dlg.FileName ?? string.Empty;
            return filename;
        }

        public GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons)
        {
            var slButtons = MessageBoxButton.OK;
            switch (buttons)
            {
                case GenericMessageBoxButton.OK:
                    slButtons = MessageBoxButton.OK;
                    break;
                case GenericMessageBoxButton.OKCancel:
                    slButtons = MessageBoxButton.OKCancel;
                    break;
                case GenericMessageBoxButton.YesNo:
                    slButtons = MessageBoxButton.YesNo;
                    break;
                case GenericMessageBoxButton.YesNoCancel:
                    slButtons = MessageBoxButton.YesNoCancel;
                    break;
                default:
                    break;
            }
            
            var result = MessageBox.Show(message, caption, slButtons);
            var returnedResult = GenericMessageBoxResult.OK;
            switch (result)
            {
                case MessageBoxResult.OK:
                    returnedResult = GenericMessageBoxResult.OK;
                    break;
                case MessageBoxResult.Cancel:
                    returnedResult = GenericMessageBoxResult.Cancel;
                    break;
                case MessageBoxResult.Yes:
                    returnedResult = GenericMessageBoxResult.Yes;
                    break;
                case MessageBoxResult.No:
                    returnedResult = GenericMessageBoxResult.No;
                    break;
                default:
                    break;

            }
            return returnedResult;
        }

        public void Show(string message, string caption)
        {
            MessageBox.Show(new Window { Topmost = true }, message, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType)
        {
           return MessageBox.Show(new Window {Topmost=true },  text, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK) == MessageBoxResult.OK;
        }

        public GenericMessageBoxResult ShowMessage(string text, string caption, GenericMessageBoxButton messagebutton, GenericMessageBoxIcon messageicon)
        {
            var slIcon = MessageBoxImage.None;
            switch (messageicon)
            {
                case GenericMessageBoxIcon.Asterisk:
                    slIcon = MessageBoxImage.Asterisk;
                    break;
                case GenericMessageBoxIcon.Error:
                    slIcon = MessageBoxImage.Error;
                    break;
                case GenericMessageBoxIcon.Exclamation:
                    slIcon = MessageBoxImage.Exclamation;
                    break;
                case GenericMessageBoxIcon.Hand:
                    slIcon = MessageBoxImage.Hand;
                    break;
                case GenericMessageBoxIcon.Information:
                    slIcon = MessageBoxImage.Information;
                    break;
                case GenericMessageBoxIcon.None:
                    slIcon = MessageBoxImage.None;
                    break;
                case GenericMessageBoxIcon.Question:
                    slIcon = MessageBoxImage.Question;
                    break;
                case GenericMessageBoxIcon.Stop:
                    slIcon = MessageBoxImage.Stop;
                    break;
                case GenericMessageBoxIcon.Warning:
                    slIcon = MessageBoxImage.Warning;
                    break;

                default:
                    break;
            }

            var slButtons = MessageBoxButton.OK;
            var defaultresult = MessageBoxResult.No;
            switch (messagebutton)
            {
                case GenericMessageBoxButton.OK:
                    slButtons = MessageBoxButton.OK;
                    defaultresult = MessageBoxResult.OK;
                    break;
                case GenericMessageBoxButton.OKCancel:
                    slButtons = MessageBoxButton.OKCancel;
                    defaultresult = MessageBoxResult.Cancel;
                    break;
                case GenericMessageBoxButton.YesNo:
                    slButtons = MessageBoxButton.YesNo;
                    defaultresult = MessageBoxResult.No;
                    break;
                case GenericMessageBoxButton.YesNoCancel:
                    slButtons = MessageBoxButton.YesNoCancel;
                    defaultresult = MessageBoxResult.Cancel;
                    break;
                default:
                    break;
            }

            var result = MessageBox.Show(text, caption, slButtons, slIcon, defaultresult, MessageBoxOptions.ServiceNotification);
            var returnedResult = GenericMessageBoxResult.OK;
            switch (result)
            {
                case MessageBoxResult.OK:
                    returnedResult = GenericMessageBoxResult.OK;
                    break;
                case MessageBoxResult.Cancel:
                    returnedResult = GenericMessageBoxResult.Cancel;
                    break;
                case MessageBoxResult.Yes:
                    returnedResult = GenericMessageBoxResult.Yes;
                    break;
                case MessageBoxResult.No:
                    returnedResult = GenericMessageBoxResult.No;
                    break;
                default:
                    break;
            }
            return returnedResult;
        }
      
    }

}
