namespace LIN.Contacts.Data;


/// <summary>
/// Nuevo contexto a la base de datos
/// </summary>
public class Context(DbContextOptions<Context> options) : DbContext(options)
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

    }


}