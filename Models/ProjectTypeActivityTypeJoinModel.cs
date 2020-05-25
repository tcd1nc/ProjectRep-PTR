using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTR.Models
{
    public class ProjectTypeActivityTypeJoinModel : ViewModelBase
    {
        
        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int activitystatuscodeid;
        public int ActivityStatusCodeID
        {
            get { return activitystatuscodeid; }
            set { SetField(ref activitystatuscodeid, value); }
        }

        int projecttypeid;
        public int ProjectTypeID
        {
            get { return projecttypeid; }
            set { SetField(ref projecttypeid, value); }
        }
        

    }
}
