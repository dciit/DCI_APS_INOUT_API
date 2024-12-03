using EKANBAN.Service;
using EKB_Monitor_Backend.Model.SCR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;

namespace EKB_Monitor_Backend.Controllers.SCRReportController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineReportController : Controller
    {

        private ConnectDB oCOnnSCM = new ConnectDB("DBSCM");
        private ConnectDB oCOnnIOT = new ConnectDB("DBIOT");
        private OraConnectDB oOraAL01 = new OraConnectDB("ALPHA01");
        private OraConnectDB oOraAL02 = new OraConnectDB("ALPHA02");



        [HttpGet]
        [Route("getLineReport")]
        public IActionResult getLineReport()
        {
            List<SCRReport> scrMain = new List<SCRReport>();


            string modelCode = "";
            string modelProd = "";
            decimal result = 0;
            decimal plan = 0;



            #region หา model code ใน SEQ ประจำวัน

            SqlCommand sqlFindModelCodeInSEQPlan = new SqlCommand();
            sqlFindModelCodeInSEQPlan.CommandText = @"

	 
               SELECT distinct SEBANGO
               FROM [dbSCM].[dbo].[WMS_MDW27_MODEL_MASTER]
               where MODEL IN (
		
			            SELECT REPLACE(PartNo,'-10','') FROM  [dbSCM].[dbo].[APS_ProductionPlan] apsPlan
	             where apsPlan.LREV = '999' and APS_PlanDate =  '" + DateTime.Now.ToString("yyyy-MM-dd") + @"' and SUBLINE = 'ASSEMBLY LINE4 (SCR)' 

               )
            ";


            DataTable dtFindModelCodeInSEQPlan = oCOnnSCM.Query(sqlFindModelCodeInSEQPlan);

            if (dtFindModelCodeInSEQPlan.Rows.Count > 0)
            {
                foreach (DataRow drdtFindModelCodeInSEQPlanin in dtFindModelCodeInSEQPlan.Rows)
                {
                    modelProd +=  "'" + drdtFindModelCodeInSEQPlanin["SEBANGO"].ToString() + "',";
                }
                modelProd = modelProd.Substring(0, modelProd.Length - 1);
            }


            #endregion



            #region หา model code

            SqlCommand sqlGetModelCode_GasTight = new SqlCommand();
            sqlGetModelCode_GasTight.CommandText = $@"	

                  SELECT TOP (1) [id]
                  ,[Serial]
                  ,[Model_Code]
                  ,[Insert_By]
                  ,[Insert_Date]
                  FROM [dbIoT].[dbo].[SCR_GasTight]
                  where FORMAT(GetDate(), 'yyyy-MM-dd') = FORMAT(Insert_date, 'yyyy-MM-dd') and Model_Code != 'ERROR' and Model_Code IN ({modelProd})
                  order by Insert_date desc";

            DataTable dtModelCodeGasTight = oCOnnIOT.Query(sqlGetModelCode_GasTight);

            if (dtModelCodeGasTight.Rows.Count > 0)
            {
                foreach (DataRow drModelCodeGasTight in dtModelCodeGasTight.Rows)
                {
                    modelCode = drModelCodeGasTight["Model_Code"].ToString();
                }


            }
         
            else
            {
                SqlCommand sqlGetModelCode_ShinkGate = new SqlCommand();
                sqlGetModelCode_ShinkGate.CommandText = $@"	

                  SELECT TOP (1) [Id]
                  ,[Pipe_Serial]
                  ,[Model]
                  ,[Insert_date]
                  FROM [dbIoT].[dbo].[SCR_Body_Shink_Fitting]
                  where FORMAT(GetDate(), 'yyyy-MM-dd') = FORMAT(Insert_date, 'yyyy-MM-dd') and Model != 'ERROR' and Model IN ({modelProd})
                  order by Insert_date desc";



                DataTable dtModelCodeShinkGate = oCOnnIOT.Query(sqlGetModelCode_ShinkGate);

                if (dtModelCodeShinkGate.Rows.Count > 0)
                {
                    foreach (DataRow drModelCodeShinkGate in dtModelCodeShinkGate.Rows)
                    {
                        modelCode = drModelCodeShinkGate["Model"].ToString();
                    }


                }
                else
                {

                    SqlCommand _model = new SqlCommand();
                    _model.CommandText = $@"	

                  SELECT TOP (1) [Id]
                  ,[Pipe_Serial]
                  ,[Model]
                  ,[Insert_date]
                  FROM [dbIoT].[dbo].[SCR_Body_Shink_Fitting]
                  where FORMAT(GetDate(), 'yyyy-MM-dd') = FORMAT(Insert_date, 'yyyy-MM-dd') and Model != 'ERROR'
                  order by Insert_date desc";



                    DataTable _dtModel = oCOnnIOT.Query(_model);

                    if (_dtModel.Rows.Count > 0)
                    {
                        foreach (DataRow _drModelCodeShinkGate in _dtModel.Rows)
                        {
                            modelCode = _drModelCodeShinkGate["Model"].ToString();
                        }


                    }


                }
            }

            #endregion   // หา ModelCode หา


            #region Model
            //SqlCommand sqlFindModelByModelCode = new SqlCommand();
            //sqlFindModelByModelCode.CommandText = @"	

            //      SELECT distinct [MODEL]
            //      FROM [dbSCM].[dbo].[WMS_MDW27_MODEL_MASTER]
            //      where SEBANGO = '" + modelCode + "'";



            //DataTable dtsqlFindModelByModelCode = oCOnnSCM.Query(sqlFindModelByModelCode);

            //if (dtsqlFindModelByModelCode.Rows.Count > 0)
            //{

            //    foreach (DataRow drFindModelByModelCode in dtsqlFindModelByModelCode.Rows)
            //    {
            //        model = drFindModelByModelCode["MODEL"].ToString();

            //    }
            //}

            #endregion


            #region หาResult



            //string newFormat = DateTime.Now.ToString("yyyy-MM-dd");


            //if (DateTime.Now.AddHours(-8).Hour >= 12)
            //{
            //    newFormat += " 20:00";
            //}
            //else
            //{
            //    newFormat += " 08:00";
            //}

            //SqlCommand sqlFindResult = new SqlCommand();
            //sqlFindResult.CommandText = @"	

            //        SELECT 
            //           [Model_Code],
            //              COUNT([Serial]) result
            //      FROM [dbIoT].[dbo].[SCR_GasTight]
            //      where Insert_Date >= '" + newFormat + @"' and Insert_Date <= GETDATE() and Model_Code = '" + modelCode + @"' and Model_Code != 'ERROR'
            //      GROUP BY [Model_Code]";



            //DataTable dtsqlFindResult = oCOnnIOT.Query(sqlFindResult);

            //if (dtsqlFindResult.Rows.Count > 0)
            //{

            //    foreach (DataRow drFindResult in dtsqlFindResult.Rows)
            //    {
            //        result = Convert.ToDecimal(drFindResult["result"]);

            //    }
            //}
            #endregion


            #region หาMODEL2



            string newFormat = DateTime.Now.ToString("yyyy-MM-dd");


            //if (DateTime.Now.AddHours(-8).Hour >= 12)
            //{
            //    newFormat += " 08:00";
            //}
            //else
            //{
            //    newFormat += " 08:00";
            //}

            newFormat += " 08:00";


            List<ModelInfo> modelList = new List<ModelInfo>();


            SqlCommand sqlFindModel = new SqlCommand();
            sqlFindModel.CommandText = $@"	

                    
	        SELECT a.APS_SEQ,
                   a.PartNo MODEL,
                   a.ModelCode,
                   a.APS_PlanQty ,
                   ISNULL(a.result,0) Actual 
                   ,(a.APS_PlanQty - ISNULL(a.result,0)) Remain
	
		        ,CASE 

			        WHEN ISNULL(a.result,0) < APS_PlanQty and a.ModelCode = '" + modelCode + @"' THEN 'CURRENT'
			        WHEN ISNULL(a.result,0) < APS_PlanQty and ISNULL(a.result,0) != 0 THEN 'SOME PRODUCTION'
			        WHEN ISNULL(a.result,0) = 0 THEN 'WAITING'
			        WHEN ISNULL(a.result,0) >= APS_PlanQty THEN 'FINISH'
		        END STATUS
	           
	           FROM (
                           SELECT   
                          
						   CAST(APS_SEQ as int) APS_SEQ
                          ,[APS_Distribute]
                          ,[PartNo]
						   ,SEBANGO ModelCode
                          ,[APS_PlanQty]
						  , ( SELECT COUNT([Serial]) result
							 FROM [192.168.226.145].[dbIoT].[dbo].[SCR_GasTight]
							where Insert_Date >= '" + newFormat + @"' and Insert_Date <= GETDATE() and Model_Code = SEBANGO and Model_Code != 'ERROR'GROUP BY [Model_Code]) result							
                      FROM [dbSCM].[dbo].[APS_ProductionPlan]  apsPlan
					  INNER JOIN [dbSCM].[dbo].[WMS_MDW27_MODEL_MASTER] on MODEL = REPLACE(PartNo,'-10','')
                      where apsPlan.LREV = '999' and APS_PlanDate = '" + DateTime.Now.AddHours(-8).ToString("yyyy-MM-dd") + @"' and SUBLINE = 'ASSEMBLY LINE4 (SCR)' ) a
              
               

                GROUP BY a.APS_SEQ,a.APS_PlanQty,a.PartNo,a.ModelCode,a.APS_PlanQty,a.result 
                order by STATUS";


            sqlFindModel.Parameters.Add(new SqlParameter("@result", result));

            DataTable dtsqlFindModel = oCOnnSCM.Query(sqlFindModel);

            if (dtsqlFindModel.Rows.Count > 0)
            {

                foreach (DataRow drFindModel in dtsqlFindModel.Rows)
                {


                    ModelInfo modelInfo = new ModelInfo();

                    modelInfo.seq = drFindModel["APS_SEQ"].ToString();
                    modelInfo.model = drFindModel["MODEL"].ToString();
                    modelInfo.modelCode = drFindModel["ModelCode"].ToString();
                    modelInfo.apsPlan = Convert.ToDecimal(drFindModel["APS_PlanQty"]);
                    modelInfo.actual = Convert.ToDecimal(drFindModel["Actual"]);

                    modelInfo.seqStatus = drFindModel["STATUS"].ToString();




                    #region หาSTOCK


                    List<LineInfo> LineList = new List<LineInfo>();

                    Dictionary<string, string[]> part_types_dict = new Dictionary<string, string[]>()

                    {
                        { "Casing", new string[] {"BODY","TOP","BOTTOM"} },
                        { "Machine", new string[] { "FS","HS","CS","LW"}},
                        { "Motor", new string[] { "STATOR","ROTOR" }},


                    };

                    SqlCommand sqlFindStockandStatus = new SqlCommand();
                    sqlFindStockandStatus.CommandText = @"	

                     SELECT distinct
                       [REF1]
                      ,[REF2]
                      ,[REF3]
                      ,[NOTE]
	                  ,BAL
                  FROM [dbSCM].[dbo].[DictMstr]
                  INNER JOIN EKB_WIP_PART_STOCK ekb on ekb.WCNO = REF1 and ekb.PARTNO = REF2 and ekb.CM = REF3 and YM = '" + DateTime.Now.AddHours(-8).ToString("yyyyMM") + @"'
                  where DESCRIPTION = '" + drFindModel["MODEL"].ToString().Replace("-10","") + "' and CODE = '" + drFindModel["ModelCode"].ToString() + "' and NOTE != 'SEAL PLATE ASSY'";



                    DataTable dtsqlFindStockandStatus = oCOnnSCM.Query(sqlFindStockandStatus);

                    if (dtsqlFindStockandStatus.Rows.Count > 0)
                    {

                        foreach (var item in part_types_dict)
                        {



                            LineInfo lineInfo = new LineInfo();
                            lineInfo.line = item.Key;

                            List<ResultStatusInfo> ResultStatusList = new List<ResultStatusInfo>();


                            DataRow[] drOverAll = dtsqlFindStockandStatus.Select();
                            var matchingRows = drOverAll.Where(row => item.Value.Contains(row["NOTE"].ToString())).ToArray();

                            foreach (var row in matchingRows)
                            {
                                ResultStatusInfo ResultStatus = new ResultStatusInfo();
                                ResultStatus.part_name = row["NOTE"].ToString() == "FS" ? "FS/OS" : row["NOTE"].ToString();
                                ResultStatus.stock = Convert.ToDecimal(row["BAL"]);
                                ResultStatus.partno = row["REF2"].ToString();
                                //ResultStatus.remain = Convert.ToDecimal(drFindModel["APS_PlanQty"]) - Convert.ToDecimal(drFindModel["RESULT"]);
                                //decimal safetyStock = Math.Ceiling((ResultStatus.remain / 2) + ResultStatus.remain);
                                ResultStatus.remain = Convert.ToDecimal(drFindModel["Remain"]);
                                ResultStatus.status = Convert.ToDecimal(row["BAL"]) >= ResultStatus.remain ? "เพียงพอ" : "ไม่เพียงพอ";

                                ResultStatusList.Add(ResultStatus);

                                lineInfo.Results = ResultStatusList;

                            }


                            LineList.Add(lineInfo);
                            modelInfo.LineName = LineList;


                        }

                        modelList.Add(modelInfo);

                        //lineResult_Main.Results = lineResultList;
                        //lineReportMainsList.Add(lineResult_Main);

                    }






                }


                #endregion





            }




            #endregion

            //List<ModelInfo> modelCurrentReturn = modelList.Take(1).ToList();
            //List<ModelInfo> modelCurrentReturnNotCurrentModel = modelList.Skip(1).OrderBy(x=>x.seq).ToList();


            List<ModelInfo> modelFinishReturn = modelList.Where(x=>x.seqStatus == "FINISH").ToList();
            //List<ModelInfo> modelSomeProdReturn = modelList.Where(x => x.seqStatus == "SOME PRODUCTION").ToList();
            List<ModelInfo> modelCurrentReturnRaw = modelList.Where(x => x.seqStatus == "CURRENT").OrderBy(x=>x.seq).ToList();
            List<ModelInfo> modelWaitingReturn = modelList.Where(x => (x.seqStatus == "SOME PRODUCTION" || x.seqStatus == "WAITING")).OrderBy(y=>y.seq).ToList();

            List<ModelInfo> modelCurrentReturnNotCurrentModel = modelList.Skip(1).OrderBy(x => x.seq).ToList();


            List<ModelInfo> modelCurrentReturn = new List<ModelInfo>();

            if (modelCurrentReturnRaw.Count > 1)
            {



                modelWaitingReturn.AddRange(modelCurrentReturnRaw.Skip(1));

                //foreach (ModelInfo item in modelCurrentReturnRaw.Skip(1))
                //{
                //    ModelInfo modelInfo = new ModelInfo();

                //    modelInfo.seq = item.seq;
                //    modelInfo.model = item.model;
                //    modelInfo.modelCode = item.modelCode;
                //    modelInfo.apsPlan = item.apsPlan;
                //    modelInfo.actual = item.actual;

                //    modelInfo.seqStatus = item.seqStatus;
                //    List<LineInfo> LineList = new List<LineInfo>();

                //    foreach (LineInfo _line in item.LineName)
                //    {
                //        LineInfo lineInfo = new LineInfo();
                //        lineInfo.line = _line.line;
                //        List<ResultStatusInfo> ResultStatusInfoList = new List<ResultStatusInfo>();

                //        foreach (ResultStatusInfo _result in _line.Results)
                //        {
                //            ResultStatusInfo ResultStatus = new ResultStatusInfo();
                //            ResultStatus.part_name = _result.part_name;
                //            ResultStatus.stock = _result.stock;
                //            ResultStatus.partno = _result.partno;

                //            ResultStatus.remain = _result.remain;
                //            ResultStatus.status = _result.status;
                //            ResultStatusInfoList.Add(ResultStatus);

                //            lineInfo.Results = ResultStatusInfoList;
                            

                //        }

                //        LineList.Add(lineInfo);
                //        modelInfo.LineName = LineList;
                //    }

                //    modelWaitingReturn.Add(modelInfo);
                //}
            }



            List<LineReportReturn> lineReturnFinishList = new List<LineReportReturn>();
            foreach (ModelInfo itemModel in modelFinishReturn)
            {
                LineReportReturn lineReportReturn = new LineReportReturn();

                lineReportReturn.seq = itemModel.seq;
                lineReportReturn.model = itemModel.model;
                lineReportReturn.modelCode = itemModel.modelCode;
                lineReportReturn.apsPlan = itemModel.apsPlan;
                lineReportReturn.actual = itemModel.actual;
                lineReportReturn.remainPlan = itemModel.apsPlan - itemModel.actual;
                lineReportReturn.status = "FINISH";

                //foreach (LineInfo itemLine in itemModel.LineName)
                //{
                //    foreach (ResultStatusInfo itemStatus in itemLine.Results)
                //    {
                //        lineReportReturn.remainPlan = itemStatus.remain;
                //        lineReportReturn.status = "FINISH";

                //        if (itemStatus.status == "ไม่เพียงพอ")
                //        {
                //            lineReportReturn.pnReady = false;
                //            break;
                //        }
                //        else
                //        {
                //            lineReportReturn.pnReady = true;
                //        }

                //    }

                //    if (lineReportReturn.status == "ไม่เพียงพอ")
                //    {
                //        break;
                //    }


                //}

                lineReturnFinishList.Add(lineReportReturn);


            }



            return Ok(new { lineReportAPI = modelList, lineReturnReportFinish = lineReturnFinishList, lineReturnReportCurrent = modelCurrentReturnRaw.Take(1), lineReturnReportWaiting = modelWaitingReturn.Take(2) }) ;

        }


        [HttpGet]
        [Route("setOracleSP")]

        public IActionResult setOracleSP()
        {
            string jibu = "64";
            string pid = DateTime.Now.ToString("ddHHmmss");
            string partno = "3P500743-1";
            string cm = "B";
            string ymds = DateTime.Now.ToString("yyyyMMdd");

       



            string strSV = $@"SELECT * FROM DST_DATPIL
                            WHERE JIBU = '64' and BWCNO = '904' and IDATE = '{ymds}' and PARTNO = '{partno}' and CM = '{cm}'
                            ORDER BY ITIME desc
                            FETCH FIRST 1 ROW ONLY";

            OracleCommand cmdWK = new OracleCommand();
            cmdWK.CommandText = strSV;

            DataTable _dt = oOraAL02.Query(cmdWK);

            if (_dt.Rows.Count > 0)
            {
                foreach (DataRow dr in _dt.Rows) {

                    string ymd = dr["IDATE"].ToString();
                    string itime = dr["ITIME"].ToString();

                    string newFormat = ymd + " " + itime;

                    DateTime result = DateTime.ParseExact(newFormat, "yyyyMMdd HHmm", CultureInfo.InvariantCulture);

                    DateTime _date = new DateTime(result.Year, result.Month, result.Day, result.Hour,result.Minute,result.Second);


                    string new_itime = _date.AddMinutes(1).ToString("HHmm");


                }
               

            }
            else
            {

            }

            return Ok("Success");
        }


    }
}
