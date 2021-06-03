using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
{
    public class BackgroundThread
    {

        private DateTime _lastActivity = DateTime.Now;

        private IDbAccess _db = null;

        public BackgroundThread(IDbAccess db)
        {
            _db = db;
        }

        public void Touch()
        {
            _lastActivity = DateTime.Now;
        }
    }
}
