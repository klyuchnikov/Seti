using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace Klyuchnikov.Seti.TwoSemestr.CommonLibrary
{
    public class Model2 : INotifyPropertyChanged
    {
        private Model2()
        {
            tasks = new List<Task>();
        }

        public readonly List<Task> tasks;
        public Task[] Tasks
        {
            get { return tasks.ToArray(); }
            set { OnPropertyChanged("Tasks"); }
        }
        public static Model2 Current = new Model2();

        public void Delegate(object state)
        {
            var url = (string)state;
            var task =
                Current.Tasks.SingleOrDefault(
                    q => q.ThisThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId);
            if (task == null)
            {
                var th = Thread.CurrentThread;
                task = new Task(url, th);
                Current.tasks.Add(task);
                Current.OnPropertyChanged("Tasks");
            }
            task.URL = url;
            Parser.Start(url);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class Task : INotifyPropertyChanged
    {
        public Thread thisthread;
        public Thread ThisThread
        {
            get { return thisthread; }
        }

        public ThreadState ThisThreadState
        {
            get
            {
                return thisthread != null ? thisthread.ThreadState : ThreadState.Unstarted;
            }
        }
        public bool ThreadIsAlive
        {
            get
            {
                return thisthread != null && thisthread.IsAlive;
            }
        }

        public string url;
        public string URL
        {
            get { return url; }
            set
            {
                url = value;
                OnPropertyChanged("URL");
            }
        }

        public Task(string url, Thread thread)
        {
            this.URL = url;
            this.thisthread = thread;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
