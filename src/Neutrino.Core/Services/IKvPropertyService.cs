using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface IKvPropertyService
    {
        IEnumerable<KvProperty> Get();

        KvProperty Get(string id);

        Task<ActionConfirmation> Create(KvProperty kvProperty);

        Task<ActionConfirmation> Update(KvProperty kvProperty);

        Task<ActionConfirmation> Delete(string id);
    }
}