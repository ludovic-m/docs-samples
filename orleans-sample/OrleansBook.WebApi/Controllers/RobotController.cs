using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainClasses;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi.Controllers
{
    [Route("robot")]
    [ApiController]
    public class RobotController : ControllerBase
    {
        string grainNamespace = "OrleansBook.GrainClasses.RobotGrain";
        private readonly IGrainFactory _grainFactory;

        public RobotController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet]
        [Route("{name}/instruction", Name = "GetNextInstruction")]
        public async Task<IActionResult> Get(string name)
        {
            IRobotGrain grain = _grainFactory.GetGrain<IRobotGrain>(name, grainNamespace);
            var instruction = await grain.GetNextInstruction();

            return Ok(instruction);
        }

        [HttpGet]
        [Route("{name}/instruction/count", Name = "GetInstructionsCount")]
        public async Task<IActionResult> GetCount(string name)
        {
            IRobotGrain grain = _grainFactory.GetGrain<IRobotGrain>(name, grainNamespace);
            var instruction = await grain.GetInstructionCount();

            return Ok(instruction);
        }

        [HttpPost]
        [Route("{name}/instruction", Name = "AddNewInstruction")]
        public async Task<IActionResult> Add(string name, string instruction)
        {
            IRobotGrain grain = _grainFactory.GetGrain<IRobotGrain>(name, grainNamespace);
            await grain.AddInstruction(instruction);
            return Ok();
        }
    }
}
