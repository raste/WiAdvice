// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Runtime.Serialization;

namespace BusinessLayer
{
    /// <summary>
    /// used for throwing user generated business exceptions
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException() : base() { }

        public BusinessException(string message) : base(message) { }

        protected BusinessException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public BusinessException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
