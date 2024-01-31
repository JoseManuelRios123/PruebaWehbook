using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PruebaWebhook.Models
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

        public virtual DbSet<WebhookModel> Webhooks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local); DataBase=servicioWebhook;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebhookModel>(entity =>
            {
                entity.ToTable("Webhook");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Document).HasMaxLength(25);

                entity.Property(e => e.DocumentType).HasMaxLength(25);

                entity.Property(e => e.Email).HasMaxLength(25);

                entity.Property(e => e.IssuerName).HasMaxLength(25);

                entity.Property(e => e.Mobile).HasMaxLength(25);

                entity.Property(e => e.Name).HasMaxLength(25);

                entity.Property(e => e.PaymentMethodName).HasMaxLength(25);

                entity.Property(e => e.Status).HasMaxLength(25);

                entity.Property(e => e.Surname).HasMaxLength(25);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
