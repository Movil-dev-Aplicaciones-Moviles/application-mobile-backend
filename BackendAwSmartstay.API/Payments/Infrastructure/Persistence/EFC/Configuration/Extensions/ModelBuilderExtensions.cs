using BackendAwSmartstay.API.Payments.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Payments.Domain.Model.Entities;
using BackendAwSmartstay.API.Payments.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BackendAwSmartstay.API.Payments.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyPaymentsConfiguration(this ModelBuilder builder)
    {
        // ─────────────────────────────────────────────────────
        // Payment Entity
        // ─────────────────────────────────────────────────────
        builder.Entity<Payment>().ToTable("payments");
        builder.Entity<Payment>().HasKey(p => p.Id);
        builder.Entity<Payment>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Payment>().Property(p => p.TransactionId).IsRequired();

        // Money Value Object — stored as decimal in the existing "amount" column
        builder.Entity<Payment>().Property(p => p.AmountRecord)
            .HasConversion(v => v.Amount, v => new Money(v))
            .HasColumnType("decimal(18,2)")
            .HasColumnName("amount")
            .IsRequired();

        // CreditCard Value Object — owned type mapped to existing columns
        builder.Entity<Payment>().OwnsOne(p => p.Card, c =>
        {
            c.WithOwner().HasForeignKey("Id");
            c.Property(card => card.HolderName)
                .HasColumnName("card_holder_name")
                .IsRequired();
            c.Property(card => card.MaskedNumber)
                .HasColumnName("card_number_masked")
                .IsRequired();
        });

        // ─────────────────────────────────────────────────────
        // GuestFolio Aggregate Root
        // ─────────────────────────────────────────────────────
        builder.Entity<GuestFolio>().ToTable("guest_folios");
        builder.Entity<GuestFolio>().HasKey(f => f.Id);
        builder.Entity<GuestFolio>().Property(f => f.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<GuestFolio>().Property(f => f.BookingId).IsRequired();
        builder.Entity<GuestFolio>().Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // GuestFolio → Charge (1:N) — cascade delete
        builder.Entity<GuestFolio>().HasMany(f => f.Charges)
            .WithOne()
            .HasForeignKey("FolioId")
            .OnDelete(DeleteBehavior.Cascade);

        // Charge entity configuration
        builder.Entity<Charge>().ToTable("charges");
        builder.Entity<Charge>().HasKey(c => c.Id);
        builder.Entity<Charge>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Charge>().Property(c => c.Category)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();
        builder.Entity<Charge>().Property(c => c.Description)
            .HasMaxLength(500)
            .IsRequired();
        builder.Entity<Charge>().Property(c => c.AmountRecord)
            .HasConversion(v => v.Amount, v => new Money(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Entity<Charge>().Property(c => c.CreatedAt).IsRequired();

        // GuestFolio → FolioPaymentRecord (1:N) — owned collection
        builder.Entity<GuestFolio>().OwnsMany(f => f.PaymentRecords, p =>
        {
            p.WithOwner().HasForeignKey("FolioId");
            p.ToTable("folio_payment_records");
            p.Property(r => r.PaymentId)
                .HasColumnName("payment_id")
                .IsRequired();
            p.Property(r => r.Amount)
                .HasConversion(v => v.Amount, v => new Money(v))
                .HasColumnType("decimal(18,2)")
                .HasColumnName("amount")
                .IsRequired();
        });
    }
}