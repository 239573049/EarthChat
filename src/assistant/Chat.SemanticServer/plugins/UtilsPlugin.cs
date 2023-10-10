using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Chat.SemanticServer.plugins
{
    public class UtilsPlugin
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IKernel _kernel;
        public UtilsPlugin(IKernel kernel)
        {
            _kernel = kernel;
        }

        [SKFunction, Description("发送邮件")]
        [SKParameter("input", "入参")]
        public string SendEmail(SKContext context)
        {
            Console.WriteLine(context.Variables["input"]);
          return "发送成功";
        }
    }
}
