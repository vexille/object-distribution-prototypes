using System.Diagnostics;

namespace DistributionPrototype.Distribution.Decorator {
    public class TimedSamplerDecorator : ISamplerDecorator {
        public delegate void PrepareDelegate(double elapsed);
        public delegate void GenerateDelegate(double elapsed);

        private ISamplerDecorator _decorator;
        private PrepareDelegate _prepare;
        private GenerateDelegate _generate;

        public TimedSamplerDecorator(ISamplerDecorator decorator, PrepareDelegate prepare, GenerateDelegate generate) {
            _decorator = decorator;
            _prepare = prepare;
            _generate = generate;
        }

        public void Prepare(object data) {
            var watch = new Stopwatch();
            watch.Start();

            _decorator.Prepare(data);

            watch.Stop();
            if (_prepare != null) _prepare(watch.Elapsed.TotalSeconds);
        }

        public int Generate(SampleGeneratedDelegate generationDelegate) {
            var watch = new Stopwatch();
            watch.Start();

            int result = _decorator.Generate(generationDelegate);

            watch.Stop();
            if (_generate != null) _generate(watch.Elapsed.TotalSeconds);

            return result;
        }
    }
}
