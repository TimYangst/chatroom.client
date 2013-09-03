using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace chatroom.client.Update
{
    public static class ZipUtils
    {


        public static void UnZip(string targetDir, string filepath)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(filepath));
            try
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = targetDir;
                    string fileName = Path.GetFileName(theEntry.Name);

                    //生成解压目录
                    if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = null;
                        try
                        {
                            //解压文件到指定的目录
                            if (File.Exists(targetDir + "\\" + fileName)) File.Delete(targetDir + "\\" + fileName);

                            streamWriter = File.Create(targetDir + "\\" + fileName);
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception fe)
                        {
                            Console.WriteLine(fe.StackTrace);

                        }
                        finally {
                            if  (streamWriter != null) streamWriter.Close();
                        }


                        
                    }
                }
                s.Close();
            }
            catch (Exception eu)
            {
                throw eu;
            }
            finally
            {
                s.Close();
            }
        }


        public static void UnZip(string filepath)
        {
            UnZip(System.IO.Directory.GetCurrentDirectory(), filepath);
        }

    }
}
