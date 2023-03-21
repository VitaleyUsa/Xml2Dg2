using HandyControl.Controls;

using HandyControl.Tools;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

//using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;

using hc = HandyControl.Controls;

using hcd = HandyControl.Data;

namespace Ikriv.Xml
{
    /// <summary>
    /// Creates XmlAttributeOverrides instance using an easy-to-use fluent interface
    /// </summary>
    public class OverrideXml
    {
        private readonly XmlAttributeOverrides _overrides = new XmlAttributeOverrides();
        private XmlAttributes _attributes;
        private string _currentMember = "";
        private Type _currentType;
        /// <summary>
        /// Adds specified instance of XmlRootAttribute for current type or member
        /// </summary>
        public OverrideXml Attr(XmlRootAttribute xmlRoot)
        {
            Open();
            _attributes.XmlRoot = xmlRoot;
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlAttributeAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlAttributeAttribute attribute)
        {
            Open();
            _attributes.XmlAttribute = attribute;
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlElementAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlElementAttribute attribute)
        {
            Open();
            _attributes.XmlElements.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlAnyElementAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlAnyElementAttribute attribute)
        {
            Open();
            _attributes.XmlAnyElements.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlArrayAttribute to current type or memeber
        /// </summary>
        public OverrideXml Attr(XmlArrayAttribute attribute)
        {
            Open();
            _attributes.XmlArray = attribute;
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlArrayItemAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlArrayItemAttribute attribute)
        {
            Open();
            _attributes.XmlArrayItems.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlTypeAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlTypeAttribute attribute)
        {
            Open();
            _attributes.XmlType = attribute;
            return this;
        }

        /// <summary>
        /// Constructs XmlAttributeOverrides instance from previously specified attributes
        /// </summary>
        public XmlAttributeOverrides Commit()
        {
            if (_attributes != null)
            {
                _overrides.Add(_currentType, _currentMember, _attributes);
                _currentMember = "";
                _attributes = null;
            }

            return _overrides;
        }

        /// <summary>
        /// Specifies that subsequent attributes wil be applied to the given member of the current type
        /// </summary>
        public OverrideXml Member(string name)
        {
            Commit();
            if (_currentType == null) throw new InvalidOperationException("Current type is not defined. Use Override<T>() to define current type");

            // attempt to verify that such member indeed exists
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            if (_currentType.GetProperty(name, flags) == null && _currentType.GetField(name, flags) == null)
            {
                throw new InvalidOperationException("Property or field '" + name + "' does not exist in type " + _currentType.Name + " or is not public");
            }

            _currentMember = name;
            return this;
        }

        /// <summary>
        /// Specifies that subsequent attributes wil be applied to type t
        /// </summary>
        public OverrideXml Override(Type t)
        {
            Commit();
            _currentType = t;
            _currentMember = "";
            return this;
        }

        /// <summary>
        /// Specifies that subsequent attributes wil be applied to type T
        /// </summary>
        public OverrideXml Override<T>()
        {
            return Override(typeof(T));
        }
        /// <summary>
        /// Adds [XmlAnyAttribute] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyAttribute()
        {
            Open();
            _attributes.XmlAnyAttribute = new XmlAnyAttributeAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyElement] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyElement()
        {
            Open();
            _attributes.XmlAnyElements.Add(new XmlAnyElementAttribute());
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyElement(name)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyElement(string name)
        {
            Open();
            _attributes.XmlAnyElements.Add(new XmlAnyElementAttribute(name));
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyElement(name,ns)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyElement(string name, string ns)
        {
            Open();
            _attributes.XmlAnyElements.Add(new XmlAnyElementAttribute(name, ns));
            return this;
        }

        /// <summary>
        /// Adds [XmlArray] attribute to current type or memeber
        /// </summary>
        public OverrideXml XmlArray()
        {
            Open();
            _attributes.XmlArray = new XmlArrayAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlArray(elementName)] attribute to current type or memeber
        /// </summary>
        public OverrideXml XmlArray(string elementName)
        {
            Open();
            _attributes.XmlArray = new XmlArrayAttribute(elementName);
            return this;
        }

        /// <summary>
        /// Adds [XmlArrayItem] attribute to current type or member
        /// </summary>
        public OverrideXml XmlArrayItem()
        {
            Open();
            _attributes.XmlArrayItems.Add(new XmlArrayItemAttribute());
            return this;
        }

        /// <summary>
        /// Adds [XmlArrayItem(elementName)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlArrayItem(string elementName)
        {
            Open();
            _attributes.XmlArrayItems.Add(new XmlArrayItemAttribute(elementName));
            return this;
        }

        /// <summary>
        /// Adds [XmlAttribute] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAttribute()
        {
            Open();
            _attributes.XmlAttribute = new XmlAttributeAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlAttribute(name)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAttribute(string name)
        {
            Open();
            _attributes.XmlAttribute = new XmlAttributeAttribute(name);
            return this;
        }

        /// <summary>
        /// Adds [XmlDefault(value)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlDefaultValue(object value)
        {
            Open();
            _attributes.XmlDefaultValue = value;
            return this;
        }

        /// <summary>
        /// Adds [XmlElement] attribute to current type or member
        /// </summary>
        public OverrideXml XmlElement()
        {
            Open();
            _attributes.XmlElements.Add(new XmlElementAttribute());
            return this;
        }

        /// <summary>
        /// Adds [XmlElement(name)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlElement(string name)
        {
            Open();
            _attributes.XmlElements.Add(new XmlElementAttribute(name));
            return this;
        }

        /// <summary>
        /// Adds [XmlIgnore] attribute to current type or member
        /// </summary>
        /// <param name="bIgnore"></param>
        public OverrideXml XmlIgnore(bool bIgnore = true)
        {
            Open();
            _attributes.XmlIgnore = bIgnore;
            return this;
        }

        /// <summary>
        /// Applies or removes [XmlNamespaceDeclarations] attribute from current type or member
        /// </summary>
        public OverrideXml Xmlns(bool value)
        {
            Open();
            _attributes.Xmlns = value;
            return this;
        }

        /// <summary>
        /// Adds [XmlRoot(elementName)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlRoot(string elementName)
        {
            Open();
            _attributes.XmlRoot = new XmlRootAttribute(elementName);
            return this;
        }
        /// <summary>
        /// Adds [XmlText] attribute to current type or member
        /// </summary>
        public OverrideXml XmlText()
        {
            Open();
            _attributes.XmlText = new XmlTextAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlType(typeName)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlType(string typeName)
        {
            Open();
            _attributes.XmlType = new XmlTypeAttribute(typeName);
            return this;
        }
        private void Open()
        {
            if (_attributes == null) _attributes = new XmlAttributes();
        }
    }
}

namespace Xml2Dgr
{
    public partial class MainWindow
    {
        private readonly Button ButtonCreateXML = new Button();
        private readonly string password = "HelloEveryoneHere";
        private readonly string profile_filepath = "profile.dat";
        private string final_file;
        private Person[] founders;
        // Создаем список пользователей
        private List<Person> users = new List<Person>();
        public MainWindow()
        {
            InitializeComponent();
            ConfigHelper.Instance.SetLang("ru");
            ButtonCreateXML.Name = "buttonChooseFile";
            ButtonCreateXML.Style = (Style)FindResource("ButtonPrimary");
            ButtonCreateXML.Content = "Выберите папку с xml";

            ButtonCreateXML.Click += ButtonChooseFile_Click;
            ButtonCreateXML.SetValue(hc.BorderElement.CornerRadiusProperty, new CornerRadius(15));
            fpContent.Children.Add(ButtonCreateXML);

            // загружаем данные о пользователях из файла при запуске приложения
            LoadUsers(password, profile_filepath);
        }

        public void ButtonChooseFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                founders = CreateFounders(XmlFormatter(fbd.SelectedPath));
                if (founders.Length == 0) return;

                spContent?.Children.Clear();

                for (int i = 0; i < founders.Length; i++)
                {
                    hc.ComboBox cb = new hc.ComboBox
                    {
                        Name = $"ComboBox{i}", // задаем имя комбобоксу для дальнейшего обращения к нему
                        Style = (Style)FindResource("ComboBoxExtend"),
                        Tag = i
                    }; // создаем комбобокс
                    hc.TitleElement.SetTitle(cb, $"{founders[i].LastName} {founders[i].FirstName} {founders[i].MiddleName}");

                    cb.Items.Add("Возникновение прав участника/учредителя");
                    cb.Items.Add("Изменение сведений об участнике/учредителе");
                    cb.Items.Add("Прекращение прав участника/учредителя");
                    cb.SelectedIndex = 0;

                    cb.SelectionChanged += ComboBox_SelectionChanged; // подписываемся на событие
                    // добавляем комбобокс в контейнер
                    spContent.Children.Add(cb);

                    //File.WriteAllText("Result.dg2", CreateFounder(XmlFormatter(fbd.SelectedPath), 1));
                }

                LoadXML(true);
            }
        }

        // Валидация ФИО
        internal static bool ValidateFIO(string This)
        {
            string fullName = This.Trim();
            return !Regex.IsMatch(fullName, @"^[а-яА-ЯёЁ]+\s[а-яА-ЯёЁ]+\s[а-яА-ЯёЁ]+$");
        }

        // Валидация телефонного номера
        internal static string ValidatePhoneNumber(string This)
        {
            var phoneNumber = This.Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");

            if (phoneNumber.StartsWith("8"))
            {
                phoneNumber = "+7" + phoneNumber.Substring(1);
            }
            else if (!phoneNumber.StartsWith("+7"))
            {
                phoneNumber = "+7" + phoneNumber;
            }
            return phoneNumber;
        }

        // Добавить сюда шаблон заполнения нотариуса и перенести в метод класса?
        private static string CreateDG2(Person[] newpeople, Person notarius, bool applicant = false)
        {
            // создаем и наполняем шаблон для участников/учредителей
            StringBuilder template = new StringBuilder();
            if (newpeople != null)
            {
                string series;

                // Добавляем участников\учредителей
                for (int i = 0; i < newpeople.Length; i++)
                {
                    Person person = newpeople[i];
                    series = Regex.Replace(person.Series, @"(.{2})", "$1 ");
                    template = template.Append(
                                        $"<Uchr DtStart=\"\" DtEnd=\"\" Num=\"-{i + 1}\" IdVidObj=\"3\" IdStatus=\"{person.IdStatus}\">\r\n" +
                                            $"<FLBef FamFL=\"{person.LastName}\" NameFL=\"{person.FirstName}\" OtchFL=\"{person.MiddleName}\" />\r\n" +
                                        $"</Uchr>\r\n" +
                                        $"<FLObj DtStart=\"\" DtEnd=\"\" Num=\"-{i + 1}\" IdVidObj=\"3\">\r\n" +
                                            $"<FL FamFL=\"{person.LastName}\" NameFL=\"{person.FirstName}\" OtchFL=\"{person.MiddleName}\" IdDokFL=\"21\" NumDok=\"{series}{person.Number}\" DtDok=\"{person.Date}\" NameOrg=\"{person.IssuerOrgan}\" KodOrg=\"{person.KodOrg}\" Birthday=\"{person.BirthDate}\" Birthplace=\"{person.BirthPlace}\" IdSex=\"{person.Sex}\" IdVidCitizen=\"1\" />\r\n" +
                                        $"</FLObj>\r\n"
                                        );
                }

                // Добавляем нотариуса\врио как заявителя
                if (applicant)
                {
                    //series = Regex.Replace(notarius.Series, @"(.{2})", "$1 ");
                    //{series}{notarius.Number}
                    template = template.Append(
                                        $"<GOtpr IdVidOtpr=\"32\" Num=\"-1\" />\r\n" +
                                        $"<FLObj DtStart=\"\" DtEnd=\"\" Num=\"-1\" IdVidObj=\"303\">\r\n" +
                                            $"<FL FamFL=\"{notarius.LastName}\" NameFL=\"{notarius.FirstName}\" OtchFL=\"{notarius.MiddleName}\" INN=\"{notarius.INN}\" IdDokFL=\"\" NumDok=\"\" DtDok=\"{notarius.Date}\" NameOrg=\"{notarius.IssuerOrgan}\" KodOrg=\"{notarius.KodOrg}\" Birthday=\"{notarius.BirthDate}\" Birthplace=\"{notarius.BirthPlace}\" />\r\n" +
                                        $"</FLObj>\r\n" +
                                        $"<ContactObj DtStart=\"\" DtEnd=\"\" Num=\"-1\" IdVidObj=\"303\">\r\n" +
                                            $"<Contact Telefon=\"{ValidatePhoneNumber(notarius.Phone)}\" EMail=\"{notarius.Email}\" />\r\n" +
                                        $"</ContactObj>");
                }
            }
            var result = template.ToString();
            result = @"<Vx VidReestr=""1"" IdVidReg=""121011"" NewForm=""4"" DtStatus="""" DtStatusProv="""">" + "\n" + @"<UL DtOGRN="" DtCreate="" DtRegLast="" DtStatus="">" + "\n" + result;
            result += "</UL>\n</Vx>";

            return result.ToString();
        }

        private static Person[] CreateFounders(string inputXML)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Person[]));
            Person[] newpeople = null;

            using (TextReader reader = new StringReader(inputXML))
                newpeople = formatter.Deserialize(reader) as Person[];

            //var overrides = new OverrideXml()
            //     .Override<Person>()
            //         .XmlRoot("continent")
            //         .Member("LastName").XmlAttribute("test")
            //     .Commit();

            //XmlSerializer fs = new XmlSerializer(typeof(Person[]), overrides);
            //fs.Serialize(Console.Out, newpeople);

            return newpeople;
        }

