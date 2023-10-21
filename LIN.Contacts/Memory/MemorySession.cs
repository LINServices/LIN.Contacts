namespace LIN.Contacts.Memory;


public class MemorySession
{

    /// <summary>
    /// Perfil.
    /// </summary>
    public ProfileModel Profile { get; set; }


    /// <summary>
    /// Lista de nombres de los chats.
    /// </summary>
    public List<ContactModel> Contactos { get; set; }




    /// <summary>
    /// Nueva session en memoria.
    /// </summary>
    public MemorySession()
    {
        Profile = new ProfileModel();
        Contactos = new();
    }


    /// <summary>
    /// Obtiene un string con la concatenación de los nombres de las conversaciones.
    /// </summary>
    public string StringOfContacts()
    {
        string final = "";
        foreach (var contact in Contactos)
            final += $"<<<{contact.Nombre} su correo es {contact.Mails[0].Email}, el tipo de contacto es {contact.Type} y su teléfono {contact.Phones[0].Number}>>>,";
        
        return final;
    }

}