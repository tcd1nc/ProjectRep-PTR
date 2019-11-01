using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTR.Models
{
    public class ProjectStatusListItem : ViewModelBase
    {
        private int _ID;
        private string _description;
        private string _status;
        private string _colour;
        private bool _isSelected;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string Description
        {
            get { return _description; }
            set { SetField(ref _description, value); }
        }

        public string Status
        {
            get { return _status; }
            set { SetField(ref _status, value); }
        }

        public string Colour
        {
            get { return _colour; }
            set { SetField(ref _colour, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetField(ref _isSelected, value);
            }
        }

    }
}
