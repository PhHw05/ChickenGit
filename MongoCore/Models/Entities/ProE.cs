using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoCore.Models.ModelViews;

namespace MongoCore.Models.Entities
{
    public class ProE : DbContext
    {
        public ProE(DbContextOptions<ProE> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMongoDB("mongodb://localhost:27017", "Demo2");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProM>().ToCollection("Product");

            modelBuilder.Entity<ProM>(entity =>
            {
                entity.Property(e => e._id)
                    .HasElementName("_id")
                    .HasConversion(
                        v => v != ObjectId.Empty ? v : ObjectId.GenerateNewId(),
                        v => v
                    );

                entity.Property(e => e.namePro)
                    .HasElementName("name");


                entity.Property(e => e.CateId)
                    .HasElementName("categoryId");

                entity.Property(e => e.Image)
                    .HasElementName("image");

                entity.OwnsMany(e => e.Variants, variant =>
                {
                    variant.ToCollection("variants");
                    variant.Property(v => v.Sku).HasElementName("sku");
                    variant.Property(v => v.Color).HasElementName("color");
                    variant.Property(v => v.Storage).HasElementName("storage");
                    variant.Property(v => v.Price).HasElementName("price");
                    variant.Property(v => v.Stock).HasElementName("stock");
                    variant.Property(v => v.Images).HasElementName("images");
                });
            });
        }
    }

}