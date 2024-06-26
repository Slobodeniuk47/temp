using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constants
{
    public static class DirectoriesInProject
    {
        public static List<string> All = new()
        {
            Images,
            UserImages,
            CategoryImages,
        };
        public static string Api = "https://localhost:7266";
        public static string Images = "Images";
        public static string UserImages = "Images/userImages";
        public static string CategoryImages = "Images/categoryImages";
    }
}
