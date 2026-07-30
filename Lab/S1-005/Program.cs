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
                Console.WriteLine("Executing Campaign S1-005 under the founder's gate-7 authorisation.");
                Console.WriteLine("Reduction remains withheld.");
                Console.WriteLine();
                if (!Calibration.RunAll())
                {
                    Console.Error.WriteLine("REFUSED: calibration failed; N0 and N1 were not ticked.");
                    return 2;
                }

                Console.WriteLine();
                return Execution.RunAll("Runs");
            }

            Console.WriteLine("S1-005 laboratory — The Competence Boundary");
            Console.WriteLine("  --calibrate  test the instrument on foreign toys only");
            Console.WriteLine("  --execute    run N0 and N1 as sealed (authorised 2026-07-30)");
            return 0;
        }
    }
}
