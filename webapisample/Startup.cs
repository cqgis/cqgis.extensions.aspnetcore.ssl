using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cqgis.extensions.aspnetcore.ssl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace webapisample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAspNetCoreRSAManager(t =>
                {
                    t.DecryptKey = "pwdlock";
                    t.PrivateKey_Des =
                        "W/JlZbE5pDeSfnEiTlJm+oMRSV6ANh+4cYFpaFEXsEckQH9SBRR3IzORQZZEylsdwDuBiOR+XTiM0HIIZtTcIgvJtUmeAz7UMnR1huthevzieC/YnrKGL+CxWNnh3M4r3++duivvVHBBC0Io7TreaKlUHiotvefcypdwl9uLhd9kgkBBgX8AwZKOGUz4GXQrAzqp9D69yefz+bsCIwuh/pW4/nNi4c3qwuyUtGuIqToOAPVnT03D3JYp8onFMkotfLuAU24ifv8/pwftSllJIS6uXK0h9P9hGrgXqWL1BLHTlDI/3cRLvTyue/D/fE0dSyONkKrAi7b0hCDqg0nQWYpLDpNqr96Q1LWEdS8PqYbNjEyTpM75ib1zrjuM6ol6JsE+iwojFjiHG1+/Yg1z43+n0vq/WTknUtTo+xdBtbV4/vc5Ul5XMkm/rL0v6D5/2PsfK7zekIuQ136lYr/fc+8awNm07J13khbP9IAxXBwKYC+zSER3WTlnQpghqTnOoUwmoNppu7+YvncVmaH/IUgNp6sHHWHXOz6XnjqSWaG2gcDtTeBZPjyHyGDYkKqoPS2KK9jPkZ29QJYmM9lg23X6PtS7vpSpfqsfLMtCWKsMBUCN2nO4+XmgO6+s/oYe4Raf5UCg+UMq1TMLHzywOebmuF2owd0HEU3nrzzV5/M+t4Fh4JAMakDWRS3RAmqLTCAvM/rzkRwYjzCZeVyITU2KwOR6hvQL7P0W4tvaHxUvzNzAk+qsEghg1Bzf/3oW4E5JghbxxWZ7MXuM5vSHOA6kqXwcvxgDkTZlF6WnvXwPPDiYuQuLFWIt6wfYVZJn4QXpgFLWGxQ9C8/x9OI3A5nG2JG807ofZSBcqYTJJmY85yhHeQ0qrKhjnAJUUN2bEhD/sjfQrhDEF2EDB2oNn6zwN2Noy6LSly9DkgcUbKjE81n7BIpTYOxXhZe9JlfylHDjjdurhsmyJYVXQiDwsky8c10nWYEjAdX4SJO1eM77GUXbUENazRCV3YgF0MKdDzY8SLwLQB4zVAJxvN6Yv6fBfP/iS75dOCgzaw/+MWA2GmxlnM6lbeNwpGSAAAzxsHcGG0zP9qPH9nKVJ8BJbOQQDKe28u2sfTw7FjpDm8N2DvJzwt3QTh9haxfFnozu54ZNhn90LKFrxpE1oZVJ9UJtB65ytdqljeT8yZ8VdcD8N8dTLnu8SpTfVe4u3iIvpDrv1DdFPSutbWw8Auk87w==";
                    t.PublicKey_Des =
                        "W/JlZbE5pDeSfnEiTlJm+oMRSV6ANh+4cYFpaFEXsEckQH9SBRR3IzORQZZEylsdwDuBiOR+XTiM0HIIZtTcIgvJtUmeAz7UMnR1huthevzieC/YnrKGL+CxWNnh3M4r3++duivvVHBBC0Io7TreaKlUHiotvefcypdwl9uLhd9kgkBBgX8AwZKOGUz4GXQrAzqp9D69yefz+bsCIwuh/pW4/nNi4c3qwuyUtGuIqToOAPVnT03D3JYp8onFMkotfLuAU24ifv8/pwftSllJIS6uXK0h9P9hGrgXqWL1BLHJLc/0IverMlJhcVsYze7alJ32jRrpS6cLpHjySAwFaw==";
                },
                t =>
                {
                    t.HearderTokenKey = "ssltoken";

                });
            
            services.AddMvc();

        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseReturnSSL();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
