using System;
using System.Reflection;
using System.Windows.Forms;

namespace RemoteCLI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string resource1 = "RemoteCLI.websocket-sharp.dll";
            string resource2 = "RemoteCLI.Newtonsoft.Json.dll";
            EmbeddedAssembly.Load(resource1, "websocket-sharp.dll");
            EmbeddedAssembly.Load(resource2, "Newtonsoft.Json.dll");
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
