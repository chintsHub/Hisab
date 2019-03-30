using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hisab.Dapper.Identity;

namespace Hisab.Dapper.Repository
{
    public interface IApplicationRoleRepository
    {
        Task<ApplicationRole> FindByRoleNameAsync(string roleName);
    }

    internal class ApplicationRoleRepository : RepositoryBase, IApplicationRoleRepository
    {
        public ApplicationRoleRepository(IDbTransaction transaction) : base(transaction)
        {

        }
        public async Task<ApplicationRole> FindByRoleNameAsync(string roleName)
        {
           return await Connection.QuerySingleOrDefaultAsync<ApplicationRole>($@"SELECT * FROM [ApplicationRole]
                    WHERE [Name] = @{nameof(roleName)}", new { roleName },Transaction);
        }
    }
}
