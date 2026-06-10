// FILE: Api/Middleware/SessionMiddleware.cs
namespace Graduation_Project.Api.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        private const string SessionCookieName = "lb_session";

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string sessionId;

            // لو في cookie موجودة → استخدمها
            if (context.Request.Cookies.TryGetValue(SessionCookieName, out var existingSession)
                && !string.IsNullOrWhiteSpace(existingSession))
            {
                sessionId = existingSession;
            }
            else
            {
                // ✅ اعمل session جديدة تلقائياً
                sessionId = Guid.NewGuid().ToString();

                context.Response.Cookies.Append(SessionCookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,   // مش قابلة للقراءة من JS
                    Secure   = true,   // HTTPS فقط
                    SameSite = SameSiteMode.Lax,
                    Expires  = DateTimeOffset.UtcNow.AddDays(30)
                });
            }

            // حطها في الـ context عشان الـ controller يقدر يقرأها
            context.Items[SessionCookieName] = sessionId;

            await _next(context);
        }
    }

    // Extension method عشان التسجيل يبقى نظيف
    public static class SessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionTracking(this IApplicationBuilder app)
            => app.UseMiddleware<SessionMiddleware>();
    }
}