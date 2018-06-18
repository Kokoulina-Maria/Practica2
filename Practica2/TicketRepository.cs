using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class TicketRepository
    {
        private Model1Container db;

        public TicketRepository(Model1Container _cont)
        {
            db = _cont;
        }

        public IEnumerable<Ticket> Tickets()
        {
            return db.TicketSet.OrderBy(cw => cw.Seat.Session.ID);
        }

        public Ticket GetTicket(int id)
        {
            return db.TicketSet.Find(id);
        }

        public List<Ticket> Find(List<Ticket> f, string ent, string atr, string sign, string eqv, string eqv2)
        {
            List<Ticket> result = new List<Ticket>();
            switch (ent)
            {
                case "Место":
                    {
                        switch (atr)
                        {
                            case "Ряд":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.Seat.NumberOfRow == int.Parse(eqv) select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.Seat.NumberOfRow != int.Parse(eqv) select d).ToList(); break; }
                                        case ">": { result = (from d in f where d.Seat.NumberOfRow > int.Parse(eqv) select d).ToList(); break; }
                                        case "<": { result = (from d in f where d.Seat.NumberOfRow < int.Parse(eqv) select d).ToList(); break; }
                                        case ">=": { result = (from d in f where d.Seat.NumberOfRow >= int.Parse(eqv) select d).ToList(); break; }
                                        case "<=": { result = (from d in f where d.Seat.NumberOfRow <= int.Parse(eqv) select d).ToList(); break; }
                                    }
                                    break;
                                }
                            case "Номер в ряду":
                                {
                                    switch (sign)
                                    {
                                        case "=": { result = (from d in f where d.Seat.NumberOfSeat == int.Parse(eqv) select d).ToList(); break; }
                                        case "!=": { result = (from d in f where d.Seat.NumberOfSeat != int.Parse(eqv) select d).ToList(); break; }
                                        case ">": { result = (from d in f where d.Seat.NumberOfSeat > int.Parse(eqv) select d).ToList(); break; }
                                        case "<": { result = (from d in f where d.Seat.NumberOfSeat < int.Parse(eqv) select d).ToList(); break; }
                                        case ">=": { result = (from d in f where d.Seat.NumberOfSeat >= int.Parse(eqv) select d).ToList(); break; }
                                        case "<=": { result = (from d in f where d.Seat.NumberOfSeat <= int.Parse(eqv) select d).ToList(); break; }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case "Билет":
                    {
                        switch (sign)
                        {
                            case "=": { result = (from d in f where d.Number == int.Parse(eqv) select d).ToList(); break; }
                            case "!=": { result = (from d in f where d.Number != int.Parse(eqv) select d).ToList(); break; }
                            case ">": { result = (from d in f where d.Number > int.Parse(eqv) select d).ToList(); break; }
                            case "<": { result = (from d in f where d.Number < int.Parse(eqv) select d).ToList(); break; }
                            case ">=": { result = (from d in f where d.Number >= int.Parse(eqv) select d).ToList(); break; }
                            case "<=": { result = (from d in f where d.Number <= int.Parse(eqv) select d).ToList(); break; }
                        }
                        break;
                    }
                default:
                    {
                        SessionRepository c = new SessionRepository(db);
                        List<Session> cin = c.Find(db.SessionSet.ToList(), ent, atr, sign, eqv, eqv2);
                        List<Ticket> cash = new List<Ticket>();
                        foreach (Session x in cin)
                        {
                            foreach (Seat y in x.Seat)
                                if (y.Ticket != null)
                                    cash.Add(y.Ticket);
                        }
                        result = (from d in f select d).Intersect(from a in cash select a).ToList();
                        break;
                    }
            }
            return result;
        }
    }
}