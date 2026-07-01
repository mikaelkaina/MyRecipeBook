using System.Collections;

namespace WebApi.Tests.InLineData;

public class CultureInLineData : IEnumerable<Object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "en-US" };
        yield return new object[] { "pt-BR" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
