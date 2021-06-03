using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbleCheckbook.Db;

namespace AbleCheckbook.Logic.Tests
{
    [TestClass()]
    public class ScheduledEventTests
    {
        [TestMethod()]
        public void SetDueWeeklyTest()
        {
            // Sunday
            ScheduledEvent schEvent = new ScheduledEvent();
            schEvent.SetDueWeekly(true, (int)DayOfWeek.Sunday, DateTime.Now);
            schEvent.SetRepeatCount(2);
            List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(DayOfWeek.Sunday, list.ElementAt<DateTime>(0).DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, list.ElementAt<DateTime>(1).DayOfWeek);
            Assert.IsTrue(list.ElementAt<DateTime>(1).CompareTo(list.ElementAt<DateTime>(0)) > 0);
            // Monday
            schEvent = new ScheduledEvent();
            schEvent.SetDueWeekly(true, (int)DayOfWeek.Monday, DateTime.Now);
            schEvent.SetRepeatCount(2);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(DayOfWeek.Monday, list.ElementAt<DateTime>(0).DayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, list.ElementAt<DateTime>(1).DayOfWeek);
            Assert.IsTrue(list.ElementAt<DateTime>(1).CompareTo(list.ElementAt<DateTime>(0)) > 0);
            // Saturday
            schEvent = new ScheduledEvent();
            schEvent.SetDueWeekly(true, (int)DayOfWeek.Saturday, DateTime.Now);
            schEvent.SetRepeatCount(2);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(DayOfWeek.Saturday, list.ElementAt<DateTime>(0).DayOfWeek);
            Assert.AreEqual(DayOfWeek.Saturday, list.ElementAt<DateTime>(1).DayOfWeek);
            Assert.IsTrue(list.ElementAt<DateTime>(1).CompareTo(list.ElementAt<DateTime>(0)) > 0);
        }

