using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class CustomerController : Controller
    {
        //private CustomerRepository repo = new CustomerRepository();
        private RoomAvRepository roomavRepo = new RoomAvRepository();
        private RoomRepository roomRepo = new RoomRepository();
        private NotificationRepository notiRepo = new NotificationRepository();
        private CustomerRepository cusRepo = new CustomerRepository();
        // GET: Book
        [HttpGet,]
        public ActionResult ConfirmRegistration()
        {
            return View();
        }
        

        [HttpGet]
        public ActionResult CustomerRegistrasion(int id)
        {
            var r = from room in this.roomRepo.GetAll()
                    where room.RoomId == id
                    select room.Catagory.ToString();

            var c = from room in this.roomRepo.GetAll()
                    select room.Catagory.ToString();

            ViewBag.SelectedCatagory = r;
            ViewBag.Catagory = c;
            return View();
        }

        public JsonResult CheckUsernameAvailability(string userdata)
        {
            DataContext db = new DataContext();
            System.Threading.Thread.Sleep(200);
            var SeachData = db.Customers.Where(x => x.Username == userdata).SingleOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        // POST: Book
        [HttpPost]
        public ActionResult CustomerRegistrasion(Customer customer, int id)
        {
            
                var r = from room in this.roomRepo.GetAll()
                        where room.RoomId == id
                        select room.Catagory.ToString();

                var c = from room in this.roomRepo.GetAll()
                        select room.Catagory.ToString();

                ViewBag.SelectedCatagory = r;
                ViewBag.Catagory = c;

                var av = from item in this.roomavRepo.GetAll()
                         where item.Availability == true && item.Room.Catagory == Request["catagory"]
                         select item.RoomId.ToString();

                ViewBag.list = av;


                customer.Catagory = Request["catagory"];
                this.cusRepo.Insert(customer);

                Notification n = new Notification()
                {
                    CustomerId = this.cusRepo.Get(customer.Username).CustomerId,
                    Seen = false,
                    Time = DateTime.Now
                };
                this.notiRepo.Insert(n);

                return View("ConfirmRegistration");
            
        }
        

        public ActionResult GetRoom(string catagory = "")
        {
            var av = from item in this.roomavRepo.GetAll()
                     where item.Availability == true && item.Room.Catagory == catagory
                     select item.RoomId.ToString();
            return Json(new { result = av }, JsonRequestBehavior.AllowGet);
        }
    }
}