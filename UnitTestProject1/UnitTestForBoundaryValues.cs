using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using WorkWithContacts;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTeUnitTestForBoundaryValuesst1
    {
        [TestMethod]
        public void Contact_VeryLongName_IsAccepted()
        {
            // Arrange
            string longName = new string('A', 1000); // Имя длиной 1000 символов

            // Act
            var contact = new Contact(longName, "89991234567");

            // Assert
            Assert.AreEqual(1000, contact.Name.Length);
        }

        [TestMethod]
        public void ContactManager_AddDuplicate_AllowsDuplicates()
        {
            // Arrange
            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
            var manager = new ContactManager();
            var contact = new Contact("Дубликат", "89990001122");

            // Act
            manager.AddContact(contact);
            manager.AddContact(contact); // Добавляем тот же контакт

            // Assert - текущая логика разрешает дубликаты
            Assert.AreEqual(2, manager.Contacts.Count);

            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
        }

        [TestMethod]
        public void SearchContacts_CaseSensitive_Search()
        {
            // Arrange
            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
            var manager = new ContactManager();
            manager.AddContact(new Contact("Иван", "89991234567"));

            // Act - поиск в нижнем регистре
            var results = manager.SearchContacts("иван");

            // Assert - поиск регистрозависимый (Contains), поэтому ничего не найдет
            Assert.AreEqual(0, results.Count);

            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
        }
    }
}
