using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.Core;

namespace Chat.SemanticServer.Services;

public class SKService : BaseService<SKService>
{
    public async Task<ModelResult> Intent(string msg)
    {

            //对话摘要  SK.Skills.Core 核心技能
            Kernel.ImportSkill(new ConversationSummarySkill(Kernel), "ConversationSummarySkill");

            var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            var intentPlugin = Kernel
                 .ImportSemanticSkillFromDirectory(pluginsDirectory, "BasePlugin");
            var travelPlugin = Kernel
                 .ImportSemanticSkillFromDirectory(pluginsDirectory, "Travel");

            var getIntentVariables = new ContextVariables
            {
                ["input"] = msg,
                ["options"] = "Attractions, Delicacy,Traffic,Weather,SendEmail"  //给GPT的意图，通过Prompt限定选用这些里面的
            };
            string intent = (await Kernel.RunAsync(getIntentVariables, intentPlugin["GetIntent"])).Result.Trim();
            ISKFunction MathFunction;
            //获取意图后动态调用Fun
            switch (intent)
            {
                case "Attractions":
                    MathFunction = Kernel.Skills.GetFunction("Travel", "Attractions");
                    break;
                case "Delicacy":
                    MathFunction = Kernel.Skills.GetFunction("Travel", "Delicacy");
                    break;
                case "Traffic":
                    MathFunction = Kernel.Skills.GetFunction("Travel", "Traffic");
                    break;
                case "Weather":
                    MathFunction = Kernel.Skills.GetFunction("Travel", "Weather");
                    break;
                case "SendEmail":
                    var sendEmailVariables = new ContextVariables
                    {
                        ["input"] = msg,
                        ["example"] = JsonSerializer.Serialize(new { send_user = "xzy", receiver_user = "xzy", body = "hello" })
                    };
                    msg = (await Kernel.RunAsync(sendEmailVariables, intentPlugin["JSON"])).Result;
                    MathFunction = Kernel.Skills.GetFunction("UtilsPlugin", "SendEmail");
                    break;
                default:
                    return new ModelResult("对不起我不知道");
            }
            var result = await Kernel.RunAsync(msg, MathFunction);

            return new ModelResult(result.Result);
    }
}