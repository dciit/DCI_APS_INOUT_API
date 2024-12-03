namespace EKB_Monitor_Backend.Model
{
    public class PackingPayload
    {

        public string? wcno { get; set; }

        public string? stDate { get; set; }

        public string? enDate { get; set; }

        public string? partNo { get; set; }
    }


    public class DailyPackingPayload
    {

        public string? wcno { get; set; }

        public string? searchDate { get; set; }


        public string? partNo { get; set; }
    }



}
