using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class SessionRepository
    {
        private Model1Container db;

        public SessionRepository(Model1Container _cont)
        {
            db = _cont;
        }

        public IEnumerable<Session> Sessions()
        {
            return db.SessionSet.OrderBy(cw => cw.Time);
        }

        public Session GetSession(int id)
        {
            return db.SessionSet.Find(id);
        }

        /// <summary>
        /// Добавление сеанса
        /// </summary>
        /// <param name="hall"></param>
        /// <param name="date"></param>
        /// <param name="film"></param>
        /// <param name="price"></param>
        public void Add(int hall, DateTime date, int film, short price)
        {
            Session c = new Session();
            c.Film = db.FilmSet.Find(film);
            c.Hall = db.HallSet.Find(hall);
            c.Price = price;
            c.Time = date;
            db.SessionSet.Add(c);
            db.SaveChanges();
            for (int i = 1; i <= c.Hall.AmountOfRow; i++)
                for (int j = 1; j <= c.Hall.AmountOfSeats; j++)
                {
                    SeatRepository l = new SeatRepository(db);
                    l.Add(c.ID, (byte)i, (byte)j);
                }
            db.SaveChanges();
        }
        /// <summary>
        /// Редактирование сеанса
        /// </summary>
        /// <param name="hall"></param>
        /// <param name="date"></param>
        /// <param name="film"></param>
        /// <param name="price"></param>
        /// <param name="ID"></param>
        public void Edit(short price, DateTime date, int ID)
        {
            db.SessionSet.Find(ID).Price = price;
            db.SessionSet.Find(ID).Time = date;
            db.SaveChanges();
        }
        /// <summary>
        /// Изменение сеанса
        /// </summary>
        /// <param name="hall"></param>
        /// <param name="date"></param>
        /// <param name="add"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool Check(int hall, DateTime date, bool add, int ID)
        {
            bool ok = true;
            TimeSpan q = new TimeSpan(4, 0, 0);
            foreach (Session x in db.SessionSet)
            {
                //if (((x.Time <= date) && ((x.Time + q) >= date) || (x.Time >= date) && ((date + q) >= x.Time)) 
                //    && ((add)&& (x.Hall.Cinema.Name == db.HallSet.Find(hall).Cinema.Name) && (x.Hall.Num == db.HallSet.Find(hall).Num) 
                //    || (!add) && (ID != x.ID)&& (x.Hall.Cinema.Name == db.HallSet.Find(db.SessionSet.Find(ID)).Cinema.Name) 
                //    && (x.Hall.Num == db.HallSet.Find(db.SessionSet.Find(ID)).Num)))
                //{
                //    //MessageBox.Show("В данном зале в это время уже идет сеанс!");
                //    ok = false;
                //    break;
                //}
                if (date <= DateTime.Now)
                {
                    //MessageBox.Show("Вы не можете добавить сеанс, который уже начался!");
                    ok = false;
                    break;
                }
            }
            return ok;
        }
        /// <summary>
        /// Проверка совместимости с БД
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        public void Delete(int ID)
        {
            List<Seat> se = db.SessionSet.Find(ID).Seat.ToList();
            foreach (Seat z in se)
            {
                SeatRepository c = new SeatRepository(db);
                c.Delete(z.ID);
            }
            db.SessionSet.Remove(db.SessionSet.Find(ID));
            db.SaveChanges();
        }
        /// <summary>
        /// Многопараметрический поиск Сеанса
        /// </summary>
        /// <param name="f"></param>
        /// <param name="ent"></param>
        /// <param name="atr"></param>
        /// <param name="sign"></param>
        /// <param name="eqv"></param>
        /// <param name="eqv2"></param>
        /// <returns></returns>
        public List<Session> Find(List<Session> f, string ent, string atr, string sign, string eqv, string eqv2)
        {
            List<Session> result = new List<Session>();
            switch (ent)
            {
                case "Фильм":
                    {
                        FilmRepository fil = new FilmRepository(db);
                        List<Film> cin = fil.Find(db.FilmSet.ToList(), atr, sign, eqv, eqv2);
                        List<Session> cash = new List<Session>();
                        foreach (Film x in cin)
                        {
                            cash.AddRange(x.Session);
                        }
                        result = (from d in f select d).Intersect(from a in cash select a).ToList();
                        break;
                    }
                case "Сеанс":
                    {
                        switch (atr)
                        {
                            case "Дата":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.Time.Date == DateTime.Parse(eqv).Date select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.Time.Date != DateTime.Parse(eqv).Date select d).ToList(); break; }
                                        case ">": { result = (from d in f where d.Time.Date > DateTime.Parse(eqv).Date select d).ToList(); break; }
                                        case "<": { result = (from d in f where d.Time.Date < DateTime.Parse(eqv).Date select d).ToList(); break; }
                                        case ">=": { result = (from d in f where d.Time.Date >= DateTime.Parse(eqv).Date select d).ToList(); break; }
                                        case "<=": { result = (from d in f where d.Time.Date <= DateTime.Parse(eqv).Date select d).ToList(); break; }
                                    }
                                    break;
                                }
                            case "Цена":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.Price == int.Parse(eqv) select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.Price != int.Parse(eqv) select d).ToList(); break; }
                                        case ">": { result = (from d in f where d.Price > int.Parse(eqv) select d).ToList(); break; }
                                        case "<": { result = (from d in f where d.Price < int.Parse(eqv) select d).ToList(); break; }
                                        case ">=": { result = (from d in f where d.Price >= int.Parse(eqv) select d).ToList(); break; }
                                        case "<=": { result = (from d in f where d.Price <= int.Parse(eqv) select d).ToList(); break; }
                                    }
                                    break;
                                }
                            case "Время":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.Time.TimeOfDay == TimeSpan.Parse(eqv) select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.Time.TimeOfDay != TimeSpan.Parse(eqv) select d).ToList(); break; }
                                        case ">": { result = (from d in f where d.Time.TimeOfDay > TimeSpan.Parse(eqv) select d).ToList(); break; }
                                        case "<": { result = (from d in f where d.Time.TimeOfDay < TimeSpan.Parse(eqv) select d).ToList(); break; }
                                        case ">=": { result = (from d in f where d.Time.TimeOfDay >= TimeSpan.Parse(eqv) select d).ToList(); break; }
                                        case "<=": { result = (from d in f where d.Time.TimeOfDay <= TimeSpan.Parse(eqv) select d).ToList(); break; }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        HallRepository c = new HallRepository(db);
                        List<Hall> cin = c.Search(db.HallSet.ToList(), ent, atr, sign, eqv, eqv2);
                        List<Session> cash = new List<Session>();
                        foreach (Hall x in cin)
                        {
                            cash.AddRange(x.Session);
                        }
                        result = (from d in f select d).Intersect(from a in cash select a).ToList();
                        break;
                    }
            }
            return result;
        }
    }
}