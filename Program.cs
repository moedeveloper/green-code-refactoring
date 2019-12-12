using System;

namespace greenbyte
{
    class Program
    {
        static void Main(string[] args)
        {
            // Just for testing .. don't have more time to create unit testing ..
            VestasCurtailmentProvider vestasCurtailmentProvider = new VestasCurtailmentProvider();
            for (int i = 0; i < 10; i++)
            {
                vestasCurtailmentProvider.SetCustomLevel(TurbineCurtailment.Bats, 12.2);
                vestasCurtailmentProvider.SetCustomLevel(TurbineCurtailment.Default, 10.2);
                vestasCurtailmentProvider.SetCustomLevel(TurbineCurtailment.Noise, 11.2);
                vestasCurtailmentProvider.SetCustomLevel(TurbineCurtailment.Technical, 15.2);
            }
            vestasCurtailmentProvider.SetCustomLevel(TurbineCurtailment.Bats, 205);
            Console.WriteLine(vestasCurtailmentProvider.GetLevel(TurbineCurtailment.Bats, DateTime.Now));
            Console.WriteLine(vestasCurtailmentProvider.GetCurrentLevel(TurbineCurtailment.Bats));
        }
    }
}
