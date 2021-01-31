using System.IO;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;

namespace GtR.Web
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
	{
		protected override void Init(IWebHostBuilder builder) => builder
			.UseContentRoot(Directory.GetCurrentDirectory())
			.UseStartup<Startup>()
			.UseLambdaServer();

		protected override void PostMarshallRequestFeature(
			IHttpRequestFeature aspNetCoreRequestFeature,
			APIGatewayProxyRequest apiGatewayRequest,
			ILambdaContext lambdaContext)
		{
			aspNetCoreRequestFeature.PathBase = "";
			aspNetCoreRequestFeature.Path = apiGatewayRequest.Path;
		}
	}
}
