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
                Console.WriteLine("Executing Campaign S1-006 under the founder's gate-7 authorisation.");
                Console.WriteLine("Reduction remains withheld.");
                Console.WriteLine();
                if (!Calibration.RunAll())
                {
                    Console.Error.WriteLine("REFUSED: calibration failed; P0 and P1 were not ticked.");
                    return 2;
                }
                Console.WriteLine();
                return Execution.RunAll("Runs");
            }

            Console.WriteLine("S1-006 laboratory — The Selected Form");
            Console.WriteLine("  --calibrate  test the instrument on foreign toys only");
            Console.WriteLine("  --execute    run P0 and P1 as authorised on 2026-07-31");
            return 0;
        }
    }
}
