namespace LIN.Contacts.Data;


public partial class Profiles
{ 


    /// <summary>
    /// Crea un perfil.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public static async Task<ReadOneResponse<ProfileModel>> Create(AuthModel<ProfileModel> data)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Create(data, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene un perfil.
    /// </summary>
    /// <param name="id">Id del perfil</param>
    public static async Task<ReadOneResponse<ProfileModel>> Read(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Read(id, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene un perfil por medio del Id de su cuenta.
    /// </summary>
    /// <param name="id">Id de la cuenta</param>
    public static async Task<ReadOneResponse<ProfileModel>> ReadByAccount(int id)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await ReadByAccount(id, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene perfiles según los Id de las cuentas.
    /// </summary>
    /// <param name="ids">Lista de Ids.</param>
    public static async Task<ReadAllResponse<ProfileModel>> ReadByAccounts(IEnumerable<int> ids)
    {

        // Contexto
        (var context, var connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await ReadByAccounts(ids, context);

        context.CloseActions(connectionKey);

        return response;

    }


}