        // Форматируем выгруженные данные из экспресса для сериализации
        private static string XmlFormatter(string path)
        {
            var files = Directory.GetFiles(@path, "*.xml");
            var stringList = new List<string>();

            // Объединяем все XML файлы
            foreach (var file in files)
                stringList.Add(File.ReadAllText(file));

            // Очищаем шаблон из экспресса от неиспользуемых тегов
            var resultText = string.Join("", stringList.ToArray());
            resultText = resultText.Replace("n1:", "").Replace("idDocument", "").Replace("importRightHolder", "");
            resultText = resultText.Replace(@" xmlns:n1=""http://fciit.ru/eisn/rr/entity-import/1.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""", "");
            resultText = resultText.Replace(@"?xml version=""1.0"" encoding=""UTF-8""?", "").Replace("<>", "").Replace("</>", "");
            int oldLength;
            do
            {
                oldLength = resultText.Length;
                resultText = resultText.Replace('\r', '\n');
                resultText = resultText.Replace("\n\n", "\n");
            } while (resultText.Length != oldLength);

            // Добавляем нужные теги для сериализации
            resultText = @"<?xml version=""1.0""?>" + "\n" + @"<ArrayOfPerson xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + resultText;
            resultText += @"</ArrayOfPerson>";

            return resultText;
        }

