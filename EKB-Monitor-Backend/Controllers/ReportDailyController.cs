using EKANBAN.Service;
using EKB_Monitor_Backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using static EKB_Monitor_Backend.Model.DataCompareTSCAlpha;
using static EKB_Monitor_Backend.Model.OverAllBarChart;

namespace EKB_Monitor_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportDailyController : Controller
    {
        // GET: ReportDailyController
        private ConnectDB oCOnnSCM = new ConnectDB("DBSCM");
        DateTime current = DateTime.Now;

        //string wcno_default = "308";
        //string YM = DateTime.Now.ToString("yyyyMM");
        //string Dates = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");


        [HttpGet]
        [Route("getDailyBarChartDay/{wcno}/{searchDate}")]
        public IActionResult getReportEKB_Day(string wcno, string searchDate)
        {
            List<StackDataModel> res = new List<StackDataModel>();

            int i = 0;


            string[] shift_Day = { "09:00-10:30", "10:30-12:00", "13:00-14:00", "15:00-16:30", "18:00-20:00" };

            string[] type = { "IN", "OUT" };


            //string Dates = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            DateTime dateConvert = DateTime.Parse(searchDate);
            string YM = dateConvert.ToString("yyyyMM");


            List<SearchDateDayNight> sdnList = new List<SearchDateDayNight>();

            // วนลูป เพือหา labels
            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = @"	SELECT	distinct					
                            PARTNO,
							CONVERT(varchar,[ShiftDate],23) as [ShiftDate]
						
						
                        
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply]
						     where YM = @YM and wcno = @WCNO and  ShiftDate >=  @Stdate and  ShiftDate <=  @enDate
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],23),PARTNO
                             order by ShiftDate";

            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", searchDate));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", searchDate));
            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);

            if (dtOverAll.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtOverAll.Rows)
                {

                    SearchDateDayNight sdn = new SearchDateDayNight();

                    sdn.partNo = drAll["PARTNO"].ToString();
                    sdn.date = drAll["ShiftDate"].ToString();

                    sdnList.Add(sdn);
                }
            }

            //หาข้อมูลตามวันที่ search เข้ามา
            foreach (SearchDateDayNight searchDateDayNight in sdnList)
            {



                // เก็บเวลาเข้าใน list

                int z = 0;
                DataTable dtTime = GetData(searchDateDayNight.date, "D", wcno, searchDateDayNight.partNo);
                List<string> Types = new List<string>() { "IN", "OUT" };
                foreach (string itemType in Types)
                {
                    foreach (DataRow col in dtTime.Rows)
                    {
                        int index = res.FindIndex(x => x.Type == "IN" ? x.RoundDeliverlyTime == "ส่ง:" + col["TIME_ROUND"].ToString() && x.Type == itemType : x.RoundDeliverlyTime == "ผลิต:" + col["TIME_ROUND"].ToString() && x.Type == itemType);
                        if (index < 0)
                        {
                            StackDataModel item_in = new StackDataModel();
                            item_in.RoundDeliverlyTime = itemType == "IN" ? "ส่ง:" + col["TIME_ROUND"].ToString() : "ผลิต:" + col["TIME_ROUND"].ToString();
                            item_in.Type = itemType;
                            item_in.SummaryData = new List<double>();
                            item_in.SummaryData.Add(Convert.ToDouble(col["QTY_" + itemType]));
                            res.Add(item_in);
                        }
                        else
                        {
                            // each day
                            res[index].SummaryData.Add(Convert.ToDouble(col["QTY_" + itemType]));
                        }
                        z++;
                    }
                }







                i++;
                // เก็บวันที่ค้นหา

            }



            return Ok(new
            {
                dateSearchListDay = sdnList,
                dataListDay = res
            });

        }



        [HttpGet]
        [Route("getDailyBarChartNight/{wcno}/{searchDate}")]
        public IActionResult getReportEKB_Night(string wcno, string searchDate)
        {
            List<StackDataModel> res = new List<StackDataModel>();

            int i = 0;


            string[] shift_Day = { "21:00", "22:30", "01:00", "02:30", "06:00" };

            string[] type = { "IN", "OUT" };


            DateTime dateConvert = DateTime.Parse(searchDate);
            string YM = dateConvert.ToString("yyyyMM");


            List<SearchDateDayNight> sdnList = new List<SearchDateDayNight>();

            // วนลูป เพือหา labels
            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = @"	SELECT	distinct					
                            PARTNO,
							CONVERT(varchar,[ShiftDate],23) as [ShiftDate]
						
						
                        
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply]
						     where YM = @YM and wcno = @WCNO and  ShiftDate >=  @Stdate and  ShiftDate <=  @enDate
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],23),PARTNO
                             order by ShiftDate";

            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", searchDate));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", searchDate));
            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);

            if (dtOverAll.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtOverAll.Rows)
                {

                    SearchDateDayNight sdn = new SearchDateDayNight();

                    sdn.partNo = drAll["PARTNO"].ToString();
                    sdn.date = drAll["ShiftDate"].ToString();

                    sdnList.Add(sdn);
                }
            }

            //หาข้อมูลตามวันที่ search เข้ามา
            foreach (SearchDateDayNight searchDateDayNight in sdnList)
            {


                // เก็บเวลาเข้าใน list


                DataTable dtTime = GetData(DateTime.Parse(searchDateDayNight.date).ToString("yyyy-MM-dd"), "N", wcno, searchDateDayNight.partNo);
                List<string> Types = new List<string>() { "IN", "OUT" };
                foreach (string itemType in Types)
                {
                    foreach (DataRow col in dtTime.Rows)
                    {
                        int index = res.FindIndex(x => x.Type == "IN" ? x.RoundDeliverlyTime == "ส่ง:" + col["TIME_ROUND"].ToString() && x.Type == itemType : x.RoundDeliverlyTime == "ผลิต:" + col["TIME_ROUND"].ToString() && x.Type == itemType);
                        if (index < 0)
                        {
                            StackDataModel item_in = new StackDataModel();
                            item_in.RoundDeliverlyTime = itemType == "IN" ? "ส่ง:" + col["TIME_ROUND"].ToString() : "ผลิต:" + col["TIME_ROUND"].ToString();
                            item_in.Type = itemType;
                            item_in.SummaryData = new List<double>();
                            item_in.SummaryData.Add(Convert.ToDouble(col["QTY_" + itemType]));
                            res.Add(item_in);
                        }
                        else
                        {
                            // each day
                            res[index].SummaryData.Add(Convert.ToDouble(col["QTY_" + itemType]));
                        }

                    }
                }






                i++;
                // เก็บวันที่ค้นหา

            }



            return Ok(new
            {
                dateSearchListNight = sdnList,
                dataListNight = res
            });

        }



        [HttpGet]
        [Route("getDailyBarChartOverAll/{wcno}/{searchDate}")]
        public IActionResult getReportEKB_OverAll(string wcno, string searchDate)
        {

            List<getLabelsOverallChart> gocList = new List<getLabelsOverallChart>();


            List<findTotalInOutOverallChart> res = new List<findTotalInOutOverallChart>();
            string YM = DateTime.Parse(searchDate).ToString("yyyyMM");

            int year = DateTime.Now.Year;
            int mont = DateTime.Parse(searchDate).Month;


            //DataTable getLbals = getLbal(payload, YM);

            string partno = "";

            SqlCommand sql_select_partno = new SqlCommand();
            sql_select_partno.CommandText = @"	SELECT TOP(1) PARTNO
                                                    FROM EKB_WIP_Part_Stock
                                                    where YM = @YM and WCNO = @WCNO
                                                    order by UpdateDate desc";

            sql_select_partno.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_partno.Parameters.Add(new SqlParameter("@WCNO", wcno));

            DataTable dtSelectPartno = oCOnnSCM.Query(sql_select_partno);

            if (dtSelectPartno.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtSelectPartno.Rows)
                {


                    partno = drAll["PARTNO"].ToString();

                }
            }



            // วนลูป เพือหา labels
            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = @"	SELECT 				
                            PARTNO,
							CONVERT(varchar,[ShiftDate],23) as [ShiftDate]
						
						
                        
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply]
						     where YM = @YM and wcno = @WCNO and  ShiftDate >=  @Stdate and  ShiftDate <=  @enDate and PARTNO = @PARTNO
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],23),PARTNO
                             order by ShiftDate";

            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@PARTNO", partno));

            //sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", payload.stDate));
            //sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", payload.enDate));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", new DateTime(year, mont, 1, 0, 0, 0).ToString("yyyy-MM-dd")));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", DateTime.Now.ToString("yyyy-MM-dd")));
            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);

            if (dtOverAll.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtOverAll.Rows)
                {

                    getLabelsOverallChart goc = new getLabelsOverallChart();

                    goc.partNo = drAll["PARTNO"].ToString();
                    goc.date = drAll["ShiftDate"].ToString();

                    gocList.Add(goc);
                }
            }

            // วนลูป เพือหาข้อมูล IN OUT ทั้งหมด



            DataTable dtFinds = new DataTable();

            dtFinds.TableName = "tbDeliverlyPartTime";
            dtFinds.Columns.Add("PARTNO", typeof(string));
            dtFinds.Columns.Add("ShiftDate", typeof(string));
            dtFinds.Columns.Add("IN", typeof(decimal));
            dtFinds.Columns.Add("OUT", typeof(decimal));
            dtFinds.Columns.Add("RJ", typeof(decimal));
            dtFinds.Columns.Add("LBAL", typeof(decimal));



            int i = 0;
            foreach (getLabelsOverallChart item in gocList)
            {
                SqlCommand sql_select_overall_dataList = new SqlCommand();
                sql_select_overall_dataList.CommandText = @"	SELECT						
                            transection.PARTNO,
							CONVERT(varchar,[ShiftDate],3) as [ShiftDate],
						
						
                             SUM([TransQty]) as TotalRound, TransType,wip.LBAL
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply] transection
                             LEFT JOIN [dbSCM].[dbo].[EKB_WIP_Part_Stock] wip on wip.PARTNO = transection.PARTNO and wip.YM = @YM

						     where transection.wcno = @WCNO and  ShiftDate =  @ShiftDate and transection.PARTNO = @PARTNO
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],3),transection.PARTNO,TransType,wip.LBAL
                             order by ShiftDate";

                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@YM", YM));
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@WCNO", wcno));
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@PARTNO", item.partNo));

                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@ShiftDate", DateTime.Parse(item.date).ToString("yyyy-MM-dd")));

                DataTable dtFindTotalInOUT = oCOnnSCM.Query(sql_select_overall_dataList);
                dtFinds.Rows.Add(item.partNo, item.date, 0, 0, 0);

                foreach (DataRow drF in dtFinds.Rows)
                {

                    foreach (DataRow drQuerry in dtFindTotalInOUT.Rows)
                    {

                        findTotalInOutOverallChart _find = new findTotalInOutOverallChart();
                        if (item.partNo == drQuerry["PARTNO"].ToString())
                        {
                            if (drQuerry["TransType"].ToString() == "IN")
                            {

                                dtFinds.Rows[i]["IN"] = drQuerry["TotalRound"];
                                dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];

                            }
                            else if (drQuerry["TransType"].ToString() == "OUT")
                            {
                                dtFinds.Rows[i]["OUT"] = drQuerry["TotalRound"];
                                dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];
                            }
                            else
                            {
                                dtFinds.Rows[i]["RJ"] = drQuerry["TotalRound"] == "" ? 0 : drQuerry["TotalRound"];
                                dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];
                            }
                        }


                    }



                }

                i++;


            }




            double[] stockLbal = new double[1];
            double[] inData = new double[dtFinds.Rows.Count];
            double[] outData = new double[dtFinds.Rows.Count];
            double[] rjData = new double[dtFinds.Rows.Count];
            double[] remainData = new double[dtFinds.Rows.Count];
            List<findTotalInOutOverallChart> _result = new List<findTotalInOutOverallChart>();

            int z = 0;

            foreach (DataRow dc in dtFinds.Rows)
            {

                if (z == 0)
                {
                    inData[z] = Convert.ToDouble(dtFinds.Rows[z]["IN"]);
                    outData[z] = Convert.ToDouble(dtFinds.Rows[z]["OUT"]);
                    rjData[z] = Convert.ToDouble(dtFinds.Rows[z]["RJ"]);
                    stockLbal[z] = Convert.ToDouble(dtFinds.Rows[z]["LBAL"]);
                    remainData[z] = (stockLbal[0] + inData[z]) - outData[z] - rjData[z];
                }
                else
                {
                    inData[z] = Convert.ToDouble(dtFinds.Rows[z]["IN"]);
                    outData[z] = Convert.ToDouble(dtFinds.Rows[z]["OUT"]);
                    rjData[z] = Convert.ToDouble(dtFinds.Rows[z]["RJ"]);
                    remainData[z] = (remainData[z - 1] + inData[z]) - outData[z] - rjData[z];
                }


                z++;
            }

            findTotalInOutOverallChart findTotalInOutOverallChart = new findTotalInOutOverallChart();

            findTotalInOutOverallChart.stocks = stockLbal;
            findTotalInOutOverallChart.inData = inData;
            findTotalInOutOverallChart.outData = outData;
            findTotalInOutOverallChart.rjData = rjData;
            findTotalInOutOverallChart.remainData = remainData;


            return Ok(new
            {
                dateSearchOverAll = gocList,
                dataListOverAll = findTotalInOutOverallChart,


            });

        }





        [HttpPost]
        [Route("getDailyBarChartOverAllByWcno")]
        public IActionResult getReportEKB_OverAllByWcno([FromBody] DailyPackingPayload payload)
        {

            List<getLabelsOverallChart> gocList = new List<getLabelsOverallChart>();


            List<findTotalInOutOverallChart> res = new List<findTotalInOutOverallChart>();
            string YM = DateTime.Parse(payload.searchDate).ToString("yyyyMM");
            int _year = DateTime.Parse(payload.searchDate).Year;
            int _month = DateTime.Parse(payload.searchDate).Month;



            int year = DateTime.Now.Year;
            int mont = DateTime.Parse(payload.searchDate).Month;




            // วนลูป เพือหา labels
            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = @"	SELECT 					
                            PARTNO,
							CONVERT(varchar,[ShiftDate],23) as [ShiftDate]
						    
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply]
						     where YM = @YM and wcno = @WCNO and  ShiftDate >=  @Stdate and  ShiftDate <=  @enDate  and PARTNO = @PARTNO
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],23),PARTNO
                             order by ShiftDate";

            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@PARTNO", payload.partNo));

            //sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", payload.stDate));
            //sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", payload.enDate));
            //sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd")));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", new DateTime(_year, _month, 1, 0, 0, 0).ToString("yyyy-MM-dd")));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", DateTime.Now.ToString("yyyy-MM-dd")));
            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);

            if (dtOverAll.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtOverAll.Rows)
                {

                    getLabelsOverallChart goc = new getLabelsOverallChart();

                    goc.partNo = drAll["PARTNO"].ToString();
                    goc.date = drAll["ShiftDate"].ToString();

                    gocList.Add(goc);
                }
            }

            // วนลูป เพือหาข้อมูล IN OUT ทั้งหมด


            List<findTotalInOutOverallChart> FindList = new List<findTotalInOutOverallChart>();

            DataTable dtFinds = new DataTable();

            dtFinds.TableName = "tbDeliverlyPartTime";
            dtFinds.Columns.Add("PARTNO", typeof(string));
            dtFinds.Columns.Add("ShiftDate", typeof(string));
            dtFinds.Columns.Add("IN", typeof(decimal));
            dtFinds.Columns.Add("OUT", typeof(decimal));
            dtFinds.Columns.Add("LBAL", typeof(decimal));



            int i = 0;
            foreach (getLabelsOverallChart item in gocList)
            {
                SqlCommand sql_select_overall_dataList = new SqlCommand();
                sql_select_overall_dataList.CommandText = @"	SELECT						
                            transection.PARTNO,
							CONVERT(varchar,[ShiftDate],3) as [ShiftDate],
						
						
                             SUM([TransQty]) as TotalRound, TransType,wip.LBAL
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply] transection
                             LEFT JOIN [dbSCM].[dbo].[EKB_WIP_Part_Stock] wip on wip.PARTNO = transection.PARTNO and wip.YM = @YM

						     where transection.wcno = @WCNO and  ShiftDate =  @ShiftDate and transection.PARTNO = @PARTNO
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],3),transection.PARTNO,TransType,wip.LBAL
                             order by ShiftDate";

                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@YM", YM));
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@PARTNO", item.partNo));

                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@ShiftDate", DateTime.Parse(item.date).ToString("yyyy-MM-dd")));

                DataTable dtFindTotalInOUT = oCOnnSCM.Query(sql_select_overall_dataList);
                dtFinds.Rows.Add(item.partNo, item.date, 0, 0);

                foreach (DataRow drF in dtFinds.Rows)
                {

                    foreach (DataRow drQuerry in dtFindTotalInOUT.Rows)
                    {

                        findTotalInOutOverallChart _find = new findTotalInOutOverallChart();
                        if (item.partNo == drQuerry["PARTNO"].ToString())
                        {
                            if (drQuerry["TransType"].ToString() == "IN")
                            {

                                dtFinds.Rows[i]["IN"] = drQuerry["TotalRound"];
                                dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];

                            }
                            else
                            {
                                dtFinds.Rows[i]["OUT"] = drQuerry["TotalRound"];
                                dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];
                            }
                        }


                    }



                }

                i++;


            }




            double[] stockLbal = new double[1];
            double[] inData = new double[dtFinds.Rows.Count];
            double[] outData = new double[dtFinds.Rows.Count];
            double[] remainData = new double[dtFinds.Rows.Count];
            List<findTotalInOutOverallChart> _result = new List<findTotalInOutOverallChart>();

            int z = 0;

            foreach (DataRow dc in dtFinds.Rows)
            {

                if (z == 0)
                {
                    inData[z] = Convert.ToDouble(dtFinds.Rows[z]["IN"]);
                    outData[z] = Convert.ToDouble(dtFinds.Rows[z]["OUT"]);
                    stockLbal[z] = Convert.ToDouble(dtFinds.Rows[z]["LBAL"]);
                    remainData[z] = (stockLbal[0] + inData[z]) - outData[z];
                }
                else
                {
                    inData[z] = Convert.ToDouble(dtFinds.Rows[z]["IN"]);
                    outData[z] = Convert.ToDouble(dtFinds.Rows[z]["OUT"]);
                    remainData[z] = (remainData[z - 1] + inData[z]) - outData[z];
                }


                z++;
            }

            findTotalInOutOverallChart findTotalInOutOverallChart = new findTotalInOutOverallChart();

            findTotalInOutOverallChart.stocks = stockLbal;
            findTotalInOutOverallChart.inData = inData;
            findTotalInOutOverallChart.outData = outData;
            findTotalInOutOverallChart.remainData = remainData;


            return Ok(new
            {
                dateSearchOverAll = gocList,
                dataListOverAll = findTotalInOutOverallChart,


            });

        }







        [HttpGet]
        [Route("getDailyCompareTSC/{wcno}/{searchDate}")]
        public IActionResult getAssessmentDataTableCompareTSC(string wcno, string searchDate)
        {
            DateTime dateConvert = DateTime.Parse(searchDate);
            string YM = dateConvert.ToString("yyyyMM");

            List<HeaderDataCompareTSC> headerTSCCompare = new List<HeaderDataCompareTSC>();

            SqlCommand sql_select = new SqlCommand();
            sql_select.CommandText = @"	SELECT [YM]
              ,[WCNO]
              ,[PARTNO]
              ,[CM]
              ,[UpdateDate]
        
  
              FROM [dbSCM].[dbo].[EKB_WIP_Part_Stock]
              WHERE YM = @YM and WCNO = @WCNO
               order by [UpdateDate] desc";

            sql_select.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select.Parameters.Add(new SqlParameter("@WCNO", wcno));

            DataTable sqlSelect = oCOnnSCM.Query(sql_select);

            if (sqlSelect.Rows.Count > 0)
            {
                foreach (DataRow header in sqlSelect.Rows)
                {

                    HeaderDataCompareTSC headerDataCompareTSC = new HeaderDataCompareTSC();
                    headerDataCompareTSC.Partno = header["PARTNO"].ToString();
                    headerDataCompareTSC.Cm = header["CM"].ToString();

                    SqlCommand sql_select_Detail = new SqlCommand();
                    sql_select_Detail.CommandText = @"	SELECT   tsc.[YM],tsc.[PARTNO],tsc.[CM]
                    ,tsc.[WCNO],[LWBAL] as TSC_LBAL ,tsc.[RECQTY] as TSC_RECQTY ,[ISQTY] as TSC_ISQTY ,[BALQTY] as TSC_BAL
                    ,ekb.LBAL as EKB_LBAL,ekb.RECQTY as EKB_RECQTY,ekb.ISSQTY as EKB_ISSQTY,ekb.BAL as EKB_BAL ,[dataDate],[LREV]
                    FROM [dbSCM].[dbo].[AL_Part_Stock_WIP] tsc

                    LEFT JOIN [dbSCM].[dbo].[EKB_WIP_Part_Stock] ekb on ekb.YM = tsc.YM  and ekb.PARTNO = tsc.PARTNO and ekb.CM = tsc.CM
                    where LREV = '999' and tsc.WCNO = @WCNO2 and tsc.YM = @YM2 and tsc.[PARTNO] = @PARTNO and tsc.CM = @CM";

                    sql_select_Detail.Parameters.Add(new SqlParameter("@YM2", header["YM"]));
                    sql_select_Detail.Parameters.Add(new SqlParameter("@WCNO2", header["WCNO"]));
                    sql_select_Detail.Parameters.Add(new SqlParameter("@PARTNO", header["PARTNO"]));
                    sql_select_Detail.Parameters.Add(new SqlParameter("@CM", header["CM"]));
                    DataTable sqlDetail = oCOnnSCM.Query(sql_select_Detail);

                    if (sqlDetail.Rows.Count > 0)
                    {
                        foreach (DataRow detail in sqlDetail.Rows)
                        {
                            List<DetailDataCompareTSC> detailDataCompareTSC_list = new List<DetailDataCompareTSC>();

                            DetailDataCompareTSC detailDataCompareTSC = new DetailDataCompareTSC();
                            detailDataCompareTSC.tsc_lbal = Convert.ToDecimal(detail["TSC_LBAL"]);
                            detailDataCompareTSC.tsc_rec = Convert.ToDecimal(detail["TSC_RECQTY"]);
                            detailDataCompareTSC.tsc_iss = Convert.ToDecimal(detail["TSC_ISQTY"]);
                            detailDataCompareTSC.tsc_bal = Convert.ToDecimal(detail["TSC_BAL"]);


                            detailDataCompareTSC.ekb_lbal = Convert.ToDecimal(detail["EKB_LBAL"]);
                            detailDataCompareTSC.ekb_rec = Convert.ToDecimal(detail["EKB_RECQTY"]);
                            detailDataCompareTSC.ekb_iss = Convert.ToDecimal(detail["EKB_ISSQTY"]);
                            detailDataCompareTSC.ekb_bal = Convert.ToDecimal(detail["EKB_BAL"]);


                            detailDataCompareTSC_list.Add(detailDataCompareTSC);

                            headerDataCompareTSC.DetailTSCData = detailDataCompareTSC_list;
                        }
                    }
                    headerTSCCompare.Add(headerDataCompareTSC);
                }
            }



            return Ok(new
            {
                headerTSCCompareData = headerTSCCompare,
            });

        }





        [HttpGet]
        [Route("getDailyReportData/{wcno}/{searchDate}")]
        public IActionResult getDataINEKB(string wcno, string searchDate)
        {
            DateTime dateConvert = DateTime.Parse(searchDate);
            string YM = dateConvert.ToString("yyyyMM");

            List<DataIN> dataIN_list = new List<DataIN>();
            List<DataIN> dataOUT_list = new List<DataIN>();
            List<DataIN> dataRJ_list = new List<DataIN>();


            SqlCommand sql_select_in = new SqlCommand();
            sql_select_in.CommandText = @"	SELECT [YM]
                  ,[WCNO]
                  ,[PARTNO]
                  ,[CM]
                  ,[TransType]
                  ,[TransQty]
                  ,[QRCodeData]
                  ,[CreateBy]
                  ,[CreateDate]
                  ,[ShiftDate]
                  ,[Shifts]
              FROM [dbSCM].[dbo].[vi_EKB_Transection_DataIN_PS]
              where TransType = 'IN' and WCNO = @WCNO and YM = @YM and ShiftDate = @searchDate
              order by CreateDate";

            sql_select_in.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_in.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select_in.Parameters.Add(new SqlParameter("@searchDate", searchDate));

            DataTable sqlSelectIN = oCOnnSCM.Query(sql_select_in);

            if (sqlSelectIN.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelectIN.Rows)
                {
                    DataIN dn = new DataIN();
                    dn.shiftDate = drIN["CreateDate"].ToString();
                    dn.qrCode = drIN["QRCodeData"].ToString();
                    dn.wcno = drIN["WCNO"].ToString();
                    dn.partNo = drIN["PARTNO"].ToString();
                    dn.cm = drIN["cm"].ToString();
                    dn.TransType = drIN["TransType"].ToString();
                    dn.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dn.createBy = drIN["CreateBy"].ToString();
                    dn.shifts = drIN["Shifts"].ToString();
                    dataIN_list.Add(dn);
                }
            }

            SqlCommand sql_select_out = new SqlCommand();
            sql_select_out.CommandText = @"	SELECT [YM]
                  ,[WCNO]
                  ,[PARTNO]
                  ,[CM]
                  ,[TransType]
                  ,[TransQty]
                  ,[QRCodeData]
                  ,[CreateBy]
                  ,[CreateDate]
                  ,[ShiftDate]
                  ,[Shifts]
              FROM [dbSCM].[dbo].[vi_EKB_Transection_DataIN_PS]
              where TransType = 'OUT' and WCNO = @WCNO and YM = @YM and ShiftDate = @searchDate
              order by CreateDate";

            sql_select_out.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_out.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select_out.Parameters.Add(new SqlParameter("@searchDate", searchDate));

            DataTable sqlSelectOUT = oCOnnSCM.Query(sql_select_out);

            if (sqlSelectOUT.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelectOUT.Rows)
                {
                    DataIN dn = new DataIN();
                    dn.shiftDate = drIN["CreateDate"].ToString();
                    dn.qrCode = drIN["QRCodeData"].ToString();
                    dn.wcno = drIN["WCNO"].ToString();
                    dn.partNo = drIN["PARTNO"].ToString();
                    dn.cm = drIN["cm"].ToString();
                    dn.TransType = drIN["TransType"].ToString();
                    dn.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dn.createBy = drIN["CreateBy"].ToString();
                    dn.shifts = drIN["Shifts"].ToString();
                    dataOUT_list.Add(dn);
                }
            }

            SqlCommand sql_selectRJ = new SqlCommand();
            sql_selectRJ.CommandText = @"	SELECT [YM]
                  ,[WCNO]
                  ,[PARTNO]
                  ,[CM]
                  ,[TransType]
                  ,[TransQty]
                  ,[QRCodeData]
                  ,[CreateBy]
                  ,[CreateDate]
                  ,[ShiftDate]
                  ,[Shifts]
              FROM [dbSCM].[dbo].[vi_EKB_Transection_DataIN_PS]
              where TransType = 'RJ' and WCNO = @WCNO and YM = @YM and ShiftDate = @searchDate
              order by CreateDate";

            sql_selectRJ.Parameters.Add(new SqlParameter("@YM", YM));
            sql_selectRJ.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_selectRJ.Parameters.Add(new SqlParameter("@searchDate", searchDate));

            DataTable sqlSelectRJ = oCOnnSCM.Query(sql_selectRJ);

            if (sqlSelectRJ.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelectRJ.Rows)
                {
                    DataIN dn = new DataIN();
                    dn.shiftDate = drIN["CreateDate"].ToString();
                    dn.qrCode = drIN["QRCodeData"].ToString();
                    dn.wcno = drIN["WCNO"].ToString();
                    dn.partNo = drIN["PARTNO"].ToString();
                    dn.cm = drIN["cm"].ToString();
                    dn.TransType = drIN["TransType"].ToString();
                    dn.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dn.createBy = drIN["CreateBy"].ToString();
                    dn.shifts = drIN["Shifts"].ToString();
                    dataRJ_list.Add(dn);
                }
            }

            return Ok(new
            {
                dataIN = dataIN_list,
                dataOut = dataOUT_list,
                dataRJ = dataRJ_list
            });
        }

        // GET api/<DataINOUTController>/5
        //[HttpGet]
        //[Route("getDailyDataOut/{wcno}/{searchDate}")]
        //public IActionResult getDataOutEKB(string wcno, string searchDate)
        //{
        //    DateTime dateConvert = DateTime.Parse(searchDate);
        //    string YM = dateConvert.ToString("yyyyMM");

        //    List<DataOUT> dataINOUT_list = new List<DataOUT>();

        //    SqlCommand sql_select = new SqlCommand();
        //    sql_select.CommandText = @"	SELECT [YM]
        //          ,[WCNO]
        //          ,[PARTNO]
        //          ,[CM]
        //          ,[TransType]
        //          ,[TransQty]
        //          ,[QRCodeData]
        //          ,[CreateBy]
        //          ,[CreateDate]
        //          ,[ShiftDate]
        //          ,[Shifts]
        //      FROM [dbSCM].[dbo].[vi_EKB_Transection_DataIN_PS]
        //      where [ShiftDate] =  @stDate and TransType = 'OUT' and WCNO like @WCNO and YM = @YM";

        //    sql_select.Parameters.Add(new SqlParameter("@YM", YM));
        //    sql_select.Parameters.Add(new SqlParameter("@WCNO", wcno));
        //    sql_select.Parameters.Add(new SqlParameter("@stDate", searchDate));


        //    DataTable sqlSelect = oCOnnSCM.Query(sql_select);

        //    if (sqlSelect.Rows.Count > 0)
        //    {
        //        foreach (DataRow drIN in sqlSelect.Rows)
        //        {
        //            DataOUT dout = new DataOUT();

        //            dout.shiftDate = Convert.ToDateTime(drIN["CreateDate"]).ToString();
        //            dout.partNo = drIN["PARTNO"].ToString();
        //            dout.wcno = drIN["WCNO"].ToString();
        //            dout.partNo = drIN["PARTNO"].ToString();
        //            dout.cm = drIN["CM"].ToString();
        //            dout.transType = drIN["TransType"].ToString();
        //            dout.transQty = Convert.ToDecimal(drIN["TransQty"]);
        //            dout.qrCode = drIN["QRCodeData"].ToString();
        //            dout.createBy = drIN["CreateBy"].ToString();
        //            dout.shifts = drIN["Shifts"].ToString();



        //            dataINOUT_list.Add(dout);
        //        }
        //    }

        //    return Ok(new
        //    {
        //        dataOut = dataINOUT_list,
        //    }); ;
        //}



        [HttpPost]
        [Route("getDailyTargetAndResult")]
        public IActionResult getTargetAndResult([FromBody] DailyPackingPayload payload)
        {

            string[] wcnos = { "306", "308", "309", "310", "312", "314" };
            //string[] wcnos = { "314" };
            List<String> _partnos = new List<string>();


     

            string YM = DateTime.Parse(payload.searchDate).ToString("yyyyMM");

            int year = DateTime.Now.Year;
            int mont = DateTime.Parse(payload.searchDate).Month;




            List<ResultTargetDaily> _mainTartgetDaily = new List<ResultTargetDaily>();
            List<findDailyOfResultAndTargets> _findDailyResultandTarget = new List<findDailyOfResultAndTargets>();

            foreach (string wcon in wcnos)
            {
                List<getLabelsOverallChart> gocList = new List<getLabelsOverallChart>();

                DataTable dtFinds = new DataTable();

                dtFinds.TableName = "tbDeliverlyPartTime";
                dtFinds.Columns.Add("WCNO", typeof(string));
                dtFinds.Columns.Add("PARTNO", typeof(string));
                dtFinds.Columns.Add("ShiftDate", typeof(string));
                dtFinds.Columns.Add("IN", typeof(decimal));
                dtFinds.Columns.Add("OUT", typeof(decimal));
                dtFinds.Columns.Add("RJ", typeof(decimal));
                dtFinds.Columns.Add("LBAL", typeof(decimal));


                SqlCommand sql_select_partno = new SqlCommand();
                sql_select_partno.CommandText = @"	SELECT PARTNO
                                                        FROM EKB_WIP_Part_Stock
                                                        where YM = @YM and WCNO = @WCNO and
                                                    (PartDesc NOT IN ('UPPER PISTON SINTERED(ST-0859)', 'LOWER PISTON SINTERED(ST-0860)', 'UPPER CYLINDER MACHINING','UPPER PISTON SINTERED(ST-0452)', 'LOWER CYLINDER MACHINING','LOWER PISTON SINTERED(ST-0453)','LOWER PISTON SINTERED(ST-0872)','UPPER PISTON SINTERED(ST-0871)'))
                                                        order by UpdateDate desc";

                sql_select_partno.Parameters.Add(new SqlParameter("@YM", YM));
                sql_select_partno.Parameters.Add(new SqlParameter("@WCNO", wcon));

                DataTable dtSelectPartno = oCOnnSCM.Query(sql_select_partno);

                if (dtSelectPartno.Rows.Count > 0)
                {
                    foreach (DataRow drAll in dtSelectPartno.Rows)
                    {

                        getLabelsOverallChart goc = new getLabelsOverallChart();
                        goc.date = Convert.ToDateTime(payload.searchDate).ToString("yyyy-MM-dd");
                        goc.partNo = drAll["PARTNO"].ToString();
                        gocList.Add(goc);

                    }
                }



                int countRound = 0;
                int i = 0;

                // วนลูป เพือหา labels

       
                foreach (getLabelsOverallChart item in gocList)
                {

                    decimal lbal = 0;
                    SqlCommand sql_select_LBAL = new SqlCommand();
                    sql_select_LBAL.CommandText = @"	SELECT LBAL
                                                    FROM EKB_WIP_Part_Stock
                                                    where YM = @YM and WCNO = @WCNO and PARTNO = @PARTNO and
                                                    (PartDesc NOT IN ('UPPER PISTON SINTERED(ST-0859)', 'LOWER PISTON SINTERED(ST-0860)', 'UPPER CYLINDER MACHINING','UPPER PISTON SINTERED(ST-0452)', 'LOWER CYLINDER MACHINING','LOWER PISTON SINTERED(ST-0453)','LOWER PISTON SINTERED(ST-0872)','UPPER PISTON SINTERED(ST-0871)'))
                                                    order by UpdateDate desc";

                    sql_select_LBAL.Parameters.Add(new SqlParameter("@YM", YM));
                    sql_select_LBAL.Parameters.Add(new SqlParameter("@WCNO", wcon));
                    sql_select_LBAL.Parameters.Add(new SqlParameter("@PARTNO", item.partNo));

                    DataTable dtSelectLBAL = oCOnnSCM.Query(sql_select_LBAL);

                    if (dtSelectLBAL.Rows.Count > 0)
                    {
                        foreach (DataRow drAll in dtSelectLBAL.Rows)
                        {


                            lbal = Convert.ToDecimal(drAll["LBAL"]);

                        }
                    }


                    SqlCommand sql_select_overall_dataList = new SqlCommand();
                    sql_select_overall_dataList.CommandText = @"	SELECT						
                            transection.PARTNO,
							CONVERT(varchar,[ShiftDate],3) as [ShiftDate],
						
						
                             SUM([TransQty]) as TotalRound, TransType,wip.LBAL
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection] transection
                             LEFT JOIN [dbSCM].[dbo].[EKB_WIP_Part_Stock] wip on wip.PARTNO = transection.PARTNO and wip.YM = @YM

						     where transection.wcno = @WCNO and  ShiftDate =  @ShiftDate and transection.PARTNO = @PARTNO and transection.YM = @YM
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],3),transection.PARTNO,TransType,wip.LBAL
                             order by ShiftDate";

                    sql_select_overall_dataList.Parameters.Add(new SqlParameter("@YM", YM));
                    sql_select_overall_dataList.Parameters.Add(new SqlParameter("@WCNO", wcon));
                    sql_select_overall_dataList.Parameters.Add(new SqlParameter("@PARTNO", item.partNo));

                    sql_select_overall_dataList.Parameters.Add(new SqlParameter("@ShiftDate", DateTime.Parse(item.date).ToString("yyyy-MM-dd")));

                    DataTable dtFindTotalInOUT = oCOnnSCM.Query(sql_select_overall_dataList);
                    dtFinds.Rows.Add(wcon, item.partNo, item.date, 0, 0, 0);
                    if (dtFindTotalInOUT.Rows.Count > 0)
                    {
                        foreach (DataRow drF in dtFinds.Rows)
                        {

                            foreach (DataRow drQuerry in dtFindTotalInOUT.Rows)
                            {

                                findTotalInOutOverallChart _find = new findTotalInOutOverallChart();
                                if (item.partNo == drQuerry["PARTNO"].ToString())
                                {
                                    if (drQuerry["TransType"].ToString() == "IN")
                                    {

                                        dtFinds.Rows[i]["IN"] = drQuerry["TotalRound"];
                                        dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];

                                    }
                                    else if (drQuerry["TransType"].ToString() == "OUT")
                                    {
                                        dtFinds.Rows[i]["OUT"] = drQuerry["TotalRound"];
                                        dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];
                                    }
                                    else
                                    {
                                        dtFinds.Rows[i]["RJ"] = drQuerry["TotalRound"] == "" ? 0 : drQuerry["TotalRound"];
                                        dtFinds.Rows[i]["LBAL"] = drQuerry["LBAL"];
                                    }
                                }


                            }


                        }

                      
                    }

                    decimal balStock = curentBal(wcon, item.partNo, payload.searchDate); // ยอด BAL ปัจจุบัน
                    decimal LBAL = FindLBAL_BeforeOneDay(wcon,item.partNo,payload.searchDate, balStock , lbal); // ยอด BAL ของวันก่อนหน้า


                    if(DateTime.Now.Day == 1)
                    {
                        dtFinds.Rows[i]["LBAL"] = lbal;


                    }
                    else
                    {
                        dtFinds.Rows[i]["LBAL"] = (dtFinds.Rows[i]["LBAL"] == DBNull.Value) ? LBAL : LBAL + Convert.ToDecimal(dtFinds.Rows[i]["IN"]) - Convert.ToDecimal(dtFinds.Rows[i]["RJ"]) - Convert.ToDecimal(dtFinds.Rows[i]["OUT"]);

                    }






                    i++;


                }


                List<ResultTargetDaily> resultsAllTarget = CalResultAndTarget(dtFinds, wcon, payload.searchDate);



                // เก็บข้อมูลแต่ละ drawing ตาม WCNO 
                foreach (DataRow data in dtFinds.Rows)
                {
                    findDailyOfResultAndTargets findDailyresultTargetModel = new findDailyOfResultAndTargets();
                    findDailyresultTargetModel.partDesc = resultsAllTarget.FirstOrDefault().partDesc;
                    findDailyresultTargetModel.wcno = data["WCNO"].ToString();
                    findDailyresultTargetModel.PARTNO = data["PARTNO"].ToString();
                    findDailyresultTargetModel.ShiftDate = data["ShiftDate"].ToString();
                    findDailyresultTargetModel.IN = Convert.ToDecimal(data["IN"]);
                    findDailyresultTargetModel.OUT = Convert.ToDecimal(data["OUT"]);
                    findDailyresultTargetModel.RJ = Convert.ToDecimal(data["RJ"]);
                    findDailyresultTargetModel.LBAL = Convert.ToDecimal(data["LBAL"]);
                    _findDailyResultandTarget.Add(findDailyresultTargetModel);
                }


                // เก็บข้อมูลแต่ละ line 
                foreach (ResultTargetDaily data in resultsAllTarget)
                {
                    ResultTargetDaily _resultTarget = new ResultTargetDaily();
                    _resultTarget.shiftDate = data.shiftDate;
                    _resultTarget.wcno = data.wcno;
                    _resultTarget.partDesc = data.partDesc;
                    _resultTarget.target = data.target;
                    _resultTarget.actual = data.actual;
                    _mainTartgetDaily.Add(_resultTarget);
                }
              



                


            }
            return Ok(new { resultsAllTarget = _mainTartgetDaily,findPartnoByWCNO = _findDailyResultandTarget });

        }



        private List<ResultTargetDaily> CalResultAndTarget(DataTable resultLBAL, string wcno, string searchDate)
        {
            List<ResultTargetDaily> targetList = new List<ResultTargetDaily>();

            //DateTime stDates = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //DateTime enDates = stDates.AddMonths(1).AddDays(-1);

            SqlCommand sql_select_targetResult = new SqlCommand();
            sql_select_targetResult.CommandText = @"SELECT  WCNO, PartDesc,MaxEKBStock as Targets
                                                                FROM  EKB_WIP_Part_Master
                                                                WHERE WCNO = @WCNOS 
                                                                AND (PartDesc NOT IN ('UPPER PISTON SINTERED(ST-0859)', 'LOWER PISTON SINTERED(ST-0860)', 'UPPER CYLINDER MACHINING','UPPER PISTON SINTERED(ST-0452)', 'LOWER CYLINDER MACHINING','LOWER PISTON SINTERED(ST-0453)','LOWER PISTON SINTERED(ST-0872)','UPPER PISTON SINTERED(ST-0871)'))
                                                                GROUP BY  WCNO,  PartDesc,MaxEKBStock";
            sql_select_targetResult.Parameters.Add(new SqlParameter("@WCNOS", wcno));

            DataTable dtTargetResult = oCOnnSCM.Query(sql_select_targetResult);


            if (dtTargetResult.Rows.Count > 0)
            {
                foreach (DataRow dtTarget in dtTargetResult.Rows)
                {



                    ResultTargetDaily resultTarget = new ResultTargetDaily();
                    resultTarget.shiftDate = Convert.ToDateTime(searchDate).ToString("dd/MM/yyyy");
                    resultTarget.wcno = dtTarget["WCNO"].ToString();
                    resultTarget.partDesc = dtTarget["PartDesc"].ToString();
                    resultTarget.target = Convert.ToDecimal(dtTarget["Targets"]);
                    resultTarget.actual = resultLBAL.AsEnumerable().Where(x => x.Field<string>("ShiftDate") == Convert.ToDateTime(searchDate).ToString("yyyy-MM-dd"))
                                                   .Sum(x => x.Field<decimal>("LBAL"));

                    targetList.Add(resultTarget);




                }
            }


            return targetList;

        }



        private DataTable GetData(string stDateConvert, string shift, string wcno, string partno)
        {

            string YM = DateTime.Parse(stDateConvert).ToString("yyyyMM");
            DataTable dtTimeIN = new DataTable();



            dtTimeIN.TableName = "tbDeliverlyPartTime";
            dtTimeIN.Columns.Add("TIME_ROUND", typeof(string));
            dtTimeIN.Columns.Add("QTY_IN", typeof(decimal));
            dtTimeIN.Columns.Add("QTY_OUT", typeof(decimal));

            if (shift == "D")
            {

                dtTimeIN.Rows.Add("11:00", 0, 0);
                dtTimeIN.Rows.Add("13:00", 0, 0);
                dtTimeIN.Rows.Add("15:00", 0, 0);
                dtTimeIN.Rows.Add("17:00", 0, 0);
                dtTimeIN.Rows.Add("19:00", 0, 0);
                dtTimeIN.Rows.Add("21:00", 0, 0);

            }
            else
            {
                dtTimeIN.Rows.Add("23:00", 0, 0);
                dtTimeIN.Rows.Add("01:00", 0, 0);
                dtTimeIN.Rows.Add("03:00", 0, 0);
                dtTimeIN.Rows.Add("05:00", 0, 0);
                dtTimeIN.Rows.Add("07:00", 0, 0);
                dtTimeIN.Rows.Add("09:00", 0, 0);


            }


            string[] type = { "IN", "OUT" };
            for (int j = 0; j < type.Length; j++)
            {
                SqlCommand sql_select_D = new SqlCommand();
                sql_select_D.CommandText = @"	SELECT						
                            PARTNO,
							CONVERT(varchar,[ShiftDate],3) as [ShiftDate],
							[TIME_ROUND], 
							[TransType]
                             ,SUM([TransQty]) as TotalRound
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply]
						     where YM = @YM and wcno = @WCNO and  ShiftDate =  @shiftdate  and Shifts = @SHIFT and TransType = @type and
                              PARTNO = @PARTNO
                        
                             GROUP BY [TransType],[TIME_ROUND] ,CONVERT(varchar,[ShiftDate],3),[YM],PARTNO
                             order by ShiftDate";

                sql_select_D.Parameters.Add(new SqlParameter("@YM", YM));
                sql_select_D.Parameters.Add(new SqlParameter("@SHIFT", shift));
                sql_select_D.Parameters.Add(new SqlParameter("@shiftdate", stDateConvert));
                sql_select_D.Parameters.Add(new SqlParameter("@WCNO", wcno));
                sql_select_D.Parameters.Add(new SqlParameter("@PARTNO", partno));
                sql_select_D.Parameters.Add(new SqlParameter("@type", type[j]));
                DataTable dtDay = oCOnnSCM.Query(sql_select_D);

                if (dtDay.Rows.Count > 0)
                {
                    foreach (DataRow drdefault in dtTimeIN.Rows)
                    {
                        foreach (DataRow drQuerry in dtDay.Rows)
                        {
                            if (drdefault["TIME_ROUND"].ToString() == drQuerry["TIME_ROUND"].ToString())
                            {
                                if (type[j] == "IN")
                                {
                                    drdefault["QTY_IN"] = drQuerry["TotalRound"];
                                    break;
                                }
                                else
                                {
                                    drdefault["QTY_OUT"] = drQuerry["TotalRound"];
                                    break;
                                }

                            }

                        }
                    }
                }


            }
            return dtTimeIN;
        }



        private Decimal FindLBAL_BeforeOneDay(string wcno , string partno , string searchDate , decimal balStock , decimal lbalStock)
        {
            DateTime stDate = new DateTime(DateTime.Parse(searchDate).Year, DateTime.Parse(searchDate).Month, DateTime.Parse(searchDate).Day, 08, 0, 0);
            DateTime enDate = DateTime.Now;

            //หายอดตั้งต้น

            decimal newLbalStock = balStock;
            decimal inoutStock = 0;
            string typeStock = "";

            SqlCommand sqlSelect = new SqlCommand();
            sqlSelect.CommandText = @"
									 SELECT TransType, SUM(TransQty) TotalQty  FROM [dbSCM].[dbo].[vi_EKB_Transection_2]
									 where CreateDate >= @stDate and CreateDate <= @enDate and PARTNO = @PARTNO AND YM = @YM
									 GROUP BY TransType";

            sqlSelect.Parameters.Add(new SqlParameter("@YM", stDate.ToString("yyyyMM")));
            sqlSelect.Parameters.Add(new SqlParameter("@PARTNO", partno));
            sqlSelect.Parameters.Add(new SqlParameter("@stDate", stDate));
            sqlSelect.Parameters.Add(new SqlParameter("@enDate", enDate));

            DataTable dtQueryData = oCOnnSCM.Query(sqlSelect);

            if (dtQueryData.Rows.Count > 0)
            {
                foreach (DataRow drQuerry in dtQueryData.Rows)
                {

                    inoutStock = Convert.ToDecimal(drQuerry["TotalQty"]);

                    typeStock = drQuerry["TransType"].ToString();


                    newLbalStock = typeStock == "IN" ? balStock - inoutStock : newLbalStock + inoutStock;
                }
            }
            //else
            //{
            //    newLbalStock = lbalStock;
            //}

            return newLbalStock;
        }

        private decimal curentBal(string wcno ,string partno ,string searchdate)
        {
            decimal bal = 0;
            string ym = DateTime.Parse(searchdate).ToString("yyyyMM");

            SqlCommand sqlSelectBal = new SqlCommand();
            sqlSelectBal.CommandText = @"SELECT [BAL]
                                         FROM [dbSCM].[dbo].[EKB_WIP_Part_Stock]
                                         where YM = @YM and  WCNO = @WCNO and PARTNO = @PARTNO";
            sqlSelectBal.Parameters.Add(new SqlParameter("@YM", ym));
            sqlSelectBal.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sqlSelectBal.Parameters.Add(new SqlParameter("@PARTNO", partno));

            DataTable dtQueryData = oCOnnSCM.Query(sqlSelectBal);

            if (dtQueryData.Rows.Count > 0)
            {
                foreach (DataRow drQuerry in dtQueryData.Rows)
                {
                    bal = Convert.ToDecimal(drQuerry["BAL"]);
                }
            }
            return bal;
        }

        private class findDayOfResultAndTarget
        {
            public string? PARTNO { get; set; }
            public string? ShiftDate { get; set; }
            public decimal? IN { get; set; }
            public decimal? OUT { get; set; }
            public decimal? RJ { get; set; }
            public decimal? LBAL { get; set; }

        }
    }
}
