using System.Data.Entity;
using ShiplogixQuotation.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections;

namespace ShiplogixQuotation.Models
{
    public class ApplicationDbContext : DbContext
    {
        

        public ApplicationDbContext() : base("name=DefaultConnection")
        {
        }

       
        public DbSet<QuotationEntity> quotations { get; set; }

        public DbSet<Currency> currencies { get; set; }
        public DbSet<SalesReference> SalesReference { get; set; }

        // If these are actual tables, keep them; otherwise comment/remove
        public DbSet<OrderType> OrderType { get; set; }
        public DbSet<SalesExecutive> SalesExecutive { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ShipmentMode> ShipmentMode { get; set; }
        public IEnumerable<object> Customer { get; internal set; }
        public DbSet<CommodityDetails> CommodityDetail { get; set; }
        public DbSet<Commodity> Commodity { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit key definitions if missing
            modelBuilder.Entity<OrderType>().HasKey(o => o.OrderTypeId);
            modelBuilder.Entity<SalesExecutive>().HasKey(s => s.SalesExecutiveId);
            modelBuilder.Entity<Currency>().HasKey(c => c.CurrencyCode);
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<ShipmentMode>().HasKey(s => s.ModeCode);          
            modelBuilder.Entity<CommodityDetails>().ToTable("CommodityDetail", "dbo");
            modelBuilder.Entity<Route>().Property(r => r.BuyingRate).HasPrecision(18, 2);
            modelBuilder.Entity<Route>().Property(r => r.SellingRate).HasPrecision(18, 2);

        }
    }
}
