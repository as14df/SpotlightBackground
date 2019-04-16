using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace SpotlightBackground
{
    class Program
    {
        // For setting wallpaper
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        static void Main(string[] args)
        {
            // Get base key
            RegistryKey HKLM64 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "DESKTOP-GPTMUFV", RegistryView.Registry64);
            RegistryKey key = HKLM64.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\Creative\\S-1-5-21-2017277926-4256666833-3962486793-1001");
          
            string[] subKeyNames = key.GetSubKeyNames();
            string destPath = "C:\\Users\\aless\\Pictures\\Hintergruende\\";

            if (subKeyNames.Length > 1)
            {
                // get folder of used image
                RegistryKey imgKey = key.OpenSubKey(subKeyNames[subKeyNames.Length - 1]);

                // Get image path, image id and destination path
                string imgPath = (string)imgKey.GetValue("landscapeImage");
                string imgID = (string)imgKey.GetValue("contentId");
                destPath +=  imgID + ".jpg";

                // Copy image to destination
                File.Copy(imgPath, destPath, true);
            }
            else
            {
                destPath += "default.png";
            }
            

            // Set image as Wallaper
            SystemParametersInfo(20, 0, destPath, 0x01 | 0x02);
        }
    }
}
