using XVNML.Core.Native;
using XVNML2U.Mono;

public class NameTest : MonoActionSender
{
    public string YourName
    {
        get
        {
            return RuntimeReferenceTable
                .Get()
                .ToString();
        }
        set
        {
            RuntimeReferenceTable
                .Set(value: value);
        }
    }

    private void Awake()
    {
        YourName = "Yourmom";
    }
}
