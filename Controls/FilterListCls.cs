using System.ComponentModel;
using System.Windows.Media;

namespace PTR
{
   
    public class FilterListItem : INotifyPropertyChanged
    {
        private bool isSelected;       
        private System.Windows.Visibility visibleState;

        public int ID { get; set; }

        public string Name { get; set; }

        public string Colour { get; set; } = "#FFFFFF";

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public System.Windows.Visibility VisibleState
        {
            get { return visibleState; }
            set
            {
                visibleState = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VisibleState"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

}