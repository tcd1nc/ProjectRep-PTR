
namespace PTR.Models
{
    public class CheckedListObject : ViewModelBase
    {
       
        private bool isChecked;
        private string name;

        public CheckedListObject()
        { }

        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set { SetField(ref isChecked, value); }
        }

    }

}
