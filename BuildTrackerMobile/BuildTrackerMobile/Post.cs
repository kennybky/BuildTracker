using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BuildTrackerMobile
{
    internal class Post : INotifyPropertyChanged
    {
        public int Id;
        private string _title;
        [JsonProperty("title")]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(); // This notifies the view or ViewModel that the value of
                //a property in the Model has changed and the view neeeds to be updated;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
