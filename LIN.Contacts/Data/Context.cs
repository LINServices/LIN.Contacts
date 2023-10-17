namespace LIN.Contacts.Data;


public class Context : DbContext
{


    /// <summary>
    /// Tabla de perfiles
    /// </summary>
    public DbSet<ProfileModel> Profiles { get; set; }


    /// <summary>
    /// Tabla de proyectos
    /// </summary>
    public DbSet<ContactModel> Contacts { get; set; }
    public DbSet<MailModel> Mails { get; set; }
    public DbSet<PhoneModel> Phones { get; set; }


    /// <summary>
    /// Nuevo contexto a la base de datos
    /// </summary>
    public Context(DbContextOptions<Context> options) : base(options) { }



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