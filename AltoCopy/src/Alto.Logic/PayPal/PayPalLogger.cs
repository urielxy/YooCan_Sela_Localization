using System;
using PayPal.Log;
using Serilog;
using Serilog.Events;

namespace Alto.Logic.PayPal
{
    public class PayPalLogger : BaseLogger
    {
        private readonly ILogger _logger;

        public override bool IsDebugEnabled => false;
        public override bool IsErrorEnabled => _logger.IsEnabled(LogEventLevel.Error);
        public override bool IsInfoEnabled => false;
        public override bool IsWarnEnabled => _logger.IsEnabled(LogEventLevel.Warning);

        public PayPalLogger(Type typeGiven) : base(typeGiven)
        {
            _logger = Log.Logger;
        }

        public override void Debug(string message)
        {
            _logger.Debug(message);
        }

        public override void Debug(string message, Exception exception)
        {
            _logger.Debug(exception, message);
        }

        public override void DebugFormat(string format, params object[] args)
        {
            _logger.Debug(format, args);
        }

        public override void Error(string message)
        {
            _logger.Error(message);
        }

        public override void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
        }

        public override void ErrorFormat(string format, params object[] args)
        {
            _logger.Error(format, args);
        }

        public override void Info(string message)
        {
            _logger.Information(message);
        }

        public override void Info(string message, Exception exception)
        {
            _logger.Information(exception, message);
        }

        public override void InfoFormat(string format, params object[] args)
        {
            _logger.Information(format, args);
        }

        public override void Warn(string message)
        {
            _logger.Warning(message);
        }

        public override void Warn(string message, Exception exception)
        {
            _logger.Warning(exception, message);
        }

        public override void WarnFormat(string format, params object[] args)
        {
            _logger.Warning(format, args);
        }
    }
}
