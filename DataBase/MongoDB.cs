using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using server_mental_maps.models;

namespace server_mental_maps.DataBase;

public class MongoDb : DbContext
{
    public MongoDb(DbContextOptions options) : base(options)
    {
    }

    DbSet<User> Users { get; set; }
    DbSet<MentalMaps> MentalMaps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MentalMaps>().ToCollection("MentalMaps");
        modelBuilder.Entity<User>().ToCollection("Users");
        
        modelBuilder.Entity<MentalMaps>((op) =>
        {
            op.Property(m => m.Id).HasColumnName("Id");
            op.Property(m => m.MentalMap).HasColumnName("MentalMap");
            op.Property(m => m.UserId).HasColumnName("UserId");
        });

        modelBuilder.Entity<User>((op) =>
        {
            op.Property(u => u.Id).HasColumnName("Id");
            op.Property(u => u.Email).HasColumnName("Email");
            op.Property(u => u.lastName).HasColumnName("LastName");
            op.Property(u => u.name).HasColumnName("Name");
            op.Property(u => u.Password).HasColumnName("Password");
            op.Property(u => u.Username).HasColumnName("Username");
        });
    }
}
