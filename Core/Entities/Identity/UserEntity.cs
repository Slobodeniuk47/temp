using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Identity
{
    [Table("tbl_Users")]
    public class UserEntity : IdentityUser<long>
    {
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(200)]
        public string Image { get; set; }
        public bool IsGoogle { get; set; } = false;
        public virtual ICollection<PermissionsEntity> Permissions { get; set; }
    }
}
