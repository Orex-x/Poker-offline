using CommunityToolkit.Mvvm.ComponentModel;

namespace PokerOfflineClient.ViewModels
{
    public partial class ShirtViewModel : ObservableObject
    {
        [ObservableProperty]
        public ImageSource source;

        [ObservableProperty]
        public string sourseFile;

        [ObservableProperty]
        public int borderSize;  
    }
}
