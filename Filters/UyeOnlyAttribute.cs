using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class UyeOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var uyeId = context.HttpContext.Session.GetInt32("UyeId");
        if (uyeId.HasValue == false)
        {
            context.Result = new RedirectToActionResult("Giris", "Uye", null);
        }

        base.OnActionExecuting(context);
    }
}
