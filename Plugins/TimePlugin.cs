using Microsoft.SemanticKernel;
public class TimePlugin{
    [KernelFunction]
    public string Date()=> DateTimeOffset.Now.ToString("D");
    [KernelFunction]
    public string Time()=> DateTimeOffset.Now.ToString("hh:mm:ss tt");
}