using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CM.NetCoreIOC.Controllers
{
    public class HomeController : Controller
    {
        Animal animal1;
        Animal animal2;
        public HomeController(Animal animal1, Animal animal2)
        {
            this.animal1 = animal1;
            this.animal2 = animal2;
        }
        public string Index()
        {
            return $"Animal 1 Name:{animal1.Call()} Animal 2 Name:{animal2.Call()}";
        }
    }
}
