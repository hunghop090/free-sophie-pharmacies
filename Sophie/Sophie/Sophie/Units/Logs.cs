using System;

namespace Sophie.Units
{
    public static class Logs
    {
        public static void debug(string value)
        {
            Console.WriteLine("==>" + value);
        }

        public static void warning(string value)
        {
            Console.WriteLine("==>" + value);
        }

        public static void information(string value)
        {
            Console.WriteLine("==>" + value);
        }

        public static void error(string value)
        {
            Console.WriteLine("==>" + value);
        }
    }
}
