//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Net.Http;
//using System.Net;

//namespace MessageMicroService.Tests
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
//        public void TestMethod1()
//        {
//            FakeResponseHandler fakeResponseHandler = new FakeResponseHandler();
//            // Add hardcoded external service responses
//            fakeResponseHandler.AddFakeResponse(
//                new Uri("http://localhost:54330/api/ProductStore/" + 1234),
//                new HttpResponseMessage(HttpStatusCode.OK)
//                {
//                    Content = new ObjectContent<>(
//                        new TPRProduct()
//                        {
//                            Name = "ProductName",
//                            Description = "A product description.",
//                            Ean = "1234",
//                            InStock = true,
//                            Price = 0.00,
//                            BrandId = 1,
//                            CategoryId = 2,
//                            Active = true
//                        }, new JsonMediaTypeFormatter())
//                });
//        }
//    }
//}
