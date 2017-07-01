using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Consensus;
using Neutrino.Consensus.Entities;
using Neutrino.Core.Repositories;
using Neutrino.Core.Services.Validators;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class KvPropertyService : IKvPropertyService
    {
        private readonly IRepository<KvProperty> _kvPropertyRepository;
        private readonly IKvPropertyValidator _kvPropertyValidator;
        private readonly ILogReplication _logReplication;

        public KvPropertyService(
            IRepository<KvProperty> kvPropertyRepository,
            IKvPropertyValidator kvPropertyValidator,
            ILogReplication logReplication)
        {
            _kvPropertyRepository = kvPropertyRepository;
            _kvPropertyValidator = kvPropertyValidator;
            _logReplication = logReplication;
        }

        public IEnumerable<KvProperty> Get()
        {
            var kvProperties = _kvPropertyRepository.Get();
            return kvProperties;
        }

        public KvProperty Get(string id)
        {
            var kvProperty = _kvPropertyRepository.Get(id);
            return kvProperty;
        }

        public async Task<ActionConfirmation> Create(KvProperty kvProperty)
        {
            var validatorConfirmation = _kvPropertyValidator.Validate(kvProperty, ActionType.Create);
            if(!validatorConfirmation.WasSuccessful)
            {
                return validatorConfirmation;
            }

            var consensusResult = await _logReplication.DistributeEntry(kvProperty, MethodType.Create);
            if(consensusResult.WasSuccessful)
            {
                _kvPropertyRepository.Create(kvProperty);
            }
            else
            {
                return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public async Task<ActionConfirmation> Update(KvProperty kvProperty)
        {
            var validatorConfirmation = _kvPropertyValidator.Validate(kvProperty, ActionType.Update);
            if(!validatorConfirmation.WasSuccessful)
            {
                return validatorConfirmation;
            }

            var consensusResult = await _logReplication.DistributeEntry(kvProperty, MethodType.Update);
            if(consensusResult.WasSuccessful)
            {
                _kvPropertyRepository.Update(kvProperty);
            }
            else
            {
                return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public async Task<ActionConfirmation> Delete(string id)
        {
            var kvProperty = _kvPropertyRepository.Get(id);
            var consensusResult = await _logReplication.DistributeEntry(kvProperty, MethodType.Delete);
            if(consensusResult.WasSuccessful)
            {
                _kvPropertyRepository.Delete(id);
            }
            else
            {
                return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
            }

            return ActionConfirmation.CreateSuccessful();
        }
    }
}