using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;

namespace Neutrino.Consensus.States
{
    public class Follower : State
    {
        private int _lastRertievedHeartbeat = 0;
        private CancellationTokenSource _checkHeartbeatTokenSource;
        private readonly IConsensusContext _consensusContext;
        private readonly ILogger<IConsensusContext> _logger;
        private readonly object _ensureLogLock = new object();

        public Follower(IConsensusContext consensusContext, ILogger<IConsensusContext> logger)
        {
            _consensusContext = consensusContext;
            _logger = logger;
        }

        public override void Proceed()
        {
            StartCheckingHeartbeats();
        }

        public override IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var appendEntriesEvent = triggeredEvent as AppendEntriesEvent;
            if(appendEntriesEvent != null)
            {
                bool leaderNodeWasChanged = false;

                lock(_ensureLogLock)
                {
                    if(_consensusContext.NodeVote.LeaderNode == null || 
                        _consensusContext.NodeVote.LeaderNode.Id != appendEntriesEvent.LeaderNode.Id)
                    {
                        leaderNodeWasChanged = true;
                    }

                    _consensusContext.NodeVote.LeaderNode = appendEntriesEvent.LeaderNode;
                }

                _consensusContext.CurrentTerm = appendEntriesEvent.Term;
                _consensusContext.NodeVote.VoteTerm = 0;
                _lastRertievedHeartbeat = 0;

                if(leaderNodeWasChanged)
                {
                    _consensusContext.EnsureLogConsistency();
                }

                return new AppendEntriesResponse(_consensusContext.CurrentTerm, true);
            }

            var requestVoteEvent = triggeredEvent as RequestVoteEvent;
            if(requestVoteEvent != null)
            {
                _logger.LogInformation($"Retreived 'LeaderRequestEvent' event (currentTerm: {requestVoteEvent.Term}, node: {requestVoteEvent.Node.Id}).");

                bool voteValue = OtherNodeCanBeLeader(requestVoteEvent);
                if(voteValue)
                {
                    _consensusContext.CurrentTerm = requestVoteEvent.Term;
                    _consensusContext.NodeVote.LeaderNode = requestVoteEvent.Node;
                    _consensusContext.NodeVote.VoteTerm = requestVoteEvent.Term;

                    _lastRertievedHeartbeat = 0;

                    _logger.LogInformation($"Voting for node ({requestVoteEvent.Node.Id}): GRANTED.");
                }
                else
                {
                    _logger.LogInformation($"Voting for node ({requestVoteEvent.Node.Id}): NOT GRANTED.");
                }

                return new RequestVoteResponse(voteValue, _consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            }

            return new EmptyResponse();
        }

        public override void DisposeCore()
        {
            _checkHeartbeatTokenSource.Cancel();
        }

        private void StartCheckingHeartbeats()
        {
            _checkHeartbeatTokenSource = new CancellationTokenSource();
            Task.Run(() => CheckHeartbeat(_checkHeartbeatTokenSource.Token), _checkHeartbeatTokenSource.Token);
        }

        private void StopCheckingHeartbeat()
        {
            _checkHeartbeatTokenSource.Cancel();
        }

        private void CheckHeartbeat(CancellationToken token)
        {
            while(!token.IsCancellationRequested) 
            {
                if(_lastRertievedHeartbeat >= _consensusContext.ElectionTimeout)
                {
                    StopCheckingHeartbeat();
                    _consensusContext.State = new Candidate(_consensusContext, _logger);
                }

                Thread.Sleep(50);
                _lastRertievedHeartbeat += 50;
            }
        }

        private bool OtherNodeCanBeLeader(RequestVoteEvent requestVoteEvent)
        {
            return requestVoteEvent.Term >= _consensusContext.CurrentTerm 
                && requestVoteEvent.Term > _consensusContext.NodeVote.VoteTerm;
        }
    }
}