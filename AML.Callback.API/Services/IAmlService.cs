using AML.Callback.API.Models;

namespace AML.Callback.API.Services
{
    public interface IAmlService
    {
        Task<bool> UpdateScreeningResult(AmlHitUpdateRequest request);
    }

}
