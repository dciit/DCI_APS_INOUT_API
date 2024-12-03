namespace EKB_Monitor_Backend.Model.SCR
{
    public class History
    {
        public string ymd { get;  set; }

        public string partno { get; set; }

        public string cm { get; set; }

        public string shift { get; set; }

        public string wcno { get; set; }

        public string type { get; set; }
    }


    public class HistoryReturn
    {
        public string refs { get; set; }

        public string type { get; set; }

        public decimal transQty { get; set; }
    }


    public class HistoryRemarkReturn
    {
      

        public string remark_date { get; set; }

        public string remark_by { get; set; }

        public string remark_status { get; set; }

        public string remark_qty { get; set; }
    }


    public class TransectionSend
    {
        
        public string ymd { get; set; }
        public string wcno { get; set; }


        public string partno { get; set; }


        public string cm { get; set; }



    }

    public class TransectionReturn
    {
        public string date { get; set; }

        public string shift { get; set; }

        public string wcno { get; set; }


        public string partno { get; set; }


        public string cm { get; set; }

        public string type { get; set; }

        public decimal qty { get; set; }


        public string qrcodeData { get; set; }

        public string createBy { get; set; }


        public string createDate { get; set; }

        public string refno { get; set; }


    }
}
