using System.Threading.Tasks;

namespace ERM_Desktop.ViewModels;

public abstract class WidgetViewModel : ViewModelBase
{
    public abstract Task OnLoadedAsync();
}