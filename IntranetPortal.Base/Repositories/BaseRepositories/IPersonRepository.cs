using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BaseRepositories
{
    public interface IPersonRepository
    {
        Task<bool> AddPersonAsync(Person person);
        Task<bool> DeletePersonAsync(string Id);
        Task<bool> EditPersonAsync(Person person);
        Task<Person> GetPersonByIdAsync(string Id);
        Task<IList<Person>> GetPersonsAsync();
    }
}
