using backend_trial.Models.Domain;

namespace backend_trial.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(User user);
    }
}
