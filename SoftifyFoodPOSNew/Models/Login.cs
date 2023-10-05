using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
//using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Login
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please provide user name")]
        [Display(Name = "User Name : ")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please provide password")]
        [Display(Name = "Password : ")]
        public string Password { get; set; }

        public string DisplayName { get; set; }
        public Int16 ComId { get; set; }   
      
        public string InvoicePrintSize { get; set; }
        public string EmpPicLocation { get; set; }
        public int BranchId { get; set; }
        public string Dashboard { get; set; }

        private void prcSetData(DataRow dr)
        {
            UserId = Int32.Parse(dr["LUserId"].ToString());
            UserName = dr["UserName"].ToString();
            Password = dr["UserPass"].ToString();
            DisplayName = dr["DisplayName"].ToString();
            ComId = Int16.Parse(dr["Comid"].ToString());
          
        
            InvoicePrintSize = dr["InvoicePrintSize"].ToString();
            EmpPicLocation = dr["EmpPicLocation"].ToString();
            BranchId = Int32.Parse(dr["BranchId"].ToString());
            Dashboard = dr["Dashboard"].ToString();
        }
        
        public static Login prcGetData(string UserName, string Password)
        {
            
            DataSet dsList = new DataSet();
            Login login = new Login();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            softifyInterfaceHelper clsProc = new softifyInterfaceHelper();
            try
            { 
                if (UserName != null && Password != null)
                {
                    if (UserName.Trim() != "" && Password.Trim() != "")
                    {
                        String strQuery = "Exec prcGetValidateLogin '" + clsProc.softifyAvoidSingleQuote(UserName) + "', '" + clsProc.softifyAvoidSingleQuote(clsProc.softifyEncryptString(Password)) + "', 'Local'  ";
                        clsCon.softifyFillDatasetUsingSQLCommand(ref dsList, strQuery);
                        dsList.Tables[0].TableName = "Login";
                        
                        foreach (DataRow row in dsList.Tables[0].Rows)
                        {
                            login.prcSetData(row);
                            break;
                        }
                    }
                }

                return login;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                dsList = null;
                clsProc = null;
                clsCon = null;
            }
        }
    }
}