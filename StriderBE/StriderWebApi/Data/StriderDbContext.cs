using Microsoft.EntityFrameworkCore;
using System;

namespace StriderWebApi.Data
{
    public class StriderDbContext : DbContext
    {
        #region Constructors
        public StriderDbContext(DbContextOptions<StriderDbContext> options) : base(options) {}
        #endregion

        #region DbSets
        #endregion
    }
}
