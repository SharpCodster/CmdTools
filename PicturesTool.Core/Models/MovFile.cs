using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace PicturesTool.Core.Models
{
    public sealed class MovFile : GenericFile
    {
        public MovFile(FileInfo file) : base(file) { }

        public override string Suffix => "VID";


        public override DateTime GetCaptureDate()
        {
            DateTime minFileDate = new DateTime(Math.Min(File.LastWriteTime.Ticks, File.CreationTime.Ticks));
            return minFileDate;
        }

        protected override void ReadTgas()
        {
        }

        public override void SetNewCaptureDate(DateTime date)
        {
            File.LastWriteTime = date;
        }
    }
}
