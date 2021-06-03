using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public static class Version
    {
        private static int _dbVer = 1;
        private static int _dbRev = 1;
        private static int _dbBld = 1;
        private static string _dbVersion = _dbVer + "." + _dbRev + "." + _dbBld;
        private static int _appVer = 1;
        private static int _appRev = 1;
        private static int _appBld = 1;
        private static string _appVersion = _appVer + "." + _appRev + "." + _appBld;

        public static int DbVer { get => _dbVer; }
        public static int DbRev { get => _dbRev; }
        public static int DbBld { get => _dbBld; }
        public static string DbVersion { get => _dbVersion; }
        public static int AppVer { get => _appVer; }
        public static int AppRev { get => _appRev; }
        public static int AppBld { get => _appBld; }
        public static string AppVersion { get => _appVersion; }
    }
}
