using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lab2CheckersClient
{
    public class ServerInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ServerInfo()
        { }

        public ServerInfo Current = new ServerInfo();

        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                port = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Port"));
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

    }
}
