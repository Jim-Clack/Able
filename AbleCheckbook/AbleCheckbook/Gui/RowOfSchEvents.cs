using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
{
    public class RowOfSchEvents : IComparable
    {

        private ScheduledEvent _schEvent = null;

        private ScheduledEvent _schEventBeforeEdit = null;

        public RowOfSchEvents(ScheduledEvent schEvent)
        {
            _schEventBeforeEdit = schEvent;
            _schEvent = schEvent.Clone();
        }

        public ScheduledEvent GetScheduledEvent()
        {
            return _schEvent;
        }

        public ScheduledEvent GetScheduledEventBeforeEdit()
        {
            return _schEventBeforeEdit;
        }

        public int CompareTo(object obj)
        {
            return this.Payee.CompareTo(((RowOfSchEvents)obj).Payee);
        }

        public Guid Id
        {
            get
            {
                return _schEvent.Id;
            }
            set
            {
                // do nothing
            }
        }

        public String Payee
        {
            get
            {
                return _schEvent.Payee;
            }
            set
            {
                // do nothing
            }
        }

        public String Memo
        {
            get
            {
                return _schEvent.Memo;
            }
            set
            {
                // do nothing
            }
        }

        public string Due
        {
            get
            {
                return _schEvent.DueDescription;
            }
            set
            {
                // do nothing
            }
        }

        public string Amount
        {
            get
            {
                return UtilityMethods.FormatCurrency(_schEvent.Amount);
            }
            set
            {
                // do nothing
            }
        }

        public string Status
        {
            get
            {
                return Strings.Get(_schEvent.GetRepeatCount(DateTime.Now.Date) < 1 ? "Expired" : "Active");
            }
            set
            {
                // Do nothing.
            }
        }

        public string LastPosting
        {
            get
            {
                return _schEvent.LastPosting.ToShortDateString();
            }
            set
            {
                // do nothing
            }
        }

    }

}
