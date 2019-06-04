using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDailyReview
{
    public enum CompleteStatus
    {
        NotStarted,
        Done,
        ValidExcuse,
        DoItNow,
        Failed
    }
    public class OneCheckItem
    {
        public string Description;
        public int Hour;
        public int Minute;
        public CompleteStatus Status;

    }
    public class DailyLog
    {
        public string DateStr;
        public int FinalScore;
        public List<OneCheckItem> AllItems;

        public DailyLog()
        {
            AllItems = new List<OneCheckItem>();
            AllItems.Add(new OneCheckItem()
            {
                Description = "Night reading the day before",
                Hour = 8,
                Minute = 0
            });
            AllItems.Add(new OneCheckItem()
            {
                Description = "Morining Excercise",
                Hour = 10,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Breakfast as planned",
                Hour = 10,
                Minute = 0
            });
            AllItems.Add(new OneCheckItem()
            {
                Description = "Morning 3 steps",
                Hour = 10,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Morning Coding mini tasks ",
                Hour = 11,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Morning review",
                Hour = 11,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Lunch as planned",
                Hour = 13,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Lunch Brush",
                Hour = 13,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Dinner as planned",
                Hour = 13,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Afternoon Coding mini tasks",
                Hour = 15,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Afternoon review",
                Hour = 15,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Nothing unhealthy",
                Hour = 17,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Kids interaction",
                Hour = 19,
                Minute = 30
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Evening review",
                Hour = 20,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "Daily chore",
                Hour = 21,
                Minute = 30
            });
        }
    }

    public class AllDailyLog
    {
        public List<DailyLog> All = new List<DailyLog>();

        public OneCheckItem CheckPendingItemForToday()
        {
            string dateStr = DateTime.Today.ToLongTimeString();
            DailyLog dl = All.Find(x => x.DateStr == dateStr);
            if (dl == null)
            {
                dl = new DailyLog();
                dl.DateStr = dateStr;
                All.Add(dl);
            }

            foreach (var one in dl.AllItems)
            {
                if (one.Hour > DateTime.Now.Hour
                    || (one.Hour == DateTime.Now.Hour && one.Minute > DateTime.Now.Minute))
                {
                    if (one.Status == CompleteStatus.NotStarted)
                        return one;
                }
            }

            return null;
        }
    }
}
