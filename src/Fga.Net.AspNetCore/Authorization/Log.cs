﻿#region License
/*
   Copyright 2021-2025 Hawxy (JT)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using Fga.Net.AspNetCore.Exceptions;
using Microsoft.Extensions.Logging;

namespace Fga.Net.AspNetCore.Authorization;

internal static partial class Log
{
    [LoggerMessage(3001, LogLevel.Debug, "FGA Check failed for User: {user}, Relation: {relation}, Object: {object}")]
    public static partial void CheckFailure(this ILogger logger, string user, string relation, string @object);

    [LoggerMessage(3002, LogLevel.Debug, "FGA Attribute returned null value(s), User: {user}, Relation: {relation}, Object: {object}")]
    public static partial void NullValuesReturned(this ILogger logger, string? user, string? relation, string? @object);

    [LoggerMessage(3003, LogLevel.Information, "Unable to compute FGA Object.")]
    public static partial void MiddlewareException(this ILogger logger, FgaMiddlewareException ex);
    
    [LoggerMessage(3004, LogLevel.Warning, "Error occurred whilst attempting to perform middleware check for User: {user}, Relation: {relation}, Object: {object}")]
    public static partial void CheckException(this ILogger logger, string user, string relation, string @object, Exception ex);

    [LoggerMessage(3005, LogLevel.Debug, "User was not in a valid format of 'type:id' or '*'. Computed user as '{user}'")]
    public static partial void InvalidUser(this ILogger logger, string user);
    
    [LoggerMessage(3006, LogLevel.Debug, "FGA Check succeeded for path '{path}' due to presence of FgaBypass attribute")]
    public static partial void BypassFound(this ILogger logger, string path);
    
}