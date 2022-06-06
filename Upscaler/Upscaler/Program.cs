using ImageGeneration;

namespace Upscaler
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Generator generator = new Generator(@"D:\E Stuff\HDA_SS_22\Computer_Vision\Prak_2\Upscaler\GeneratorFiles\GeneratorFile1.txt");
            generator.Start();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}