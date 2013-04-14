using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

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
        public static void Delegate(object state)
        {
            var url = (string)state;
            var task =
                Model2.Current.Tasks.SingleOrDefault(
                    q => q.ThisThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId);
            if (task == null)
            {
                task = new Task(url, Thread.CurrentThread);
                Model2.Current.tasks.Add(task);
                Model2.Current.OnPropertyChanged("Tasks");
            }
            task.URL = url;
            /*   Dispatcher.Invoke(new Action(() =>
               {
                   listBox1.Items.Add(state);
               }), null);*/
            Parser.Start(url);
        }

        public Task(string url, Thread thread)
        {
            this.URL = url;
            this.thisthread = thread;
            var timer = new System.Timers.Timer(100);
            timer.Elapsed += delegate { OnPropertyChanged("ThisThreadState"); OnPropertyChanged("ThreadIsAlive"); };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
