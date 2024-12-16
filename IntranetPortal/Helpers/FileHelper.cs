using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Helpers
{
    public static class FileHelper
    {
        public static bool IsFileOpen(this FileInfo f)
        {
            FileStream stream = null;
            try
            {
                stream = f.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return false;
        }
    }
}
