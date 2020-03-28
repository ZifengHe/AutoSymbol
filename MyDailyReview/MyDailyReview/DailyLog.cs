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
                Description = "减少时间段内信息的来源，原因是为了控制",
                KeyAnswer = "情绪",
                Hour = 9,
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
                Description = "每天过得是否有意义的标准是什么",
                KeyAnswer = "笔记",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "貌似损失，其实人生一半的增量来自于",
                KeyAnswer = "妥协",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "不看空和不批评的原因是有一种什么巨大成本",
                KeyAnswer = "时间",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "消除决策恐慌靠的是什么",
                KeyAnswer = "数学",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "管理多巴胺需求就是生活的",
                KeyAnswer = "全部",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "所有的冲动情绪都是想以怎样的方式享受到结果",
                KeyAnswer = "快速",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "不确定性是智者欢迎的",
                KeyAnswer = "机遇",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "不盲目自信的原因是最大的不确定因素总是",
                KeyAnswer = "未知",
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
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "可以感觉好，是因为得到了",
                KeyAnswer = "洞见",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "无糖是因为得到了",
                KeyAnswer = "不痒",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "什么只需等待，必会得到",
                KeyAnswer = "好运",
                Hour = 9,
                Minute = 0
            });
           

            AllItems.Add(new OneCheckItem()
            {
                Description = "表达不同意见应该以什么开头",
                KeyAnswer = "yes",
                Hour = 9,
                Minute = 0
            });

           

            AllItems.Add(new OneCheckItem()
            {
                Description = "获取智慧的套路",
                KeyAnswer = "自问",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "高维赢低维靠的是",
                KeyAnswer = "套路",
                Hour = 9,
                Minute = 0
            });

            AllItems.Add(new OneCheckItem()
            {
                Description = "做家务得到的是",
                KeyAnswer = "干净",
                Hour = 9,
                Minute = 0
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
