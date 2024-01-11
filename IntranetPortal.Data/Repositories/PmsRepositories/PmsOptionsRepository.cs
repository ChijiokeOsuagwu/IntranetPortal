using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Data.Repositories.PmsRepositories
{
    public class PmsOptionsRepository
    {
        public IConfiguration _config { get; }
        public PmsOptionsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

    }
}
