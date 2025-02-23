using LIN.Types.Contacts.Models;
using LIN.Types.Responses;
using Microsoft.EntityFrameworkCore;

namespace LIN.Contacts.Persistence.Data;

public partial class Contacts(Context.DataContext context)
{

    /// <summary>
    /// Crear nuevo contacto.
    /// </summary>
    /// <param name="data">Modelo.</param>
    /// <param name="context">Contexto.</param>
    public async Task<CreateResponse> Create(ContactModel data)
    {
        // Ejecución
        try
        {

            // El usuario ya existe.
            context.Attach(data.Im);

            var res = context.Contacts.Add(data);
            await context.SaveChangesAsync();
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
    public async Task<ReadOneResponse<ContactModel>> Read(int id)
    {

        // Ejecución
        try
        {

            var profile = await (from P in context.Contacts
                                 where P.Id == id
                                 select new ContactModel
                                 {
                                     Picture = P.Picture,
                                     Birthday = P.Birthday,
                                     Id = P.Id,
                                     Mails = P.Mails,
                                     Name = P.Name,
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
    public async Task<ReadOneResponse<bool>> Iam(int contact, int profile)
    {
        // Ejecución
        try
        {

            var have = await (from P in context.Contacts
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
    public async Task<ReadAllResponse<ContactModel>> ReadAll(int id)
    {
        // Ejecución
        try
        {
            // Query de contactos
            var contacts = await (from contact in context.Contacts
                                  where contact.Im.Id == id
                                  orderby contact.Name
                                  select new ContactModel
                                  {
                                      Picture = contact.Picture,
                                      Birthday = contact.Birthday,
                                      Id = contact.Id,
                                      Mails = contact.Mails,
                                      Name = contact.Name,
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
    public async Task<ResponseBase> Delete(int id)
    {
        // Ejecución
        try
        {

            // Eliminar.
            await context.Contacts.Where(t => t.Id == id).ExecuteDeleteAsync();

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
    public async Task<ResponseBase> Update(ContactModel contactModel)
    {

        // Ejecución
        try
        {

            // Eliminar.
            await context.Contacts.Where(t => t.Id == contactModel.Id).ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(contact => contact.Picture, b => contactModel.Picture ?? b.Picture).
                            SetProperty(contact => contact.Name, b => contactModel.Name ?? b.Name).
                            SetProperty(contact => contact.Type, contactModel.Type)
                );


            // Actualizar números.
            foreach (var phone in contactModel.Phones)
            {
                await context.Phones.Where(t => t.Id == phone.Id).ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(contact => contact.Number, b => phone.Number ?? b.Number)
                );
            }


            // Actualizar correos.
            foreach (var mails in contactModel.Mails)
            {
                await context.Mails.Where(t => t.Id == mails.Id).ExecuteUpdateAsync(
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