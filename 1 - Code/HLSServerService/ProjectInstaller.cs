using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace HLSServerService
{
    /// <summary> 
    /// Project Installer
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void HLSServerServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
        }
    }
}
