using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Display(Name = "Client Code")]
        public string ClientCode { get; set; }       

        [Display(Name = "Client Name")]
        [Required(ErrorMessage = "Please Provide Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Client Short Name")]
        public string shortName { get; set; }

        [Display(Name = "Client Address")]
        public string ClientAddress { get; set; }   

        //public int AccId { get; set; }
        public string AccName { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Web")]
        public string Web { get; set; }

        [Display(Name = "AreaIncharge")]
        public int AreaInchargeId { get; set; } 


        [Display(Name = "Opening Balance")]
        [Required(ErrorMessage = "Please Provide Opening Balance")]
        public double OPBalance { get; set; }        
         
        [Display(Name = "Opening Date")]
        public string dateOP { get; set; }

        public string ClosingBalance { get; set; }


        [Display(Name = "Reference")]
        public int ReferenceId { get; set; }
        //public bool IsAllowCredit { get; set; }
        public double CreditLimit { get; set; }

        public string dtFrom { get; set; }
        public string dtTo { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; } 

        public List<ClientContact> ContactInfo
        {
            get; set;
        }
        public class ClientContact
        {
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Designation { get; set; }
            public string Email { get; set; }
            public string Web { get; set; }            
        }         

        private void prcSetData(IDataRecord reader)
        {
            ClientId = Convert.ToInt32(reader["ClientId"].ToString());
            ClientCode = reader["ClientCode"].ToString();
            ClientName = reader["ClientName"].ToString();           
            ClientAddress = reader["ClientAddress"].ToString();           
            Phone = reader["Phone"].ToString();
            Mobile = reader["Mobile"].ToString();
            Email = reader["Email"].ToString();
            AreaInchargeId = Convert.ToInt32(reader["AreaIncharge"].ToString());

            ReferenceId = Convert.ToInt32(reader["ReferanceId"].ToString());
            OPBalance = Convert.ToDouble(reader["OPBalnce"].ToString());
            CreditLimit = Convert.ToDouble(reader["CreditLimit"].ToString()); 
            dateOP = reader["dateOP"].ToString(); 
            Web =reader["web"].ToString();
            //IsAllowCredit = Convert.ToBoolean(byte.Parse(reader["IsAllowCredit"].ToString()));
            Remarks = reader["Remarks"].ToString();
        }

       
        //For Truck Data set use For Edit Controller
        public static Client prcGetData(int comId, int id)
        {
            IDataReader reader = null;
            Client model = new Client();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC [prcGet_ClientList] " + comId + ", " + id;
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

        //Edit Data set Doc
        public static DataSet prcGetDataSub( int comId, int id)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC [prcGet_ClientList]  " + comId + ", " + id;

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
        //end DataSet prcGetData

        public static List<ClientContact> prcSetDataSub(DataSet dsList)
        {
            List<ClientContact> ContactInfo = new List<ClientContact>();

            for (int i = 0; i < dsList.Tables[1].Rows.Count; i++)
            {

                ClientContact aMOdel = new ClientContact();
                aMOdel.Designation = dsList.Tables[1].Rows[i]["Designation"].ToString();
                aMOdel.Name = dsList.Tables[1].Rows[i]["Name"].ToString();
                aMOdel.Email =dsList.Tables[1].Rows[i]["Email"].ToString();
                aMOdel.Mobile = dsList.Tables[1].Rows[i]["Mobile"].ToString();
                
                ContactInfo.Add(aMOdel);
            }

            return ContactInfo;

        }
        



        //For Combo Use
        public static DataSet prcGetData(string Flag, int comId)
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec [prcGet_Client] '" + Flag + "', " + comId + " ";

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