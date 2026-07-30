using System;

namespace Genesis.Lab.S1_005
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
                Console.Error.WriteLine("REFUSED: S1-005 execution is not authorised.");
                Console.Error.WriteLine("The sealed N0 and N1 parcels have not been ticked.");
                return 2;
            }

            Console.WriteLine("S1-005 laboratory — The Competence Boundary");
            Console.WriteLine("  --calibrate  test the instrument on foreign toys only");
            Console.WriteLine("  --execute    refused until a separate founder authorisation");
            return 0;
        }
    }
}

