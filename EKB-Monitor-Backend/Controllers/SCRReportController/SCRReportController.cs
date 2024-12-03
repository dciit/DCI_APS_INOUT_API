using EKANBAN.Service;
using EKB_Monitor_Backend.Model;
using EKB_Monitor_Backend.Model.SCR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;


namespace EKB_Monitor_Backend.Controllers.SCRReportController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SCRReportController : ControllerBase
    {
        private ConnectDB oCOnnSCM = new ConnectDB("DBSCM");


        [HttpGet]
        [Route("getDatasetSCR/{ym}/{ymd}")]
        public IActionResult getDatasetSCR(string ym, string ymd)
        {
            List<SCRReport> scrMain = new List<SCRReport>();


            string[] line = { "Casing Line", "Machine Line" };




            foreach (string item in line)
            {
                SCRReport sCRReport = new SCRReport();
                string[] partno = new string[5];
                decimal[] in_stock = new decimal[5];
                decimal[] out_stock = new decimal[5];
                decimal[] bal_stock = new decimal[5];
                int i = 0;

                if (item == "Casing Line")
                {

                    SCR_IN_OUT scr = new SCR_IN_OUT();
                    sCRReport.line = item;

                    SqlCommand sql_select_overall_Labels = new SqlCommand();
                    sql_select_overall_Labels.CommandText = @"	

                   SELECT ts.YMD,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,

                     ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'IN' and YM = @YM and YMD = @YMD and WCNO = '904' and  PARTNO = ts.PARTNO and CM = ts.CM and  CreateBy not in ('BATCH-PICKLIST','BATCH') and RefNo = '-' ),0)  IN_STOCK
                     ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'OUT' and YM = @YM and YMD = @YMD and WCNO = '904' and PARTNO = ts.PARTNO and CM = ts.CM  and CreateBy not in ('BATCH-PICKLIST','BATCH') and RefNo = '-' ),0)  OUT_STOCK
                    ,stock.BAL BAL_STOCK
                   FROM EKB_WIP_PART_STOCK_TRANSACTION ts
                   INNER JOIN EKB_WIP_PART_STOCK stock on stock.PARTNO = ts.PARTNO and stock.CM = ts.CM and stock.YM = @YM and stock.WCNO = '904'
                   where ts.YM = @YM and ts.YMD = @YMD and ts.WCNO = '904' and CreateBy not in ('BATCH-PICKLIST','BATCH') and RefNo = '-' and QRCodeData like 'PART_SET_IN_C%'
                   GROUP BY ts.YMD,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,stock.BAL";

                    sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", ym));
                    sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YMD", ymd));

                    DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);

                    if (dtOverAll.Rows.Count > 0)
                    {
                        Array.Resize(ref partno, dtOverAll.Rows.Count);
                        Array.Resize(ref in_stock, dtOverAll.Rows.Count);
                        Array.Resize(ref out_stock, dtOverAll.Rows.Count);
                        Array.Resize(ref bal_stock, dtOverAll.Rows.Count);
                        foreach (DataRow drAll in dtOverAll.Rows)
                        {


                            partno[i] = drAll["PARTNO"].ToString() + " " + drAll["CM"].ToString();
                            in_stock[i] = Convert.ToDecimal(drAll["IN_STOCK"]);
                            out_stock[i] = Convert.ToDecimal(drAll["OUT_STOCK"]);
                            bal_stock[i] = Convert.ToDecimal(drAll["BAL_STOCK"]);



                            i++;

                        }
                        scr.partno = partno;
                        scr.in_stock = in_stock;
                        scr.out_stock = out_stock;
                        scr.bal_stock = bal_stock;

                        sCRReport.report = scr;
                    }

                    scrMain.Add(sCRReport);
                }
                else
                {

                    SCR_IN_OUT scr = new SCR_IN_OUT();
                    sCRReport.line = item;

                    SqlCommand sql_select_overall_Labels = new SqlCommand();
                    sql_select_overall_Labels.CommandText = @"	

                   SELECT ts.YMD,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,

                     ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'IN' and YM = @YM and YMD = @YMD and WCNO = '904' and  PARTNO = ts.PARTNO and CM = ts.CM and  CreateBy not in ('BATCH-PICKLIST','BATCH') and RefNo = '-' ),0)  IN_STOCK
                     ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'OUT' and YM = @YM and YMD = @YMD and WCNO = '904' and PARTNO = ts.PARTNO and CM = ts.CM  and CreateBy not in ('BATCH-PICKLIST','BATCH') and RefNo = '-' ),0)  OUT_STOCK
                    ,stock.BAL BAL_STOCK
                   FROM EKB_WIP_PART_STOCK_TRANSACTION ts
                   INNER JOIN EKB_WIP_PART_STOCK stock on stock.PARTNO = ts.PARTNO and stock.CM = ts.CM and stock.YM = @YM and stock.WCNO = '904'
                   where ts.YM = @YM and ts.YMD = @YMD and ts.WCNO = '904' and CreateBy not in ('BATCH-PICKLIST','BATCH') and RefNo = '-' and QRCodeData not like 'PART_SET_IN_C%'
                   GROUP BY ts.YMD,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,stock.BAL";

                    sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YM", ym));
                    sql_select_overall_Labels.Parameters.Add(new SqlParameter("@YMD", ymd));

                    DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);


                    if (dtOverAll.Rows.Count > 0)
                    {
                        Array.Resize(ref partno, dtOverAll.Rows.Count);
                        Array.Resize(ref in_stock, dtOverAll.Rows.Count);
                        Array.Resize(ref out_stock, dtOverAll.Rows.Count);
                        Array.Resize(ref bal_stock, dtOverAll.Rows.Count);
                        foreach (DataRow drAll in dtOverAll.Rows)
                        {


                            partno[i] = drAll["PARTNO"].ToString() + " " + drAll["CM"].ToString();
                            in_stock[i] = Convert.ToDecimal(drAll["IN_STOCK"]);
                            out_stock[i] = Convert.ToDecimal(drAll["OUT_STOCK"]);
                            bal_stock[i] = Convert.ToDecimal(drAll["BAL_STOCK"]);



                            i++;

                        }
                        scr.partno = partno;
                        scr.in_stock = in_stock;
                        scr.out_stock = out_stock;
                        scr.bal_stock = bal_stock;

                        sCRReport.report = scr;
                    }

                    scrMain.Add(sCRReport);
                }
            }






            return Ok(scrMain);

        }



        [HttpGet]
        [Route("getPartDesc")]
        public IActionResult getPartDesc()
        {
            string[] partDesc = new string[5];
            int i = 0;
            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = @"	

                  SELECT 

                      distinct [NOTE]
    
                  FROM [dbSCM].[dbo].[DictMstr]
                  where DICT_SYSTEM = 'WIP_STOCK' and DICT_TYPE = 'PART_SET_OUT'";


            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);
            Array.Resize(ref partDesc, dtOverAll.Rows.Count);

            if (dtOverAll.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtOverAll.Rows)
                {
                    partDesc[i] = drAll["NOTE"].ToString();
                    i++;

                }
            }

            return Ok(partDesc);
        }


        #region query ดึงข้อมูล report SCR monitor
        //[HttpGet]
        //[Route("getDataReportInOut/{ymd}/{shift}/{type}/{status}")]

        //public IActionResult getDataReportInOut(string ymd, string shift, string type, string status)
        //{


        //    List<DataIN_OUT_Report_ALL> dataIN_OUT_ALL_List = new List<DataIN_OUT_Report_ALL>();




        //    Dictionary<string, string[]> part_types_dict = new Dictionary<string, string[]>()

        //    {
        //        { "BODY", new string[] {"BODY" } },
        //        { "TOP", new string[] { "CASING TOP ASSY"}},
        //        { "BOTTOM", new string[] { "CASING BOTTOM ASSY" }},
        //        { "FS", new string[] { "FIXED SCROLL" }},
        //        { "OS", new string[] { "ORBITING SCROLL" } },
        //        { "CS", new string[] { "CRANK SHAFT" } },
        //        { "HS", new string[] { "HOUSING SCROL" } },
        //        { "LW", new string[] { "LOWER" } },

        //        { "STATOR", new string[] { "STATOR" } },
        //        { "ROTOR", new string[] { "ROTOR" } }



        //    };





        //    foreach (var item in (type == "ALL" ? part_types_dict : part_types_dict.Where(x => x.Key == type)))
        //    {
        //        List<DataIN_OUT_Report_BY_TYPE> dataIN_OUT_Reports = new List<DataIN_OUT_Report_BY_TYPE>();


        //        DataIN_OUT_Report_ALL dataIN_OUT_Report_ALL = new DataIN_OUT_Report_ALL();
        //        dataIN_OUT_Report_ALL.part_type = item.Key + " (" + item.Value[0] + ")";

        //        SqlCommand sql_select_overall_Labels = new SqlCommand();
        //        sql_select_overall_Labels.CommandText = $@"	

        //               SELECT ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,models,part_desc,
        //             ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'IN' and SHIFT = @shift and YMD = @ymd and WCNO = '904' and  PARTNO = ts.PARTNO and CM = ts.CM ),0)  IN_STOCK
        //             ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'OUT' and SHIFT =@shift and YMD = @ymd and WCNO = '904' and PARTNO = ts.PARTNO and CM = ts.CM  ),0)  OUT_STOCK
        //            ,stock.BAL BAL_STOCK, ISNULL(logs.NOTE,'-') REMARK , CASE WHEN logs.NOTE != '-' THEN 'TRUE' ELSE 'FALSE' END IS_REMARK
        //           FROM EKB_WIP_PART_STOCK_TRANSACTION ts
        //           LEFT JOIN (select STRING_AGG(model,',' ) models, part_desc, partno, cm from(
        //            select distinct substring([DESCRIPTION],1,6) model, note part_desc, ref2 partno, ref3 cm
        //            from DictMstr d where DICT_TYPE = 'PART_SET_OUT' and DICT_STATUS = 'ACTIVE' 
        //            ) t1 
        //           group by part_desc, partno, cm  
        //            ) m on m.partno = ts.PARTNO and m.cm = ts.CM
        //           INNER JOIN EKB_WIP_PART_STOCK stock on stock.PARTNO = ts.PARTNO and stock.CM = ts.CM and stock.YM = @ym and stock.WCNO = '904'
        //           LEFT JOIN DICT_SYSTEM_LOGS logs on logs.REF_CODE = ts.YMD and logs.DESCRIPTION = ts.SHIFT and logs.REF1 = ts.WCNO and logs.REF2 = ts.PARTNO and
        //           logs.REF3 = ts.CM and logs.REF4 = models and logs.REF5 = part_desc and DICT_SYSTEM = 'APS_WIP_STOCK' and DICT_TYPE = 'FAC2_SCR' and DICT_STATUS = 'ACTIVE'
        //           where ts.YMD = @ymd and SHIFT =@shift and ts.WCNO = '904' and CreateBy not in ('BATCH-PICKLIST','BATCH') and part_desc IN(@parttype)
        //           GROUP BY ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,stock.BAL,models,part_desc,logs.NOTE
        //           order by BAL_STOCK";

        //        sql_select_overall_Labels.Parameters.Add(new SqlParameter("@ym", DateTime.Now.ToString("yyyyMM")));
        //        sql_select_overall_Labels.Parameters.Add(new SqlParameter("@ymd", ymd));
        //        sql_select_overall_Labels.Parameters.Add(new SqlParameter("@shift", shift));
        //        sql_select_overall_Labels.Parameters.Add(new SqlParameter("@parttype", item.Key));
        //        DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);


        //        if (dtOverAll.Rows.Count > 0)
        //        {
        //            foreach (DataRow drAll in dtOverAll.Rows)
        //            {



        //                DataIN_OUT_Report_BY_TYPE dataIN_OUT_Report = new DataIN_OUT_Report_BY_TYPE();
        //                dataIN_OUT_Report.ymd = drAll["YMD"].ToString();
        //                dataIN_OUT_Report.shift = drAll["SHIFT"].ToString();
        //                dataIN_OUT_Report.wcno = drAll["WCNO"].ToString();
        //                dataIN_OUT_Report.partno = drAll["PARTNO"].ToString();
        //                dataIN_OUT_Report.cm = drAll["cm"].ToString();
        //                dataIN_OUT_Report.partDesc = drAll["part_desc"].ToString();
        //                dataIN_OUT_Report.model = drAll["models"].ToString();
        //                dataIN_OUT_Report.bal_stock = Convert.ToDecimal(drAll["BAL_STOCK"]);
        //                dataIN_OUT_Report.remark = drAll["REMARK"].ToString();
        //                dataIN_OUT_Report.remark_stauts = drAll["IS_REMARK"].ToString();


        //                dataIN_OUT_Report.in_stock = Convert.ToDecimal(drAll["OUT_STOCK"]) < 0 ? Convert.ToDecimal(drAll["IN_STOCK"]) + Math.Abs(Convert.ToDecimal(drAll["OUT_STOCK"])) : Convert.ToDecimal(drAll["IN_STOCK"]);
        //                dataIN_OUT_Report.out_stock = Convert.ToDecimal(drAll["OUT_STOCK"]) > 0 ? Convert.ToDecimal(drAll["OUT_STOCK"]) : 0;
        //                dataIN_OUT_Report.shift_lbal_stock = findLbalBeforeShift(dataIN_OUT_Report.ymd, dataIN_OUT_Report.shift, dataIN_OUT_Report.partno, dataIN_OUT_Report.cm, dataIN_OUT_Report.bal_stock);
        //                //dataIN_OUT_Report.shift_lbal_stock = 0;


        //                dataIN_OUT_Report.shift_bal_stock = (dataIN_OUT_Report.shift_lbal_stock + dataIN_OUT_Report.in_stock) - dataIN_OUT_Report.out_stock;
        //                dataIN_OUT_Report.status = findStatus(dataIN_OUT_Report.shift_lbal_stock, dataIN_OUT_Report.in_stock, dataIN_OUT_Report.out_stock, dataIN_OUT_Report.shift_bal_stock);
        //                dataIN_OUT_Reports.Add(dataIN_OUT_Report);



        //            }




        //            dataIN_OUT_Report_ALL.reportAll = filterStatus(status, dataIN_OUT_Reports);
        //            dataIN_OUT_ALL_List.Add(dataIN_OUT_Report_ALL);
        //        }
        //    }


        //    return Ok(dataIN_OUT_ALL_List);

        //}
        #endregion



        [HttpGet]
        [Route("getDataReportInOut/{ymd}/{shift}/{type}/{status}/{stock}")]

        public IActionResult getDataReportInOut(string ymd, string shift, string type, string status, string stock)
        {
            SqlCommand SqlLog = new SqlCommand();
            SqlLog.CommandText = @$"SELECT * FROM [dbSCM].[dbo].[DICT_SYSTEM_LOGS]   WHERE [DESCRIPTION] = @shift  and REF_CODE = @ymd  ORDER BY CREATE_DATE DESC";
            SqlLog.Parameters.Add(new SqlParameter("@ymd", ymd));
            SqlLog.Parameters.Add(new SqlParameter("@shift", shift));

            DataTable dtLogs = oCOnnSCM.Query(SqlLog);
            string msg = $"start: {DateTime.Now.ToString()}{Environment.NewLine}";
            string sqlWC = "";

            if (stock == "MC")
            {
                sqlWC = "IN ('281','282','241','242','243','244','245','246','247','248','274','275','278','263','264')";
            }
            else if (stock == "MAIN")
            {
                sqlWC = "= '904'";
            }

            string ym = DateTime.ParseExact(ymd, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyyMM");

            List<DataIN_OUT_Report_ALL> dataIN_OUT_ALL_List = new List<DataIN_OUT_Report_ALL>();


            //=================================

            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = $@"	

                       SELECT ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,models,part_desc,SHIFT_LBAL,
                     ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'IN' and SHIFT = @shift and YMD = @ymd and WCNO {sqlWC} and  PARTNO = ts.PARTNO and CM = ts.CM ),0)  IN_STOCK
                     ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'OUT' and SHIFT =@shift and YMD = @ymd and WCNO {sqlWC} and PARTNO = ts.PARTNO and CM = ts.CM  ),0)  OUT_STOCK
                    ,stock.BAL BAL_STOCK
                   FROM EKB_WIP_PART_STOCK_TRANSACTION ts
                   LEFT JOIN (select STRING_AGG(model,',' ) models, part_desc, partno, cm from(
				                select distinct substring([DESCRIPTION],1,6) model, note part_desc, ref2 partno, ref3 cm
				                from DictMstr d where DICT_TYPE like 'PART_SET_%' and DICT_STATUS = 'ACTIVE' 
				                ) t1 
			                group by part_desc, partno, cm  
			                 ) m on m.partno = ts.PARTNO and m.cm = ts.CM
                   INNER JOIN EKB_WIP_PART_STOCK stock on stock.PARTNO = ts.PARTNO and stock.CM = ts.CM and stock.YM = @ym and stock.WCNO {sqlWC}
                   INNER JOIN [dbSCM].[dbo].[EKB_WIP_PART_STOCK_SHIFT_BAL] shiftBAL on shiftBAL.WCNO = ts.WCNO and shiftBAL.PARTNO = ts.PARTNO and shiftBAL.CM  = ts.CM and shiftBAL.YM = @ym and shiftBAL.YMD = @ymd and shiftBAL.SHIFT = @shift 
                  
                   where ts.YMD = @ymd and ts.SHIFT =@shift and ts.WCNO {sqlWC} and CreateBy not in ('BATCH-PICKLIST','BATCH') 
                   GROUP BY ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM,stock.PartDesc,stock.BAL,models,part_desc,SHIFT_LBAL
                   order by BAL_STOCK";

            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@ym", ym));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@ymd", ymd));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@shift", shift));
            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);
            //=========================================



            Dictionary<string, string[]> part_types_dict = new Dictionary<string, string[]>()

            {
                { "BODY", new string[] {"BODY" } },
                { "TOP", new string[] { "CASING TOP ASSY"}},
                { "BOTTOM", new string[] { "CASING BOTTOM ASSY" }},
                { "FS", new string[] { "FIXED SCROLL" }},
                { "OS", new string[] { "ORBITING SCROLL" } },
                { "CS", new string[] { "CRANK SHAFT" } },
                { "HS", new string[] { "HOUSING SCROL" } },
                { "LW", new string[] { "LOWER" } },

                { "STATOR", new string[] { "STATOR" } },
                { "ROTOR", new string[] { "ROTOR" } }

            };


            msg += $"get dict : {DateTime.Now.ToString()}{Environment.NewLine}";



            foreach (var item in (type == "ALL" ? part_types_dict : part_types_dict.Where(x => x.Key == type)))
            {
                List<DataIN_OUT_Report_BY_TYPE> dataIN_OUT_Reports = new List<DataIN_OUT_Report_BY_TYPE>();


                msg += $"get {item.Value[0]} : {DateTime.Now.ToString()}{Environment.NewLine}";




                DataIN_OUT_Report_ALL dataIN_OUT_Report_ALL = new DataIN_OUT_Report_ALL();
                dataIN_OUT_Report_ALL.part_type = item.Key + " (" + item.Value[0] + ")";

                //------------------------
                DataRow[] drOverAll = dtOverAll.Select($"  part_desc = '{item.Key}' ");

                msg += $"end query {item.Value[0]} : {DateTime.Now.ToString()}{Environment.NewLine}";

                if (drOverAll.Length > 0)
                {
                    foreach (DataRow drAll in drOverAll)
                    {


                        var oLogs = dtLogs.AsEnumerable().FirstOrDefault(x => x.Field<string>("REF1") == drAll["WCNO"].ToString() &&
                                                                         x.Field<string>("REF2") == drAll["PARTNO"].ToString());


                        DataIN_OUT_Report_BY_TYPE dataIN_OUT_Report = new DataIN_OUT_Report_BY_TYPE();
                        dataIN_OUT_Report.ymd = drAll["YMD"].ToString();
                        dataIN_OUT_Report.shift = drAll["SHIFT"].ToString();
                        dataIN_OUT_Report.wcno = drAll["WCNO"].ToString();
                        dataIN_OUT_Report.partno = drAll["PARTNO"].ToString();
                        dataIN_OUT_Report.cm = drAll["cm"].ToString();
                        dataIN_OUT_Report.partDesc = drAll["part_desc"].ToString();
                        dataIN_OUT_Report.model = drAll["models"].ToString();
                        dataIN_OUT_Report.bal_stock = Convert.ToDecimal(drAll["BAL_STOCK"]);
                        dataIN_OUT_Report.remark = oLogs != null ? oLogs.Field<string>("NOTE") : "";


                        dataIN_OUT_Report.in_stock = Convert.ToDecimal(drAll["OUT_STOCK"]) < 0 ? Convert.ToDecimal(drAll["IN_STOCK"]) + Math.Abs(Convert.ToDecimal(drAll["OUT_STOCK"])) : Convert.ToDecimal(drAll["IN_STOCK"]);
                        dataIN_OUT_Report.out_stock = Convert.ToDecimal(drAll["OUT_STOCK"]) > 0 ? Convert.ToDecimal(drAll["OUT_STOCK"]) : 0;
                        //dataIN_OUT_Report.shift_lbal_stock = findLbalBeforeShift(dataIN_OUT_Report.ymd, dataIN_OUT_Report.shift, dataIN_OUT_Report.partno, dataIN_OUT_Report.cm, dataIN_OUT_Report.bal_stock);
                        dataIN_OUT_Report.shift_lbal_stock = Convert.ToDecimal(drAll["SHIFT_LBAL"]);


                        dataIN_OUT_Report.remark_update = oLogs != null ? oLogs.Field<DateTime>("CREATE_DATE").ToString("dd/MM/yyyy HH:mm") : "";
                        dataIN_OUT_Report.remark_by = oLogs != null ? oLogs.Field<string>("UPDATE_BY") : "";

                        dataIN_OUT_Report.shift_bal_stock = (dataIN_OUT_Report.shift_lbal_stock + dataIN_OUT_Report.in_stock) - dataIN_OUT_Report.out_stock;
                        dataIN_OUT_Report.status = findStatus(dataIN_OUT_Report.shift_lbal_stock, dataIN_OUT_Report.in_stock, dataIN_OUT_Report.out_stock, dataIN_OUT_Report.shift_bal_stock, dataIN_OUT_Report.bal_stock);
                        //dataIN_OUT_Report.status = "";

                        dataIN_OUT_Reports.Add(dataIN_OUT_Report);



                    }

                    msg += $"end loop {item.Value[0]} : {DateTime.Now.ToString()}{Environment.NewLine}";




                    dataIN_OUT_Report_ALL.reportAll = filterStatus(status, dataIN_OUT_Reports);
                    //dataIN_OUT_Report_ALL.reportAll = dataIN_OUT_Reports;

                    dataIN_OUT_ALL_List.Add(dataIN_OUT_Report_ALL);
                }

                msg += $"end {item.Value[0]} : {DateTime.Now.ToString()}{Environment.NewLine}";
            }


            msg += $"end all : {DateTime.Now.ToString()}{Environment.NewLine}";

            return Ok(dataIN_OUT_ALL_List);


        }

        [HttpPost]
        [Route("remarkAction")]
        public IActionResult remarkAction(Remark _remark)
        {
            SqlCommand sql_check_dict = new SqlCommand();
            sql_check_dict.CommandText = $@"	

                       SELECT [DICT_SYSTEM],[DICT_TYPE],[CODE],[REF_CODE],[DESCRIPTION]
                            ,[REF1],[REF2],[REF3],[REF4],[REF5]
                        ,[NOTE],[CREATE_DATE],[UPDATE_BY],[UPDATE_DATE],[DICT_STATUS]
                        FROM [dbSCM].[dbo].[DICT_SYSTEM_LOGS]
                        WHERE DICT_SYSTEM = 'APS_WIP_STOCK' and DICT_TYPE = 'FAC2_SCR' and CODE = @YM and REF_CODE = @YMD and DESCRIPTION = @SHIFT 
                        and REF1 = @WCNO and REF2 = @PARTNO and REF3 = @CM and REF4 = @MODEL and  REF5 = @PART_NAME and DICT_STATUS = 'ACTIVE'";

            sql_check_dict.Parameters.Add(new SqlParameter("@YM", _remark.ym));
            sql_check_dict.Parameters.Add(new SqlParameter("@YMD", _remark.ymd));
            sql_check_dict.Parameters.Add(new SqlParameter("@SHIFT", _remark.shift));
            sql_check_dict.Parameters.Add(new SqlParameter("@WCNO", _remark.wcno));
            sql_check_dict.Parameters.Add(new SqlParameter("@PARTNO", _remark.partno));
            sql_check_dict.Parameters.Add(new SqlParameter("@CM", _remark.cm));
            sql_check_dict.Parameters.Add(new SqlParameter("@MODEL", _remark.model));
            sql_check_dict.Parameters.Add(new SqlParameter("@PART_NAME", _remark.part_name));
            sql_check_dict.Parameters.Add(new SqlParameter("@EMP", _remark.empcode));
            DataTable dtCheckDist = oCOnnSCM.Query(sql_check_dict);


            if (dtCheckDist.Rows.Count > 0)
            {
                foreach (DataRow drcheckdist in dtCheckDist.Rows)
                {

                    SqlCommand sql_update_dict = new SqlCommand();
                    sql_update_dict.CommandText = $@"	
                        

                UPDATE [dbSCM].[dbo].[DICT_SYSTEM_LOGS]
                SET UPDATE_BY = @EMP , NOTE = @REMARK
                WHERE 
                 DICT_SYSTEM = 'APS_WIP_STOCK' and DICT_TYPE = 'FAC2_SCR' and CODE = @YM and REF_CODE = @YMD and DESCRIPTION = @SHIFT 
                 and REF1 = @WCNO and REF2 = @PARTNO and REF3 = @CM and REF4 = @MODEL and  REF5 = @PART_NAME and UPDATE_BY = @EMP and DICT_STATUS = 'ACTIVE'";

                    sql_update_dict.Parameters.Add(new SqlParameter("@YM", drcheckdist["CODE"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@YMD", drcheckdist["REF_CODE"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@SHIFT", drcheckdist["DESCRIPTION"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@WCNO", drcheckdist["REF1"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@PARTNO", drcheckdist["REF2"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@CM", drcheckdist["REF3"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@MODEL", drcheckdist["REF4"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@PART_NAME", drcheckdist["REF5"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@EMP", drcheckdist["UPDATE_BY"].ToString()));
                    sql_update_dict.Parameters.Add(new SqlParameter("@REMARK", _remark.remark));

                    oCOnnSCM.ExecuteCommand(sql_update_dict);

                }
                return Ok(new { status = "update" });
            }
            else
            {
                SqlCommand sql_insert_dict = new SqlCommand();
                sql_insert_dict.CommandText = $@"	
                        

                INSERT INTO [dbSCM].[dbo].[DICT_SYSTEM_LOGS] ([DICT_SYSTEM],[DICT_TYPE],[CODE],REF_CODE,[DESCRIPTION],[REF1],[REF2],[REF3],[REF4],[REF5],[NOTE],[CREATE_DATE],[UPDATE_BY],[UPDATE_DATE],[DICT_STATUS])
                VALUES ('APS_WIP_STOCK','FAC2_SCR',@YM,@YMD,@SHIFT,@WCNO,@PARTNO,@CM,@MODEL,@PART_NAME,@REMARK,GETDATE(),@EMP,GETDATE(),'ACTIVE')";

                sql_insert_dict.Parameters.Add(new SqlParameter("@YM", _remark.ym));
                sql_insert_dict.Parameters.Add(new SqlParameter("@YMD", _remark.ymd));
                sql_insert_dict.Parameters.Add(new SqlParameter("@SHIFT", _remark.shift));
                sql_insert_dict.Parameters.Add(new SqlParameter("@MODEL", _remark.model));
                sql_insert_dict.Parameters.Add(new SqlParameter("@WCNO", _remark.wcno));
                sql_insert_dict.Parameters.Add(new SqlParameter("@PARTNO", _remark.partno));
                sql_insert_dict.Parameters.Add(new SqlParameter("@CM", _remark.cm));
                sql_insert_dict.Parameters.Add(new SqlParameter("@PART_NAME", _remark.part_name));
                sql_insert_dict.Parameters.Add(new SqlParameter("@REMARK", _remark.remark));
                sql_insert_dict.Parameters.Add(new SqlParameter("@EMP", _remark.empcode));
                oCOnnSCM.ExecuteCommand(sql_insert_dict);

                return Ok(new { status = "add" });
            }
        }



        [HttpPost]
        [Route("getRemarkEdit")]
        public IActionResult getRemarkEdit(Remark _remark)
        {
            string empCode = "";
            string remark = "";

            SqlCommand sql_check_dict = new SqlCommand();
            sql_check_dict.CommandText = $@"	

                       SELECT [DICT_SYSTEM],[DICT_TYPE],[CODE],[REF_CODE],[DESCRIPTION]
                            ,[REF1],[REF2],[REF3],[REF4],[REF5]
                        ,[NOTE],[CREATE_DATE],[UPDATE_BY],[UPDATE_DATE],[DICT_STATUS]
                        FROM [dbSCM].[dbo].[DICT_SYSTEM_LOGS]
                        WHERE DICT_SYSTEM = 'APS_WIP_STOCK' and DICT_TYPE = 'FAC2_SCR' and CODE = @YM and REF_CODE = @YMD and DESCRIPTION = @SHIFT 
                        and REF1 = @WCNO and REF2 = @PARTNO and REF3 = @CM and REF4 = @MODEL and  REF5 = @PART_NAME and DICT_STATUS = 'ACTIVE'";

            sql_check_dict.Parameters.Add(new SqlParameter("@YM", _remark.ym));
            sql_check_dict.Parameters.Add(new SqlParameter("@YMD", _remark.ymd));
            sql_check_dict.Parameters.Add(new SqlParameter("@SHIFT", _remark.shift));
            sql_check_dict.Parameters.Add(new SqlParameter("@WCNO", _remark.wcno));
            sql_check_dict.Parameters.Add(new SqlParameter("@PARTNO", _remark.partno));
            sql_check_dict.Parameters.Add(new SqlParameter("@CM", _remark.cm));
            sql_check_dict.Parameters.Add(new SqlParameter("@MODEL", _remark.model));
            sql_check_dict.Parameters.Add(new SqlParameter("@PART_NAME", _remark.part_name));

            DataTable dtCheckDist = oCOnnSCM.Query(sql_check_dict);


            if (dtCheckDist.Rows.Count > 0)
            {
                foreach (DataRow drcheckdist in dtCheckDist.Rows)
                {
                    empCode = drcheckdist["UPDATE_BY"].ToString();
                    remark = drcheckdist["NOTE"].ToString();


                }



            }

            return Ok(new { empCode = empCode, remark = remark });

        }


        private DataTable getLbalBeforeShift(string ymd, string shift)
        {
            string dateString = ymd;
            string format = "yyyyMMdd";
            string newFormat = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (shift == "D")
            {   // ex: 2024-08-27 08:00  (D)
                newFormat += " 08:00";
            }
            else
            {
                // ex: 2024-08-27 20:00  (N)
                newFormat += " 20:00";
            }


            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = $@"	
                             SELECT ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM
							 
							 ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'IN' and SHIFT = ts.SHIFT and YMD = ts.YMD and WCNO = '904' and  PARTNO = ts.PARTNO and CM = ts.CM ),0)  IN_SHIFT_STOCK
						     ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'OUT' and SHIFT = ts.SHIFT and YMD = ts.YMD and WCNO = '904' and PARTNO = ts.PARTNO and CM = ts.CM  ),0)  OUT_SHIFT_STOCK
							 
							 FROM EKB_WIP_PART_STOCK_TRANSACTION ts
							 where YM = @ym and WCNO ='904' and PARTNO = @partno and CM = @cm and CreateDate >= @newFormat and CreateBy not in ('BATCH-PICKLIST','BATCH')
							 GROUP BY ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM
							 order by YMD ,SHIFT";


            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@ym", DateTime.Now.ToString("yyyyMM")));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@newFormat", newFormat));

            return oCOnnSCM.Query(sql_select_overall_Labels);

        }


        private decimal findLbalBeforeShift(string ymd, string shift, string partno, string cm, decimal currentStock)
        {
            string dateString = ymd;
            string format = "yyyyMMdd";
            string newFormat = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            if (shift == "D")
            {   // ex: 2024-08-27 08:00  (D)
                newFormat += " 08:00";
            }
            else
            {
                // ex: 2024-08-27 20:00  (N)
                newFormat += " 20:00";
            }

            decimal sumIN = 0;
            decimal sumOUT = 0;

            SqlCommand sql_select_overall_Labels = new SqlCommand();
            sql_select_overall_Labels.CommandText = $@"	
	                            
                             SELECT ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM
							 
							 ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'IN' and SHIFT = ts.SHIFT and YMD = ts.YMD and WCNO = '904' and  PARTNO = ts.PARTNO and CM = ts.CM ),0)  IN_SHIFT_STOCK
						     ,ISNULL((SELECT SUM(TransQty) FROM EKB_WIP_PART_STOCK_TRANSACTION where TransType = 'OUT' and SHIFT = ts.SHIFT and YMD = ts.YMD and WCNO = '904' and PARTNO = ts.PARTNO and CM = ts.CM  ),0)  OUT_SHIFT_STOCK
							 
							 FROM EKB_WIP_PART_STOCK_TRANSACTION ts
							 where YM = @ym and WCNO ='904' and PARTNO = @partno and CM = @cm and CreateDate >= @newFormat and CreateBy not in ('BATCH-PICKLIST','BATCH')
							 GROUP BY ts.YMD,ts.SHIFT,ts.WCNO,ts.PARTNO,ts.CM
							 order by YMD ,SHIFT   ";


            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@ym", DateTime.Now.ToString("yyyyMM")));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@partno", partno));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@cm", cm));
            sql_select_overall_Labels.Parameters.Add(new SqlParameter("@newFormat", newFormat));

            DataTable dtOverAll = oCOnnSCM.Query(sql_select_overall_Labels);

            if (dtOverAll.Rows.Count > 0)
            {
                foreach (DataRow drAll in dtOverAll.Rows)
                {

                    sumIN += Convert.ToDecimal(drAll["IN_SHIFT_STOCK"]);
                    sumOUT += Convert.ToDecimal(drAll["OUT_SHIFT_STOCK"]);

                }

                return (currentStock + sumOUT) - sumIN;
            }
            else
            {
                return 0;
            }


        }







        private string findStatus(decimal lbal, decimal in_stock, decimal out_stock, decimal shift_bal, decimal currentStock)
        {
            //string status = "";
            //decimal percentRangeINOUT = 0;
            //if (in_stock != 0 && out_stock != 0)
            //{
            //    percentRangeINOUT = ((Math.Abs(in_stock - out_stock) / (Math.Abs(in_stock + out_stock) / 2))) * 100;


            //}


            //if ((in_stock > 0 && out_stock == 0) && lbal > 0 && bal > 0)
            //{
            //    status = "มียอด IN ไม่มียอด OUT";
            //}
            //else if ((in_stock == 0 && out_stock > 0) && lbal > 0 && bal > 0)
            //{
            //    status = "มียอด OUT ไม่มียอด IN";
            //}
            //else if (lbal < 0 && bal >= 0)
            //{
            //    status = "SHIFT LBAL (A) < 0";
            //}
            //else if (bal < 0 && lbal >= 0)
            //{
            //    status = "SHIFT BAL (D) < 0";
            //}
            //else if(lbal < 0 && bal < 0)
            //{
            //    status = "LBAL (A) และ BAL (D) < 0";
            //}
            //else if ((percentRangeINOUT >= 0 && percentRangeINOUT <= 50) && bal >= 0 && lbal >= 0 )
            //{
            //    status = "IN ไม่เท่ากับ OUT (0-50%)";
            //}
            //else if (percentRangeINOUT > 50 && percentRangeINOUT <= 200 && bal >= 0 && lbal >= 0)
            //{
            //    status = "IN ไม่เท่ากับ OUT (50-200%)";
            //}
            //else if (percentRangeINOUT > 200 && bal > 0 && lbal > 0)
            //{
            //    status = "IN ไม่เท่ากับ OUT (>200%)";
            //}
            //else
            //{
            //    status = "ปกติ";
            //}



            string status = "";

            if (currentStock < 0)
            {
                status = "STOCK ปัจจุบัน < 0";
            }

            else if (lbal < 0 && shift_bal >= 0)
            {
                status = "LBAL (A) < 0";
            }
            else if (shift_bal < 0 && lbal >= 0)
            {
                status = "SHIFT BAL (D) < 0";
            }
            else if (lbal < 0 && shift_bal < 0)
            {
                status = "LBAL (A) และ BAL (D) < 0";
            }

            else
            {
                status = "STOCK ปกติ";
            }

            return status;

        }



        private List<DataIN_OUT_Report_BY_TYPE> filterStatus(string status, List<DataIN_OUT_Report_BY_TYPE> dataIN_OUT_Reports)
        {



            if (status == "STATUS-0")
            {
                dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "STOCK ปกติ").ToList();

            }


            else if (status == "STATUS-1")
            {
                dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "STOCK ปัจจุบัน < 0").ToList();

            }


            else if (status == "STATUS-2")
            {
                dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "SHIFT LBAL (A) < 0").ToList();

            }

            else if (status == "STATUS-3")
            {
                dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "SHIFT BAL (D) < 0").ToList();

            }

            else if (status == "STATUS-4")
            {
                dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "LBAL (A) และ BAL (D) < 0").ToList();

            }



            //if (status == "STATUS-4")
            //{
            //    dataIN_OUT_Reports =  dataIN_OUT_Reports.Where(x => x.status == "มียอด IN ไม่มียอด OUT").ToList();
            //}
            //else if (status == "STATUS-5")
            //{
            //    dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "มียอด OUT ไม่มียอด IN").ToList();

            //}


            //else if (status == "STATUS-6")
            //{
            //    dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "IN ไม่เท่ากับ OUT (0-50%)").ToList();

            //}

            //else if (status == "STATUS-7")
            //{
            //    dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "IN ไม่เท่ากับ OUT (50-200%)").ToList();

            //}

            //else if (status == "STATUS-8")
            //{
            //    dataIN_OUT_Reports = dataIN_OUT_Reports.Where(x => x.status == "IN ไม่เท่ากับ OUT (>200%)").ToList();

            //}




            return dataIN_OUT_Reports;
        }



        [HttpPost]
        [Route("findHistoryINOUT")]
        public IActionResult findHistoryINOUT(History _hitory)
        {
            string sqlCon = "";
            if (_hitory.type == "MAIN")
            {
                sqlCon = "= '904'";

            }
            else if (_hitory.type == "MC")
            {
                sqlCon = "IN ('281','282','241','242','243','244','245','246','247','248','274','275','278','263','264')";
            }

            List<HistoryReturn> historyReturns = new List<HistoryReturn>();

            SqlCommand sql_check_dict = new SqlCommand();
            sql_check_dict.CommandText = $@"	

                      select 
                            CASE 
			                         WHEN QRCodeData like '7%' then 'BATCH-GAS_TIGHT' 
				                     WHEN QRCodeData like 'ADJUST' then 'ADJUST'
				                     WHEN RefNo like 'FROM%' then 'FROM W/C TO MAIN'
				                     WHEN RefNo like 'TRANSFER%' then 'TRANSFER W/C TO MAIN'
				                     WHEN RefNo like 'Up%' then 'UploadResult (IT)'
				                     WHEN (TransType = 'OUT' and QRCodeData != '' and CreateBy != '' and TransQty != 0) then 'ตัดจากเครื่องล้าง' + ' (' + CreateBy + ')'
				                     WHEN (TransType = 'IN' and QRCodeData != '' and CreateBy != '' and TransQty != 0) then 'แสกนเข้าเครืองล้าง' + ' (' + CreateBy + ')'
	                                END REF,
 
                            TransType,

                           SUM(TransQty) TransQTY
 
 
                     from EKB_WIP_PART_STOCK_TRANSACTION
                     where WCNO {sqlCon} and YMD = @YMD and PARTNO = @PARTNO  and CM = @CM and Shift = @SHIFT  
                     GROUP BY 	CASE 
			                        WHEN QRCodeData like '7%' then 'BATCH-GAS_TIGHT' 
				                     WHEN QRCodeData like 'ADJUST' then 'ADJUST'
				                     WHEN RefNo like 'FROM%' then 'FROM W/C TO MAIN'
				                     WHEN RefNo like 'TRANSFER%' then 'TRANSFER W/C TO MAIN'
				                     WHEN RefNo like 'Up%' then 'UploadResult (IT)'
				                     WHEN (TransType = 'OUT' and QRCodeData != '' and CreateBy != '' and TransQty != 0) then 'ตัดจากเครื่องล้าง' + ' (' + CreateBy + ')'
				                     WHEN (TransType = 'IN' and QRCodeData != '' and CreateBy != '' and TransQty != 0) then 'แสกนเข้าเครืองล้าง' + ' (' + CreateBy + ')'
	                        END,TransType
                    order by REF desc";

            sql_check_dict.Parameters.Add(new SqlParameter("@YMD", _hitory.ymd));
            sql_check_dict.Parameters.Add(new SqlParameter("@WC", _hitory.wcno));
            sql_check_dict.Parameters.Add(new SqlParameter("@PARTNO", _hitory.partno));
            sql_check_dict.Parameters.Add(new SqlParameter("@CM", _hitory.cm));
            sql_check_dict.Parameters.Add(new SqlParameter("@SHIFT", _hitory.shift));


            DataTable dtCheckDist = oCOnnSCM.Query(sql_check_dict);


            if (dtCheckDist.Rows.Count > 0)
            {
                foreach (DataRow drcheckdist in dtCheckDist.Rows)
                {
                    HistoryReturn historyReturn = new HistoryReturn();
                    historyReturn.refs = drcheckdist["REF"].ToString();
                    historyReturn.type = drcheckdist["TransType"].ToString();
                    historyReturn.transQty = Convert.ToDecimal(drcheckdist["TransQTY"]);
                    historyReturns.Add(historyReturn);
                }
            }


            List<HistoryRemarkReturn> historyRemarkReturnsList = new List<HistoryRemarkReturn>();

            SqlCommand SqlRemarkHistory = new SqlCommand();
            SqlRemarkHistory.CommandText = @$"SELECT * FROM [dbSCM].[dbo].[DICT_SYSTEM_LOGS]  
                                              WHERE  REF_CODE = @YMD and DESCRIPTION = @SHIFT and REF1 = @WC and REF2 = @PARTNO and REF3 = @CM 
                                              ORDER BY CREATE_DATE DESC";
            SqlRemarkHistory.Parameters.Add(new SqlParameter("@YMD", _hitory.ymd));
            SqlRemarkHistory.Parameters.Add(new SqlParameter("@WC", _hitory.wcno));
            SqlRemarkHistory.Parameters.Add(new SqlParameter("@PARTNO", _hitory.partno));
            SqlRemarkHistory.Parameters.Add(new SqlParameter("@CM", _hitory.cm));
            SqlRemarkHistory.Parameters.Add(new SqlParameter("@SHIFT", _hitory.shift));

            DataTable dtLogs = oCOnnSCM.Query(SqlRemarkHistory);

            if (dtLogs.Rows.Count > 0)
            {
                foreach (DataRow drcheckdist in dtLogs.Rows)
                {
                    HistoryRemarkReturn historyRemarkReturn = new HistoryRemarkReturn();
                    historyRemarkReturn.remark_status = drcheckdist["NOTE"].ToString();
                    historyRemarkReturn.remark_by = drcheckdist["UPDATE_BY"].ToString();
                    historyRemarkReturn.remark_date = drcheckdist["UPDATE_DATE"].ToString();

                    historyRemarkReturnsList.Add(historyRemarkReturn);
                }
            }



            List<TransectionReturn> transectionReturnsList = new List<TransectionReturn>();

            string sqlString = $@"SELECT
                               [YMD]
                              ,[SHIFT]
                              ,[WCNO]
                              ,[PARTNO]
                              ,[CM]
                              ,[TransType]
                              ,[TransQty]
                              ,[QRCodeData]
                              ,[CreateBy]
                              ,[CreateDate]
                                ,[RefNo]
                          FROM [dbSCM].[dbo].[EKB_WIP_PART_STOCK_TRANSACTION]
                          where YMD = '{_hitory.ymd}' and WCNO = '{_hitory.wcno}' and PARTNO = '{_hitory.partno}' and CM = '{_hitory.cm}'
                          order by CreateDate desc";

            SqlCommand sql_TransectionLog = new SqlCommand();
            sql_TransectionLog.CommandText = sqlString;



            DataTable dtTransection = oCOnnSCM.Query(sql_TransectionLog);


            if (dtTransection.Rows.Count > 0)
            {
                foreach (DataRow drTransection in dtTransection.Rows)
                {


                    TransectionReturn transectionReturn = new TransectionReturn();

                    transectionReturn.date = drTransection["YMD"].ToString();
                    transectionReturn.shift = drTransection["SHIFT"].ToString();
                    transectionReturn.wcno = drTransection["WCNO"].ToString();
                    transectionReturn.partno = drTransection["PARTNO"].ToString();
                    transectionReturn.cm = drTransection["CM"].ToString();
                    transectionReturn.type = drTransection["TransType"].ToString();
                    transectionReturn.qty = Convert.ToDecimal(drTransection["TransQty"]);
                    transectionReturn.qrcodeData = drTransection["QRCodeData"].ToString();
                    transectionReturn.createBy = drTransection["CreateBy"].ToString();
                    transectionReturn.createDate = drTransection["CreateDate"].ToString();
                    transectionReturn.refno = drTransection["RefNo"].ToString();



                    transectionReturnsList.Add(transectionReturn);

                }
            }



            return Ok(new { historyReturns = historyReturns, historyRemarkReturnsList = historyRemarkReturnsList , TransectionReutrunList = transectionReturnsList });
        }



  
    }
}
    










