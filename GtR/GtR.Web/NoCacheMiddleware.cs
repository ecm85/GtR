using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GtR.Web
{
	public class NoCacheMiddleware
	{
		private readonly RequestDelegate next;

		public NoCacheMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
			context.Response.Headers["Pragma"] = "no-cache";
			context.Response.Headers["Expires"] = "0";
			await next.Invoke(context);
		}
	}
}
