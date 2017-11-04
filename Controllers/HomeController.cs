using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    public class HomeController : Controller
    {
        //public RedirectResult Index()
        //{
        //    return Redirect("swagger");
        //}
        public ContentResult Index()
        {
            return Content(
                "<div style='width: 300px; margin:auto; margin-top: 100px'>" +
                "<h3>Welcome to Cinema API!</h3>" +
                "<a href='swagger'>👉 Go to API Document</a>" +
                "</div>", "text/html"
                );
        }
    }
}