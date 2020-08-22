using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Poker.Server.DatabaseContext;
using Poker.Server.Services;
using Poker.Shared.Models.DatabaseModels;
using Poker.Shared.Models.DomainModels;

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public RegistrationController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // POST: api/Registration
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<bool> PostPokerUserModel([FromBody]RegistrationData registrationData)
        {
            return await _databaseService.RegisterUser(registrationData);
        }
    }
}
