using System.Collections;
using System.Collections.Generic;

namespace Fga.Net.Tests;

// https://andrewlock.net/creating-strongly-typed-xunit-theory-test-data-with-theorydata/
public abstract class TheoryData : IEnumerable<object[]>
{
    readonly List<object[]> data = new List<object[]>();

    protected void AddRow(params object[] values)
    {
        data.Add(values);
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}