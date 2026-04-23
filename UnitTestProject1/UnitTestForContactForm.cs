using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using WorkWithContacts;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestForContactForm
    {
        [TestClass]
        public class ContactFormTests
        {
            private ContactForm _form;

            [TestInitialize]
            public void Setup()
            {
                // Удаляю файл контактов перед тестами формы
                if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
                _form = new ContactForm();
            }

            [TestCleanup]
            public void Cleanup()
            {
                _form?.Dispose();
                if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
            }

            // Вспомогательный метод для доступа к приватным полям формы
            private T GetPrivateField<T>(string fieldName)
            {
                var field = typeof(ContactForm).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                return (T)field.GetValue(_form);
            }

            // Вспомогательный метод для вызова приватных методов
            private void InvokePrivateMethod(string methodName, params object[] parameters)
            {
                var method = typeof(ContactForm).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(_form, parameters);
            }

            [TestMethod]
            public void UIElements_AreVisibleAndEnabled()
            {
                // Проверяем, что элементы управления существуют и видны
                var nameBox = GetPrivateField<TextBox>("nameTextBox");
                var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
                var addButton = GetPrivateField<Button>("addContactButton");
                var listBox = GetPrivateField<ListBox>("contactsListBox");

                Assert.IsTrue(nameBox.Visible);
                Assert.IsTrue(phoneBox.Visible);
                Assert.IsTrue(addButton.Visible);
                Assert.IsTrue(listBox.Visible);
            }

            [TestMethod]
            public void AddContactButton_AddContact()
            {
                var nameBox = GetPrivateField<TextBox>("nameTextBox");
                var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
                var manager = GetPrivateField<ContactManager>("contactManager");

                nameBox.Text = "Анна";
                phoneBox.Text = "89998887766";
                InvokePrivateMethod("AddContactButton_Click", null, EventArgs.Empty);
                Assert.AreEqual(1, manager.Contacts.Count);
            }

            [TestMethod]
            public void AddContactButton_ShowsWarning_WithEmpty()
            {
                var nameBox = GetPrivateField<TextBox>("nameTextBox");
                var phoneBox = GetPrivateField<TextBox>("phoneNumberTextBox");
                var manager = GetPrivateField<ContactManager>("contactManager");

                nameBox.Text = ""; // Пустое имя
                phoneBox.Text = "89998887766";
                InvokePrivateMethod("AddContactButton_Click", null, EventArgs.Empty);
                // контакт не должен добавиться
                Assert.AreEqual(0, manager.Contacts.Count);
            }

            [TestMethod]
            public void RemoveContactButton_ShowsWarning_DontChooseContact()
            {
                var listBox = GetPrivateField<ListBox>("contactsListBox");
                var manager = GetPrivateField<ContactManager>("contactManager");

                // Добавим контакт, но не выберем его
                manager.AddContact(new Contact("Иван", "89991112233"));
                listBox.SelectedIndex = -1; // Ничего не выбрано
                InvokePrivateMethod("RemoveContactButton_Click", null, EventArgs.Empty);

                // контакт должен остаться (удаление не произошло)
                Assert.AreEqual(1, manager.Contacts.Count);
            }
        }
    }
}
