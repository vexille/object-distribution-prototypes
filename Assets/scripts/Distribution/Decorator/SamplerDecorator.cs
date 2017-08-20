
namespace DistributionPrototype.Distribution.Decorator {
    public abstract class SamplerDecorator : ISamplerDecorator {
        protected float _width;
        protected float _height;

        public SamplerDecorator(float width, float height) {
            _width = width;
            _height = height;
        }

        public virtual void Prepare(object data) {

        }

        public abstract int Generate(SampleGeneratedDelegate generationDelegate);
    }
}
