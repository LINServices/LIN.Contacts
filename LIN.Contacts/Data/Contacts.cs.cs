namespace LIN.Contacts.Data;

public partial class Contacts
{

    /// <summary>
    /// Crear nuevo contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    /// <param name="context">Contexto.</param>
    public static async Task<CreateResponse> Create(ContactModel data, Conexión context)
    {
        // Ejecución
        try
        {

            // El usuario ya existe.
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
    /// Obtiene un contacto.
    /// </summary>
    /// <param name="id">Id del contacto</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<ContactModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var profile = await (from P in context.DataBase.Contacts
                                 where P.Id == id
                                 select new ContactModel
                                 {
                                     Picture = P.Picture,
                                     Birthday = P.Birthday,
                                     Id = P.Id,
                                     Mails = P.Mails,
                                     Nombre = P.Nombre,
                                     Type = P.Type,
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
    /// Acceso IAM en un contacto.
    /// </summary>
    /// <param name="contact">Id del contacto.</param>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadOneResponse<bool>> Iam(int contact, int profile, Conexión context)
    {

        // Ejecución
        try
        {

            var have = await (from P in context.DataBase.Contacts
                              where P.Id == contact
                              && P.Im.Id == profile
                              select P.Id
                                 ).FirstOrDefaultAsync();


            if (have <= 0)
                return new(Responses.Unauthorized);

            return new(Responses.Success, true);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ReadAllResponse<ContactModel>> ReadAll(int id, Conexión context)
    {


        // Ejecución
        try
        {

            // Query de contactos
            var contacts = await (from contact in context.DataBase.Contacts
                                  where contact.Im.Id == id
                                  orderby contact.Nombre
                                  select new ContactModel
                                  {
                                      Picture = contact.Picture,
                                      Birthday = contact.Birthday,
                                      Id = contact.Id,
                                      Mails = contact.Mails,
                                      Nombre = contact.Nombre,
                                      Type = contact.Type,
                                      Phones = contact.Phones
                                  }).ToListAsync();

            return new(Responses.Success, contacts);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Eliminar un contacto.
    /// </summary>
    /// <param name="id">Id del contacto.</param>
    /// <param name="context">Contexto de conexión.</param>
    public static async Task<ResponseBase> Delete(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Eliminar.
            await context.DataBase.Contacts.Where(t => t.Id == id).ExecuteDeleteAsync();

            return new(Responses.Success);
        }
        catch
        {
        }
        return new();
    }



    /// <summary>
    /// Actualizar un contacto.
    /// </summary>
    /// <param name="id">Id del contacto.</param>
    /// <param name="contactModel">Modelo del contacto.</param>
    /// <param name="context">Contexto.</param>
    public static async Task<ResponseBase> Update(ContactModel contactModel, Conexión context)
    {

        // Ejecución
        try
        {

            // Eliminar.
            await context.DataBase.Contacts.Where(t => t.Id == contactModel.Id).ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(contact => contact.Picture, b => contactModel.Picture ?? b.Picture).
                            SetProperty(contact => contact.Nombre, b => contactModel.Nombre ?? b.Nombre).
                            SetProperty(contact => contact.Type, contactModel.Type)
                );


            // Actualizar números.
            foreach (var phone in contactModel.Phones)
            {
                await context.DataBase.Phones.Where(t => t.Id == phone.Id).ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(contact => contact.Number, b => phone.Number ?? b.Number)
                );
            }


            // Actualizar correos.
            foreach (var mails in contactModel.Mails)
            {
                await context.DataBase.Mails.Where(t => t.Id == mails.Id).ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(contact => contact.Email, b => mails.Email ?? b.Email)
                );
            }

            return new(Responses.Success);
        }
        catch
        {
        }
        return new();
    }



}