/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Avro.Util
{
    /// <summary>
    /// The 'timestamp-micros' logical type.
    /// </summary>
    public class TimestampMicrosecond : LogicalUnixEpochType<DateTime>
    {
        /// <summary>
        /// The logical type name for TimestampMicrosecond.
        /// </summary>
        public static readonly string LogicalTypeName = "timestamp-micros";

        /// <summary>
        /// Initializes a new TimestampMicrosecond logical type.
        /// </summary>
        public TimestampMicrosecond() : base(LogicalTypeName)
        { }

        /// <inheritdoc/>
        public override void ValidateSchema(LogicalSchema schema)
        {
            if (Schema.Type.Long != schema.BaseSchema.Tag)
                throw new AvroTypeException("'timestamp-micros' can only be used with an underlying long type");
        }

        /// <inheritdoc/>
        public override object ConvertToBaseValue(object logicalValue, LogicalSchema schema)
        {
            return ConvertToBaseValue<long>(logicalValue, schema);
        }

        /// <inheritdoc/>   
        public override T ConvertToBaseValue<T>(object logicalValue, LogicalSchema schema)
        {
            if (typeof(T) == typeof(long))
            {
                var date = ((DateTime)logicalValue).ToUniversalTime();
                return (T)(object)((date - UnixEpochDateTime).Ticks / TicksPerMicrosecond);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)logicalValue;
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)((DateTime)logicalValue).ToString("O");
            }
            else
            {
                throw new AvroTypeException($"Cannot convert to type {typeof(T)}");
            }
        }

        /// <inheritdoc/>
        public override object ConvertToLogicalValue(object baseValue, LogicalSchema schema)
        {
            if (baseValue is long)
            {
                return UnixEpochDateTime.AddTicks((long)baseValue * TicksPerMicrosecond);
            }
            else if (baseValue is DateTime)
            {
                return baseValue;
            }
            else if (baseValue is string)
            {
                return DateTime.Parse((string)baseValue);
            }
            else
            {
                throw new AvroTypeException("Invalid type conversion");
            }
        }
    }
}
