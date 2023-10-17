namespace LIN.Contacts.Data;


public class Contacts
{



    #region Abstracciones


    /// <summary>
    /// Crea un contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public async static Task<CreateResponse> Create(ContactModel data)
    {

        // Contexto
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Create(data, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene un contacto
    /// </summary>
    /// <param name="id">ID del contacto</param>
    public async static Task<ReadOneResponse<ContactModel>> Read(int id)
    {

        // Contexto
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await Read(id, context);

        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil
    /// </summary>
    /// <param name="id">ID del perfil</param>
    public async static Task<ReadAllResponse<ContactModel>> ReadAll(int id)
    {

        // Contexto
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        // respuesta
        var response = await ReadAll(id, context);

        context.CloseActions(connectionKey);

        return response;

    }


    #endregion




    /// <summary>
    /// Crea un contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    /// <param name="context">Contexto de conexión.</param>
    public async static Task<CreateResponse> Create(ContactModel data, Conexión context)
    {
        // Ejecución
        try
        {

            foreach (var e in data.Mails)
                e.Profile = data.Im;

            foreach (var e in data.Phones)
                e.Profile = data.Im;

            context.DataBase.Attach(data.Im);

            var res = context.DataBase.Contacts.Add(data);
            await context.DataBase.SaveChangesAsync();
            return new(Responses.Success, data.Id);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene un contacto
    /// </summary>
    /// <param name="id">ID del contacto</param>
    /// <param name="context">Contexto de conexión.</param>
    public async static Task<ReadOneResponse<ContactModel>> Read(int id, Conexión context)
    {


        // Ejecución
        try
        {

            var profile = await (from P in context.DataBase.Contacts
                                 where P.Id == id
                                 select new ContactModel
                                 {
                                     Birthday = P.Birthday,
                                     Id = P.Id,
                                     Mails = P.Mails,
                                     Nombre = P.Nombre,
                                     Phones = P.Phones
                                 }).FirstOrDefaultAsync();

            return new(Responses.Success, profile ?? new());
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="id">ID del perfil.</param>
    /// <param name="context">Contexto de conexión.</param>
    public async static Task<ReadAllResponse<ContactModel>> ReadAll(int id, Conexión context)
    {


        // Ejecución
        try
        {

            // Query de contactos
            var contacts = await (from P in context.DataBase.Contacts
                                  where P.Im.Id == id
                                  orderby P.Nombre
                                  select new ContactModel
                                  {
                                      Birthday = P.Birthday,
                                      Id = P.Id,
                                      Mails = P.Mails,
                                      Nombre = P.Nombre,
                                      Phones = P.Phones
                                  }).ToListAsync();

            return new(Responses.Success, contacts);
        }
        catch
        {
        }
        return new();
    }



}