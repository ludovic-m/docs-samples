using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi.Controllers
{
    [Route("batch")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public BatchController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IDictionary<string, string> values)
        {
            var grain = _grainFactory.GetGrain<IBatchGrain>(0, "OrleansBook.GrainClasses.RobotGrain"); // It's a stateless worker, any identity is good

            var input = values.Select(k => (k.Key, k.Value)).ToArray();

            await grain.AddInstructions(input);

            return Ok();
        }
    }
}
