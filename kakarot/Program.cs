namespace kakarot
{
    internal class Program : Form
    {
      
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
         static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            using (var splash = new Splash())
            {
                Application.Run(splash);
            }
            Application.Run(new fjntr());
        }
    }
}