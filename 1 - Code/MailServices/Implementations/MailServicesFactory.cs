using System;
using System.Net;
using log4net;
using Util.Common.Exceptions;
using Util.MailServices.Interfaces;

namespace Util.MailServices.Implementations
{
    public class MailServicesFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static IMailServices instance = null;

        public static IMailServices CreateMailServices()
        {
            try
            {
                return instance ?? (instance = new SmtpMailServices());
            }
            catch (Exception ex)
            {
                var tpEx = new TechnicalProblemException("Error creating smtp mail provider.", ex);
                Log.Error(tpEx.ToString());
                throw tpEx;
            }            
        }
    }
}
