using Mover.Core.Dto.User;
using System.Text.Json;

namespace Mover.HttpUtility
{
    public static class AppHttpContext
    {
        static IServiceProvider services = null;

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        /// <summary>
        /// Provides static access to the current HttpContext
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }

    }
    public static class SessionInfo
    {

        public static int SessionTimeOutInMinutes = 30;
        public static string SessionKey = "UserDetail";

        public static void SetSessionObject(this ISession session, UserSessionDto value)
        {
            string checkUserJson = JsonSerializer.Serialize(value);
            session.SetString(SessionKey, checkUserJson);
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
        public static UserSessionDto GetCurrentUser()
        {
            var result = AppHttpContext.Current.Session.GetObjectFromJson<UserSessionDto>(SessionKey);
            return result;
        }
    }
}
