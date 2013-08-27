using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace chatroom.client.console
{
    class NumberDllLoadTester
    {
        [DllImport("D:\\project\\chatroom\\dll\\number\\Project.dll")]
        public static extern int func(int a);

    }
    class StringTypeTester
    {
        [DllImport("D:\\project\\chatroom\\dll\\string\\Project.dll")]
        public static extern string conj(string a, string b);
    }
    class Program
    {
        static void Main(string[] args)
        {
            string a = "12345";
            string b = "678910";
            Console.WriteLine(StringTypeTester.conj(a,b));
            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.ReadKey();
        }
    }
}
