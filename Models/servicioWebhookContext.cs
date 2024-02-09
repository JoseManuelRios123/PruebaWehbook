using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Prueba2Webhook.Models;

namespace Prueba2Webhook.Models
{
    public partial class servicioWebhookContext : DbContext
    {
        public servicioWebhookContext()
        {
        }

        public servicioWebhookContext(DbContextOptions<servicioWebhookContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Amount> Amounts { get; set; } = null!;
        public virtual DbSet<Payer> Payers { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Request> Requests { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<WebhookModel> WebhookModels { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local); DataBase=servicioWebhook;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Amount>(entity =>
            {
                entity.ToTable("Amount");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Amounts)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Amount_Payment");

            });

            modelBuilder.Entity<Payer>(entity =>
            {
                entity.ToTable("Payer");

                entity.Property(e => e.Document).HasMaxLength(255);

                entity.Property(e => e.DocumentType).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Mobile).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Surname).HasMaxLength(255);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.IssuerName).HasMaxLength(255);

                entity.Property(e => e.PaymentMethodName).HasMaxLength(255);

                entity.HasMany(d => d.Requests)
                    .WithMany(p => p.Payments)
                    .UsingEntity<Dictionary<string, object>>(
                        "PaymentRequest",
                        l => l.HasOne<Request>().WithMany().HasForeignKey("RequestId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Payment_R__Reque__571DF1D5"),
                        r => r.HasOne<Payment>().WithMany().HasForeignKey("PaymentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Payment_R__Payme__5629CD9C"),
                        j =>
                        {
                            j.HasKey("PaymentId", "RequestId").HasName("PK__Payment___286FEF2F3E4D1AD8");

                            j.ToTable("Payment_Request");
                        });
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("Request");

                entity.HasOne(d => d.Payer)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.PayerId)
                    .HasConstraintName("FK__Request__PayerId__534D60F1");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__Request__StatusI__52593CB8");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.Property(e => e.StatusValue).HasMaxLength(255);
            });

            modelBuilder.Entity<WebhookModel>(entity =>
            {
                entity.ToTable("WebhookModel");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.WebhookModels)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__WebhookMo__Reque__59FA5E80");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
