using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WorkWithContacts;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestForContact
    {
        [TestMethod]
        public void Contact_Norm()
        {
            var contact = new Contact("Иван Иванов", "89991234567");
            Assert.AreEqual("Иван Иванов", contact.Name);
            Assert.AreEqual("89991234567", contact.PhoneNumber);
        }
        [TestMethod]
        public void Contact_With_Spez_Symbols()
        {
            // Проверка в имени можно писать спец.символы
            var contact = new Contact("Иван @", "89991112233");
            Assert.AreEqual("Иван @", contact.Name);
        }

        [TestMethod]
        public void Contact_With_Numbers()
        {
            // Проверка в имени можно писать цифры
            var contact = new Contact("Иванов 123", "89994445566");
            Assert.AreEqual("Иванов 123", contact.Name);
        }

        [TestMethod]
        public void Contact_EmptyData()
        {
            var contact = new Contact("", "");
            //Проверка пустых значений в имени и телефоне
            Assert.AreEqual("", contact.Name);
            Assert.AreEqual("", contact.PhoneNumber);
        }
    }
}
