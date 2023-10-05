using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;
using Softify;


namespace SoftifyFoodPOSNew.Models
{
    public class LoginCategory
    {
        [Key]
        [Display(Name = "Id :")]
        public int CatId { get; set; }

        [Required(ErrorMessage = "Please Provide Category Name.")]
        [Display(Name = "Name :")]
        public string CatName { get; set; }

        [Display(Name = "Parent Name :")]
        public int ParentId { get; set; }

        [Display(Name = "Is Parent:")]
        public Boolean IsParent { get; set; }

        [Display(Name = "Is Inactive :")]
        public Boolean IsInactive { get; set; }
        

     

        // AS Like Our List Form  
        public static List<LoginCategory> prcGetData()
        {
            IDataReader reader = null;
            List<LoginCategory> list = new List<LoginCategory>();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetLoginCategory '', 0 ";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    LoginCategory Item = new LoginCategory();
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
     
        /// For Edit as Like DisplayDetails
        private void prcSetData(IDataRecord reader)
        {
            CatId = Int32.Parse(reader["CatId"].ToString());
            CatName = reader["CatName"].ToString();
            ParentId = Int32.Parse(reader["ParentId"].ToString());
            IsParent = (Int32.Parse(reader["IsParent"].ToString()) == 0) ? false : true;
            IsInactive = (Int32.Parse(reader["IsInactive"].ToString()) == 0) ? false : true;
        }
        // Same as Display Details
        public static LoginCategory prcGetData(int Id)
        {
            IDataReader reader = null;
            LoginCategory model = new LoginCategory();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetLoginCategory '', " + Id + " ";
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

        // as Like Load List
        public static DataSet prcGetData(string flag, int id)
        {
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                DataSet dsList = new DataSet();
                string sqlQuery = "Exec prcGetLoginCategory '" + flag + "' ";
                
                if (flag == "Details")
                {
                    sqlQuery = "Exec prcGetLoginCategory '" + flag + "','" + id + "' ";
                }

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



        //public class Context : DbContext
        //{
        //    public DbSet<LoginCategory> Departments { get; set; }
        //}
    }
}