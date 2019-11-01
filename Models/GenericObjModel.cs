

namespace PTR.Models
{
    public class GenericObjModel : ViewModelBase
    {
        public GenericObjModel()
        {
            ID = 0;
            Name = string.Empty;
            Description = string.Empty;
            Deleted = false;
        }

        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        string description;
        public string Description
        {
            get { return description; }
            set { SetField(ref description, value); }
        }

        bool deleted;
        public bool Deleted
        {
            get { return deleted; }
            set { SetField(ref deleted, value); }
        }
    }
}
