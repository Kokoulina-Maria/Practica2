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
    public class FilmsController : Controller
    {
        static private DataManager _DataManager = new DataManager();
        static public List<Practica2.Models.Film> list = _DataManager.FR.Films().ToList();
        static public List<Practica2.Models.Film> nowlist = _DataManager.FR.Films().ToList();
        static public List<Practica2.Models.Film> starlist = new List<Film>();

        // GET: Films
        public ActionResult Index()
        {
            list = _DataManager.FR.Films().ToList();
            nowlist = _DataManager.FR.Films().ToList();
            starlist = new List<Film>();
            return View(_DataManager.FR.Films());
        }

        // GET: Films/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = _DataManager.FR.GetFilm((int)id);
            if (film == null)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // GET: Films/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string name, string description, string year, string length, string ageLimit, string producer)
        {
            int l, y;
            if (!int.TryParse(length, out l))
                ModelState.AddModelError("Length", "Неверный ввод");
            if (!int.TryParse(year, out y))
                ModelState.AddModelError("Year", "Неверный ввод");
            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("Name", "Введите название фильма");

            if (string.IsNullOrWhiteSpace(description))
                ModelState.AddModelError("Description", "Введите описание фильма");

            if ((y<1900)||(y > 3000))
                ModelState.AddModelError("Year", "Введите год правильно");

            if ((l < 0)||(l >240))
                ModelState.AddModelError("Length", "Введите длительность правильно (в минутах)");

            if (string.IsNullOrWhiteSpace(producer))
                ModelState.AddModelError("Producer", "Введите режиссера");

            if (_DataManager.FR.Check(name, true, 0)==false)
                ModelState.AddModelError("Name", "Уже существует фильм с таким названием!");

            if (ModelState.IsValid)
            {
                _DataManager.FR.Add(name, ageLimit, description, (byte)l, producer, (short)y, " ");
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Films/Edit/5
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = _DataManager.FR.GetFilm((int)id);
            if (film == null)
            {
                return HttpNotFound();
            }
            ViewData.Model = film;
            List<SelectListItem> films = new List<SelectListItem>();
            films.Add(new SelectListItem { Text = "0+", Value = "0+", Selected = film.AgeLimit == "0+" });
            films.Add(new SelectListItem { Text = "6+", Value = "6+", Selected = film.AgeLimit == "6+" });
            films.Add(new SelectListItem { Text = "12+", Value = "12+", Selected = film.AgeLimit == "12+" });
            films.Add(new SelectListItem { Text = "16+", Value = "16+", Selected = film.AgeLimit == "16+" });
            films.Add(new SelectListItem { Text = "18+", Value = "18+", Selected = film.AgeLimit == "18+" });
            ViewBag.AgeLimit = films;
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, string name, string description, string year, string length, string ageLimit, string producer)
        {
            Film film = _DataManager.FR.GetFilm((int)id);
            if (film == null)
            {
                return HttpNotFound();
            }
            int l, y;
            if (!int.TryParse(length, out l))
                ModelState.AddModelError("Length", "Неверный ввод");
            if (!int.TryParse(year, out y))
                ModelState.AddModelError("Year", "Неверный ввод");
            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("Name", "Введите название фильма");

            if (string.IsNullOrWhiteSpace(description))
                ModelState.AddModelError("Description", "Введите описание фильма");

            if ((y < 1900) || (y > 3000))
                ModelState.AddModelError("Year", "Введите год правильно");

            if ((l < 0) || (l > 240))
                ModelState.AddModelError("Length", "Введите длительность правильно (в минутах)");

            if (string.IsNullOrWhiteSpace(producer))
                ModelState.AddModelError("Producer", "Введите режиссера");

            if (_DataManager.FR.Check(name, false, id) == false)
                ModelState.AddModelError("Name", "Уже существует фильм с таким названием!");

            if (ModelState.IsValid)
            {
                _DataManager.FR.Edit(name, id, ageLimit, description, (byte)l, producer, (short)y, " ");
                return RedirectToAction("Index");
            }
            ViewData.Model = film;
            List<SelectListItem> films = new List<SelectListItem>();
            films.Add(new SelectListItem { Text = "0+", Value = "0+", Selected = film.AgeLimit == "0+" });
            films.Add(new SelectListItem { Text = "6+", Value = "6+", Selected = film.AgeLimit == "6+" });
            films.Add(new SelectListItem { Text = "12+", Value = "12+", Selected = film.AgeLimit == "12+" });
            films.Add(new SelectListItem { Text = "16+", Value = "16+", Selected = film.AgeLimit == "16+" });
            films.Add(new SelectListItem { Text = "18+", Value = "18+", Selected = film.AgeLimit == "18+" });
            ViewBag.AgeLimit = films;
            return View();
        }

        // GET: Films/Delete/5
        public ActionResult Delete(int? id, string back)
        {
            Film film = _DataManager.FR.GetFilm((int)id);
            if (film == null)
            {
                return HttpNotFound();
            }
            _DataManager.FR.Delete((int)id);

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
            if (((entity == "Год") || (entity == "Длительность")) && (!ReadInt(text)))
                ModelState.AddModelError("Exception", "Неверный ввод");
            if (!((entity == "Год") || (entity == "Длительность")) && (!((sign == "=") || (sign == "!="))))
                ModelState.AddModelError("Exception", "Для сравнения строк можно использовать только = и !=");
            if (ModelState.IsValid)
            {
                string e = "Фильм";
                string a;
                a = entity;
                if (entity == "Режиссер") a = "Продюссер";
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
            if (ok) nowlist = _DataManager.FR.Find((List<Film>)nowlist, atribut, sign, text, text);
            else
            {
                ((List<Film>)starlist).AddRange(nowlist);
                nowlist = _DataManager.FR.Films().ToList();
                nowlist = _DataManager.FR.Find(nowlist, atribut, sign, text, text);
            }
            List<Film> p = new List<Film>();
            p.AddRange(starlist);
            p.AddRange(nowlist);
            list = (from x in p select x).Distinct().ToList();
        }
        public ActionResult ExportToExcel(string back)
        {
            List<Film> ex = new List<Film>();
            if (back == "Input")
                ex = _DataManager.FR.Films().ToList();
            else
                ex.AddRange(list);

            Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Лист1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "Список ";
            worksheet.Cells[1, 1] = "Название";
            worksheet.Cells[1, 2] = "Год";
            worksheet.Cells[1, 3] = "Длительность";
            worksheet.Cells[1, 4] = "Возрастное ограничение";
            worksheet.Cells[1, 5] = "Режиссер";

            for (int i = 0; i < ex.Count; i++)
            {
                worksheet.Cells[i + 2, 1] = ex[i].Name;
                worksheet.Cells[i + 2, 2] = ex[i].Year;
                worksheet.Cells[i + 2, 3] = ex[i].length;
                worksheet.Cells[i + 2, 4] = ex[i].AgeLimit;
                worksheet.Cells[i + 2, 5] = ex[i].Producer;
            }

            Col(worksheet, "A1", 15, 15);
            string save = "Фильмы" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
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

        // POST: Films/Delete/5
    }
}
