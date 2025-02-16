using LIN.Types.Contacts.Models;
using Microsoft.EntityFrameworkCore;

namespace LIN.Contacts.Persistence.Context;

/// <summary>
/// Nuevo contexto a la base de datos
/// </summary>
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    /// <summary>
    /// Tabla de perfiles.
    /// </summary>
    public DbSet<ProfileModel> Profiles { get; set; }


    /// <summary>
    /// Tabla de contactos.
    /// </summary>
    public DbSet<ContactModel> Contacts { get; set; }


    /// <summary>
    /// Tabla de Emails.
    /// </summary>
    public DbSet<MailModel> Mails { get; set; }


    /// <summary>
    /// Tabla de teléfonos.
    /// </summary>
    public DbSet<PhoneModel> Phones { get; set; }


    /// <summary>
    /// Naming DB
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Indices y identidad
        modelBuilder.Entity<ProfileModel>()
           .HasIndex(e => e.AccountId)
           .IsUnique();

        modelBuilder.Entity<MailModel>()
             .HasOne(m => m.Contact)
             .WithMany(c => c.Mails)
             .HasForeignKey(m => m.ContactId)
             .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PhoneModel>()
               .HasOne(m => m.Contact)
               .WithMany(c => c.Phones)
               .HasForeignKey(m => m.ContactId)
               .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProfileModel>().ToTable("Profile");
        modelBuilder.Entity<MailModel>().ToTable("Mail");
        modelBuilder.Entity<PhoneModel>().ToTable("Phone");
        modelBuilder.Entity<ContactModel>().ToTable("Contact");
    }

}