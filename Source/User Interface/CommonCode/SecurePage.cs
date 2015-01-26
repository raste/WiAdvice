﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

namespace UserInterface.CommonCode
{
    #region "SecurePage Class"
    class SecurePage
    {
        string _path = "";
        string _pathType = "";
        public string Path
        {
            get { return this._path; }
            set { this._path = value; }
        }
        public string PathType
        {
            get { return this._pathType; }
            set { this._pathType = value; }
        }
    }
    #endregion
}
