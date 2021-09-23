using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SpotlightBackground
{
    class Program
    {
        /// <summary>
        /// For registry access
        /// </summary>
        /// <param name="uAction"></param>
        /// <param name="uParam"></param>
        /// <param name="lpvParam"></param>
        /// <param name="fuWinIni"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        static void Main(string[] args)
        {
            // Get base key
            var userId = WindowsIdentity.GetCurrent().User.Value;
            var HKLM64 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, Environment.MachineName, RegistryView.Registry64);
            var key = HKLM64.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Creative\{userId}");

            var subKeyNames = key.GetSubKeyNames().ToList();
            var destinationPath = $@"{Directory.GetCurrentDirectory()}\BackgroundImages\";

            if (subKeyNames.Any())
            {
                // get folder of used image
                var imageKey = key.OpenSubKey(subKeyNames.Last());

                // Get image path, image id and destination path
                var imagePath = imageKey.GetValue("landscapeImage").ToString();
                var imageId = imageKey.GetValue("contentId").ToString();
                destinationPath +=  $"{imageId}.jpg";

                // Copy image to destination
                File.Copy(imagePath, destinationPath, true);
            }
            else
            {
                destinationPath += "default.png";
            }
            
            // Set image as Wallaper
            SystemParametersInfo(20, 0, destinationPath, 0x01 | 0x02);
        }
    }
}
