using MobileBackend.DataAccess;
using MobileBackend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MobileBackend.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult HoursPerWorkAssignment()
        {
            TimesheetEntities entities = new TimesheetEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // haetaan kaikki kuluvan päivän tuntikirjaukset
                List<Timesheet> allTimesheetsToday = (from ts in entities.Timesheets
                                                      where (ts.StartTime > today) &&
                                                      (ts.StartTime < tomorrow) &&
                                                      (ts.WorkComplete == true)
                                                      select ts).ToList();

                // ryhmitellään kirjaukset tehtävittäin, ja lasketaan kestot
                List<HoursPerWorkAssignmentModel> model = new List<HoursPerWorkAssignmentModel>();

                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    int assignmentId = timesheet.Id_WorkAssignment.Value;
                    HoursPerWorkAssignmentModel existing = model.Where(
                        m => m.WorkAssignmentId == assignmentId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.TotalHours += (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours;
                    }
                    else
                    {
                        existing = new HoursPerWorkAssignmentModel()
                        {
                            WorkAssignmentId = assignmentId,
                            WorkAssignmentName = timesheet.WorkAssignment.Title,
                            TotalHours = (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours
                        };
                        model.Add(existing);
                    }
                }

                return View(model);
            }
            finally
            {
                entities.Dispose();
            }
        }

        public ActionResult HoursPerWorkAssignmentAsExcel()
        {
            // TODO: hae tiedot tietokannasta!
            StringBuilder csv = new StringBuilder();

            // luodaan CSV-muotoinen tiedosto
            csv.AppendLine("Matti;123,5");
            csv.AppendLine("Jesse;86,25");
            csv.AppendLine("Kaisa;99,00");

            // palautetaan CSV-tiedot selaimelle
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Työtunnit.csv");
        }

        public ActionResult HoursPerWorkAssignmentAsExcel2()
        {
            // TODO: hae tiedot tietokannasta!
            StringBuilder csv = new StringBuilder();

            // luodaan CSV-muotoinen tiedosto
            TimesheetEntities entities = new TimesheetEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // haetaan kaikki kuluvan päivän tuntikirjaukset
                List<Timesheet> allTimesheetsToday = (from ts in entities.Timesheets
                                                      where (ts.StartTime > today) &&
                                                      (ts.StartTime < tomorrow) &&
                                                      (ts.WorkComplete == true)
                                                      select ts).ToList();

                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    csv.AppendLine(timesheet.Id_Employee + ";" +
                        timesheet.StartTime + ";" + timesheet.StopTime+";");
                }
            }
            finally
            {
                entities.Dispose();
            }

            // palautetaan CSV-tiedot selaimelle
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Työtunnit.csv");
        }
        public ActionResult HoursPerWorkAssignmentAsExcel3(string start, string stop)
        {
            // TODO: hae tiedot tietokannasta!
            StringBuilder csv = new StringBuilder();

            // luodaan CSV-muotoinen tiedosto
            TimesheetEntities entities = new TimesheetEntities();
            try
            {
                //DateTime today = DateTime.Today;
                //DateTime tomorrow = today.AddDays(1);

                DateTime startDate = Convert.ToDateTime(start);
                DateTime endDate = Convert.ToDateTime(stop);

                // haetaan kaikki kuluvan päivän tuntikirjaukset
                List<Timesheet> allTimesheetsToday = (from ts in entities.Timesheets
                                                      where (ts.StartTime > startDate) &&
                                                      (ts.StartTime < endDate) &&
                                                      (ts.WorkComplete == true)
                                                      select ts).ToList();

                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    csv.AppendLine(timesheet.Id_Employee + ";" +
                        timesheet.StartTime + ";" + timesheet.StopTime + ";");
                }
            }
            finally
            {
                entities.Dispose();
            }

            // palautetaan CSV-tiedot selaimelle
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Työtunnit.csv");
        }
    }
}