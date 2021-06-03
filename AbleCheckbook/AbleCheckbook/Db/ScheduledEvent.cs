
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AbleCheckbook.Db
{

    /// <summary>
    /// What is the repeat period of the event?
    /// </summary>
    public enum SchedulePeriod
    { 
        Weekly = 0,
        BiWeekly = 1,
        Monthly = 2,
        MonthlySsa = 3,
        Annually = 4,
    }

    /// <summary>
    /// This triggers a scheduled automatic entry or a reminder
    /// </summary>
    /// <remarks>
    /// Indexes per SetDueXxx() methods:
    ///   DayOfWeek    0-based
    ///   DayOfMonth   1-based (actual day number, like 1 for the first)
    ///   MonthOfYear  1-based (Jan = 1)
    ///   WeekOfMonth  0-based
    ///   Year         1-based (actual year number, like 2031)
    ///  Note that these are what you would expect and are consistent with
    ///  normal use. But it's important to refer to this often because the
    ///  mix of 0-based and 1-based indexes can be confusing. 
    /// INTERNAL INDEXES:
    ///   Note that the .NET DateTime object, the user interface, and the 
    ///   SetDue methods herein use some one-based counts, whereas the fields
    ///   (_dayOfXxx, etc.) are all zero-based. Bitmasks are zero-based.
    ///   The DayOfWeek enum .NET and herein is zero-based, starting Sunday.
    /// BITMASKS:
    ///   Days of the week and days of the month are stored as bitmasks so
    ///   that we can (in the future) support events that occur multiple times
    ///   per week or per month. Currently that is such a rare need that it
    ///   is not implemented but can be accomplished by using multiple events.
    /// DANGER DANGER DANGER: 
    ///   Do not use setters for the due date settings (_dayOfXxx, etc.), only
    ///   the SetXxx methods, as those public setters/getters are there for
    ///   .NET serialization and should not be set independently.
    /// USAGE:
    ///   ...Initialization...
    ///    1. Ctor();
    ///    2. this.Payee, Category, TransKind, etc.
    ///    3. this.SetDueXxxx2(...);
    ///    4. this.SetRepeatCount(...) // optionally
    ///   ...Querying...
    ///    6. DateTime dueDate = this.DueXxx;
    ///   ...Application...
    ///    8. Xxx3 = this.Payee, Category, TransKind, etc.
    ///    9. this.LastPosting = Xxx4;
    /// </remarks>
    public class ScheduledEvent
    {

        /// <summary>
        /// Our placeholder for "way-off-in-the-future".
        /// </summary>
        public static DateTime Eternity = new DateTime(2100, 12, 31);

        /// <summary>
        /// What is the maximum allowable number of occurrences of an event?
        /// </summary>
        public const int MaxOccurrences = 1200;

        /// <summary>
        /// How many ticks in a week?
        /// </summary>
        public static long TicksPerWeek = new DateTime(2100, 4, 8).Ticks - new DateTime(2100, 4, 1).Ticks;

        /// <summary>
        /// How many ticks in a day?
        /// </summary>
        public static long TicksPerDay = new DateTime(2100, 4, 2).Ticks - new DateTime(2100, 4, 1).Ticks;

        /// <summary>
        /// This uniquely identifies a scheduled event.
        /// </summary>
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Is this just a reminder (highlight as such) or is it an automated entry?
        /// </summary>
        private bool _isReminder = true;

        /// <summary>
        /// Due alternate occurence only, such as for next-vs-this week?
        /// </summary>
        private bool _isOddWeeksOnly = true;

        /// <summary>
        /// When was the last time we scheduled this?
        /// </summary>
        private DateTime _lastPosting = new DateTime();

        /// <summary>
        /// How to plan the scheduling.
        /// </summary>
        private SchedulePeriod _period = SchedulePeriod.Monthly;

        /// <summary>
        /// Ending date, last occurrence. Ticks = 0 to deativate. 
        /// </summary>
        private DateTime _endingDate = new DateTime(0L);

        /// <summary>
        /// Is last payment different? 0 = no, it's the same
        /// </summary>
        private long _finalPaymentAmount = 0L;

        /// <summary>
        /// Future use.
        /// </summary>
        private bool _isEstimatedAmount = false;

        /// <summary>
        /// Memo to be written to scheduled entry.
        /// </summary>
        private string _memo = "";

        /// <summary>
        /// Days of month as a bitmask where bit 0 = 1st day, bit 32 = last day, whatever that might be.
        /// </summary>
        private long _dayOfMonthBits = 0L;

        /// <summary>
        /// Day of week bitmask where bit 0 = Monday, bit 6 = Sunday.
        /// </summary>
        private int _dayOfWeekBits = 0;

        /// <summary>
        /// Week of month where bit 0 = first.
        /// </summary>
        private int _weekOfMonth = 0;

        /// <summary>
        /// Month where 0 = Jan, bit 11 = Dec.
        /// </summary>
        private int _monthOfYear = 0;

        /// <summary>
        /// Name of payee.
        /// </summary>
        private string _payee = "";

        /// <summary>
        /// Name of category.
        /// </summary>
        private string _categoryName = "";

        /// <summary>
        /// Is this a credit (vs debit) transaction?
        /// </summary>
        private bool _isCredit = false;

        /// <summary>
        /// Monetary amount of each transaction.
        /// </summary>
        private long _amount = 0L;

        // Getters/Setters
        public Guid Id { get => _id; set => _id = value; }
        public bool IsReminder { get => _isReminder; set => _isReminder = value; }
        public bool IsOddWeeksOnly { get => _isOddWeeksOnly; set => _isOddWeeksOnly = value; }
        public DateTime LastPosting { get => _lastPosting; set => _lastPosting = value; }
        public DateTime EndingDate { get => _endingDate; set => _endingDate = value; }
        public long DayOfMonthBits { get => _dayOfMonthBits; set => _dayOfMonthBits = value; }
        public int DayOfWeekBits { get => _dayOfWeekBits; set => _dayOfWeekBits = value; }
        public int WeekOfMonth { get => _weekOfMonth; set => _weekOfMonth = value; }
        public int MonthOfYear { get => _monthOfYear; set => _monthOfYear = value; }
        public SchedulePeriod Period { get => _period; set => _period = value; }
        public long FinalPaymentAmount { get => _finalPaymentAmount; set => _finalPaymentAmount = value; }
        public bool IsEstimatedAmount { get => _isEstimatedAmount; set => _isEstimatedAmount = value; }
        public string Payee { get => _payee; set => _payee = value; }
        public string CategoryName { get => _categoryName; set => _categoryName = value; }
        public bool IsCredit { get => _isCredit; set => _isCredit = value; }
        public long Amount { get => _amount; set => _amount = value; }
        public string Memo { get => _memo; set => _memo = value; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public ScheduledEvent()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Create a deep duplicate with its own Id.
        /// </summary>
        /// <returns>The duplicate</returns>
        public ScheduledEvent Clone()
        {
            ScheduledEvent schedEvent = new ScheduledEvent();
            schedEvent._id = Guid.NewGuid();
            schedEvent._period = this._period;
            schedEvent._dayOfMonthBits = this._dayOfMonthBits;
            schedEvent._dayOfWeekBits = this._dayOfWeekBits;
            schedEvent._monthOfYear = this._monthOfYear;
            schedEvent._weekOfMonth = this._weekOfMonth;
            schedEvent._endingDate = this._endingDate;
            schedEvent._finalPaymentAmount = this._finalPaymentAmount;
            schedEvent._isEstimatedAmount = this._isEstimatedAmount;
            schedEvent._isOddWeeksOnly = this._isOddWeeksOnly;
            schedEvent._isReminder = this._isReminder;
            schedEvent._lastPosting = this._lastPosting;
            schedEvent._payee = this._payee;
            schedEvent._categoryName = this._categoryName;
            schedEvent._isCredit = this._isCredit;
            schedEvent._amount = this._amount;
            schedEvent._memo = this._memo;
            return schedEvent;
        }

        //////////////////////////// Initialization //////////////////////////

        /// <summary>
        /// Set up due dates for weekly events.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfWeek">0=Sunday ... 6=Saturday</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueWeekly(bool isReminder, int dayOfWeek, DateTime endingDate)
        {
            _period = SchedulePeriod.Weekly;
            _dayOfWeekBits = 1 << dayOfWeek;
            _endingDate = endingDate.Date;
            _lastPosting = DateTime.Now.AddDays(-1).Date;
            if(_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up dates for biweekly event.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfWeek">0=Sunday ... 6=Saturday</param>
        /// <param name="skipThisWeek">true to skip the first occurence and begin with the second one</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueBiWeekly(bool isReminder, int dayOfWeek, bool skipThisWeek, DateTime endingDate)
        {
            _period = SchedulePeriod.Weekly;    // <-- temporarily, to fool "DueNext"
            _dayOfWeekBits = 1 << dayOfWeek;
            _endingDate = endingDate.Date;
            _lastPosting = DateTime.Now.AddDays(-1).Date;
            DateTime nextDueDate = DueDateUnending(DateTime.Now.Date, DateTime.Now.Date.AddMonths(2));
            _isOddWeeksOnly = IsOddWeek(nextDueDate) != skipThisWeek;
            _period = SchedulePeriod.BiWeekly;  // <-- now correct it
            if (_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up dates for biweekly event.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfWeek">0=Sunday ... 6=Saturday</param>
        /// <param name="skipThisWeek">Day of current-or-next-month for next occurrence</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueBiWeekly(bool isReminder, int dayOfWeek, int nextDayDue, DateTime endingDate)
        {
            if(nextDayDue < 1 || nextDayDue > 31)
            {
                nextDayDue = 31;
            }
            _period = SchedulePeriod.BiWeekly; 
            _dayOfWeekBits = 1 << dayOfWeek;
            _endingDate = endingDate.Date;
            DateTime testDate = new DateTime(DateTime.Now.Date.Ticks);
            while(testDate.Day != nextDayDue)
            {
                testDate = testDate.AddDays(1).Date;
            }
            while((int)testDate.DayOfWeek != dayOfWeek)
            {
                testDate = testDate.AddDays(1).Date;
            }
            _isOddWeeksOnly = IsOddWeek(testDate);
            _lastPosting = testDate.AddDays(-1).Date;
            if (_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up dates for biweekly event.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfWeek">0=Sunday ... 6=Saturday</param>
        /// <param name="sampleDueDate">sample due date</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueBiWeekly(bool isReminder, int dayOfWeek, DateTime sampleDueDate, DateTime endingDate)
        {
            _period = SchedulePeriod.BiWeekly;
            _dayOfWeekBits = 1 << dayOfWeek;
            _endingDate = endingDate.Date;
            DateTime testDate = new DateTime(DateTime.Now.Date.Ticks);
            _isOddWeeksOnly = IsOddWeek(sampleDueDate);
            _lastPosting = testDate.AddDays(-1).Date;
            if (_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up due dates for monthly events.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfMonth">1=first day, 2=second day, 32=last day of any month</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueMonthly(bool isReminder, int dayOfMonth, DateTime endingDate)
        {
            _period = SchedulePeriod.Monthly;
            _dayOfMonthBits = 1 << (dayOfMonth - 1);
            _endingDate = endingDate.Date;
            _lastPosting = DateTime.Now.AddDays(-1).Date;
            if (_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up due dates for monthly events that come due on a given day-of-week and week number.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfWeek">0=Sunday ... 6=Saturday</param>
        /// <param name="weekOfMonth">0=first occurrence in month, 1=second occurence, etc.</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueMonthlySsa(bool isReminder, int dayOfWeek, int weekOfMonth, DateTime endingDate)
        {
            _period = SchedulePeriod.MonthlySsa;
            _dayOfWeekBits = 1 << dayOfWeek;
            _weekOfMonth = weekOfMonth;
            _endingDate = endingDate.Date;
            _lastPosting = DateTime.Now.AddDays(-1).Date;
            if (_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up due dates for yearly events.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfMonth">1=first day, 2=second day, 32=last day of any month</param>
        /// <param name="monthOfYear">1=January ... 12=December</param>
        /// <param name="endingDate">Last date of occurence.</param>
        public void SetDueAnnually(bool isReminder, int dayOfMonth, int monthOfYear, DateTime endingDate)
        {
            _period = SchedulePeriod.Annually;
            _dayOfMonthBits = 1 << (dayOfMonth - 1);
            _monthOfYear = monthOfYear - 1;
            _endingDate = endingDate.Date;
            _lastPosting = DateTime.Now.AddDays(-1).Date;
            if (_endingDate.Ticks > Eternity.Ticks)
            {
                _endingDate = Eternity;
            }
        }

        /// <summary>
        /// Set up due date for one-time events.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dayOfMonth">1=first day, 2=second day, 32=last day of any month</param>
        /// <param name="monthOfYear">1=January ... 12=December</param>
        /// <param name="year">2021 ... etc.</param>
        public void SetDueOneTime(bool isReminder, int dayOfMonth, int monthOfYear, int year)
        {
            SetDueAnnually(isReminder, dayOfMonth, monthOfYear, Eternity);
            DateTime targetDate = DueNext();
            int currentYear = DateTime.Now.Year;
            int numberofYears = year - currentYear;
            _endingDate = new DateTime(targetDate.Year + numberofYears, targetDate.Month, targetDate.Day).Date;
            _lastPosting = new DateTime(targetDate.Year + numberofYears - 1, targetDate.Month, targetDate.Day).Date;
        }

        /// <summary>
        /// Set up due date for one-time events.
        /// </summary>
        /// <param name="isReminder">true if this is just a reminder, false if it's an automatic entry</param>
        /// <param name="dueDate">Due date</param>
        public void SetDueOneTime(bool isReminder, DateTime dueDate)
        {
            SetDueOneTime(isReminder, dueDate.Date.Day, dueDate.Date.Month, dueDate.Date.Year);
        }

        /// <summary>
        /// Set EndingDate based on "number of occurences" 
        /// </summary>
        /// <param name="occurrences">How many times event should occur from today onward.</param>
        /// <returns>True if set, false if occurrences is an unrealistic value</returns>
        public bool SetRepeatCount(int occurrences)
        {
            if(occurrences < 1 || occurrences > MaxOccurrences)
            {
                return false;
            }
            DateTime dueDate = DateTime.Now.AddDays(-1).Date;
            for (int occurrence = 0; occurrence < occurrences; occurrence++)
            {
                dueDate = DueDateUnending(dueDate.AddDays(1).Date, Eternity);
            }
            if(dueDate.Date.CompareTo(new DateTime(0L)) > 0)
            {
                _endingDate = dueDate;
                return true;
            }
            return false;
        }

        ////////////////////////////// Querying //////////////////////////////

        /// <summary>
        /// Get a list of due dates since the last posting.
        /// </summary>
        /// <returns>list of unposted due dates, possibly empty</returns>
        public List<DateTime> DueDatesSinceLastPosting()
        {
            return DueDatesBetween(_lastPosting.AddDays(1).Date, _endingDate.Date);
        }

        /// <summary>
        /// Get a list of due dates since the last posting.
        /// </summary>
        /// <param name="endDate">list should not go beyond this date.</param>
        /// <returns>list of unposted due dates, possibly empty</returns>
        public List<DateTime> DueDatesSinceLastPosting(DateTime endDate)
        {
            return DueDatesBetween(_lastPosting.AddDays(1).Date, endDate.Date);
        }

        /// <summary>
        /// Get a list of due dates between two dates
        /// </summary>
        /// <param name="startDate">start looking for due dates from this one forward</param>
        /// <param name="endDate">list should not go beyond this date.</param>
        /// <returns>list of unposted due dates, possibly empty</returns>
        public List<DateTime> DueDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> list = new List<DateTime>();
            DateTime testDate = startDate;
            while (true)
            {
                testDate = DueDateStartingAt(testDate).Date;
                if(testDate.Ticks < 1 || testDate.CompareTo(endDate) >= 0)
                {
                    break;
                }
                list.Add(new DateTime(testDate.Ticks)); // clone
                testDate = testDate.AddDays(1).Date;
            }
            return list;
        }

        /// <summary>
        /// Get the next due date on or after a given date.
        /// </summary>
        /// <param name="startDate">Date on which to begin looking forward for a due date.</param>
        /// <returns>The next due date. If none, then return DateTime.Ticks=0L.</returns>
        public DateTime DueDateStartingAt(DateTime startDate)
        {
            return DueDateUnending(startDate.Date, _endingDate.Date);
        }

        /// <summary>
        /// Get a list of due dates while ignoring RepeatCount or EndingDate.
        /// </summary>
        /// <param name="startDate">start looking for due dates from this one forward</param>
        /// <param name="endDate">list should not go beyond this date.</param>
        /// <returns>list of due dates, possibly empty</returns>
        public DateTime DueDateUnending(DateTime startDate, DateTime endDate)
        { 
            if(endDate.Date.Ticks < 1L || startDate.Date.CompareTo(endDate) >= 0)
            {
                return new DateTime(0L);
            }
            if(endDate.Date.CompareTo(Eternity) > 0)
            {
                endDate = Eternity;
            }
            bool found = false;
            DateTime dueDate = new DateTime(startDate.Date.Ticks);
            while(!found) 
            {
                if(dueDate.Date.CompareTo(endDate) > 0)
                {
                    return new DateTime(0L);
                }
                DateTime tomorrow = dueDate.AddDays(1).Date;
                bool isLastDayOfMonth = (tomorrow.Day < dueDate.Day);
                bool isOnOrPastLastDay = (isLastDayOfMonth && ((_dayOfMonthBits & (0x1F << (dueDate.Day - 1))) != 0));
                bool isMonthlyDueDay = ((_dayOfMonthBits & (1 << (dueDate.Day - 1))) != 0);
                switch (Period)
                {
                    case SchedulePeriod.Monthly:
                        if (isMonthlyDueDay || isOnOrPastLastDay)
                        {
                            found = true;
                        }
                        break;
                    case SchedulePeriod.MonthlySsa:
                        if (((_dayOfWeekBits & (1 << (int)dueDate.DayOfWeek)) != 0) &&
                            _weekOfMonth == ((int)dueDate.Day - 1) / 7)
                        {
                            found = true;
                        }
                        break;
                    case SchedulePeriod.BiWeekly:
                        if (((_dayOfWeekBits & (1 << (int)dueDate.DayOfWeek)) != 0) &&
                            IsOddWeek(dueDate) == _isOddWeeksOnly)
                        {
                            found = true;
                        }
                        break;
                    case SchedulePeriod.Weekly:
                        if ((_dayOfWeekBits & (1 << (int)dueDate.DayOfWeek)) != 0)
                        {
                            found = true;
                        }
                        break;
                    case SchedulePeriod.Annually:
                        if ((isMonthlyDueDay || isOnOrPastLastDay) && _monthOfYear == dueDate.Month - 1)
                        {
                            found = true;
                        }
                        break;
                } // end switch/case
                if (found)
                {
                    break;
                }
                dueDate = dueDate.AddDays(1).Date; // no match, so try the next day
            }
            return dueDate;
        }

        /// <summary>
        /// When is this due next?
        /// </summary>
        /// <returns>Due date - may be in the past if LastPosting was a while ago.</returns>
        public DateTime DueNext()
        {
            if(_lastPosting == null || _lastPosting < new DateTime(1970, 2, 2))
            {
                return DateTime.Now; // Should never happen, but...
            }
            return DueDateStartingAt(_lastPosting.AddDays(1).Date);
        }

        /// <summary>
        /// Get a man-readable description of the due date calculation
        /// </summary>
        public string DueDescription
        {
            get
            {
                string delimiter = "";
                string daysOfMonth = "";
                for(int index = 0; index < 32; ++index)
                {
                    long mask = 1 << index;
                    if((_dayOfMonthBits & mask) != 0)
                    {
                        daysOfMonth = daysOfMonth + delimiter + (index + 1);
                        delimiter = ", ";
                    }
                }
                delimiter = "";
                string daysOfWeek = "";
                for (int index = 0; index < 32; ++index)
                {
                    long mask = 1 << index;
                    if ((_dayOfWeekBits & mask) != 0)
                    {
                        daysOfWeek = daysOfWeek + delimiter + Enum.GetNames(typeof(DayOfWeek))[index].ToString();
                        delimiter = ", ";
                    }
                }
                switch (_period)
                {
                    case SchedulePeriod.Monthly:
                        return Strings.Get("Monthly on") + " " + daysOfMonth;
                    case SchedulePeriod.Annually:
                        return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[_monthOfYear] + " " + daysOfMonth;
                    case SchedulePeriod.Weekly:
                        return daysOfWeek;
                    case SchedulePeriod.MonthlySsa:
                        return Strings.Get("Monthly on") + " " + daysOfWeek + " " + Strings.Get("Occ#") + " " + (1 + _weekOfMonth);
                    case SchedulePeriod.BiWeekly:
                        return Strings.Get("Every") + " " + Strings.Get(_isOddWeeksOnly ? "Odd" : "Even") + " " + daysOfWeek;
                }
                return "?";
            }
        }

        /// <summary>
        /// Assemble a man-readable string for this record.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ScheduledEvent" + Id.ToString() + " - " + _payee + " " + 
                DueDescription + " " + UtilityMethods.FormatCurrency(_amount, 3);
        }

        ////////////////////////////// Support ///////////////////////////////

        /// <summary>
        /// Get the number of remaining occurrences.
        /// </summary>
        /// <param name="startFrom">When to start counting</param>
        /// <returns>Repeat count, possibly zero</returns>
        public int GetRepeatCount(DateTime startFrom)
        {
            DateTime nullDate = new DateTime(0L);
            int occurrence = 0;
            DateTime dueDate = startFrom.AddDays(-1).Date;
            for (occurrence = 0; occurrence <= MaxOccurrences; occurrence++)
            {
                dueDate = DueDateStartingAt(dueDate.AddDays(1)).Date;
                if (dueDate.CompareTo(nullDate) <= 0)
                {
                    break;
                }
            }
            return occurrence;
        }

        /// <summary>
        /// Return a key that's a string but collates by payee.
        /// </summary>
        /// <returns>Collatable key.</returns>
        public string UniqueKey()
        {
            return _payee.Trim() + "-" + _id;
        }

        /// <summary>
        /// [static] Is a particular day on an odd week? (Used for biweekly alternating due dates)
        /// </summary>
        /// <param name="testDate">the date</param>
        /// <returns>true only if it occurs on an odd week.</returns>
        public static bool IsOddWeek(DateTime testDate)
        {
            long weeksInTestDate = testDate.Date.Ticks / TicksPerWeek;
            return (weeksInTestDate % 2) != 0;
        }

    }

}
