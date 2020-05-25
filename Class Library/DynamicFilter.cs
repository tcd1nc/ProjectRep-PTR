using PTR.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

namespace PTR
{
    public static class DynamicFilter
    {      
        public static DataTable FilterDataTable(DataTable masterprojectlist, List<string> FieldList, Dictionary<string, FilterPopupModel> DictFilterPopup)
        {
            Dictionary<string, List<string>> locdictFilterPopup = new Dictionary<string, List<string>>();
            foreach (string s in FieldList)
                if (DictFilterPopup[s].IsApplied)
                    locdictFilterPopup.Add(s, DictFilterPopup[s].FilterData.Where(y => y.IsChecked == true).Select(x => x.Description).ToList<string>());

            string s1 = "";
            int ctr = 0;
            List<string>[] arrayoflists = new List<string>[locdictFilterPopup.Count()];
            foreach (var c in locdictFilterPopup)
            {
                if (ctr > 0)
                    s1 = s1 + " And ";
                s1 = s1 + "@" + ctr.ToString() + ".Contains(outerIt[\"" + c.Key + "\"].ToString())";
                arrayoflists[ctr] = locdictFilterPopup[c.Key];
                ctr++;
            }
            IQueryable<DataRow> t1;
            if (locdictFilterPopup.Count() > 0)
            {
                t1 = masterprojectlist.AsEnumerable().AsQueryable().Where(s1, arrayoflists);
                if (t1.Count() > 0)
                {
                    DataTable tblFiltered = t1.CopyToDataTable();
                    StaticCollections.ReFormatColumns(ref masterprojectlist, ref tblFiltered);
                    return tblFiltered;
                }
                else
                    return masterprojectlist.Clone();
            }
            else
                return masterprojectlist;
        }
    }
   
}
