namespace EKB_Monitor_Backend.Model
{



    public class ResultTarget
    {

        public string shiftDate { get; set; }

        public string shiftDateFormat { get; set; }
        public string wcno { get; set; }

        public string partDesc { get; set; }

        public decimal target { get; set; }

        public decimal actual { get; set; }
    }


    public class ResultTargetDaily
    {
        public string shiftDate { get; set; }
        public string wcno { get; set; }

        public string partDesc { get; set; }

        public decimal target { get; set; }

        public decimal actual { get; set; }





    }


    public class findDailyOfResultAndTargets
    {   
        public string wcno { get; set; }
        public string? PARTNO { get; set; }
        public string? ShiftDate { get; set; }
        public decimal? IN { get; set; }
        public decimal? OUT { get; set; }
        public decimal? RJ { get; set; }
        public decimal? LBAL { get; set; }

        public string partDesc { get; set; }

    }

}
