using Horarium.Attributes;

namespace Horarium.Test.TypeName
{
    public class GenericJob<TFirst> { }

    [GenericJob]
    public class GenericJobWithAttribute<TFirst> { }
}