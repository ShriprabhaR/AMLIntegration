using AML.Callback.API.Models;

namespace AML.Callback.API.Services
{
    public interface IAmlService
    {
        Task<List<CustomerResponse>> UpdateScreeningResult(List<AmlHitUpdateRequest> request);
    }

}
