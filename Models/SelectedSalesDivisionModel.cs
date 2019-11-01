
namespace PTR.Models
{
    public class SelectedSalesDivisionModel : ViewModelBase
    {

        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int salesdivid;
        public int SalesDivID
        {
            get { return salesdivid; }
            set { SetField(ref salesdivid, value); }
        }

        bool ischecked;
        public bool IsChecked
        {
            get { return ischecked; }
            set { SetField(ref ischecked, value); }
        }

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }
    }
}
