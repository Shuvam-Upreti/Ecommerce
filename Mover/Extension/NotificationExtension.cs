using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mover.Extension
{
    public static class NotificationExtension
    {

        public static void NotifySuccess(this Controller controller, string message)
        {
            var msg = new
            {
                message,
                status = NotificationType.success.ToString()
            };

            controller.TempData["Message"] = JsonConvert.SerializeObject(msg);
        }
        public static void NotifyError(this Controller controller, string message)
        {
            var msg = new
            {
                message,
                status = NotificationType.error.ToString()
            };

            controller.TempData["Message"] = JsonConvert.SerializeObject(msg);
        }

        public static void NotifyInfo(this Controller controller, string message)
        {
            var msg = new
            {
                message,
                status = NotificationType.info.ToString()
            };

            controller.TempData["Message"] = JsonConvert.SerializeObject(msg);
        }

        public static void NotifyModelStateErrors(this Controller controller)
        {
            var errors = string.Join("<br>", controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
            controller.NotifyInfo(errors);
        }
    }

    public enum NotificationType
    {
        error,
        success,
        info
    }
}
