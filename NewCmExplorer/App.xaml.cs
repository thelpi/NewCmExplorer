using System.Windows;

namespace NewCmExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// <seealso cref="Application"/>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Gui.MainWindow().ShowDialog();
        }
    }
}
