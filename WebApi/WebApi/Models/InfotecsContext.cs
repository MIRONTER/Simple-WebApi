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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-R6LPKL1\\SQLEXPRESS;Initial Catalog=infotecs; Integrated Security=False;User ID=sa;Password=1234; TrustServerCertificate=True ");

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
