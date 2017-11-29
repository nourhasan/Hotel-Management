using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int BookId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        //public virtual RoomAv RoomAvs { get; set; }
        public virtual Book Books { get; set; }
        public virtual ICollection<UserBalance> UserBalances { get; set; }
    }
}
