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
        Task<bool> DeletePersonAsync(string Id, string deletedBy, string deletedTime);
        Task<bool> EditPersonAsync(Person person);
        Task<bool> EditPersonImagePathAsync(string personId, string imagePath, string updatedBy);
        Task<Person> GetPersonByIdAsync(string Id);
        Task<Person> GetPersonByNameAsync(string personName);
        Task<IList<Person>> SearchPersonsByNameAsync(string personName);
        Task<IList<Person>> GetPersonsAsync();
    }
}
