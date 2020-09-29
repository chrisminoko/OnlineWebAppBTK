using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineWebApp.Models;

namespace OnlineWebApp.Controllers
{
    public class ItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Brand).Include(i => i.Categories).Include(i => i.Sales).Include(i => i.Tag);
            return View(items.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Tag_Name");
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Type");
            ViewBag.Sale_Id = new SelectList(db.Sales, "Sale_Id", "Sale_Name");
            ViewBag.Tag_Id = new SelectList(db.Tags, "Tag_Id", "Tag_Name");
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Items items, HttpPostedFileBase filelist)
        {
            if (ModelState.IsValid)
            {
                items.Front_View = ConvertToBytes(filelist);
                items.Left_View = ConvertToBytes(filelist);
                items.Rght_View = ConvertToBytes(filelist);
                items.DateCreated = System.DateTime.Now;
                db.Items.Add(items);
                db.SaveChanges();
                var sale = (from s in db.Items
                            where s.Item_Id == items.Item_Id
                            select s.Sales.Sale_Name).Single();
                if(sale == "On Sale")
                {return RedirectToAction("Create", "Discounts", new { id =items.Item_Id}); }
                else
                return RedirectToAction("Index");
            }

            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Tag_Name", items.Brand_Id);
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Type", items.Category_Id);
            ViewBag.Sale_Id = new SelectList(db.Sales, "Sale_Id", "Sale_Name", items.Sale_Id);
            ViewBag.Tag_Id = new SelectList(db.Tags, "Tag_Id", "Tag_Name", items.Tag_Id);
            return View(items);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Tag_Name", items.Brand_Id);
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Type", items.Category_Id);
            ViewBag.Sale_Id = new SelectList(db.Sales, "Sale_Id", "Sale_Name", items.Sale_Id);
            ViewBag.Tag_Id = new SelectList(db.Tags, "Tag_Id", "Tag_Name", items.Tag_Id);
            return View(items);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Items items)
        {
            if (ModelState.IsValid)
            {
                db.Entry(items).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Brand_Id = new SelectList(db.Brands, "Brand_Id", "Tag_Name", items.Brand_Id);
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Category_Type", items.Category_Id);
            ViewBag.Sale_Id = new SelectList(db.Sales, "Sale_Id", "Sale_Name", items.Sale_Id);
            ViewBag.Tag_Id = new SelectList(db.Tags, "Tag_Id", "Tag_Name", items.Tag_Id);
            return View(items);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items items = db.Items.Find(id);
            if (items == null)
            {
                return HttpNotFound();
            }
            return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Items items = db.Items.Find(id);
            db.Items.Remove(items);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
