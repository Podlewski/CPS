namespace Logic
{
    static class ExtensionMethods
    {
        public static double GenerateSignal(this Generator generator, string signal, double time)
        {
            switch (signal.Substring(0, 1))
            {
                case "01":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "02":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "03":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "04":
                    return generator.HalfWaveRectifierSignal(time);
                case "05":
                    return generator.FullWaveRectifierSignal(time);
                case "06":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "07":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "08":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "09":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "10":
                    return generator.SinusoidalSignal(time); // do zmiany
                case "11":
                    return generator.SinusoidalSignal(time); // do zmiany
                default:
                    return 0;
            }
        }

    }
}
