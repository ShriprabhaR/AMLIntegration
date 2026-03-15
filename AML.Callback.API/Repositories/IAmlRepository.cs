using AML.Callback.API.Models;

namespace AML.Callback.API.Repositories
{
    public interface IAmlRepository
    {
        Task<bool> UpdateCustomerScreening(AmlHitUpdateRequest request);
    }
}
