/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2017-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: Rewrite
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basicknowledge
{
    public abstract class Animal
    {
        public Animal()
        {
            Console.WriteLine("New Animal");
        }
        public virtual void Shout()
        {
            Console.WriteLine("Animal Shout");
        }
    }
    public class Dog : Animal
    {
        public Dog()
        {
            Console.WriteLine("New Dog");
        }
        public override void Shout()
        {
            Console.WriteLine("Dog Shout");
        }
    }
}
