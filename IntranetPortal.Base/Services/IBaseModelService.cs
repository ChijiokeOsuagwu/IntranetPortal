using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IBaseModelService
    {
        Task<bool> CreatePersonAsync(Person person);

        Task<bool> UpdatePersonAsync(Person person);

        Task<bool> PersonExistsAsync(string personId);

        Task<Person> GetPersonAsync(string personId);

        Task<string> GenerateAutoNumberAsync(string NumberType);

        Task<bool> IncrementAutoNumberAsync(string NumberType);
    }
}
