using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ShiplogixQuotation.Models;
using ShiplogixQuotation.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Web;
using System.Collections.Generic;

namespace ShiplogixQuotation.Controllers
{
    public class QuotationController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly string uploadFolder = "~/Uploads/";

        
        public ActionResult Create()
        {
            BindDropdowns();
            var vm = new QuotationViewModel
            {
                QuotationDate = DateTime.Today,
                QuotationValidity = DateTime.Today
            };
            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuotationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Collect all model validation errors into a single string (HTML formatted)
                var errors = string.Join("<br/>", ModelState.Values
                                         .SelectMany(v => v.Errors)
                                         .Select(e => e.ErrorMessage));

                // If AJAX request, return JSON errors
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = errors });
                }

                // Otherwise, return view with dropdowns bound
                BindDropdowns();
                return View(model);
            }

            
            var entity = new QuotationEntity
            {
                OrderTypeId = model.OrderTypeId,
                OrderType = db.OrderType
                              .Where(o => o.OrderTypeId == model.OrderTypeId)
                              .Select(o => o.DocTypeDescription)
                              .FirstOrDefault(),

                SalesExecutiveId = model.SalesExecutiveId,
                SalesExecutive = db.SalesExecutive
                                   .Where(s => s.SalesExecutiveId == model.SalesExecutiveId)
                                   .Select(s => s.Name)
                                   .FirstOrDefault(),

                CustomerId = model.CustomerId,
                Customer = db.Customers
                             .Where(c => c.CustomerId == model.CustomerId)
                             .Select(c => c.CustomerName)
                             .FirstOrDefault(),

                ModeCode = model.ModeCode,
                ShipmentMode = db.ShipmentMode
                                 .Where(m => m.ModeCode == model.ModeCode)
                                 .Select(m => m.ModeName)
                                 .FirstOrDefault(),

                CurrencyId = model.CurrencyId,
                BaseCurrency = db.currencies
                                 .Where(c => c.CurrencyId == model.CurrencyId)
                                 .Select(c => c.CurrencyName)
                                 .FirstOrDefault(),

                SalesReferenceId = model.SalesReferenceId,
                SalesOpsReference = db.SalesReference
                                      .Where(r => r.ReferenceId == model.SalesReferenceId)
                                      .Select(r => r.ReferenceNumber)
                                      .FirstOrDefault(),

                QuotationDate = model.QuotationDate,
                QuotationValidity = model.QuotationValidity,
                RequestNumber = model.RequestNumber,
                CopyQuotation = model.CopyQuotation,
                Origin = model.Origin,
                Destination = model.Destination,
                Remarks = model.Remarks,
              
                ValidTill = model.ValidTill,

                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            db.quotations.Add(entity);
            db.SaveChanges();

          
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = "Quotation saved successfully!",
                    redirectUrl = Url.Action("Edit", new { id = entity.QuotationId })
                });
            }

           
            TempData["Success"] = "Quotation saved successfully!";
            return RedirectToAction("Edit", new { id = entity.QuotationId });
        }
      
        public ActionResult Edit(int id)
        {
            var e = db.quotations.FirstOrDefault(q => q.QuotationId == id);
            if (e == null) return HttpNotFound();

            var vm = new QuotationViewModel
            {
                QuotationId = e.QuotationId,
                OrderTypeId = e.OrderTypeId,
                SalesExecutiveId = e.SalesExecutiveId,
                CustomerId = e.CustomerId,
                ModeCode = e.ModeCode,
                CurrencyId = e.CurrencyId,
                SalesReferenceId = e.SalesReferenceId,
                QuotationDate = e.QuotationDate,
                QuotationValidity = e.QuotationValidity,
                RequestNumber = e.RequestNumber,
                CopyQuotation = e.CopyQuotation,
                Origin = e.Origin,
                Destination = e.Destination,
                Remarks = e.Remarks,
             
                ValidTill = e.ValidTill
            };
            vm.Commodities = db.CommodityDetail
                     .Where(c => c.QuotationId == id)
                     .OrderBy(c => c.CommodityDetailId)
                     .ToList();

            vm.Routes = db.Routes
                         .Where(r => r.QuotationId == id)
                         .OrderBy(r => r.RouteId)
                         .ToList();

            vm.Attachments = db.Attachment
                               .Where(a => a.QuotationId == id)
                               .OrderBy(a => a.Id)
                               .ToList();

            BindDropdowns();           
            return View("Create", vm);
        }

        private void BindDropdowns()
        {
           
            ViewBag.OrderType = db.OrderType
                .Where(o => o.IsActive)
                .OrderBy(o => o.DocTypeDescription)
                .ToList()
                .Select(o => new ShiplogixQuotation.ViewModels.OrderType
                {
                    OrderTypeId = o.OrderTypeId,
                    DocTypeDescription = o.DocTypeDescription
                })
                .ToList();

            ViewBag.SalesExecutive = db.SalesExecutive
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToList()
                .Select(s => new ShiplogixQuotation.ViewModels.SalesExecutive
                {
                    SalesExecutiveId = s.SalesExecutiveId,
                    Name = s.Name
                })
                .ToList();

            ViewBag.Customer = db.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToList()
                .Select(c => new ShiplogixQuotation.ViewModels.Customer
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    typ = c.typ,
                    CreatedDate = c.CreatedDate
                })
                .ToList();

            ViewBag.ShipmentModes = db.ShipmentMode
                .Where(m => m.IsActive)
                .OrderBy(m => m.ModeName)
                .ToList()
                .Select(m => new ShiplogixQuotation.ViewModels.ShipmentMode
                {
                    ModeCode = m.ModeCode,
                    ModeName = m.ModeName
                })
                .ToList();

            ViewBag.currencies = db.currencies
                .Where(c => c.IsActive)
                .OrderBy(c => c.CurrencyName)
                .ToList()
                .Select(c => new ShiplogixQuotation.ViewModels.Currency
                {
                    CurrencyId = c.CurrencyId,
                    CurrencyCode = c.CurrencyCode,
                    CurrencyName = c.CurrencyName
                })
                .ToList();

            ViewBag.SalesReferences = db.SalesReference
                .Where(r => r.IsActive)
                .OrderBy(r => r.ReferenceNumber)
                .ToList()
                .Select(r => new ShiplogixQuotation.ViewModels.SalesReference
                {
                    ReferenceId = r.ReferenceId,
                    RecordDate = r.RecordDate,
                    ReferenceNumber = r.ReferenceNumber,
                    ContactName = r.ContactName,
                    CompanyName = r.CompanyName
                })
                .ToList();

            ViewBag.Commodity = db.Commodity
                .Where(c => c.IsActive)
                .OrderBy(c => c.CommodityName)
                .ToList()
                .Select(c => new ShiplogixQuotation.ViewModels.Commodity
                {
                    CommodityId = c.CommodityId,
                    CommodityName = c.CommodityName
                })
                .ToList();
        }

        
        public JsonResult GetCommodities([DataSourceRequest]DataSourceRequest request, int quotationId)
        {
            ViewBag.Commodity = db.Commodity.Select(c => new
            {
                CommodityId = c.CommodityId,
                CommodityName = c.CommodityName
            }).ToList();

            var data = db.CommodityDetail
                 .Where(cd => cd.QuotationId == quotationId)
                 .OrderByDescending(cd => cd.CommodityDetailId)
                 .ToList()
                 .Select(cd => new
                 {
                     cd.CommodityDetailId,
                     cd.CommodityId,
                     cd.QuotationId,
                     cd.UnitType,
                     cd.TypeQty,
                     GrossWeight = cd.GrossWeight ?? 0m,
                     NetWeight = cd.NetWeight ?? 0m,
                     Volume = cd.Volume ?? 0m,
                     VolumetricWeight = cd.VolumetricWeight ?? 0m,
                     cd.Hazardous,
                     cd.Reefer,
                     cd.OverSize,
                     cd.Ventilation
                 }).ToList();
            var result = data.ToDataSourceResult(request); 
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCommodity([DataSourceRequest] DataSourceRequest request, CommodityDetails dto)
        {
            if (ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
                // Find entity by both QuotationId and CommodityDetailId
                var entity = db.CommodityDetail.FirstOrDefault(cd =>
                    cd.CommodityDetailId == dto.CommodityDetailId &&
                    cd.QuotationId == dto.QuotationId);

                if (entity != null)
                {
                    // Update values
                    entity.QuotationId = dto.QuotationId;
                    entity.CommodityId = dto.CommodityId;
                    entity.UnitType = dto.UnitType;
                    entity.TypeQty = dto.TypeQty;
                    entity.GrossWeight = dto.GrossWeight;
                    entity.NetWeight = dto.NetWeight;
                    entity.Volume = dto.Volume;
                    entity.VolumetricWeight = dto.VolumetricWeight;
                    entity.Hazardous = dto.Hazardous;
                    entity.Reefer = dto.Reefer;
                    entity.OverSize = dto.OverSize;
                    entity.Ventilation = dto.Ventilation;
                    entity.CreatedDate = DateTime.Now;
                    entity.UpdatedDate = DateTime.Now;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Optionally log ex
                        ModelState.AddModelError("", "Unable to save changes. Try again.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Commodity not found for the given Quotation.");
                }
            }

            return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommodity([DataSourceRequest] DataSourceRequest request, CommodityDetails model)
        {
            if (model != null)
            {
                var existing = db.CommodityDetail.FirstOrDefault(cd => cd.CommodityDetailId == model.CommodityDetailId);
                if (existing != null)
                {
                    db.CommodityDetail.Remove(existing);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveCommodityDetail(CommodityDetails dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                    .Select(x => new
                    {
                        Field = x.Key,
                        Messages = x.Value.Errors
                            .Select(e => e.ErrorMessage ?? e.Exception?.Message)
                    });
                return Json(new { success = false, message = "Invalid data", errors });
            }

            try
            {
                if (dto.QuotationId <= 0)
                    return Json(new { success = false, message = "QuotationId missing. Save header first." });

               
                if (!dto.GrossWeight.HasValue || !dto.Volume.HasValue)
                    return Json(new { success = false, message = "Gross Weight and Volume are required." });

                dto.CreatedDate = DateTime.Now;
                dto.UpdatedDate = DateTime.Now;

                db.CommodityDetail.Add(dto);
                db.SaveChanges();

                return Json(new { success = true, id = dto.CommodityDetailId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }
        public JsonResult GetPorts(string text = "", int take = 50)
        {
            // optional quick search by text on code or name
            var q = db.Ports.AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                q = q.Where(p => p.Code.Contains(text) || p.Name.Contains(text));
            }

            var list = q.OrderBy(p => p.Name)
                        .Select(p => new PortViewModel
                        {
                            PortId = p.PortId,
                            Code = p.Code,
                            Name = p.Name,
                            Type = p.Type,
                            Country = p.Country
                        })
                        .Take(take)
                        .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoutes([DataSourceRequest] DataSourceRequest request, int? quotationId)
        {
            var list = Enumerable.Empty<RouteViewModel>();

            if (quotationId.HasValue && quotationId.Value > 0)
            {
                list = db.Routes
                    .Where(r => r.QuotationId == quotationId.Value)
                    .OrderByDescending(r => r.RouteId)
                    .Select(r => new RouteViewModel
                    {
                        RouteId = r.RouteId,
                        QuotationId = r.QuotationId,
                        Mode = r.Mode,
                        ProviderId = r.ProviderId,
                        OriginPortId = r.OriginPortId,
                        OriginPortName = r.OriginPortId != null ? r.OriginPort.Code + " - " + r.OriginPort.Name : null,
                        DestinationPortId = r.DestinationPortId,
                        DestinationPortName = r.DestinationPortId != null ? r.DestinationPort.Code + " - " + r.DestinationPort.Name : null,
                        TSP1PortId = r.TSP1PortId,
                        TSP2PortId = r.TSP2PortId,
                        TSP3PortId = r.TSP3PortId,
                        Duration = r.Duration,
                        BuyingRate = r.BuyingRate,
                        SellingRate = r.SellingRate,
                        Type = r.Type,
                        Terms = r.Terms,

                    })
                    .ToList();
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
     
        public JsonResult CreateRoute([DataSourceRequest] DataSourceRequest request, RouteViewModel model)
        
        {
            if (model == null || model.QuotationId <= 0)
            {
                ModelState.AddModelError("", "Invalid route data.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            try
            {
                var entity = new Route
                {
                    QuotationId = model.QuotationId,
                    Mode = model.Mode,
                    ProviderId = model.ProviderId,
                    OriginPortId = model.OriginPortId,
                    DestinationPortId = model.DestinationPortId,
                    TSP1PortId = model.TSP1PortId,
                    TSP2PortId = model.TSP2PortId,
                    TSP3PortId = model.TSP3PortId,
                    Duration = model.Duration,
                    BuyingRate = model.BuyingRate,
                    SellingRate = model.SellingRate,
                    Type = model.Type,
                    Terms = model.Terms,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                db.Routes.Add(entity);
                db.SaveChanges();
                model.RouteId = entity.RouteId;
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRoute([DataSourceRequest] DataSourceRequest request, Route dto)
        {
            if (ModelState.IsValid)
            {
                // load the existing entity from DB
                var entity = db.Routes.Include(r => r.OriginPort).Include(r => r.DestinationPort)
                                      .SingleOrDefault(r => r.RouteId == dto.RouteId);

                if (entity == null)
                {
                    ModelState.AddModelError("", "Route not found.");
                    return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
                }

                // Update only the editable properties (do NOT clobber navigation/display fields)
                entity.Mode = dto.Mode;
                entity.ProviderId = dto.ProviderId;
                entity.OriginPortId = dto.OriginPortId;
                entity.DestinationPortId = dto.DestinationPortId;
                entity.TSP1PortId = dto.TSP1PortId;
                entity.TSP2PortId = dto.TSP2PortId;
                entity.TSP3PortId = dto.TSP3PortId;
                entity.Duration = dto.Duration;
                entity.BuyingRate = dto.BuyingRate;
                entity.SellingRate = dto.SellingRate;
                entity.Type = dto.Type;
                entity.Terms = dto.Terms;
                entity.IsActive = dto.IsActive;
                entity.UpdatedDate = DateTime.UtcNow;

                db.SaveChanges();

            
                var resultDto = new
                {
                    entity.RouteId,
                    entity.QuotationId,
                    entity.Mode,
                    entity.ProviderId,
                    entity.OriginPortId,
                    entity.DestinationPortId,
                    entity.TSP1PortId,
                    entity.TSP2PortId,
                    entity.TSP3PortId,
                    entity.Duration,
                    entity.BuyingRate,
                    entity.SellingRate,
                    entity.Type,
                    entity.Terms,
                    entity.IsActive,
                    entity.CreatedDate,
                    entity.UpdatedDate,
                    OriginPortName = entity.OriginPort != null ? entity.OriginPort.Name : null,
                    DestinationPortName = entity.DestinationPort != null ? entity.DestinationPort.Name : null
                };

                return Json(new[] { resultDto }.ToDataSourceResult(request, ModelState));
            }
            return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoute([DataSourceRequest] DataSourceRequest request, Route dto)
        {
            if (dto != null)
            {
                var entity = db.Routes.Find(dto.RouteId);
                if (entity != null)
                {
                    db.Routes.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
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
