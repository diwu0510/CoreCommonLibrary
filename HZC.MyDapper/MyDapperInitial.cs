using Microsoft.Extensions.DependencyInjection;
using System;

namespace HZC.MyDapper
{
    public static class MyDapperInitial
    {
        public static void AddMyDapper<TDapperContext, TPrimaryKey>(this IServiceCollection services, Action<DapperContextOption<TPrimaryKey>> options) 
            where TDapperContext : BaseDapperContext<TPrimaryKey>
        {
            var option = new DapperContextOption<TPrimaryKey>();
            options.Invoke(option);

            services.AddSingleton(option);
            services.AddScoped<TDapperContext>();
        }
    }
}
