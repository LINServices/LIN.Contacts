namespace LIN.Contacts.Data;

public partial class Contacts
{

    /// <summary>
    /// Crear nuevo contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public static async Task<CreateResponse> Create(ContactModel data)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Create(data, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Obtener un contacto.
    /// </summary>
    /// <param name="id">Id del contacto.</param>
    public static async Task<ReadOneResponse<ContactModel>> Read(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Read(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Validar acceso Iam.
    /// </summary>
    /// <param name="contact">Id del contacto.</param>
    /// <param name="profile">Id del perfil.</param>
    public static async Task<ReadOneResponse<bool>> Iam(int contact, int profile)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Iam(contact, profile, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Obtener los contactos asociados a un perfil.
    /// </summary>
    /// <param name="id"></param>
    public static async Task<ReadAllResponse<ContactModel>> ReadAll(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await ReadAll(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Eliminar un contacto.
    /// </summary>
    /// <param name="id">Id del contacto.</param>
    public static async Task<ResponseBase> Delete(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Delete(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    /// <summary>
    /// Actualizar un contacto.
    /// </summary>
    /// <param name="contactModel">Modelo</param>
    public static async Task<ResponseBase> Update(ContactModel contactModel)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Update(contactModel, context);

        context.CloseActions(connectionKey);

        return response;

    }
   
}