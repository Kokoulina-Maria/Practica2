using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Practica2.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace Practica2.Controllers
{
    public class BookingsController : Controller
    {
        static private DataManager _DataManager = new DataManager();
        static public List<Practica2.Models.Booking> list = _DataManager.BR.Bookings().ToList();
        static public List<Practica2.Models.Booking> nowlist = _DataManager.BR.Bookings().ToList();
        static public List<Practica2.Models.Booking> starlist = new List<Booking>();

        // GET: Tickets
        public ActionResult Index()
        {
            list = _DataManager.BR.Bookings().ToList();
            nowlist = _DataManager.BR.Bookings().ToList();
            starlist = new List<Booking>();
            return View(_DataManager.BR.Bookings());
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Find()
        {
            return View(list);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Find(string entity, string sign, string text, string extra)
        {
            if (string.IsNullOrWhiteSpace(text))
                ModelState.AddModelError("Exception", "Введите искомое значение");
            if (((entity == "Год") || (entity == "Длительность") || (entity == "Номер зала") || (entity == "Цена") || (entity == "Номер") || (entity == "Ряд") || (entity == "Номер в ряду")) && (!ReadInt(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if ((entity == "Дата") && (!ReadDate(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if ((entity == "Время") && (!ReadTime(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if (!((entity == "Год") || (entity == "Длительность") || (entity == "Номер зала") || (entity == "Цена") || (entity == "Дата") || (entity == "Время") || (entity == "Номер") || (entity == "Ряд") || (entity == "Номер в ряду")) && (!((sign == "=") || (sign == "!="))))
                ModelState.AddModelError("Exception", "Для сравнения строк можно использовать только = и !=");
            if (ModelState.IsValid)
            {
                string e = "";
                string a;
                if ((entity == "Фильм") || (entity == "Год") || (entity == "Длительность") || (entity == "Возрастное ограничение") || (entity == "Режиссер"))
                    e = "Фильм";
                if ((entity == "Город") || (entity == "Кинотеатр"))
                    e = "Кинотеатр";
                if ((entity == "Номер зала") || (entity == "Тип зала"))
                    e = "Зал";
                if ((entity == "Дата") || (entity == "Цена") || (entity == "Время"))
                    e = "Сеанс";
                if ((entity == "Ряд") || (entity == "Номер в ряду"))
                    e = "Место";
                if (entity == "Номер")
                    e = "Бронь";
                a = entity;
                if (entity == "Фильм") a = "Название";
                if (entity == "Режиссер") a = "Продюссер";
                if (entity == "Кинотеатр") a = "Название";
                if (entity == "Номер зала") a = "Номер";
                if (entity == "Тип зала") a = "Тип";
                if (extra == "И") Search(true, e, a, sign, text);
                else Search(false, e, a, sign, text);
            }
            return View(list);
        }
        public bool ReadInt(string text)
        {
            int i;
            bool ok = int.TryParse(text, out i);
            return ok;
        }
        public bool ReadDate(string text)
        {
            bool ok = true;
            try
            {
                DateTime t = new DateTime();
                t = DateTime.Parse(text);
            }
            catch
            {
                ok = false;
            }
            return ok;
        }
        public bool ReadTime(string text)
        {
            bool ok = true;
            try
            {
                TimeSpan t = new TimeSpan();
                t = TimeSpan.Parse(text);
            }
            catch
            {
                ok = false;
            }
            return ok;
        }
        public void Search(bool ok, string entity, string atribut, string sign, string text)
        {
            if (ok) nowlist = _DataManager.BR.Find((List<Booking>)nowlist, entity, atribut, sign, text, text);
            else
            {
                ((List<Booking>)starlist).AddRange(nowlist);
                nowlist = _DataManager.BR.Bookings().ToList();
                nowlist = _DataManager.BR.Find(nowlist, entity, atribut, sign, text, text);
            }
            List<Booking> p = new List<Booking>();
            p.AddRange(starlist);
            p.AddRange(nowlist);
            list = (from x in p select x).Distinct().ToList();
        }
        public ActionResult ExportToExcel(string back)
        {
            List<Booking> ex = new List<Booking>();
            if (back == "Input")
                ex = _DataManager.BR.Bookings().ToList();
            else
                ex.AddRange(list);

            Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Лист1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Список ";
            worksheet.Cells[1, 1] = "Номер";
            worksheet.Cells[1, 2] = "Ряд";
            worksheet.Cells[1, 3] = "Место";
            worksheet.Cells[1, 4] = "Цена";
            worksheet.Cells[1, 5] = "Фильм";
            worksheet.Cells[1, 6] = "Зал";
            worksheet.Cells[1, 7] = "Кинотеатр";
            worksheet.Cells[1, 8] = "Дата и время";

            for (int i = 0; i < ex.Count; i++)
            {
                worksheet.Cells[i + 2, 1] = ex[i].Number;
                worksheet.Cells[i + 2, 2] = ex[i].Seat.NumberOfRow;
                worksheet.Cells[i + 2, 3] = ex[i].Seat.NumberOfSeat;
                worksheet.Cells[i + 2, 4] = ex[i].Seat.Session.Price;
                worksheet.Cells[i + 2, 5] = ex[i].Seat.Session.Film.Name;
                worksheet.Cells[i + 2, 6] = ex[i].Seat.Session.Hall.Num;
                worksheet.Cells[i + 2, 7] = ex[i].Seat.Session.Hall.Cinema.Name;
                worksheet.Cells[i + 2, 8] = ex[i].Seat.Session.Time;
            }

            Col(worksheet, "A1", 15, 15);
            string save = "Брони" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            workbook.SaveAs(save, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            app.Quit();
            ReleaseOb(worksheet);
            ReleaseOb(workbook);
            ReleaseOb(app);
            return RedirectToAction(back);
        }
        public void Col(Microsoft.Office.Interop.Excel._Worksheet sheet, string start, int rows, int col)
        {
            Excel.Range r = sheet.get_Range(start, System.Reflection.Missing.Value);
            r = r.get_Resize(rows, col);
            r.Columns.AutoFit();
            ReleaseOb(r);
        }
        public void ReleaseOb(object ob)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ob);
                ob = null;
            }
            catch
            {
                ob = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
