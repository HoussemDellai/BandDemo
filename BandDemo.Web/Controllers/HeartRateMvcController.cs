using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BandDemo.Web.Models;

namespace BandDemo.Web.Controllers
{
    public class HeartRateMvcController : Controller
    {
        private BandContext db = new BandContext();

        // GET: HeartRateMvc
        public ActionResult Index()
        {
            var heartRates = db.HeartRateDatas.ToList();

            return View(heartRates);
        }

        // GET: HeartRateMvc/Charts
        public ActionResult Charts()
        {
            var heartRates = db.HeartRateDatas.ToList();
            
            return View(heartRates);
        }

        // GET: HeartRateMvc/FilteredCharts
        public ActionResult FilteredCharts()
        {
            var heartRates = db.HeartRateDatas.ToList();

            var heartRatesByMinute = new List<HeartRateData>();

            var minutes = heartRates.Select(data => data.CreatedAt.Minute).Distinct();

            foreach (var minute in minutes)
            {
                heartRatesByMinute.Add(heartRates.First(data => data.CreatedAt.Minute == minute));
            }

            return View(heartRatesByMinute);
        }

        // GET: HeartRateMvc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HeartRateData heartRateData = db.HeartRateDatas.Find(id);
            if (heartRateData == null)
            {
                return HttpNotFound();
            }
            return View(heartRateData);
        }

        // GET: HeartRateMvc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HeartRateMvc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Value,CreatedAt")] HeartRateData heartRateData)
        {
            if (ModelState.IsValid)
            {
                db.HeartRateDatas.Add(heartRateData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(heartRateData);
        }

        // GET: HeartRateMvc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HeartRateData heartRateData = db.HeartRateDatas.Find(id);
            if (heartRateData == null)
            {
                return HttpNotFound();
            }
            return View(heartRateData);
        }

        // POST: HeartRateMvc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,CreatedAt")] HeartRateData heartRateData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(heartRateData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(heartRateData);
        }

        // GET: HeartRateMvc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HeartRateData heartRateData = db.HeartRateDatas.Find(id);
            if (heartRateData == null)
            {
                return HttpNotFound();
            }
            return View(heartRateData);
        }

        // POST: HeartRateMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HeartRateData heartRateData = db.HeartRateDatas.Find(id);
            db.HeartRateDatas.Remove(heartRateData);
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
