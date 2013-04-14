using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klyuchnikov.Seti.OneSemestr.Lab2CheckersServer
{
    [Serializable]
    public class User : INotifyPropertyChanged
    {
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

        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
            }
        }

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
        private Guid opponentGuid;
        public Guid OpponentGuid
        {
            get { return opponentGuid; }
            set
            {
                opponentGuid = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Opponent"));
            }
        }
        public User OpponentUser
        { get { return this.opponentGuid == Guid.Empty ? null : Server.Current.ListUsers.Single(a => a.Guid == opponentGuid); } }

        public override string ToString()
        {
            if (opponentGuid == Guid.Empty)
                return string.Format("{0}: {1}, IP: {2}", guid, userName, ip);
            else
                return string.Format("{0}: {1}, IP: {2} Играет с {3}", guid, userName, ip, OpponentUser.UserName);

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
