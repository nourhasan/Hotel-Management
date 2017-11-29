using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class BookingRepository : IBookingRepository
    {
        private DataContext context;

        public BookingRepository() { this.context = new DataContext(); }

        public List<Booking> GetAll()
        {
            return this.context.Bookings.ToList();
        }

        public Booking Get(int id)
        {
            return this.context.Bookings.SingleOrDefault(b => b.BookingId == id);
        }

        public int Insert(Booking booking)
        {
            this.context.Bookings.Add(booking);
            return this.context.SaveChanges();
        }

        public int Update(Booking booking)
        {
            Booking bookingToUpdate = this.context.Bookings.SingleOrDefault(b => b.BookingId == booking.BookingId);

            bookingToUpdate.BookId = booking.BookId;
            bookingToUpdate.CheckIn = booking.CheckIn;
            bookingToUpdate.CheckOut = booking.CheckOut;
            bookingToUpdate.RoomAvs.RoomAvId = booking.RoomAvId;
 
           
            return this.context.SaveChanges();
        }

        public int Delete(int id)
        {
            Booking bookingToDelete = this.context.Bookings.SingleOrDefault(b => b.BookingId == id);
            this.context.Bookings.Remove(bookingToDelete);

            return this.context.SaveChanges();
        }
    }
}
