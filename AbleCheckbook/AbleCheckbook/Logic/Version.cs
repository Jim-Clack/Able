using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public static class Version
    {
        private static int _dbMajor = 1;
        private static int _dbMinor = 1;
        private static int _dbRevis = 1;
        private static int _dbBuild = 1;
        private static string _dbVersion = _dbMajor + "." + _dbMinor + "." + _dbRevis + "(" + _dbBuild + ")";
        private static int _appMajor = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
        private static int _appMinor = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
        private static int _appRevis = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;
        private static int _appBuild = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
        private static string _appVersion = _appMajor + "." + _appMinor + "." + _appRevis + "(" + _appBuild + ")";

        public static int DbMajor { get => _dbMajor; }
        public static int DbMinor { get => _dbMinor; }
        public static int DbRevis { get => _dbRevis; }
        public static int DbBuild { get => _dbBuild; }
        public static string DbVersion { get => _dbVersion; }
        public static int AppMajor { get => _appMajor; }
        public static int AppMinor { get => _appMinor; }
        public static int AppRevis { get => _appRevis; }
        public static int AppBuild { get => _appBuild; }
        public static string AppVersion { get => _appVersion; }
    }
}
