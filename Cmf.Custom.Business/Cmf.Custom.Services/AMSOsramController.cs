using Microsoft.AspNetCore.Mvc;

namespace Cmf.Custom.AMSOsram.Services
{
    /// <summary>
    /// AMSOsram Services
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AMSOsramController : ControllerBase
    {
        private const string OBJECT_TYPE_NAME = "Cmf.Custom.AMSOsram.Services.AMSOsramManagement";
    }
}