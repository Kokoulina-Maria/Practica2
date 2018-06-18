using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class BookingRepository
    {
        private Model1Container db;

        public BookingRepository(Model1Container _cont)
        {
            db = _cont;
        }

        public IEnumerable<Booking> Bookings()
        {
            return db.BookingSet.OrderBy(cw => cw.Seat.Session.ID);
        }

        public Booking GetBooking(int id)
        {
            return db.BookingSet.Find(id);
        }

        public List<Booking> Find(List<Booking> f, string ent, string atr, string sign, string eqv, string eqv2)
        {
            List<Booking> result = new List<Booking>();
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
                case "Бронь":
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
                        List<Booking> cash = new List<Booking>();
                        foreach (Session x in cin)
                        {
                            foreach (Seat y in x.Seat)
                                if (y.Booking != null)
                                    cash.Add(y.Booking);
                        }
                        result = (from d in f select d).Intersect(from a in cash select a).ToList();
                        break;
                    }
            }
            return result;
        }
    }
}