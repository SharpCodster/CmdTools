using PicturesTool.Core.Logics;
using System;
using System.IO;
using TagLib.Image;
using System.Linq;

namespace PicturesTool.Core.Models
{
    public abstract class GenericFile
    {
        /*
            Video:".mkv", ".ogv", ".avi",". wmv", ".asf", ".mp4", ".m4p", ".m4v", ".mpeg", ".mpg", ".mpe", ".mpv", ".mpg", ".m2v"
            Audio: aa, aax, aac, aiff, ape, dsf, flac, m4a, m4b, m4p, mp3, mpc, mpp, ogg, oga, wav, wma, wv, webm
            Images: bmp, gif, jpeg, pbm, pgm, ppm, pnm, pcx, png, tiff, dng, svg
         */

        public static string[] ImgExtension = { ".bmp", ".gif", ".jpg", ".jpeg", ".pbm", ".pgm", ".ppm", ".pnm", ".pcx", ".png", ".tiff", ".dng", ".svg" };
        public static string[] VidExtension = { ".mkv", ".ogv", ".avi", ". wmv", ".asf", ".mp4", ".m4p", ".m4v", ".mpeg", ".mpg", ".mpe", ".mpv", ".mpg", ".m2v" };
        public static string[] OtherExtension = { ".mov" };

        public static GenericFile GetFile(FileInfo file)
        {
            if (ImgExtension.Contains(file.Extension.ToLower()))
            {
                return new ImageFile(file);
            }
            else if (VidExtension.Contains(file.Extension.ToLower()))
            {
                return new VideoFile(file);
            }
            else if (string.Equals(file.Extension, ".mov", StringComparison.OrdinalIgnoreCase))
            {
                return new MovFile(file);
            }
            else
            {
                return new NoTagFile(file);
            }
        }

        public abstract string Suffix { get; }

        public FileInfo File { get; }


        private string _newName;
        public string NewName
        {
            get
            {
                if (String.IsNullOrEmpty(_newName))
                {
                    _newName = GenerateNewName();
                }
                return _newName;
            }
        }

        public GenericFile(FileInfo file)
        {
            File = file;
            ReadTgas();
        }

        protected abstract void ReadTgas();
        public abstract DateTime GetCaptureDate();
        public abstract void SetNewCaptureDate(DateTime date);

        public virtual void Rename()
        {
            string startString = NewName.Substring(0, 15);
            string endString = NewName.Substring(NewName.Length - (4 + File.Extension.Length), 4 + File.Extension.Length);

            if (!File.Name.StartsWith(startString) 
                || !File.Name.EndsWith(endString))
            {
                while (System.IO.File.Exists(Path.Combine(File.DirectoryName, _newName)))
                {
                    _newName = GenerateNewName();
                }
                File.MoveTo(Path.Combine(File.DirectoryName, _newName));
            }
        }


        private string GenerateNewName()
        {
            DateTime captureDate = GetCaptureDate();
            string newName = NewNameGenerator.GetNewFileName(captureDate, Suffix, File.Extension);
            return newName;
        }
    }
}
