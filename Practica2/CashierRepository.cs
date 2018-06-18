using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class CashierRepository
    {
        private Model1Container db;
        public CashierRepository(Model1Container _cont)
        {
            db = _cont;
        }

        public IEnumerable<Cashier> Cashiers()
        {
            return db.CashierSet.OrderBy(cw => cw.Cinema.ID);
        }

        public Cashier GetCashier(int id)
        {
            return db.CashierSet.Find(id);
        }

        public bool Cheack(string login, bool add, int ID)
        {
            bool ok = true;
            foreach (Cashier x in db.CashierSet)
            {
                if ((x.Login == login) && ((add) || (!add) && (ID != x.ID)))
                {
                    //MessageBox.Show("Кассир с таким логином уже существует!");
                    ok = false;
                    break;
                }
            }
            if (login == "Admin")
            {
                //MessageBox.Show("Пользователь с таким логином уже существует!");
                ok = false;
            }
            return ok;
        }

        public void Add(string login, string FIO, string password, byte cinema)
        {
            Cashier c = new Cashier();
            c.FIO = FIO;
            c.Login = login;
            c.Password = password;
            c.Cinema = db.CinemaSet.Find(cinema);
            db.CashierSet.Add(c);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            Cashier cw = GetCashier(id);
            if (cw != null)
            {
                db.CashierSet.Remove(cw);
                db.SaveChanges();
            }
        }

        public void Edit(string login, string FIO, string password, int ID)
        {
            db.CashierSet.Find(ID).FIO = FIO;
            db.CashierSet.Find(ID).Login = login;
            db.CashierSet.Find(ID).Password = password;
            db.SaveChanges();
        }

        public List<Cashier> Find(List<Cashier> f, string ent, string atr, string sign, string eqv)
        {
            List<Cashier> result = new List<Cashier>();
            switch (ent)
            {
                case "Кассир":
                    {
                        switch (atr)
                        {
                            case "ФИО":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.FIO == eqv select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.FIO != eqv select d).ToList(); break; }
                                    }
                                    break;
                                }
                            case "Логин":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.Login == eqv select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.Login != eqv select d).ToList(); break; }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case "Кинотеатр":
                    {
                        CinemaRepository c = new CinemaRepository(db);
                        List<Cinema> cin = c.Search(db.CinemaSet.ToList(), atr, sign, eqv);
                        List<Cashier> cash = new List<Cashier>();
                        foreach (Cinema x in cin)
                        {
                            cash.AddRange(x.Сashier);
                        }
                        result = (from d in f select d).Intersect(from a in cash select a).ToList();
                        break;
                    }
            }
            return result;
        }
    }
}