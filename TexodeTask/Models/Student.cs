using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace TexodeTask.Models
{
    public enum  Gender
    {
        Male = 0,
        Female = 1
    }
    [Serializable]
    public class Student
    {
        private int id;
        private string first_name;
        private string last_name;
        private uint age;
        private int gender;
        [XmlAttribute("Id")]
        [Key]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        [XmlElement("FirstName")]
        public string First_Name
        {
            get
            {
                return first_name;
            }
            set
            {
                first_name = value;
            }
        }
        [XmlElement("Last")]
        public string Last_Name
        {
            get
            {
                return last_name;
            }
            set
            {
                last_name = value;
            }
        }
        [XmlElement("Age")]
        public int Age
        {
            get
            {
                return (int)age;
            }
            set
            {
                if(value > 100)
                {
                    age = 100;
                    MessageBox.Show("Возраст студента был автоматически сокращён до 100 лет, т.к. изначально превышал данное значение");
                }
                else if(value < 16)
                {
                    age = 16;
                    MessageBox.Show($"Возраст студента {First_Name + ' ' + Last_Name} был автоматически увеличен до 16 лет, т.к. изначально был меньше, чем данное значение");
                }
                else
                    age = (uint)value;
            }
        }
        [XmlElement("Gender")]
        public int Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        public Student() { }
        public Student(int id, string fn, string ln, int age, Gender g)
        {
            this.Id = id;
            this.First_Name = fn;
            this.Last_Name = ln;
            this.Age = Math.Abs(age);
            this.Gender = (ushort)g;
        }
    }
}