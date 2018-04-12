using Matricks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Matricks.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(string user, string password);
        Task<User> Login(string userName, string password);

         bool ValidateUserName(string userName);
    }
}