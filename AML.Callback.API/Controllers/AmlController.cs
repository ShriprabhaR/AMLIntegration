using AML.Callback.API.Models;
using AML.Callback.API.Services;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> UpdateScreeningResult([FromBody] AmlHitUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("AML request received: {@Request}", request);

                var result = await _amlService.UpdateScreeningResult(request);

                if (result)
                {
                    return Ok(new AmlResponseMdl
                    {
                        Status = "SUCCESS",
                        Message = "Screening result updated",
                        AlertId = request.AlertId
                    });
                }

                return BadRequest(new AmlResponseMdl
                {
                    Status = "FAILED",
                    Message = "Update failed",
                    AlertId = request.AlertId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AML screening");

                return StatusCode(500, new
                {
                    status = "FAILED",
                    message = "Internal server error"
                });
            }
        }
    }
}
