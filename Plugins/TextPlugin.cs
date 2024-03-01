using   Microsoft.SemanticKernel;
public class TextPlugin{

    [KernelFunction]
    public string Uppercase(string input)=>input.ToUpper();
    [KernelFunction]
    public string FullName(string firstName,string lastName)=> $"{firstName} {lastName}";
}