using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mizer.Annotations;

namespace Mizer
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Set[] _sets;

        public ISourceProvider SourceProvider { get; set; }

        public Set[] Sets
        {
            get { return _sets; }
            set
            {
                if (_sets == value)
                    return;
                _sets = value;
                OnPropertyChanged();
            }
        }

        public SimpleCommand Run
        {
            get
            {
                return new SimpleCommand
                {
                    ExecuteDelegate =
                        async obj =>
                        {
                            if (SourceProvider == null)
                                SourceProvider = new Magiccards();
                            Sets = await SourceProvider.ReadSetList();
                        },
                    CanExecuteDelegate = obj => SourceProvider == null || !SourceProvider.IsBusy
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}