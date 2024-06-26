using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constants
{
    public static class Roles
    {
        public static List<string> All = new()
        {
            Admin,
            Moder,
            Editor,
            Supporter,
            PermanentUser,
            User,
        };

        public const string Admin = "Admin";
        public const string Moder = "Moder";
        public const string Editor = "Editor";
        public const string Supporter = "Supporter";

        public const string PermanentUser = "Permanent User";
        public const string User = "User";
    }
}
