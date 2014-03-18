namespace HLSServerService
{
    /// <summary> 
    /// Installer Design
    /// </summary>
    public partial class ProjectInstaller
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
            this.hlsServerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.hlsServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();

            // HLSServerServiceProcessInstaller
            this.hlsServerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.hlsServerServiceProcessInstaller.Password = null;
            this.hlsServerServiceProcessInstaller.Username = null;
            this.hlsServerServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.HLSServerServiceProcessInstaller_AfterInstall);

            // HLSServerServiceInstaller
            this.hlsServerServiceInstaller.ServiceName = "HLSServerService";
            this.hlsServerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // ProjectInstaller
            this.Installers.AddRange(new System.Configuration.Install.Installer[] 
            {
                this.hlsServerServiceProcessInstaller,
                this.hlsServerServiceInstaller
            });
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller hlsServerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller hlsServerServiceInstaller;
    }
}