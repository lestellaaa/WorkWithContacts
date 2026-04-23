using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using WorkWithContacts;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestForContactManager
    {
        private ContactManager _manager;
        private const string TestFile = "contacts.txt";

        [TestInitialize]
        public void Setup()
        {
            // Перед каждым тестом удаляем файл, чтобы тесты были чистыми
            if (File.Exists(TestFile)) File.Delete(TestFile);
            _manager = new ContactManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // После каждого теста удаляем файл
            if (File.Exists(TestFile)) File.Delete(TestFile);
        }

        [TestMethod]
        public void AddContact_ValidContact_IncreasesCount()
        {
            var contact = new Contact("Петр", "89001112233");
            int initialCount = _manager.Contacts.Count;
            _manager.AddContact(contact);
            Assert.AreEqual(initialCount + 1, _manager.Contacts.Count);
        }

        [TestMethod]
        public void AddContact_Null_ThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _manager.AddContact(null));
        }

        [TestMethod]
        public void RemoveContact_ExistingContact_DecreasesCount()
        {
            var contact = new Contact("Петр", "89001112233");
            _manager.AddContact(contact);
            int initialCount = _manager.Contacts.Count;
            _manager.RemoveContact(contact);
            Assert.AreEqual(initialCount - 1, _manager.Contacts.Count);
        }

        [TestMethod]
        public void SearchContacts_ByName_FindsMatch()
        {
            _manager.AddContact(new Contact("Иван", "89991112233"));
            _manager.AddContact(new Contact("Петр", "89994445566"));
            var results = _manager.SearchContacts("Иван");
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Иван", results[0].Name);
        }

        [TestMethod]
        public void SearchContacts_ByPhone_FindsMatch()
        {
            _manager.AddContact(new Contact("Иван", "89991112233"));
            var results = _manager.SearchContacts("89991112233");
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void SearchContacts_NoMatch_ReturnsEmpty()
        {
            _manager.AddContact(new Contact("Иван", "89991112233"));
            var results = _manager.SearchContacts("Никто");
            Assert.AreEqual(0, results.Count);
        }
    }
}