        // обработчик события добавления нового пользователя
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFIO.Text) &&
                !string.IsNullOrWhiteSpace(txtINN.Text) &&
                //!string.IsNullOrWhiteSpace(txtBirthDate.Text) &&
                //!string.IsNullOrWhiteSpace(txtBirthPlace.Text) &&
                //!string.IsNullOrWhiteSpace(txtPassport.Text) &&
                //!string.IsNullOrWhiteSpace(txtPassportDate.Text) &&
                //!string.IsNullOrWhiteSpace(txtIssuerOrgan.Text) &&
                //!string.IsNullOrWhiteSpace(txtKodOrg.Text) &&
                !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !string.IsNullOrWhiteSpace(txtPhone.Text)
                )
            {
                // добавляем нового пользователя в список
                //string passport = txtPassport.Text.Replace(" ", "").Replace("-", "");
                //if (string.IsNullOrWhiteSpace(passport) || passport.Length < 8) { System.Windows.MessageBox.Show("Проверьте правильность введенных серии и номера паспорта (формат: 1234 123456)"); return; }
                if (ValidateFIO(txtFIO.Text)) { System.Windows.MessageBox.Show("Проверьте правильность введенных ФИО"); return; }
                //string passport_series = passport.Substring(0, 4);
                //string passport_number = passport.Substring(4);

                if (btnNewEdit.Content.Equals("Добавить"))
                {
                    users.Add(new Person()
                    {
                        LastName = txtFIO.Text.Split()[0],
                        FirstName = txtFIO.Text.Split()[1],
                        MiddleName = txtFIO.Text.Split()[2],
                        INN = txtINN.Text,
                        //BirthDate = txtBirthDate.Text,
                        //BirthPlace = txtBirthPlace.Text,
                        //Series = passport_series,
                        //Number = passport_number,
                        //Date = txtPassportDate.Text,
                        //IssuerOrgan = txtIssuerOrgan.Text,
                        //KodOrg = txtKodOrg.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text
                    });

                    // обновляем отображение списка пользователей
                    UsersView.ItemsSource = null;
                    UsersView.ItemsSource = users;
                }
                else if (btnNewEdit.Content.Equals("Изменить"))
                {
                    (UsersView.SelectedItem as Person).LastName = txtFIO.Text.Split()[0];
                    (UsersView.SelectedItem as Person).FirstName = txtFIO.Text.Split()[1];
                    (UsersView.SelectedItem as Person).MiddleName = txtFIO.Text.Split()[2];
                    (UsersView.SelectedItem as Person).INN = txtINN.Text;
                    //(UsersView.SelectedItem as Person).BirthDate = txtBirthDate.Text;
                    //(UsersView.SelectedItem as Person).BirthPlace = txtBirthPlace.Text;
                    //(UsersView.SelectedItem as Person).Series = passport_series;
                    //(UsersView.SelectedItem as Person).Number = passport_number;
                    //(UsersView.SelectedItem as Person).Date = txtPassportDate.Text;
                    //(UsersView.SelectedItem as Person).IssuerOrgan = txtIssuerOrgan.Text;
                    //(UsersView.SelectedItem as Person).KodOrg = txtKodOrg.Text;
                    (UsersView.SelectedItem as Person).Email = txtEmail.Text;
                    (UsersView.SelectedItem as Person).Phone = txtPhone.Text;
                }

                // сохраняем данные о пользователях в файл
                SaveUsers(password, profile_filepath);

                // обновляем отображение списка пользователей
                UsersView.ItemsSource = null;
                UsersView.ItemsSource = users;

                DrawerLeft.IsOpen = false;
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            switch (step.StepIndex)
            {
                case 1:
                    buttonBack.IsEnabled = false;
                    spContent.Visibility = Visibility.Collapsed;
                    ButtonCreateXML.Visibility = Visibility.Visible;
                    break;

                case 2:
                    spContent.Visibility = Visibility.Visible;
                    spProfileList.Visibility = Visibility.Collapsed;
                    break;

                case 3:
                    spProfileList.Visibility = Visibility.Visible;
                    rtxtResult.Visibility = Visibility.Collapsed;
                    break;
            }

            step.Prev();
        }

        private void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            switch (step.StepIndex)
            {
                case 0:
                    LoadXML();
                    break;

                case 1:
                    spContent.Visibility = Visibility.Collapsed;
                    spProfileList.Visibility = Visibility.Visible;
                    break;

                case 2:
                    if ((btnApplicant.IsChecked == true) && (UsersView.SelectedItem == null))
                    {
                        System.Windows.MessageBox.Show("Заявитель не выбран");
                        return;
                    }

                    bool applicant = false;

                    spProfileList.Visibility = Visibility.Collapsed;
                    rtxtResult.Document.Blocks.Clear();
                    rtxtResult.Visibility = Visibility.Visible;

                    if (btnApplicant.IsChecked == true && (UsersView.SelectedItem != null))
                        applicant = true;

                    final_file = CreateDG2(founders, UsersView.SelectedItem as Person, applicant);

                    rtxtResult.AppendText(final_file);
                    break;

                case 3:
                    System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog
                    {
                        FileName = DateTime.Now.ToString("dd.MM.yyyy HH.mm"),
                        AddExtension = true,
                        Filter = "*.dg2|*.dg2"
                    };
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        File.WriteAllText(dialog.FileName, final_file);
                        Growl.SuccessGlobal(new hcd.GrowlInfo
                        {
                            Message = $"Файл успешно сохранен! \n {dialog.FileName}",
                            ShowDateTime = false,
                        });
                    }

                    break;
            }

            step.Next();
        }

        private void ClearProfileInput()
        {
            txtFIO.Text = "";
            txtINN.Text = "";
            //txtBirthDate.Text = "";
            //txtBirthPlace.Text = "";
            //txtPassport.Text = "";
            //txtPassportDate.Text = "";
            //txtIssuerOrgan.Text = "";
            //txtKodOrg.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is hc.ComboBox cb) // получаем комбобокс, который вызвал событие
            {
                int.TryParse(cb.Tag.ToString(), out int cbTag);
                System.Windows.MessageBox.Show(cb.SelectedIndex.ToString());
                founders[cbTag].IdStatus = cb.SelectedIndex + 1;
            }
        }

        // обработчик события удаления пользователя
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            // удаляем выбранного пользователя из списка
            users.Remove(UsersView.SelectedItem as Person);

            // сохраняем данные о пользователях в файл
            SaveUsers(password, profile_filepath);

            // обновляем отображение списка пользователей
            UsersView.ItemsSource = null;
            UsersView.ItemsSource = users;
        }

        // обработчик события изменения пользователя
        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersView.SelectedItem == null)
                return;

            if (UsersView.SelectedItem != null)
            {
                btnNewEdit.Content = "Изменить";
                txtFIO.Text = $"{(UsersView.SelectedItem as Person).LastName} {(UsersView.SelectedItem as Person).FirstName} {(UsersView.SelectedItem as Person).MiddleName}";
                txtINN.Text = (UsersView.SelectedItem as Person).INN;
                //txtBirthDate.Text = (UsersView.SelectedItem as Person).BirthDate;
                //txtBirthPlace.Text = (UsersView.SelectedItem as Person).BirthPlace;
                //txtPassport.Text = $"{(UsersView.SelectedItem as Person).Series} {(UsersView.SelectedItem as Person).Number}";
                //txtPassportDate.Text = (UsersView.SelectedItem as Person).Date;
                //txtIssuerOrgan.Text = (UsersView.SelectedItem as Person).IssuerOrgan;
                //txtKodOrg.Text = (UsersView.SelectedItem as Person).KodOrg;
                txtEmail.Text = (UsersView.SelectedItem as Person).Email;
                txtPhone.Text = (UsersView.SelectedItem as Person).Phone;
            }
        }

        private void FIOTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что вводимый символ является буквой
            if (!Regex.IsMatch(e.Text, "[а-яА-ЯёЁ]+"))
            {
                // Если не является, отменяем ввод
                e.Handled = true;
            }
        }

        // Валидация поля ИНН
        private void INNTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
            }
        }

        private void INNTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            hc.TextBox textBox = sender as hc.TextBox;
            if (textBox.Text.Length > 12)
            {
                int index = textBox.CaretIndex;
                textBox.Text = textBox.Text.Substring(0, 12);
                textBox.CaretIndex = index;
            }
        }

        // Валидация поля кода отделения паспортного стола
        private void KodOrgTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != '-')
            {
                e.Handled = true;
            }
        }

        private void KodOrgTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            hc.TextBox textBox = sender as hc.TextBox;
            if (textBox.Text.Length > 7)
            {
                int index = textBox.CaretIndex;
                textBox.Text = textBox.Text.Substring(0, 7);
                textBox.CaretIndex = index;
            }
        }

        // загружаем данные о пользователях из файла
        private void LoadUsers(string password, string filepath)
        {
            if (File.Exists(filepath))
            {
                byte[] salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
                byte[] key = new Rfc2898DeriveBytes(password, salt, 1000).GetBytes(32);
                byte[] iv = new byte[16];
                try
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        using (FileStream fs = new FileStream(filepath, FileMode.Open))
                        {
                            using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                using (StreamReader sr = new StreamReader(cs))
                                {
                                    string json = sr.ReadToEnd();
                                    users = JsonConvert.DeserializeObject<List<Person>>(json);
                                    if (users == null) { users = new List<Person>(); }
                                    UsersView.ItemsSource = users;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // сломался файл profile.dat?
                }
            }
        }

        private void LoadXML(bool next = false)
        {
            ButtonCreateXML.Visibility = Visibility.Collapsed;
            spContent.Visibility = Visibility.Visible;
            buttonBack.Visibility = Visibility.Visible;
            buttonBack.IsEnabled = true;
            buttonForward.Visibility = Visibility.Visible;

            if (next)
                step.Next();
        }

        // обработчик события удаления пользователя
        private void NewUserButton_Click(object sender, RoutedEventArgs e)
        {
            btnNewEdit.Content = "Добавить";
        }

        // Валидация поля паспортных данных
        private void PassportTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
            }
        }

        private void PassportTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            hc.TextBox textBox = sender as hc.TextBox;

            if (textBox.Text.Length > 11)
            {
                int index = textBox.CaretIndex;
                textBox.Text = textBox.Text.Substring(0, 11);
                textBox.CaretIndex = index;
            }
        }

        // Валидация поля телефонных номеров
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != '+')
            {
                e.Handled = true;
            }
        }

        // сохраняем данные о пользователях в файл
        private void SaveUsers(string password, string filepath)
        {
            string json = JsonConvert.SerializeObject(users);

            byte[] salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            byte[] key = new Rfc2898DeriveBytes(password, salt, 1000).GetBytes(32);
            byte[] iv = new byte[16];
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(json);
                        }
                    }
                }
            }
        }
        private void UsersView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersView.SelectedItem == null)
            {
                ClearProfileInput();
                btnChangeProfile.IsEnabled = false;
            }

            btnChangeProfile.IsEnabled = true;
        }
    }

    // Лучше использовать builder
    [XmlType("person")]
    public class Person
    {
        private DateTime birthDate_;
        private DateTime date_;
        private string sex_;
        public Person()
        { }

        public Person(string lastName, string firstName, string middleName, string birthDate, string birthPlace, string sex, string country, string inn, string snils, string address, string email, string phone, string number, string series, string issuerOrgan, string kodOrg, string date, string type, int id)
        {
            LastName = lastName;
            FirstName = firstName;
            MiddleName = middleName;
            BirthDate = birthDate;
            BirthPlace = birthPlace;
            Sex = sex;
            Country = country;
            INN = inn;
            Snils = snils;
            Address = address;
            Email = email;
            Phone = phone;
            Number = number;
            Series = series;
            IssuerOrgan = issuerOrgan;
            KodOrg = kodOrg;
            Date = date;
            Type = type;
            IdStatus = id;
        }

        [XmlElement(ElementName = "address")]
        public string Address { get; set; }

        [XmlElement(ElementName = "birthDate")]
        public string BirthDate
        {
            get => birthDate_.ToString("dd/MM/yyyy"); set => DateTime.TryParse(value, out birthDate_);
        }

        [XmlElement(ElementName = "birthPlace")]
        public string BirthPlace { get; set; }

        [XmlElement(ElementName = "country")]
        public string Country { get; set; }

        [XmlElement(ElementName = "date")]
        public string Date
        {
            get => date_.ToString("dd/MM/yyyy"); set => DateTime.TryParse(value, out date_);
        }

        [XmlElement(ElementName = "email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "firstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "id")]
        public int IdStatus { get; set; } = 1;

        [XmlElement(ElementName = "inn")]
        public string INN { get; set; }

        [XmlElement(ElementName = "issuerOrgan")]
        public string IssuerOrgan { get; set; }

        [XmlElement(ElementName = "kodOrg")]
        public string KodOrg { get; set; } = "";

        [XmlElement(ElementName = "lastName")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "middleName")]
        public string MiddleName { get; set; }
        [XmlElement(ElementName = "number")]
        public string Number { get; set; }

        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }

        [XmlElement(ElementName = "series")]
        public string Series { get; set; }

        [XmlElement(ElementName = "sex")]
        public string Sex
        {
            get => sex_; set
            {
                switch (value)
                {
                    case "м":
                        sex_ = "1";
                        break;

                    case "ж":
                        sex_ = "2";
                        break;

                    default:
                        sex_ = "";
                        break;
                }
            }
        }
        [XmlElement(ElementName = "snils")]
        public string Snils { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
    }
}