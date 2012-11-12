using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Referee.ViewModels;
using Referee.Models;

namespace RefereeTest.ViewModels
{
    [TestClass]
    public class NominationMessageTest
    {
        NominationMessage _nominationMessage;
        RefereeEntity _referee;


        public NominationMessageTest()
        {
            this._referee = new RefereeEntity() { Id = Guid.NewGuid(), FirstName = "Michał", LastName = "Jagusiak" };
            this._nominationMessage = new NominationMessage()
            {
                Type = "game",
                Mailadr = "jagmaster@o2.pl",
                NominationId = 1231,
                RefereeId = this._referee.Id,
                HashConfirmation = "somehash"
            };
        }

        [TestMethod]
        public void GetReferee()
        {
            Assert.IsInstanceOfType(this._nominationMessage.GetReferee(), typeof(string));
            Assert.IsNotNull(this._nominationMessage.GetReferee());
        }

        [TestMethod]
        public void GetEventType()
        {
            string EventType = _nominationMessage.Type == "game" ? "mecz" : "turniej";
            Assert.IsTrue(EventType == _nominationMessage.GetEventType());
        }
    }
}
