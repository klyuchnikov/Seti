using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lab2CheckersClient
{
    public class User : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            private set
            {
                if (name != value)
                    name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public User(string name)
        {
            this.name = name;
        }
        public override string ToString()
        {
            return name + " - " + (this.IsFree ? "свободен" : "играет");
        }

        private bool isFree;

        public bool IsFree
        {
            get { return isFree; }
            set
            {
                isFree = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsFree"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
