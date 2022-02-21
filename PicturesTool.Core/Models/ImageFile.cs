using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TagLib.Image;

namespace PicturesTool.Core.Models
{
    public class ImageFile : GenericFile
    {
        public ImageFile(FileInfo file) : base(file) { }

        public override string Suffix => "IMG";

        private CombinedImageTag _tags;

        public override DateTime GetCaptureDate()
        {
            return (DateTime)_tags.DateTime;
        }

        protected override void ReadTgas()
        {
            using (var fileTag = TagLib.File.Create(File.FullName))
            {
                _tags = fileTag.Tag as TagLib.Image.CombinedImageTag;
                
                if (_tags.DateTime == null)
                {
                    DateTime minFileDate = new DateTime(Math.Min(File.LastWriteTime.Ticks, File.CreationTime.Ticks));

                    if (File.Name.StartsWith("WhatsApp Image "))
                    {
                        // "WhatsApp Image 2020-05-30 at 19.36.38.jpeg"
                        int year = Convert.ToInt32(File.Name.Substring(15, 4));
                        int month = Convert.ToInt32(File.Name.Substring(20, 2));
                        int day = Convert.ToInt32(File.Name.Substring(23, 2));

                        int hour = Convert.ToInt32(File.Name.Substring(29, 2));
                        int min = Convert.ToInt32(File.Name.Substring(32, 2));
                        int sec = Convert.ToInt32(File.Name.Substring(35, 2));

                        minFileDate = new DateTime(year, month, day, hour, min, sec);
                    }

                    if (_tags.Exif == null)
                    {
                        var exif = (TagLib.IFD.IFDTag)fileTag.GetTag(TagLib.TagTypes.TiffIFD, true);
                        exif.DateTimeDigitized = minFileDate;
                        exif.DateTime = minFileDate;
                    }
                    else
                    {
                        _tags.DateTime = minFileDate;
                    }

                    if (!fileTag.PossiblyCorrupt)
                    {
                        fileTag.Save();
                        File.Refresh();
                        File.LastWriteTime = (DateTime)_tags.DateTime;
                    }
                }
            }
        }

        public override void SetNewCaptureDate(DateTime date)
        {
            using (var fileTag = TagLib.File.Create(File.FullName))
            {
                _tags = fileTag.Tag as TagLib.Image.CombinedImageTag;

                _tags.DateTime = date;
                _tags.Exif.DateTimeDigitized = date;
                File.LastWriteTime = date;

                if (!fileTag.PossiblyCorrupt)
                {
                    fileTag.Save();
                }
            }
        }
    }
}
