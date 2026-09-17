namespace Bomber.Client;

static class Startup
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Game());
    }    
}