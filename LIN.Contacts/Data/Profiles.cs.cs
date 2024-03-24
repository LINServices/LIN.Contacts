namespace LIN.Contacts.Data;


public partial class Profiles
{


    /// <summary>
    /// Crea un perfil.
    /// </summary>
    /// <param name="data">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<ProfileModel>> Create(AuthModel<ProfileModel> data, Conexión context)
    {
        // Id
        data.Profile.Id = 0;

        // Ejecución
        try
        {
            var res = context.DataBase.Profiles.Add(data.Profile);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.Profile);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene un perfil
    /// </summary>
    /// <param name="id">Id del perfil</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<ProfileModel>> Read(int id, Conexión context)
    {


        // Ejecución
        try
        {

            var profile = await (from P in context.DataBase.Profiles
                                 where P.Id == id
                                 select P).FirstOrDefaultAsync();

            return new(Responses.Success, profile ?? new());
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene un perfil por medio del Id de su cuenta.
    /// </summary>
    /// <param name="id">Id de la cuenta</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<ProfileModel>> ReadByAccount(int id, Conexión context)
    {

        // Ejecución
        try
        {
            // Consulta.
            var profile = await (from P in context.DataBase.Profiles
                                 where P.AccountId == id
                                 select P).FirstOrDefaultAsync();

            // Si no existe.
            if (profile == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profile ?? new());
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene perfiles según los Id de las cuentas.
    /// </summary>
    /// <param name="ids">Lista de Ids.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadAllResponse<ProfileModel>> ReadByAccounts(IEnumerable<int> ids, Conexión context)
    {


        // Ejecución
        try
        {

            var profile = await (from P in context.DataBase.Profiles
                                 where ids.Contains(P.AccountId)
                                 select P).ToListAsync();

            if (profile == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profile ?? []);
        }
        catch
        {
        }
        return new();
    }


}