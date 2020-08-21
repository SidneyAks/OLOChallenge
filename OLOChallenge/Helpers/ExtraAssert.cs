using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Helpers
{
    public static class ExtraAssert
    {
        public static void Throws<T>(Action action, string Message = null) where T : Exception
        {
            Exception CaughtEx = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                CaughtEx = ex;
            }
            if (CaughtEx is null)
            {
                throw new AssertFailedException($"Assert.Throws failed. No Exception Was Thrown. {Message}");
            } else if (!(CaughtEx is T))
            {
                throw new AssertFailedException($"Assert.Throws failed. Expected: {typeof(T).Name}, Actual: {CaughtEx.GetType().Name}. {Message}", CaughtEx);
            }
        }


        public static void Succeeds(Action Action, string Message = null)
        {
            try
            {
                Action();
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"Assert.Succeeds failed. Action did not succeed. {Message}", ex);
            }
        }

        public static T Succeeds<T>(Func<T> Function, string Message = null)
        {
            try
            {
                return Function();
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"Assert.Succeeds failed. Action did not succeed. {Message}", ex);
            }
        }

        public static void PreconditionSucceeds(Action Action, string Message = null)
        {
            try
            {
                Action();
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"Assert.Precondition failed. {Message}", ex);
            }
        }
    }
}
