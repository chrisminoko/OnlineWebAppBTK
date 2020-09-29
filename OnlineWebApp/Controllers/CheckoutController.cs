using Microsoft.AspNet.Identity;
using OnlineWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayFast;
using PayFast.AspNet;
using System.Configuration;

namespace OnlineWebApp.Controllers
{   [Authorize]
    public class CheckoutController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult AddressAndPayment()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            var cart = Business_Logics.GetCart(this.HttpContext);
            TryUpdateModel(order);
            if (ModelState.IsValid)
            {
                var uid = User.Identity.GetUserId();

                var id = (from user in db.Clients
                                where user.Client_Id == uid
                                select user.Client_Id).SingleOrDefault();
                var username  = (from user in db.Clients
                                 where user.Client_Id == id
                                 select user.Display_Name).SingleOrDefault();
                order.OrderDate =System.DateTime.Now;
                order.Client_Id = uid;
                order.Username = User.Identity.Name;
                order.Total = cart.GetTotal();
                db.Orders.Add(order);
                db.SaveChanges();
             
                cart.CreateOrder(order);
                
                return RedirectToAction("Complete", "Checkout", new { id = order.Order_Id });
            }

            return View(order);
        }

        public ActionResult Complete(int id)
        {

            bool isValid = db.Orders.Any(o => o.Order_Id == id && o.Username == User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }



        private readonly PayFastSettings payFastSettings;


        #region Constructor

        public CheckoutController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }

        #endregion Constructor

        #region Methods


        public ActionResult Recurring()
        {

            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            recurringRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";

            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "drnendwandwe@gmail.com";

            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult OnceOff(double tot)
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            double amount = 20/* Convert.ToDouble(db.Items.Select(x => x.CostPrice).FirstOrDefault())*/;
            var products = "Gold" /*db.Items.Select(x => x.Name).ToList()*/;
            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = tot;
            onceOffRequest.item_name = "Once off option";
            onceOffRequest.item_description = "Some details about the once off payment";


            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";

            return Redirect(redirectUrl);
        }


        public ActionResult OnceOff(int id)
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            //Application application = db.Applications.Where(a => a.ApplicationID == id).FirstOrDefault();
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            double amount = 20/* Convert.ToDouble(db.Items.Select(x => x.CostPrice).FirstOrDefault())*/;
            var products = "Gold" /*db.Items.Select(x => x.Name).ToList()*/;
            // Transaction Details
            decimal? PackagePrice = (from p in db.OrderDetails
                                     where p.Order_Id == id
                                     select p.UnitPrice).FirstOrDefault();

            //var PackageName = (from p in db.Applications
            //                   where p.ApplicationID == id
            //                   select p.Package.PackageType.PackageName).FirstOrDefault();

            //var Description = (from p in db.Applications
            //                   where p.ApplicationID == id
            //                   select p.Package.PackageDescription).FirstOrDefault();

            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = Convert.ToDouble(PackagePrice);
            onceOffRequest.item_name = "";
            onceOffRequest.item_description = "";


            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            //application.PaymentStatus = "Approved";
            //application.Status = "Active";
            //application.StartDate = DateTime.Parse(DateTime.Now.ToString("yyy.MM.dd")).Date;
            //application.ExpiryDateDate = DateTime.Parse(DateTime.Now.ToString("yyy.MM.dd")).Date.AddMonths(1);
            //db.Entry(application).State = EntityState.Modified;
            //db.SaveChanges();
            return Redirect(redirectUrl);
        }
        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;
            #endregion Methods
            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";
            double amount = 70; /*Convert.ToDouble(db.FoodOrders.Select(x => x.Total).FirstOrDefault());*/
            var products = "Gold"/* db.FoodOrders.Select(x => x.UserEmail).ToList()*/;
            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

   
        public ActionResult Error()
        {
            return View();
        }
    }
}