using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HZC.Utils.YunPian
{
    public static class YunPianServicesCollectionExtensions
    {
        public static void AddCustomYunPianService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<YunPianSettings>(configuration.GetSection("YunPianSettings"));

            services.AddYunPianService(options =>
                options.ApiKey = configuration.GetSection("YunPianSettings:YunPianApiKey").Value);
               
            services.AddSingleton<YunPianService>();
        }
    }
}
