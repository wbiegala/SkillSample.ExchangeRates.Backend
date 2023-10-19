using SkillSample.ExchangeRates.Backend.Domain.Base;

namespace SkillSample.ExchangeRates.Backend.Domain.Model
{
    public class Currency : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Code { get; private set; }

        /// <summary>
        /// For EF
        /// </summary>
        protected Currency() { }

        public Currency(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
