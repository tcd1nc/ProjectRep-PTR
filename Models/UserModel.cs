
namespace PTR.Models
{
    public class UserModel : ModelBaseVM
    {
        string loginname;
        public string LoginName {
            get { return loginname; }
            set { SetField(ref loginname, value); }
        }

        string email;
        public string Email
        {
            get { return email; }
            set { SetField(ref email, value); }
        }

        bool administrator;
        public bool Administrator
        {
            get { return administrator; }
            set { SetField(ref administrator, value); }
        }
        
        bool showothers;
        public bool ShowOthers
        {
            get { return showothers; }
            set { SetField(ref showothers, value); }
        }

        string businessunits;
        public string BusinessUnits
        {
            get { return businessunits; }
            set { SetField(ref businessunits, value); }
        }

        string gin;
        public string GIN
        {
            get { return gin; }
            set { SetField(ref gin, value); }
        }
                
        //FullyObservableCollection<SelectedItemModel> salesdivisionscoll;
        //public FullyObservableCollection<SelectedItemModel> SalesDivisionsColl
        //{
        //    get { return salesdivisionscoll; }
        //    set { SetField(ref salesdivisionscoll, value); }
        //}

        //FullyObservableCollection<SelectedItemModel> countriescoll;
        //public FullyObservableCollection<SelectedItemModel> CountriesColl
        //{
        //    get { return countriescoll; }
        //    set { SetField(ref countriescoll, value); }
        //}
                       
        string administrationmnu;
        public string AdministrationMnu
        {
            get { return administrationmnu; }
            set { SetField(ref administrationmnu, value); }
        }
        string projectsmnu;
        public string ProjectsMnu
        {
            get { return projectsmnu; }
            set { SetField(ref projectsmnu, value); }
        }
        string reportsmnu;
        public string ReportsMnu
        {
            get { return reportsmnu; }
            set { SetField(ref reportsmnu, value); }
        }

        //FullyObservableCollection<SelectedItemModel> administrationmnucoll;
        //public FullyObservableCollection<SelectedItemModel> AdministrationMnuColl
        //{
        //    get { return administrationmnucoll; }
        //    set { SetField(ref administrationmnucoll, value); }
        //}

        //FullyObservableCollection<SelectedItemModel> projectsmnucoll;
        //public FullyObservableCollection<SelectedItemModel> ProjectsMnuColl
        //{
        //    get { return projectsmnucoll; }
        //    set { SetField(ref projectsmnucoll, value); }
        //}

        //FullyObservableCollection<SelectedItemModel> reportsmnucoll;
        //public FullyObservableCollection<SelectedItemModel> ReportsMnuColl
        //{
        //    get { return reportsmnucoll; }
        //    set { SetField(ref reportsmnucoll, value); }
        //}

        bool alloweditcompletedcancelled;
        public bool AllowEditCompletedCancelled
        {
            get { return alloweditcompletedcancelled; }
            set { SetField(ref alloweditcompletedcancelled, value); }
        }

    }
    
}