using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Poker.Server.Providers;
using Poker.Server.Services;
using Poker.Shared.Models.DomainModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly PokerUserProvider _pokerUserProvider;

        public LoginController(DatabaseService databaseService, PokerUserProvider pokerUserProvider)
        {
            _databaseService = databaseService;
            _pokerUserProvider = pokerUserProvider;
        }

        // POST api/<LoginController>
        [HttpPost]
        public async Task<LoggedInUser> Post([FromBody] LoginData loginData)
        {
            var pokerUser = await _databaseService.GetUser(loginData);
            var id = _pokerUserProvider.Add(pokerUser);
            return new LoggedInUser(id, pokerUser);
        }

        [HttpDelete]
        [Route("{id}")]
        public void Delete(string id)
        {
            _pokerUserProvider.Remove(id);
        }

    }
}
