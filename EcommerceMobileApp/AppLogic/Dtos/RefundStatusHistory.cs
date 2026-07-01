using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EcommerceMobileApp.AppLogic.Dtos
{
    public class RefundStatusHistoryDto : INotifyPropertyChanged
    {
        private bool isCurrent;

        public bool IsCurrent
        {
            get => isCurrent;
            set
            {
                isCurrent = value;
                OnPropertyChanged();
            }
        }

        public string Status { get; set; }
        public DateTime DateCreated { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
