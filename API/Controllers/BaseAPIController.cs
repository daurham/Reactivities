using Microsoft.AspNetCore.Mvc;
/* IMPORTANT NOTE:
    The purpose of this file is to avoid having to retype these attributes on every controller. 
    Therefore, I derive all new classes from this one.
*/

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseAPIController
    {
        
    }
}