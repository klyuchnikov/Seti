using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lab2CheckersClient
{
    [Serializable]
    public class User : INotifyPropertyChanged
    {
        private string ip;
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IP"));
            }
        }

        public int id;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
            }
        }

        public string opponent;
        public string Opponent
        {
            get { return ip; }
            set
            {
                opponent = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Opponent"));
            }
        }

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
            return name + " - " + (this.opponent == null ? "свободен" : "играет");
        }
        /*
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
        }*/


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
