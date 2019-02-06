using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TexodeTask.Commands;
using TexodeTask.Models;
using TexodeTask.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TexodeTask.WievModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            _model = Model.GetModel();
            editableStudent = new Student();
            Edit_or_Add = false;
        }
        public readonly Model _model;
        private bool eoa;
        public bool Edit_or_Add
        {
            get
            {
                return eoa;
            }
            set
            {
                eoa = value;
                OnPropertyChanged();
            }
        }
        private bool IsNeedToSaveChanges = false;
        public ObservableCollection<Student> students => _model.Students;
        public List<Student> StudentsToAddOrEdit = new List<Student>();
        public List<int> StudentsToRemove = new List<int>();
        public List<int> agevalues => _model.AgeValues;
        private Student editableStudent;
        public Student EditableStudent
        {
            get
            {
                return this.editableStudent;
            }
            set
            {
                editableStudent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #region commands
        public ICommand StartEditStudent
        {
            get
            {
                return new Command((obj) =>
                {
                    EditableStudent = (Student)obj;
                    Edit_or_Add = true;
                });
            }
        }

        public ICommand EditCanceled
        {
            get
            {
                return new Command((obj) =>
                {
                    Edit_or_Add = false;
                });
            }
        }


        #region newcommans4Entity
        public ICommand AddNewStudentToDB
        {
            get
            {
                return new Command((obj) =>
                {
                    try
                    {
                        var parms = (object[])obj;
                        // Имя и фамилия с больших букв на русском/английском без приставок, цифр и прочего
                        Regex s_name = new Regex("(^[A-Z]{1}[a-z]{1,24} [A-Z]{1}[a-z]{1,24}$)|(^[А-Я]{1}[а-я]{1,24} [А-Я]{1}[а-я]{1,24}$)");
                        if (s_name.IsMatch((string)parms[0] + ' ' + (string)parms[1]))
                        {
                            Student ns = new Student
                                (
                                    (students.Count > 0 ? (int)students.Max(el => { return el.Id; }) + 1 : (int)students.Count + 1),
                                    (string)parms[0],
                                    (string)parms[1],
                                    (int)parms[2],
                                    (Gender)parms[3]
                                );
                            students.Add(ns);
                            StudentsToAddOrEdit.Add(ns);
                            if (!IsNeedToSaveChanges) IsNeedToSaveChanges = true;
                        }
                        else
                            throw new Exception("Имя и фамилия должны начинаться с заглавных букв, не должны содержать никаких символов кроме латинских или русских букв и должны быть длиной от 2 до 25 символов.\nИсправьте их и повторите попытку");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка при добавлении студента");
                    }

                });
            }
        }

        public ICommand RemoveStudentFromDB
        {
            get
            {
                return new Command(obj=>
                {
                    System.Collections.IList items = (System.Collections.IList)obj;
                    var collection = items.Cast<Student>().ToList();
                    System.Windows.Forms.DialogResult dr =
                        System.Windows.Forms.MessageBox.Show(
                            "Вы уверены, что хотите удалить " + (collection.Count > 1 ?
                            "выбранные элементы?" : "выбранный элемент?"),
                            "Подтвердите удаление записей",
                            System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (!IsNeedToSaveChanges) IsNeedToSaveChanges = true;
                        foreach (Student s in collection)
                        {
                            students.Remove(s);
                            StudentsToRemove.Add(s.Id);
                        }
                    }
                });
            }
        }

        public ICommand EditStudentInDB
        {
            get
            {
                return new Command((obj) =>
                {
                    try
                    {
                        var parms = (object[])obj;
                        Regex s_name = new Regex("(^[A-Z]{1}[a-z]{1,24} [A-Z]{1}[a-z]{1,24}$)|(^[А-Я]{1}[а-я]{1,24} [А-Я]{1}[а-я]{1,24}$)");
                        if (s_name.IsMatch((string)parms[0] + ' ' + (string)parms[1]))
                        {
                            Edit_or_Add = false;
                            int id = editableStudent.Id;
                            int index = students.IndexOf(students.Single(el => { return el.Id == id; }));
                            Student ns = new Student
                                (
                                    id,
                                    (string)parms[0],
                                    (string)parms[1],
                                    (int)parms[2],
                                    (Gender)parms[3]
                                );
                            students[index] = ns;
                            StudentsToAddOrEdit.Add(ns);
                            if(!IsNeedToSaveChanges) IsNeedToSaveChanges = true;
                        }
                        else
                            throw new Exception("Имя и фамилия должны начинаться с заглавных букв, не должны содержать никаких символов кроме латинских или русских букв и должны быть длиной от 2 до 25 символов.\nИсправьте их и повторите попытку");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка при редактировании студента");
                    }
                });
            }
        }

        public ICommand SaveDBChanges
        {
            get
            {
                return new Command((obj) =>
                {
                    try
                    {
                        using (studentsDB db = new studentsDB())
                        {
                            foreach(Student s in StudentsToAddOrEdit)
                            {
                                Student b = db.Students.Find(s.Id);
                                if (b != null)
                                {
                                    b.First_Name = s.First_Name;
                                    b.Last_Name = s.Last_Name;
                                    b.Age = s.Age;
                                    b.Gender = s.Gender;
                                }
                                else
                                {
                                    db.Students.Add(s);
                                }
                            }
                            foreach(int s in StudentsToRemove)
                            {
                                db.Students.Remove(db.Students.Find(s));
                            }
                            db.SaveChanges();
                            IsNeedToSaveChanges = false;
                            MessageBox.Show("Изменения сохранены в базе данных!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ошибка при сохранении информации в БД");
                    }
                }, (obj) =>
                {
                    return IsNeedToSaveChanges;
                });
            }
        }
        #endregion

        #region oldcommands4xml
        public ICommand AddNewStudent
        {
            get
            {
                return new Command((obj) =>
                {
                    try
                    {
                        var parms = (object[])obj;
                        Regex s_name = new Regex("(^[A-Z]{1}[a-z]{1,24} [A-Z]{1}[a-z]{1,24}$)|(^[А-Я]{1}[а-я]{1,24} [А-Я]{1}[а-я]{1,24}$)");
                        // string LatNamePattern = @"[A-Z]{1}[a-z]*";
                        if (s_name.IsMatch((string)parms[0] + ' ' + (string)parms[1]))
                        {
                            students.Add(new Student
                                (
                                    (students.Count > 0 ? (int)students.Max(el => { return el.Id; }) + 1 : (int)students.Count + 1),
                                    (string)parms[0],
                                    (string)parms[1],
                                    (int)parms[2],
                                    (Gender)parms[3])
                                );
                            if (!IsNeedToSaveChanges) IsNeedToSaveChanges = true;
                        }
                        else
                            throw new Exception("Имя и фамилия должны начинаться с заглавных букв, не должны содержать никаких символов кроме латинских или русских букв и должны быть длиной от 2 до 25 символов.\nИсправьте их и повторите попытку");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            }
        }

        public ICommand SerializeStudentList
        {
            get
            {
                return new Command((obj) =>
                {
                    XmlWorker.Serialize(students.ToList());
                    IsNeedToSaveChanges = false;
                    MessageBox.Show("Записи успешно сохранены в файл 'Students.xml'", "Сериализация завершена");
                },(obj)=>
                {
                    return IsNeedToSaveChanges;
                });
            }
        }

        public ICommand RemoveStudentsFromList
        {
            get
            {
                return new Command((obj) =>
                {
                    System.Collections.IList items = (System.Collections.IList)obj;
                    var collection = items.Cast<Student>().ToList();
                    System.Windows.Forms.DialogResult dr =
                        System.Windows.Forms.MessageBox.Show(
                            $"Вы уверены, что хотите удалить " + (collection.Count > 1? 
                            "выбранные элементы?": "выбранный элемент?"),
                            "Подтвердите удаление записей",
                            System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (!IsNeedToSaveChanges) IsNeedToSaveChanges = true;
                        foreach (Student s in collection)
                        {
                            students.Remove(s);
                        }
                    }
                });
            }
        }

        public ICommand EditAccepted
        {
            get
            {
                return new Command((obj) =>
                {
                    try
                    {
                        if (!IsNeedToSaveChanges) IsNeedToSaveChanges = true;
                        var parms = (object[])obj;
                        Regex s_name = new Regex("(^[A-Z]{1}[a-z]{1,24} [A-Z]{1}[a-z]{1,24}$)|(^[А-Я]{1}[а-я]{1,24} [А-Я]{1}[а-я]{1,24}$)");
                        if (s_name.IsMatch((string)parms[0] + ' ' + (string)parms[1]))
                        {
                            Edit_or_Add = false;
                            int id = students.Single(el => { return el.Id == editableStudent.Id; }).Id;
                            int index = students.IndexOf(students.Single(el => { return el.Id == id; }));
                            students[index] = new Student
                                (
                                    id,
                                    (string)parms[0],
                                    (string)parms[1],
                                    (int)parms[2],
                                    (Gender)parms[3]);
                            }
                        else
                            throw new Exception("Имя и фамилия должны начинаться с заглавных букв, не должны содержать никаких символов кроме латинских или русских букв и должны быть длиной от 2 до 25 символов.\nИсправьте их и повторите попытку");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка валидации данных");
                    }

                });
            }
        }
        #endregion

        #endregion
    }
}