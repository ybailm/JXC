using Project.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.ViewModel
{
    public class AttendantViewModel:NotificationObject
    {
        private Attendant attendant;
        public Attendant Attendant
        {
            get
            {
                return attendant;
            }
            set
            {
                attendant = value;
                RaisePropertyChanged(nameof(Attendant));                
            }
        }
        private bool isChecked ;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
