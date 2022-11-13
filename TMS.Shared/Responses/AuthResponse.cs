﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Shared.Responses
{
    public class AuthResponse
    {

        public AuthResponse(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }

        public AuthResponse(List<string> errors)
        {
            Errors = errors;
        }

        public AuthResponse(string token, string refreshToken, List<string> errors)
        {
            Token = token;
            RefreshToken = refreshToken;
            Errors = errors;
        }

        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Errors { get; set; }

    }
}
