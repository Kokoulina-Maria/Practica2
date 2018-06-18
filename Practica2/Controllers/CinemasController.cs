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
    public class CinemasController : Controller
    {
        static private DataManager _DataManager=new DataManager();
        static public List<Practica2.Models.Cinema> list = _DataManager.CR.Cinemas().ToList();
        static public List<Practica2.Models.Cinema> nowlist = _DataManager.CR.Cinemas().ToList();
        static public List<Practica2.Models.Cinema> starlist = new List<Cinema>();

        // GET: Cinemas
        public ActionResult Index()
        {
            list = _DataManager.CR.Cinemas().ToList();
            nowlist = _DataManager.CR.Cinemas().ToList();
            starlist = new List<Cinema>();
            return View(_DataManager.CR.Cinemas());
        }

        // GET: Cinemas/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cinemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string name, string city, string adress)
        {
            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("Name", "Введите название кинотеатра");

            if (string.IsNullOrWhiteSpace(adress))
                ModelState.AddModelError("Adress", "Введите адрес кинотеатра");

            if (string.IsNullOrWhiteSpace(city))
                ModelState.AddModelError("City", "Введите город кинотеатра");

            if (_DataManager.CR.Check(name, city, adress, true, 0) == false)
                ModelState.AddModelError("Name", "Уже существует кинотеатр с таким названием!");

            if (ModelState.IsValid)
            {
                _DataManager.CR.Add(adress, name, city);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Cinemas/Edit/5
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cinema cinema = _DataManager.CR.GetCinema((int)id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            return View(cinema);
        }

        // POST: Cinemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, string adress, string name, string city)
        {
            Cinema cinema = _DataManager.CR.GetCinema((int)id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("Name", "Введите название кинотеатра");

            if (string.IsNullOrWhiteSpace(adress))
                ModelState.AddModelError("Adress", "Введите адрес кинотеатра");

            if (string.IsNullOrWhiteSpace(city))
                ModelState.AddModelError("City", "Введите город кинотеатра");

            if (_DataManager.CR.Check(name, city, adress, false, id) == false)
                ModelState.AddModelError("Name", "Уже существует кинотеатр с таким названием!");

            if (ModelState.IsValid)
            {
                _DataManager.CR.Edit(name, city, adress, id);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Cinemas/Delete/5
        public ActionResult Delete(int? id, string back)
        {
            Cinema cinema = _DataManager.CR.GetCinema((int)id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            _DataManager.CR.Delete((int)id);

            return RedirectToAction(back);
        }

        public ActionResult Restore(int? id, string back)
        {
            Cinema cinema = _DataManager.CR.GetCinema((int)id);
            if (cinema == null)
            {
                return HttpNotFound();
            }
            _DataManager.CR.Restore((int)id);

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
                string e = "";
                string a;
                    e = "Кинотеатр";
                a = entity;
                if (extra == "И") Search(true, e, a, sign, text);
                else Search(false, e, a, sign, text);
            }
            return View(list);
        }
        public void Search(bool ok, string entity, string atribut, string sign, string text)
        {
            if (ok) nowlist = _DataManager.CR.Search((List<Cinema>)nowlist, atribut, sign, text);
            else
            {
                ((List<Cinema>)starlist).AddRange(nowlist);
                nowlist = _DataManager.CR.Cinemas().ToList();
                nowlist = _DataManager.CR.Search(nowlist, atribut, sign, text);
            }
            List<Cinema> p = new List<Cinema>();
            p.AddRange(starlist);
            p.AddRange(nowlist);
            list = (from x in p select x).Distinct().ToList();
        }
        public ActionResult ExportToExcel(string back)
        {
            List<Cinema> ex = new List<Cinema>();
            if (back == "Input")
                ex = _DataManager.CR.Cinemas().ToList();
            else
                ex.AddRange(list);

            Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Лист1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Список ";
            worksheet.Cells[1, 1] = "Название";
            worksheet.Cells[1, 2] = "Город";
            worksheet.Cells[1, 3] = "Адрес";
            worksheet.Cells[1, 4] = "Удален";

            for (int i = 0; i < ex.Count; i++)
            {
                worksheet.Cells[i + 2, 1] = ex[i].Name;
                worksheet.Cells[i + 2, 2] = ex[i].City;
                worksheet.Cells[i + 2, 3] = ex[i].Adress;
                worksheet.Cells[i + 2, 4] = ex[i].Deleted;
            }

            Col(worksheet, "A1", 15, 15);
            string save = "Кинотеатры" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
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
