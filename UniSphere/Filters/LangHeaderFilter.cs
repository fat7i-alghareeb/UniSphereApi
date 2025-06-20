using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace UniSphere.Api.Filters;

public class LangHeaderFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> SupportedLanguages = new() { "ar", "en" };
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        if (!request.Headers.TryGetValue("lang", out var langHeader) || string.IsNullOrWhiteSpace(langHeader))
        {
            context.Result = new BadRequestObjectResult(new { error = "Missing 'lang' header. Supported values: 'ar', 'en'." });
            return;
        }

        var lang = langHeader.ToString().Trim().ToLowerInvariant();
        if (!SupportedLanguages.Contains(lang))
        {
            context.Result = new BadRequestObjectResult(new { error = "Invalid 'lang' header. Supported values: 'ar', 'en'." });
            return;
        }

        // Make the language available for later use
        context.HttpContext.Items["lang"] = lang;
        await next();
    }
}
