/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/11 15:24:36
 * Modify: 2018/4/11 15:24:36
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basicknowledge
{
    public class Sortalgorithm
    {
        /// <summary>
        /// 二分排序方法
        /// 核心思想在于根据索引元素，查找之前的元素，然后将索引之前的元素拆分，找到中间位置的元素，如果目标元素大于中间的元素，再对后半部分的元素取中间值
        /// 如果目标元素小于中间元素，那就再取前半部分的中间元素，直到左边的索引小于右边的索引，然后将目标left即为目标位置，将left以及目标位置的元素前一位元素向后平移一位，将目标元素置于left即可
        /// 如下：2 处于index为 3 ，此时left为0，right：2，mid:1,中间元素为4，2 le 4,所以取前半部分的元素继续比对,此时left:0,right0,mid:0,中间元素1，2 ge 1,left变为1，也就代表1-2的元素向后平移一位
        /// 变为 ： 1，4，4，5，3，9，0，7，6 然后将目标位置left替换为 2,变为 ： 1，2，4，5，3，9，0，7，6
        /// </summary>
        public void MiddleSplitSort()
        {
            int[] arr = { 1, 4, 5, 2, 3, 9, 0, 7, 6 };
            /**
             * 直接使用同一个数组方式
             */
            for (var i = 1; i < arr.Length; i++)
            {
                var get = arr[i];
                var left = 0;
                var right = i - 1;

                // 每次找出中间位置然后进行比较，最终确定索引位置
                while (left <= right)
                {
                    var mid = ((left + right) / 2);
                    if (arr[mid] > get)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }

                for (var k = i - 1; k >= left; k--)
                {
                    arr[k + 1] = arr[k];
                }

                arr[left] = get;
                Console.Write("arr:");
                for (int j = 0; j < arr.Length; j++)
                {
                    Console.Write(arr[j] + ",");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 冒泡排序
        /// n(n-1)/2
        /// </summary>
        public void BubbleSort()
        {
            int temp = 0;
            int[] arr = { 1, 4, 5, 2, 3, 9, 0, 7, 6 };
            Console.WriteLine("arr：");
            foreach (int item in arr)
            {
                Console.Write(item + ",");
            }
            Console.WriteLine();
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        temp = arr[j + 1];
                        arr[j + 1] = arr[j];
                        arr[j] = temp;
                    }
                }
            }
            Console.WriteLine("arr：");
            foreach (int item in arr)
            {
                Console.Write(item + ",");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
