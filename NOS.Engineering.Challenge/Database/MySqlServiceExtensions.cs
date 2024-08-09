using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public static class MySqlServiceExtensions
    {
        public static IServiceCollection AddMySqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string mySqlConnection = configuration.GetConnectionString("DefaultConnection")!;

            services.AddDbContextPool<AppDbContext>
                (
                    options => options.UseMySql(
                                                mySqlConnection, 
                                                ServerVersion.AutoDetect(mySqlConnection)
                                                )
                );

            return services;
        }
    }
}
