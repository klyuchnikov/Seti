using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace _2SLab1
{
    public class Document : INotifyPropertyChanged
    {
        private static int Count;
        public int ID { get; set; }
        public string URL { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
         public Document()
         {
         }

        public Document(string name, string url)
        {
            this.ID = Count++;
            this.Name = name;
            this.URL = url;
        }

        public Tag[] Tags
        {
            get { return Model.Current.Tags.Where(a => a.Document == this).ToArray(); }
        }

        public void Update()
        {
            OnPropertyChanged("Name");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Tag : INotifyPropertyChanged
    {
        private static int Count;
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Document Document;
        public Tag(Document doc, string name, string value)
        {
            this.ID = Count++;
            this.Name = name;
            this.Value = value;
            this.Document = doc;
        }
        public Attribute[] Attributes
        {
            get { return Model.Current.Attributes.Where(a => a.Tag == this).ToArray(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Attribute : INotifyPropertyChanged
    {

        private static int Count;
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Tag Tag;
        public Attribute(Tag tag, string name, string value)
        {
            this.ID = Count++;
            this.Name = name;
            this.Value = value;
            this.Tag = tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   public class Model : INotifyPropertyChanged
    {
        private Model()
        {
            documents = new List<Document>();
            tags = new List<Tag>();
            attributes = new List<Attribute>();
        }
        public static Model Current = new Model();

        public readonly List<Document> documents;
        public Document[] Documents
        {
            get { return documents.ToArray(); }
            set
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Documents"));
                foreach (var document1 in documents)
                {
                    document.Update();
                }
            }
        }
        private Document document;
        public Document Document
        {
            get { return document; }
            set
            {
                document = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Document"));
            }
        }


        private readonly List<Tag> tags;
        public List<Tag> Tags
        {
            get { return tags; }
            set
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Tags"));
            }
        }
        private readonly List<Attribute> attributes;
        public List<Attribute> Attributes
        {
            get { return attributes; }
            set
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Attributes"));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
