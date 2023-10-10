using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Threading.Tasks;
using Chat.SemanticServer.plugins.MathPlugin;

namespace Xzy.SK.Api.plugins.MathPlugin
{
    public class NativeNested
    {
        private readonly IKernel _kernel;
        public NativeNested(IKernel kernel)
        {
            _kernel = kernel;
        }

        //通过自然语义先找到最大和最小的2个值，然后用最大值减去最小值得到结果返回
        [SKFunction]
        public async Task<string> Test(SKContext context)
        {
            string request = context.Variables["input"];
            var mathPlugin1 = _kernel.ImportSemanticSkillFromDirectory("plugins", "MathPlugin");
            var mathPlugin2 = _kernel.ImportSkill(new MathSK(), "MathPlugin");

            var maxmin = await _kernel.RunAsync(request, mathPlugin1["FindMaxMin"]);

            var nums = maxmin.Result.Split("-");

            var variables = new ContextVariables
            {
                ["num1"] = nums[0],
                ["num2"] = nums[1]
            };
            var result = await _kernel.RunAsync(variables, mathPlugin2["Subtraction"]);
            return result.Result;
        }
    }
}
