namespace EKB_Monitor_Backend.Model
{
    public class OverAllBarChart
    {

        public class getLabelsOverallChart
        {
            public string? partNo { get; set; }

            public string? date { get; set; }
        }

        public class findTotalInOutOverallChart
        {


            public double[]? stocks { get; set; }
            public double[]? inData { get; set; }

            public double[]? outData { get; set; }

            public double[]? rjData { get; set; }
            public double[]? remainData { get; set; }

            

        }

        public class lbalDaily
        {


            public decimal? LbalLastMonth { get; set; }

            public string? Ymd { get; set; }

            public decimal? In { get; set; }

            public decimal? Out { get; set; }

            public decimal? Bal { get; set; }

            public string? partNo { get; set; }
        }

    }
}
