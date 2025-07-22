using System.Security.Cryptography;
using System;
using System.Linq;
using WixSharp;
using System.Text.RegularExpressions;
using WixToolset.Dtf.WindowsInstaller;

namespace LotComPrinterSetup
{

    // block not needed when passing a version number to cli.
    // /// <summary>
    // /// Provides constant environment variables to the packager.
    // /// </summary>
    // class Constants
    // {
    //     public const string ProgramVersion = "1.0.0";
    // }

    /// <summary>
    /// Packages the LotCom Printer application's generated release files into an MSI package.
    /// </summary>
    public partial class Program
    {
        /// <summary>
        /// Accepts the command line arguments args and tests the first as a valid SemVer format Version Number.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool CheckVersionNumber(string[] args)
        {
            // confirm a version is passed in args
            if (args is null || args.Length < 1 || args[0] is null)
            {
                Console.WriteLine("No version number provided; exiting setup.");
                return false;
            }
            // confirm the passed argument is a valid SemVer version number (1.0.2, 0.3.01.21, etc)
            else
            {
                string VersionNumber = args[0];
                Regex SemVer = new Regex(@"[0-9]+.[0-9]+.[0-9]+");
                if (!SemVer.IsMatch(VersionNumber))
                {
                    Console.WriteLine($"'{VersionNumber}' is not a valid version number; exiting setup.");
                    return false;
                }
            }
            // first arg in passed args[] was a valid version number
            return true;
        }

        /// <summary>
        /// Generates a new GUID (unique product ID) from input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static Guid GenerateProductId(string input)
        {
            // create a new SHA256 hash and compute its initial hash value
            SHA256 Sha256 = SHA256.Create();
            byte[] HashBytes = Sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            // ensure the byte array is exactly 16 bytes long, as required for a GUID.
            // SHA256 generates 32 bytes, so we take the first 16 bytes.
            byte[] GuidBytes = new byte[16];
            Array.Copy(HashBytes, GuidBytes, 16);
            // Construct the GUID from the 16-byte array.
            return new Guid(GuidBytes);
        }

        /// <summary>
        /// Creates a new Wix Project and builds an MSI from that Project.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // return; // REMOVE THIS LINE TO ENABLE BUILDING
            // check and capture the version number from cli
            string VersionNumber;
            if (CheckVersionNumber(args))
            {
                VersionNumber = args[0];
            }
            else
            {
                return;
            }
            Environment.CurrentDirectory = @"D:\a\LotCom-printer\LotCom-printer\LotComPrinter\bin\Release\net9.0-windows10.0.19041.0\win10-x64";  // setup project script home
            Environment.SetEnvironmentVariable("LATEST_RELEASE", VersionNumber);
            // generates a new GUID for the specific version installation
            Guid ProductId = GenerateProductId($"LotCom Printer {VersionNumber}");  // Do not change
            // creates a new WiX# project for the LotComPrinter application
            Project LotComPrinterSetup = new Project
            (
                "LotComPrinterSetup",   // WiX# project name
                new Dir
                (
                    @"C:\ProgramData\Yamada North America\LotCom Printer", // directory of the new WiX# Project
                    new Files(@"*.*")  // includes all Latest Release files in new WiX# project's folder
                ),
                new CloseApplication
                (
                    new Id($"LotComPrinter_{VersionNumber}"),   // WiX# Id that "stamps" the XML files in the project MSI
                    "LotCoMPrinter.exe",    // targeted .exe file
                    false,                  // show a close message
                    false                   // do not prompt a reboot
                )
                {
                    Timeout = 15    // set the CloseApplication action's timeout to 15 seconds
                }
            );
            // set the Project's properties
            LotComPrinterSetup.Name = "LotCom Printer";
            LotComPrinterSetup.ProductId = ProductId;
            LotComPrinterSetup.UpgradeCode = new Guid("B8922687-A74A-45D4-96AA-17C91224DB7A"); // Do not change
            LotComPrinterSetup.Version = new Version(VersionNumber);
            LotComPrinterSetup.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
            // LotComPrinterSetup.GUID = new Guid("99bcb1fa-e3c6-4ba7-b08e-1ea0ce5e4fda");
            // LotComPrinterSetup.LicenceFile = @".\License.rtf";
            LotComPrinterSetup.ControlPanelInfo.Comments = "LotCom Printer Application";
            // LotComPrinterSetup.ControlPanelInfo.Readme = "https://github.com/LotCoM/LotCoM-printer/blob/stable/README.md";
            // LotComPrinterSetup.ControlPanelInfo.HelpLink = "https://github.com/LotCoM/LotCoM-printer/blob/stable/README.md";
            LotComPrinterSetup.ControlPanelInfo.HelpTelephone = "(937) 260-9790";
            LotComPrinterSetup.ControlPanelInfo.UrlInfoAbout = "https://github.com/LotCoM/LotCoM-printer/blob/stable/README.md";
            // LotComPrinterSetup.ControlPanelInfo.UrlUpdateInfo = "https://github.com/oleg-shilo/wixsharp/update";
            LotComPrinterSetup.ControlPanelInfo.ProductIcon = @"lotcom_logo.scale-100.png";
            LotComPrinterSetup.ControlPanelInfo.Contact = "YNA IT";
            LotComPrinterSetup.ControlPanelInfo.Manufacturer = "Yamada North America";
            LotComPrinterSetup.ControlPanelInfo.InstallLocation = "[INSTALLDIR]";
            LotComPrinterSetup.ControlPanelInfo.NoModify = true;
            LotComPrinterSetup.ControlPanelInfo.NoRepair = true;
            // LotComPrinterSetup.ControlPanelInfo.NoRemove = true;
            // LotComPrinterSetup.ControlPanelInfo.SystemComponent = true; //if set will not be shown in Control Panel
            LotComPrinterSetup.SourceBaseDir = Environment.CurrentDirectory;
            LotComPrinterSetup.OutDir = @".\Installer";
            LotComPrinterSetup.OutFileName = $"LotComPrinter_{LotComPrinterSetup.Version}";
            LotComPrinterSetup.UI = WUI.WixUI_Minimal;
            LotComPrinterSetup.ResolveWildCards();
            // get the .exe file
            WixSharp.File ExeFile = LotComPrinterSetup.AllFiles.Single(x => x.Name.EndsWith("LotCoMPrinter.exe"));
            // set the .exe file shortcut wildcards
            ExeFile.Shortcuts = new[]
            {
                new FileShortcut("LotCom Printer", @"%StartMenuFolder%"),
                new FileShortcut("LotCom Printer", @"%Desktop%")
            };
            // build the MSI package from the Setup Project
            LotComPrinterSetup.BuildMsi();
        }
    }
}