using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using TexodeTask.Helpers;

namespace TexodeTask.Models
{
    public class Model: INotifyPropertyChanged // использовал паттерн Singletone
    {
        private ObservableCollection<Student> students;
        public ObservableCollection<Student> Students
        {
            get
            {
                return students;
            }
            set
            {
                students = value;
                OnPropertyChanged();
            }
        }

        private List<int> ageValues;
        public List<int> AgeValues
        {
            get
            {
                return ageValues;
            }
            set
            {
                ageValues = value;
                OnPropertyChanged();
            }
        }
        private static Model Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private Model()
        {
            this.Students = new ObservableCollection<Student>();
            //Students = XmlWorker.Deserialize();
            this.AgeValues = new List<int>();
            for (ushort i = 16; i < 101; i++)
            {
                this.AgeValues.Add(i);
            }
            try
            {
                using (studentsDB db = new studentsDB())
                {
                    if(db.Students.Count() > 0)
                    {
                        Students = new ObservableCollection<Student>(db.Students);
                    }
                    else
                    {
                        MessageBox.Show("База данных пуста!");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        public static Model GetModel()
        {
            if(Instance == null)
            {
                Instance = new Model();
            }
            return Instance;
        }
    }
}
