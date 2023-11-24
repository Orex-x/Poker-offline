using CommunityToolkit.Mvvm.ComponentModel;

namespace PokerOfflinelient.ViewModels
{
    public partial class CardViewModel : ObservableObject
    {
        public bool isShow;

        [ObservableProperty]
        ImageSource source;

        public string CardFileName { get; set; }

        public string ShirtFileName { get; set; }
       
        public void ShowCard()
        {
            Source = ImageSource.FromFile(CardFileName);
            isShow = true;
        }

        public void ShowShirt()
        {
            Source = ImageSource.FromFile(ShirtFileName);
            isShow = false;
        }

        public void TurnOver()
        {
            if (isShow)
                Source = ImageSource.FromFile(ShirtFileName);
            else
            {
                if(!string.IsNullOrEmpty(CardFileName))
                    Source = ImageSource.FromFile(CardFileName);
                else
                    Source = ImageSource.FromFile(ShirtFileName);
            }

            isShow = !isShow;
        }
    }
}
