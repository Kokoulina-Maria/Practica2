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
    public class CashiersController : Controller
    {
        static private DataManager _DataManager= new DataManager();
        static public List<Practica2.Models.Cashier> list = _DataManager.CShR.Cashiers().ToList();
        static public List<Practica2.Models.Cashier> nowlist = _DataManager.CShR.Cashiers().ToList();
        static public List<Practica2.Models.Cashier> starlist = new List<Cashier>();

        // GET: Cashiers
        public ActionResult Index()
        {
            list = _DataManager.CShR.Cashiers().ToList();
            nowlist = _DataManager.CShR.Cashiers().ToList();
            starlist = new List<Cashier>();
            return View(_DataManager.CShR.Cashiers());
        }

        // GET: Cashiers/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            ViewData["Cinemas"] = new SelectList(_DataManager.CR.Cinemas(), "ID", "Name");
            return View();
        }

        // POST: Cashiers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string login, string FIO, string password, sbyte cinema)
        {
            Cinema cin = _DataManager.CR.GetCinema((int)cinema);
            if (cin == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(login))
                ModelState.AddModelError("Login", "Введите логин");
            if (string.IsNullOrWhiteSpace(FIO))
                ModelState.AddModelError("FIO", "Введите ФИО");
            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("Password", "Введите пасспорт");
            if (cinema < 0)
                ModelState.AddModelError("Cinema", "Выберите кинотеатр");
            if (_DataManager.CShR.Cheack(login, true, 0) == false)
                ModelState.AddModelError("Login", "Кассир с таким логином уже существует!");

            if (ModelState.IsValid)
            {
                _DataManager.CShR.Add(login, FIO, password, (byte)cinema);
                return RedirectToAction("Index");
            }

            ViewData["Cinemas"] = new SelectList(_DataManager.CR.Cinemas(), "ID", "Name");
            return View();
        }

        // GET: Cashiers/Edit/5
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cashier cashier = _DataManager.CShR.GetCashier((int)id);
            if (cashier == null)
            {
                return HttpNotFound();
            }
            return View(cashier);
        }

        // POST: Cashiers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(sbyte id, string login, string FIO, string password)
        {
            Cashier cashier = _DataManager.CShR.GetCashier((int)id);
            if (cashier == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(login))
                ModelState.AddModelError("Login", "Введите логин");

            if (string.IsNullOrWhiteSpace(FIO))
                ModelState.AddModelError("FIO", "Введите ФИО");

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("Password", "Введите пароль");

            if (_DataManager.CShR.Cheack(login, false, id) == false)
                ModelState.AddModelError("Login", "Кассир с таким логином уже существует!");

            if (ModelState.IsValid)
            {
                _DataManager.CShR.Edit(login, FIO, password, id);
                return RedirectToAction("Index");
            }
            return View(cashier);
        }

        // GET: Cashiers/Delete/5
        public ActionResult Delete(int? id, string back)
        {
            Cashier cashier = _DataManager.CShR.GetCashier((int)id);
            if (cashier == null)
            {
                return HttpNotFound();
            }
            _DataManager.CShR.Delete((int)id);

            return RedirectToAction(back);
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
            if (ModelState.IsValid)
            {
                string e = "Кассир";
                string a;
                if ((entity == "Город") || (entity == "Кинотеатр"))
                    e = "Кинотеатр";
                a = entity;
                if (entity == "Кинотеатр") a = "Название";
                if (extra == "И") Search(true, e, a, sign, text);
                else Search(false, e, a, sign, text);
            }
            return View(list);
        }
        public void Search(bool ok, string entity, string atribut, string sign, string text)
        {
            if (ok) nowlist = _DataManager.CShR.Find((List<Cashier>)nowlist, entity, atribut, sign, text);
            else
            {
                ((List<Cashier>)starlist).AddRange(nowlist);
                nowlist = _DataManager.CShR.Cashiers().ToList();
                nowlist = _DataManager.CShR.Find(nowlist, entity, atribut, sign, text);
            }
            List<Cashier> p = new List<Cashier>();
            p.AddRange(starlist);
            p.AddRange(nowlist);
            list = (from x in p select x).Distinct().ToList();
        }
        public ActionResult ExportToExcel(string back)
        {
            string fn = "Кассиры" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            List<Cashier> ex = new List<Cashier>();
            if (back == "Input")
                ex = _DataManager.CShR.Cashiers().ToList();
            else
                ex.AddRange(list);

            Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Лист1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Список ";
            worksheet.Cells[1, 1] = "ФИО";
            worksheet.Cells[1, 2] = "Логин";
            worksheet.Cells[1, 3] = "Пароль";
            worksheet.Cells[1, 4] = "Кинотеатр";

            for (int i = 0; i < ex.Count; i++)
            {
                worksheet.Cells[i + 2, 1] = ex[i].FIO;
                worksheet.Cells[i + 2, 2] = ex[i].Login;
                worksheet.Cells[i + 2, 3] = ex[i].Password;
                worksheet.Cells[i + 2, 4] = ex[i].Cinema.Name;
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
