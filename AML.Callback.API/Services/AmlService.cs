using AML.Callback.API.Models;
using AML.Callback.API.Repositories;

namespace AML.Callback.API.Services
{
    public class AmlService : IAmlService
    {
        private readonly IAmlRepository _repository;
        private readonly ILogger<AmlService> _logger;

        public AmlService(IAmlRepository repository, ILogger<AmlService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> UpdateScreeningResult(AmlHitUpdateRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("AML request is null");
                throw new ArgumentException("Request cannot be null");
            }

            if (request.ProscribedStatus != 0 && request.ProscribedStatus != 1)
            {
                _logger.LogWarning("Invalid ProscribedStatus: {Status}", request.ProscribedStatus);
                throw new ArgumentException("ProscribedStatus must be 0 or 1");
            }

            try
            {
                var result = await _repository.UpdateCustomerScreening(request);

                if (!result)
                {
                    _logger.LogWarning("Database update failed for CustomerId: {CustomerId}", request.CustomerId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating AML screening result");
                throw;
            }
        }
    }
}