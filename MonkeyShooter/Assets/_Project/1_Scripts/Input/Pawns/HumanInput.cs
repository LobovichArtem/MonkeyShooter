
public sealed class HumanInput : BaseInput<HumanInputData, HumanPacker>
{
    public override string RequiredActionMap => "Human";

    public HumanInput() : base() { }
}