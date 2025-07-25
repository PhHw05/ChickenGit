using Microsoft.EntityFrameworkCore;
using MongoCore.Models.ModelViews;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;

namespace MongoCore.Models.Entities
{
    public class UserE : DbContext
    {
        public UserE(DbContextOptions<CateE> options) : base(options)
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
            var UserMgrEntity = modelBuilder.Entity<UserM>();
            UserMgrEntity.ToCollection("User");
            UserMgrEntity.Property(x => x._id).HasElementName("_id").HasConversion(typeof(ObjectId));
            UserMgrEntity.Property(x => x.idUser).HasElementName("idUser");
            UserMgrEntity.Property(x => x.nameUser).HasElementName("nameUser");
            UserMgrEntity.Property(x => x.Image).HasElementName("Image");
            UserMgrEntity.Property(x => x.Email).HasElementName("Email");
            UserMgrEntity.Property(x => x.Password).HasElementName("Password");
            UserMgrEntity.Property(x => x.Role).HasElementName("Role");
            UserMgrEntity.Property(x => x.Address).HasElementName("Address");
            UserMgrEntity.Property(x => x.Phone).HasElementName("Phone");

        }

        public DbSet<UserM> Users { get; set; }
    }
}
