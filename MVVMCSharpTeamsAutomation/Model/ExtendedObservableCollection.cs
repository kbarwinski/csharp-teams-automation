using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCSharpTeamsAutomation.Model
{
    /// <summary>
    /// Class which inherits from ObservableCollection.
    /// It's purpose is to send OnCollectionChanged notifications when any of it's contained items properties change.
    /// Thanks to it, UI parts binded to it's instance refresh automatically after updating it's items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ExtendedObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public ExtendedObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public ExtendedObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
