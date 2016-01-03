namespace Delugional
{
    public class IdGenerator
    {
        public static IdGenerator Default { get; } = new IdGenerator();

        private int currentId;

        public int Next()
        {
            if (currentId < 0)
                currentId = 0;
            return currentId++;
        }
    }
}