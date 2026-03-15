using AML.Callback.API.Models;
using AML.Callback.API.Repositories;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace AML.Callback.API.Services
{
    public class AmlService : IAmlService
    {
        private readonly IAmlRepository _repository;

        public AmlService(IAmlRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> UpdateScreeningResult(AmlHitUpdateRequest request)
        {
            return await _repository.UpdateCustomerScreening(request);
        }
    }
}
