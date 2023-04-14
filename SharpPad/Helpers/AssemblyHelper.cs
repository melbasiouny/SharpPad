namespace SharpPad.Helpers;

internal class AssemblyHelper
{
    public static string Version
    {
        get
        {
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
