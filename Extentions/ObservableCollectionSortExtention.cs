using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Extentions
{
    public static class ObservableCollectionSortExtention
    {
        public static ObservableCollection<TSource> OrderByDescending<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {           
            var sortedList = source.AsEnumerable().OrderByDescending(keySelector);            
            return new ObservableCollection<TSource>(sortedList);
        }

        public static ObservableCollection<TSource> OrderBy<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            var sortedList = source.AsEnumerable().OrderBy(keySelector);
            return new ObservableCollection<TSource>(sortedList);
        }
    }
}
