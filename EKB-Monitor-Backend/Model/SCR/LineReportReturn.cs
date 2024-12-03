namespace EKB_Monitor_Backend.Model.SCR
{
    public class LineReportReturn
    {
        public string? seq { get; set; }

        public string? model { get; set; }

        public string? modelCode { get; set; }

        public decimal? actual { get; set; }
        public decimal? apsPlan { get; set; }


        public decimal? remainPlan { get; set; }


        public string? status { get; set; }

        public Boolean? pnReady { get; set; }


    }


}
