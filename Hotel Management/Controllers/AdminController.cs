using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class AdminController : Controller
    {
        private AdminRepository adminRepo = new AdminRepository();
        private EmployeeRepository empRepo = new EmployeeRepository();
        private RoomRepository roomRepo = new RoomRepository();
        private RoomAvRepository roomavRepo = new RoomAvRepository();
        private BookingRepository bookingRepo = new BookingRepository();
        private UserBalanceRepository UserBalanceRepo = new UserBalanceRepository();
        private CustomerRepository cusRepo = new CustomerRepository();
        private NotificationRepository notiRepo = new NotificationRepository();
        private MessageRepository msgRepo = new MessageRepository();
        public static int tempId = 0;
        public ActionResult Index()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View();
        }
        //admin signup and details
        public ActionResult AdminProfile()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.adminRepo.GetAll());
        }
        [HttpGet]
        public ActionResult CreateAdmin()
        {

            return View();
        }
        public JsonResult AdminUsernameAvailability(string userdata)
        {
            DataContext db = new DataContext();
            System.Threading.Thread.Sleep(200);
            var SeachData = db.Admins.Where(x => x.UserName == userdata).SingleOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(Admin admin)
        {

            if (ModelState.IsValid)
            {

                this.adminRepo.Insert(admin);
                return RedirectToAction("AdminProfile");
            }
            else
            {
                return View(admin);
            }
        }
        [HttpGet]
        public ActionResult AdminProfileEdit(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.adminRepo.Get(id));
        }

        [HttpPost]
        public ActionResult AdminProfileEdit(Admin admin)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }

            this.adminRepo.Update(admin);
            return RedirectToAction("AdminProfile");


        }

        [HttpGet]
        public ActionResult AdminProfileDetails(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.adminRepo.Get(id));
        }

        [HttpGet]
        public ActionResult AdminProfileDelete(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.adminRepo.Get(id));
        }

        [HttpPost, ActionName("AdminProfileDelete")]
        public ActionResult AdminProfileConfirmDelete(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            this.adminRepo.Delete(id);
            return RedirectToAction("AdminProfile");
        }
        [HttpGet]
        public ActionResult BookCustomer(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            tempId = id;
            Customer cs = this.cusRepo.Get(id);

            var avRoom = from item in this.roomavRepo.GetAll()
                         where item.Availability == true && item.Room.Catagory == cs.Catagory
                         select item.RoomAvId;
            ViewBag.avRoom = avRoom;

            return View(cs);
        }

        public float TotalCost(int id)
        {
            float TotalCost = 0;

            var cus = from item in this.bookingRepo.GetAll()
                      where item.CustomerId == id
                      select item.TotalCost;

            foreach (var item in cus)
            {
                TotalCost = (float)(TotalCost + item);
            }

            return TotalCost;
        }

        [HttpPost]
        public ActionResult BookCustomer()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            Customer cs = this.cusRepo.Get(tempId);

            Booking b = new Booking()
            {
                CustomerId = cs.CustomerId,
                RoomAvId = Int32.Parse(Request["room"]),
                TotalCost = this.roomRepo.Get(cs.Catagory).Rent * cs.Nights
            };

            this.bookingRepo.Insert(b);

            RoomAv rav = this.roomavRepo.Get(b.RoomAvId);
            rav.Availability = false;
            this.roomavRepo.Update(rav);

            UserBalance xyz = new UserBalance();
            xyz = this.UserBalanceRepo.GetByCustomer(cs.CustomerId);

            var ubc = from item in this.UserBalanceRepo.GetAll()
                      where item.CustomerId == cs.CustomerId
                      select item.CustomerId;

            try
            {
                if (xyz.CustomerId == 0)
                {
                    UserBalance balance = new UserBalance()
                    {
                        CustomerId = cs.CustomerId,
                        UserBalanceAmount = TotalCost(cs.CustomerId),
                        Paid = 0,
                        RemainingAcount = TotalCost(cs.CustomerId)
                    };
                    this.UserBalanceRepo.Insert(balance);
                }
                else
                {
                    UserBalance balance = this.UserBalanceRepo.GetByCustomer(cs.CustomerId);
                    balance.UserBalanceAmount = TotalCost(cs.CustomerId);
                    balance.RemainingAcount = TotalCost(cs.CustomerId);
                    this.UserBalanceRepo.Update(balance);
                }
            }
            catch (Exception ex)
            {
                UserBalance balance = new UserBalance()
                {
                    CustomerId = cs.CustomerId,
                    UserBalanceAmount = TotalCost(cs.CustomerId),
                    Paid = 0,
                    RemainingAcount = TotalCost(cs.CustomerId)
                };
                Console.WriteLine(ex.ToString());
                this.UserBalanceRepo.Insert(balance);
            }
            

            return View("Index");
        }
        
        public ActionResult Bookings()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.bookingRepo.GetAll());
        }
        
        public ActionResult DetailsBooking(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.cusRepo.Get(id));
        }

        [HttpGet]
        public ActionResult Balancesheet(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.UserBalanceRepo.GetByCustomer(id));
        }

        public ActionResult UserBalances()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.UserBalanceRepo.GetAll());
        }

        [HttpGet]
        public ActionResult EditUserBalance(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.UserBalanceRepo.Get(id));
        }

        [HttpPost]
        public ActionResult EditUserBalance(UserBalance ub)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            ub.Paid = ub.Paid;
            ub.RemainingAcount = ub.UserBalanceAmount - ub.Paid;
            this.UserBalanceRepo.Update(ub);
            return Redirect("/Admin/DetailsUserBalance?id=" + ub.CustomerId);
        }

        public ActionResult DetailsUserBalance(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.UserBalanceRepo.Get(id));
        }

        //Customer Start -------------------------------------------------------------------
        public ActionResult Customers()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.cusRepo.GetAll());
        }


        [HttpGet]
        public ActionResult EditCustomer(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.cusRepo.Get(id));
        }

        [HttpPost]
        public ActionResult EditCustomer(Customer customer)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.cusRepo.Update(customer);
                return RedirectToAction("Customers");
            }
            else
            {
                return View(customer);
            }
        }

        [HttpGet]
        public ActionResult DetailsCustomer(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.cusRepo.Get(id));
        }

        [HttpGet]
        public ActionResult DeleteCustomer(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.cusRepo.Get(id));
        }

        [HttpPost, ActionName("DeleteCustomer")]
        public ActionResult DeleteConfirmedCustomer(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            this.cusRepo.Delete(id);
            return RedirectToAction("Customers");
        }
        // Customer End -------------------------------------------------------------------


        public ActionResult Notifications()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.notiRepo.GetAll());
        }

        [HttpGet]
        public ActionResult DetailsNotification(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }

            Notification n = this.notiRepo.Get(id);
            n.Seen = true;
            this.notiRepo.Update(n);

            return View(this.cusRepo.Get(id));
        }

        [HttpGet]
        public ActionResult DeleteNotification(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.notiRepo.Get(id));
        }

        [HttpPost, ActionName("DeleteNotification")]
        public ActionResult DeleteConfirmedNotification(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            this.notiRepo.Delete(id);
            return RedirectToAction("Notifications");
        }


        
        public ActionResult GetNotifications()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            var noti = from item in this.notiRepo.GetAll()
                       where item.Seen == false
                       select item.NotificationId.ToString();
            return Json(new { result = noti }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMessages()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            var msg = from item in this.msgRepo.GetAll()
                       where item.Seen == false
                       select item.MessageId.ToString();
            return Json(new { result = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Messages()
        {
            return View(this.msgRepo.GetAll());
        }

        public ActionResult DetailsMessage(int id)
        {
            Message msg = this.msgRepo.Get(id);
            msg.Seen = true;
            this.msgRepo.Update(msg);
            return View(msg);
        }

        [HttpGet]
        public ActionResult DeleteMessage(int id)
        {
            return View(this.msgRepo.Get(id));
        }

        [HttpPost, ActionName("DeleteMessage")]
        public ActionResult ConfirmDeleteMessage(int id)
        {
            this.msgRepo.Delete(id);
            return View();
        }

        // Notification End -------------------------------------------------------------------

        //Room av-----------------------------------------------

        [HttpGet]
        public ActionResult AvailableRoom()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            //DataContext db = new DataContext();
            List<RoomAv> roomlist = roomavRepo.GetAll();
            RoomAvModel roomAvModel = new RoomAvModel();
            List<RoomAvModel> roomMList = roomlist.Select
                (x => new RoomAvModel
                {
                    RoomAvId = x.RoomAvId,
                    RoomId = x.RoomId,
                    Catagory = x.Room.Catagory ,
                    Rent =x.Room.Rent}).ToList();
            

            return View(roomMList);
        }
        [HttpGet]
        public ActionResult CreateRoomAv()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
           // DataContext db = new DataContext();
            List<Room> roomList = roomRepo.GetAll();
            ViewBag.RoomList = new SelectList(roomList,"RoomId","Catagory");
            
            return View();
            
        }
        [HttpPost]
        public ActionResult CreateRoomAv(RoomAv roomAv)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.roomavRepo.Insert(roomAv);
                return RedirectToAction("AvailableRoom");
            }
            else
            {
                return View(roomAv);
            }
        }
        [HttpGet]
        public ActionResult EditRoomAv(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }

            //DataContext db = new DataContext();


            return View(this.roomavRepo.Get(id));


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoomAv(RoomAv roomAv)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.roomavRepo.Update(roomAv);
                return RedirectToAction("AvailableRoom");
            }
            else
            {
                return View(roomAv);
            }
        }
        [HttpGet]
        public ActionResult DetailsRoomAv(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            DataContext db = new DataContext();
            RoomAv roomlist = db.Roomavs.SingleOrDefault(x => x.RoomAvId == id);
            RoomAvModel roomAvModel = new RoomAvModel();
            roomAvModel.RoomAvId = roomlist.RoomAvId;
            roomAvModel.Catagory = roomlist.Room.Catagory;
            roomAvModel.Rent = roomlist.Room.Rent;
            return View(roomAvModel);
        }

     [HttpGet]
        public ActionResult DeleteRoomAv(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.roomavRepo.Get(id));
        }

        [HttpPost, ActionName("DeleteRoomAv")]
        public ActionResult DeleteConfirmedRoomAv(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            this.roomavRepo.Delete(id);
            return RedirectToAction("AvailableRoom");
        }


        // Employee Start -------------------------------------------------------------------
        public ActionResult Employees()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.empRepo.GetAll());
        }

        [HttpGet]
        public ActionResult CreateEmployee()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateEmployee(Employee emp)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.empRepo.Insert(emp);
                return RedirectToAction("Employees");
            }
            else
            {
                return View(emp);
            }
        }

        [HttpGet]
        public ActionResult EditEmployee(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.empRepo.Get(id));
        }

        [HttpPost]
        public ActionResult EditEmployee(Employee emp)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.empRepo.Update(emp);
                return RedirectToAction("Employees");
            }
            else
            {
                return View(emp);
            }
        }

        [HttpGet]
        public ActionResult DetailsEmployee(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.empRepo.Get(id));
        }

        [HttpGet]
        public ActionResult DeleteEmployee(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.empRepo.Get(id));
        }

        [HttpPost, ActionName("DeleteEmployee")]
        public ActionResult DeleteConfirmedEmployee(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            this.empRepo.Delete(id);
            return RedirectToAction("Employees");
        }
        // Employee End -------------------------------------------------------------------

        





        //*********************************************************************************//*/







        // Room Start -------------------------------------------------------------------*/
        public ActionResult Rooms()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.roomRepo.GetAll());
        }

        [HttpGet]
        public ActionResult CreateRoom()
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateRoom(Room room)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.roomRepo.Insert(room);
                return RedirectToAction("Rooms");
            }
            else
            {
                return View(room);
            }
        }

        [HttpGet]
        public ActionResult EditRoom(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.roomRepo.Get(id));
        }

        [HttpPost]
        public ActionResult EditRoom(Room room)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            if (ModelState.IsValid)
            {
                this.roomRepo.Update(room);
                return RedirectToAction("Rooms");
            }
            else
            {
                return View(room);
            }
        }

        [HttpGet]
        public ActionResult DetailsRoom(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.roomRepo.Get(id));
        }

        [HttpGet]
        public ActionResult DeleteRoom(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            return View(this.roomRepo.Get(id));
        }

        [HttpPost, ActionName("DeleteRoom")]
        public ActionResult DeleteConfirmedRoom(int id)
        {
            if (Session["logedIn"] == null)
            {
                return Redirect("/unauthorized");
            }
            this.roomRepo.Delete(id);
            return RedirectToAction("Rooms");
        }
       
        // Room End -------------------------------------------------------------------

        public ActionResult UpdateCustomerBalance(int paid, int cid)
        {
            float remaining = 0;

            UserBalance ub = this.UserBalanceRepo.GetByCustomer(cid);
            remaining = ub.UserBalanceAmount - paid;


            return Json(new { result = remaining }, JsonRequestBehavior.AllowGet);
        }

       
    }
}