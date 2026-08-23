namespace FsmViewer.Core.Interfaces;

public interface IFsmElement
{
    void Accept(IFsmVisitor visitor);
}
