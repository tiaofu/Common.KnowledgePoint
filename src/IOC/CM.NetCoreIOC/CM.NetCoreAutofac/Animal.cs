using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CM.NetCoreAutofac
{
    /// <summary>
    /// 动物类
    /// </summary>
    public abstract class Animal
    {
        public virtual string Call() { return ""; }
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

        public override string Call()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// 测试类
    /// </summary>
    public class TestA
    {
    }
    public class TestB : TestA
    {
        public Animal animal { get; set; }
    }
}
