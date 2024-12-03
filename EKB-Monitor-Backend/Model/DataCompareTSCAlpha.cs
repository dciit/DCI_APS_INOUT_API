namespace EKB_Monitor_Backend.Model
{
    public class DataCompareTSCAlpha
    {

        public class HeaderDataCompareTSC
        {

          

            public string? Partno { get; set; }
            public string? Cm { get; set; }

            public List<DetailDataCompareTSC>? DetailTSCData { get; set; }


            
        }


        public class DetailDataCompareTSC
        {
            public decimal? tsc_lbal { get; set; }

            public decimal? tsc_rec { get; set; }

            public decimal? tsc_iss { get; set; }

            public decimal? tsc_bal { get; set; }



            public decimal? ekb_lbal { get; set; }

            public decimal? ekb_rec { get; set; }

            public decimal? ekb_iss { get; set; }

            public decimal? ekb_bal { get; set; }
        }


      
    }
}
