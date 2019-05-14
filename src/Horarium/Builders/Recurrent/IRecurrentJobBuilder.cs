namespace Horarium.Builders.Recurrent
{
    public interface IRecurrentJobBuilder : IJobBuilder
    {
        /// <summary>
        /// Add special key(unique identity for recurrent job), default is class name
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        IRecurrentJobBuilder WithKey(string jobKey);
    }
}