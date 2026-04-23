using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WorkWithContacts;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTeUnitTestForBoundaryValuesst1
    {
        [TestMethod]
        public void Contact_VeryLongName()
        {
            string longName = new string('A', 1000); // длинное имя 
            var contact = new Contact(longName, "89991234567");
            Assert.AreEqual(1000, contact.Name.Length);
        }

        [TestMethod]
        public void AddDuplicate()
        {
            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
            var manager = new ContactManager();
            var contact = new Contact("Дубликат", "89990001122");
            manager.AddContact(contact);
            manager.AddContact(contact); // Добавляем тот же контакт

            // логика разрешает дубликаты
            Assert.AreEqual(2, manager.Contacts.Count);

            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
        }

        [TestMethod]
        public void SearchContacts()
        {
            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
            var manager = new ContactManager();
            manager.AddContact(new Contact("Иван", "89991234567"));

            // поиск в нижнем регистре
            var results = manager.SearchContacts("иван");

            // поиск регистрозависимый, поэтому ничего не найдет
            Assert.AreEqual(0, results.Count);

            if (File.Exists("contacts.txt")) File.Delete("contacts.txt");
        }
    }
}
