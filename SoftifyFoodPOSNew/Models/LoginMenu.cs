using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class LoginMenu
    {
        [Key]
        [Display(Name = "Id :")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Please Provide Menu Name.")]
        [Display(Name = "Name :")]
        public string MenuName { get; set; }

        [Display(Name = "Controller Name :")]
        public string MenuController { get; set; }

        [Display(Name = "Action Name:")]
        public string MenuLink { get; set; }

        [Display(Name = "Parent :")]
        public int ParentId { get; set; }

        [Display(Name = "Is Parent :")]
        public Boolean IsParent { get; set; }

        [Display(Name = "Is Inactive :")]
        public Boolean IsInactive { get; set; }

        [Display(Name = "Menu Icon :")]
        public string MenuIcon { get; set; }

        private void prcSetData(IDataRecord reader)
        {
            MenuId = Int32.Parse(reader["MenuId"].ToString());
            MenuName = reader["MenuName"].ToString();
            MenuController = reader["MenuController"].ToString();
            MenuLink = reader["MenuLink"].ToString();
            ParentId = Int32.Parse(reader["ParentId"].ToString());
            IsParent = (Int32.Parse(reader["IsParent"].ToString()) == 0) ? false : true;
            IsInactive = (Int32.Parse(reader["IsInactive"].ToString()) == 0) ? false : true;
            MenuIcon = reader["MenuIcon"].ToString();
        }

        public static List<LoginMenu> prcGetData()
        {
            IDataReader reader = null;
            List<LoginMenu> list = new List<LoginMenu>();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                const string sqlQuery = "Exec prcGetLoginMenu '', 0,'SoftifyFoodPOSNew'";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    LoginMenu Item = new LoginMenu();
                    Item.prcSetData(reader);
                    list.Add(Item);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                clsCon = null;
            }
        }

        public static LoginMenu prcGetData(int Id)
        {
            IDataReader reader = null;
            LoginMenu model = new LoginMenu();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetLoginMenu '', " + Id + ",'SoftifyFoodPOSNew' ";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    model.prcSetData(reader);
                    break;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                clsCon = null;
            }
        }

        public static DataSet prcGetData(string Flag,string id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetLoginMenu '"+Flag+"','"+id+ "' ,'SoftifyFoodPOSNew'";
                clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, sqlQuery);
                return dsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                clsCon = null;
            }
        }

    }
}