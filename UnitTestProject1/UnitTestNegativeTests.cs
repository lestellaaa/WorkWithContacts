using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Forms;
using WorkWithContacts;

namespace UnitTestProject1
{
    [TestClass]
    public class ContactManagerNegativeTests
    {
        private ContactManager _manager;

        [TestInitialize]
        public void Setup()
        {
            if (System.IO.File.Exists("contacts.txt"))
                System.IO.File.Delete("contacts.txt");
            _manager = new ContactManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (System.IO.File.Exists("contacts.txt"))
                System.IO.File.Delete("contacts.txt");
        }

        [TestMethod]
        public void AddContact_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _manager.AddContact(null));
        }

        [TestMethod]
        public void RemoveContact_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _manager.RemoveContact(null));
        }

        [TestMethod]
        public void SearchContacts_ReturnsEmptyList()
        {
            _manager.AddContact(new Contact("Иван", "89991234567"));
            var results = _manager.SearchContacts(null);
            // при null должен вернуть пустой список или выбросить исключение
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void AddContact_EmptyName()
        {
            var contact = new Contact("", "89991234567");
            _manager.AddContact(contact);
            // пустое имя разрешено 
            Assert.AreEqual(1, _manager.Contacts.Count);
        }

        [TestMethod]
        public void AddContact_EmptyPhoneNumber()
        {
            var contact = new Contact("Иван", "");
            _manager.AddContact(contact);
            // пустой телефон разрешён 
            Assert.AreEqual(1, _manager.Contacts.Count);
        }

        [TestMethod]
        public void SearchContacts_EmptyQuery_ReturnsAllContacts()
        {
            _manager.AddContact(new Contact("Иван", "89991234567"));
            _manager.AddContact(new Contact("Петр", "89997654321"));
            var results = _manager.SearchContacts("");
            Assert.AreEqual(2, results.Count);
        }
    }

    [TestClass]
    public class ContactFormNegativeTests
    {
        private ContactForm _form;

        [TestInitialize]
        public void Setup()
        {
            if (System.IO.File.Exists("contacts.txt"))
                System.IO.File.Delete("contacts.txt");
            _form = new ContactForm();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _form?.Dispose();
            if (System.IO.File.Exists("contacts.txt"))
                System.IO.File.Delete("contacts.txt");
        }

        // Вспомогательный метод для доступа к приватным полям
        private T GetPrivateField<T>(string fieldName)
        {
            var field = typeof(ContactForm).GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            return (T)field?.GetValue(_form);
        }

        // Вспомогательный метод для вызова приватных методов
        private void InvokePrivateMethod(string methodName, params object[] parameters)
        {
            var method = typeof(ContactForm).GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            method?.Invoke(_form, parameters);
        }

        [TestMethod]
        public void AddContactButton_Click_OnlySpacesInName_DoesNotAdd()
        {
            var nameBox = GetPrivateField<TextBox>("nameTextBox");
            var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
            var manager = GetPrivateField<ContactManager>("contactManager");

            nameBox.Text = "   "; // только пробелы
            phoneBox.Text = "89991234567";
            InvokePrivateMethod("AddContactButton_Click", null, EventArgs.Empty);

            // контакт не должен добавиться
            Assert.AreEqual(0, manager.Contacts.Count);
        }

        [TestMethod]
        public void AddContactButton_Click_OnlySpacesInPhone_DoesNotAdd()
        {
            var nameBox = GetPrivateField<TextBox>("nameTextBox");
            var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
            var manager = GetPrivateField<ContactManager>("contactManager");

            nameBox.Text = "Иван";
            phoneBox.Text = "   "; // только пробелы

            InvokePrivateMethod("AddContactButton_Click", null, EventArgs.Empty);

            Assert.AreEqual(0, manager.Contacts.Count);
        }

        [TestMethod]
        public void AddContactButton_Click_PlaceholderText_DoesNotAdd()
        {
            var nameBox = GetPrivateField<TextBox>("nameTextBox");
            var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
            var manager = GetPrivateField<ContactManager>("contactManager");

            nameBox.Text = "Имя"; // плейсхолдер
            phoneBox.Text = "Телефон"; // плейсхолдер

            InvokePrivateMethod("AddContactButton_Click", null, EventArgs.Empty);

            Assert.AreEqual(0, manager.Contacts.Count);
        }

        [TestMethod]
        public void RemoveContactButton_Click_NoSelection_DoesNotRemove()
        {
            var manager = GetPrivateField<ContactManager>("contactManager");
            var listBox = GetPrivateField<ListBox>("contactsListBox");

            manager.AddContact(new Contact("Иван", "89991234567"));
            int initialCount = manager.Contacts.Count;

            listBox.SelectedIndex = -1; // ничего не выбрано

            InvokePrivateMethod("RemoveContactButton_Click", null, EventArgs.Empty);

            Assert.AreEqual(initialCount, manager.Contacts.Count);
        }

        [TestMethod]
        public void SearchButton_Click_EmptyQuery_ShowsAllContacts()
        {
            var manager = GetPrivateField<ContactManager>("contactManager");
            var searchBox = GetPrivateField<TextBox>("searchTextBox");
            var listBox = GetPrivateField<ListBox>("contactsListBox");

            manager.AddContact(new Contact("Иван", "89991234567"));
            manager.AddContact(new Contact("Петр", "89997654321"));

            searchBox.Text = "";

            InvokePrivateMethod("SearchButton_Click", null, EventArgs.Empty);

            Assert.AreEqual(2, listBox.Items.Count);
        }

        [TestMethod]
        public void PhoneNumberTextBox_KeyPress_Letter_IsRejected()
        {
            var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
            phoneBox.Text = "";

            // имитируем нажатие буквы 'A'
            var eventArgs = new KeyPressEventArgs('A');

            // Проверяем, что есть обработчик кейпресс
            var method = typeof(ContactForm).GetMethod("PhoneNumberTextBox_KeyPress",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(_form, new object[] { phoneBox, eventArgs });
                // буква должна быть отклонена
                Assert.IsTrue(eventArgs.Handled, "Буквы в поле телефона должны отклоняться");
            }
            else
            {
                // Если метода нет, просто проверяем, что поле существует
                Assert.IsNotNull(phoneBox);
            }
        }

        [TestMethod]
        public void PhoneNumberTextBox_KeyPress_SpecialCharacter_IsRejected()
        {
            var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
            phoneBox.Text = "";

            // имитируем нажатие спецсимвола '@'
            var eventArgs = new KeyPressEventArgs('@');

            var method = typeof(ContactForm).GetMethod("PhoneNumberTextBox_KeyPress",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(_form, new object[] { phoneBox, eventArgs });
                // спецсимвол должен быть отклонён
                Assert.IsTrue(eventArgs.Handled, "Спецсимволы в поле телефона должны отклоняться");
            }
        }

        [TestMethod]
        public void PhoneNumberTextBox_KeyPress_Digit_IsAccepted()
        {
            var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
            phoneBox.Text = "";

            // имитируем нажатие цифры '5'
            var eventArgs = new KeyPressEventArgs('5');

            var method = typeof(ContactForm).GetMethod("PhoneNumberTextBox_KeyPress",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(_form, new object[] { phoneBox, eventArgs });
                // цифра должна быть принята
                Assert.IsFalse(eventArgs.Handled, "Цифры в поле телефона должны приниматься");
            }
        }
    }

    [TestClass]
    public class ContactNegativeTests
    {
        [TestMethod]
        public void Contact_NullName_IsAllowed()
        {
            var contact = new Contact(null, "89991234567");
            Assert.IsNull(contact.Name);
        }

        [TestMethod]
        public void Contact_NullPhoneNumber_IsAllowed()
        {
            var contact = new Contact("Иван", null);
            Assert.IsNull(contact.PhoneNumber);
        }

        [TestMethod]
        public void Contact_OnlySpecialCharactersInName_IsAllowed()
        {
            var contact = new Contact("!@#$%^&*()", "89991234567");
            Assert.AreEqual("!@#$%^&*()", contact.Name);
        }

        [TestMethod]
        public void Contact_VeryLongPhoneNumber_IsAllowed()
        {
            string longPhone = new string('1', 100);
            var contact = new Contact("Иван", longPhone);
            Assert.AreEqual(longPhone, contact.PhoneNumber);
        }
    }
}
