using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VwM.Authorization
{
    public sealed class Context : IContext
    {
        private readonly ILogger<Context> _logger;
        private readonly Options _options;


        #region ctor
        public Context(ILogger<Context> logger, IOptions<Options> options)
        {
            _logger = logger;
            _options = options.Value;
        }
        #endregion


        public User GetUser(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
                throw new ArgumentNullException(nameof(login));

            User result = null;

            try
            {
                if (login == _options.Login && password == _options.Password)
                {
                    result = new User
                    {
                        Login = login,
                        DisplayName = "Administrator"
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to validate user '{login}'.");
            }

            return result;
        }
    }
}
