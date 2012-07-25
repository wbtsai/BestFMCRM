using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace BestRadioNet.Core.Helper
{
    public static class LogHelper
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ToLog
        /// using Delegate method 
        /// </summary>
        /// <typeparam name="T">return T Type</typeparam>
        /// <param name="method">Ref methods</param>
        /// <returns></returns>
        public static T ToLog<T>(Func<Logger, T> method)
        {
            return ToLog2(string.Empty, method, null);
        }

        /// <summary>
        /// To Log for empty title
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static T ToLog<T>(string title, Func<Logger, T> method)
        {
            return ToLog2(title, method, null);
        }

        /// <summary>
        /// ToLog 
        /// using catch msgEx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="method"></param>
        /// <param name="msgEx"></param>
        /// <returns></returns>
        public static T ToLog<T>(string title, Func<Logger, T> method, Action<Exception> msgEx)
        {
            return ToLog2<T>(title, method, msgEx);
        }

        /// <summary>
        /// ToLog
        /// using empty title
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="msgEx"></param>
        /// <returns></returns>
        public static T ToLog<T>(Func<Logger, T> method, Action<Exception> msgEx)
        {
            return ToLog2<T>(string.Empty, method, msgEx);
        }


        /// <summary>
        /// void ToLog
        /// </summary>
        /// <param name="title"></param>
        /// <param name="method"></param>
        public static void ToLog(string title, Action<Logger> method)
        {
            ToLog2(title, method, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public static void ToLog(Action<Logger> method)
        {
            ToLog2(string.Empty, method, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="method"></param>
        /// <param name="msg"></param>
        public static void ToLog(string title, Action<Logger> method, Action<Exception> msg)
        {
            ToLog2(title, method, msg);

        }

        public static void ToLog(Action<Logger> method, Action<Exception> msg)
        {
            ToLog2(string.Empty, method, msg);
        }

        private static T ToLog2<T>(string title, Func<Logger, T> method, Action<Exception> msgEx)
        {
            return ToLogMessage<T>(title, method, msgEx);
        }

        private static void ToLog2(string title, Action<Logger> method, Action<Exception> msg)
        {
            ToLogMessage<string>(title, logger =>
            {
                method(logger);
                return "";
            },
                msg
            );
        }

        /// <summary>
        /// ToLog Message 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="method"></param>
        /// <param name="msgEx"></param>
        /// <returns></returns>
        private static T ToLogMessage<T>(string title, Func<Logger, T> method, Action<Exception> msgEx)
        {
            // get call stack
            StackTrace stackTrace = new StackTrace();

            var loggerName = stackTrace.GetFrame(3).GetMethod().DeclaringType.FullName;

            log = LogManager.GetLogger(loggerName);

            // get calling method name
            var methodName = stackTrace.GetFrame(3).GetMethod().Name;


            log.Info("------------------------------------------");

            if (!string.IsNullOrEmpty(title))
            {
                log.Info(title);
            }

            log.Info("Enter " + methodName);
            log.Info("             ");

            try
            {
                return method(log);
            }
            catch (Exception ex)
            {

                string errMsg = "";

                if (ex.InnerException != null)
                {
                    errMsg += ex.InnerException.Message;
                }

                log.Error("Exception:" + ex.Message + errMsg + ex.StackTrace);

                if (msgEx != null)
                {
                    msgEx(ex);
                }
                else
                {
                    throw ex;
                }

            }
            finally
            {
                log.Info("             ");
                log.Info("Leave " + methodName);
                log.Info("----------------------------------------------");
            }

            return default(T);
        }
    }
}
