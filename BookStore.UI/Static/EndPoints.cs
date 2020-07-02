using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.UI.Static
{
    public class EndPoints
    {
        public static string BaseUrl = "http://localhost:46045/";
        public static string AuthorsEndpoint = $"{BaseUrl}api/Authors/";
        public static string BooksEndpoint = $"{BaseUrl}api/Books/";
        public static string RegisterEndpoint = $"{BaseUrl}api/users/register";
        public static string LoginEndpoint = $"{BaseUrl}api/users/login/";

    }
}
