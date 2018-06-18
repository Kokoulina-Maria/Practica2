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
    public class HallsController : Controller
    {
        static private DataManager _DataManager = new DataManager();
        static public List<Practica2.Models.Hall> list = _DataManager.HR.Halls().ToList();
        static public List<Practica2.Models.Hall> nowlist = _DataManager.HR.Halls().ToList();
        static public List<Practica2.Models.Hall> starlist = new List<Hall>();

        // GET: Halls
        public ActionResult Index()
        {
            list = _DataManager.HR.Halls().ToList();
            nowlist = _DataManager.HR.Halls().ToList();
            starlist = new List<Hall>();
            return View(_DataManager.HR.Halls());
        }

        // GET: Halls/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            ViewData["Cinemas"] = new SelectList(_DataManager.CR.Cinemas(), "ID", "Name");
            return View();
        }

        // POST: Halls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string num, string type, string amountOfRow, string amountOfSeats, int cinema)
        {
            Cinema cin = _DataManager.CR.GetCinema((int)cinema);
            if (cin == null)
            {
                return HttpNotFound();
            }
            int n, r, s;
            if (string.IsNullOrWhiteSpace(type))
                ModelState.AddModelError("Type", "Выберите тип зала");
            if (!int.TryParse(num, out n))
                ModelState.AddModelError("Num", "Неверный ввод");
            if (!int.TryParse(amountOfRow, out r))
                ModelState.AddModelError("AmountOfRow", "Неверный ввод");
            if (!int.TryParse(amountOfSeats, out s))
                ModelState.AddModelError("AmountOfSeats", "Неверный ввод");
            if (cinema < 0)
                ModelState.AddModelError("Cinema", "Выберите кинотеатр");
            if ((n <= 0)||(n>100))
                ModelState.AddModelError("Num", "Номер должен быть положительным");
            if ((r <= 0) || ((r > 20)))
                ModelState.AddModelError("AmountOfRow", "Количество рядов должно быть положительным, миньшим 20");
            if ((s <= 0) || ((s > 20)))
                ModelState.AddModelError("AmountOfSeats", "Количество мест в ряду должно быть положительным, меньшим 20");
            if (_DataManager.HR.Check((byte)cinema, n, true, 0) == false)
                ModelState.AddModelError("Num", "В данном кинотеатре уже есть зал с таким номером!");

            if (ModelState.IsValid)
            {
                _DataManager.HR.Add((byte)cinema, (byte)n, type, (byte)r, (byte)s);
                return RedirectToAction("Index");
            }

            ViewData["Cinemas"] = new SelectList(_DataManager.CR.Cinemas(), "ID", "Name");
            return View();
        }

        // GET: Halls/Edit/5
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hall hall = _DataManager.HR.GetHall((int)id);

            if (hall == null)
            {
                return HttpNotFound();
            }
            ViewData.Model = hall;
            List< SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "2D", Value = "2D", Selected = hall.Type == "2D" });
            types.Add(new SelectListItem { Text = "3D", Value = "3D", Selected = hall.Type == "3D" });
            types.Add(new SelectListItem { Text = "4D", Value = "4D", Selected = hall.Type == "4D" });
            types.Add(new SelectListItem { Text = "IMAX", Value = "IMAX", Selected = hall.Type == "IMAX" });
            types.Add(new SelectListItem { Text = "VIP", Value = "VIP", Selected = hall.Type == "VIP" });

            ViewBag.Type = types;

            return View(hall);
        }

        // POST: Halls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, string num, string type, string amountOfRow, string amountOfSeats)
        {
            Hall hall = _DataManager.HR.GetHall((int)id);
            if (hall == null)
            {
                return HttpNotFound();
            }
            int s, n, r;
            if (!int.TryParse(num, out n))
                ModelState.AddModelError("Num", "Неверный ввод");
            if (!int.TryParse(amountOfRow, out r))
                ModelState.AddModelError("AmountOfRow", "Неверный ввод");
            if (!int.TryParse(amountOfSeats, out s))
                ModelState.AddModelError("AmountOfSeats", "Неверный ввод");
            if (string.IsNullOrWhiteSpace(type))
                ModelState.AddModelError("Type", "Выберите тип зала");
            if (n <= 0)
                ModelState.AddModelError("Num", "Номер должен быть положительным");
            if ((r <= 0) || ((r > 20)))
                ModelState.AddModelError("AmountOfRow", "Количество рядов должно быть положительным, миньшим 20");
            if ((s <= 0) || ((s > 20)))
                ModelState.AddModelError("AmountOfSeats", "Количество мест в ряду должно быть положительным, меньшим 20");
            if (_DataManager.HR.Check(0,n, false, id) == false)
                ModelState.AddModelError("Num", "В данном кинотеатре уже есть зал с таким номером!");

            if (ModelState.IsValid)
            {
                _DataManager.HR.Edit(0,(byte)n, type, (byte)r, (byte)s, id);
                return RedirectToAction("Index");
            }
            ViewData.Model = hall;
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "2D", Value = "2D", Selected = hall.Type == "2D" });
            types.Add(new SelectListItem { Text = "3D", Value = "3D", Selected = hall.Type == "3D" });
            types.Add(new SelectListItem { Text = "4D", Value = "4D", Selected = hall.Type == "4D" });
            types.Add(new SelectListItem { Text = "IMAX", Value = "IMAX", Selected = hall.Type == "IMAX" });
            types.Add(new SelectListItem { Text = "VIP", Value = "VIP", Selected = hall.Type == "VIP" });

            ViewBag.Type = types;

            return View(hall);
        }

        public ActionResult Delete(int? id, string back)
        {
            Hall hall = _DataManager.HR.GetHall((int)id);

            if (hall == null)
            {
                return HttpNotFound();
            }
            _DataManager.HR.Delete((int)id);

            return RedirectToAction(back);
        }

        public ActionResult Restore(int? id, string back)
        {
            Hall hall = _DataManager.HR.GetHall((int)id);

            if (hall == null)
            {
                return HttpNotFound();
            }
            _DataManager.HR.Restore((int)id);

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
            if (((entity == "Номер зала") ||(entity == "Количество рядов") || (entity == "Количество мест в ряду")) && (!ReadInt(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if (!(((entity == "Номер зала") || (entity == "Количество рядов") || (entity == "Количество мест в ряду"))) && (!((sign == "=") || (sign == "!="))))
                ModelState.AddModelError("Exception", "Для сравнения строк можно использовать только = и !=");
            if (ModelState.IsValid)
            {
                string e = "Зал";
                string a;
                if ((entity == "Город") || (entity == "Кинотеатр"))
                    e = "Кинотеатр";
                a = entity;
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
        public void Search(bool ok, string entity, string atribut, string sign, string text)
        {
            if (ok) nowlist = _DataManager.HR.Search((List<Hall>)nowlist, entity, atribut, sign, text, text);
            else
            {
                ((List<Hall>)starlist).AddRange(nowlist);
                nowlist = _DataManager.HR.Halls().ToList();
                nowlist = _DataManager.HR.Search(nowlist, entity, atribut, sign, text, text);
            }
            List<Hall> p = new List<Hall>();
            p.AddRange(starlist);
            p.AddRange(nowlist);
            list = (from x in p select x).Distinct().ToList();
        }
        public ActionResult ExportToExcel(string back)
        {
            string fn = "Залы" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            List<Hall> ex = new List<Hall>();
            if (back == "Input")
                ex = _DataManager.HR.Halls().ToList();
            else
                ex.AddRange(list);

            Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Лист1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Список ";
            worksheet.Cells[1, 1] = "Номер";
            worksheet.Cells[1, 2] = "Тип";
            worksheet.Cells[1, 3] = "Количество рядов";
            worksheet.Cells[1, 4] = "Количество мест в ряду";
            worksheet.Cells[1, 5] = "Кинотеатр";
            worksheet.Cells[1, 6] = "Удален";

            for (int i = 0; i < ex.Count; i++)
            {
                worksheet.Cells[i + 2, 1] = ex[i].Num;
                worksheet.Cells[i + 2, 2] = ex[i].Type;
                worksheet.Cells[i + 2, 3] = ex[i].AmountOfRow;
                worksheet.Cells[i + 2, 4] = ex[i].AmountOfSeats;
                worksheet.Cells[i + 2, 5] = ex[i].Cinema.Name;
                worksheet.Cells[i + 2, 6] = ex[i].Deleted;
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
