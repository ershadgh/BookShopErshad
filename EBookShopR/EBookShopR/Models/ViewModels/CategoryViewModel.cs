using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Models.ViewModels
{
    public class TreeViewCategory
    {

        public TreeViewCategory()
        {
            Subs = new List<TreeViewCategory>();
            
        }
        public TreeViewCategory(int[] categoryid)
        {
            this.categoryid = categoryid;
        }
        public int id { get; set; }
        public string title { get; set; }
      
        public int[] categoryid { get; set; }   
        public List<TreeViewCategory> Subs { get; set; }
        
    }
}
