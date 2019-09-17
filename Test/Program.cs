using HZC.Utils;
using System;
using HZC.MyDapper.Conditions;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            {
//                var name = "饕餮";
//                var pinyin = PinYinUtil.ConvertToAllSpell(name);
//                var abbr = PinYinUtil.GetFirstSpell(name);
//
//                Console.WriteLine(pinyin + "-" + abbr);
            }

            {

                var builder = new ConditionBuilder("@", "Students");

                builder = builder.AndEqual("ClazzId", 1)
                    .AndContains("Tags", "测试")
                    .AndContains(new[] {"Name", "Mobile", "Card"}, "1333")
                    .AndIn("Id", new[] {1, 2, 3}, "Schools")
                    .AndCondition("CreateAt=UpdateAt")
                    .AndOr(ConditionClausList.New()
                        .Add("Test1", SqlOperator.EndsWith, "132")
                        .Add("Test", SqlOperator.GreaterThan, DateTime.Now, "School")
                    );

                var sql = builder.ToCondition();
                var param = builder.ToParameters();

                Console.WriteLine(sql);

            }
            Console.ReadLine();
        }
    }
}
