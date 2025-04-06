namespace Mover.Middleware
{
    public class GuestIdMiddleware
    {
        private readonly RequestDelegate _next;
        public GuestIdMiddleware(RequestDelegate next)
        {
            _next=next;
        }

        public async Task Invoke(HttpContext context)
        {
 
            if (!context.Request.Cookies.ContainsKey("GuestId"))
            {
                string guestId = Guid.NewGuid().ToString();
                context.Response.Cookies.Append("GuestId", guestId, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(90),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }

            await _next(context);
        }
    }

}
