using AML.Callback.API.Models;

namespace AML.Callback.API.Repositories
{
    public interface IAmlRepository
    {
        Task<List<CustomerResponse>> UpdateCustomerScreening(List<AmlHitUpdateRequest> requests);
    }
}
