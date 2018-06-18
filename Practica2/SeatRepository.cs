using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class SeatRepository
    {
        private Model1Container db;

        public SeatRepository(Model1Container _cont)
        {
            db = _cont;
        }

        public IEnumerable<Seat> Seats()
        {
            return db.SeatSet.OrderBy(cw => cw.Session.ID);
        }

        public Seat GetSeat(int id)
        {
            return db.SeatSet.Find(id);
        }

        /// <summary>
        /// Добавление места
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="DB"></param>
        public void Add(int ID, byte i, byte j)
        {
            Seat s = new Seat();
            s.Session = db.SessionSet.Find(ID);
            s.State = "Свободно";
            s.NumberOfRow = i;
            s.NumberOfSeat = j;
            db.SeatSet.Add(s);
            db.SaveChanges();
        }

        public void Osvobodit(Int64 ID)
        {
            DeleteTickets(ID);
            db.SeatSet.Find(ID).State = "Свободно";
            db.SaveChanges();
        }
        public void Zanat(Int64 ID)
        {
            DeleteTickets(ID);
            db.SeatSet.Find(ID).State = "Занято";
            Ticket t = new Ticket();
            t.Seat = db.SeatSet.Find(ID);
            db.TicketSet.Add(t);
            db.SaveChanges();
        }
        public void Bron(Int64 ID)
        {
            DeleteTickets(ID);
            db.SeatSet.Find(ID).State = "Забронировано";
            Booking t = new Booking();
            t.Seat = db.SeatSet.Find(ID);
            db.BookingSet.Add(t);
            db.SaveChanges();
        }

        /// <summary>
        /// Удаление места
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        public void Delete(Int64 ID)
        {
            DeleteTickets(ID);
            db.SeatSet.Remove(db.SeatSet.Find(ID));
        }
        public void DeleteTickets(Int64 ID)
        {
            Ticket t;
            if (db.SeatSet.Find(ID).Ticket != null)
            {//удаляем билеты, если есть
                t = (db.SeatSet.Find(ID)).Ticket;
                db.TicketSet.Remove(t);
            }
            Booking r;
            if ((db.SeatSet.Find(ID)).Booking != null)
            {//удаляем брони, если есть
                r = (db.SeatSet.Find(ID)).Booking;
                db.BookingSet.Remove(r);
            }
        }
    }
}