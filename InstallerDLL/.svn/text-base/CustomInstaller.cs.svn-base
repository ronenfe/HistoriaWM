using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;
using System.IO;

namespace InstallerDLL
{
    [RunInstaller(true)]
    public partial class CustomInstaller : Installer
    {
        public CustomInstaller()
        {
            InitializeComponent();
            this.BeforeInstall += new InstallEventHandler(CustomInstaller_BeforeInstall);
            this.AfterInstall += new InstallEventHandler(CustomInstaller_AfterInstall);
            this.BeforeUninstall += new InstallEventHandler(CustomInstaller_BeforeUninstall);
        }
        private const string CEAPPMGR_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\CEAPPMGR.EXE";
        private const string ACTIVESYNC_INSTALL_PATH = @"SOFTWARE\Microsoft\Windows CE Services";
        private const string INSTALLED_DIR = "InstalledDir";
        private const string CEAPPMGR_EXE_FILE = @"CEAPPMGR.EXE";
        private const string CEAPPMGR_INI_FILE = @"Historia.ini";
        private const string APP_SUBDIR = @"\SBSH Historia";
        private string TEMP_PATH =  Environment.SystemDirectory + @"\TEMP\Historia";

        private string GetAppInstallDirectory()
        {
            // Get the ActiveSync install directory
            RegistryKey keyActiveSync =
                    Registry.LocalMachine.OpenSubKey(ACTIVESYNC_INSTALL_PATH);
            if (keyActiveSync == null)
            {
                throw new Exception("ActiveSync is not installed.");
            }
            // Build the target directory path under the ActiveSync folder
            string activeSyncPath = (string)keyActiveSync.GetValue(INSTALLED_DIR);
            string installPath = activeSyncPath + APP_SUBDIR;
            keyActiveSync.Close();
            return installPath;
        }

        void CustomInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            // Find the location where the application will be installed
            string installPath = GetAppInstallDirectory();
            // Create the target directory
            Directory.CreateDirectory(installPath);
            // Copy your application files to the directory
            foreach (string installFile in Directory.GetFiles(TEMP_PATH))
            {
                File.Copy(installFile, Path.Combine(installPath,
                Path.GetFileName(installFile)), true);
            }
            // Get the path to ceappmgr.exe
            RegistryKey keyAppMgr =
                    Registry.LocalMachine.OpenSubKey(CEAPPMGR_PATH);
            string appMgrPath = (string)keyAppMgr.GetValue(null);
            keyAppMgr.Close();
            // Run CeAppMgr.exe to install the files to the device
            System.Diagnostics.Process.Start(appMgrPath,"\"" + Path.Combine(installPath, CEAPPMGR_INI_FILE) + "\"");
        }

        void CustomInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            // Delete the temp files
            foreach (string tempFile in Directory.GetFiles(TEMP_PATH))
            {
                File.Delete(tempFile);
            }
        }

        void CustomInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            // Find where the application is installed
            string installPath = GetAppInstallDirectory();
            // Delete the files
            foreach (string appFile in Directory.GetFiles(installPath))
            {
                File.Delete(appFile);
            }
            // Delete the folder
            Directory.Delete(installPath);
        }
    }
}