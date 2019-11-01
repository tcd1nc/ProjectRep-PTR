using System;
using System.Windows;
using System.Windows.Controls;

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
        object[] CompletedProjectDialog(Window owner, object[] values);
        object ShowCustomMessageDlg(double width, double height, string title = null, string message = null, ItemCollection items = null,
            System.Drawing.Icon msgimage = null);
        DateTime? ConfirmExpectedSalesDateDialog(Window owner, DateTime? value);
        bool EvaluationPlanDialog(Window owner, int id, int projectid);
        bool MilestoneDialog(Window owner, int id, int projectid);
        bool OpenReminderDlg(ViewModels.PTMainViewModel ptmvm);
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
            bool result;

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
                     
                case "EditUser":
                    Views.AssociatesView eav = new Views.AssociatesView
                    {
                        Owner = owner
                    };
                    eav.ShowDialog();
                    return (bool)eav.DialogResult;                                        
                                    
                case "EditCustomer":
                    Views.EditCustomerView2 ecv = new Views.EditCustomerView2
                    {
                        Owner = owner
                    };
                    ecv.ShowDialog();
                    return (bool)ecv.DialogResult;
                     
                case "EditCountry":
                    Views.EditCountryView ecountryv = new Views.EditCountryView
                    {
                        Owner = owner
                    };
                    ecountryv.ShowDialog();
                    return (bool)ecountryv.DialogResult;
                     
                case "ProjectMasterReport":
                    Views.PlaybookView2 playbookvw = new Views.PlaybookView2
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

                default:
                    return false;
            }           
        }

        public bool OpenReminderDlg(ViewModels.PTMainViewModel ptmvm)
        {                              
            Views.UserReminderDialog dlg = new Views.UserReminderDialog(ptmvm)
            {
                Owner = Application.Current.Windows[0]
            };
            dlg.ShowDialog();
            return  (bool)dlg.DialogResult;                         
        }


        public object[] CompletedProjectDialog(Window owner, object[] values)
        { 
            if(owner == null)
                owner = Application.Current.Windows[0];
            Views.CompletedProjectDialogView compproject = new Views.CompletedProjectDialogView(values)
            {
                Owner = owner
            };
            compproject.ShowDialog();            
            return (object[])(compproject.Tag);           
        }
        
        public DateTime? ConfirmExpectedSalesDateDialog(Window owner, DateTime? value)
        {
            if(owner ==null)
                owner = Application.Current.Windows[0];
            Views.ConfirmExpectedSalesDateDialogView confirmdate = new Views.ConfirmExpectedSalesDateDialogView(value)
            {
                Owner = owner
            };
            confirmdate.ShowDialog();
            return (DateTime?)(confirmdate.Tag);
        }

        public bool EvaluationPlanDialog(Window owner, int id, int projectid)
        {
            if (owner == null)
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

        public object ShowCustomMessageDlg(double width, double height, string title = null, string message = null,  ItemCollection items = null,
            System.Drawing.Icon msgimage = null)
        {         
            CustomDialog cdlg = new CustomDialog(width, height, title, message, items, msgimage);
            return cdlg.Tag;
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
            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        public bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType)
        {
           return MessageBox.Show(text, caption, MessageBoxButton.OK) == MessageBoxResult.OK;
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
            switch (messagebutton)
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

            var result = MessageBox.Show(text, caption, slButtons, slIcon);
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
