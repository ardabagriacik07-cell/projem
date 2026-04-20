using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var admin = context.HttpContext.Session.GetString("Admin");
        if (string.IsNullOrWhiteSpace(admin))
        {
            context.Result = new RedirectToActionResult("Login", "Admin", null);
        }

        base.OnActionExecuting(context);
    }
}
