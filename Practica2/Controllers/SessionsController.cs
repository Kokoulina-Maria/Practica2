using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Practica2.Models;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Practica2.Controllers
{
    public class SessionsController : Controller
    {
        static private DataManager _DataManager = new DataManager();
        static public List<Practica2.Models.Session> list = _DataManager.SsR.Sessions().ToList();
        static public List<Practica2.Models.Session> nowlist=_DataManager.SsR.Sessions().ToList();
        static public List<Practica2.Models.Session> starlist= new List<Session>();

        // GET: Sessions
        public ActionResult Index()
        {
            list = _DataManager.SsR.Sessions().ToList();
            nowlist = _DataManager.SsR.Sessions().ToList();
            starlist = new List<Session>();
            return View(_DataManager.SsR.Sessions());
        }

        // GET: Sessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = _DataManager.SsR.GetSession((int)id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        public ActionResult Zanat(int? id)
        {
            try
            {
                _DataManager.SR.Zanat((Int64)id);
            }
            catch
            {
                return HttpNotFound();
            }

            return RedirectToAction("Details", new { id=_DataManager.SR.GetSeat((int)id).Session.ID});
        }
        public ActionResult Osvobodit(int? id)
        {
            try
            {
                _DataManager.SR.Osvobodit((Int64)id);
            }
            catch
            {
                return HttpNotFound();
            }

            return RedirectToAction("Details", new { id = _DataManager.SR.GetSeat((int)id).Session.ID });
        }
        public ActionResult Bron(int? id)
        {
            try
            {
                _DataManager.SR.Bron((Int64)id);
            }
            catch
            {
                return HttpNotFound();
            }

            return RedirectToAction("Details", new { id = _DataManager.SR.GetSeat((int)id).Session.ID });
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
            if (((entity=="Год")|| (entity == "Длительность")|| (entity == "Номер зала") || (entity == "Цена"))&&(!ReadInt(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if ((entity=="Дата")&&(!ReadDate(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if ((entity == "Время") && (!ReadTime(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if (!((entity == "Год") || (entity == "Длительность") || (entity == "Номер зала") || (entity == "Цена")|| (entity == "Дата")|| (entity == "Время"))&&(!((sign=="=")|| (sign == "!="))))
                ModelState.AddModelError("Exception", "Для сравнения строк можно использовать только = и !=");
            if (ModelState.IsValid)
            {
                string e="";
                string a;
                if ((entity == "Фильм") || (entity == "Год") || (entity == "Длительность") || (entity == "Возрастное ограничение") || (entity == "Режиссер"))
                    e = "Фильм";
                if ((entity == "Город") || (entity == "Кинотеатр"))
                    e = "Кинотеатр";
                if ((entity == "Номер зала") || (entity == "Тип зала"))
                    e = "Зал";
                if ((entity == "Дата") || (entity == "Цена") || (entity == "Время"))
                    e = "Сеанс";
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
            if (ok) nowlist = _DataManager.SsR.Find((List<Session>)nowlist, entity, atribut, sign, text, text);
            else
            {
                ((List<Session>)starlist).AddRange(nowlist);
                nowlist = _DataManager.SsR.Sessions().ToList();
                nowlist= _DataManager.SsR.Find(nowlist, entity, atribut, sign, text, text);
            }
            List<Session> p = new List<Session>();
            p.AddRange(starlist);
            p.AddRange(nowlist);
            list = (from x in p select x).Distinct().ToList();
        }

        // GET: Sessions/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            ViewData["Halls"] = new SelectList(_DataManager.HR.Halls(), "ID", "Num");
            ViewData["Films"] = new SelectList(_DataManager.FR.Films(), "ID", "Name");
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string price, string date, string time, sbyte hall, int film)
        {
            DateTime d;
            DateTime h;
            short p;
            Hall session = _DataManager.HR.GetHall((int)hall);
            if (session == null)
            {
                return HttpNotFound();
            }
            Film session1 = _DataManager.FR.GetFilm((int)film);
            if (session1 == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(date))
                ModelState.AddModelError("Date", "Введите дату");
            if (!DateTime.TryParse(date, out d))
                ModelState.AddModelError("Date", "Дата введена неверно");
            if (string.IsNullOrWhiteSpace(time))
                ModelState.AddModelError("Time", "Введите время");
            if (!short.TryParse(price, out p))
                ModelState.AddModelError("Price", "Цена введена неверно");
            if (string.IsNullOrWhiteSpace(price))
                ModelState.AddModelError("Price", "Введите время");
            if (film < 0)
                ModelState.AddModelError("Film", "Выберите фильм");
            if (hall < 0)
                ModelState.AddModelError("Hall", "Выберите зал");
            if ((p < 0) || (p > 10000))
                ModelState.AddModelError("Price", "Цена должна быть неотрицательным числом, не большим 10000");                       
            if (ModelState.IsValid)
            {
                d = DateTime.Parse(date);
                h = DateTime.Parse(time);
                DateTime c = new DateTime(d.Year, d.Month, d.Day, h.Hour, h.Minute, h.Second);
                if (_DataManager.SsR.Check(hall, c, true, 0) == false)
                    ModelState.AddModelError("Time", "В данном зале в это время уже идет сеанс и/или данный сеанс уже начался");
                if (ModelState.IsValid)
                {
                    _DataManager.SsR.Add(hall, c, film, p);
                    return RedirectToAction("Index");
                }
            }
            ViewData["Halls"] = new SelectList(_DataManager.HR.Halls(), "ID", "Num");
            ViewData["Films"] = new SelectList(_DataManager.FR.Films(), "ID", "Name");
            return View();
        }

        // GET: Sessions/Edit/5
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int? id)
       {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = _DataManager.SsR.GetSession((int)id);
            if (session == null)
            {
                return HttpNotFound();
            }
            ViewData.Model = session;
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, string price, string time)
        {
            Session session = _DataManager.SsR.GetSession((int)id);
            if (session == null)
            {
                return HttpNotFound();
            }
            short p;
            DateTime d;
            if (!short.TryParse(price, out p))
                ModelState.AddModelError("Price", "Цена введена неверно");
            else
            if ((p < 0) || (p > 10000))
                ModelState.AddModelError("Price", "Цена должна быть неотрицательным числом, не большим 10000");
            if ((!string.IsNullOrWhiteSpace(time)) &&(!DateTime.TryParse(time, out d)))
                ModelState.AddModelError("Time", "Дата и время введены неверно");
            if (ModelState.IsValid)
            {
                DateTime c;
                if ((!string.IsNullOrWhiteSpace(time)))
                {
                    d = DateTime.Parse(time);
                    c = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
                    if (_DataManager.SsR.Check(_DataManager.SsR.GetSession(id).Hall.ID, c, false, id) == false)
                        ModelState.AddModelError("Time", "В данном зале в это время уже идет сеанс и/или данный сеанс уже начался");
                }
                else c = _DataManager.SsR.GetSession(id).Time;
                if (ModelState.IsValid)
                {
                    _DataManager.SsR.Edit(p, c, id);
                    return RedirectToAction("Index");
                }
            }          
            ViewData.Model = session;
            return View();
        }

        // GET: Sessions/Delete/5
        public ActionResult Delete(int? id, string back)
        {
            Session session = _DataManager.SsR.GetSession((int)id);
            if (session == null)
            {
                return HttpNotFound();
            }
            _DataManager.SsR.Delete((int)id);
            return RedirectToAction(back);
        }

        public ActionResult ExportToExcel(string back)
        {
            string fn = "Сеансы" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            List<Session> ex= new List<Session>();
            if (back == "Input")
                ex = _DataManager.SsR.Sessions().ToList();
            else
                ex.AddRange(list);
            
            Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Лист1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Список ";
            worksheet.Cells[1, 1] = "Цена";
            worksheet.Cells[1, 2] = "Дата и время";
            worksheet.Cells[1, 3] = "Фильм";
            worksheet.Cells[1, 4] = "Зал";
            worksheet.Cells[1, 5] = "Кинотеатр";
            worksheet.Cells[1, 6] = "Город";

            for (int i = 0; i < ex.Count; i++)
            {
                worksheet.Cells[i+2, 1] = ex[i].Price;
                worksheet.Cells[i + 2, 2] = ex[i].Time;
                worksheet.Cells[i+2, 3] = ex[i].Film.Name;
                worksheet.Cells[i+2, 4] = ex[i].Hall.Num;
                worksheet.Cells[i+2, 5] = ex[i].Hall.Cinema.Name;
                worksheet.Cells[i+2, 6] = ex[i].Hall.Cinema.City;
            }

            Col(worksheet, "A1", 15, 15);
            workbook.SaveAs(fn, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
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
