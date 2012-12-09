﻿using System;
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

        private Guid guid;
        public Guid Guid
        {
            get { return guid; }
            set
            {
                guid = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Guid"));
            }
        }

        private Guid opponentGuid;
        public Guid OpponentGuid
        {
            get { return opponentGuid; }
            set
            {
                opponentGuid = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OpponentGuid"));
                    PropertyChanged(this, new PropertyChangedEventArgs("OpponentUser"));
                }
            }
        }
        public User OpponentUser
        {
            get { return GameProcess.Inctance.Users.Single(a => a.Guid == opponentGuid); }
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
            return name + " - " + (this.opponentGuid == Guid.Empty ? "свободен" : "играет");
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
