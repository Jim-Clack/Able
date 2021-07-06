using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Db
{
    public interface IAccount
    {
        string OnlineBankingAcct { get; set; }
        string OnlineBankingUrl { get; set; }
        string OnlineBankingUser { get; set; }
        string OnlineBankingPwd { get; set; }
        bool OnlineBankingLive { get; set; }
        bool OnlineBankingAggressive { get; set; }
    }
}
