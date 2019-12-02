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
        Failed,
        KeyInputComplete
    }
    public class OneCheckItem
    {
        public string Description;
        public string KeyAnswer;
        public int Hour;
        public int Minute;
        public CompleteStatus Status;

    }
    public class DailyLog
    {
        public string DateStr;
        public int FinalScore;
        public List<OneCheckItem> AllItems;

        public void Init()
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
                Description = "锻炼是因为得到了",
                KeyAnswer="自控",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "是否终生学习的标准是什么",
                KeyAnswer = "笔记",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "一个防止沉溺于什么样的奖励",
                KeyAnswer = "快",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "不确定性是智者欢迎的",
                KeyAnswer = "机遇",
                Hour = 20,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "高情商交流需要回避的字是",
                KeyAnswer = "你",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "未来会好的，因为有什么样的可能性",
                KeyAnswer = "无限",
                Hour = 9,
                Minute = 0
            });



            AllItems.Add(new OneCheckItem()
            {
                Description = "低热量饮食是因为得到了",
                KeyAnswer = "不痛",
                Hour = 9,
                Minute = 20
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "可以感觉好，是因为得到了",
                KeyAnswer = "洞见",
                Hour = 11,
                Minute = 20
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "无糖是因为得到了",
                KeyAnswer = "不痒",
                Hour = 10,
                Minute = 40
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "什么只需等待，必会得到",
                KeyAnswer = "好运",
                Hour = 12,
                Minute = 40
            });
           

            AllItems.Add(new OneCheckItem()
            {
                Description = "表达不同意见应该以什么开头",
                KeyAnswer = "yes",
                Hour = 14,
                Minute = 30
            });

           

            AllItems.Add(new OneCheckItem()
            {
                Description = "获取智慧的套路",
                KeyAnswer = "自问",
                Hour = 18,
                Minute = 30
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "高维赢低维靠的是",
                KeyAnswer = "套路",
                Hour = 21,
                Minute = 30
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "做家务得到的是",
                KeyAnswer = "干净",
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
            string dateStr = DateTime.Today.ToShortDateString();
            DailyLog dl = All.Find(x => x.DateStr == dateStr);
            if (dl == null)
            {
                dl = new DailyLog();
                dl.Init();
                dl.DateStr = dateStr;
                All.Add(dl);
            }

            foreach (var one in dl.AllItems)
            {
                if ((one.Status == CompleteStatus.NotStarted && (DateTime.Now.Hour > one.Hour
                    || (one.Hour == DateTime.Now.Hour && DateTime.Now.Minute > one.Minute)))
                    || (one.Status == CompleteStatus.DoItNow && (DateTime.Now.Hour > one.Hour + 1)))
                {
                    return one;
                }

                if(string.IsNullOrEmpty(one.KeyAnswer)==false
                    && one.Status!= CompleteStatus.KeyInputComplete
                    && (DateTime.Now.Hour > one.Hour || (one.Hour == DateTime.Now.Hour && DateTime.Now.Minute > one.Minute)))
                {
                    return one;
                }
            }

            return null;
        }
    }
}
