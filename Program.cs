using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Codeing........

var keyOpenAI = app.Configuration["OpenAI:Key"]!;

var modelGptOpenAI = "gpt-4-0125-preview";
//var modelGpt = "gpt-4";Model deployment

//setting (New Style)
var setting = new OpenAIPromptExecutionSettings(){
    MaxTokens = 100,
    Temperature = 0.7 //more value, more creatative
};

// old style
// setting.MaxTokens = 100;
// setting.Temperature = 0.7;

app.MapPost("/chat_completion",async (string prompt)=>{
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId: modelGptOpenAI,apiKey:keyOpenAI)
        .Build();

    //get prompt from front end and sent direct to OpenAI
    var kernelFunction = kernel.CreateFunctionFromPrompt(prompt,setting);

    // call AI to Process
    var result = await kernel.InvokeAsync(kernelFunction);

    return result.GetValue<string>();
});


// How to excute plugins
app.MapPost("/kernel_plugin",async (string firstName,string lastName)=>{
    var kernel = new Kernel();

    //import Plugin
    var kernelPlugin = kernel.ImportPluginFromType<TextPlugin>();

    //pass parameters to plugin (Native function)
    var result = await kernel.InvokeAsync(kernelPlugin["Fullname"],new(){
        ["firstName"] = firstName,
        ["lastName"] = lastName
    });

    return result.GetValue<string>();
});

//mix Kernel fn and plugin
app.MapPost("/prompt_definition",async()=>{
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId: modelGptOpenAI,apiKey:keyOpenAI)
        .Build();

    kernel.ImportPluginFromType<TimePlugin>("time");
    
    var promptDefinition = @"
        Is it moring,afternoon, evening or night(moring/afternoon/evening/night)?
        Is it weekend (weekend/not weekend)?

        ###
        Today is :{{time.Date}}
        Current time is : {{time.Time}}
        ";

    // var promptTemplateFactory = new KernelPromptTemplateFactory();
    // var promptTemplate = promptTemplateFactory
    //     .Create(new PromptTemplateConfig(promptDefinition));

    // var prompt = await promptTemplate.RenderAsync(kernel);

    //get prompt from front end and sent to OpenAI
    var kernelFunction = kernel.CreateFunctionFromPrompt(promptDefinition,setting);

    // call AI to Process
    var result = await kernel.InvokeAsync(kernelFunction);

    return result.GetValue<string>();

});

app.MapPost("/variable",async(string input)=>{
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId: modelGptOpenAI,apiKey:keyOpenAI)
        .Build();

    var promptDefinition = @"
        Translate text below to Thai language
        ###
        {{$input}}
        ";

    // var promptTemplateFactory = new KernelPromptTemplateFactory();
    // var promptTemplate = promptTemplateFactory
    //     .Create(new PromptTemplateConfig(promptDefinition));

    // var prompt = await promptTemplate.RenderAsync(kernel);

    //get prompt from front end and sent to OpenAI
    var kernelFunction = kernel.CreateFunctionFromPrompt(promptDefinition,setting);

    // call AI to Process
    var result = await kernel.InvokeAsync(kernelFunction,new(){
        ["input"] = input
    });

    return result.GetValue<string>();

});

//use search engine TBC

app.Run();
