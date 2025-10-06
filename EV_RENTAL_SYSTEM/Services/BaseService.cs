using EV_RENTAL_SYSTEM.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace EV_RENTAL_SYSTEM.Services
{
    /// <summary>
    /// Base service providing common functionality for all services
    /// </summary>
    public abstract class BaseService
    {
        protected readonly ILogger _logger;

        protected BaseService(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Create a success response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Response data</param>
        /// <param name="message">Success message</param>
        /// <returns>Success response</returns>
        protected ServiceResponse<T> SuccessResult<T>(T data, string message = "Success")
        {
            return ServiceResponse<T>.SuccessResult(data, message);
        }

        /// <summary>
        /// Create an error response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="message">Error message</param>
        /// <param name="errors">Additional errors</param>
        /// <returns>Error response</returns>
        protected ServiceResponse<T> ErrorResult<T>(string message, List<string>? errors = null)
        {
            return ServiceResponse<T>.ErrorResult(message, errors);
        }

        /// <summary>
        /// Handle exception and return error response
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="ex">Exception</param>
        /// <param name="operation">Operation name for logging</param>
        /// <param name="userMessage">User-friendly error message</param>
        /// <returns>Error response</returns>
        protected ServiceResponse<T> HandleException<T>(Exception ex, string operation, string userMessage = "An error occurred")
        {
            _logger.LogError(ex, "Error in {Operation}", operation);
            return ErrorResult<T>(userMessage);
        }

        /// <summary>
        /// Handle exception and return error response with custom message
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="ex">Exception</param>
        /// <param name="operation">Operation name for logging</param>
        /// <param name="userMessage">User-friendly error message</param>
        /// <param name="logContext">Additional context for logging</param>
        /// <returns>Error response</returns>
        protected ServiceResponse<T> HandleException<T>(Exception ex, string operation, string userMessage, object? logContext = null)
        {
            if (logContext != null)
            {
                _logger.LogError(ex, "Error in {Operation} with context {@Context}", operation, logContext);
            }
            else
            {
                _logger.LogError(ex, "Error in {Operation}", operation);
            }
            return ErrorResult<T>(userMessage);
        }

        /// <summary>
        /// Validate required parameter
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>True if valid, false otherwise</returns>
        protected bool ValidateRequired<T>(T? value, string parameterName)
        {
            if (value == null)
            {
                _logger.LogWarning("Required parameter {ParameterName} is null", parameterName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate string parameter
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="minLength">Minimum length</param>
        /// <returns>True if valid, false otherwise</returns>
        protected bool ValidateString(string? value, string parameterName, int minLength = 1)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < minLength)
            {
                _logger.LogWarning("Invalid parameter {ParameterName}: {Value}", parameterName, value);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate ID parameter
        /// </summary>
        /// <param name="id">ID to validate</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>True if valid, false otherwise</returns>
        protected bool ValidateId(int id, string parameterName)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid parameter {ParameterName}: {Value}", parameterName, id);
                return false;
            }
            return true;
        }
    }
}
