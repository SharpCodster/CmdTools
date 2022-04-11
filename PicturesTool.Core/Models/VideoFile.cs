using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TagLib;
using TagLib.Image;

namespace PicturesTool.Core.Models
{
    public sealed class VideoFile : GenericFile
    {
        public VideoFile(FileInfo file) : base(file) { }

        public override string Suffix => "VID";

        private Tag _tags;

        public override DateTime GetCaptureDate()
        {
            return (DateTime)_tags.DateTagged;
        }

        protected override void ReadTgas()
        {
            using (var fileTag = TagLib.File.Create(File.FullName))
            {
                _tags = fileTag.Tag;

                if (_tags.TagTypes == TagTypes.Apple && _tags.DateTagged != null)
                {
                    DateTime date = (DateTime)_tags.DateTagged;

                    //date = new DateTime(2016, 9, 10, date.Hour, date.Minute, date.Second);
                    //_tags.DateTagged = date;
                    //fileTag.Save();
                    if (date != File.LastWriteTime)
                    {
                        File.LastWriteTime = date;
                    }

                    if (date != File.CreationTime)
                    {
                        File.CreationTime = date;
                    }

                    File.Refresh();
                }

                if (_tags.DateTagged == null)
                {
                    DateTime minFileDate = new DateTime(Math.Min(File.LastWriteTime.Ticks, File.CreationTime.Ticks));
                    DateTime fileNamedate = DateTime.Now;

                    if (File.Name.StartsWith("WP_"))
                    {
                        int year = Convert.ToInt32(File.Name.Substring(3, 4));
                        int month = Convert.ToInt32(File.Name.Substring(7, 2));
                        int day = Convert.ToInt32(File.Name.Substring(9, 2));

                        fileNamedate = new DateTime(year, month, day);
                    }
                    else if (File.Name.StartsWith("VID-"))
                    {
                        int year = Convert.ToInt32(File.Name.Substring(4, 4));
                        int month = Convert.ToInt32(File.Name.Substring(8, 2));
                        int day = Convert.ToInt32(File.Name.Substring(10, 2));

                        fileNamedate = new DateTime(year, month, day);
                    } 
                    else if (File.Name.StartsWith("VID_"))
                    {
                        int year = Convert.ToInt32(File.Name.Substring(4, 4));
                        int month = Convert.ToInt32(File.Name.Substring(8, 2));
                        int day = Convert.ToInt32(File.Name.Substring(10, 2));

                        int hour = Convert.ToInt32(File.Name.Substring(13, 2));
                        int min = Convert.ToInt32(File.Name.Substring(15, 2));
                        int sec = Convert.ToInt32(File.Name.Substring(17, 2));

                        fileNamedate = new DateTime(year, month, day, hour, min, sec);
                    }
                    

                    if (fileNamedate < minFileDate)
                    {
                        minFileDate = new DateTime(fileNamedate.Year, fileNamedate.Month, fileNamedate.Day, minFileDate.Hour, minFileDate.Minute, minFileDate.Second);
                    }

                    _tags.DateTagged = minFileDate;

                    if (fileTag.Writeable)
                    {
                        fileTag.Save();
                        File.Refresh();
                        File.CreationTime = minFileDate;
                        File.LastWriteTime = minFileDate;
                        File.Refresh();
                    }
                    else
                    {
                        int i = 0;
                    }
                }
            }
        }

        public override void SetNewCaptureDate(DateTime date)
        {
            using (var fileTag = TagLib.File.Create(File.FullName))
            {
                _tags = fileTag.Tag;
                _tags.DateTagged = date;

                if (!fileTag.PossiblyCorrupt)
                {
                    fileTag.Save();
                }

                File.CreationTime = date;
                File.LastWriteTime = date;
                File.Refresh();
            }
        }
    }
}
