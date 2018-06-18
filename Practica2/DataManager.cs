using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Practica2.Models;

namespace Practica2
{
    public class DataManager
    {
        private Model1Container cont;
        public BookingRepository BR;
        public CashierRepository CShR;
        public CinemaRepository CR;
        public FilmRepository FR;
        public HallRepository HR;
        public SeatRepository SR;
        public SessionRepository SsR;
        public TicketRepository TR;

        public DataManager()
        {
            cont = new Model1Container();
            BR = new BookingRepository(cont);
            CShR = new CashierRepository(cont);
            CR = new CinemaRepository(cont);
            FR = new FilmRepository(cont);
            HR = new HallRepository(cont);
            SR = new SeatRepository(cont);
            SsR = new SessionRepository(cont);
            TR = new TicketRepository(cont);
        }
    }
}