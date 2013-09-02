using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.GZip;

namespace Update
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                var d = DateTime.Now;
                while (DateTime.Now.Subtract(d).TotalSeconds < 10) Application.DoEvents();
            }
            catch { }
        }
    }
}
