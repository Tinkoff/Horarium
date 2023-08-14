using Horarium.Attributes;

namespace Horarium.Test.TypeName
{
    public class GenericJobWithTwoArguments<TFirst, TSecond> { }

    [GenericJob]
    public class GenericJobWithTwoArgumentsWithAttribute<TFirst, TSecond> { }
}