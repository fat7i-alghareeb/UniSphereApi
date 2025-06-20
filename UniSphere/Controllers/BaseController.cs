using Microsoft.AspNetCore.Mvc;

namespace UniSphere.Api.Controllers;

[ApiController]
[Route("api/")]
[Produces("application/json")]
public class BaseController : ControllerBase
{
    protected Languages Lang 
    {
        get
        {
            if (HttpContext.Items["lang"] is string and "ar")
            {
                return Languages.Ar ;
            }
            return Languages.En; // Default fallback
        }
    }
}

// Better placed in a separate file (e.g., Enums/Languages.cs)
public enum Languages
{
    En,
    Ar
}
