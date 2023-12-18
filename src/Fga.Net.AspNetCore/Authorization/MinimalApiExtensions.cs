#region License
/*
   Copyright 2021-2024 Hawxy (JT)

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
using Fga.Net.AspNetCore.Authorization.Attributes;
using Microsoft.AspNetCore.Builder;

namespace Fga.Net.AspNetCore.Authorization;

/// <summary>
/// Fga extensions for use with Minimal APIs
/// </summary>
public static class MinimalApiExtensions
{
    /// <summary>
    /// <inheritdoc cref="FgaHeaderObjectAttribute"/>
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder"/>.</param>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="headerKey">The header to get the value from. Will throw an exception if not present.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder" />.</returns>
    public static TBuilder WithFgaHeaderCheck<TBuilder>(this TBuilder builder, string relation, string type, string headerKey) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new FgaHeaderObjectAttribute(relation, type, headerKey));
    }

    /// <summary>
    /// <inheritdoc cref="FgaPropertyObjectAttribute"/>
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder"/>.</param>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="property">The JSON property to get the value from. Must be a string or number. Will throw an exception if not present.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder" />.</returns>
    public static TBuilder WithFgaPropertyCheck<TBuilder>(this TBuilder builder, string relation, string type, string property) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new FgaPropertyObjectAttribute(relation, type, property));
    }

    /// <summary>
    /// <inheritdoc cref="FgaQueryObjectAttribute"/>
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder"/>.</param>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="queryKey">The query key to get the value from. Will throw an exception if not present.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder" />.</returns>
    public static TBuilder WithFgaQueryCheck<TBuilder>(this TBuilder builder, string relation, string type, string queryKey) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new FgaQueryObjectAttribute(relation, type, queryKey));
    }

    /// <summary>
    /// <inheritdoc cref="FgaRouteObjectAttribute"/>
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder"/>.</param>
    /// <param name="relation">The relationship to check, such as writer or viewer</param>
    /// <param name="type">The relation between the user and object</param>
    /// <param name="routeKey">The route key to get the value from. Will throw an exception if not present.</param>
    /// <returns>The <see cref="T:Microsoft.AspNetCore.Builder.IEndpointConventionBuilder" />.</returns>
    public static TBuilder WithFgaRouteCheck<TBuilder>(this TBuilder builder, string relation, string type, string routeKey) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new FgaRouteObjectAttribute(relation, type, routeKey));
    }
}