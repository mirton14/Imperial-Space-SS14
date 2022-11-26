using Content.Client.Links;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Corvax.JoinQueue;

[GenerateTypedNameReferences]
public sealed partial class QueueGui : Control
{
    public event Action? QuitPressed;
    
    public QueueGui()
    {
        RobustXamlLoader.Load(this);
        LayoutContainer.SetAnchorPreset(this, LayoutContainer.LayoutPreset.Wide);
        
        QuitButton.OnPressed += (_) => QuitPressed?.Invoke();
        //PriorityJoinButton.OnPressed += (_) =>
        //{
        //    IoCManager.Resolve<IUriOpener>().OpenUri(UILinks.Patreon);
        //};
    }

    public void UpdateInfo(int total, int position)
    {
        QueueTotal.Text = total.ToString();
        QueuePosition.Text = position.ToString();
    }
}
