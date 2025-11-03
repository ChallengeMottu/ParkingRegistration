using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Asp.Versioning;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Application.Services.interfaces.v2;

namespace PulseSystem.API.Tests.Integration.Configuration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IParkingService>? ParkingServiceMock { get; private set; }
        public Mock<IGatewayService>? GatewayServiceMock { get; private set; }
        
        public Mock<IZoneService>? ZoneServiceMock { get; private set; }
        
        public Mock<IParkingServiceV2>? ParkingServiceV2Mock { get; private set; }
        
        

        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
               
                services.RemoveAll(typeof(IAuthenticationSchemeProvider));
                services.RemoveAll(typeof(IAuthenticationHandlerProvider));
                
                services.AddApiVersioning(options =>
                    {
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ReportApiVersions = true;
                    })
                    .AddApiExplorer(options =>
                    {
                        options.GroupNameFormat = "'v'VVV"; 
                        options.SubstituteApiVersionInUrl = true;
                    });

                
                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultScheme = "Test";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                
                


                
                services.RemoveAll(typeof(IParkingService));
                ParkingServiceMock = new Mock<IParkingService>();
                services.AddSingleton(ParkingServiceMock.Object);
                
                services.RemoveAll(typeof(IGatewayService));
                GatewayServiceMock = new Mock<IGatewayService>();
                services.AddSingleton(GatewayServiceMock.Object);
                
                services.RemoveAll(typeof(IZoneService));
                ZoneServiceMock = new Mock<IZoneService>();
                services.AddSingleton(ZoneServiceMock.Object);
                
                services.RemoveAll(typeof(IParkingServiceV2));
                ParkingServiceV2Mock = new Mock<IParkingServiceV2>();
                services.AddSingleton(ParkingServiceV2Mock.Object);
                
                
            });
        }

    }

    
    
    
}
