using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class EffectiveFactor
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public EffectiveFactorTypes EffectiveFactorType { get; private set; }
        public byte[] TimeStamp { get; private set; }

        public EffectiveFactor()
        {

        }

        public EffectiveFactor(string name, EffectiveFactorTypes effectiveFactorType)
        {
            Name = name;
            EffectiveFactorType = effectiveFactorType;
        }
    }
}