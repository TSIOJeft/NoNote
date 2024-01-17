using System;
using System.Windows;
using NoNote.config;

namespace NoNote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            ConfigUtil.myFolder = appDir;
            ConfigUtil.configArray = new ConfigUtil().getConfig();
        }
    }
}