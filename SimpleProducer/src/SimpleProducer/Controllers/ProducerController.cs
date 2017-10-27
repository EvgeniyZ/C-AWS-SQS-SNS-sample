using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Mvc;
using SimpleProducer.Models;

namespace SimpleProducer.Controllers
{
    [Route("api/[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        public ProducerController(AmazonSimpleNotificationServiceClient snsClient)
        {
            _snsClient = snsClient;
        }

        [HttpPost]
        public async Task<IActionResult> ProduceMessage([FromBody] MessageModel message)
        {
            //await _snsClient.PublishAsync("", "");
            return Ok();
        }
    }
}
