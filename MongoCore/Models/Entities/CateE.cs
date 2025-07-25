using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.Bson;
using MongoCore.Models.ModelViews;

namespace MongoCore.Models.Entities
{
    public class CateE : DbContext
    {
        public CateE(DbContextOptions<CateE> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMongoDB(connectionString: "mongodb://localhost:27017", databaseName: "Demo2");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var cateMgrEntity = modelBuilder.Entity<CateM>();
            cateMgrEntity.ToCollection("Category");
            cateMgrEntity.Property(x => x._id).HasElementName("_id").HasConversion(typeof(ObjectId));
            cateMgrEntity.Property(x => x.idCate).HasElementName("idcate");
            cateMgrEntity.Property(x => x.nameCate).HasElementName("namecate");
        }

        public DbSet<CateM> Categories { get; set; }
    }
}