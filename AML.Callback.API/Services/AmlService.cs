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

        public async Task<List<CustomerResponse>> UpdateScreeningResult(List<AmlHitUpdateRequest> requests)
        {
            if (requests == null || !requests.Any())
            {
                _logger.LogWarning("AML request list is empty");
                throw new ArgumentException("Request cannot be empty");
            }

            try
            {
                var responses = await _repository.UpdateCustomerScreening(requests);

                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating AML screening result");

                return requests.Select(x => new CustomerResponse
                {
                    CustomerId = x.CustomerId,
                    StatusCode = 500,
                    Message = "Failed"
                }).ToList();
            }
        }
    }
}