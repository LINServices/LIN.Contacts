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
    public IEnumerable<ContactModel> Contactos { get; set; }

    /// <summary>
    /// Nueva session en memoria.
    /// </summary>
    public MemorySession()
    {
        Profile = new();
        Contactos = [];
    }

    /// <summary>
    /// Obtiene un string con la concatenación de los nombres de las conversaciones.
    /// </summary>
    public string StringOfContacts()
    {
        var final = "IMPORTANTE contestar con la información del contacto o contactos del usuario. como el mail, el teléfono o numero, nombre etc.\r\nTIENES Acceso a los siguientes contactos:";

        foreach (var contact in Contactos)
            final += $$""" { {{contact.Nombre}} el correo es {{contact.Mails[0].Email}}, el tipo de contacto es {{contact.Type}} y el teléfono {{contact.Phones[0].Number}}},""";

        return final + "\nRecuerda que siempre contestar con la información que el usuario requiere de los contactos.";
    }

}