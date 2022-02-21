using PicturesTool.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PicturesTool.Core.Logics
{
    public static class Pictures
    {
        public static List<GenericFile> ReadFiles(DirectoryInfo dir, bool recursive = true)
        {
            List<GenericFile> res = new List<GenericFile>();

            foreach (FileInfo file in dir.GetFiles())
            {
                res.Add(GenericFile.GetFile(file));
            }

            if (recursive)
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    res.AddRange(ReadFiles(subdir));
                }
            }
            
            return res;
        }
    }
}
