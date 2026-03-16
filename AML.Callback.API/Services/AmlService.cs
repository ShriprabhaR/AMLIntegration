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

        public async Task<List<CustomerResponse>> UpdateScreeningResult(List<AmlUpdateRequest> requests)
        {
            if (requests == null || !requests.Any())
            {
                _logger.LogWarning("AML request list is empty");
                throw new ArgumentException("Request cannot be empty");

            }
            foreach (var req in requests)
            {
                if (req.AlertId == 0)
                    throw new ArgumentException($"AlertId cannot be 0 for CustomerId: {req.CustomerId}");

                if (req.ProscribedStatus != 0 && req.ProscribedStatus != 1)
                    throw new ArgumentException($"ProscribedStatus must be 0 or 1 for CustomerId: {req.CustomerId}");
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