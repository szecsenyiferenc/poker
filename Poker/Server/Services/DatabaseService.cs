using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poker.Server.DatabaseContext;
using Poker.Shared.Models.DatabaseModels;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Services
{
    public class DatabaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly Mapper _mapper;

        public DatabaseService(Mapper mapper)
        {
            _context = new ApplicationDbContext();
            _mapper = mapper;
        }

        public async Task<PokerUser> GetUser(LoginData loginData)
        {
            try
            {
                var currentUserModel = await _context.PokerUsers.FirstOrDefaultAsync(p => p.Username == loginData.Username && p.Password == loginData.Password);
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
                _context.PokerUsers.Add(pokerUserModel);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<PokerUser> GetUserFromBase64(string base64Auth)
        {
            try
            {
                string rawString = base64Auth.Substring(6, base64Auth.Length - 6);
                string rawLoginData = StringUtils.Base64Decode(rawString);
                string[] loginDataArray = rawLoginData.Split(':');

                LoginData loginData = new LoginData(loginDataArray[0], loginDataArray[1]);

                return await GetUser(loginData);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
