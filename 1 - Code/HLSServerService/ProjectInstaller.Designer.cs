namespace HLSServerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.HLSServerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.HLSServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // HLSServerServiceProcessInstaller
            // 
            this.HLSServerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.HLSServerServiceProcessInstaller.Password = null;
            this.HLSServerServiceProcessInstaller.Username = null;
            this.HLSServerServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.HLSServerServiceProcessInstaller_AfterInstall);
            // 
            // HLSServerServiceInstaller
            // 
            this.HLSServerServiceInstaller.ServiceName = "HLSServerService";
            this.HLSServerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.HLSServerServiceProcessInstaller,
            this.HLSServerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller HLSServerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller HLSServerServiceInstaller;
    }
}