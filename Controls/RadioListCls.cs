using System.ComponentModel;

namespace PTR
{
   
    public class RadioListItem : INotifyPropertyChanged
    {
        private bool isSelected;
        private System.Windows.Visibility visibleState;

        public int ID { get; set; }

        public string Name { get; set; }
        

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public string GroupName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public System.Windows.Visibility VisibleState
        {
            get { return visibleState; }
            set
            {
                visibleState = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VisibleState"));
            }
        }
    }

}
