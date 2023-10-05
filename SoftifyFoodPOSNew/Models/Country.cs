using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using Softify;

namespace SoftifyFoodPOSNew.Models
{
    public class Country
    {
        //countryId, countryCode, countryName,countrySName, CurrName, CurrSName, timeDifference
        public int countryId { get; set; }

        [Required(ErrorMessage = "Please, Provide country name")]
        public string countryName { get; set; }
        public string countryCode { get; set; }
        public string countrySName { get; set; }
        public string CurrName { get; set; }
        public string CurrSName { get; set; }
        public string timeDifference { get; set; }
        
        private void prcSetData(IDataReader reader)
        {

            countryId = Convert.ToInt32(reader["countryId"].ToString());
            countryName = reader["countryName"].ToString();
            countryCode = reader["countryCode"].ToString();
            countrySName = reader["countrySName"].ToString();
            CurrName = reader["CurrName"].ToString();
            CurrSName = reader["CurrSName"].ToString();
            timeDifference = reader["timeDifference"].ToString();
        }

        public static List<Country> prcGetData()
        {
            IDataReader reader = null;
            List<Country> list = new List<Country>();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "EXEC prcGetCountry -1 ";
                reader = clsCon.GetReader(sqlQuery);
                while (reader.Read())
                {
                    Country Item = new Country();
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

        public static Country prcGetData(long id)
        {
            IDataReader reader = null;
            Country model = new Country();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetCountry " + id + " ";
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

        public static DataSet prcGetDatacombo()
        {
            DataSet dsList = new DataSet();
            SoftifySQLConnection clsCon = new SoftifySQLConnection(true);
            try
            {
                string sqlQuery = "Exec prcGetCountry 0 ";
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