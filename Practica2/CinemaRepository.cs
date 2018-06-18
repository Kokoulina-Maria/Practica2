using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class CinemaRepository
    {
        private Model1Container db;
        public CinemaRepository(Model1Container _cont)
        {
            db = _cont;
        }

        public IEnumerable<Cinema> Cinemas()
        {
            return db.CinemaSet.OrderBy(cw => cw.City);
        }

        public Cinema GetCinema(int id)
        {
            return db.CinemaSet.Find(id);
        }

        public bool Check(string name, string city, string adress, bool add, int ID)
        {
            bool ok = true;
            foreach (Cinema x in db.CinemaSet)
            {
                if ((x.City == city) && (x.Adress == adress) && ((add) || (!add) && (ID != x.ID)))
                {
                    //Response.Write("Кинотеатр по таком адресу уже существует!");
                    ok = false;
                    break;
                }
                if ((x.Name == name) && ((add) || (!add) && (ID != x.ID)))
                {
                    ok = false;
                    break;
                }
            }
            return ok;
        }

        public void Add(string adress, string name, string city)
        {
            db.CinemaSet.Add(new Cinema { Name = name, City = city, Adress = adress });
            db.SaveChanges();
        }

        public void Delete(int ID)
        {
            List<Hall> h = (db.CinemaSet.Find(ID)).Hall.ToList();
            foreach (Hall x in h)
            {//удаляем все кинотеатры данного фильма
                HallRepository ha = new HallRepository(db);
                ha.Delete(x.ID);
            }
            List<Cashier> c = (db.CinemaSet.Find(ID)).Сashier.ToList();
            foreach (Cashier x in c)
            {
                db.CashierSet.Remove(x);
            }
            db.CinemaSet.Find(ID).Deleted = true;
            db.SaveChanges();
        }

        public void Restore(int ID)
        {
            foreach (Hall x in (db.CinemaSet.Find(ID)).Hall)
            {//восстанавливаем все кинотеатры данного фильма 
                HallRepository ha = new HallRepository(db);
                ha.Restore(x.ID);
            }
            db.CinemaSet.Find((db.CinemaSet.Find(ID)).ID).Deleted = false;
            db.SaveChanges();
        }

        public void Edit(string name, string city, string adress, int ID)
        {
            db.CinemaSet.Find(ID).Name = name;
            db.CinemaSet.Find(ID).City = city;
            db.CinemaSet.Find(ID).Adress = adress;
            db.SaveChanges();
        }
        public List<Cinema> Search(List<Cinema> f, string atr, string sign, string eqv)
        {
            List<Cinema> result = null;
            switch (atr)
            {
                case "Название":
                    {
                        switch (sign)
                        {
                            case "=": { result = (from d in f where d.Name == eqv select d).ToList(); break; }
                            case "!=": { result = (from d in f where d.Name != eqv select d).ToList(); break; }
                        }
                        break;
                    }
                case "Адрес":
                    {
                        switch (sign)
                        {
                            case "=": { result = (from d in f where d.Adress == eqv select d).ToList(); break; }
                            case "!=": { result = (from d in f where d.Adress != eqv select d).ToList(); break; }
                        }
                        break;
                    }
                case "Город":
                    {
                        switch (sign)
                        {
                            case "=": { result = (from d in f where d.City == eqv select d).ToList(); break; }
                            case "!=": { result = (from d in f where d.City != eqv select d).ToList(); break; }
                        }
                        break;
                    }
            }
            return result;
        }
    }
}