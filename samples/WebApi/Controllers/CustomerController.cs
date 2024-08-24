using EasyChain;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IChain<CustomerPayload> _chain;

        public CustomerController(IChain<CustomerPayload> chain)
        {
            _chain = chain;
        }

        [HttpPost]
        public async Task CreateCustomer([FromBody] CustomerPayload payload)
        {
            await _chain.Run(payload);
        }
    }
}
