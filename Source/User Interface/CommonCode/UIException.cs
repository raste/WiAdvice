﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Runtime.Serialization;

namespace UserInterface.CommonCode
{
    /// <summary>
    /// used for throwing user generated UserInterface exceptions
    /// </summary>
    public class UIException : Exception
    {
        public UIException() : base() { }

        public UIException(string message) : base(message) { }

        protected UIException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public UIException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
