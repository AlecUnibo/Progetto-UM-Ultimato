namespace GestioneAccessi.Services.Shared
{
    public partial class SharedService
    {
        GestioneAccessiDbContext _dbContext;

        public SharedService(GestioneAccessiDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
