using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using TexodeTask.Models;

namespace TexodeTask.Helpers
{
    public static class XmlWorker
    {
        public static void Serialize(List<Student> list)
        {
            string pathToXmlFile = Directory.GetCurrentDirectory() + "\\Students.xml";
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Students";
            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>), xRoot);
            using (FileStream fs = new FileStream(pathToXmlFile, FileMode.Create))
            {
                serializer.Serialize(fs, list, ns);
            }
        }
        public static ObservableCollection<Student> Deserialize()
        {
            try
            {
                string pathToXmlFile = Directory.GetCurrentDirectory() + "\\Students.xml";
                if (File.Exists(pathToXmlFile))
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "Students";
                    XmlSerializer xs = new XmlSerializer(typeof(List<Student>), xRoot);
                    ObservableCollection<Student> a = new ObservableCollection<Student>((List<Student>)xs.Deserialize(new FileStream(pathToXmlFile, FileMode.Open)));
                    return a;
                }
                else
                {
                    throw new Exception("Отсутствует файл Students.xml в папке проекта");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message);
            }
            return new ObservableCollection<Student>();
        }
    }
}
