using Softify;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SoftifyFoodPOSNew.Models;

namespace SoftifyFoodPOSNew.Models
{
    public class MenuPermission
    {
        public MenuPermission()
        {
            MPMenuList = new List<MenuList>();
        }
       
        public IList<MenuList> MPMenuList { get; set; }

        public class MenuList
        {             
            public long MenuId { get; set; }
            public string MenuName { get; set; }
            public string Title { get; set; }
            public int ParentId { get; set; }
            public int isParent { get; set; }
            

            public Boolean IsCheck { get; set; }
            public string MenuController { get; set; }
            public string MenuLink { get; set; }
            public bool IsUpdate { get; set; }
            public bool IsDelete { get; set; }
            public bool IsMaster { get; set; }           
        }


    } //end : Class
}