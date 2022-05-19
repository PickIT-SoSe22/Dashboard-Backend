
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;


public class JwtAuthenticationAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<Validator>();
        if (validator == null)
        {
            return;
        }

        var token = (string) context.HttpContext.Request.Headers["authorization"];
        if (token.IsNullOrEmpty())
        {
            context.Result = new BadRequestResult();
            return;
        }

        token = Regex.Replace(token, "bearer", "", RegexOptions.IgnoreCase).Replace(" ", "");
        var validatedToken = validator.ValidateToken(token);
        if (!validatedToken.Valid)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items.Add("Validated_Token", validatedToken);
        await next();
    }
}