using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Interfaces;
using Interfaces.IServices;
using Services;

namespace CommissioningManager2
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            Application.Run(ServiceProvider.GetRequiredService<DashBoard>());

            //Application.Run(new DashBoard());
        }

        public static IServiceProvider ServiceProvider { get; private set; }
        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.AddTransient<IFileServices, FileServices>();
                    services.AddTransient<DashBoard>();
                });
        }

        //public static class ServiceProviderHolder
        //{
        //    public static IServiceProvider ServiceProvider { get; set; }
        //}
        //private static void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddDbContext<ScheduleDBContext>();
        //    services.AddTransient<AddMachine>();
        //}
    }
}