using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.DTO.AccountDTO
{
    public class ExternalLoginDTO
    {
        public string Provider { get; set; }
        public string Token { get; set; }
    }
}
