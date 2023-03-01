﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Library.Auth
{
    public class AuthService
    {
        private const string module = "USERS";
        private readonly AuthRepository repository;
        public AuthService()
        {
            repository = new AuthRepository();
        }

        public EmployeeViewModel Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) throw new ApplicationException("Username is required.");
            if (string.IsNullOrEmpty(password)) throw new ApplicationException("Password is required.");

            var user = repository.Login(username, password);

            if (user is null) throw new ApplicationException("Wrong username or password.");
            return user;
        }
    }
}
