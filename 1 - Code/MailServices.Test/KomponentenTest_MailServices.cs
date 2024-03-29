﻿using System;
using System.Net;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Util.MailServices.Implementations;
using Util.MailServices.Interfaces;

namespace Tests.KomponentenTest.MailServices
{
    [TestClass]
    public class KomponentenTest_MailServices
    {
        private static IMailServices mailServices = null;

        [ClassInitialize]
        public static void SetUp(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();

            NetworkCredential nc = new NetworkCredential("abj798", "blub");
            mailServices = MailServicesFactory.CreateMailServices();
            mailServices.SetCredentials(nc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSendMailFail()
        {
            mailServices.SendMail(null);
        }
    }
}
