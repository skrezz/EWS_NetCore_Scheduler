using EWS_NetCore_Scheduler.Service;
using EWS_NetCore_Scheduler.Interfaces;
using Moq;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Exchange.WebServices.Data;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using EWS_NetCore_Scheduler.Models;
using Xunit;

namespace xUnitTests
{
    public class SchedulingServiceTests
    {
        private readonly SchedulingService _shedServTest;
        private readonly Mock<IEWSActing> _IEWSActingMock = new Mock<IEWSActing>();
        public SchedulingServiceTests()
        {
            _shedServTest =  new SchedulingService(_IEWSActingMock.Object);
        }
        [Fact]
        public void DelAppo_WithRightId(string userLogin)
        {
            //arrange
            var id = Guid.NewGuid();
            //Act
            var res= _shedServTest.DelAppo(id.ToString(),userLogin);
            //assert
            Assert.Equal("deleted", res);
        }

        [Fact]
        public void GetAppo_WithRightDate(string userLogin)
        {
            //arrange
            var startDate = new DateTime();
            var appo = new Appo[0];
            string[] Ids = new string[]
            {
                new Guid().ToString(),
                new Guid().ToString()
            };
            var Jsonappo = new JsonResult(appo);
            /*FindItemsResults<Item> findItem =_IEWSActingMock.Object.FindAppointments(_IEWSActingMock.Object.CrEwsService());

            findItem.Items.Add(new Appointment(_IEWSActingMock.Object.CrEwsService()));
            
            _IEWSActingMock.Setup(x => x.FindAppointments(_IEWSActingMock.Object.CrEwsService())).Returns(findItem);*/
                
            //Act
            var res = _shedServTest.GetAppos(Ids, startDate.ToString(),"",userLogin);
            //assert
            Assert.Equal(Jsonappo.Value, res.Value);
        }
    }
}