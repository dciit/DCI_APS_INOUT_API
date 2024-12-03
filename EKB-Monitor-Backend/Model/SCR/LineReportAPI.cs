namespace EKB_Monitor_Backend.Model.SCR
{
    public class ModelInfo
    {
        public string? seq { get; set; }
        public string? model { get; set; }

        public string? modelCode { get; set; }

        public decimal? apsPlan { get; set; }

        public decimal? actual { get; set; }

        public string? seqStatus { get; set; }

        public List<LineInfo>? LineName { get; set;}

    }


    public class LineInfo
    {
        public string? line { get; set; }

        public List<ResultStatusInfo>? Results { get; set; }
    }



    public class ResultStatusInfo
    {   


        public string? partno { get; set; }
        public string? part_name { get; set; }

        public decimal stock { get; set; }

        public decimal remain { get; set; }

        public string? status { get; set; }



     
    }
}