        [TestMethod()]
        public void SetDueBiWeeklyTest()
        {
            // Sunday
            ScheduledEvent schEvent1 = new ScheduledEvent();
            schEvent1.SetDueBiWeekly(true, (int)DayOfWeek.Sunday, false, DateTime.Now);
            schEvent1.SetRepeatCount(2);
            ScheduledEvent schEvent2 = new ScheduledEvent();
            schEvent2.SetDueBiWeekly(true, (int)DayOfWeek.Sunday, true, DateTime.Now);
            schEvent2.SetRepeatCount(2);
            List<DateTime> list1 = schEvent1.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            List<DateTime> list2 = schEvent2.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(2, list1.Count);
            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual(DayOfWeek.Sunday, list1.ElementAt<DateTime>(0).Date.DayOfWeek);
            Assert.AreEqual(7, list2.ElementAt<DateTime>(0).Date.Subtract(list1.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(7, list1.ElementAt<DateTime>(1).Date.Subtract(list2.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(7, list2.ElementAt<DateTime>(1).Date.Subtract(list1.ElementAt<DateTime>(1).Date).Days);
            // Monday
            schEvent1 = new ScheduledEvent();
            schEvent1.SetDueBiWeekly(true, (int)DayOfWeek.Monday, false, DateTime.Now);
            schEvent1.SetRepeatCount(2);
            schEvent2 = new ScheduledEvent();
            schEvent2.SetDueBiWeekly(true, (int)DayOfWeek.Monday, true, DateTime.Now);
            schEvent2.SetRepeatCount(2);
            list1 = schEvent1.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            list2 = schEvent2.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(2, list1.Count);
            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual(DayOfWeek.Monday, list1.ElementAt<DateTime>(0).Date.DayOfWeek);
            Assert.AreEqual(7, list2.ElementAt<DateTime>(0).Date.Subtract(list1.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(7, list1.ElementAt<DateTime>(1).Date.Subtract(list2.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(7, list2.ElementAt<DateTime>(1).Date.Subtract(list1.ElementAt<DateTime>(1).Date).Days);
            // Saturday
            schEvent1 = new ScheduledEvent();
            schEvent1.SetDueBiWeekly(true, (int)DayOfWeek.Saturday, false, DateTime.Now);
            schEvent1.SetRepeatCount(2);
            schEvent2 = new ScheduledEvent();
            schEvent2.SetDueBiWeekly(true, (int)DayOfWeek.Saturday, true, DateTime.Now);
            schEvent2.SetRepeatCount(2);
            list1 = schEvent1.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            list2 = schEvent2.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(2, list1.Count);
            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual(DayOfWeek.Saturday, list1.ElementAt<DateTime>(0).Date.DayOfWeek);
            Assert.AreEqual(7, list2.ElementAt<DateTime>(0).Date.Subtract(list1.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(7, list1.ElementAt<DateTime>(1).Date.Subtract(list2.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(7, list2.ElementAt<DateTime>(1).Date.Subtract(list1.ElementAt<DateTime>(1).Date).Days);
        }

        [TestMethod()]
        public void SetDueBiWeeklyByDayTest()
        {
            DateTime dueDate = DateTime.Now.AddDays(8); // over a week away
            ScheduledEvent schEvent = new ScheduledEvent();
            schEvent.SetDueBiWeekly(true, (int)dueDate.DayOfWeek, (int)dueDate.Day, DateTime.Now);
            schEvent.SetRepeatCount(5);
            List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(dueDate.DayOfWeek, list.ElementAt<DateTime>(0).Date.DayOfWeek);
            Assert.AreEqual(dueDate.Date, list.ElementAt<DateTime>(0).Date);
            Assert.AreEqual(14, list.ElementAt<DateTime>(1).Date.Subtract(list.ElementAt<DateTime>(0).Date).Days);
            Assert.AreEqual(14, list.ElementAt<DateTime>(2).Date.Subtract(list.ElementAt<DateTime>(1).Date).Days);
            Assert.AreEqual(14, list.ElementAt<DateTime>(3).Date.Subtract(list.ElementAt<DateTime>(2).Date).Days);
            Assert.AreEqual(14, list.ElementAt<DateTime>(4).Date.Subtract(list.ElementAt<DateTime>(3).Date).Days);
        }

        [TestMethod()]
        public void SetDueMonthlyTest()
        {
            // 1st of month
            ScheduledEvent schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 1, DateTime.Now);
            schEvent.SetRepeatCount(15);
            List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(15, list.Count);
            Assert.AreEqual(1, list.ElementAt<DateTime>(0).Day);
            Assert.AreEqual(1, list.ElementAt<DateTime>(1).Day);
            Assert.AreEqual(1, list.ElementAt<DateTime>(11).Day);
            Assert.AreEqual(1, list.ElementAt<DateTime>(14).Day);
            Assert.IsTrue((list.ElementAt<DateTime>(1).Date.Subtract(list.ElementAt<DateTime>(0).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(2).Date.Subtract(list.ElementAt<DateTime>(1).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(3).Date.Subtract(list.ElementAt<DateTime>(2).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(4).Date.Subtract(list.ElementAt<DateTime>(3).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(5).Date.Subtract(list.ElementAt<DateTime>(4).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(6).Date.Subtract(list.ElementAt<DateTime>(5).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(7).Date.Subtract(list.ElementAt<DateTime>(6).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(8).Date.Subtract(list.ElementAt<DateTime>(7).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(9).Date.Subtract(list.ElementAt<DateTime>(8).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(10).Date.Subtract(list.ElementAt<DateTime>(9).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(11).Date.Subtract(list.ElementAt<DateTime>(10).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(12).Date.Subtract(list.ElementAt<DateTime>(11).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(13).Date.Subtract(list.ElementAt<DateTime>(12).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(14).Date.Subtract(list.ElementAt<DateTime>(13).Date)).Days >= 28);
            // 2nd of month
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 2, DateTime.Now);
            schEvent.SetRepeatCount(3);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, list.ElementAt<DateTime>(0).Day);
            Assert.AreEqual(2, list.ElementAt<DateTime>(1).Day);
            Assert.AreEqual(2, list.ElementAt<DateTime>(2).Day);
            Assert.IsTrue((list.ElementAt<DateTime>(1).Date.Subtract(list.ElementAt<DateTime>(0).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(2).Date.Subtract(list.ElementAt<DateTime>(1).Date)).Days >= 28);
            // 28th of month
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 28, DateTime.Now);
            schEvent.SetRepeatCount(3);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(28, list.ElementAt<DateTime>(0).Day);
            Assert.AreEqual(28, list.ElementAt<DateTime>(1).Day);
            Assert.AreEqual(28, list.ElementAt<DateTime>(2).Day);
            Assert.IsTrue((list.ElementAt<DateTime>(1).Date.Subtract(list.ElementAt<DateTime>(0).Date)).Days >= 28);
            Assert.IsTrue((list.ElementAt<DateTime>(2).Date.Subtract(list.ElementAt<DateTime>(1).Date)).Days >= 28);
            // 29th of month
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 29, DateTime.Now);
            schEvent.SetRepeatCount(12);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            VerifyLastDaysOfMonth(list, 29);
            // 30th of month
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 30, DateTime.Now);
            schEvent.SetRepeatCount(12);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            VerifyLastDaysOfMonth(list, 30);
            // 31th of month
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 31, DateTime.Now);
            schEvent.SetRepeatCount(12);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            VerifyLastDaysOfMonth(list, 31);
            // Last day of month (32)
            schEvent = new ScheduledEvent();
            schEvent.SetDueMonthly(true, 32, DateTime.Now);
            schEvent.SetRepeatCount(12);
            list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            VerifyLastDaysOfMonth(list, 32);
        }

        [TestMethod()]
        public void SetDueMonthlySsaTest()
        {
            for (int weekNumber = 0; weekNumber < 4; weekNumber++)
            {
                for (int dayNumber = 0; dayNumber < 7; dayNumber++)
                {
                    ScheduledEvent schEvent = new ScheduledEvent();
                    schEvent.SetDueMonthlySsa(true, dayNumber, weekNumber, DateTime.Now);
                    schEvent.SetRepeatCount(30);
                    List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
                    Assert.AreEqual(30, list.Count);
                    Assert.AreEqual(dayNumber, (int)list.ElementAt<DateTime>(0).DayOfWeek);
                    Assert.AreEqual(weekNumber, (list.ElementAt<DateTime>(0).Day - 1) / 7);
                    Assert.IsTrue((list.ElementAt<DateTime>(29).Date.Subtract(list.ElementAt<DateTime>(0).Date)).Days > 860);
                    Assert.IsTrue((list.ElementAt<DateTime>(29).Date.Subtract(list.ElementAt<DateTime>(0).Date)).Days < 890);
                }
            }
        }

        [TestMethod()]
        public void SetDueAnnuallyTest()
        {
            for(int month = 1; month <= 12; month += 11)
            {
                for (int day = 1; day < 29; day += 7)
                {
                    ScheduledEvent schEvent = new ScheduledEvent();
                    DateTime startDate = new DateTime(DateTime.Now.Year, month, day);
                    if (DateTime.Now.CompareTo(startDate) > 0)
                    {
                        startDate = startDate.AddYears(1);
                    }
                    schEvent.SetDueAnnually(true, startDate.Day, startDate.Month, startDate.AddYears(2).AddDays(1));
                    List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
                    Assert.AreEqual(3, list.Count);
                    Assert.AreEqual(startDate.Month, list.ElementAt<DateTime>(0).Month);
                    Assert.AreEqual(startDate.Day, list.ElementAt<DateTime>(0).Day);
                    Assert.AreEqual(startDate.Month, list.ElementAt<DateTime>(1).Month);
                    Assert.AreEqual(startDate.Day, list.ElementAt<DateTime>(1).Day);
                    Assert.AreEqual(startDate.Month, list.ElementAt<DateTime>(2).Month);
                    Assert.AreEqual(startDate.Day, list.ElementAt<DateTime>(2).Day);
                    Assert.IsTrue((list.ElementAt<DateTime>(2).Subtract(list.ElementAt<DateTime>(0))).Days >= 730);
                }
            }
            for(int day = 29; day < 32; day++)
            {
                ScheduledEvent schEvent = new ScheduledEvent();
                DateTime startDate = new DateTime(DateTime.Now.Year, 1, day);
                if (DateTime.Now.Date.CompareTo(startDate) > 0)
                {
                    startDate = startDate.AddYears(1);
                }
                schEvent.SetDueAnnually(true, startDate.Day, startDate.Month, startDate.AddYears(2).AddDays(1));
                List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(startDate.Month, list.ElementAt<DateTime>(0).Month);
                Assert.IsTrue(list.ElementAt<DateTime>(0).Day >= 28);
                Assert.AreEqual(startDate.Month, list.ElementAt<DateTime>(1).Month);
                Assert.IsTrue(list.ElementAt<DateTime>(1).Day >= 28);
                Assert.AreEqual(startDate.Month, list.ElementAt<DateTime>(2).Month);
                Assert.IsTrue(list.ElementAt<DateTime>(2).Day >= 28);
                Assert.IsTrue((list.ElementAt<DateTime>(2).Subtract(list.ElementAt<DateTime>(0))).Days >= 730);
            }
        }

        [TestMethod()]
        public void SetDueOneTimeTest()
        {
            ScheduledEvent schEvent = new ScheduledEvent();
            DateTime dueDate = DateTime.Now.AddDays(3);
            schEvent.SetDueOneTime(true, dueDate.Day, dueDate.Month, dueDate.Year);
            List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(dueDate.Year, list.ElementAt<DateTime>(0).Year);
            Assert.AreEqual(dueDate.Month, list.ElementAt<DateTime>(0).Month);
            Assert.AreEqual(dueDate.Day, list.ElementAt<DateTime>(0).Day);
        }

        [TestMethod()]
        public void SetDueOneTimeTest1()
        {
            ScheduledEvent schEvent = new ScheduledEvent();
            DateTime dueDate = DateTime.Now.AddDays(3);
            schEvent.SetDueOneTime(true, dueDate);
            List<DateTime> list = schEvent.DueDatesBetween(DateTime.Now, ScheduledEvent.Eternity);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(dueDate.Year, list.ElementAt<DateTime>(0).Year);
            Assert.AreEqual(dueDate.Month, list.ElementAt<DateTime>(0).Month);
            Assert.AreEqual(dueDate.Day, list.ElementAt<DateTime>(0).Day);
        }

        [TestMethod()]
        public void DueDatesSinceLastPostingTest()
        {
            ScheduledEvent schEvent = new ScheduledEvent();
            DateTime dueDate = DateTime.Now.AddDays(1);
            schEvent.SetDueWeekly(true, (int)dueDate.DayOfWeek, dueDate.AddDays(29));
            List<DateTime> list = schEvent.DueDatesSinceLastPosting();
            Assert.AreEqual(5, list.Count);
            schEvent.LastPosting = list.ElementAt<DateTime>(1);
            list = schEvent.DueDatesSinceLastPosting(ScheduledEvent.Eternity);
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// Verify that a list of 12 monthly dates are correct.
        /// </summary>
        /// <param name="list">List of 12  sequential due dates</param>
        /// <param name="maxDays">day of month under evaluation</param>
        private static void VerifyLastDaysOfMonth(List<DateTime> list, int maxDays)
        {
            Assert.AreEqual(12, list.Count);
            int shortMonths28 = 0;
            int leapMonths29 = 0;
            int mediumMonths30 = 0;
            int longMonths31 = 0;
            int bizarreMonths = 0;
            for (int index = 0; index < 12; index++)
            {
                switch (list.ElementAt<DateTime>(index).Day)
                {
                    case 28:
                        shortMonths28++;
                        break;
                    case 29:
                        leapMonths29++;
                        break;
                    case 30:
                        mediumMonths30++;
                        break;
                    case 31:
                        longMonths31++;
                        break;
                    default:
                        bizarreMonths++;
                        break;
                }
            }
            if (maxDays < 30)
            {
                Assert.AreEqual(12, shortMonths28 + leapMonths29);
            }
            else if (maxDays < 31)
            {
                Assert.AreEqual(1, shortMonths28 + leapMonths29);
                Assert.AreEqual(11, mediumMonths30);
            }
            else
            {
                Assert.AreEqual(1, shortMonths28 + leapMonths29);
                Assert.AreEqual(4, mediumMonths30);
                Assert.AreEqual(7, longMonths31);
            }
            Assert.IsTrue((list.ElementAt<DateTime>(11).Date.Subtract(list.ElementAt<DateTime>(0).Date)).Days > 330);
        }

    }
}