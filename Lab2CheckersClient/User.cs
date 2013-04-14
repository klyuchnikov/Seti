using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Klyuchnikov.Seti.OneSemestr.Lab2CheckersClient
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
            get { return this.opponentGuid == Guid.Empty ? null : GameProcess.Inctance.Users.Single(a => a.Guid == opponentGuid); }
        }
        private string name;
        public string UserName
        {
            get { return name; }
            set
            {
                    name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
            }
        }

        public User(string name)
        {
            this.name = name;
        }
        public User() { }

        private int userID;
        public int UserID
        {
            get { return userID; }
            set
            {
                userID = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserID"));
            }
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

        internal void Update(User user1)
        {
            this.OpponentGuid = user1.OpponentGuid;
            this.UserName = user1.UserName;
            this.IP = user1.IP;
        }
    }
}
