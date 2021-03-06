﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Poker.Server.DatabaseContext;
using Poker.Shared.Managers;
using Poker.Shared.Models.DatabaseModels;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Server.Services
{
    public class DatabaseService : IBalanceManager
    {
        private readonly ApplicationDbContext _context;
        private readonly Mapper _mapper;

        public DatabaseService(Mapper mapper, IConfiguration configuration)
        {
            _context = new ApplicationDbContext(configuration.GetConnectionString("MicrosoftSql"));
            _mapper = mapper;
        }

        public async Task<PokerUser> GetUser(LoginData loginData)
        {
            try
            {
                var currentUserModel = await _context.PokerUsers.FirstOrDefaultAsync(p => p.Username == loginData.Username && p.Password == Encrypt(loginData.Password));
                var currentUser = _mapper.Map<PokerUser>(currentUserModel);

                return currentUser;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> RegisterUser(RegistrationData registrationData)
        {
            try
            {
                var pokerUserModel = _mapper.Map<PokerUserModel>(registrationData);

                pokerUserModel.Password = Encrypt(pokerUserModel.Password);
                pokerUserModel.Balance = 1000;

                _context.PokerUsers.Add(pokerUserModel);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task UpdateBalance(PokerUser pokerUser)
        {
            var currentUserModel = await _context.PokerUsers.FirstOrDefaultAsync(p => p.Username == pokerUser.Username);
            currentUserModel.Balance = pokerUser.Balance;
            await _context.SaveChangesAsync();
        }

        private string Encrypt(string password)
        {
            var data = Encoding.ASCII.GetBytes(password);
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(data);
            var pw = Encoding.ASCII.GetString(sha1data);
            sha1.Clear();
            return pw;
        }
    }
}
