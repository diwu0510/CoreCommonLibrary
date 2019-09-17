using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HZC.Utils.UpYun
{
    public static class YunPianServicesCollectionExtensions
    {
        public static void AddUpYunService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UpYunSettings>(configuration.GetSection("UpYunSettings"));

            services.AddTransient<UpYunService>();
        }
    }
}
