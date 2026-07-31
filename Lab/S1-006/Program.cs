using System;

namespace Genesis.Lab.S1_006
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--calibrate")
            {
                return Calibration.RunAll() ? 0 : 1;
            }

            if (args.Length >= 1 && args[0] == "--execute")
            {
                Console.Error.WriteLine(
                    "REFUSED: Campaign S1-006 execution has not been authorised. P0 and P1 were not ticked.");
                return 2;
            }

            Console.WriteLine("S1-006 laboratory — The Selected Form");
            Console.WriteLine("  --calibrate  test the instrument on foreign toys only");
            Console.WriteLine("  --execute    refused until separate founder authorisation");
            return 0;
        }
    }
}
