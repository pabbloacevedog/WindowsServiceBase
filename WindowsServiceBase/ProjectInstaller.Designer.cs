namespace WindowsServiceBase
{
    partial class ProjectInstallerEtiquetadoError
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectInstallerEtiquetadoError));
            this.serviceProcessEtiquetadoLineaB = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceEtiquetadoLineaB = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessEtiquetadoLineaB
            // 
            this.serviceProcessEtiquetadoLineaB.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessEtiquetadoLineaB.Password = null;
            this.serviceProcessEtiquetadoLineaB.Username = null;
            // 
            // serviceEtiquetadoLineaB
            // 
            this.serviceEtiquetadoLineaB.Description = resources.GetString("serviceEtiquetadoLineaB.Description");
            this.serviceEtiquetadoLineaB.DisplayName = "SERVICES WindowsServiceBase Linea B";
            this.serviceEtiquetadoLineaB.ServiceName = "WindowsServiceBaseLineaB";
            this.serviceEtiquetadoLineaB.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstallerEtiquetadoError
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessEtiquetadoLineaB,
            this.serviceEtiquetadoLineaB});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessEtiquetadoLineaB;
        private System.ServiceProcess.ServiceInstaller serviceEtiquetadoLineaB;
    }
}