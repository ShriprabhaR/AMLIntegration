using AML.Callback.API.Models;
using AML.Callback.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AML.Callback.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmlController : ControllerBase
    {
        private readonly IAmlService _amlService;
        private readonly ILogger<AmlController> _logger;

        public AmlController(IAmlService amlService, ILogger<AmlController> logger)
        {
            _amlService = amlService;
            _logger = logger;
        }

        [HttpPost("update-screening-result")]
        public async Task<IActionResult> UpdateScreeningResult(List<AmlUpdateRequest> request)
        {
            _logger.LogInformation("AML request received: {@Request}", request);

            var responseList = await _amlService.UpdateScreeningResult(request);

            return Ok(new ApiResponse
            {
                Responses = responseList
            });
        }
    }
}