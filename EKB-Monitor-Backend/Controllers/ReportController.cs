using EKANBAN.Service;
using EKB_Monitor_Backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using static EKB_Monitor_Backend.Model.DataCompareTSCAlpha;
using static EKB_Monitor_Backend.Model.OverAllBarChart;

namespace EKB_Monitor_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {

        private ConnectDB oCOnnSCM = new ConnectDB("DBSCM");
        DateTime current = DateTime.Now;

        [HttpPost]
        [Route("getAssessmentDashboardBarChartDay")]
        public IActionResult getReportEKB_Day([FromBody] PackingPayload payload)
        {
            List<StackDataModel> res = new List<StackDataModel>();

            int i = 0;

            string[] type = { "IN", "OUT" };


            //DateTime stDateConvert = DateTime.ParseExact(payload.stDate, "yyyy-MM-dd",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            //DateTime enDateConvert = DateTime.ParseExact(payload.enDate, "yyyy-MM-dd",
            //                    System.Globalization.CultureInfo.InvariantCulture);

            //DateTime dateConvert = new DateTime(int.Parse(payload.stDate.Substring(0,4)), int.Parse(payload.stDate.Substring(5, 6)), 1, 0, 0, 0);
            DateTime dateConvert = DateTime.Parse(payload.stDate);



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
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", payload.stDate));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", payload.enDate));
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
                DataTable dtTime = GetData(searchDateDayNight.date, "D", payload.wcno, searchDateDayNight.partNo);
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



        [HttpPost]
        [Route("getAssessmentDashboardBarChartNight")]
        public IActionResult getReportEKB_Night([FromBody] PackingPayload payload)
        {
            List<StackDataModel> res = new List<StackDataModel>();

            int i = 0;


            string[] type = { "IN", "OUT" };


            //DateTime stDateConvert = DateTime.ParseExact(payload.stDate, "yyyy-MM-dd",
            //                           System.Globalization.CultureInfo.InvariantCulture);

            //DateTime enDateConvert = DateTime.ParseExact(payload.enDate, "yyyy-MM-dd",
            //                    System.Globalization.CultureInfo.InvariantCulture);

            DateTime dateConvert = DateTime.Parse(payload.stDate);
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
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@Stdate", payload.stDate));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@enDate", payload.enDate));
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


                DataTable dtTime = GetData(DateTime.Parse(searchDateDayNight.date).ToString("yyyy-MM-dd"), "N", payload.wcno, searchDateDayNight.partNo);
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




        [HttpPost]
        [Route("getAssessmentDashboardBarChartOverAll")]
        public IActionResult getReportEKB_OverAll([FromBody] PackingPayload payload)
        {



            List<getLabelsOverallChart> gocList = new List<getLabelsOverallChart>();


            List<findTotalInOutOverallChart> res = new List<findTotalInOutOverallChart>();
            string YM = DateTime.Parse(payload.stDate).ToString("yyyyMM");

            int year = DateTime.Now.Year;
            int mont = DateTime.Parse(payload.stDate).Month;


            //DataTable getLbals = getLbal(payload, YM);

            string partno = "";

            SqlCommand sql_select_partno = new SqlCommand();
            sql_select_partno.CommandText = @"	SELECT TOP(1) PARTNO
                                                    FROM EKB_WIP_Part_Stock
                                                    where YM = @YM and WCNO = @WCNO
                                                    order by UpdateDate desc";

            sql_select_partno.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_partno.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));

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
						
						
                        
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection]
						     where YM = @YM and wcno = @WCNO and  ShiftDate >=  @Stdate and  ShiftDate <=  @enDate and PARTNO = @PARTNO
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],23),PARTNO
                             order by ShiftDate";

            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
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
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection] transection
                             LEFT JOIN [dbSCM].[dbo].[EKB_WIP_Part_Stock] wip on wip.PARTNO = transection.PARTNO and wip.YM = @YM

						     where transection.wcno = @WCNO and  ShiftDate =  @ShiftDate and transection.PARTNO = @PARTNO and transection.YM =  @YM
						    
                        
                             GROUP BY CONVERT(varchar,[ShiftDate],3),transection.PARTNO,TransType,wip.LBAL
                             order by ShiftDate";

                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@YM", YM));
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
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
        [Route("getAssessmentDashboardBarChartOverAllByWcno")]
        public IActionResult getReportEKB_OverAllByWcno([FromBody] PackingPayload payload)
        {



            List<getLabelsOverallChart> gocList = new List<getLabelsOverallChart>();


            List<findTotalInOutOverallChart> res = new List<findTotalInOutOverallChart>();
            string YM = DateTime.Parse(payload.stDate).ToString("yyyyMM");
            int _year = DateTime.Parse(payload.stDate).Year;
            int _month = DateTime.Parse(payload.stDate).Month;



            int year = DateTime.Now.Year;
            int mont = DateTime.Parse(payload.stDate).Month;




            // วนลูป เพือหา labels
            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = @"	SELECT 					
                            PARTNO,
							CONVERT(varchar,[ShiftDate],23) as [ShiftDate]
						    
                             FROM [dbSCM].[dbo].[vi_EKB_Transection]
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
      
                             FROM [dbSCM].[dbo].[vi_EKB_Transection] transection
                             LEFT JOIN [dbSCM].[dbo].[EKB_WIP_Part_Stock] wip on wip.PARTNO = transection.PARTNO and wip.YM = @YM

						     where transection.wcno = @WCNO and  ShiftDate =  @ShiftDate and transection.PARTNO = @PARTNO and transection.YM = @YM
						    
                        
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
        [Route("getAssessmentDataTableCompareTSC/{wcno}/{ym}")]
        public IActionResult getAssessmentDataTableCompareTSC(string wcno, string ym)
        {

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

            //sql_select.Parameters.Add(new SqlParameter("@YM", DateTime.Now.ToString("yyyyMM")));
            sql_select.Parameters.Add(new SqlParameter("@YM", ym));
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
                    where LREV = '999' and tsc.WCNO = @WCNO2 and tsc.YM = @YM2 and tsc.[PARTNO] = @PARTNO and tsc.CM = @CM ";

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
        [Route("getAssessmentDataExportExcel/{wcno}/{stDate}/{enDate}")]
        public IActionResult getDataINEKB(string wcno, string stDate, string enDate)
        {
            DateTime dateConvert = DateTime.Parse(stDate);
            string YM = dateConvert.ToString("yyyyMM");

            List<DataOUT> dataIN_list = new List<DataOUT>();

            SqlCommand sql_select = new SqlCommand();
            //     sql_select.CommandText = @"	SELECT
            //                     WCNO,
            //                     PARTNO,
            //                     CM,
            //                     Shifts,
            //CONVERT(varchar,[ShiftDate],3) as [ShiftDate],
            //[TIME_ROUND], 
            //[TransType]
            //                      ,SUM([TransQty]) as TotalRound
            // ,CreateBy

            //                      FROM [dbSCM].[dbo].[vi_EKB_Transection_PartSupply]
            //    where YM = @YM and wcno = @WCNO and   ShiftDate >=  @stDate and  ShiftDate <=  @enDate  and TransType = 'IN' 

            //                      GROUP BY [TransType],[TIME_ROUND] ,CONVERT(varchar,[ShiftDate],3),[YM],PARTNO,CreateBy,WCNO,CM,Shifts
            //                      order by CONVERT(varchar,[ShiftDate],3
            //                      

            sql_select.CommandText = @"	SELECT  [YM],[WCNO],[PARTNO],[CM],[TransType],[TransQty],[QRCodeData]
                                        ,[CreateBy],[CreateDate],[ShiftDate],[Shifts]
                                        FROM [dbSCM].[dbo].[vi_EKB_Transection_DataIN_PS]
                                        where YM = @YM and wcno = @WCNO and   ShiftDate >=  @stDate and  ShiftDate <=  @enDate  and TransType = 'IN'";

            sql_select.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select.Parameters.Add(new SqlParameter("@stDate", stDate));
            sql_select.Parameters.Add(new SqlParameter("@enDate", enDate));


            DataTable sqlSelect = oCOnnSCM.Query(sql_select);

            if (sqlSelect.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelect.Rows)
                {
                    DataOUT dout = new DataOUT();

                    //DataIN dn = new DataIN();
                    //dn.shiftDate = drIN["ShiftDate"].ToString();
                    //dn.wcno = drIN["WCNO"].ToString();
                    //dn.partNo = drIN["PARTNO"].ToString();
                    //dn.cm = drIN["cm"].ToString();
                    //dn.TransType = drIN["TransType"].ToString();
                    //dn.timeRound = drIN["TIME_ROUND"].ToString();
                    //dn.totalRound = Convert.ToDecimal(drIN["TotalRound"]);
                    //dn.createBy = drIN["CreateBy"].ToString();
                    //dn.shifts = drIN["Shifts"].ToString();
                    //dataINOUT_list.Add(dn);
                    dout.shiftDate = Convert.ToDateTime(drIN["CreateDate"]).ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.wcno = drIN["WCNO"].ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.cm = drIN["CM"].ToString();
                    dout.transType = drIN["TransType"].ToString();
                    dout.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dout.qrCode = drIN["QRCodeData"].ToString();
                    dout.createBy = drIN["CreateBy"].ToString();
                    dout.shifts = drIN["Shifts"].ToString();
                    dataIN_list.Add(dout);
                }
            }



            List<DataOUT> dataOUT_list = new List<DataOUT>();

            SqlCommand sql_selectOut = new SqlCommand();
            sql_selectOut.CommandText = @"	SELECT [YM]
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
              where  ShiftDate >=  @stDate and  ShiftDate <=  @enDate  and TransType = 'OUT' and WCNO =@WCNO and YM = @YM
              order by CreateDate";

            sql_selectOut.Parameters.Add(new SqlParameter("@YM", YM));
            sql_selectOut.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_selectOut.Parameters.Add(new SqlParameter("@stDate", stDate));
            sql_selectOut.Parameters.Add(new SqlParameter("@enDate", enDate));


            DataTable sqlSelectOut = oCOnnSCM.Query(sql_selectOut);

            if (sqlSelectOut.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelectOut.Rows)
                {
                    DataOUT dout = new DataOUT();

                    dout.shiftDate = Convert.ToDateTime(drIN["CreateDate"]).ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.wcno = drIN["WCNO"].ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.cm = drIN["CM"].ToString();
                    dout.transType = drIN["TransType"].ToString();
                    dout.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dout.qrCode = drIN["QRCodeData"].ToString();
                    dout.createBy = drIN["CreateBy"].ToString();
                    dout.shifts = drIN["Shifts"].ToString();



                    dataOUT_list.Add(dout);
                }
            }



            List<DataOUT> dataRJ_list = new List<DataOUT>();

            SqlCommand sql_selectRj = new SqlCommand();
            sql_selectRj.CommandText = @"	SELECT [YM]
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
              where  ShiftDate >=  @stDate and  ShiftDate <=  @enDate  and TransType = 'RJ' and WCNO =@WCNO and YM = @YM
              order by CreateDate";

            sql_selectRj.Parameters.Add(new SqlParameter("@YM", YM));
            sql_selectRj.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_selectRj.Parameters.Add(new SqlParameter("@stDate", stDate));
            sql_selectRj.Parameters.Add(new SqlParameter("@enDate", enDate));


            DataTable sqlSelectRJ = oCOnnSCM.Query(sql_selectRj);

            if (sqlSelectRJ.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelectRJ.Rows)
                {
                    DataOUT dout = new DataOUT();

                    dout.shiftDate = Convert.ToDateTime(drIN["CreateDate"]).ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.wcno = drIN["WCNO"].ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.cm = drIN["CM"].ToString();
                    dout.transType = drIN["TransType"].ToString();
                    dout.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dout.qrCode = drIN["QRCodeData"].ToString();
                    dout.createBy = drIN["CreateBy"].ToString();
                    dout.shifts = drIN["Shifts"].ToString();



                    dataRJ_list.Add(dout);
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
        [HttpGet]
        [Route("getAssessmentDataOut/{wcno}/{stDate}/{enDate}")]
        public IActionResult getDataOutEKB(string wcno, string stDate, string enDate)
        {
            DateTime dateConvert = DateTime.Parse(stDate);
            string YM = dateConvert.ToString("yyyyMM");

            List<DataOUT> dataINOUT_list = new List<DataOUT>();

            SqlCommand sql_select = new SqlCommand();
            sql_select.CommandText = @"	SELECT [YM]
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
              where  ShiftDate >=  @stDate and  ShiftDate <=  @enDate  and TransType = 'OUT' and WCNO =@WCNO and YM = @YM
              order by CreateDate";

            sql_select.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select.Parameters.Add(new SqlParameter("@stDate", stDate));
            sql_select.Parameters.Add(new SqlParameter("@enDate", enDate));


            DataTable sqlSelect = oCOnnSCM.Query(sql_select);

            if (sqlSelect.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelect.Rows)
                {
                    DataOUT dout = new DataOUT();

                    dout.shiftDate = Convert.ToDateTime(drIN["CreateDate"]).ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.wcno = drIN["WCNO"].ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.cm = drIN["CM"].ToString();
                    dout.transType = drIN["TransType"].ToString();
                    dout.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dout.qrCode = drIN["QRCodeData"].ToString();
                    dout.createBy = drIN["CreateBy"].ToString();
                    dout.shifts = drIN["Shifts"].ToString();



                    dataINOUT_list.Add(dout);
                }
            }

            return Ok(new
            {
                dataOut = dataINOUT_list,
            }); ;
        }


        [HttpGet]
        [Route("getAssessmentDataRJ/{wcno}/{stDate}/{enDate}")]
        public IActionResult getDataRJEKB(string wcno, string stDate, string enDate)
        {
            DateTime dateConvert = DateTime.Parse(stDate);
            string YM = dateConvert.ToString("yyyyMM");

            List<DataOUT> dataRJ_list = new List<DataOUT>();

            SqlCommand sql_select = new SqlCommand();
            sql_select.CommandText = @"	SELECT [YM]
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
              where  ShiftDate >=  @stDate and  ShiftDate <=  @enDate  and TransType = 'RJ' and WCNO =@WCNO and YM = @YM
              order by CreateDate";

            sql_select.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select.Parameters.Add(new SqlParameter("@WCNO", wcno));
            sql_select.Parameters.Add(new SqlParameter("@stDate", stDate));
            sql_select.Parameters.Add(new SqlParameter("@enDate", enDate));


            DataTable sqlSelect = oCOnnSCM.Query(sql_select);

            if (sqlSelect.Rows.Count > 0)
            {
                foreach (DataRow drIN in sqlSelect.Rows)
                {
                    DataOUT dout = new DataOUT();

                    dout.shiftDate = Convert.ToDateTime(drIN["CreateDate"]).ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.wcno = drIN["WCNO"].ToString();
                    dout.partNo = drIN["PARTNO"].ToString();
                    dout.cm = drIN["CM"].ToString();
                    dout.transType = drIN["TransType"].ToString();
                    dout.transQty = Convert.ToDecimal(drIN["TransQty"]);
                    dout.qrCode = drIN["QRCodeData"].ToString();
                    dout.createBy = drIN["CreateBy"].ToString();
                    dout.shifts = drIN["Shifts"].ToString();



                    dataRJ_list.Add(dout);
                }
            }

            return Ok(new
            {
                dataRJ = dataRJ_list,
            }); ;
        }



        [HttpGet]
        [Route("getPartnoByWCNO/{wcno}")]
        public IActionResult getPartnoByWcno(string wcno)
        {


            int i = 0;
            SqlCommand sql_select = new SqlCommand();
            sql_select.CommandText = @"	SELECT  [YM],[WCNO],[PARTNO],[CM]

              FROM [dbSCM].[dbo].[EKB_WIP_Part_Stock]
              where YM = @YM and WCNO = @WCNO 
              order by UpdateDate desc";

            sql_select.Parameters.Add(new SqlParameter("@YM", DateTime.Now.ToString("yyyyMM")));
            sql_select.Parameters.Add(new SqlParameter("@WCNO", wcno));

            DataTable dtgetPartno = oCOnnSCM.Query(sql_select);
            string[] partNo = new string[dtgetPartno.Rows.Count];
            if (dtgetPartno.Rows.Count > 0)
            {
                foreach (DataRow drIN in dtgetPartno.Rows)
                {

                    partNo[i] = drIN["PARTNO"].ToString();


                    i++;
                }
            }

            return Ok(partNo);
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



        [HttpPost]
        [Route("getTargetAndResult")]
        public IActionResult getTargetAndResult([FromBody] PackingPayload payload)
        {
            List<String> _partnos = new List<string>();
            List<findTotalInOutOverallChart> res = new List<findTotalInOutOverallChart>();
            string YM = DateTime.Parse(payload.stDate).ToString("yyyyMM");

            int year = DateTime.Now.Year;
            int mont = DateTime.Parse(payload.stDate).Month;



            DataTable dtFinds = new DataTable();

            dtFinds.TableName = "tbDeliverlyPartTime";
            dtFinds.Columns.Add("PARTNO", typeof(string));
            dtFinds.Columns.Add("ShiftDate", typeof(string));
            dtFinds.Columns.Add("IN", typeof(decimal));
            dtFinds.Columns.Add("OUT", typeof(decimal));
            dtFinds.Columns.Add("RJ", typeof(decimal));
            dtFinds.Columns.Add("LBAL", typeof(decimal));


            SqlCommand sql_select_partno = new SqlCommand();
            sql_select_partno.CommandText = @"	SELECT PARTNO
                                                    FROM EKB_WIP_Part_Stock
                                                    where YM = @YM and WCNO = @WCNO
                                                    order by UpdateDate desc";

            sql_select_partno.Parameters.Add(new SqlParameter("@YM", YM));
            sql_select_partno.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));

            DataTable dtSelectPartno = oCOnnSCM.Query(sql_select_partno);

            if (dtSelectPartno.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtSelectPartno.Rows)
                {


                    _partnos.Add(drAll["PARTNO"].ToString());

                }
            }



            int countRound = 0;
            int i = 0;
            // วนลูป เพือหา labels
            List<getLabelsOverallChart> gocList = new List<getLabelsOverallChart>();

            foreach (string _partno in _partnos)
            {


        
                DateTime stDate = new DateTime(DateTime.Parse(payload.stDate).Year, DateTime.Parse(payload.stDate).Month, 1);
                DateTime enDate = stDate.AddMonths(1).AddDays(-1);
                for (DateTime dt = stDate; dt <= enDate; dt = dt.AddDays(1))
                {
                    getLabelsOverallChart goc = new getLabelsOverallChart();
                    goc.date = Convert.ToDateTime(dt).ToString("yyyy-MM-dd");
                    goc.partNo = _partno;
                    gocList.Add(goc);


                }
            }

            int gabTwodate = (DateTime.Parse(payload.enDate).Day - DateTime.Parse(payload.stDate).Day) + 1;

       

            foreach (getLabelsOverallChart item in gocList)
            {

                decimal lbal = 0;
                SqlCommand sql_select_LBAL = new SqlCommand();
                sql_select_LBAL.CommandText = @"	SELECT LBAL
                                                    FROM EKB_WIP_Part_Stock
                                                    where YM = @YM and WCNO = @WCNO and PARTNO = @PARTNO
                                                    order by UpdateDate desc";

                sql_select_LBAL.Parameters.Add(new SqlParameter("@YM", YM));
                sql_select_LBAL.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
                sql_select_LBAL.Parameters.Add(new SqlParameter("@PARTNO", item.partNo));

                DataTable dtSelectLBAL = oCOnnSCM.Query(sql_select_LBAL);

                if (dtSelectPartno.Rows.Count > 0)
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
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@WCNO", payload.wcno));
                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@PARTNO", item.partNo));

                sql_select_overall_dataList.Parameters.Add(new SqlParameter("@ShiftDate", item.date));

                DataTable dtFindTotalInOUT = oCOnnSCM.Query(sql_select_overall_dataList);
                dtFinds.Rows.Add(item.partNo, item.date, 0, 0, 0);
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

                    countRound = i;
                }

                int days = DateTime.DaysInMonth(year, mont);

                if (i % days != 0)
                {
                    dtFinds.Rows[i]["LBAL"] = (dtFinds.Rows[i]["LBAL"] == DBNull.Value && i == 0) ? lbal : (Convert.ToDecimal(dtFinds.Rows[i == 0 ? i : i - 1]["LBAL"]) + Convert.ToDecimal(dtFinds.Rows[i]["IN"])) - Convert.ToDecimal(dtFinds.Rows[i]["RJ"]) - Convert.ToDecimal(dtFinds.Rows[i]["OUT"]);

                }
                else
                {
                    //dtFinds.Rows[i]["LBAL"] = dtFinds.Rows[i]["LBAL"] == DBNull.Value ? lbal : (Convert.ToDecimal(dtFinds.Rows[gabTwodate]["LBAL"]) + Convert.ToDecimal(dtFinds.Rows[i]["IN"])) - Convert.ToDecimal(dtFinds.Rows[i]["RJ"]) - Convert.ToDecimal(dtFinds.Rows[i]["OUT"]);
                    dtFinds.Rows[i]["LBAL"] = dtFinds.Rows[i]["LBAL"] == DBNull.Value ? lbal : (lbal + Convert.ToDecimal(dtFinds.Rows[i]["IN"])) - Convert.ToDecimal(dtFinds.Rows[i]["RJ"]) - Convert.ToDecimal(dtFinds.Rows[i]["OUT"]);

                }
                i++;


            }
       
            
            List<ResultTarget> resultsAllTarget = CalResultAndTarget(dtFinds, payload.wcno, payload.stDate, payload.enDate);
            List<findDayOfResultAndTarget> _findDayOfResultAndTargetList = new List<findDayOfResultAndTarget>();
            foreach (DataRow _keepDataFindDay in dtFinds.Rows)
            {
                findDayOfResultAndTarget _findDay = new findDayOfResultAndTarget();
                _findDay.partDesc = resultsAllTarget.FirstOrDefault().partDesc;
                _findDay.PARTNO = _keepDataFindDay["PARTNO"].ToString();
                _findDay.ShiftDate = _keepDataFindDay["ShiftDate"].ToString();
                _findDay.IN = Convert.ToDecimal(_keepDataFindDay["IN"]);
                _findDay.OUT = Convert.ToDecimal(_keepDataFindDay["OUT"]);
                _findDay.RJ = Convert.ToDecimal(_keepDataFindDay["RJ"]);
                _findDay.LBAL = Convert.ToDecimal(_keepDataFindDay["LBAL"]);
                _findDayOfResultAndTargetList.Add(_findDay);
            }

            //DateTime.Parse(payload.stDate)
            resultsAllTarget = resultsAllTarget.
                               Where(x => DateTime.Parse(x.shiftDateFormat) >= DateTime.Parse(payload.stDate) && DateTime.Parse(payload.enDate) >= DateTime.Parse(x.shiftDateFormat)
                               ).ToList();

            return Ok(new { resultsAllTarget = resultsAllTarget, _findDayOfResultAndTargetList = _findDayOfResultAndTargetList });
        }

        private List<ResultTarget> CalResultAndTarget(DataTable resultLBAL, string wcno, string stDate, string enDate)
        {
            List<ResultTarget> targetList = new List<ResultTarget>();

            DateTime stDates = new DateTime(DateTime.Parse(stDate).Year, DateTime.Parse(stDate).Month, 1);
            DateTime enDates = stDates.AddMonths(1).AddDays(-1);
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
                    for (DateTime dt = stDates; dt <= enDates; dt = dt.AddDays(1))
                    {


                        ResultTarget resultTarget = new ResultTarget();
                        resultTarget.shiftDateFormat = Convert.ToDateTime(dt).ToString("yyyy-MM-dd");
                        resultTarget.shiftDate = Convert.ToDateTime(dt).ToString("dd/MM/yyyy");
                        resultTarget.wcno = dtTarget["WCNO"].ToString();
                        resultTarget.partDesc = dtTarget["PartDesc"].ToString();
                        resultTarget.target = Convert.ToDecimal(dtTarget["Targets"]);
                        resultTarget.actual = resultLBAL.AsEnumerable().Where(x => x.Field<string>("ShiftDate") == Convert.ToDateTime(dt).ToString("yyyy-MM-dd"))
                                                       .Sum(x => x.Field<decimal>("LBAL"));

                        targetList.Add(resultTarget);


                    }

                }
            }


            return targetList;

        }



        private class findDayOfResultAndTarget
        {
            public string? PARTNO { get; set; }
            public string? ShiftDate { get; set; }
            public decimal? IN { get; set; }
            public decimal? OUT { get; set; }
            public decimal? RJ { get; set; }
            public decimal? LBAL { get; set; }

            public string? partDesc { get; set; }


        }
    }


}




