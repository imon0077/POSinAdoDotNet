using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class LoginUser
    {

        [Key]
        [Display(Name = "Id :")]
        public int LUserId { get; set; }

        [Required(ErrorMessage = "Please Provide User Name.")]
        [Display(Name = "Name :")]
        public string UserName { get; set; }

        [PasswordPropertyText]
        [Required(ErrorMessage = "Please Provide User Password.")]
        [Display(Name = "Password :")]
        public string UserPass { get; set; }

        [Display(Name = "Display Name:")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Please Provide User Category.")]
        [Display(Name = "Category :")]
        public int CatId { get; set; }

        [Required(ErrorMessage = "Please Provide Reference Person")]
        public int RefId { get; set; }
        public int BranchId { get; set; }

        [Display(Name = "Is Inactive :")]
        public Boolean IsInactive { get; set; }


        private void prcSetData(IDataRecord reader)
        {
            LUserId = Int32.Parse(reader["LUserId"].ToString());
            UserName = reader["UserName"].ToString();
            UserPass = reader["UserPass"].ToString();
            DisplayName = reader["DisplayName"].ToString();
            CatId = Int32.Parse(reader["CatId"].ToString());
            RefId = Int32.Parse(reader["RefId"].ToString());
            BranchId = Int32.Parse(reader["BranchId"].ToString());
            IsInactive = (Int32.Parse(reader["IsInactive"].ToString()) == 0) ? false : true;
        }


        public static LoginUser prcGetData(int Id)
        {
            IDataReader reader = null;
            LoginUser model = new LoginUser();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetLoginUser " + HttpContext.Current.Session["ComId"] + ", " + Id + " ";
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

        public static DataSet prcGetDataCombo()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetLoginUser  0 ";
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