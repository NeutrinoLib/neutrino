using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Consensus;
using Neutrino.Consensus.Entities;
using Neutrino.Core.Repositories;
using Neutrino.Core.Services.Validators;
using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

namespace Neutrino.Core.Services
{
    public class KvPropertyService : IKvPropertyService
    {
        private readonly IRepository<KvProperty> _kvPropertyRepository;
        private readonly IKvPropertyValidator _kvPropertyValidator;
        private readonly ILogReplication _logReplication;
        private readonly IConsensusContext _consensusContext;

        public KvPropertyService(
            IRepository<KvProperty> kvPropertyRepository,
            IKvPropertyValidator kvPropertyValidator,
            ILogReplication logReplication,
            IConsensusContext consensusContext)
        {
            _kvPropertyRepository = kvPropertyRepository;
            _kvPropertyValidator = kvPropertyValidator;
            _logReplication = logReplication;
            _consensusContext = consensusContext;
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

            if(_consensusContext.IsLeader())
            {
                var consensusResult = await _logReplication.DistributeEntry(kvProperty, MethodType.Create);
                if(consensusResult.WasSuccessful)
                {
                    _kvPropertyRepository.Create(kvProperty);
                }
                else
                {
                    return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
                }
            }
            else
            {
                _kvPropertyRepository.Create(kvProperty);
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

            if(_consensusContext.IsLeader())
            {
                var consensusResult = await _logReplication.DistributeEntry(kvProperty, MethodType.Update);
                if(consensusResult.WasSuccessful)
                {
                    _kvPropertyRepository.Update(kvProperty);
                }
                else
                {
                    return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
                }
            }
            else
            {
                _kvPropertyRepository.Update(kvProperty);   
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public async Task<ActionConfirmation> Delete(string id)
        {
            var kvProperty = _kvPropertyRepository.Get(id);

            if(_consensusContext.IsLeader())
            {
                var consensusResult = await _logReplication.DistributeEntry(kvProperty, MethodType.Delete);
                if(consensusResult.WasSuccessful)
                {
                    _kvPropertyRepository.Remove(id);
                }
                else
                {
                    return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
                }
            }
            else
            {
                _kvPropertyRepository.Remove(id);
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public void Clear()
        {
            _kvPropertyRepository.Clear();
        }
    }
}