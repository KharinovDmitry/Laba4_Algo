namespace TextSort
{
    internal class AnalyzeResult
    {
        public string Title { get; }

        public List<TimeMeasurement> Measurements { get; }

        public AnalyzeResult(string title)
        {
            Title = title;
            Measurements = new List<TimeMeasurement>();
        }

        public void AddMeasurement(int arrayLength, double time)
        {
            Measurements.Add(new TimeMeasurement(arrayLength, time));
        }
    }

    internal class TimeMeasurement
    {
        public int ArrayLength { get; }
        public double TimeInS { get; }

        public TimeMeasurement(int arrayLength, double timeInS)
        {
            ArrayLength = arrayLength;
            TimeInS = timeInS;
        }
    }
}