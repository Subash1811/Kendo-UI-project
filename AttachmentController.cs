
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ShiplogixQuotation.Models;
using ShiplogixQuotation.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;




    public class AttachmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly string uploadFolder = "~/Uploads/";

    // === ATTACHMENTS ===
    public ActionResult GetAttachments([DataSourceRequest] DataSourceRequest request)
        {
            var data = db.Attachment
                         .Select(a => new
                         {
                             a.Id,
                             a.FileType,
                             a.FileName
                         })
                         .AsEnumerable()  
                         .Select(a => new Attachment
                         {
                             Id = a.Id,
                             FileType = a.FileType,
                             FileName = a.FileName
                         });

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // Create new attachment
        [HttpPost]
        public ActionResult CreateAttachment([DataSourceRequest] DataSourceRequest request, Attachment model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    FileType = model.FileType,
                    FileName = model.FileName,
                    QuotationId = model.QuotationId
                };
                db.Attachment.Add(entity);
                db.SaveChanges();

                model.Id = entity.Id;
                model.QuotationId = entity.QuotationId;
        }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult UpdateAttachment([DataSourceRequest] DataSourceRequest request, Attachment model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.Attachment.Find(model.Id);
                if (entity != null)
                {
                    entity.FileType = model.FileType;
                    entity.FileName = model.FileName;  
                    db.SaveChanges();
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

    
        [HttpPost]
        public ActionResult DeleteAttachment([DataSourceRequest] DataSourceRequest request, Attachment model)
        {
            var entity = db.Attachment.Find(model.Id);
            if (entity != null)
            {
                db.Attachment.Remove(entity);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        
        [HttpPost]
        public ActionResult SaveFile()
        {
            foreach (string fileName in Request.Files)
            {
                var file = Request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {
                    var uploadDir = Server.MapPath("~/Uploads");

                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var savedFileName = Path.GetFileName(file.FileName);
                    var savePath = Path.Combine(uploadDir, savedFileName);
                    file.SaveAs(savePath);

                    // Return virtual path (e.g., /Uploads/abc.txt)
                    var virtualPath = Url.Content("~/Uploads/" + savedFileName);

                    return Json(new { FileName = virtualPath }); // This will be used in JS
                }
            }

            return Json(new { FileName = "" });
        }

            public ActionResult Download(string fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return new HttpStatusCodeResult(400, "Invalid file name");

                // Safely build the path (prevent directory traversal)
                var uploadsDir = Server.MapPath("~/Uploads/");
                var fullPath = Path.Combine(uploadsDir, Path.GetFileName(fileName));

                if (!System.IO.File.Exists(fullPath))
                    return HttpNotFound("File not found");

                return File(fullPath, "application/octet-stream", fileName);
            }

    protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }

