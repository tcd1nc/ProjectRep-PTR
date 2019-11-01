using System;

namespace PTR.Models
{
    public class EPModel : ViewModelBase
    {        
        public int ProjectID { get; set; }
        public int CustomerID { get; set; }

        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }       
       
        string objectives;
        public string Objectives
        {
            get { return objectives; }
            set { SetField(ref objectives, value); }
        }

        string strategy;
        public string Strategy
        {
            get { return strategy; }
            set { SetField(ref strategy, value); }
        }

        DateTime? created;
        public DateTime? Created
        {
            get { return created; }
            set { SetField(ref created, value); }
        }

        DateTime? discussed;
        public DateTime? Discussed
        {
            get { return discussed; }
            set { SetField(ref discussed, value); }
        }
       
    }
}
