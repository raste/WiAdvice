// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;

using DataAccess;

namespace BusinessLayer
{
    /// <summary>
    /// Provides means for extracting typed data from a <see cref="DataAccess.ScalarValue"/> instance.
    /// </summary>
    public class BusinessScalarValue
    {
        private ScalarValue scalarValue;

        public BusinessScalarValue(ScalarValue scalarVal)
        {
            if (scalarVal == null)
            {
                throw new ArgumentNullException("scalarVal");
            }
            scalarValue = scalarVal;
        }

        private void CheckValidity(Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (scalarValue == null)
            {
                throw new InvalidOperationException("scalarVal" + " is null.");
            }

            bool valid = false;

            if (destinationType == typeof(long))
            {
                if ((scalarValue.DataType != null) && (string.Equals("bigint", scalarValue.DataType.ToLower()) == true))
                {
                    valid = true;
                }
            }
            else if (destinationType == typeof(int))
            {
                if ((scalarValue.DataType != null) && (string.Equals("int", scalarValue.DataType.ToLower()) == true))
                {
                    valid = true;
                }
            }
            else if (destinationType == typeof(string))
            {
                if (scalarValue.DataType != null)
                {
                    string dbDataType = scalarValue.DataType.ToLower();
                    switch (dbDataType)
                    {
                        case "nvarchar":
                        case "varchar":
                        case "nchar":
                        case "char":
                            valid = true;
                            break;
                    }
                }
            }

            if (valid == false)
            {
                string dbTypeStr = scalarValue.DataType;
                if (dbTypeStr == null)
                {
                    dbTypeStr = "NULL";
                }
                else if (dbTypeStr == string.Empty)
                {
                    dbTypeStr = "[unspecified]";
                }
                string errorMessage =
                    string.Format("\"{0}\" cannot be extracted from \"{1}\" database type.",
                    destinationType.FullName, dbTypeStr);
                throw new BusinessException(errorMessage);
            }
        }

        public long LongValue
        {
            get
            {
                Type destinationType = typeof(long);

                CheckValidity(destinationType);

                long result;

                if (long.TryParse(scalarValue.Value, out result) == true)
                {
                    return result;
                }
                else
                {
                    string errMsg =
                        string.Format("\"{0}\" is not a valid {1}.",
                        scalarValue.Value ?? string.Empty, destinationType.FullName);
                    throw new BusinessException(errMsg);
                }
            }
        }

        public int IntValue
        {
            get
            {
                Type destinationType = typeof(int);

                CheckValidity(destinationType);

                int result;

                if (int.TryParse(scalarValue.Value, out result) == true)
                {
                    return result;
                }
                else
                {
                    string errMsg =
                        string.Format("\"{0}\" is not a valid {1}.",
                        scalarValue.Value ?? string.Empty, destinationType.FullName);
                    throw new BusinessException(errMsg);
                }
            }
        }
    }
}
