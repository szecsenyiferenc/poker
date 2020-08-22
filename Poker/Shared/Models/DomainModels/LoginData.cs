using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class LoginData
    {

        public LoginData()
        {

        }
        public LoginData(string username, string password)
        {
            Username = username;
            Password = password;
        }

        [Required (ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"{Username}:{Password}";
        }
    }
}
