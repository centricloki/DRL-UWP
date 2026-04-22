using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class GroupInfoCollection<T> : ObservableCollection<T>
    {
        public object Key { get; set; }

        public new IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)base.GetEnumerator();
        }
    }
}
