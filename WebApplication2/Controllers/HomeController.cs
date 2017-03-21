using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.Azure; // Namespace for Azure Configuration Manager
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage
using Microsoft.WindowsAzure.Storage.File; // Namespace for File storage
using System.Runtime.InteropServices;
using System.IO;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        [DllImport(@"BackupSystem.dll", EntryPoint = "GetCredit")]
        public static extern UInt64 GetCredit(string sPath);

        [DllImport(@"BackupSystem.dll", EntryPoint = "GetTotalBet")]
        public static extern UInt64 GetTotalBet(string sPath);

        [DllImport(@"BackupSystem.dll", EntryPoint = "GetTotalWon")]
        public static extern UInt64 GetTotalWon(string sPath);

        [DllImport(@"BackupSystem.dll", EntryPoint = "GetGamePlayed")]
        public static extern UInt32 GetGamePlayed(string sPath);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("backupsystem");

            // Ensure that the directory exists.
            if (share.Exists())
            {

                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("accounts");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference("d_accs_data_2way_0.bak");

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        // Write the contents of the file to the console window.
                        long n = file.Properties.Length;
                        byte[] data = new byte[n];
                        Console.WriteLine("Start downloading " + n + " bytes...");
                        file.BeginDownloadToByteArray(data, 0, new AsyncCallback(HandleAccsDownloadCallBack), data);
                    }
                }

                // Get a reference to the directory we created previously.
                sampleDir = rootDir.GetDirectoryReference("stats");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference("d_mach_stats_2way_0.bak");

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        // Write the contents of the file to the console window.
                        long n = file.Properties.Length;
                        byte[] data = new byte[n];
                        Console.WriteLine("Start downloading " + n + " bytes...");
                        file.BeginDownloadToByteArray(data, 0, new AsyncCallback(HandleStatsDownloadCallBack), data);
                    }
                }

                /*
                // Get a reference to the file we created previously.
                CloudFile file = sampleDir.GetFileReference("TestEnglish.txt"); 

                // Ensure that the file exists.
                if (file.Exists())
                {
                    // Write the contents of the file to the console window.
                    //Console.WriteLine(file.DownloadTextAsync().Result);
                    ViewBag.Message = file.DownloadTextAsync().Result;
                }*/
            }

            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void HandleAccsDownloadCallBack(IAsyncResult ar)
        {
            byte[] data = (byte[])ar.AsyncState;
            string sDownloadFolder = @"C:\\Users\\David\\Source\\Repos\\CloudStorageAPI\\CloudStorage\\CloudStorage\\bin\\Download\\";
            System.IO.File.WriteAllBytes(sDownloadFolder + "d_accs_data_2way_0.bak", data);

            UInt64 nCredit = GetCredit(sDownloadFolder);
            Console.WriteLine("Credit=" + nCredit);

            ViewBag.Message = "Credit=" + nCredit;

            View();
        }

        private void HandleStatsDownloadCallBack(IAsyncResult ar)
        {
            byte[] data = (byte[])ar.AsyncState;
            string sDownloadFolder = @"C:\\Users\\David\\Source\\Repos\\CloudStorageAPI\\CloudStorage\\CloudStorage\\bin\\Download\\";
            System.IO.File.WriteAllBytes(sDownloadFolder + "d_mach_stats_2way_0.bak", data);

            UInt64 nBet = GetTotalBet(sDownloadFolder);
            UInt64 nWon = GetTotalBet(sDownloadFolder);
            UInt32 nGamePlayed = GetGamePlayed(sDownloadFolder);
            Console.WriteLine("GamePlayed=" + nGamePlayed + ", Bet=" + nBet + ", Won=" + nWon);
        }
    }
}