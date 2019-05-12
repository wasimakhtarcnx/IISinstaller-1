using System;
using System.IO;
using System.Linq;
using Microsoft.Web.Administration;

namespace IISinstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerManager server = new ServerManager();

            ApplicationPool myApplicationPool = null;

            //we will create a new ApplicationPool named 'MyApplicationPool'
            //we will first check to make sure that this pool does not already exist
            //since the ApplicationPools property is a collection, we can use the Linq FirstOrDefault method
            //to check for its existence by name
            if (server.ApplicationPools != null && server.ApplicationPools.Count > 0)
            {
                if (server.ApplicationPools.Where(p => p.Name == "sms.local").Count()>0)
                {
                    //if we find the pool already there, we will get a referecne to it for update
                    myApplicationPool = server.ApplicationPools.FirstOrDefault(p => p.Name == "sms.local");
                }
                else
                {
                    //if the pool is not already there we will create it
                    myApplicationPool = server.ApplicationPools.Add("sms.local");
                }
            }
            else
            {
                //if the pool is not already there we will create it
                myApplicationPool = server.ApplicationPools.Add("sms.local");
            }

            if (myApplicationPool != null)
            {
                //for this sample, we will set the pool to run under the NetworkService identity
                myApplicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;

                //we set the runtime version
                myApplicationPool.ManagedRuntimeVersion = "No Managed Code";

                //we save our new ApplicationPool!
                server.CommitChanges();
            }
            //Create W
            if (server.Sites != null && server.Sites.Count > 0)
            {
                //we will first check to make sure that the site isn't already there
                if (server.Sites.FirstOrDefault(s => s.Name == "ncstcsmsoffline") == null)
                {
                    //we will just pick an arbitrary location for the site
                    string path = @"C:\inetpub\wwwroot\ncstcsmslocal\";

                    //we must specify the Binding information
                    string ip = "*";
                    string port = "80";
                    string hostName = "*";

                    string bindingInfo = string.Format(@"{0}:{1}:{2}", ip, port, hostName);

                    //add the new Site to the Sites collection
                    Site site = server.Sites.Add("ncstcsmsoffline", "http", bindingInfo, path);

                    //set the ApplicationPool for the new Site
                    site.ApplicationDefaults.ApplicationPoolName = myApplicationPool.Name;

                    //save the new Site!
                    server.CommitChanges();
                }
            }
            string sourceDirectory = @"C:\Users\WasimSana\source\repos\SchoolManagementSystemTool\SchoolManagementSystemTool\bin\Release\netcoreapp2.2\";
            string targetDirectory = @"C:\inetpub\wwwroot\ncstcsmslocal\";

            Copy(sourceDirectory, targetDirectory);

        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
