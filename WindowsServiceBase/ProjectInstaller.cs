using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsServiceBase
{
    [RunInstaller(true)]
    public partial class ProjectInstallerEtiquetadoError : System.Configuration.Install.Installer
    {
        public ProjectInstallerEtiquetadoError()
        {
            InitializeComponent();
        }
    }
}
