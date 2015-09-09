using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BandDemo.Web.Models;
using Microsoft.Azure.NotificationHubs;

namespace BandDemo.Web.Controllers
{
    public class HeartRatesController : ApiController
    {
        private BandContext db = new BandContext();

        // GET: api/HeartRates
        public IQueryable<HeartRateData> GetHeartRateDatas()
        {
            return db.HeartRateDatas;
        }

        // GET: api/FilteredHeartRates
        [Route("api/FilteredHeartRates")]
        public List<HeartRateData> GetFilteredHeartRateDatas()
        {
            var heartRates = db.HeartRateDatas.ToList();

            var heartRatesByMinute = new List<HeartRateData>();

            var minutes = heartRates.Select(data => data.CreatedAt.Minute).Distinct();

            foreach (var minute in minutes)
            {
                heartRatesByMinute.Add(heartRates.First(data => data.CreatedAt.Minute == minute));
            }

            return heartRatesByMinute;
        }

        // GET: api/HeartRates/5
        [ResponseType(typeof(HeartRateData))]
        public IHttpActionResult GetHeartRateData(int id)
        {
            HeartRateData heartRateData = db.HeartRateDatas.Find(id);
            if (heartRateData == null)
            {
                return NotFound();
            }

            return Ok(heartRateData);
        }

        // PUT: api/HeartRates/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHeartRateData(int id, HeartRateData heartRateData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != heartRateData.Id)
            {
                return BadRequest();
            }

            db.Entry(heartRateData).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeartRateDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/HeartRates
        [ResponseType(typeof(HeartRateData))]
        public async Task<IHttpActionResult> PostHeartRateData(HeartRateData heartRateData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await SendNotificationAsync(heartRateData);

            //var hub = NotificationHubClient.CreateClientFromConnectionString(
            //    "Endpoint=sb://banddemonotificationhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=8im4ijcsb95TOZ7uwSSLi/j9HIMEoTh6MdpVFsqyQNw=",
            //    "banddemonotificationhub");

            //var toast = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            //            "<wp:Notification xmlns:wp=\"WPNotification\">" +
            //                "<wp:Toast>" +
            //                    "<wp:Text1>" + heartRateData.Value + "</wp:Text1>" +
            //                    "<wp:Text2>" + heartRateData.CreatedAt + "</wp:Text2>" +
            //                "</wp:Toast> " +
            //            "</wp:Notification>";

            //// Create the Tile message.
            //string tile = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            //"<wp:Notification xmlns:wp=\"WPNotification\">" +
            //    "<wp:Tile>" +
            //      "<wp:Title>Seyes khouk</wp:Title>" +
            //      "<wp:BackTitle>" + heartRateData.Value + "</wp:BackTitle>" +
            //      "<wp:BackContent>" + heartRateData.CreatedAt + "</wp:BackContent>" +
            //   "</wp:Tile> " +
            //"</wp:Notification>";
            ////"<wp:Count>" + "3" + "</wp:Count>" +

            //await hub.SendMpnsNativeNotificationAsync(toast);
            //await hub.SendMpnsNativeNotificationAsync(tile);

            db.HeartRateDatas.Add(heartRateData);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = heartRateData.Id }, heartRateData);
        }

        private async Task SendNotificationAsync(HeartRateData heartRateData)
        {
            NotificationHubClient hub = NotificationHubClient
                .CreateClientFromConnectionString("Endpoint=sb://banddemonotificationhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=8im4ijcsb95TOZ7uwSSLi/j9HIMEoTh6MdpVFsqyQNw=", 
                "banddemonotificationhub");

            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + heartRateData.Value + @"</text><text id=""2"">" + heartRateData.CreatedAt.Hour + ":" + heartRateData.CreatedAt.Minute + "</text></binding></visual></toast>";

            var tile = @"<?xml version=""1.0"" encoding=""utf-8""?><tile><visual><binding template =""TileSquareBlock""><text id=""1"">" + heartRateData.Value + @"</text><text id=""2"">" + heartRateData.CreatedAt.Hour + ":" + heartRateData.CreatedAt.Minute + "</text></binding></visual></tile>";

            await hub.SendWindowsNativeNotificationAsync(toast);
            await hub.SendWindowsNativeNotificationAsync(tile);
        }

        // DELETE: api/HeartRates/5
        [ResponseType(typeof(HeartRateData))]
        public IHttpActionResult DeleteHeartRateData(int id)
        {
            HeartRateData heartRateData = db.HeartRateDatas.Find(id);
            if (heartRateData == null)
            {
                return NotFound();
            }

            db.HeartRateDatas.Remove(heartRateData);
            db.SaveChanges();

            return Ok(heartRateData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HeartRateDataExists(int id)
        {
            return db.HeartRateDatas.Count(e => e.Id == id) > 0;
        }
    }
}