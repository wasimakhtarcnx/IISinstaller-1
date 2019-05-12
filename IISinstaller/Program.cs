using System;
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
                if (server.ApplicationPools.Where(p => p.Name == "MyApplicationPool").Count()>0)
                {
                    //if we find the pool already there, we will get a referecne to it for update
                    myApplicationPool = server.ApplicationPools.FirstOrDefault(p => p.Name == "MyApplicationPool");
                }
                else
                {
                    //if the pool is not already there we will create it
                    myApplicationPool = server.ApplicationPools.Add("MyApplicationPool");
                }
            }
            else
            {
                //if the pool is not already there we will create it
                myApplicationPool = server.ApplicationPools.Add("MyApplicationPool");
            }

            if (myApplicationPool != null)
            {
                //for this sample, we will set the pool to run under the NetworkService identity
                myApplicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;

                //we set the runtime version
                myApplicationPool.ManagedRuntimeVersion = "v4.0";

                //we save our new ApplicationPool!
                server.CommitChanges();
            }
            //Create W
            if (server.Sites != null && server.Sites.Count > 0)
            {
                //we will first check to make sure that the site isn't already there
                if (server.Sites.FirstOrDefault(s => s.Name == "MySite") == null)
                {
                    //we will just pick an arbitrary location for the site
                    string path = @"c:\MySiteFolder\";

                    //we must specify the Binding information
                    string ip = "*";
                    string port = "80";
                    string hostName = "*";

                    string bindingInfo = string.Format(@"{0}:{1}:{2}", ip, port, hostName);

                    //add the new Site to the Sites collection
                    Site site = server.Sites.Add("MySite", "http", bindingInfo, path);

                    //set the ApplicationPool for the new Site
                    site.ApplicationDefaults.ApplicationPoolName = myApplicationPool.Name;

                    //save the new Site!
                    server.CommitChanges();
                }
            }

        }
    }
}
