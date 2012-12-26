using System;
using Referee.ViewModels;
using Referee.Models;
using Xunit;

namespace xUnitTestLibrary.ViewModels
{
    
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

        [Fact]
        public void GetReferee()
        {
            Assert.IsType(typeof(string), this._nominationMessage.GetReferee());
            Assert.NotNull(this._nominationMessage.GetReferee());
        }

        [Fact]
        public void GetEventType()
        {
            string EventType = _nominationMessage.Type == "game" ? "mecz" : "turniej";
            Assert.True(EventType == _nominationMessage.GetEventType());
        }
    }
}
