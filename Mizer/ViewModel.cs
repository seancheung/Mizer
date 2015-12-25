using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Mizer.Annotations;

namespace Mizer
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Set[] _sets;
        private Card[] _cards;
        private Set _selectedSet;
        private Card _selectedCard;
        private bool _isLoadingSets;
        private bool _isLoadingCards;

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

        public Set SelectedSet
        {
            get { return _selectedSet; }
            set
            {
                if (_selectedSet == value)
                    return;
                _selectedSet = value;
                OnPropertyChanged();
            }
        }

        public Card[] Cards
        {
            get { return _cards; }
            set
            {
                if (_cards == value)
                    return;
                _cards = value;
                OnPropertyChanged();
            }
        }

        public Card SelectedCard
        {
            get { return _selectedCard; }
            set
            {
                if (_selectedCard == value)
                    return;
                _selectedCard = value;
                OnPropertyChanged();
            }
        }

        public SimpleCommand LoadSets
        {
            get
            {
                return new SimpleCommand
                {
                    ExecuteDelegate =
                        async obj =>
                        {
                            IsLoadingSets = true;
                            if (!ReadSets())
                            {
                                if (SourceProvider == null)
                                    SourceProvider = new Magiccards();
                                Sets = await SourceProvider.ReadSetList();
                                if (Sets != null && Sets.Length > 0)
                                    SaveSets();
                            }
                            IsLoadingSets = false;
                        },
                    CanExecuteDelegate = obj => SourceProvider == null || !SourceProvider.IsBusy
                };
            }
        }

        public SimpleCommand LoadCards
        {
            get
            {
                return new SimpleCommand
                {
                    ExecuteDelegate =
                        async obj =>
                        {
                            if (SelectedSet != null)
                            {
                                IsLoadingCards = true;
                                if (!ReadCards(SelectedSet))
                                {
                                    if (SourceProvider == null)
                                        SourceProvider = new Magiccards();
                                    Cards = await SourceProvider.ReadCardList(SelectedSet);
                                }
                                SaveCards(SelectedSet);
                                IsLoadingCards = false;
                            }
                        },
                    CanExecuteDelegate = obj => SourceProvider == null || !SourceProvider.IsBusy
                };
            }
        }


        public bool IsLoadingSets
        {
            get { return _isLoadingSets; }
            set
            {
                if (_isLoadingSets == value)
                    return;
                _isLoadingSets = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingCards
        {
            get { return _isLoadingCards; }
            set
            {
                if (_isLoadingCards == value)
                    return;
                _isLoadingCards = value;
                OnPropertyChanged();
            }
        }

        private void SaveSets()
        {
            using (var sw = File.CreateText("sets.xml"))
            {
                var ser = new XmlSerializer(typeof (Set[]));
                ser.Serialize(sw, Sets);
            }
        }

        private bool ReadSets()
        {
            if (!File.Exists("sets.xml"))
                return false;
            using (var stream = File.OpenRead("sets.xml"))
            {
                var ser = new XmlSerializer(typeof (Set[]));
                Sets = (Set[]) ser.Deserialize(stream);
            }
            return true;
        }

        private void SaveCards(Set set)
        {
            using (var sw = File.CreateText(string.Format("{0}.{1}.xml", set.Code, set.Lang)))
            {
                var ser = new XmlSerializer(typeof (Set));
                ser.Serialize(sw, set);
            }
        }

        private bool ReadCards(Set set)
        {
            if (!File.Exists(string.Format("{0}.{1}.xml", set.Code, set.Lang)))
                return false;
            using (var stream = File.OpenRead(string.Format("{0}.{1}.xml", set.Code, set.Lang)))
            {
                var ser = new XmlSerializer(typeof (Set));
                Cards = ((Set) ser.Deserialize(stream)).Cards;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}