using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    [Serializable]
    public class NotificationObject : INotifyPropertyChanged
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }        
    }
}
