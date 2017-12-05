using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net;
using MessagingMicroService.Controllers;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MessageMicroService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        MessagesMVCController MvcController;
        MessagesController ApiController;
        Mock<MessageContext> mockContext;
        Message testMessage1;
        Message testMessage2;
        List<Message> testMessageList;
        Mock<DbSet<Message>> dbSet;

        [TestInitialize]
        public void Initializer()
        {
            testMessage1 = new Message()
            {
                MessageID = 1,
                Title = "Test Title",
                MessageContent = "Test content",
                DateSent = DateTime.Now,
                ReceiverUserID = "1",
                SenderUserID = "2"
            };

            testMessage2 = new Message()
            {
                MessageID = 2,
                Title = "Test2 Title",
                MessageContent = "Test2 content",
                DateSent = DateTime.Now,
                ReceiverUserID = "3",
                SenderUserID = "4"
            };

            testMessageList = new List<Message>()
            {
                testMessage1,
                testMessage2
            };

            dbSet = new Mock<DbSet<Message>>();
            dbSet.As<IQueryable<Message>>().Setup(q => q.Provider).Returns(() => testMessageList.AsQueryable().Provider);
            dbSet.As<IQueryable<Message>>().Setup(q => q.Expression).Returns(() => testMessageList.AsQueryable().Expression);
            dbSet.As<IQueryable<Message>>().Setup(q => q.ElementType).Returns(() => testMessageList.AsQueryable().ElementType);
            dbSet.As<IQueryable<Message>>().Setup(q => q.GetEnumerator()).Returns(() => testMessageList.AsQueryable().GetEnumerator());
            dbSet.Setup(x => x.Add(It.IsAny<Message>())).Callback<Message>(testMessageList.Add);
            dbSet.Setup(set => set.AddRange(It.IsAny<IEnumerable<Message>>())).Callback<IEnumerable<Message>>(testMessageList.AddRange);
            dbSet.Setup(set => set.Remove(It.IsAny<Message>())).Callback<Message>(t => testMessageList.Remove(t));
            dbSet.Setup(set => set.RemoveRange(It.IsAny<IEnumerable<Message>>())).Callback<IEnumerable<Message>>(ts =>
            {
                foreach (var t in ts) { testMessageList.Remove(t); }
            });

            mockContext = new Mock<MessageContext>(new DbContextOptions<MessageContext>());
            mockContext.Setup(x => x.Messages).Returns(dbSet.Object);

            var testClaims = new ClaimsPrincipal(new ClaimsIdentity(TokenGenerator.UserToken("Customer").Claims));

            ApiController = new MessagesController(mockContext.Object);
            ApiController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = testClaims
                }
            };

            MvcController = new MessagesMVCController(mockContext.Object);
            MvcController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = testClaims
                }
            };
        }

        [TestMethod]
        public void ApiGetAllMessageSuccess()
        {
            var response = (ObjectResult)ApiController.Get();
            Assert.AreEqual(dbSet.Object, response.Value);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiGetMessageByMessageIDSuccess()
        {
            var response = (ObjectResult)ApiController.Get(1);
            Assert.AreEqual(testMessage1, response.Value);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiGetMessageByMessageIDFail()
        {
            var response = (ObjectResult)ApiController.Get(1);
            Assert.AreNotEqual(testMessage2, response.Value);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiGetMessageByUserIDSuccess()
        {
            var response = (ObjectResult)ApiController.GetByUser("1");
            var list = (IQueryable<Message>)response.Value;
            Assert.AreEqual(testMessage1.MessageID, list.First().MessageID);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiGetMessageByUserIDFail()
        {
            var response = (ObjectResult)ApiController.GetByUser("1");
            var list = (IQueryable<Message>)response.Value;
            Assert.AreNotEqual(testMessage2, list.First());
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiSaveMessageSuccess()
        {
            var response = (OkResult)ApiController.SaveMessage(new Message()
            {
                MessageID = 3,
                Title = "Test Message 3",
                MessageContent = "Test Content 3",
                DateSent = DateTime.Now,
                ReceiverUserID = "5",
                SenderUserID = "6"
            });
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiSaveMessageFail()
        {
            var response = (NotFoundResult)ApiController.SaveMessage(null);
            Assert.AreEqual(404, response.StatusCode);
        }

        [TestMethod]
        public void ApiPutSuccess()
        {
            var response = (OkResult)ApiController.Put(2, new Message()
            {
                MessageID = 2,
                Title = "Test Message 2 Update",
                MessageContent = "Test Content 2 Update",
                DateSent = DateTime.Now,
                ReceiverUserID = "3",
                SenderUserID = "4"
            });
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiPutFailBadRequest()
        {
            var response = (BadRequestResult)ApiController.Put(1, null);
            Assert.AreEqual(400, response.StatusCode);
        }

        [TestMethod]
        public void ApiPutFailNotFound()
        {
            var response = (NotFoundResult)ApiController.Put(7, new Message()
            {
                MessageID = 7,
                Title = "Test Message 2 Update",
                MessageContent = "Test Content 2 Update",
                DateSent = DateTime.Now,
                ReceiverUserID = "3",
                SenderUserID = "4"
            });
            Assert.AreEqual(404, response.StatusCode);
        }

        [TestMethod]
        public void ApiDeleteSuccess()
        {
            var response = (OkResult)ApiController.Delete(2);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ApiDeleteFailNotFound()
        {
            var response = (NotFoundResult)ApiController.Delete(7);
            Assert.AreEqual(404, response.StatusCode);
        }
    }
}
