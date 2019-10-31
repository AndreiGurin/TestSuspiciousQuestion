using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TestSuspiciousQuestion
{
    class ViewModel : INotifyPropertyChanged
    {
        private String _displayName;
        private Task _refreshViewTask;

        public ViewModel()
        {
            Initialize();
            Console.WriteLine($"Constructor is finishing... - {DateTime.Now.ToLongTimeString()}");
        }

        public String DisplayName
        {
            get
            {
                Console.WriteLine($"Getter invoked - {DateTime.Now.ToLongTimeString()}");
                if (_refreshViewTask != null && !_refreshViewTask.IsCompleted)
                {
                    Console.WriteLine($"Getter is waiting... - {DateTime.Now.ToLongTimeString()}");
                    if (!_refreshViewTask.Wait(10000))
                    {
                        Console.WriteLine($"Getter is finishing... - {DateTime.Now.ToLongTimeString()}");
                        return "Хм... чот не получили ничего. Странно.";
                    }
                }
                Console.WriteLine($"Getter is finishing... - {DateTime.Now.ToLongTimeString()}");
                return _displayName;
            }

            set
            {
                Console.WriteLine($"Setter invoked - {DateTime.Now.ToLongTimeString()}");
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged();
                }
                Console.WriteLine($"Setter is finishing... - {DateTime.Now.ToLongTimeString()}");
            }
        }
        
        private async void Initialize()
        {
            _refreshViewTask = RefreshView();
            await _refreshViewTask;
        }

        private Task<String> GetDisplayName()
        {
            return Task<String>.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine($"GetDisplayName() awakened - {DateTime.Now.ToLongTimeString()}");
                return "Мы получили что-то полезное.";
            });
        }

        private async Task RefreshView()
        {
            DisplayName = await GetDisplayName();
            Console.WriteLine($"RefreshView() finishing... - {DateTime.Now.ToLongTimeString()}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
