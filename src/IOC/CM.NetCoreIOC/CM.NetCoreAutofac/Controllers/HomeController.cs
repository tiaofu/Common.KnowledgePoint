using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CM.NetCoreAutofac.Controllers
{
    public class HomeController : Controller
    {
        public Animal animal1 { get; set; }
        public Animal animal2 { get; set; }

        public TestA a { get; set; }
        public HomeController(Animal animal1, Animal animal2,TestA a)
        {
            this.animal1 = animal1;
            this.animal2 = animal2;
            this.a = a;
        }
        public string Index()
        {
            return $"Animal 1 Name:{animal1.Call()} Animal 2 Name:{animal2.Call()}";
        }
    }
}
