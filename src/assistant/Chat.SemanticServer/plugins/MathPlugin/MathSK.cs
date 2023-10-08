using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Chat.SemanticServer.plugins.MathPlugin
{
    public class MathSk
    {
        /// <summary>
        /// 得到负数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [SKFunction, Description("得到负数")]
        public string Negative(string number)
        {
            return (Convert.ToInt32(number) * -1).ToString();
        }

        /// <summary>
        /// 两个数相减
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [SKFunction, Description("两个数相减")]
        [SKParameter("num1", "第一个数")]
        [SKParameter("num2", "第二个数")]
        public string Subtraction(SKContext context)
        {
            return (
                Convert.ToDouble(context.Variables["num1"], CultureInfo.InvariantCulture) -
                Convert.ToDouble(context.Variables["num2"], CultureInfo.InvariantCulture)
            ).ToString(CultureInfo.InvariantCulture);
        }
    }
}
