using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CM.NetCoreIOC
{
    /// <summary>
    /// 动物类
    /// </summary>
    public interface Animal
    {
        string Call();
    }

    /// <summary>
    /// 狗狗类
    /// </summary>
    public class Dog : Animal
    {
        public Dog()
        {
            this.Name = Guid.NewGuid().ToString();
        }
        public string Name { get; set; }

        public string Call()
        {
            return this.Name;
        }
    }
}
