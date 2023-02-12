using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public partial class InfotecsContext : DbContext
{
    public InfotecsContext()
    {
    }

    public InfotecsContext(DbContextOptions<InfotecsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Value> Values { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("Infotecs");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.FileName);

            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fileName");
            entity.Property(e => e.AllTime).HasColumnName("allTime");
            entity.Property(e => e.AverageN).HasColumnName("averageN");
            entity.Property(e => e.AverageSeconds).HasColumnName("averageSeconds");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.MaxN).HasColumnName("maxN");
            entity.Property(e => e.MedianN).HasColumnName("medianN");
            entity.Property(e => e.MinDate)
                .HasColumnType("datetime")
                .HasColumnName("minDate");
            entity.Property(e => e.MinN).HasColumnName("minN");
        });

        modelBuilder.Entity<Value>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fileName");
            entity.Property(e => e.N).HasColumnName("n");
            entity.Property(e => e.Seconds).HasColumnName("seconds");

            entity.HasOne(d => d.FileNameNavigation).WithMany(p => p.Values)
                .HasForeignKey(d => d.FileName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Values_Results");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
