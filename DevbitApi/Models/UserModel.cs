﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevbitApi.Models
{
    public class UserModel
    {
        public int id { get; set; }

        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
        public object Token { get;  set; }
    }
}
