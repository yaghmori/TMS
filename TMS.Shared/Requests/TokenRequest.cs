﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Shared.Requests
{
    public class TokenRequest
    {


        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }
}
