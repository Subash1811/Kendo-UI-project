using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ShiplogixQuotation.ViewModels;
using ShiplogixQuotation.Models;

public class IndexController : Controller
{
    private readonly ApplicationDbContext db = new ApplicationDbContext();

    // GET: Quotation
    public ActionResult Index()
    {
        // Customers
        var customers = db.Customers
            .AsEnumerable()
            .Select(c => new ShiplogixQuotation.ViewModels.Customer
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                typ = c.typ
            })
            .ToList();

        // Sales Executives
        var salesExecutives = db.SalesExecutive
            .AsEnumerable()
            .Select(s => new ShiplogixQuotation.ViewModels.SalesExecutive
            {
                SalesExecutiveId = s.SalesExecutiveId,
                Name = s.Name
            })
            .ToList();

        // Shipment Modes
        var shipmentModes = db.ShipmentMode
            .AsEnumerable()
            .Select(m => new ShiplogixQuotation.ViewModels.ShipmentMode
            {
                ModeCode = m.ModeCode,
                ModeName = m.ModeName
            })
            .ToList();

        // Sales References
        var salesReferences = db.SalesReference
            .AsEnumerable()
            .Select(sr => new ShiplogixQuotation.ViewModels.SalesReference
            {
                ReferenceNumber = sr.ReferenceNumber,
                ContactName = sr.ContactName,
                CompanyName = sr.CompanyName
            })
            .ToList();

        // Pass all data to ViewBag
        ViewBag.Customer = customers;
        ViewBag.SalesExecutive = salesExecutives;
        ViewBag.ShipmentMode = shipmentModes;
        ViewBag.SalesReference = salesReferences;

        var model = new QuotationViewModel();
        return View(model);
    }




    // AJAX: Get filtered quotations for the grid
    public ActionResult GetQuotations([DataSourceRequest] DataSourceRequest request,
                                  DateTime? fromDate, DateTime? toDate,
                                  string quotationNumber, int? customerId, string customer,
                                  string origin, string destination,
                                  int? salesExecutiveId, int? modeCode, int? salesReferenceId)
    {
       
        var query = db.quotations.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(q => q.QuotationDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(q => q.QuotationDate <= toDate.Value);

        if (salesReferenceId.HasValue)
            query = query.Where(q => q.SalesReferenceId == salesReferenceId.Value);

        if (customerId.HasValue)
            query = query.Where(q => q.CustomerId == customerId.Value);

        if (!string.IsNullOrEmpty(origin))
            query = query.Where(q => q.Origin == origin);

        if (!string.IsNullOrEmpty(destination))
            query = query.Where(q => q.Destination == destination);



        if (!string.IsNullOrEmpty(customer))
            query = query.Where(q => q.Customer.Contains(customer));  

        if (salesExecutiveId.HasValue)
            query = query.Where(q => q.SalesExecutiveId == salesExecutiveId.Value);

        if (modeCode.HasValue)
            query = query.Where(q => q.ModeCode == modeCode.Value);

        query = query.OrderByDescending(q => q.QuotationId);

       
        var result = query.Select(q => new
        {

            QuotationId = q.QuotationId,
            QuotationDate = q.QuotationDate,
            ValidTill = q.QuotationValidity,
            CustomerName = q.Customer,
            Origin = q.Origin,
            Destination = q.Destination,
            SalesOpsReference = q.SalesOpsReference,
            SalesExecutiveName = q.SalesExecutive,
            ShipmentModeName = q.ShipmentMode


        });

        return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            db.Dispose();
        base.Dispose(disposing);
    }
}
