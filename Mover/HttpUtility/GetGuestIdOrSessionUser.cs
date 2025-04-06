using Mover.Core.Dto.User;
using Microsoft.AspNetCore.Http;

namespace Mover.HttpUtility
{
    public class GetGuestIdOrSessionUser
    {
        private readonly string guestCookieKey = "GuestId";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetGuestIdOrSessionUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string? guestId, UserSessionDto? currentUser)> GetGuestIdOrSessionUserId()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                return (null, null);
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var currentUser = SessionInfo.GetCurrentUser();
                return (null, currentUser);
            }
            else
            {
                var guestId = context.Request.Cookies[guestCookieKey];
                return (guestId, null);
            }
        }
    }
}
