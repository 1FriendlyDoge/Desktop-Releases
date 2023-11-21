using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ERM_Desktop.ViewModels;

public abstract class PageViewModel : ViewModelBase
{
    public abstract Task OnNavigatedToAsync();

    public abstract Task OnNavigatedFromAsync();
}

public class ViewModelBase : ObservableObject { }