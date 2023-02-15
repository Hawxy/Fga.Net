using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenFga.Sdk.Api;
using OpenFga.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fga.Net.AspNetCore.Transactions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FgaBatchAttribute : Attribute
{
    public bool Transaction { get; }

    public FgaBatchAttribute(bool transaction = false)
    {
        Transaction = transaction;
    }
}

public class FgaBatchMiddleware : IMiddleware
{
    private readonly string[] _writeMethods = { HttpMethod.Post.Method, HttpMethod.Delete.Method, HttpMethod.Patch.Method, HttpMethod.Put.Method };

    private readonly IFgaContext _context;

    public FgaBatchMiddleware(IFgaContext context)
    {
        _context = context;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_writeMethods.Contains(context.Request.Method))
        {
             await next(context);
             return;
        }

        var attribute = context.GetEndpoint()?.Metadata.GetMetadata<FgaBatchAttribute>();
        if (attribute is null)
        {
            await next(context);
            return;

        }

        await next(context);

        await _context.WriteBatchAsync();
    }
}


public interface IFgaOperation
{
    public Task WriteAsync(OpenFgaApi api);
}


public class TupleOperation : IFgaOperation
{

    private readonly List<TupleKey>? _writes;
    private readonly List<TupleKey>? _deletes;

    public TupleOperation(List<TupleKey>? writes = null, List<TupleKey>? deletes = null)
    {
        if (writes?.Count == 0 && deletes?.Count == 0)
        {
            throw new InvalidOperationException("At least one write or delete must be performed.");
        }

        _writes = writes;
        _deletes = deletes;
    }

    public async Task WriteAsync(OpenFgaApi api)
    {
        var write = _writes != null ? new TupleKeys(_writes) : null;
        var delete = _deletes != null ? new TupleKeys(_deletes) : null;
        await api.Write(new WriteRequest() { Writes = write, Deletes = delete });
    }
}

public interface IFgaContext
{
    void EnqueueTupleWrite(TupleOperation tuple);

    void EnqueueTupleDelete(TupleOperation tuple);

    Task WriteBatchAsync();


}

public class FgaBatchContext
{
    private readonly HashSet<IFgaOperation> _operations;

    public FgaBatchContext()
    {
        _operations = new HashSet<IFgaOperation>();
    }

    public void EnqueueTupleWrite(TupleOperation tuple)
    {
        _operations.Add(tuple);
    }

    public void EnqueueTupleDelete(TupleOperation tuple)
    {
        _operations.Add(tuple);
    }




}

public static class FgaTransactionMiddlewareExtensions
{
    public static IApplicationBuilder UseFgaTransactions(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FgaTransactionMiddleware>();
    }
}