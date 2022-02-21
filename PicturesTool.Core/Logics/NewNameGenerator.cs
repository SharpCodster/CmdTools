using System;

namespace PicturesTool.Core.Logics
{
    public static class NewNameGenerator
    {
        private static Random _rand;
        private static Random Rand
        {
            get
            {
                if (_rand == null)
                {
                    _rand = new Random(DateTime.Now.Second + DateTime.Now.Minute);
                }
                return _rand;
            }
        }



        public static string GetNewFileName(DateTime captured, string suffix, string extension)
        {
            int milliSec = Rand.Next(0, 100);

            if (milliSec == 100)
            {
                milliSec = 99;
            }

            string newFileName = $"{captured.ToString("yyyyMMdd_HHmmss")}{milliSec.ToString("00")}_{suffix}{extension.ToLower()}";

            return newFileName;
        }

    }
}
