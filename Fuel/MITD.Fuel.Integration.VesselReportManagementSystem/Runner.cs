using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MITD.Fuel.Integration.VesselReportManagementSystem.Data;
using MITD.Fuel.Integration.VesselReportManagementSystem.ServiceWrapper;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;

namespace MITD.Fuel.Integration.VesselReportManagementSystem
{
    public class Runner
    {
        private VoyageCostEntities db;
        private ServiceWrapper.VesselReportServiceWrapper _reportServiceWrapper;
        public Runner()
        {
            db = new VoyageCostEntities();
        }

        public void Start()
        {
            var report = new RPMInfo();
            var resultReport = new ResultFuelReportDto();
            try
            {

                _reportServiceWrapper = new VesselReportServiceWrapper();

                var lst = GetReports();
                WriteMessage(true, lst.Count);

                foreach (var c in lst)
                {
                    var syncEvent = new AutoResetEvent(false);
                    _reportServiceWrapper.Add((res, exception) =>
                    {
                        report = c;
                        resultReport = res;
                        syncEvent.Set();


                    }, Mapper.ReportMapper.Map(c));


                    syncEvent.WaitOne();

                    var befortype = c.State;
                    c.State = (int)resultReport.Type;
                    Update(c);

                    if (resultReport.Type == ResultType.Exception)
                        LogService(new Exception(resultReport.Message), c.ID);

                    WriteMessage(c.ID.ToString(),
                        ((ResultType)befortype).ToString(),
                        ((ResultType)resultReport.Type).ToString(),
                        resultReport.Message);
                }



                WriteMessage(false, 0);


            }
            catch (Exception ex)
            {
                report.State = (int)ResultType.Failure;
                Update(report);
                LogService(ex, report.ID);
                HandellException(ex);
            }

        }

        private List<Data.RPMInfo> GetReports()
        {

            return db.RPMInfoes.Where(c =>
                c.State == (int)ResultType.Exception || c.State == (int)ResultType.New ||
                c.State == (int)ResultType.Failure).ToList();
        }

        private void Update(Data.RPMInfo rpmInfo)
        {
            try
            {

                db.Entry(rpmInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogService(ex, rpmInfo.ID);
                HandellException(ex);
            }


        }

        private void WriteMessage(string recordId, string beforState, string afterState, string message)
        {
            var stringBuilder = new StringBuilder();


            var msg = string.Format("Record Id : {0} ,Befor State: {1} ,After State: {2} ", recordId, beforState,
           afterState);

            stringBuilder.AppendLine(msg);
            if (!String.IsNullOrEmpty(message))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                stringBuilder.AppendLine("Message : " + message);
                System.Console.ResetColor();
            }
            stringBuilder.AppendLine("=========================================================================");
            System.Console.WriteLine(stringBuilder);



        }

        private void WriteMessage(bool startFlag, int count)
        {
            if (startFlag)
                System.Console.WriteLine("Count Of Record : {0}   Start Proccess ........", count.ToString());
            else
            {
                System.Console.WriteLine("====================== End of Proccess ======================");
                System.Console.ReadKey();
            }
        }

        private void WriteMessage(Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("System Failure ==> ");
            System.Console.ResetColor();
            System.Console.WriteLine(ex.Message);
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Stack Trace ==> ");
            System.Console.ResetColor();
            System.Console.WriteLine(ex.StackTrace);
            System.Console.ReadKey();



        }

        private void HandellException(Exception ex)
        {
            WriteMessage(ex);
        }

        private void LogService(Exception ex, int recordId)
        {
            try
            {
                FuelReportLog fuelReportLog = new FuelReportLog()
                {
                    Date = DateTime.Now,
                    FailureMessage = ex.Message,
                    RecordId = recordId,
                    StackTrace = ex.StackTrace

                };

                db.Entry(fuelReportLog).State = EntityState.Added;
                db.SaveChanges();
            }
            catch (Exception exp)
            {

                HandellException(exp);
            }


        }

    }
}
