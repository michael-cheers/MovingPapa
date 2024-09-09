using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MovingPapa.DB;

public partial class MovingpapaContext : DbContext
{
    public MovingpapaContext()
    {
    }

    public MovingpapaContext(DbContextOptions<MovingpapaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Move> Moves { get; set; }

    public virtual DbSet<QuotesAndContact> QuotesAndContacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL(
            Environment.GetEnvironmentVariable("MYSQL_PASSWORD") is string p ?
            $"server=movingpapa;port=3306;uid=user;pwd={p};database=movingpapa" :
            "server=localhost;port=12784;uid=root;pwd=MB9f2tvBekn8hnpFAaRZ;database=movingpapa");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.QuoteId, "QuoteId");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.MoveDate).HasColumnType("datetime");
            entity.Property(e => e.MoveDetails).HasColumnType("json");
            entity.Property(e => e.MoveTime).HasColumnType("enum('Early Morning','Afternoon','Late Afternoon','Evening')");
            entity.Property(e => e.PhoneNumber).HasMaxLength(256);
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Uuid).HasMaxLength(256);

            entity.HasOne(d => d.Quote).WithMany(p => p.Moves)
                .HasForeignKey(d => d.QuoteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Moves_ibfk_1");
        });

        modelBuilder.Entity<QuotesAndContact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("QuotesAndContact");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Addresses).HasColumnType("json");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(256);
            entity.Property(e => e.MoveDate).HasColumnType("datetime");
            entity.Property(e => e.MoveInfo).HasColumnType("json");
            entity.Property(e => e.MoveTime).HasColumnType("enum('Early Morning','Afternoon','Late Afternoon','Evening')");
            entity.Property(e => e.PhoneNumber).HasMaxLength(256);
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TimeUpdated).HasColumnType("datetime");
            entity.Property(e => e.Uuid).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
