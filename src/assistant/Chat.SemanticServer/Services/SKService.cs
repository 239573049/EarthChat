using System.Text.Json;
using Chat.SemanticServer.plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.Core;
using Xzy.SK.Api.plugins;

namespace Chat.SemanticServer.Services;

public class SKService : ServiceBase
{
    public SKService()
    {
        App.MapGet("/api/v1/SK", Intent);
    }

    protected IKernel Kernel => GetRequiredService<IKernel>();

    public async Task<ModelResult> Intent(string msg,[FromServices]IKernel kernel)
    {
        //对话摘要  SK.Skills.Core 核心技能
        kernel.ImportSkill(new ConversationSummarySkill(kernel), "ConversationSummarySkill");

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins");
        var intentPlugin = kernel
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "BasePlugin");
        var travelPlugin = kernel
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "Travel");

        var NativeNested = kernel.ImportSkill(new UtilsPlugin(kernel), "UtilsPlugin");
        var getIntentVariables = new ContextVariables
        {
            ["input"] = msg,
            ["options"] = "Attractions, Delicacy,Traffic,Weather,SendEmail" //给GPT的意图，通过Prompt限定选用这些里面的
        };
        string intent = (await kernel.RunAsync(getIntentVariables, intentPlugin["GetIntent"])).Result.Trim();
        ISKFunction MathFunction;
        //获取意图后动态调用Fun
        switch (intent)
        {
            case "Attractions":
                MathFunction = kernel.Skills.GetFunction("Travel", "Attractions");
                break;
            case "Delicacy":
                MathFunction = kernel.Skills.GetFunction("Travel", "Delicacy");
                break;
            case "Traffic":
                MathFunction = kernel.Skills.GetFunction("Travel", "Traffic");
                break;
            case "Weather":
                MathFunction = kernel.Skills.GetFunction("Travel", "Weather");
                break;
            case "SendEmail":
                var sendEmailVariables = new ContextVariables
                {
                    ["input"] = msg,
                    ["example"] = JsonSerializer.Serialize(new
                        { send_user = "xzy", receiver_user = "xzy", body = "hello" })
                };
                msg = (await kernel.RunAsync(sendEmailVariables, intentPlugin["JSON"])).Result;
                MathFunction = kernel.Skills.GetFunction("UtilsPlugin", "SendEmail");
                break;
            default:
                return new ModelResult("对不起我不知道");
        }

        var result = await kernel.RunAsync(msg, MathFunction);

        return new ModelResult(result.Result);
    }
}