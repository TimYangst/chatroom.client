using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace chatroom.client.console
{
    class DllLoadTester
    {
        [DllImport("D:\\project\\chatroom\\dll\\Project.dll")]
        public static extern int func(int a);

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DllLoadTester.func(3));
            Console.ReadKey();
        }
    }
}
