namespace EKB_Monitor_Backend.Model
{

    public class DataIN
    {
        public string? shiftDate { get; set; }

        public string? wcno { get; set; }
        public string? partNo { get; set; }

        public string? cm { get; set; }


        public string? timeRound { get; set; }

        public string? TransType { get; set; }

        public decimal? totalRound { get; set; }
        public decimal? transQty { get; set; }

        public string? createBy { get; set; }
        public string? shifts { get; set; }

        public string? qrCode { get; set; }

    }


    public class DataOUT
    {
        public string? shiftDate { get; set; }

        public string? wcno { get; set; }

        public string? partNo { get; set; }

        public string? cm { get; set; }

        public string? transType { get; set; }

        public decimal? transQty { get; set; }

        public string? shifts { get; set; }

        public string? qrCode { get; set; }

        public string? createBy { get; set; }

    


    }
}

