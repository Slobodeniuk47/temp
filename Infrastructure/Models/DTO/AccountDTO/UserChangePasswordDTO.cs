﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.DTO.AccountDTO
{
    public class UserChangePasswordDTO
    {
        public long UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
