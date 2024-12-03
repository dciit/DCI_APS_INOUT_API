namespace EKB_Monitor_Backend.Model
{
  
    public class StackDataModel
    {
        public List<double>? SummaryData { get; set; }

        public string? RoundDeliverlyTime { get; set; }

        public string? Type { get; set; }

        public string? wcno { get; set; }
    }

    public class SearchDateDayNight
    {   
        public string? wcno { get; set; }
        public string? partNo { get; set; }

        public string? date { get; set; }
    }


}
