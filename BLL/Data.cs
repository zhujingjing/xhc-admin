using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Data_Sys_User
    {
        public Data_Sys_User()
        {
            Login = false;
        }
        public bool Login { get; set; }

        public string Id { get; set; }

        public string User_Name { get; set; }

        public string User_Pwd { get; set; }

        public string Type { get; set; }

        public string Table_Name { get; set; }

        public string Other_id { get; set; }

        public string Comment { get; set; }


    }

    public class Data_Shop_Pay
    {
        public string Shop_Pay_Id { get; set; }
        public string Shop_Id { get; set; }
        public string Sys_User_Id { get; set; }
        public string User_Id { get; set; }

        public string Order_No { get; set; }
        public string State { get; set; }
        public double Money { get; set; }
        public DateTime Pay_Time { get; set; }
        public string  Shop_Name{ get; set; }
        public string  Sys_User_Name{ get; set; }
        public string User_Name { get; set; }
        public string Tel { get; set; }
        public string Comment { get; set; }
        public DateTime Create_Time { get; set; }

        public string Create_Time_Str { get; set; }
    }

}
