namespace Delugional
{
    internal class IdGenerator
    {
        public static IdGenerator Default { get; } = new IdGenerator();

        private uint currentId;

        public uint Next()
        {
            return currentId++;
        }
    }
}