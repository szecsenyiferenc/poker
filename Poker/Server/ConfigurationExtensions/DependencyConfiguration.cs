using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poker.Server.DatabaseContext;
using Poker.Server.Providers;
using Poker.Server.Services;
using Poker.Shared.Models.DatabaseModels;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.ConfigurationExtensions
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection InitIOC(this IServiceCollection services)
        {
            services.AddSingleton<AutoMapper.IConfigurationProvider>(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PokerUserModel, PokerUser>();
                cfg.CreateMap<PokerUser, PokerUserModel>();

                cfg.CreateMap<RegistrationData, PokerUserModel>();
                cfg.CreateMap<Table, TableViewModel>();
            }));
            services.AddSingleton<Mapper>();
            services.AddSingleton<TableProvider>();
            services.AddSingleton<PokerUserProvider>();
            services.AddTransient<DatabaseService>();

            return services;
        }
    }
}
